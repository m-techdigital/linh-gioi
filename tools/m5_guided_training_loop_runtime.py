#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import os
import shutil
import subprocess
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


def resolve_player(raw: str | None) -> Path | None:
    for value in [raw, os.environ.get('LGO_M5_GUIDED_UNITY_PLAYER'), os.environ.get('LGO_M5_UNITY_PLAYER')]:
        if not value:
            continue
        path = Path(value).expanduser().resolve()
        if path.exists() and os.access(path, os.X_OK):
            return path
    return None


def bundle(output_dir: Path, log: Path, store_dir: Path) -> Path:
    timestamp = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%SZ')
    archive = output_dir / f'lgo-m5-guided-training-loop-smoke-{timestamp}.tar.gz'
    with tarfile.open(archive, 'w:gz') as tar:
        for path in sorted(output_dir.glob('*')):
            if path == archive or path.suffix == '.gz':
                continue
            if path.is_file():
                tar.add(path, arcname=path.name)
        if store_dir.exists():
            tar.add(store_dir, arcname='store')
    digest = subprocess.check_output(['sha256sum', str(archive)], text=True)
    (archive.with_suffix(archive.suffix + '.sha256')).write_text(digest, encoding='utf-8')
    append(log, f'EVIDENCE_BUNDLE={archive}')
    return archive


def run_unity_smoke(root: Path, player: Path, base_url: str, result_json: Path, log: Path) -> None:
    command = [
        str(player),
        '-batchmode',
        '-nographics',
        '--lgo-m5-guided-training-loop-smoke',
        '--lgo-m5-guided-api-url', base_url,
        '--lgo-m5-guided-result', str(result_json),
        '--lgo-m5-guided-dev-key', 'm5-guided-training-loop-dev-key',
        '--lgo-m5-guided-character-name', 'M5GuidedHero',
        '--lgo-m5-guided-class-id', 'class.sword',
    ]
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
    for line in completed.stdout.splitlines():
        append(log, line)
    append(log, f'UNITY_SMOKE_EXIT_CODE={completed.returncode}')
    if completed.returncode != 0:
        raise SystemExit(completed.returncode)
    if not result_json.exists():
        append(log, f'UNITY_M5_GUIDED_RESULT_MISSING path={result_json}')
        raise SystemExit(41)
    result = json.loads(result_json.read_text(encoding='utf-8'))
    required = {
        'status': 'PASS',
        'enteredWorld': True,
        'gateKeeperInteractionTriggered': True,
        'trainingStoneInteractionTriggered': True,
        'savePositionStillWorks': True,
    }
    for key, value in required.items():
        if result.get(key) != value:
            append(log, 'UNITY_M5_GUIDED_RESULT_FAIL ' + json.dumps(result, sort_keys=True))
            raise SystemExit(42)
    if 'Gate Keeper' not in result.get('initialObjective', ''):
        append(log, 'UNITY_M5_GUIDED_INITIAL_OBJECTIVE_FAIL ' + json.dumps(result, sort_keys=True))
        raise SystemExit(43)
    if 'Training Stone' not in result.get('afterGateKeeperObjective', ''):
        append(log, 'UNITY_M5_GUIDED_ADVANCE_FAIL ' + json.dumps(result, sort_keys=True))
        raise SystemExit(44)
    if 'Objective complete' not in result.get('finalObjective', ''):
        append(log, 'UNITY_M5_GUIDED_OBJECTIVE_NOT_COMPLETE ' + json.dumps(result, sort_keys=True))
        raise SystemExit(45)
    append(log, 'UNITY_M5_GUIDED_TRAINING_LOOP_PASS ' + json.dumps(result, sort_keys=True))


def main() -> int:
    parser = argparse.ArgumentParser(description='Run the M5 guided training loop Unity player smoke.')
    parser.add_argument('--root', required=True)
    parser.add_argument('--output-dir', required=True)
    parser.add_argument('--port', required=True)
    parser.add_argument('--unity-player')
    args = parser.parse_args()

    root = Path(args.root).resolve()
    output_dir = Path(args.output_dir).resolve()
    store_dir = output_dir / 'store'
    log = output_dir / 'm5-guided-training-loop-runtime.log'
    api_log = output_dir / 'api.log'
    base_url = f'http://127.0.0.1:{args.port}'
    player = resolve_player(args.unity_player)

    shutil.rmtree(store_dir, ignore_errors=True)
    output_dir.mkdir(parents=True, exist_ok=True)
    store_dir.mkdir(parents=True, exist_ok=True)
    log.write_text('', encoding='utf-8')

    append(log, '== Linh Gioi Online M5 guided training loop runtime smoke ==')
    append(log, f'ROOT={root}')
    append(log, f'OUT_DIR={output_dir}')
    append(log, f'STORE_DIR={store_dir}')
    append(log, f'BASE_URL={base_url}')

    if player is None:
        append(log, 'UNITY_RUNTIME_GATE=UNVERIFIED_ENVIRONMENT')
        append(log, 'REASON=no executable M5 guided Unity player was provided via --unity-player or LGO_M5_GUIDED_UNITY_PLAYER')
        bundle(output_dir, log, store_dir)
        return 30

    env = os.environ.copy()
    env['LG_API_HOST'] = '127.0.0.1'
    env['LG_API_PORT'] = args.port
    env['LG_API_PERSISTENCE_DIR'] = str(store_dir)
    with api_log.open('wb') as output:
        process = subprocess.Popen(['./server/run-api.sh'], cwd=root, env=env, stdout=output, stderr=subprocess.STDOUT)
    append(log, f'API_PID={process.pid}')
    try:
        wait_for_api(base_url, api_log, log)
        run_unity_smoke(root, player, base_url, output_dir / 'unity-m5-guided-training-loop.json', log)
    finally:
        stop_process(process)

    store_file = store_dir / 'players-v1.json'
    if not store_file.exists():
        append(log, f'ERROR: persistence file missing: {store_file}')
        return 46
    if 'm5-guided-training-loop-dev-key' in store_file.read_text(encoding='utf-8', errors='replace'):
        append(log, 'ERROR: raw M5 guided dev key leaked into persistence file.')
        return 47

    archive = bundle(output_dir, log, store_dir)
    append(log, f'M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS output={output_dir} bundle={archive}')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
