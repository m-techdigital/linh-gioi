import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image

from compose_lgo_pose_stack_review_board import compose_stack_board


class ComposePoseStackReviewBoardTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.output = self.root / "board.png"
        self.report = self.root / "report.json"

    def png(self, name, color):
        path = self.root / name
        Image.new("RGBA", (1024, 1536), color).save(path)
        return path

    def test_labels_do_not_overwrite_registered_source_pixels(self):
        from compose_lgo_pose_stack_review_board import compose_pose
        body = self.png("body.png", (10, 20, 30, 255))
        image, _ = compose_pose("idle", [{"slot": "body", "path": str(body), "state": "authority"}])
        self.assertGreater(image.height, 1536)
        header = image.height - 1536
        self.assertEqual(image.crop((0, header, 1024, image.height)).tobytes(), Image.open(body).convert("RGBA").tobytes())

    def test_report_requires_fix_when_stack_contains_rejected_or_missing_layers(self):
        body = self.png("body.png", (10, 10, 10, 255))
        outer = self.png("outer.png", (255, 0, 0, 128))
        manifest = {
            "poses": ["run_a"],
            "layers": {
                "run_a": [
                    {"slot": "body", "path": str(body), "state": "authority"},
                    {"slot": "outer_top", "path": str(outer), "state": "rejected"},
                    {"slot": "shoulder_chest_guard", "state": "missing"},
                ]
            },
        }

        result = compose_stack_board(manifest, self.output, self.report)

        self.assertEqual(result["status"], "STACK_REVIEW_FIX_REQUIRED")
        self.assertTrue(self.output.exists())
        self.assertEqual(result["summary"]["rejected"], 1)
        self.assertEqual(result["summary"]["missing"], 1)

    def test_all_draft_or_better_layers_create_review_required_board(self):
        body = self.png("body.png", (10, 10, 10, 255))
        waist = self.png("waist.png", (255, 200, 0, 128))
        manifest = {
            "poses": ["run_a"],
            "layers": {
                "run_a": [
                    {"slot": "body", "path": str(body), "state": "authority"},
                    {"slot": "waist_belt", "path": str(waist), "state": "draft"},
                ]
            },
        }

        result = compose_stack_board(manifest, self.output, self.report)

        self.assertEqual(result["status"], "STACK_REVIEW_REQUIRED")
        self.assertTrue(json.loads(self.report.read_text())["runtimeEligible"] is False)


if __name__ == "__main__":
    unittest.main()
