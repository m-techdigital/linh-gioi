import json
import tempfile
import unittest
from pathlib import Path

from validate_lgo_outfit_surface_contract import validate_contract


class OutfitSurfaceContractTests(unittest.TestCase):
    def _write(self, payload):
        tmp = tempfile.TemporaryDirectory()
        path = Path(tmp.name) / "surface-contract.json"
        path.write_text(json.dumps(payload, ensure_ascii=False), encoding="utf-8")
        self.addCleanup(tmp.cleanup)
        return path

    def _base_contract(self):
        return {
            "contractId": "phap-lv1-six-pose-surface-contract-v1",
            "status": "NEED_OWNER_DECISION",
            "sourceSpaceProfile": {
                "canvas": [1024, 1536],
                "originX": 512,
                "groundY": 1484,
                "unitScale": "1.70/1536",
            },
            "poses": ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"],
            "selectedRoute": None,
            "routeOptions": {
                "SLEEVELESS_PHAP_LV1": {"bodyAuthority": "bare_arm", "requiresNativeSourceEdit": True},
                "SLEEVED_PHAP_LV1": {"bodyAuthority": "upper_arm_cloth", "requiresMasks": True},
            },
            "itemFamilies": [
                {
                    "slotId": "outer_top",
                    "familyType": "cloth_body",
                    "ownership": ["torso_cloth", "collar", "hem", "ornament"],
                    "occlusion": {"mayCover": ["torso_inner"], "mustNotCover": ["upper_arm_skin"]},
                    "sourceGate": "SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE",
                    "routeDependency": "SELECT_ROUTE_BEFORE_AUTHORING",
                },
                {
                    "slotId": "waist_belt",
                    "familyType": "part_rigid",
                    "ownership": ["waist_plate", "belt_tail"],
                    "occlusion": {"mayCover": ["waist", "pelvis_cloth"], "mustNotCover": ["upper_arm_skin"]},
                    "sourceGate": "SOURCE_REVIEW_REQUIRED",
                },
                {
                    "slotId": "shoulder_chest_guard",
                    "familyType": "part_rigid",
                    "ownership": ["shoulder_plate", "chest_strap"],
                    "occlusion": {"mayCover": ["torso_cloth", "shoulder_edge"], "mustNotCover": ["hand", "face"]},
                    "sourceGate": "SOURCE_REVIEW_REQUIRED",
                },
            ],
            "evidence": {
                "methodAudit": "docs/art/LGO-OUTFIT-PRODUCTION-METHOD-AUDIT-2026-09-13.md",
                "coverageAudit": "build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/repair-layer-audit-v7-after-all-slot-staging.json",
            },
            "runtimePromotionAllowed": False,
        }

    def test_valid_draft_contract_needs_owner_decision_when_route_unselected(self):
        path = self._write(self._base_contract())

        report = validate_contract(path)

        self.assertEqual("NEED_OWNER_DECISION", report["status"])
        self.assertEqual(0, report["failureCount"])
        self.assertIn("ROUTE_SELECTION_REQUIRED", report["decisionGates"])
        self.assertFalse(report["runtimePromotionAllowed"])

    def test_rejects_sleeved_route_without_registered_pose_overlays(self):
        payload = self._base_contract()
        payload["selectedRoute"] = "SLEEVED_PHAP_LV1"
        path = self._write(payload)

        report = validate_contract(path)

        self.assertEqual("REJECT_OUTFIT_SURFACE_CONTRACT", report["status"])
        failures = report["families"][0]["failures"]
        self.assertIn("SLEEVED_ROUTE_REQUIRES_UPPER_ARM_CLOTH_OWNERSHIP", failures)
        self.assertIn("SLEEVED_ROUTE_REQUIRES_REGISTERED_POSE_OVERLAYS", failures)

    def test_sleeved_route_accepts_whole_body_pose_overlay_declaration(self):
        payload = self._base_contract()
        payload["selectedRoute"] = "SLEEVED_PHAP_LV1"
        payload["itemFamilies"][0]["ownership"].append("upper_arm_cloth")
        payload["itemFamilies"][0]["poseSourceMode"] = "REGISTERED_FRONT_BACK_OVERLAYS"
        payload["itemFamilies"][0]["bodyAuthority"] = "WHOLE_BODY_POSE_IMAGES"
        path = self._write(payload)

        report = validate_contract(path)

        self.assertEqual("NEED_SOURCE_ARTIFACT_REVIEW", report["status"])
        self.assertEqual([], report["families"][0]["failures"])

    def test_valid_declaration_still_needs_source_artifact_review(self):
        payload = self._base_contract()
        payload["status"] = "SOURCE_CONTRACT_READY_FOR_BOARD"
        payload["selectedRoute"] = "SLEEVELESS_PHAP_LV1"
        payload["itemFamilies"][0]["routeDependency"] = "IDLE_NATIVE_SOURCE_MUST_REMOVE_SLEEVES"
        path = self._write(payload)

        report = validate_contract(path)

        self.assertEqual("NEED_SOURCE_ARTIFACT_REVIEW", report["status"])
        self.assertEqual("PASS", report["declarationStatus"])
        self.assertTrue(report["declarationValid"])
        self.assertEqual("NOT_VALIDATED", report["sourceArtifactStatus"])
        self.assertFalse(report["sourceArtifactValid"])
        self.assertEqual(0, report["failureCount"])
        self.assertFalse(report["runtimePromotionAllowed"])

    def test_pass_requires_declaration_and_accepted_source_artifact(self):
        payload = self._base_contract()
        payload["status"] = "SOURCE_CONTRACT_READY_FOR_BOARD"
        payload["selectedRoute"] = "SLEEVELESS_PHAP_LV1"
        payload["itemFamilies"][0]["routeDependency"] = "IDLE_NATIVE_SOURCE_MUST_REMOVE_SLEEVES"
        payload["sourceArtifactValidation"] = {"status": "SOURCE_ARTIFACT_VISUAL_ACCEPTED"}
        path = self._write(payload)

        report = validate_contract(path)

        self.assertEqual("PASS", report["status"])
        self.assertEqual("PASS", report["declarationStatus"])
        self.assertTrue(report["declarationValid"])
        self.assertEqual("SOURCE_ARTIFACT_VISUAL_ACCEPTED", report["sourceArtifactStatus"])
        self.assertTrue(report["sourceArtifactValid"])
        self.assertEqual(0, report["failureCount"])
        self.assertFalse(report["runtimePromotionAllowed"])

    def test_rejects_unknown_slot_and_runtime_promotion(self):
        payload = self._base_contract()
        payload["runtimePromotionAllowed"] = True
        payload["itemFamilies"].append({"slotId": "cape", "familyType": "cloth_body", "ownership": ["back_cloth"]})
        path = self._write(payload)

        report = validate_contract(path)

        self.assertEqual("REJECT_OUTFIT_SURFACE_CONTRACT", report["status"])
        self.assertIn("RUNTIME_PROMOTION_MUST_REMAIN_FALSE", report["globalFailures"])
        self.assertIn("UNKNOWN_SLOT", report["families"][-1]["failures"])


if __name__ == "__main__":
    unittest.main()
