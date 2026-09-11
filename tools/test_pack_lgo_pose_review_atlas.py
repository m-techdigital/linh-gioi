import hashlib
import json
import subprocess
import sys
from pathlib import Path
import tempfile
import unittest

from PIL import Image, ImageDraw
from pack_lgo_pose_review_atlas import pack_review
from pack_lgo_vo_lv1_map_avatar import project_canvas_rect


class PoseReviewAtlasTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def source(self, name, box=(300, 200, 470, 1400)):
        image = Image.new('RGBA', (1024, 1536))
        ImageDraw.Draw(image).rectangle(box, fill=(190, 100, 30, 255))
        path = self.root / (name + '.png')
        image.save(path)
        return {'id': name, 'source': str(path),
                'sourceSha256': hashlib.sha256(path.read_bytes()).hexdigest()}

    def test_default_review_density_is_div4(self):
        _, report = pack_review([self.source('idle')])
        self.assertEqual(report['samplingDivisor'], 4)
        self.assertEqual(report['status'], 'REVIEW_ONLY')
        self.assertFalse(report['runtimeEligible'])

    def test_trim_preserves_reconstruction_and_world_projection(self):
        sources = [self.source('idle'), self.source('run_a', (60, 520, 900, 1479))]
        for divisor in (4, 8):
            with self.subTest(divisor=divisor):
                atlas, report = pack_review(sources, divisor=divisor, max_side=1024)
                for source, sprite in zip(sources, report['sprites']):
                    with Image.open(source['source']) as opened:
                        expected = opened.resize((1024//divisor, 1536//divisor), Image.Resampling.LANCZOS)
                    restored = Image.new('RGBA', expected.size)
                    x, y, w, h = sprite['atlasRectTopLeft']
                    left, top, right, bottom = sprite['sourceCanvasRect']
                    restored.paste(atlas.crop((x, y, x+w, y+h)), (left//divisor, top//divisor))
                    self.assertEqual(expected.tobytes(), restored.tobytes())
                    projection = project_canvas_rect([left, top, right, bottom])
                    self.assertEqual(projection, {key: sprite[key] for key in projection})
                self.assertFalse(report['runtimeEligible'])

    def test_identical_pixels_share_atlas_rect_but_keep_pose_ids(self):
        atlas, report = pack_review([self.source('a'), self.source('b')])
        self.assertEqual(report['uniqueSprites'], 1)
        self.assertEqual(report['sprites'][0]['atlasRectTopLeft'], report['sprites'][1]['atlasRectTopLeft'])
        self.assertEqual([p['id'] for p in report['sprites']], ['a', 'b'])

    def test_changed_source_is_rejected(self):
        source = self.source('idle')
        source['sourceSha256'] = '0'*64
        with self.assertRaisesRegex(ValueError, 'fingerprint'):
            pack_review([source])

    def test_noncanonical_source_and_duplicate_ids_are_rejected(self):
        source = self.source('idle')
        with self.assertRaisesRegex(ValueError, 'Duplicate'):
            pack_review([source, source])
        path = Path(source['source'])
        Image.new('RGBA', (128, 192)).save(path)
        source['sourceSha256'] = hashlib.sha256(path.read_bytes()).hexdigest()
        with self.assertRaisesRegex(ValueError, 'canonical'):
            pack_review([source])

    def test_budget_overflow_does_not_silently_downsample(self):
        with self.assertRaisesRegex(ValueError, 'budget'):
            pack_review([self.source('idle')], max_side=64)

    def test_empty_source_list_and_empty_sprite_are_rejected(self):
        with self.assertRaises(ValueError):
            pack_review([])
        source = self.source('idle')
        path = Path(source['source'])
        Image.new('RGBA', (1024, 1536)).save(path)
        source['sourceSha256'] = hashlib.sha256(path.read_bytes()).hexdigest()
        with self.assertRaisesRegex(ValueError, 'empty'):
            pack_review([source])

    def test_cli_exports_player_filenames_div4_and_jump_pivot(self):
        sources = [self.source('idle'), self.source('run_a', (60, 520, 900, 1479)),
                   self.source('run_b', (90, 510, 880, 1479)),
                   self.source('jump_tuck', (200, 300, 700, 1200))]
        records = self.root / 'sources.json'
        records.write_text(json.dumps(sources))
        output = self.root / 'pack'
        result = subprocess.run([sys.executable, str(Path(__file__).with_name('pack_lgo_pose_review_atlas.py')),
                                 '--sources', str(records), '--output-dir', str(output),
                                 '--jump-pivot-source', '512', '820'], capture_output=True, text=True)
        self.assertEqual(result.returncode, 0, result.stderr)
        report = json.loads((output / 'atlas-review.json').read_text())
        self.assertEqual(report['samplingDivisor'], 4)
        self.assertEqual(report['jumpPivotSource'], [512, 820])
        self.assertEqual(report['status'], 'REVIEW_ONLY')
        self.assertFalse(report['runtimeEligible'])
        self.assertEqual(report['atlasSha256'], hashlib.sha256((output / 'atlas-review.png').read_bytes()).hexdigest())
        self.assertLessEqual(max(report['atlasSize']), 1024)

    def test_cli_rejects_jump_without_registered_pivot_before_writing(self):
        records = self.root / 'sources.json'
        records.write_text(json.dumps([self.source('jump_tuck')]))
        output = self.root / 'pack'
        result = subprocess.run([sys.executable, str(Path(__file__).with_name('pack_lgo_pose_review_atlas.py')),
                                 '--sources', str(records), '--output-dir', str(output)],
                                capture_output=True, text=True)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn('jump pivot', result.stderr.lower())
        self.assertFalse(output.exists())
