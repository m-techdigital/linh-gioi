#!/usr/bin/env python3
"""Pack Võ Lv1/10/20/30 paper-doll tiers and two-gender motion for runtime."""

from __future__ import annotations

import argparse
import json
from pathlib import Path

from PIL import Image

from pack_lgo_vo_lv1_map_avatar import (
    ATLAS_SIZE, CANVAS, GROUND_SOURCE_Y, MOTION_FRAMES, SLOTS,
    TARGET_FULL_HEIGHT, WORLD_HEIGHT, digest, shelf_pack, skill_slash,
)

LEVELS = (1, 10, 20, 30)


def static_source(root: Path, progression: Path, level: int) -> Path:
    return root if level == 1 else progression / f"lv{level:03d}"


def pack_static(source_dir: Path, level: int, output: Path) -> tuple[list[dict], dict, dict | None]:
    scale = TARGET_FULL_HEIGHT / (GROUND_SOURCE_Y - 16)
    entries = []
    order_by_slot = {slot: 8 + index for index, slot in enumerate(SLOTS)}
    for gender in ("male", "female"):
        for kind, filename in (("base", "base.png"), ("full", "full.png")):
            path = source_dir / gender / filename
            source = Image.open(path).convert("RGBA")
            box = source.getchannel("A").getbbox()
            if source.size != CANVAS or box is None:
                raise ValueError(f"Invalid source: {path}")
            image = source.crop(box).resize((round((box[2]-box[0])*scale), round((box[3]-box[1])*scale)), Image.Resampling.LANCZOS)
            entries.append({"id": f"lv{level:03d}_{gender}_{kind}", "gender": gender, "kind": kind,
                            "slot": "", "order": 6 if kind == "base" else 18, "path": path, "box": box, "image": image})
        for slot in SLOTS:
            path = source_dir / gender / f"slot-{slot}.png"
            source = Image.open(path).convert("RGBA")
            box = source.getchannel("A").getbbox()
            if source.size != CANVAS or box is None:
                raise ValueError(f"Invalid slot: {path}")
            image = source.crop(box).resize((max(1, round((box[2]-box[0])*scale)), max(1, round((box[3]-box[1])*scale))), Image.Resampling.LANCZOS)
            entries.append({"id": f"lv{level:03d}_{gender}_slot_{slot}", "gender": gender, "kind": "slot",
                            "slot": slot, "order": order_by_slot[slot], "path": path, "box": box, "image": image})
    if level == 1:
        entries.append({"id": "skill_slash", "gender": "shared", "kind": "effect", "slot": "", "order": 24,
                        "path": None, "box": (0, 0, 120, 120), "image": skill_slash()})
    shelf_pack(entries)
    atlas = Image.new("RGBA", (ATLAS_SIZE, ATLAS_SIZE))
    parts, effect = [], None
    for entry in entries:
        image = entry["image"]
        atlas.alpha_composite(image, (entry["left"], entry["top"]))
        box = entry["box"]
        packed = {"id": entry["id"], "level": level, "gender": entry["gender"], "kind": entry["kind"],
                  "slot": entry["slot"], "atlas": f"lv{level:03d}", "x": entry["left"],
                  "y": ATLAS_SIZE-entry["top"]-image.height, "w": image.width, "h": image.height,
                  "order": entry["order"]}
        if entry["kind"] == "effect":
            effect = packed | {"worldW": 2.45, "worldH": 2.20, "dx": 1.08, "dy": .58}
            continue
        width, height = box[2]-box[0], box[3]-box[1]
        cx, cy = (box[0]+box[2])*.5, (box[1]+box[3])*.5
        packed |= {"worldW": width/CANVAS[1]*WORLD_HEIGHT, "worldH": height/CANVAS[1]*WORLD_HEIGHT,
                   "dx": (cx-CANVAS[0]*.5)/CANVAS[1]*WORLD_HEIGHT,
                   "dy": (GROUND_SOURCE_Y-cy)/CANVAS[1]*WORLD_HEIGHT,
                   "source": str(entry["path"].relative_to(source_dir)), "sourceSha256": digest(entry["path"])}
        parts.append(packed)
    filename = "vo-lv1-map-avatar-atlas.png" if level == 1 else f"vo-lv{level}-equipment-atlas.png"
    path = output / filename
    atlas.quantize(colors=256, method=Image.Quantize.FASTOCTREE, dither=Image.Dither.NONE).save(path, optimize=True, compress_level=9)
    return parts, {"id": f"lv{level:03d}", "file": filename, "sha256": digest(path), "pngBytes": path.stat().st_size}, effect


