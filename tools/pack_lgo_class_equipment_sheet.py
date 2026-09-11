#!/usr/bin/env python3
"""Build a class equipment review pack from two reviewed 4x10 chroma sheets.

The Kiếm runtime manifest is the locked shared-rig fit template.  This packer keeps
its bone, order, rect, and world-space contract while replacing only the artwork.
"""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image


LEVELS = (1, 10, 20, 30)
SLOTS = (
    "main_weapon", "head_hair", "inner_top", "outer_top", "lower_body",
    "waist_belt", "arm_guard", "footwear", "shoulder_chest_guard", "class_accessory",
)
PAIRED = {"lower_body", "arm_guard", "footwear"}


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def chroma_crop(sheet: Image.Image, column: int, row: int) -> Image.Image:
    x0, x1 = round(column * sheet.width / 4), round((column + 1) * sheet.width / 4)
    y0, y1 = round(row * sheet.height / 10), round((row + 1) * sheet.height / 10)
    cell = sheet.crop((x0, y0, x1, y1)).convert("RGBA")
    pixels = []
    for red, green, blue, _ in cell.getdata():
        distance = ((255 - red) ** 2 + green ** 2 + (255 - blue) ** 2) ** .5
        # Image generation can leave a faint near-magenta texture.  Treat a
        # generous chroma radius as background, then keep a short soft edge.
        alpha = max(0, min(255, round((distance - 70) * 6)))
        pixels.append((red, green, blue, alpha))
    cell.putdata(pixels)
    bbox = cell.getchannel("A").getbbox()
    if bbox is None:
        raise ValueError(f"empty sheet cell column={column} row={row}")
    return cell.crop(bbox)


def fit_into(image: Image.Image, width: int, height: int) -> Image.Image:
    target = Image.new("RGBA", (width, height))
    scale = min((width - 4) / image.width, (height - 4) / image.height)
    resized = image.resize((max(1, round(image.width * scale)), max(1, round(image.height * scale))), Image.Resampling.LANCZOS)
    target.alpha_composite(resized, ((width - resized.width) // 2, (height - resized.height) // 2))
    return target


def build(class_id: str, male_sheet: Path, female_sheet: Path, template_path: Path,
          source_dir: Path, runtime_dir: Path) -> dict:
    template = json.loads(template_path.read_text())
    sheets = {"male": Image.open(male_sheet), "female": Image.open(female_sheet)}
    source_dir.mkdir(parents=True, exist_ok=True)
    runtime_dir.mkdir(parents=True, exist_ok=True)
    sources: dict[tuple[str, int, str], Image.Image] = {}
    for gender, sheet in sheets.items():
        if sheet.size[0] < 1000 or sheet.size[1] < 1200:
            raise ValueError(f"{gender} sheet is below review resolution: {sheet.size}")
        for column, level in enumerate(LEVELS):
            for row, slot in enumerate(SLOTS):
                crop = chroma_crop(sheet, column, row)
                path = source_dir / f"{class_id}-lv{level:03d}-{gender}-{slot}.png"
                crop.save(path, optimize=True)
                sources[(gender, level, slot)] = crop

    atlases = {gender: Image.new("RGBA", (1024, 1024)) for gender in sheets}
    manifest = json.loads(json.dumps(template))
    manifest["id"] = f"{class_id}-lv1-30-equipment-runtime-v1"
    manifest["classId"] = class_id
    for asset in manifest["assets"]:
        gender = "female" if "equipment-female" in asset["path"] else "male"
        name = f"{class_id}-equipment-{gender}-atlas.png"
        asset["path"] = str(runtime_dir / name)
        asset["generator"] = "pack_lgo_class_equipment_sheet"
    for component in manifest["components"]:
        gender, level, slot, side = component["gender"], component["level"], component["slotId"], component["side"]
        source = sources[(gender, level, slot)]
        if slot in PAIRED:
            middle = source.width // 2
            source = source.crop((0, 0, middle, source.height) if side == "left" else (middle, 0, source.width, source.height))
        rect = component["atlasRect"]
        fitted = fit_into(source, rect[2], rect[3])
        atlases[gender].alpha_composite(fitted, (rect[0], 1024 - rect[1] - rect[3]))
        component["itemId"] = component["itemId"].replace("kiem-", class_id + "-")
        component["source"] = str(source_dir / f"{class_id}-lv{level:03d}-{gender}-{slot}.png")
        component["sourceSha256"] = sha256(Path(component["source"]))
        component["atlas"] = f"{class_id}-equipment-{gender}-atlas"
        component.pop("reviewNote", None)
        if slot == "head_hair":
            if class_id == "phap":
                # Pháp hair is a defining long silhouette.  Scale the locked
                # head-anchor box uniformly so its source aspect stays intact.
                factor = 1.25 if gender == "male" else 1.65
                component["worldW"] *= factor
                component["worldH"] *= factor
                component["worldY"] -= .02 if gender == "male" else .01
                if gender == "female":
                    component["worldX"] += .12
            component["itemId"] += "-front"
            component["reviewNote"] = "single side-view candidate; no rear turnaround"

    for gender, atlas in atlases.items():
        path = runtime_dir / f"{class_id}-equipment-{gender}-atlas.png"
        atlas.save(path, optimize=True)
        asset = next(item for item in manifest["assets"] if f"equipment-{gender}" in item["path"])
        asset.update(bytes=path.stat().st_size, sha256=sha256(path), width=1024, height=1024)
    manifest_path = runtime_dir / "manifest.json"
    manifest_path.write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n")
    (source_dir / "source-manifest.json").write_text(json.dumps({
        "classId": class_id, "layout": {"levels": LEVELS, "slots": SLOTS},
        "maleSheet": str(male_sheet), "femaleSheet": str(female_sheet),
        "maleSheetSha256": sha256(male_sheet), "femaleSheetSha256": sha256(female_sheet),
        "items": 80, "status": "DRAFT_RUNTIME_FIT_SOURCE",
    }, ensure_ascii=False, indent=2) + "\n")
    return manifest


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--class-id", required=True)
    parser.add_argument("--male-sheet", type=Path, required=True)
    parser.add_argument("--female-sheet", type=Path, required=True)
    parser.add_argument("--template", type=Path, required=True)
    parser.add_argument("--source-dir", type=Path, required=True)
    parser.add_argument("--runtime-dir", type=Path, required=True)
    args = parser.parse_args()
    manifest = build(args.class_id, args.male_sheet, args.female_sheet, args.template, args.source_dir, args.runtime_dir)
    print(f"PASS class={args.class_id} items=80 components={len(manifest['components'])} atlases=2x1024")


if __name__ == "__main__":
    main()
