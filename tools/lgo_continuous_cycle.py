#!/usr/bin/env python3
from __future__ import annotations

import argparse
import datetime as dt
import os
import shutil
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
HANDOFF_ROOT = Path("/Users/minhdc/Projects/LGO-Handoffs")
FROZEN_PATHS = (
    "protocol",
    "gamedata/schemas",
    "docs/adr",
    "client/Unity/Assets/Game/UI/design-tokens.json",
)


def run(command: list[str], *, check: bool = True) -> subprocess.CompletedProcess[str]:
    print("$ " + " ".join(command))
    result = subprocess.run(command, cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    if result.stdout:
        print(result.stdout, end="" if result.stdout.endswith("\n") else "\n")
    if check and result.returncode != 0:
        raise RuntimeError(f"command failed with exit {result.returncode}: {' '.join(command)}")
    return result


def clean_python_cache() -> None:
    for base in ("server", "tests", "tools"):
        root = ROOT / base
        if not root.exists():
            continue
        for path in root.rglob("__pycache__"):
            shutil.rmtree(path, ignore_errors=True)
        for path in root.rglob("*.pyc"):
            path.unlink(missing_ok=True)


def ensure_repo_root() -> None:
    if ROOT.name != "LinhGioiOnline":
        raise RuntimeError(f"expected repo root LinhGioiOnline, got {ROOT}")


def check_frozen_surfaces() -> None:
    result = run(["git", "--no-pager", "diff", "--name-only", "--", *FROZEN_PATHS], check=True)
    changed = [line.strip() for line in result.stdout.splitlines() if line.strip()]
    if changed:
        raise RuntimeError("frozen surfaces changed:\n" + "\n".join(changed))


def source_gates() -> None:
    run(["git", "--no-pager", "diff", "--check"])
    run(["python3.12", "tools/validate_package_hygiene.py"])
    run(["python3.12", "tools/validate_m6_combat_hardening_continuation.py"])
    run(["python3.12", "tools/validate_lgo_art_v3b_candidates.py"])
    run(["./tools/lgo_playable_closure_check.sh", "--source-only"])
    run(["./tools/lgo_playable_closure_check.sh", "--package-ready"])


def runtime_gates() -> None:
    run(["./tools/lgo_playable_closure_check.sh", "--runtime"])


def write_report(phase: str, status: str, commit_status: str) -> Path:
    HANDOFF_ROOT.mkdir(parents=True, exist_ok=True)
    stamp = dt.datetime.now().strftime("%Y%m%d-%H%M%S")
    report = HANDOFF_ROOT / f"lgo-continuous-cycle-{phase}-{stamp}.md"
    status_text = run(["git", "--no-pager", "status", "--short", "--untracked-files=all"], check=True).stdout
    report.write_text(
        "\n".join(
            [
                f"# LGO Continuous Cycle Report {stamp}",
                "",
                f"Phase: `{phase}`",
                f"Status: `{status}`",
                f"Commit status: `{commit_status}`",
                "",
                "## Boundaries",
                "",
                "- No protocol/schema/ADR/design-token changes are allowed by this cycle.",
                "- Image work is deferred unless a runtime-size-budgeted asset task is active.",
                "- V1 reference/mockup, V2 structural placeholder, and V3B runtime candidate classifications remain in force.",
                "",
                "## Git Status",
                "",
                "```text",
                status_text.rstrip(),
                "```",
                "",
            ]
        ),
        encoding="utf-8",
    )
    print(f"REPORT {report}")
    return report


def audit_worktree_if_large() -> None:
    status = run(["git", "--no-pager", "status", "--short", "--untracked-files=all"], check=True).stdout
    if len([line for line in status.splitlines() if line.strip()]) > 250:
        run(["python3.12", "tools/lgo_worktree_audit.py"], check=True)


def maybe_commit_and_push(message: str, push: bool) -> str:
    status = run(["git", "--no-pager", "status", "--short", "--untracked-files=all"], check=True).stdout
    if not status.strip():
        return "NO_CHANGES"
    if len(status.splitlines()) > 250:
        run(["python3.12", "tools/lgo_worktree_audit.py"], check=True)
        return "DEFERRED_DIRTY_WORKTREE_REVIEW_REQUIRED"
    run(["git", "add", "-A"])
    run(["git", "commit", "-m", message])
    if push:
        remotes = run(["git", "remote"], check=True).stdout.split()
        if not remotes:
            return "COMMITTED_PUSH_SKIPPED_NO_REMOTE"
        run(["git", "push"])
        return "COMMITTED_AND_PUSHED"
    return "COMMITTED_PUSH_NOT_REQUESTED"


def main() -> int:
    parser = argparse.ArgumentParser(description="Run a governed Linh Gioi continuous development cycle.")
    parser.add_argument("--phase", choices=("source", "runtime", "full"), default="source")
    parser.add_argument("--commit", action="store_true")
    parser.add_argument("--push", action="store_true")
    parser.add_argument("--message", default="chore: continuous development cycle")
    args = parser.parse_args()

    status = "PASS"
    commit_status = "NOT_REQUESTED"
    try:
        ensure_repo_root()
        clean_python_cache()
        check_frozen_surfaces()
        if args.phase in ("source", "full"):
            source_gates()
        if args.phase in ("runtime", "full"):
            runtime_gates()
        clean_python_cache()
        run(["python3.12", "tools/validate_package_hygiene.py"])
        check_frozen_surfaces()
        audit_worktree_if_large()
        if args.commit or args.push:
            commit_status = maybe_commit_and_push(args.message, args.push)
    except Exception as exc:
        status = "FAIL"
        print(f"ERROR {exc}", file=sys.stderr)
        write_report(args.phase, status, commit_status)
        return 1
    write_report(args.phase, status, commit_status)
    print(f"LGO_CONTINUOUS_CYCLE_{args.phase.upper()}_{status}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
