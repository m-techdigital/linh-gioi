#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MARKER = "M6_COMBAT_HARDENING_CONTINUATION_VALIDATION_PASS_v0.56.0"
errors: list[str] = []

FROZEN_PREFIXES = (
    "protocol/",
    "gamedata/schemas/",
    "docs/adr/",
)
FROZEN_FILES = {
    "client/Unity/Assets/Game/UI/design-tokens.json",
}

def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f"missing: {path}")
        return ""
    return target.read_text(encoding="utf-8", errors="replace")


def require_file(path: str, executable: bool = False) -> None:
    target = ROOT / path
    if not target.is_file():
        errors.append(f"missing file: {path}")
        return
    if executable and not os.access(target, os.X_OK):
        errors.append(f"file is not executable: {path}")


def require(path: str, *markers: str) -> None:
    content = read(path)
    for marker in markers:
        if marker not in content:
            errors.append(f"{path} missing marker: {marker}")


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(["git", "--no-pager", *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append("git command failed: git --no-pager " + " ".join(args) + " " + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def check_frozen_surfaces() -> None:
    for path in git_lines("diff", "--name-only"):
        if path in FROZEN_FILES or path.startswith(FROZEN_PREFIXES):
            errors.append(f"frozen surface modified: {path}")


def main() -> int:
    require_file("tools/validate_m6_combat_hardening_continuation.py", executable=True)
    require(
        "client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs",
        "acceptedIntentId",
        "acceptedSequence",
        "acceptedCooldownMs",
        "acceptedOutcome",
        "acceptedSnapshotTargetValid",
        "rejectedNoTargetRetryable",
        "rejectedOutOfRangeRetryable",
        "rejectedCooldownRetryable",
        "rejectedCooldownRemainingMs",
        "rejectedCooldownSnapshotTargetValid",
        "diagnostic evidence is incomplete",
    )
    require(
        "client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatE2ERunner.cs",
        "acceptedIntentId",
        "acceptedSequence",
        "acceptedCooldownMs",
        "resultOutcome",
        "resultEffectAmount",
        "snapshotCooldownRemainingMs",
        "rejectedNoTargetCode",
        "rejectedOutOfRangeCode",
        "rejectedCooldownCode",
        "rejectedInvalidSkillCode",
        "rejectedCooldownRetryable",
        "server accepted diagnostic evidence missing",
        "server rejection diagnostic evidence missing",
    )
    require(
        "docs/tasks/M6-COMBAT-HARDENING-CONTINUATION-v0.56.0.md",
        "M6_COMBAT_HARDENING_CONTINUATION_SOURCE_READY_v0.56.0",
        "No new combat mechanics",
        "diagnostic evidence",
    )
    require(
        "docs/execution/checklists/M6-COMBAT-HARDENING-CONTINUATION-CHECKLIST-v0.56.0.md",
        "M6_COMBAT_HARDENING_CONTINUATION_CHECKLIST_READY_v0.56.0",
        "Frozen Surface Audit",
        "Runtime Evidence",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_m6_combat_hardening_continuation.py",
    )
    check_frozen_surfaces()

    if errors:
        print("M6 COMBAT HARDENING CONTINUATION VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print(MARKER)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
