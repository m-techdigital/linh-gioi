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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyBaseButtonFrame(Button button)",
        "ApplyRuntimeIconFrame(VisualElement icon, Color background)",
        "ApplySettingToggleFrame(Toggle toggle, Color accent)",
        "ApplyBadgeFrame(VisualElement badge)",
        "ApplyToastFrame(Label label, Color accent)",
        "ApplyStatusChipFrame(Label label, Color accent)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "using static LinhGioi.UI.RuntimeUiFactory;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSkin.ApplyBaseButtonFrame(button);",
        "RuntimeUiSkin.ApplyRuntimeIconFrame(icon, new Color(0.02f, 0.08f, 0.16f, 0.82f));",
        "RuntimeUiSkin.ApplySettingToggleFrame(toggle, value ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Muted);",
        "RuntimeUiSkin.ApplyBadgeFrame(badge);",
        "RuntimeUiSkin.ApplyToastFrame(label, RuntimeArtCatalog.Gold);",
        "RuntimeUiSkin.ApplyStatusChipFrame(label, accent);",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-SKIN-ADOPTION-AUDIT-PASS-v1.0.md",
        "LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY",
        "No gameplay, protocol, GameData, ADR, or design-token change",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_skin_adoption_audit",
        "validate_lgo_runtime_ui_skin_adoption_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-SKIN-ADOPTION-AUDIT-PASS-v1.0",
        "LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-SKIN-ADOPTION-AUDIT-PASS v1.0",
        "LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI SKIN ADOPTION AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
