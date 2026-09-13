import json
import math
import tempfile
import unittest
from pathlib import Path

from PIL import Image

from measure_lgo_pose_garment_anchors import measure_guide, render_overlay, write_measurements


class GarmentAnchorMeasurementTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.guide = self.root / "guide.json"
        poses = []
        for pose, neck, near_shoulder, far_shoulder, near_hip, far_hip in [
            ("idle", [510, 263], [403, 322], [585, 342], [480, 718], [589, 720]),
            ("run_a", [636, 505], [536, 527], [719, 581], [491, 877], [570, 842]),
        ]:
            poses.append(
                {
                    "pose": pose,
                    "status": "DRAFT_GUIDE_REQUIRES_REVIEW",
                    "source": str(self.root / f"{pose}.png"),
                    "sourceSha256": "0" * 64,
                    "sourceCanvas": [1024, 1536],
                    "landmarks": [
                        {"id": "neck", "xy": neck},
                        {"id": "near_shoulder", "xy": near_shoulder},
                        {"id": "far_shoulder", "xy": far_shoulder},
                        {"id": "near_hip", "xy": near_hip},
                        {"id": "far_hip", "xy": far_hip},
                    ],
                }
            )
        self.guide.write_text(
            json.dumps(
                {
                    "status": "DRAFT_GUIDE_REQUIRES_REVIEW",
                    "sourceSpaceProfile": "lgo_character_canvas_1024x1536_v1",
                    "sourceCanvas": [1024, 1536],
                    "poses": poses,
                }
            )
        )

    def test_measures_torso_axis_and_slot_anchors_without_runtime_promotion(self):
        result = measure_guide(self.guide)

        self.assertEqual(result["status"], "GARMENT_ANCHOR_MEASUREMENTS_REVIEW_REQUIRED")
        self.assertFalse(result["runtimeEligible"])
        self.assertEqual(set(result["poses"]), {"idle", "run_a"})
        idle = result["poses"]["idle"]
        self.assertGreater(idle["measurements"]["torsoLengthPx"], 450)
        self.assertGreater(idle["measurements"]["shoulderWidthPx"], 180)
        self.assertEqual(idle["anchors"]["neck"]["xy"], [510.0, 263.0])
        self.assertEqual(len(idle["slotGuides"]["waist_belt"]["line"]), 2)

    def test_run_pose_measurements_are_pose_specific_not_global_pixel_guesses(self):
        result = measure_guide(self.guide)
        idle = result["poses"]["idle"]["slotGuides"]["waist_belt"]
        run = result["poses"]["run_a"]["slotGuides"]["waist_belt"]

        self.assertNotEqual(idle["center"], run["center"])
        idle_angle = idle["angleDegrees"]
        run_angle = run["angleDegrees"]
        self.assertTrue(math.isfinite(idle_angle))
        self.assertTrue(math.isfinite(run_angle))
        self.assertNotEqual(round(idle_angle, 2), round(run_angle, 2))

    def test_records_sanity_without_promoting_review_required_measurements(self):
        result = measure_guide(self.guide)

        self.assertEqual(result["guideSanity"]["status"], "ANCHOR_GUIDE_SANITY_NO_OUTLIERS")
        self.assertEqual(result["guideSanity"]["outliers"], [])
        self.assertIn("shoulderWidthPx", result["guideSanity"]["medians"])
        self.assertEqual(result["proportionSanity"]["status"], "PROPORTION_SANITY_NO_OUTLIERS")
        self.assertIn("shoulderToTorso", result["proportionSanity"]["medians"])

    def append_normal_run_b_pose(self, data):
        normal_run_b = json.loads(json.dumps(data["poses"][1]))
        normal_run_b["pose"] = "run_b"
        replacements = {
            "neck": {"id": "neck", "xy": [630, 499]},
            "near_shoulder": {"id": "near_shoulder", "xy": [530, 520]},
            "far_shoulder": {"id": "far_shoulder", "xy": [710, 570]},
            "near_hip": {"id": "near_hip", "xy": [555, 845]},
            "far_hip": {"id": "far_hip", "xy": [470, 870]},
        }
        normal_run_b["landmarks"] = [replacements.get(item["id"], item) for item in normal_run_b["landmarks"]]
        data["poses"].append(normal_run_b)

    def test_flags_pose_metric_outliers_before_fit_work(self):
        data = json.loads(self.guide.read_text())
        self.append_normal_run_b_pose(data)
        data["poses"][1]["landmarks"] = [
            item if item["id"] != "far_shoulder" else {"id": "far_shoulder", "xy": [545, 530]}
            for item in data["poses"][1]["landmarks"]
        ]
        self.guide.write_text(json.dumps(data))

        result = measure_guide(self.guide)

        self.assertEqual(result["guideSanity"]["status"], "ANCHOR_GUIDE_SANITY_REVIEW_REQUIRED")
        flagged = {(item["pose"], item["metric"]) for item in result["guideSanity"]["outliers"]}
        self.assertIn(("run_a", "shoulderWidthPx"), flagged)

    def test_outlier_report_includes_direct_fix_target_for_collapsed_shoulder_pair(self):
        data = json.loads(self.guide.read_text())
        self.append_normal_run_b_pose(data)
        data["poses"][1]["landmarks"] = [
            item if item["id"] != "far_shoulder" else {"id": "far_shoulder", "xy": [545, 530]}
            for item in data["poses"][1]["landmarks"]
        ]
        self.guide.write_text(json.dumps(data))

        result = measure_guide(self.guide)

        fixes = result["directComparison"]["fixTargets"]
        run_fix = next(item for item in fixes if item["pose"] == "run_a" and item["metric"] == "shoulderWidthPx")
        self.assertEqual(run_fix["currentLandmarks"], {"near_shoulder": [536.0, 527.0], "far_shoulder": [545.0, 530.0]})
        self.assertGreater(run_fix["targetMetricPx"], 150)
        self.assertEqual(run_fix["recommendedNextStep"], "review_or_move_far_shoulder_to_target_before_garment_fit")
        self.assertIn("targetFarShoulderPreserveNear", run_fix)

    def test_write_measurements_records_payload_hash(self):
        output = self.root / "measurements.json"
        result = write_measurements(self.guide, output)
        written = json.loads(output.read_text())

        self.assertEqual(written["auditPayloadSha256"], result["auditPayloadSha256"])
        self.assertFalse(written["runtimeEligible"])

    def test_render_overlay_marks_current_and_target_fix_on_source_image(self):
        source = self.root / "run_a.png"
        Image.new("RGBA", (1024, 1536), (240, 240, 240, 255)).save(source)
        data = json.loads(self.guide.read_text())
        data["poses"][1]["source"] = str(source)
        self.append_normal_run_b_pose(data)
        data["poses"][1]["landmarks"] = [
            item if item["id"] != "far_shoulder" else {"id": "far_shoulder", "xy": [545, 530]}
            for item in data["poses"][1]["landmarks"]
        ]
        self.guide.write_text(json.dumps(data))
        result = measure_guide(self.guide)
        overlay = self.root / "overlay.png"

        render_overlay(result, overlay)

        self.assertTrue(overlay.exists())
        rendered = Image.open(overlay).convert("RGBA")
        target = next(
            item
            for item in result["directComparison"]["fixTargets"]
            if item["pose"] == "run_a" and item["metric"] == "shoulderWidthPx"
        )
        x, y = [round(v) for v in target["targetFarShoulderPreserveNear"]]
        pose_index = list(result["poses"]).index("run_a")
        board_x = x + (pose_index % 3) * 1024
        board_y = y + (pose_index // 3) * 1536
        self.assertEqual(rendered.getpixel((board_x, board_y))[:3], (255, 0, 255))

    def test_proportion_sanity_compares_head_and_shoulder_ratios_before_asset(self):
        data = json.loads(self.guide.read_text())
        for pose in data["poses"]:
            pose["landmarks"].extend(
                [
                    {"id": "crown", "xy": [500, 100]},
                    {"id": "chin", "xy": [500, 220]},
                ]
            )
        data["poses"][1]["landmarks"] = [
            item if item["id"] != "chin" else {"id": "chin", "xy": [500, 360]}
            for item in data["poses"][1]["landmarks"]
        ]
        self.guide.write_text(json.dumps(data))

        result = measure_guide(self.guide)

        self.assertEqual(result["proportionSanity"]["status"], "PROPORTION_SANITY_REVIEW_REQUIRED")
        flagged = {(item["pose"], item["ratio"]) for item in result["proportionSanity"]["outliers"]}
        self.assertIn(("run_a", "headToTorso"), flagged)


if __name__ == "__main__":
    unittest.main()
