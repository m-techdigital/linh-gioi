#!/usr/bin/env python3.12
"""Capture Map01A class equipment review matrices from a macOS Player.

This helper intentionally treats class equipment output as technical evidence:
slot counts and screenshots are captured, but visual approval remains a manual
review gate because draft class art can be structurally valid and still look bad.
"""
from __future__ import annotations

import argparse
import json
import plistlib
import shutil
import subprocess
from pathlib import Path

CLASS_FLAGS = {
    "kiem": "--lgo-kiem-capture",
    "phap": "--lgo-phap-capture",
    "co": "--lgo-co-capture",
    "linh": "--lgo-linh-capture",
}


def resolve_player(player: Path) -> Path:
    player = player.resolve()
    if player.is_dir() and player.suffix == ".app":
        with (player / "Contents/Info.plist").open("rb") as file:
            executable = plistlib.load(file)["CFBundleExecutable"]
        player = player / "Contents/MacOS" / executable
    if not player.is_file():
        raise FileNotFoundError(player)
    return player


def build_command(player: Path, class_id: str, output_dir: Path, width: int = 1600, height: int = 900) -> list[str]:
    if class_id not in CLASS_FLAGS:
        raise ValueError("unsupported class: " + class_id)
    output_dir = output_dir.resolve()
    return [
        str(resolve_player(player)),
        "-logFile", str(output_dir / "player.log"),
        "-screen-fullscreen", "0",
        "-screen-width", str(width),
        "-screen-height", str(height),
        "--lgo-map01a-art-preview",
        CLASS_FLAGS[class_id],
        "--lgo-map01a-art-dir", str(output_dir),
    ]


def capture(player: Path, class_id: str, output_dir: Path, keep_existing: bool = False) -> dict:
    output_dir = output_dir.resolve()
    if output_dir.exists() and not keep_existing:
        shutil.rmtree(output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    command = build_command(player, class_id, output_dir)
    result = subprocess.run(command, cwd=Path(__file__).resolve().parents[1])
    manifest = output_dir / "manifest.json"
    if not manifest.is_file():
        raise RuntimeError("class capture produced no manifest; command=" + " ".join(command))
    data = json.loads(manifest.read_text())
    if data.get("status") == "PASS":
        raise RuntimeError("class capture must not claim visual PASS for draft art")
    if result.returncode != 0:
        raise RuntimeError(f"Player capture failed with exit {result.returncode}: {manifest}")
    return data


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--player", type=Path, required=True)
    parser.add_argument("--out", type=Path, required=True)
    parser.add_argument("--class", dest="classes", choices=tuple(CLASS_FLAGS), action="append", required=True)
    parser.add_argument("--keep-existing", action="store_true")
    args = parser.parse_args(argv)
    for class_id in args.classes:
        out = args.out / class_id
        data = capture(args.player, class_id, out, args.keep_existing)
        print(f"LGO_CLASS_CAPTURE {class_id} status={data.get('status')} frames={data.get('frames')} out={out.resolve()}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
