#!/usr/bin/env python3
"""Split the reviewed Võ Lv10/20/30 progression sheet and female motion sheet."""

from __future__ import annotations

import argparse
import json
from pathlib import Path

from PIL import Image

from prepare_lgo_vo_lv1_generated_batch import (
    MOTION_FRAMES,
    digest,
    key_magenta,
    prepare_gender,
)


SHEET_SIZE = (1024, 1536)
CELL_SIZE = (SHEET_SIZE[0] // 3, SHEET_SIZE[1] // 2)
LEVELS = (10, 20, 30)


def align_cell(cell: Image.Image, base: Image.Image) -> Image.Image:
    source_box = cell.getchannel("A").getbbox()
    target_box = base.getchannel("A").getbbox()
    if source_box is None or target_box is None:
        raise ValueError("Empty progression cell or base")
    cropped = cell.crop(source_box).resize(
        (target_box[2] - target_box[0], target_box[3] - target_box[1]),
        Image.Resampling.LANCZOS,
    )
    aligned = Image.new("RGBA", base.size)
    aligned.alpha_composite(cropped, (target_box[0], target_box[1]))
    return aligned


def prepare_progression(sheet_path: Path, base_dir: Path, output: Path) -> list[dict]:
    sheet = key_magenta(Image.open(sheet_path))
    if sheet.size != SHEET_SIZE:
        raise ValueError(f"Progression sheet must be {SHEET_SIZE}: {sheet.size}")
    aligned_dir = output / "aligned-candidates"
    aligned_dir.mkdir()
    records = []
    for row, gender in enumerate(("male", "female")):
        base_path = base_dir / ("07-common-male-base-aligned-draft.png" if gender == "male"
                                else "08-common-female-base-aligned-draft.png")
        base = Image.open(base_path).convert("RGBA")
        for column, level in enumerate(LEVELS):
            left = column * CELL_SIZE[0]
            right = SHEET_SIZE[0] if column == 2 else (column + 1) * CELL_SIZE[0]
            cell = sheet.crop((left, row * CELL_SIZE[1], right, (row + 1) * CELL_SIZE[1]))
            candidate = align_cell(cell, base)
            candidate_path = aligned_dir / f"{gender}-lv{level:03d}.png"
            candidate.save(candidate_path, optimize=True)
            level_dir = output / f"lv{level:03d}"
            level_dir.mkdir(exist_ok=True)
            records.append(prepare_gender(gender, candidate_path, base_path, level_dir) | {"level": level})
    return records


def prepare_female_motion(sheet_path: Path, output: Path) -> dict:
    source = key_magenta(Image.open(sheet_path))
    if source.size != (1536, 1024):
        raise ValueError(f"Female motion sheet must be 1536x1024: {source.size}")
    motion_dir = output / "motion-female"
    motion_dir.mkdir()
    frames = []
    for index, frame_id in enumerate(MOTION_FRAMES):
        left, top = index % 3 * 512, index // 3 * 512
        frame = source.crop((left, top, left + 512, top + 512))
        box = frame.getchannel("A").getbbox()
        if box is None or box[2] - box[0] < 80 or box[3] - box[1] < 180:
            raise ValueError(f"Female motion cell {frame_id} is empty or clipped: {box}")
        frame.save(motion_dir / f"{frame_id}.png", optimize=True)
        frames.append({"id": frame_id, "cell": [index % 3, index // 3], "bbox": list(box)})
    return {
        "source": str(sheet_path), "sourceSha256": digest(sheet_path),
        "grid": [3, 2], "cell": [512, 512], "frames": frames,
    }


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--progression-sheet", type=Path, required=True)
    parser.add_argument("--female-motion-sheet", type=Path, required=True)
    parser.add_argument("--base-dir", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    args = parser.parse_args()
    if args.output_dir.exists():
        raise FileExistsError("Use a new versioned output directory: " + str(args.output_dir))
    args.output_dir.mkdir(parents=True)
    records = prepare_progression(args.progression_sheet, args.base_dir, args.output_dir)
    motion = prepare_female_motion(args.female_motion_sheet, args.output_dir)
    report = {
        "id": "vo-lv010-030-progression-and-female-motion-v1",
        "status": "DRAFT_REQUIRES_PLAYER_REVIEW",
        "classId": "vo",
        "levels": list(LEVELS),
        "records": records,
        "femaleMotion": motion,
        "notes": [
            "All six progression outfits came from one coherent reviewed sheet.",
            "Each cell is aligned to the recovered common base before the ten-slot delta split.",
            "Source candidates stay outside Unity; only display-sized atlases may enter Resources.",
        ],
    }
    (args.output_dir / "manifest.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n")
    print(json.dumps({"id": report["id"], "outfits": len(records), "femaleMotionFrames": len(motion["frames"])}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
