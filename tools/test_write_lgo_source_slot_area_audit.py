#!/usr/bin/env python3.12
"""Regression tests for source-pose slot ownership area audit."""
from __future__ import annotations

import tempfile
import unittest
from pathlib import Path

from PIL import Image

import write_lgo_source_slot_area_audit as audit


class WriteLgoSourceSlotAreaAuditTests(unittest.TestCase):
    def test_audit_reports_slot_ratios_and_threshold_flags(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            surface = Path(directory) / "surface"
            for slot in audit.SLOTS:
                for pose in audit.POSES:
                    target = surface / slot / f"{pose}.png"
                    target.parent.mkdir(parents=True, exist_ok=True)
                    image = Image.new("RGBA", (100, 100), (0, 0, 0, 0))
                    if slot == "outer_top":
                        for y in range(60):
                            for x in range(100):
                                image.putpixel((x, y), (30, 90, 180, 255))
                    elif slot == "class_accessory":
                        for y in range(60, 85):
                            for x in range(100):
                                image.putpixel((x, y), (220, 180, 40, 255))
                    elif slot == "head_hair":
                        for y in range(85, 95):
                            for x in range(100):
                                image.putpixel((x, y), (20, 20, 20, 255))
                    image.save(target)

            payload = audit.audit_surface(surface)

            jump = next(row for row in payload["rows"] if row["pose"] == "jump_tuck")
            self.assertAlmostEqual(2500 / 9500, jump["ratios"]["class_accessory"], places=3)
            self.assertIn("class_accessory", jump["flags"])
            self.assertIn("outer_top", jump["flags"])
            self.assertNotIn("head_hair", jump["flags"])


if __name__ == "__main__":
    unittest.main()
