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
    "ApplyLgoInventoryButtonBase",
    "ApplyLgoInventoryPanelShell",
    "ApplyLgoInventoryItemRow",
    "ApplyLgoInventoryCountBadge",
    "ApplyLgoInventoryBadge",
    "ApplyLgoInventoryStateBadge",
    "ApplyLgoInventoryStatsCard",
    "LgoInventoryContentFitPanelClass",
    "ApplyLgoInventoryContentFitPanel",
    "LgoInventoryCompactShellClass",
    "ApplyLgoInventoryCompactShell",
    "ApplyLgoStatusCard",
    "LgoLayeredFrameClass",
    "LgoFrameCornerClass",
    "AddLgoFrameCorner",
    "ApplyLgoLayeredFrame",
    "corner.AddToClassList(LgoFrameCornerClass)",
    "LgoDetailCardClass",
    "LgoActionButtonClass",
    "LgoActionPrimaryClass",
    "LgoActionStandardClass",
    "LgoInputFieldClass",
    "LgoOrnamentRailClass",
    "LgoItemIconFrameClass",
    "LgoTitleLabelClass",
    "LgoSubtitleLabelClass",
    "LgoTitleLabel",
    "LgoSubtitleLabel",
    "ApplyLgoItemIcon",
    "ApplyLgoSelectedTab",
    "ApplyLgoDisabledAction",
    "ApplyLgoHudCombatAction",
    "LgoHudActionIconClass",
    "AttachLgoHudActionIcon",
    "LgoHudNavigationActionClass",
    "ApplyLgoHudNavigationAction",
    "ApplyLgoHudShortcutAction",
    "ApplyLgoHudContextAction",
    "ApplyLgoHudQuestTab",
    "LgoHudInfoPanelClass",
    "ApplyLgoHudInfoPanel",
    "LgoHudPlayerCardClass",
    "ApplyLgoHudPlayerCard",
    "LgoHudPortraitClass",
    "ApplyLgoHudPortrait",
    "ApplyLgoHudLocationChip",
    "ApplyLgoHudMapPanel",
    "ApplyLgoHudQuestPanel",
    "LgoHudCompositionClass",
    "ApplyLgoHudComposition",
    "ApplyLgoDialoguePrimaryAction",
    "ApplyLgoDialogueSecondaryAction",
    "LgoDialoguePortraitClass",
    "ApplyLgoDialoguePortrait",
    "LgoEntryCtaActionClass",
    "LgoEntryAuthPrimaryClass",
    "LgoEntryAuthSecondaryClass",
    "LgoEntryShellClass",
    "ApplyLgoEntryShell",
    "ApplyLgoEntryControlCard",
    "CreateLgoEntryIcon",
    "AttachLgoEntryFieldIcon",
    "AttachLgoEntrySideActionIcon",
    "ApplyLgoEntryBrandCrest",
    "ApplyLgoEntryCtaAction",
    "ApplyLgoEntryAuthAction",
    "LgoEntryTextFieldClass",
    "ApplyLgoEntryTextField",
    "field.AddToClassList(LgoEntryTextFieldClass)",
    "ApplyLgoTextFieldInnerFrame",
    "ApplyLgoEntrySecondaryAction",
    "ApplyLgoEntrySideAction",
    "ApplyLgoCharacterSelectCard",
    "ApplyLgoCharacterSelectPrimaryAction",
    "ApplyLgoSkillCategoryCard",
    "ApplyLgoSkillIcon",
    "ApplyLgoSkillNode",
    "ApplyLgoPotentialNode",
]
FORBIDDEN_LOCAL_PATTERNS = [
    re.compile(r"private\s+static\s+readonly\s+Color\s+(?!Ui)[A-Za-z0-9_]*(Glass|Gold|Blue|Border|Text|SubText)"),
    re.compile(r"private\s+static\s+void\s+StyleFrame\s*\("),
    re.compile(r"private\s+static\s+Label\s+(?!LgoLabel\b)[A-Za-z0-9_]*Label\s*\("),
]
FORBIDDEN_PARALLEL_SKIN_HELPER = re.compile(
    r"private\s+(?:static\s+)?(?:"
    r"void\s+(?P<void_name>(?:Style|Build|Create|Make)[A-Za-z0-9_]*(?:Modal|Dialog|Card|Tab|Detail|Panel)[A-Za-z0-9_]*)"
    r"|(?:VisualElement|Button|Label)\s+(?P<element_name>[A-Za-z0-9_]*(?:Modal|Dialog|Card|Tab|Detail|Panel)[A-Za-z0-9_]*))\s*\("
)
ALLOWED_PARALLEL_SKIN_HELPERS = {
    "InventoryPanel": "CongDongLamArrivalHud.Inventory.cs",
    "MakeCharacterCard": "CongDongLamArrivalHud.CharacterSelect.cs",
}
# Exact legacy snippets that previously caused each screen to grow its own skin.
FORBIDDEN_SNIPPETS = [
    "InventoryGlass",
    "InventoryGlassRaised",
    "InventoryGold",
    "InventoryBlue",
    "style.gap =",
    "Box(",
    "ApplyLgoItemIcon(_characterHeroPortrait)",
]

