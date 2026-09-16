"""Shared painted-item lighting; not a per-item scale, tint or alpha rewrite."""
import unittest
from PIL import Image
from prepare_lgo_item_readability import lift_shadows

class ItemReadabilityTests(unittest.TestCase):
    def test_preserves_alpha_canvas_empty_pixels_and_black_white(self):
        image = Image.new('RGBA', (384, 384))
        pixels = [(20, 40, 80, 255), (130, 70, 20, 120), (91, 64, 21, 0),
                  (0, 0, 0, 255), (255, 255, 255, 255)]
        for x, pixel in enumerate(pixels): image.putpixel((100+x, 100), pixel)
        result = lift_shadows(image)
        self.assertEqual(image.size, result.size)
        self.assertEqual(image.getchannel('A').tobytes(), result.getchannel('A').tobytes())
        self.assertEqual(pixels[2:], [result.getpixel((x, 100)) for x in range(102, 105)])
        self.assertGreater(result.getpixel((100, 100))[2], 80)
        self.assertEqual(pixels[0], image.getpixel((100, 100)), 'Never mutate the source')

    def test_uniform_gain_retains_channel_ratios_and_order(self):
        image = Image.new('RGBA', (384, 384))
        for value in range(1, 256): image.putpixel((value, 100), (value, value//2, value//4, 255))
        result = lift_shadows(image)
        previous = 0
        for value in range(1, 256):
            before = image.getpixel((value,100)); after = result.getpixel((value,100))
            self.assertTrue(after[0] >= after[1] >= after[2])
            self.assertGreaterEqual(after[0], previous); previous = after[0]
            self.assertLessEqual(abs(after[1] - before[1]*after[0]/before[0]), 1.1)
            self.assertLessEqual(abs(after[2] - before[2]*after[0]/before[0]), 1.1)

    def test_rejects_unregistered_canvas_or_missing_alpha(self):
        for mode, size in [('RGB', (384, 384)), ('RGBA', (128, 128))]:
            with self.subTest(mode=mode, size=size):
                with self.assertRaises(ValueError): lift_shadows(Image.new(mode, size))

    def test_repeat_from_original_is_pixel_deterministic(self):
        original = Image.new('RGBA', (384, 384), (32, 28, 19, 193))
        self.assertEqual(lift_shadows(original).tobytes(), lift_shadows(original).tobytes())

    def test_existing_highlights_and_metal_colors_are_not_relit(self):
        image = Image.new('RGBA', (384, 384))
        colors = [(128,64,32,255), (185,132,62,255), (210,212,214,180), (255,180,0,255)]
        for index, color in enumerate(colors): image.putpixel((100+index,100), color)
        result = lift_shadows(image)
        for index, color in enumerate(colors):
            self.assertEqual(color, result.getpixel((100+index,100)))

if __name__ == '__main__': unittest.main()
