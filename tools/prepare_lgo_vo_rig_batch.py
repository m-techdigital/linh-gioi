#!/usr/bin/env python3
"""Recover alpha and split one Võ 5x2 skeletal-rig source in one pass."""

from __future__ import annotations

import argparse
import hashlib
import json
from collections import deque
from pathlib import Path

from PIL import Image

from prepare_lgo_vo_missing_motion_batch import recover_alpha


PART_NAMES = (
    "head", "torso-hips", "left-upper-arm", "left-forearm-hand", "right-upper-arm",
    "right-forearm-hand", "left-thigh", "left-shin-foot", "right-thigh", "right-shin-foot",
)


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def component_boxes(image: Image.Image, min_pixels: int) -> list[tuple[int, int, int, int]]:
    width, height = image.size
    alpha = bytearray(image.convert("RGBA").getchannel("A").tobytes())
    seen = bytearray(width * height)
    components = []
    for seed, value in enumerate(alpha):
        if not value or seen[seed]:
            continue
        queue = deque([seed])
        seen[seed] = 1
        count = 0
        min_x, min_y, max_x, max_y = width, height, 0, 0
        while queue:
            index = queue.popleft()
            y, x = divmod(index, width)
            count += 1
            min_x, min_y = min(min_x, x), min(min_y, y)
            max_x, max_y = max(max_x, x), max(max_y, y)
            neighbours = []
            if x: neighbours.append(index - 1)
            if x + 1 < width: neighbours.append(index + 1)
            if y: neighbours.append(index - width)
            if y + 1 < height: neighbours.append(index + width)
            for neighbour in neighbours:
                if alpha[neighbour] and not seen[neighbour]:
                    seen[neighbour] = 1
                    queue.append(neighbour)
        if count >= min_pixels:
            components.append((count, (min_x, min_y, max_x + 1, max_y + 1)))
    if len(components) != len(PART_NAMES):
        raise ValueError(f"Expected 10 isolated rig parts, found {len(components)}")
    ordered_y = sorted(components, key=lambda item: (item[1][1] + item[1][3]) * .5)
    top = sorted(ordered_y[:5], key=lambda item: item[1][0])
    bottom = sorted(ordered_y[5:], key=lambda item: item[1][0])
    return [box for _count, box in top + bottom]


def normalized_part(image: Image.Image, box: tuple[int, int, int, int], margin: int) -> Image.Image:
    part = image.crop(box)
    usable = 512 - margin * 2
    scale = min(1.0, usable / part.width, usable / part.height)
    if scale < 1.0:
        part = part.resize((round(part.width * scale), round(part.height * scale)), Image.Resampling.LANCZOS)
    output = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    output.alpha_composite(part, ((512 - part.width) // 2, (512 - part.height) // 2))
    return output


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    parser.add_argument("--min-pixels", type=int, default=10_000)
    parser.add_argument("--safe-margin", type=int, default=20)
    args = parser.parse_args()
    if args.output_dir.exists():
        raise FileExistsError("Refusing to overwrite rig output: " + str(args.output_dir))
    image = recover_alpha(Image.open(args.source))
    boxes = component_boxes(image, args.min_pixels)
    args.output_dir.mkdir(parents=True)
    records = []
    for index, (name, box) in enumerate(zip(PART_NAMES, boxes), 1):
        path = args.output_dir / f"{index:02d}-{name}.png"
        normalized_part(image, box, args.safe_margin).save(path, optimize=True)
        records.append({"id": name, "sourceBox": list(box), "file": path.name, "sha256": digest(path)})
    report = {
        "id": "vo-10part-rig-source-v1", "status": "SOURCE_ALPHA_COMPONENT_QA_PASS",
        "runtimeApproved": False, "source": str(args.source), "sourceSha256": digest(args.source),
        "parts": records,
        "notes": ["Component QA does not approve anatomy or runtime fit.", "Visual rig assembly review is required."],
    }
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n")
    print(json.dumps({"status": report["status"], "parts": len(records)}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
