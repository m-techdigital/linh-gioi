import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

from audit_lgo_source_pose_semantics import POSES, SLOT_DIRS, audit_pack
from compose_lgo_pose_review_loadout import digest


class SourcePoseSemanticAuditTests(unittest.TestCase):
    def make_pack(self, accessory_width=20, gender='male', family='vo_male_v3'):
        temp = tempfile.TemporaryDirectory(); self.addCleanup(temp.cleanup)
        root = Path(temp.name); source = root / 'source'; pack = root / 'pack'; source.mkdir(); pack.mkdir()
        path = source / 'body.png'; image = Image.new('RGBA', (1024, 1536))
        ImageDraw.Draw(image).rectangle((400, 300, 599, 1299), fill=(255, 200, 160, 255)); image.save(path)
        body_parts = [{'id': pose, 'source': str(path)} for pose in POSES]
        (pack / 'atlas-review.png').write_bytes(b'body-atlas')
        (pack/'atlas-review.json').write_text(json.dumps({'samplingDivisor':4,'sprites':body_parts,
            'atlasSha256': digest(pack / 'atlas-review.png'), 'fitFamily': family, 'gender': gender}))
        body_hashes = {'basePoseAtlasSha256': digest(pack / 'atlas-review.png'),
                       'basePoseManifestSha256': digest(pack / 'atlas-review.json')}
        for index,directory in enumerate(SLOT_DIRS):
            target=pack/directory; target.mkdir()
            path=source/f'{directory}.png'; image=Image.new('RGBA',(1024,1536))
            width=accessory_width if directory=='class-accessory-review' else 10
            start_x = 50 if width > 700 else 300
            ImageDraw.Draw(image).rectangle((start_x, 500+index*3, start_x+width-1, 549+index*3),
                                            fill=(20,80,220,255)); image.save(path)
            parts = [{'id': pose, 'source': str(path)} for pose in POSES]
            (target / 'atlas-review.png').write_bytes(directory.encode())
            (target/'atlas-review.json').write_text(json.dumps({'sprites':parts,
                **body_hashes, 'unlockLevel':1, 'fitFamily': family,
                'atlasSha256': digest(target / 'atlas-review.png'),
                'itemId':f'kiem_{gender}_lv001_{directory}'}))
        return pack

    def change_slot(self, pack, directory, **changes):
        path = pack / directory / 'atlas-review.json'
        manifest = json.loads(path.read_text()); manifest.update(changes)
        path.write_text(json.dumps(manifest))

    def test_flags_accessory_that_owns_a_large_fraction_of_the_character(self):
        result=audit_pack('bad',self.make_pack(accessory_width=900))
        self.assertEqual(result['status'],'FIX_REQUIRED')
        self.assertTrue(any(error.startswith('CLASS_ACCESSORY_OWNS_OUTFIT') for error in result['errors']))

    def test_accepts_small_semantic_accessory_on_one_shared_body(self):
        self.assertEqual(audit_pack('good',self.make_pack())['status'],'PASS')

    def test_accepts_mixed_levels_on_declared_female_body_authority(self):
        pack = self.make_pack(gender='female', family='common_female_v1')
        self.change_slot(pack, 'outer-top-review', unlockLevel=10, itemId='kiem_female_lv010_outer_top')
        result = audit_pack('mixed', pack)
        self.assertEqual(result['status'], 'PASS')
        self.assertEqual(result['levels'], [1, 10])
        self.assertIsNone(result['level'])
        self.assertEqual((result['gender'], result['fitFamily']), ('female', 'common_female_v1'))

    def test_compares_slot_body_claims_against_actual_body_not_only_each_other(self):
        pack = self.make_pack()
        for directory in SLOT_DIRS:
            self.change_slot(pack, directory, basePoseAtlasSha256='same-wrong-hash')
        result = audit_pack('false-authority', pack)
        self.assertEqual(sum(error.startswith('SLOT_BODY_AUTHORITY_MISMATCH') for error in result['errors']), 10)

    def test_detects_modified_body_and_slot_atlas_bytes(self):
        pack = self.make_pack()
        (pack / 'atlas-review.png').write_bytes(b'changed-body')
        (pack / 'inner-top-review/atlas-review.png').write_bytes(b'changed-item')
        result = audit_pack('tampered', pack)
        self.assertIn('BODY_ATLAS_HASH_MISMATCH', result['errors'])
        self.assertIn('SLOT_ATLAS_HASH_MISMATCH:inner-top-review', result['errors'])

    def test_rejects_profile_gender_and_class_mismatch(self):
        pack = self.make_pack()
        self.change_slot(pack, 'inner-top-review', fitFamily='different_profile')
        self.change_slot(pack, 'outer-top-review', itemId='kiem_female_lv001_outer_top')
        self.change_slot(pack, 'footwear-review', itemId='phap_male_lv001_footwear')
        errors = audit_pack('mismatch', pack)['errors']
        self.assertIn('SLOT_FIT_FAMILY_MISMATCH:inner-top-review', errors)
        self.assertIn('SLOT_GENDER_MISMATCH:outer-top-review', errors)
        self.assertIn('CLASS_ID_INCONSISTENT', errors)


if __name__ == '__main__': unittest.main()
