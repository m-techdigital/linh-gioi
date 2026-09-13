import json
import struct
import tempfile
import unittest
import zlib
from pathlib import Path

from audit_lgo_six_pose_repair_source_layers import audit_repair_layers


POSES = ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"]


def write_rgba_png(path, width=8, height=6, alpha_box=(2, 2, 4, 4), full_alpha=False):
    path.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for y in range(height):
        row = bytearray([0])
        for x in range(width):
            alpha = 255 if full_alpha or (
                alpha_box and alpha_box[0] <= x <= alpha_box[2] and alpha_box[1] <= y <= alpha_box[3]
            ) else 0
            row.extend([10, 20, 30, alpha])
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


class SixPoseRepairSourceLayerAuditTests(unittest.TestCase):
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
                }
            ]
        }

    def write_complete_slot(self):
        for variant in ("A", "B"):
            for pose in POSES:
                write_rgba_png(
                    self.root
                    / "outer-top-six-pose-source-repair-v1"
                    / variant
                    / pose
                    / "front.png"
                )
                write_rgba_png(
                    self.root
                    / "outer-top-six-pose-source-repair-v1"
                    / variant
                    / pose
                    / "back.png",
                    alpha_box=None,
                )

    def test_passes_complete_registered_layers_without_runtime_promotion(self):
        self.write_complete_slot()

        report = audit_repair_layers(self.root, self.plan, canvas_size=[8, 6])

        self.assertEqual(report["status"], "SOURCE_REPAIR_LAYER_READY_FOR_VISUAL_BOARD")
        self.assertEqual(report["failures"], [])
        self.assertFalse(report["runtimePromotionAllowed"])
        self.assertEqual(report["slots"]["outer_top"]["present"], 24)

    def test_rejects_missing_files_and_full_canvas_composite_preview(self):
        self.write_complete_slot()
        missing = self.root / "outer-top-six-pose-source-repair-v1" / "A" / "run_a" / "front.png"
        missing.unlink()
        write_rgba_png(
            self.root / "outer-top-six-pose-source-repair-v1" / "B" / "idle" / "front.png",
            full_alpha=True,
        )

        report = audit_repair_layers(self.root, self.plan, canvas_size=[8, 6])

        self.assertEqual(report["status"], "SOURCE_REPAIR_LAYER_GAP")
        self.assertIn("outer_top/A/run_a/front.png missing", report["failures"])
        self.assertIn("outer_top/B/idle/front.png full-canvas alpha composite", report["failures"])


if __name__ == "__main__":
    unittest.main()
