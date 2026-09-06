#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def require(path: str, *markers: str) -> None:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return
    text = file_path.read_text(encoding="utf-8", errors="replace")
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{path} missing marker: {marker}")


def forbid(path: str, *markers: str) -> None:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return
    text = file_path.read_text(encoding="utf-8", errors="replace")
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{path} forbidden marker still present: {marker}")


def check_frozen() -> None:
    result = subprocess.run(
        ["git", "--no-pager", "diff", "--name-only", "--", "protocol", "gamedata/schemas", "docs/adr", "client/Unity/Assets/Game/UI/design-tokens.json"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "git frozen diff failed")
    elif result.stdout.strip():
        ERRORS.append("frozen surface changed")


def main() -> int:
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_lobbyContent = NewCharacterHallContentRow(layout);",
        "_lobbyHeaderBlock = NewSectionHeaderBlock(\"Điện Nhân Vật\", RuntimeArtCatalog.Gold, \"LGO Character Hall Header Block\");",
        "_selectedPreview = NewSelectedCharacterPreviewPanel();",
        "NewCharacterPortraitFrame(layout, portraitTexture, null)",
        "_createPanel = NewCharacterCreatePanel(layout);",
        "_createBody = NewModalBody(\"LGO Character Create Modal Body\");",
        "_createFooter = NewModalFooter(\"LGO Character Create Modal Footer\");",
        "_lobbyPanel = NewCharacterHallPanel(layout);",
        "_characterList = NewCharacterListPanel(layout);",
        "RuntimeCharacterHallResponsiveLayout.Apply(",
        "RuntimeCharacterHallResponsiveLayout.ApplySelectedDetails(layout, _selectedCharacter != null, _selectedStatus, _selectedObjective);",
    )
    forbid(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        '_characterList.Add(NewReadabilityRow("Bước 1", "Đặt danh xưng tu sĩ bên dưới.", RuntimeArtCatalog.Spirit));',
        '_characterList.Add(NewReadabilityRow("Bước 2", "Tạo hồ sơ rồi vào sân luyện.", RuntimeArtCatalog.Gold));',
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeCharacterHallResponsiveLayout.cs",
        "OwnerMarker = \"LGO Character Hall Responsive Layout Helper v1\"",
        "LGO Character Hall Selected Compact Header Contract v1",
        "SetDisplayed(lobbyHeaderBlock, !(layout.IsMobile && hasSelectedCharacter));",
        "lobbyIntro.style.display = layout.IsMobile && hasSelectedCharacter ? DisplayStyle.None : DisplayStyle.Flex;",
        "RuntimeUiFactory.ApplyCharacterListResponsive(characterList, layout, width, hasSelectedCharacter);",
        "RuntimeUiFactory.ApplySelectedCharacterPreviewResponsive(selectedPreview, selectedName, layout, width, hasSelectedCharacter);",
        "RuntimeUiOverflowGuard.ApplyModalBody(createBody);",
        "RuntimeUiOverflowGuard.ApplyModalFooter(createFooter, compactStandaloneCreate ? 0 : 6);",
        "ApplySelectedActionRatio(layout, characterActionRow, createButton, enterWorldButton);",
        "LGO Character Hall Selected CTA Ratio Base v1",
        "LGO Character Hall Mobile Full Safe Shell v1",
        "lobbyPanel.style.width = Length.Percent(100);",
        "lobbyPanel.style.maxWidth = layout.IsMobile ? Length.Percent(100)",
        "layout.IsTablet ? RuntimeUiSizing.CharacterHallTabletPanelMaxWidth : RuntimeUiSizing.CharacterHallPanelMaxWidth",
        "createPanel.style.maxHeight = layout.IsMobile ? 174 : RuntimeUiSizing.CharacterCreatePanelMaxHeight;",
        "LGO Character Hall Selected NonMobile Dock Base v1",
        "else if (collapsed && !layout.IsTablet)",
        "LGO Character Hall Selected Detail Collapse Base v1",
        "ApplySelectedDetails(RuntimeUiLayoutProfile layout, bool hasSelectedCharacter, Label selectedStatus, Label selectedObjective)",
        "layout.CharacterHallSelectedActionDockWidth",
        "layout.CharacterHallSelectedActionDockMaxHeight",
        "layout.CharacterHallSelectedActionDockInsetHorizontal",
        "layout.CharacterHallSelectedActionDockBottom",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewCharacterHallPanel(RuntimeUiLayoutProfile layout)",
        "NewCharacterCreatePanel(RuntimeUiLayoutProfile layout)",
        "NewCharacterHallContentRow(RuntimeUiLayoutProfile layout)",
        "NewCharacterListPanel(RuntimeUiLayoutProfile layout)",
        "NewSelectedCharacterPreviewPanel()",
        "NewCharacterPortraitFrame(RuntimeUiLayoutProfile layout, Texture2D portraitTexture, Texture2D fallbackTexture)",
        "LGO Character Hall V3B Composition Panel",
        "LGO Character Hall Create Cultivator Panel V3B",
        "LGO Character Hall Main Selection Grid V3B",
        "LGO Character Hall Bounded List Scroll Base v1",
        "LGO Character Hall Bounded List Scroll",
        "new ScrollView(ScrollViewMode.Vertical)",
        "RuntimeUiOverflowGuard.ApplyBoundedScroll(list, layout.CharacterListMaxHeight(false), 0f);",
        "RuntimeUiOverflowGuard.ApplyBoundedScroll(scroll, listMaxHeight, 0f);",
        "list.style.width = listMaxWidth;",
        "list.style.flexGrow = hasSelectedCharacter ? 1 : 0;",
        "scroll.verticalScrollerVisibility = ScrollerVisibility.Auto;",
        "LGO Character Hall Selected Cultivator Card V3B",
        "LGO Character Hall V3B Cultivator Portrait",
        "layout.CharacterPortraitWidth",
        "layout.CharacterSelectedPreviewMaxWidth",
        "preview.style.display = !hasSelectedCharacter ? DisplayStyle.None : DisplayStyle.Flex;",
        "preview.style.width = hasSelectedCharacter ? layout.CharacterSelectedPreviewMaxWidth : StyleKeyword.Auto;",
        "preview.style.flexBasis = layout.IsMobile && hasSelectedCharacter ? layout.CharacterSelectedPreviewMaxWidth : StyleKeyword.Auto;",
        "layout.CharacterSelectedPreviewHeight",
        "RuntimeUiSkin.ApplyCharacterHallPanelFrame(panel, layout.IsMobile);",
        "RuntimeUiSkin.ApplyCharacterListFrame(list);",
        "RuntimeUiSkin.ApplyCharacterPreviewFrame(preview, layout.IsMobile && hasSelectedCharacter);",
        "RuntimeUiSkin.ApplyCharacterCreateFrame(panel);",
        "RuntimeUiSkin.ApplyCharacterPortraitFrame(portrait);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "CharacterListMaxHeight(bool hasSelectedCharacter)",
        "CharacterHallSelectedActionDockWidth",
        "Mathf.Clamp(Height * 0.34f, 270f, 330f)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyCharacterHallPanelFrame(VisualElement panel, bool lightProfile = false)",
        "ApplyCharacterPreviewFrame(VisualElement preview, bool lightProfile = false)",
        "LGO Character Hall Mobile Light Shell v1",
        "LGO Character Hall Selected Hero Light Frame v1",
        "LGO Character Hall No Stretched Texture v1",
        "panel.style.backgroundImage = StyleKeyword.None;",
        "lightProfile ? new Color(0.005f, 0.024f, 0.052f, 0.56f) : new Color(0.005f, 0.024f, 0.052f, 0.70f)",
        "ApplyCharacterListFrame(VisualElement list)",
        "ApplySubtleNestedFrame(list, RuntimeArtCatalog.Gold, 0.34f);",
    )
    require(
        "docs/tasks/LGO-CHARACTER-HALL-PANEL-DENSITY-PASS-v1.0.md",
        "LGO_CHARACTER_HALL_PANEL_DENSITY_READY",
        "No account",
        "VISUAL_RUNTIME_PASS",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "character_hall_panel_density",
        "validate_lgo_character_hall_panel_density.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-CHARACTER-HALL-PANEL-DENSITY-PASS-v1.0",
        "LGO_CHARACTER_HALL_PANEL_DENSITY_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-CHARACTER-HALL-PANEL-DENSITY-PASS v1.0",
        "LGO_CHARACTER_HALL_PANEL_DENSITY_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO CHARACTER HALL PANEL DENSITY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_CHARACTER_HALL_PANEL_DENSITY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
