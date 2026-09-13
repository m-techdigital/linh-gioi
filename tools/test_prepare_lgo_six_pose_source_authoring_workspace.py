import json
import tempfile
import unittest
from pathlib import Path

from prepare_lgo_six_pose_source_authoring_workspace import prepare_authoring_workspace
from stage_lgo_six_pose_repair_layers import write_rgba_png


def write_solid_png(path: Path, width=8, height=6, rgba=(20, 30, 40, 255)):
    pixels = bytearray(rgba * (width * height))
    write_rgba_png(path, width, height, pixels)


class SixPoseSourceAuthoringWorkspaceTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.source_root = self.root / "source"
        self.body_root = self.root / "body"
        self.workspace = self.root / "workspace"
        self.source_root.mkdir()
        self.body_root.mkdir()
        for pose in ["idle", "run_a", "jump_tuck"]:
            name = "idle-base-unchanged.png" if pose == "idle" else f"{pose}-review.png"
            write_solid_png(self.body_root / name)
        write_solid_png(self.source_root / "outer-idle" / "outer.png", rgba=(100, 80, 60, 255))

    def test_prepares_workspace_without_writing_packable_repair_layers(self):
        brief = {
            "targets": [
                {
                    "slot": "outer_top",
                    "pose": "run_a",
                    "requiredVariants": ["A", "B"],
                    "requiredComponents": ["front", "back"],
                    "slotGuide": {
                        "center": [4, 3],
                        "shoulderLine": [[2, 2], [6, 2]],
                        "waistLine": [[2, 4], [6, 4]],
                    },
                },
                {
                    "slot": "outer_top",
                    "pose": "jump_tuck",
                    "requiredVariants": ["A", "B"],
                    "requiredComponents": ["front", "back"],
                    "slotGuide": {"center": [4, 3]},
                },
            ]
        }
        repair_plan = {
            "slotRepairs": [
                {
                    "slot": "outer_top",
                    "candidateDirectory": "outer-top-six-pose-source-repair-v1",
                }
            ]
        }
        source_mapping = {"slots": {"outer_top": {"idle": "outer-idle/outer.png"}}}

        report = prepare_authoring_workspace(
            brief,
            repair_plan,
            source_mapping,
            self.body_root,
            self.source_root,
            self.workspace,
            canvas_size=[8, 6],
        )

        self.assertEqual(report["status"], "SOURCE_AUTHORING_WORKSPACE_READY")
        self.assertFalse(report["runtimePromotionAllowed"])
        self.assertTrue((self.workspace / "DO-NOT-PACK.md").exists())
        self.assertTrue((self.workspace / "README.md").exists())
        self.assertIn("Do not pack", (self.workspace / "README.md").read_text())
        self.assertEqual(report["targetCount"], 2)
        first = report["targets"][0]
        self.assertEqual(first["sourceReference"]["relativePath"], "outer-idle/outer.png")
        self.assertTrue(Path(first["bodyReference"]).exists())
        self.assertTrue(Path(first["guideOverlay"]).exists())
        self.assertTrue(Path(first["templates"][0]["path"]).exists())
        jump = report["targets"][1]
        self.assertTrue(jump["requiresBodyAnatomyAcceptance"])
        self.assertIn("DO_NOT_SYNTHESIZE_JUMP_FROM_IDLE_OR_RUN", jump["guardrails"])
        self.assertFalse(
            (self.source_root / "outer-top-six-pose-source-repair-v1" / "A" / "run_a" / "front.png").exists()
        )
        manifest = json.loads((self.workspace / "authoring-workspace-manifest.json").read_text())
        self.assertEqual(manifest["targetCount"], 2)


if __name__ == "__main__":
    unittest.main()
