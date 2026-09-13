import io
import tempfile
import unittest
import zipfile
from pathlib import Path

from PIL import Image

from audit_lgo_ora_author_layers import audit_ora_author_layers


class OraAuthorLayerAuditTests(unittest.TestCase):
    def _ora(self, author_alpha):
        temporary = tempfile.TemporaryDirectory()
        path = Path(temporary.name) / "source.ora"
        image = Image.new("RGBA", (1024, 1536), (10, 20, 30, author_alpha))
        buffer = io.BytesIO()
        image.save(buffer, format="PNG")
        stack = (
            '<image w="1024" h="1536" name="fixture"><stack>'
            '<layer name="AUTHOR - torso" src="data/layer0.png" />'
            '</stack></image>'
        )
        with zipfile.ZipFile(path, "w") as archive:
            archive.writestr("mimetype", "image/openraster")
            archive.writestr("stack.xml", stack)
            archive.writestr("data/layer0.png", buffer.getvalue())
        return temporary, path

    def test_accepts_non_empty_registered_author_layer(self):
        temporary, path = self._ora(255)
        self.addCleanup(temporary.cleanup)
        report = audit_ora_author_layers(path)
        self.assertEqual("PASS", report["status"])
        self.assertGreater(report["layers"][0]["nonTransparentPixels"], 0)

    def test_rejects_named_but_empty_author_layer(self):
        temporary, path = self._ora(0)
        self.addCleanup(temporary.cleanup)
        report = audit_ora_author_layers(path)
        self.assertEqual("REJECT_EMPTY_AUTHORING_LAYERS", report["status"])
        self.assertIn("EMPTY_AUTHOR_LAYER", report["layers"][0]["failures"])


if __name__ == "__main__":
    unittest.main()
