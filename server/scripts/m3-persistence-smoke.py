#!/usr/bin/env python3
from __future__ import annotations
import argparse
import json
import math
import sys
import urllib.error
import urllib.request


def request(method: str, url: str, payload: dict | None = None) -> tuple[int, dict]:
    data = None if payload is None else json.dumps(payload).encode('utf-8')
    headers = {'Content-Type': 'application/json'} if payload is not None else {}
    req = urllib.request.Request(url, data=data, method=method, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=10) as response:
            body = response.read().decode('utf-8')
            return response.status, json.loads(body) if body else {}
    except urllib.error.HTTPError as error:
        body = error.read().decode('utf-8')
        try:
            parsed = json.loads(body) if body else {}
        except json.JSONDecodeError:
            parsed = {'raw': body}
        return error.code, parsed


def assert_status(actual: int, expected: int, label: str) -> None:
    if actual != expected:
        raise AssertionError(f'{label}: expected status {expected}, got {actual}')


def assert_close(actual: float, expected: float, label: str) -> None:
    if not math.isclose(float(actual), expected, rel_tol=0.0, abs_tol=0.0001):
        raise AssertionError(f'{label}: expected {expected}, got {actual}')


def main() -> int:
    parser = argparse.ArgumentParser(description='M3 account/character persistence HTTP smoke')
    parser.add_argument('--base-url', default='http://127.0.0.1:18080')
    parser.add_argument('--expect-existing', action='store_true')
    args = parser.parse_args()
    base = args.base_url.rstrip('/')

    status, health = request('GET', base + '/health')
    assert_status(status, 200, 'health')
    if health.get('status') != 'UP':
        raise AssertionError(f'health status not UP: {health}')

    status, login = request('POST', base + '/dev/auth/login', {'devKey': 'm3-smoke-dev-key', 'displayName': 'M3Smoke'})
    assert_status(status, 200, 'dev login')
    account = login['account']
    account_id = account['accountId']
    if not account_id.startswith('account.dev.'):
        raise AssertionError(f'unexpected account id: {account_id}')
    if args.expect_existing and login.get('created') is not False:
        raise AssertionError('expected persisted dev account to exist after restart')

    status, characters = request('GET', base + f'/accounts/{account_id}/characters')
    assert_status(status, 200, 'list characters')

    existing = next((item for item in characters if item.get('name') == 'M3Hero'), None)
    if existing is None:
        status, created = request('POST', base + f'/accounts/{account_id}/characters', {'name': 'M3Hero', 'classId': 'class.sword'})
        assert_status(status, 201, 'create character')
        character = created
    else:
        character = existing
    character_id = character['characterId']
    if not character_id.startswith('character.'):
        raise AssertionError(f'unexpected character id: {character_id}')

    status, moved = request('POST', base + f'/characters/{character_id}/position', {'x': 1.25, 'y': 0.0, 'z': -2.5, 'yawDegrees': 180.0})
    assert_status(status, 200, 'save position')
    assert_close(moved['x'], 1.25, 'saved x')
    assert_close(moved['z'], -2.5, 'saved z')
    assert_close(moved['yawDegrees'], 180.0, 'saved yaw')

    status, loaded = request('GET', base + f'/characters/{character_id}')
    assert_status(status, 200, 'load character')
    assert_close(loaded['x'], 1.25, 'loaded x')
    assert_close(loaded['z'], -2.5, 'loaded z')
    assert_close(loaded['yawDegrees'], 180.0, 'loaded yaw')

    status, invalid = request('POST', base + f'/accounts/{account_id}/characters', {'name': 'No', 'classId': 'class.sword'})
    assert_status(status, 400, 'invalid character name')
    status, missing = request('GET', base + '/characters/character.missing')
    assert_status(status, 404, 'missing character')

    print('M3_PERSISTENCE_SMOKE_PASS')
    print('account_id=' + account_id)
    print('character_id=' + character_id)
    print('entity_id=' + str(loaded['entityId']))
    print('x=' + str(loaded['x']))
    print('z=' + str(loaded['z']))
    print('yaw_degrees=' + str(loaded['yawDegrees']))
    print('expect_existing=' + str(args.expect_existing).lower())
    return 0


if __name__ == '__main__':
    sys.exit(main())
