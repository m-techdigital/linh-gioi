#!/usr/bin/env python3.12
"""Open one interactive Player with a single switchable source-pose actor."""
import argparse
import json
import plistlib
import subprocess
from pathlib import Path

# Owner review starts from the locked Võ source-pose stack. Pháp stays in
# PACK_SUFFIXES for explicit audits, but is excluded from the interactive catalog
# until it has non-base source-pose class art for the same gate as Kiếm/Cơ/Linh.
CLASSES = ('vo', 'kiem', 'co', 'linh')
PACK_SUFFIXES = {
    'vo': ('-source-pose-review-preserved-lv1/pack', '-source-pose-review-preserved-lv10/pack', None, None),
    'kiem': ('-source-pose-review-v1/pack', '-source-pose-review-lv10-v1/pack',
             '-female-source-pose-review-v1/pack', '-female-source-pose-review-lv10-v1/pack'),
    'phap': ('-source-pose-review-deterministic-v7/pack', None, None, None),
    'co': ('-source-pose-review-v1/pack', '-source-pose-review-lv10-semantic-v3/pack',
           '-female-source-pose-review-semantic-v3/pack', '-female-source-pose-review-lv10-semantic-v3/pack'),
    'linh': ('-source-pose-review-v1/pack', '-source-pose-review-lv10-semantic-v3/pack',
             '-female-source-pose-review-v1/pack', '-female-source-pose-review-lv10-v1/pack'),
}

REJECTED_SELECTION_MARKERS = ('REJECTED', 'WITHDRAWN')


def validate_source_selection(manifest_path: Path, manifest: dict, cache: dict[Path, dict]) -> None:
    """Reject an atlas whose source belongs to a withdrawn authoring selection."""
    for sprite in manifest.get('sprites', ()):
        source_value = sprite.get('source') if isinstance(sprite, dict) else None
        if not source_value:
            continue
        source = Path(source_value)
        if not source.is_absolute():
            source = manifest_path.parent / source
        for parent in (source.parent, *source.parents):
            selection_path = parent / 'authoring-selection.json'
            if selection_path.is_file():
                selection = cache.setdefault(selection_path, json.loads(selection_path.read_text()))
                status = str(selection.get('status', '')).upper()
                if any(marker in status for marker in REJECTED_SELECTION_MARKERS):
                    reason = selection.get('rejectionReason') or selection.get('reason') or ''
                    raise ValueError(
                        f'Nguồn review đã bị loại ({status}): {selection_path}'
                        + (f' — {reason}' if reason else ''))
                break


def resolve_player(path: Path) -> Path:
    path = path.resolve()
    if path.is_dir() and path.suffix == '.app':
        with (path / 'Contents/Info.plist').open('rb') as file:
            executable = plistlib.load(file)['CFBundleExecutable']
        path = path / 'Contents/MacOS' / executable
    if not path.is_file():
        raise FileNotFoundError(path)
    return path


def class_pack_paths(repo: Path, class_id: str) -> tuple[Path | None, Path | None, Path | None, Path | None]:
    if class_id not in PACK_SUFFIXES:
        raise ValueError('Unsupported source-pose class: ' + class_id)
    paths = tuple(repo / 'build' / (class_id + suffix) if suffix is not None else None
                  for suffix in PACK_SUFFIXES[class_id])
    selection_cache = {}
    for path in paths:
        if path is None: continue
        if not (path / 'atlas-review.json').is_file():
            raise FileNotFoundError('Missing source-pose pack: ' + str(path))
        for manifest_path in (path / 'atlas-review.json', *sorted(path.glob('*-review/atlas-review.json'))):
            manifest = json.loads(manifest_path.read_text())
            if manifest.get('status') in ('SOURCE_REJECTED', 'FIX_REQUIRED'):
                raise ValueError(f'Nguồn review đã bị loại, cần sửa source trước khi mở Player: {manifest_path}')
            validate_source_selection(manifest_path, manifest, selection_cache)
            for pose, scale in manifest.get('poseScaleCorrections', {}).items():
                if type(scale) not in (int, float) or scale != 1:
                    raise ValueError(
                        f'Nguồn review đổi tỷ lệ riêng pose {pose} ({scale}); '
                        f'cần trả về body authority và sửa garment tại source: {manifest_path}')
    return paths


def build_class_args(repo: Path, class_ids=CLASSES) -> list[str]:
    result = []
    for class_id in class_ids:
        male, male_alt, female, female_alt = class_pack_paths(repo, class_id)
        result += ['--lgo-source-pose-class', class_id,
                   *(str(path.resolve()) if path is not None else '-'
                     for path in (male, male_alt, female, female_alt))]
    return result


def build_player_command(player: Path, repo: Path, log: Path) -> list[str]:
    """Build the sole owner-review command: registered source poses only.

    Legacy ``--lgo-*-review`` flags select the static class-fit renderer.  They
    are deliberately absent here so the interactive review cannot silently
    switch to a second character representation.
    """
    executable = resolve_player(player)
    first = class_pack_paths(repo, CLASSES[0])
    command = [str(executable), '-logFile', str(log), '-screen-fullscreen', '0',
               '-screen-width', '1440', '-screen-height', '900', '--lgo-map01a-art-preview',
               '--lgo-vo-registered', '--lgo-vo-registered-equipment',
               *[arg for flag, path in zip(
                   ('--lgo-vo-pose-review-dir', '--lgo-vo-pose-review-alt-dir',
                    '--lgo-vo-pose-review-female-dir', '--lgo-vo-pose-review-female-alt-dir'), first)
                 if path is not None for arg in (flag, str(path.resolve()))],
               *build_class_args(repo)]
    legacy = {'--lgo-kiem-review', '--lgo-phap-review', '--lgo-co-review', '--lgo-linh-review'}
    if legacy.intersection(command):
        raise RuntimeError('Owner review command selected the revoked static-fit renderer')
    return command


def main(argv=None) -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--player', type=Path, required=True)
    parser.add_argument('--repo', type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument('--log', type=Path)
    args = parser.parse_args(argv)
    repo = args.repo.resolve()
    log = (args.log or repo / 'build/source-pose-class-switch-player-v1/interactive-player.log').resolve()
    log.parent.mkdir(parents=True, exist_ok=True)
    command = build_player_command(args.player, repo, log)
    process = subprocess.Popen(command, cwd=resolve_player(args.player).parent, start_new_session=True)
    print(f'LGO_SOURCE_POSE_REVIEW_STARTED pid={process.pid} classes={",".join(CLASSES)} log={log}')


if __name__ == '__main__':
    main()
