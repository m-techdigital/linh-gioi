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
            ERRORS.append(f"{rel} still contains rejected inline metric: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "using UnityEngine.UIElements;",
        "RootPaddingHorizontal",
        "HeaderMinHeight(bool authVisible)",
        "LoginStageDisplay",
        "LoginGateKeeperWidth",
        "LoginNpcGroundingColor",
        "LoginControlColumnWidth",
        "LoginCardBackground",
        "LoginServerRowMaxWidth",
        "LoginButtonMarginTop",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "layout.RootPaddingHorizontal",
        "layout.HeaderMinHeight(authVisible)",
        "layout.LoginStageDisplay",
        "layout.LoginGateKeeperWidth",
        "layout.LoginNpcGroundingColor",
        "layout.LoginControlColumnWidth",
        "layout.LoginCardBackground",
        "layout.LoginServerRowMaxWidth",
        "layout.LoginButtonMarginTop",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_root.style.paddingLeft = mobile ? 12 : tablet ? 18 : 28;",
        "_loginStage.style.width = tablet ? 262 : 304;",
        "_loginGateKeeper.style.width = tablet ? 248 : 292;",
        "_loginControlColumn.style.width = mobile ? Length.Percent(100) : tablet ? Length.Percent(56) : Length.Percent(54);",
        "_loginCard.style.minHeight = mobile ? Mathf.RoundToInt(100f * mobileScale) : tablet ? 128 : 136;",
        "_loginServerRow.style.maxWidth = mobile ? Length.Percent(100) : 436;",
    )
    require(
        "docs/design/RUNTIME-UI-RESPONSIVE-STYLE-APPLICATION-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_RESPONSIVE_STYLE_APPLICATION_AUDIT_READY",
        "RuntimeUiLayoutProfile",
        "M4PlayableClientController",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-RESPONSIVE-STYLE-APPLICATION-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_RESPONSIVE_STYLE_APPLICATION_AUDIT_READY",
        "No gameplay",
        "LGO-RUNTIME-UI-RESPONSIVE-STYLE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_responsive_style_application_audit",
        "validate_lgo_runtime_ui_responsive_style_application_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-RESPONSIVE-STYLE-APPLICATION-AUDIT-v1.0",
        "LGO_RUNTIME_UI_RESPONSIVE_STYLE_APPLICATION_AUDIT_READY",
        "LGO-RUNTIME-UI-RESPONSIVE-STYLE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-RESPONSIVE-STYLE-APPLICATION-AUDIT v1.0",
        "LGO_RUNTIME_UI_RESPONSIVE_STYLE_APPLICATION_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI RESPONSIVE STYLE APPLICATION AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_RESPONSIVE_STYLE_APPLICATION_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
