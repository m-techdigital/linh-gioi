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


def require(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "LGO Runtime UI Primitive Factory v1",
        "internal static VisualElement NewPanel(float maxWidth)",
        "internal static VisualElement NewPreviewPanel",
        "internal static VisualElement NewReadabilityRow",
        "internal static VisualElement NewWorldHudGroup",
        "internal static void ApplyHudStatusCompact",
        "internal static Label NewCompactStatusLabel",
        "internal static Label NewSectionTitle",
        "internal static Label NewMutedLabel",
        "internal static VisualElement NewLoginOrnamentRule",
        "internal static Label NewStatusLabel",
        "internal static VisualElement NewButtonRow",
        "RuntimeUiSkin.ApplyPanelFrame(panel);",
        "RuntimeUiSkin.ApplyPreviewPanelFrame(preview);",
        "RuntimeUiSkin.ApplyInsetRowFrame(label, color);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "using static LinhGioi.UI.RuntimeUiFactory;",
        "_lobbyPanel = NewPanel(840);",
        "_selectedPreview = NewPreviewPanel(\"TU SĨ\", \"Hồ sơ đang chọn\");",
        "_combatTargetStatus = NewCompactStatusLabel(\"Bia luyện: chưa vào sân.\", RuntimeArtCatalog.Gold, 13);",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private static VisualElement NewPanel(float maxWidth)",
        "private static VisualElement NewPreviewPanel(",
        "private static VisualElement NewReadabilityRow(",
        "private static Label NewStatusLabel(",
        "private static VisualElement NewButtonRow(params Button[] buttons)",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS-v1.0.md",
        "LGO_RUNTIME_UI_PRIMITIVE_FACTORY_READY",
        "RuntimeUiFactory",
        "No gameplay change",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_primitive_factory",
        "validate_lgo_runtime_ui_primitive_factory.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS-v1.0",
        "LGO_RUNTIME_UI_PRIMITIVE_FACTORY_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS v1.0",
        "LGO_RUNTIME_UI_PRIMITIVE_FACTORY_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI PRIMITIVE FACTORY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_PRIMITIVE_FACTORY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
