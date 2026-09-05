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


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains noisy frame marker: {marker}")


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
    skin = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplySubtleNestedFrame(VisualElement element, Color accent, float alpha = 0.32f)",
        "ApplySubtleNestedFrame(list, RuntimeArtCatalog.Gold, 0.34f);",
        "ApplySubtleNestedFrame(preview, RuntimeArtCatalog.Spirit, 0.38f);",
        "ApplySubtleNestedFrame(panel, RuntimeArtCatalog.Spirit, 0.30f);",
        "ApplySubtleNestedFrame(card, RuntimeArtCatalog.Spirit, 0.30f);",
        "element.style.borderTopWidth = 1;",
        "element.style.borderBottomWidth = 1;",
    )
    if skin.count("ApplySubtleNestedFrame(") < 5:
        ERRORS.append("RuntimeUiSkin should route repeated nested character frames through ApplySubtleNestedFrame")
    reject(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "new Color(0.93f, 0.73f, 0.36f, 0.70f), new Color(0.14f, 0.78f, 0.90f, 0.42f)",
        "new Color(0.93f, 0.73f, 0.36f, 0.68f), new Color(0.14f, 0.78f, 0.90f, 0.38f)",
    )
    require(
        "docs/design/RUNTIME-UI-PANEL-HIERARCHY-SIMPLIFICATION-PASS-v1.0.md",
        "LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_READY",
        "Parent panel framing remains stronger than child panel framing",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-PANEL-HIERARCHY-SIMPLIFICATION-PASS-v1.0.md",
        "LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_READY",
        "LGO-RUNTIME-UI-PANEL-HIERARCHY-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-PANEL-HIERARCHY-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-PANEL-HIERARCHY-SIMPLIFICATION-PASS v1.0",
        "LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_panel_hierarchy_simplification",
        "validate_lgo_runtime_ui_panel_hierarchy_simplification.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI PANEL HIERARCHY SIMPLIFICATION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
