import json
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

import validate_lgo_owner_review_catalog as validator


class OwnerReviewCatalogValidatorTests(unittest.TestCase):
    def test_current_catalog_contains_visually_audited_source_pose_classes(self):
        self.assertEqual(validator.main(), 0)



    def test_owner_facing_closeup_sheets_must_be_independent_per_class(self):
        duplicate = {
            "kiem": ("manifest-a.json", "shared.jpg"),
            "phap": ("manifest-b.json", "shared.jpg"),
            "co": ("manifest-c.json", "co.jpg"),
            "linh": ("manifest-d.json", "linh.jpg"),
        }
        with patch.dict(validator.PLAYER_EVIDENCE, duplicate, clear=True):
            self.assertEqual(validator.main(), 1)

    def test_owner_facing_non_vo_class_requires_four_pack_paths(self):
        with patch.dict(validator.launcher.PACK_SUFFIXES, {"phap": ("-source-pose-review-deterministic-v7/pack", None, None, None)}):
            self.assertIn("male/female Lv1/Lv10", validator.validate_exposed_pack_matrix("phap"))

    def test_owner_facing_pack_matrix_requires_existing_atlas(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            with patch.object(validator, "ROOT", root), patch.dict(validator.launcher.PACK_SUFFIXES, {"phap": ("-missing-a/pack", "-missing-b/pack", "-missing-c/pack", "-missing-d/pack")}):
                self.assertIn("missing owner-facing source-pose pack", validator.validate_exposed_pack_matrix("phap"))

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

    def test_player_evidence_rejects_jump_scale_that_exceeds_idle_height(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            manifest = root / "registered-manifest.json"
            closeup = root / "closeup.jpg"
            manifest.write_text(json.dumps({
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "frames": 190,
                "errors": [],
                "poseReviewFullLevelsVerified": [1, 10],
                "poseReviewMixedVerified": True,
                "maxBodyVariants": 1,
                "actorFrameMetrics": [
                    {"file": "01-male-idle.png", "screenHeightRatio": 0.20, "rootScaleX": 1.0, "rootScaleY": 1.0},
                    {"file": "18-male-jump.png", "screenHeightRatio": 0.31, "rootScaleX": 1.0, "rootScaleY": 1.0},
                    {"file": "90-female-idle.png", "screenHeightRatio": 0.20, "rootScaleX": 1.0, "rootScaleY": 1.0},
                    {"file": "108-female-jump.png", "screenHeightRatio": 0.20, "rootScaleX": 1.0, "rootScaleY": 1.0},
                ],
            }))
            closeup.write_bytes(b"jpg")
            with patch.dict(validator.PLAYER_EVIDENCE, {"kiem": (str(manifest), str(closeup))}):
                self.assertIn("jump scale", validator.validate_player_evidence("kiem"))


    def test_owner_catalog_pack_paths_must_be_launchable_with_real_launcher_guard(self):
        for class_id in validator.EXPECTED_CLASSES:
            with self.subTest(class_id=class_id):
                self.assertIsNone(validator.validate_exposed_pack_matrix(class_id))
                validator.launcher.class_pack_paths(validator.ROOT, class_id)


    def test_held_out_class_must_not_be_counted_as_owner_review_evidence(self):
        self.assertIn("phap", validator.HELD_OUT_CLASSES)
        self.assertNotIn("phap", validator.EXPECTED_CLASSES)
        self.assertNotIn("phap", validator.launcher.CLASSES)
        self.assertNotIn("phap", validator.active_player_evidence())


    def test_player_evidence_requires_closeup_provenance_manifest(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            manifest = root / "registered-manifest.json"
            closeup = root / "closeup.jpg"
            manifest.write_text(json.dumps({
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "frames": 190,
                "errors": [],
                "poseReviewFullLevelsVerified": [1, 10],
                "poseReviewMixedVerified": True,
                "maxBodyVariants": 1,
                "actorFrameMetrics": [
                    {"file": "01-male-idle.png", "screenHeightRatio": 0.20, "rootScaleX": 1.0, "rootScaleY": 1.0},
                    {"file": "19-male-jump.png", "screenHeightRatio": 0.18, "rootScaleX": 1.0, "rootScaleY": 1.0},
                    {"file": "90-female-idle.png", "screenHeightRatio": 0.20, "rootScaleX": 1.0, "rootScaleY": 1.0},
                    {"file": "108-female-jump.png", "screenHeightRatio": 0.18, "rootScaleX": 1.0, "rootScaleY": 1.0},
                ],
            }))
            closeup.write_bytes(b"jpg")
            with patch.dict(validator.PLAYER_EVIDENCE, {"kiem": (str(manifest), str(closeup))}):
                self.assertIn("close-up provenance", validator.validate_player_evidence("kiem"))



if __name__ == "__main__":
    unittest.main()
