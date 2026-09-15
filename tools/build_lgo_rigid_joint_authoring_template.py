#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import math
from pathlib import Path
from typing import Any

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
DEFAULT_PROFILE = ROOT / "docs/art/data/lgo-rigid-joint-interface-profile-v1.json"
DEFAULT_OUTPUT = ROOT / "build/rigid-outfit-pilot/final-1-joint-authoring-v1"
FONT_REGULAR = Path("/System/Library/Fonts/Supplemental/Arial.ttf")
FONT_BOLD = Path("/System/Library/Fonts/Supplemental/Arial Bold.ttf")


def _required_number(mapping: dict[str, Any], key: str) -> float:
    value = mapping.get(key)
    if not isinstance(value, (int, float)) or value < 0:
        raise ValueError(f"{key} must be a non-negative number")
    return float(value)


def compute_joint_contract(
    joint: dict[str, Any], safety: dict[str, Any], canvas: tuple[int, int]
) -> dict[str, Any]:
    pivot = joint.get("pivotPx")
    if (
        not isinstance(pivot, list)
        or len(pivot) != 2
        or not all(isinstance(value, (int, float)) for value in pivot)
    ):
        raise ValueError(f"joint {joint.get('id', '<unknown>')} must define one shared pivotPx")

    width, height = canvas
    if not (0 <= pivot[0] < width and 0 <= pivot[1] < height):
        raise ValueError(f"joint {joint.get('id', '<unknown>')} pivotPx is outside canvas")

    rotation_range = joint.get("rotationRangeDeg")
    if (
        not isinstance(rotation_range, list)
        or len(rotation_range) != 2
        or not all(isinstance(value, (int, float)) for value in rotation_range)
        or rotation_range[1] < rotation_range[0]
    ):
        raise ValueError(f"joint {joint.get('id', '<unknown>')} has invalid rotationRangeDeg")

    outline = _required_number(safety, "outlinePx")
    filter_guard = _required_number(safety, "filterGuardPx")
    registration = _required_number(safety, "registrationTolerancePx")
    sample_step = _required_number(safety, "angleSampleStepDeg")
    if sample_step <= 0:
        raise ValueError("angleSampleStepDeg must be greater than zero")

    body_width = _required_number(joint, "bodyWidthPx")
    outfit_width = _required_number(joint, "outfitWidthPx")
    cloth_clearance = _required_number(joint, "clothClearancePx")
    source_safety = outline + filter_guard + registration
    body_half_width = body_width / 2.0
    outfit_half_width = outfit_width / 2.0
    body_radius = math.ceil(body_half_width + source_safety)
    outfit_radius = math.ceil(
        max(outfit_half_width, body_half_width + cloth_clearance) + source_safety
    )
    sweep_span = rotation_range[1] - rotation_range[0]
    sweep_samples = math.ceil(sweep_span / sample_step) + 1

    return {
        **joint,
        "pivotPx": [int(round(pivot[0])), int(round(pivot[1]))],
        "pivotNormalized": [round(pivot[0] / width, 6), round(pivot[1] / height, 6)],
        "sourceSafetyPx": int(math.ceil(source_safety)),
        "bodyCapRadiusPx": body_radius,
        "outfitCapRadiusPx": outfit_radius,
        "bodyMinimumPivotEmbedPx": body_radius,
        "outfitMinimumPivotEmbedPx": outfit_radius,
        "sweepSampleCount": sweep_samples,
        "bodyJoinRule": "CHILD_CAP_OVER_PARENT_UNDERLAP",
        "outfitJoinRule": "CHILD_COVER_OVER_PARENT_UNDERLAP",
        "pivotOwnership": "ONE_SHARED_BODY_BONE_PIVOT",
    }


def _fonts() -> tuple[ImageFont.ImageFont, ImageFont.ImageFont, ImageFont.ImageFont]:
    if FONT_REGULAR.exists() and FONT_BOLD.exists():
        return (
            ImageFont.truetype(str(FONT_BOLD), 38),
            ImageFont.truetype(str(FONT_BOLD), 23),
            ImageFont.truetype(str(FONT_REGULAR), 18),
        )
    default = ImageFont.load_default()
    return default, default, default


