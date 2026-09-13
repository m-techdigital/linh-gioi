#!/usr/bin/env python3.12
"""Build the shared Map01A HUD action icon atlas from deterministic vector primitives."""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
DEFAULT_OUT = ROOT / "client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AHudIcons"
RUNTIME_ATLAS_PATH = "client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AHudIcons/map01a-hud-icons.png"
SCALE = 4
CELL = 128
ICON = 96
INK = (226, 244, 255, 255)
CYAN = (64, 176, 255, 255)
GOLD = (241, 191, 88, 255)
GLOW = (22, 116, 222, 105)
ICON_IDS = (
    "run", "jump", "attack", "skill",
    "character", "inventory", "skills", "menu",
    "account", "lock", "eye", "notice",
    "support", "cinematic", "server", "crest",
)


def _scaled(points):
    return tuple(round(value * SCALE) for value in points)


def _line(draw, points, fill=INK, width=6, joint="curve"):
    draw.line([_scaled(point) for point in points], fill=fill, width=width * SCALE, joint=joint)


def _ellipse(draw, box, fill=None, outline=INK, width=5):
    draw.ellipse(_scaled(box), fill=fill, outline=outline, width=width * SCALE)


def _polygon(draw, points, fill=INK, outline=None, width=1):
    points = [_scaled(point) for point in points]
    draw.polygon(points, fill=fill)
    if outline:
        draw.line(points + [points[0]], fill=outline, width=width * SCALE, joint="curve")


def _rounded(draw, box, radius, fill=None, outline=INK, width=5):
    draw.rounded_rectangle(_scaled(box), radius=radius * SCALE, fill=fill, outline=outline, width=width * SCALE)


def _base() -> tuple[Image.Image, ImageDraw.ImageDraw]:
    image = Image.new("RGBA", (ICON * SCALE, ICON * SCALE), (0, 0, 0, 0))
    return image, ImageDraw.Draw(image)


def draw_run():
    image, draw = _base()
    _ellipse(draw, (55, 11, 71, 27), fill=GOLD, outline=INK, width=3)
    _line(draw, ((57, 31), (43, 47), (58, 59)), CYAN, 8)
    _line(draw, ((45, 45), (25, 48)), INK, 6)
    _line(draw, ((48, 38), (68, 43), (79, 34)), INK, 6)
    _line(draw, ((57, 59), (76, 72), (87, 72)), INK, 7)
    _line(draw, ((56, 59), (38, 72), (20, 82)), INK, 7)
    _line(draw, ((17, 57), (31, 57)), GOLD, 4)
    _line(draw, ((11, 67), (28, 67)), GOLD, 4)
    return image


def draw_jump():
    image, draw = _base()
    _ellipse(draw, (41, 17, 57, 33), fill=GOLD, outline=INK, width=3)
    _line(draw, ((49, 34), (49, 57)), CYAN, 8)
    _line(draw, ((49, 42), (29, 53)), INK, 6)
    _line(draw, ((49, 42), (68, 51)), INK, 6)
    _line(draw, ((49, 57), (34, 72), (24, 66)), INK, 7)
    _line(draw, ((49, 57), (65, 72), (78, 66)), INK, 7)
    _line(draw, ((23, 38), (23, 20), (16, 27)), GOLD, 4)
    _line(draw, ((23, 20), (30, 27)), GOLD, 4)
    _line(draw, ((76, 38), (76, 20), (69, 27)), GOLD, 4)
    _line(draw, ((76, 20), (83, 27)), GOLD, 4)
    return image


def draw_attack():
    image, draw = _base()
    _polygon(draw, ((20, 73), (67, 21), (75, 14), (69, 31), (29, 78)), CYAN, INK, 2)
    _polygon(draw, ((18, 71), (31, 76), (23, 85), (12, 82)), GOLD, INK, 2)
    _line(draw, ((17, 48), (34, 31), (53, 19)), GOLD, 4)
    _line(draw, ((42, 82), (64, 76), (82, 61)), INK, 4)
    return image


def draw_skill():
    image, draw = _base()
    _ellipse(draw, (10, 10, 86, 86), outline=GLOW, width=6)
    _rounded(draw, (32, 34, 66, 69), 8, fill=(25, 86, 145, 255), outline=INK, width=4)
    for x in (35, 43, 51, 59):
        _rounded(draw, (x, 21, x + 8, 47), 4, fill=CYAN, outline=INK, width=2)
    _line(draw, ((31, 49), (21, 42), (17, 50), (34, 72), (58, 79)), GOLD, 5)
    _line(draw, ((16, 18), (23, 25)), CYAN, 4)
    _line(draw, ((78, 18), (71, 25)), CYAN, 4)
    return image


