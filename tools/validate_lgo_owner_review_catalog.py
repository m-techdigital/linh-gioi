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
        "build/source-pose-catalog-audit-v2/kiem-owner-review-closeup.jpg",
    ),
    "phap": (
        "build/phap-canonical-v2-runtime-v1/pc/registered-manifest.json",
        "build/source-pose-catalog-audit-v2/phap-owner-review-closeup.jpg",
    ),
    "co": (
        "build/co-semantic-v3-runtime-v3/pc/registered-manifest.json",
        "build/source-pose-catalog-audit-v2/co-owner-review-closeup.jpg",
    ),
    "linh": (
        "build/linh-semantic-v3-runtime-v2/pc/registered-manifest.json",
        "build/source-pose-catalog-audit-v2/linh-owner-review-closeup.jpg",
    ),
}
MAX_JUMP_TO_IDLE_SCREEN_HEIGHT_RATIO = 1.08
ROOT_SCALE_EPSILON = 0.001


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
    scale_error = validate_pose_motion_scale_metrics(data, class_id)
    if scale_error:
        return scale_error
    return None


def _metric_float(metric: dict, key: str) -> float | None:
    value = metric.get(key)
    if isinstance(value, (int, float)):
        return float(value)
    return None


def _find_pose_metric(metrics: list[dict], gender: str, token: str) -> dict | None:
    for metric in metrics:
        filename = str(metric.get("file", ""))
        if gender in filename and token in filename:
            return metric
    return None


def validate_pose_motion_scale_metrics(data: dict, class_id: str) -> str | None:
    metrics = data.get("actorFrameMetrics")
    if not isinstance(metrics, list) or not metrics:
        return f"missing actor frame scale metrics for {class_id}"
    for metric in metrics:
        filename = str(metric.get("file", "unknown"))
        scale_x = _metric_float(metric, "rootScaleX")
        scale_y = _metric_float(metric, "rootScaleY")
        if scale_x is None or scale_y is None:
            return f"missing root scale metric for {class_id}: {filename}"
        if abs(scale_x - 1.0) > ROOT_SCALE_EPSILON or abs(scale_y - 1.0) > ROOT_SCALE_EPSILON:
            return f"unexpected runtime root scale for {class_id}: {filename} scale=({scale_x},{scale_y})"
    for gender in ("male", "female"):
        idle = _find_pose_metric(metrics, gender, "idle")
        jump = _find_pose_metric(metrics, gender, "-jump.png")
        if idle is None or jump is None:
            return f"missing idle/jump scale metric for {class_id}: {gender}"
        idle_height = _metric_float(idle, "screenHeightRatio")
        jump_height = _metric_float(jump, "screenHeightRatio")
        if idle_height is None or jump_height is None or idle_height <= 0:
            return f"invalid idle/jump screen height metric for {class_id}: {gender}"
        ratio = jump_height / idle_height
        if ratio > MAX_JUMP_TO_IDLE_SCREEN_HEIGHT_RATIO:
            return f"jump scale exceeds idle height for {class_id}: {gender} ratio={ratio:.3f} max={MAX_JUMP_TO_IDLE_SCREEN_HEIGHT_RATIO:.3f}"
    return None


def validate_exposed_pack_matrix(class_id: str) -> str | None:
    suffixes = launcher.PACK_SUFFIXES.get(class_id)
    if suffixes is None:
        return f"missing pack suffix matrix for {class_id}"
    if len(suffixes) != 4:
        return f"pack suffix matrix must have four entries for {class_id}: {suffixes}"
    if class_id != "vo" and any(suffix is None for suffix in suffixes):
        return f"owner-facing class must expose male/female Lv1/Lv10 packs for {class_id}: {suffixes}"
    for suffix in suffixes:
        if suffix is None:
            continue
        pack = ROOT / "build" / (class_id + suffix)
        if not (pack / "atlas-review.json").is_file():
            return f"missing owner-facing source-pose pack for {class_id}: build/{class_id + suffix}"
    return None


def main() -> int:
    if tuple(launcher.CLASSES) != EXPECTED_CLASSES:
        return fail("interactive catalog must contain only visually audited source-pose classes in the approved order")
    for class_id in EXPECTED_CLASSES:
        error = validate_exposed_pack_matrix(class_id)
        if error:
            return fail(error)
    source = (ROOT / "tools/launch_lgo_source_pose_review.py").read_text(encoding="utf-8")
    required = "non-base source-pose art has Player close-up evidence"
    if required not in source:
        return fail("launcher is missing the non-base class-art gate comment")
    closeups = [PLAYER_EVIDENCE[class_id][1] for class_id in EXPECTED_CLASSES[1:]]
    if len(set(closeups)) != len(closeups):
        return fail("each owner-facing class must have an independent close-up visual sheet")
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
