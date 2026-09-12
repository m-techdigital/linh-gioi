import unittest
import numpy as np

from repartition_lgo_source_pose_slots import choose_accessory, repartition_pose, SLOTS


class ReassignSourcePoseAccessoryTests(unittest.TestCase):
    def test_reassigns_broad_accessory_to_outer_without_changing_union(self):
        images = {slot: np.zeros((128, 128, 4), dtype=np.uint8) for slot in SLOTS}
        images['class_accessory'][5:105, 5:105] = (20, 40, 200, 255)
        images['class_accessory'][28:34, 29:35] = 0
        images['waist_belt'][28:34, 29:35] = (220, 180, 30, 255)
        anchors = {}
        for index, slot in enumerate(slot for slot in SLOTS if slot not in ('main_weapon', 'class_accessory')):
            anchor = np.zeros((128, 128), dtype=bool)
            anchor[110 + index, 110 + index] = True
            anchors[slot] = anchor
        anchors['outer_top'][5:70, 5:105] = True
        anchors['waist_belt'][28:34, 29:35] = True
        result = repartition_pose(images, anchors)
        self.assertEqual(0, np.count_nonzero(result['class_accessory'][5:25, :, 3]))
        self.assertGreater(np.count_nonzero(result['class_accessory'][:, :, 3]), 0)
        self.assertGreater(np.count_nonzero(result['outer_top'][:, :, 3]), 5000)
        before = np.logical_or.reduce([images[slot][:, :, 3] > 8 for slot in SLOTS])
        after = np.logical_or.reduce([result[slot][:, :, 3] > 8 for slot in SLOTS])
        np.testing.assert_array_equal(before, after)

    def test_component_choice_is_bounded_and_near_anchor(self):
        mask = np.zeros((300, 300), dtype=bool)
        mask[20:140, 20:140] = True
        mask[170:185, 170:190] = True
        anchor = np.zeros_like(mask); anchor[160:200, 160:205] = True
        chosen = choose_accessory(mask, anchor)
        self.assertEqual(300, int(chosen.sum()))


if __name__ == '__main__':
    unittest.main()
