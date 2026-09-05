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


def require_png(rel: str, min_size: int = 24_000) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing screenshot: {rel}")
        return
    if path.stat().st_size < min_size:
        ERRORS.append(f"screenshot too small: {rel} size={path.stat().st_size}")


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
        "docs/tasks/LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-EVIDENCE-REFRESH-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_EVIDENCE_REFRESH_READY",
        "login.png",
        "character-lobby.png",
        "character-select.png",
        "world-hub.png",
        "npc-dialogue.png",
        "session-menu.png",
        "No `VISUAL_RUNTIME_PASS` claim",
        "LGO-RUNTIME-UI-PANEL-HIERARCHY-SIMPLIFICATION-PASS-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-PANEL-HIERARCHY-SIMPLIFICATION-PASS-v1.0",
        "LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_EVIDENCE_REFRESH_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-EVIDENCE-REFRESH v1.0",
        "LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_EVIDENCE_REFRESH_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_component_metric_ownership_evidence_refresh",
        "validate_lgo_runtime_ui_component_metric_ownership_evidence_refresh.py",
    )
    require(
        "build/visual-evidence/latest/visual-runtime-evidence-review.md",
        "login.png",
        "character-lobby.png",
        "character-select.png",
        "world-hub.png",
        "npc-dialogue.png",
        "session-menu.png",
        "not claim `VISUAL_RUNTIME_PASS`",
    )
    require(
        "build/visual-evidence/latest/visual-runtime-evidence-heuristics.json",
        "login.png",
        "character-lobby.png",
        "character-select.png",
        "world-hub.png",
        "npc-dialogue.png",
        "session-menu.png",
        "EVIDENCE_CAPTURED_FOR_REVIEW",
    )
    require_png("build/visual-evidence/latest/login.png")
    require_png("build/visual-evidence/latest/character-lobby.png")
    require_png("build/visual-evidence/latest/character-select.png")
    require_png("build/visual-evidence/latest/world-hub.png")
    require_png("build/visual-evidence/latest/npc-dialogue.png")
    require_png("build/visual-evidence/latest/session-menu.png")
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI COMPONENT METRIC OWNERSHIP EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
