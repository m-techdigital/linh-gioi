#!/usr/bin/env python3
"""Pack the reviewed Võ Lv1 male/female paper-doll batch at runtime display size."""

from __future__ import annotations

import argparse
import hashlib
import json
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter


CANVAS = (1024, 1536)
GROUND_SOURCE_Y = 1484
WORLD_HEIGHT = 1.70
SPACE_PROFILE_ID = "lgo_character_canvas_1024x1536_v1"
TARGET_FULL_HEIGHT = 340
ATLAS_SIZE = 1024
SLOTS = (
    "main_weapon", "head_hair", "inner_top", "outer_tunic", "lower_garment",
    "waist", "arm_guard", "boots", "light_armor", "accessory",
)
MOTION_FRAMES = ("idle", "walk_a", "walk_b", "dash", "punch_windup", "punch_impact")


def project_canvas_rect(box) -> dict:
    """Project an authored rectangle without recentering or fitting its contents.

    WORLD_HEIGHT is the canvas span, not each character/pose's bounding height.
    Atlas resolution and packing coordinates deliberately do not enter this map.
    """
    if not isinstance(box, (tuple, list)) or len(box) != 4 or not all(math.isfinite(float(v)) for v in box):
        raise ValueError("A finite source canvas rectangle is required")
    left, top, right, bottom = box
    if not (0 <= left < right <= CANVAS[0] and 0 <= top < bottom <= CANVAS[1]):
        raise ValueError("Rectangle lies outside the registered source canvas")
    return {"sourceSpaceProfile": SPACE_PROFILE_ID, "sourceCanvasRect": list(box),
            "worldW": (right - left) / CANVAS[1] * WORLD_HEIGHT,
            "worldH": (bottom - top) / CANVAS[1] * WORLD_HEIGHT,
            "dx": ((left + right) * .5 - CANVAS[0] * .5) / CANVAS[1] * WORLD_HEIGHT,
            "dy": (GROUND_SOURCE_Y - (top + bottom) * .5) / CANVAS[1] * WORLD_HEIGHT}


