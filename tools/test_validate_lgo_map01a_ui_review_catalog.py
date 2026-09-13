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
    def test_current_catalog_uses_approved_five_tab_player_and_excludes_legacy_class_select(self) -> None:
        text = (ROOT / validator.DOC).read_text(encoding="utf-8")
        current_paths = validator.iter_current_evidence_paths(text)

        self.assertIn("build/map01a-search-count-runtime-v3/spirit-pet.png", current_paths)
        self.assertIn("build/map01a-search-count-runtime-v3/bag-search-binh-mau-selected.png", current_paths)
        self.assertFalse(any("map01a-five-tab-player-copy-runtime-v1" in path for path in current_paths), current_paths)
        self.assertFalse(any("character-select" in path for path in current_paths), current_paths)
        self.assertFalse(any("inventory-tab-runtime" in path for path in current_paths), current_paths)

    def _fixture(self) -> tempfile.TemporaryDirectory[str]:
        temp = tempfile.TemporaryDirectory()
        root = Path(temp.name)
        doc = root / "docs/design/LGO-MAP01A-UI-REVIEW-CATALOG-v0.1.md"
        doc.parent.mkdir(parents=True)
        doc.write_text(
            "# Map01A UI Review Catalog\n\n"
            "Marker: `LGO_MAP01A_UI_REVIEW_CATALOG_READY`\n\n"
            "## Current evidence\n\n"
            "entry/login: `build/map01a-entry-remember-runtime-v1/entry-login.png`, `build/map01a-entry-remember-runtime-v1/manifest.json`\n"
            "five tabs: `build/map01a-search-count-runtime-v3/character-info.png`, `build/map01a-search-count-runtime-v3/bag.png`, `build/map01a-search-count-runtime-v3/bag-search-binh-mau.png`, `build/map01a-search-count-runtime-v3/bag-search-binh-mau-selected.png`, `build/map01a-search-count-runtime-v3/skills.png`, `build/map01a-search-count-runtime-v3/potential.png`, `build/map01a-search-count-runtime-v3/spirit-pet.png`, `build/map01a-search-count-runtime-v3/manifest.json`\n"
            "route: `build/map01a-item-detail-runtime-v1/01-arrival-q01.png`, `build/map01a-item-detail-runtime-v1/18-q09-portal-open.png`, `build/map01a-item-detail-runtime-v1/manifest.json`\n"
            "menu: `build/map01a-modal-input-runtime-v1/menu.png`, `build/map01a-modal-input-runtime-v1/manifest.json`\n"
            "docs/design/LGO-MAP01A-ITEM-ICON-SOURCE-AUDIT-v0.1.md\n"
            "No approved dedicated UI icon set\n"
            "not owner approval\n"
            "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED\n"
            "usesOsMouseOrKeyboard=false\n",
            encoding="utf-8",
        )
        audit = root / "docs/design/LGO-MAP01A-ITEM-ICON-SOURCE-AUDIT-v0.1.md"
        audit.write_text(
            "# Item Icon Source Audit\n\n"
            "SOURCE_AUDIT_CURRENT\n"
            "No approved dedicated UI icon set\n"
            "prettier but fake icon is a regression\n",
            encoding="utf-8",
        )
        write_json(root / "build/map01a-entry-remember-runtime-v1/manifest.json", {
            "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
            "usesOsMouseOrKeyboard": False,
            "width": 1600,
            "height": 900,
            "captureScope": "map01a-entry-login",
        })
        (root / "build/map01a-entry-remember-runtime-v1/entry-login.png").write_bytes(b"png")
        write_json(root / validator.HUB_MANIFEST, {
            "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
            "usesOsMouseOrKeyboard": False,
            "width": 1600,
            "height": 900,
            "frames": validator.HUB_FRAMES,
        })
        hub_dir = root / Path(validator.HUB_MANIFEST).parent
        for name in validator.HUB_FRAMES:
            (hub_dir / name).write_bytes(b"png")
        write_json(root / validator.ROUTE_MANIFEST, {
            "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
            "width": 1600,
            "height": 900,
            "frames": 18,
            "dialogueFrames": 38,
            "questWorldFramesUnobstructed": True,
            "inventoryItemDetailVerified": True,
        })
        route_dir = root / Path(validator.ROUTE_MANIFEST).parent
        for name in validator.ROUTE_FRAMES:
            (route_dir / name).write_bytes(b"png")
        write_json(root / "build/map01a-modal-input-runtime-v1/manifest.json", {
            "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
            "usesOsMouseOrKeyboard": False,
            "width": 1600,
            "height": 900,
            "captureScope": "map01a-menu",
        })
        (root / "build/map01a-modal-input-runtime-v1/menu.png").write_bytes(b"png")
        return temp

    def test_current_repo_has_reviewable_map01a_ui_catalog(self) -> None:
        self.assertEqual([], validator.validate_root(ROOT))


    def test_rejects_missing_current_evidence_path_listed_in_catalog(self) -> None:
        with self._fixture() as temp:
            doc = Path(temp) / "docs/design/LGO-MAP01A-UI-REVIEW-CATALOG-v0.1.md"
            text = doc.read_text(encoding="utf-8")
            text = text.replace(
                "entry/login: `build/map01a-entry-remember-runtime-v1/entry-login.png`",
                "entry/login: `build/missing-entry-runtime/entry-login.png`",
            )
            doc.write_text(text, encoding="utf-8")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("catalog current evidence path missing" in item for item in violations), violations)

    def test_rejects_legacy_character_select_as_current_evidence(self) -> None:
        with self._fixture() as temp:
            root = Path(temp)
            doc = root / "docs/design/LGO-MAP01A-UI-REVIEW-CATALOG-v0.1.md"
            text = doc.read_text(encoding="utf-8")
            text = text.replace(
                "## Current evidence\n\n",
                "## Current evidence\n\nlegacy: `build/map01a-character-select-runtime/character-select.png`\n",
            )
            doc.write_text(text, encoding="utf-8")
            legacy = root / "build/map01a-character-select-runtime/character-select.png"
            legacy.parent.mkdir(parents=True, exist_ok=True)
            legacy.write_bytes(b"png")

            violations = validator.validate_root(root)

        self.assertTrue(any("legacy evidence must not be current" in item for item in violations), violations)

    def test_rejects_os_input_capture_for_modal_evidence(self) -> None:
        with self._fixture() as temp:
            manifest = Path(temp) / "build/map01a-entry-remember-runtime-v1/manifest.json"
            data = json.loads(manifest.read_text())
            data["usesOsMouseOrKeyboard"] = True
            write_json(manifest, data)

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("usesOsMouseOrKeyboard" in item for item in violations), violations)

    def test_rejects_incomplete_quest_capture_matrix(self) -> None:
        with self._fixture() as temp:
            manifest = Path(temp) / validator.ROUTE_MANIFEST
            data = json.loads(manifest.read_text())
            data["frames"] = 17
            write_json(manifest, data)

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("frames" in item for item in violations), violations)

    def test_rejects_route_evidence_hidden_by_inventory_overlay(self) -> None:
        with self._fixture() as temp:
            manifest = Path(temp) / validator.ROUTE_MANIFEST
            data = json.loads(manifest.read_text())
            data["questWorldFramesUnobstructed"] = False
            write_json(manifest, data)

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("world frames must be unobstructed" in item for item in violations), violations)

    def test_rejects_route_without_selected_item_detail_evidence(self) -> None:
        with self._fixture() as temp:
            manifest = Path(temp) / validator.ROUTE_MANIFEST
            data = json.loads(manifest.read_text())
            data["inventoryItemDetailVerified"] = False
            write_json(manifest, data)

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("selected item detail" in item for item in violations), violations)

    def test_rejects_catalog_without_item_icon_source_audit(self) -> None:
        with self._fixture() as temp:
            audit = Path(temp) / "docs/design/LGO-MAP01A-ITEM-ICON-SOURCE-AUDIT-v0.1.md"
            audit.unlink()

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ITEM-ICON-SOURCE-AUDIT" in item or "item icon source audit" in item for item in violations), violations)


if __name__ == "__main__":
    unittest.main()
