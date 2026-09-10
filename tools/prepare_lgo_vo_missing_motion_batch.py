#!/usr/bin/env python3
"""QA checkerboard-backed Võ motion sheets before any runtime import.

The tool deliberately refuses sheets whose silhouettes enter a cell safety margin.
This prevents generated hair, feet, cloth, or effects from being clipped when the
sheet is split into atlas frames.
"""

from __future__ import annotations

import argparse
import hashlib
import json
from collections import deque
from pathlib import Path

from PIL import Image


MOTION_FRAMES = (
    "run_a", "run_b", "jump_rise", "jump_apex",
    "basic_windup", "basic_impact", "lien_quyen_hit_a", "lien_quyen_finish_b",
)


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def _neutral_light(pixel: tuple[int, int, int]) -> bool:
    r, g, b = pixel
    # Image generation may add faint neutral watermark-like arcs over the
    # checkerboard. They can be much darker than the two nominal squares.
    return min(pixel) >= 130 and max(pixel) - min(pixel) <= 20


def border_connected_checker_mask(image: Image.Image) -> bytearray:
    """Return pixels belonging to a light neutral background connected to an edge.

    Flooding from the border protects light skin/costume regions enclosed by the
    character outline, unlike a global white/gray chroma key.
    """
    rgb = image.convert("RGB")
    width, height = rgb.size
    source = rgb.load()
    background = bytearray(width * height)
    queue: deque[tuple[int, int]] = deque()

    def seed(x: int, y: int) -> None:
        index = y * width + x
        if not background[index] and _neutral_light(source[x, y]):
            background[index] = 1
            queue.append((x, y))

    for x in range(width):
        seed(x, 0)
        seed(x, height - 1)
    for y in range(height):
        seed(0, y)
        seed(width - 1, y)

    while queue:
        x, y = queue.popleft()
        for next_x, next_y in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if 0 <= next_x < width and 0 <= next_y < height:
                seed(next_x, next_y)
    return background


def recover_alpha(image: Image.Image) -> Image.Image:
    rgba = image.convert("RGBA")
    if image.mode == "RGBA" and image.getchannel("A").getextrema()[0] < 255:
        return rgba
    background = border_connected_checker_mask(image)
    pixels = list(rgba.getdata())
    rgba.putdata([
        (0, 0, 0, 0) if background[index] else (r, g, b, 255)
        for index, (r, g, b, _alpha) in enumerate(pixels)
    ])
    return rgba


def analyze_grid(
    image: Image.Image,
    columns: int,
    rows: int,
    safe_margin: int,
) -> list[dict]:
    if image.width % columns or image.height % rows:
        raise ValueError("Sheet dimensions must divide evenly by the requested grid")
    cell_width = image.width // columns
    cell_height = image.height // rows
    alpha = image.convert("RGBA").getchannel("A")
    records = []
    for row in range(rows):
        for column in range(columns):
            left = column * cell_width
            top = row * cell_height
            cell = alpha.crop((left, top, left + cell_width, top + cell_height))
            bbox = cell.getbbox()
            touched = []
            if bbox is None:
                touched.append("empty")
            else:
                if bbox[0] < safe_margin:
                    touched.append("left")
                if bbox[1] < safe_margin:
                    touched.append("top")
                if cell_width - bbox[2] < safe_margin:
                    touched.append("right")
                if cell_height - bbox[3] < safe_margin:
                    touched.append("bottom")
            records.append({
                "index": row * columns + column,
                "cell": [column, row],
                "bbox": list(bbox) if bbox else None,
                "touchedMargins": touched,
                "safe": not touched,
            })
    return records


