#!/usr/bin/env python3.12
"""Mở candidate quảng trường Đá Luyện trong bản Player đã build."""
import argparse
from pathlib import Path
import subprocess


def main():
    root = Path(__file__).resolve().parents[1]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--app', type=Path, default=root / 'build/unity-player-macos/LinhGioiOnline.app')
    args = parser.parse_args()
    if not args.app.is_dir():
        parser.error('Chưa có Player: build bằng M0LinuxPlayerEvidenceBuilder.BuildMacOSPlayerSmoke trước.')
    subprocess.run(['open', '-n', str(args.app.resolve()), '--args', '--lgo-training-square',
                    '--lgo-device-profile', 'desktop', '-screen-fullscreen', '0',
                    '-screen-width', '1920', '-screen-height', '1080'], check=True)


if __name__ == '__main__':
    main()
