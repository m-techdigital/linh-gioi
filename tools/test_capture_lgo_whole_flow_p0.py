#!/usr/bin/env python3
import importlib
import tempfile
import time
import unittest
from pathlib import Path

try:
    capture = importlib.import_module("tools.capture_lgo_whole_flow_p0")
except ModuleNotFoundError:
    capture = importlib.import_module("capture_lgo_whole_flow_p0")


class WholeFlowP0CaptureTests(unittest.TestCase):
    def test_profiles_match_approved_viewports(self):
        self.assertEqual(capture.PROFILES, {
            "pc": (1600, 900),
            "tablet": (1024, 768),
            "mobile": (1600, 720),
        })

    def test_surface_matrix_covers_current_product_flow(self):
        self.assertEqual(tuple(capture.SCREEN_CAPTURES), (
            "entry", "server-select", "register", "password-recovery",
            "password-recovery-verify", "password-recovery-new-password",
            "character-select", "character-entry", "menu",
        ))
        self.assertEqual(capture.FLOW_COMPONENTS, ("quest", "character-hub"))
    def test_auth_commands_request_validation_evidence_without_os_input(self):
        player = Path("/tmp/LinhGioiOnline.app/Contents/MacOS/Unity")
        out = Path("/tmp/evidence")
        command = capture.build_screen_command(player, out, "pc", "entry")
        self.assertIn("--lgo-map01a-entry-capture", command)
        self.assertIn("--lgo-map01a-auth-validation-capture", command)
        self.assertNotIn("osascript", command)

        register = capture.build_screen_command(player, out, "tablet", "register")
        self.assertIn("--lgo-map01a-register-capture", register)
        self.assertIn("--lgo-map01a-auth-validation-capture", register)
        self.assertIn("--lgo-product-account-states-capture", register)

        recovery = capture.build_screen_command(player, out, "mobile", "password-recovery")
        self.assertIn("--lgo-map01a-password-recovery-capture", recovery)
        self.assertIn("--lgo-map01a-auth-validation-capture", recovery)
        self.assertIn("--lgo-product-account-states-capture", recovery)

        verify = capture.build_screen_command(player, out, "pc", "password-recovery-verify")
        self.assertIn("--lgo-map01a-password-recovery-verify-capture", verify)
        self.assertNotIn("--lgo-map01a-auth-validation-capture", verify)
        self.assertIn("--lgo-product-account-states-capture", verify)
        new_password = capture.build_screen_command(player, out, "pc", "password-recovery-new-password")
        self.assertIn("--lgo-map01a-password-recovery-new-password-capture", new_password)
        self.assertNotIn("--lgo-map01a-auth-validation-capture", new_password)
        self.assertIn("--lgo-product-account-states-capture", new_password)

    def test_character_entry_capture_uses_internal_loaded_state_without_auth_network(self):
        player = Path("/tmp/LinhGioiOnline.app/Contents/MacOS/Unity")
        out = Path("/tmp/evidence")
        self.assertIn("character-entry", capture.SCREEN_CAPTURES)
        spec = capture.SCREEN_CAPTURES["character-entry"]
        self.assertEqual(spec.frames, ("character-select.png", "map01a-entry.png"))
        command = capture.build_screen_command(player, out, "pc", "character-entry")
        self.assertIn("--lgo-map01a-character-entry-capture", command)
        self.assertNotIn("--lgo-map01a-auth-validation-capture", command)
        self.assertNotIn("--lgo-product-account-states-capture", command)
        self.assertNotIn("osascript", command)

    def test_non_auth_commands_do_not_request_validation_state(self):
        player = Path("/tmp/LinhGioiOnline.app/Contents/MacOS/Unity")
        command = capture.build_screen_command(player, Path("/tmp/evidence"), "pc", "menu")
        self.assertIn("--lgo-map01a-menu-capture", command)
        self.assertNotIn("--lgo-map01a-auth-validation-capture", command)
    def test_existing_profile_is_rejected_without_explicit_replace(self):
        with tempfile.TemporaryDirectory() as tmp:
            profile = Path(tmp) / "pc"
            profile.mkdir()
            with self.assertRaises(FileExistsError):
                capture.prepare_profile_directory(profile, replace=False)
            capture.prepare_profile_directory(profile, replace=True)
            self.assertTrue(profile.is_dir())
            self.assertEqual(list(profile.iterdir()), [])

    def test_required_frame_validation_rejects_missing_and_stale_files(self):
        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            started = time.time_ns()
            missing = capture.validate_required_frames(root, ("menu.png",), started)
            self.assertEqual(missing, ["MISSING_FRAME:menu.png"])

            frame = root / "menu.png"
            frame.write_bytes(b"png")
            stale_ns = started - 10_000_000_000
            import os
            os.utime(frame, ns=(stale_ns, stale_ns))
            self.assertEqual(
                capture.validate_required_frames(root, ("menu.png",), started),
                ["STALE_FRAME:menu.png"],
            )
    def test_quest_command_uses_p0_viewport_instead_of_legacy_pc_size(self):
        player = Path("/tmp/LinhGioiOnline.app/Contents/MacOS/Unity")
        out = Path("/tmp/quest")
        for profile, (width, height) in capture.PROFILES.items():
            with self.subTest(profile=profile):
                command = capture.build_quest_command(player, out, profile)
                self.assertEqual(command[command.index("-screen-width") + 1], str(width))
                self.assertEqual(command[command.index("-screen-height") + 1], str(height))
                self.assertIn("--lgo-map01a-art-capture", command)
                self.assertIn("--lgo-map01a-quest-only", command)
                self.assertIn("--lgo-map01a-device", command)
                self.assertNotIn("capture_lgo_map01a_art.py", " ".join(command))

    def test_screen_contract_requires_default_and_validation_auth_frames(self):
        self.assertEqual(capture.SCREEN_CAPTURES["entry"].frames,
                         ("entry-login.png", "entry-login-validation.png"))
        self.assertEqual(capture.SCREEN_CAPTURES["register"].frames,
                         ("register-account.png", "register-validation.png", "register-loading.png", "register-conflict.png"))
        self.assertEqual(capture.SCREEN_CAPTURES["password-recovery"].frames,
                         ("password-recovery-request.png", "password-recovery-validation.png",
                          "password-recovery-loading.png", "password-recovery-unavailable.png"))
        self.assertEqual(capture.SCREEN_CAPTURES["password-recovery-verify"].frames,
                         ("password-recovery-verify.png", "password-recovery-verify-invalid-expired.png"))
        self.assertEqual(capture.SCREEN_CAPTURES["password-recovery-new-password"].frames,
                         ("password-recovery-new-password.png", "password-recovery-new-password-rule.png"))


# PNG header validation is intentionally independent from Unity manifest claims.
class WholeFlowP0PngTests(unittest.TestCase):
    def test_png_dimensions_reads_ihdr_size(self):
        import struct
        with tempfile.TemporaryDirectory() as tmp:
            frame = Path(tmp) / "frame.png"
            frame.write_bytes(
                b"\x89PNG\r\n\x1a\n" + b"\x00\x00\x00\x0d" + b"IHDR" + struct.pack(">II", 1600, 900)
            )
            self.assertEqual(capture.png_dimensions(frame), (1600, 900))


if __name__ == "__main__":
    unittest.main()
