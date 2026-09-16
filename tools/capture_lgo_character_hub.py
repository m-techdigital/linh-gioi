#!/usr/bin/env python3
"""Capture every Character Hub tab at the three real review viewports.

The Player drives tab selection and screenshots internally, so this tool never
uses the owner's mouse or keyboard.  Each profile is validated before its
evidence can be accepted.
"""
from __future__ import annotations

import argparse
import json
import shutil
import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
PROFILES = {
    "pc": (1600, 900),
    "tablet": (1024, 768),
    "mobile": (1600, 720),
}
CHARACTER_HUB_CLASS_IDS = ("vo", "kiem", "phap", "co", "linh")
POTENTIAL_CLASS_FRAMES = tuple(
    f"potential-{class_id}-{state}.png"
    for class_id in CHARACTER_HUB_CLASS_IDS
    for state in ("default", "selected")
)
SKILL_CLASS_FRAMES = tuple(
    frame for class_id in CHARACTER_HUB_CLASS_IDS
    for frame in (f"skills-{class_id}.png", f"skills-{class_id}-selected.png")
)
SPIRIT_PET_CLASS_FRAMES = tuple(
    f"spirit-pet-{class_id}.png" for class_id in CHARACTER_HUB_CLASS_IDS
)
EQUIPMENT_SLOTS = ("main_weapon", "head_hair", "inner_top", "outer_tunic", "lower_garment",
                   "waist", "arm_guard", "boots", "light_armor", "accessory")
REQUIRED_FRAMES = (
    "character-info.png",
    "bag.png",
    "bag-search-binh-mau.png",
    "bag-search-binh-mau-selected.png",
    "skills-default.png",
    "skills.png",
    *(f"item-{slot}-selected.png" for slot in EQUIPMENT_SLOTS),
    *SKILL_CLASS_FRAMES,
    "potential-default.png",
    "potential.png",
    *POTENTIAL_CLASS_FRAMES,
    "spirit-pet.png",
    *SPIRIT_PET_CLASS_FRAMES,
)


def build_player_command(
    player: Path, out: Path, profile: str, width: int, height: int
) -> list[str]:
    return [
        str(player),
        "-logFile", str(out / "player.log"),
        "-screen-fullscreen", "0",
        "-screen-width", str(width),
        "-screen-height", str(height),
        "--lgo-map01a-device", profile,
        "--lgo-map01a-inventory-tabs-capture",
        "--lgo-map01a-art-dir", str(out),
    ]


def validate_manifest(manifest: dict, out: Path, profile: str) -> list[str]:
    errors: list[str] = []
    width, height = PROFILES[profile]
    if (manifest.get("width"), manifest.get("height")) != (width, height):
        errors.append("VIEWPORT_MISMATCH")
    if manifest.get("status") != "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED":
        errors.append("CAPTURE_STATUS_INVALID")
    if manifest.get("captureScope") != "map01a-inventory-tabs":
        errors.append("CAPTURE_SCOPE_INVALID")
    if manifest.get("usesOsMouseOrKeyboard") is not False:
        errors.append("OS_INPUT_USED")
    if tuple(manifest.get("potentialClassProfiles", ())) != CHARACTER_HUB_CLASS_IDS:
        errors.append("POTENTIAL_CLASS_PROFILE_MISMATCH")
    if manifest.get("skillSelectedNodeIndex") != 5:
        errors.append("SKILL_SELECTED_NODE_MISMATCH")
    if tuple(manifest.get("skillClassProfiles", ())) != CHARACTER_HUB_CLASS_IDS:
        errors.append("SKILL_CLASS_PROFILE_MISMATCH")
    if tuple(manifest.get("spiritPetClassProfiles", ())) != CHARACTER_HUB_CLASS_IDS:
        errors.append("SPIRIT_PET_CLASS_PROFILE_MISMATCH")
    if manifest.get("classSwitchScope") != "character-hub-data-only-no-renderer-change":
        errors.append("CLASS_SWITCH_SCOPE_INVALID")
    if tuple(manifest.get("frames", ())) != REQUIRED_FRAMES:
        errors.append("FRAME_LIST_MISMATCH")
    for frame in REQUIRED_FRAMES:
        path = out / frame
        if not path.is_file() or path.stat().st_size == 0:
            errors.append("MISSING_FRAME:" + frame)
    return errors


def capture_profile(player: Path, out: Path, profile: str, timeout: int) -> None:
    width, height = PROFILES[profile]
    out.mkdir(parents=True, exist_ok=False)
    command = build_player_command(player, out, profile, width, height)
    with (out / "launch.log").open("w", encoding="utf-8") as launch_log:
        result = subprocess.run(
            command,
            cwd=player.parent,
            stdout=launch_log,
            stderr=subprocess.STDOUT,
            timeout=timeout,
            check=False,
        )
    if result.returncode != 0:
        raise RuntimeError(f"{profile}: Player exited {result.returncode}; see {out / 'player.log'}")
    manifest_path = out / "manifest.json"
    if not manifest_path.is_file():
        raise RuntimeError(f"{profile}: missing {manifest_path}")
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    errors = validate_manifest(manifest, out, profile)
    if errors:
        raise RuntimeError(f"{profile}: " + ", ".join(errors))
    print(
        f"LGO_CHARACTER_HUB_CAPTURE_PASS profile={profile} "
        f"viewport={width}x{height} frames={len(REQUIRED_FRAMES)}"
    )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--player",
        type=Path,
        required=True,
        help="Path to LinhGioiOnline.app/Contents/MacOS/Unity",
    )
    parser.add_argument("--out-dir", type=Path, required=True)
    parser.add_argument("--profile", choices=("all", *PROFILES), default="all")
    parser.add_argument("--timeout", type=int, default=120)
    parser.add_argument(
        "--replace",
        action="store_true",
        help="Replace only the selected generated evidence directory.",
    )
    args = parser.parse_args()
    player = args.player.expanduser().resolve()
    out_root = args.out_dir.expanduser().resolve()
    if not player.is_file() or player.parent.name != "MacOS":
        parser.error("--player must point to a macOS Player executable")
    selected = tuple(PROFILES) if args.profile == "all" else (args.profile,)
    if args.profile == "all" and args.replace and out_root.exists():
        shutil.rmtree(out_root)
    out_root.mkdir(parents=True, exist_ok=True)
    for profile in selected:
        out = out_root / profile
        if args.replace and out.exists():
            shutil.rmtree(out)
        capture_profile(player, out, profile, args.timeout)
    print("LGO_CHARACTER_HUB_THREE_VIEWPORT_CAPTURE_COMPLETE")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (RuntimeError, subprocess.TimeoutExpired, json.JSONDecodeError) as exc:
        print("FIX_REQUIRED: " + str(exc), file=sys.stderr)
        raise SystemExit(1)
