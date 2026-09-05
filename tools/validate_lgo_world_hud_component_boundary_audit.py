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
        "internal static VisualElement NewWorldHudRoot",
        "hud.style.alignSelf = Align.FlexStart;",
        "RuntimeUiSkin.ApplyPadding(hud, RuntimeUiSpacing.WorldHudRootPaddingHorizontal, RuntimeUiSpacing.WorldHudRootPaddingVertical);",
        "internal static VisualElement NewOrnamentRule",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        '_worldHud = NewWorldHudRoot("LGO World HUD Action Shell V3B Skin v1", 390);',
        'NewSectionHeaderBlock("Sân Luyện An Toàn", RuntimeArtCatalog.Spirit',
        'NewSectionHeaderBlock("Điện Nhân Vật", RuntimeArtCatalog.Gold',
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private static VisualElement NewOrnamentRule(Color color)",
        "_worldHud = NewPanel(390);\n            _worldHud.name =",
    )
    require(
        "docs/design/WORLD-HUD-COMPONENT-BOUNDARY-AUDIT-v1.0.md",
        "LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY",
        "RuntimeUiFactory.NewWorldHudRoot",
        "Keep In Controller",
        "No gameplay change",
    )
    require(
        "docs/tasks/LGO-WORLD-HUD-COMPONENT-BOUNDARY-AUDIT-v1.0.md",
        "LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY",
        "No combat mechanic change",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hud_component_boundary_audit",
        "validate_lgo_world_hud_component_boundary_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUD-COMPONENT-BOUNDARY-AUDIT-v1.0",
        "LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUD-COMPONENT-BOUNDARY-AUDIT v1.0",
        "LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUD COMPONENT BOUNDARY AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
