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
