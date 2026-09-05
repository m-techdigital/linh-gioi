#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing file: {rel}")
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def require(rel: str, *markers: str) -> str:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")
    return text


def check_frozen() -> None:
    result = subprocess.run(
        [
            "git",
            "--no-pager",
            "diff",
            "--name-only",
            "--",
            "protocol",
            "gamedata/schemas",
            "docs/adr",
            "client/Unity/Assets/Game/UI/design-tokens.json",
        ],
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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSizing.cs",
        "CharacterCreatePanelMinHeight",
        "CharacterCreatePanelMaxHeight",
    )
    factory = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewCharacterCreatePanel(RuntimeUiLayoutProfile layout)",
        "panel.name = \"LGO Character Hall Create Cultivator Panel V3B\";",
        "RuntimeUiSkin.ApplyPadding(panel, layout.CreatePanelPaddingHorizontal, layout.CreatePanelPaddingHorizontal, layout.CreatePanelPaddingTop, layout.CreatePanelPaddingBottom);",
        "panel.style.minHeight = RuntimeUiSizing.CharacterCreatePanelMinHeight;",
        "panel.style.maxHeight = RuntimeUiSizing.CharacterCreatePanelMaxHeight;",
        "RuntimeUiSkin.ApplyCharacterCreateFrame(panel);",
    )
    controller = require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_createPanel = NewCharacterCreatePanel(layout);",
        "_createPanel.style.maxHeight = mobile ? 174 : RuntimeUiSizing.CharacterCreatePanelMaxHeight;",
        "_characterName = NewTextField(\"Danh xưng\", \"LinhGioiHero\");",
        "_createButton = NewCompactSecondaryButton(\"Tạo tu sĩ\", () => RunAsync(CreateCharacterAsync));",
        "_enterWorldButton = NewCompactPrimaryButton(\"Vào sân luyện\", () => RunAsync(EnterWorldAsync));",
    )
    if "new VisualElement();\n            _createPanel.name = \"LGO Character Hall Create Cultivator Panel V3B\"" in controller:
        ERRORS.append("M4PlayableClientController still hand-constructs the Character Hall create panel shell")
    require(
        "tools/validate_lgo_character_hall_style_adoption.py",
        "NewCharacterCreatePanel(RuntimeUiLayoutProfile layout)",
        "_createPanel = NewCharacterCreatePanel(layout);",
    )
    require(
        "docs/design/RUNTIME-UI-FORM-SECTION-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_FORM_SECTION_BASE_READY",
        "RuntimeUiFactory.NewCharacterCreatePanel",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-FORM-SECTION-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_FORM_SECTION_BASE_READY",
        "LGO-RUNTIME-UI-FORM-SECTION-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_form_section_base_audit",
        "validate_lgo_runtime_ui_form_section_base_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-FORM-SECTION-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_FORM_SECTION_BASE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-FORM-SECTION-BASE-AUDIT v1.0",
        "LGO_RUNTIME_UI_FORM_SECTION_BASE_READY",
    )
    _ = factory
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI FORM SECTION BASE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_FORM_SECTION_BASE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
