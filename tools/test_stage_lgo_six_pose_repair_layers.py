import json
import struct
import tempfile
import unittest
import zlib
from pathlib import Path

from audit_lgo_six_pose_repair_source_layers import audit_repair_layers
from inspect_lgo_source_png_inventory import inspect_png
from stage_lgo_six_pose_repair_layers import stage_repair_layers


POSES = ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"]


def write_rgba_png(path, width=8, height=6, alpha_box=(2, 2, 4, 4), color=(50, 70, 90)):
    path.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for y in range(height):
        row = bytearray([0])
        for x in range(width):
            alpha = 255 if (
                alpha_box and alpha_box[0] <= x <= alpha_box[2] and alpha_box[1] <= y <= alpha_box[3]
            ) else 0
            row.extend([color[0], color[1], color[2], alpha])
        rows.append(bytes(row))

    def chunk(kind, data):
        payload = kind + data
        return struct.pack(">I", len(data)) + payload + struct.pack(">I", zlib.crc32(payload) & 0xFFFFFFFF)

    path.write_bytes(
        b"".join(
            [
                b"\x89PNG\r\n\x1a\n",
                chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)),
                chunk(b"IDAT", zlib.compress(b"".join(rows), level=9)),
                chunk(b"IEND", b""),
            ]
        )
    )


class StageSixPoseRepairLayersTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.plan = {
            "slotRepairs": [
                {
                    "slot": "outer_top",
                    "candidateDirectory": "outer-top-six-pose-source-repair-v1",
                    "requiredPoses": POSES,
                    "requiredVariants": ["A", "B"],
                    "requiredComponents": ["front", "back"],
                },
                {
                    "slot": "waist_belt",
                    "candidateDirectory": "waist-belt-six-pose-source-repair-v1",
                    "requiredPoses": POSES,
                    "requiredVariants": ["A", "B"],
                    "requiredComponents": ["front", "back"],
                },
            ]
        }
        self.mapping = {
            "outer_top": {
                "idle": "outer-top-material-idle-v2/outer-top-idle-material.png",
            },
            "waist_belt": {
                "idle": "waist-belt-material-idle-v2/waist-belt-idle-material-export.png",
                "run_a": "waist-belt-run-four-material-v1/run_a-waist-belt-material-export.png",
            },
        }
        for relative in [
            "outer-top-material-idle-v2/outer-top-idle-material.png",
            "waist-belt-material-idle-v2/waist-belt-idle-material-export.png",
            "waist-belt-run-four-material-v1/run_a-waist-belt-material-export.png",
        ]:
            write_rgba_png(self.root / relative)

    def test_stages_available_clean_front_layers_and_leaves_missing_poses_missing(self):
        report = stage_repair_layers(self.root, self.plan, self.mapping, canvas_size=[8, 6])

        self.assertEqual(report["status"], "PARTIAL_SOURCE_REPAIR_DRAFT_STAGED")
        self.assertFalse(report["runtimePromotionAllowed"])
        self.assertEqual(report["slots"]["outer_top"]["stagedPoseCount"], 1)
        self.assertEqual(report["slots"]["waist_belt"]["stagedPoseCount"], 2)
        self.assertIn("outer_top/run_a missing clean source", report["missingSources"])
        self.assertFalse(
            (self.root / "outer-top-six-pose-source-repair-v1" / "A" / "run_a" / "front.png").exists()
        )

        a_front = self.root / "waist-belt-six-pose-source-repair-v1" / "A" / "run_a" / "front.png"
        b_front = self.root / "waist-belt-six-pose-source-repair-v1" / "B" / "run_a" / "front.png"
        a_back = self.root / "waist-belt-six-pose-source-repair-v1" / "A" / "run_a" / "back.png"
        self.assertTrue(a_front.exists())
        self.assertTrue(b_front.exists())
        self.assertTrue(a_back.exists())
        self.assertNotEqual(a_front.read_bytes(), b_front.read_bytes())
        self.assertEqual(inspect_png(a_front, self.root, True)["alphaBBox"], [2, 2, 4, 4])
        self.assertEqual(inspect_png(b_front, self.root, True)["alphaBBox"], [2, 2, 4, 4])
        self.assertEqual(inspect_png(a_back, self.root, True)["nonzeroAlpha"], 0)

    def test_staged_layers_still_fail_repair_gate_until_all_required_poses_exist(self):
        stage_repair_layers(self.root, self.plan, self.mapping, canvas_size=[8, 6])

        audit = audit_repair_layers(self.root, self.plan, canvas_size=[8, 6])

        self.assertEqual(audit["status"], "SOURCE_REPAIR_LAYER_GAP")
        self.assertIn("outer_top/A/run_contact_a/front.png missing", audit["failures"])
        self.assertFalse(audit["runtimePromotionAllowed"])


if __name__ == "__main__":
    unittest.main()
