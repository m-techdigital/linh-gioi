#!/usr/bin/env python3
from __future__ import annotations

from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

SECTIONS = [
    ("repo_total", "."),
    ("git_history", ".git"),
    ("unity_client_source", "client/Unity"),
    ("unity_assets", "client/Unity/Assets"),
    ("runtime_art", "client/Unity/Assets/Game/Art/Runtime"),
    ("runtime_art_v3b", "client/Unity/Assets/Game/Art/Runtime/V3B"),
    ("reference_art", "docs/reference-art"),
    ("server_build_outputs", "server"),
    ("protobuf_toolchain", "tools/protobuf"),
    ("build_evidence", "build"),
]

IGNORED_DIRS = {
    ".git",
    "Library",
    "Temp",
    "Logs",
    "obj",
    "__pycache__",
}

RUNTIME_EXTENSIONS = {".png", ".jpg", ".jpeg", ".cs", ".uss", ".uxml", ".json", ".asmdef", ".dll"}


def tree_size(path: Path, *, ignore_cache: bool = False) -> int:
    if not path.exists():
        return 0
    if path.is_file():
        return path.stat().st_size
    total = 0
    for child in path.rglob("*"):
        if ignore_cache and any(part in IGNORED_DIRS for part in child.relative_to(path).parts):
            continue
        if child.is_file():
            total += child.stat().st_size
    return total


def format_size(size: int) -> str:
    value = float(size)
    for unit in ("B", "KB", "MB", "GB"):
        if value < 1024 or unit == "GB":
            return f"{value:.1f} {unit}" if unit != "B" else f"{int(value)} B"
        value /= 1024
    return f"{value:.1f} GB"


def large_runtime_files(limit: int = 256 * 1024) -> list[Path]:
    assets = ROOT / "client/Unity/Assets"
    if not assets.exists():
        return []
    files = [
        path
        for path in assets.rglob("*")
        if path.is_file() and path.suffix.lower() in RUNTIME_EXTENSIONS and path.stat().st_size >= limit
    ]
    return sorted(files, key=lambda item: item.stat().st_size, reverse=True)


def main() -> int:
    print("# LGO Build Size Budget Report")
    print()
    print("Marker: `LGO_BUILD_SIZE_BUDGET_REPORT_READY`")
    print()
    print("This report separates runtime payload candidates from repository-only review/reference weight.")
    print()
    print("| Section | Path | Size | Runtime payload? |")
    print("|---|---|---:|---|")
    for key, rel in SECTIONS:
        path = ROOT / rel
        size = tree_size(path, ignore_cache=rel != ".")
        runtime = "yes" if key in {"unity_client_source", "unity_assets", "runtime_art", "runtime_art_v3b"} else "no"
        print(f"| `{key}` | `{rel}` | {format_size(size)} | {runtime} |")
    print()
    print("## Runtime Files Over 256KB")
    print()
    files = large_runtime_files()
    if not files:
        print("- none")
    else:
        for path in files:
            rel = path.relative_to(ROOT)
            print(f"- `{rel}`: {format_size(path.stat().st_size)}")
    print()
    print("## Notes")
    print()
    print("- `.git`, `docs/reference-art`, server target jars, and protobuf toolchains are repository/tooling weight, not direct runtime art payload.")
    print("- Mobile builds must rely on Unity platform import profiles and role budgets instead of importing oversized reference boards.")
    print("- V1/V2/V3 reference/mockup/composite sheets remain non-runtime unless explicitly separated and approved.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
