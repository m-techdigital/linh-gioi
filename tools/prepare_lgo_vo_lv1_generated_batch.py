#!/usr/bin/env python3
"""Key and split one aligned Võ Lv1 male/female imagegen batch into paper-doll layers."""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageChops, ImageDraw


CANVAS = (1024, 1536)
SLOTS = (
    "main_weapon", "head_hair", "inner_top", "outer_tunic", "lower_garment",
    "waist", "arm_guard", "boots", "light_armor", "accessory",
)
MOTION_FRAMES = ("idle", "walk_a", "walk_b", "dash", "punch_windup", "punch_impact")


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def key_magenta(image: Image.Image) -> Image.Image:
    keyed = image.convert("RGBA")
    pixels = []
    for r, g, b, a in keyed.getdata():
        score = min(r, b) - g
        if score >= 100:
            alpha = 0
        elif score <= 48:
            alpha = a
        else:
            alpha = round(a * (100 - score) / 52)
        pixels.append((r, g, b, alpha) if alpha else (0, 0, 0, 0))
    keyed.putdata(pixels)
    return keyed


def is_skin(r: int, g: int, b: int) -> bool:
    return r > 135 and g > 90 and b > 75 and 4 < r - g < 95 and 4 < g - b < 90


def slot_for(gender: str, x: int, y: int, rgb: tuple[int, int, int]) -> str | None:
    r, g, b = rgb
    red = r > 105 and r > g * 1.35 and r > b * 1.20
    pale = min(r, g, b) > 145 and max(r, g, b) - min(r, g, b) < 75
    gold = r > 115 and g > 70 and r > b * 1.35 and g > b * 1.15
    if gender == "male":
        if y < 235:
            return "head_hair"
        if y >= 1260:
            return "boots"
        if (270 <= x <= 430 or 625 <= x <= 760) and 525 <= y <= 830:
            if is_skin(r, g, b):
                return None
            return "main_weapon" if y >= 690 else "arm_guard"
        if 500 <= y <= 970 and (red or pale) and not is_skin(r, g, b):
            return "accessory"
        if 500 <= y < 690:
            return "waist"
        if 670 <= y < 1260:
            return "lower_garment"
        if 220 <= y < 545 and 360 <= x <= 655:
            if y < 380 and (x < 480 or x > 560) and gold:
                return "light_armor"
            return "outer_tunic" if gold or pale or red else "inner_top"
    else:
        if y < 330:
            return "head_hair"
        if y >= 1280:
            return "boots"
        if (350 <= x <= 465 or 590 <= x <= 700) and 540 <= y <= 860:
            if is_skin(r, g, b):
                return None
            return "main_weapon" if y >= 725 else "arm_guard"
        if 560 <= y <= 900 and (red or pale) and not is_skin(r, g, b):
            return "accessory"
        if 555 <= y < 700:
            return "waist"
        if 680 <= y < 830:
            return "lower_garment"
        if 300 <= y < 575 and 415 <= x <= 635:
            if y < 440 and (x < 525 or x > 540) and gold:
                return "light_armor"
            return "outer_tunic" if gold or pale or red else "inner_top"
    return None


