#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import os
import subprocess
import tarfile
from datetime import datetime, timezone
from pathlib import Path


def append(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open('a', encoding='utf-8') as handle:
        handle.write(text + '\n')
    print(text, flush=True)


def resolve_player(raw: str | None) -> Path | None:
    for value in [raw, os.environ.get('LGO_M4_VISUAL_UNITY_PLAYER')]:
        if not value:
            continue
        path = Path(value).expanduser().resolve()
        if path.exists() and os.access(path, os.X_OK):
            return path
    return None


def bundle(output_dir: Path, log: Path) -> Path:
    timestamp = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%SZ')
    archive = output_dir / f'lgo-m4-visual-placeholder-foundation-smoke-{timestamp}.tar.gz'
    with tarfile.open(archive, 'w:gz') as tar:
        for path in sorted(output_dir.glob('*')):
            if path == archive or path.suffix == '.gz':
                continue
            if path.is_file():
                tar.add(path, arcname=path.name)
    digest = subprocess.check_output(['sha256sum', str(archive)], text=True)
    (archive.with_suffix(archive.suffix + '.sha256')).write_text(digest, encoding='utf-8')
    append(log, f'EVIDENCE_BUNDLE={archive}')
    return archive


def main() -> int:
    parser = argparse.ArgumentParser(description='Run M4 visual placeholder foundation smoke.')
    parser.add_argument('--root', required=True)
    parser.add_argument('--output-dir', required=True)
    parser.add_argument('--unity-player')
    args = parser.parse_args()

    root = Path(args.root).resolve()
    output_dir = Path(args.output_dir).resolve()
    output_dir.mkdir(parents=True, exist_ok=True)
    log = output_dir / 'm4-visual-placeholder-foundation-runtime.log'
    log.write_text('', encoding='utf-8')

    append(log, '== Linh Gioi Online M4 visual placeholder foundation smoke ==')
    append(log, f'ROOT={root}')
    append(log, f'OUT_DIR={output_dir}')

    validation = subprocess.run(['python3', 'tools/validate_m4_visual_foundation.py'], cwd=root, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    for line in validation.stdout.splitlines():
        append(log, line)
    if validation.returncode != 0:
        bundle(output_dir, log)
        return validation.returncode

    player = resolve_player(args.unity_player)
    if player is None:
        append(log, 'UNITY_RUNTIME_GATE=UNVERIFIED_ENVIRONMENT')
        append(log, 'REASON=no executable M4 visual Unity player was provided via --unity-player or LGO_M4_VISUAL_UNITY_PLAYER')
        bundle(output_dir, log)
        return 30

    result_json = output_dir / 'unity-visual-foundation.json'
    command = [
        str(player),
        '-batchmode',
        '-nographics',
        '--lgo-m4-visual-foundation-smoke',
        '--lgo-m4-visual-result',
        str(result_json),
    ]
    append(log, '+ ' + ' '.join(command))
    completed = subprocess.run(command, cwd=root, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    for line in completed.stdout.splitlines():
        append(log, line)
    append(log, f'UNITY_SMOKE_EXIT_CODE={completed.returncode}')
    if completed.returncode != 0:
        bundle(output_dir, log)
        return completed.returncode
    if not result_json.exists():
        append(log, f'UNITY_VISUAL_RESULT_MISSING path={result_json}')
        bundle(output_dir, log)
        return 41
    result = json.loads(result_json.read_text(encoding='utf-8'))
    if result.get('status') != 'PASS':
        append(log, 'UNITY_VISUAL_RESULT_FAIL ' + json.dumps(result, sort_keys=True))
        bundle(output_dir, log)
        return 42

    archive = bundle(output_dir, log)
    append(log, f'M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS output={output_dir} bundle={archive}')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
