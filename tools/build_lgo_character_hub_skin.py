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

from PIL import Image, ImageChops, ImageDraw, ImageFilter


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
POTENTIAL_SIZE = (600, 520)
POTENTIAL_NODE_CENTERS = (
    (300, 62), (100, 230), (500, 230), (190, 410), (430, 410),
)


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


def build_shell() -> Image.Image:
    size = (1024, 676)
    image = _gradient(size, (8, 38, 70), (1, 13, 28))
    image = Image.alpha_composite(image, _radial_glow(size, (512, -30), 700, (18, 106, 178), 105))
    image = Image.alpha_composite(image, _radial_glow(size, (930, 220), 480, (16, 78, 132), 38))

    motif = Image.new("RGBA", size, (0, 0, 0, 0))
    md = ImageDraw.Draw(motif)
    cloud = (41, 132, 190, 31)
    for offset in (0, 24):
        md.line(_bezier(((22 + offset, 90), (90 + offset, 16), (130 + offset, 126), (205 + offset, 54))), fill=cloud, width=2)
        md.line(_bezier(((100 + offset, 40), (150 + offset, 9), (192 + offset, 74), (246 + offset, 28))), fill=cloud, width=1)
    for x in range(36, 990, 48):
        md.line([(x, 18), (x + 18, 18)], fill=(119, 169, 202, 15), width=1)
    right = motif.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    motif = Image.alpha_composite(motif, right)
    image = Image.alpha_composite(image, motif)

    return image


