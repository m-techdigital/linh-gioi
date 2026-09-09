#!/usr/bin/env python3
from __future__ import annotations

from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
IMAGE_SUFFIXES = {
    '.png', '.jpg', '.jpeg', '.webp', '.bmp', '.gif', '.psd', '.tga', '.tif', '.tiff'
}
SKIP_PARTS = {
    '.git', 'build', 'Library', 'Temp', 'Obj', 'Logs', 'UserSettings', 'Packages'
}


def main() -> int:
    found: list[str] = []
    for path in ROOT.rglob('*'):
        if not path.is_file():
            continue
        rel = path.relative_to(ROOT)
        if any(part in SKIP_PARTS for part in rel.parts):
            continue
        if path.suffix.lower() in IMAGE_SUFFIXES:
            found.append(rel.as_posix())

    if found:
        print('LGO_2D_BRANCH_NO_SOURCE_IMAGES_FAIL')
        for rel in sorted(found)[:240]:
            print(rel)
        if len(found) > 240:
            print(f'... and {len(found) - 240} more')
        return 1

    print('LGO_2D_BRANCH_NO_SOURCE_IMAGES_PASS')
    return 0


if __name__ == '__main__':
    sys.exit(main())
