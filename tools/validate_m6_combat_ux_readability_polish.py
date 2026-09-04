#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
ALLOWED_FILES = {
    "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
    "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
    "tools/validate_m6_combat_ux_readability_polish.py",
    "tools/lgo_playable_closure_check.sh",
    "tools/validate_m6_combat_protocol_gamedata_contract.py",
    "tools/validate_m6_combat_readiness_spec.py",
    "tools/validate_m6_combat_input_feedback_stability.py",
    "tools/validate_m6_combat_ux_feedback.py",
    "tools/validate_m6_server_combat_contract_spec.py",
    "tools/lgo_m4_closure_check.sh",
    "docs/tasks/M6-COMBAT-UX-READABILITY-POLISH-v0.53.0.md",
    "docs/design/M6-COMBAT-UX-READABILITY-POLISH-v0.53.0.md",
    "docs/execution/checklists/M6-COMBAT-UX-READABILITY-POLISH-CHECKLIST-v0.53.0.md",
    "M6-COMBAT-UX-READABILITY-POLISH-FINAL-REPORT-v0.53.0.md",
    "HANDOFF-LG-M6-COMBAT-UX-READABILITY-POLISH-v0.53.0.md",
    "LGO-M6-COMBAT-UX-READABILITY-POLISH-v0.53.0-CHANGED-FILES.txt",
    "LGO-M6-COMBAT-UX-READABILITY-POLISH-v0.53.0-DELETIONS.txt",
    "tools/validate_m6_combat_gamedata_balance.py",
    "docs/tasks/M6-COMBAT-GAMEDATA-BALANCE-ADVERSARIAL-v0.54.0.md",
    "docs/design/M6-COMBAT-GAMEDATA-BALANCE-NOTES-v0.54.0.md",
    "docs/execution/checklists/M6-COMBAT-GAMEDATA-BALANCE-CHECKLIST-v0.54.0.md",
    "M6-COMBAT-GAMEDATA-BALANCE-FINAL-REPORT-v0.54.0.md",
    "HANDOFF-LG-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0.md",
    "LGO-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0-CHANGED-FILES.txt",
    "LGO-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0-DELETIONS.txt",
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
    require_file("tools/validate_m6_combat_ux_readability_polish.py", executable=True)
    require(
        "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
        "TargetDummyRangeText",
        "DescribeTargetDummyRangeState",
        "Ngoài tầm",
        "Đang hồi chiêu",
        "Chưa chọn mục tiêu",
        "Chém Gió",
        "không tạo kết quả chiến đấu thật",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_combatRangeStatus",
        "ApplyStatusAccent",
        "Tấn công thử",
        "Đang hồi chiêu",
        "RuntimeArtCatalog.Danger",
        "CombatButtonCooldownTexture",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "m6_combat_ux_readability_polish",
        "validate_m6_combat_ux_readability_polish.py",
    )
    require(
        "docs/tasks/M6-COMBAT-UX-READABILITY-POLISH-v0.53.0.md",
        "M6_COMBAT_UX_READABILITY_POLISH_SOURCE_READY_v0.53.0",
        "No new combat mechanics",
    )
    require(
        "docs/design/M6-COMBAT-UX-READABILITY-POLISH-v0.53.0.md",
        "target selected",
        "out-of-range",
        "cooldown",
        "Vietnamese",
    )
    require(
        "M6-COMBAT-UX-READABILITY-POLISH-FINAL-REPORT-v0.53.0.md",
        "M6_COMBAT_UX_READABILITY_POLISH_CLOSED_LOCAL_v0.53.0",
        "M6_COMBAT_UX_READABILITY_POLISH_PASS_v0.53.0",
    )

    for path in git_lines("diff", "--name-only"):
        if path in FORBIDDEN_FILES or path.startswith(FORBIDDEN_PREFIXES):
            errors.append(f"forbidden frozen surface modified: {path}")
        if path in ALLOWED_FILES:
            continue

    if errors:
        print("M6 COMBAT UX READABILITY POLISH VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6_COMBAT_UX_READABILITY_POLISH_PASS_v0.53.0")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
