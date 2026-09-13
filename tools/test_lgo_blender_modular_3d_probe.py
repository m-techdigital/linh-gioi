import unittest

from lgo_blender_modular_3d_probe import probe_contract, validate_probe_report


class Modular3DProbeContractTests(unittest.TestCase):
    def test_contract_covers_same_benchmark_units_and_actions(self):
        contract = probe_contract()
        self.assertEqual(
            {"upper", "lower", "footwear", "waist", "rigid_hand_item"},
            set(contract["equipmentSlots"]),
        )
        self.assertEqual(
            {"idle", "run", "jump", "attack", "return_to_idle"},
            set(contract["actions"]),
        )
        self.assertEqual("DIRECT_3D_RUNTIME", contract["renderPath"])

    def test_valid_report_requires_soft_weights_and_rigid_item_shape_contract(self):
        report = self._passing_report()
        result = validate_probe_report(report)
        self.assertEqual("NARROW_TECHNICAL_PASS_TOOLCHAIN_ONLY", result["status"])
        self.assertEqual([], result["failures"])

    def test_missing_action_or_sprite_export_is_rejected(self):
        report = self._passing_report()
        report["actions"].remove("attack")
        report["renderPath"] = "BLENDER_TO_SPRITE"
        failures = validate_probe_report(report)["failures"]
        self.assertIn("MISSING_REQUIRED_ACTIONS", failures)
        self.assertIn("NOT_DIRECT_3D_RUNTIME", failures)

    def test_deformed_rigid_item_or_missing_soft_weights_is_rejected(self):
        report = self._passing_report()
        report["rigidHandItem"]["hasArmatureModifier"] = True
        report["softGarments"][0]["vertexGroups"] = []
        failures = validate_probe_report(report)["failures"]
        self.assertIn("RIGID_ITEM_USES_SOFT_DEFORMATION", failures)
        self.assertIn("SOFT_GARMENT_MISSING_AUTHORED_WEIGHTS", failures)

    @staticmethod
    def _passing_report():
        contract = probe_contract()
        return {
            "renderPath": contract["renderPath"],
            "equipmentSlots": list(contract["equipmentSlots"]),
            "actions": list(contract["actions"]),
            "armature": {"boneCount": 15},
            "softGarments": [
                {"slot": "upper", "hasArmatureModifier": True, "vertexGroups": ["spine"]},
                {"slot": "lower", "hasArmatureModifier": True, "vertexGroups": ["pelvis"]},
            ],
            "rigidHandItem": {
                "slot": "rigid_hand_item",
                "parentBone": "hand.R",
                "hasArmatureModifier": False,
            },
            "artifact": {
                "glbExists": True,
                "glbBytes": 2048,
                "fbxExists": True,
                "fbxBytes": 4096,
                "animationCount": 5,
            },
        }

    def test_native_unity_fbx_export_is_required(self):
        report = self._passing_report()
        report["artifact"]["fbxExists"] = False
        report["artifact"]["fbxBytes"] = 0
        self.assertIn("UNITY_FBX_EXPORT_MISSING", validate_probe_report(report)["failures"])


if __name__ == "__main__":
    unittest.main()
