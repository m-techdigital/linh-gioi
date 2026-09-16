import json
import unittest
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
ATLAS = ROOT / 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01ASkillIcons'


class SkillIconModulesTests(unittest.TestCase):
    def test_one_frame_twelve_ring_free_contents(self):
        manifest = json.loads((ATLAS / 'manifest.json').read_text())
        frames = [p for p in manifest['parts'] if p.get('role') == 'shared-frame']
        self.assertEqual(1, len(frames), 'The ring must exist as exactly one reusable sprite.')
        self.assertEqual('frame', frames[0]['id'])
        self.assertEqual(12, sum(p.get('role') == 'inner-symbol' for p in manifest['parts']))

    def test_frame_aperture_is_empty_and_content_has_no_outer_rim(self):
        manifest = json.loads((ATLAS / 'manifest.json').read_text())
        self.assertEqual([512, 512], manifest['textureSize'])
        atlas = Image.open(ATLAS / 'map01a-skill-icons.png').convert('RGBA')
        for p in manifest['parts']:
            tile = atlas.crop((p['x'], 512-p['y']-128, p['x']+128, 512-p['y'])).getchannel('A')
            self.assertIsNotNone(tile.getbbox(), p['id'])
            if p['id'] == 'frame':
                self.assertIsNone(tile.crop((32, 32, 96, 96)).getbbox())
            else:
                self.assertIsNone(tile.crop((0, 0, 128, 8)).getbbox(), p['id'])

    def test_pack_is_deterministic_and_rejects_unregistered_source(self):
        import tempfile
        import importlib.util
        self.assertIsNotNone(importlib.util.find_spec("pack_lgo_skill_icons"), "One registered packing pipeline is required.")
        import pack_lgo_skill_icons as packer
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            packer.pack(packer.SOURCE, root / 'a')
            packer.pack(packer.SOURCE, root / 'b')
            name = 'map01a-skill-icons.png'
            self.assertEqual((root/'a'/name).read_bytes(), (root/'b'/name).read_bytes())
            wrong = root / 'wrong.png'
            Image.new('RGBA', (1448, 1086)).save(wrong)
            with self.assertRaisesRegex(ValueError, 'hash'):
                packer.pack(wrong, root / 'rejected')
            self.assertFalse((root/'rejected'/name).exists())


