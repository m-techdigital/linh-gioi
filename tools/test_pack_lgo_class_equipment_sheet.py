import unittest

import numpy as np
from PIL import Image, ImageDraw

from pack_lgo_class_equipment_sheet import chroma_crop, chroma_rig_component, split_outer_component


class ChromaCropTests(unittest.TestCase):
    def make_sheet(self):
        sheet = Image.new("RGB", (400, 1000), (255, 0, 255))
        draw = ImageDraw.Draw(sheet)
        # Intended item in the middle of the test cell.
        draw.rectangle((32, 28, 67, 70), fill=(15, 35, 55))
        # Large fragments leaking across the nominal top/bottom row boundaries.
        draw.rectangle((8, 0, 90, 8), fill=(20, 40, 60))
        draw.rectangle((5, 92, 94, 99), fill=(20, 40, 60))
        return sheet

    def test_discards_adjacent_row_bleed(self):
        crop = chroma_crop(self.make_sheet(), 0, 0)
        alpha = np.asarray(crop)[:, :, 3]
        self.assertEqual((36, 43), (crop.width, crop.height))
        self.assertTrue(np.all(alpha[[0, -1], :] > 0))

    def test_discards_nearby_detached_fragment_from_next_row(self):
        sheet = Image.new("RGB", (400, 1000), (255, 0, 255))
        draw = ImageDraw.Draw(sheet)
        draw.rectangle((32, 25, 67, 68), fill=(15, 35, 55))
        draw.rectangle((40, 75, 60, 88), fill=(20, 40, 60))
        crop = chroma_crop(sheet, 0, 0)
        self.assertEqual(44, crop.height)

    def test_keeps_two_central_components_for_paired_slot(self):
        sheet = Image.new("RGB", (400, 1000), (255, 0, 255))
        draw = ImageDraw.Draw(sheet)
        draw.rectangle((18, 30, 38, 72), fill=(10, 25, 45))
        draw.rectangle((62, 30, 82, 72), fill=(10, 25, 45))
        draw.rectangle((5, 0, 95, 9), fill=(10, 25, 45))
        crop = chroma_crop(sheet, 0, 0, expected_components=2)
        alpha = np.asarray(crop)[:, :, 3]
        columns = np.flatnonzero(np.any(alpha > 8, axis=0))
        self.assertGreater(np.diff(columns).max(), 10)

    def test_paired_slot_does_not_keep_tiny_neighbouring_item(self):
        sheet = Image.new("RGB", (400, 1000), (255, 0, 255))
        draw = ImageDraw.Draw(sheet)
        draw.rectangle((20, 28, 80, 72), fill=(10, 25, 45))
        draw.rectangle((42, 90, 57, 99), fill=(10, 25, 45))
        crop = chroma_crop(sheet, 0, 0, expected_components=2)
        self.assertEqual(45, crop.height)


class OuterComponentTests(unittest.TestCase):
    def test_splits_rect_and_world_width_without_changing_union(self):
        source = {"itemId": "phap-lv001-male-outer_top", "side": "center",
                  "bone": "torso-hips", "worldX": 0.3, "worldW": 0.9,
                  "atlasRect": [10, 20, 101, 50]}
        parts = split_outer_component(source)
        self.assertEqual(["left-upper-arm", "torso-hips", "right-upper-arm"],
                         [part["bone"] for part in parts])
        self.assertEqual(101, sum(part["atlasRect"][2] for part in parts))
        self.assertAlmostEqual(0.9, sum(part["worldW"] for part in parts))
        self.assertEqual(10, parts[0]["atlasRect"][0])
        self.assertEqual(111, parts[-1]["atlasRect"][0] + parts[-1]["atlasRect"][2])


class RigComponentSheetTests(unittest.TestCase):
    def test_extracts_exact_five_by_three_cell(self):
        sheet = Image.new("RGB", (500, 300), (255, 0, 255))
        draw = ImageDraw.Draw(sheet)
        draw.rectangle((310, 210, 360, 270), fill=(10, 30, 50))
        crop = chroma_rig_component(sheet, 13)
        self.assertEqual((51, 61), crop.size)

    def test_discards_generation_specks_inside_rig_cell(self):
        sheet = Image.new("RGB", (500, 300), (255, 0, 255))
        draw = ImageDraw.Draw(sheet)
        draw.rectangle((310, 210, 360, 270), fill=(10, 30, 50))
        draw.rectangle((390, 205, 393, 208), fill=(10, 30, 50))
        crop = chroma_rig_component(sheet, 13)
        self.assertEqual((51, 61), crop.size)

    def test_keeps_intentional_detached_rig_detail(self):
        sheet = Image.new("RGB", (500, 300), (255, 0, 255))
        draw = ImageDraw.Draw(sheet)
        draw.rectangle((310, 210, 360, 270), fill=(10, 30, 50))
        draw.rectangle((375, 230, 381, 236), fill=(10, 30, 50))
        crop = chroma_rig_component(sheet, 13)
        self.assertEqual((72, 61), crop.size)

    def test_discards_component_bleeding_from_adjacent_rig_cell(self):
        sheet = Image.new("RGB", (500, 300), (255, 0, 255))
        draw = ImageDraw.Draw(sheet)
        draw.rectangle((110, 20, 160, 80), fill=(10, 30, 50))
        draw.rectangle((100, 35, 108, 65), fill=(10, 30, 50))
        crop = chroma_rig_component(sheet, 1)
        self.assertEqual((51, 61), crop.size)


if __name__ == "__main__":
    unittest.main()
