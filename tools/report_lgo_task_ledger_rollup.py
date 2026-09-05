#!/usr/bin/env python3
from __future__ import annotations

import argparse
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
LEDGER = ROOT / "docs/execution/TASK-LEDGER.md"
NEXT_ACTION = ROOT / "docs/execution/NEXT-ACTION.md"
OUTPUT = ROOT / "docs/execution/TASK-LEDGER-ROLLUP.md"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8", errors="replace")


def extract_next_task(text: str) -> str:
    match = re.search(r"## Next task\s+\n\s*`([^`]+)`", text)
    return match.group(1) if match else "UNKNOWN"


def extract_current_phase(text: str) -> str:
    match = re.search(r"- Current phase: ([^\n]+)", text)
    return match.group(1).strip() if match else "See NEXT-ACTION.md"


def ledger_rows(text: str) -> list[str]:
    return [
        line
        for line in text.splitlines()
        if line.startswith("| ")
        and not line.startswith("|---")
        and "Task ID" not in line
    ]


def task_id(row: str) -> str:
    cells = [cell.strip() for cell in row.strip("|").split("|")]
    return cells[0] if cells else row


def render(limit: int) -> str:
    next_text = read(NEXT_ACTION)
    rows = ledger_rows(read(LEDGER))
    recent = rows[-limit:] if limit > 0 else rows
    lines = [
        "# Linh Giới Online — Task Ledger Rollup",
        "",
        "Marker: `LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY`",
        "",
        "## Quick State",
        "",
        f"- Current phase: {extract_current_phase(next_text)}",
        f"- Next task: `{extract_next_task(next_text)}`",
        "- Source of truth: `docs/execution/TASK-LEDGER.md` remains append-only.",
        "- Purpose: scan recent work quickly without deleting historical task rows or marker coverage.",
        "",
        "## Recent Tasks",
        "",
        "| Recent | Task ID | Status / Decision | Next allowed step |",
        "|---:|---|---|---|",
    ]
    start_index = max(len(rows) - len(recent) + 1, 1)
    for index, row in enumerate(recent, start=start_index):
        cells = [cell.strip() for cell in row.strip("|").split("|")]
        ident = cells[0] if len(cells) > 0 else "UNKNOWN"
        status = cells[2] if len(cells) > 2 else "UNKNOWN"
        decision = cells[9] if len(cells) > 9 else ""
        next_step = cells[10] if len(cells) > 10 else ""
        decision_text = status
        if decision:
            decision_text = f"{status}; {decision}"
        lines.append(f"| {index} | {ident} | {decision_text} | {next_step} |")
    lines.extend(
        [
            "",
            "## Operating Notes",
            "",
            "- Do not edit this rollup as the canonical task history; update `TASK-LEDGER.md` first.",
            "- Regenerate this file with `python3.12 tools/report_lgo_task_ledger_rollup.py` after a coherent batch closes.",
            "- Keep historical markers in `NEXT-ACTION.md` until a dedicated registry migration is validated.",
            "- This report does not claim runtime or visual PASS.",
            "",
        ]
    )
    return "\n".join(lines)


def main() -> int:
    parser = argparse.ArgumentParser(description="Write a compact rollup of the latest LGO task ledger rows.")
    parser.add_argument("--limit", type=int, default=18)
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    if not LEDGER.is_file() or not NEXT_ACTION.is_file():
        print("LGO_TASK_LEDGER_ROLLUP_FAILED missing ledger or next action")
        return 1
    text = render(args.limit)
    if args.check:
        current = OUTPUT.read_text(encoding="utf-8", errors="replace") if OUTPUT.exists() else ""
        if current != text:
            print("LGO_TASK_LEDGER_ROLLUP_STALE")
            return 1
        print("LGO_TASK_LEDGER_ROLLUP_CHECK_PASS")
        return 0
    OUTPUT.write_text(text, encoding="utf-8")
    print(f"LGO_TASK_LEDGER_ROLLUP_WRITTEN {OUTPUT.relative_to(ROOT)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
