import json
import tempfile
import unittest
from pathlib import Path

from apply_lgo_pose_landmark_overrides import apply_overrides
from audit_lgo_pose_method_repeats import audit_method_repeats
from plan_lgo_pose_pipeline_next_action import plan_next_action


class PosePipelineGateTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def mkdirs(self, *names):
        for name in names:
            (self.root / name).mkdir()

    def test_blocks_repeating_failed_polygon_method_family(self):
        self.mkdirs("outer-top-run-four-material-v1", "outer-top-run-four-material-v2")

        result = audit_method_repeats(self.root, proposed_method_family="polygon_eye_fit")

        self.assertEqual(result["status"], "METHOD_REPEAT_BLOCKED")
        self.assertEqual(result["proposedMethodFamily"], "polygon_eye_fit")
        self.assertEqual(result["methodFamilies"]["polygon_eye_fit"]["candidateCount"], 2)

    def test_allows_method_family_with_new_measurement_contract(self):
        self.mkdirs("outer-top-run-four-material-v1", "outer-top-run-four-material-v2")

        result = audit_method_repeats(self.root, proposed_method_family="measured_anchor_stack")

        self.assertEqual(result["status"], "METHOD_CHANGE_ALLOWED")
        self.assertEqual(result["methodFamilies"]["polygon_eye_fit"]["repeatStatus"], "LOCKED_AFTER_REPEATED_FAILURE")

    def write_planner_inputs(self, guide_status, proposed_status, source_guide_status="APPROVED_GUIDE_AUTHORITY"):
        measurements = self.root / "measurements.json"
        method_audit = self.root / "method-audit.json"
        measurements.write_text(
            json.dumps(
                {
                    "guideStatus": source_guide_status,
                    "guideSanity": {
                        "status": guide_status,
                        "outliers": [{"pose": "run_b", "metric": "shoulderWidthPx"}]
                        if guide_status == "ANCHOR_GUIDE_SANITY_REVIEW_REQUIRED"
                        else [],
                    },
                    "proportionSanity": {"status": "PROPORTION_SANITY_NO_OUTLIERS", "outliers": []},
                    "directComparison": {"fixTargets": [{"pose": "run_b", "metric": "shoulderWidthPx"}]},
                }
            )
        )
        method_audit.write_text(
            json.dumps(
                {
                    "status": proposed_status,
                    "blockedMethodFamilies": ["polygon_eye_fit"],
                    "proposedMethodFamily": "measured_anchor_stack",
                }
            )
        )
        return measurements, method_audit

    def write_compiler_pilot(self, **overrides):
        data = {
            "status": "GARMENT_FAMILY_COMPILER_PILOT_PASS",
            "methodFamily": "garment_family_compiler",
            "fitStrategy": "semantic_occlusion_weighted_source_space",
            "sourceSpaceProfile": "lgo_character_canvas_1024x1536_v1",
            "poseCoverage": {
                "required": ["idle", "walk", "run", "jump_tuck", "attack", "hurt"],
                "rendered": ["idle", "walk", "run", "jump_tuck", "attack", "hurt"],
            },
            "evidence": {
                "semanticOcclusionOrWeights": True,
                "surfaceVariantReusesGeometryAndMapping": True,
                "geometryVariantRegeneratesFromParameters": True,
                "unseenItemAfterTemplateLock": True,
                "noPerPosePngEdits": True,
                "noRuntimeOffsetCompensation": True,
            },
            "manualIntervention": {
                "perVariantPixelEdits": 0,
                "sharedSourceAdjustmentRounds": 1,
            },
        }
        for key, value in overrides.items():
            if isinstance(value, dict) and isinstance(data.get(key), dict):
                data[key].update(value)
            else:
                data[key] = value
        path = self.root / "compiler-pilot.json"
        path.write_text(json.dumps(data))
        return path

    def test_requires_guide_fix_before_asset_when_measurement_has_outliers(self):
        measurements, method_audit = self.write_planner_inputs(
            "ANCHOR_GUIDE_SANITY_REVIEW_REQUIRED", "METHOD_CHANGE_ALLOWED"
        )

        result = plan_next_action(measurements, method_audit)

        self.assertEqual(result["status"], "FIX_GUIDE_BEFORE_ASSET")
        self.assertFalse(result["assetAuthoringAllowed"])

    def test_clean_measurements_allow_diagnostics_but_not_garment_production(self):
        measurements, method_audit = self.write_planner_inputs(
            "ANCHOR_GUIDE_SANITY_NO_OUTLIERS", "METHOD_CHANGE_ALLOWED"
        )

        result = plan_next_action(measurements, method_audit)

        self.assertFalse(result["assetAuthoringAllowed"])
        self.assertEqual(result["status"], "DIAGNOSTIC_STACK_BOARD_ALLOWED")
        self.assertTrue(result["diagnosticBoardAllowed"])
        self.assertFalse(result["reuseProven"])

    def test_clean_measurements_with_compiler_pilot_pass_allow_controlled_candidate_authoring(self):
        measurements, method_audit = self.write_planner_inputs(
            "ANCHOR_GUIDE_SANITY_NO_OUTLIERS", "METHOD_CHANGE_ALLOWED"
        )
        pilot = self.write_compiler_pilot()

        result = plan_next_action(measurements, method_audit, compiler_pilot_path=pilot)

        self.assertEqual(result["status"], "GARMENT_FAMILY_COMPILER_CANDIDATE_ALLOWED")
        self.assertTrue(result["assetAuthoringAllowed"])
        self.assertTrue(result["reuseProven"])
        self.assertFalse(result["runtimePromotionAllowed"])
        self.assertIn("unseen_item_after_template_lock", result["provenEvidence"])

    def test_compiler_pilot_with_candidate_guide_requires_guide_review_before_asset_authoring(self):
        measurements, method_audit = self.write_planner_inputs(
            "ANCHOR_GUIDE_SANITY_NO_OUTLIERS",
            "METHOD_CHANGE_ALLOWED",
            source_guide_status="CANDIDATE_REVIEW_ONLY",
        )
        pilot = self.write_compiler_pilot()

        result = plan_next_action(measurements, method_audit, compiler_pilot_path=pilot)

        self.assertEqual(result["status"], "REVIEW_GUIDE_CANDIDATE_BEFORE_ASSET")
        self.assertFalse(result["assetAuthoringAllowed"])
        self.assertTrue(result["candidateGuideAllowed"])
        self.assertEqual(result["guideStatus"], "CANDIDATE_REVIEW_ONLY")

    def test_compiler_pilot_blocks_direct_panel_fit_even_when_images_exist(self):
        measurements, method_audit = self.write_planner_inputs(
            "ANCHOR_GUIDE_SANITY_NO_OUTLIERS", "METHOD_CHANGE_ALLOWED"
        )
        pilot = self.write_compiler_pilot(
            fitStrategy="direct_2d_pattern_panel_affine_fit",
            evidence={"semanticOcclusionOrWeights": False},
        )

        result = plan_next_action(measurements, method_audit, compiler_pilot_path=pilot)

        self.assertEqual(result["status"], "COMPILER_PILOT_EVIDENCE_REQUIRED")
        self.assertFalse(result["assetAuthoringAllowed"])
        self.assertIn("semantic_occlusion_or_weights", result["missingEvidence"])
        self.assertIn("direct_panel_fit_rejected", result["rejectedEvidence"])

    def test_compiler_pilot_requires_unseen_item_after_template_lock(self):
        measurements, method_audit = self.write_planner_inputs(
            "ANCHOR_GUIDE_SANITY_NO_OUTLIERS", "METHOD_CHANGE_ALLOWED"
        )
        pilot = self.write_compiler_pilot(
            evidence={"unseenItemAfterTemplateLock": False},
        )

        result = plan_next_action(measurements, method_audit, compiler_pilot_path=pilot)

        self.assertEqual(result["status"], "COMPILER_PILOT_EVIDENCE_REQUIRED")
        self.assertFalse(result["assetAuthoringAllowed"])
        self.assertIn("unseen_item_after_template_lock", result["missingEvidence"])

    def test_blocks_when_proposed_method_repeats_a_locked_family(self):
        measurements, method_audit = self.write_planner_inputs(
            "ANCHOR_GUIDE_SANITY_NO_OUTLIERS", "METHOD_REPEAT_BLOCKED"
        )

        result = plan_next_action(measurements, method_audit)

        self.assertEqual(result["status"], "CHANGE_METHOD_BEFORE_ASSET")
        self.assertFalse(result["assetAuthoringAllowed"])

    def test_blocks_asset_when_broad_proportion_contract_has_outliers(self):
        measurements, method_audit = self.write_planner_inputs(
            "ANCHOR_GUIDE_SANITY_NO_OUTLIERS", "METHOD_CHANGE_ALLOWED"
        )
        data = json.loads(measurements.read_text())
        data["proportionSanity"] = {
            "status": "PROPORTION_SANITY_REVIEW_REQUIRED",
            "outliers": [{"pose": "jump_tuck", "ratio": "headToTorso"}],
        }
        measurements.write_text(json.dumps(data))

        result = plan_next_action(measurements, method_audit)

        self.assertEqual(result["status"], "FIX_PROPORTION_CONTRACT_BEFORE_ASSET")
        self.assertFalse(result["assetAuthoringAllowed"])

    def test_writes_landmark_candidate_without_mutating_source_guide(self):
        guide = self.root / "guide.json"
        overrides = self.root / "overrides.json"
        output = self.root / "candidate.json"
        guide.write_text(
            json.dumps(
                {
                    "status": "DRAFT_GUIDE_REQUIRES_REVIEW",
                    "poses": [
                        {
                            "pose": "run_b",
                            "landmarks": [
                                {"id": "near_shoulder", "xy": [568, 542]},
                                {"id": "far_shoulder", "xy": [511, 561]},
                            ],
                        }
                    ],
                }
            )
        )
        overrides.write_text(
            json.dumps(
                {
                    "status": "CANDIDATE_REVIEW_ONLY",
                    "reason": "visual zoom review",
                    "overrides": {"run_b": {"near_shoulder": [645, 548], "far_shoulder": [500, 570]}},
                }
            )
        )

        result = apply_overrides(guide, overrides, output)

        original = json.loads(guide.read_text())
        candidate = json.loads(output.read_text())
        self.assertEqual(original["poses"][0]["landmarks"][0]["xy"], [568, 542])
        self.assertEqual(candidate["status"], "CANDIDATE_REVIEW_ONLY")
        self.assertEqual(candidate["poses"][0]["landmarks"][0]["xy"], [645, 548])
        self.assertEqual(result["changedLandmarks"][0]["pose"], "run_b")


if __name__ == "__main__":
    unittest.main()
