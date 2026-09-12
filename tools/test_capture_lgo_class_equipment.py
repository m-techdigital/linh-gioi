import tempfile
import unittest
from pathlib import Path

import capture_lgo_class_equipment as capture


class ClassEquipmentCaptureCommandTests(unittest.TestCase):
    def test_command_uses_map01a_preview_and_absolute_output(self):
        with tempfile.TemporaryDirectory() as tmp:
            player = Path(tmp) / "Unity"
            player.write_text("stub")
            out = Path(tmp) / "relative-out"
            command = capture.build_command(player, "phap", out)
            self.assertIn("--lgo-map01a-art-preview", command)
            self.assertIn("--lgo-phap-capture", command)
            art_dir = command[command.index("--lgo-map01a-art-dir") + 1]
            self.assertTrue(Path(art_dir).is_absolute())
            self.assertIn(str(out.resolve()), command)

    def test_rejects_unknown_class(self):
        with tempfile.TemporaryDirectory() as tmp:
            player = Path(tmp) / "Unity"
            player.write_text("stub")
            with self.assertRaisesRegex(ValueError, "unsupported class"):
                capture.build_command(player, "vo", Path(tmp) / "out")


if __name__ == "__main__":
    unittest.main()
