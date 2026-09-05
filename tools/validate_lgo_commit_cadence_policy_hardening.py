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


def check_policy_docs() -> None:
    agents = read("AGENTS.md")
    workflow = read("docs/execution/CODEX-CONTINUOUS-WORKFLOW.md")
    autopilot = read("docs/execution/CODEX-AUTOPILOT.md")
    task_doc = read("docs/tasks/LGO-COMMIT-CADENCE-POLICY-HARDENING-v1.0.md")
    next_action = read("docs/execution/NEXT-ACTION.md")
    ledger = read("docs/execution/TASK-LEDGER.md")

    require(agents, "Default to fewer commits", "AGENTS")
    require(workflow, "small validator/text-only edits should usually be grouped", "continuous workflow")
    require(autopilot, "does not commit by default", "autopilot docs")
    require(autopilot, "LGO_AUTOPILOT_COMMIT=1", "autopilot docs")
    require(task_doc, "LGO_COMMIT_CADENCE_POLICY_HARDENING_READY", "task doc")
    require(task_doc, "Set autopilot auto-commit default to off.", "task doc")
    require(next_action, "LGO_COMMIT_CADENCE_POLICY_HARDENING_READY", "NEXT-ACTION")
    require(next_action, "LGO-RUNTIME-UI-QUALITY-DEBT-TRIAGE-v1.0", "NEXT-ACTION")
    require(ledger, "LGO-COMMIT-CADENCE-POLICY-HARDENING v1.0", "TASK-LEDGER")


def check_script_default() -> None:
    script = read("tools/lgo_codex_autopilot.sh")
    require(script, 'LGO_AUTOPILOT_COMMIT="${LGO_AUTOPILOT_COMMIT:-0}"', "autopilot script")
    require(script, "LGO_AUTOPILOT_COMMIT default 0", "autopilot usage")
    require(script, "LGO_AUTOPILOT_PUSH default 0", "autopilot usage")


def check_closure_hook() -> None:
    closure = read("tools/lgo_playable_closure_check.sh")
    require(closure, "tools/validate_lgo_commit_cadence_policy_hardening.py", "closure py_compile")
    require(closure, "commit_cadence_policy_hardening", "closure source-only phase")


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
    check_policy_docs()
    check_script_default()
    check_closure_hook()
    check_frozen_surfaces()
    if ERRORS:
        print("LGO COMMIT CADENCE POLICY HARDENING VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f"- {error}", file=sys.stderr)
        return 1
    print("LGO_COMMIT_CADENCE_POLICY_HARDENING_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
