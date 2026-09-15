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


POTENTIAL_MODULE_IDS = ("attack", "defense", "vitality", "spirit", "agility", "core")
POTENTIAL_SOURCE_CENTERS = ((298, 250), (770, 248), (1238, 248), (298, 724), (770, 724), (1240, 720))
POTENTIAL_MODULE_OUTPUT = ROOT / "client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01APotentialIcons"


def compose_potential_icon_modules(board: Image.Image) -> tuple[Image.Image, list[dict]]:
    """One 128px frame plus six ring-free contents in a single 512x256 atlas."""
    if board.size != (1536, 1024):
        raise ValueError("Potential modules require the registered 1536x1024 UI source")
    board = board.convert("RGBA")
    atlas = Image.new("RGBA", (512, 256))
    content_mask = Image.new("L", (304, 304))
    content_mask.putdata([round(255 * max(0, min(1, (152 - math.hypot(x-151.5, y-151.5)) / 2)))
                          for y in range(304) for x in range(304)])
    parts = []
    for index, (icon_id, (cx, cy)) in enumerate(zip(POTENTIAL_MODULE_IDS, POTENTIAL_SOURCE_CENTERS)):
        x, y = index % 4 * 128, index // 4 * 128
        content = board.crop((cx-152, cy-152, cx+152, cy+152))
        content.putalpha(ImageChops.multiply(content.getchannel("A"), content_mask))
        atlas.alpha_composite(content.resize((84, 84), Image.Resampling.LANCZOS), (x+22, y+22))
        parts.append({"id": icon_id, "x": x, "y": 128-y, "w": 128, "h": 128,
                      "role": "inner-symbol", "sharedFrameId": "frame"})
    # The sword medallion supplies the single frame master. Its registered
    # aperture is cleared; neither its blade nor a canonical screenshot is copied.
    frame = board.crop((58, 10, 538, 490))
    aperture = Image.new("L", (480, 480))
    aperture.putdata([round(255 * max(0, min(1, (math.hypot(x-239.5, y-239.5)-158) / 2)))
                      for y in range(480) for x in range(480)])
    frame.putalpha(ImageChops.multiply(frame.getchannel("A"), aperture))
    atlas.alpha_composite(frame.resize((128, 128), Image.Resampling.LANCZOS), (256, 128))
    parts.append({"id": "frame", "x": 256, "y": 0, "w": 128, "h": 128, "role": "shared-frame"})
    return atlas, parts


def pack_potential_icon_modules(source: Path, output_dir: Path = POTENTIAL_MODULE_OUTPUT) -> dict:
    if hashlib.sha256(source.read_bytes()).hexdigest() != CORE_SOURCE_SHA256:
        raise ValueError("Potential icon source hash mismatch: " + str(source))
    with Image.open(source) as board:
        atlas, parts = compose_potential_icon_modules(board)
    output_dir.mkdir(parents=True, exist_ok=True)
    path = output_dir / "map01a-potential-icons.png"
    atlas.save(path, optimize=True)
    if path.stat().st_size > 200_000:
        raise ValueError("Potential modules exceed their 200000-byte PNG budget")
    manifest = {"id": "map01a-potential-icons-v1", "status": "DRAFT_RUNTIME_REVIEW",
                "runtimeApproved": False, "revision": "shared-frame-inner-symbols-v2",
                "sourceBoard": str(source), "sourceSha256": CORE_SOURCE_SHA256,
                "sourceCenters": POTENTIAL_SOURCE_CENTERS, "frameSourceRect": [58,10,538,490],
                "frameAperture": "radius158-feather2-native480", "contentMask": "radius152-feather2-native304",
                "textureSize": [512,256], "cellSize": [128,128], "contentRect": [22,22,84,84],
                "pngBytes": path.stat().st_size, "sha256": _sha256(path), "parts": parts,
                "assets": [{"path": str(path.relative_to(ROOT)) if path.is_relative_to(ROOT) else path.name,
                            "role": "ui-potential-icon-atlas", "generator": "build_lgo_character_hub_skin",
                            "referenceOnly": False, "sha256": _sha256(path)}],
                "displayPolicy": "One frame sprite shared by topology, detail and cost; class data binds inner sprites only. Native 128px cells cover 108-124px display without upscale."}
    (output_dir / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2)+"\n")
    return manifest


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

    # Reuse the reviewed UI motif, not a procedural body. It is baked before
    # node/value frames, so the fixed topology remains their only geometry owner.
    core = build_meditation_core().resize((224 * scale, 224 * scale), Image.Resampling.LANCZOS)
    image.alpha_composite(core, (188 * scale, 148 * scale))
    draw = ImageDraw.Draw(image)

    for cx, cy in POTENTIAL_NODE_CENTERS:
        # Reusable frame sprites belong to the immutable runtime base, not this bitmap.
        value = (cx - 42, cy + 80, cx + 40, cy + 108)
        add = (cx + 42, cy + 80, cx + 70, cy + 108)
        draw.rectangle(box(value), fill=(1, 12, 23, 245), outline=(63, 123, 168, 220), width=5)
        draw.rectangle(box(add), fill=(2, 17, 30, 250), outline=(244, 185, 66, 242), width=6)
        line([(cx + 50, cy + 94), (cx + 62, cy + 94)], (255, 211, 92, 255), 2)
        line([(cx + 56, cy + 88), (cx + 56, cy + 100)], (255, 211, 92, 255), 2)

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
        "geometryRevision": "v5-independent-shared-icon-frames",
        "coreSource": {"sha256": CORE_SOURCE_SHA256, "sourceRect": CORE_SOURCE_RECT,
                       "mask": "radius152-inward-feather6", "moduleSha256": CORE_SHA256},
        "canonicalDesignSet": str(CANONICAL_ROOT),
        "canonicalSha256": canonical,
        "assets": assets,
        "displayPolicy": "shell uses approved 1098:724 aspect; panel/tab/action/close are shared across all five character-hub screens; Potential geometry is one 600x520 template with one separately referenced shared frame sprite per node, while class data overlays remain geometry-free",
        "sourceMethod": "deterministic Pillow raster authored from approved navy, cyan-glow and old-gold visual language; no canonical-board crop",
        "pixelBudget": f"actual-display-sized UI chrome; {total_bytes} bytes total; no mipmaps",
        "importPolicy": "UI textures keep native display resolution caps (shell 1024, Potential topology 1024, panels/controls 512, close 128) with mipmaps disabled",
    }
    (output_dir / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return manifest


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--output-dir", type=Path, default=DEFAULT_OUTPUT)
    parser.add_argument("--refresh-potential-icon-modules-from-source", type=Path, help="Separate the registered frame and six inner symbols into one shared atlas")
    parser.add_argument("--refresh-core-from-source", type=Path, help="Re-extract the pinned UI motif; never a canonical screen crop")
    args = parser.parse_args()
    if args.refresh_potential_icon_modules_from_source:
        pack_potential_icon_modules(args.refresh_potential_icon_modules_from_source)
    if args.refresh_core_from_source:
        build_meditation_core(args.refresh_core_from_source).save(DEFAULT_OUTPUT / CORE_NAME, optimize=True)
    manifest = build(args.output_dir.resolve())
    print(f"LGO_CHARACTER_HUB_SKIN_BUILT assets={len(manifest['assets'])} output={args.output_dir.resolve()}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
