import unittest

from build_lgo_rigid_bind_design_profile import (
    CANVAS_HEIGHT,
    CANVAS_WIDTH,
    GROUND_Y,
    REQUIRED_JOINTS,
    profile_config,
    project_point,
)


class RigidBindDesignProfileTests(unittest.TestCase):
    def test_both_bodies_register_to_canonical_root_and_ground(self):
        for body in profile_config()["bodies"].values():
            root = project_point(body, body["landmarks"]["root"])
            pelvis = project_point(body, body["landmarks"]["pelvis"])
            ground = project_point(body, [body["landmarks"]["pelvis"][0], body["sourceGroundY"]])
            self.assertEqual(root, (CANVAS_WIDTH // 2, GROUND_Y))
            self.assertEqual(pelvis[0], CANVAS_WIDTH // 2)
            self.assertEqual(ground[1], GROUND_Y)

    def test_both_bodies_have_complete_reusable_skeleton_semantics(self):
        for body in profile_config()["bodies"].values():
            self.assertEqual(set(body["landmarks"]), set(REQUIRED_JOINTS))

    def test_projected_landmarks_stay_inside_common_canvas(self):
        for body in profile_config()["bodies"].values():
            for point in body["landmarks"].values():
                x, y = project_point(body, point)
                self.assertGreaterEqual(x, 0)
                self.assertLess(x, CANVAS_WIDTH)
                self.assertGreaterEqual(y, 0)
                self.assertLess(y, CANVAS_HEIGHT)


if __name__ == "__main__":
    unittest.main()
