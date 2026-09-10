import json
import shutil
import tempfile
import unittest
from pathlib import Path
from validate_2d_branch_no_source_images import runtime_allowlist, ROOT
import validate_2d_branch_no_source_images as guard
from unittest.mock import patch

PACK = 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonIllustrated'

class RuntimeArtGuardTests(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.addCleanup(self.tmp.cleanup)
        self.root = Path(self.tmp.name)
        shutil.copytree(ROOT / PACK, self.root / PACK)

    def test_known_new_pack_is_allowed(self):
        self.assertEqual(len(runtime_allowlist(self.root)), 2)


    def test_approved_vo_runtime_art_pack_is_allowed_when_manifest_matches(self):
        pack = 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1ApprovedRuntimeArt'
        pack_dir = self.root / pack
        pack_dir.mkdir(parents=True)
        assets = []
        for name, width, height, role in [
            ('vo-lv1-starter-atlas.png', 2048, 2048, 'paper-doll-atlas'),
            ('vo-lv1-skill-atlas.png', 1024, 1024, 'skill-vfx-atlas'),
        ]:
            raw = b'\x89PNG\r\n\x1a\n' + b'\x00\x00\x00\rIHDR' + width.to_bytes(4, 'big') + height.to_bytes(4, 'big') + b'approved-runtime-art'
            path = pack_dir / name
            path.write_bytes(raw)
            assets.append({
                'path': f'{pack}/{name}',
                'generator': 'approved_original_2d_art',
                'referenceOnly': False,
                'role': role,
                'sha256': __import__('hashlib').sha256(raw).hexdigest(),
            })
        (pack_dir / 'manifest.json').write_text(json.dumps({
            'id': 'vo-lv1-approved-runtime-art-v1',
            'status': 'APPROVED_RUNTIME_ART',
            'assets': assets,
        }))

        allowed = runtime_allowlist(self.root)

        self.assertIn(f'{pack}/vo-lv1-starter-atlas.png', allowed)
        self.assertIn(f'{pack}/vo-lv1-skill-atlas.png', allowed)

    def test_changed_png_is_rejected(self):
        with (self.root / PACK / 'skyline.png').open('ab') as f:
            f.write(b'tamper')
        with self.assertRaises(ValueError):
            runtime_allowlist(self.root)

    def test_reference_or_path_escape_is_rejected(self):
        p = self.root / PACK / 'manifest.json'
        original = json.loads(p.read_text())
        for key, value in [('path', '../../concept.png'), ('referenceOnly', True), ('generator', 'old-board')]:
            data = json.loads(json.dumps(original))
            data['assets'][0][key] = value
            p.write_text(json.dumps(data))
            with self.assertRaises(ValueError, msg=key):
                runtime_allowlist(self.root)

    def test_unlisted_reference_image_remains_blocked(self):
        rogue = self.root / 'docs/reference/old-board.png'
        rogue.parent.mkdir(parents=True)
        rogue.write_bytes(b'not an allowed runtime image')
        with patch.object(guard, 'ROOT', self.root):
            self.assertEqual(guard.main(), 1)

    def test_missing_pack_texture_is_rejected(self):
        (self.root / PACK / 'skyline.png').unlink()
        with self.assertRaises(ValueError):
            runtime_allowlist(self.root)

if __name__ == '__main__':
    unittest.main()
