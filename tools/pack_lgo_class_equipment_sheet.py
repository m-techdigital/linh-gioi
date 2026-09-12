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

import cv2
import numpy as np
from PIL import Image


LEVELS = (1, 10, 20, 30)
SLOTS = (
    "main_weapon", "head_hair", "inner_top", "outer_top", "lower_body",
    "waist_belt", "arm_guard", "footwear", "shoulder_chest_guard", "class_accessory",
)
PAIRED = {"lower_body", "arm_guard", "footwear"}


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def _keep_cell_subject(alpha: np.ndarray, expected_components: int) -> np.ndarray:
    """Reject artwork leaking in from the rows above/below a generated grid cell.

    The attachment sheets have no gutter: long art from an adjacent row can cross
    the nominal cell edge.  The intended item is centred in its row, while leaked
    fragments hug the top or bottom edge.  Keep the expected one/two principal
    components nearest the row centre, then retain small detached details only
    when they sit close to a selected principal component.
    """
    count, labels, stats, centroids = cv2.connectedComponentsWithStats(
        (alpha > 8).astype(np.uint8), 8)
    candidates = [index for index in range(1, count)
                  if stats[index, cv2.CC_STAT_AREA] >= 40]
    if not candidates:
        return alpha
    centre_y = (alpha.shape[0] - 1) * .5
    principals = sorted(candidates, key=lambda index: (
        abs(float(centroids[index, 1]) - centre_y),
        -int(stats[index, cv2.CC_STAT_AREA])))[:expected_components]
    if len(principals) > 1:
        largest = max(int(stats[index, cv2.CC_STAT_AREA]) for index in principals)
        principals = [index for index in principals
                      if int(stats[index, cv2.CC_STAT_AREA]) >= largest * .2]
    keep = np.isin(labels, principals)
    # The accepted sheets draw each item (or each member of a paired item) as one
    # connected silhouette.  Keeping extra components here reintroduces collars,
    # belts, or shoes leaking from the neighbouring row.
    return np.where(keep, alpha, 0).astype(np.uint8)


def _keep_rig_subject(alpha: np.ndarray) -> np.ndarray:
    """Keep a rig cell's principal silhouette and intentional detached details."""
    count, labels, stats, _ = cv2.connectedComponentsWithStats(
        (alpha > 8).astype(np.uint8), 8)
    if count <= 1:
        return alpha
    principal = max(range(1, count), key=lambda index: int(stats[index, cv2.CC_STAT_AREA]))
    largest = int(stats[principal, cv2.CC_STAT_AREA])
    threshold = max(20, round(largest * .003))
    height, width = alpha.shape
    accepted = []
    for index in range(1, count):
        x, y, w, h, area = (int(value) for value in stats[index])
        touches_edge = x <= 1 or y <= 1 or x + w >= width - 1 or y + h >= height - 1
        if area >= threshold and (index == principal or not touches_edge):
            accepted.append(index)
    return np.where(np.isin(labels, accepted), alpha, 0).astype(np.uint8)


def chroma_crop(sheet: Image.Image, column: int, row: int, expected_components: int = 1) -> Image.Image:
    x0, x1 = round(column * sheet.width / 4), round((column + 1) * sheet.width / 4)
    y0, y1 = round(row * sheet.height / 10), round((row + 1) * sheet.height / 10)
    cell = sheet.crop((x0, y0, x1, y1)).convert("RGBA")
    rgba = np.asarray(cell).copy()
    distance = np.sqrt((255 - rgba[:, :, 0].astype(float)) ** 2
                       + rgba[:, :, 1].astype(float) ** 2
                       + (255 - rgba[:, :, 2].astype(float)) ** 2)
    alpha = np.clip(np.rint((distance - 70) * 6), 0, 255).astype(np.uint8)
    rgba[:, :, 3] = _keep_cell_subject(alpha, expected_components)
    cell = Image.fromarray(rgba, "RGBA")
    bbox = cell.getchannel("A").getbbox()
    if bbox is None:
        raise ValueError(f"empty sheet cell column={column} row={row}")
    return cell.crop(bbox)


def chroma_rig_component(sheet: Image.Image, index: int) -> Image.Image:
    if not 0 <= index < 15:
        raise ValueError("rig component index must be in [0, 15)")
    column, row = index % 5, index // 5
    x0, x1 = round(column * sheet.width / 5), round((column + 1) * sheet.width / 5)
    y0, y1 = round(row * sheet.height / 3), round((row + 1) * sheet.height / 3)
    cell = sheet.crop((x0, y0, x1, y1)).convert("RGBA")
    rgba = np.asarray(cell).copy()
    distance = np.sqrt((255 - rgba[:, :, 0].astype(float)) ** 2
                       + rgba[:, :, 1].astype(float) ** 2
                       + (255 - rgba[:, :, 2].astype(float)) ** 2)
    alpha = np.clip(np.rint((distance - 70) * 6), 0, 255).astype(np.uint8)
    rgba[:, :, 3] = _keep_rig_subject(alpha)
    cell = Image.fromarray(rgba, "RGBA")
    bbox = cell.getchannel("A").getbbox()
    if bbox is None:
        raise ValueError(f"empty rig component cell index={index}")
    return cell.crop(bbox)


