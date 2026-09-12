#!/usr/bin/env python3
"""Guard Map01A runtime UI against per-screen duplicate skin systems."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

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
        "_inventoryGridPanel = InventoryPanel(\"Map01A Inventory Grid Panel\")",
        "_inventoryHeroPanel = InventoryPanel(\"Map01A Inventory Character Panel\")",
        "_storagePanel = InventoryPanel(\"Map01A Storage Panel\")",
        "body.Add(_inventoryGridPanel)",
        "body.Add(_inventoryHeroPanel)",
        "body.Add(_storagePanel)",
        "body.Add(_inventoryDetailPanel)",
        "ApplyLgoSelectedTab(_bagTab",
        "ApplyLgoSelectedTab(_characterInfoTab",
        "_bagTab = InventoryButton(() => ShowInventoryMode(false)",
        "_characterInfoTab = InventoryButton(() => ShowInventoryMode(true)",
        "_storageTab = InventoryButton(ShowStorageMode",
        "_inventoryDetailPanel.style.marginLeft = 10",
        "_inventoryDetailPanel.style.flexBasis = 300",
    ],
}

REQUIRED_AGENT_MARKERS = [
    "UI/UX cùng pattern phải dùng shared base/skin/helper",
    "không giữ hai hệ UI song song",
    "entry/login, character select, inventory/bag, character info, storage/chest và item-detail phải dùng cùng shell/shared component",
    "detail món đặt ở panel phải theo design đã chốt",
    "python3.12 tools/validate_lgo_ui_shared_skin.py",
]

REQUIRED_TEST_MARKERS = [
    "InventorySeparatesBagAndCharacterInfoTabsWithSharedSelection",
    "Item detail must stay on the right side of the bag grid",
    "CharacterSelectModalUsesSharedSkinAndDoesNotAdvanceQuest",
    "Entry/login must not leave the in-game HUD visible behind the modal",
]


def fail(message: str) -> int:
    print("LGO_UI_SHARED_SKIN_FAIL " + message, file=sys.stderr)
    return 1


def _check_order(text: str, markers: list[str], rel: str, violations: list[str]) -> None:
    positions: list[tuple[str, int]] = []
    for marker in markers:
        index = text.find(marker)
        if index < 0:
            violations.append(f"{rel}: missing order marker {marker}")
        else:
            positions.append((marker, index))
    if len(positions) == len(markers):
        bad = [(a, b) for (a, ai), (b, bi) in zip(positions, positions[1:]) if ai >= bi]
        if bad:
            first, second = bad[0]
            violations.append(f"{rel}: Inventory detail panel must be added after shared content columns; order violation {first} before {second}")


def validate_root(root: Path = ROOT) -> list[str]:
    root = root.resolve()
    ui_dir = root / "client/Unity/Assets/Game/UI/Runtime"
    skin = ui_dir / "CongDongLamArrivalHud.Skin.cs"
    partials = sorted(ui_dir.glob("CongDongLamArrivalHud*.cs"))
    violations: list[str] = []

    agents = root / "AGENTS.md"
    if not agents.is_file():
        violations.append("AGENTS.md: missing project rules")
    else:
        agents_text = agents.read_text(encoding="utf-8", errors="replace")
        for marker in REQUIRED_AGENT_MARKERS:
            if marker not in agents_text:
                violations.append(f"AGENTS.md: missing shared UI governance marker {marker}")

    if not skin.is_file():
        violations.append("client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs: missing shared skin")
    else:
        skin_text = skin.read_text(encoding="utf-8", errors="replace")
        missing = [marker for marker in REQUIRED_SKIN_MARKERS if marker not in skin_text]
        for marker in missing:
            violations.append(f"{skin.relative_to(root)}: skin missing marker {marker}")

    for path in partials:
        text = path.read_text(encoding="utf-8", errors="replace")
        rel = str(path.relative_to(root))
        if path == skin:
            continue
        for snippet in FORBIDDEN_SNIPPETS:
            if snippet in text:
                violations.append(f"{rel}: forbidden snippet {snippet}")
        for pattern in FORBIDDEN_LOCAL_PATTERNS:
            for match in pattern.finditer(text):
                violations.append(f"{rel}: local skin pattern {match.group(0)}")

    for filename, markers in REQUIRED_PARTIAL_MARKERS.items():
        path = ui_dir / filename
        rel = f"client/Unity/Assets/Game/UI/Runtime/{filename}"
        if not path.is_file():
            violations.append(f"{rel}: missing shared UI partial")
            continue
        text = path.read_text(encoding="utf-8", errors="replace")
        for marker in markers:
            if marker not in text:
                violations.append(f"{rel}: missing structural marker {marker}")
        if filename == "CongDongLamArrivalHud.Inventory.cs":
            _check_order(
                text,
                [
                    "body.Add(_inventoryGridPanel)",
                    "body.Add(_inventoryHeroPanel)",
                    "body.Add(_storagePanel)",
                    "body.Add(_inventoryDetailPanel)",
                ],
                rel,
                violations,
            )

    tests = root / "client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs"
    if not tests.is_file():
        violations.append("client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs: missing UI regression tests")
    else:
        test_text = tests.read_text(encoding="utf-8", errors="replace")
        for marker in REQUIRED_TEST_MARKERS:
            if marker not in test_text:
                violations.append(f"{tests.relative_to(root)}: missing UI regression marker {marker}")

    return violations


def main() -> int:
    violations = validate_root(ROOT)
    if violations:
        for item in violations:
            print(item, file=sys.stderr)
        return fail(f"violations={len(violations)}")
    partials = sorted((ROOT / "client/Unity/Assets/Game/UI/Runtime").glob("CongDongLamArrivalHud*.cs"))
    print("LGO_UI_SHARED_SKIN_PASS partials=" + str(len(partials)))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
