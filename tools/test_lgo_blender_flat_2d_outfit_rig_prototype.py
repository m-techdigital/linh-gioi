import unittest

from lgo_blender_flat_2d_outfit_rig_prototype import validate_prototype_report


class BlenderFlat2DOutfitRigPrototypeTests(unittest.TestCase):
    def test_report_requires_editable_source_two_upper_items_and_detachable_parts(self):
        report = self._passing_report()
        result = validate_prototype_report(report)
        self.assertEqual("SOURCE_READY_FOR_PLAYER_PROBE", result["status"])
        self.assertEqual([], result["failures"])

    def test_second_item_must_reuse_same_rig_and_not_use_pose_specific_offsets(self):
        report = self._passing_report()
        report["outfitItems"][1]["usesSameRigAs"] = "other_rig"
        report["outfitItems"][1]["perPoseOffsets"] = {"jump_tuck": [3, 0]}
        failures = validate_prototype_report(report)["failures"]
        self.assertIn("SECOND_ITEM_DOES_NOT_REUSE_FIRST_RIG", failures)
        self.assertIn("ITEM_USES_PER_POSE_OFFSETS", failures)

    def test_rejects_missing_sleeved_upper_or_detachable_belt_guard(self):
        report = self._passing_report()
        report["outfitItems"] = report["outfitItems"][:1]
        report["detachableSlots"] = ["waist_belt"]
        failures = validate_prototype_report(report)["failures"]
        self.assertIn("SECOND_UPPER_ITEM_MISSING", failures)
        self.assertIn("DETACHABLE_SHOULDER_CHEST_GUARD_MISSING", failures)

    @staticmethod
    def _passing_report():
        return {
            "renderPath": "BLENDER_FLAT_CARD_RIG_TO_UNITY_FBX_PLAYER",
            "source": {"blendExists": True, "fbxExists": True, "editableRig": True},
            "armature": {"boneCount": 16, "usesBoneScaleAnimation": False},
            "outfitItems": [
                {"itemId": "phap_lv001_outer_top_sleeved_a", "family": "outer_top", "hasSleeves": True,
                 "usesSameRigAs": "self", "perPoseOffsets": {}},
                {"itemId": "phap_lv001_outer_top_sleeved_b", "family": "outer_top", "hasSleeves": True,
                 "usesSameRigAs": "phap_lv001_outer_top_sleeved_a", "perPoseOffsets": {}},
            ],
            "detachableSlots": ["waist_belt", "shoulder_chest_guard"],
            "actions": ["idle", "run", "jump", "attack", "return_to_idle"],
            "runtimePromotionAllowed": False,
        }


if __name__ == "__main__":
    unittest.main()
