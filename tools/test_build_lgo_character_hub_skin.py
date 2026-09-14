#!/usr/bin/env python3
"""Visual-asset invariants for the shared Character Hub chrome generator."""
from __future__ import annotations

import unittest

import build_lgo_character_hub_skin as skin


class BuildLgoCharacterHubSkinTests(unittest.TestCase):
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
