#!/usr/bin/env python3.12
"""Regression tests for source off-slot review board writer."""
from __future__ import annotations

import json
import tempfile
import unittest
from pathlib import Path

from PIL import Image

import write_lgo_source_off_slot_boards as writer


class WriteLgoSourceOffSlotBoardsTests(unittest.TestCase):
    def test_write_boards_can_be_called_after_import_and_records_sha_provenance(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            surface = root / "surface"
            body = root / "body"
            body.mkdir()
            for pose in writer.POSES:
                Image.new("RGBA", (96, 128), (80, 40, 20, 255)).save(body / f"{pose}.png")
            for slot_index, slot in enumerate(writer.SLOTS):
                for pose in writer.POSES:
                    target = surface / slot / f"{pose}.png"
                    target.parent.mkdir(parents=True, exist_ok=True)
                    Image.new("RGBA", (96, 128), (slot_index * 10, 80, 180, 90)).save(target)

            writer.write_boards(surface, body)

            provenance = json.loads((surface / "off-slot-board-provenance.json").read_text(encoding="utf-8"))
            self.assertEqual("SOURCE_REVIEW_REQUIRED", provenance["visualReviewStatus"])
            self.assertEqual(list(writer.POSES), provenance["poses"])
            self.assertEqual(list(writer.SLOTS), provenance["slots"])
            self.assertEqual([f"{pose}-ten-slot-off-review.jpg" for pose in writer.POSES], [board["file"] for board in provenance["boards"]])
            for board in provenance["boards"]:
                self.assertRegex(board["sha256"], r"^[0-9a-f]{64}$")
                self.assertGreater((surface / board["file"]).stat().st_size, 0)


if __name__ == "__main__":
    unittest.main()
