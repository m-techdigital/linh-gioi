#!/usr/bin/env python3
"""Validate the current Map01A UI review catalog and evidence matrix."""
from __future__ import annotations

import json
import re
import sys
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
DOC = "docs/design/LGO-MAP01A-UI-REVIEW-CATALOG-v0.1.md"
ITEM_ICON_AUDIT_DOC = "docs/design/LGO-MAP01A-ITEM-ICON-SOURCE-AUDIT-v0.1.md"
READY = "LGO_MAP01A_UI_REVIEW_CATALOG_READY"
TECH_STATUS = "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED"

ENTRY_EVIDENCE = (
    "build/map01a-entry-remember-runtime-v1/manifest.json",
    "build/map01a-entry-remember-runtime-v1/entry-login.png",
)
HUB_MANIFEST = "build/map01a-bag-screen-runtime-v4/pc/manifest.json"
HUB_FRAMES = [
    "character-info.png",
    "bag.png",
    "bag-search-binh-mau.png",
    "bag-search-binh-mau-selected.png",
    "skills.png",
    "potential.png",
    "spirit-pet.png",
]
ROUTE_MANIFEST = "build/map01a-completion-copy-runtime-v2/manifest.json"
ROUTE_FRAMES = ["01-arrival-q01.bmp", "18-q09-portal-open.bmp"]
MENU_EVIDENCE = (
    "build/map01a-modal-input-runtime-v1/manifest.json",
    "build/map01a-modal-input-runtime-v1/menu.png",
)
LEGACY_CURRENT_PATH_MARKERS = ("character-select-runtime", "inventory-tab-runtime", "inventory-column-balance-runtime")


def iter_current_evidence_paths(text: str) -> list[str]:
    marker = "## Current evidence"
    start = text.find(marker)
    if start < 0:
        return []
    next_section = text.find("\n## ", start + len(marker))
    section = text[start:] if next_section < 0 else text[start:next_section]
    paths: list[str] = []
    for match in re.finditer(r"`([^`]+)`", section):
        value = match.group(1).strip()
        for rel in re.split(r"\s+and\s+|,\s*", value):
            rel = rel.strip()
            if not rel.startswith("build/"):
                continue
            if "{" in rel or "}" in rel or rel.endswith("/"):
                continue
            paths.append(rel)
    return paths


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
            "character-info.png",
            "bag.png",
            "bag-search-binh-mau.png",
            "bag-search-binh-mau-selected.png",
            "skills.png",
            "potential.png",
            "spirit-pet.png",
            "01-arrival-q01.png",
            "18-q09-portal-open.bmp",
            "menu.png",
            "not owner approval",
            ITEM_ICON_AUDIT_DOC,
            "No approved dedicated UI icon set",
        ]:
            if marker not in text:
                violations.append(f"{DOC}: missing marker {marker}")

        current_paths = iter_current_evidence_paths(text)
        for rel in current_paths:
            if not (root / rel).is_file():
                violations.append(f"{DOC}: catalog current evidence path missing: {rel}")
        for legacy_marker in LEGACY_CURRENT_PATH_MARKERS:
            if any(legacy_marker in rel for rel in current_paths):
                violations.append(f"{DOC}: legacy evidence must not be current: {legacy_marker}")

        audit = root / ITEM_ICON_AUDIT_DOC
        if not audit.is_file():
            violations.append(f"missing item icon source audit: {ITEM_ICON_AUDIT_DOC}")
        else:
            audit_text = audit.read_text(encoding="utf-8", errors="replace")
            for marker in [
                "SOURCE_AUDIT_CURRENT",
                "No approved dedicated UI icon set",
                "prettier but fake icon is a regression",
            ]:
                if marker not in audit_text:
                    violations.append(f"{ITEM_ICON_AUDIT_DOC}: missing marker {marker}")

    for label, evidence, expected_scope in [
        ("entry/login", ENTRY_EVIDENCE, "map01a-entry-login"),
        ("gameplay menu", MENU_EVIDENCE, "map01a-menu"),
    ]:
        manifest_rel, png_rel = evidence
        data = load_json(root, manifest_rel, violations)
        require_file(root, png_rel, violations)
        if data.get("status") != TECH_STATUS:
            violations.append(f"{label}: status must be {TECH_STATUS}")
        if data.get("usesOsMouseOrKeyboard") is not False:
            violations.append(f"{label}: usesOsMouseOrKeyboard must be false")
        if data.get("width") != 1600 or data.get("height") != 900:
            violations.append(f"{label}: expected 1600x900, got {data.get('width')}x{data.get('height')}")
        if data.get("captureScope") != expected_scope:
            violations.append(f"{label}: captureScope must be {expected_scope}")

    hub = load_json(root, HUB_MANIFEST, violations)
    if hub.get("status") != TECH_STATUS:
        violations.append(f"five-tab hub: status must be {TECH_STATUS}")
    if hub.get("usesOsMouseOrKeyboard") is not False:
        violations.append("five-tab hub: usesOsMouseOrKeyboard must be false")
    if hub.get("width") != 1600 or hub.get("height") != 900:
        violations.append("five-tab hub: expected 1600x900")
    if hub.get("frames") != HUB_FRAMES:
        violations.append("five-tab hub: manifest must list all five approved tab frames")
    hub_base = str(Path(HUB_MANIFEST).parent)
    for frame in HUB_FRAMES:
        require_file(root, f"{hub_base}/{frame}", violations)

    route = load_json(root, ROUTE_MANIFEST, violations)
    if route.get("status") != TECH_STATUS:
        violations.append(f"route: status must be {TECH_STATUS}")
    if route.get("width") != 1600 or route.get("height") != 900:
        violations.append("route: expected 1600x900")
    if route.get("frames") != 18:
        violations.append("route: frames must be 18")
    if route.get("dialogueFrames") != 38:
        violations.append("route: dialogueFrames must be 38")
    if route.get("questWorldFramesUnobstructed") is not True:
        violations.append("route: Q05-Q09 world frames must be unobstructed")
    if route.get("inventoryItemDetailVerified") is not True:
        violations.append("route: Q04 must verify selected item detail on the right")
    route_base = str(Path(ROUTE_MANIFEST).parent)
    for frame in ROUTE_FRAMES:
        require_file(root, f"{route_base}/{frame}", violations)

    return violations


def main() -> int:
    violations = validate_root(ROOT)
    if violations:
        print("LGO_MAP01A_UI_REVIEW_CATALOG_FAILED", file=sys.stderr)
        for item in violations:
            print(" - " + item, file=sys.stderr)
        return 1
    print("LGO_MAP01A_UI_REVIEW_CATALOG_PASS screens=entry,five_tab_hub,route,menu")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
