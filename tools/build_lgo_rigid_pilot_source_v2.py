#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
DEFAULT_INPUT = ROOT / "client/Unity/Assets/Game/Character/Runtime/Resources/LGORigidPilot/v1"
DEFAULT_OUTPUT = ROOT / "client/Unity/Assets/Game/Character/Runtime/Resources/LGORigidPilot/rigid-source-v2"


SPLITS = (
    ("forearm_hand_near", "forearm_near", "hand_near", 0.62, 18),
    ("forearm_hand_far", "forearm_far", "hand_far", 0.62, 18),
    ("lower_sleeve_near", "bracer_near", "glove_near", 0.62, 16),
    ("lower_sleeve_far", "bracer_far", "glove_far", 0.62, 16),
    ("shin_foot_near", "shin_near", "foot_near", 0.66, 20),
    ("shin_foot_far", "shin_far", "foot_far", 0.66, 20),
    ("boot_near", "boot_shaft_near", "boot_foot_near", 0.66, 20),
    ("boot_far", "boot_shaft_far", "boot_foot_far", 0.66, 20),
)


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def masked_copy(source: Image.Image, *, joint_y: int, overlap: int, proximal: bool) -> Image.Image:
    result = source.copy().convert("RGBA")
    alpha = result.getchannel("A")
    pixels = alpha.load()
    limit = joint_y + overlap if proximal else joint_y - overlap
    for y in range(alpha.height):
        clear = y > limit if proximal else y < limit
        if clear:
            for x in range(alpha.width):
                pixels[x, y] = 0
    result.putalpha(alpha)
    return result


def build(input_dir: Path, output_dir: Path) -> dict:
    output_dir.mkdir(parents=True, exist_ok=True)
    modules: list[dict] = []
    for source_id, proximal_id, distal_id, ratio, overlap in SPLITS:
        source_path = input_dir / f"{source_id}.png"
        source = Image.open(source_path).convert("RGBA")
        joint_y = round(source.height * ratio)
        for output_id, proximal in ((proximal_id, True), (distal_id, False)):
            output_path = output_dir / f"{output_id}.png"
            image = masked_copy(source, joint_y=joint_y, overlap=overlap, proximal=proximal)
            image.save(output_path, optimize=True)
            alpha = image.getchannel("A")
            modules.append(
                {
                    "id": output_id,
                    "source": source_path.name,
                    "sourceSha256": sha256(source_path),
                    "outputSha256": sha256(output_path),
                    "mode": image.mode,
                    "width": image.width,
                    "height": image.height,
                    "jointY": joint_y,
                    "overlapPixels": overlap * 2,
                    "alphaExtrema": list(alpha.getextrema()),
                    "alphaBoundingBox": list(alpha.getbbox() or (0, 0, 0, 0)),
                    "role": "proximal" if proximal else "distal",
                }
            )
    report = {
        "status": "RIGID_NATIVE_SOURCE_V2_READY",
        "method": "FIXED_SOURCE_CANVAS_ALPHA_SPLIT_WITH_SYMMETRIC_OVERLAP",
        "coordinateProfile": "lgo_character_canvas_1024x1536_v1",
        "perPoseAssets": False,
        "perPoseOffsets": False,
        "geometryChangedAtRuntime": False,
        "moduleCount": len(modules),
        "modules": modules,
    }
    (output_dir / "source-registration.json").write_text(
        json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    return report


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--input-dir", type=Path, default=DEFAULT_INPUT)
    parser.add_argument("--output-dir", type=Path, default=DEFAULT_OUTPUT)
    args = parser.parse_args()
    report = build(args.input_dir.resolve(), args.output_dir.resolve())
    print(json.dumps({"status": report["status"], "moduleCount": report["moduleCount"]}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
