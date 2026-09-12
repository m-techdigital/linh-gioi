import tempfile
import unittest
from pathlib import Path

from launch_lgo_source_pose_review import PACK_SUFFIXES, build_class_args, build_player_command


class SourcePoseReviewLaunchTests(unittest.TestCase):
    def test_player_command_uses_only_registered_source_pose_path(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            player = root / 'LinhGioiOnline'
            player.write_text('player')
            for class_id in ('kiem', 'phap', 'co', 'linh'):
                for suffix in PACK_SUFFIXES[class_id]:
                    pack = root / 'build' / (class_id + suffix)
                    pack.mkdir(parents=True)
                    (pack / 'atlas-review.json').write_text('{}')

            command = build_player_command(player, root, root / 'player.log')

            self.assertIn('--lgo-vo-pose-review-dir', command)
            self.assertEqual(command.count('--lgo-source-pose-class'), 4)
            self.assertFalse(any(arg in command for arg in (
                '--lgo-kiem-review', '--lgo-phap-review', '--lgo-co-review', '--lgo-linh-review')))

    def test_phap_uses_canonical_layer_pack_with_baked_jump_scale(self):
        self.assertEqual(
            ('-source-pose-review-canonical-v2/pack', '-source-pose-review-lv10-canonical-v2/pack',
             '-female-source-pose-review-canonical-v2/pack', '-female-source-pose-review-lv10-canonical-v2/pack'),
            PACK_SUFFIXES['phap'],
        )

    def test_builds_one_catalog_entry_per_class_with_four_existing_pack_paths(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for class_id in ('kiem', 'phap'):
                for suffix in PACK_SUFFIXES[class_id]:
                    pack = root / 'build' / (class_id + suffix)
                    pack.mkdir(parents=True)
                    (pack / 'atlas-review.json').write_text('{}')
            args = build_class_args(root, ('kiem', 'phap'))
            self.assertEqual(args.count('--lgo-source-pose-class'), 2)
            self.assertEqual(args[1], 'kiem')
            self.assertEqual(args[7], 'phap')


if __name__ == '__main__':
    unittest.main()
