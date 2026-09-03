#!/usr/bin/env python3
from __future__ import annotations

import argparse
import os
import shutil
import signal
import subprocess
import sys
import tarfile
import time
import urllib.request
from datetime import datetime, timezone
from pathlib import Path


def append(path: Path, text: str) -> None:
    with path.open('a', encoding='utf-8') as handle:
        handle.write(text + '\n')
    print(text, flush=True)


def wait_for_api(base_url: str, api_log: Path, log: Path, timeout_seconds: float = 40.0) -> None:
    deadline = time.monotonic() + timeout_seconds
    last_error = ''
    while time.monotonic() < deadline:
        try:
            with urllib.request.urlopen(base_url + '/health', timeout=2) as response:
                if response.status == 200:
                    return
        except Exception as exc:  # noqa: BLE001 - diagnostic only
            last_error = repr(exc)
        time.sleep(1)
    append(log, f'ERROR: API did not become ready: {last_error}')
    if api_log.exists():
        append(log, '--- API log tail ---')
        tail = api_log.read_text(encoding='utf-8', errors='replace').splitlines()[-160:]
        for line in tail:
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


def run_smoke(root: Path, base_url: str, expect_existing: bool, log: Path) -> None:
    command = [sys.executable, 'server/scripts/m3-persistence-smoke.py', '--base-url', base_url]
    if expect_existing:
        command.append('--expect-existing')
    append(log, '+ ' + ' '.join(command))
    completed = subprocess.run(command, cwd=root, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    if completed.stdout:
        for line in completed.stdout.splitlines():
            append(log, line)
    if completed.returncode != 0:
        raise SystemExit(completed.returncode)


def run_api_pass(root: Path, base_url: str, port: str, store_dir: Path, api_log: Path, log: Path, expect_existing: bool) -> None:
    env = os.environ.copy()
    env['LG_API_HOST'] = '127.0.0.1'
    env['LG_API_PORT'] = port
    env['LG_API_PERSISTENCE_DIR'] = str(store_dir)
    append(log, f'+ start API expect_existing={str(expect_existing).lower()} port={port}')
    with api_log.open('wb') as output:
        process = subprocess.Popen(['./server/run-api.sh'], cwd=root, env=env, stdout=output, stderr=subprocess.STDOUT)
    append(log, f'API_PID={process.pid}')
    try:
        wait_for_api(base_url, api_log, log)
        append(log, f'API_READY log={api_log}')
        run_smoke(root, base_url, expect_existing, log)
    finally:
        stop_process(process)


def main() -> int:
    parser = argparse.ArgumentParser(description='Run M3 API persistence runtime smoke with restart proof.')
    parser.add_argument('--root', required=True)
    parser.add_argument('--output-dir', required=True)
    parser.add_argument('--port', required=True)
    args = parser.parse_args()

    root = Path(args.root).resolve()
    output_dir = Path(args.output_dir).resolve()
    store_dir = output_dir / 'store'
    log = output_dir / 'm3-api-persistence-once.log'
    api_first_log = output_dir / 'api-first.log'
    api_restart_log = output_dir / 'api-restart.log'
    base_url = f'http://127.0.0.1:{args.port}'

    shutil.rmtree(store_dir, ignore_errors=True)
    store_dir.mkdir(parents=True, exist_ok=True)
    log.write_text('', encoding='utf-8')

    append(log, '== Linh Gioi Online M3 API persistence smoke ==')
    append(log, f'ROOT={root}')
    append(log, f'OUT_DIR={output_dir}')
    append(log, f'STORE_DIR={store_dir}')
    append(log, f'BASE_URL={base_url}')

    run_api_pass(root, base_url, args.port, store_dir, api_first_log, log, expect_existing=False)
    run_api_pass(root, base_url, args.port, store_dir, api_restart_log, log, expect_existing=True)

    if log.read_text(encoding='utf-8', errors='replace').count('M3_PERSISTENCE_SMOKE_PASS') != 2:
        append(log, 'ERROR: M3 smoke did not record both first-pass and restart-pass markers.')
        return 42
    store_file = store_dir / 'players-v1.json'
    if not store_file.exists():
        append(log, f'ERROR: persistence file missing: {store_file}')
        return 43
    if 'm3-smoke-dev-key' in store_file.read_text(encoding='utf-8', errors='replace'):
        append(log, 'ERROR: raw dev key leaked into persistence file.')
        return 44

    timestamp = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%SZ')
    bundle = output_dir / f'lgo-m3-api-persistence-smoke-{timestamp}.tar.gz'
    with tarfile.open(bundle, 'w:gz') as tar:
        for path in [log, output_dir / 'validate-m3-source.log', output_dir / 'require-java-25.log', output_dir / 'server-api-package.log', api_first_log, api_restart_log]:
            if path.exists():
                tar.add(path, arcname=path.name)
        tar.add(store_dir, arcname='store')
    digest = subprocess.check_output(['sha256sum', str(bundle)], text=True)
    (bundle.with_suffix(bundle.suffix + '.sha256')).write_text(digest, encoding='utf-8')

    upload_manifest = output_dir / 'UPLOAD-THESE-FILES-M3-PERSISTENCE.txt'
    upload_manifest.write_text(
        'Upload these files when closing M3 persistence runtime:\n'
        f'{log}\n{bundle}\n{bundle}.sha256\n',
        encoding='utf-8',
    )
    append(log, upload_manifest.read_text(encoding='utf-8').rstrip())
    append(log, f'M3_API_PERSISTENCE_RUNTIME_SMOKE_PASS output={output_dir}')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