def pack_motion(source_dir: Path, gender: str, output: Path) -> tuple[list[dict], dict]:
    idle = Image.open(source_dir / f"motion-{gender}" / "idle.png").convert("RGBA")
    idle_box = idle.getchannel("A").getbbox()
    scale = TARGET_FULL_HEIGHT / (idle_box[3]-idle_box[1])
    entries = []
    for frame_id in MOTION_FRAMES:
        path = source_dir / f"motion-{gender}" / f"{frame_id}.png"
        source = Image.open(path).convert("RGBA"); box = source.getchannel("A").getbbox()
        image = source.crop(box).resize((round((box[2]-box[0])*scale), round((box[3]-box[1])*scale)), Image.Resampling.LANCZOS)
        entries.append({"id": frame_id, "path": path, "box": box, "image": image})
    shelf_pack(entries)
    atlas = Image.new("RGBA", (ATLAS_SIZE, ATLAS_SIZE)); frames = []
    pixels_per_world = (idle_box[3]-idle_box[1])/WORLD_HEIGHT
    for entry in entries:
        image = entry["image"]; box = entry["box"]
        atlas.alpha_composite(image, (entry["left"], entry["top"]))
        cx, cy = (box[0]+box[2])*.5, (box[1]+box[3])*.5
        frames.append({"id": f"{gender}_{entry['id']}", "gender": gender, "atlas": f"motion_{gender}",
                       "x": entry["left"], "y": ATLAS_SIZE-entry["top"]-image.height, "w": image.width, "h": image.height,
                       "worldW": (box[2]-box[0])/pixels_per_world, "worldH": (box[3]-box[1])/pixels_per_world,
                       "dx": (cx-256)/pixels_per_world, "dy": (510-cy)/pixels_per_world, "order": 19,
                       "sourceSha256": digest(entry["path"])})
    filename = f"vo-lv1-{gender}-motion-atlas.png"
    if gender == "male": filename = "vo-lv1-motion-atlas.png"
    path = output / filename
    atlas.quantize(colors=256, method=Image.Quantize.FASTOCTREE, dither=Image.Dither.NONE).save(path, optimize=True, compress_level=9)
    return frames, {"id": f"motion_{gender}", "file": filename, "sha256": digest(path), "pngBytes": path.stat().st_size}


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--lv1-dir", type=Path, required=True)
    parser.add_argument("--progression-dir", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    args = parser.parse_args()
    if args.output_dir.exists(): raise FileExistsError("Use a new output directory")
    args.output_dir.mkdir(parents=True)
    parts, atlases = [], []
    effect = None
    for level in LEVELS:
        level_parts, atlas, level_effect = pack_static(static_source(args.lv1_dir, args.progression_dir, level), level, args.output_dir)
        parts.extend(level_parts); atlases.append(atlas)
        if level_effect is not None: effect = level_effect
    if effect is None: raise ValueError("Missing Võ skill effect")
    motion, motion_atlases = [], []
    male_frames, male_atlas = pack_motion(args.lv1_dir, "male", args.output_dir)
    female_frames, female_atlas = pack_motion(args.progression_dir, "female", args.output_dir)
    motion += male_frames + female_frames; motion_atlases += [male_atlas, female_atlas]
    resource_root = "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1MapAvatarArt/"
    atlas_records = atlases + motion_atlases
    assets = []
    for atlas in atlas_records:
        is_motion = atlas["id"].startswith("motion_")
        assets.append({"path": resource_root + atlas["file"], "sha256": atlas["sha256"],
                       "width": ATLAS_SIZE, "height": ATLAS_SIZE,
                       "role": "two-gender-motion-atlas" if is_motion else "two-gender-tier-equipment-atlas",
                       "generator": "reference_guided_imagegen_motion_batch" if is_motion else "aligned_imagegen_delta_batch",
                       "referenceOnly": False})
    manifest = {"id": "vo-lv1-30-map-avatar-v3", "status": "DRAFT_RUNTIME_REVIEW", "classId": "vo",
                "levels": list(LEVELS), "genders": ["male", "female"], "slots": list(SLOTS),
                "atlases": atlas_records, "assets": assets, "parts": parts, "effects": [effect], "motionFrames": motion,
                "nonClaims": ["Lv1/10/20/30 visual progression checkpoint", "one skill preview", "requires Player review"]}
    (args.output_dir / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, separators=(",", ":"))+"\n")
    print(json.dumps({"id": manifest["id"], "parts": len(parts), "motionFrames": len(motion),
                      "pngBytes": sum(a["pngBytes"] for a in manifest["atlases"])}))
    return 0


if __name__ == "__main__": raise SystemExit(main())
