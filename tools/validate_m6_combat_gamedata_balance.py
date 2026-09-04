#!/usr/bin/env python3
from __future__ import annotations

import importlib.util
import json
import shutil
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MARKER = "M6_COMBAT_GAMEDATA_BALANCE_VALIDATION_PASS_v0.54.0"

FROZEN_PREFIXES = (
    "protocol/",
    "gamedata/schemas/",
    "docs/adr/",
    "client/Unity/Assets/Game/UI/design-tokens.json",
)

REQUIRED_DOCS = (
    "docs/tasks/M6-COMBAT-GAMEDATA-BALANCE-ADVERSARIAL-v0.54.0.md",
    "docs/design/M6-COMBAT-GAMEDATA-BALANCE-NOTES-v0.54.0.md",
    "M6-COMBAT-GAMEDATA-BALANCE-FINAL-REPORT-v0.54.0.md",
    "HANDOFF-LG-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0.md",
    "LGO-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0-CHANGED-FILES.txt",
    "LGO-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0-DELETIONS.txt",
)

SPEC = importlib.util.spec_from_file_location("validate_gamedata", ROOT / "tools" / "validate_gamedata.py")
MODULE = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = MODULE
assert SPEC.loader is not None
SPEC.loader.exec_module(MODULE)


def fail(message: str) -> None:
    print(f"M6 COMBAT GAMEDATA BALANCE VALIDATION FAILED: {message}", file=sys.stderr)
    raise SystemExit(1)


def replace(path: Path, old: str, new: str) -> None:
    text = path.read_text(encoding="utf-8")
    if old not in text:
        fail(f"fixture edit marker not found: {path}:{old}")
    path.write_text(text.replace(old, new), encoding="utf-8")


def validate_copy(edit_name: str, edit) -> tuple[bool, tuple[str, ...], dict]:
    with tempfile.TemporaryDirectory() as tmp:
        gd = Path(tmp) / "gamedata"
        shutil.copytree(ROOT / "gamedata", gd)
        edit(gd)
        result = MODULE.validate_gamedata(gd)
        return result.valid, result.errors, result.manifest


def expect_rejected(name: str, edit, expected: str) -> None:
    valid, errors, _manifest = validate_copy(name, edit)
    if valid:
        fail(f"{name}: invalid combat fixture unexpectedly accepted")
    if not any(expected in item for item in errors):
        fail(f"{name}: expected diagnostic containing {expected!r}, got {errors!r}")


def check_current_balance() -> dict:
    result = MODULE.validate_gamedata(ROOT / "gamedata")
    if not result.valid:
        fail("current GameData is invalid: " + "; ".join(result.errors))

    docs = result.manifest.get("documents", [])
    skills = [item["data"] for item in docs if item.get("kind") == "skills"]
    monsters = [item["data"] for item in docs if item.get("kind") == "monsters"]
    if not skills:
        fail("no skill data found")
    if not monsters:
        fail("no monster data found")

    for skill in skills:
        sid = skill.get("id", "<unknown>")
        cooldown_ms = skill.get("cooldown_ms")
        skill_cd = skill.get("cooldown", {}).get("skill_ms")
        global_cd = skill.get("cooldown", {}).get("global_ms")
        target_range = skill.get("targeting", {}).get("max_range_m")
        range_m = skill.get("range_m")
        amount = skill.get("effect", {}).get("placeholder_amount")
        coefficient = skill.get("damage", {}).get("coefficient")
        rule = skill.get("targeting", {}).get("rule")

        if cooldown_ms != skill_cd:
            fail(f"{sid}: top-level cooldown_ms must match cooldown.skill_ms for M6 prototype readability")
        if not (250 <= global_cd <= cooldown_ms):
            fail(f"{sid}: cooldown.global_ms must be between 250ms and skill cooldown")
        if target_range != range_m:
            fail(f"{sid}: range_m must match targeting.max_range_m")
        if rule == "single_target" and skill.get("targeting", {}).get("requires_target") is not True:
            fail(f"{sid}: single_target skills must require a target")
        if not (1 <= amount <= 250):
            fail(f"{sid}: placeholder_amount outside M6 dev bounds 1..250")
        if not (0.1 <= coefficient <= 5.0):
            fail(f"{sid}: damage.coefficient outside M6 dev bounds 0.1..5.0")

    for monster in monsters:
        mid = monster.get("id", "<unknown>")
        if not (1 <= monster.get("max_hp", 0) <= 5000):
            fail(f"{mid}: max_hp outside M6 dev bounds 1..5000")
        if monster.get("move_speed", -1) < 0:
            fail(f"{mid}: move_speed must not be negative")

    first = json.dumps(result.manifest, ensure_ascii=False, sort_keys=True, indent=2)
    second = json.dumps(MODULE.validate_gamedata(ROOT / "gamedata").manifest, ensure_ascii=False, sort_keys=True, indent=2)
    if first != second:
        fail("deterministic manifest hash changed across repeated validation")
    return result.manifest


def check_adversarial_fixtures() -> None:
    expect_rejected(
        "duplicate combat skill id",
        lambda gd: shutil.copy(gd / "skills" / "wind_slash.yaml", gd / "skills" / "wind_slash_duplicate.yaml"),
        "duplicate id",
    )
    expect_rejected(
        "missing skill class reference",
        lambda gd: replace(gd / "skills" / "wind_slash.yaml", "class_id: class.sword", "class_id: class.missing"),
        "unknown class",
    )
    expect_rejected(
        "negative cooldown",
        lambda gd: replace(gd / "skills" / "wind_slash.yaml", "cooldown_ms: 6000", "cooldown_ms: -1"),
        "cooldown_ms",
    )
    expect_rejected(
        "zero range",
        lambda gd: replace(gd / "skills" / "wind_slash.yaml", "max_range_m: 4.5", "max_range_m: -0.5"),
        "targeting.max_range_m",
    )
    expect_rejected(
        "invalid target type",
        lambda gd: replace(gd / "skills" / "wind_slash.yaml", "rule: single_target", "rule: raid_boss_only"),
        "targeting.rule",
    )
    expect_rejected(
        "effect amount outside schema bound",
        lambda gd: replace(gd / "skills" / "wind_slash.yaml", "placeholder_amount: 12", "placeholder_amount: 1000000"),
        "effect.placeholder_amount",
    )
    expect_rejected(
        "monster hp invalid",
        lambda gd: replace(gd / "monsters" / "shadow_slime.yaml", "max_hp: 120", "max_hp: 0"),
        "max_hp",
    )


def check_docs() -> None:
    for rel in REQUIRED_DOCS:
        path = ROOT / rel
        if not path.is_file():
            fail(f"missing required v0.54 doc: {rel}")
    report = (ROOT / "M6-COMBAT-GAMEDATA-BALANCE-FINAL-REPORT-v0.54.0.md").read_text(encoding="utf-8")
    for marker in (MARKER, "M6_COMBAT_GAMEDATA_BALANCE_CLOSED_LOCAL_v0.54.0", "NO_SCHEMA_CHANGE_REQUIRED"):
        if marker not in report:
            fail(f"final report missing marker: {marker}")


def check_frozen_surface() -> None:
    changed = (ROOT / "LGO-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0-CHANGED-FILES.txt").read_text(encoding="utf-8")
    for line in changed.splitlines():
        item = line.strip()
        if item.startswith("- "):
            rel = item[2:]
            if rel.startswith(FROZEN_PREFIXES):
                fail(f"changed-files lists frozen surface: {rel}")


def main() -> int:
    check_current_balance()
    check_adversarial_fixtures()
    check_docs()
    check_frozen_surface()
    print(MARKER)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
