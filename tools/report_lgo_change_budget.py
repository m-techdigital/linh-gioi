#!/usr/bin/env python3
from __future__ import annotations

import argparse
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
DEFAULT_MAX_FILES = 18
DEFAULT_MAX_CHANGED_LINES = 700
HEAVY_PATH_PREFIXES = (
    "protocol/",
    "gamedata/schemas/",
    "docs/adr/",
    "client/Unity/Assets/Game/UI/design-tokens.json",
)
NOISY_PATH_PARTS = (
    "/Library/",
    "/Temp/",
    "/Logs/",
    "__pycache__",
)


@dataclass(frozen=True)
class DiffStat:
    path: str
    added: int
    deleted: int

    @property
    def changed(self) -> int:
        return self.added + self.deleted


def run_git(args: list[str]) -> str:
    result = subprocess.run(
        ["git", "--no-pager", *args],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        check=False,
    )
    if result.returncode != 0:
        print(result.stdout, end="")
        raise SystemExit(result.returncode)
    return result.stdout


def parse_numstat(text: str) -> list[DiffStat]:
    stats: list[DiffStat] = []
    for line in text.splitlines():
        parts = line.split("\t")
        if len(parts) < 3:
            continue
        added_raw, deleted_raw, path = parts[0], parts[1], parts[2]
        added = 0 if added_raw == "-" else int(added_raw)
        deleted = 0 if deleted_raw == "-" else int(deleted_raw)
        stats.append(DiffStat(path=path, added=added, deleted=deleted))
    return stats


def untracked_files() -> list[str]:
    status = run_git(["status", "--short", "--untracked-files=all"])
    files: list[str] = []
    for line in status.splitlines():
        if line.startswith("?? "):
            files.append(line[3:])
    return files


def main() -> int:
    parser = argparse.ArgumentParser(description="Report LGO local change budget.")
    parser.add_argument("--max-files", type=int, default=DEFAULT_MAX_FILES)
    parser.add_argument("--max-changed-lines", type=int, default=DEFAULT_MAX_CHANGED_LINES)
    parser.add_argument("--enforce", action="store_true")
    args = parser.parse_args()

    stats = parse_numstat(run_git(["diff", "--numstat"]))
    untracked = untracked_files()
    changed_paths = {item.path for item in stats}
    all_paths = sorted(changed_paths | set(untracked))
    changed_lines = sum(item.changed for item in stats)
    frozen = [path for path in all_paths if path.startswith(HEAVY_PATH_PREFIXES)]
    noisy = [path for path in all_paths if any(part in f"/{path}" for part in NOISY_PATH_PARTS)]

    over_files = len(all_paths) > args.max_files
    over_lines = changed_lines > args.max_changed_lines
    status = "PASS"
    if frozen or noisy or over_files or over_lines:
        status = "WARN"
    if args.enforce and status == "WARN":
        status = "FIX_REQUIRED"

    print(f"LGO_CHANGE_BUDGET_STATUS {status}")
    print(f"LGO_CHANGE_BUDGET_FILES {len(all_paths)} max={args.max_files}")
    print(f"LGO_CHANGE_BUDGET_CHANGED_LINES {changed_lines} max={args.max_changed_lines}")
    if over_files:
        print("LGO_CHANGE_BUDGET_WARN file_count_exceeds_budget")
    if over_lines:
        print("LGO_CHANGE_BUDGET_WARN changed_lines_exceed_budget")
    for path in frozen:
        print(f"LGO_CHANGE_BUDGET_WARN frozen_surface_changed {path}")
    for path in noisy:
        print(f"LGO_CHANGE_BUDGET_WARN noisy_generated_or_cache_path {path}")
    for item in sorted(stats, key=lambda stat: stat.changed, reverse=True)[:8]:
        print(f"LGO_CHANGE_BUDGET_TOP {item.changed} +{item.added} -{item.deleted} {item.path}")
    for path in untracked[:8]:
        print(f"LGO_CHANGE_BUDGET_UNTRACKED {path}")
    if not all_paths:
        print("LGO_CHANGE_BUDGET_CLEAN")

    return 1 if args.enforce and status == "FIX_REQUIRED" else 0


if __name__ == "__main__":
    raise SystemExit(main())
