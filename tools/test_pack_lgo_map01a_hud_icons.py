import hashlib
import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image

from pack_lgo_map01a_hud_icons import ICON_IDS, build


class PackMap01AHudIconsTests(unittest.TestCase):
    def test_build_is_deterministic_and_declares_runtime_asset(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            first = root / "first"
            second = root / "second"

            first_manifest = build(first)
            second_manifest = build(second)

            first_png = first / "map01a-hud-icons.png"
            second_png = second / "map01a-hud-icons.png"
            self.assertEqual(first_png.read_bytes(), second_png.read_bytes())
            self.assertEqual(hashlib.sha256(first_png.read_bytes()).hexdigest(), first_manifest["sha256"])
            self.assertEqual([512, 256], first_manifest["textureSize"])
            self.assertEqual(list(ICON_IDS), [part["id"] for part in first_manifest["parts"]])
            self.assertEqual("ui-hud-atlas", first_manifest["assets"][0]["role"])
            self.assertEqual("pack_lgo_map01a_hud_icons", first_manifest["assets"][0]["generator"])
            self.assertFalse(first_manifest["assets"][0]["referenceOnly"])
            self.assertEqual(first_manifest, json.loads((first / "manifest.json").read_text(encoding="utf-8")))
            self.assertEqual(first_manifest["sha256"], second_manifest["sha256"])

    def test_each_icon_has_visible_pixels_inside_its_declared_cell(self):
        with tempfile.TemporaryDirectory() as temp:
            output = Path(temp) / "pack"
            manifest = build(output)
            atlas = Image.open(output / "map01a-hud-icons.png").convert("RGBA")

            for part in manifest["parts"]:
                top = atlas.height - part["y"] - part["h"]
                icon = atlas.crop((part["x"], top, part["x"] + part["w"], top + part["h"]))
                alpha = icon.getchannel("A")
                visible = sum(1 for value in alpha.get_flattened_data() if value > 16)
                self.assertGreater(visible, 450, part["id"])
                self.assertLess(visible, part["w"] * part["h"] * 0.75, part["id"])


if __name__ == "__main__":
    unittest.main()
