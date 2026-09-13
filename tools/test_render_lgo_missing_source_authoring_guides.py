import json
import struct
import tempfile
import unittest
import zlib
from pathlib import Path

from inspect_lgo_source_png_inventory import inspect_png
from render_lgo_missing_source_authoring_guides import render_authoring_guides


def write_rgba_png(path, width=8, height=6, color=(230, 220, 210, 255), alpha_box=None):
    path.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for _y in range(height):
        row = bytearray([0])
        for _x in range(width):
            alpha = color[3]
            if alpha_box:
                alpha = 255 if alpha_box[0] <= _x <= alpha_box[2] and alpha_box[1] <= _y <= alpha_box[3] else 0
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


class MissingSourceAuthoringGuideRenderTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.body = self.root / "body"
        self.body.mkdir()
        for pose in ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"]:
            name = "idle-base-unchanged.png" if pose == "idle" else f"{pose}-review.png"
            write_rgba_png(self.body / name, alpha_box=(2, 1, 5, 4))

    def brief(self):
        return {
            "targets": [
                {
                    "slot": "outer_top",
                    "pose": "run_a",
                    "slotGuide": {
                        "center": [4, 3],
                        "shoulderLine": [[2, 2], [6, 2]],
                        "waistLine": [[2, 4], [6, 4]],
                    },
                },
                {
                    "slot": "waist_belt",
                    "pose": "jump_tuck",
                    "slotGuide": {
                        "center": [4, 3],
                        "line": [[2, 3], [6, 3]],
                    },
                },
            ]
        }

    def test_renders_body_board_with_guide_lines_and_report(self):
        output = self.root / "guide-board.png"
        report_path = self.root / "report.json"

        report = render_authoring_guides(
            self.brief(),
            self.body,
            output,
            report_path,
            canvas_size=[8, 6],
            panel_scale=1,
        )

        self.assertEqual(report["status"], "MISSING_SOURCE_AUTHORING_GUIDE_BOARD_READY")
        self.assertFalse(report["runtimePromotionAllowed"])
        self.assertTrue(output.exists())
        info = inspect_png(output, output.parent, True)
        self.assertEqual(info["size"], [16, 18])
        self.assertEqual(info["nonzeroAlpha"], 16 * 18)
        self.assertEqual(report["targetCount"], 2)
        self.assertEqual(json.loads(report_path.read_text())["output"], str(output))


if __name__ == "__main__":
    unittest.main()
