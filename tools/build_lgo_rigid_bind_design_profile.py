#!/usr/bin/env python3
"""Register a coherent bind design once on the canonical LGO character canvas."""
from __future__ import annotations

import argparse
import hashlib
import json
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
DEFAULT_SOURCE = ROOT / "build/rigid-outfit-pilot/final-1-coherent-body-design-v9/male-female-coherent-bind-design-v1.png"
DEFAULT_OUTPUT = ROOT / "build/rigid-outfit-pilot/final-1-coherent-body-design-v9/canonical-bind-profile-v1"
CANVAS_WIDTH = 1024
CANVAS_HEIGHT = 1536
GROUND_Y = 1484

REQUIRED_JOINTS = (
    "root", "pelvis", "torso", "chest", "neck", "head",
    "shoulder_near", "elbow_near", "wrist_near", "hand_near",
    "shoulder_far", "elbow_far", "wrist_far", "hand_far",
    "hip_near", "knee_near", "ankle_near", "foot_near",
    "hip_far", "knee_far", "ankle_far", "foot_far",
)

CORE_BONES = (("root", "pelvis"), ("pelvis", "torso"), ("torso", "chest"), ("chest", "neck"), ("neck", "head"))
NEAR_BONES = (
    ("chest", "shoulder_near"), ("shoulder_near", "elbow_near"),
    ("elbow_near", "wrist_near"), ("wrist_near", "hand_near"),
    ("pelvis", "hip_near"), ("hip_near", "knee_near"),
    ("knee_near", "ankle_near"), ("ankle_near", "foot_near"),
)
FAR_BONES = (
    ("chest", "shoulder_far"), ("shoulder_far", "elbow_far"),
    ("elbow_far", "wrist_far"), ("wrist_far", "hand_far"),
    ("pelvis", "hip_far"), ("hip_far", "knee_far"),
    ("knee_far", "ankle_far"), ("ankle_far", "foot_far"),
)


def profile_config() -> dict:
    """Source-board registration authored once; outfits never alter these points."""
    return {
        "profileId": "lgo_coherent_bind_design_candidate_v1",
        "sourceSpaceProfile": "lgo_character_canvas_1024x1536_v1",
        "canvas": [CANVAS_WIDTH, CANVAS_HEIGHT],
        "groundY": GROUND_Y,
        "status": "LANDMARK_REVIEW_CANDIDATE",
        "bodies": {
            "male": {
                "sourceCrop": [250, 0, 750, 984],
                "sourceGroundY": 953,
                "scale": 1.45,
                "landmarks": {
                    "root": [520, 953], "pelvis": [520, 565], "torso": [520, 470],
                    "chest": [520, 405], "neck": [520, 348], "head": [528, 230],
                    "shoulder_near": [579, 390], "elbow_near": [610, 508],
                    "wrist_near": [640, 604], "hand_near": [646, 631],
                    "shoulder_far": [465, 390], "elbow_far": [401, 515],
                    "wrist_far": [378, 607], "hand_far": [379, 633],
                    "hip_near": [548, 583], "knee_near": [566, 739],
                    "ankle_near": [562, 866], "foot_near": [604, 922],
                    "hip_far": [486, 581], "knee_far": [423, 749],
                    "ankle_far": [403, 879], "foot_far": [405, 925],
                },
            },
            "female": {
                "sourceCrop": [760, 40, 1280, 984],
                "sourceGroundY": 953,
                "scale": 1.45,
                "landmarks": {
                    "root": [1047, 953], "pelvis": [1047, 579], "torso": [1043, 480],
                    "chest": [1042, 420], "neck": [1040, 371], "head": [1040, 269],
                    "shoulder_near": [1101, 405], "elbow_near": [1134, 526],
                    "wrist_near": [1164, 604], "hand_near": [1170, 630],
                    "shoulder_far": [984, 405], "elbow_far": [934, 535],
                    "wrist_far": [911, 610], "hand_far": [907, 636],
                    "hip_near": [1073, 594], "knee_near": [1094, 753],
                    "ankle_near": [1098, 875], "foot_near": [1140, 925],
                    "hip_far": [1012, 593], "knee_far": [977, 753],
                    "ankle_far": [954, 879], "foot_far": [960, 925],
                },
            },
        },
    }


