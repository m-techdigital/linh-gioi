import importlib
import json
import struct
import tempfile
import unittest
from pathlib import Path

capture = importlib.import_module("tools.capture_lgo_product_auth")


class ProductAuthCaptureTests(unittest.TestCase):
    def test_profiles_and_frames_match_auth_visual_contract(self):
        self.assertEqual(capture.PROFILES, {
            "pc": (1600, 900), "tablet": (1024, 768), "mobile": (1600, 720)
        })
        self.assertEqual(capture.REQUIRED_FRAMES, (
            "entry-login.png",
            "entry-login-validation.png",
            "entry-login-authenticating.png",
            "entry-login-invalid-credentials.png",
            "entry-login-success-character-select.png",
        ))

    def test_command_uses_internal_product_auth_capture_without_os_input(self):
        command = capture.build_player_command(Path("/tmp/Unity"), Path("/tmp/out"), "pc")
        self.assertIn("--lgo-map01a-entry-capture", command)
        self.assertIn("--lgo-product-auth-states-capture", command)
        self.assertIn("--lgo-map01a-device", command)
        self.assertEqual(command[command.index("--lgo-map01a-device") + 1], "pc")
        self.assertEqual(command[command.index("-screen-width") + 1], "1600")
        self.assertEqual(command[command.index("-screen-height") + 1], "900")

    def test_manifest_validation_requires_current_dimensions_and_internal_input(self):
        with tempfile.TemporaryDirectory() as tmp:
            out = Path(tmp)
            for frame in capture.REQUIRED_FRAMES:
                (out / frame).write_bytes(
                    b"\x89PNG\r\n\x1a\n" + b"\x00\x00\x00\x0d" + b"IHDR" + struct.pack(">II", 1600, 900)
                )
            manifest = {
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "captureScope": "map01a-entry-login",
                "usesOsMouseOrKeyboard": False,
                "width": 1600,
                "height": 900,
                "validationFrame": "entry-login-validation.png",
                "productAuthFrames": list(capture.REQUIRED_FRAMES[2:]),
            }
            (out / "manifest.json").write_text(json.dumps(manifest), encoding="utf-8")
            self.assertEqual(capture.validate_profile(out, "pc"), [])
            manifest["usesOsMouseOrKeyboard"] = True
            (out / "manifest.json").write_text(json.dumps(manifest), encoding="utf-8")
            self.assertIn("OS_INPUT_USED", capture.validate_profile(out, "pc"))


if __name__ == "__main__":
    unittest.main()
