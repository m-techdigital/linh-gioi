#!/usr/bin/env python3
"""Guard Map01A runtime UI against per-screen duplicate skin systems."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

REVOKED_STATIC_CLASS_PREVIEW_PATHS = (
    "client/Unity/Assets/Game/World/Runtime/TwoDClassMixedLoadoutFitPreview.cs",
    "client/Unity/Assets/Game/World/Runtime/CongDongLamMap01AClassEquipmentCapture.cs",
    "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/KiemMixedLoadoutFitPreview",
    "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/PhapMixedLoadoutFitPreview",
    "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/CoMixedLoadoutFitPreview",
    "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/LinhMixedLoadoutFitPreview",
    "tools/capture_lgo_class_equipment.py",
)

REQUIRED_SKIN_MARKERS = [
    "ApplyLgoFrame",
    "ApplyLgoGlassPanel",
    "ApplyLgoModalShell",
    "ApplyLgoCharacterHubShell",
    "ApplyLgoCharacterHubTitle",
    "ApplyLgoCharacterHubBackdrop",
    "ApplyLgoCharacterHubSurface",
    "ApplyLgoCharacterHubPanelSurface",
    "ApplyLgoCharacterHubTabState",
    "ApplyLgoCharacterHubPrimaryAction",
    "ApplyLgoCharacterHubGoldAction",
    "ApplyLgoCharacterHubInspectorAction",
    "ApplyLgoCharacterHubHeroIconFrame",
    "ApplyLgoCharacterHubSelectionState",
    "ApplyLgoCharacterHubInteractiveMotion",
    "ApplyLgoCharacterHubFiligreeFrame",
    "ApplyLgoCharacterHubSectionFrame",
    "ApplyLgoCharacterHubInsetFrame",
    "RemoveLgoOuterBorder",
    "ApplyLgoCharacterHubDetailCard",
    "RuntimeUiSkin.ApplyOrnamentedShellFrame",
    "AnimateLgoCharacterHubOpen",
    "AnimateLgoCharacterHubSwap",
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
    "ApplyLgoEntryServerCard",
    "ApplyLgoEntryStatusLine",
    "ApplyLgoEntryPasswordReveal",
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
    "ApplyLgoEntryServerSwitchAction",
    "ApplyLgoEntrySideAction",
    "LgoServerSelectPanelClass",
    "LgoServerSelectCardClass",
    "LgoServerSelectActionClass",
    "ApplyLgoServerSelectPanel",
    "ApplyLgoServerSelectCard",
    "ApplyLgoServerSelectAction",
    "LgoRegisterPanelClass",
    "LgoRegisterAgreementClass",
    "LgoRegisterPrimaryClass",
    "LgoRegisterBackClass",
    "LgoRegisterPasswordRevealClass",
    "ApplyLgoRegisterPanel",
    "ApplyLgoRegisterAgreement",
    "ApplyLgoRegisterPrimary",
    "ApplyLgoRegisterBack",
    "ApplyLgoRegisterPasswordReveal",
    "LgoAuthFlowPanelClass",
    "LgoAuthFlowPrimaryClass",
    "LgoAuthFlowBackClass",
    "ApplyLgoAuthFlowPanel",
    "ApplyLgoAuthFlowPrimary",
    "ApplyLgoAuthFlowBack",
    "ApplyLgoCharacterSelectPanel",
    "ApplyLgoCharacterSelectProfile",
    "ApplyLgoCharacterSelectEmptySlot",
    "ApplyLgoCharacterSelectAction",
    "ApplyLgoCharacterSelectServerRow",
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
FORBIDDEN_CLASS_SPECIFIC_UI_PATTERNS = [
    re.compile(r"_scene\.(?:Get|Set|Cycle|Select|Toggle|Trigger|Is|Has|Can)?Vo[A-Z]"),
    re.compile(r'ActiveEquipmentClassId\s*==\s*"(?:vo|kiem|phap|co|linh)"'),
]
FORBIDDEN_CHARACTER_HUB_REBUILD_SNIPPETS = [
    "_skillsPanel?.RemoveFromHierarchy()",
    "_potentialPanel?.RemoveFromHierarchy()",
    "_spiritPetPanel?.RemoveFromHierarchy()",
    "InitializeSkillsView(_characterHubBody)",
    "InitializePotentialView(_characterHubBody)",
    "InitializeSpiritPetView(_characterHubBody)",
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
        "UpdateCharacterHubOpenAnimation(_scene.InventoryOpen)",
    ],
    "CongDongLamArrivalHud.Entry.cs": [
        "ApplyLgoEntryShell(_entryPanel)",
        "ApplyLgoEntryControlCard(_entryControlCard)",
        "ApplyLgoEntryBrandCrest(brandCrest",
        "AttachLgoEntryFieldIcon(field",
        "AttachLgoEntrySideActionIcon(button",
        "ApplyLgoStatusCard(notice, 18, 12)",
        "ApplyLgoEntryServerCard(serverCard)",
        "ApplyLgoEntryStatusLine(_entryStatus)",
        "ApplyLgoEntryPasswordReveal(reveal",
        "ApplyLgoEntryTextField(field)",
        "ApplyLgoEntryAuthAction(login, true)",
        "ApplyLgoEntryAuthAction(register, false)",
        "ApplyLgoEntryServerSwitchAction(serverSwitch",
        "ApplyLgoEntrySecondaryAction(forgot",
        "ApplyLgoEntrySideAction(button)",
        "Map01A Entry Side Action ",
        "_safe.style.display = _entryOpen ? DisplayStyle.None : DisplayStyle.Flex",
    ],
    "CongDongLamArrivalHud.CharacterSelect.cs": [
        "ApplyLgoCharacterSelectPanel(panel)",
        "ApplyLgoCharacterSelectProfile(selected, true)",
        "ApplyLgoCharacterSelectEmptySlot(slot)",
        "LgoTitleLabel(\"CHỌN NHÂN VẬT\", 25",
        "Map01A Character Select Account Panel",
        "Map01A Character Select Stage",
        "Map01A Character Select Enter Game",
        "Map01A Character Empty Slot \" + index",
        "UpdateHudShellVisibility()",
    ],
    "CongDongLamArrivalHud.ServerSelect.cs": [
        "Map01A Server Select Overlay",
        "ApplyLgoServerSelectPanel(_serverSelectOverlay)",
        "ApplyLgoServerSelectCard(server)",
        "ApplyLgoServerSelectAction(back, false)",
        "ApplyLgoServerSelectAction(confirm, true)",
        "S1 · Đông Lâm",
        "ServerSelectReturnTarget.Entry",
        "ServerSelectReturnTarget.CharacterSelect",
    ],
    "CongDongLamArrivalHud.Register.cs": [
        "Map01A Register Overlay",
        "ApplyLgoRegisterPanel(_registerOverlay)",
        "Map01A Register Account Field",
        "Map01A Register Password Field",
        "Map01A Register Confirm Password Field",
        "ApplyLgoRegisterAgreement(agreement)",
        "ApplyLgoRegisterPrimary(submit)",
        "ApplyLgoRegisterBack(back)",
        "Dịch vụ đăng ký chưa kết nối. Vui lòng thử lại sau.",
    ],
    "CongDongLamArrivalHud.PasswordRecovery.cs": [
        "Map01A Password Recovery Overlay",
        "ApplyLgoAuthFlowPanel(_passwordRecoveryOverlay, 410)",
        "Map01A Password Recovery Account Field",
        "ApplyLgoAuthFlowPrimary(submit)",
        "ApplyLgoAuthFlowBack(back)",
        "Dịch vụ khôi phục mật khẩu chưa kết nối. Vui lòng thử lại sau.",
        "UpdateEntryControlCardVisibility()",
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
        "ApplyLgoCharacterHubHeroIconFrame(_inventoryDetailIcon)",
        "ApplyLgoCharacterHubDetailCard(_inventoryDetailPanel)",
        "ApplyLgoCharacterHubDetailCard(_characterHeroCard, 12, 10)",
        "ApplyLgoCharacterHubSelectionState(_equipmentTiles[index]",
        "_characterHeroPortrait.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit",
        "ApplyLgoItemIcon(quickIcon)",
        "ApplyLgoItemIcon(icon)",
        "ApplyLgoCharacterHubShell(_inventory)",
        "ApplyLgoInventoryItemRow(row, _touch)",
        "ApplyLgoInventoryCountBadge(countLabel)",
        "ApplyLgoInventoryBadge(badge)",
        "ApplyLgoInventoryStateBadge(_inventoryDetailStateBadge)",
        "ApplyLgoInventoryStatsCard(_inventoryDetailStatsCard)",
        "ApplyLgoInventoryContentFitPanel(_inventoryGridPanel)",
        "RefreshInventoryShellMode()",
        "ApplyLgoInventoryCompactShell(_inventory, compact)",
        "_bagTab = InventoryButton(() => ShowInventoryMode(false)",
        "_characterInfoTab = InventoryButton(() => ShowInventoryMode(true)",
        "_inventoryDetailPanel.style.marginLeft = InventoryDesktopColumnGap",
        "_inventoryDetailPanel.style.flexBasis = InventoryDesktopDetailColumnWidth",
        "_inventoryGridPanel.style.flexBasis = InventoryDesktopMainColumnWidth",
        "_inventoryCategoryRail.style.flexDirection = FlexDirection.Column",
        "InitializeCharacterHub(body)",
    ],
    "CongDongLamArrivalHud.CharacterHub.cs": [
        "CharacterHubPotentialTopology",
        "Map01A Potential Detail Facts",
        "Map01A Potential Current Level",
        "Map01A Potential Current Effect",
        "Map01A Potential Next Effect",
        "Map01A Potential Cost Row",
        "lgo-potential-node-overlay",
        "ApplyPotentialNodeSelection",
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
        "Map01A Spirit Pet Skill Row ",
        "CharacterHubSpiritPetPreview.SkillPreview",
        "InitializeHubInspector(body)",
        "BindCharacterHubProfile()",
        "ApplyLgoDisabledAction(action)",
        "ApplyLgoCharacterHubInspectorAction(action)",
        "ApplyLgoCharacterHubTabState(_characterInfoTab",
        "ApplyLgoCharacterHubTabState(_bagTab",
        "ApplyLgoCharacterHubSelectionState(node, selected)",
        "ApplyLgoCharacterHubHeroIconFrame(_hubDetailIcon)",
        "ApplyLgoCharacterHubDetailCard(_spiritPetPreview, 10, 8)",
        "ApplyLgoCharacterHubDetailCard(_hubPreviewDetailPanel)",
        "AnimateLgoCharacterHubSwap(_hubPreviewDetailPanel)",
    ],
    "CharacterHubPotentialTopology.cs": [
        "Map01A Potential Topology Base",
        "Map01A Potential Topology Artwork",
        "lgo-potential-topology",
        "PrebuiltNodeFrameCount",
        "PrebuiltValueFrameCount",
        "PrebuiltAddFrameCount",
        "PrebuiltAddGlyphCount",
        "PrebuiltMeridianAnchorCount",
    ],
    "CharacterHubClassCatalog.cs": [
        "SharedPotentials = Array.AsReadOnly",
        "public string DefaultPotentialName { get; }",
        "public string CurrentEffect { get; }",
        "public string NextEffect { get; }",
    ],
}

REQUIRED_AGENT_MARKERS = [
    "Mọi objective/autopilot prompt cũ còn nhắc class, pose, wardrobe hoặc source art đã bị owner thay thế",
    "screen active duy nhất",
    "scenario/state/interaction → một canonical design → asset/provenance/pixel budget → implementation plan",
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
    "class chỉ truyền profile/data (`classId`, `itemId`, skill, tiềm năng, linh thú)",
    "Character Hub dựng topology Skill, Tiềm năng và Linh thú đúng một lần từ shared base",
    "Năm định nghĩa thuộc tính Tiềm năng dùng một catalog bất biến chung",
    "Đổi class chỉ bind profile/data/icon/text/state vào các node có sẵn",
    "Helper ngoại lệ như `InventoryPanel` chỉ được nằm trong partial sở hữu flow",
    "python3.12 tools/validate_lgo_ui_shared_skin.py",
]

REQUIRED_TEST_MARKERS = [
    "InventorySeparatesBagAndCharacterInfoTabsWithSharedSelection",
    "Item detail must stay on the right side of the bag grid",
    "CharacterNavigationOpensApprovedCharacterHubWithoutLegacyClassSelector",
    "EntryScreenMatchesCanonicalSingleCtaLayoutWithoutChangingMapState",
    "The canonical design has one primary login CTA and no second Start action",
    "lgo-entry-password-reveal",
    "CharacterSelectUsesOneSavedProfileAndNeverMutatesClassSelection",
    "Selecting the saved profile must never cycle class/pose review source",
    "ServerSelectUsesOneRealServerAndReturnsToItsOpeningScreen",
    "Selecting a server must not mutate class, pose, wardrobe or gameplay source state",
    "RegisterScreenValidatesLocallyAndReturnsToEntry",
    "Register validation must not mutate class, pose, wardrobe or gameplay state",
    "PasswordRecoveryRequestValidatesLocallyAndReturnsToEntry",
    "Password recovery must not mutate class, pose, wardrobe or gameplay state",
    "CharacterHubClassRefreshRebindsOneStableSharedTopology",
    "The circles, outer ring and connectors must be one prebuilt shared topology",
    "Potential topology must prebuild all value boxes",
    "Potential topology must prebuild all add boxes",
    "Class refresh must rebind the shared Potential detail template",
    "Potential level/value belongs in its canonical facts row",
]

REQUIRED_RUNTIME_SKIN_MARKERS = [
    "ApplyOrnamentedShellFrame",
    "DrawShellFrameEdge",
    "const float segmentLength = 16f",
    "DrawShellCorner",
    "painter.ClosePath()",
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

    for relative in REVOKED_STATIC_CLASS_PREVIEW_PATHS:
        if (root / relative).exists():
            violations.append(
                f"{relative}: legacy static class renderer/capture path was revoked; "
                "Character Hub must use the active source-pose actor"
            )

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
        noisy_selection = (
            "private static void ApplyLgoCharacterHubSelectionState(VisualElement element, bool selected)\n"
            "        {\n"
            "            ApplyLgoLayeredFrame(element);"
        )
        if noisy_selection in skin_text:
            violations.append(
                f"{skin.relative_to(root)}: compact selection state must not repeat decorative frame corners"
            )

    runtime_skin = ui_dir / "RuntimeUiSkin.cs"
    if not runtime_skin.is_file():
        violations.append("client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs: missing shared runtime skin")
    else:
        runtime_skin_text = runtime_skin.read_text(encoding="utf-8", errors="replace")
        for marker in REQUIRED_RUNTIME_SKIN_MARKERS:
            if marker not in runtime_skin_text:
                violations.append(f"{runtime_skin.relative_to(root)}: shared vector frame missing marker {marker}")

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
        for pattern in FORBIDDEN_CLASS_SPECIFIC_UI_PATTERNS:
            for match in pattern.finditer(text):
                violations.append(
                    f"{rel}: class-specific UI runtime branch {match.group(0)}; "
                    "bind the shared Character Hub contract/profile instead"
                )
        if path.name == "CongDongLamArrivalHud.CharacterHub.cs":
            for snippet in FORBIDDEN_CHARACTER_HUB_REBUILD_SNIPPETS:
                if snippet in text:
                    violations.append(
                        f"{rel}: Character Hub class refresh must bind the stable shared topology; "
                        f"forbidden rebuild snippet {snippet}"
                    )
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
