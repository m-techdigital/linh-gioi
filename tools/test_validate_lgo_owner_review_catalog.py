import json
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

import validate_lgo_owner_review_catalog as validator


class OwnerReviewCatalogValidatorTests(unittest.TestCase):
    def test_current_catalog_contains_visually_audited_source_pose_classes(self):
        self.assertEqual(validator.main(), 0)

    def test_player_evidence_rejects_manifest_errors(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            manifest = root / "registered-manifest.json"
            closeup = root / "closeup.jpg"
            manifest.write_text(json.dumps({
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "frames": 190,
                "errors": ["bad"],
                "poseReviewFullLevelsVerified": [1, 10],
                "poseReviewMixedVerified": True,
                "maxBodyVariants": 1,
            }))
            closeup.write_bytes(b"jpg")
            with patch.dict(validator.PLAYER_EVIDENCE, {"kiem": (str(manifest), str(closeup))}):
                self.assertIn("errors", validator.validate_player_evidence("kiem"))

    def test_player_evidence_requires_closeup_sheet(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            manifest = root / "registered-manifest.json"
            manifest.write_text(json.dumps({
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "frames": 190,
                "errors": [],
                "poseReviewFullLevelsVerified": [1, 10],
                "poseReviewMixedVerified": True,
                "maxBodyVariants": 1,
            }))
            with patch.dict(validator.PLAYER_EVIDENCE, {"kiem": (str(manifest), str(root / "missing.jpg"))}):
                self.assertIn("missing close-up", validator.validate_player_evidence("kiem"))


if __name__ == "__main__":
    unittest.main()
