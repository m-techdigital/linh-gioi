import json
import tempfile
import unittest
from pathlib import Path

import capture_lgo_character_hub as capture


class CaptureLgoCharacterHubTests(unittest.TestCase):
    def test_profiles_use_three_real_target_viewports(self) -> None:
        self.assertEqual((1280, 720), capture.PROFILES["pc"])
        self.assertEqual((1024, 768), capture.PROFILES["tablet"])
        self.assertEqual((1600, 720), capture.PROFILES["mobile"])
        self.assertEqual(3, len(set(capture.PROFILES.values())))

    def test_capture_requires_data_only_potential_frames_for_all_five_classes(self) -> None:
        expected = tuple(
            f"potential-{class_id}-{state}.png"
            for class_id in ("vo", "kiem", "phap", "co", "linh")
            for state in ("default", "selected")
        )
        for frame in expected:
            self.assertIn(frame, capture.REQUIRED_FRAMES)

    def test_player_command_uses_target_resolution_and_internal_capture(self) -> None:
        command = capture.build_player_command(
            Path("/tmp/Unity"), Path("/tmp/evidence"), "mobile", 1600, 720
        )
        self.assertIn("-screen-width", command)
        self.assertEqual("1600", command[command.index("-screen-width") + 1])
        self.assertEqual("720", command[command.index("-screen-height") + 1])
        self.assertIn("--lgo-map01a-inventory-tabs-capture", command)
        self.assertNotIn("osascript", command)

    def test_manifest_must_match_profile_resolution_and_complete_frames(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            out = Path(temp)
            for frame in capture.REQUIRED_FRAMES:
                (out / frame).write_bytes(b"png")
            manifest = {
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "captureScope": "map01a-inventory-tabs",
                "usesOsMouseOrKeyboard": False,
                "width": 1280,
                "height": 720,
                "frames": list(capture.REQUIRED_FRAMES),
                "potentialClassProfiles": list(capture.POTENTIAL_CLASS_IDS),
                "classSwitchScope": "character-hub-data-only-no-renderer-change",
            }
            self.assertEqual([], capture.validate_manifest(manifest, out, "pc"))
            manifest["width"] = 1024
            self.assertIn("VIEWPORT_MISMATCH", capture.validate_manifest(manifest, out, "pc"))
            manifest["width"] = 1280
            manifest["potentialClassProfiles"] = ["vo"]
            self.assertIn("POTENTIAL_CLASS_PROFILE_MISMATCH", capture.validate_manifest(manifest, out, "pc"))
            manifest["potentialClassProfiles"] = list(capture.POTENTIAL_CLASS_IDS)
            manifest["classSwitchScope"] = "renderer-class-switch"
            self.assertIn("CLASS_SWITCH_SCOPE_INVALID", capture.validate_manifest(manifest, out, "pc"))

    def test_manifest_rejects_missing_frame_and_os_input(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            out = Path(temp)
            manifest = {
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "captureScope": "map01a-inventory-tabs",
                "usesOsMouseOrKeyboard": True,
                "width": 1024,
                "height": 768,
                "frames": list(capture.REQUIRED_FRAMES),
                "potentialClassProfiles": list(capture.POTENTIAL_CLASS_IDS),
                "classSwitchScope": "character-hub-data-only-no-renderer-change",
            }
            errors = capture.validate_manifest(manifest, out, "tablet")
            self.assertIn("OS_INPUT_USED", errors)
            self.assertTrue(any(error.startswith("MISSING_FRAME:") for error in errors), errors)


if __name__ == "__main__":
    unittest.main()
