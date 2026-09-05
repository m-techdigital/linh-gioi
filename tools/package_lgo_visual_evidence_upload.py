#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import json
import subprocess
import sys
import zipfile
from datetime import datetime, timezone
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
PROFILE_ROOT = ROOT / "build/visual-evidence/profiles"
OUT_DIR = ROOT / "build/chatgpt-handoff"
PROFILES = ("desktop", "tablet", "mobile")
SCREENSHOTS = (
    "login.png",
    "character-lobby.png",
    "character-select.png",
    "enter-world.png",
    "world-hub.png",
    "near-gatekeeper-prompt.png",
    "near-training-stone-prompt.png",
    "npc-dialogue.png",
    "session-menu.png",
    "target-dummy-state.png",
)
TEXT_EVIDENCE = (
    "visual-runtime-evidence-manifest.json",
    "visual-runtime-evidence-heuristics.json",
    "visual-runtime-evidence-heuristics.md",
    "visual-runtime-evidence-review.md",
)
LOG_EVIDENCE = (
    "api.log",
    "player.log",
    "player-unity.log",
    "unity-build.log",
)


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def git_head() -> str:
    result = subprocess.run(
        ["git", "--no-pager", "rev-parse", "--short", "HEAD"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        raise RuntimeError(result.stderr.strip() or "git rev-parse failed")
    return result.stdout.strip()


def require(path: Path, errors: list[str]) -> None:
    if not path.is_file():
        errors.append(str(path.relative_to(ROOT)))
    elif path.stat().st_size <= 0:
        errors.append(str(path.relative_to(ROOT)) + " is empty")


def collect_files(include_logs: bool) -> tuple[list[Path], list[str]]:
    errors: list[str] = []
    files: list[Path] = []
    for rel in ("index.md", "index.json", "profile-review.log"):
        path = PROFILE_ROOT / rel
        require(path, errors)
        files.append(path)
    for profile in PROFILES:
        profile_dir = PROFILE_ROOT / profile
        for name in SCREENSHOTS:
            path = profile_dir / name
            require(path, errors)
            files.append(path)
        for name in TEXT_EVIDENCE:
            path = profile_dir / name
            require(path, errors)
            files.append(path)
        if include_logs:
            for name in LOG_EVIDENCE:
                path = profile_dir / name
                if path.is_file() and path.stat().st_size > 0:
                    files.append(path)
    return sorted(set(files)), errors


def archive_name(path: Path) -> str:
    return str(path.relative_to(ROOT))


def write_readme(zip_file: zipfile.ZipFile, *, head: str, include_logs: bool, file_count: int) -> None:
    readme = f"""# Linh Gioi Online Visual Evidence Upload

Marker: `LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY`

Generated: `{datetime.now(timezone.utc).isoformat()}`
Git head: `{head}`

## Contents

- Desktop/tablet/mobile runtime screenshots for Login, Character Hall, Enter World, World Hub, near-object prompts, NPC Dialogue, Session Menu, and target dummy state.
- Runtime evidence manifests, heuristics, review notes, and profile index.
- Logs included: `{str(include_logs).lower()}`
- File count: `{file_count}`

## Non-Claims

- This package is evidence for human/AI review.
- It does not claim `VISUAL_RUNTIME_PASS`.
- It does not contain Unity `Library`, `Temp`, cache folders, build player binaries, or PID files.
"""
    zip_file.writestr("LGO-VISUAL-EVIDENCE-UPLOAD-README.md", readme)


def write_package(files: list[Path], *, include_logs: bool) -> tuple[Path, Path, Path]:
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    head = git_head()
    stamp = datetime.now(timezone.utc).strftime("%Y%m%d-%H%M%S")
    zip_path = OUT_DIR / f"LGO-VISUAL-EVIDENCE-UPLOAD-current-{head}-{stamp}.zip"
    manifest_path = OUT_DIR / f"LGO-VISUAL-EVIDENCE-UPLOAD-current-{head}-{stamp}.manifest.json"
    sha_path = OUT_DIR / f"LGO-VISUAL-EVIDENCE-UPLOAD-current-{head}-{stamp}.sha256"

    manifest = {
        "marker": "LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY",
        "generated_utc": datetime.now(timezone.utc).isoformat(),
        "git_head": head,
        "include_logs": include_logs,
        "profiles": list(PROFILES),
        "screenshots": list(SCREENSHOTS),
        "files": [archive_name(path) for path in files],
        "non_claims": [
            "No VISUAL_RUNTIME_PASS claim",
            "No Unity Library/Temp/cache/player binary included",
            "No protocol/GameData/ADR/design-token change",
        ],
    }

    with zipfile.ZipFile(zip_path, "w", zipfile.ZIP_DEFLATED) as archive:
        write_readme(archive, head=head, include_logs=include_logs, file_count=len(files))
        for path in files:
            archive.write(path, archive_name(path))
        archive.writestr("LGO-VISUAL-EVIDENCE-UPLOAD-MANIFEST.json", json.dumps(manifest, ensure_ascii=False, indent=2) + "\n")

    digest = sha256(zip_path)
    manifest["zip"] = str(zip_path.relative_to(ROOT))
    manifest["sha256"] = digest
    manifest_path.write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    sha_path.write_text(f"{digest}  {zip_path.name}\n", encoding="utf-8")
    return zip_path, manifest_path, sha_path


def main() -> int:
    parser = argparse.ArgumentParser(description="Package LGO visual runtime evidence for lightweight upload/review.")
    parser.add_argument("--include-logs", action="store_true", help="Include API/player/Unity logs in the upload ZIP.")
    parser.add_argument("--verify-only", action="store_true", help="Validate evidence presence without writing a ZIP.")
    args = parser.parse_args()

    files, errors = collect_files(include_logs=args.include_logs)
    if errors:
        print("LGO VISUAL EVIDENCE UPLOAD PACKAGE FAILED", file=sys.stderr)
        for error in errors:
            print(" - missing evidence: " + error, file=sys.stderr)
        return 1
    if args.verify_only:
        print(f"LGO_VISUAL_EVIDENCE_UPLOAD_VERIFY_PASS files={len(files)}")
        return 0

    zip_path, manifest_path, sha_path = write_package(files, include_logs=args.include_logs)
    print("LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY")
    print(f"zip={zip_path.relative_to(ROOT)}")
    print(f"manifest={manifest_path.relative_to(ROOT)}")
    print(f"sha256={sha_path.relative_to(ROOT)}")
    print(f"bytes={zip_path.stat().st_size}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