# Runtime UI decisions that previously regressed when a new screen was built as a
# parallel one-off implementation. Keep these checks structural and cheap so the
# guard can run with every Map01A UI edit.
REQUIRED_PARTIAL_MARKERS = {
    "CongDongLamArrivalHud.cs": [
        "_dialogue = new VisualElement { name = \"Map01A Dialogue Panel\" }",
        "ApplyLgoGlassPanel(_dialogue",
        "ApplyLgoDetailCard(dialogueBody, 12, 10)",
        "ApplyLgoDialoguePrimaryAction(_dialogueContinue",
        "ApplyLgoDialogueSecondaryAction(option",
        "ApplyLgoDialoguePortrait(_dialoguePortrait)",
        "LgoTitleLabel(\"Hạ Vân\", 18)",
        "LgoSubtitleLabel(\"\", 12)",
        "ApplyLgoHudContextAction(_talk",
        "ApplyLgoHudContextAction(_npcTalk",
        "ApplyLgoHudCombatAction(_run",
        "ApplyLgoHudCombatAction(_jump",
        "ApplyLgoHudCombatAction(_basic",
        "ApplyLgoHudCombatAction(_skill",
        "AttachLgoHudActionIcon(_run",
        "AttachLgoHudActionIcon(_inventoryToggle",
        "AttachLgoHudActionIcon(_skillsShortcut",
        "ApplyLgoHudNavigationAction(_characterSelectButton",
        "ApplyLgoHudNavigationAction(_inventoryToggle",
        "ApplyLgoHudShortcutAction(_skillsShortcut",
        "ApplyLgoHudQuestTab(_questMissionsTab",
        "ApplyLgoHudQuestTab(_questPartyTab",
        "ApplyLgoHudLocationChip(title)",
        "ApplyLgoHudPlayerCard(_vitals)",
        "ApplyLgoHudPortrait(_vitalsPortrait)",
        "ApplyLgoHudComposition(_playerHudCluster)",
        "ApplyLgoHudComposition(_rightHudCluster)",
        "ApplyLgoHudQuestPanel(_quest)",
        "ApplyLgoHudMapPanel(_minimap)",
        "ApplyLgoHudInfoPanel(_pad)",
        "private const float InventoryDesktopColumnGap",
        "private const float InventoryDesktopDetailColumnWidth",
        "private const float InventoryDesktopMainColumnWidth",
        "private const float InventoryGridCellBasisPercent",
    ],
    "CongDongLamArrivalHud.Entry.cs": [
        "ApplyLgoEntryShell(panel)",
        "ApplyLgoEntryControlCard(controlCard)",
        "ApplyLgoEntryBrandCrest(brandCrest",
        "AttachLgoEntryFieldIcon(field",
        "AttachLgoEntrySideActionIcon(button",
        "ApplyLgoStatusCard(notice, 14, 10)",
        "ApplyLgoDetailCard(serverCard, 14, 10)",
        "ApplyLgoEntryTextField(field)",
        "ApplyLgoOrnamentRail(rail)",
        "LgoTitleLabel(\"Đăng nhập\", 20)",
        "var authScope = LgoSubtitleLabel(",
        "ApplyLgoEntryCtaAction(start, true)",
        "ApplyLgoEntryAuthAction(login, true)",
        "ApplyLgoEntryAuthAction(register, false)",
        "ApplyLgoEntrySecondaryAction(serverSwitch",
        "ApplyLgoEntrySecondaryAction(forgot",
        "ApplyLgoEntrySideAction(button)",
        "Map01A Entry Side Action ",
        "_safe.style.display = _entryOpen ? DisplayStyle.None : DisplayStyle.Flex",
    ],
    "CongDongLamArrivalHud.CharacterSelect.cs": [
        "ApplyLgoModalShell(panel, 20)",
        "ApplyLgoCharacterSelectCard(card)",
        "LgoTitleLabel(\"Chọn Nhân Vật\", 28",
        "LgoSubtitleLabel(\"review local",
        "ApplyLgoCharacterSelectPrimaryAction(close)",
        "UpdateHudShellVisibility()",
    ],
    "CongDongLamArrivalHud.Inventory.cs": [
        "_inventoryDetailPanel = InventoryPanel(\"Map01A Inventory Detail Panel\")",
        "_inventoryGridPanel = InventoryPanel(\"Map01A Inventory Grid Panel\")",
        "_inventoryHeroPanel = InventoryPanel(\"Map01A Inventory Character Panel\")",
        "body.Add(_inventoryGridPanel)",
        "body.Add(_inventoryHeroPanel)",
        "body.Add(_inventoryDetailPanel)",
        "LgoTitleLabel(\"HÀNH TRANG\", 26)",
        "LgoSubtitleLabel(\"Túi đồ",
        "_skillsTab = InventoryButton(() => ShowCharacterHubPreviewMode(CharacterHubMode.Skills)",
        "_potentialTab = InventoryButton(() => ShowCharacterHubPreviewMode(CharacterHubMode.Potential)",
        "_spiritPetTab = InventoryButton(() => ShowCharacterHubPreviewMode(CharacterHubMode.SpiritPet)",
        "ApplyLgoInventoryButtonBase(button, _touch)",
        "ApplyLgoInventoryPanelShell(panel)",
        "ApplyLgoItemIcon(_inventoryDetailIcon)",
        "_characterHeroPortrait.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit",
        "ApplyLgoItemIcon(quickIcon)",
        "ApplyLgoItemIcon(icon)",
        "ApplyLgoModalShell(_inventory, 12)",
        "ApplyLgoInventoryItemRow(row, _touch)",
        "ApplyLgoInventoryCountBadge(countLabel)",
        "ApplyLgoInventoryBadge(badge)",
        "ApplyLgoInventoryStateBadge(_inventoryDetailStateBadge)",
        "ApplyLgoInventoryStatsCard(_inventoryDetailStatsCard)",
        "ApplyLgoInventoryContentFitPanel(_inventoryGridPanel)",
        "RefreshInventoryShellMode()",
        "ApplyLgoInventoryCompactShell(_inventory, compact)",
        "ApplyLgoDetailCard(_characterHeroCard, 12, 10)",
        "_bagTab = InventoryButton(() => ShowInventoryMode(false)",
        "_characterInfoTab = InventoryButton(() => ShowInventoryMode(true)",
        "_inventoryDetailPanel.style.marginLeft = InventoryDesktopColumnGap",
        "_inventoryDetailPanel.style.flexBasis = InventoryDesktopDetailColumnWidth",
        "_inventoryGridPanel.style.flexBasis = InventoryDesktopMainColumnWidth",
        "_inventoryCategoryRail.style.flexDirection = FlexDirection.Column",
        "InitializeCharacterHub(body)",
    ],
    "CongDongLamArrivalHud.CharacterHub.cs": [
        "CreateHubSurface",
        "CreateHubTile",
        "ApplyLgoInventoryGridCell(button)",
        "CreateHubRailControl",
        "ApplyLgoSkillCategoryCard(button, _touch)",
        "ApplyLgoSkillNode(node)",
        "ApplyLgoPotentialNode(node)",
        "InitializeSkillsView(body)",
        "InitializePotentialView(body)",
        "InitializeSpiritPetView(body)",
        "InitializeHubInspector(body)",
        "ApplyLgoDetailCard(_hubPreviewDetailPanel)",
        "ApplyLgoDisabledAction(action)",
        "ApplyLgoSelectedTab(_characterInfoTab",
        "ApplyLgoSelectedTab(_bagTab",
    ],
}

