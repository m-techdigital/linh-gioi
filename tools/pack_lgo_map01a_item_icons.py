#!/usr/bin/env python3
"""Build a deterministic Map01A item-icon atlas from provenance-backed crops."""
from __future__ import annotations

from collections import deque
import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageDraw


ITEM_IDS = {
    "item-hp-potion": "health_potion",
    "item-mp-potion": "mana_potion",
    "item-equipment-fragment": "class_reward",
    "item-dumpling": "dumpling",
    "item-coin": "coin",
}


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def remove_connected_pale_matte(source: Image.Image) -> Image.Image:
    image = source.convert("RGBA")
    pixels = list(image.get_flattened_data())
    width, height = image.size

    def is_matte(index: int) -> bool:
        red, green, blue, alpha = pixels[index]
        return alpha > 0 and min(red, green, blue) >= 145 and max(red, green, blue) - min(red, green, blue) < 85 and blue >= red - 24

    background: set[int] = set()
    queue: deque[int] = deque()

    def seed(index: int) -> None:
        if index not in background and is_matte(index):
            background.add(index)
            queue.append(index)

    for x in range(width):
        seed(x)
        seed((height - 1) * width + x)
    for y in range(height):
        seed(y * width)
        seed(y * width + width - 1)

    while queue:
        index = queue.popleft()
        x, y = index % width, index // width
        for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if 0 <= nx < width and 0 <= ny < height:
                neighbor = ny * width + nx
                if neighbor not in background and is_matte(neighbor):
                    background.add(neighbor)
                    queue.append(neighbor)

    image.putdata([(0, 0, 0, 0) if index in background else pixel for index, pixel in enumerate(pixels)])
    return image


def run(source: Path, output: Path, review_output: Path) -> dict:
    source_manifest_path = source / "MANIFEST.json"
    source_manifest = json.loads(source_manifest_path.read_text(encoding="utf-8"))
    entries = {entry["name"]: entry for entry in source_manifest["entries"]}
    selected = []
    for name, item_id in ITEM_IDS.items():
        entry = entries.get(name)
        if entry is None:
            raise ValueError("Missing source manifest entry: " + name)
        source_path = source / entry["file"]
        if sha256(source_path) != entry["sha256"]:
            raise ValueError("Source crop hash changed: " + entry["file"])
        selected.append((name, item_id, entry, remove_connected_pale_matte(Image.open(source_path))))

    if output.exists() and any(output.iterdir()):
        raise ValueError("Output directory must be empty: " + str(output))
    output.mkdir(parents=True, exist_ok=True)
    review_output.parent.mkdir(parents=True, exist_ok=True)

    atlas = Image.new("RGBA", (512, 128))
    parts = []
    for index, (name, item_id, entry, image) in enumerate(selected):
        cell_x = index * 96
        x = cell_x + (96 - image.width) // 2
        y_top = (128 - image.height) // 2
        atlas.alpha_composite(image, (x, y_top))
        parts.append({
            "id": item_id,
            "sourceName": name,
            "x": x,
            "y": 128 - y_top - image.height,
            "w": image.width,
            "h": image.height,
            "sourceSha256": entry["sha256"],
            "source": entry["source"],
            "sourceRect": entry["rect"],
        })
    atlas_path = output / "map01a-item-icons.png"
    atlas.save(atlas_path, optimize=True)

    review = Image.new("RGB", (1000, 410), "#071724")
    draw = ImageDraw.Draw(review)
    for index, (_, item_id, _, image) in enumerate(selected):
        x = index * 200 + 10
        enlarged = image.resize((146, 142), Image.Resampling.LANCZOS)
        for row, color in enumerate(("#071724", "#bac7ca")):
            tile = Image.new("RGBA", (180, 170), color)
            tile.alpha_composite(enlarged, (17, 12))
            review.paste(tile.convert("RGB"), (x, row * 176))
        draw.text((x + 2, 370), item_id, fill="#f0c865")
    review.save(review_output, optimize=True)

    manifest = {
        "id": "map01a-item-icons-v1",
        "status": "DRAFT_RUNTIME_REVIEW",
        "runtimeApproved": False,
        "sourceManifest": str(source_manifest_path),
        "sourceManifestSha256": sha256(source_manifest_path),
        "alphaPolicy": "border-connected-pale-matte-v1",
        "textureSize": [512, 128],
        "pngBytes": atlas_path.stat().st_size,
        "assets": [{
            "path": "client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AItems/map01a-item-icons.png",
            "role": "ui-item-atlas",
            "generator": "pack_lgo_map01a_item_icons",
            "referenceOnly": False,
            "sha256": sha256(atlas_path),
        }],
        "parts": parts,
    }
    (output / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return manifest


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--review-output", type=Path, required=True)
    args = parser.parse_args()
    result = run(args.source, args.output, args.review_output)
    print(json.dumps({"status": result["status"], "parts": len(result["parts"]), "pngBytes": result["pngBytes"]}))
