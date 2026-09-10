import unittest

from pack_lgo_vo_lv1_30_avatar import rig_pose_profiles


class VoRigPoseProfileTests(unittest.TestCase):
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
