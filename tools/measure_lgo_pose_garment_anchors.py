#!/usr/bin/env python3.12
"""Measure pose garment anchors from the shared landmark guide.

The output is an authoring measurement aid. It is not a runtime approval and
does not permit automatic warping while the guide remains review-required.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import math
import statistics
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


def payload_sha256(data: dict) -> str:
    payload = dict(data)
    payload.pop("auditPayloadSha256", None)
    return hashlib.sha256(
        json.dumps(payload, ensure_ascii=False, sort_keys=True, separators=(",", ":")).encode()
    ).hexdigest()


def point(value) -> tuple[float, float]:
    return float(value[0]), float(value[1])


def add(a, b):
    return a[0] + b[0], a[1] + b[1]


def sub(a, b):
    return a[0] - b[0], a[1] - b[1]


def mul(a, s: float):
    return a[0] * s, a[1] * s


def mid(a, b):
    return (a[0] + b[0]) / 2.0, (a[1] + b[1]) / 2.0


def length(a):
    return math.hypot(a[0], a[1])


def normalize(a):
    value = length(a)
    if value <= 1e-6:
        raise ValueError("Cannot normalize zero vector")
    return a[0] / value, a[1] / value


def round_point(a):
    return [round(a[0], 2), round(a[1], 2)]


def line(center, direction, half_width):
    delta = mul(direction, half_width)
    return [round_point(sub(center, delta)), round_point(add(center, delta))]


def pose_landmarks(pose: dict) -> dict[str, tuple[float, float]]:
    landmarks = {item["id"]: point(item["xy"]) for item in pose.get("landmarks", [])}
    required = {"neck", "near_shoulder", "far_shoulder", "near_hip", "far_hip"}
    missing = sorted(required - set(landmarks))
    if missing:
        raise ValueError(f"{pose.get('pose')}: missing landmarks {missing}")
    return landmarks


def measure_pose(pose: dict) -> dict:
    landmarks = pose_landmarks(pose)
    neck = landmarks["neck"]
    shoulder_mid = mid(landmarks["near_shoulder"], landmarks["far_shoulder"])
    hip_mid = mid(landmarks["near_hip"], landmarks["far_hip"])
    shoulder_vec = sub(landmarks["far_shoulder"], landmarks["near_shoulder"])
    hip_vec = sub(landmarks["far_hip"], landmarks["near_hip"])
    torso_vec = sub(hip_mid, neck)
    torso_axis = normalize(torso_vec)
    shoulder_axis = normalize(shoulder_vec)
    shoulder_width = length(shoulder_vec)
    hip_width = length(hip_vec)
    torso_length = length(torso_vec)
    chest_center = add(neck, mul(torso_axis, torso_length * 0.36))
    waist_center = add(neck, mul(torso_axis, torso_length * 0.82))
    belt_half = max(hip_width * 0.68, shoulder_width * 0.42)
    shoulder_half = shoulder_width * 0.62
    outer_half = max(shoulder_width * 0.55, hip_width * 0.8)
    angle = math.degrees(math.atan2(shoulder_axis[1], shoulder_axis[0]))
    return {
        "source": pose.get("source"),
        "sourceSha256": pose.get("sourceSha256"),
        "guideStatus": pose.get("status"),
        "anchors": {
            "neck": {"xy": round_point(neck)},
            "shoulderMid": {"xy": round_point(shoulder_mid)},
            "hipMid": {"xy": round_point(hip_mid)},
            "chestCenter": {"xy": round_point(chest_center)},
            "waistCenter": {"xy": round_point(waist_center)},
        },
        "landmarkSnapshot": {
            key: round_point(value)
            for key, value in landmarks.items()
            if key in {
                "crown", "chin", "neck",
                "near_shoulder", "far_shoulder",
                "near_elbow", "far_elbow",
                "near_wrist", "far_wrist",
                "near_hip", "far_hip",
                "near_knee", "far_knee",
                "near_ankle", "far_ankle",
            }
        },
        "measurements": {
            "torsoLengthPx": round(torso_length, 2),
            "shoulderWidthPx": round(shoulder_width, 2),
            "hipWidthPx": round(hip_width, 2),
            "shoulderLineAngleDegrees": round(angle, 2),
        },
        "slotGuides": {
            "outer_top": {
                "center": round_point(chest_center),
                "shoulderLine": line(shoulder_mid, shoulder_axis, shoulder_half),
                "waistLine": line(waist_center, shoulder_axis, outer_half),
                "torsoAxis": [round_point(neck), round_point(hip_mid)],
            },
            "waist_belt": {
                "center": round_point(waist_center),
                "line": line(waist_center, shoulder_axis, belt_half),
                "angleDegrees": round(angle, 2),
                "halfWidthPx": round(belt_half, 2),
            },
            "shoulder_chest_guard": {
                "center": round_point(add(neck, mul(torso_axis, torso_length * 0.25))),
                "shoulderLine": line(shoulder_mid, shoulder_axis, shoulder_half * 0.85),
                "chestLine": line(chest_center, shoulder_axis, shoulder_half * 0.55),
                "angleDegrees": round(angle, 2),
            },
        },
    }




def optional_landmark_ratio(entry: dict, ratio_name: str) -> float | None:
    landmarks = entry.get("landmarkSnapshot", {})
    torso = entry["measurements"].get("torsoLengthPx", 0)
    if torso <= 1e-6:
        return None
    if ratio_name == "headToTorso" and {"crown", "chin"} <= set(landmarks):
        return round(length(sub(tuple(landmarks["chin"]), tuple(landmarks["crown"]))) / torso, 4)
    if ratio_name == "shoulderToTorso":
        return round(entry["measurements"]["shoulderWidthPx"] / torso, 4)
    if ratio_name == "hipToTorso":
        return round(entry["measurements"]["hipWidthPx"] / torso, 4)
    return None


def proportion_sanity(measured: dict) -> dict:
    ratio_names = ("headToTorso", "shoulderToTorso", "hipToTorso")
    ratios = {}
    for pose, entry in measured.items():
        ratios[pose] = {
            name: optional_landmark_ratio(entry, name)
            for name in ratio_names
        }
    available = {
        name: [pose_ratios[name] for pose_ratios in ratios.values() if pose_ratios[name] is not None]
        for name in ratio_names
    }
    if not any(available.values()):
        return {
            "status": "PROPORTION_SANITY_NO_OPTIONAL_RATIOS",
            "ratios": ratios,
            "outliers": [],
            "usage": "Add crown/chin and limb landmarks before treating body proportions as reviewed.",
        }
    medians = {
        name: round(float(statistics.median(values)), 4)
        for name, values in available.items()
        if values
    }
    thresholds = {
        "headToTorso": (0.65, 1.35),
        "shoulderToTorso": (0.55, 1.45),
        "hipToTorso": (0.55, 1.45),
    }
    outliers = []
    for pose, pose_ratios in ratios.items():
        for name, value in pose_ratios.items():
            if value is None or name not in medians:
                continue
            low, high = thresholds[name]
            reason = metric_outlier(value, medians[name], low, high)
            if reason:
                outliers.append(
                    {
                        "pose": pose,
                        "ratio": name,
                        "value": value,
                        "median": medians[name],
                        "reason": reason,
                    }
                )
    return {
        "status": "PROPORTION_SANITY_REVIEW_REQUIRED" if outliers else "PROPORTION_SANITY_NO_OUTLIERS",
        "ratios": ratios,
        "medians": medians,
        "outliers": outliers,
        "usage": "Broad pose-family ratio gate. Use to decide if landmarks/body need review before asset authoring; do not auto-scale individual poses.",
    }

def median_metric(measured: dict, metric: str) -> float:
    values = [entry["measurements"][metric] for entry in measured.values()]
    if not values:
        raise ValueError(f"No values for {metric}")
    return float(statistics.median(values))


def metric_outlier(value: float, median_value: float, low_ratio: float, high_ratio: float) -> str | None:
    if median_value <= 1e-6:
        return None
    ratio = value / median_value
    if ratio < low_ratio:
        return f"low:{ratio:.2f}x_median"
    if ratio > high_ratio:
        return f"high:{ratio:.2f}x_median"
    return None


def guide_sanity(measured: dict) -> dict:
    medians = {
        "torsoLengthPx": median_metric(measured, "torsoLengthPx"),
        "shoulderWidthPx": median_metric(measured, "shoulderWidthPx"),
        "hipWidthPx": median_metric(measured, "hipWidthPx"),
    }
    checks = {
        "torsoLengthPx": (0.72, 1.28),
        "shoulderWidthPx": (0.55, 1.45),
        "hipWidthPx": (0.55, 1.45),
    }
    outliers = []
    for pose, entry in measured.items():
        for metric, (low, high) in checks.items():
            value = entry["measurements"][metric]
            reason = metric_outlier(value, medians[metric], low, high)
            if reason:
                outliers.append(
                    {
                        "pose": pose,
                        "metric": metric,
                        "value": value,
                        "median": round(medians[metric], 2),
                        "reason": reason,
                    }
                )
    return {
        "status": "ANCHOR_GUIDE_SANITY_REVIEW_REQUIRED" if outliers else "ANCHOR_GUIDE_SANITY_NO_OUTLIERS",
        "medians": {key: round(value, 2) for key, value in medians.items()},
        "outliers": outliers,
        "usage": "Review outliers before using anchors for garment phom, lineart, masks, or batch stack boards.",
    }


def median_angle_degrees(values: list[float]) -> float:
    if not values:
        return 0.0
    return float(statistics.median(values))


def direct_comparison(measured: dict, sanity: dict) -> dict:
    shoulder_ok_angles = [
        entry["measurements"]["shoulderLineAngleDegrees"]
        for pose, entry in measured.items()
        if not any(
            item["pose"] == pose and item["metric"] == "shoulderWidthPx"
            for item in sanity["outliers"]
        )
    ]
    target_shoulder_angle = median_angle_degrees(shoulder_ok_angles)
    fix_targets = []
    for item in sanity["outliers"]:
        pose_name = item["pose"]
        metric = item["metric"]
        target = {
            "pose": pose_name,
            "metric": metric,
            "currentMetricPx": item["value"],
            "targetMetricPx": item["median"],
            "reason": item["reason"],
        }
        if metric == "shoulderWidthPx":
            entry = measured[pose_name]
            # The slot guide shoulderLine is expanded from the original center, so use
            # stored original landmarks when available from measured internals below.
            original = entry.get("landmarkSnapshot", {})
            near_shoulder = tuple(original.get("near_shoulder", entry["anchors"]["shoulderMid"]["xy"]))
            far_shoulder = tuple(original.get("far_shoulder", entry["anchors"]["shoulderMid"]["xy"]))
            radians = math.radians(target_shoulder_angle)
            target_far = (
                near_shoulder[0] + item["median"] * math.cos(radians),
                near_shoulder[1] + item["median"] * math.sin(radians),
            )
            target.update(
                {
                    "currentLandmarks": {
                        "near_shoulder": round_point(near_shoulder),
                        "far_shoulder": round_point(far_shoulder),
                    },
                    "targetShoulderAngleDegrees": round(target_shoulder_angle, 2),
                    "targetFarShoulderPreserveNear": round_point(target_far),
                    "suspectedCause": "collapsed_or_swapped_shoulder_landmarks",
                    "recommendedNextStep": "review_or_move_far_shoulder_to_target_before_garment_fit",
                }
            )
        else:
            target["recommendedNextStep"] = "review_landmarks_before_garment_fit"
        fix_targets.append(target)
    return {
        "status": "DIRECT_FIX_TARGETS_REQUIRED" if fix_targets else "DIRECT_COMPARISON_NO_FIX_TARGETS",
        "basis": "pose metrics compared against median measured in the same 1024x1536 source-space guide",
        "fixTargets": fix_targets,
    }


def draw_point(draw: ImageDraw.ImageDraw, xy: list[float], color: tuple[int, int, int], radius: int = 10) -> None:
    x, y = xy
    draw.ellipse((x - radius, y - radius, x + radius, y + radius), fill=color, outline=(0, 0, 0), width=2)


def draw_line(draw: ImageDraw.ImageDraw, points: list[list[float]], color: tuple[int, int, int], width: int = 5) -> None:
    draw.line([(x, y) for x, y in points], fill=color, width=width)


def pose_fix_targets(measurements: dict, pose: str) -> list[dict]:
    return [item for item in measurements["directComparison"]["fixTargets"] if item["pose"] == pose]


def render_pose_overlay(pose_name: str, pose: dict, fixes: list[dict]) -> Image.Image:
    source = pose.get("source")
    if source and Path(source).exists():
        image = Image.open(source).convert("RGBA")
    else:
        image = Image.new("RGBA", (1024, 1536), (246, 246, 246, 255))
    overlay = Image.new("RGBA", image.size, (255, 255, 255, 0))
    draw = ImageDraw.Draw(overlay)

    draw_line(draw, pose["slotGuides"]["outer_top"]["torsoAxis"], (0, 128, 255), 5)
    draw_line(draw, pose["slotGuides"]["outer_top"]["shoulderLine"], (0, 180, 0), 5)
    draw_line(draw, pose["slotGuides"]["waist_belt"]["line"], (255, 160, 0), 5)
    for key in ["neck", "shoulderMid", "hipMid", "chestCenter", "waistCenter"]:
        draw_point(draw, pose["anchors"][key]["xy"], (0, 128, 255), 7)

    for fix in fixes:
        if fix["metric"] == "shoulderWidthPx":
            current = fix["currentLandmarks"]
            draw_point(draw, current["near_shoulder"], (255, 0, 0), 12)
            draw_point(draw, current["far_shoulder"], (255, 0, 0), 12)
            draw_line(draw, [current["near_shoulder"], current["far_shoulder"]], (255, 0, 0), 7)
            target = fix["targetFarShoulderPreserveNear"]
            draw_line(draw, [current["near_shoulder"], target], (255, 0, 255), 7)
            draw_point(draw, target, (255, 0, 255), 12)
    image = Image.alpha_composite(image, overlay)
    draw = ImageDraw.Draw(image)
    summary = pose["measurements"]
    label = (
        f"{pose_name}  shoulder={summary['shoulderWidthPx']}px  "
        f"torso={summary['torsoLengthPx']}px"
    )
    draw.rectangle((12, 12, 760, 58), fill=(255, 255, 255, 220))
    draw.text((24, 24), label, fill=(0, 0, 0), font=ImageFont.load_default())
    return image


def render_overlay(measurements: dict, output: Path | str) -> Path:
    output = Path(output)
    pose_items = list(measurements["poses"].items())
    if not pose_items:
        raise ValueError("No poses to render")
    cell_w, cell_h = measurements.get("sourceCanvas") or [1024, 1536]
    columns = min(3, len(pose_items))
    rows = math.ceil(len(pose_items) / columns)
    board = Image.new("RGBA", (cell_w * columns, cell_h * rows), (224, 224, 224, 255))
    for index, (pose_name, pose) in enumerate(pose_items):
        x = (index % columns) * cell_w
        y = (index // columns) * cell_h
        rendered = render_pose_overlay(pose_name, pose, pose_fix_targets(measurements, pose_name))
        board.alpha_composite(rendered, (x, y))
    output.parent.mkdir(parents=True, exist_ok=True)
    board.save(output)
    return output


def measure_guide(guide_path: Path | str) -> dict:
    guide_path = Path(guide_path)
    guide = json.loads(guide_path.read_text())
    if guide.get("sourceCanvas") != [1024, 1536]:
        raise ValueError("Unsupported guide canvas")
    result = {
        "status": "GARMENT_ANCHOR_MEASUREMENTS_REVIEW_REQUIRED",
        "runtimeEligible": False,
        "guide": str(guide_path),
        "guideStatus": guide.get("status"),
        "sourceSpaceProfile": guide.get("sourceSpaceProfile"),
        "sourceCanvas": guide.get("sourceCanvas"),
        "usage": (
            "Measurement aid only. Use these anchors to draw/review phom; "
            "do not use as automatic warp, per-item runtime offset, or visual approval."
        ),
        "poses": {},
    }
    for pose in guide.get("poses", []):
        result["poses"][pose["pose"]] = measure_pose(pose)
    result["guideSanity"] = guide_sanity(result["poses"])
    result["proportionSanity"] = proportion_sanity(result["poses"])
    result["directComparison"] = direct_comparison(result["poses"], result["guideSanity"])
    result["auditPayloadSha256"] = payload_sha256(result)
    return result


def write_measurements(guide_path: Path | str, output: Path | str) -> dict:
    output = Path(output)
    output.parent.mkdir(parents=True, exist_ok=True)
    result = measure_guide(guide_path)
    output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n")
    return result


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("guide", type=Path)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--overlay", type=Path)
    args = parser.parse_args()
    result = write_measurements(args.guide, args.output)
    if args.overlay:
        render_overlay(result, args.overlay)
    print(json.dumps({"status": result["status"], "poses": list(result["poses"])}, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
