import unittest

from PIL import Image

from pack_lgo_vo_lv1_30_avatar import GARMENT_LAYOUT, remove_connected_background, rig_pose_profiles


class VoRigPoseProfileTests(unittest.TestCase):
    def test_garment_layout_covers_multi_bone_slots(self):
        self.assertEqual(12, len(GARMENT_LAYOUT))
        self.assertEqual(2, sum(item[0] == "light_armor" for item in GARMENT_LAYOUT))
        self.assertEqual(2, sum(item[0] == "lower_garment" for item in GARMENT_LAYOUT))
        self.assertEqual(2, sum(item[0] == "arm_guard" for item in GARMENT_LAYOUT))
        self.assertEqual(2, sum(item[0] == "boots" for item in GARMENT_LAYOUT))

    def test_background_removal_keeps_isolated_component(self):
        image = Image.new("RGB", (16, 16), (30, 43, 59))
        for y in range(5, 11):
            for x in range(6, 10):
                image.putpixel((x, y), (190, 40, 30))

        result = remove_connected_background(image)

        self.assertEqual(0, result.getpixel((0, 0))[3])
        self.assertEqual(255, result.getpixel((8, 8))[3])
        self.assertEqual((6, 5, 10, 11), result.getchannel("A").getbbox())

    def test_child_rotations_are_local_to_the_parent_bone(self):
        profiles = {
            item["part"]: item["rotation"]
            for item in rig_pose_profiles()
            if item["gender"] == "male" and item["pose"] == "skill"
        }

        torso_world = profiles["torso-hips"]
        upper_arm_world = torso_world + profiles["right-upper-arm"]
        forearm_world = upper_arm_world + profiles["right-forearm-hand"]

        self.assertEqual(-13, torso_world)
        self.assertEqual(-78, upper_arm_world)
        self.assertEqual(-96, forearm_world)


if __name__ == "__main__":
    unittest.main()
