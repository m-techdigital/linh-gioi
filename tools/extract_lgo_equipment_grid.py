#!/usr/bin/env python3
"""Extract a class equipment grid from a reviewed source using a data-only crop plan."""
import argparse
import hashlib
import json
from collections import deque
from pathlib import Path

from PIL import Image, ImageDraw

CANONICAL_SLOTS = [
    "main_weapon", "head_hair", "inner_top", "outer_top", "lower_body",
    "waist_belt", "arm_guard", "footwear", "shoulder_chest_guard", "class_accessory",
]


def sha256(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def remove_connected_light_background(image):
    image = image.convert("RGBA")
    pixels = image.load()
    width, height = image.size
    queue, seen = deque(), set()

    def is_background(x, y):
        red, green, blue, alpha = pixels[x, y]
        return alpha == 0 or (red > 220 and green > 220 and blue > 220
                              and max(red, green, blue) - min(red, green, blue) < 28)

    for x in range(width):
        for y in (0, height - 1):
            if is_background(x, y):
                queue.append((x, y)); seen.add((x, y))
    for y in range(height):
        for x in (0, width - 1):
            if is_background(x, y):
                queue.append((x, y)); seen.add((x, y))
    while queue:
        x, y = queue.popleft()
        pixels[x, y] = (0, 0, 0, 0)
        for point in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if (0 <= point[0] < width and 0 <= point[1] < height
                    and point not in seen and is_background(*point)):
                seen.add(point); queue.append(point)
    bounds = image.getchannel("A").getbbox()
    if not bounds:
        raise ValueError("Crop contains no foreground pixels")
    image = image.crop(bounds)
    pixels = image.load()
    width, height = image.size
    seen = set()
    for origin_y in range(height):
        for origin_x in range(width):
            if (origin_x, origin_y) in seen or pixels[origin_x, origin_y][3] == 0:
                continue
            component, queue = [], deque([(origin_x, origin_y)])
            seen.add((origin_x, origin_y))
            while queue:
                x, y = queue.popleft(); component.append((x, y))
                for point in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
                    if (0 <= point[0] < width and 0 <= point[1] < height
                            and point not in seen and pixels[point[0], point[1]][3] > 0):
                        seen.add(point); queue.append(point)
            min_x = min(point[0] for point in component); max_x = max(point[0] for point in component)
            min_y = min(point[1] for point in component); max_y = max(point[1] for point in component)
            neutral_light = len(component) < 200 and all(
                min(pixels[x, y][:3]) > 180 and max(pixels[x, y][:3]) - min(pixels[x, y][:3]) < 30
                for x, y in component)
            if (max_y - min_y <= 1 and max_x - min_x >= 3) or neutral_light:
                for x, y in component: pixels[x, y] = (0, 0, 0, 0)
    bounds = image.getchannel("A").getbbox()
    return image.crop(bounds) if bounds else image


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--plan", type=Path, required=True)
    parser.add_argument("--out-dir", type=Path, required=True)
    args = parser.parse_args()
    plan = json.loads(args.plan.read_text())
    if plan.get("slots") != CANONICAL_SLOTS:
        raise ValueError("Plan slots must match the 10 canonical slots in canonical order")
    out = args.out_dir.resolve()
    out.mkdir(parents=True, exist_ok=False)
    levels, slots = plan["levels"], plan["slots"]
    manifest = {"version": 2, "classId": plan["classId"], "levels": levels,
                "slots": slots, "plan": str(args.plan.resolve()),
                "status": "SOURCE_CANDIDATES_EXTRACTED_BASE_FIT_UNVERIFIED",
                "sources": [], "items": []}
    contact_items = []
    for source in plan["sources"]:
        path = Path(source["path"]).resolve()
        actual_hash = sha256(path)
        if actual_hash != source["sha256"]:
            raise ValueError("Source hash mismatch: " + str(path))
        image = Image.open(path)
        xs, ys, padding = source["xBoundaries"], source["yBoundaries"], source.get("padding", 4)
        if len(xs) != len(levels) + 1 or len(ys) != len(slots) + 1:
            raise ValueError("Grid boundaries do not match levels/slots")
        manifest["sources"].append({"gender": source["gender"], "path": str(path),
                                    "sha256": actual_hash, "size": list(image.size)})
        for row, slot in enumerate(slots):
            for column, level in enumerate(levels):
                crop = [xs[column] + padding, ys[row] + padding,
                        xs[column + 1] - padding, ys[row + 1] - padding]
                item = remove_connected_light_background(image.crop(tuple(crop)))
                filename = f'{plan["classId"]}-lv{level:03d}-{source["gender"]}-{slot}.png'
                target = out / filename
                item.save(target, optimize=True)
                manifest["items"].append({"id": target.stem, "gender": source["gender"],
                    "level": level, "slot": slot, "crop": crop, "width": item.width,
                    "height": item.height, "bytes": target.stat().st_size, "sha256": sha256(target),
                    "fitStatus": "candidate", "runtimeEligible": False})
                contact_items.append((target.stem, item))
    expected = len(levels) * len(slots) * len(plan["sources"])
    if len(manifest["items"]) != expected:
        raise ValueError(f'Expected {expected} items, got {len(manifest["items"])}')
    (out / "source-manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n")
    cell_width, cell_height, columns = 180, 150, len(levels) * len(plan["sources"])
    rows = (len(contact_items) + columns - 1) // columns
    contact = Image.new("RGB", (cell_width * columns, cell_height * rows), (13, 19, 29))
    draw = ImageDraw.Draw(contact)
    for index, (label, item) in enumerate(contact_items):
        item = item.copy(); item.thumbnail((cell_width - 8, cell_height - 25))
        x = (index % columns) * cell_width + (cell_width - item.width) // 2
        y = (index // columns) * cell_height
        contact.paste(item, (x, y), item.getchannel("A"))
        draw.text(((index % columns) * cell_width + 3, y + cell_height - 22),
                  label.replace(plan["classId"] + "-", "")[:27], fill="white")
    contact.save(out / "contact.png", optimize=True)
    print(f'LGO_EQUIPMENT_GRID_CANDIDATES_PASS class={plan["classId"]} items={expected} '
          f'baseFit=unverified runtimeEligible=false out={out}')


if __name__ == "__main__":
    main()
