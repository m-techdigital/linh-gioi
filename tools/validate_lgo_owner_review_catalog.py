#!/usr/bin/env python3.12
"""Guard the owner-review source-pose catalog from exposing unapproved class art."""
from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

import launch_lgo_source_pose_review as launcher  # noqa: E402

EXPECTED_CLASSES = ("vo", "kiem", "phap", "co", "linh")
PLAYER_EVIDENCE = {
    "kiem": (
        "build/kiem-semantic-v3-runtime-v1/pc/registered-manifest.json",
        "build/source-pose-catalog-audit-v1/kiem-semantic-v3-actor-closeup.jpg",
    ),
    "phap": (
        "build/phap-semantic-v3-runtime-v3/pc/registered-manifest.json",
        "build/source-pose-catalog-audit-v1/phap-co-linh-semantic-closeup.jpg",
    ),
    "co": (
        "build/co-semantic-v3-runtime-v3/pc/registered-manifest.json",
        "build/source-pose-catalog-audit-v1/phap-co-linh-semantic-closeup.jpg",
    ),
    "linh": (
        "build/linh-semantic-v3-runtime-v2/pc/registered-manifest.json",
        "build/source-pose-catalog-audit-v1/phap-co-linh-semantic-closeup.jpg",
    ),
}


def fail(message: str) -> int:
    print("LGO_OWNER_REVIEW_CATALOG_FAIL " + message, file=sys.stderr)
    return 1


def validate_player_evidence(class_id: str) -> str | None:
    manifest_rel, closeup_rel = PLAYER_EVIDENCE[class_id]
    manifest = ROOT / manifest_rel
    closeup = ROOT / closeup_rel
    if not manifest.is_file():
        return f"missing Player manifest for {class_id}: {manifest_rel}"
    if not closeup.is_file() or closeup.stat().st_size <= 0:
        return f"missing close-up visual sheet for {class_id}: {closeup_rel}"
    try:
        data = json.loads(manifest.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        return f"invalid Player manifest json for {class_id}: {exc}"
    if data.get("status") != "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED":
        return f"unexpected Player status for {class_id}: {data.get('status')}"
    if data.get("frames") != 190:
        return f"unexpected Player frame count for {class_id}: {data.get('frames')}"
    if data.get("errors"):
        return f"Player manifest has errors for {class_id}: {data.get('errors')}"
    if data.get("poseReviewFullLevelsVerified") != [1, 10]:
        return f"missing Lv1/Lv10 verification for {class_id}: {data.get('poseReviewFullLevelsVerified')}"
    if data.get("poseReviewMixedVerified") is not True:
        return f"missing mixed-level verification for {class_id}"
    if data.get("maxBodyVariants") != 1:
        return f"expected one active body variant for {class_id}: {data.get('maxBodyVariants')}"
    return None


def main() -> int:
    if tuple(launcher.CLASSES) != EXPECTED_CLASSES:
        return fail("interactive catalog must contain only visually audited source-pose classes in the approved order")
    source = (ROOT / "tools/launch_lgo_source_pose_review.py").read_text(encoding="utf-8")
    required = "non-base source-pose art has Player close-up evidence"
    if required not in source:
        return fail("launcher is missing the non-base class-art gate comment")
    for class_id in EXPECTED_CLASSES[1:]:
        if class_id not in launcher.PACK_SUFFIXES:
            return fail("audit pack suffix missing for " + class_id)
        error = validate_player_evidence(class_id)
        if error:
            return fail(error)
    print("LGO_OWNER_REVIEW_CATALOG_PASS classes=vo,kiem,phap,co,linh review_only=true player_evidence=4")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
