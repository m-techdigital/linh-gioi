#!/usr/bin/env python3
"""Capture registered outfit movement, combat recovery and ten-slot checks in Player."""
import argparse
import hashlib
import json
import plistlib
import subprocess
import time
from pathlib import Path

PROFILES = {'mobile': (1600, 720), 'tablet': (1024, 768), 'pc': (1280, 720)}
POSE_REVIEW_SLOT_DIRS = {
    'main_weapon': 'main-weapon-review', 'head_hair': 'head-hair-review',
    'inner_top': 'inner-top-review', 'outer_top': 'outer-top-review',
    'lower_body': 'lower-body-review', 'waist_belt': 'waist-belt-review',
    'arm_guard': 'arm-guard-review', 'footwear': 'footwear-review',
    'shoulder_chest_guard': 'shoulder-chest-guard-review',
    'class_accessory': 'class-accessory-review',
}


def resolve_player(path):
    player = path.resolve()
    if player.is_dir() and player.suffix == '.app':
        with (player / 'Contents/Info.plist').open('rb') as file:
            executable = plistlib.load(file)['CFBundleExecutable']
        player = player / 'Contents/MacOS' / executable
    if not player.is_file():
        raise FileNotFoundError('Player executable not found: ' + str(player))
    return player


def player_code_fingerprint(player):
    """The engine executable alone does not identify the game's C# implementation."""
    contents = player.parent.parent
    managed = contents / 'Resources/Data/Managed'
    assemblies = sorted(managed.glob('LinhGioi.*.dll'))
    if not (managed / 'LinhGioi.World.dll').is_file():
        raise FileNotFoundError('Expected Mono Player game assemblies: ' + str(managed))
    return {str(path.relative_to(contents)): hashlib.sha256(path.read_bytes()).hexdigest()
            for path in [player, *assemblies]}


def validate_registered_capture_result(*, code, result, width, height, png_count, closed_far_arms=False, closed_body=False, registered_equipment=False, wardrobe_matrix=False, pose_review_variants=False):
    errors = []
    if code != 0:
        errors.append('PLAYER_EXIT_CODE_' + str(code))
    if result.get('status') != 'TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED':
        errors.append('STATUS_NOT_TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED')
    if closed_far_arms and not result.get('closedFarArms'):
        errors.append('CLOSED_FAR_ARMS_MISSING')
    if closed_body and not result.get('closedBody'):
        errors.append('CLOSED_BODY_MISSING')
    if registered_equipment:
        if not result.get('registeredEquipment'):
            errors.append('REGISTERED_EQUIPMENT_MISSING')
        if not result.get('closedBody'):
            errors.append('REGISTERED_EQUIPMENT_REQUIRES_CLOSED_BODY')
        if result.get('maxEquipmentAttachments') != 17:
            errors.append('REGISTERED_EQUIPMENT_ATTACHMENT_COUNT_MISMATCH')
        if result.get('maxBodyVariants') != 1:
            errors.append('REGISTERED_EQUIPMENT_BODY_VARIANT_COUNT_MISMATCH')
    if pose_review_variants:
        if not result.get('poseReviewLv10Verified'):
            errors.append('POSE_REVIEW_LV10_NOT_VERIFIED')
        if not result.get('poseReviewMixedVerified'):
            errors.append('POSE_REVIEW_MIXED_NOT_VERIFIED')
        if result.get('poseReviewVariantSwitches', 0) < 20:
            errors.append('POSE_REVIEW_VARIANT_SWITCH_COUNT_MISMATCH')
    expected_frames = 186 if wardrobe_matrix else 154
    if wardrobe_matrix:
        core = ('inner_top', 'outer_tunic', 'lower_garment', 'waist')
        rows = result.get('wardrobeCombinations', [])
        expected = {(gender, bits) for gender in ('male', 'female') for bits in range(16)}
        if len(rows) != 32 or {(r.get('gender'), r.get('bits')) for r in rows} != expected:
            errors.append('WARDROBE_COMBINATIONS_INCOMPLETE')
        frames = [r.get('frame') for r in rows]
        if len(set(frames)) != 32 or any(not isinstance(f, int) or not 1 <= f <= expected_frames for f in frames):
            errors.append('WARDROBE_FRAME_REUSE_OR_MISSING')
        for row in rows:
            bits = row.get('bits')
            frame = row.get('frame')
            gender = row.get('gender')
            if (not isinstance(bits, int) or not isinstance(frame, int)
                    or gender not in ('male', 'female')
                    or row.get('file') != f'{frame:02d}-{gender}-wardrobe-{bits:02d}.png'):
                errors.append('WARDROBE_IMAGE_REFERENCE_MISMATCH')
            if not isinstance(bits, int) or row.get('enabledCore') != [slot for bit, slot in enumerate(core) if bits & (1 << bit)]:
                errors.append('WARDROBE_EQUIPMENT_STATE_MISMATCH')
                break
    expected_scalars = {
        'frames': expected_frames,
        'basePoseFrames': 30,
        'actionTransitions': 20,
        'heldJumpRestarts': 4,
        'toggles': 40,
        'width': width,
        'height': height,
    }
    for key, expected in expected_scalars.items():
        if result.get(key) != expected:
            errors.append(key.upper() + '_MISMATCH')
    if result.get('errors'):
        errors.append('PLAYER_REPORTED_ERRORS')
    if png_count != expected_frames:
        errors.append('PNG_FRAME_COUNT_MISMATCH')

    metric_frames = result.get('actorScreenMetricFrames')
    min_ratio = result.get('minActorScreenHeightRatio')
    max_ratio = result.get('maxActorScreenHeightRatio')
    if metric_frames is None:
        errors.append('ACTOR_SCREEN_METRIC_FRAMES_MISSING')
    elif metric_frames != result.get('frames'):
        errors.append('ACTOR_SCREEN_METRIC_FRAMES_INCOMPLETE')
    if min_ratio is None or max_ratio is None:
        errors.append('ACTOR_SCREEN_HEIGHT_RATIO_MISSING')
    elif min_ratio <= 0 or max_ratio < min_ratio:
        errors.append('ACTOR_SCREEN_HEIGHT_RATIO_INVALID')
    return errors