def fit_into(image: Image.Image, width: int, height: int) -> Image.Image:
    target = Image.new("RGBA", (width, height))
    scale = min((width - 4) / image.width, (height - 4) / image.height)
    resized = image.resize((max(1, round(image.width * scale)), max(1, round(image.height * scale))), Image.Resampling.LANCZOS)
    target.alpha_composite(resized, ((width - resized.width) // 2, (height - resized.height) // 2))
    return target


def split_outer_component(component: dict) -> list[dict]:
    """Split one torso-bound jacket into torso and two articulated sleeves.

    The three atlas rectangles partition the old rectangle exactly, so idle still
    reconstructs the reviewed item while motion can rotate each sleeve with its
    shared upper-arm bone.
    """
    rect = component["atlasRect"]
    widths = (rect[2] // 3, rect[2] - 2 * (rect[2] // 3), rect[2] // 3)
    specs = (
        ("left-sleeve", "left", "left-upper-arm", -1),
        ("torso", "center", "torso-hips", 0),
        ("right-sleeve", "right", "right-upper-arm", 1),
    )
    result = []
    offset = 0
    for (suffix, side, bone, direction), width in zip(specs, widths):
        part = json.loads(json.dumps(component))
        part["itemId"] += "-" + suffix
        part["side"] = side
        part["bone"] = bone
        part["worldX"] += direction * component["worldW"] / 3
        part["worldW"] = component["worldW"] * width / rect[2]
        part["atlasRect"] = [rect[0] + offset, rect[1], width, rect[3]]
        result.append(part)
        offset += width
    return result


def apply_rig_sheet_override(class_id: str, gender: str, level: int, sheet_path: Path,
                             source_dir: Path, atlas: Image.Image, components: list[dict]) -> None:
    names = (
        "main_weapon", "head_hair", "inner_top", "outer_top-torso",
        "outer_top-left-sleeve", "outer_top-right-sleeve", "lower_body-left",
        "lower_body-right", "waist_belt", "arm_guard-left", "arm_guard-right",
        "footwear-left", "footwear-right", "shoulder_chest_guard", "class_accessory",
    )
    sheet = Image.open(sheet_path)
    images = {name: chroma_rig_component(sheet, index) for index, name in enumerate(names)}
    lookup = {}
    for component in components:
        if component["gender"] != gender or component["level"] != level:
            continue
        slot, side = component["slotId"], component["side"]
        if slot == "outer_top":
            suffix = "torso" if side == "center" else side + "-sleeve"
            key = slot + "-" + suffix
        elif slot in PAIRED:
            key = slot + "-" + side
        else:
            key = slot
        lookup[key] = component
    missing = sorted(set(names) - set(lookup))
    if missing:
        raise ValueError("rig sheet component mapping is incomplete: " + ",".join(missing))
    for name, image in images.items():
        component = lookup[name]
        rect = component["atlasRect"]
        top = atlas.height - rect[1] - rect[3]
        atlas.paste((0, 0, 0, 0), (rect[0], top, rect[0] + rect[2], top + rect[3]))
        atlas.alpha_composite(fit_into(image, rect[2], rect[3]), (rect[0], top))
        path = source_dir / f"{class_id}-lv{level:03d}-{gender}-rig-{name}.png"
        image.save(path, optimize=True)
        component["source"] = str(path)
        component["sourceSha256"] = sha256(path)
    # Common humanoid attachment sizes; only art changes between classes/levels.
    for side, direction in (("left", -1), ("right", 1)):
        sleeve = lookup[f"outer_top-{side}-sleeve"]
        sleeve.update(worldX=direction * .25, worldY=1.10, worldW=.32, worldH=.50)
    lookup["outer_top-torso"].update(worldX=0, worldY=1.02, worldW=.55, worldH=.70)


def build(class_id: str, male_sheet: Path, female_sheet: Path, template_path: Path,
          source_dir: Path, runtime_dir: Path,
          rig_sheets: dict[tuple[str, int], Path] | None = None) -> dict:
    rig_sheets = rig_sheets or {}
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
                crop = chroma_crop(sheet, column, row, 2 if slot in PAIRED else 1)
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
    packed_components = []
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
            if class_id in {"phap", "linh"}:
                # Pháp/Linh hair is a defining long silhouette. Scale the locked
                # head-anchor box uniformly so its source aspect stays intact.
                factor = 1.25 if gender == "male" else 1.65
                component["worldW"] *= factor
                component["worldH"] *= factor
                component["worldY"] -= .02 if gender == "male" else .01
                if gender == "female":
                    component["worldX"] += .12
            component["itemId"] += "-front"
            component["reviewNote"] = "single side-view candidate; no rear turnaround"
        should_articulate = slot == "outer_top" and (gender, level) in rig_sheets
        packed_components.extend(split_outer_component(component) if should_articulate else [component])
    manifest["components"] = packed_components
    for (gender, level), rig_sheet in sorted(rig_sheets.items()):
        if gender not in sheets or level not in LEVELS:
            raise ValueError(f"invalid rig sheet target: {gender}:lv{level}")
        apply_rig_sheet_override(class_id, gender, level, rig_sheet,
                                 source_dir, atlases[gender], packed_components)

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
    parser.add_argument("--rig-sheet", action="append", default=[],
                        help="repeatable GENDER:LEVEL:PATH component-sheet override")
    args = parser.parse_args()
    rig_sheets = {}
    for spec in args.rig_sheet:
        try:
            gender, level_text, path_text = spec.split(":", 2)
            key = (gender, int(level_text))
        except ValueError as exc:
            raise SystemExit(f"invalid --rig-sheet {spec!r}; expected GENDER:LEVEL:PATH") from exc
        if key in rig_sheets:
            raise SystemExit(f"duplicate --rig-sheet target: {gender}:lv{level_text}")
        rig_sheets[key] = Path(path_text)
    manifest = build(args.class_id, args.male_sheet, args.female_sheet, args.template,
                     args.source_dir, args.runtime_dir, rig_sheets)
    print(f"PASS class={args.class_id} items=80 components={len(manifest['components'])} atlases=2x1024")


if __name__ == "__main__":
    main()
