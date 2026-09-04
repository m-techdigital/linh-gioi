#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MARKER = "M6_COMBAT_FOUNDATION_CLOSURE_VALIDATION_PASS_v0.55.0"
REQUIRED = {
    "docs/execution/PROJECT-STATE.md": [
        "M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0",
        "M6 combat foundation v0.55.0",
    ],
    "docs/execution/TASK-LEDGER.md": [
        "LG-M6-COMBAT-FOUNDATION v0.55.0",
        "M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0",
    ],
    "docs/execution/checklists/M6-COMBAT-FOUNDATION-CLOSURE-CHECKLIST-v0.55.0.md": [
        "M6_COMBAT_FOUNDATION_CLOSURE_CHECKLIST_READY_v0.55.0",
    ],
    "M6-COMBAT-FOUNDATION-FINAL-REPORT-v0.55.0.md": [
        "M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0",
        "M6_COMBAT_FOUNDATION_CLOSURE_VALIDATION_PASS_v0.55.0",
        "Next branch: A",
    ],
    "HANDOFF-LG-M6-COMBAT-FOUNDATION-v0.55.0.md": [
        "M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0",
    ],
    "docs/execution/prompts/M6-COMBAT-HARDENING-CONTINUATION-v0.56.0.md": [
        "M6 Combat Hardening Continuation v0.56.0",
    ],
    "LGO-M6-COMBAT-FOUNDATION-v0.55.0-CHANGED-FILES.txt": [
        "tools/validate_m6_combat_foundation_closure.py",
    ],
    "LGO-M6-COMBAT-FOUNDATION-v0.55.0-DELETIONS.txt": [
        "DELETED / none",
    ],
}
FROZEN_PREFIXES = (
    "protocol/",
    "gamedata/schemas/",
    "docs/adr/",
    "client/Unity/Assets/Game/UI/design-tokens.json",
)


def fail(message: str) -> None:
    print(f"M6 COMBAT FOUNDATION CLOSURE VALIDATION FAILED: {message}", file=sys.stderr)
    raise SystemExit(1)


def git_status_lines() -> list[str]:
    result = subprocess.run(
        ["git", "--no-pager", "status", "--short", "--untracked-files=all"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        fail(result.stderr.strip() or "git status failed")
    return result.stdout.splitlines()


def check_required_files() -> None:
    for rel, markers in REQUIRED.items():
        path = ROOT / rel
        if not path.is_file():
            fail(f"missing file: {rel}")
        text = path.read_text(encoding="utf-8", errors="replace")
        for marker in markers:
            if marker not in text:
                fail(f"{rel} missing marker: {marker}")


def check_no_frozen_changes() -> None:
    for line in git_status_lines():
        rel = line[3:] if len(line) >= 4 else line
        if rel.startswith(FROZEN_PREFIXES):
            fail(f"frozen surface changed: {rel}")


def main() -> int:
    check_required_files()
    check_no_frozen_changes()
    print(MARKER)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
