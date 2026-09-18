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
CHARACTER_HUB_CANONICAL_WIDTH = 1098.0
CHARACTER_HUB_CANONICAL_HEIGHT = 724.0
CHARACTER_HUB_HEIGHT_OCCUPANCY = {
    "pc": CHARACTER_HUB_CANONICAL_HEIGHT / 941.0,
    "tablet": 0.72,
    "mobile": 0.62,
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


def validate_skill_node(node: int) -> None:
    if type(node) is not int or not 0 <= node <= 8:
        raise ValueError("Skill capture node must be an integer from 0 to 8")


def build_player_command(
    player: Path, out: Path, profile: str, width: int, height: int, skill_node: int = 5
) -> list[str]:
    validate_skill_node(skill_node)
    return [
        str(player),
        "-logFile", str(out / "player.log"),
        "-screen-fullscreen", "0",
        "-screen-width", str(width),
        "-screen-height", str(height),
        "--lgo-map01a-device", profile,
        "--lgo-map01a-inventory-tabs-capture",
        "--lgo-map01a-art-dir", str(out),
        "--lgo-character-hub-skill-node", str(skill_node),
    ]


def expected_evidence_authority(profile: str) -> str:
    return "macos-player" if profile == "pc" else "macos-aspect-simulation"


def validate_ui_metrics(manifest: dict, profile: str) -> list[str]:
    errors: list[str] = []
    width, height = PROFILES[profile]
    expected_authority = expected_evidence_authority(profile)
    if manifest.get("evidenceAuthority") != expected_authority:
        errors.append("EVIDENCE_AUTHORITY_MISMATCH")
    metrics = manifest.get("uiMetrics")
    if not isinstance(metrics, dict):
        return errors + ["MISSING_UI_METRICS"]
    if metrics.get("evidenceAuthority") != expected_authority:
        errors.append("UI_METRICS_AUTHORITY_MISMATCH")
    if (metrics.get("screenWidth"), metrics.get("screenHeight")) != (width, height):
        errors.append("UI_METRICS_SCREEN_MISMATCH")
    panel_width, panel_height = metrics.get("panelWidth"), metrics.get("panelHeight")
    if not isinstance(panel_width, int) or panel_width <= 0 or not isinstance(panel_height, int) or panel_height <= 0:
        errors.append("UI_METRICS_PANEL_INVALID")
    panel_settings = metrics.get("panelSettings", "")
    if "referenceResolution=1672x941" not in panel_settings or "match=1" not in panel_settings:
        errors.append("UI_PANEL_POLICY_MISMATCH")
    actual_shell_width = float(metrics.get("characterHubShellWidth", 0))
    actual_shell_height = float(metrics.get("characterHubShellHeight", 0))
    if actual_shell_height <= 0:
        errors.append("CHARACTER_HUB_SHELL_METRICS_MISMATCH")
    else:
        expected_occupancy = CHARACTER_HUB_HEIGHT_OCCUPANCY[profile]
        expected_shell_height = min(
            CHARACTER_HUB_CANONICAL_HEIGHT,
            float(panel_height) * expected_occupancy,
        )
        expected_shell_width = (
            expected_shell_height * CHARACTER_HUB_CANONICAL_WIDTH / CHARACTER_HUB_CANONICAL_HEIGHT
        )
        if abs(actual_shell_width - expected_shell_width) > 2.0 or abs(actual_shell_height - expected_shell_height) > 2.0:
            errors.append("CHARACTER_HUB_SHELL_METRICS_MISMATCH")
        if abs(
            actual_shell_width / actual_shell_height
            - CHARACTER_HUB_CANONICAL_WIDTH / CHARACTER_HUB_CANONICAL_HEIGHT
        ) > 0.01:
            errors.append("CHARACTER_HUB_SHELL_ASPECT_MISMATCH")
        reported_ratio = float(metrics.get("characterHubShellScreenHeightRatio", 0))
        actual_ratio = actual_shell_height / float(panel_height)
        if abs(reported_ratio - actual_ratio) > 0.002 or abs(actual_ratio - expected_shell_height / float(panel_height)) > 0.005:
            errors.append("CHARACTER_HUB_OCCUPANCY_INVALID")
        # Legacy evidence field only: it must describe the measured Hub shell,
        # never drive the responsive policy.
        presentation_scale = float(metrics.get("presentationScale", 0))
        measured_shell_scale = actual_shell_height / CHARACTER_HUB_CANONICAL_HEIGHT
        if abs(presentation_scale - measured_shell_scale) > 0.003:
            errors.append("PRESENTATION_SCALE_METRIC_MISMATCH")
    if metrics.get("minimumTouchTargetPanelUnits") != 44:
        errors.append("TOUCH_TARGET_TOKEN_MISMATCH")
    if float(metrics.get("minimumTouchTargetScreenPixels", 0)) <= 0:
        errors.append("TOUCH_TARGET_SCREEN_METRIC_INVALID")
    return errors


def validate_manifest(manifest: dict, out: Path, profile: str, skill_node: int = 5) -> list[str]:
    validate_skill_node(skill_node)
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
    errors.extend(validate_ui_metrics(manifest, profile))
    if tuple(manifest.get("potentialClassProfiles", ())) != CHARACTER_HUB_CLASS_IDS:
        errors.append("POTENTIAL_CLASS_PROFILE_MISMATCH")
    if type(manifest.get("skillSelectedNodeIndex")) is not int or manifest["skillSelectedNodeIndex"] != skill_node:
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


def capture_profile(player: Path, out: Path, profile: str, timeout: int, skill_node: int = 5) -> None:
    width, height = PROFILES[profile]
    out.mkdir(parents=True, exist_ok=False)
    command = build_player_command(player, out, profile, width, height, skill_node)
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
    errors = validate_manifest(manifest, out, profile, skill_node)
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
    parser.add_argument("--skill-node", type=int, choices=range(9), default=5)
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
        capture_profile(player, out, profile, args.timeout, args.skill_node)
    print("LGO_CHARACTER_HUB_THREE_VIEWPORT_CAPTURE_COMPLETE")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (RuntimeError, subprocess.TimeoutExpired, json.JSONDecodeError) as exc:
        print("FIX_REQUIRED: " + str(exc), file=sys.stderr)
        raise SystemExit(1)
