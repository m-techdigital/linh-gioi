#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import os
import shutil
import subprocess
import sys
import tarfile
import time
import urllib.request
from datetime import datetime, timezone
from pathlib import Path


def append(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open('a', encoding='utf-8') as handle:
        handle.write(text + '\n')
    print(text, flush=True)


def wait_for_api(base_url: str, api_log: Path, log: Path, timeout_seconds: float = 45.0) -> None:
    deadline = time.monotonic() + timeout_seconds
    last_error = ''
    while time.monotonic() < deadline:
        try:
            with urllib.request.urlopen(base_url + '/health', timeout=2) as response:
                body = response.read().decode('utf-8', errors='replace')
                if response.status == 200 and 'UP' in body:
                    append(log, f'API_HEALTH_PASS body={body}')
                    return
        except Exception as exc:
            last_error = repr(exc)
        time.sleep(1)
    append(log, f'API_HEALTH_FAIL last_error={last_error}')
    if api_log.exists():
        append(log, '--- API log tail ---')
        for line in api_log.read_text(encoding='utf-8', errors='replace').splitlines()[-160:]:
            append(log, line)
    raise SystemExit(40)


def stop_process(process: subprocess.Popen[bytes]) -> None:
    if process.poll() is not None:
        return
    try:
        process.terminate()
        process.wait(timeout=10)
    except subprocess.TimeoutExpired:
        process.kill()
        process.wait(timeout=10)


def resolve_unity_player(raw: str | None) -> Path | None:
    candidates = [raw, os.environ.get('LGO_M4_UNITY_PLAYER')]
    for candidate_raw in candidates:
        if not candidate_raw:
            continue
        candidate = Path(candidate_raw).expanduser().resolve()
        if candidate.exists() and os.access(candidate, os.X_OK):
            return candidate
    return None


def run_unity_smoke(root: Path, player: Path, base_url: str, out_json: Path, log: Path, expect_existing: bool) -> None:
    command = [
        str(player),
        '-batchmode',
        '-nographics',
        '--lgo-m4-playable-vertical-slice-smoke',
        '--lgo-m4-api-url', base_url,
        '--lgo-m4-result', str(out_json),
        '--lgo-m4-dev-key', 'm4-playable-dev-key',
        '--lgo-m4-character-name', 'M4VerticalHero',
        '--lgo-m4-class-id', 'class.sword',
    ]
    if expect_existing:
        command.append('--lgo-m4-expect-existing')
    append(log, '+ ' + ' '.join(command))
    try:
        completed = subprocess.run(command, cwd=root, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    except OSError as exc:
        append(log, f'UNITY_SMOKE_EXEC_ERROR errno={getattr(exc, "errno", "unknown")} message={exc}')
        if getattr(exc, 'errno', None) == 8:
            append(log, 'UNITY_RUNTIME_GATE=UNVERIFIED_ENVIRONMENT')
            append(log, 'REASON=unity player executable format is incompatible with current host OS/architecture')
            raise SystemExit(48) from exc
        raise
    if completed.stdout:
        for line in completed.stdout.splitlines():
            append(log, line)
    append(log, f'UNITY_SMOKE_EXIT_CODE={completed.returncode}')
    if completed.returncode != 0:
        raise SystemExit(completed.returncode)
    if not out_json.exists():
        append(log, f'UNITY_SMOKE_RESULT_MISSING path={out_json}')
        raise SystemExit(41)
    result = json.loads(out_json.read_text(encoding='utf-8'))
    if result.get('status') != 'PASS':
        append(log, 'UNITY_SMOKE_RESULT_FAIL ' + json.dumps(result, sort_keys=True))
        raise SystemExit(42)
    if expect_existing and not result.get('reusedExistingCharacter'):
        append(log, 'UNITY_SMOKE_DID_NOT_REUSE_PERSISTED_CHARACTER')
        raise SystemExit(44)
    append(log, 'UNITY_M4_PLAYABLE_VERTICAL_SLICE_PASS ' + json.dumps(result, sort_keys=True))


def run_api_pass(root: Path, base_url: str, port: str, store_dir: Path, api_log: Path, log: Path, player: Path, expect_existing: bool, out_json: Path) -> None:
    env = os.environ.copy()
    env['LG_API_HOST'] = '127.0.0.1'
    env['LG_API_PORT'] = port
    env['LG_API_PERSISTENCE_DIR'] = str(store_dir)
    append(log, f'+ start API for M4 playable smoke expect_existing={str(expect_existing).lower()} port={port}')
    with api_log.open('wb') as output:
        process = subprocess.Popen(['./server/run-api.sh'], cwd=root, env=env, stdout=output, stderr=subprocess.STDOUT)
    append(log, f'API_PID={process.pid}')
    try:
        wait_for_api(base_url, api_log, log)
        run_unity_smoke(root, player, base_url, out_json, log, expect_existing)
    finally:
        stop_process(process)


def bundle_evidence(output_dir: Path, log: Path, store_dir: Path) -> Path:
    timestamp = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%SZ')
    bundle = output_dir / f'lgo-m4-playable-vertical-slice-smoke-{timestamp}.tar.gz'
    with tarfile.open(bundle, 'w:gz') as tar:
        for path in sorted(output_dir.glob('*')):
            if path == bundle or path.suffix == '.gz':
                continue
            if path.exists() and path.is_file():
                tar.add(path, arcname=path.name)
        if store_dir.exists():
            tar.add(store_dir, arcname='store')
    digest = subprocess.check_output(['sha256sum', str(bundle)], text=True)
    (bundle.with_suffix(bundle.suffix + '.sha256')).write_text(digest, encoding='utf-8')
    return bundle


def main() -> int:
    parser = argparse.ArgumentParser(description='Run the M4 playable vertical slice Unity player smoke.')
    parser.add_argument('--root', required=True)
    parser.add_argument('--output-dir', required=True)
    parser.add_argument('--port', required=True)
    parser.add_argument('--unity-player')
    args = parser.parse_args()

    root = Path(args.root).resolve()
    output_dir = Path(args.output_dir).resolve()
    store_dir = output_dir / 'store'
    log = output_dir / 'm4-playable-vertical-slice-runtime.log'
    base_url = f'http://127.0.0.1:{args.port}'
    player = resolve_unity_player(args.unity_player)

    shutil.rmtree(store_dir, ignore_errors=True)
    output_dir.mkdir(parents=True, exist_ok=True)
    store_dir.mkdir(parents=True, exist_ok=True)
    log.write_text('', encoding='utf-8')

    append(log, '== Linh Gioi Online M4 playable vertical slice runtime smoke ==')
    append(log, f'ROOT={root}')
    append(log, f'OUT_DIR={output_dir}')
    append(log, f'STORE_DIR={store_dir}')
    append(log, f'BASE_URL={base_url}')

    if player is None:
        append(log, 'UNITY_RUNTIME_GATE=UNVERIFIED_ENVIRONMENT')
        append(log, 'REASON=no executable M4 Unity player was provided via --unity-player or LGO_M4_UNITY_PLAYER')
        bundle_evidence(output_dir, log, store_dir)
        return 30

    run_api_pass(root, base_url, args.port, store_dir, output_dir / 'api-first.log', log, player, False, output_dir / 'unity-first.json')
    run_api_pass(root, base_url, args.port, store_dir, output_dir / 'api-restart.log', log, player, True, output_dir / 'unity-restart.json')

    store_file = store_dir / 'players-v1.json'
    if not store_file.exists():
        append(log, f'ERROR: persistence file missing: {store_file}')
        return 45
    if 'm4-playable-dev-key' in store_file.read_text(encoding='utf-8', errors='replace'):
        append(log, 'ERROR: raw M4 dev key leaked into persistence file.')
        return 46

    bundle = bundle_evidence(output_dir, log, store_dir)
    append(log, f'M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS output={output_dir} bundle={bundle}')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
