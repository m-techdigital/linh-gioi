#!/usr/bin/env python3
from __future__ import annotations
import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

REQUIRED_FILES = [
    'server/realtime/src/main/java/com/linhgioi/server/realtime/session/OnlineSession.java',
    'server/realtime/src/main/java/com/linhgioi/server/realtime/session/OnlineSessionHandler.java',
    'server/realtime/src/test/java/com/linhgioi/server/realtime/session/OnlineSessionTest.java',
    'server/realtime/src/test/java/com/linhgioi/server/realtime/session/OnlineSessionHandlerTest.java',
    'server/realtime/src/test/java/com/linhgioi/server/realtime/session/OnlineSessionIntegrationTest.java',
    'server/scripts/online-session-smoke.py',
    'tools/m2_online_session_evidence/build_m2_online_session_evidence.sh',
    'tools/m2_online_session_evidence/run_m2_online_session_smoke.sh',
    'tools/prepare_unity_local_assets.sh',
    'tools/run_m2_local_runtime_once.sh',
    'M2-RUNTIME-CANDIDATE-LOCAL-COMMANDS-v0.6.2.md',
    'tools/m2_online_session_evidence/verify_m2_evidence_bundle.py',
    'client/Unity/Assets/Game/Networking/Runtime/OnlineSessionSmokeRunner.cs',
    'client/Unity/Assets/Game/Tests/EditMode/M2OnlineSessionTests.cs',
    'docs/tasks/M2-ONLINE-SESSION-PROTOTYPE.md',
    'docs/execution/M2-RUNTIME-EVIDENCE.md',
    'docs/execution/checklists/M2-RUNTIME-CLOSURE-CHECKLIST.md',
    'docs/execution/prompts/P9-M2-ONLINE-SESSION-PROTOTYPE.md',
    'm2-manifest.json',
]

FORBIDDEN_PRODUCTION_MARKERS = [
    ('protocol/', 'M2 must consume existing movement/handshake messages without protocol edits.'),
    ('gamedata/schemas/', 'M2 must not mutate GameData schemas.'),
]


def read(path: str) -> str:
    p = ROOT / path
    if not p.exists():
        errors.append(f'missing: {path}')
        return ''
    return p.read_text(encoding='utf-8')


def require_contains(label: str, content: str, needle: str) -> None:
    if needle not in content:
        errors.append(f'{label} missing required marker: {needle}')


def require_regex(label: str, content: str, pattern: str) -> None:
    if not re.search(pattern, content, re.MULTILINE):
        errors.append(f'{label} missing required pattern: {pattern}')


def validate_json(path: str, required_markers: list[str]) -> None:
    content = read(path)
    try:
        data = json.loads(content)
    except json.JSONDecodeError as exc:
        errors.append(f'{path} is invalid JSON: {exc}')
        return
    serialized = json.dumps(data, sort_keys=True)
    for marker in required_markers:
        if marker not in serialized:
            errors.append(f'{path} missing JSON marker: {marker}')


def validate_asmdef(path: str, required_refs: list[str]) -> None:
    p = ROOT / path
    if not p.exists():
        errors.append(f'missing asmdef: {path}')
        return
    data = json.loads(p.read_text(encoding='utf-8'))
    refs = data.get('references', [])
    for ref in required_refs:
        if ref not in refs:
            errors.append(f'{path} missing asmdef reference: {ref}')


