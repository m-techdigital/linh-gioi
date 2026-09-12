#!/usr/bin/env python3
"""Regression tests for the Map01A UI review evidence catalog validator."""
from __future__ import annotations

import json
import shutil
import tempfile
import unittest
from pathlib import Path

import validate_lgo_map01a_ui_review_catalog as validator

ROOT = Path(__file__).resolve().parents[1]


def write_json(path: Path, data: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(data, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


class ValidateMap01AUiReviewCatalogTests(unittest.TestCase):
    def _fixture(self) -> tempfile.TemporaryDirectory[str]:
        temp = tempfile.TemporaryDirectory()
        root = Path(temp.name)
        doc = root / "docs/design/LGO-MAP01A-UI-REVIEW-CATALOG-v0.1.md"
        doc.parent.mkdir(parents=True)
        doc.write_text(
            "# Map01A UI Review Catalog\n\n"
            "Marker: `LGO_MAP01A_UI_REVIEW_CATALOG_READY`\n\n"
            "entry/login: `build/map01a-entry-form-runtime/entry-login.png`\n"
            "character select: `build/map01a-character-select-runtime/character-select.png`\n"
            "inventory: `build/map01a-detail-right-player/quest-capture/pc/07-q04-inventory-open.png`\n"
            "character-info.png\n"
            "storage.png\n"
            "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED\n"
            "usesOsMouseOrKeyboard=false\n",
            encoding="utf-8",
        )
        write_json(root / "build/map01a-entry-form-runtime/manifest.json", {
            "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
            "usesOsMouseOrKeyboard": False,
            "width": 1600,
            "height": 900,
        })
        (root / "build/map01a-entry-form-runtime/entry-login.png").write_bytes(b"png")
        write_json(root / "build/map01a-character-select-runtime/manifest.json", {
            "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
            "usesOsMouseOrKeyboard": False,
            "width": 1600,
            "height": 900,
        })
        (root / "build/map01a-character-select-runtime/character-select.png").write_bytes(b"png")
        write_json(root / "build/map01a-inventory-tab-runtime/manifest.json", {
            "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
            "usesOsMouseOrKeyboard": False,
            "width": 1600,
            "height": 900,
            "frames": ["character-info.png", "storage.png"],
        })
        (root / "build/map01a-inventory-tab-runtime/character-info.png").write_bytes(b"png")
        (root / "build/map01a-inventory-tab-runtime/storage.png").write_bytes(b"png")
        for profile, size in {"pc": (1280, 720), "tablet": (1024, 768), "mobile": (1600, 720)}.items():
            write_json(root / f"build/map01a-detail-right-player/quest-capture/{profile}/manifest.json", {
                "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
                "width": size[0],
                "height": size[1],
                "frames": 18,
                "dialogueFrames": 38,
                "errors": [],
            })
            capture_dir = root / f"build/map01a-detail-right-player/quest-capture/{profile}"
            for name in ["07-q04-inventory-open.png", "18-q09-portal-open.png"]:
                (capture_dir / name).write_bytes(b"png")
        return temp

    def test_current_repo_has_reviewable_map01a_ui_catalog(self) -> None:
        self.assertEqual([], validator.validate_root(ROOT))

    def test_rejects_os_input_capture_for_modal_evidence(self) -> None:
        with self._fixture() as temp:
            manifest = Path(temp) / "build/map01a-entry-form-runtime/manifest.json"
            data = json.loads(manifest.read_text())
            data["usesOsMouseOrKeyboard"] = True
            write_json(manifest, data)

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("usesOsMouseOrKeyboard" in item for item in violations), violations)

    def test_rejects_incomplete_quest_capture_matrix(self) -> None:
        with self._fixture() as temp:
            manifest = Path(temp) / "build/map01a-detail-right-player/quest-capture/pc/manifest.json"
            data = json.loads(manifest.read_text())
            data["frames"] = 17
            write_json(manifest, data)

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("frames" in item for item in violations), violations)


if __name__ == "__main__":
    unittest.main()
