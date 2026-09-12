import unittest

import numpy as np

from normalize_lgo_source_pose_pack import scale_rgba_about


class NormalizeSourcePosePackTests(unittest.TestCase):
    def test_scales_opaque_pixels_about_registered_pivot_without_changing_canvas(self):
        image = np.zeros((12, 12, 4), dtype=np.uint8)
        image[2:10, 2:10] = (20, 40, 60, 255)

        scaled = scale_rgba_about(image, 0.5, (6, 6))

        self.assertEqual((12, 12, 4), scaled.shape)
        self.assertEqual((4, 4, 8, 8), alpha_bounds(scaled))
        self.assertEqual(255, int(scaled[6, 6, 3]))
        self.assertLessEqual(int(np.abs(scaled[6, 6, :3].astype(int) - (20, 40, 60)).max()), 2)

    def test_identity_returns_byte_identical_copy(self):
        image = np.arange(8 * 9 * 4, dtype=np.uint8).reshape((8, 9, 4))
        scaled = scale_rgba_about(image, 1.0, (3, 4))
        self.assertTrue(np.array_equal(image, scaled))
        self.assertIsNot(image, scaled)


def alpha_bounds(image):
    ys, xs = np.nonzero(image[:, :, 3] > 127)
    return int(xs.min()), int(ys.min()), int(xs.max() + 1), int(ys.max() + 1)


if __name__ == '__main__':
    unittest.main()