def pose_review_fingerprint(directory):
    names = ['atlas-review.json', 'atlas-review.png']
    for subdir in POSE_REVIEW_SLOT_DIRS.values():
        if (directory / subdir).is_dir():
            names += [f'{subdir}/atlas-review.json', f'{subdir}/atlas-review.png']
    return {name: hashlib.sha256((directory / name).read_bytes()).hexdigest()
            for name in names}


def validate_owner_pose_source(directory):
    # Owner accepted the old session's whole-pose motion. v3 changes only div8→div4.
    # Newer v5/v7 redraw/retarget candidates were explicitly rejected on 2026-09-11.
    approved = {
        'atlas-review.json': '6f78205aa3d1b2f2f4ea7fc43d7abe39a6fb2a7dec93ec5ee28ab997b4bfd98e',
        'atlas-review.png': '27630a5ceece2500e412620b70cf43e61a80bbef5d4391ae450d3d19d6829010',
    }
    fingerprint = pose_review_fingerprint(directory)
    if any(fingerprint.get(name) != digest for name, digest in approved.items()):
        raise ValueError('Use the owner-selected legacy-base-run-contact-jump-v3-div4 pack; changed poses are not the approved motion')


def validate_pose_review_unchanged(directory, expected):
    if pose_review_fingerprint(directory) != expected:
        raise ValueError('Pose pack changed during capture; reject mixed evidence')


def validate_pose_review_pack(directory):
    pack = json.loads((directory / 'atlas-review.json').read_text())
    if pack.get('status') != 'REVIEW_ONLY' or pack.get('runtimeEligible') is not False:
        raise ValueError('Pose pack must remain REVIEW_ONLY')
    if pack.get('samplingDivisor') != 4:
        raise ValueError('Current POSE THỬ review requires div4')
    atlas = (directory / 'atlas-review.png').read_bytes()
    if pack.get('atlasSha256') != hashlib.sha256(atlas).hexdigest() or pack.get('pngBytes') != len(atlas):
        raise ValueError('Pose atlas fingerprint/size changed')
    return pack