def _render_template(report: dict[str, Any], output_path: Path) -> None:
    width = report["canvas"]["width"]
    height = report["canvas"]["height"]
    image = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image, "RGBA")
    for joint in report["joints"]:
        x, y = joint["pivotPx"]
        outfit_r = joint["outfitCapRadiusPx"]
        body_r = joint["bodyCapRadiusPx"]
        draw.ellipse(
            (x - outfit_r, y - outfit_r, x + outfit_r, y + outfit_r),
            fill=(38, 190, 181, 42),
            outline=(38, 190, 181, 230),
            width=2,
        )
        draw.ellipse(
            (x - body_r, y - body_r, x + body_r, y + body_r),
            fill=(255, 180, 124, 52),
            outline=(255, 180, 124, 240),
            width=2,
        )
        draw.line((x - 8, y, x + 8, y), fill=(255, 75, 91, 255), width=2)
        draw.line((x, y - 8, x, y + 8), fill=(255, 75, 91, 255), width=2)
    image.save(output_path, optimize=True)


def _render_review_board(report: dict[str, Any], output_path: Path) -> None:
    title_font, heading_font, body_font = _fonts()
    image = Image.new("RGB", (2200, 1600), "#0a1b24")
    draw = ImageDraw.Draw(image)
    draw.text((48, 38), "LGO RIGID JOINT AUTHORING — BODY + OUTFIT", font=title_font, fill="#eef7fa")
    draw.text(
        (48, 92),
        "Một pivot dùng chung · body tự kín khi tháo đồ · outfit có cap/underlap riêng",
        font=heading_font,
        fill="#8bd5de",
    )

    panels = [
        ("1. BODY TRƯỚC", "Parent body kéo qua pivot\nChild body có cap tròn, render trên\nKhông contour ở vùng bị chồng"),
        ("2. OUTFIT SAU", "Mỗi mảnh outfit thuộc đúng một bone\nChild cover phủ parent underlap\nKhông ảnh bắc qua hai bone"),
        ("3. THÁO ĐỒ", "Body-only vẫn kín mọi góc xoay\nTắt từng part/slot không mất da\nKhông pixel body nằm trong equipment"),
        ("4. CHUYỂN ĐỘNG", "Cùng sprite, pivot và local fit\nChỉ position + rotation\nScale = 1; không deform/swap"),
    ]
    for index, (heading, lines) in enumerate(panels):
        x = 48 + index * 530
        draw.rounded_rectangle((x, 145, x + 490, 340), 18, fill="#13303b", outline="#3fb8b4", width=3)
        draw.text((x + 20, 165), heading, font=heading_font, fill="#ffd074")
        draw.multiline_text((x + 20, 215), lines, font=body_font, fill="#e8f0f2", spacing=9)

    draw.text((48, 385), "CÔNG THỨC KHÓA", font=heading_font, fill="#eef7fa")
    formulas = [
        "M = outline + filter guard + registration tolerance",
        "R_body = ceil(body joint width / 2 + M)",
        "R_outfit = ceil(max(outfit width / 2, body width / 2 + clearance) + M)",
        "Mỗi sprite kề pivot phải chứa đủ cap/underlap sâu ít nhất R; hình tròn không đổi khi xoay.",
        "Pivot item = pivot body = pivot bone; local fit chỉ author một lần trên common canvas.",
    ]
    for index, line in enumerate(formulas):
        draw.text((72, 430 + index * 42), line, font=body_font, fill="#d4e5e9")

    draw.text((48, 660), "JOINT INTERFACE PROFILE", font=heading_font, fill="#eef7fa")
    columns = [48, 300, 470, 640, 820, 990, 1190, 1410, 1660, 1900]
    headers = ["Joint", "Pivot", "Body W", "Outfit W", "R body", "R outfit", "ROM", "Samples", "Depth", "State"]
    for x, header in zip(columns, headers):
        draw.text((x, 705), header, font=body_font, fill="#84d9d1")
    y = 748
    for joint in report["joints"]:
        values = [
            joint["id"],
            f"{joint['pivotPx'][0]},{joint['pivotPx'][1]}",
            str(joint["bodyWidthPx"]),
            str(joint["outfitWidthPx"]),
            str(joint["bodyCapRadiusPx"]),
            str(joint["outfitCapRadiusPx"]),
            f"{joint['rotationRangeDeg'][0]}..{joint['rotationRangeDeg'][1]}",
            str(joint["sweepSampleCount"]),
            joint["depth"],
            "CALIBRATE" if report["status"].endswith("CALIBRATION") else "READY",
        ]
        for x, value in zip(columns, values):
            draw.text((x, y), value, font=body_font, fill="#f0f5f6")
        draw.line((48, y + 30, 2150, y + 30), fill="#234653", width=1)
        y += 43

    box_top = 1400
    draw.rounded_rectangle((48, box_top, 2150, 1540), 18, fill="#321e22", outline="#ff756d", width=3)
    draw.text((72, box_top + 18), "GATE NGUỒN", font=heading_font, fill="#ff8b82")
    draw.text(
        (72, box_top + 58),
        "Chưa có body bind source đo thật → profile chỉ là công thức, không rig. Sau calibration: body-only sweep → từng slot off → all-on.",
        font=body_font,
        fill="#ffe5dc",
    )
    draw.text(
        (72, box_top + 94),
        "Có khe body ở bất kỳ góc nào: sửa body cap/underlap. Có hỏng outfit: sửa segmentation/pivot/cover; không thêm pose offset.",
        font=body_font,
        fill="#ffe5dc",
    )
    image.save(output_path, quality=94, optimize=True)


