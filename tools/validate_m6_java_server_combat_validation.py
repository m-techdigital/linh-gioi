#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
ALLOWED_PREFIXES = ["server/realtime/src/main/java/", "server/realtime/src/test/java/"]
ALLOWED_FILES = {
    "tools/validate_m6_java_server_combat_validation.py",
    "tools/lgo_playable_closure_check.sh",
    "docs/tasks/M6-JAVA-SERVER-COMBAT-VALIDATION-v0.41.0.md",
    "M6-JAVA-SERVER-COMBAT-VALIDATION-FINAL-REPORT-v0.41.0.md",
    "HANDOFF-LG-M6-JAVA-SERVER-COMBAT-VALIDATION-v0.41.0.md",
    "LGO-M6-JAVA-SERVER-COMBAT-VALIDATION-v0.41.0-CHANGED-FILES.txt",
    "LGO-M6-JAVA-SERVER-COMBAT-VALIDATION-v0.41.0-DELETIONS.txt",
}
FORBIDDEN_PREFIXES = ["protocol/", "gamedata/schemas/", "docs/adr/", "client/Unity/Assets/Game/UI/design-tokens.json"]


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
        errors.append("git command failed: git --no-pager " + " ".join(args))
        return []
    return result.stdout.splitlines()


def main() -> int:
    require_file("tools/validate_m6_java_server_combat_validation.py", executable=True)
    require(
        "server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatValidationService.java",
        "CombatIntent",
        "CombatAccepted",
        "CombatRejected",
        "CombatStateSnapshot",
        "HandshakeProtocol.PROTOCOL_VERSION",
        "LongSupplier",
        "cooldownUntilByActor",
    )
    require(
        "server/realtime/src/test/java/com/linhgioi/server/realtime/combat/CombatValidationServiceTest.java",
        "validIntentIsAcceptedWithCooldownSnapshot",
        "invalidTargetIsRejected",
        "cooldownBlocksThenRecovers",
    )
    require(
        "docs/tasks/M6-JAVA-SERVER-COMBAT-VALIDATION-v0.41.0.md",
        "M6_JAVA_SERVER_COMBAT_VALIDATION_SOURCE_READY_v0.41.0",
        "generated Java protocol classes",
        "No protocol/GameData changes",
    )

    source = read("server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatValidationService.java")
    for forbidden in ["CombatIntentDto", "CombatStateDto", "record CombatIntent", "class CombatIntent"]:
        if forbidden in source:
            errors.append(f"server combat validation uses forbidden parallel DTO marker: {forbidden}")

    for path in git_lines("diff", "--name-only"):
        if path in ALLOWED_FILES or any(path.startswith(prefix) for prefix in ALLOWED_PREFIXES):
            continue
        for prefix in FORBIDDEN_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f"forbidden source path modified in v0.41: {path}")

    if errors:
        print("M6 JAVA SERVER COMBAT VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6 JAVA SERVER COMBAT VALIDATION PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
