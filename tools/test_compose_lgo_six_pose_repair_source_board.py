import json
import struct
import tempfile
import unittest
import zlib
from pathlib import Path

from compose_lgo_six_pose_repair_source_board import compose_repair_board
from inspect_lgo_source_png_inventory import inspect_png


POSES = ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"]


def write_rgba_png(path, width=8, height=6, alpha_box=None, color=(0, 0, 0, 255)):
    path.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for y in range(height):
        row = bytearray([0])
        for x in range(width):
            if alpha_box:
                alpha = 255 if alpha_box[0] <= x <= alpha_box[2] and alpha_box[1] <= y <= alpha_box[3] else 0
            else:
                alpha = color[3]
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


class SixPoseRepairSourceBoardTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name) / "source"
        self.root.mkdir()
        self.body = Path(self.temp.name) / "body"
        self.body.mkdir()
        for pose in POSES:
            name = "idle-base-unchanged.png" if pose == "idle" else f"{pose}-review.png"
            write_rgba_png(self.body / name, color=(220, 210, 200, 255))
        write_rgba_png(
            self.root / "outer-top-six-pose-source-repair-v1" / "A" / "idle" / "front.png",
            alpha_box=(2, 2, 4, 4),
            color=(40, 60, 90, 255),
        )
        self.plan = {
            "slotRepairs": [
                {
                    "slot": "outer_top",
                    "candidateDirectory": "outer-top-six-pose-source-repair-v1",
                    "requiredPoses": POSES,
                    "requiredVariants": ["A", "B"],
                    "requiredComponents": ["front", "back"],
                }
            ]
        }

    def test_composes_variant_board_with_missing_layer_report_without_runtime_promotion(self):
        output = Path(self.temp.name) / "board.png"
        report_path = Path(self.temp.name) / "report.json"

        report = compose_repair_board(
            self.root,
            self.body,
            self.plan,
            output,
            report_path,
            variant="A",
            canvas_size=[8, 6],
            panel_scale=1,
        )

        self.assertEqual(report["status"], "SOURCE_REPAIR_BOARD_FIX_REQUIRED")
        self.assertFalse(report["runtimePromotionAllowed"])
        self.assertTrue(output.exists())
        self.assertEqual(inspect_png(output, output.parent, True)["size"], [16, 18])
        self.assertIn("outer_top/run_a/front missing", report["missingLayers"])
        written = json.loads(report_path.read_text())
        self.assertEqual(written["output"], str(output))


if __name__ == "__main__":
    unittest.main()
