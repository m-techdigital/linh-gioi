import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw


class VoRigBatchTests(unittest.TestCase):
    def test_cli_extracts_ten_components_in_grid_order(self):
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            source = root / "source.png"
            image = Image.new("RGBA", (500, 200), (0, 0, 0, 0))
            draw = ImageDraw.Draw(image)
            for row in range(2):
                for column in range(5):
                    left = column * 100 + 20
                    top = row * 100 + 20
                    draw.rectangle((left, top, left + 40 + column, top + 55), fill=(30 + row, 40 + column, 50, 255))
            image.save(source)

            result = subprocess.run([
                sys.executable, str(Path(__file__).with_name("prepare_lgo_vo_rig_batch.py")),
                "--source", str(source), "--output-dir", str(root / "parts"),
                "--report", str(root / "report.json"), "--min-pixels", "100",
            ], capture_output=True, text=True)

            self.assertEqual(result.returncode, 0, result.stderr)
            parts = sorted((root / "parts").glob("*.png"))
            self.assertEqual(len(parts), 10)
            self.assertEqual(parts[0].name, "01-head.png")
            self.assertEqual(parts[-1].name, "10-right-shin-foot.png")
            for path in parts:
                with Image.open(path) as part:
                    self.assertEqual(part.size, (512, 512))


if __name__ == "__main__":
    unittest.main()
