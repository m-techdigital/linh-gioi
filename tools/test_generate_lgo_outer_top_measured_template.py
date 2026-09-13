import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image

from generate_lgo_outer_top_measured_template import generate_templates


class GenerateOuterTopMeasuredTemplateTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.measurements = self.root / "measurements.json"
        self.output = self.root / "outer"
        self.measurements.write_text(
            json.dumps(
                {
                    "status": "GARMENT_ANCHOR_MEASUREMENTS_REVIEW_REQUIRED",
                    "guideSanity": {"status": "ANCHOR_GUIDE_SANITY_NO_OUTLIERS"},
                    "poses": {
                        "run_a": {
                            "slotGuides": {
                                "outer_top": {
                                    "shoulderLine": [[450, 500], [650, 540]],
                                    "waistLine": [[430, 820], [620, 850]],
                                    "torsoAxis": [[550, 470], [520, 860]],
                                }
                            }
                        }
                    },
                }
            )
        )

    def test_generates_review_only_canvas_mask_and_lineart(self):
        result = generate_templates(self.measurements, self.output, poses=["run_a"])

        self.assertEqual(result["status"], "OUTER_TOP_MEASURED_TEMPLATE_REVIEW_REQUIRED")
        self.assertFalse(result["runtimeEligible"])
        self.assertEqual(result["templateStyle"], "measured_trapezoid_v1")
        mask = self.output / "run_a-outer-top-template-mask.png"
        lineart = self.output / "run_a-outer-top-template-lineart.png"
        self.assertTrue(mask.exists())
        self.assertTrue(lineart.exists())
        with Image.open(mask) as image:
            self.assertEqual(image.size, (1024, 1536))
            self.assertIsNotNone(image.convert("RGBA").getchannel("A").getbbox())

    def test_rejects_unsane_measurement_guide(self):
        data = json.loads(self.measurements.read_text())
        data["guideSanity"]["status"] = "ANCHOR_GUIDE_SANITY_REVIEW_REQUIRED"
        self.measurements.write_text(json.dumps(data))

        with self.assertRaises(ValueError):
            generate_templates(self.measurements, self.output, poses=["run_a"])

    def test_cloth_lineart_style_records_collar_armhole_and_hem_refinements(self):
        result = generate_templates(
            self.measurements,
            self.output,
            poses=["run_a"],
            template_style="cloth_lineart_v2",
        )

        self.assertEqual(result["templateStyle"], "cloth_lineart_v2")
        pose = result["poses"][0]
        self.assertEqual(
            pose["refinements"],
            ["curved_side_seams", "collar_cutout", "near_armhole_cutout", "curved_hem"],
        )
        preview = self.output / "run_a-outer-top-template-preview.png"
        with Image.open(preview) as image:
            self.assertEqual(image.size, (1024, 1536))

    def test_cloth_lineart_v3_keeps_armhole_as_mask_guide_not_visible_line(self):
        result = generate_templates(
            self.measurements,
            self.output,
            poses=["run_a"],
            template_style="cloth_lineart_v3",
        )

        pose = result["poses"][0]
        self.assertEqual(result["templateStyle"], "cloth_lineart_v3")
        self.assertIn("mask_only_near_armhole", pose["refinements"])
        self.assertNotIn("near_armhole_cutout", pose["visibleLineart"])


if __name__ == "__main__":
    unittest.main()
