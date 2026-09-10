#!/usr/bin/env python3
"""Pack Võ Lv1/10/20/30 paper-doll tiers and two-gender motion for runtime."""

from __future__ import annotations

import argparse
import json
import math
from pathlib import Path

from PIL import Image

from pack_lgo_vo_lv1_map_avatar import (
    ATLAS_SIZE, CANVAS, GROUND_SOURCE_Y, MOTION_FRAMES as LEGACY_MOTION_FRAMES, SLOTS,
    TARGET_FULL_HEIGHT, WORLD_HEIGHT, digest, shelf_pack, skill_slash,
)

LEVELS = (1, 10, 20, 30)
MOTION_TARGET_HEIGHT = 230
NEW_MOTION_FRAMES = (
    "run_a", "run_b", "jump_rise", "jump_apex",
    "basic_windup", "basic_impact", "lien_quyen_hit_a", "lien_quyen_finish_b",
)
MOTION_FRAMES = (*LEGACY_MOTION_FRAMES, *NEW_MOTION_FRAMES)
RIG_PARTS = (
    "head", "torso-hips", "left-upper-arm", "left-forearm-hand", "right-upper-arm",
    "right-forearm-hand", "left-thigh", "left-shin-foot", "right-thigh", "right-shin-foot",
)
RIG_HEIGHTS = {
    "head": .42, "torso-hips": .72,
    "left-upper-arm": .46, "right-upper-arm": .46,
    "left-forearm-hand": .45, "right-forearm-hand": .45,
    "left-thigh": .55, "right-thigh": .55,
    "left-shin-foot": .54, "right-shin-foot": .54,
}
RIG_REST = {
    "head": (0, 1.48), "torso-hips": (0, 1.02),
    "left-upper-arm": (-.19, 1.12), "left-forearm-hand": (-.24, .78),
    "right-upper-arm": (.19, 1.12), "right-forearm-hand": (.24, .78),
    "left-thigh": (-.10, .58), "left-shin-foot": (-.10, .25),
    "right-thigh": (.10, .58), "right-shin-foot": (.10, .25),
}
RIG_ORDERS = {
    "head": 8, "torso-hips": 7,
    "left-upper-arm": 6, "left-forearm-hand": 6, "right-upper-arm": 8, "right-forearm-hand": 8,
    "left-thigh": 6, "left-shin-foot": 6, "right-thigh": 7, "right-shin-foot": 7,
}
RIG_PIVOTS = {
    "head": (0, 1.32), "torso-hips": (0, .70),
    "left-upper-arm": (-.17, 1.30), "left-forearm-hand": (-.22, .95),
    "right-upper-arm": (.17, 1.30), "right-forearm-hand": (.22, .95),
    "left-thigh": (-.09, .75), "left-shin-foot": (-.10, .40),
    "right-thigh": (.09, .75), "right-shin-foot": (.10, .40),
}
RIG_PARENTS = {
    "torso-hips": "", "head": "torso-hips",
    "left-upper-arm": "torso-hips", "left-forearm-hand": "left-upper-arm",
    "right-upper-arm": "torso-hips", "right-forearm-hand": "right-upper-arm",
    "left-thigh": "torso-hips", "left-shin-foot": "left-thigh",
    "right-thigh": "torso-hips", "right-shin-foot": "right-thigh",
}
NEW_MOTION_WORLD_HEIGHTS = {
    "run_a": 1.68, "run_b": 1.68, "jump_rise": 1.68, "jump_apex": 1.36,
    "basic_windup": 1.48, "basic_impact": 1.48,
    "lien_quyen_hit_a": 1.48, "lien_quyen_finish_b": 1.48,
}

