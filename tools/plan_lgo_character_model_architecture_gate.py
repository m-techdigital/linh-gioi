#!/usr/bin/env python3.12
"""Plan the finite LGO character-model architecture benchmark from evidence."""
from __future__ import annotations

import argparse
import json
from pathlib import Path
from typing import Any


GATE_ID = "LGO_CHARACTER_MODEL_ARCHITECTURE_REVIEW_01"
CANDIDATE_ORDER = ("skeletal_2d", "modular_3d")
REQUIRED_SLOTS = {"upper", "lower", "footwear", "waist", "rigid_hand_item"}
REQUIRED_ACTIONS = {"idle", "run", "jump", "attack", "return_to_idle"}
REQUIRED_TRANSITIONS = {
    "start_stop_direction_change",
    "run_jump_fall_land_run",
    "run_attack_run",
}
BASELINE_METRICS = {
    "manualSourceOperations",
    "unseenItemMinutes",
    "sourceCorrectionRounds",
}
COST_DIMENSIONS = (
    "sourceOperations",
    "unseenItemMinutes",
    "sourceCorrectionRounds",
    "specialPoseAttachments",
    "sourceFilesChanged",
    "codeFilesChanged",
    "generatedArtifacts",
    "pcFrameTimeP95Ms",
    "pcMemoryMb",
    "pcDrawCalls",
)


def _hard_failure_ids(record: dict[str, Any]) -> tuple[list[str], bool]:
    failure_ids: list[str] = []
    repeated = False
    for failure in record.get("hardFailures", []):
        if isinstance(failure, str):
            failure_ids.append(failure)
            continue
        failure_ids.append(str(failure.get("id", "unnamed_hard_failure")))
        if failure.get("reproductionCount", 0) >= 2:
            repeated = True

    rounds = record.get("manualIntervention", {}).get("sourceCorrectionRounds")
    if isinstance(rounds, (int, float)) and rounds > 2:
        failure_ids.append("source_correction_round_limit_exceeded")
        repeated = True
    return failure_ids, repeated


def _cost_vector(record: dict[str, Any]) -> dict[str, float] | None:
    manual = record.get("manualIntervention", {})
    changes = record.get("changeSurface", {})
    pc = record.get("performance", {}).get("pc", {})
    vector = {
        "sourceOperations": manual.get("sourceOperations"),
        "unseenItemMinutes": manual.get("unseenItemMinutes"),
        "sourceCorrectionRounds": manual.get("sourceCorrectionRounds"),
        "specialPoseAttachments": manual.get("specialPoseAttachments"),
        "sourceFilesChanged": changes.get("sourceFilesChanged"),
        "codeFilesChanged": changes.get("codeFilesChanged"),
        "generatedArtifacts": changes.get("generatedArtifacts"),
        "pcFrameTimeP95Ms": pc.get("frameTimeP95Ms"),
        "pcMemoryMb": pc.get("memoryMb"),
        "pcDrawCalls": pc.get("drawCalls"),
    }
    if any(not isinstance(vector[key], (int, float)) for key in COST_DIMENSIONS):
        return None
    return vector


