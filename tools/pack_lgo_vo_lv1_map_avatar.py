#!/usr/bin/env python3
"""Pack reviewed Võ Lv1 WIP into one runtime-sized base/full/modular atlas."""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter


CANVAS = (1024, 1536)
GROUND_SOURCE_Y = 1484
WORLD_HEIGHT = 1.70
TARGET_FULL_HEIGHT = 340
INPUTS = (
    ("base", "07-common-male-base-aligned-draft.png", None, 5),
    ("full", "01-vo-male-lv001-full-equipped-alpha-draft.png", None, 8),
    ("inner_top", "03-vo-male-lv001-inner-top-trim-draft.png", (385, 227), 6),
    ("arm_guard", "04-vo-male-lv001-arm-guard-trim-draft.png", (309, 528), 7),
    ("main_weapon", "05-vo-male-lv001-main-weapon-trim-draft.png", (328, 694), 8),
)


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def skill_slash() -> Image.Image:
    """Small clean Võ impact mark; generated once into the shared atlas."""
    size = 480
    glow = Image.new("RGBA", (size, size))
    g = ImageDraw.Draw(glow)
    for width, alpha in ((70, 35), (42, 70), (20, 145)):
        g.arc((42, 74, 438, 430), 196, 338, fill=(255, 137, 25, alpha), width=width)
        g.arc((72, 106, 408, 396), 204, 330, fill=(255, 222, 92, alpha), width=max(8, width // 2))
    glow = glow.filter(ImageFilter.GaussianBlur(7))
    crisp = Image.new("RGBA", (size, size))
    c = ImageDraw.Draw(crisp)
    c.arc((48, 80, 432, 424), 196, 338, fill=(255, 190, 44, 245), width=22)
    c.arc((78, 112, 402, 390), 204, 330, fill=(255, 250, 191, 255), width=9)
    c.polygon(((386, 310), (468, 334), (395, 351), (444, 400), (368, 360)), fill=(255, 221, 91, 235))
    glow.alpha_composite(crisp)
    return glow.resize((120, 120), Image.Resampling.LANCZOS)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source-dir", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    args = parser.parse_args()
    if args.output_dir.exists():
        raise FileExistsError("Use a new versioned output directory: " + str(args.output_dir))
    args.output_dir.mkdir(parents=True)
    atlas = Image.new("RGBA", (512, 512))
    scale = TARGET_FULL_HEIGHT / (1484 - 16)
    cursor_x = 6
    parts = []
    for part_id, filename, fixed_origin, order in INPUTS:
        path = args.source_dir / filename
        with Image.open(path) as raw:
            image = raw.convert("RGBA")
        if fixed_origin is None:
            if image.size != CANVAS:
                raise ValueError(f"{filename} must preserve common canvas {CANVAS}")
            box = image.getchannel("A").getbbox()
            if box != (275, 16, 801, 1484):
                raise ValueError(f"Unexpected reviewed silhouette bbox for {filename}: {box}")
            trimmed = image.crop(box)
            source_box = box
        else:
            box = image.getchannel("A").getbbox()
            if box != (0, 0, image.width, image.height):
                raise ValueError(f"Trim asset has unexpected empty edge: {filename} {box}")
            source_box = (fixed_origin[0], fixed_origin[1],
                          fixed_origin[0] + image.width, fixed_origin[1] + image.height)
            trimmed = image
        resized = trimmed.resize(
            (max(1, round(trimmed.width * scale)), max(1, round(trimmed.height * scale))),
            Image.Resampling.LANCZOS,
        )
        if part_id in {"base", "full"}:
            left, top = cursor_x, 6
            cursor_x += resized.width + 8
        else:
            left = cursor_x
            top = 366
            cursor_x += resized.width + 8
        if left + resized.width > 506 or top + resized.height > 506:
            raise ValueError("Runtime avatar atlas overflow: " + part_id)
        atlas.paste(resized, (left, top))
        source_width = source_box[2] - source_box[0]
        source_height = source_box[3] - source_box[1]
        center_x = (source_box[0] + source_box[2]) * .5
        center_y = (source_box[1] + source_box[3]) * .5
        parts.append({
            "id": part_id,
            "x": left,
            "y": 512 - top - resized.height,
            "w": resized.width,
            "h": resized.height,
            "worldW": source_width / CANVAS[1] * WORLD_HEIGHT,
            "worldH": source_height / CANVAS[1] * WORLD_HEIGHT,
            "dx": (center_x - CANVAS[0] * .5) / CANVAS[1] * WORLD_HEIGHT,
            "dy": (GROUND_SOURCE_Y - center_y) / CANVAS[1] * WORLD_HEIGHT,
            "order": order,
            "source": filename,
            "sourceSha256": digest(path),
            "sourceCanvasRect": list(source_box),
        })
    effect = skill_slash()
    effect_left, effect_top = 386, 366
    atlas.alpha_composite(effect, (effect_left, effect_top))
    optimized = atlas.quantize(colors=256, method=Image.Quantize.FASTOCTREE, dither=Image.Dither.NONE)
    atlas_path = args.output_dir / "vo-lv1-map-avatar-atlas.png"
    optimized.save(atlas_path, optimize=True, compress_level=9)
    review = Image.new("RGBA", atlas.size, "#17242f")
    review.alpha_composite(optimized.convert("RGBA"))
    review.convert("RGB").save(args.output_dir / "review-dark.jpg", quality=92)
    manifest = {
        "id": "vo-lv1-map-avatar-v1",
        "status": "DRAFT_RUNTIME_REVIEW",
        "classId": "vo",
        "levelBand": "1-30",
        "gender": "male",
        "canvas": list(CANVAS),
        "groundSourceY": GROUND_SOURCE_Y,
        "worldHeight": WORLD_HEIGHT,
        "atlas": [512, 512],
        "pngBytes": atlas_path.stat().st_size,
        "assets": [{
            "path": "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1MapAvatarArt/vo-lv1-map-avatar-atlas.png",
            "sha256": digest(atlas_path),
            "width": 512,
            "height": 512,
            "role": "paper-doll-preview-atlas",
            "generator": "reviewed_owner_wip",
            "referenceOnly": False,
        }],
        "parts": parts,
        "effects": [{
            "id": "skill_slash",
            "x": effect_left,
            "y": 512 - effect_top - effect.height,
            "w": effect.width,
            "h": effect.height,
            "worldW": 2.45,
            "worldH": 2.20,
            "dx": 1.08,
            "dy": 0.58,
            "order": 12,
        }],
        "modes": {
            "base": ["base"],
            "full": ["full"],
            "modular": ["base", "inner_top", "arm_guard", "main_weapon"],
        },
        "nonClaims": ["not final alpha", "not motion complete", "three of ten equipment slots only"],
    }
    (args.output_dir / "vo-lv1-map-avatar-layout.json").write_text(
        json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    print(json.dumps({"id": manifest["id"], "parts": len(parts), "pngBytes": manifest["pngBytes"]}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
