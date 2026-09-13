#!/usr/bin/env python3.12
"""Generate review-only outer_top mask/lineart templates from measured anchors."""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageDraw

CANVAS = (1024, 1536)


def sha256(path: Path | str) -> str:
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def point(value: list[float]) -> tuple[float, float]:
    return float(value[0]), float(value[1])


def interp(a: tuple[float, float], b: tuple[float, float], t: float) -> tuple[float, float]:
    return a[0] + (b[0] - a[0]) * t, a[1] + (b[1] - a[1]) * t


def add(a: tuple[float, float], b: tuple[float, float]) -> tuple[float, float]:
    return a[0] + b[0], a[1] + b[1]


def sub(a: tuple[float, float], b: tuple[float, float]) -> tuple[float, float]:
    return a[0] - b[0], a[1] - b[1]


def mul(a: tuple[float, float], scale: float) -> tuple[float, float]:
    return a[0] * scale, a[1] * scale


def curve(points: list[tuple[float, float]], steps: int = 12) -> list[tuple[float, float]]:
    if len(points) == 2:
        return [interp(points[0], points[1], index / steps) for index in range(steps + 1)]
    if len(points) != 3:
        raise ValueError("Only quadratic curves are supported")
    output = []
    for index in range(steps + 1):
        t = index / steps
        a = interp(points[0], points[1], t)
        b = interp(points[1], points[2], t)
        output.append(interp(a, b, t))
    return output


def polygon_from_guides(outer_top: dict) -> list[tuple[float, float]]:
    shoulder_left, shoulder_right = [point(v) for v in outer_top["shoulderLine"]]
    waist_left, waist_right = [point(v) for v in outer_top["waistLine"]]
    return [
        interp(shoulder_left, shoulder_right, 0.08),
        interp(shoulder_left, shoulder_right, 0.92),
        interp(waist_left, waist_right, 0.92),
        interp(waist_left, waist_right, 0.08),
    ]


def cloth_outline_from_guides(outer_top: dict) -> list[tuple[float, float]]:
    shoulder_left, shoulder_right = [point(v) for v in outer_top["shoulderLine"]]
    waist_left, waist_right = [point(v) for v in outer_top["waistLine"]]
    neck, hip = [point(v) for v in outer_top["torsoAxis"]]
    shoulder_mid = interp(shoulder_left, shoulder_right, 0.5)
    left_top = interp(shoulder_left, shoulder_right, 0.18)
    right_top = interp(shoulder_left, shoulder_right, 0.86)
    left_waist = interp(waist_left, waist_right, 0.18)
    right_waist = interp(waist_left, waist_right, 0.86)
    left_control = add(interp(left_top, left_waist, 0.48), (-28, 6))
    right_control = add(interp(right_top, right_waist, 0.48), (28, 10))
    hem_control = add(interp(left_waist, right_waist, 0.5), (0, 34))
    neckline = add(interp(neck, shoulder_mid, 0.58), (0, 20))
    return (
        curve([left_top, shoulder_mid, right_top], 10)
        + curve([right_top, right_control, right_waist], 14)[1:]
        + curve([right_waist, hem_control, left_waist], 14)[1:]
        + curve([left_waist, left_control, left_top], 14)[1:]
        + [neckline]
    )


def collar_polygon(outer_top: dict) -> list[tuple[float, float]]:
    neck, hip = [point(v) for v in outer_top["torsoAxis"]]
    shoulder_left, shoulder_right = [point(v) for v in outer_top["shoulderLine"]]
    shoulder_mid = interp(shoulder_left, shoulder_right, 0.5)
    width = abs(shoulder_right[0] - shoulder_left[0]) * 0.18
    return [
        (shoulder_mid[0] - width, shoulder_mid[1] - 6),
        (shoulder_mid[0] + width, shoulder_mid[1] - 2),
        interp(neck, hip, 0.18),
    ]


def armhole_polygon(outer_top: dict) -> list[tuple[float, float]]:
    shoulder_left, shoulder_right = [point(v) for v in outer_top["shoulderLine"]]
    waist_left, waist_right = [point(v) for v in outer_top["waistLine"]]
    top = interp(shoulder_left, shoulder_right, 0.11)
    lower = interp(waist_left, waist_right, 0.2)
    control = add(interp(top, lower, 0.38), (-42, -4))
    inner = add(interp(top, lower, 0.48), (28, 8))
    return curve([top, control, lower], 14) + curve([lower, inner, top], 10)[1:]