POSE_DELTAS = {
    "idle": (0.0, 0.0, 0.0),
    "walk": (0.018, 0.008, 2.0),
    "run": (0.055, 0.018, -7.0),
    "jump": (0.0, 0.075, -4.0),
    "basic_attack": (0.075, 0.018, -11.0),
    "skill": (0.105, 0.028, -16.0),
}
SLOT_POSE_FACTORS = {
    "main_weapon": (1.6, 1.1, 2.0),
    "head_hair": (.35, 1.25, .55),
    "inner_top": (.55, .70, .55),
    "outer_tunic": (.75, .80, .85),
    "lower_garment": (.45, .55, .70),
    "waist": (.65, .65, .75),
    "arm_guard": (1.25, .80, 1.55),
    "boots": (.85, 1.35, 1.10),
    "light_armor": (.60, .75, .65),
    "accessory": (.95, 1.05, 1.20),
}


def attachment_profiles() -> list[dict]:
    """Small pose deltas shared by all four equipment tiers."""
    profiles = []
    for gender in ("male", "female"):
        direction = 1.0 if gender == "male" else .92
        for pose, (base_x, base_y, base_rotation) in POSE_DELTAS.items():
            for slot in SLOTS:
                factor_x, factor_y, factor_rotation = SLOT_POSE_FACTORS[slot]
                rotation = base_rotation * factor_rotation * direction
                scale_x = 1.0
                scale_y = 1.0
                if pose == "run" and slot in {"outer_tunic", "lower_garment", "accessory"}:
                    scale_x, scale_y = 1.035, .985
                elif pose in {"basic_attack", "skill"} and slot in {"main_weapon", "arm_guard"}:
                    scale_x, scale_y = 1.045, .975
                profiles.append({
                    "id": f"{gender}_{pose}_{slot}", "gender": gender, "pose": pose, "slot": slot,
                    "dx": round(base_x * factor_x * direction, 4),
                    "dy": round(base_y * factor_y, 4),
                    "rotation": round(rotation, 3), "scaleX": scale_x, "scaleY": scale_y,
                })
    return profiles


def rig_pose_profiles() -> list[dict]:
    profiles = []
    rotations = {
        "idle": {},
        "walk": {"left-upper-arm": 10, "left-forearm-hand": 5, "right-upper-arm": -10,
                 "right-forearm-hand": -5, "left-thigh": -10, "left-shin-foot": 7,
                 "right-thigh": 10, "right-shin-foot": -7},
        "run": {"head": 4, "torso-hips": -7, "left-upper-arm": 22, "left-forearm-hand": 14,
                "right-upper-arm": -24, "right-forearm-hand": -18, "left-thigh": -24,
                "left-shin-foot": 18, "right-thigh": 24, "right-shin-foot": -18},
        "jump": {"head": -3, "torso-hips": -3, "left-upper-arm": -25, "left-forearm-hand": -18,
                 "right-upper-arm": -35, "right-forearm-hand": -25, "left-thigh": 28,
                 "left-shin-foot": -35, "right-thigh": -18, "right-shin-foot": 30},
        "basic_attack": {"head": -5, "torso-hips": -9, "left-upper-arm": 28, "left-forearm-hand": 18,
                         "right-upper-arm": -58, "right-forearm-hand": -72, "left-thigh": 14,
                         "right-thigh": -16},
        "skill": {"head": -8, "torso-hips": -13, "left-upper-arm": -42, "left-forearm-hand": -64,
                  "right-upper-arm": -78, "right-forearm-hand": -96, "left-thigh": 22,
                  "left-shin-foot": -18, "right-thigh": -25, "right-shin-foot": 22},
    }
    for gender in ("male", "female"):
        direction = 1 if gender == "male" else .92
        for pose, values in rotations.items():
            for part in RIG_PARTS:
                world_rotation = values.get(part, 0) * direction
                parent = RIG_PARENTS[part]
                parent_world_rotation = values.get(parent, 0) * direction if parent else 0
                rotation = world_rotation - parent_world_rotation
                center_x, center_y = RIG_REST[part]
                pivot_x, pivot_y = RIG_PIVOTS[part]
                radians = math.radians(rotation)
                offset_x, offset_y = center_x - pivot_x, center_y - pivot_y
                rotated_x = offset_x * math.cos(radians) - offset_y * math.sin(radians)
                rotated_y = offset_x * math.sin(radians) + offset_y * math.cos(radians)
                profiles.append({"id": f"{gender}_{pose}_{part}", "gender": gender, "pose": pose,
                                 "part": part, "dx": round(pivot_x + rotated_x - center_x, 4),
                                 "dy": round(pivot_y + rotated_y - center_y, 4),
                                 "rotation": round(rotation, 3)})
    return profiles


