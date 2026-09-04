#!/usr/bin/env python3
from __future__ import annotations

import csv
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"


def main() -> int:
    if not MANIFEST.is_file():
        print("missing V3B runtime manifest")
        return 1

    with MANIFEST.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))

    print("| Role | Runtime Asset | Dimensions | File Size | Classification |")
    print("|---|---:|---:|---:|---|")
    for row in sorted(rows, key=lambda item: (ROOT / item["unity_path"]).stat().st_size if (ROOT / item["unity_path"]).is_file() else 0, reverse=True):
        path = ROOT / row["unity_path"]
        size_kb = path.stat().st_size / 1024 if path.is_file() else 0
        dims = f"{row.get('runtime_width', '?')}x{row.get('runtime_height', '?')}"
        print(f"| `{row['role']}` | `{row['unity_path']}` | {dims} | {size_kb:.1f} KB | `{row.get('classification', '')}` |")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