def validate_pose_review_log(player_log, directory, pack):
    lines = set(player_log.splitlines())
    if 'LGO_POSE_REVIEW_LOADED ' + str(directory.resolve()) not in lines:
        raise ValueError('Pose review loaded path does not match requested pack')
    required = {sprite['id'] for sprite in pack['sprites']}
    if not required:
        raise ValueError('Pose pack is empty')
    missing = sorted(pose for pose in required if 'LGO_POSE_REVIEW_FRAME ' + pose not in lines)
    if missing:
        raise ValueError('Pose review did not execute: ' + ', '.join(missing))
    for slot, subdir in POSE_REVIEW_SLOT_DIRS.items():
        overlay = directory / subdir
        if overlay.is_dir() and f'LGO_POSE_REVIEW_OVERLAY_LOADED {slot} {overlay.resolve()}' not in lines:
            raise ValueError(f'Pose review {slot} overlay did not load from requested pack')
    return sorted(required)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--player', type=Path, required=True)
    parser.add_argument('--out-dir', type=Path, required=True)
    parser.add_argument('--timeout', type=int, default=90)
    parser.add_argument('--profile', choices=['all', *PROFILES], default='all')
    parser.add_argument('--anatomical', action='store_true')
    parser.add_argument('--closed-far-arms', action='store_true')
    parser.add_argument('--closed-body', action='store_true')
    parser.add_argument('--registered-equipment', action='store_true')
    parser.add_argument('--wardrobe-matrix', action='store_true', help='Append all 16 core garment combinations for both genders')
    parser.add_argument('--pose-review-dir', type=Path, help='Owner-selected v3 div4 motion stack; supersedes the registered presentation during review')
    parser.add_argument('--pose-review-alt-dir', type=Path, help='Optional same-body item variants loaded into the active pose-review actor')
    args = parser.parse_args()
    if args.wardrobe_matrix and not args.registered_equipment:
        parser.error("--wardrobe-matrix requires --registered-equipment")
    if args.pose_review_dir:
        validate_owner_pose_source(args.pose_review_dir)
    if args.pose_review_alt_dir:
        if not args.pose_review_dir:
            parser.error('--pose-review-alt-dir requires --pose-review-dir')
        validate_owner_pose_source(args.pose_review_alt_dir)
    player = resolve_player(args.player)
    player_fingerprint = player_code_fingerprint(player)
    pose_fingerprint = pose_review_fingerprint(args.pose_review_dir) if args.pose_review_dir else None
    pose_pack = validate_pose_review_pack(args.pose_review_dir) if args.pose_review_dir else None
    alternate_fingerprint = pose_review_fingerprint(args.pose_review_alt_dir) if args.pose_review_alt_dir else None
    for profile in PROFILES if args.profile == 'all' else [args.profile]:
        if player_code_fingerprint(player) != player_fingerprint:
            raise ValueError('Player code changed between capture profiles')
        if pose_pack is not None:
            validate_pose_review_unchanged(args.pose_review_dir, pose_fingerprint)
        if alternate_fingerprint is not None:
            validate_pose_review_unchanged(args.pose_review_alt_dir, alternate_fingerprint)
        width, height = PROFILES[profile]
        out = args.out_dir.resolve() / profile
        out.mkdir(parents=True, exist_ok=False)
        with (out / 'launch.log').open('w') as log:
            process = subprocess.Popen([str(player), '-logFile', str(out / 'player.log'),
                '-screen-fullscreen', '0', '-screen-width', str(width), '-screen-height', str(height),
                '--lgo-map01a-art-preview', '--lgo-vo-registered', '--lgo-registered-capture',
                '--lgo-map01a-device', profile, '--lgo-map01a-art-dir', str(out)] + (['--lgo-wardrobe-matrix'] if args.wardrobe_matrix else []) + (['--lgo-vo-anatomical'] if args.anatomical or args.closed_far_arms or args.closed_body else []) + (['--lgo-vo-closed-far-arms'] if args.closed_far_arms else []) + (['--lgo-vo-closed-body'] if args.closed_body else []) + (['--lgo-vo-registered-equipment'] if args.registered_equipment else []) + (['--lgo-vo-pose-review-dir', str(args.pose_review_dir.resolve())] if args.pose_review_dir else []) + (['--lgo-vo-pose-review-alt-dir', str(args.pose_review_alt_dir.resolve())] if args.pose_review_alt_dir else []),
                cwd=player.parent, stdout=log, stderr=subprocess.STDOUT)
            try:
                started = time.monotonic()
                while process.poll() is None and time.monotonic() - started < min(25, args.timeout):
                    path = out / 'player.log'
                    if path.exists() and 'UnloadTime:' in path.read_text(errors='replace'):
                        break
                    time.sleep(.25)
                if process.poll() is None:
                    # Activate this PID, not another interactive app with the same bundle ID.
                    script = "ObjC.import('AppKit'); $.NSRunningApplication.runningApplicationWithProcessIdentifier(" + str(process.pid) + ").activateWithOptions(2);"
                    subprocess.run(['osascript', '-l', 'JavaScript', '-e', script], check=True, stdout=log, stderr=log)
                code = process.wait(timeout=max(1, args.timeout - (time.monotonic() - started)))
            except Exception:
                process.kill()
                process.wait()
                raise
        if player_code_fingerprint(player) != player_fingerprint:
            raise ValueError('Player code changed during capture')
        result = json.loads((out / 'registered-manifest.json').read_text())
        if args.pose_review_dir:
            validate_pose_review_unchanged(args.pose_review_dir, pose_fingerprint)
            player_log = (out / 'player.log').read_text(errors='replace')
            executed_poses = validate_pose_review_log(player_log, args.pose_review_dir, pose_pack)
            if args.pose_review_alt_dir:
                validate_pose_review_unchanged(args.pose_review_alt_dir, alternate_fingerprint)
                if 'LGO_POSE_REVIEW_VARIANTS_LOADED ' + str(args.pose_review_alt_dir.resolve()) not in player_log.splitlines():
                    raise ValueError('Alternate pose review pack did not load into the active actor')
        validation_errors = validate_registered_capture_result(
            code=code,
            result=result,
            width=width,
            height=height,
            png_count=len(list(out.glob('*.png'))),
            closed_far_arms=args.closed_far_arms,
            closed_body=args.closed_body,
            registered_equipment=args.registered_equipment,
            wardrobe_matrix=args.wardrobe_matrix,
            pose_review_variants=args.pose_review_alt_dir is not None,
        )
        if args.wardrobe_matrix:
            for row in result.get('wardrobeCombinations', []):
                name = row.get('file', '')
                if Path(name).name != name or not (out / name).is_file():
                    validation_errors.append('WARDROBE_IMAGE_MISSING')
        if validation_errors:
            raise SystemExit('FIX_REQUIRED: ' + str(out) + ' ' + ','.join(validation_errors))
        if pose_pack is not None:
            provenance = {
                'status': 'TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED',
                'playerExecutable': str(player),
                'playerExecutableSha256': hashlib.sha256(player.read_bytes()).hexdigest(),
                'playerCodeFingerprint': player_fingerprint,
                'packDirectory': str(args.pose_review_dir.resolve()),
                'manifestSha256': pose_fingerprint['atlas-review.json'],
                'atlasSha256': pose_fingerprint['atlas-review.png'],
                'posePackFingerprint': pose_fingerprint,
                'alternatePosePackFingerprint': alternate_fingerprint,
                'packStableBeforeAfterCapture': True,
                'samplingDivisor': pose_pack['samplingDivisor'],
                'runtimeEligible': False,
                'executedPoses': executed_poses,
                'frames': result['frames'],
            }
            (out / 'pose-review-provenance.json').write_text(json.dumps(provenance, indent=2) + chr(10))
        print(f"{profile}: {result['frames']} Player frames; technical checks passed; visual review required", flush=True)


if __name__ == '__main__':
    main()
