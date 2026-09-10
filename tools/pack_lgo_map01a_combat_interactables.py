#!/usr/bin/env python3
"""Pack the reviewed Map 01A enemy, chest, and resource batch at runtime size."""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageFilter


NAMES = (
    "young-wild-boar",
    "young-wolf",
    "wild-mushroom",
    "small-herb-spirit",
    "common-chest",
    "young-spirit-herb",
)
HEIGHTS = (1.05, 1.18, 1.02, .92, .78, 1.08)
POSITIONS = (38.0, 40.0, 42.0, 39.0, 32.0, 29.0)


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def pack(source: Path, output: Path) -> None:
    with Image.open(source) as raw:
        sheet = raw.convert("RGBA")
    if sheet.width % 3 or sheet.height % 2:
        raise ValueError(f"Expected an exact 3x2 sheet, got {sheet.size}")
    output.mkdir(parents=True, exist_ok=False)
    atlas = Image.new("RGBA", (512, 512))
    cell_width, cell_height = sheet.width // 3, sheet.height // 2
    parts = []
    for index, name in enumerate(NAMES):
        column, row = index % 3, index // 3
        cell = sheet.crop((column * cell_width, row * cell_height,
                           (column + 1) * cell_width, (row + 1) * cell_height))
        bbox = cell.getchannel("A").point(lambda value: 255 if value > 8 else 0).getbbox()
        if bbox is None:
            raise ValueError("Empty combat cell: " + name)
        cell = cell.crop(bbox)
        alpha = cell.getchannel("A")
        inside = alpha.filter(ImageFilter.MinFilter(5))
        pixels = []
        for (red, green, blue, value), minimum in zip(cell.getdata(), inside.getdata()):
            if value and minimum < value and min(red, blue) > green + 24:
                spill = min(red, blue) - green - 24
                red -= spill
                blue -= spill
            pixels.append((red, green, blue, value) if value else (0, 0, 0, 0))
        cell.putdata(pixels)
        cell.thumbnail((152, 205), Image.Resampling.LANCZOS)
        atlas_column_width = 170
        left = column * atlas_column_width + (atlas_column_width - cell.width) // 2
        top = row * 256 + 238 - cell.height
        atlas.paste(cell, (left, top))
        parts.append({
            "id": name,
            "x": left,
            "y": 512 - top - cell.height,
            "w": cell.width,
            "h": cell.height,
            "sourceCell": index,
            "nativeAspectPreserved": True,
        })
    atlas_path = output / "combat-atlas.png"
    atlas.save(atlas_path, optimize=True)
    review = Image.new("RGBA", atlas.size, "#17242f")
    review.alpha_composite(atlas)
    review.convert("RGB").save(output / "review-dark.jpg", quality=92)
    layers = []
    for index, part in enumerate(parts):
        height = HEIGHTS[index]
        layers.append({
            "id": part["id"],
            "part": part["id"],
            "x": POSITIONS[index],
            "y": -1.62 + height / 2,
            "width": height * part["w"] / part["h"],
            "height": height,
            "order": 2,
            "parallax": 0,
        })
    manifest = {
        "id": "map01a-combat-interactables-v1",
        "status": "DRAFT_REQUIRES_PLAYER_REVIEW",
        "source": str(source),
        "sourceSha256": digest(source),
        "parts": parts,
        "layers": layers,
        "atlas": [512, 512],
        "pngBytes": atlas_path.stat().st_size,
        "estimatedBc3Bytes": 512 * 512,
        "upscaled": False,
        "spawnRule": "four enemies at combat edge only; chest and herb remain outside combat spawn set",
    }
    (output / "combat-layout.json").write_text(
        json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    print(json.dumps({"count": len(parts), "pngBytes": manifest["pngBytes"]}))


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    arguments = parser.parse_args()
    pack(arguments.source, arguments.output)
