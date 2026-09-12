#!/usr/bin/env python3.12
"""Create a source-pose candidate with oversized class-accessory pixels moved to outer_top."""
from __future__ import annotations

import argparse
import json
import shutil
from pathlib import Path

import numpy as np
from PIL import Image

POSES = ("idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck")
SLOTS = (
    "main_weapon", "inner_top", "lower_body", "outer_top", "waist_belt",
    "footwear", "arm_guard", "shoulder_chest_guard", "head_hair", "class_accessory",
)


def _load(path: Path) -> np.ndarray:
    return np.asarray(Image.open(path).convert("RGBA")).copy()


def _save(path: Path, image: np.ndarray) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    Image.fromarray(image, "RGBA").save(path, compress_level=4)


def _alpha(image: np.ndarray) -> np.ndarray:
    return image[:, :, 3] > 8


def _dilate(mask: np.ndarray, radius: int = 24) -> np.ndarray:
    import cv2

    kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (radius * 2 + 1, radius * 2 + 1))
    return cv2.dilate(mask.astype(np.uint8), kernel) > 0


def _keep_accessory_mask(accessory_alpha: np.ndarray, anchor_alpha: np.ndarray) -> np.ndarray:
    import cv2

    if not accessory_alpha.any():
        return accessory_alpha
    near = _dilate(anchor_alpha)
    count, labels, stats, _ = cv2.connectedComponentsWithStats(accessory_alpha.astype(np.uint8), 8)
    keep = np.zeros_like(accessory_alpha, dtype=bool)
    for index in range(1, count):
        component = labels == index
        area = int(stats[index, cv2.CC_STAT_AREA])
        if area <= 9000 and np.any(component & near):
            keep |= component
    if keep.any():
        return keep
    return accessory_alpha & near


def _union_alpha(surface: Path, pose: str) -> np.ndarray:
    masks = []
    for slot in SLOTS:
        masks.append(_alpha(_load(surface / slot / f"{pose}.png")))
    return np.logical_or.reduce(masks)


def repair_pose(source: Path, output: Path, pose: str) -> dict:
    images = {slot: _load(source / slot / f"{pose}.png") for slot in SLOTS}
    before = np.logical_or.reduce([_alpha(images[slot]) for slot in SLOTS])
    accessory_alpha = _alpha(images["class_accessory"])
    keep = _keep_accessory_mask(accessory_alpha, _alpha(images["waist_belt"]))
    move = accessory_alpha & ~keep

    repaired_accessory = images["class_accessory"].copy()
    repaired_accessory[move, 3] = 0
    repaired_outer = images["outer_top"].copy()
    repaired_outer[move] = images["class_accessory"][move]

    images["class_accessory"] = repaired_accessory
    images["outer_top"] = repaired_outer
    after = np.logical_or.reduce([_alpha(images[slot]) for slot in SLOTS])
    if not np.array_equal(before, after):
        raise AssertionError(f"repair changed full-compose alpha coverage for pose {pose}")

    for slot, image in images.items():
        _save(output / slot / f"{pose}.png", image)
    return {
        "pose": pose,
        "movedPixels": int(move.sum()),
        "keptAccessoryPixels": int(keep.sum()),
        "inputAccessoryPixels": int(accessory_alpha.sum()),
    }


def repair_surface(source: Path, output: Path) -> dict:
    if output.exists():
        raise FileExistsError(output)
    for slot in SLOTS:
        for pose in POSES:
            path = source / slot / f"{pose}.png"
            if not path.is_file():
                raise FileNotFoundError(path)
    for child in source.iterdir():
        if child.is_file():
            output.mkdir(parents=True, exist_ok=True)
            shutil.copy2(child, output / child.name)
    records = [repair_pose(source, output, pose) for pose in POSES]
    payload = {
        "status": "SOURCE_REVIEW_REQUIRED",
        "visualReviewStatus": "SOURCE_REVIEW_REQUIRED",
        "method": "move class_accessory pixels not connected near waist_belt anchor into outer_top",
        "inputSurface": str(source.resolve()),
        "outputSurface": str(output.resolve()),
        "poses": list(POSES),
        "slots": list(SLOTS),
        "records": records,
    }
    (output / "class-accessory-ownership-repair.json").write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    (output / "DO-NOT-PACK.md").write_text(
        "# DO NOT PACK\n\n"
        "This source surface is a diagnostic repair candidate only. "
        "It must not be packed or exposed in Player until off-slot boards and Player visual review are accepted.\n",
        encoding="utf-8",
    )
    return payload


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args(argv)
    payload = repair_surface(args.source.resolve(), args.output.resolve())
    moved = sum(record["movedPixels"] for record in payload["records"])
    print(f"LGO_CLASS_ACCESSORY_OWNERSHIP_REPAIRED output={args.output} movedPixels={moved}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
