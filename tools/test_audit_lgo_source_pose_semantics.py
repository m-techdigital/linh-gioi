import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image

from audit_lgo_source_pose_semantics import POSES, SLOT_DIRS, audit_pack


class SourcePoseSemanticAuditTests(unittest.TestCase):
    def make_pack(self, accessory_width=20):
        temp = tempfile.TemporaryDirectory(); self.addCleanup(temp.cleanup)
        root = Path(temp.name); source = root / 'source'; pack = root / 'pack'; source.mkdir(); pack.mkdir()
        body_parts = []
        for pose in POSES:
            path = source / f'body-{pose}.png'; Image.new('RGBA', (1024,1536), (0,0,0,0)).save(path)
            im=Image.open(path); px=im.load()
            for y in range(300,1300):
                for x in range(400,600): px[x,y]=(255,200,160,255)
            im.save(path); body_parts.append({'id':pose,'source':str(path)})
        (pack/'atlas-review.json').write_text(json.dumps({'samplingDivisor':4,'sprites':body_parts}))
        for index,directory in enumerate(SLOT_DIRS):
            target=pack/directory; target.mkdir(); parts=[]
            for pose in POSES:
                path=source/f'{directory}-{pose}.png'; im=Image.new('RGBA',(1024,1536),(0,0,0,0)); px=im.load()
                width=accessory_width if directory=='class-accessory-review' else 10
                start_x = 50 if width > 700 else 300
                for y in range(500+index*3,550+index*3):
                    for x in range(start_x,start_x+width): px[x,y]=(20,80,220,255)
                im.save(path); parts.append({'id':pose,'source':str(path)})
            (target/'atlas-review.json').write_text(json.dumps({'sprites':parts,
                'basePoseAtlasSha256':'a','basePoseManifestSha256':'b','unlockLevel':1,
                'itemId':f'kiem_male_lv001_{directory}'}))
        return pack

    def test_flags_accessory_that_owns_a_large_fraction_of_the_character(self):
        result=audit_pack('bad',self.make_pack(accessory_width=900))
        self.assertEqual(result['status'],'FIX_REQUIRED')
        self.assertTrue(any(error.startswith('CLASS_ACCESSORY_OWNS_OUTFIT') for error in result['errors']))

    def test_accepts_small_semantic_accessory_on_one_shared_body(self):
        self.assertEqual(audit_pack('good',self.make_pack())['status'],'PASS')


if __name__ == '__main__': unittest.main()
