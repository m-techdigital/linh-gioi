import tempfile
import unittest
import json
from pathlib import Path
from unittest.mock import patch

from launch_lgo_source_pose_review import PACK_SUFFIXES, build_class_args, build_player_command, main


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

    def test_invalid_source_never_starts_player(self):
        cases = ({'poseScaleCorrections': {'jump_tuck': 0.6666666667}},
                 {'status': 'SOURCE_REJECTED'}, {'status': 'FIX_REQUIRED'})
        for invalid in cases:
            with self.subTest(invalid=invalid), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                player = root / 'Player'; player.write_text('player')
                for class_id, suffixes in PACK_SUFFIXES.items():
                    for suffix in suffixes:
                        pack = root / 'build' / (class_id + suffix)
                        pack.mkdir(parents=True)
                        data = invalid if class_id == 'phap' else {'status': 'REVIEW_ONLY'}
                        (pack / 'atlas-review.json').write_text(json.dumps(data))
                with patch('launch_lgo_source_pose_review.subprocess.Popen') as launch:
                    with self.assertRaisesRegex(ValueError, 'Nguồn review'):
                        main(['--player', str(player), '--repo', str(root)])
                    launch.assert_not_called()

    def test_identity_scale_is_allowed_but_invalid_item_scale_is_not(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for suffix in PACK_SUFFIXES['phap']:
                pack = root / 'build' / ('phap' + suffix); pack.mkdir(parents=True)
                (pack / 'atlas-review.json').write_text(json.dumps(
                    {'status': 'REVIEW_ONLY', 'poseScaleCorrections': {'jump_tuck': 1}}))
            self.assertIn('phap', build_class_args(root, ('phap',)))
            item = root / 'build' / ('phap' + PACK_SUFFIXES['phap'][0]) / 'outer-top-review'
            item.mkdir()
            (item / 'atlas-review.json').write_text(json.dumps(
                {'poseScaleCorrections': {'jump_tuck': 1.5}}))
            with self.assertRaisesRegex(ValueError, 'jump_tuck'):
                build_class_args(root, ('phap',))

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
