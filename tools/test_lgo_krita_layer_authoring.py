"""Reject authoring jobs that could lose pose registration or locked sources."""
import copy
import hashlib
import importlib.util
import tempfile
import unittest
from pathlib import Path

from PIL import Image


class KritaAuthoringPreflightTests(unittest.TestCase):
    def setUp(self):
        spec = importlib.util.find_spec('lgo_krita_layer_authoring')
        self.assertIsNotNone(spec, 'Native authoring adapter is not implemented')
        self.module = __import__('lgo_krita_layer_authoring')
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.output = self.root / 'candidate'
        image = Image.new('RGBA', (1024, 1536))
        image.putpixel((311, 729), (83, 52, 147, 151))
        self.source = self.root / 'source.png'
        image.save(self.source)
        ref = {'path': str(self.source), 'sha256': hashlib.sha256(self.source.read_bytes()).hexdigest()}
        poses = ('idle', 'run_contact_a', 'run_a', 'run_contact_b', 'run_b', 'jump_tuck')
        self.job = {
            'candidateId': 'phap_male_inner_top_lv001_layer_v1',
            'slot': 'inner_top', 'gender': 'male', 'fitFamily': 'vo_male_v3',
            'sourceSpaceProfile': 'lgo_character_canvas_1024x1536_v1',
            'sourceCanvas': [1024, 1536], 'jumpPivotSource': [512, 820],
            'layerOrderProfile': 'lgo_complete_garment_layers_v1',
            'sourceStatus': 'SOURCE_REVIEW_REQUIRED',
            'designReferences': [ref],
            'poses': {pose: {'body': ref, 'front': ref, 'back': ref} for pose in poses},
            'variantB': {'h': 12, 's': -15, 'v': 0},
        }

    def test_accepts_source_repair_without_promoting_and_does_not_write(self):
        self.module.validate_job(self.job, self.output)
        self.assertFalse(self.output.exists())

    def test_revision_preflight_rejects_changed_native_and_wrong_format(self):
        native = self.root / 'edited.kra'
        native.write_bytes(b'edited native fixture')
        digest = self.module.fingerprint(native)
        self.module.validate_revision(self.job, native, digest, self.output)
        native.write_bytes(b'changed after selection')
        with self.assertRaisesRegex(ValueError, 'Native fingerprint'):
            self.module.validate_revision(self.job, native, digest, self.output)
        with self.assertRaisesRegex(ValueError, 'KRA'):
            self.module.validate_revision(self.job, self.source,
                                          self.module.fingerprint(self.source), self.output)
        self.assertFalse(self.output.exists())

    def test_rejects_changed_body_hash_before_creating_output(self):
        job = copy.deepcopy(self.job)
        job['poses']['jump_tuck']['body']['sha256'] = '0' * 64
        with self.assertRaisesRegex(ValueError, 'fingerprint'):
            self.module.validate_job(job, self.output)
        self.assertFalse(self.output.exists())

    def test_rejects_missing_pose_wrong_canvas_and_pivot_drift(self):
        mutations = [lambda j: j['poses'].pop('jump_tuck'),
                     lambda j: j.update(sourceCanvas=[1024, 1024]),
                     lambda j: j.update(jumpPivotSource=[512, 900]),
                     lambda j: j.update(layerOrderProfile='legacy'),
                     lambda j: j.update(slot='body')]
        for mutate in mutations:
            job = copy.deepcopy(self.job)
            mutate(job)
            with self.subTest(job=job), self.assertRaises(ValueError):
                self.module.validate_job(job, self.output)

    def test_rejects_output_aliasing_locked_source_or_existing_directory(self):
        alias = self.root / 'alias'
        alias.symlink_to(self.root, target_is_directory=True)
        for output in (self.root, alias, self.source):
            with self.subTest(output=output), self.assertRaises((ValueError, FileExistsError)):
                self.module.validate_job(self.job, output)

    def test_rejects_reference_substituted_for_component_role(self):
        job = copy.deepcopy(self.job)
        job['poses']['idle']['reference'] = job['poses']['idle'].pop('front')
        with self.assertRaises(ValueError):
            self.module.validate_job(job, self.output)

    def test_rejects_transparent_required_component_but_allows_declared_occlusion(self):
        Image.new('RGBA', (1024, 1536)).save(self.source)
        empty = {'path': str(self.source), 'sha256': hashlib.sha256(self.source.read_bytes()).hexdigest()}
        job = copy.deepcopy(self.job)
        # PNG emptiness is checked inside Krita; declaration itself must be explicit.
        job['poses']['idle']['front'] = dict(empty, emptyReason='')
        with self.assertRaises(ValueError):
            self.module.validate_job(job, self.output)

    def test_rejects_raw_inputs_with_wrong_dimensions(self):
        Image.new('RGBA', (512, 768)).save(self.source)
        digest = hashlib.sha256(self.source.read_bytes()).hexdigest()
        job = copy.deepcopy(self.job)
        for pose in job['poses'].values():
            for ref in pose.values(): ref['sha256'] = digest
        for ref in job['designReferences']: ref['sha256'] = digest
        with self.assertRaisesRegex(ValueError, 'canvas'):
            self.module.validate_job(job, self.output)

    def test_rejects_rigid_mapping_for_cloth_or_nonrigid_parameters(self):
        transform = {'sourceAnchor': [585, 570], 'targetAnchor': [410, 744], 'angleDegrees': 35}
        self.job['rigidPoseTransforms'] = {pose: transform for pose in self.job['poses']}
        with self.assertRaises(ValueError):
            self.module.validate_job(self.job, self.output)
        self.job['slot'] = 'class_accessory'
        self.job['rigidPoseTransforms']['idle'] = dict(transform, scaleX=0.8)
        with self.assertRaises(ValueError):
            self.module.validate_job(self.job, self.output)

    def test_revision_rejects_nonrigid_accessory_transform(self):
        xml = '<transform_params><data mode="0"><free_transform>'
        for key, value in dict(scaleX=1, scaleY=1, shearX=0, shearY=0, aX=0, aY=0).items():
            xml += '<' + key + ' value="' + str(value) + '"/>'
        xml += '<flattenedPerspectiveTransform m11="1" m12="0" m13="0" m21="0" m22="1" m23="0" m31="0" m32="0" m33="1"/></free_transform></data></transform_params>'
        self.module.validate_rigid_mask(xml)
        for bad in (xml.replace('scaleX value="1"', 'scaleX value="0.8"'),
                    xml.replace('mode="0"', 'mode="1"'),
                    xml.replace('m13="0"', 'm13="0.1"')):
            with self.assertRaisesRegex(ValueError, 'Rigid'):
                self.module.validate_rigid_mask(bad)

    def test_rigid_transform_preserves_identity_scale_and_serializes_authored_anchor(self):
        from xml.etree import ElementTree as E
        xml = '<transform_params><data><free_transform><originalCenter x="0" y="0"/><transformedCenter x="0" y="0"/><aZ value="0"/><scaleX value="1"/><scaleY value="1"/></free_transform></data></transform_params>'
        result = self.module.rigid_transform_xml(xml, {'sourceAnchor': [585, 570],
                  'targetAnchor': [410, 744], 'angleDegrees': 90})
        root = E.fromstring(result)
        self.assertEqual(root.find('.//originalCenter').attrib, {'x': '585', 'y': '570'})
        self.assertEqual(root.find('.//transformedCenter').attrib, {'x': '410', 'y': '744'})
        self.assertAlmostEqual(float(root.find('.//aZ').get('value')), 1.5707963267948966)
        self.assertEqual(root.find('.//scaleX').get('value'), '1')
        self.assertEqual(root.find('.//scaleY').get('value'), '1')


if __name__ == '__main__':
    unittest.main()
