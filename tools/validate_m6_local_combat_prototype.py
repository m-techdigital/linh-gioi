#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FROZEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FORBIDDEN_STATUS = ("__pycache__/", ".pyc", ".DS_Store", "__MACOSX/")


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
    require(
        "docs/tasks/M6-LOCAL-COMBAT-PROTOTYPE-v0.49.0.md",
        "M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0",
        "LocalCombatPrototypeState",
        "rejected no-target",
        "No protocol changes",
        "No GameData schema changes",
    )
    require(
        "docs/design/M6-LOCAL-COMBAT-PROTOTYPE-DESIGN-v0.49.0.md",
        "Accepted Path",
        "Rejected Paths",
        "NO_TARGET",
        "OUT_OF_RANGE",
        "COOLDOWN_ACTIVE",
        "No server-authoritative combat",
    )
    require(
        "client/Unity/Assets/Game/World/Runtime/LocalCombatPrototypeState.cs",
        "WindSlashCooldownMs = 6000",
        "WindSlashPlaceholderAmount = 12",
        "WindSlashRangeM = 4.5f",
        "TryWindSlash",
        "CombatAccepted",
        "CombatRejected",
        "CombatResult",
        "CombatStateSnapshot",
        "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0",
        "NO_TARGET",
        "OUT_OF_RANGE",
        "COOLDOWN_ACTIVE",
    )
    require(
        "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
        "LocalCombatPrototypeState",
        "TryLocalCombatPrototypeAt",
        "TryLocalCombatPrototypeWithoutTargetForSmoke",
        "Chấp nhận cục bộ",
        "Từ chối cục bộ",
        "CombatPlaceholderAssets.TargetDummyHit",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "TryLocalCombatPrototype",
        "Gửi ý định chiến đấu",
        "Hồi chiêu mô phỏng",
    )
    require(
        "client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs",
        "executedChecks",
        "rejectedNoTarget",
        "rejectedOutOfRange",
        "rejectedCooldownReason",
        "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0",
        "M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS",
        "local-only prototype",
    )
    require(
        "tools/run_m6_minimal_local_combat_once.sh",
        "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0",
        "executedChecks",
        "rejectedNoTarget",
        "rejectedOutOfRange",
        "cooldownBlockedAfterRepeatedInput",
    )
    require("M6-LOCAL-COMBAT-PROTOTYPE-FINAL-REPORT-v0.49.0.md", "M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0", "accepted Wind Slash", "rejected no-target", "non-claims")
    require("HANDOFF-LG-M6-LOCAL-COMBAT-PROTOTYPE-v0.49.0.md", "M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0", "Frozen Surface Audit", "Next allowed task")
    require("LGO-M6-LOCAL-COMBAT-PROTOTYPE-v0.49.0-CHANGED-FILES.txt", "LocalCombatPrototypeState.cs", "validate_m6_local_combat_prototype.py")
    require("LGO-M6-LOCAL-COMBAT-PROTOTYPE-v0.49.0-DELETIONS.txt", "DELETED", "none")
    require_file("tools/validate_m6_local_combat_prototype.py", executable=True)

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

    if errors:
        print("M6 LOCAL COMBAT PROTOTYPE VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6_LOCAL_COMBAT_PROTOTYPE_VALIDATION_PASS_v0.49.0")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