def registration_errors(parts) -> list[str]:
    errors = []
    for part in parts:
        label = part.get("id", "<unnamed>")
        if part.get("sourceSpaceProfile") != SPACE_PROFILE_ID:
            errors.append(label + ": missing/unsupported source space profile")
            continue
        try:
            expected = project_canvas_rect(part.get("sourceCanvasRect", []))
            if any(not math.isfinite(float(part[k])) or abs(part[k] - expected[k]) > 1e-6
                   for k in ("worldW", "worldH", "dx", "dy")):
                errors.append(label + ": scale/placement differs from source registration")
        except (ValueError, TypeError, KeyError):
            errors.append(label + ": invalid source registration")
    return errors


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def skill_slash() -> Image.Image:
    size = 480
    glow = Image.new("RGBA", (size, size))
    draw = ImageDraw.Draw(glow)
    for width, alpha in ((70, 35), (42, 70), (20, 145)):
        draw.arc((42, 74, 438, 430), 196, 338, fill=(255, 137, 25, alpha), width=width)
        draw.arc((72, 106, 408, 396), 204, 330, fill=(255, 222, 92, alpha), width=max(8, width // 2))
    glow = glow.filter(ImageFilter.GaussianBlur(7))
    crisp = Image.new("RGBA", (size, size))
    draw = ImageDraw.Draw(crisp)
    draw.arc((48, 80, 432, 424), 196, 338, fill=(255, 190, 44, 245), width=22)
    draw.arc((78, 112, 402, 390), 204, 330, fill=(255, 250, 191, 255), width=9)
    draw.polygon(((386, 310), (468, 334), (395, 351), (444, 400), (368, 360)), fill=(255, 221, 91, 235))
    glow.alpha_composite(crisp)
    return glow.resize((120, 120), Image.Resampling.LANCZOS)


def shelf_pack(entries: list[dict]) -> None:
    x = y = 6
    row_height = 0
    for entry in sorted(entries, key=lambda item: item["image"].height, reverse=True):
        image = entry["image"]
        if x + image.width + 6 > ATLAS_SIZE:
            x = 6
            y += row_height + 6
            row_height = 0
        if y + image.height + 6 > ATLAS_SIZE:
            raise ValueError("Võ Lv1 runtime atlas overflow; do not silently downsample again")
        entry["left"], entry["top"] = x, y
        x += image.width + 6
        row_height = max(row_height, image.height)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source-dir", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    args = parser.parse_args()
    if args.output_dir.exists():
        raise FileExistsError("Use a new versioned output directory: " + str(args.output_dir))
    args.output_dir.mkdir(parents=True)
    scale = TARGET_FULL_HEIGHT / (GROUND_SOURCE_Y - 16)
    entries: list[dict] = []
    order_by_slot = {slot: 8 + index for index, slot in enumerate(SLOTS)}
    for gender in ("male", "female"):
        for kind, filename in (("base", "base.png"), ("full", "full.png")):
            path = args.source_dir / gender / filename
            source = Image.open(path).convert("RGBA")
            box = source.getchannel("A").getbbox()
            if source.size != CANVAS or box is None:
                raise ValueError(f"Invalid common canvas source: {path}")
            trimmed = source.crop(box)
            resized = trimmed.resize((round(trimmed.width * scale), round(trimmed.height * scale)), Image.Resampling.LANCZOS)
            entries.append({"id": f"{gender}_{kind}", "gender": gender, "kind": kind, "slot": "", "order": 6 if kind == "base" else 18,
                            "path": path, "box": box, "image": resized})
        for slot in SLOTS:
            path = args.source_dir / gender / f"slot-{slot}.png"
            source = Image.open(path).convert("RGBA")
            box = source.getchannel("A").getbbox()
            if source.size != CANVAS or box is None:
                raise ValueError(f"Invalid generated slot: {path}")
            trimmed = source.crop(box)
            resized = trimmed.resize((max(1, round(trimmed.width * scale)), max(1, round(trimmed.height * scale))), Image.Resampling.LANCZOS)
            entries.append({"id": f"{gender}_slot_{slot}", "gender": gender, "kind": "slot", "slot": slot,
                            "order": order_by_slot[slot], "path": path, "box": box, "image": resized})

    effect = skill_slash()
    entries.append({"id": "skill_slash", "gender": "shared", "kind": "effect", "slot": "", "order": 24,
                    "path": None, "box": (0, 0, 120, 120), "image": effect})
    shelf_pack(entries)
    atlas = Image.new("RGBA", (ATLAS_SIZE, ATLAS_SIZE))
    parts = []
    effect_data = None
    for entry in entries:
        image = entry["image"]
        atlas.alpha_composite(image, (entry["left"], entry["top"]))
        box = entry["box"]
        if entry["kind"] == "effect":
            effect_data = {"id": "skill_slash", "x": entry["left"], "y": ATLAS_SIZE - entry["top"] - image.height,
                           "w": image.width, "h": image.height, "worldW": 2.45, "worldH": 2.20,
                           "dx": 1.08, "dy": .58, "order": entry["order"]}
            continue
        parts.append({
            "id": entry["id"], "gender": entry["gender"], "kind": entry["kind"], "slot": entry["slot"],
            "x": entry["left"], "y": ATLAS_SIZE - entry["top"] - image.height, "w": image.width, "h": image.height,
            **project_canvas_rect(box),
            "order": entry["order"], "source": str(entry["path"].relative_to(args.source_dir)),
            "sourceSha256": digest(entry["path"]),
        })
    optimized = atlas.quantize(colors=256, method=Image.Quantize.FASTOCTREE, dither=Image.Dither.NONE)
    atlas_path = args.output_dir / "vo-lv1-map-avatar-atlas.png"
    optimized.save(atlas_path, optimize=True, compress_level=9)
    review = Image.new("RGBA", atlas.size, "#17242f")
    review.alpha_composite(optimized.convert("RGBA"))
    review.convert("RGB").save(args.output_dir / "review-dark.jpg", quality=92)

    motion_sources = []
    idle_source = Image.open(args.source_dir / "motion-male" / "idle.png").convert("RGBA")
    idle_box = idle_source.getchannel("A").getbbox()
    motion_scale = TARGET_FULL_HEIGHT / (idle_box[3] - idle_box[1])
    for frame_id in MOTION_FRAMES:
        path = args.source_dir / "motion-male" / f"{frame_id}.png"
        source = Image.open(path).convert("RGBA")
        box = source.getchannel("A").getbbox()
        trimmed = source.crop(box)
        resized = trimmed.resize((round(trimmed.width * motion_scale), round(trimmed.height * motion_scale)), Image.Resampling.LANCZOS)
        motion_sources.append({"id": frame_id, "path": path, "box": box, "image": resized})
    shelf_pack(motion_sources)
    motion_atlas = Image.new("RGBA", (ATLAS_SIZE, ATLAS_SIZE))
    motion_frames = []
    pixels_per_world = (idle_box[3] - idle_box[1]) / WORLD_HEIGHT
    for entry in motion_sources:
        image = entry["image"]
        motion_atlas.alpha_composite(image, (entry["left"], entry["top"]))
        box = entry["box"]
        center_x, center_y = (box[0] + box[2]) * .5, (box[1] + box[3]) * .5
        motion_frames.append({
            "id": entry["id"], "x": entry["left"], "y": ATLAS_SIZE - entry["top"] - image.height,
            "w": image.width, "h": image.height,
            "worldW": (box[2] - box[0]) / pixels_per_world, "worldH": (box[3] - box[1]) / pixels_per_world,
            "dx": (center_x - 256) / pixels_per_world, "dy": (510 - center_y) / pixels_per_world,
            "order": 19, "source": str(entry["path"].relative_to(args.source_dir)), "sourceSha256": digest(entry["path"]),
        })
    motion_optimized = motion_atlas.quantize(colors=256, method=Image.Quantize.FASTOCTREE, dither=Image.Dither.NONE)
    motion_path = args.output_dir / "vo-lv1-motion-atlas.png"
    motion_optimized.save(motion_path, optimize=True, compress_level=9)
    motion_review = Image.new("RGBA", motion_atlas.size, "#17242f")
    motion_review.alpha_composite(motion_optimized.convert("RGBA"))
    motion_review.convert("RGB").save(args.output_dir / "review-motion-dark.jpg", quality=92)
    manifest = {
        "id": "vo-lv1-map-avatar-v2", "status": "DRAFT_RUNTIME_REVIEW", "classId": "vo", "levelBand": "1-30",
        "genders": ["male", "female"], "slots": list(SLOTS), "canvas": list(CANVAS), "groundSourceY": GROUND_SOURCE_Y,
        "worldHeight": WORLD_HEIGHT, "atlas": [ATLAS_SIZE, ATLAS_SIZE], "pngBytes": atlas_path.stat().st_size,
        "assets": [{"path": "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1MapAvatarArt/vo-lv1-map-avatar-atlas.png",
                    "sha256": digest(atlas_path), "width": ATLAS_SIZE, "height": ATLAS_SIZE,
                    "role": "two-gender-ten-slot-paper-doll-preview-atlas", "generator": "aligned_imagegen_delta_batch", "referenceOnly": False},
                   {"path": "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1MapAvatarArt/vo-lv1-motion-atlas.png",
                    "sha256": digest(motion_path), "width": ATLAS_SIZE, "height": ATLAS_SIZE,
                    "role": "vo-male-lv1-runtime-motion-frames", "generator": "reference_guided_imagegen_motion_batch", "referenceOnly": False}],
        "parts": parts, "effects": [effect_data], "motionFrames": motion_frames,
        "modes": {"base": ["base"], "full": ["full"], "modular": ["base", *SLOTS]},
        "nonClaims": ["Lv1 art checkpoint within Lv1-30 scope", "motion still uses transform proof", "requires Player visual review"],
    }
    # Keep the runtime catalog compact: it is parsed by Unity, while the readable
    # production provenance stays in the source-side preparation manifest.
    (args.output_dir / "manifest.json").write_text(
        json.dumps(manifest, ensure_ascii=False, separators=(",", ":")) + "\n"
    )
    print(json.dumps({"id": manifest["id"], "parts": len(parts), "slotsPerGender": len(SLOTS), "pngBytes": manifest["pngBytes"]}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
