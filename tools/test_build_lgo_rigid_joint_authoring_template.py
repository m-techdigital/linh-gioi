import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image

from build_lgo_rigid_joint_authoring_template import build, compute_joint_contract


class RigidJointAuthoringTemplateTests(unittest.TestCase):
    def test_joint_contract_derives_rotation_invariant_cap_from_width_and_margin(self):
        joint = {
            "id": "elbow_L",
            "pivotPx": [400, 600],
            "bodyWidthPx": 40,
            "outfitWidthPx": 56,
            "clothClearancePx": 3,
            "rotationRangeDeg": [-120, 80],
            "parentBodyPart": "body.upper_arm_L",
            "childBodyPart": "body.lower_arm_L",
            "parentOutfitPart": "outfit.upper_sleeve_L",
            "childOutfitPart": "outfit.bracer_L",
            "depth": "near",
        }
        safety = {
            "outlinePx": 2,
            "filterGuardPx": 2,
            "registrationTolerancePx": 1,
            "angleSampleStepDeg": 5,
        }

        result = compute_joint_contract(joint, safety, (1024, 1536))

        self.assertEqual(25, result["bodyCapRadiusPx"])
        self.assertEqual(33, result["outfitCapRadiusPx"])
        self.assertEqual(25, result["bodyMinimumPivotEmbedPx"])
        self.assertEqual(33, result["outfitMinimumPivotEmbedPx"])
        self.assertEqual(41, result["sweepSampleCount"])
        self.assertEqual([0.390625, 0.390625], result["pivotNormalized"])
        self.assertEqual("CHILD_CAP_OVER_PARENT_UNDERLAP", result["bodyJoinRule"])

    def test_build_emits_body_only_and_each_outfit_part_removed_proof_states(self):
        profile = {
            "profileId": "fixture.v1",
            "status": "AUTHORING_PROFILE",
            "canvas": {"width": 256, "height": 384},
            "safety": {
                "outlinePx": 2,
                "filterGuardPx": 2,
                "registrationTolerancePx": 1,
                "angleSampleStepDeg": 5,
            },
            "joints": [
                {
                    "id": "knee_L",
                    "pivotPx": [100, 250],
                    "bodyWidthPx": 36,
                    "outfitWidthPx": 48,
                    "clothClearancePx": 2,
                    "rotationRangeDeg": [-145, 35],
                    "parentBodyPart": "body.thigh_L",
                    "childBodyPart": "body.shin_L",
                    "parentOutfitPart": "outfit.trouser_L",
                    "childOutfitPart": "outfit.knee_guard_L",
                    "depth": "near",
                }
            ],
        }

        with tempfile.TemporaryDirectory() as temp_dir:
            output_dir = Path(temp_dir)
            report = build(profile, output_dir)

            self.assertEqual(
                [
                    "body_only",
                    "outfit_all",
                    "without_outfit.trouser_L",
                    "without_outfit.knee_guard_L",
                ],
                report["requiredUnequipProofStates"],
            )
            self.assertTrue(report["bodyMustPassWithoutAnyEquipment"])
            with Image.open(output_dir / "joint-interface-template.png") as template:
                self.assertEqual("RGBA", template.mode)
            saved = json.loads((output_dir / "joint-authoring-report.json").read_text())
            self.assertEqual("RIGID_JOINT_AUTHORING_TEMPLATE_READY", saved["status"])

    def test_rejects_a_joint_without_one_shared_pivot(self):
        joint = {
            "id": "ankle_L",
            "bodyPivotPx": [100, 300],
            "outfitPivotPx": [102, 300],
            "bodyWidthPx": 30,
            "outfitWidthPx": 42,
            "rotationRangeDeg": [-60, 60],
        }

        with self.assertRaisesRegex(ValueError, "one shared pivotPx"):
            compute_joint_contract(
                joint,
                {
                    "outlinePx": 2,
                    "filterGuardPx": 2,
                    "registrationTolerancePx": 1,
                    "angleSampleStepDeg": 5,
                },
                (1024, 1536),
            )


if __name__ == "__main__":
    unittest.main()