def evaluate_candidate(record: dict[str, Any]) -> dict[str, Any]:
    toolchain = record.get("toolchain", {})
    common = record.get("commonTask", {})
    continuity = record.get("motionContinuity", {})
    integrity = record.get("deformationIntegrity", {})
    manual = record.get("manualIntervention", {})
    automation = record.get("automation", {})
    visual = record.get("visualReview", {})
    performance = record.get("performance", {})
    pc = performance.get("pc", {})
    mobile = performance.get("mobile", {})
    hard_failures, repeated_failure = _hard_failure_ids(record)
    closed = record.get("probeStatus") == "CLOSED_HARD_FAILURE" or repeated_failure

    missing: list[str] = []
    if toolchain.get("status") != "READY":
        missing.append("ready_toolchain_with_version_and_provenance")
    elif not toolchain.get("version") or not toolchain.get("provenance"):
        missing.append("ready_toolchain_with_version_and_provenance")
    if record.get("probeStatus") != "COMPLETE":
        missing.append("completed_probe")
    if common.get("neutralSwappableBody") is not True:
        missing.append("neutral_swappable_body")
    if not REQUIRED_SLOTS.issubset(set(common.get("slotsCompleted", []))):
        missing.append("five_required_slots")
    if not REQUIRED_ACTIONS.issubset(set(common.get("actionsCompleted", []))):
        missing.append("five_required_actions")
    if common.get("unequipCompleted") is not True:
        missing.append("unequip")
    if common.get("mixedLoadoutCompleted") is not True:
        missing.append("mixed_loadout")
    if common.get("unseenItemAfterToolingLock") is not True:
        missing.append("unseen_item_after_tooling_lock")
    if common.get("oldCombinationsRegressionPassed") is not True:
        missing.append("old_combinations_regression")
    if not REQUIRED_TRANSITIONS.issubset(set(continuity.get("transitionsCompleted", []))):
        missing.append("required_runtime_transitions")
    if continuity.get("midKeyframeSamplesReviewed") is not True:
        missing.append("mid_keyframe_samples_reviewed")
    if continuity.get("equipmentSwapDuringMotion") is not True:
        missing.append("equipment_swap_during_motion")
    if continuity.get("equipmentSwapPreservedAnimationTime") is not True:
        missing.append("equipment_swap_preserves_animation_time")
    if continuity.get("rootScaleStable") is not True:
        missing.append("root_scale_stable")
    if continuity.get("singleTransformOwner") is not True:
        missing.append("single_transform_owner")
    if continuity.get("actualVelocityDrivesLocomotion") is not True:
        missing.append("actual_velocity_drives_locomotion")
    body_drift = integrity.get("bodyBoneLengthDriftMaxRatio")
    if not isinstance(body_drift, (int, float)) or body_drift > 0.001:
        missing.append("body_bone_length_stability")
    rigid_drift = integrity.get("rigidEdgeLengthDriftMaxRatio")
    if not isinstance(rigid_drift, (int, float)) or rigid_drift > 0.001:
        missing.append("rigid_shape_preservation")
    socket_drift = integrity.get("rigidSocketDriftMaxSourcePx")
    if not isinstance(socket_drift, (int, float)) or socket_drift > 1.0:
        missing.append("rigid_socket_registration")
    if integrity.get("softTriangleInversionCount") != 0:
        missing.append("no_soft_mesh_triangle_inversion")
    seam_gap = integrity.get("seamGapMaxSourcePx")
    if not isinstance(seam_gap, (int, float)) or seam_gap > 2.0:
        missing.append("seam_continuity_within_two_source_pixels")
    if integrity.get("authoredWeightsForDeformingParts") is not True:
        missing.append("authored_weights_for_deforming_parts")
    if integrity.get("sameTimestampBodyFullMixedReviewed") is not True:
        missing.append("same_timestamp_body_full_mixed_review")
    if manual.get("perPosePixelEdits") != 0:
        missing.append("zero_per_pose_pixel_edits")
    if automation.get("deterministicReplay") is not True:
        missing.append("deterministic_replay")
    if automation.get("provenanceRecorded") is not True:
        missing.append("output_provenance")
    if _cost_vector(record) is None:
        missing.append("complete_cost_vector")
    if pc.get("status") != "MEASURED":
        missing.append("pc_runtime_measurement")
    if mobile.get("status") not in {"MEASURED", "DEFERRED_DEVICE_REQUIRED"}:
        missing.append("honest_mobile_measurement_status")
    if visual.get("status") != "APPROVED" or not visual.get("artifacts"):
        missing.append("approved_visual_review")
    if hard_failures:
        missing.append("no_hard_failures")

    eligible = not missing and not closed
    return {
        "eligible": eligible,
        "closedByAntiLoop": closed,
        "hardFailures": hard_failures,
        "missingEvidence": missing,
        "costVector": _cost_vector(record),
        "mobileProductionGatePending": mobile.get("status") == "DEFERRED_DEVICE_REQUIRED",
    }


def _baseline_missing(baseline: dict[str, Any]) -> list[str]:
    missing: list[str] = []
    if baseline.get("status") != "MEASURED":
        missing.append("baseline_status_measured")
    if not baseline.get("evidence"):
        missing.append("baseline_evidence_path")
    metrics = baseline.get("metrics", {})
    for key in sorted(BASELINE_METRICS):
        if not isinstance(metrics.get(key), (int, float)):
            missing.append(f"baseline_metric_{key}")
    return missing


