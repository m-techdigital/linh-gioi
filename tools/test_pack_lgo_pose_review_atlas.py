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

    def accepted_surface_contract(self):
        contract = self.root / 'surface-contract-accepted.json'
        contract.write_text(json.dumps({
            'sourceSpaceProfile': {'canvas': [1024, 1536], 'originX': 512, 'groundY': 1484, 'unitScale': '1.70/1536'},
            'poses': ['idle', 'run_contact_a', 'run_a', 'run_contact_b', 'run_b', 'jump_tuck'],
            'selectedRoute': 'SLEEVELESS_PHAP_LV1',
            'itemFamilies': [
                {'slotId': 'outer_top', 'familyType': 'cloth_body', 'ownership': ['torso_cloth'],
                 'routeDependency': 'IDLE_NATIVE_SOURCE_MUST_REMOVE_SLEEVES'},
            ],
            'sourceArtifactValidation': {'status': 'SOURCE_ARTIFACT_VISUAL_ACCEPTED'},
            'runtimePromotionAllowed': False,
        }))
        return contract

    def source_under_authoring_selection(self, source, status):
        selected = self.root / ('selection-' + status.lower())
        selected.mkdir()
        old_path = Path(source['source'])
        new_path = selected / old_path.name
        old_path.rename(new_path)
        source['source'] = str(new_path)
        (selected / 'authoring-selection.json').write_text(json.dumps({
            'status': status,
            'runtimeEligible': False,
            'allowedSourceContent': [{
                'path': old_path.name,
                'sha256': source['sourceSha256'],
            }],
        }))
        return source

    def test_default_review_density_is_div4(self):
        _, report = pack_review([self.source('idle')])
        self.assertEqual(report['samplingDivisor'], 4)
        self.assertEqual(report['status'], 'REVIEW_ONLY')
        self.assertFalse(report['runtimeEligible'])

    def test_entry_exit_set_fits_without_doubling_texture_memory(self):
        sizes = [(137, 373), (216, 312), (228, 285), (156, 312),
                 (179, 280), (167, 230), (150, 347), (143, 358)]
        sources = [self.source('pose' + str(i), (0, 0, w - 1, h - 1))
                   for i, (w, h) in enumerate(sizes)]
        atlas, report = pack_review(sources, divisor=1)
        self.assertEqual(report['rgba8Bytes'], 512 * 1024 * 4)
        for part, size in zip(report['sprites'], sizes):
            x, y, w, h = part['atlasRectTopLeft']
            self.assertEqual((w, h), size)
            self.assertEqual(atlas.crop((x, y, x + w, y + h)).tobytes(),
                             Image.new('RGBA', size, (190, 100, 30, 255)).tobytes())

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

    def test_pack_entrypoint_rejects_source_under_rejected_authoring_selection(self):
        rejected = self.root / 'rejected-design-v1'
        rejected.mkdir()
        (rejected / 'authoring-selection.json').write_text(json.dumps({
            'status': 'REJECTED_SOURCE_VISUAL',
            'runtimeEligible': False,
        }))
        source = self.source('idle')
        moved = rejected / 'idle.png'
        Path(source['source']).rename(moved)
        source['source'] = str(moved)

        with self.assertRaisesRegex(ValueError, 'REJECTED_SOURCE_VISUAL'):
            pack_review([source])

    def test_pack_entrypoint_rejects_quarantined_evidence_path(self):
        quarantined = self.root / 'rejected-evidence' / 'bad-v1'
        quarantined.mkdir(parents=True)
        source = self.source('idle')
        moved = quarantined / 'idle.png'
        Path(source['source']).rename(moved)
        source['source'] = str(moved)

        with self.assertRaisesRegex(ValueError, 'rejected-evidence'):
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

    def test_rgb_source_cannot_gain_fake_alpha_during_pack(self):
        source = self.source('idle')
        path = Path(source['source'])
        Image.new('RGB', (1024, 1536), (128, 128, 128)).save(path)
        source['sourceSha256'] = hashlib.sha256(path.read_bytes()).hexdigest()

        with self.assertRaisesRegex(ValueError, 'explicit alpha'):
            pack_review([source])

    def test_fully_opaque_rgba_source_is_not_a_transparent_sprite(self):
        source = self.source('idle')
        path = Path(source['source'])
        Image.new('RGBA', (1024, 1536), (128, 128, 128, 255)).save(path)
        source['sourceSha256'] = hashlib.sha256(path.read_bytes()).hexdigest()

        with self.assertRaisesRegex(ValueError, 'transparent background'):
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

    def test_component_pose_ids_and_fully_occluded_frame_are_preserved(self):
        front = {**self.source('front'), 'id': 'idle', 'componentId': 'front', 'order': 29}
        rear = {**self.source('rear'), 'id': 'idle', 'componentId': 'back', 'order': 23}
        path = Path(rear['source'])
        Image.new('RGBA', (1024, 1536)).save(path)
        rear['sourceSha256'] = hashlib.sha256(path.read_bytes()).hexdigest()
        atlas, report = pack_review([front, rear], divisor=2, allow_empty_components=True)
        self.assertEqual([(p['id'], p['componentId'], p['order']) for p in report['sprites']],
                         [('idle', 'front', 29), ('idle', 'back', 23)])
        empty = report['sprites'][1]
        self.assertTrue(empty['emptyComponent'])
        x, y, w, h = empty['atlasRectTopLeft']
        self.assertIsNone(atlas.crop((x, y, x+w, y+h)).getchannel('A').getbbox())
        self.assertFalse(report['sprites'][0].get('emptyComponent', False))
        with self.assertRaisesRegex(ValueError, 'Duplicate'):
            pack_review([front, front], allow_empty_components=True)
        with self.assertRaisesRegex(ValueError, 'empty'):
            pack_review([rear])
        with self.assertRaisesRegex(ValueError, 'empty'):
            pack_review([{k: v for k, v in rear.items() if k != 'componentId'}],
                        allow_empty_components=True)

    def test_implicit_main_cannot_collide_with_explicit_main_component(self):
        source = self.source('idle')
        with self.assertRaisesRegex(ValueError, 'Duplicate'):
            pack_review([source, {**source, 'componentId': 'main'}])

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

    def test_cli_requires_surface_contract_for_outfit_review_entrypoint(self):
        records = self.root / 'sources.json'
        records.write_text(json.dumps([{**self.source('idle'), 'slotId': 'outer_top'}]))
        output = self.root / 'outer-top-review'

        result = subprocess.run([sys.executable, str(Path(__file__).with_name('pack_lgo_pose_review_atlas.py')),
                                 '--sources', str(records), '--output-dir', str(output)],
                                capture_output=True, text=True)

        self.assertNotEqual(result.returncode, 0)
        self.assertIn('surface contract required for outfit review pack', result.stderr.lower())
        self.assertFalse(output.exists())

    def test_cli_accepts_outfit_review_entrypoint_only_with_ready_source_artifact(self):
        records = self.root / 'sources.json'
        source = self.source_under_authoring_selection(
            {**self.source('idle'), 'slotId': 'outer_top'},
            'SOURCE_ARTIFACT_VISUAL_ACCEPTED',
        )
        records.write_text(json.dumps([source]))
        output = self.root / 'outer-top-review'

        result = subprocess.run([sys.executable, str(Path(__file__).with_name('pack_lgo_pose_review_atlas.py')),
                                 '--sources', str(records), '--output-dir', str(output),
                                 '--surface-contract', str(self.accepted_surface_contract())],
                                capture_output=True, text=True)

        self.assertEqual(result.returncode, 0, result.stderr)
        report = json.loads((output / 'atlas-review.json').read_text())
        self.assertEqual(report['surfaceContractStatus'], 'PASS')
        self.assertTrue(report['sourceArtifactValid'])

    def test_cli_rejects_pending_outfit_source_even_with_accepted_contract(self):
        source = self.source_under_authoring_selection(
            {**self.source('idle'), 'slotId': 'outer_top'},
            'SOURCE_REVIEW_REQUIRED',
        )
        records = self.root / 'sources.json'
        records.write_text(json.dumps([source]))

        result = subprocess.run([
            sys.executable, str(Path(__file__).with_name('pack_lgo_pose_review_atlas.py')),
            '--sources', str(records), '--output-dir', str(self.root / 'outer-top-review'),
            '--surface-contract', str(self.accepted_surface_contract()),
        ], capture_output=True, text=True)

        self.assertNotEqual(result.returncode, 0)
        self.assertIn('source_artifact_visual_accepted', result.stderr.lower())
        self.assertFalse((self.root / 'outer-top-review').exists())

    def test_cli_rejects_outfit_source_missing_from_accepted_source_allowlist(self):
        source = self.source_under_authoring_selection(
            {**self.source('idle'), 'slotId': 'outer_top'},
            'SOURCE_ARTIFACT_VISUAL_ACCEPTED',
        )
        selection_path = Path(source['source']).parent / 'authoring-selection.json'
        selection = json.loads(selection_path.read_text())
        selection['allowedSourceContent'][0]['sha256'] = '0' * 64
        selection_path.write_text(json.dumps(selection))
        records = self.root / 'sources.json'
        records.write_text(json.dumps([source]))

        result = subprocess.run([
            sys.executable, str(Path(__file__).with_name('pack_lgo_pose_review_atlas.py')),
            '--sources', str(records), '--output-dir', str(self.root / 'outer-top-review'),
            '--surface-contract', str(self.accepted_surface_contract()),
        ], capture_output=True, text=True)

        self.assertNotEqual(result.returncode, 0)
        self.assertIn('allowedsourcecontent', result.stderr.lower())
        self.assertFalse((self.root / 'outer-top-review').exists())


    def test_cli_rejects_surface_contract_that_needs_route_decision(self):
        sources = [{**self.source('idle'), 'slotId': 'outer_top'}]
        records = self.root / 'sources.json'
        records.write_text(json.dumps(sources))
        contract = self.root / 'surface-contract.json'
        contract.write_text(json.dumps({
            'sourceSpaceProfile': {'canvas': [1024, 1536], 'originX': 512, 'groundY': 1484, 'unitScale': '1.70/1536'},
            'poses': ['idle', 'run_contact_a', 'run_a', 'run_contact_b', 'run_b', 'jump_tuck'],
            'selectedRoute': None,
            'itemFamilies': [{'slotId': 'outer_top', 'familyType': 'cloth_body', 'ownership': ['torso_cloth']}],
            'runtimePromotionAllowed': False,
        }))
        result = subprocess.run([sys.executable, str(Path(__file__).with_name('pack_lgo_pose_review_atlas.py')),
                                 '--sources', str(records), '--output-dir', str(self.root / 'outer-top-review'),
                                 '--surface-contract', str(contract)], capture_output=True, text=True)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn('Surface contract not ready for pack', result.stderr)
        self.assertIn('NEED_OWNER_DECISION', result.stderr)