def prepare_gender(gender: str, candidate_path: Path, base_path: Path, output: Path) -> dict:
    candidate = key_magenta(Image.open(candidate_path))
    base = Image.open(base_path).convert("RGBA")
    if candidate.size != CANVAS or base.size != CANVAS:
        raise ValueError(f"{gender}: source must preserve {CANVAS}")
    base_bbox = base.getchannel("A").getbbox()
    raw_full_bbox = candidate.getchannel("A").getbbox()
    if raw_full_bbox is None or base_bbox is None:
        raise ValueError(f"{gender}: empty source")
    if max(abs(raw_full_bbox[i] - base_bbox[i]) for i in range(4)) > 3:
        raise ValueError(f"{gender}: imagegen changed the reviewed alignment: {raw_full_bbox} vs {base_bbox}")
    shift_x = base_bbox[0] - raw_full_bbox[0]
    shift_y = base_bbox[3] - raw_full_bbox[3]
    aligned = Image.new("RGBA", CANVAS)
    aligned.alpha_composite(candidate, (shift_x, shift_y))
    candidate = aligned
    full_bbox = candidate.getchannel("A").getbbox()

    delta = ImageChops.difference(candidate.convert("RGB"), base.convert("RGB"))
    delta_pixels = list(delta.getdata())
    source_pixels = list(candidate.getdata())
    base_alpha = list(base.getchannel("A").getdata())
    layers = {slot: Image.new("RGBA", CANVAS) for slot in SLOTS}
    layer_pixels = {slot: [(0, 0, 0, 0)] * (CANVAS[0] * CANVAS[1]) for slot in SLOTS}
    counts = {slot: 0 for slot in SLOTS}
    for index, ((r, g, b, a), diff, under_alpha) in enumerate(zip(source_pixels, delta_pixels, base_alpha)):
        if not a:
            continue
        # Keep newly visible silhouette pixels and meaningful visual changes. Minor redraw noise stays in base.
        if under_alpha and max(diff) < 32:
            continue
        x = index % CANVAS[0]
        y = index // CANVAS[0]
        slot = slot_for(gender, x, y, (r, g, b))
        if slot is None:
            continue
        layer_pixels[slot][index] = (r, g, b, a)
        counts[slot] += 1
    for slot in SLOTS:
        if counts[slot] < 24:
            raise ValueError(f"{gender}: empty/weak generated slot {slot}: {counts[slot]} pixels")
        layers[slot].putdata(layer_pixels[slot])

    gender_dir = output / gender
    gender_dir.mkdir(parents=True)
    base.save(gender_dir / "base.png", optimize=True)
    candidate.save(gender_dir / "full.png", optimize=True)
    composed = base.copy()
    for slot in SLOTS:
        layers[slot].save(gender_dir / f"slot-{slot}.png", optimize=True)
        composed.alpha_composite(layers[slot])
    composed.save(gender_dir / "modular-composed.png", optimize=True)

    review = Image.new("RGB", (512 * 4, 512 * 3), "#20303a")
    labels = ("base", "full", *SLOTS)
    images = (base, candidate, *(layers[slot] for slot in SLOTS))
    draw = ImageDraw.Draw(review)
    for index, (label, image) in enumerate(zip(labels, images)):
        thumb = image.copy()
        thumb.thumbnail((280, 440), Image.Resampling.LANCZOS)
        x = (index % 4) * 512 + (512 - thumb.width) // 2
        y = (index // 4) * 512 + 44
        review.paste(thumb, (x, y), thumb)
        draw.text((index % 4 * 512 + 12, index // 4 * 512 + 12), label, fill="#f5d87c")
    review.save(output / f"review-{gender}.jpg", quality=92)
    return {
        "gender": gender,
        "candidate": str(candidate_path),
        "candidateSha256": digest(candidate_path),
        "base": str(base_path),
        "baseSha256": digest(base_path),
        "canvas": list(CANVAS),
        "baseBbox": list(base_bbox),
        "rawFullBbox": list(raw_full_bbox),
        "fullBbox": list(full_bbox),
        "alignmentShift": [shift_x, shift_y],
        "slotPixels": counts,
    }


def prepare_motion(source_path: Path, output: Path) -> dict:
    source = key_magenta(Image.open(source_path))
    if source.size != (1536, 1024):
        raise ValueError(f"Motion sheet must be 3x2 at 1536x1024: {source.size}")
    motion_dir = output / "motion-male"
    motion_dir.mkdir()
    frames = []
    for index, frame_id in enumerate(MOTION_FRAMES):
        x, y = index % 3 * 512, index // 3 * 512
        frame = source.crop((x, y, x + 512, y + 512))
        box = frame.getchannel("A").getbbox()
        if box is None or box[2] - box[0] < 80 or box[3] - box[1] < 180:
            raise ValueError(f"Motion cell {frame_id} is empty or clipped: {box}")
        frame.save(motion_dir / f"{frame_id}.png", optimize=True)
        frames.append({"id": frame_id, "cell": [index % 3, index // 3], "bbox": list(box)})
    return {"source": str(source_path), "sourceSha256": digest(source_path), "grid": [3, 2], "cell": [512, 512], "frames": frames}


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--candidate-dir", type=Path, required=True)
    parser.add_argument("--base-dir", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    parser.add_argument("--motion-sheet", type=Path, required=True)
    args = parser.parse_args()
    if args.output_dir.exists():
        raise FileExistsError("Use a new versioned output directory: " + str(args.output_dir))
    args.output_dir.mkdir(parents=True)
    records = [
        prepare_gender("male", args.candidate_dir / "01-vo-male-lv001-full-equipped-magenta-candidate.png",
                       args.base_dir / "07-common-male-base-aligned-draft.png", args.output_dir),
        prepare_gender("female", args.candidate_dir / "02-vo-female-lv001-full-equipped-magenta-candidate.png",
                       args.base_dir / "08-common-female-base-aligned-draft.png", args.output_dir),
    ]
    motion = prepare_motion(args.motion_sheet, args.output_dir)
    report = {
        "id": "vo-lv001-generated-paper-doll-batch-v1",
        "status": "DRAFT_REQUIRES_PLAYER_REVIEW",
        "classId": "vo",
        "level": 1,
        "slots": list(SLOTS),
        "records": records,
        "motion": motion,
        "notes": [
            "Imagegen candidates are aligned edits of the recovered common bases.",
            "Layers contain visual deltas only; the common base retains skin and the training under-layer.",
            "Source boards remain redraw references and are not cropped directly into runtime assets.",
        ],
    }
    (args.output_dir / "manifest.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n")
    print(json.dumps({"id": report["id"], "genders": 2, "slotsPerGender": len(SLOTS)}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
