#!/usr/bin/env python3.12
"""Write per-pose 10-slot off review boards for a source-pose candidate surface."""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

POSES = ("idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck")
SLOTS = (
    "main_weapon", "inner_top", "lower_body", "outer_top", "waist_belt",
    "footwear", "arm_guard", "shoulder_chest_guard", "head_hair", "class_accessory",
)
LEGACY_BODY_NAMES = {
    "idle": "idle-base-unchanged.png",
    "run_contact_a": "run_contact_a-review.png",
    "run_a": "run_a-review.png",
    "run_contact_b": "run_contact_b-review.png",
    "run_b": "run_b-review.png",
    "jump_tuck": "jump_tuck-review.png",
}


def _load_json(path: Path) -> dict:
    if not path.is_file():
        return {}
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError:
        return {}


def _body_path(body_dir: Path, pose: str) -> Path:
    direct = body_dir / f"{pose}.png"
    if direct.is_file():
        return direct
    legacy = body_dir / LEGACY_BODY_NAMES[pose]
    if legacy.is_file():
        return legacy
    raise FileNotFoundError(f"Missing body authority for pose {pose}: {direct} or {legacy}")


def _thumbnail(image, max_size=(370, 540)):
    from PIL import Image

    copy = image.copy()
    copy.thumbnail(max_size, Image.Resampling.LANCZOS)
    return copy


def write_pose_board(surface: Path, body_dir: Path, pose: str) -> Path:
    from PIL import Image, ImageDraw

    body = Image.open(_body_path(body_dir, pose)).convert("RGBA")
    layers = {slot: Image.open(surface / slot / f"{pose}.png").convert("RGBA") for slot in SLOTS}
    board = Image.new("RGB", (1600, 1920), (225, 225, 225))
    draw = ImageDraw.Draw(board)
    variants = [("all_on", None), *[(f"off_{slot}", slot) for slot in SLOTS]]
    for index, (label, hidden) in enumerate(variants):
        comp = Image.new("RGBA", body.size)
        if hidden != "main_weapon":
            comp = Image.alpha_composite(comp, layers["main_weapon"])
        comp = Image.alpha_composite(comp, body)
        for slot in SLOTS:
            if slot == "main_weapon" or slot == hidden:
                continue
            comp = Image.alpha_composite(comp, layers[slot])
        thumb = _thumbnail(comp)
        col, row = index % 4, index // 4
        x = col * 400 + (400 - thumb.width) // 2
        y = row * 640 + 48 + (550 - thumb.height) // 2
        board.paste(thumb, (x, y), thumb)
        draw.text((col * 400 + 10, row * 640 + 14), label, fill=(20, 20, 20))
    out = surface / f"{pose}-ten-slot-off-review.jpg"
    board.save(out, quality=96, subsampling=0)
    return out


def write_boards(surface: Path, body_dir: Path) -> None:
    missing = []
    for slot in SLOTS:
        for pose in POSES:
            if not (surface / slot / f"{pose}.png").is_file():
                missing.append(f"{slot}/{pose}.png")
    if missing:
        raise FileNotFoundError("Missing source-pose slot files: " + ", ".join(missing[:8]))
    outputs = [write_pose_board(surface, body_dir, pose) for pose in POSES]
    manifest = _load_json(surface / "manifest.json") or _load_json(surface / "audit.json")
    payload = {
        "status": "SOURCE_REVIEW_REQUIRED",
        "visualReviewStatus": "SOURCE_REVIEW_REQUIRED",
        "surface": str(surface.resolve()),
        "bodyDir": str(body_dir.resolve()),
        "sourceStatus": manifest.get("status"),
        "poses": list(POSES),
        "slots": list(SLOTS),
        "boards": [
            {"file": path.name, "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}
            for path in outputs
        ],
    }
    (surface / "off-slot-board-provenance.json").write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--surface", type=Path, required=True)
    parser.add_argument("--body-dir", type=Path)
    args = parser.parse_args(argv)
    surface = args.surface.resolve()
    manifest = _load_json(surface / "manifest.json")
    body_dir = args.body_dir or (Path(manifest["bodyPoseAuthority"]) if manifest.get("bodyPoseAuthority") else None)
    if body_dir is None:
        raise ValueError("--body-dir is required when manifest.json has no bodyPoseAuthority")
    write_boards(surface, body_dir.resolve())
    print(f"LGO_SOURCE_OFF_SLOT_BOARDS_WRITTEN surface={surface}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
