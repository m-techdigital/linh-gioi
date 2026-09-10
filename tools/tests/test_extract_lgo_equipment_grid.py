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
    def make_plan(self, root, slots, background_mode="light"):
        source = root / "grid.png"
        background = (255, 0, 255) if background_mode == "magenta" else "white"
        image = Image.new("RGB", (80, 200), background)
        draw = ImageDraw.Draw(image)
        for row in range(10):
            for column in range(4):
                x, y = column * 20, row * 20
                if background_mode == "magenta":
                    draw.rectangle((x + 4, y + 4, x + 15, y + 15), fill=(170, 100, 170))
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
                "backgroundMode": background_mode,
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

    def test_magenta_source_becomes_real_transparency(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            result = self.run_tool(
                self.make_plan(root, CANONICAL_SLOTS, background_mode="magenta"),
                root / "out")
            self.assertEqual(result.returncode, 0, result.stderr)
            item = Image.open(root / "out" / "kiem-lv001-male-main_weapon.png").convert("RGBA")
            self.assertEqual(item.getchannel("A").getextrema(), (147, 255))
            edge = item.getpixel((0, 0))
            self.assertLessEqual(min(edge[0], edge[2]) - edge[1], 24)
            self.assertLess(item.width, 18)
            self.assertLess(item.height, 18)


if __name__ == "__main__":
    unittest.main()
