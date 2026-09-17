#!/usr/bin/env python3
"""Capture deterministic Product Auth Login visual states from a real macOS Player."""
from __future__ import annotations

import argparse
import json
import shutil
import subprocess
import sys
from pathlib import Path

try:
    from tools.capture_lgo_whole_flow_p0 import png_dimensions
except ImportError:
    from capture_lgo_whole_flow_p0 import png_dimensions

ROOT = Path(__file__).resolve().parents[1]
PROFILES = {"pc": (1600, 900), "tablet": (1024, 768), "mobile": (1600, 720)}
REQUIRED_FRAMES = (
    "entry-login.png",
    "entry-login-validation.png",
    "entry-login-authenticating.png",
    "entry-login-invalid-credentials.png",
    "entry-login-success-character-select.png",
)


def build_player_command(player: Path, out: Path, profile: str) -> list[str]:
    width, height = PROFILES[profile]
    return [
        str(player), "-logFile", str(out / "player.log"),
        "-screen-fullscreen", "0", "-screen-width", str(width), "-screen-height", str(height),
        "--lgo-map01a-device", profile,
        "--lgo-map01a-entry-capture",
        "--lgo-product-auth-states-capture",
        "--lgo-map01a-art-dir", str(out),
    ]


def validate_profile(out: Path, profile: str) -> list[str]:
    errors: list[str] = []
    manifest_path = out / "manifest.json"
    if not manifest_path.is_file():
        return ["MISSING_MANIFEST"]
    try:
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError:
        return ["INVALID_MANIFEST_JSON"]
    width, height = PROFILES[profile]
    if manifest.get("status") != "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED":
        errors.append("CAPTURE_STATUS_INVALID")
    if manifest.get("captureScope") != "map01a-entry-login":
        errors.append("CAPTURE_SCOPE_INVALID")
    if manifest.get("usesOsMouseOrKeyboard") is not False:
        errors.append("OS_INPUT_USED")
    if (manifest.get("width"), manifest.get("height")) != (width, height):
        errors.append("VIEWPORT_MISMATCH")
    if manifest.get("validationFrame") != "entry-login-validation.png":
        errors.append("VALIDATION_FRAME_MISMATCH")
    if tuple(manifest.get("productAuthFrames", ())) != REQUIRED_FRAMES[2:]:
        errors.append("PRODUCT_AUTH_FRAME_LIST_MISMATCH")
    for frame in REQUIRED_FRAMES:
        path = out / frame
        if not path.is_file() or path.stat().st_size == 0:
            errors.append("MISSING_FRAME:" + frame)
            continue
        try:
            dimensions = png_dimensions(path)
        except (ValueError, OSError):
            errors.append("INVALID_PNG:" + frame)
            continue
        if dimensions != (width, height):
            errors.append("PNG_DIMENSION_MISMATCH:" + frame)
    return errors


def capture_profile(player: Path, out: Path, profile: str, timeout: int, replace: bool) -> None:
    if out.exists():
        if not replace:
            raise RuntimeError("Evidence directory already exists: " + str(out))
        shutil.rmtree(out)
    out.mkdir(parents=True)
    command = build_player_command(player, out, profile)
    with (out / "launch.log").open("w", encoding="utf-8") as log:
        result = subprocess.run(
            command, cwd=player.parent, stdout=log, stderr=subprocess.STDOUT,
            timeout=timeout, check=False,
        )
    if result.returncode != 0:
        raise RuntimeError(f"{profile}: Player exited {result.returncode}; see {out / 'player.log'}")
    errors = validate_profile(out, profile)
    if errors:
        raise RuntimeError(f"{profile}: " + ", ".join(errors))
    width, height = PROFILES[profile]
    print(f"LGO_PRODUCT_AUTH_CAPTURE_PASS profile={profile} viewport={width}x{height} frames={len(REQUIRED_FRAMES)}")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--player", type=Path, required=True)
    parser.add_argument("--out-dir", type=Path, required=True)
    parser.add_argument("--profile", choices=("all", *PROFILES), default="all")
    parser.add_argument("--timeout", type=int, default=120)
    parser.add_argument("--replace", action="store_true")
    args = parser.parse_args()
    player = args.player.expanduser().resolve()
    out_root = args.out_dir.expanduser().resolve()
    if not player.is_file() or player.parent.name != "MacOS":
        parser.error("--player must point to a macOS Player executable")
    profiles = tuple(PROFILES) if args.profile == "all" else (args.profile,)
    out_root.mkdir(parents=True, exist_ok=True)
    for profile in profiles:
        capture_profile(player, out_root / profile, profile, args.timeout, args.replace)
    if args.profile == "all":
        print("LGO_PRODUCT_AUTH_THREE_VIEWPORT_CAPTURE_COMPLETE")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (RuntimeError, subprocess.TimeoutExpired, json.JSONDecodeError) as exc:
        print("FIX_REQUIRED: " + str(exc), file=sys.stderr)
        raise SystemExit(1)
