#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

FORBIDDEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FORBIDDEN_FILES = {"client/Unity/Assets/Game/UI/design-tokens.json"}
ALLOWED_PREFIXES = (
    "server/realtime/src/main/java/",
    "server/realtime/src/test/java/",
)
ALLOWED_FILES = {
    "tools/validate_m6_server_authoritative_combat_pilot.py",
    "tools/run_m6_server_authoritative_combat_pilot.sh",
    "tools/lgo_playable_closure_check.sh",
    "docs/tasks/M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-v0.51.0.md",
    "docs/execution/checklists/M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-CHECKLIST-v0.51.0.md",
    "M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-FINAL-REPORT-v0.51.0.md",
    "HANDOFF-LG-M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-v0.51.0.md",
    "LGO-M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-v0.51.0-CHANGED-FILES.txt",
    "LGO-M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-v0.51.0-DELETIONS.txt",
    "LGO-M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-v0.51.0-ARTIFACTS-SHA256.txt",
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
    result = subprocess.run(
        ["git", "--no-pager", *args],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        errors.append("git command failed: git --no-pager " + " ".join(args))
        return []
    return result.stdout.splitlines()


def main() -> int:
    require_file("tools/validate_m6_server_authoritative_combat_pilot.py", executable=True)
    require_file("tools/run_m6_server_authoritative_combat_pilot.sh", executable=True)
    require(
        "server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatValidationService.java",
        "validatePilot",
        "CombatAccepted",
        "CombatRejected",
        "CombatResult",
        "CombatStateSnapshot",
        "DEFAULT_SKILL_RANGE_M",
        "DEFAULT_PLACEHOLDER_EFFECT_AMOUNT",
        "combat_intent_rejected_",
        "no_target",
        "out_of_range",
        "cooldown",
        "SERVER_AUTHORITATIVE_PLACEHOLDER_HIT",
    )
    require(
        "server/realtime/src/test/java/com/linhgioi/server/realtime/combat/CombatValidationServiceTest.java",
        "validPilotIntentEmitsAcceptedResultAndSnapshot",
        "noTargetIsRejected",
        "invalidTargetIsRejected",
        "unknownSkillIsRejected",
        "outOfRangeIsRejected",
        "cooldownBlocksThenRecovers",
    )
    require(
        "tools/run_m6_server_authoritative_combat_pilot.sh",
        "CombatValidationServiceTest",
        "M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_PASS_v0.51.0",
    )
    require(
        "docs/tasks/M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-v0.51.0.md",
        "M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_SOURCE_READY_v0.51.0",
        "existing protocol",
        "No protocol/GameData schema changes",
    )
    require(
        "M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-FINAL-REPORT-v0.51.0.md",
        "M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_CLOSED_LOCAL_v0.51.0",
        "M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_PASS_v0.51.0",
        "No production combat claim",
    )
    require(
        "HANDOFF-LG-M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-v0.51.0.md",
        "Frozen Surface Audit",
        "Contract Change",
        "Next Allowed Task",
    )

    service = read("server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatValidationService.java")
    for forbidden in ("CombatIntentDto", "CombatStateDto", "record CombatIntent", "class CombatIntent"):
        if forbidden in service:
            errors.append(f"forbidden parallel DTO marker in server pilot: {forbidden}")

    for path in git_lines("diff", "--name-only"):
        if path in FORBIDDEN_FILES or path.startswith(FORBIDDEN_PREFIXES):
            errors.append(f"forbidden frozen surface modified: {path}")
        if path in ALLOWED_FILES or path.startswith(ALLOWED_PREFIXES):
            continue

    if errors:
        print("M6 SERVER AUTHORITATIVE COMBAT PILOT VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6 SERVER AUTHORITATIVE COMBAT PILOT VALIDATION PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
