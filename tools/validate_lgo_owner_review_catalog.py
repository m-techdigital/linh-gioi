#!/usr/bin/env python3.12
"""Guard the owner-review source-pose catalog from exposing unapproved class art."""
from __future__ import annotations

import hashlib
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

import launch_lgo_source_pose_review as launcher  # noqa: E402
import write_lgo_owner_review_closeups as closeup_writer  # noqa: E402

EXPECTED_CLASSES = ("vo", "kiem", "co", "linh")
PLAYER_EVIDENCE = {
    "kiem": (
        "build/kiem-semantic-v3-runtime-v1/pc/registered-manifest.json",
        "build/source-pose-catalog-audit-v2/kiem-owner-review-closeup.jpg",
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
HELD_OUT_CLASSES = {
    "phap": {
        "reason": "semantic-v3 rejected visually; canonical-v2 still has poseScaleCorrections.jump_tuck=2/3; complete-garment authoring is OWNER_REJECTED_VISUAL",
        "candidateManifest": "build/phap-canonical-v2-runtime-v1/pc/registered-manifest.json",
        "candidateCloseup": "build/source-pose-catalog-audit-v2/phap-owner-review-closeup.jpg",
        "blockedCompleteGarmentManifest": "build/phap-complete-garment-lv10-runtime-v1/pc/registered-manifest.json",
    },
}
MAX_JUMP_TO_IDLE_SCREEN_HEIGHT_RATIO = 1.08
ROOT_SCALE_EPSILON = 0.001
SOURCE_POSES = ("idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck")
SOURCE_SLOTS = (
    "main_weapon", "inner_top", "lower_body", "outer_top", "waist_belt",
    "footwear", "arm_guard", "shoulder_chest_guard", "head_hair", "class_accessory",
)
SOURCE_VISUAL_ACCEPTED_STATUS = "VISUAL_ACCEPTED_FOR_PACKING"


def fail(message: str) -> int:
    print("LGO_OWNER_REVIEW_CATALOG_FAIL " + message, file=sys.stderr)
    return 1


def active_player_evidence() -> dict[str, tuple[str, str]]:
    return {class_id: PLAYER_EVIDENCE[class_id] for class_id in EXPECTED_CLASSES if class_id != "vo"}


def validate_player_evidence(class_id: str) -> str | None:
    manifest_rel, closeup_rel = active_player_evidence()[class_id]
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
    provenance_error = validate_closeup_provenance(class_id, closeup)
    if provenance_error:
        return provenance_error
    return None


def validate_closeup_provenance(class_id: str, closeup: Path) -> str | None:
    provenance_path = closeup.with_suffix(".json")
    if not provenance_path.is_file():
        return f"missing close-up provenance for {class_id}: {provenance_path.relative_to(ROOT) if provenance_path.is_absolute() and provenance_path.is_relative_to(ROOT) else provenance_path}"
    try:
        provenance = json.loads(provenance_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        return f"invalid close-up provenance json for {class_id}: {exc}"
    if provenance.get("classId") != class_id:
        return f"close-up provenance class mismatch for {class_id}: {provenance.get('classId')}"
    expected_runtime = closeup_writer.CLASS_RUNTIME_DIRS.get(class_id)
    if provenance.get("runtimeDir") != expected_runtime:
        return f"close-up provenance runtime mismatch for {class_id}: {provenance.get('runtimeDir')} != {expected_runtime}"
    expected_frames = [{"label": label, "file": filename} for label, filename in closeup_writer.FRAME_MATRIX]
    if provenance.get("frames") != expected_frames:
        return f"close-up provenance frame matrix mismatch for {class_id}"
    digest = hashlib.sha256(closeup.read_bytes()).hexdigest()
    if provenance.get("sha256") != digest:
        return f"close-up provenance sha mismatch for {class_id}"
    return None


def _source_status(candidate: Path) -> str:
    for filename in ("manifest.json", "authoring-selection.json", "audit.json"):
        path = candidate / filename
        if not path.is_file():
            continue
        try:
            data = json.loads(path.read_text(encoding="utf-8"))
        except json.JSONDecodeError:
            continue
        status = str(data.get("status", "")).upper()
        if status:
            return status
    return ""


def validate_phap_source_candidate(candidate: Path) -> str | None:
    if (candidate / "DO-NOT-PACK.md").is_file():
        return f"Pháp source candidate has DO-NOT-PACK marker: {candidate}"
    status = _source_status(candidate)
    if any(marker in status for marker in ("REJECTED", "WITHDRAWN", "FIX_REQUIRED")):
        return f"Pháp source candidate is rejected: {candidate} status={status}"
    missing = []
    for slot in SOURCE_SLOTS:
        for pose in SOURCE_POSES:
            if not (candidate / slot / f"{pose}.png").is_file():
                missing.append(f"{slot}/{pose}.png")
    if missing:
        return f"Pháp source candidate missing 10-slot pose files: {candidate} missing={missing[:4]}"
    required_boards = ["six-pose-full-compose.jpg"] + [f"{pose}-ten-slot-off-review.jpg" for pose in SOURCE_POSES]
    missing_boards = [name for name in required_boards if not (candidate / name).is_file()]
    if missing_boards:
        return f"Pháp source candidate missing off-slot review boards: {candidate} missing={missing_boards}"
    provenance_error = validate_off_slot_board_provenance(candidate)
    if provenance_error:
        return provenance_error
    return None


def validate_off_slot_board_provenance(candidate: Path) -> str | None:
    provenance_path = candidate / "off-slot-board-provenance.json"
    if not provenance_path.is_file():
        return f"Pháp source candidate missing off-slot board provenance: {candidate}"
    try:
        provenance = json.loads(provenance_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        return f"Pháp source candidate invalid off-slot board provenance: {candidate} error={exc}"
    if provenance.get("poses") != list(SOURCE_POSES):
        return f"Pháp source candidate off-slot board provenance pose mismatch: {candidate}"
    if provenance.get("slots") != list(SOURCE_SLOTS):
        return f"Pháp source candidate off-slot board provenance slot mismatch: {candidate}"
    visual_status = str(provenance.get("visualReviewStatus", provenance.get("status", ""))).upper()
    if visual_status != SOURCE_VISUAL_ACCEPTED_STATUS:
        return f"Pháp source candidate requires visual accepted off-slot review before promotion: {candidate} status={visual_status or 'MISSING'}"
    boards = provenance.get("boards")
    if not isinstance(boards, list):
        return f"Pháp source candidate off-slot board provenance has no board list: {candidate}"
    expected = {f"{pose}-ten-slot-off-review.jpg" for pose in SOURCE_POSES}
    seen = set()
    for board in boards:
        if not isinstance(board, dict):
            return f"Pháp source candidate off-slot board provenance has invalid board entry: {candidate}"
        filename = board.get("file")
        if filename not in expected:
            return f"Pháp source candidate off-slot board provenance unexpected board: {candidate} file={filename}"
        seen.add(filename)
        board_path = candidate / filename
        digest = hashlib.sha256(board_path.read_bytes()).hexdigest()
        if board.get("sha256") != digest:
            return f"Pháp source candidate off-slot board provenance sha mismatch: {candidate} file={filename}"
    missing = sorted(expected - seen)
    if missing:
        return f"Pháp source candidate off-slot board provenance missing boards: {candidate} missing={missing}"
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
    active_evidence = active_player_evidence()
    closeups = [active_evidence[class_id][1] for class_id in EXPECTED_CLASSES[1:]]
    if len(set(closeups)) != len(closeups):
        return fail("each owner-facing class must have an independent close-up visual sheet")
    for class_id in HELD_OUT_CLASSES:
        if class_id in launcher.CLASSES or class_id in EXPECTED_CLASSES or class_id in active_evidence:
            return fail("held-out class must not be exposed as owner-review evidence: " + class_id)
    for class_id in EXPECTED_CLASSES[1:]:
        if class_id not in launcher.PACK_SUFFIXES:
            return fail("audit pack suffix missing for " + class_id)
        error = validate_player_evidence(class_id)
        if error:
            return fail(error)
    print("LGO_OWNER_REVIEW_CATALOG_PASS classes=" + ",".join(EXPECTED_CLASSES) + " review_only=true player_evidence=" + str(len(EXPECTED_CLASSES) - 1))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
