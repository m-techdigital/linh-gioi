#!/usr/bin/env python3
"""Visual-asset invariants for the shared Character Hub chrome generator."""
from __future__ import annotations

import unittest
import tempfile
from pathlib import Path

import build_lgo_character_hub_skin as skin


class BuildLgoCharacterHubSkinTests(unittest.TestCase):
    def test_potential_topology_is_one_actual_size_reusable_template(self) -> None:
        image = skin.build_potential_topology()

        self.assertEqual(image.size, (600, 520))
        self.assertEqual(image.mode, "RGBA")
        # Orbit/core/value/add geometry stays in one bitmap. Node rings are
        # separate shared sprite instances owned once by the runtime base.
        for x, y in skin.POTENTIAL_NODE_CENTERS:
            self.assertGreater(image.getpixel((x-40, y+94))[3], 200)
        self.assertEqual(image.getpixel((300, 11))[3], 0, "Do not bake a second copy of the icon ring")
        self.assertIn("character-hub-potential-topology.png", skin.BUILDERS)

    def test_potential_add_glyphs_belong_below_the_medallion_and_caption(self) -> None:
        image = skin.build_potential_topology()
        for x, y in skin.POTENTIAL_NODE_CENTERS:
            red, green, blue, alpha = image.getpixel((x + 56, y + 94))
            self.assertGreater(red, 190)
            self.assertGreater(green, 130)
            self.assertLess(blue, 160)
            self.assertGreater(alpha, 200)

    def test_potential_core_reuses_a_pinned_224px_ui_module(self) -> None:
        core = skin.build_meditation_core()
        self.assertEqual(core.size, (224, 224))
        self.assertEqual(core.mode, "RGBA")
        self.assertEqual(core.getpixel((0, 0))[3], 0)
        self.assertGreater(core.getpixel((112, 112))[3], 240)
        self.assertIn("character-hub-potential-core.png", skin.BUILDERS)
        image = skin.build_potential_topology()
        expected = core.getpixel((112, 96))
        actual = image.getpixel((300, 244))
        self.assertLess(max(abs(a - b) for a, b in zip(expected[:3], actual[:3])), 12)

    def test_potential_topology_runtime_png_stays_under_100_kib(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            skin.build(Path(directory))
            path = Path(directory) / "character-hub-potential-topology.png"
            self.assertLess(path.stat().st_size, 100 * 1024)

    def test_potential_icon_modules_separate_one_frame_from_six_contents(self) -> None:
        self.assertTrue(hasattr(skin, "compose_potential_icon_modules"),
                        "Potential icons still duplicate complete medallions")
        from PIL import Image
        atlas, parts = skin.compose_potential_icon_modules(Image.new("RGBA", (1536, 1024), "white"))
        self.assertEqual(atlas.size, (512, 256))
        self.assertEqual([p["id"] for p in parts],
                         ["attack", "defense", "vitality", "spirit", "agility", "core", "frame"])
        for part in parts:
            tile = atlas.crop((part["x"], 256-part["y"]-128, part["x"]+128, 256-part["y"]))
            if part["id"] == "frame":
                self.assertEqual(tile.getpixel((64, 64))[3], 0, "Shared frame must have a transparent aperture")
                self.assertGreater(tile.getpixel((114, 64))[3], 240)
            else:
                self.assertGreater(tile.getpixel((64, 64))[3], 240)
                for point in ((8,64), (120,64), (64,8), (64,120)):
                    self.assertEqual(tile.getpixel(point)[3], 0, "Inner art must not contain the gold ring")
        second, second_parts = skin.compose_potential_icon_modules(Image.new("RGBA", (1536, 1024), "white"))
        self.assertEqual(atlas.tobytes(), second.tobytes())
        self.assertEqual(parts, second_parts)

    def test_panel_motif_stays_confined_to_corner_regions(self) -> None:
        image = skin.build_panel()
        # The center must remain visually quiet; ornament belongs to the corners.
        center = image.crop((128, 128, 384, 384))
        rgb = center.convert("RGB")
        pixels = list(rgb.get_flattened_data() if hasattr(rgb, "get_flattened_data") else rgb.getdata())
        row_deltas = []
        for y in range(center.height):
            row = pixels[y * center.width:(y + 1) * center.width]
            row_deltas.append(max(pixel[2] for pixel in row) - min(pixel[2] for pixel in row))
        self.assertLess(max(row_deltas), 5)

    def test_shell_surface_does_not_bake_a_second_frame_system(self) -> None:
        image = skin.build_shell().convert("RGB")
        top = [image.getpixel((x, 1)) for x in range(16, image.width - 16)]
        gold_like = sum(pixel[0] > 130 and pixel[1] > 90 and pixel[2] < 100 for pixel in top)
        self.assertLess(gold_like / len(top), 0.05)

if __name__ == "__main__":
    unittest.main()
