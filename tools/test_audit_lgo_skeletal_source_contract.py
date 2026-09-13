import json
import tempfile
import unittest
from pathlib import Path

from audit_lgo_skeletal_source_contract import audit_source_contract
from test_audit_lgo_skeletal_blueprint_package import write_cutout


class SkeletalSourceContractTests(unittest.TestCase):
    def _fixture(self, mode="RGBA", size=(1024, 1536), alpha=0, ownership=None):
        temporary = tempfile.TemporaryDirectory()
        root = Path(temporary.name)
        image_path = root / "upper.png"
        write_cutout(image_path, mode=mode, size=size, full_opaque=alpha == 255)
        manifest = {
            "canvas": {"width": 1024, "height": 1536, "originX": 512, "groundY": 1484},
            "allowedSlots": ["upper", "lower", "footwear", "waist", "rigid_hand_item", "body"],
            "components": [
                {
                    "id": "upper_torso",
                    "path": image_path.name,
                    "slot": "upper",
                    "ownership": ownership or ["torso"],
                }
            ],
        }
        manifest_path = root / "manifest.json"
        manifest_path.write_text(json.dumps(manifest), encoding="utf-8")
        return temporary, manifest_path

    def test_accepts_registered_rgba_component_with_transparency(self):
        temporary, manifest = self._fixture()
        self.addCleanup(temporary.cleanup)
        report = audit_source_contract(manifest)
        self.assertEqual("PASS", report["status"])
        self.assertEqual(0, report["failureCount"])

    def test_rejects_rgb_checkerboard_even_when_dimensions_match(self):
        temporary, manifest = self._fixture(mode="RGB")
        self.addCleanup(temporary.cleanup)
        report = audit_source_contract(manifest)
        self.assertEqual("REJECT_SOURCE_CONTRACT", report["status"])
        self.assertIn("MISSING_ALPHA_CHANNEL", report["components"][0]["failures"])

    def test_rejects_fully_opaque_alpha(self):
        temporary, manifest = self._fixture(alpha=255)
        self.addCleanup(temporary.cleanup)
        report = audit_source_contract(manifest)
        self.assertIn("NO_TRANSPARENT_PIXELS", report["components"][0]["failures"])

    def test_rejects_wrong_canvas_and_cross_slot_ownership(self):
        temporary, manifest = self._fixture(size=(512, 512), ownership=["torso", "waist"])
        self.addCleanup(temporary.cleanup)
        report = audit_source_contract(manifest)
        failures = report["components"][0]["failures"]
        self.assertIn("CANVAS_DIMENSIONS_MISMATCH", failures)
        self.assertIn("OWNERSHIP_CROSSES_SLOT_BOUNDARY", failures)


if __name__ == "__main__":
    unittest.main()
