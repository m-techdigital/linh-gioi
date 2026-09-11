import hashlib
import json
import tempfile
import unittest
from pathlib import Path

from compose_lgo_pose_review_loadout import DIRECTORIES, SLOTS, compose_loadout


class ComposePoseReviewLoadoutTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.packs = {name: self.make_pack(name, level) for name, level in (('lv1', 1), ('lv10', 10))}

    def tearDown(self):
        self.temp.cleanup()

    def make_pack(self, alias, level):
        root = self.root / alias
        root.mkdir()
        (root / 'atlas-review.png').write_bytes(b'approved-body-atlas')
        (root / 'atlas-review.json').write_text('{"status":"REVIEW_ONLY"}\n')
        body_atlas = hashlib.sha256((root / 'atlas-review.png').read_bytes()).hexdigest()
        body_manifest = hashlib.sha256((root / 'atlas-review.json').read_bytes()).hexdigest()
        for slot in SLOTS:
            directory = root / DIRECTORIES[slot]
            directory.mkdir()
            atlas = (alias + slot).encode()
            (directory / 'atlas-review.png').write_bytes(atlas)
            manifest = {'reviewSlot': slot, 'fitFamily': 'vo_male_v3',
                        'basePoseAtlasSha256': body_atlas, 'basePoseManifestSha256': body_manifest,
                        'itemId': f'vo_male_lv{level:03d}_{slot}', 'unlockLevel': level,
                        'atlasSha256': hashlib.sha256(atlas).hexdigest()}
            (directory / 'atlas-review.json').write_text(json.dumps(manifest))
        return root

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


if __name__ == '__main__':
    unittest.main()
