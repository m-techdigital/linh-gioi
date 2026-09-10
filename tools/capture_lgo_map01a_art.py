#!/usr/bin/env python3
"""Capture the opt-in Map01A art slice in a real macOS Player, separate from baseline evidence."""
import argparse
import json
import subprocess
import time
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROFILES = {'mobile': (1600, 720), 'tablet': (1024, 768), 'pc': (1280, 720)}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--player', type=Path, default=ROOT / 'build/map01a-art/player/LinhGioiMap01A.app/Contents/MacOS/Unity')
    parser.add_argument('--out-dir', type=Path, default=ROOT / 'build/map01a-art/capture')
    parser.add_argument('--timeout', type=int, default=90)
    parser.add_argument('--profile', choices=['all', *PROFILES], default='all')
    args = parser.parse_args()
    if args.profile == 'all':
        for profile in PROFILES:
            subprocess.run([sys.executable, __file__, '--player', str(args.player), '--out-dir', str(args.out_dir / profile), '--timeout', str(args.timeout), '--profile', profile], check=True)
        print('MAP01A_THREE_ASPECT_CAPTURE_COMPLETE; not physical device certification')
        return
    width, height = PROFILES[args.profile]
    player, out = args.player.resolve(), args.out_dir.resolve()
    if not player.is_file() or player.parent.name != 'MacOS':
        raise SystemExit('RUNTIME_BLOCKED_ENV: missing macOS Player')
    # Never erase earlier capture logs or reuse a stale successful manifest.
    out.mkdir(parents=True, exist_ok=False)
    with (out / 'launch.log').open('w') as log:
        process = subprocess.Popen([str(player), '-logFile', str(out / 'player.log'),
            '-screen-fullscreen', '0', '-screen-width', str(width), '-screen-height', str(height),
            '--lgo-map01a-device', args.profile, '--lgo-map01a-art-capture', '--lgo-map01a-art-dir', str(out)],
            cwd=player.parent, stdout=log, stderr=subprocess.STDOUT)
        try:
            started = time.monotonic()
            # LaunchServices cannot activate the app until Unity finishes initial loading.
            while process.poll() is None and time.monotonic() - started < min(25, args.timeout):
                player_log = out / 'player.log'
                if player_log.exists() and 'UnloadTime:' in player_log.read_text(errors='replace'):
                    break
                time.sleep(.25)
            if process.poll() is None:
                subprocess.run(['open', str(player.parents[2])], check=True, stdout=log, stderr=log)
            code = process.wait(timeout=max(1, args.timeout - (time.monotonic() - started)))
        except (subprocess.TimeoutExpired, subprocess.CalledProcessError):
            process.kill()
            process.wait()
            raise SystemExit('VISUAL_CAPTURE_TIMEOUT_OR_LAUNCH_FAILURE: ' + str(out))
    if code != 0:
        raise SystemExit('FIX_REQUIRED: Player exited ' + str(code))
    manifest = json.loads((out / 'manifest.json').read_text())
    required = ['01-arrival', '02-dialogue', '03-grand-gate', '04-gate-captain',
                '05-market', '06-well-bridge', '07-combat-edge', '08-portal',
                '09-vo-base', '10-vo-modular', '11-vo-walk', '12-vo-skill']
    import math
    foot_error = manifest.get('maxFootError', float('nan'))
    parallax = manifest.get('parallaxDelta', float('nan'))
    if (manifest.get('status') != 'TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED'
            or manifest.get('frames') != 12
            or not manifest.get('dialogueOpened') or not manifest.get('greetingCompleted')
            or not manifest.get('voBaseVerified') or not manifest.get('voModularVerified')
            or not manifest.get('voWalkVerified') or not manifest.get('voSkillVerified')
            or manifest.get('voSkillCastCount') != 1
            or (manifest.get('width'), manifest.get('height')) != (width, height)
            or manifest.get('mapQuestFlowVerified') is not False
            or not math.isfinite(foot_error) or foot_error > .001
            or not math.isfinite(parallax) or abs(parallax) <= .01
            or any(not (out / (name + '.bmp')).is_file() for name in required)):
        raise SystemExit('FIX_REQUIRED: incomplete Map01A art capture ' + str(out))
    for name in required:
        subprocess.run(['sips', '-s', 'format', 'png', str(out / (name + '.bmp')), '--out', str(out / (name + '.png'))], check=True, stdout=subprocess.DEVNULL)
    import struct
    for name in required:
        raw = (out / (name + '.png')).read_bytes()
        if raw[:8] != b'\x89PNG\r\n\x1a\n' or struct.unpack('>II', raw[16:24]) != (width, height):
            raise SystemExit('FIX_REQUIRED: invalid capture PNG ' + name)
    print('LGO_MAP01A_ART_CAPTURE_TECHNICAL_PASS frames=12; visual review and quest flow still required; ' + str(out))



if __name__ == '__main__':
    main()