def project_point(body: dict, point: list[int]) -> tuple[int, int]:
    pelvis_x = body["landmarks"]["pelvis"][0]
    scale = body["scale"]
    return (
        round((point[0] - pelvis_x) * scale + CANVAS_WIDTH / 2),
        round((point[1] - body["sourceGroundY"]) * scale + GROUND_Y),
    )


def _font(size: int) -> ImageFont.ImageFont:
    path = Path("/System/Library/Fonts/Supplemental/Arial.ttf")
    return ImageFont.truetype(str(path), size) if path.exists() else ImageFont.load_default()


def _canonical_image(source: Image.Image, body: dict) -> Image.Image:
    crop_box = tuple(body["sourceCrop"])
    crop = source.crop(crop_box)
    scale = body["scale"]
    resized = crop.resize((round(crop.width * scale), round(crop.height * scale)), Image.Resampling.LANCZOS)
    pelvis_x = body["landmarks"]["pelvis"][0]
    dest_x = round(CANVAS_WIDTH / 2 - (pelvis_x - crop_box[0]) * scale)
    dest_y = round(GROUND_Y - (body["sourceGroundY"] - crop_box[1]) * scale)
    canvas = Image.new("RGB", (CANVAS_WIDTH, CANVAS_HEIGHT), (250, 250, 249))
    canvas.paste(resized, (dest_x, dest_y))
    return canvas


def _draw_chain(draw: ImageDraw.ImageDraw, points: dict[str, tuple[int, int]], bones: tuple, color: tuple[int, int, int]) -> None:
    for start, end in bones:
        draw.line((points[start], points[end]), fill=(20, 27, 34), width=18)
        draw.line((points[start], points[end]), fill=color, width=10)


def _overlay(clean: Image.Image, body: dict, sex: str) -> Image.Image:
    canvas = clean.copy().convert("RGBA")
    veil = Image.new("RGBA", canvas.size, (4, 19, 25, 0))
    ImageDraw.Draw(veil).rectangle((0, 0, CANVAS_WIDTH, CANVAS_HEIGHT), fill=(4, 19, 25, 62))
    canvas.alpha_composite(veil)
    draw = ImageDraw.Draw(canvas)
    points = {name: project_point(body, point) for name, point in body["landmarks"].items()}
    _draw_chain(draw, points, FAR_BONES, (244, 184, 78))
    _draw_chain(draw, points, CORE_BONES, (108, 222, 147))
    _draw_chain(draw, points, NEAR_BONES, (53, 205, 238))
    for name, point in points.items():
        color = (108, 222, 147)
        if name.endswith("_far"):
            color = (244, 184, 78)
        elif name.endswith("_near"):
            color = (53, 205, 238)
        radius = 11 if name in {"head", "neck", "pelvis", "root"} else 9
        draw.ellipse((point[0]-radius, point[1]-radius, point[0]+radius, point[1]+radius), fill=color, outline=(20, 27, 34), width=4)
    draw.rounded_rectangle((24, 24, 1000, 112), radius=18, fill=(3, 26, 34, 225), outline=(104, 214, 193, 255), width=3)
    draw.text((48, 40), f"{sex.upper()} — canonical bind landmarks v1", font=_font(28), fill=(245, 246, 238))
    draw.text((48, 76), "green core  •  cyan near chain  •  gold far chain  •  source scale fixed", font=_font(18), fill=(171, 225, 216))
    draw.line((32, GROUND_Y, CANVAS_WIDTH-32, GROUND_Y), fill=(105, 224, 207), width=3)
    return canvas.convert("RGB")


