#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FROZEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FORBIDDEN_STATUS = ("__pycache__/", ".pyc", ".DS_Store", "__MACOSX/", "build/")


def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f"missing: {path}")
        return ""
    return target.read_text(encoding="utf-8", errors="replace")


def require(path: str, *markers: str) -> None:
    content = read(path)
    for marker in markers:
        if marker not in content:
            errors.append(f"{path} missing marker: {marker}")


def require_file(path: str, executable: bool = False) -> None:
    target = ROOT / path
    if not target.is_file():
        errors.append(f"missing file: {path}")
        return
    if executable and not os.access(target, os.X_OK):
        errors.append(f"file is not executable: {path}")


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(["git", "--no-pager", *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append("git command failed: git --no-pager " + " ".join(args) + " " + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    require_file("tools/validate_m6_local_combat_runtime_closure.py", executable=True)
    require(
        "HANDOFF-LG-M6-LOCAL-COMBAT-PROTOTYPE-v0.49.0.md",
        "M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0",
        "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0",
    )
    require(
        "docs/tasks/M6-LOCAL-COMBAT-RUNTIME-CLOSURE-v0.50.0.md",
        "M6_LOCAL_COMBAT_RUNTIME_CLOSED_LOCAL_v0.50.0",
        "M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0",
        "accepted Wind Slash",
        "NO_TARGET",
        "OUT_OF_RANGE",
        "COOLDOWN_ACTIVE",
        "No production combat",
    )
    require(
        "docs/execution/checklists/M6-LOCAL-COMBAT-RUNTIME-CLOSURE-CHECKLIST-v0.50.0.md",
        "M6_LOCAL_COMBAT_RUNTIME_CLOSED_LOCAL_v0.50.0",
        "Runtime Cases",
        "Runtime smoke uses nonzero checks",
        "Frozen Surface Audit",
    )
    require(
        "M6-LOCAL-COMBAT-RUNTIME-CLOSURE-FINAL-REPORT-v0.50.0.md",
        "M6_LOCAL_COMBAT_RUNTIME_CLOSED_LOCAL_v0.50.0",
        "M6_LOCAL_COMBAT_RUNTIME_CLOSURE_PASS_v0.50.0",
        "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0",
        "LGO_PLAYABLE_VISUAL_EVIDENCE_READY",
        "Runtime Gate Table",
        "Non-Claims",
    )
    require(
        "HANDOFF-LG-M6-LOCAL-COMBAT-RUNTIME-CLOSURE-v0.50.0.md",
        "M6_LOCAL_COMBAT_RUNTIME_CLOSED_LOCAL_v0.50.0",
        "M6_LOCAL_COMBAT_RUNTIME_CLOSURE_PASS_v0.50.0",
        "Next Allowed Task",
        "Frozen Surface Audit",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_m6_local_combat_runtime_closure.py",
        "M6_LOCAL_COMBAT_RUNTIME_CLOSURE_PASS_v0.50.0",
        "LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS",
    )
    require(
        "client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs",
        "executedChecks",
        "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0",
        "rejectedNoTarget",
        "rejectedOutOfRange",
        "rejectedCooldownReason",
    )
    require(
        "tools/run_m6_minimal_local_combat_once.sh",
        "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0",
        "executedChecks",
        "rejectedNoTarget",
        "rejectedOutOfRange",
        "cooldownBlockedAfterRepeatedInput",
    )
    require(
        "LGO-M6-LOCAL-COMBAT-RUNTIME-CLOSURE-v0.50.0-CHANGED-FILES.txt",
        "tools/validate_m6_local_combat_runtime_closure.py",
        "tools/lgo_playable_closure_check.sh",
    )
    require("LGO-M6-LOCAL-COMBAT-RUNTIME-CLOSURE-v0.50.0-DELETIONS.txt", "DELETED", "none")

    for path in git_lines("diff", "--name-only"):
        if path == "client/Unity/Assets/Game/UI/design-tokens.json":
            errors.append(f"frozen surface modified: {path}")
        for prefix in FROZEN_PREFIXES:
            if path.startswith(prefix):
                errors.append(f"frozen surface modified: {path}")

    for line in git_lines("status", "--short", "--untracked-files=all"):
        status = line[:2]
        if "D" in status:
            continue
        path = line[3:] if len(line) >= 4 else line
        if any(fragment in path for fragment in FORBIDDEN_STATUS):
            errors.append(f"forbidden cache/source artifact present: {path}")

    combined_docs = "\n".join(
        read(path)
        for path in [
            "docs/tasks/M6-LOCAL-COMBAT-RUNTIME-CLOSURE-v0.50.0.md",
            "M6-LOCAL-COMBAT-RUNTIME-CLOSURE-FINAL-REPORT-v0.50.0.md",
            "HANDOFF-LG-M6-LOCAL-COMBAT-RUNTIME-CLOSURE-v0.50.0.md",
        ]
    )
    forbidden_claims = ["production combat is implemented", "production art is claimed", "full MMO readiness is claimed"]
    for claim in forbidden_claims:
        if claim in combined_docs:
            errors.append(f"forbidden production/full-MMO claim: {claim}")

    if errors:
        print("M6 LOCAL COMBAT RUNTIME CLOSURE VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6_LOCAL_COMBAT_RUNTIME_CLOSURE_VALIDATION_PASS_v0.50.0")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
