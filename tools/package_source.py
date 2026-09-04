#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import os
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

EXCLUDED_DIR_NAMES = {'.git', 'build', 'Library', 'Temp', 'Logs', 'target', 'obj', '__pycache__', '__MACOSX'}
EXCLUDED_FILE_NAMES = {'.DS_Store'}
EXCLUDED_SUFFIXES = ('.zip', '.tar.gz', '.sha256')
EXCLUDED_PREFIXES = (
    'build/',
    'client/Unity/Library/',
    'client/Unity/Temp/',
    'client/Unity/Logs/',
    'client/Unity/Assets/Game/Generated/',
    'client/Unity/Assets/Game/Protocol/Generated/',
    '__MACOSX/',
)
EXCLUDED_EXACT = {
    'client/Unity/Assets/Game/Generated.meta',
    'client/Unity/Assets/Game/Protocol/Generated.meta',
    'M4-PLAYABLE-VISUAL-FOUNDATION-FINAL-REVIEW-REPORT-v0.10.1.md',
}


def is_excluded(rel: str) -> bool:
    path = Path(rel)
    if path.name in EXCLUDED_FILE_NAMES:
        return True
    if rel in EXCLUDED_EXACT:
        return True
    if rel.endswith(EXCLUDED_SUFFIXES):
        return True
    return any(rel.startswith(prefix) for prefix in EXCLUDED_PREFIXES)


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open('rb') as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b''):
            digest.update(chunk)
    return digest.hexdigest()


def write_delta(zip_path: Path, changed_files: Path) -> None:
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as archive:
        for raw in changed_files.read_text(encoding='utf-8').splitlines():
            rel = raw.strip()
            if not rel or is_excluded(rel) or rel.endswith('ARTIFACTS.sha256'):
                continue
            path = ROOT / rel
            if path.is_file():
                archive.write(path, rel)


def write_full(zip_path: Path) -> None:
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as archive:
        for dirpath, dirnames, filenames in os.walk(ROOT):
            current = Path(dirpath)
            rel_dir = current.relative_to(ROOT).as_posix()
            dirnames[:] = [
                name for name in dirnames
                if name not in EXCLUDED_DIR_NAMES and not is_excluded(((rel_dir + '/') if rel_dir != '.' else '') + name + '/')
            ]
            for filename in filenames:
                path = current / filename
                rel = path.relative_to(ROOT).as_posix()
                if is_excluded(rel) or rel == zip_path.name:
                    continue
                archive.write(path, rel)


def main() -> int:
    parser = argparse.ArgumentParser(description='Create Linh Gioi source handoff packages with standard hygiene exclusions.')
    parser.add_argument('--full-zip', required=True)
    parser.add_argument('--delta-zip', required=True)
    parser.add_argument('--changed-files', required=True)
    parser.add_argument('--sha-summary', required=True)
    args = parser.parse_args()

    full_zip = ROOT / args.full_zip
    delta_zip = ROOT / args.delta_zip
    changed_files = ROOT / args.changed_files
    sha_summary = ROOT / args.sha_summary

    for path in [full_zip, delta_zip, Path(str(full_zip) + '.sha256'), Path(str(delta_zip) + '.sha256'), sha_summary]:
        if path.exists():
            path.unlink()

    write_full(full_zip)
    write_delta(delta_zip, changed_files)

    lines = []
    for path in [full_zip, delta_zip]:
        digest = sha256(path)
        Path(str(path) + '.sha256').write_text(f'{digest}  {path.name}\n', encoding='utf-8')
        lines.append(f'{digest}  {path.name}')
    sha_summary.write_text('\n'.join(lines) + '\n', encoding='utf-8')
    print('\n'.join(lines))
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
