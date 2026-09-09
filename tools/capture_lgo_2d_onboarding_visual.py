#!/usr/bin/env python3
from __future__ import annotations

import argparse
import os
import shutil
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_PLAYER_EXE = ROOT / "build/2d-onboarding-player/LinhGioiOnline2D.app/Contents/MacOS/Unity"
DEFAULT_OUT_DIR = ROOT / "build/2d-onboarding-visual"


def main() -> int:
    parser = argparse.ArgumentParser(description="Capture Linh Giới 2D onboarding Player screenshots.")
    parser.add_argument("--player", default=str(DEFAULT_PLAYER_EXE), help="Path to macOS Player executable inside the .app bundle.")
    parser.add_argument("--out-dir", default=str(DEFAULT_OUT_DIR), help="Evidence output directory.")
    parser.add_argument("--width", type=int, default=1280)
    parser.add_argument("--height", type=int, default=720)
    parser.add_argument("--timeout", type=int, default=90)
    parser.add_argument("--keep", action="store_true", help="Keep existing output directory contents before capture.")
    args = parser.parse_args()

    player = Path(args.player).expanduser().resolve()
    out_dir = Path(args.out_dir).expanduser().resolve()
    if not player.is_file() or not os.access(player, os.X_OK):
        print(f"RUNTIME_BLOCKED_ENV missing executable Player: {player}", file=sys.stderr)
        return 32
    if player.parent.name != "MacOS":
        print(f"FIX_REQUIRED expected Player executable under Contents/MacOS: {player}", file=sys.stderr)
        return 2

    if out_dir.exists() and not args.keep:
        shutil.rmtree(out_dir)
    out_dir.mkdir(parents=True, exist_ok=True)

    manifest = out_dir / "twod-onboarding-visual-manifest.json"
    command = [
        str(player),
        "-logFile", str(out_dir / "player-capture.log"),
        "-screen-fullscreen", "0",
        "-screen-width", str(args.width),
        "-screen-height", str(args.height),
        "--lgo-2d-onboarding-visual-capture",
        "--lgo-2d-visual-dir", str(out_dir),
    ]
    env = os.environ.copy()
    env["LGO_2D_ONBOARDING_VISUAL_CAPTURE"] = "1"

    # macOS Unity Player bundles must be launched from Contents/MacOS for command-line
    # evidence runs to enter the scene lifecycle reliably. Launching from the repo root
    # can initialize the engine and then exit before GameBootstrap writes artifacts.
    try:
        result = subprocess.run(
            command,
            cwd=str(player.parent),
            env=env,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
            timeout=args.timeout,
            check=False,
        )
    except subprocess.TimeoutExpired:
        print(f"VISUAL_CAPTURE_TIMEOUT timeout={args.timeout} out_dir={out_dir}", file=sys.stderr)
        log_path = out_dir / "player-capture.log"
        if log_path.is_file():
            print(log_path.read_text(encoding="utf-8", errors="replace")[-4000:], file=sys.stderr)
        return 41

    if result.stdout:
        print(result.stdout[-3000:])
    if result.returncode != 0:
        print(f"FIX_REQUIRED Player visual capture exited code={result.returncode}; log={out_dir / 'player-capture.log'}", file=sys.stderr)
        return result.returncode
    if not manifest.is_file():
        print(f"FIX_REQUIRED visual manifest missing: {manifest}", file=sys.stderr)
        return 40

    print(f"LGO_2D_ONBOARDING_VISUAL_CAPTURE_PASS manifest={manifest}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
