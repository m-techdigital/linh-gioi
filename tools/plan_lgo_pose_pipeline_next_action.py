#!/usr/bin/env python3.12
"""Plan the next safe pose-clothing pipeline action from current gates."""
from __future__ import annotations

import argparse
import json
from pathlib import Path


def read_json(path: Path | str) -> dict:
    return json.loads(Path(path).read_text(encoding="utf-8"))


PILOT_EVIDENCE_KEYS = {
    "semanticOcclusionOrWeights": "semantic_occlusion_or_weights",
    "surfaceVariantReusesGeometryAndMapping": "surface_variant_reuses_geometry_and_mapping",
    "geometryVariantRegeneratesFromParameters": "geometry_variant_regenerates_from_parameters",
    "unseenItemAfterTemplateLock": "unseen_item_after_template_lock",
    "noPerPosePngEdits": "no_per_pose_png_edits",
    "noRuntimeOffsetCompensation": "no_runtime_offset_compensation",
}

REJECTED_FIT_STRATEGIES = {
    "direct_2d_pattern_panel_affine_fit": "direct_panel_fit_rejected",
    "skin_color_occlusion_clip": "skin_color_heuristic_rejected",
    "per_pose_bbox_normalize": "per_pose_bbox_normalization_rejected",
}

GUIDE_AUTHORITY_STATUSES = {
    "APPROVED_GUIDE_AUTHORITY",
    "SOURCE_GUIDE_AUTHORITY",
    "GUIDE_AUTHORITY_APPROVED",
}

def evaluate_compiler_pilot(pilot_path: Path | str) -> dict:
    pilot = read_json(pilot_path)
    evidence = pilot.get("evidence", {})
    pose_coverage = pilot.get("poseCoverage", {})
    rendered_poses = set(pose_coverage.get("rendered", []))
    required_poses = set(pose_coverage.get("required", []))
    manual = pilot.get("manualIntervention", {})
    fit_strategy = pilot.get("fitStrategy")

    missing = [
        label
        for key, label in PILOT_EVIDENCE_KEYS.items()
        if evidence.get(key) is not True
    ]
    missing_poses = sorted(required_poses - rendered_poses)
    if len(required_poses) < 6 or "jump_tuck" not in rendered_poses or missing_poses:
        missing.append("six_pose_coverage_including_jump")
    if manual.get("perVariantPixelEdits") != 0:
        missing.append("zero_per_variant_pixel_edits")
    if pilot.get("sourceSpaceProfile") != "lgo_character_canvas_1024x1536_v1":
        missing.append("source_space_profile_1024x1536_v1")

    rejected = []
    if fit_strategy in REJECTED_FIT_STRATEGIES:
        rejected.append(REJECTED_FIT_STRATEGIES[fit_strategy])

    if missing or rejected or pilot.get("status") != "GARMENT_FAMILY_COMPILER_PILOT_PASS":
        return {
            "status": "COMPILER_PILOT_EVIDENCE_REQUIRED",
            "assetAuthoringAllowed": False,
            "diagnosticBoardAllowed": True,
            "reuseProven": False,
            "runtimePromotionAllowed": False,
            "missingEvidence": missing,
            "rejectedEvidence": rejected,
            "requiredEvidenceBeforeAsset": [
                "semantic_body_part_occlusion_or_weight_mask",
                "surface_variant_reuse_without_geometry_or_mapping_change",
                "bounded_geometry_param_regeneration",
                "unseen_item_after_template_lock",
                "six_pose_coverage_including_jump",
                "zero_per_variant_pixel_edits",
            ],
        }

    return {
        "status": "GARMENT_FAMILY_COMPILER_CANDIDATE_ALLOWED",
        "assetAuthoringAllowed": True,
        "diagnosticBoardAllowed": True,
        "reuseProven": True,
        "runtimePromotionAllowed": False,
        "provenEvidence": list(PILOT_EVIDENCE_KEYS.values()) + [
            "six_pose_coverage_including_jump",
            "zero_per_variant_pixel_edits",
            "source_space_profile_1024x1536_v1",
        ],
        "reason": "Compiler pilot evidence proves reusable source-space candidate authoring; runtime promotion still requires source registration, bind fit and visual review gates.",
        "nextRequiredGate": "source_registration_bind_fit_and_visual_review_before_runtime",
    }