def build(profile: dict[str, Any], output_dir: Path) -> dict[str, Any]:
    canvas_data = profile.get("canvas", {})
    width = int(canvas_data.get("width", 0))
    height = int(canvas_data.get("height", 0))
    if width <= 0 or height <= 0:
        raise ValueError("canvas width and height must be greater than zero")
    joints = [
        compute_joint_contract(joint, profile.get("safety", {}), (width, height))
        for joint in profile.get("joints", [])
    ]
    if not joints:
        raise ValueError("profile must contain at least one joint")

    outfit_parts: list[str] = []
    for joint in joints:
        for key in ("parentOutfitPart", "childOutfitPart"):
            part = joint.get(key)
            if part and part not in outfit_parts:
                outfit_parts.append(part)

    calibrated = profile.get("status") == "CALIBRATED_BODY_BIND_PROFILE"
    report = {
        "status": (
            "RIGID_JOINT_AUTHORING_TEMPLATE_READY"
            if calibrated or profile.get("status") == "AUTHORING_PROFILE"
            else "RIGID_JOINT_FORMULA_READY_AWAITING_BODY_CALIBRATION"
        ),
        "profileId": profile.get("profileId"),
        "profileStatus": profile.get("status"),
        "canvas": {"width": width, "height": height},
        "safety": profile.get("safety", {}),
        "formula": {
            "sourceSafetyPx": "outlinePx + filterGuardPx + registrationTolerancePx",
            "bodyCapRadiusPx": "ceil(bodyWidthPx / 2 + sourceSafetyPx)",
            "outfitCapRadiusPx": "ceil(max(outfitWidthPx / 2, bodyWidthPx / 2 + clothClearancePx) + sourceSafetyPx)",
        },
        "bodyMustPassWithoutAnyEquipment": True,
        "equipmentMayNeverSupplyBodyPixels": True,
        "oneSharedPivotForBodyAndOutfit": True,
        "requiredUnequipProofStates": [
            "body_only",
            "outfit_all",
            *[f"without_{part}" for part in outfit_parts],
        ],
        "joints": joints,
        "prohibited": [
            "RECTANGULAR_ALPHA_SPLIT_AS_JOINT_PROOF",
            "BODY_PIXELS_IN_EQUIPMENT",
            "PART_SPANNING_MULTIPLE_BONES",
            "PER_POSE_OFFSET",
            "DEFORMATION",
            "ANIMATED_SCALE",
            "POSE_SPRITE_SWAP",
        ],
    }
    output_dir.mkdir(parents=True, exist_ok=True)
    (output_dir / "joint-authoring-report.json").write_text(
        json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    _render_template(report, output_dir / "joint-interface-template.png")
    _render_review_board(report, output_dir / "joint-contract-review.png")
    return report


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--profile", type=Path, default=DEFAULT_PROFILE)
    parser.add_argument("--output-dir", type=Path, default=DEFAULT_OUTPUT)
    args = parser.parse_args()
    profile = json.loads(args.profile.read_text(encoding="utf-8"))
    report = build(profile, args.output_dir)
    print(json.dumps({"status": report["status"], "jointCount": len(report["joints"])}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
