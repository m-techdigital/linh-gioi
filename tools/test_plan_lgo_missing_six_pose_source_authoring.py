import json
import tempfile
import unittest
from pathlib import Path

from plan_lgo_missing_six_pose_source_authoring import plan_missing_source_authoring, render_markdown


class MissingSixPoseSourceAuthoringPlanTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def audit(self):
        return {
            "status": "SOURCE_REPAIR_LAYER_GAP",
            "failures": [
                "outer_top/A/run_a/front.png missing",
                "outer_top/A/run_a/back.png missing",
                "outer_top/B/run_a/front.png missing",
                "outer_top/B/run_a/back.png missing",
                "waist_belt/A/jump_tuck/front.png missing",
                "waist_belt/B/jump_tuck/back.png missing",
            ],
        }

    def measurements(self):
        return {
            "status": "GARMENT_ANCHOR_MEASUREMENTS_REVIEW_REQUIRED",
            "guideStatus": "CANDIDATE_REVIEW_ONLY",
            "guideSanity": {"status": "ANCHOR_GUIDE_SANITY_NO_OUTLIERS", "outliers": []},
            "proportionSanity": {"status": "PROPORTION_SANITY_NO_OUTLIERS", "outliers": []},
            "poses": {
                "run_a": {
                    "slotGuides": {
                        "outer_top": {
                            "center": [598.02, 632.62],
                            "shoulderLine": [[520, 600], [670, 640]],
                            "waistLine": [[500, 780], [650, 820]],
                            "torsoAxis": [[560, 500], [550, 800]],
                        }
                    },
                    "measurements": {"shoulderWidthPx": 190.8, "torsoLengthPx": 369.87},
                },
                "jump_tuck": {
                    "slotGuides": {
                        "waist_belt": {
                            "center": [439.71, 803.1],
                            "line": [[390, 790], [490, 815]],
                            "angleDegrees": 18.56,
                            "halfWidthPx": 62.4,
                        }
                    },
                    "measurements": {"shoulderWidthPx": 144.52, "torsoLengthPx": 379.63},
                },
            },
        }

    def test_collapses_layer_failures_to_unique_slot_pose_authoring_targets_with_measurements(self):
        plan = plan_missing_source_authoring(self.audit(), self.measurements())

        self.assertEqual(plan["status"], "MISSING_SOURCE_AUTHORING_BRIEF_READY")
        self.assertFalse(plan["runtimePromotionAllowed"])
        self.assertEqual([(task["slot"], task["pose"]) for task in plan["targets"]], [
            ("outer_top", "run_a"),
            ("waist_belt", "jump_tuck"),
        ])
        self.assertEqual(plan["targets"][0]["slotGuide"]["center"], [598.02, 632.62])
        self.assertEqual(plan["targets"][1]["slotGuide"]["angleDegrees"], 18.56)
        self.assertIn("do_not_synthesize_missing_pose", plan["guardrails"])

    def test_markdown_explains_where_dimensions_are_used(self):
        plan = plan_missing_source_authoring(self.audit(), self.measurements())

        markdown = render_markdown(plan)

        self.assertIn("# Missing six-pose source authoring brief", markdown)
        self.assertIn("outer_top / run_a", markdown)
        self.assertIn("center `[598.02, 632.62]`", markdown)
        self.assertIn("waist_belt / jump_tuck", markdown)
        self.assertIn("không phải runtime offset", markdown)


if __name__ == "__main__":
    unittest.main()