def _dominance_decision(evaluations: dict[str, dict[str, Any]]) -> dict[str, Any]:
    eligible = [name for name in CANDIDATE_ORDER if evaluations[name]["eligible"]]
    if len(eligible) == 1:
        return {
            "status": "ONLY_ELIGIBLE_CANDIDATE",
            "dominantCandidate": eligible[0],
            "reason": "the other candidate is closed by reproduced hard failure",
        }

    vector_2d = evaluations["skeletal_2d"]["costVector"]
    vector_3d = evaluations["modular_3d"]["costVector"]
    advantages_2d = [key for key in COST_DIMENSIONS if vector_2d[key] < vector_3d[key]]
    advantages_3d = [key for key in COST_DIMENSIONS if vector_3d[key] < vector_2d[key]]
    if advantages_2d and not advantages_3d:
        return {
            "status": "DOMINANT_CANDIDATE",
            "dominantCandidate": "skeletal_2d",
            "skeletal2dAdvantages": advantages_2d,
            "modular3dAdvantages": [],
        }
    if advantages_3d and not advantages_2d:
        return {
            "status": "DOMINANT_CANDIDATE",
            "dominantCandidate": "modular_3d",
            "skeletal2dAdvantages": [],
            "modular3dAdvantages": advantages_3d,
        }
    return {
        "status": "OWNER_TRADEOFF_REQUIRED",
        "dominantCandidate": None,
        "skeletal2dAdvantages": advantages_2d,
        "modular3dAdvantages": advantages_3d,
    }


def plan_architecture_gate(manifest: dict[str, Any]) -> dict[str, Any]:
    candidate_records = manifest.get("candidates", {})
    evaluations = {
        name: evaluate_candidate(candidate_records.get(name, {}))
        for name in CANDIDATE_ORDER
    }
    result: dict[str, Any] = {
        "gateId": GATE_ID,
        "candidates": evaluations,
        "runtimePromotionAllowed": False,
    }

    baseline_missing = _baseline_missing(manifest.get("baseline", {}))
    if baseline_missing:
        return {
            **result,
            "status": "BASELINE_EVIDENCE_REQUIRED",
            "nextAction": "measure_pose_sprite_baseline",
            "missingEvidence": baseline_missing,
        }

    for name in CANDIDATE_ORDER:
        record = candidate_records.get(name, {})
        evaluation = evaluations[name]
        if evaluation["closedByAntiLoop"]:
            continue
        if record.get("probeStatus") != "COMPLETE":
            label = "SKELETAL_2D" if name == "skeletal_2d" else "MODULAR_3D"
            return {
                **result,
                "status": f"RUN_{label}_PROBE",
                "nextAction": f"run_{name}_common_task_probe",
                "nextCandidate": name,
            }
        non_visual_missing = [
            item for item in evaluation["missingEvidence"] if item != "approved_visual_review"
        ]
        if non_visual_missing:
            return {
                **result,
                "status": "COMPLETE_COMMON_TASK_EVIDENCE",
                "nextAction": f"complete_{name}_evidence",
                "nextCandidate": name,
                "missingEvidence": non_visual_missing,
            }
        if "approved_visual_review" in evaluation["missingEvidence"]:
            return {
                **result,
                "status": "NEED_HUMAN_VISUAL_REVIEW",
                "nextAction": f"review_{name}_visual_artifacts",
                "nextCandidate": name,
            }

    eligible = [name for name in CANDIDATE_ORDER if evaluations[name]["eligible"]]
    closed = [name for name in CANDIDATE_ORDER if evaluations[name]["closedByAntiLoop"]]
    if len(eligible) == 2 or (len(eligible) == 1 and len(closed) == 1):
        return {
            **result,
            "status": "BENCHMARK_READY_FOR_DECISION",
            "nextAction": "review_architecture_decision",
            "decision": _dominance_decision(evaluations),
        }
    return {
        **result,
        "status": "NO_ELIGIBLE_ARCHITECTURE_CANDIDATE",
        "nextAction": "redesign_source_or_fit_family_before_new_probe",
    }


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--manifest", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    manifest = json.loads(args.manifest.read_text(encoding="utf-8"))
    result = plan_architecture_gate(manifest)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({"status": result["status"], "nextAction": result["nextAction"]}, ensure_ascii=False))
    return 0 if result["status"] == "BENCHMARK_READY_FOR_DECISION" else 2


if __name__ == "__main__":
    raise SystemExit(main())
