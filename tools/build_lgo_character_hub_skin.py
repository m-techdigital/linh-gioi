#!/usr/bin/env python3
"""Build the reusable Map01A character-hub chrome from the approved visual language.

The canonical screenshots remain reference-only.  This script draws clean runtime
surfaces at their actual display aspect and never crops pixels from the boards.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter


ROOT = Path(__file__).resolve().parents[1]
DEFAULT_OUTPUT = ROOT / "client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AUiSkin"
CANONICAL_ROOT = Path("/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/redesign-v4-five-tabs")
CANONICAL_FILES = (
    "01-nhan-vat-nam-tab-compact-APPROVED.png",
    "02-ruong-do-phan-loai-doc-tab-compact-APPROVED.png",
    "03-ky-nang-five-tab-APPROVED.png",
    "04-tiem-nang-five-tab-APPROVED.png",
    "05-linh-thu-five-tab-APPROVED.png",
)

GOLD = (231, 181, 74, 238)
GOLD_LIGHT = (255, 225, 137, 245)
GOLD_DARK = (103, 68, 22, 230)
CYAN = (52, 190, 255, 245)


def _gradient(size: tuple[int, int], top: tuple[int, int, int], bottom: tuple[int, int, int]) -> Image.Image:
    width, height = size
    image = Image.new("RGBA", size)
    pixels = image.load()
    for y in range(height):
        t = y / max(1, height - 1)
        color = tuple(round(top[i] * (1 - t) + bottom[i] * t) for i in range(3)) + (255,)
        for x in range(width):
            pixels[x, y] = color
    return image


def _radial_glow(size: tuple[int, int], center: tuple[float, float], radius: float,
                 color: tuple[int, int, int], alpha: int) -> Image.Image:
    width, height = size
    layer = Image.new("RGBA", size, (0, 0, 0, 0))
    pixels = layer.load()
    cx, cy = center
    for y in range(height):
        for x in range(width):
            distance = math.hypot(x - cx, y - cy) / radius
            if distance < 1:
                strength = (1 - distance) ** 2
                pixels[x, y] = (*color, round(alpha * strength))
    return layer


def _bezier(points: tuple[tuple[float, float], ...], steps: int = 48) -> list[tuple[float, float]]:
    p0, p1, p2, p3 = points
    result = []
    for index in range(steps + 1):
        t = index / steps
        u = 1 - t
        result.append((
            u ** 3 * p0[0] + 3 * u * u * t * p1[0] + 3 * u * t * t * p2[0] + t ** 3 * p3[0],
            u ** 3 * p0[1] + 3 * u * u * t * p1[1] + 3 * u * t * t * p2[1] + t ** 3 * p3[1],
        ))
    return result


def _corner_ornament(size: int = 150) -> Image.Image:
    layer = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(layer)
    draw.line([(5, 65), (5, 19), (19, 5), (65, 5)], fill=GOLD_LIGHT, width=3)
    draw.line([(11, 59), (11, 25), (25, 11), (59, 11)], fill=GOLD_DARK, width=2)
    draw.line(_bezier(((20, 5), (29, 35), (61, 13), (79, 39))), fill=GOLD, width=3)
    draw.line(_bezier(((5, 20), (35, 29), (13, 61), (39, 79))), fill=GOLD, width=3)
    draw.line(_bezier(((29, 18), (45, 40), (67, 18), (91, 49))), fill=(210, 153, 53, 180), width=2)
    draw.polygon([(14, 14), (22, 10), (28, 14), (22, 20)], fill=GOLD_LIGHT)
    draw.ellipse((17, 17, 25, 25), fill=(37, 143, 218, 255), outline=GOLD_LIGHT, width=2)
    return layer


def build_shell() -> Image.Image:
    size = (1024, 676)
    image = _gradient(size, (8, 38, 70), (1, 13, 28))
    image = Image.alpha_composite(image, _radial_glow(size, (512, -30), 700, (18, 106, 178), 105))
    image = Image.alpha_composite(image, _radial_glow(size, (930, 220), 480, (16, 78, 132), 38))

    motif = Image.new("RGBA", size, (0, 0, 0, 0))
    md = ImageDraw.Draw(motif)
    cloud = (41, 132, 190, 42)
    for offset in (0, 24):
        md.line(_bezier(((22 + offset, 90), (90 + offset, 16), (130 + offset, 126), (205 + offset, 54))), fill=cloud, width=3)
        md.line(_bezier(((100 + offset, 40), (150 + offset, 9), (192 + offset, 74), (246 + offset, 28))), fill=cloud, width=2)
    right = motif.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    motif = Image.alpha_composite(motif, right)
    image = Image.alpha_composite(image, motif)

    glow = Image.new("RGBA", size, (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    gd.rounded_rectangle((5, 5, 1018, 670), radius=12, outline=(255, 205, 92, 160), width=7)
    glow = glow.filter(ImageFilter.GaussianBlur(8))
    image = Image.alpha_composite(image, glow)

    draw = ImageDraw.Draw(image)
    draw.rounded_rectangle((3, 3, 1020, 672), radius=12, outline=GOLD_DARK, width=3)
    draw.rounded_rectangle((7, 7, 1016, 668), radius=10, outline=GOLD_LIGHT, width=2)
    draw.rounded_rectangle((13, 13, 1010, 662), radius=8, outline=(151, 103, 35, 190), width=2)
    draw.line([(44, 54), (395, 54), (412, 42), (612, 42), (629, 54), (980, 54)], fill=(221, 169, 67, 155), width=2)
    draw.polygon([(512, 35), (520, 43), (512, 51), (504, 43)], fill=GOLD_LIGHT)
    draw.ellipse((508, 39, 516, 47), fill=(25, 140, 222, 255))

    corner = _corner_ornament()
    image.alpha_composite(corner, (0, 0))
    image.alpha_composite(corner.transpose(Image.Transpose.FLIP_LEFT_RIGHT), (1024 - corner.width, 0))
    image.alpha_composite(corner.transpose(Image.Transpose.FLIP_TOP_BOTTOM), (0, 676 - corner.height))
    image.alpha_composite(corner.transpose(Image.Transpose.ROTATE_180), (1024 - corner.width, 676 - corner.height))
    return image


def build_panel() -> Image.Image:
    size = (512, 512)
    image = _gradient(size, (5, 28, 52), (1, 12, 25))
    image = Image.alpha_composite(image, _radial_glow(size, (430, 30), 420, (16, 93, 155), 56))
    motif = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(motif)
    color = (42, 124, 177, 35)
    draw.line(_bezier(((325, 0), (380, 82), (482, 0), (512, 90))), fill=color, width=3)
    draw.line(_bezier(((360, 0), (398, 58), (466, 32), (512, 132))), fill=color, width=2)
    draw.line(_bezier(((0, 430), (78, 366), (128, 502), (205, 456))), fill=color, width=3)
    return Image.alpha_composite(image, motif)


def _bevel_surface(size: tuple[int, int], top: tuple[int, int, int], bottom: tuple[int, int, int],
                   outline: tuple[int, int, int, int], glow_color: tuple[int, int, int] | None = None) -> Image.Image:
    width, height = size
    image = Image.new("RGBA", size, (0, 0, 0, 0))
    if glow_color:
        glow = Image.new("RGBA", size, (0, 0, 0, 0))
        gd = ImageDraw.Draw(glow)
        gd.rounded_rectangle((9, 9, width - 10, height - 10), radius=8, outline=(*glow_color, 220), width=7)
        image = Image.alpha_composite(image, glow.filter(ImageFilter.GaussianBlur(7)))
    fill = _gradient((width - 12, height - 12), top, bottom)
    mask = Image.new("L", fill.size, 0)
    ImageDraw.Draw(mask).rounded_rectangle((0, 0, fill.width - 1, fill.height - 1), radius=6, fill=255)
    image.paste(fill, (6, 6), mask)
    draw = ImageDraw.Draw(image)
    draw.rounded_rectangle((6, 6, width - 7, height - 7), radius=6, outline=outline, width=2)
    draw.line([(16, 11), (width - 17, 11)], fill=(255, 255, 255, 82), width=1)
    draw.line([(16, height - 12), (width - 17, height - 12)], fill=GOLD_DARK, width=2)
    draw.polygon([(8, height // 2), (14, height // 2 - 6), (20, height // 2), (14, height // 2 + 6)], fill=outline)
    draw.polygon([(width - 9, height // 2), (width - 15, height // 2 - 6), (width - 21, height // 2), (width - 15, height // 2 + 6)], fill=outline)
    return image


def build_close() -> Image.Image:
    size = (96, 96)
    image = Image.new("RGBA", size, (0, 0, 0, 0))
    glow = Image.new("RGBA", size, (0, 0, 0, 0))
    points = [(28, 4), (68, 4), (92, 28), (92, 68), (68, 92), (28, 92), (4, 68), (4, 28)]
    ImageDraw.Draw(glow).line(points + [points[0]], fill=(255, 196, 76, 190), width=8, joint="curve")
    image = Image.alpha_composite(image, glow.filter(ImageFilter.GaussianBlur(7)))
    draw = ImageDraw.Draw(image)
    draw.polygon(points, fill=(3, 24, 45, 252), outline=GOLD_LIGHT)
    inner = [(31, 10), (65, 10), (86, 31), (86, 65), (65, 86), (31, 86), (10, 65), (10, 31)]
    draw.line(inner + [inner[0]], fill=GOLD_DARK, width=3, joint="curve")
    for x, y in ((18, 18), (78, 18), (18, 78), (78, 78)):
        draw.ellipse((x - 3, y - 3, x + 3, y + 3), fill=GOLD_LIGHT)
    return image


BUILDERS = {
    "character-hub-surface.png": build_shell,
    "character-hub-panel-surface.png": build_panel,
    "character-hub-tab-idle.png": lambda: _bevel_surface((320, 72), (12, 50, 86), (3, 22, 43), (74, 111, 144, 220)),
    "character-hub-tab-selected.png": lambda: _bevel_surface((320, 72), (24, 132, 250), (3, 73, 199), (80, 219, 255, 255), (22, 168, 255)),
    "character-hub-action-blue.png": lambda: _bevel_surface((384, 72), (31, 139, 244), (4, 67, 179), (91, 220, 255, 255), (24, 155, 255)),
    "character-hub-action-gold.png": lambda: _bevel_surface((384, 72), (255, 221, 135), (190, 119, 31), GOLD_LIGHT, (255, 190, 61)),
    "character-hub-close.png": build_close,
}


def _sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def build(output_dir: Path) -> dict:
    output_dir.mkdir(parents=True, exist_ok=True)
    assets = []
    roles = {
        "character-hub-surface.png": "ui-modal-surface",
        "character-hub-panel-surface.png": "ui-panel-surface",
        "character-hub-tab-idle.png": "ui-tab-idle",
        "character-hub-tab-selected.png": "ui-tab-selected",
        "character-hub-action-blue.png": "ui-action-blue",
        "character-hub-action-gold.png": "ui-action-gold",
        "character-hub-close.png": "ui-close-frame",
    }
    for name, builder in BUILDERS.items():
        path = output_dir / name
        builder().save(path, format="PNG", optimize=True)
        assets.append({
            "path": str(path.relative_to(ROOT)) if path.is_relative_to(ROOT) else name,
            "generator": "build_lgo_character_hub_skin",
            "referenceOnly": False,
            "role": roles[name],
            "sha256": _sha256(path),
        })
    canonical = []
    for name in CANONICAL_FILES:
        path = CANONICAL_ROOT / name
        canonical.append({"name": name, "sha256": _sha256(path) if path.is_file() else "unavailable"})
    total_bytes = sum((output_dir / name).stat().st_size for name in BUILDERS)
    manifest = {
        "id": "map01a-character-hub-chrome-v2",
        "status": "DRAFT_RUNTIME_REVIEW",
        "canonicalDesignSet": str(CANONICAL_ROOT),
        "canonicalSha256": canonical,
        "assets": assets,
        "displayPolicy": "shell uses approved 1098:724 aspect; panel/tab/action/close are shared across all five character-hub screens",
        "sourceMethod": "deterministic Pillow raster authored from approved navy, cyan-glow and old-gold visual language; no canonical-board crop",
        "pixelBudget": f"actual-display-sized UI chrome; {total_bytes} bytes total; no mipmaps",
        "importPolicy": "UI textures keep native display resolution caps (shell 1024, panels/controls 512, close 128) with mipmaps disabled",
    }
    (output_dir / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return manifest


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--output-dir", type=Path, default=DEFAULT_OUTPUT)
    args = parser.parse_args()
    manifest = build(args.output_dir.resolve())
    print(f"LGO_CHARACTER_HUB_SKIN_BUILT assets={len(manifest['assets'])} output={args.output_dir.resolve()}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