def draw_character():
    image, draw = _base()
    _ellipse(draw, (35, 14, 61, 40), fill=GOLD, outline=INK, width=3)
    _line(draw, ((48, 41), (48, 52)), CYAN, 7)
    _polygon(draw, ((20, 81), (24, 63), (38, 52), (48, 61), (58, 52), (72, 63), (77, 81)), (24, 90, 150, 255), INK, 4)
    _polygon(draw, ((38, 52), (48, 61), (58, 52), (54, 71), (42, 71)), GOLD)
    return image


def draw_inventory():
    image, draw = _base()
    _line(draw, ((31, 31), (36, 19), (60, 19), (65, 31)), GOLD, 5)
    _rounded(draw, (20, 29, 76, 81), 13, fill=(23, 81, 139, 255), outline=INK, width=5)
    _line(draw, ((22, 47), (74, 47)), CYAN, 4)
    _polygon(draw, ((43, 43), (53, 43), (53, 56), (48, 62), (43, 56)), GOLD, INK, 2)
    return image


def draw_skills():
    image, draw = _base()
    _polygon(draw, ((14, 24), (43, 29), (48, 77), (18, 70)), (20, 72, 126, 255), INK, 4)
    _polygon(draw, ((82, 24), (53, 29), (48, 77), (78, 70)), (20, 72, 126, 255), INK, 4)
    _line(draw, ((48, 31), (48, 76)), GOLD, 4)
    _polygon(draw, ((48, 12), (52, 21), (62, 24), (52, 28), (48, 38), (44, 28), (34, 24), (44, 21)), CYAN)
    return image


def draw_menu():
    image, draw = _base()
    _ellipse(draw, (24, 24, 72, 72), fill=(23, 81, 139, 255), outline=INK, width=5)
    _ellipse(draw, (39, 39, 57, 57), fill=(3, 25, 50, 255), outline=GOLD, width=4)
    for points in (
        ((45, 8), (51, 8), (53, 25), (43, 25)), ((45, 71), (53, 71), (51, 88), (45, 88)),
        ((8, 45), (25, 43), (25, 53), (8, 51)), ((71, 43), (88, 45), (88, 51), (71, 53)),
        ((18, 18), (24, 14), (36, 27), (29, 34)), ((67, 62), (81, 72), (76, 80), (62, 67)),
        ((72, 16), (80, 22), (67, 36), (61, 29)), ((16, 72), (29, 61), (36, 67), (22, 80)),
    ):
        _polygon(draw, points, GOLD, INK, 1)
    return image


def draw_account():
    image, draw = _base()
    _ellipse(draw, (34, 14, 62, 42), fill=GOLD, outline=INK, width=3)
    _rounded(draw, (20, 50, 76, 82), 16, fill=(23, 81, 139, 255), outline=INK, width=5)
    _line(draw, ((48, 52), (48, 78)), CYAN, 4)
    return image


def draw_lock():
    image, draw = _base()
    _line(draw, ((31, 44), (31, 31), (37, 19), (48, 14), (59, 19), (65, 31), (65, 44)), GOLD, 6)
    _rounded(draw, (22, 40, 74, 82), 9, fill=(23, 81, 139, 255), outline=INK, width=5)
    _ellipse(draw, (43, 52, 53, 62), fill=GOLD, outline=INK, width=2)
    _line(draw, ((48, 61), (48, 72)), GOLD, 4)
    return image


def draw_eye():
    image, draw = _base()
    _line(draw, ((10, 48), (24, 32), (48, 24), (72, 32), (86, 48), (72, 64), (48, 72), (24, 64), (10, 48)), INK, 5)
    _ellipse(draw, (34, 34, 62, 62), fill=(23, 81, 139, 255), outline=CYAN, width=4)
    _ellipse(draw, (43, 43, 53, 53), fill=GOLD, outline=INK, width=2)
    return image


def draw_notice():
    image, draw = _base()
    _polygon(draw, ((16, 43), (58, 24), (58, 72), (16, 55)), (23, 81, 139, 255), INK, 4)
    _rounded(draw, (11, 42, 24, 57), 3, fill=GOLD, outline=INK, width=3)
    _polygon(draw, ((28, 57), (42, 60), (39, 80), (28, 76)), GOLD, INK, 3)
    _line(draw, ((68, 31), (82, 20)), CYAN, 4)
    _line(draw, ((70, 48), (87, 48)), CYAN, 4)
    _line(draw, ((68, 65), (82, 76)), CYAN, 4)
    return image


def draw_support():
    image, draw = _base()
    _line(draw, ((18, 52), (18, 39), (24, 24), (37, 15), (59, 15), (72, 24), (78, 39), (78, 52)), GOLD, 6)
    _rounded(draw, (12, 43, 29, 70), 7, fill=(23, 81, 139, 255), outline=INK, width=4)
    _rounded(draw, (67, 43, 84, 70), 7, fill=(23, 81, 139, 255), outline=INK, width=4)
    _line(draw, ((77, 67), (69, 78), (53, 78)), CYAN, 5)
    _ellipse(draw, (45, 73, 56, 83), fill=GOLD, outline=INK, width=2)
    return image


