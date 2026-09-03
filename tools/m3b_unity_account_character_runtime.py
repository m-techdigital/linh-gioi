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
        except Exception as exc:  # noqa: BLE001 - diagnostic only
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
    if raw:
        candidate = Path(raw).expanduser().resolve()
        if candidate.exists() and os.access(candidate, os.X_OK):
            return candidate
    env = os.environ.get('LGO_M3B_UNITY_PLAYER')
    if env:
        candidate = Path(env).expanduser().resolve()
        if candidate.exists() and os.access(candidate, os.X_OK):
            return candidate
    return None


def run_unity_smoke(root: Path, player: Path, base_url: str, out_json: Path, log: Path, expect_existing: bool) -> None:
    command = [
        str(player),
        '-batchmode',
        '-nographics',
        '--lgo-m3b-account-character-smoke',
        '--lgo-m3b-api-url', base_url,
        '--lgo-m3b-result', str(out_json),
        '--lgo-m3b-dev-key', 'm3b-unity-dev-key',
        '--lgo-m3b-character-name', 'M3BHero',
        '--lgo-m3b-class-id', 'class.sword',
    ]
    if expect_existing:
        command.append('--lgo-m3b-expect-existing')
    append(log, '+ ' + ' '.join(command))
    try:
        completed = subprocess.run(command, cwd=root, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    except OSError as exc:
        append(log, f'UNITY_SMOKE_EXEC_ERROR errno={getattr(exc, "errno", "unknown")} message={exc}')
        if getattr(exc, 'errno', None) == 8:
            append(log, 'UNITY_RUNTIME_GATE=UNVERIFIED_ENVIRONMENT')
            append(log, 'REASON=unity player executable format is incompatible with the current host OS/architecture')
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
    if expect_existing and not result.get('expectExisting'):
        append(log, 'UNITY_SMOKE_EXPECT_EXISTING_FLAG_MISSING')
        raise SystemExit(43)
    if expect_existing and not result.get('reusedExistingCharacter'):
        append(log, 'UNITY_SMOKE_DID_NOT_REUSE_PERSISTED_CHARACTER')
        raise SystemExit(44)
    append(log, 'UNITY_M3B_ACCOUNT_CHARACTER_SMOKE_PASS ' + json.dumps(result, sort_keys=True))


def run_api_pass(root: Path, base_url: str, port: str, store_dir: Path, api_log: Path, log: Path, player: Path, expect_existing: bool, out_json: Path) -> None:
    env = os.environ.copy()
    env['LG_API_HOST'] = '127.0.0.1'
    env['LG_API_PORT'] = port
    env['LG_API_PERSISTENCE_DIR'] = str(store_dir)
    append(log, f'+ start API for M3B Unity smoke expect_existing={str(expect_existing).lower()} port={port}')
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
    bundle = output_dir / f'lgo-m3b-unity-account-character-smoke-{timestamp}.tar.gz'
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
    upload = output_dir / 'UPLOAD-THESE-FILES-M3B-UNITY-ACCOUNT.txt'
    upload.write_text(f'Upload these files when closing M3-B Unity account/character integration:\n{log}\n{bundle}\n{bundle}.sha256\n', encoding='utf-8')
    return bundle


def main() -> int:
    parser = argparse.ArgumentParser(description='Run M3-B Unity account/character API persistence smoke against a real Spring Boot API process.')
    parser.add_argument('--root', required=True)
    parser.add_argument('--output-dir', required=True)
    parser.add_argument('--port', required=True)
    parser.add_argument('--unity-player')
    args = parser.parse_args()

    root = Path(args.root).resolve()
    output_dir = Path(args.output_dir).resolve()
    store_dir = output_dir / 'store'
    log = output_dir / 'm3b-unity-account-character-runtime.log'
    base_url = f'http://127.0.0.1:{args.port}'
    player = resolve_unity_player(args.unity_player)

    shutil.rmtree(store_dir, ignore_errors=True)
    output_dir.mkdir(parents=True, exist_ok=True)
    store_dir.mkdir(parents=True, exist_ok=True)
    log.write_text('', encoding='utf-8')

    append(log, '== Linh Gioi Online M3-B Unity account/character runtime smoke ==')
    append(log, f'ROOT={root}')
    append(log, f'OUT_DIR={output_dir}')
    append(log, f'STORE_DIR={store_dir}')
    append(log, f'BASE_URL={base_url}')

    if player is None:
        append(log, 'UNITY_RUNTIME_GATE=UNVERIFIED_ENVIRONMENT')
        append(log, 'REASON=no executable M3-B Unity Linux player was provided via --unity-player or LGO_M3B_UNITY_PLAYER')
        bundle_evidence(output_dir, log, store_dir)
        return 30

    if not (root / 'server/api/target/server-api-0.1.0-SNAPSHOT.jar').exists():
        append(log, 'ERROR: server/api/target/server-api-0.1.0-SNAPSHOT.jar missing. Run ./server/build.sh first.')
        return 4

    run_api_pass(root, base_url, args.port, store_dir, output_dir / 'api-first.log', log, player, False, output_dir / 'unity-first.json')
    run_api_pass(root, base_url, args.port, store_dir, output_dir / 'api-restart.log', log, player, True, output_dir / 'unity-restart.json')

    store_file = store_dir / 'players-v1.json'
    if not store_file.exists():
        append(log, f'ERROR: persistence file missing: {store_file}')
        return 45
    if 'm3b-unity-dev-key' in store_file.read_text(encoding='utf-8', errors='replace'):
        append(log, 'ERROR: raw M3B Unity dev key leaked into persistence file.')
        return 46

    bundle = bundle_evidence(output_dir, log, store_dir)
    append(log, f'M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS output={output_dir} bundle={bundle}')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
