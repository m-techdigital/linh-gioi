#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
ALLOWED_FILES = {
    "server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatSmokeServer.java",
    "client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs",
    "client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatE2ERunner.cs",
    "client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatE2ERunner.cs.meta",
    "tools/run_m6_unity_java_combat_e2e.sh",
    "tools/validate_m6_unity_java_combat_e2e.py",
    "tools/lgo_playable_closure_check.sh",
    "docs/tasks/M6-UNITY-JAVA-COMBAT-E2E-RUNTIME-CLOSURE-v0.52.0.md",
    "docs/execution/checklists/M6-UNITY-JAVA-COMBAT-E2E-CHECKLIST-v0.52.0.md",
    "M6-UNITY-JAVA-COMBAT-E2E-RUNTIME-CLOSURE-FINAL-REPORT-v0.52.0.md",
    "HANDOFF-LG-M6-UNITY-JAVA-COMBAT-E2E-RUNTIME-CLOSURE-v0.52.0.md",
    "LGO-M6-UNITY-JAVA-COMBAT-E2E-v0.52.0-CHANGED-FILES.txt",
    "LGO-M6-UNITY-JAVA-COMBAT-E2E-v0.52.0-DELETIONS.txt",
}
FORBIDDEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FORBIDDEN_FILES = {"client/Unity/Assets/Game/UI/design-tokens.json"}


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
        errors.append("git command failed: git --no-pager " + " ".join(args))
        return []
    return result.stdout.splitlines()


def main() -> int:
    require_file("tools/validate_m6_unity_java_combat_e2e.py", executable=True)
    require_file("tools/run_m6_unity_java_combat_e2e.sh", executable=True)
    require(
        "server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatSmokeServer.java",
        "validatePilot",
        "RESPONSE_RESULT",
        "RESPONSE_SNAPSHOT",
        "intent.getLocalPreviewOnly()",
    )
    require(
        "client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs",
        "M6UnityJavaCombatE2ERunner.ShouldRun",
        "M6UnityJavaCombatE2ERunner.RunFromCommandLineAsync",
    )
    require(
        "client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatE2ERunner.cs",
        "--lgo-m6-unity-java-combat-e2e",
        "M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0",
        "CombatAccepted.Parser.ParseFrom",
        "CombatResult.Parser.ParseFrom",
        "CombatStateSnapshot.Parser.ParseFrom",
        "rejectedNoTarget",
        "rejectedOutOfRange",
        "rejectedCooldown",
        "rejectedInvalidSkill",
        "Mô phỏng cục bộ",
    )
    require(
        "tools/run_m6_unity_java_combat_e2e.sh",
        "CombatSmokeServer",
        "--lgo-m6-unity-java-combat-e2e",
        "M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0",
    )
    require(
        "docs/tasks/M6-UNITY-JAVA-COMBAT-E2E-RUNTIME-CLOSURE-v0.52.0.md",
        "M6_UNITY_JAVA_COMBAT_E2E_SOURCE_READY_v0.52.0",
        "existing protocol",
        "No protocol/GameData schema changes",
    )
    require(
        "M6-UNITY-JAVA-COMBAT-E2E-RUNTIME-CLOSURE-FINAL-REPORT-v0.52.0.md",
        "M6_UNITY_JAVA_COMBAT_E2E_CLOSED_LOCAL_v0.52.0",
        "M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0",
        "M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_PASS_v0.51.0",
    )

    for path in git_lines("diff", "--name-only"):
        if path in FORBIDDEN_FILES or path.startswith(FORBIDDEN_PREFIXES):
            errors.append(f"forbidden frozen surface modified: {path}")
        if path in ALLOWED_FILES:
            continue

    if errors:
        print("M6 UNITY JAVA COMBAT E2E VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6 UNITY JAVA COMBAT E2E VALIDATION PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
