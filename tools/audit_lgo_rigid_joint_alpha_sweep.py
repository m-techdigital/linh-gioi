#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import json
import math
from pathlib import Path
from typing import Any

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
FONT = Path("/System/Library/Fonts/Supplemental/Arial.ttf")
FONT_BOLD = Path("/System/Library/Fonts/Supplemental/Arial Bold.ttf")


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def _angles(rotation_range: tuple[float, float], step: float) -> list[float]:
    low, high = rotation_range
    if step <= 0 or high < low:
        raise ValueError("rotation range and angle step are invalid")
    count = math.ceil((high - low) / step)
    values = [min(low + index * step, high) for index in range(count + 1)]
    if values[-1] != high:
        values.append(high)
    return values


def _rotate_about_pivot(alpha: Image.Image, pivot: tuple[int, int], angle: float) -> Image.Image:
    radians = math.radians(angle)
    cosine = math.cos(radians)
    sine = math.sin(radians)
    px, py = pivot
    inverse = (
        cosine,
        sine,
        px - cosine * px - sine * py,
        -sine,
        cosine,
        py + sine * px - cosine * py,
    )
    return alpha.transform(
        alpha.size,
        Image.Transform.AFFINE,
        inverse,
        resample=Image.Resampling.BICUBIC,
        fillcolor=0,
    )


def _disk_points(pivot: tuple[int, int], radius: int, size: tuple[int, int]) -> list[tuple[int, int]]:
    px, py = pivot
    width, height = size
    points: list[tuple[int, int]] = []
    for y in range(max(0, py - radius), min(height, py + radius + 1)):
        for x in range(max(0, px - radius), min(width, px + radius + 1)):
            if (x - px) ** 2 + (y - py) ** 2 <= radius**2:
                points.append((x, y))
    return points


def _missing_pixels(alpha: Image.Image, points: list[tuple[int, int]], threshold: int) -> int:
    pixels = alpha.load()
    return sum(1 for x, y in points if pixels[x, y] < threshold)


def audit_joint(
    parent_path: Path,
    child_path: Path,
    *,
    pivot: tuple[int, int],
    cap_radius: int,
    filter_guard: int,
    rotation_range: tuple[float, float],
    angle_step: float,
    alpha_threshold: int = 250,
) -> dict[str, Any]:
    with Image.open(parent_path) as parent_source, Image.open(child_path) as child_source:
        if parent_source.mode != "RGBA" or child_source.mode != "RGBA":
            raise ValueError("parent and child must be real RGBA source images")
        if parent_source.size != child_source.size:
            raise ValueError("parent and child must use the same source canvas")
        parent = parent_source.getchannel("A")
        child = child_source.getchannel("A")

    audit_radius = cap_radius - filter_guard
    if audit_radius <= 0:
        raise ValueError("capRadiusPx must be greater than filterGuardPx")
    points = _disk_points(pivot, audit_radius, parent.size)
    expected_pixels = len(points)
    if expected_pixels == 0:
        raise ValueError("pivot/cap does not intersect the source canvas")

    parent_missing = _missing_pixels(parent, points, alpha_threshold)
    samples = []
    for angle in _angles(rotation_range, angle_step):
        rotated_child = _rotate_about_pivot(child, pivot, angle)
        missing = _missing_pixels(rotated_child, points, alpha_threshold)
        samples.append({
            "angleDeg": angle,
            "childMissingPixels": missing,
            "childCoverageRatio": round((expected_pixels - missing) / expected_pixels, 6),
        })
    max_child_missing = max(sample["childMissingPixels"] for sample in samples)
    passed = parent_missing == 0 and max_child_missing == 0
    return {
        "status": "JOINT_ALPHA_SWEEP_PASS" if passed else "JOINT_ALPHA_SWEEP_FAILED",
        "parentPath": str(parent_path),
        "parentSha256": sha256(parent_path),
        "childPath": str(child_path),
        "childSha256": sha256(child_path),
        "canvas": list(parent.size),
        "pivotPx": list(pivot),
        "capRadiusPx": cap_radius,
        "filterGuardPx": filter_guard,
        "auditRadiusPx": audit_radius,
        "alphaThreshold": alpha_threshold,
        "expectedDiscPixels": expected_pixels,
        "parentMissingPixels": parent_missing,
        "parentCoverageRatio": round((expected_pixels - parent_missing) / expected_pixels, 6),
        "maxChildMissingPixels": max_child_missing,
        "sampleCount": len(samples),
        "samples": samples,
    }


def _font(path: Path, size: int) -> ImageFont.ImageFont:
    return ImageFont.truetype(str(path), size) if path.exists() else ImageFont.load_default()


def _tinted(alpha: Image.Image, color: tuple[int, int, int]) -> Image.Image:
    image = Image.new("RGBA", alpha.size, (*color, 0))
    image.putalpha(alpha)
    return image