def draw_cinematic():
    image, draw = _base()
    _rounded(draw, (12, 22, 84, 74), 8, fill=(20, 72, 126, 255), outline=INK, width=5)
    _polygon(draw, ((42, 34), (42, 63), (66, 48)), CYAN, GOLD, 2)
    _line(draw, ((19, 83), (77, 83)), GOLD, 4)
    return image


def draw_server():
    image, draw = _base()
    for top in (14, 39, 64):
        _rounded(draw, (17, top, 79, top + 20), 6, fill=(20, 72, 126, 255), outline=INK, width=4)
        _ellipse(draw, (25, top + 6, 33, top + 14), fill=GOLD, outline=GOLD, width=1)
        _line(draw, ((42, top + 10), (68, top + 10)), CYAN, 3)
    return image


def draw_crest():
    image, draw = _base()
    _polygon(draw, ((48, 7), (82, 48), (48, 89), (14, 48)), (15, 60, 110, 220), GOLD, 4)
    _polygon(draw, ((48, 18), (59, 43), (53, 67), (48, 80), (43, 67), (37, 43)), CYAN, INK, 2)
    _line(draw, ((27, 48), (69, 48)), INK, 4)
    _ellipse(draw, (42, 41, 54, 53), fill=GOLD, outline=INK, width=2)
    return image


DRAWERS = {
    "run": draw_run, "jump": draw_jump, "attack": draw_attack, "skill": draw_skill,
    "character": draw_character, "inventory": draw_inventory, "skills": draw_skills, "menu": draw_menu,
    "account": draw_account, "lock": draw_lock, "eye": draw_eye, "notice": draw_notice,
    "support": draw_support, "cinematic": draw_cinematic, "server": draw_server, "crest": draw_crest,
}


def build(out_dir: Path, review_path: Path | None = None) -> dict:
    out_dir.mkdir(parents=True, exist_ok=True)
    rows = (len(ICON_IDS) + 3) // 4
    atlas = Image.new("RGBA", (CELL * 4, CELL * rows), (0, 0, 0, 0))
    parts = []
    rendered = {}
    for index, icon_id in enumerate(ICON_IDS):
        icon = DRAWERS[icon_id]().resize((ICON, ICON), Image.Resampling.LANCZOS)
        rendered[icon_id] = icon
        col, row = index % 4, index // 4
        px, py = col * CELL + 16, row * CELL + 16
        atlas.alpha_composite(icon, (px, py))
        parts.append({"id": icon_id, "x": px, "y": atlas.height - py - ICON, "w": ICON, "h": ICON})
    atlas_path = out_dir / "map01a-hud-icons.png"
    atlas.save(atlas_path, optimize=True)
    digest = hashlib.sha256(atlas_path.read_bytes()).hexdigest()
    manifest = {
        "id": "map01a-hud-icons-v1",
        "status": "DRAFT_RUNTIME_REVIEW",
        "runtimeApproved": False,
        "designReference": "LGO-2D-UI-Owner-Demos-2026-09-13/preferred-v2/01-hud-mobile-cong-dong-lam-joystick-cum-chien-dau-phai.png",
        "policy": "deterministic-shared-grid-no-text-no-screenshot-crop",
        "textureSize": [atlas.width, atlas.height],
        "pngBytes": atlas_path.stat().st_size,
        "sha256": digest,
        "assets": [{
            "path": RUNTIME_ATLAS_PATH,
            "role": "ui-hud-atlas",
            "generator": "pack_lgo_map01a_hud_icons",
            "referenceOnly": False,
            "sha256": digest,
        }],
        "parts": parts,
    }
    (out_dir / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    if review_path:
        review_path.parent.mkdir(parents=True, exist_ok=True)
        board = Image.new("RGB", (4 * 180, rows * 150), (7, 28, 52))
        draw = ImageDraw.Draw(board)
        font = ImageFont.load_default(size=18)
        for index, icon_id in enumerate(ICON_IDS):
            col, row = index % 4, index // 4
            cx, cy = col * 180 + 42, row * 150 + 12
            draw.rounded_rectangle((cx - 8, cy - 5, cx + 112, cy + 115), 28, fill=(10, 48, 83), outline=(205, 160, 73), width=2)
            board.paste(rendered[icon_id], (cx, cy), rendered[icon_id])
            draw.text((col * 180 + 90, row * 150 + 130), icon_id, fill=(238, 220, 174), font=font, anchor="mm")
        board.save(review_path, quality=92)
    return manifest


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--out-dir", type=Path, default=DEFAULT_OUT)
    parser.add_argument("--review", type=Path)
    args = parser.parse_args()
    manifest = build(args.out_dir.resolve(), args.review.resolve() if args.review else None)
    print(f"LGO_MAP01A_HUD_ICON_PACK_PASS icons={len(manifest['parts'])} sha256={manifest['sha256']} bytes={manifest['pngBytes']}")


if __name__ == "__main__":
    main()
