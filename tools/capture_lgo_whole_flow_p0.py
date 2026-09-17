#!/usr/bin/env python3
"""Capture the current Map01A product flow from one fresh macOS Player."""
from __future__ import annotations

import argparse
import json
import shutil
import subprocess
import sys
import time
from dataclasses import dataclass
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROFILES = {
    "pc": (1600, 900),
    "tablet": (1024, 768),
    "mobile": (1600, 720),
}
FLOW_COMPONENTS = ("quest", "character-hub")


@dataclass(frozen=True)
class ScreenCaptureSpec:
    flag: str
    scope: str
    frames: tuple[str, ...]
    capture_validation: bool = False
    product_state_frames: tuple[str, ...] = ()
SCREEN_CAPTURES = {
    "entry": ScreenCaptureSpec(
        "--lgo-map01a-entry-capture", "map01a-entry-login",
        ("entry-login.png", "entry-login-validation.png"), True),
    "server-select": ScreenCaptureSpec(
        "--lgo-map01a-server-select-capture", "map01a-server-select",
        ("server-select.png",)),
    "register": ScreenCaptureSpec(
        "--lgo-map01a-register-capture", "map01a-register",
        ("register-account.png", "register-validation.png", "register-loading.png", "register-conflict.png"),
        True, ("register-loading.png", "register-conflict.png")),
    "password-recovery": ScreenCaptureSpec(
        "--lgo-map01a-password-recovery-capture", "map01a-password-recovery-request",
        ("password-recovery-request.png", "password-recovery-validation.png",
         "password-recovery-loading.png", "password-recovery-unavailable.png"),
        True, ("password-recovery-loading.png", "password-recovery-unavailable.png")),
    "password-recovery-verify": ScreenCaptureSpec(
        "--lgo-map01a-password-recovery-verify-capture", "map01a-password-recovery-verify",
        ("password-recovery-verify.png", "password-recovery-verify-invalid-expired.png"),
        False, ("password-recovery-verify-invalid-expired.png",)),
    "password-recovery-new-password": ScreenCaptureSpec(
        "--lgo-map01a-password-recovery-new-password-capture", "map01a-password-recovery-new-password",
        ("password-recovery-new-password.png", "password-recovery-new-password-rule.png"),
        False, ("password-recovery-new-password-rule.png",)),
    "character-select": ScreenCaptureSpec(
        "--lgo-map01a-character-select-capture", "map01a-character-select",
        ("character-select.png",)),
    "character-entry": ScreenCaptureSpec(
        "--lgo-map01a-character-entry-capture", "map01a-character-entry",
        ("character-select.png", "map01a-entry.png")),
    "menu": ScreenCaptureSpec(
        "--lgo-map01a-menu-capture", "map01a-menu", ("menu.png",)),
}

QUEST_FRAMES = (
    "01-arrival-q01", "02-ha-van-dialogue", "03-q01-complete", "04-q02-grand-gate",
    "05-quan-thu-dialogue", "06-q03-complete", "07-q04-inventory-open", "08-q04-tong-phu-dialogue",
    "09-q04-starter-supplies", "10-q04-health-potion-used", "11-q05-thanh-nhi",
    "12-q05-spirit-herb", "13-q08-hidden-chest", "14-q06-lao-tran", "15-q06-combat",
    "16-q07-class-loot", "17-q07-class-item-equipped", "18-q09-portal-open",
)
REPRESENTATIVE_QUEST_FRAMES = (
    "01-arrival-q01.png", "02-ha-van-dialogue.png", "15-q06-combat.png",
)
REPRESENTATIVE_HUB_FRAMES = (
    "character-info.png", "bag.png", "skills.png", "potential.png", "spirit-pet.png",
)
def build_screen_command(player: Path, out: Path, profile: str, screen: str) -> list[str]:
    if profile not in PROFILES:
        raise ValueError("Unknown profile: " + profile)
    if screen not in SCREEN_CAPTURES:
        raise ValueError("Unknown screen: " + screen)
    width, height = PROFILES[profile]
    spec = SCREEN_CAPTURES[screen]
    command = [
        str(player), "-logFile", str(out / "player.log"),
        "-screen-fullscreen", "0", "-screen-width", str(width), "-screen-height", str(height),
        "--lgo-map01a-device", profile, spec.flag,
        "--lgo-map01a-art-dir", str(out),
    ]
    if spec.capture_validation:
        command.append("--lgo-map01a-auth-validation-capture")
    if spec.product_state_frames:
        command.append("--lgo-product-account-states-capture")
    return command


