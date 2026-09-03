#!/usr/bin/env python3
from __future__ import annotations
import argparse
import hashlib
import tarfile
import xml.etree.ElementTree as ET
import zipfile
from pathlib import Path

REQUIRED_EVIDENCE = {
    'unity-version.txt',
    'commands.log',
    'prepare-unity-protocol.log',
    'unity-import-generate.log',
    'unity-editmode.log',
    'unity-editmode-results.xml',
    'unity-linux-player-build.log',
    'unity-evidence-summary.md',
    'generated-unity-file-list.txt',
    'player-file-list.txt',
}
M2_TEST_NAMES = {
    'MoveIntentSerializesDeterministicSequenceAxisAndDelta',
    'PlayerTransformSnapshotCarriesAuthoritativeAckAndPosition',
    'TcpRealtimeClientRejectsNonNormalizedMoveIntentBeforeSend',
    'TcpRealtimeClientRejectsNonFiniteMoveIntentBeforeSend',
}


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open('rb') as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b''):
            h.update(chunk)
    return h.hexdigest()


def check_sha(path: Path, sha_path: Path) -> None:
    expected = sha_path.read_text(encoding='utf-8').split()[0]
    actual = sha256(path)
    if actual != expected:
        raise SystemExit(f'ERROR: checksum mismatch for {path}: expected {expected}, got {actual}')
    print(f'SHA256_PASS file={path.name} sha256={actual}')


def assert_editmode_results(xml_text: str) -> None:
    root = ET.fromstring(xml_text)
    total = int(root.attrib.get('total', '0'))
    failed = int(root.attrib.get('failed', '0'))
    skipped = int(root.attrib.get('skipped', '0'))
    if total <= 0:
        raise SystemExit('ERROR: Unity EditMode result XML has zero tests')
    if failed != 0:
        raise SystemExit(f'ERROR: Unity EditMode result XML reports failed={failed}')
    if skipped != 0:
        raise SystemExit(f'ERROR: Unity EditMode result XML reports skipped={skipped}')
    names = {node.attrib.get('name', '') for node in root.iter('test-case')}
    missing = sorted(name for name in M2_TEST_NAMES if not any(name in candidate for candidate in names))
    if missing:
        raise SystemExit(f'ERROR: M2 EditMode result XML is missing expected tests: {missing}')


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument('--player-archive', type=Path, required=True)
    ap.add_argument('--player-sha256', type=Path, required=True)
    ap.add_argument('--evidence-zip', type=Path, required=True)
    ap.add_argument('--evidence-sha256', type=Path, required=True)
    args = ap.parse_args()

    check_sha(args.player_archive, args.player_sha256)
    check_sha(args.evidence_zip, args.evidence_sha256)

    with zipfile.ZipFile(args.evidence_zip) as zf:
        names = set(zf.namelist())
        missing = sorted(REQUIRED_EVIDENCE - names)
        if missing:
            raise SystemExit(f'ERROR: evidence zip missing required files: {missing}')
        version_text = zf.read('unity-version.txt').decode('utf-8', 'replace')
        if '6000.3.2f1' not in version_text:
            raise SystemExit('ERROR: evidence does not prove Unity 6000.3.2f1')
        assert_editmode_results(zf.read('unity-editmode-results.xml').decode('utf-8', 'replace'))
        build_log = zf.read('unity-linux-player-build.log').decode('utf-8', 'replace')
        if 'LGO_LINUX_PLAYER_BUILD' not in build_log and 'Build succeeded' not in build_log:
            raise SystemExit('ERROR: Linux player build evidence is missing')

    with tarfile.open(args.player_archive, 'r:gz') as tf:
        members = [m.name for m in tf.getmembers() if not Path(m.name).name.startswith('._')]
        if not any(Path(name).name == 'LinhGioiM0PlayerSmoke.x86_64' for name in members):
            raise SystemExit('ERROR: player archive does not contain LinhGioiM0PlayerSmoke.x86_64 executable')

    print('M2_EVIDENCE_BUNDLE_VERIFY_PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
