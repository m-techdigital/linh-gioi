import hashlib
import tempfile
import unittest
from pathlib import Path

from tools.bootstrap_lgo_blender import (
    BlenderRelease,
    blender_python_command,
    parse_checksum_manifest,
    verify_sha256,
)


class BlenderBootstrapTests(unittest.TestCase):
    def test_manifest_selects_exact_macos_arm64_release(self) -> None:
        release = BlenderRelease(version="4.5.13", platform="macos-arm64")
        digest = "a" * 64
        manifest = (
            f"{digest}  blender-4.5.13-macos-arm64.dmg\n"
            f"{'b' * 64}  blender-4.5.13-macos-x64.dmg\n"
        )

        self.assertEqual(release.archive_name, "blender-4.5.13-macos-arm64.dmg")
        self.assertEqual(parse_checksum_manifest(manifest, release.archive_name), digest)

    def test_missing_exact_release_is_rejected(self) -> None:
        with self.assertRaisesRegex(ValueError, "not found"):
            parse_checksum_manifest(
                f"{'a' * 64}  blender-4.5.12-macos-arm64.dmg\n",
                "blender-4.5.13-macos-arm64.dmg",
            )

    def test_checksum_mismatch_is_rejected(self) -> None:
        with tempfile.TemporaryDirectory() as temporary_directory:
            archive = Path(temporary_directory) / "blender.dmg"
            archive.write_bytes(b"not blender")

            with self.assertRaisesRegex(ValueError, "SHA-256 mismatch"):
                verify_sha256(archive, "0" * 64)

    def test_checksum_match_returns_actual_digest(self) -> None:
        with tempfile.TemporaryDirectory() as temporary_directory:
            archive = Path(temporary_directory) / "blender.dmg"
            payload = b"verified blender archive fixture"
            archive.write_bytes(payload)
            expected = hashlib.sha256(payload).hexdigest()

            self.assertEqual(verify_sha256(archive, expected), expected)

    def test_background_python_command_fails_on_script_error(self) -> None:
        command = blender_python_command(Path("/tool/Blender"), Path("probe.py"))

        self.assertEqual(command[0], "/tool/Blender")
        self.assertIn("--background", command)
        self.assertIn("--factory-startup", command)
        self.assertEqual(command[command.index("--python-exit-code") + 1], "2")
        self.assertEqual(command[command.index("--python") + 1], "probe.py")


if __name__ == "__main__":
    unittest.main()
