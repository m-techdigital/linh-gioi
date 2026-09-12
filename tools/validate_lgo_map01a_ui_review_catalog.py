#!/usr/bin/env python3
"""Validate the current Map01A UI review catalog and evidence matrix."""
from __future__ import annotations

import json
import sys
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
DOC = "docs/design/LGO-MAP01A-UI-REVIEW-CATALOG-v0.1.md"
READY = "LGO_MAP01A_UI_REVIEW_CATALOG_READY"
TECH_STATUS = "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED"

MODAL_EVIDENCE = [
    ("entry/login", "build/map01a-entry-form-runtime/manifest.json", "build/map01a-entry-form-runtime/entry-login.png", 1600, 900),
    ("character select", "build/map01a-character-select-runtime/manifest.json", "build/map01a-character-select-runtime/character-select.png", 1600, 900),
]
QUEST_PROFILES = {
    "pc": (1280, 720),
    "tablet": (1024, 768),
    "mobile": (1600, 720),
}
REQUIRED_QUEST_FRAMES = [
    "07-q04-inventory-open.png",
    "18-q09-portal-open.png",
]
INVENTORY_TAB_EVIDENCE = [
    ("character info tab", "build/map01a-inventory-tab-runtime/character-info.png"),
    ("storage tab", "build/map01a-inventory-tab-runtime/storage.png"),
]


def load_json(root: Path, rel: str, violations: list[str]) -> dict[str, Any]:
    path = root / rel
    if not path.is_file():
        violations.append(f"missing json: {rel}")
        return {}
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception as exc:  # pragma: no cover - defensive error text
        violations.append(f"invalid json: {rel}: {exc}")
        return {}


def require_file(root: Path, rel: str, violations: list[str]) -> None:
    if not (root / rel).is_file():
        violations.append(f"missing file: {rel}")


def validate_root(root: Path = ROOT) -> list[str]:
    root = root.resolve()
    violations: list[str] = []
    doc = root / DOC
    if not doc.is_file():
        violations.append(f"missing document: {DOC}")
    else:
        text = doc.read_text(encoding="utf-8", errors="replace")
        for marker in [
            READY,
            TECH_STATUS,
            "usesOsMouseOrKeyboard=false",
            "entry-login.png",
            "character-select.png",
            "07-q04-inventory-open.png",
            "pc/tablet/mobile",
            "character-info.png",
            "storage.png",
            "not owner approval",
        ]:
            if marker not in text:
                violations.append(f"{DOC}: missing marker {marker}")

    for label, manifest_rel, png_rel, width, height in MODAL_EVIDENCE:
        data = load_json(root, manifest_rel, violations)
        require_file(root, png_rel, violations)
        if data.get("status") != TECH_STATUS:
            violations.append(f"{label}: status must be {TECH_STATUS}")
        if data.get("usesOsMouseOrKeyboard") is not False:
            violations.append(f"{label}: usesOsMouseOrKeyboard must be false")
        if data.get("width") != width or data.get("height") != height:
            violations.append(f"{label}: expected {width}x{height}, got {data.get('width')}x{data.get('height')}")

    tab_manifest = load_json(root, "build/map01a-inventory-tab-runtime/manifest.json", violations)
    if tab_manifest.get("status") != TECH_STATUS:
        violations.append(f"inventory tabs: status must be {TECH_STATUS}")
    if tab_manifest.get("usesOsMouseOrKeyboard") is not False:
        violations.append("inventory tabs: usesOsMouseOrKeyboard must be false")
    if tab_manifest.get("frames") != ["character-info.png", "storage.png"]:
        violations.append("inventory tabs: frames must list character-info.png and storage.png")
    for label, rel in INVENTORY_TAB_EVIDENCE:
        require_file(root, rel, violations)

    for profile, (width, height) in QUEST_PROFILES.items():
        base = f"build/map01a-detail-right-player/quest-capture/{profile}"
        data = load_json(root, base + "/manifest.json", violations)
        if data.get("status") != TECH_STATUS:
            violations.append(f"{profile}: status must be {TECH_STATUS}")
        if data.get("width") != width or data.get("height") != height:
            violations.append(f"{profile}: expected {width}x{height}, got {data.get('width')}x{data.get('height')}")
        if data.get("frames") != 18:
            violations.append(f"{profile}: frames must be 18")
        if data.get("dialogueFrames") != 38:
            violations.append(f"{profile}: dialogueFrames must be 38")
        if data.get("errors") not in ([], None):
            violations.append(f"{profile}: errors must be empty")
        for frame in REQUIRED_QUEST_FRAMES:
            require_file(root, base + "/" + frame, violations)

    return violations


def main() -> int:
    violations = validate_root(ROOT)
    if violations:
        print("LGO_MAP01A_UI_REVIEW_CATALOG_FAILED", file=sys.stderr)
        for item in violations:
            print(" - " + item, file=sys.stderr)
        return 1
    print("LGO_MAP01A_UI_REVIEW_CATALOG_PASS screens=entry,character_select,inventory profiles=pc,tablet,mobile")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
