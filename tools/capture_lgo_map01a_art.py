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
    args.player = args.player.resolve()
    args.out_dir = args.out_dir.resolve()
    if args.profile == 'all':
        for profile in PROFILES:
            subprocess.run([sys.executable, __file__, '--player', str(args.player), '--out-dir', str(args.out_dir / profile), '--timeout', str(args.timeout), '--profile', profile], check=True)
        print('MAP01A_THREE_ASPECT_CAPTURE_COMPLETE; not physical device certification')
        return
    width, height = PROFILES[args.profile]
    player, out = args.player, args.out_dir
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
    required = ['01-arrival-q01', '02-ha-van-dialogue', '03-q01-complete', '04-q02-grand-gate',
                '05-quan-thu-dialogue', '06-q03-complete', '07-q04-inventory-open', '08-q04-tong-phu-dialogue',
                '09-q04-starter-supplies', '10-q04-health-potion-used', '11-q05-thanh-nhi',
                '12-q05-spirit-herb', '13-q08-hidden-chest', '14-q06-lao-tran', '15-q06-combat',
                '16-q07-class-loot', '17-q07-class-item-equipped', '18-q09-portal-open',
                '19-vo-base', '20-vo-modular', '21-vo-walk', '22-vo-female-full',
                '23-vo-female-slot-toggle', '24-vo-female-walk', '25-vo-lv10-female',
                '26-vo-lv20-female', '27-vo-lv30-female',
                '28-vo-female-run', '29-vo-female-jump-rise', '30-vo-female-jump-apex',
                '31-vo-female-basic-windup', '32-vo-female-basic-impact',
                '33-vo-female-lien-quyen-hit', '34-vo-female-lien-quyen-finish',
                '35-vo-male-run', '36-vo-male-jump', '37-vo-male-basic', '38-vo-male-lien-quyen',
                '39-vo-male-modular-run', '40-vo-male-modular-jump', '41-vo-male-modular-basic',
                '42-vo-male-modular-lien-quyen', '43-vo-female-lv30-modular-run',
                '44-vo-female-lv30-modular-jump', '45-vo-female-lv30-modular-basic',
                '46-vo-female-lv30-modular-lien-quyen']
    slots = ['main-weapon', 'head-hair', 'inner-top', 'outer-tunic', 'lower-garment',
             'waist', 'arm-guard', 'boots', 'light-armor', 'accessory']
    required += [f'{47 + index:02d}-vo-male-lv1-off-{slot}' for index, slot in enumerate(slots)]
    required += [f'{57 + index:02d}-vo-female-lv30-off-{slot}' for index, slot in enumerate(slots)]
    import math
    foot_error = manifest.get('maxFootError', float('nan'))
    parallax = manifest.get('parallaxDelta', float('nan'))
    if (manifest.get('status') != 'TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED'
            or manifest.get('frames') != 66
            or not manifest.get('dialogueOpened') or not manifest.get('greetingCompleted')
            or not manifest.get('voBaseVerified') or not manifest.get('voModularVerified')
            or not manifest.get('voWalkVerified') or not manifest.get('voSkillVerified')
            or not manifest.get('voFemaleVerified') or not manifest.get('voSlotToggleVerified')
            or not manifest.get('voFemaleMotionVerified') or not manifest.get('voProgressionVerified')
            or not all(manifest.get(key) for key in ('voRunVerified', 'voJumpVerified',
                'voBasicVerified', 'voLienQuyenVerified'))
            or not manifest.get('voAttachmentLv1Verified')
            or not manifest.get('voAttachmentLv30FemaleVerified')
            or not manifest.get('voEquipmentComponentBindingVerified')
            or not manifest.get('voTenSlotMatrixVerified')
            or not manifest.get('voSharedRuntimeStateVerified')
            or not manifest.get('mapQuestFlowVerified') or manifest.get('activeQuestId') != 'COMPLETE'
            or not manifest.get('functionalUiVerified')
            or manifest.get('completedQuestCount') != 9
            or not all(manifest.get(key) for key in ('starterSupplies', 'spiritHerb', 'hiddenChest',
                'combatAccepted', 'enemyDefeated', 'enemyLooted', 'portalUnlocked'))
            or not all(manifest.get(key) for key in ('minimapUnlocked', 'healthPotionUsed', 'classRewardEquipped'))
            or manifest.get('healthPotionCount') != 2 or manifest.get('manaPotionCount') != 2
            or manifest.get('playerHealth') != 100
            or manifest.get('voSkillCastCount') != 3
            or manifest.get('voSkillHitCount') != 3 or manifest.get('voTrainingTargetHp') != 0
            or (manifest.get('width'), manifest.get('height')) != (width, height)
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
    print('LGO_MAP01A_PLAYABLE_CAPTURE_TECHNICAL_PASS frames=66 quest=Q01-Q09 vo-motion-attachments ten-slot-matrix; visual review still required; ' + str(out))



if __name__ == '__main__':
    main()