def prepare_profile_directory(profile_dir: Path, replace: bool) -> None:
    if profile_dir.exists():
        if not replace:
            raise FileExistsError("Profile evidence already exists: " + str(profile_dir))
        shutil.rmtree(profile_dir)
    profile_dir.mkdir(parents=True)


def validate_required_frames(root: Path, frames: tuple[str, ...], started_ns: int) -> list[str]:
    errors: list[str] = []
    for frame in frames:
        path = root / frame
        if not path.is_file() or path.stat().st_size == 0:
            errors.append("MISSING_FRAME:" + frame)
        elif path.stat().st_mtime_ns < started_ns:
            errors.append("STALE_FRAME:" + frame)
    return errors
def _read_manifest(path: Path) -> dict:
    if not path.is_file():
        raise RuntimeError("Missing manifest: " + str(path))
    return json.loads(path.read_text(encoding="utf-8"))


def capture_screen(player: Path, profile_dir: Path, profile: str, screen: str, timeout: int,
                   started_ns: int) -> dict:
    out = profile_dir / screen
    out.mkdir()
    command = build_screen_command(player, out, profile, screen)
    with (out / "launch.log").open("w", encoding="utf-8") as log:
        result = subprocess.run(command, cwd=player.parent, stdout=log, stderr=subprocess.STDOUT,
                                timeout=timeout, check=False)
    if result.returncode != 0:
        raise RuntimeError(f"{profile}/{screen}: Player exited {result.returncode}")
    manifest = _read_manifest(out / "manifest.json")
    spec = SCREEN_CAPTURES[screen]
    width, height = PROFILES[profile]
    if manifest.get("status") != "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED":
        raise RuntimeError(f"{profile}/{screen}: capture status invalid")
    if manifest.get("captureScope") != spec.scope:
        raise RuntimeError(f"{profile}/{screen}: capture scope mismatch")
    if (manifest.get("width"), manifest.get("height")) != (width, height):
        raise RuntimeError(f"{profile}/{screen}: viewport mismatch")
    if manifest.get("usesOsMouseOrKeyboard") is not False:
        raise RuntimeError(f"{profile}/{screen}: OS input claim invalid")
    errors = validate_required_frames(out, spec.frames, started_ns)
    if errors:
        raise RuntimeError(f"{profile}/{screen}: " + ", ".join(errors))
    if spec.capture_validation and manifest.get("validationFrame") != spec.frames[1]:
        raise RuntimeError(f"{profile}/{screen}: validation frame contract missing")
    if spec.product_state_frames and manifest.get("productAccountFrames") != list(spec.product_state_frames):
        raise RuntimeError(f"{profile}/{screen}: product-account state frame contract missing")
    _validate_png_sizes(out, spec.frames, (width, height))
    return manifest
def build_quest_command(player: Path, out: Path, profile: str) -> list[str]:
    if profile not in PROFILES:
        raise ValueError("Unknown profile: " + profile)
    width, height = PROFILES[profile]
    return [
        str(player), "-logFile", str(out / "player.log"),
        "-screen-fullscreen", "0", "-screen-width", str(width), "-screen-height", str(height),
        "--lgo-map01a-device", profile, "--lgo-map01a-art-capture",
        "--lgo-map01a-art-dir", str(out), "--lgo-map01a-quest-only",
    ]


