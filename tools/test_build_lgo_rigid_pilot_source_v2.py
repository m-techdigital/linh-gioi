import tempfile
import unittest
from pathlib import Path

from PIL import Image

from build_lgo_rigid_pilot_source_v2 import SPLITS, build


class RigidPilotSourceV2Tests(unittest.TestCase):
    def test_split_keeps_canvas_alpha_and_overlap_without_pose_variants(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            input_dir = root / "input"
            output_dir = root / "output"
            input_dir.mkdir()
            for source_id, *_ in SPLITS:
                image = Image.new("RGBA", (40, 100), (220, 130, 90, 255))
                image.save(input_dir / f"{source_id}.png")

            report = build(input_dir, output_dir)

            self.assertEqual("RIGID_NATIVE_SOURCE_V2_READY", report["status"])
            self.assertEqual(16, report["moduleCount"])
            self.assertFalse(report["perPoseAssets"])
            for module in report["modules"]:
                self.assertEqual("RGBA", module["mode"])
                self.assertEqual([0, 255], module["alphaExtrema"])
                self.assertGreaterEqual(module["overlapPixels"], 32)


if __name__ == "__main__":
    unittest.main()
