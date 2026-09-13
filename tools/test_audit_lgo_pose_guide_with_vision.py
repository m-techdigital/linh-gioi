import unittest

from audit_lgo_pose_guide_with_vision import compare_pose


class VisionPoseGuideAuditTests(unittest.TestCase):
    def test_selects_near_far_orientation_with_smallest_joint_error(self):
        pose = {
            "pose": "idle",
            "landmarks": [
                {"id": "near_shoulder", "xy": [10, 20], "reviewRadiusPx": 5},
                {"id": "far_shoulder", "xy": [90, 20], "reviewRadiusPx": 5},
                {"id": "neck", "xy": [50, 10], "reviewRadiusPx": 5},
            ],
        }
        vision = {"joints": {
            "right_shoulder": {"x": 11, "y": 20, "confidence": 0.9},
            "left_shoulder": {"x": 89, "y": 20, "confidence": 0.9},
            "neck": {"x": 50, "y": 12, "confidence": 0.9},
        }}

        result = compare_pose(pose, vision)

        self.assertEqual(result["nearVisionSide"], "right")
        self.assertTrue(all(item["withinManualReviewRadius"] for item in result["landmarks"]))
        self.assertTrue(all(item["withinTwoPixelBindTolerance"] for item in result["landmarks"]))

    def test_marks_low_confidence_points_unusable_even_when_coordinates_match(self):
        pose = {"pose": "idle", "landmarks": [
            {"id": "near_shoulder", "xy": [10, 20], "reviewRadiusPx": 5},
            {"id": "far_shoulder", "xy": [90, 20], "reviewRadiusPx": 5},
        ]}
        vision = {"joints": {
            "right_shoulder": {"x": 10, "y": 20, "confidence": 0.0},
            "left_shoulder": {"x": 90, "y": 20, "confidence": 0.0},
        }}

        result = compare_pose(pose, vision)

        self.assertTrue(all(not item["usableForComparison"] for item in result["landmarks"]))
        self.assertTrue(all(not item["withinTwoPixelBindTolerance"] for item in result["landmarks"]))


if __name__ == "__main__":
    unittest.main()