def pack_rig(rig_dir: Path, output: Path) -> tuple[list[dict], dict]:
    entries = []
    for gender in ("male", "female"):
        for index, part in enumerate(RIG_PARTS, 1):
            path = rig_dir / gender / f"{index:02d}-{part}.png"
            source = Image.open(path).convert("RGBA")
            box = source.getchannel("A").getbbox()
            if source.size != (512, 512) or box is None:
                raise ValueError(f"Invalid rig part: {path}")
            target_height = round(MOTION_TARGET_HEIGHT * RIG_HEIGHTS[part] / WORLD_HEIGHT)
            scale = target_height / (box[3] - box[1])
            image = source.crop(box).resize((round((box[2] - box[0]) * scale), target_height), Image.Resampling.LANCZOS)
            entries.append({"id": f"{gender}_{part}", "gender": gender, "part": part,
                            "path": path, "image": image})
    shelf_pack(entries)
    atlas = Image.new("RGBA", (ATLAS_SIZE, ATLAS_SIZE))
    packed = []
    for entry in entries:
        image = entry["image"]
        atlas.alpha_composite(image, (entry["left"], entry["top"]))
        dx, dy = RIG_REST[entry["part"]]
        pivot_x, pivot_y = RIG_PIVOTS[entry["part"]]
        packed.append({"id": entry["id"], "gender": entry["gender"], "part": entry["part"],
                       "parent": RIG_PARENTS[entry["part"]], "pivotX": pivot_x, "pivotY": pivot_y,
                       "atlas": "rig", "x": entry["left"], "y": ATLAS_SIZE - entry["top"] - image.height,
                       "w": image.width, "h": image.height,
                       "worldW": image.width / image.height * RIG_HEIGHTS[entry["part"]],
                       "worldH": RIG_HEIGHTS[entry["part"]], "dx": dx, "dy": dy,
                       "order": RIG_ORDERS[entry["part"]],
                       "sourceSha256": digest(entry["path"])})
    path = output / "vo-lv1-rig-atlas.png"
    atlas.quantize(colors=256, method=Image.Quantize.FASTOCTREE, dither=Image.Dither.NONE).save(path, optimize=True, compress_level=9)
    return packed, {"id": "rig", "file": path.name, "sha256": digest(path), "pngBytes": path.stat().st_size}


def static_source(root: Path, progression: Path, level: int) -> Path:
    return root if level == 1 else progression / f"lv{level:03d}"


