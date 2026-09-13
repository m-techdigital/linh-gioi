import json
import struct
import tempfile
import unittest
import zlib
from pathlib import Path

from inspect_lgo_source_png_inventory import inspect_inventory, write_inventory


def write_rgba_png(path, width=4, height=3, opaque_points=None):
    opaque_points = set(opaque_points or [])
    path.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for y in range(height):
        row = bytearray([0])
        for x in range(width):
            alpha = 255 if (x, y) in opaque_points else 0
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


class SourcePngInventoryTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def test_inspects_png_size_hash_alpha_count_and_bbox_by_directory(self):
        write_rgba_png(
            self.root / "outer-top-six-pose-source-repair-v1" / "A" / "idle" / "front.png",
            opaque_points={(1, 1), (2, 2)},
        )
        write_rgba_png(
            self.root / "outer-top-six-pose-source-repair-v1" / "A" / "idle" / "back.png",
            opaque_points=set(),
        )

        report = inspect_inventory(
            self.root,
            ["outer-top-six-pose-source-repair-v1"],
            scan_alpha=True,
        )

        record = report["directories"]["outer-top-six-pose-source-repair-v1"]
        files = {Path(item["path"]).name: item for item in record["files"]}
        self.assertEqual(record["pngCount"], 2)
        self.assertEqual(files["front.png"]["size"], [4, 3])
        self.assertTrue(files["front.png"]["hasAlpha"])
        self.assertEqual(files["front.png"]["nonzeroAlpha"], 2)
        self.assertEqual(files["front.png"]["alphaBBox"], [1, 1, 2, 2])
        self.assertEqual(files["back.png"]["nonzeroAlpha"], 0)
        self.assertIsNone(files["back.png"]["alphaBBox"])

    def test_write_inventory_outputs_parseable_json(self):
        write_rgba_png(self.root / "waist-belt-six-pose-source-repair-v1" / "front.png")
        output = self.root / "inventory.json"

        report = write_inventory(
            self.root,
            ["waist-belt-six-pose-source-repair-v1"],
            output,
            scan_alpha=False,
        )

        written = json.loads(output.read_text())
        self.assertEqual(written["directories"], report["directories"])
        self.assertEqual(written["directories"]["waist-belt-six-pose-source-repair-v1"]["pngCount"], 1)


if __name__ == "__main__":
    unittest.main()
