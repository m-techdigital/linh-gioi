import unittest
from pathlib import Path

from capture_lgo_map01a_art import build_player_command


class Map01AArtCaptureCommandTests(unittest.TestCase):
    def test_source_pose_quest_capture_has_one_renderer_authority(self):
        command = build_player_command(
            Path('/tmp/Player'), Path('/tmp/evidence'), 1280, 720, 'pc', True,
            Path('/tmp/source-pose-pack'))

        self.assertIn('--lgo-vo-pose-review-dir', command)
        self.assertIn('--lgo-map01a-quest-only', command)
        self.assertNotIn('--lgo-vo-registered', command)
        self.assertNotIn('--lgo-vo-registered-equipment', command)


if __name__ == '__main__':
    unittest.main()