def plan_next_action(
    measurements_path: Path | str,
    method_audit_path: Path | str,
    compiler_pilot_path: Path | str | None = None,
) -> dict:
    measurements = read_json(measurements_path)
    method_audit = read_json(method_audit_path)
    guide_status = measurements.get("guideStatus")
    guide_sanity = measurements.get("guideSanity", {})
    proportion_sanity = measurements.get("proportionSanity", {})
    method_status = method_audit.get("status")

    if method_status == "METHOD_REPEAT_BLOCKED":
        return {
            "status": "CHANGE_METHOD_BEFORE_ASSET",
            "assetAuthoringAllowed": False,
            "reason": "proposed method repeats a locked failed method family",
            "blockedMethodFamilies": method_audit.get("blockedMethodFamilies", []),
            "requiredEvidenceBeforeAsset": "new_measurable_authoring_contract_or_method_change",
        }

    if guide_sanity.get("status") == "ANCHOR_GUIDE_SANITY_REVIEW_REQUIRED":
        return {
            "status": "FIX_GUIDE_BEFORE_ASSET",
            "assetAuthoringAllowed": False,
            "reason": "pose anchor measurements contain outliers; do not author garments against unstable guide",
            "outliers": guide_sanity.get("outliers", []),
            "fixTargets": measurements.get("directComparison", {}).get("fixTargets", []),
            "requiredEvidenceBeforeAsset": "rerun_measurement_with_anchor_guide_sanity_no_outliers",
        }

    if proportion_sanity.get("status") == "PROPORTION_SANITY_REVIEW_REQUIRED":
        return {
            "status": "FIX_PROPORTION_CONTRACT_BEFORE_ASSET",
            "assetAuthoringAllowed": False,
            "reason": "broad pose-family proportion contract has outliers; do not author garments against unstable body ratios",
            "outliers": proportion_sanity.get("outliers", []),
            "requiredEvidenceBeforeAsset": "rerun_measurement_with_proportion_sanity_no_outliers",
        }

    if (
        method_status == "METHOD_CHANGE_ALLOWED"
        and guide_sanity.get("status") == "ANCHOR_GUIDE_SANITY_NO_OUTLIERS"
        and proportion_sanity.get("status") in {"PROPORTION_SANITY_NO_OUTLIERS", "PROPORTION_SANITY_NO_OPTIONAL_RATIOS"}
    ):
        if guide_status and guide_status not in GUIDE_AUTHORITY_STATUSES:
            return {
                "status": "REVIEW_GUIDE_CANDIDATE_BEFORE_ASSET",
                "assetAuthoringAllowed": False,
                "candidateGuideAllowed": True,
                "diagnosticBoardAllowed": True,
                "reuseProven": False,
                "runtimePromotionAllowed": False,
                "guideStatus": guide_status,
                "reason": "measurements are clean, but the pose guide is not approved authority; review or promote the guide candidate before garment authoring",
                "requiredEvidenceBeforeAsset": "approved_pose_guide_authority_or_owner_accepted_candidate_override",
            }
        if compiler_pilot_path is not None:
            return evaluate_compiler_pilot(compiler_pilot_path)
        return {
            "status": "DIAGNOSTIC_STACK_BOARD_ALLOWED",
            "assetAuthoringAllowed": False,
            "diagnosticBoardAllowed": True,
            "reuseProven": False,
            "reason": "Số đo không có ngoại lệ chỉ cho phép đối chiếu; chưa chứng minh anatomy, phom hoặc khả năng tái sử dụng.",
            "requiredEvidenceBeforeAsset": [
                "reviewed_fit_family_with_occlusion_and_hidden_regions",
                "two_designs_reusing_mapping_across_all_six_poses_including_jump",
                "measured_second_item_effort_and_source_export_round_trip",
            ],
            "requiredReviewArtifact": "four_pose_stack_board_body_outer_top_waist_belt_shoulder_chest_guard",
        }

    return {
        "status": "PIPELINE_REVIEW_REQUIRED",
        "assetAuthoringAllowed": False,
        "reason": "measurement and method gates are not in a known authoring-safe state",
        "guideSanityStatus": guide_sanity.get("status"),
        "proportionSanityStatus": proportion_sanity.get("status"),
        "methodAuditStatus": method_status,
        "requiredEvidenceBeforeAsset": "resolve_gate_state_before_asset_candidate",
    }


def write_plan(
    measurements_path: Path | str,
    method_audit_path: Path | str,
    output: Path | str,
    compiler_pilot_path: Path | str | None = None,
) -> dict:
    output = Path(output)
    output.parent.mkdir(parents=True, exist_ok=True)
    result = plan_next_action(measurements_path, method_audit_path, compiler_pilot_path=compiler_pilot_path)
    output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return result


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--measurements", type=Path, required=True)
    parser.add_argument("--method-audit", type=Path, required=True)
    parser.add_argument("--compiler-pilot", type=Path)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = write_plan(args.measurements, args.method_audit, args.output, compiler_pilot_path=args.compiler_pilot)
    print(json.dumps({"status": result["status"], "assetAuthoringAllowed": result["assetAuthoringAllowed"]}, ensure_ascii=False))
    return 0 if result["assetAuthoringAllowed"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
