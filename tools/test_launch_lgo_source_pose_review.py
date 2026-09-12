import tempfile
import unittest
import json
from pathlib import Path
from unittest.mock import patch

from launch_lgo_source_pose_review import CLASSES, PACK_SUFFIXES, build_class_args, build_player_command, main


class SourcePoseReviewLaunchTests(unittest.TestCase):
    def test_player_command_uses_only_registered_source_pose_path(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            player = root / 'LinhGioiOnline'
            player.write_text('player')
            for class_id in CLASSES:
                for suffix in PACK_SUFFIXES[class_id]:
                    if suffix is None: continue
                    pack = root / 'build' / (class_id + suffix)
                    pack.mkdir(parents=True)
                    (pack / 'atlas-review.json').write_text('{}')

            command = build_player_command(player, root, root / 'player.log')

            self.assertIn('--lgo-vo-pose-review-dir', command)
            self.assertEqual(command.count('--lgo-source-pose-class'), len(CLASSES))
            self.assertFalse(any(arg in command for arg in (
                '--lgo-kiem-review', '--lgo-phap-review', '--lgo-co-review', '--lgo-linh-review')))

    def test_current_launch_excludes_phap_until_class_art_is_complete(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            player = root / 'Player'; player.write_text('player')
            for class_id, suffixes in PACK_SUFFIXES.items():
                for suffix in suffixes:
                    if suffix is None: continue
                    pack = root / 'build' / (class_id + suffix)
                    pack.mkdir(parents=True)
                    (pack / 'atlas-review.json').write_text('{}')
            command = build_player_command(player, root, root / 'player.log')
            self.assertEqual(command.count('--lgo-source-pose-class'), len(CLASSES))
            self.assertNotIn('phap', command)
            self.assertEqual(command[command.index('--lgo-source-pose-class') + 1], 'vo')
            self.assertIn('vo-source-pose-review-preserved-lv1', command[command.index('--lgo-vo-pose-review-dir') + 1])

    def test_invalid_source_never_starts_player(self):
        cases = ({'poseScaleCorrections': {'jump_tuck': 0.6666666667}},
                 {'status': 'SOURCE_REJECTED'}, {'status': 'FIX_REQUIRED'})
        for invalid in cases:
            with self.subTest(invalid=invalid), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                player = root / 'Player'; player.write_text('player')
                for class_id, suffixes in PACK_SUFFIXES.items():
                    for suffix in suffixes:
                        if suffix is None: continue
                        pack = root / 'build' / (class_id + suffix)
                        pack.mkdir(parents=True)
                        data = invalid if class_id == 'kiem' else {'status': 'REVIEW_ONLY'}
                        (pack / 'atlas-review.json').write_text(json.dumps(data))
                with patch('launch_lgo_source_pose_review.subprocess.Popen') as launch:
                    with self.assertRaisesRegex(ValueError, 'Nguồn review'):
                        main(['--player', str(player), '--repo', str(root)])
                    launch.assert_not_called()

    def test_owner_rejected_authoring_selection_never_starts_player(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            player = root / 'Player'; player.write_text('player')
            selected = root / 'selected' / 'phap-lv001' / 'complete-garment-authoring-v1'
            selected.mkdir(parents=True)
            (selected / 'authoring-selection.json').write_text(json.dumps({
                'status': 'OWNER_REJECTED_VISUAL',
                'runtimeEligible': False,
                'rejectionReason': 'wrong level silhouette',
            }))
            source = selected / 'outer_top' / 'registered-six-pose-v1' / 'idle' / 'front.png'
            source.parent.mkdir(parents=True)
            source.write_bytes(b'not read by launcher')
            for class_id, suffixes in PACK_SUFFIXES.items():
                for suffix in suffixes:
                    if suffix is None: continue
                    pack = root / 'build' / (class_id + suffix)
                    pack.mkdir(parents=True)
                    (pack / 'atlas-review.json').write_text(json.dumps({'status': 'REVIEW_ONLY'}))
                    if class_id == 'kiem':
                        item = pack / 'outer-top-review'
                        item.mkdir()
                        (item / 'atlas-review.json').write_text(json.dumps({
                            'status': 'REVIEW_ONLY',
                            'sprites': [{'source': str(source)}],
                        }))
            with patch('launch_lgo_source_pose_review.subprocess.Popen') as launch:
                with self.assertRaisesRegex(ValueError, 'OWNER_REJECTED_VISUAL'):
                    main(['--player', str(player), '--repo', str(root)])
                launch.assert_not_called()

    def test_identity_scale_is_allowed_but_invalid_item_scale_is_not(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for suffix in PACK_SUFFIXES['phap']:
                if suffix is None: continue
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
                    if suffix is None: continue
                    pack = root / 'build' / (class_id + suffix)
                    pack.mkdir(parents=True)
                    (pack / 'atlas-review.json').write_text('{}')
            args = build_class_args(root, ('kiem', 'phap'))
            self.assertEqual(args.count('--lgo-source-pose-class'), 2)
            self.assertEqual(args[1], 'kiem')
            self.assertEqual(args[7], 'phap')


if __name__ == '__main__':
    unittest.main()