def template_geometry(outer_top: dict, template_style: str) -> tuple[
    list[tuple[float, float]],
    list[tuple[float, float]],
    list[tuple[float, float]] | None,
    list[str],
    list[str],
]:
    if template_style == "measured_trapezoid_v1":
        return polygon_from_guides(outer_top), collar_polygon(outer_top), None, [], ["outline", "collar_cutout"]
    if template_style == "cloth_lineart_v2":
        return (
            cloth_outline_from_guides(outer_top),
            collar_polygon(outer_top),
            armhole_polygon(outer_top),
            ["curved_side_seams", "collar_cutout", "near_armhole_cutout", "curved_hem"],
            ["outline", "collar_cutout", "near_armhole_cutout", "torso_axis", "shoulder_line"],
        )
    if template_style == "cloth_lineart_v3":
        return (
            cloth_outline_from_guides(outer_top),
            collar_polygon(outer_top),
            armhole_polygon(outer_top),
            ["curved_side_seams", "collar_cutout", "mask_only_near_armhole", "curved_hem"],
            ["outline", "collar_cutout", "torso_axis", "shoulder_line"],
        )
    raise ValueError(f"Unsupported template style: {template_style}")


def generate_pose_template(pose: str, pose_measurement: dict, output: Path, template_style: str) -> dict:
    outer_top = pose_measurement["slotGuides"]["outer_top"]
    poly, collar, armhole, refinements, visible_lineart = template_geometry(outer_top, template_style)

    mask = Image.new("RGBA", CANVAS, (0, 0, 0, 0))
    draw = ImageDraw.Draw(mask)
    draw.polygon(poly, fill=(22, 84, 132, 120))
    draw.polygon(collar, fill=(0, 0, 0, 0))
    if armhole:
        draw.polygon(armhole, fill=(0, 0, 0, 0))

    lineart = Image.new("RGBA", CANVAS, (0, 0, 0, 0))
    draw = ImageDraw.Draw(lineart)
    draw.line(poly + [poly[0]], fill=(0, 220, 255, 255), width=5)
    draw.line(collar + [collar[0]], fill=(255, 220, 0, 255), width=4)
    if armhole and "near_armhole_cutout" in visible_lineart:
        draw.line(armhole + [armhole[0]], fill=(255, 120, 0, 255), width=4)
    draw.line([tuple(v) for v in outer_top["torsoAxis"]], fill=(0, 128, 255, 255), width=3)
    draw.line([tuple(v) for v in outer_top["shoulderLine"]], fill=(0, 220, 0, 255), width=3)

    mask_path = output / f"{pose}-outer-top-template-mask.png"
    lineart_path = output / f"{pose}-outer-top-template-lineart.png"
    preview_path = output / f"{pose}-outer-top-template-preview.png"
    preview = Image.alpha_composite(mask, lineart)
    mask.save(mask_path)
    lineart.save(lineart_path)
    preview.save(preview_path)
    return {
        "pose": pose,
        "mask": str(mask_path),
        "lineart": str(lineart_path),
        "preview": str(preview_path),
        "maskSha256": sha256(mask_path),
        "lineartSha256": sha256(lineart_path),
        "source": "measured_anchor_template",
        "state": "template_review_only",
        "templateStyle": template_style,
        "refinements": refinements,
        "visibleLineart": visible_lineart,
    }


def generate_templates(
    measurements_path: Path | str,
    output: Path | str,
    poses: list[str] | None = None,
    template_style: str = "measured_trapezoid_v1",
) -> dict:
    measurements_path = Path(measurements_path)
    output = Path(output)
    measurements = json.loads(measurements_path.read_text(encoding="utf-8"))
    if measurements.get("guideSanity", {}).get("status") != "ANCHOR_GUIDE_SANITY_NO_OUTLIERS":
        raise ValueError("Measurement guide sanity must pass before generating outer_top templates")
    selected = poses or list(measurements["poses"])
    output.mkdir(parents=True, exist_ok=True)
    records = []
    for pose in selected:
        records.append(generate_pose_template(pose, measurements["poses"][pose], output, template_style))
    result = {
        "status": "OUTER_TOP_MEASURED_TEMPLATE_REVIEW_REQUIRED",
        "runtimeEligible": False,
        "templateStyle": template_style,
        "measurements": str(measurements_path),
        "measurementSha256": sha256(measurements_path),
        "poses": records,
        "usage": "Review-only lineart/mask template. Fill/material work may start only after stack visual review.",
    }
    (output / "outer-top-measured-template-report.json").write_text(
        json.dumps(result, ensure_ascii=False, indent=2) + "\n",
        encoding="utf-8",
    )
    return result


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--measurements", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--poses", nargs="*")
    parser.add_argument("--template-style", default="measured_trapezoid_v1")
    args = parser.parse_args()
    result = generate_templates(args.measurements, args.output, args.poses, args.template_style)
    print(json.dumps({"status": result["status"], "poses": [item["pose"] for item in result["poses"]]}, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