def _render_contact_sheet(package: dict[str, Any], report: dict[str, Any], output_path: Path) -> None:
    sample_columns = 5
    cell_width, cell_height = 180, 190
    board_width = 80 + sample_columns * cell_width
    board_height = 120 + len(report["joints"]) * cell_height
    board = Image.new("RGB", (board_width, board_height), "#0a1b24")
    draw = ImageDraw.Draw(board)
    title = _font(FONT_BOLD, 28)
    label = _font(FONT, 16)
    draw.text((24, 20), "LGO RIGID JOINT — REAL ALPHA ROTATION SWEEP", font=title, fill="#eff8fa")
    draw.text((24, 62), "Blue: parent underlap · Orange: rotated child cap · red ring: audited disc", font=label, fill="#8bd5de")

    for row, (joint_input, joint_report) in enumerate(zip(package["joints"], report["joints"])):
        parent_path = Path(joint_input["parentPath"])
        child_path = Path(joint_input["childPath"])
        with Image.open(parent_path) as parent_source, Image.open(child_path) as child_source:
            parent_alpha = parent_source.getchannel("A")
            child_alpha = child_source.getchannel("A")
        samples = joint_report["samples"]
        sample_indices = [round(index * (len(samples) - 1) / (sample_columns - 1)) for index in range(sample_columns)]
        px, py = joint_report["pivotPx"]
        radius = joint_report["capRadiusPx"]
        crop_radius = max(radius * 2, 32)
        crop_box = (px - crop_radius, py - crop_radius, px + crop_radius, py + crop_radius)
        y = 105 + row * cell_height
        draw.text((24, y), f"{joint_input['id']}  {joint_report['status']}", font=label,
                  fill="#70e1ac" if joint_report["status"].endswith("PASS") else "#ff7a70")
        for column, sample_index in enumerate(sample_indices):
            sample = samples[sample_index]
            angle = sample["angleDeg"]
            rotated = _rotate_about_pivot(child_alpha, (px, py), angle)
            composite = Image.new("RGBA", parent_alpha.size, (245, 248, 248, 255))
            composite.alpha_composite(_tinted(parent_alpha, (62, 143, 204)))
            composite.alpha_composite(_tinted(rotated, (239, 143, 75)))
            crop = composite.crop(crop_box).resize((140, 140), Image.Resampling.NEAREST).convert("RGB")
            x = 24 + column * cell_width
            board.paste(crop, (x, y + 28))
            draw.ellipse((x + 35, y + 63, x + 105, y + 133), outline="#ff4e63", width=2)
            draw.text((x + 4, y + 169), f"{angle:g}° miss={sample['childMissingPixels']}", font=label,
                      fill="#dce8ea")
    board.save(output_path, quality=94, optimize=True)


def build(package: dict[str, Any], output_dir: Path) -> dict[str, Any]:
    joints = []
    for joint in package.get("joints", []):
        result = audit_joint(
            Path(joint["parentPath"]),
            Path(joint["childPath"]),
            pivot=tuple(joint["pivotPx"]),
            cap_radius=int(joint["capRadiusPx"]),
            filter_guard=int(joint["filterGuardPx"]),
            rotation_range=tuple(joint["rotationRangeDeg"]),
            angle_step=float(joint["angleStepDeg"]),
            alpha_threshold=int(joint.get("alphaThreshold", 250)),
        )
        result["id"] = joint["id"]
        joints.append(result)
    if not joints:
        raise ValueError("package must contain at least one joint")
    failure_count = sum(joint["status"] != "JOINT_ALPHA_SWEEP_PASS" for joint in joints)
    report = {
        "status": "RIGID_JOINT_ALPHA_SWEEP_PASS" if failure_count == 0 else "RIGID_JOINT_ALPHA_SWEEP_FAILED",
        "profileId": package.get("profileId"),
        "jointCount": len(joints),
        "failureCount": failure_count,
        "scope": "SOURCE_ALPHA_GEOMETRY_ONLY_VISUAL_SILHOUETTE_REVIEW_STILL_REQUIRED",
        "joints": joints,
    }
    output_dir.mkdir(parents=True, exist_ok=True)
    (output_dir / "joint-alpha-sweep-report.json").write_text(
        json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    _render_contact_sheet(package, report, output_dir / "joint-alpha-sweep-contact-sheet.png")
    return report


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--package", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    args = parser.parse_args()
    package = json.loads(args.package.read_text(encoding="utf-8"))
    report = build(package, args.output_dir)
    print(json.dumps({"status": report["status"], "failureCount": report["failureCount"]}))
    return 0 if report["failureCount"] == 0 else 2


if __name__ == "__main__":
    raise SystemExit(main())
