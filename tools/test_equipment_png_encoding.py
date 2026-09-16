"""Lossless PNG choices must not change RGBA or weaken the byte budget."""
import io
import unittest
from PIL import Image
import pack_lgo_equipment_item_icons as packer

class EquipmentPngEncodingTests(unittest.TestCase):
    def image(self):
        image = Image.new('RGBA', (160, 160))
        image.putdata([(i%256,(i//8)%256,(i//32)%256,(i*3)%256) for i in range(160*160)])
        return image

    def test_opt_in_encoding_keeps_pixels_and_never_grows_default(self):
        image = self.image(); original = image.tobytes()
        default = packer.encode_png(image, None)
        best = packer.encode_png(image, 'best-lossless-v1')
        self.assertLessEqual(len(best), len(default))
        with Image.open(io.BytesIO(best)) as result:
            self.assertEqual(original, result.tobytes())
        self.assertEqual(best, packer.encode_png(image, 'best-lossless-v1'))
        self.assertEqual(original, image.tobytes())

    def test_default_png_bytes_unchanged(self):
        image = self.image(); expected = io.BytesIO()
        image.save(expected, format='PNG', optimize=True)
        self.assertEqual(expected.getvalue(), packer.encode_png(image, None))

    def test_unknown_encoding_rejected(self):
        for config in (True, {}, 'lossy', 'skip-budget'):
            with self.assertRaises(ValueError): packer.encode_png(self.image(), config)
