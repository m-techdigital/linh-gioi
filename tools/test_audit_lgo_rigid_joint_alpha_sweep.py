import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

from audit_lgo_rigid_joint_alpha_sweep import audit_joint, build


def round_cap_fixture(path: Path, *, parent: bool) -> None:
    image = Image.new("RGBA", (96, 96), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)
    draw.ellipse((28, 28, 68, 68), fill=(255, 255, 255, 255))
    if parent:
        draw.rectangle((40, 8, 56, 48), fill=(255, 255, 255, 255))
    else:
        draw.rectangle((40, 48, 56, 88), fill=(255, 255, 255, 255))
    image.save(path)


def rectangular_split_fixture(path: Path, *, parent: bool) -> None:
    image = Image.new("RGBA", (96, 96), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)
    if parent:
        draw.rectangle((36, 8, 60, 52), fill=(255, 255, 255, 255))
    else:
        draw.rectangle((36, 44, 60, 88), fill=(255, 255, 255, 255))
    image.save(path)


class RigidJointAlphaSweepTests(unittest.TestCase):
    def test_round_parent_underlap_and_child_cap_cover_joint_at_every_angle(self):
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            parent = root / "parent.png"
            child = root / "child.png"
            round_cap_fixture(parent, parent=True)
            round_cap_fixture(child, parent=False)

            result = audit_joint(
                parent,
                child,
                pivot=(48, 48),
                cap_radius=20,
                filter_guard=2,
                rotation_range=(-150, 150),
                angle_step=15,
            )

            self.assertEqual("JOINT_ALPHA_SWEEP_PASS", result["status"])
            self.assertEqual(21, result["sampleCount"])
            self.assertEqual(0, result["parentMissingPixels"])
            self.assertEqual(0, result["maxChildMissingPixels"])

    def test_horizontal_rectangular_overlap_is_rejected_as_rotation_safe_joint(self):
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            parent = root / "parent.png"
            child = root / "child.png"
            rectangular_split_fixture(parent, parent=True)
            rectangular_split_fixture(child, parent=False)

            result = audit_joint(
                parent,
                child,
                pivot=(48, 48),
                cap_radius=20,
                filter_guard=2,
                rotation_range=(-90, 90),
                angle_step=15,
            )

            self.assertEqual("JOINT_ALPHA_SWEEP_FAILED", result["status"])
            self.assertGreater(result["parentMissingPixels"], 0)
            self.assertGreater(result["maxChildMissingPixels"], 0)

    def test_package_build_writes_machine_report_and_visual_contact_sheet(self):
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            parent = root / "parent.png"
            child = root / "child.png"
            round_cap_fixture(parent, parent=True)
            round_cap_fixture(child, parent=False)
            output = root / "evidence"

            report = build(
                {
                    "profileId": "fixture.v1",
                    "joints": [
                        {
                            "id": "elbow_near",
                            "parentPath": str(parent),
                            "childPath": str(child),
                            "pivotPx": [48, 48],
                            "capRadiusPx": 20,
                            "filterGuardPx": 2,
                            "rotationRangeDeg": [-90, 90],
                            "angleStepDeg": 15,
                        }
                    ],
                },
                output,
            )

            self.assertEqual("RIGID_JOINT_ALPHA_SWEEP_PASS", report["status"])
            self.assertEqual(0, report["failureCount"])
            self.assertTrue((output / "joint-alpha-sweep-report.json").is_file())
            with Image.open(output / "joint-alpha-sweep-contact-sheet.png") as board:
                self.assertEqual("RGB", board.mode)
                self.assertGreaterEqual(board.width, 800)


if __name__ == "__main__":
    unittest.main()
