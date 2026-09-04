#!/usr/bin/env python3
from __future__ import annotations

import argparse
import collections
import json
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_OUT = ROOT / "build" / "lgo-worktree-audit"

CATEGORIES = (
    ("frozen_protocol", ("protocol/",)),
    ("frozen_gamedata_schema", ("gamedata/schemas/",)),
    ("frozen_adr", ("docs/adr/",)),
    ("frozen_design_tokens", ("client/Unity/Assets/Game/UI/design-tokens.json",)),
    ("unity_runtime_art", ("client/Unity/Assets/Game/Art/Runtime/",)),
    ("unity_packages", ("client/Unity/Assets/Packages/", "client/Unity/Assets/packages.config", "client/Unity/Assets/NuGet.config")),
    ("unity_ui", ("client/Unity/Assets/Game/UI/",)),
    ("unity_world", ("client/Unity/Assets/Game/World/",)),
    ("reference_art", ("docs/reference-art/",)),
    ("art_docs", ("docs/art/",)),
    ("execution_docs", ("docs/execution/",)),
    ("task_docs", ("docs/tasks/",)),
    ("handoff_report", ("HANDOFF-", "LGO-", "M6-")),
    ("tools", ("tools/",)),
)


def git_status() -> list[str]:
    result = subprocess.run(
        ["git", "--no-pager", "status", "--short", "--untracked-files=all"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        raise RuntimeError(result.stderr.strip() or "git status failed")
    return [line for line in result.stdout.splitlines() if line.strip()]


def category_for(path: str) -> str:
    for name, prefixes in CATEGORIES:
        if any(path == prefix.rstrip("/") or path.startswith(prefix) for prefix in prefixes):
            return name
    return "other"


def parse_status_line(line: str) -> tuple[str, str]:
    status = line[:2].strip() or "??"
    path = line[3:] if len(line) >= 4 else line
    if " -> " in path:
        path = path.split(" -> ", 1)[1]
    return status, path


def write_outputs(out_dir: Path) -> None:
    out_dir.mkdir(parents=True, exist_ok=True)
    rows = []
    counts: collections.Counter[str] = collections.Counter()
    frozen = []
    for line in git_status():
        status, path = parse_status_line(line)
        category = category_for(path)
        counts[category] += 1
        item = {"status": status, "path": path, "category": category}
        rows.append(item)
        if category.startswith("frozen_"):
            frozen.append(item)

    (out_dir / "worktree-audit.json").write_text(json.dumps({"counts": counts, "files": rows}, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    (out_dir / "worktree-audit.csv").write_text(
        "status,category,path\n" + "".join(f"{row['status']},{row['category']},{row['path']}\n" for row in rows),
        encoding="utf-8",
    )
    lines = ["# LGO Worktree Audit", "", "Marker: `LGO_WORKTREE_AUDIT_READY`", "", "## Counts", ""]
    for category, count in sorted(counts.items()):
        lines.append(f"- `{category}`: {count}")
    lines.extend(["", "## Frozen Surface Status", ""])
    if frozen:
        lines.extend(f"- `{row['status']}` `{row['path']}`" for row in frozen)
    else:
        lines.append("- No frozen surface changes detected in git status.")
    lines.extend(["", "## Commit Guidance", ""])
    if len(rows) > 250:
        lines.append("- Commit automation should remain deferred until this audit is reviewed or the worktree is intentionally staged by category.")
    else:
        lines.append("- Worktree size is within the automatic commit threshold.")
    (out_dir / "worktree-audit.md").write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"LGO_WORKTREE_AUDIT_READY files={len(rows)} out={out_dir}")


def main() -> int:
    parser = argparse.ArgumentParser(description="Categorize Linh Gioi git status for safe continuous-cycle handoff/commit decisions.")
    parser.add_argument("--out-dir", default=str(DEFAULT_OUT))
    args = parser.parse_args()
    write_outputs(Path(args.out_dir))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
