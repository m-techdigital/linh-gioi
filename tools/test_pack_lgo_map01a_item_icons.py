import tempfile
import unittest
from pathlib import Path

from PIL import Image

from pack_lgo_map01a_item_icons import remove_connected_pale_matte


class PackMap01AItemIconsTests(unittest.TestCase):
    def test_removes_only_border_connected_pale_matte(self):
        with tempfile.TemporaryDirectory() as temp:
            path = Path(temp) / "source.png"
            image = Image.new("RGBA", (7, 7), (210, 220, 226, 255))
            image.putpixel((3, 3), (225, 230, 232, 255))
            for point in ((2, 3), (3, 2), (4, 3), (3, 4)):
                image.putpixel(point, (120, 30, 24, 255))
            image.save(path)

            result = remove_connected_pale_matte(Image.open(path))

            self.assertEqual(0, result.getpixel((0, 0))[3])
            self.assertEqual(255, result.getpixel((3, 3))[3])
            self.assertEqual((120, 30, 24), result.getpixel((2, 3))[:3])


if __name__ == "__main__":
    unittest.main()