class SkillArtworkIntakeTests(unittest.TestCase):
    """Synthetic inputs test the intake contract; never promote these as art."""
    def setUp(self):
        import inspect
        import tempfile
        import os
        import pack_lgo_skill_icons as packer
        self.assertIn('registry', inspect.signature(packer.pack).parameters,
                      'The existing packer must accept registered inner artwork without class-specific builders.')
        self.packer = packer
        self.temp = tempfile.TemporaryDirectory(prefix='lgo-skill-intake-', dir=os.environ.get('LGO_TEST_TMPDIR'))
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.output = self.root / 'candidate'
        self.registry = self.root / 'intake.json'
        library = json.loads((ATLAS / 'skill-library.json').read_text())['skills']
        self.ids = [s['iconId'] for s in library if s['iconId'] not in packer.IDS]
        self.assertEqual(36, len(self.ids))
        self.board = self.root / 'synthetic.png'
        image = Image.new('RGBA', (1536, 1536), (35, 85, 145, 255))
        image.save(self.board)
        (self.root / 'review.txt').write_text('SYNTHETIC TEST FIXTURE. NOT production artwork.\n')
        import hashlib
        self.document = {'version': 1, 'review': {'status': 'SELF_REVIEWED', 'evidence': 'review.txt'},
                         'sources': [{'id': 'test-board', 'path': self.board.name,
                                      'sha256': hashlib.sha256(self.board.read_bytes()).hexdigest(), 'size': [1536, 1536]}],
                         'parts': [{'id': key, 'sourceId': 'test-board', 'rect': [i % 6 * 256, i // 6 * 256, 256, 256]}
                                   for i, key in enumerate(self.ids)]}

    def write_registry(self, document=None):
        self.registry.write_text(json.dumps(self.document if document is None else document))
        return self.registry

    @staticmethod
    def tile(atlas, manifest, key):
        part = next(p for p in manifest['parts'] if p['id'] == key)
        top = atlas.height - part['y'] - part['h']
        return atlas.crop((part['x'], top, part['x'] + part['w'], top + part['h']))

    def test_all_missing_skills_use_one_intake_and_preserve_existing_art(self):
        result = self.packer.pack(output=self.output, registry=self.write_registry())
        self.assertEqual([1024, 1024], result['textureSize'])
        self.assertEqual(49, len(result['parts']))
        self.assertEqual(1, sum(p['role'] == 'shared-frame' for p in result['parts']))
        self.assertEqual(set(self.packer.IDS) | set(self.ids) | {'frame'}, {p['id'] for p in result['parts']})
        baseline = json.loads((ATLAS / 'manifest.json').read_text())
        with Image.open(ATLAS / 'map01a-skill-icons.png') as old, Image.open(self.output / 'map01a-skill-icons.png') as new:
            for part in baseline['parts']:
                self.assertEqual(self.tile(old, baseline, part['id']).tobytes(), self.tile(new, result, part['id']).tobytes())
            for key in self.ids:
                alpha = self.tile(new, result, key).getchannel('A')
                self.assertIsNotNone(alpha.getbbox(), key)
                self.assertIsNone(alpha.crop((0, 0, 128, 8)).getbbox(), key)
        self.assertIn('registrySha256', result['artworkIntake'])
        self.assertEqual(36, result['artworkIntake']['contentCount'])

    def test_rejects_bad_registration_before_touching_existing_output(self):
        import copy
        self.packer.pack(output=self.output)
        originals = {p.name: p.read_bytes() for p in self.output.iterdir()}
        mutations = [
            ('hash', lambda d: d['sources'][0].update(sha256='0' * 64)),
            ('review', lambda d: d['review'].update(status='DRAFT')),
            ('evidence', lambda d: d['review'].update(evidence='missing-review.txt')),
            ('unknown', lambda d: d['parts'][0].update(id='undefined_skill')),
            ('duplicate', lambda d: d['parts'].append(dict(d['parts'][0]))),
            ('existing', lambda d: d['parts'][0].update(id='frame')),
            ('source', lambda d: d['parts'][0].update(sourceId='absent')),
            ('size', lambda d: d['sources'][0].update(size=[100, 100])),
            ('bounds', lambda d: d['parts'][0].update(rect=[1450, 0, 256, 256])),
            ('upscale', lambda d: d['parts'][0].update(rect=[0, 0, 64, 64])),
            ('square', lambda d: d['parts'][0].update(rect=[0, 0, 128, 256])),
            ('integer', lambda d: d['parts'][0].update(rect=[False, 0, 256, 256])),
        ]
        for label, mutate in mutations:
            with self.subTest(label=label):
                document = copy.deepcopy(self.document)
                mutate(document)
                with self.assertRaises(ValueError):
                    self.packer.pack(output=self.output, registry=self.write_registry(document))
                self.assertEqual(originals, {p.name: p.read_bytes() for p in self.output.iterdir()})

    def test_registration_order_does_not_change_pixels(self):
        first = self.packer.pack(output=self.output, registry=self.write_registry())
        before = (self.output / 'map01a-skill-icons.png').read_bytes()
        self.document['parts'].reverse()
        second = self.packer.pack(output=self.root / 'second', registry=self.write_registry())
        self.assertEqual(before, (self.root / 'second/map01a-skill-icons.png').read_bytes())
        self.assertEqual(first['parts'], second['parts'])

    def test_no_registry_preserves_published_atlas_and_manifest_byte_for_byte(self):
        self.packer.pack(output=self.output)
        for name in ('map01a-skill-icons.png', 'manifest.json'):
            self.assertEqual((ATLAS / name).read_bytes(), (self.output / name).read_bytes(), name)

    def test_budget_failure_preserves_the_last_valid_pair(self):
        from unittest.mock import patch
        self.packer.pack(output=self.output)
        before = {p.name: p.read_bytes() for p in self.output.iterdir()}
        with patch.object(self.packer, 'BASE_BYTE_BUDGET', 16), patch.object(self.packer, 'EXTRA_BYTE_BUDGET', 0):
            with self.assertRaisesRegex(ValueError, 'budget'):
                self.packer.pack(output=self.output, registry=self.write_registry())
        self.assertEqual(before, {p.name: p.read_bytes() for p in self.output.iterdir()})

    def candidate_runtime_pack(self):
        import validate_2d_branch_no_source_images as guard
        fake_root = self.root / 'runtime-fixture'
        relative = ATLAS.relative_to(ROOT)
        output = fake_root / relative
        manifest = self.packer.pack(output=output, registry=self.write_registry())
        name = 'map01a-skill-icons.png.meta'
        (output / name).write_text((ATLAS / name).read_text().replace('maxTextureSize: 512', 'maxTextureSize: 1024'))
        (output / 'skill-library.json').write_bytes((ATLAS / 'skill-library.json').read_bytes())
        spec = next(s for s in guard.RUNTIME_ART_PACKS if s['id'] == manifest['id'])
        return guard, fake_root, output, spec, manifest

    def test_runtime_gate_accepts_registered_expansion_without_global_budget_relaxation(self):
        guard, fake_root, output, spec, manifest = self.candidate_runtime_pack()
        try:
            allowed = guard._validate_runtime_pack(fake_root, spec)
        except ValueError as error:
            self.fail('Registered 49-module candidate is rejected by the fixed-size gate: ' + str(error))
        self.assertEqual({str(ATLAS.relative_to(ROOT) / 'map01a-skill-icons.png')}, allowed)
        self.assertEqual(400000, spec['max_bytes'], 'The baseline budget must not be widened globally.')
        import copy
        mutations = [lambda m: m['artworkIntake'].update(contentCount=0),
                     lambda m: m['artworkIntake']['review'].update(status='DRAFT'),
                     lambda m: m['parts'][-1].update(role='shared-frame'),
                     lambda m: m['parts'][-1].update(sourceId='absent'),
                     lambda m: m['parts'][-1].update(sourceRect=[0, 0, 32, 32])]
        for mutate in mutations:
            broken = copy.deepcopy(manifest)
            mutate(broken)
            (output / 'manifest.json').write_text(json.dumps(broken))
            with self.assertRaises(ValueError):
                guard._validate_runtime_pack(fake_root, spec)
