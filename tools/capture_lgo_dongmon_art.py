#!/usr/bin/env python3
"""Capture the opt-in Dong Mon draft in a real macOS Player, separate from baseline evidence."""
import argparse
import json
import subprocess
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--player', type=Path, default=ROOT / 'build/2d-onboarding-player/LinhGioiOnline2D.app/Contents/MacOS/Unity')
    parser.add_argument('--out-dir', type=Path, default=ROOT / 'build/dongmon-art/player')
    parser.add_argument('--timeout', type=int, default=90)
    args = parser.parse_args()
    player, out = args.player.resolve(), args.out_dir.resolve()
    if not player.is_file() or player.parent.name != 'MacOS':
        raise SystemExit('RUNTIME_BLOCKED_ENV: missing macOS Player')
    # Never erase earlier capture logs or reuse a stale successful manifest.
    out.mkdir(parents=True, exist_ok=False)
    with (out / 'launch.log').open('w') as log:
        process = subprocess.Popen([str(player), '-logFile', str(out / 'player.log'),
            '-screen-fullscreen', '0', '-screen-width', '1280', '-screen-height', '720',
            '--lgo-dongmon-art-capture', '--lgo-dongmon-art-dir', str(out)],
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
    required = ['01-arrival', '02-approach', '03-dialogue', '04-guidance', '05-parallax']
    if (manifest.get('status') != 'PASS' or manifest.get('atlasParts') != 3
            or not manifest.get('dialogueOpened') or not manifest.get('guidanceReached')
            or abs(manifest.get('parallaxDelta', 0)) <= .01
            or any(not (out / (name + '.bmp')).is_file() for name in required)):
        raise SystemExit('FIX_REQUIRED: incomplete art capture ' + str(out))
    for name in required:
        subprocess.run(['sips', '-s', 'format', 'png', str(out / (name + '.bmp')), '--out', str(out / (name + '.png'))], check=True, stdout=subprocess.DEVNULL)
    print('LGO_DONGMON_ART_CAPTURE_PASS frames=5; visual review still required; ' + str(out))


if __name__ == '__main__':
    main()
