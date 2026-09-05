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
        "internal static Label NewToast(string text)",
        "internal static void ApplyStatusChip(Label label, Color accent)",
        "internal static void ApplyStatusAccent(Label label, Color accent)",
        "internal static void ApplyCombatButtonSkin(Button button, Texture2D texture, bool coolingDown)",
        "RuntimeUiSkin.ApplyToastFrame(label, RuntimeArtCatalog.Gold);",
        "RuntimeUiSkin.ApplyStatusChipFrame(label, accent);",
        "button.style.minWidth = coolingDown ? 142 : 132;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "using static LinhGioi.UI.RuntimeUiFactory;",
        '_toast = NewToast("Linh Môn đã sẵn sàng.");',
        "ApplyStatusChip(_status, busy ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Muted);",
        "ApplyCombatButtonSkin(_localCombatButton, CombatPlaceholderAssets.CombatButtonPressedTexture, false);",
        "ApplyStatusAccent(_combatRangeStatus",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private static void ApplyCombatButtonSkin",
        "private static void ApplyStatusAccent",
        "private static Label NewToast",
        "private static void ApplyStatusChip",
        "RuntimeUiSkin.ApplyToastFrame(label, RuntimeArtCatalog.Gold);",
        "RuntimeUiSkin.ApplyStatusChipFrame(label, accent);",
    )
    require(
        "docs/design/RUNTIME-UI-STYLE-OWNERSHIP-DRIFT-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY",
        "M4PlayableClientController` owns gameplay/session/account state",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-STYLE-OWNERSHIP-DRIFT-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY",
        "No gameplay change",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_style_ownership_drift_audit",
        "validate_lgo_runtime_ui_style_ownership_drift_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-STYLE-OWNERSHIP-DRIFT-AUDIT-v1.0",
        "LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-STYLE-OWNERSHIP-DRIFT-AUDIT v1.0",
        "LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI STYLE OWNERSHIP DRIFT AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
