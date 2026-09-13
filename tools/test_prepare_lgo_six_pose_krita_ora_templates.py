import json
import tempfile
import unittest
import zipfile
from pathlib import Path
from xml.etree import ElementTree

from prepare_lgo_six_pose_krita_ora_templates import prepare_ora_templates
from stage_lgo_six_pose_repair_layers import write_rgba_png


def write_png(path: Path, width=8, height=6, rgba=(1, 2, 3, 255)):
    write_rgba_png(path, width, height, bytearray(rgba * (width * height)))


class SixPoseKritaOraTemplateTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.workspace = self.root / "workspace"
        target = self.workspace / "targets" / "outer_top" / "run_a"
        target.mkdir(parents=True)
        for name in ["body-reference.png", "guide-overlay-on-body.png", "source-reference.png"]:
            write_png(target / name)
        for name in [
            "A-front-transparent-template.png",
            "A-back-transparent-template.png",
            "B-front-transparent-template.png",
            "B-back-transparent-template.png",
        ]:
            write_png(target / "blank-layer-templates" / name, rgba=(0, 0, 0, 0))
        self.manifest = {
            "status": "SOURCE_AUTHORING_WORKSPACE_READY",
            "runtimePromotionAllowed": False,
            "canvasSize": [8, 6],
            "targets": [
                {
                    "slot": "outer_top",
                    "pose": "run_a",
                    "targetDir": str(target),
                    "bodyReference": str(target / "body-reference.png"),
                    "guideOverlay": str(target / "guide-overlay-on-body.png"),
                    "sourceReference": {"copiedTo": str(target / "source-reference.png")},
                    "templates": [
                        {"variant": "A", "component": "front", "path": str(target / "blank-layer-templates" / "A-front-transparent-template.png")},
                        {"variant": "A", "component": "back", "path": str(target / "blank-layer-templates" / "A-back-transparent-template.png")},
                        {"variant": "B", "component": "front", "path": str(target / "blank-layer-templates" / "B-front-transparent-template.png")},
                        {"variant": "B", "component": "back", "path": str(target / "blank-layer-templates" / "B-back-transparent-template.png")},
                    ],
                }
            ],
        }
        self.manifest_path = self.workspace / "authoring-workspace-manifest.json"
        self.manifest_path.parent.mkdir(parents=True, exist_ok=True)
        self.manifest_path.write_text(json.dumps(self.manifest), encoding="utf-8")

    def test_writes_openraster_template_with_author_layers_and_manifest(self):
        report = prepare_ora_templates(self.manifest_path, self.workspace / "krita-ora-templates")

        self.assertEqual(report["status"], "KRITA_ORA_AUTHORING_TEMPLATES_READY")
        self.assertFalse(report["runtimePromotionAllowed"])
        self.assertEqual(report["templateCount"], 1)
        ora = Path(report["templates"][0]["path"])
        self.assertTrue(ora.exists())
        with zipfile.ZipFile(ora) as archive:
            self.assertEqual(archive.namelist()[0], "mimetype")
            self.assertEqual(archive.read("mimetype"), b"image/openraster")
            stack = ElementTree.fromstring(archive.read("stack.xml"))
            self.assertEqual(stack.attrib["w"], "8")
            names = [node.attrib["name"] for node in stack.findall(".//layer")]
            self.assertIn("AUTHOR - A front", names)
            self.assertIn("AUTHOR - B back", names)
            self.assertIn("GUIDE - overlay on body", names)
        self.assertTrue((self.workspace / "krita-ora-templates" / "krita-ora-template-manifest.json").exists())


if __name__ == "__main__":
    unittest.main()