def build_panel() -> Image.Image:
    size = (512, 512)
    image = _gradient(size, (5, 28, 52), (1, 12, 25))
    image = Image.alpha_composite(image, _radial_glow(size, (490, 18), 150, (16, 93, 155), 38))
    image = Image.alpha_composite(image, _radial_glow(size, (18, 490), 140, (16, 76, 130), 24))
    motif = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(motif)
    color = (42, 124, 177, 24)
    draw.line(_bezier(((450, 0), (458, 38), (500, 28), (512, 72))), fill=color, width=1)
    draw.line(_bezier(((472, 0), (474, 25), (505, 30), (512, 52))), fill=color, width=1)
    draw.arc((444, -16, 530, 70), 74, 218, fill=(59, 148, 202, 22), width=1)
    draw.line(_bezier(((0, 450), (38, 458), (28, 500), (72, 512))), fill=color, width=1)
    draw.line(_bezier(((0, 472), (25, 474), (30, 505), (52, 512))), fill=color, width=1)
    draw.arc((-18, 442, 70, 530), 252, 38, fill=(59, 148, 202, 22), width=1)
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
    draw.rounded_rectangle((10, 10, width - 11, height - 11), radius=4, outline=(*outline[:3], min(150, outline[3])), width=1)
    draw.line([(16, 11), (width - 17, 11)], fill=(255, 255, 255, 82), width=1)
    draw.line([(16, height - 12), (width - 17, height - 12)], fill=GOLD_DARK, width=2)
    draw.line([(width // 3, 15), (2 * width // 3, 15)], fill=(170, 226, 255, 42), width=1)
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


CORE_NAME = "character-hub-potential-core.png"
CORE_SHA256 = "4aa14ce9475c440b012700c5b83cf6b693b6444e72d2527fd77948367941dd9a"
CORE_SOURCE_SHA256 = "df7a30492efb627f90cb1fc2cdda6e03f22fa9760ebe0844800ac882bcdf850f"
CORE_SOURCE_RECT = (1088, 568, 1392, 872)


def build_meditation_core(source: Path | None = None) -> Image.Image:
    path = source if source is not None else DEFAULT_OUTPUT / CORE_NAME
    expected = CORE_SOURCE_SHA256 if source is not None else CORE_SHA256
    if hashlib.sha256(path.read_bytes()).hexdigest() != expected:
        raise ValueError("Potential core source hash mismatch: " + str(path))
    with Image.open(path) as image:
        core = image.convert("RGBA")
    if source is None:
        if core.size != (224, 224):
            raise ValueError("Potential core module must be native 224x224")
        return core
    if core.size != (1536, 1024):
        raise ValueError("Unexpected Potential UI source canvas")
    core = core.crop(CORE_SOURCE_RECT)
    mask = Image.new("L", (304, 304))
    mask.putdata([round(255 * max(0, min(1, (152 - math.hypot(x - 151.5, y - 151.5)) / 6)))
                  for y in range(304) for x in range(304)])
    core.putalpha(ImageChops.multiply(core.getchannel("A"), mask))
    return core.resize((224, 224), Image.Resampling.LANCZOS)


def build_potential_topology() -> Image.Image:
    """Draw the fixed Potential geometry once; runtime only binds data overlays."""
    scale = 4
    width, height = POTENTIAL_SIZE
    size = (width * scale, height * scale)
    image = Image.new("RGBA", size, (0, 0, 0, 0))
    glow = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)
    glow_draw = ImageDraw.Draw(glow)

    def box(bounds: tuple[float, float, float, float]) -> tuple[int, int, int, int]:
        return tuple(round(value * scale) for value in bounds)

    def line(points: list[tuple[float, float]], fill: tuple[int, int, int, int], line_width: float) -> None:
        draw.line([(round(x * scale), round(y * scale)) for x, y in points], fill=fill,
                  width=max(1, round(line_width * scale)), joint="curve")

    def ellipse(center: tuple[float, float], radius: tuple[float, float],
                fill: tuple[int, int, int, int] | None = None,
                outline: tuple[int, int, int, int] | None = None, line_width: float = 1) -> None:
        cx, cy = center
        rx, ry = radius
        draw.ellipse(box((cx - rx, cy - ry, cx + rx, cy + ry)), fill=fill, outline=outline,
                     width=max(1, round(line_width * scale)))

    center = (300, 260)
    # One fixed orbit and one fixed connector system shared by every class.
    for node_center in POTENTIAL_NODE_CENTERS:
        glow_draw.line([(center[0] * scale, center[1] * scale),
                        (node_center[0] * scale, node_center[1] * scale)],
                       fill=(28, 155, 255, 95), width=5 * scale)
        line([center, node_center], (1, 8, 15, 235), 5)
        line([center, node_center], (67, 177, 255, 170), 1.25)

    glow_draw.ellipse(box((52, 44, 548, 476)), outline=(28, 147, 255, 110), width=7 * scale)
    glow_draw.ellipse(box((64, 52, 536, 468)), outline=(255, 184, 52, 90), width=6 * scale)
    image = Image.alpha_composite(image, glow.filter(ImageFilter.GaussianBlur(8 * scale)))
    draw = ImageDraw.Draw(image)

    ellipse(center, (250, 218), outline=(26, 87, 132, 150), line_width=1)
    ellipse(center, (244, 212), outline=(213, 151, 43, 105), line_width=1)
    ellipse(center, (236, 204), outline=(3, 11, 20, 245), line_width=5)
    ellipse(center, (236, 204), outline=(229, 167, 54, 205), line_width=1.5)
    ellipse(center, (220, 190), outline=(41, 126, 182, 92), line_width=1)
    ellipse(center, (203, 176), outline=(49, 145, 206, 95), line_width=1)

    # Quiet rune tracks and cardinal ornaments give the shared orbit the same
    # authored depth as the approved board without baking any class data into it.
    for inset, start in ((12, 8), (20, 31), (30, 54)):
        bounds = box((52 + inset, 44 + inset, 548 - inset, 476 - inset))
        for quarter in range(4):
            angle = start + quarter * 90
            draw.arc(bounds, angle, angle + 43, fill=(52, 154, 217, 76), width=3 * scale)
    for index in range(16):
        angle = math.tau * index / 16
        x = center[0] + math.cos(angle) * 212
        y = center[1] + math.sin(angle) * 183
        rune_size = 4 if index % 4 else 7
        diamond = [(x, y - rune_size), (x + rune_size, y),
                   (x, y + rune_size), (x - rune_size, y)]
        draw.line([(round(px * scale), round(py * scale)) for px, py in diamond + [diamond[0]]],
                  fill=(235, 174, 56, 150 if index % 4 else 225), width=max(2, scale))

    for index in range(24):
        angle = math.tau * index / 24
        x = center[0] + math.cos(angle) * 220
        y = center[1] + math.sin(angle) * 190
        radius = 5 if index % 4 == 0 else 2.5
        color = (255, 201, 76, 215) if index % 4 == 0 else (78, 178, 238, 150)
        ellipse((x, y), (radius, radius), fill=color)
        ellipse((x, y), (radius + 3, radius + 3), outline=(224, 162, 53, 105), line_width=1)

    # Central meditation seal and silhouette are also part of the fixed template.
    core_glow = Image.new("RGBA", size, (0, 0, 0, 0))
    core_glow_draw = ImageDraw.Draw(core_glow)
    core_glow_draw.ellipse(box((190, 150, 410, 370)), fill=(18, 137, 229, 55))
    core_glow_draw.ellipse(box((254, 214, 346, 306)), fill=(255, 166, 34, 105))
    image = Image.alpha_composite(image, core_glow.filter(ImageFilter.GaussianBlur(22 * scale)))
    draw = ImageDraw.Draw(image)
    ellipse(center, (112, 112), fill=(2, 20, 38, 210), outline=(225, 164, 49, 220), line_width=1.5)
    ellipse(center, (104, 104), outline=(62, 187, 255, 205), line_width=1.5)
    ellipse(center, (96, 96), outline=(24, 105, 168, 150), line_width=1)
    ellipse(center, (78, 78), outline=(229, 169, 55, 105), line_width=1)
    for angle in range(0, 360, 45):
        radians = math.radians(angle)
        line([(center[0], center[1]),
              (center[0] + math.cos(radians) * 92, center[1] + math.sin(radians) * 92)],
             (43, 151, 216, 76), 1)

    aura = (67, 191, 255, 235)
    aura_soft = (26, 112, 178, 190)
    energy = (255, 181, 48, 252)
    ink = (1, 15, 28, 255)
    cloth = (3, 37, 61, 255)
    # Head, tied hair and shoulder mantle form one readable silhouette at the
    # actual in-game size instead of a collection of unrelated body strokes.
    hair = [(278, 197), (284, 181), (294, 188), (300, 176), (306, 188),
            (318, 183), (322, 200), (314, 212), (286, 212)]
    draw.polygon([(x * scale, y * scale) for x, y in hair], fill=ink)
    ellipse((300, 203), (17, 21), fill=ink, outline=aura, line_width=1.5)
    torso = [(284, 216), (263, 230), (252, 282), (276, 310), (300, 296),
             (324, 310), (348, 282), (337, 230), (316, 216)]
    draw.polygon([(x * scale, y * scale) for x, y in torso], fill=cloth)
    line(torso + [torso[0]], aura_soft, 5)
    line(torso + [torso[0]], aura, 1.5)
    for points, outer_width, inner_width in (
        ([(274, 234), (246, 248), (220, 276), (237, 291), (265, 277)], 13, 8),
        ([(326, 234), (354, 248), (380, 276), (363, 291), (335, 277)], 13, 8),
        ([(280, 292), (248, 311), (214, 318), (246, 335), (294, 324)], 26, 19),
        ([(320, 292), (352, 311), (386, 318), (354, 335), (306, 324)], 26, 19),
    ):
        line(points, aura, outer_width)
        line(points, ink, inner_width)
    lap = [(210, 318), (246, 343), (300, 331), (354, 343), (390, 318),
           (365, 348), (326, 354), (300, 346), (274, 354), (235, 348)]
    draw.polygon([(x * scale, y * scale) for x, y in lap], fill=ink)
    line(lap + [lap[0]], aura_soft, 5)
    line(lap + [lap[0]], aura, 1.5)
    meridian = [(300, 225), (300, 246), (300, 267), (300, 289), (300, 312)]
    line(meridian, energy, 2)
    line([(300, 245), (275, 256), (253, 282)], (255, 180, 40, 220), 1.5)
    line([(300, 245), (325, 256), (347, 282)], (255, 180, 40, 220), 1.5)
    line([(275, 282), (300, 300), (325, 282)], (255, 180, 40, 175), 1)
    for point in meridian:
        ellipse(point, (10, 10), fill=(255, 150, 25, 55))
        ellipse(point, (4, 4), fill=(255, 181, 42, 255), outline=(255, 232, 154, 245), line_width=1)

    for cx, cy in POTENTIAL_NODE_CENTERS:
        # The topology owns one quiet outer slot frame. The authored icon supplies
        # its inner medallion, avoiding the previous stack of repeated concentric
        # rings while keeping all placement geometry out of class-bound controls.
        node_glow = Image.new("RGBA", size, (0, 0, 0, 0))
        node_glow_draw = ImageDraw.Draw(node_glow)
        node_glow_draw.ellipse(box((cx - 62, cy - 62, cx + 62, cy + 62)),
                               outline=(255, 178, 42, 105), width=7 * scale)
        image = Image.alpha_composite(image, node_glow.filter(ImageFilter.GaussianBlur(7 * scale)))
        draw = ImageDraw.Draw(image)
        ellipse((cx, cy), (61, 61), outline=(2, 11, 20, 248), line_width=7)
        ellipse((cx, cy), (58, 58), outline=(238, 176, 60, 230), line_width=2)
        ellipse((cx, cy), (46, 46), fill=(2, 28, 52, 42))
        for dx, dy in ((-69, 0), (69, 0), (0, -69), (0, 69)):
            if dx:
                line([(cx + dx - 7, cy), (cx + dx + 7, cy)], (244, 184, 65, 220), 2)
            else:
                line([(cx, cy + dy - 7), (cx, cy + dy + 7)], (244, 184, 65, 220), 2)
        value = (cx - 42, cy + 37, cx + 40, cy + 65)
        add = (cx + 42, cy + 37, cx + 70, cy + 65)
        draw.rectangle(box(value), fill=(1, 12, 23, 245), outline=(63, 123, 168, 220), width=5)
        draw.rectangle(box(add), fill=(2, 17, 30, 250), outline=(244, 185, 66, 242), width=6)
        line([(cx + 50, cy + 51), (cx + 62, cy + 51)], (255, 211, 92, 255), 2)
        line([(cx + 56, cy + 45), (cx + 56, cy + 57)], (255, 211, 92, 255), 2)

    return image.resize(POTENTIAL_SIZE, Image.Resampling.LANCZOS)


BUILDERS = {
    CORE_NAME: build_meditation_core,
    "character-hub-surface.png": build_shell,
    "character-hub-panel-surface.png": build_panel,
    "character-hub-tab-idle.png": lambda: _bevel_surface((320, 72), (12, 50, 86), (3, 22, 43), (74, 111, 144, 220)),
    "character-hub-tab-selected.png": lambda: _bevel_surface((320, 72), (24, 132, 250), (3, 73, 199), (80, 219, 255, 255), (22, 168, 255)),
    "character-hub-action-blue.png": lambda: _bevel_surface((384, 72), (31, 139, 244), (4, 67, 179), (91, 220, 255, 255), (24, 155, 255)),
    "character-hub-action-gold.png": lambda: _bevel_surface((384, 72), (255, 221, 135), (190, 119, 31), GOLD_LIGHT, (255, 190, 61)),
    "character-hub-close.png": build_close,
    "character-hub-potential-topology.png": build_potential_topology,
}


def _sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def build(output_dir: Path) -> dict:
    output_dir.mkdir(parents=True, exist_ok=True)
    assets = []
    roles = {
        CORE_NAME: "ui-potential-meditation-core",
        "character-hub-surface.png": "ui-modal-surface",
        "character-hub-panel-surface.png": "ui-panel-surface",
        "character-hub-tab-idle.png": "ui-tab-idle",
        "character-hub-tab-selected.png": "ui-tab-selected",
        "character-hub-action-blue.png": "ui-action-blue",
        "character-hub-action-gold.png": "ui-action-gold",
        "character-hub-close.png": "ui-close-frame",
        "character-hub-potential-topology.png": "ui-potential-topology-template",
    }
    for name, builder in BUILDERS.items():
        path = output_dir / name
        image = builder()
        if name == "character-hub-potential-topology.png":
            image = image.quantize(colors=256, method=Image.Quantize.FASTOCTREE, dither=Image.Dither.NONE)
        image.save(path, format="PNG", optimize=True)
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
        "displayPolicy": "shell uses approved 1098:724 aspect; panel/tab/action/close are shared across all five character-hub screens; Potential geometry is one 600x520 template with one outer slot frame per node, while class data overlays remain geometry-free",
        "sourceMethod": "deterministic Pillow raster authored from approved navy, cyan-glow and old-gold visual language; no canonical-board crop",
        "pixelBudget": f"actual-display-sized UI chrome; {total_bytes} bytes total; no mipmaps",
        "importPolicy": "UI textures keep native display resolution caps (shell 1024, Potential topology 1024, panels/controls 512, close 128) with mipmaps disabled",
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
