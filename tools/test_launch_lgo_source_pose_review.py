import tempfile
import unittest
from pathlib import Path

from launch_lgo_source_pose_review import build_class_args


class SourcePoseReviewLaunchTests(unittest.TestCase):
    def test_builds_one_catalog_entry_per_class_with_four_existing_pack_paths(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for class_id in ('kiem', 'phap'):
                for suffix in ('-source-pose-review-v1/pack', '-source-pose-review-lv10-v1/pack',
                               '-female-source-pose-review-v1/pack', '-female-source-pose-review-lv10-v1/pack'):
                    pack = root / 'build' / (class_id + suffix)
                    pack.mkdir(parents=True)
                    (pack / 'atlas-review.json').write_text('{}')
            args = build_class_args(root, ('kiem', 'phap'))
            self.assertEqual(args.count('--lgo-source-pose-class'), 2)
            self.assertEqual(args[1], 'kiem')
            self.assertEqual(args[7], 'phap')


if __name__ == '__main__':
    unittest.main()
