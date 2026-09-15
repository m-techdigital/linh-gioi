import unittest

from tools.probe_lgo_blender_rigid_capabilities import (
    capability_probe_spec,
    decide_capability_status,
)


class BlenderRigidCapabilityProbeTests(unittest.TestCase):
    def test_probe_requires_rigid_bone_parent_and_transparent_render(self) -> None:
        spec = capability_probe_spec()

        self.assertEqual(spec["parts"]["Torso"], "Chest")
        self.assertEqual(spec["parts"]["UpperArmNear"], "UpperArmNear")
        self.assertEqual(spec["parts"]["LowerArmNear"], "LowerArmNear")
        self.assertTrue(spec["requiresTransparentFilm"])
        self.assertEqual(spec["allowedTransformChannels"], ["location", "rotation_euler"])
        self.assertEqual(spec["forbidden"], ["ARMATURE_MODIFIER", "VERTEX_GROUP", "SHAPE_KEY", "SCALE_KEY"])

    def test_complete_reopen_audit_passes(self) -> None:
        audit = {
            "freshProcessReopen": True,
            "filmTransparent": True,
            "renderChannels": 4,
            "alphaMin": 0.0,
            "alphaMax": 1.0,
            "boneParentFailures": [],
            "modifierFailures": [],
            "vertexGroupFailures": [],
            "shapeKeyFailures": [],
            "scaleFailures": [],
            "scaleCurveFailures": [],
        }

        self.assertEqual(decide_capability_status(audit), "BLENDER_RIGID_RGBA_REOPEN_PASS")

    def test_opaque_or_deformed_probe_is_rejected(self) -> None:
        opaque = {
            "freshProcessReopen": True,
            "filmTransparent": True,
            "renderChannels": 4,
            "alphaMin": 1.0,
            "alphaMax": 1.0,
            "boneParentFailures": [],
            "modifierFailures": [],
            "vertexGroupFailures": [],
            "shapeKeyFailures": [],
            "scaleFailures": [],
            "scaleCurveFailures": [],
        }
        deformed = {**opaque, "alphaMin": 0.0, "modifierFailures": ["UpperArmNear"]}

        self.assertEqual(decide_capability_status(opaque), "BLENDER_RIGID_RGBA_REOPEN_FAIL")
        self.assertEqual(decide_capability_status(deformed), "BLENDER_RIGID_RGBA_REOPEN_FAIL")


if __name__ == "__main__":
    unittest.main()
