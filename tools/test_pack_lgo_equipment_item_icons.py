"""Exact item identity, provenance and additive atlas regression tests."""
import copy
import hashlib
import json
from pathlib import Path
import tempfile
import unittest
from PIL import Image, ImageDraw
import pack_lgo_equipment_item_icons as packer

class EquipmentItemIconPackingTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        (self.root/'review.txt').write_text('Self-reviewed candidate; not owner acceptance')
        self.base = self.root/'base'; self.base.mkdir()
        image = Image.new('RGBA',(640,256),(20,40,60,255))
        image.save(self.base/packer.PNG)
        self.meta = {'id':'map01a-character-equipment-icons-v1','sha256':packer.sha(self.base/packer.PNG),'parts':[{'id':'old','x':0,'y':0,'w':128,'h':128}],'itemBindings':[],'assets':[]}
        (self.base/'manifest.json').write_text(json.dumps(self.meta))
        image = Image.new('RGBA',(384,384));ImageDraw.Draw(image).rectangle((96,96,288,288),fill=(180,130,60,255))
        image.save(self.root/'art.png');image.convert('RGB').save(self.root/'demo.png')
        self.item = {'itemId':'vo_waist_lv001','classId':'vo','slot':'waist','gender':'male','level':1,'iconId':'vo_male_lv001_waist','source':'art.png','sourceSha256':packer.sha(self.root/'art.png'),'designSource':'demo.png','designSha256':packer.sha(self.root/'demo.png'),'designRect':[10,10,80,80]}
    def run_pack(self, items=None, name='out'):
        registry=self.root/'registry.json';registry.write_text(json.dumps({'items':items if items is not None else [self.item], 'review':{'status':'SELF_REVIEWED','evidence':'review.txt','sha256':packer.sha(self.root/'review.txt')}}))
        return packer.run(self.base,registry,self.root/name)
    def test_exact_binding_and_legacy_pixels_survive(self):
        meta=self.run_pack();self.assertEqual([640,384],meta['textureSize'])
        self.assertEqual('vo_waist_lv001',meta['itemBindings'][0]['itemId'])
        self.assertEqual(128,meta['parts'][0]['y'])
        old=Image.open(self.base/packer.PNG);new=Image.open(self.root/'out'/packer.PNG)
        self.assertEqual(old.tobytes(),new.crop((0,0,640,256)).tobytes())
        self.assertFalse(meta['runtimeApproved'])
    def test_replay_is_byte_identical(self):
        self.run_pack(name='a');self.run_pack(name='b')
        for name in (packer.PNG,'manifest.json'):
            self.assertEqual((self.root/'a'/name).read_bytes(),(self.root/'b'/name).read_bytes())
    def test_invalid_identity_and_provenance_rejected_before_write(self):
        mutations={'classId':['','kiem ','*'],'itemId':['','*'],'gender':['', 'unknown'],'level':[0,-1,True], 'sourceSha256':['0'*64],'designSha256':['0'*64],'designRect':[[-1,0,8,8],[380,0,8,8]]}
        for field,values in mutations.items():
            for value in values:
                with self.subTest(field=field,value=value):
                    item=dict(self.item);item[field]=value
                    with self.assertRaises(ValueError):self.run_pack([item])
                    self.assertFalse((self.root/'out').exists())
    def test_duplicate_item_and_icon_ids_rejected(self):
        for duplicate in [self.item,dict(self.item,itemId='vo_other_lv001'),dict(self.item,iconId='different')]:
            with self.subTest(duplicate=duplicate):
                with self.assertRaises(ValueError):self.run_pack([self.item,duplicate])
    def test_source_canvas_and_alpha_are_required(self):
        for mode,size in [('RGB',(384,384)),('RGBA',(128,128))]:
            Image.new(mode,size,(1,2,3)).save(self.root/'art.png');item=dict(self.item,sourceSha256=packer.sha(self.root/'art.png'))
            with self.assertRaises(ValueError):self.run_pack([item])
    def test_preserves_existing_item_bindings(self):
        original={'classId':'kiem','itemId':'kiem_example','slot':'waist','gender':'female','level':10,'iconId':'old'}
        self.meta['itemBindings']=[original];(self.base/'manifest.json').write_text(json.dumps(self.meta))
        self.assertEqual(original,self.run_pack()['itemBindings'][0])

    def test_bad_base_and_nonempty_output_are_preserved(self):
        self.run_pack();before={p.name:p.read_bytes() for p in (self.root/'out').iterdir()}
        with self.assertRaises(ValueError):self.run_pack()
        self.assertEqual(before,{p.name:p.read_bytes() for p in (self.root/'out').iterdir()})
        self.meta['sha256']='0'*64;(self.base/'manifest.json').write_text(json.dumps(self.meta))
        with self.assertRaises(ValueError):self.run_pack(name='badbase')
        self.assertFalse((self.root/'badbase').exists())
    def test_fixed_viewport_rejects_content_outside_it(self):
        im=Image.open(self.root/'art.png');ImageDraw.Draw(im).rectangle((5,5,15,15),fill=(255,255,255,255));im.save(self.root/'art.png')
        item=dict(self.item,sourceSha256=packer.sha(self.root/'art.png'))
        with self.assertRaises(ValueError):self.run_pack([item])
        self.assertFalse((self.root/'out').exists())
    def test_shared_canvas_projection_not_per_item_fit(self):
        self.run_pack()
        with Image.open(self.root/'art.png') as src,Image.open(self.root/'out'/packer.PNG) as atlas:
            expected=src.crop((64,64,320,320)).resize((120,120),Image.Resampling.LANCZOS)
            self.assertEqual(expected.tobytes(),atlas.crop((4,260,124,380)).tobytes())

    def test_unreviewed_registry_cannot_promote_content(self):
        reg=self.root/'unreviewed.json';reg.write_text(json.dumps({'items':[self.item]}))
        with self.assertRaises(ValueError):packer.run(self.base,reg,self.root/'out')
        self.assertFalse((self.root/'out').exists())

    def test_runtime_budget_requires_consistent_registration_metadata(self):
        import validate_2d_branch_no_source_images as guard
        meta=self.run_pack();spec=next(s for s in guard.RUNTIME_ART_PACKS if s['id']=='map01a-character-equipment-icons-v1')
        checked=guard._equipment_intake_spec(spec,meta)
        self.assertEqual((640,384,'ui-equipment-icon-atlas'),checked['assets'][packer.PNG])
        for field,bad in [('textureSize',[640,512]),('itemBindings',[]),('itemDesignBindings',[]),('runtimeApproved',True)]:
            with self.subTest(field=field):
                changed=copy.deepcopy(meta);changed[field]=bad
                with self.assertRaises(ValueError):guard._equipment_intake_spec(spec,changed)

if __name__ == '__main__':unittest.main()
