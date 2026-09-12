import hashlib
import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

from compose_lgo_pose_review_loadout import DIRECTORIES, SLOTS, compose_loadout
from pack_lgo_pose_review_atlas import pack_review


class ComposePoseReviewLoadoutTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.packs = {name: self.make_pack(name, level) for name, level in (('lv1', 1), ('lv10', 10))}

    def tearDown(self):
        self.temp.cleanup()

    def make_pack(self, alias, level, family='vo_male_v3', gender='male', class_id='vo'):
        root = self.root / alias
        root.mkdir()
        (root / 'atlas-review.png').write_bytes(b'approved-body-atlas')
        body_atlas = hashlib.sha256((root / 'atlas-review.png').read_bytes()).hexdigest()
        (root / 'atlas-review.json').write_text(json.dumps({'status': 'REVIEW_ONLY',
            'atlasSha256': body_atlas, 'fitFamily': family, 'gender': gender,
            'skeletonVersion': 'authored_skeleton_v1'}))
        body_manifest = hashlib.sha256((root / 'atlas-review.json').read_bytes()).hexdigest()
        for slot in SLOTS:
            directory = root / DIRECTORIES[slot]
            directory.mkdir()
            atlas = (alias + slot).encode()
            (directory / 'atlas-review.png').write_bytes(atlas)
            manifest = {'reviewSlot': slot, 'fitFamily': family,
                        'basePoseAtlasSha256': body_atlas, 'basePoseManifestSha256': body_manifest,
                        'itemId': f'{class_id}_{gender}_lv{level:03d}_{slot}', 'unlockLevel': level,
                        'atlasSha256': hashlib.sha256(atlas).hexdigest()}
            (directory / 'atlas-review.json').write_text(json.dumps(manifest))
        return root

    def change_manifest(self, path, **changes):
        manifest = json.loads(path.read_text())
        manifest.update(changes)
        path.write_text(json.dumps(manifest))

    def test_composes_ten_independent_items_from_two_levels(self):
        selection = {slot: ('lv1' if index % 2 == 0 else 'lv10') for index, slot in enumerate(SLOTS)}
        output = self.root / 'mixed'
        loadout = compose_loadout(self.packs, selection, output)
        self.assertEqual(10, len(loadout['items']))
        self.assertEqual({1, 10}, {item['unlockLevel'] for item in loadout['items']})
        self.assertEqual('slotId -> itemId', loadout['resolver'])
        self.assertTrue(loadout['atomicPreflight'])
        self.assertTrue(all((output / directory / 'atlas-review.png').is_file()
                            for directory in DIRECTORIES.values()))

    def test_rejects_incomplete_selection_before_writing(self):
        with self.assertRaisesRegex(ValueError, 'exactly the ten'):
            compose_loadout(self.packs, {'main_weapon': 'lv1'}, self.root / 'bad')
        self.assertFalse((self.root / 'bad').exists())

    def test_rejects_slot_from_another_body_atomically(self):
        manifest = self.packs['lv10'] / DIRECTORIES['head_hair'] / 'atlas-review.json'
        data = json.loads(manifest.read_text())
        data['basePoseAtlasSha256'] = 'different-body'
        manifest.write_text(json.dumps(data))
        selection = {slot: ('lv10' if slot == 'head_hair' else 'lv1') for slot in SLOTS}
        output = self.root / 'bad-body'
        with self.assertRaisesRegex(ValueError, 'Body authority mismatch'):
            compose_loadout(self.packs, selection, output)
        self.assertFalse(output.exists())

    def test_female_loadout_uses_declared_body_profile_and_skeleton(self):
        pack = self.make_pack('female', 10, 'common_female_v1', 'female', 'phap')
        result = compose_loadout({'female': pack}, {slot: 'female' for slot in SLOTS}, self.root / 'female-result')
        self.assertEqual(result['bodyProfile'], 'common_female_v1')
        self.assertEqual(result['skeletonVersion'], 'authored_skeleton_v1')
        self.assertEqual((result['classId'], result['gender']), ('phap', 'female'))

    def test_rejects_tampered_body_and_item_atlases(self):
        for target, message in ((self.packs['lv1'] / 'atlas-review.png', 'Body atlas hash'),
                                (self.packs['lv1'] / DIRECTORIES['inner_top'] / 'atlas-review.png', 'Item atlas hash')):
            with self.subTest(target=target):
                before = target.read_bytes()
                target.write_bytes(before + b'tampered')
                output = self.root / 'tampered'
                with self.assertRaisesRegex(ValueError, message):
                    compose_loadout(self.packs, {slot: 'lv1' for slot in SLOTS}, output)
                self.assertFalse(output.exists())
                target.write_bytes(before)

    def test_rejects_incompatible_item_identity_even_with_matching_body_hashes(self):
        path = self.packs['lv1'] / DIRECTORIES['outer_top'] / 'atlas-review.json'
        before = path.read_text()
        for changes, message in (({'itemId': 'phap_male_lv001_outer_top'}, 'Class mismatch'),
                                 ({'itemId': 'vo_female_lv001_outer_top'}, 'Gender mismatch'),
                                 ({'fitFamily': 'different_profile'}, 'Fit family mismatch')):
            with self.subTest(changes=changes):
                self.change_manifest(path, **changes)
                with self.assertRaisesRegex(ValueError, message):
                    compose_loadout(self.packs, {slot: 'lv1' for slot in SLOTS}, self.root / 'bad-identity')
                self.assertFalse((self.root / 'bad-identity').exists())
                path.write_text(before)

    def test_mixed_level_outer_removal_reveals_complete_inner_garment_in_all_six_poses(self):
        poses = ('idle', 'run_contact_a', 'run_a', 'run_contact_b', 'run_b', 'jump_tuck')
        colors = {'body': (170, 120, 80, 255), 'inner_top': (20, 80, 220, 255),
                  'outer_top': (220, 30, 40, 255)}
        source_root = self.root / 'authored'; source_root.mkdir()

        def write_atlas(directory, role, order):
            sources = []
            for index, pose in enumerate(poses):
                shift = index * 4
                box = {'body': (350 + shift, 500, 700 + shift, 1000),
                       'inner_top': (400 + shift, 600, 624 + shift, 800),
                       'outer_top': (440 + shift, 560, 584 + shift, 840)}[role]
                image = Image.new('RGBA', (1024, 1536))
                ImageDraw.Draw(image).rectangle(box, fill=colors[role])
                path = source_root / f'{role}-{pose}.png'; image.save(path)
                sources.append({'id': pose, 'source': str(path),
                                'sourceSha256': hashlib.sha256(path.read_bytes()).hexdigest()})
            atlas, manifest = pack_review(sources, divisor=4)
            atlas.save(directory / 'atlas-review.png')
            previous = json.loads((directory / 'atlas-review.json').read_text())
            previous.update(manifest)
            previous['atlasSha256'] = hashlib.sha256((directory / 'atlas-review.png').read_bytes()).hexdigest()
            for part in previous['sprites']: part['order'] = order
            (directory / 'atlas-review.json').write_text(json.dumps(previous))

        write_atlas(self.packs['lv1'], 'body', 24)
        for name in ('atlas-review.png', 'atlas-review.json'):
            (self.packs['lv10'] / name).write_bytes((self.packs['lv1'] / name).read_bytes())
        body_hashes = {key: hashlib.sha256((self.packs['lv1'] / filename).read_bytes()).hexdigest()
                       for key, filename in (('basePoseAtlasSha256', 'atlas-review.png'),
                                             ('basePoseManifestSha256', 'atlas-review.json'))}
        for pack in self.packs.values():
            for slot in SLOTS:
                self.change_manifest(pack / DIRECTORIES[slot] / 'atlas-review.json', **body_hashes)
        write_atlas(self.packs['lv1'] / DIRECTORIES['inner_top'], 'inner_top', 27)
        write_atlas(self.packs['lv10'] / DIRECTORIES['outer_top'], 'outer_top', 28)
        output = self.root / 'overlapping-loadout'
        selection = {slot: 'lv10' if slot == 'outer_top' else 'lv1' for slot in SLOTS}
        compose_loadout(self.packs, selection, output)

        def unpack(directory, pose):
            manifest = json.loads((directory / 'atlas-review.json').read_text())
            part = next(part for part in manifest['sprites'] if part['id'] == pose)
            x, y, width, height = part['atlasRectTopLeft']
            left, top, _, _ = part['sourceCanvasRect']
            canvas = Image.new('RGBA', (256, 384))
            with Image.open(directory / 'atlas-review.png') as atlas:
                canvas.paste(atlas.crop((x, y, x + width, y + height)), (left // 4, top // 4))
            return canvas

        for index, pose in enumerate(poses):
            with self.subTest(pose=pose):
                body = unpack(output, pose)
                inner = unpack(output / DIRECTORIES['inner_top'], pose)
                outer = unpack(output / DIRECTORIES['outer_top'], pose)
                all_on = Image.alpha_composite(Image.alpha_composite(body, inner), outer)
                outer_off = Image.alpha_composite(body, inner)
                # Interior was fully hidden by outer. Its complete blue cloth must survive packing.
                probe = (115 + index, 160, 141 + index, 190)
                for rendered, expected in ((all_on, colors['outer_top']), (outer_off, colors['inner_top']),
                                           (body, colors['body'])):
                    self.assertEqual(rendered.crop(probe).getextrema(), tuple((value, value) for value in expected))


if __name__ == '__main__':
    unittest.main()