def main() -> int:
    for rel in REQUIRED_FILES:
        if not (ROOT / rel).exists():
            errors.append(f'missing: {rel}')

    session = read('server/realtime/src/main/java/com/linhgioi/server/realtime/session/OnlineSession.java')
    require_contains('OnlineSession', session, 'MOVE_SPEED_UNITS_PER_SECOND')
    require_contains('OnlineSession', session, 'MAX_CLIENT_DELTA_SECONDS')
    require_contains('OnlineSession', session, 'applyMove')
    require_contains('OnlineSession', session, 'PlayerTransformSnapshot')
    require_contains('OnlineSession', session, 'MoveIntent.sequence must be positive')
    require_contains('OnlineSession', session, 'MoveIntent.move_axis values must be normalized')
    require_contains('OnlineSession', session, 'MoveIntent.move_axis magnitude must be <= 1')
    require_regex('OnlineSession', session, r'sequence > acknowledgedSequence')

    handler = read('server/realtime/src/main/java/com/linhgioi/server/realtime/session/OnlineSessionHandler.java')
    require_contains('OnlineSessionHandler', handler, 'realtime_session_move_applied')
    require_contains('OnlineSessionHandler', handler, 'realtime_session_move_rejected')
    require_contains('OnlineSessionHandler', handler, 'realtime_session_malformed_move_intent')
    require_contains('OnlineSessionHandler', handler, 'context.close()')

    handshake = read('server/realtime/src/main/java/com/linhgioi/server/realtime/protocol/HandshakeHandler.java')
    require_contains('HandshakeHandler', handshake, 'realtime_session_opened')
    require_contains('HandshakeHandler', handshake, 'OnlineSessionHandler')
    require_contains('HandshakeHandler', handshake, 'context.pipeline().replace')

    server_tests = read('server/realtime/src/test/java/com/linhgioi/server/realtime/session/OnlineSessionIntegrationTest.java')
    for marker in [
        'acceptedClientCanSendMoveIntentAndReceiveAuthoritativeSnapshot',
        'invalidPostHandshakeMoveClosesClientButServerSurvivesReconnect',
        'assertEquals(0.4f',
    ]:
        require_contains('OnlineSessionIntegrationTest', server_tests, marker)

    py_smoke = read('server/scripts/online-session-smoke.py')
    require_contains('online-session-smoke.py', py_smoke, 'M2_ONLINE_SESSION_SMOKE_PASS')
    require_contains('online-session-smoke.py', py_smoke, 'move_intent')
    require_contains('online-session-smoke.py', py_smoke, 'assert_snapshot')

    client = read('client/Unity/Assets/Game/Networking/Runtime/TcpRealtimeClient.cs')
    require_contains('TcpRealtimeClient', client, 'SendMoveIntentAsync')
    require_contains('TcpRealtimeClient', client, 'ValidateMoveIntent')
    require_contains('TcpRealtimeClient', client, 'MoveIntent.move_axis magnitude must be <= 1')
    require_contains('TcpRealtimeClient', client, 'PlayerTransformSnapshot.Parser.ParseFrom')
    require_contains('TcpRealtimeClient', client, 'Realtime client must be connected before sending movement')

    smoke = read('client/Unity/Assets/Game/Networking/Runtime/OnlineSessionSmokeRunner.cs')
    require_contains('OnlineSessionSmokeRunner', smoke, '--lgo-m2-online-session-smoke')
    require_contains('OnlineSessionSmokeRunner', smoke, '--lgo-m2-host')
    require_contains('OnlineSessionSmokeRunner', smoke, '--lgo-m2-port')
    require_contains('OnlineSessionSmokeRunner', smoke, 'SendMoveIntentAsync')
    require_contains('OnlineSessionSmokeRunner', smoke, 'duplicateSnapshot')
    require_contains('OnlineSessionSmokeRunner', smoke, 'secondSnapshot')
    require_contains('OnlineSessionSmokeRunner', smoke, 'status = "PASS"')

    bootstrap = read('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs')
    require_contains('GameBootstrap', bootstrap, 'OnlineSessionSmokeRunner.ShouldRun()')
    require_contains('GameBootstrap', bootstrap, 'OnlineSessionSmokeRunner.RunFromCommandLineAsync')

    unity_tests = read('client/Unity/Assets/Game/Tests/EditMode/M2OnlineSessionTests.cs')
    require_contains('M2OnlineSessionTests', unity_tests, 'MoveIntentSerializesDeterministicSequenceAxisAndDelta')
    require_contains('M2OnlineSessionTests', unity_tests, 'PlayerTransformSnapshotCarriesAuthoritativeAckAndPosition')
    require_contains('M2OnlineSessionTests', unity_tests, 'TcpRealtimeClientRejectsNonNormalizedMoveIntentBeforeSend')
    require_contains('M2OnlineSessionTests', unity_tests, 'TcpRealtimeClientRejectsNonFiniteMoveIntentBeforeSend')

    validate_asmdef('client/Unity/Assets/Game/Networking/LinhGioi.Networking.asmdef', ['LinhGioi.Foundation', 'LinhGioi.Protocol'])
    validate_asmdef('client/Unity/Assets/Game/Bootstrap/LinhGioi.Bootstrap.asmdef', ['LinhGioi.Networking', 'LinhGioi.Protocol'])
    validate_asmdef('client/Unity/Assets/Game/Tests/EditMode/LinhGioi.Tests.EditMode.asmdef', ['LinhGioi.Networking', 'LinhGioi.Protocol'])

    validate_json('m2-manifest.json', [
        'M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE',
        '--lgo-m2-online-session-smoke',
        'M1_OFFLINE_COMBAT_RUNTIME_CLOSED',
    ])

    prompt = read('docs/execution/prompts/P9-M2-ONLINE-SESSION-PROTOTYPE.md')
    require_contains('M2 prompt', prompt, 'M1_OFFLINE_COMBAT_RUNTIME_CLOSED')
    require_contains('M2 prompt', prompt, 'No protocol/schema changes')
    require_contains('M2 prompt', prompt, 'single-session online loop')

    local_commands = read('M2-RUNTIME-CANDIDATE-LOCAL-COMMANDS-v0.6.2.md')
    require_contains('M2 local commands', local_commands, 'M2_LOCAL_RUNTIME_CANDIDATE_READY')
    require_contains('M2 local commands', local_commands, 'M2_LOCAL_RUNTIME_CANDIDATE_PARTIAL')

    local_assets = read('tools/prepare_unity_local_assets.sh')
    require_contains('prepare_unity_local_assets.sh', local_assets, 'UNITY_LOCAL_ASSETS_READY')
    require_contains('prepare_unity_local_assets.sh', local_assets, 'Google.Protobuf.dll')
    require_contains('prepare_unity_local_assets.sh', local_assets, 'prepare_unity_protocol.py')

    one_command = read('tools/run_m2_local_runtime_once.sh')
    require_contains('run_m2_local_runtime_once.sh', one_command, 'M2_LOCAL_RUNTIME_CANDIDATE_READY')
    require_contains('run_m2_local_runtime_once.sh', one_command, 'M2_LOCAL_RUNTIME_CANDIDATE_PARTIAL')
    require_contains('run_m2_local_runtime_once.sh', one_command, 'Full M2 runtime candidate did not produce required Unity player/editor evidence files')
    require_contains('run_m2_local_runtime_once.sh', one_command, './server/run-realtime.sh')
    require_contains('run_m2_local_runtime_once.sh', one_command, 'build_m2_online_session_evidence.sh')
    require_contains('run_m2_local_runtime_once.sh', one_command, 'online-session-smoke.py')
    require_contains('run_m2_local_runtime_once.sh', one_command, 'UPLOAD-THESE-FILES-M2-RUNTIME-CANDIDATE')

    gamedata_validator = read('tools/validate_gamedata.py')
    require_contains('validate_gamedata.py', gamedata_validator, 'stdlib-fallback')
    require_contains('validate_gamedata.py', gamedata_validator, '_MiniDraft202012Validator')

    verifier = read('tools/m2_online_session_evidence/verify_m2_evidence_bundle.py')
    require_contains('verify_m2_evidence_bundle.py', verifier, 'TcpRealtimeClientRejectsNonNormalizedMoveIntentBeforeSend')
    require_contains('verify_m2_evidence_bundle.py', verifier, 'TcpRealtimeClientRejectsNonFiniteMoveIntentBeforeSend')

    smoke_script = read('tools/m2_online_session_evidence/run_m2_online_session_smoke.sh')
    require_contains('run_m2_online_session_smoke.sh', smoke_script, 'duplicateSnapshot')
    require_contains('run_m2_online_session_smoke.sh', smoke_script, 'secondSnapshot')

    evidence_doc = read('docs/execution/M2-RUNTIME-EVIDENCE.md')
    require_contains('M2 runtime evidence', evidence_doc, '--lgo-m2-online-session-smoke')
    require_contains('M2 runtime evidence', evidence_doc, 'M2_ONLINE_SESSION_RUNTIME_CLOSED')
    require_contains('M2 runtime evidence', evidence_doc, 'Unity-to-Java session smoke')

    checklist = read('docs/execution/checklists/M2-RUNTIME-CLOSURE-CHECKLIST.md')
    require_contains('M2 checklist', checklist, 'Unity-to-Java session smoke')
    require_contains('M2 checklist', checklist, 'Reconnect/failure path')
    require_contains('M2 checklist', checklist, 'server survival')

    project_state = read('docs/execution/PROJECT-STATE.md')
    require_contains('PROJECT-STATE', project_state, 'M2 Online Session Prototype')
    require_contains('PROJECT-STATE', project_state, 'M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE')
    require_contains('PROJECT-STATE', project_state, 'M1_OFFLINE_COMBAT_RUNTIME_CLOSED')

    generated = ROOT / 'client/Unity/Assets/Game/Generated'
    generated_files = [p for p in generated.rglob('*') if p.is_file()] if generated.exists() else []
    if generated_files:
        print(
            'M2 ONLINE SESSION STATIC VALIDATION NOTICE: '
            'client/Unity/Assets/Game/Generated contains local disposable Unity outputs; '
            'this is allowed for local runtime work but must be excluded from release deltas/full-source packages.'
        )

    if errors:
        print('M2 ONLINE SESSION STATIC VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M2 ONLINE SESSION STATIC VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
