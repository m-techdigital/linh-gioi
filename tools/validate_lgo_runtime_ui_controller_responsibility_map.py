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
        "docs/design/RUNTIME-UI-CONTROLLER-RESPONSIBILITY-MAP-v1.0.md",
        "LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_READY",
        "`M4PlayableClientController` remains the playable shell coordinator",
        "`RuntimeUiSkin`: shared frame, color, padding, border, and role styling helpers",
        "`RuntimeUiFactory`: stateless leaf widgets and small composed primitives",
        "Root document and screen assembly",
        "Auth/account flow",
        "Character list/create/select flow",
        "World HUD and interaction labels",
        "Local combat preview UI",
        "Responsive layout profile",
        "Runtime evidence hooks",
        "LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0",
        "No gameplay change",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-CONTROLLER-RESPONSIBILITY-MAP-v1.0.md",
        "LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_READY",
        "`M4PlayableClientController` remains the stateful playable shell coordinator",
        "Broad screen-level controller splitting is deferred",
        "LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "using static LinhGioi.UI.RuntimeUiFactory;",
        "private void BuildAuthPanel()",
        "private void BuildLobbyPanel()",
        "private void BuildWorldHud()",
        "private async Task LoginAsync()",
        "private async Task RefreshCharactersAsync()",
        "private async Task EnterWorldAsync()",
        "private void ApplyResponsiveLayoutProfile(bool force)",
        "internal async Task CaptureEvidenceLoginAsync()",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "LGO Runtime UI Primitive Factory v1",
        "internal static Button NewPrimaryButton",
        "internal static Toggle NewLocalSettingToggle",
        "internal static VisualElement NewCombatCooldownIcon()",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_controller_responsibility_map",
        "validate_lgo_runtime_ui_controller_responsibility_map.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-CONTROLLER-RESPONSIBILITY-MAP-v1.0",
        "LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-CONTROLLER-RESPONSIBILITY-MAP v1.0",
        "LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI CONTROLLER RESPONSIBILITY MAP VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