def equipment_components(entry: dict, level: int) -> list[dict]:
    """Map every equipment slot to a rig bone without duplicating atlas pixels."""
    if entry["kind"] != "slot":
        return []
    image, box = entry["image"], entry["box"]
    scale = TARGET_FULL_HEIGHT / (GROUND_SOURCE_Y - 16)
    center_bone_by_slot = {
        "main_weapon": "right-forearm-hand", "head_hair": "head",
        "inner_top": "torso-hips", "outer_tunic": "torso-hips",
        "lower_garment": "torso-hips", "waist": "torso-hips",
        "light_armor": "torso-hips", "accessory": "torso-hips",
    }
    if entry["slot"] in {"arm_guard", "boots"}:
        split = max(1, min(image.width - 1, round((CANVAS[0] * .5 - box[0]) * scale)))
        pieces = (("left", 0, split), ("right", split, image.width - split))
        bone_prefix = "forearm-hand" if entry["slot"] == "arm_guard" else "shin-foot"
        bone_by_side = {side: f"{side}-{bone_prefix}" for side in ("left", "right")}
    else:
        pieces = (("center", 0, image.width),)
        bone_by_side = {"center": center_bone_by_slot[entry["slot"]]}
    components = []
    for side, start, width in pieces:
        source_left = box[0] + start / scale
        source_right = source_left + width / scale
        source_cx = (source_left + source_right) * .5
        source_cy = (box[1] + box[3]) * .5
        components.append({
            "id": f"lv{level:03d}_{entry['gender']}_{entry['slot']}_{side}",
            "level": level, "gender": entry["gender"], "slot": entry["slot"], "side": side,
            "bone": bone_by_side[side], "atlas": f"lv{level:03d}",
            "x": entry["left"] + start, "y": ATLAS_SIZE - entry["top"] - image.height,
            "w": width, "h": image.height, "order": entry["order"],
            "worldW": (source_right - source_left) / CANVAS[1] * WORLD_HEIGHT,
            "worldH": (box[3] - box[1]) / CANVAS[1] * WORLD_HEIGHT,
            "dx": (source_cx - CANVAS[0] * .5) / CANVAS[1] * WORLD_HEIGHT,
            "dy": (GROUND_SOURCE_Y - source_cy) / CANVAS[1] * WORLD_HEIGHT,
        })
    return components


def pack_static(source_dir: Path, level: int, output: Path) -> tuple[list[dict], list[dict], dict, dict | None]:
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
    parts, components, effect = [], [], None
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
        components.extend(equipment_components(entry, level))
    filename = "vo-lv1-map-avatar-atlas.png" if level == 1 else f"vo-lv{level}-equipment-atlas.png"
    path = output / filename
    atlas.quantize(colors=256, method=Image.Quantize.FASTOCTREE, dither=Image.Dither.NONE).save(path, optimize=True, compress_level=9)
    return parts, components, {"id": f"lv{level:03d}", "file": filename, "sha256": digest(path), "pngBytes": path.stat().st_size}, effect


