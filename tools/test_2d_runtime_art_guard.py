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
