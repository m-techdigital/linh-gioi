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
        # The template owns the complete fixed geometry. Dynamic class data only
        # overlays icons and labels, so all five node centres must already be visible.
        for x, y in skin.POTENTIAL_NODE_CENTERS:
            sample = image.crop((x - 64, y - 64, x + 65, y + 65))
            self.assertGreater(sample.getchannel("A").getbbox()[2], 100)
            # A node slot has one quiet authored outer frame. The icon atlas owns
            # the inner medallion, so the topology must not bake another bright
            # ring through the icon content area.
            self.assertLess(image.getpixel((x + 51, y))[3], 180)
        self.assertIn("character-hub-potential-topology.png", skin.BUILDERS)

    def test_potential_topology_runtime_png_stays_under_100_kib(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            skin.build(Path(directory))
            path = Path(directory) / "character-hub-potential-topology.png"
            self.assertLess(path.stat().st_size, 100 * 1024)

    def test_panel_motif_stays_confined_to_corner_regions(self) -> None:
        image = skin.build_panel()
        # The center must remain visually quiet; ornament belongs to the corners.
        center = image.crop((128, 128, 384, 384))
        pixels = list(center.convert("RGB").get_flattened_data())
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