def pack_motion(source_dir: Path, extended_dir: Path, gender: str, output: Path) -> tuple[list[dict], dict]:
    idle = Image.open(source_dir / f"motion-{gender}" / "idle.png").convert("RGBA")
    idle_box = idle.getchannel("A").getbbox()
    scale = MOTION_TARGET_HEIGHT / (idle_box[3]-idle_box[1])
    entries = []
    for frame_id in MOTION_FRAMES:
        path = ((source_dir / f"motion-{gender}") if frame_id in LEGACY_MOTION_FRAMES else extended_dir) / f"{frame_id}.png"
        source = Image.open(path).convert("RGBA"); box = source.getchannel("A").getbbox()
        if box is None or source.size != (512, 512):
            raise ValueError(f"Invalid motion frame: {path}")
        if frame_id in NEW_MOTION_WORLD_HEIGHTS:
            world_height = NEW_MOTION_WORLD_HEIGHTS[frame_id]
            target_height = round(MOTION_TARGET_HEIGHT * world_height / WORLD_HEIGHT)
            frame_scale = target_height / (box[3] - box[1])
            dx, dy = 0.0, world_height * .5
        else:
            world_height = (box[3] - box[1]) / ((idle_box[3] - idle_box[1]) / WORLD_HEIGHT)
            frame_scale = scale
            cx, cy = (box[0]+box[2])*.5, (box[1]+box[3])*.5
            dx = (cx-256) / ((idle_box[3]-idle_box[1])/WORLD_HEIGHT)
            dy = (510-cy) / ((idle_box[3]-idle_box[1])/WORLD_HEIGHT)
        image = source.crop(box).resize((round((box[2]-box[0])*frame_scale), round((box[3]-box[1])*frame_scale)), Image.Resampling.LANCZOS)
        entries.append({"id": frame_id, "path": path, "box": box, "image": image,
                        "worldH": world_height, "dx": dx, "dy": dy})
    shelf_pack(entries)
    atlas = Image.new("RGBA", (ATLAS_SIZE, ATLAS_SIZE)); frames = []
    for entry in entries:
        image = entry["image"]; box = entry["box"]
        atlas.alpha_composite(image, (entry["left"], entry["top"]))
        frames.append({"id": f"{gender}_{entry['id']}", "gender": gender, "atlas": f"motion_{gender}",
                       "x": entry["left"], "y": ATLAS_SIZE-entry["top"]-image.height, "w": image.width, "h": image.height,
                       "worldW": (box[2]-box[0])/(box[3]-box[1])*entry["worldH"], "worldH": entry["worldH"],
                       "dx": entry["dx"], "dy": entry["dy"], "order": 19,
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
    parser.add_argument("--extended-motion-male-dir", type=Path, required=True)
    parser.add_argument("--extended-motion-female-dir", type=Path, required=True)
    parser.add_argument("--rig-dir", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    args = parser.parse_args()
    if args.output_dir.exists(): raise FileExistsError("Use a new output directory")
    args.output_dir.mkdir(parents=True)
    parts, components, atlases = [], [], []
    effect = None
    for level in LEVELS:
        level_parts, level_components, atlas, level_effect = pack_static(
            static_source(args.lv1_dir, args.progression_dir, level), level, args.output_dir)
        parts.extend(level_parts); components.extend(level_components); atlases.append(atlas)
        if level_effect is not None: effect = level_effect
    if effect is None: raise ValueError("Missing Võ skill effect")
    motion, motion_atlases = [], []
    male_frames, male_atlas = pack_motion(args.lv1_dir, args.extended_motion_male_dir, "male", args.output_dir)
    female_frames, female_atlas = pack_motion(args.progression_dir, args.extended_motion_female_dir, "female", args.output_dir)
    motion += male_frames + female_frames; motion_atlases += [male_atlas, female_atlas]
    rig_parts, rig_atlas = pack_rig(args.rig_dir, args.output_dir)
    resource_root = "client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1MapAvatarArt/"
    atlas_records = atlases + motion_atlases + [rig_atlas]
    assets = []
    for atlas in atlas_records:
        is_motion = atlas["id"].startswith("motion_")
        assets.append({"path": resource_root + atlas["file"], "sha256": atlas["sha256"],
                       "width": ATLAS_SIZE, "height": ATLAS_SIZE,
                       "role": "two-gender-motion-atlas" if is_motion else (
                           "two-gender-skeletal-rig-atlas" if atlas["id"] == "rig" else "two-gender-tier-equipment-atlas"),
                       "generator": "reference_guided_imagegen_motion_batch" if is_motion else (
                           "reference_guided_imagegen_rig_batch" if atlas["id"] == "rig" else "aligned_imagegen_delta_batch"),
                       "referenceOnly": False})
    manifest = {"id": "vo-lv1-30-map-avatar-v7", "status": "DRAFT_RUNTIME_REVIEW", "classId": "vo",
                "levels": list(LEVELS), "genders": ["male", "female"], "slots": list(SLOTS),
                "atlases": atlas_records, "assets": assets, "parts": parts, "effects": [effect], "motionFrames": motion,
                "attachmentProfiles": attachment_profiles(),
                "equipmentComponents": components,
                "rigParts": rig_parts, "rigPoseProfiles": rig_pose_profiles(),
                "nonClaims": ["reusable hierarchical paper-doll rig technical checkpoint",
                              "garment attachment art visual fix required",
                              "animated paper-doll attachment review required"]}
    (args.output_dir / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, separators=(",", ":"))+"\n")
    print(json.dumps({"id": manifest["id"], "parts": len(parts), "equipmentComponents": len(components), "motionFrames": len(motion),
                      "pngBytes": sum(a["pngBytes"] for a in manifest["atlases"])}))
    return 0


if __name__ == "__main__": raise SystemExit(main())