def capture_quest_flow(player: Path, profile_dir: Path, profile: str, timeout: int,
                       started_ns: int) -> None:
    out = profile_dir / "quest"
    out.mkdir()
    command = build_quest_command(player, out, profile)
    with (out / "launch.log").open("w", encoding="utf-8") as log:
        result = subprocess.run(command, cwd=player.parent, stdout=log, stderr=subprocess.STDOUT,
                                timeout=timeout, check=False)
    if result.returncode != 0:
        raise RuntimeError(f"{profile}/quest: Player exited {result.returncode}")
    manifest = _read_manifest(out / "manifest.json")
    width, height = PROFILES[profile]
    required_true = (
        "mapQuestFlowVerified", "functionalUiVerified", "dialogueOpened", "greetingCompleted",
        "dialogueRevisitsVerified", "questWorldFramesUnobstructed",
    )
    if (manifest.get("status") != "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED"
            or manifest.get("captureScope") != "map-quests-q01-q09"
            or manifest.get("frames") != 18
            or (manifest.get("width"), manifest.get("height")) != (width, height)
            or not all(manifest.get(key) for key in required_true)):
        raise RuntimeError(f"{profile}/quest: manifest contract failed")
    for frame in QUEST_FRAMES:
        bmp = out / (frame + ".bmp")
        png = out / (frame + ".png")
        if not bmp.is_file() or bmp.stat().st_size == 0:
            raise RuntimeError(f"{profile}/quest: missing BMP {frame}")
        subprocess.run(["sips", "-s", "format", "png", str(bmp), "--out", str(png)],
                       check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    errors = validate_required_frames(out, tuple(frame + ".png" for frame in QUEST_FRAMES), started_ns)
    if errors:
        raise RuntimeError(f"{profile}/quest: " + ", ".join(errors))
    _validate_png_sizes(out, tuple(frame + ".png" for frame in QUEST_FRAMES), (width, height))


def capture_character_hub(player: Path, profile_dir: Path, profile: str, timeout: int,
                          started_ns: int) -> None:
    try:
        from tools.capture_lgo_character_hub import capture_profile
    except ImportError:
        from capture_lgo_character_hub import capture_profile
    out = profile_dir / "character-hub"
    capture_profile(player, out, profile, timeout)
    errors = validate_required_frames(out, REPRESENTATIVE_HUB_FRAMES, started_ns)
    if errors:
        raise RuntimeError(f"{profile}/character-hub: " + ", ".join(errors))
    _validate_png_sizes(out, REPRESENTATIVE_HUB_FRAMES, PROFILES[profile])

def png_dimensions(path: Path) -> tuple[int, int]:
    import struct
    raw = path.read_bytes()[:24]
    if len(raw) < 24 or raw[:8] != b"\x89PNG\r\n\x1a\n" or raw[12:16] != b"IHDR":
        raise ValueError("Invalid PNG header: " + str(path))
    return struct.unpack(">II", raw[16:24])


def _validate_png_sizes(root: Path, frames: tuple[str, ...], expected: tuple[int, int]) -> None:
    bad = [frame for frame in frames if png_dimensions(root / frame) != expected]
    if bad:
        raise RuntimeError("PNG_DIMENSION_MISMATCH:" + ",".join(bad))


def profile_required_frames() -> tuple[str, ...]:
    frames: list[str] = []
    for screen, spec in SCREEN_CAPTURES.items():
        frames.extend(f"{screen}/{frame}" for frame in spec.frames)
    frames.extend(f"quest/{frame}" for frame in REPRESENTATIVE_QUEST_FRAMES)
    frames.extend(f"character-hub/{frame}" for frame in REPRESENTATIVE_HUB_FRAMES)
    return tuple(frames)

def capture_whole_profile(player: Path, out_root: Path, profile: str, timeout: int,
                          replace: bool = False) -> None:
    profile_dir = out_root / profile
    prepare_profile_directory(profile_dir, replace)
    started_ns = time.time_ns()
    scopes: dict[str, str] = {}
    for screen in SCREEN_CAPTURES:
        manifest = capture_screen(player, profile_dir, profile, screen, timeout, started_ns)
        scopes[screen] = manifest["captureScope"]
    capture_quest_flow(player, profile_dir, profile, timeout, started_ns)
    capture_character_hub(player, profile_dir, profile, timeout, started_ns)

    required = profile_required_frames()
    errors = validate_required_frames(profile_dir, required, started_ns)
    if errors:
        raise RuntimeError(f"{profile}: " + ", ".join(errors))
    width, height = PROFILES[profile]
    manifest = {
        "status": "TECHNICAL_PASS_VISUAL_REVIEW_REQUIRED",
        "captureScope": "whole-flow-p0",
        "profile": profile,
        "width": width,
        "height": height,
        "usesOsMouseOrKeyboard": False,
        "runStartedUnixNs": started_ns,
        "screenScopes": scopes,
        "requiredFrames": list(required),
    }
    (profile_dir / "manifest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    print(f"LGO_WHOLE_FLOW_P0_CAPTURE_PASS profile={profile} viewport={width}x{height} frames={len(required)}")

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
    out_root.mkdir(parents=True, exist_ok=True)
    selected = tuple(PROFILES) if args.profile == "all" else (args.profile,)
    for profile in selected:
        capture_whole_profile(player, out_root, profile, args.timeout, args.replace)
    if args.profile == "all":
        print("LGO_WHOLE_FLOW_P0_THREE_VIEWPORT_CAPTURE_COMPLETE")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (RuntimeError, FileExistsError, subprocess.TimeoutExpired, json.JSONDecodeError, ValueError) as exc:
        print("FIX_REQUIRED: " + str(exc), file=sys.stderr)
        raise SystemExit(1)
