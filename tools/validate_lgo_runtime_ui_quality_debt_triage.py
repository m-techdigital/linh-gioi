#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(relative: str) -> str:
    path = ROOT / relative
    if not path.exists():
        ERRORS.append(f"Missing required file: {relative}")
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def require(text: str, needle: str, context: str) -> None:
    if needle not in text:
        ERRORS.append(f"{context} missing marker/text: {needle}")


def check_docs() -> None:
    design = read("docs/design/RUNTIME-UI-QUALITY-DEBT-TRIAGE-v1.0.md")
    task = read("docs/tasks/LGO-RUNTIME-UI-QUALITY-DEBT-TRIAGE-v1.0.md")
    next_action = read("docs/execution/NEXT-ACTION.md")
    ledger = read("docs/execution/TASK-LEDGER.md")

    require(design, "LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY", "triage design")
    require(design, "build/visual-evidence/latest/login.png", "triage evidence")
    require(design, "Gate Keeper reads pasted-on/floating", "triage login issue")
    require(design, "LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0", "triage decision")
    require(design, "No `VISUAL_RUNTIME_PASS` is claimed.", "triage non-claim")
    require(task, "LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY", "task doc")
    require(task, "Selected Next Task", "task doc")
    require(next_action, "LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY", "NEXT-ACTION")
    require(next_action, "LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0", "NEXT-ACTION")
    require(ledger, "LGO-RUNTIME-UI-QUALITY-DEBT-TRIAGE v1.0", "TASK-LEDGER")
    require(ledger, "LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY", "TASK-LEDGER")


def check_closure_hook() -> None:
    closure = read("tools/lgo_playable_closure_check.sh")
    require(closure, "tools/validate_lgo_runtime_ui_quality_debt_triage.py", "closure py_compile")
    require(closure, "runtime_ui_quality_debt_triage", "closure source-only phase")


def check_frozen_surfaces() -> None:
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
        check=False,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    if result.returncode != 0:
        ERRORS.append(f"git frozen-surface diff failed: {result.stderr.strip()}")
        return
    changed = [line for line in result.stdout.splitlines() if line.strip()]
    if changed:
        ERRORS.append("Frozen surfaces changed: " + ", ".join(changed))


def main() -> int:
    check_docs()
    check_closure_hook()
    check_frozen_surfaces()
    if ERRORS:
        print("LGO RUNTIME UI QUALITY DEBT TRIAGE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f"- {error}", file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
