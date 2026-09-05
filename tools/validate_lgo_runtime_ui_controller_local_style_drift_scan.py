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
            ERRORS.append(f"{rel} still contains duplicate marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private static bool IsDisplayed(VisualElement element)",
        "private static void SetDisplayed(VisualElement element, bool visible)",
        "private static void SetElementVisibility(VisualElement element, bool visible)",
        "SetDisplayed(_authPanel, true);",
        "SetDisplayed(_sessionMenuPanel, visible);",
        "SetElementVisibility(_worldHud, !(sessionVisible && compactViewport));",
        "SetDisplayed(_dialoguePanel, visible);",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_sessionMenuPanel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;",
        "_dialoguePanel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;",
        "_localCombatPanel.style.display = visible ? DisplayStyle.None : DisplayStyle.Flex;",
        "_worldHud.style.visibility = sessionVisible && compactViewport ? Visibility.Hidden : Visibility.Visible;",
    )
    require(
        "docs/design/RUNTIME-UI-CONTROLLER-LOCAL-STYLE-DRIFT-SCAN-v1.0.md",
        "LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_READY",
        "Controller-local visibility state is legitimate",
        "No gameplay behavior change",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-DRIFT-SCAN-v1.0.md",
        "LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_READY",
        "LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-DRIFT-SCAN v1.0",
        "LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_controller_local_style_drift_scan",
        "validate_lgo_runtime_ui_controller_local_style_drift_scan.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI CONTROLLER LOCAL STYLE DRIFT SCAN VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
