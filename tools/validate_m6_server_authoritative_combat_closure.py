#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []


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
    require_file("tools/validate_m6_server_authoritative_combat_closure.py", executable=True)
    require("M6-SERVER-AUTHORITATIVE-COMBAT-CLOSURE-FINAL-REPORT-v0.44.0.md", "M6_SERVER_AUTHORITATIVE_COMBAT_FOUNDATION_RUNTIME_CLOSED_LOCAL_v0.44.0", "M6_UNITY_JAVA_COMBAT_SMOKE_PASS", "No full MMO runtime closure")
    require("HANDOFF-LG-M6-SERVER-AUTHORITATIVE-COMBAT-FOUNDATION-v0.44.0.md", "Stage Decisions", "Frozen Surface Audit", "Package Artifacts")
    require("LGO-M6-SERVER-AUTHORITATIVE-COMBAT-FOUNDATION-v0.44.0-CHANGED-FILES.txt", "protocol/combat.proto", "CombatValidationService.java", "M6UnityJavaCombatSmokeRunner.cs")
    require("LGO-M6-SERVER-AUTHORITATIVE-COMBAT-FOUNDATION-v0.44.0-DELETIONS.txt", "DELETED")
    require("LGO-M6-SERVER-AUTHORITATIVE-COMBAT-FOUNDATION-v0.44.0-ARTIFACTS-SHA256.txt", "linh-gioi-m6-server-authoritative-combat-foundation-v0.44.0-full-source.zip", "linh-gioi-m6-server-authoritative-combat-foundation-v0.44.0-delta.zip")
    require("tools/lgo_playable_closure_check.sh", "m6_unity_combat_intent_client_runtime", "m6_unity_java_combat_runtime")
    for path in git_lines("diff", "--name-only"):
        if path.startswith(("protocol/", "gamedata/schemas/", "docs/adr/")):
            errors.append(f"forbidden contract surface modified after v0.40/v0.43: {path}")
        if path == "client/Unity/Assets/Game/UI/design-tokens.json":
            errors.append("design tokens modified in v0.44 closure")

    if errors:
        print("M6 SERVER AUTHORITATIVE COMBAT CLOSURE VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6 SERVER AUTHORITATIVE COMBAT CLOSURE VALIDATION PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
