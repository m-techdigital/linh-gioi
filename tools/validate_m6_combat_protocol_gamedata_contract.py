#!/usr/bin/env python3
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

ALLOWED_CHANGED = {
    "protocol/combat.proto",
    "gamedata/schemas/skill.schema.json",
    "gamedata/skills/wind_slash.yaml",
    "gamedata/compiled/gamedata-manifest.json",
    "tests/gamedata/test_gamedata_pipeline.py",
    "tests/gamedata/__pycache__/test_gamedata_pipeline.cpython-312.pyc",
    "tools/validate_m6_combat_protocol_gamedata_contract.py",
    "tools/validate_m6_server_combat_contract_spec.py",
    "tools/validate_m6_combat_readiness_spec.py",
    "tools/validate_master_roadmap.py",
    "tools/validate_m4_2_playable_ui.py",
    "tools/validate_m4_stabilization.py",
    "tools/validate_m4_visible_ui.py",
    "tools/lgo_playable_closure_check.sh",
    "server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatValidationService.java",
    "server/realtime/src/test/java/com/linhgioi/server/realtime/combat/CombatValidationServiceTest.java",
    "tools/run_m6_server_authoritative_combat_pilot.sh",
    "tools/validate_m6_server_authoritative_combat_pilot.py",
    "docs/tasks/M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md",
    "docs/design/LGO-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md",
    "M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-FINAL-REPORT-v0.40.0.md",
    "HANDOFF-LG-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md",
    "LGO-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0-CHANGED-FILES.txt",
    "LGO-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0-DELETIONS.txt",
}
FORBIDDEN_PREFIXES = [
    "docs/adr/",
    "client/Unity/Assets/Game/UI/design-tokens.json",
    "server/",
]
FORBIDDEN_OUTPUT_PREFIXES = [
    "build/",
    "client/Unity/Library/",
    "client/Unity/Temp/",
    "client/Unity/Logs/",
    "client/Unity/Assets/Game/Generated/",
    "client/Unity/Assets/Game/Protocol/Generated/",
    "server/scripts/__pycache__/",
    "tools/__pycache__/",
    "tests/gamedata/__pycache__/",
]


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
    result = subprocess.run(
        ["git", "--no-pager", *args],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        errors.append("git command failed: git --no-pager " + " ".join(args) + " " + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    require_file("tools/validate_m6_combat_protocol_gamedata_contract.py", executable=True)
    require(
        "docs/tasks/M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md",
        "M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0",
        "CombatIntent",
        "CombatAccepted",
        "CombatRejected",
        "CombatResult",
        "CombatStateSnapshot",
        "skill activation",
        "cooldown",
        "targeting rule",
        "effect rule placeholder",
        "telegraph/readability rule placeholder",
        "No Java server combat validation implementation",
        "No Unity combat intent integration",
    )
    require(
        "docs/design/LGO-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md",
        "client input",
        "server validates",
        "Local preview is never a server result",
        "canonical protobuf messages",
    )
    require(
        "CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md",
        "why current contracts are insufficient",
        "Proposed Protocol Areas",
        "Proposed GameData Areas",
    )

    proto = read("protocol/combat.proto")
    for marker in ["message CombatIntent", "message CombatAccepted", "message CombatRejected", "message CombatResult", "message CombatStateSnapshot", "ErrorInfo error", "bool local_preview_only"]:
        if marker not in proto:
            errors.append(f"protocol/combat.proto missing marker: {marker}")
    for legacy in ["message BasicAttackIntent", "message SkillIntent", "message CombatResultEvent"]:
        if legacy not in proto:
            errors.append(f"protocol/combat.proto lost legacy marker: {legacy}")

    schema = json.loads(read("gamedata/schemas/skill.schema.json") or "{}")
    required = set(schema.get("required", []))
    for field in ["activation", "cooldown", "targeting", "effect", "telegraph"]:
        if field not in required:
            errors.append(f"skill schema does not require combat field: {field}")
        if field not in schema.get("properties", {}):
            errors.append(f"skill schema missing combat property: {field}")

    skill = read("gamedata/skills/wind_slash.yaml")
    for marker in ["activation:", "cooldown:", "targeting:", "effect:", "telegraph:", "rule: single_target", "rule: placeholder_damage", "readability_ms: 250"]:
        if marker not in skill:
            errors.append(f"wind_slash.yaml missing marker: {marker}")
    manifest = read("gamedata/compiled/gamedata-manifest.json")
    for marker in ["activation", "targeting", "telegraph", "b5a57d7a51c5a78d87aa8542c9eeff68ed9fc1c961fd58e7ab7d8617ad1a4abf"]:
        if marker not in manifest:
            errors.append(f"compiled GameData manifest missing marker: {marker}")

    tests = read("tests/gamedata/test_gamedata_pipeline.py")
    for marker in ["test_skill_activation_rule_is_rejected", "test_skill_cooldown_rule_is_rejected", "test_skill_targeting_rule_is_rejected"]:
        if marker not in tests:
            errors.append(f"GameData tests missing invalid fixture coverage: {marker}")

    for path in git_lines("diff", "--name-only"):
        if path not in ALLOWED_CHANGED:
            for prefix in FORBIDDEN_PREFIXES:
                if path == prefix or path.startswith(prefix):
                    errors.append(f"forbidden source path modified in v0.40: {path}")
            if path.startswith(("protocol/", "gamedata/")) and path not in ALLOWED_CHANGED:
                errors.append(f"unexpected protocol/GameData path modified in v0.40: {path}")
    for line in git_lines("status", "--short", "--untracked-files=all"):
        status = line[:2]
        if "D" in status:
            continue
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f"generated/cache/build output under source status: {path}")

    if errors:
        print("M6 COMBAT PROTOCOL GAMEDATA CONTRACT VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6 COMBAT PROTOCOL GAMEDATA CONTRACT VALIDATION PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
