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
    "client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatSmokeRunner.cs",
    "client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatSmokeRunner.cs.meta",
    "tools/run_m6_unity_java_combat_smoke.sh",
    "tools/validate_m6_unity_java_combat_smoke.py",
    "tools/lgo_playable_closure_check.sh",
    "docs/tasks/M6-UNITY-JAVA-COMBAT-SMOKE-v0.43.0.md",
    "M6-UNITY-JAVA-COMBAT-SMOKE-FINAL-REPORT-v0.43.0.md",
    "HANDOFF-LG-M6-UNITY-JAVA-COMBAT-SMOKE-v0.43.0.md",
    "LGO-M6-UNITY-JAVA-COMBAT-SMOKE-v0.43.0-CHANGED-FILES.txt",
    "LGO-M6-UNITY-JAVA-COMBAT-SMOKE-v0.43.0-DELETIONS.txt",
}
FORBIDDEN_PREFIXES = ["protocol/", "gamedata/schemas/", "docs/adr/"]


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
    require_file("tools/validate_m6_unity_java_combat_smoke.py", executable=True)
    require_file("tools/run_m6_unity_java_combat_smoke.sh", executable=True)
    require(
        "server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatSmokeServer.java",
        "CombatIntent.parseFrom",
        "CombatAccepted",
        "CombatRejected",
        "M6_COMBAT_SMOKE_SERVER_READY",
    )
    require(
        "client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatSmokeRunner.cs",
        "--lgo-m6-unity-java-combat-smoke",
        "M6_UNITY_JAVA_COMBAT_SMOKE_PASS",
        "Google.Protobuf",
        "CombatAccepted.Parser.ParseFrom",
        "CombatRejected.Parser.ParseFrom",
    )
    require(
        "docs/tasks/M6-UNITY-JAVA-COMBAT-SMOKE-v0.43.0.md",
        "M6_UNITY_JAVA_COMBAT_SMOKE_SOURCE_READY_FOR_RUNTIME_v0.43.0",
        "canonical protobuf",
        "No protocol/GameData changes",
    )
    for path in git_lines("diff", "--name-only"):
        if path in ALLOWED_FILES:
            continue
        for prefix in FORBIDDEN_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f"forbidden source path modified in v0.43: {path}")

    if errors:
        print("M6 UNITY JAVA COMBAT SMOKE VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6 UNITY JAVA COMBAT SMOKE VALIDATION PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
