#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

FORBIDDEN_PREFIXES = (
    ".git/",
    "build/",
    "client/Unity/Library/",
    "client/Unity/Temp/",
    "client/Unity/Logs/",
    "client/Unity/Assets/Game/Generated/",
    "client/Unity/Assets/Game/Protocol/Generated/",
    "__MACOSX/",
)

FORBIDDEN_DIFF_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
REQUIRED_ZIPS = (
    "linh-gioi-m6-server-authoritative-combat-foundation-v0.44.1-full-source.zip",
    "linh-gioi-m6-server-authoritative-combat-foundation-v0.44.1-delta.zip",
)


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


def source_artifact_forbidden(path: str) -> bool:
    return (
        "/__pycache__/" in path
        or path.startswith("__pycache__/")
        or path.endswith(".pyc")
        or path.endswith("/.DS_Store")
        or path == ".DS_Store"
        or path.startswith("__MACOSX/")
    )


def check_zip(path: Path) -> None:
    if not path.is_file():
        errors.append(f"missing package: {path.name}")
        return
    with zipfile.ZipFile(path) as archive:
        names = archive.namelist()
    if not names:
        errors.append(f"empty package: {path.name}")
    for name in names:
        if source_artifact_forbidden(name):
            errors.append(f"{path.name} contains forbidden cache/source artifact: {name}")
        for prefix in FORBIDDEN_PREFIXES:
            if name == prefix.rstrip("/") or name.startswith(prefix):
                errors.append(f"{path.name} contains forbidden generated/build entry: {name}")
        if name.endswith(".zip") or name.endswith(".sha256"):
            errors.append(f"{path.name} contains packaged artifact output: {name}")


def require_file(path: str, *markers: str) -> None:
    target = ROOT / path
    if not target.is_file():
        errors.append(f"missing file: {path}")
        return
    content = target.read_text(encoding="utf-8", errors="replace")
    for marker in markers:
        if marker not in content:
            errors.append(f"{path} missing marker: {marker}")


def main() -> int:
    require_file("tools/package_source.py", "__pycache__", ".pyc", "client/Unity/Library/", "client/Unity/Assets/Game/Protocol/Generated/")
    require_file("tools/validate_package_hygiene.py", "__pycache__", ".pyc", "PACKAGE HYGIENE VALIDATION PASS")
    require_file("LGO-M6-PACKAGE-HYGIENE-HOTFIX-v0.44.1-DELETIONS.txt", "DELETED")
    require_file("LGO-M6-PACKAGE-HYGIENE-HOTFIX-v0.44.1-ARTIFACTS-SHA256.txt", "v0.44.1-full-source.zip", "v0.44.1-delta.zip")

    deletions = ROOT / "LGO-M6-PACKAGE-HYGIENE-HOTFIX-v0.44.1-DELETIONS.txt"
    if deletions.is_file() and deletions.read_text(encoding="utf-8", errors="replace").splitlines()[0:1] != ["DELETED"]:
        errors.append("deletion manifest must start with DELETED")

    for line in git_lines("status", "--short", "--untracked-files=all"):
        status = line[:2]
        if "D" in status:
            continue
        path = line[3:] if len(line) >= 4 else line
        if source_artifact_forbidden(path):
            errors.append(f"forbidden cache/source artifact present: {path}")

    for path in git_lines("diff", "--name-only"):
        if path == "client/Unity/Assets/Game/UI/design-tokens.json":
            errors.append(f"frozen surface modified: {path}")
        for prefix in FORBIDDEN_DIFF_PREFIXES:
            if path.startswith(prefix):
                errors.append(f"frozen surface modified: {path}")

    for name in REQUIRED_ZIPS:
        check_zip(ROOT / name)
        sha = ROOT / f"{name}.sha256"
        if not sha.is_file():
            errors.append(f"missing package sha: {sha.name}")
        elif name not in sha.read_text(encoding="utf-8", errors="replace"):
            errors.append(f"{sha.name} does not reference {name}")

    if errors:
        print("M6 PACKAGE HYGIENE HOTFIX VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6_PACKAGE_HYGIENE_HOTFIX_PASS_v0.44.1")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
