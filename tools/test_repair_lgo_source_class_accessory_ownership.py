#!/usr/bin/env python3.12
"""Regression tests for class-accessory source ownership repair."""
from __future__ import annotations

import tempfile
import unittest
from pathlib import Path

import numpy as np
from PIL import Image

import repair_lgo_source_class_accessory_ownership as repair


class RepairClassAccessoryOwnershipTests(unittest.TestCase):
    def test_moves_large_non_accessory_pixels_to_outer_top_and_preserves_union(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            source = root / "source"
            output = root / "output"
            for slot in repair.SLOTS:
                for pose in repair.POSES:
                    target = source / slot / f"{pose}.png"
                    target.parent.mkdir(parents=True, exist_ok=True)
                    image = Image.new("RGBA", (128, 128), (0, 0, 0, 0))
                    if slot == "class_accessory":
                        for y in range(4, 108):
                            for x in range(4, 108):
                                image.putpixel((x, y), (200, 120, 40, 255))
                        for y in range(112, 118):
                            for x in range(112, 118):
                                image.putpixel((x, y), (250, 220, 80, 255))
                    if slot == "waist_belt":
                        for y in range(111, 119):
                            for x in range(111, 119):
                                image.putpixel((x, y), (80, 40, 10, 255))
                    image.save(target)

            payload = repair.repair_surface(source, output)

            self.assertEqual("SOURCE_REVIEW_REQUIRED", payload["status"])
            accessory = np.asarray(Image.open(output / "class_accessory" / "jump_tuck.png").convert("RGBA"))
            outer = np.asarray(Image.open(output / "outer_top" / "jump_tuck.png").convert("RGBA"))
            self.assertLess(int((accessory[:, :, 3] > 8).sum()), 100)
            self.assertGreater(int((outer[:, :, 3] > 8).sum()), 10000)
            before = repair._union_alpha(source, "jump_tuck")
            after = repair._union_alpha(output, "jump_tuck")
            np.testing.assert_array_equal(before, after)


if __name__ == "__main__":
    unittest.main()
