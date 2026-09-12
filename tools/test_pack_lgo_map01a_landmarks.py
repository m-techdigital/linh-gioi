import json
import tempfile
import unittest
from pathlib import Path
from PIL import Image, ImageDraw
from pack_lgo_map01a_landmarks import pack


class LandmarkPackingTests(unittest.TestCase):
    def make_source(self, path):
        image = Image.new('RGBA', (1536, 1024))
        draw = ImageDraw.Draw(image)
        for x, y in ((40, 60), (580, 60), (1060, 60), (40, 600), (580, 600), (1060, 600)):
            draw.rectangle((x, y, x + 400, y + 300), fill=(40, 80, 100, 255))
        # The hall roof belongs to the first object but crosses the old 512 boundary.
        draw.rectangle((430, 80, 545, 130), fill=(220, 170, 70, 255))
        image.save(path)
        return image

    def test_implicit_grid_rejects_a_building_cut_at_cell_boundary(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory); source = root / 'source.png'; self.make_source(source)
            with self.assertRaisesRegex(ValueError, 'boundary'):
                pack(source, root / 'out')

    def test_authored_regions_keep_complete_roof_and_isolate_merchant(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory); source = root / 'source.png'; image = self.make_source(source)
            regions = [[0, 0, 560, 510], [560, 0, 1024, 510], [1024, 0, 1536, 510],
                       [0, 512, 560, 1024], [560, 512, 1024, 1024], [1024, 512, 1536, 1024]]
            pack(source, root / 'out', regions=regions)
            manifest = json.loads((root / 'out/landmarks-layout.json').read_text())
            hall, merchant = manifest['parts'][:2]
            self.assertEqual(hall['sourceRect'], regions[0])
            self.assertGreater(hall['sourceContentRect'][2], 512)
            self.assertGreaterEqual(merchant['sourceContentRect'][0], 580)
            self.assertEqual(manifest['sourcePixelCoverage'], 1)
            with Image.open(root / 'out/landmarks-atlas.png') as atlas:
                def has_roof_gold(part):
                    x, y, w, h = (part[key] for key in ('x', 'y', 'w', 'h'))
                    pixels = atlas.crop((x, atlas.height-y-h, x+w, atlas.height-y)).convert('RGBA').getdata()
                    return any(r > 180 and g > 130 and b < 110 and a > 200 for r, g, b, a in pixels)
                self.assertTrue(has_roof_gold(hall), 'The crossing roof must survive into the packed pixels')
                self.assertFalse(has_roof_gold(merchant), 'The merchant must not inherit its neighbour’s roof')

    def test_regions_must_not_overlap_or_drop_source_content(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory); source = root / 'source.png'; self.make_source(source)
            with self.assertRaisesRegex(ValueError, 'overlap|coverage|boundary'):
                pack(source, root / 'out', regions=[[0, 0, 550, 512]] * 6)


if __name__ == '__main__':
    unittest.main()
