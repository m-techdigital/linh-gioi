#!/usr/bin/env python3
"""Guard Map01A runtime UI against per-screen duplicate skin systems."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
UI_DIR = ROOT / "client/Unity/Assets/Game/UI/Runtime"
SKIN = UI_DIR / "CongDongLamArrivalHud.Skin.cs"
PARTIALS = sorted(UI_DIR.glob("CongDongLamArrivalHud*.cs"))

REQUIRED_SKIN_MARKERS = [
    "ApplyLgoFrame",
    "ApplyLgoGlassPanel",
    "ApplyLgoModalShell",
    "ApplyLgoButton",
    "ApplyLgoSelectedTab",
]
FORBIDDEN_LOCAL_PATTERNS = [
    re.compile(r"private\s+static\s+readonly\s+Color\s+(?!Ui)[A-Za-z0-9_]*(Glass|Gold|Blue|Border|Text|SubText)"),
    re.compile(r"private\s+static\s+void\s+StyleFrame\s*\("),
    re.compile(r"private\s+static\s+Label\s+(?!LgoLabel\b)[A-Za-z0-9_]*Label\s*\("),
]
# Exact legacy snippets that previously caused each screen to grow its own skin.
FORBIDDEN_SNIPPETS = [
    "InventoryGlass",
    "InventoryGlassRaised",
    "InventoryGold",
    "InventoryBlue",
    "style.gap =",
]

# Runtime UI decisions that previously regressed when a new screen was built as a
# parallel one-off implementation. Keep these checks structural and cheap so the
# guard can run with every Map01A UI edit.
REQUIRED_PARTIAL_MARKERS = {
    "CongDongLamArrivalHud.Entry.cs": [
        "ApplyLgoModalShell(panel, 24)",
        "StyleEntryButton",
        "_safe.style.display = _entryOpen ? DisplayStyle.None : DisplayStyle.Flex",
    ],
    "CongDongLamArrivalHud.CharacterSelect.cs": [
        "ApplyLgoModalShell(panel, 20)",
        "ApplyLgoButton(card)",
        "UpdateHudShellVisibility()",
    ],
    "CongDongLamArrivalHud.Inventory.cs": [
        "_inventoryDetailPanel = InventoryPanel(\"Map01A Inventory Detail Panel\")",
        "body.Add(_inventoryDetailPanel)",
        "_inventoryGridPanel = InventoryPanel(\"Map01A Inventory Grid Panel\")",
        "ApplyLgoSelectedTab(_bagTab",
        "ApplyLgoSelectedTab(_characterInfoTab",
    ],
}

REQUIRED_TEST_MARKERS = [
    "InventorySeparatesBagAndCharacterInfoTabsWithSharedSelection",
    "Item detail must stay on the right side of the bag grid",
    "CharacterSelectModalUsesSharedSkinAndDoesNotAdvanceQuest",
    "Entry/login must not leave the in-game HUD visible behind the modal",
]


def fail(message: str) -> int:
    print("LGO_UI_SHARED_SKIN_FAIL " + message, file=sys.stderr)
    return 1


def main() -> int:
    if not SKIN.is_file():
        return fail("missing CongDongLamArrivalHud.Skin.cs shared skin")
    skin_text = SKIN.read_text(encoding="utf-8", errors="replace")
    missing = [marker for marker in REQUIRED_SKIN_MARKERS if marker not in skin_text]
    if missing:
        return fail("skin_missing_markers=" + ",".join(missing))

    violations: list[str] = []
    for path in PARTIALS:
        text = path.read_text(encoding="utf-8", errors="replace")
        rel = path.relative_to(ROOT)
        if path == SKIN:
            continue
        for snippet in FORBIDDEN_SNIPPETS:
            if snippet in text:
                violations.append(f"{rel}: forbidden snippet {snippet}")
        for pattern in FORBIDDEN_LOCAL_PATTERNS:
            for match in pattern.finditer(text):
                violations.append(f"{rel}: local skin pattern {match.group(0)}")
    for filename, markers in REQUIRED_PARTIAL_MARKERS.items():
        path = UI_DIR / filename
        if not path.is_file():
            violations.append(f"client/Unity/Assets/Game/UI/Runtime/{filename}: missing shared UI partial")
            continue
        text = path.read_text(encoding="utf-8", errors="replace")
        for marker in markers:
            if marker not in text:
                violations.append(f"{path.relative_to(ROOT)}: missing structural marker {marker}")

    tests = ROOT / "client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs"
    if not tests.is_file():
        violations.append("client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs: missing UI regression tests")
    else:
        test_text = tests.read_text(encoding="utf-8", errors="replace")
        for marker in REQUIRED_TEST_MARKERS:
            if marker not in test_text:
                violations.append(f"{tests.relative_to(ROOT)}: missing UI regression marker {marker}")

    if violations:
        for item in violations:
            print(item, file=sys.stderr)
        return fail(f"violations={len(violations)}")
    print("LGO_UI_SHARED_SKIN_PASS partials=" + str(len(PARTIALS)))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