REQUIRED_AGENT_MARKERS = [
    "UI/UX cùng pattern phải dùng shared base/skin/helper",
    "không giữ hai hệ UI song song",
    "entry/login, character select, inventory/bag, character info, storage/chest và item-detail phải dùng cùng shell/shared component",
    "detail món đặt ở panel phải theo design đã chốt",
    "Design demo owner upload là visual reference cho toàn bộ login/entry, character select, HUD, inventory/bag, storage/chest, fashion/wardrobe, dialog và item-detail",
    "không dựng khung thô chỉ để có chức năng",
    "Áp dụng cho toàn bộ design owner mới upload, không chỉ một màn riêng lẻ",
    "Nếu UI/UX giống nhau, cập nhật base/shared component trước rồi mới bind data/state/action từng màn",
    "Character hub dùng một hàng tab gọn có thể mở rộng",
    "modal/dialog/card/tab/button/detail panel dùng base chung",
        "Không tạo helper skin song song kiểu `StyleModalDialog`, `BuildCardPanel`, `CreateDetailPanel`",
    "Nếu hai UI/UX giống nhau mà cần khác hành vi, tách data/state/action",
    "Helper ngoại lệ như `InventoryPanel` chỉ được nằm trong partial sở hữu flow",
    "python3.12 tools/validate_lgo_ui_shared_skin.py",
]

REQUIRED_TEST_MARKERS = [
    "InventorySeparatesBagAndCharacterInfoTabsWithSharedSelection",
    "Item detail must stay on the right side of the bag grid",
    "CharacterNavigationOpensApprovedCharacterHubWithoutLegacyClassSelector",
    "Entry/login must not leave the in-game HUD visible behind the modal",
    "Entry/login side actions should be active navigation affordances with status feedback",
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
        for match in FORBIDDEN_PARALLEL_SKIN_HELPER.finditer(text):
            name = match.group("void_name") or match.group("element_name")
            allowed_owner = ALLOWED_PARALLEL_SKIN_HELPERS.get(name)
            if allowed_owner is None:
                violations.append(f"{rel}: parallel modal/dialog/card/tab/button/detail/panel skin helper {name}; use CongDongLamArrivalHud.Skin.cs shared base or a narrow wrapper around ApplyLgo*")
            elif path.name != allowed_owner:
                violations.append(f"{rel}: helper {name} is only allowed in {allowed_owner}; add shared skin/base in CongDongLamArrivalHud.Skin.cs instead of copying the wrapper")

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
                ["body.Add(_inventoryGridPanel)", "body.Add(_inventoryHeroPanel)", "body.Add(_inventoryDetailPanel)"],
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
