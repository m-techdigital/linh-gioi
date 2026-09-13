import json
import tempfile
import unittest
from pathlib import Path

from plan_lgo_six_pose_source_repair_batch import plan_repair_batch, render_markdown


class SixPoseSourceRepairBatchPlanTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def coverage(self):
        source_root = self.root / "pose-matched-layer-authoring-v1"
        return {
            "status": "SOURCE_COVERAGE_INCOMPLETE",
            "runtimeEligible": False,
            "sourceRoot": str(source_root),
            "bodyRoot": str(self.root / "legacy-base-run-contact-jump-v3-div4"),
            "poses": [
                "idle",
                "run_contact_a",
                "run_a",
                "run_contact_b",
                "run_b",
                "jump_tuck",
            ],
            "variants": ["A", "B"],
            "slots": {
                "inner_top": {
                    "kind": "six_pose_native_front_back",
                    "path": str(source_root / "inner-top-native-v2"),
                    "present": 24,
                    "requiredCount": 24,
                },
                "class_accessory": {
                    "kind": "six_pose_native_front_back",
                    "path": str(source_root / "accessory-native-v1"),
                    "present": 24,
                    "requiredCount": 24,
                },
                "outer_top": {
                    "kind": "idle_material_candidate",
                    "path": str(source_root / "outer-top-material-idle-v2"),
                    "coveredCandidatePoses": ["idle"],
                    "missingPoses": [
                        "run_contact_a",
                        "run_a",
                        "run_contact_b",
                        "run_b",
                        "jump_tuck",
                    ],
                },
                "waist_belt": {
                    "kind": "idle_material_candidate",
                    "path": str(source_root / "waist-belt-material-idle-v2"),
                    "runDraftPath": str(source_root / "waist-belt-run-four-material-v1"),
                    "coveredCandidatePoses": [
                        "idle",
                        "run_contact_a",
                        "run_a",
                        "run_contact_b",
                        "run_b",
                    ],
                    "missingPoses": ["jump_tuck"],
                },
                "shoulder_chest_guard": {
                    "kind": "idle_material_candidate",
                    "path": str(source_root / "shoulder-chest-guard-material-idle-v4"),
                    "coveredCandidatePoses": ["idle"],
                    "missingPoses": [
                        "run_contact_a",
                        "run_a",
                        "run_contact_b",
                        "run_b",
                        "jump_tuck",
                    ],
                },
            },
            "blockingGates": [
                "outer_top: only idle candidate, no six-pose A/B source acceptance",
                "waist_belt: draft covers 5/6 poses but lacks A/B source acceptance",
                "shoulder_chest_guard: only idle candidate, no six-pose A/B source acceptance",
                "outer_top collar/hem ownership unresolved",
                "jump body anatomy candidate not accepted",
            ],
        }

    def test_plans_one_grouped_repair_batch_from_coverage_without_runtime_promotion(self):
        plan = plan_repair_batch(self.coverage(), batch_id="six-pose-source-repair-batch-v1")

        self.assertEqual(plan["status"], "SOURCE_REPAIR_BATCH_READY")
        self.assertFalse(plan["runtimePromotionAllowed"])
        self.assertEqual(
            [slot["slot"] for slot in plan["slotRepairs"]],
            ["outer_top", "waist_belt", "shoulder_chest_guard"],
        )
        self.assertEqual(
            plan["slotRepairs"][0]["candidateDirectory"],
            "outer-top-six-pose-source-repair-v1",
        )
        self.assertEqual(plan["slotRepairs"][0]["missingPoses"], [
            "run_contact_a",
            "run_a",
            "run_contact_b",
            "run_b",
            "jump_tuck",
        ])
        self.assertEqual(plan["slotRepairs"][1]["reuseSourceDirectories"], [
            "waist-belt-material-idle-v2",
            "waist-belt-run-four-material-v1",
        ])
        self.assertIn("jump_tuck_body_anatomy_acceptance", plan["sharedDependencies"])
        self.assertIn("skeletal_generated_cutout_runtime_probe", plan["forbiddenMethodFamilies"])
        self.assertIn("per_pose_pixel_nudging", plan["forbiddenMethodFamilies"])

    def test_rendered_markdown_names_evidence_board_and_forbidden_loops(self):
        plan = plan_repair_batch(self.coverage(), batch_id="six-pose-source-repair-batch-v1")

        markdown = render_markdown(plan)

        self.assertIn("# Six-pose source repair batch v1", markdown)
        self.assertIn("source-board-v1/contact-sheet.png", markdown)
        self.assertIn("outer-top-six-pose-source-repair-v1", markdown)
        self.assertIn("Không dùng skeletal generated-cutout", markdown)
        self.assertIn("Không chỉnh mò từng pose/pixel", markdown)
        self.assertIn("runtimePromotionAllowed=false", markdown)


if __name__ == "__main__":
    unittest.main()
