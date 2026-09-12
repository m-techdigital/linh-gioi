#!/usr/bin/env python3.12
"""Open one interactive Player with a single switchable source-pose actor."""
import argparse
import plistlib
import subprocess
from pathlib import Path

CLASSES = ('kiem', 'phap', 'co', 'linh')
PACK_SUFFIXES = {
    'kiem': ('-source-pose-review-v1/pack', '-source-pose-review-lv10-v1/pack',
             '-female-source-pose-review-v1/pack', '-female-source-pose-review-lv10-v1/pack'),
    'phap': ('-source-pose-review-canonical-v2/pack', '-source-pose-review-lv10-canonical-v2/pack',
             '-female-source-pose-review-canonical-v2/pack', '-female-source-pose-review-lv10-canonical-v2/pack'),
    'co': ('-source-pose-review-v1/pack', '-source-pose-review-lv10-semantic-v3/pack',
           '-female-source-pose-review-semantic-v3/pack', '-female-source-pose-review-lv10-semantic-v3/pack'),
    'linh': ('-source-pose-review-v1/pack', '-source-pose-review-lv10-semantic-v3/pack',
             '-female-source-pose-review-v1/pack', '-female-source-pose-review-lv10-v1/pack'),
}


def resolve_player(path: Path) -> Path:
    path = path.resolve()
    if path.is_dir() and path.suffix == '.app':
        with (path / 'Contents/Info.plist').open('rb') as file:
            executable = plistlib.load(file)['CFBundleExecutable']
        path = path / 'Contents/MacOS' / executable
    if not path.is_file():
        raise FileNotFoundError(path)
    return path


def class_pack_paths(repo: Path, class_id: str) -> tuple[Path, Path, Path, Path]:
    if class_id not in PACK_SUFFIXES:
        raise ValueError('Unsupported source-pose class: ' + class_id)
    paths = tuple(repo / 'build' / (class_id + suffix) for suffix in PACK_SUFFIXES[class_id])
    for path in paths:
        if not (path / 'atlas-review.json').is_file():
            raise FileNotFoundError('Missing source-pose pack: ' + str(path))
    return paths


def build_class_args(repo: Path, class_ids=CLASSES) -> list[str]:
    result = []
    for class_id in class_ids:
        male, male_alt, female, female_alt = class_pack_paths(repo, class_id)
        result += ['--lgo-source-pose-class', class_id, str(male.resolve()), str(male_alt.resolve()),
                   str(female.resolve()), str(female_alt.resolve())]
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
               '--lgo-vo-pose-review-dir', str(first[0].resolve()),
               '--lgo-vo-pose-review-alt-dir', str(first[1].resolve()),
               '--lgo-vo-pose-review-female-dir', str(first[2].resolve()),
               '--lgo-vo-pose-review-female-alt-dir', str(first[3].resolve()),
               *build_class_args(repo)]
    legacy = {'--lgo-kiem-review', '--lgo-phap-review', '--lgo-co-review', '--lgo-linh-review'}
    if legacy.intersection(command):
        raise RuntimeError('Owner review command selected the revoked static-fit renderer')
    return command


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--player', type=Path, required=True)
    parser.add_argument('--repo', type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument('--log', type=Path)
    args = parser.parse_args()
    repo = args.repo.resolve()
    log = (args.log or repo / 'build/source-pose-class-switch-player-v1/interactive-player.log').resolve()
    log.parent.mkdir(parents=True, exist_ok=True)
    command = build_player_command(args.player, repo, log)
    process = subprocess.Popen(command, cwd=resolve_player(args.player).parent, start_new_session=True)
    print(f'LGO_SOURCE_POSE_REVIEW_STARTED pid={process.pid} classes={",".join(CLASSES)} log={log}')


if __name__ == '__main__':
    main()
