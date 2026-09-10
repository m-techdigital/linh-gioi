import hashlib
import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw


ROOT = Path(__file__).resolve().parents[2]
SCRIPT = ROOT / "tools" / "extract_lgo_equipment_grid.py"
CANONICAL_SLOTS = [
    "main_weapon", "head_hair", "inner_top", "outer_top", "lower_body",
    "waist_belt", "arm_guard", "footwear", "shoulder_chest_guard", "class_accessory",
]


class EquipmentGridExtractorTests(unittest.TestCase):
    def make_plan(self, root, slots):
        source = root / "grid.png"
        image = Image.new("RGB", (80, 200), "white")
        draw = ImageDraw.Draw(image)
        for row in range(10):
            for column in range(4):
                x, y = column * 20, row * 20
                draw.rectangle((x + 5, y + 5, x + 14, y + 14), fill=(20 + row, 50 + column, 110))
        image.save(source)
        plan = {
            "classId": "kiem",
            "levels": [1, 10, 20, 30],
            "slots": slots,
            "sources": [{
                "gender": "male",
                "path": str(source),
                "sha256": hashlib.sha256(source.read_bytes()).hexdigest(),
                "xBoundaries": [0, 20, 40, 60, 80],
                "yBoundaries": list(range(0, 201, 20)),
                "padding": 1,
            }],
        }
        path = root / "plan.json"
        path.write_text(json.dumps(plan))
        return path

    def run_tool(self, plan, out):
        return subprocess.run(
            [sys.executable, str(SCRIPT), "--plan", str(plan), "--out-dir", str(out)],
            cwd=ROOT,
            text=True,
            capture_output=True,
        )

    def test_manifest_marks_every_crop_as_unverified_candidate(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            result = self.run_tool(self.make_plan(root, CANONICAL_SLOTS), root / "out")
            self.assertEqual(result.returncode, 0, result.stderr)
            manifest = json.loads((root / "out" / "source-manifest.json").read_text())
            self.assertEqual(manifest["status"], "SOURCE_CANDIDATES_EXTRACTED_BASE_FIT_UNVERIFIED")
            self.assertEqual(len(manifest["items"]), 40)
            self.assertTrue(all(item["fitStatus"] == "candidate" for item in manifest["items"]))
            self.assertTrue(all(item["runtimeEligible"] is False for item in manifest["items"]))

    def test_rejects_legacy_or_invented_slot_names(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            legacy = CANONICAL_SLOTS.copy()
            legacy[3] = "outer_tunic"
            result = self.run_tool(self.make_plan(root, legacy), root / "out")
            self.assertNotEqual(result.returncode, 0)
            self.assertIn("canonical slots", result.stderr)


if __name__ == "__main__":
    unittest.main()
