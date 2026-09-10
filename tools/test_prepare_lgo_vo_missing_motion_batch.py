import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

from prepare_lgo_vo_missing_motion_batch import (
    analyze_grid,
    border_connected_checker_mask,
    normalize_grid,
)


class MissingMotionBatchTests(unittest.TestCase):
    def test_border_flood_preserves_enclosed_light_costume_pixels(self):
        image = Image.new("RGB", (64, 64))
        pixels = image.load()
        for y in range(64):
            for x in range(64):
                value = 254 if (x // 8 + y // 8) % 2 == 0 else 211
                pixels[x, y] = (value, value, value)
        draw = ImageDraw.Draw(image)
        draw.rectangle((18, 12, 45, 55), fill=(20, 20, 20))
        draw.rectangle((22, 16, 41, 51), fill=(250, 248, 240))

        background = border_connected_checker_mask(image)

        self.assertTrue(background[0])
        self.assertFalse(background[32 * 64 + 32])

    def test_grid_rejects_foreground_touching_safe_margin(self):
        image = Image.new("RGBA", (200, 100), (0, 0, 0, 0))
        draw = ImageDraw.Draw(image)
        draw.rectangle((20, 15, 94, 85), fill=(20, 20, 20, 255))
        draw.rectangle((120, 15, 180, 85), fill=(20, 20, 20, 255))

        report = analyze_grid(image, columns=2, rows=1, safe_margin=10)

        self.assertFalse(report[0]["safe"])
        self.assertIn("right", report[0]["touchedMargins"])
        self.assertTrue(report[1]["safe"])

    def test_normalize_grid_fits_every_pose_inside_one_batch_margin(self):
        image = Image.new("RGBA", (200, 100), (0, 0, 0, 0))
        draw = ImageDraw.Draw(image)
        draw.rectangle((2, 4, 96, 94), fill=(20, 20, 20, 255))
        draw.rectangle((104, 2, 198, 98), fill=(20, 20, 20, 255))

        normalized = normalize_grid(image, columns=2, rows=1, safe_margin=12)
        report = analyze_grid(normalized, columns=2, rows=1, safe_margin=12)

        self.assertTrue(all(cell["safe"] for cell in report))


if __name__ == "__main__":
    unittest.main()