def normalize_grid(
    image: Image.Image,
    columns: int,
    rows: int,
    safe_margin: int,
) -> Image.Image:
    """Fit every isolated pose into its cell in one deterministic batch."""
    if image.width % columns or image.height % rows:
        raise ValueError("Sheet dimensions must divide evenly by the requested grid")
    cell_width = image.width // columns
    cell_height = image.height // rows
    target_width = cell_width - 2 * safe_margin
    target_height = cell_height - 2 * safe_margin
    if target_width <= 0 or target_height <= 0:
        raise ValueError("Safe margin leaves no usable cell area")
    output = Image.new("RGBA", image.size, (0, 0, 0, 0))
    for row in range(rows):
        for column in range(columns):
            origin_x = column * cell_width
            origin_y = row * cell_height
            cell = image.crop((origin_x, origin_y, origin_x + cell_width, origin_y + cell_height))
            bbox = cell.getchannel("A").getbbox()
            if bbox is None:
                raise ValueError(f"Empty motion cell: {column},{row}")
            pose = cell.crop(bbox)
            scale = min(1.0, target_width / pose.width, target_height / pose.height)
            if scale < 1.0:
                pose = pose.resize(
                    (max(1, round(pose.width * scale)), max(1, round(pose.height * scale))),
                    Image.Resampling.LANCZOS,
                )
            paste_x = origin_x + (cell_width - pose.width) // 2
            paste_y = origin_y + (cell_height - pose.height) // 2
            output.alpha_composite(pose, (paste_x, paste_y))
    return output


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    parser.add_argument("--columns", type=int, required=True)
    parser.add_argument("--rows", type=int, required=True)
    parser.add_argument("--safe-margin", type=int, default=16)
    parser.add_argument("--alpha-output", type=Path)
    parser.add_argument("--frames-output-dir", type=Path)
    args = parser.parse_args()

    source = Image.open(args.source)
    alpha = recover_alpha(source)
    source_cells = analyze_grid(alpha, args.columns, args.rows, 2)
    source_clipped = [cell["index"] for cell in source_cells if not cell["safe"]]
    normalized = normalize_grid(alpha, args.columns, args.rows, args.safe_margin) if not source_clipped else alpha
    cells = analyze_grid(normalized, args.columns, args.rows, args.safe_margin)
    unsafe = [cell["index"] for cell in cells if not cell["safe"]]
    rejected = sorted(set(source_clipped + unsafe))
    frame_records = []
    if args.frames_output_dir and not rejected:
        if args.frames_output_dir.exists():
            raise FileExistsError("Refusing to overwrite frame output: " + str(args.frames_output_dir))
        args.frames_output_dir.mkdir(parents=True)
        cell_width = normalized.width // args.columns
        cell_height = normalized.height // args.rows
        for index, frame_id in enumerate(MOTION_FRAMES):
            column, row = index % args.columns, index // args.columns
            cell = normalized.crop((column * cell_width, row * cell_height,
                                    (column + 1) * cell_width, (row + 1) * cell_height))
            frame = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
            frame.alpha_composite(cell, ((512 - cell_width) // 2, (512 - cell_height) // 2))
            path = args.frames_output_dir / f"{frame_id}.png"
            frame.save(path, optimize=True)
            frame_records.append({"id": frame_id, "path": str(path), "sha256": digest(path)})
    report = {
        "id": "vo-missing-motion-source-qa-v1",
        "status": "SOURCE_REJECTED_CELL_CLIPPING" if source_clipped else (
            "SOURCE_REJECTED_CELL_CLEARANCE" if unsafe else "SOURCE_ALPHA_CELL_QA_PASS"
        ),
        "runtimeApproved": False,
        "source": str(args.source),
        "sourceSha256": digest(args.source),
        "sourceMode": source.mode,
        "size": list(source.size),
        "grid": [args.columns, args.rows],
        "safeMarginPixels": args.safe_margin,
        "sourceClippedCells": source_clipped,
        "unsafeCells": unsafe,
        "sourceCells": source_cells,
        "cells": cells,
        "frames": frame_records,
        "notes": [
            "Passing this source QA does not approve artwork for runtime.",
            "Each extracted pose still requires anatomy, identity, baseline, scale, and visual-direction review.",
        ],
    }
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n")
    if args.alpha_output and not rejected:
        if args.alpha_output.exists():
            raise FileExistsError("Refusing to overwrite alpha output: " + str(args.alpha_output))
        args.alpha_output.parent.mkdir(parents=True, exist_ok=True)
        normalized.save(args.alpha_output, optimize=True)
    print(json.dumps({"status": report["status"], "sourceClippedCells": source_clipped, "unsafeCells": unsafe}))
    return 1 if rejected else 0


if __name__ == "__main__":
    raise SystemExit(main())