def _length(a: tuple[int, int], b: tuple[int, int]) -> float:
    return round(math.dist(a, b), 2)


def _sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def build(source_path: Path, output_dir: Path) -> dict:
    output_dir.mkdir(parents=True, exist_ok=True)
    source = Image.open(source_path).convert("RGB")
    config = profile_config()
    bodies_report = {}
    previews = []
    for sex, body in config["bodies"].items():
        clean = _canonical_image(source, body)
        overlay = _overlay(clean, body, sex)
        clean_path = output_dir / f"{sex}-canonical-bind-clean-v1.png"
        overlay_path = output_dir / f"{sex}-canonical-bind-landmarks-v1.png"
        clean.save(clean_path, optimize=True)
        overlay.save(overlay_path, optimize=True)
        points = {name: project_point(body, point) for name, point in body["landmarks"].items()}
        lengths = {f"{a}__{b}": _length(points[a], points[b]) for a, b in CORE_BONES + NEAR_BONES + FAR_BONES}
        bodies_report[sex] = {
            "clean": str(clean_path), "cleanSha256": _sha256(clean_path),
            "overlay": str(overlay_path), "overlaySha256": _sha256(overlay_path),
            "landmarksPx": points, "boneLengthsPx": lengths,
        }
        previews.append((sex, clean, overlay))

    board = Image.new("RGB", (1536, 1216), (5, 26, 34))
    draw = ImageDraw.Draw(board)
    draw.text((36, 24), "LGO coherent bind design → canonical registration", font=_font(32), fill=(244, 246, 239))
    draw.text((36, 66), "Clean design and technical overlay are separate; one body profile is reused by every outfit.", font=_font(20), fill=(155, 221, 210))
    cells = []
    for sex, clean, overlay in previews:
        cells.extend(((f"{sex} clean", clean), (f"{sex} skeleton", overlay)))
    for index, (label, image) in enumerate(cells):
        thumb = image.resize((360, 540), Image.Resampling.LANCZOS)
        x = 24 + index * 378
        board.paste(thumb, (x, 130))
        draw.text((x, 686), label, font=_font(21), fill=(245, 204, 109))
    draw.rounded_rectangle((36, 760, 1500, 1168), radius=20, fill=(10, 45, 56), outline=(78, 170, 164), width=3)
    lines = [
        "1. The clean silhouette is the visual authority; skeleton dots never enter runtime art.",
        "2. Pelvis is registered at x=512 and both feet share groundY=1484.",
        "3. Near/far chains use the same semantic bones for male and female.",
        "4. These pivots are calibrated once. Outfit parts reuse them without pose offsets.",
        "5. Layer authoring stays locked until this neutral design and landmark map pass visual review.",
    ]
    for i, line in enumerate(lines):
        draw.text((72, 805 + i*62), line, font=_font(24), fill=(229, 238, 232))
    board_path = output_dir / "male-female-canonical-bind-review-v1.png"
    board.save(board_path, optimize=True)
    report = {
        **{key: value for key, value in config.items() if key != "bodies"},
        "source": str(source_path), "sourceSha256": _sha256(source_path),
        "designRole": "COHERENT_DESIGN_CANDIDATE_ONLY",
        "runtimeReady": False, "layerSourceReady": False,
        "compositeSlicingAllowed": False,
        "bodies": bodies_report,
        "reviewBoard": str(board_path), "reviewBoardSha256": _sha256(board_path),
        "nextGate": "NATIVE_LAYERED_SOURCE_WITH_HIDDEN_GEOMETRY",
    }
    report_path = output_dir / "canonical-bind-profile-v1.json"
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return report


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, default=DEFAULT_SOURCE)
    parser.add_argument("--output-dir", type=Path, default=DEFAULT_OUTPUT)
    args = parser.parse_args()
    report = build(args.source.resolve(), args.output_dir.resolve())
    print(json.dumps({"status": report["status"], "reviewBoard": report["reviewBoard"]}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
