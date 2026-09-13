#!/usr/bin/env python3.12
"""Audit a full skeletal 2D body blueprint package before runtime probe."""
from __future__ import annotations

import argparse
import json
import struct
import zlib
from pathlib import Path
from typing import Any


EXPECTED_CANVAS = {"width": 1024, "height": 1536, "originX": 512, "groundY": 1484}
REQUIRED_POSES = {
    "idle",
    "run_contact_a",
    "run_a",
    "run_contact_b",
    "run_b",
    "jump_tuck",
}
REQUIRED_COMPONENTS = {
    "head_neck",
    "torso_hips",
    "near_upper_arm",
    "near_forearm_hand",
    "far_upper_arm",
    "far_forearm_hand",
    "near_thigh",
    "near_shin_foot",
    "far_thigh",
    "far_shin_foot",
}
REQUIRED_JOINTS = {
    "crown",
    "chin",
    "neck",
    "near_shoulder",
    "near_elbow",
    "near_wrist",
    "far_shoulder",
    "far_elbow",
    "far_wrist",
    "near_hip",
    "near_knee",
    "near_ankle",
    "far_hip",
    "far_knee",
    "far_ankle",
}
REQUIRED_HIDDEN_SURFACES = {
    "under_arm_torso_edge",
    "sleeve_underlap_area",
    "inside_leg_overlap",
    "lower_garment_contact_zone",
    "foot_sole_alternative",
}


class PngAuditError(ValueError):
    pass


def _read_png(path: Path) -> dict[str, Any]:
    data = path.read_bytes()
    if not data.startswith(b"\x89PNG\r\n\x1a\n"):
        raise PngAuditError("not a PNG file")
    offset = 8
    width = height = color_type = bit_depth = None
    idat = bytearray()
    while offset < len(data):
        length = struct.unpack(">I", data[offset : offset + 4])[0]
        kind = data[offset + 4 : offset + 8]
        chunk = data[offset + 8 : offset + 8 + length]
        offset += 12 + length
        if kind == b"IHDR":
            width, height, bit_depth, color_type, _, _, _ = struct.unpack(">IIBBBBB", chunk)
        elif kind == b"IDAT":
            idat.extend(chunk)
        elif kind == b"IEND":
            break
    if width is None or height is None or color_type is None or bit_depth != 8:
        raise PngAuditError("unsupported or malformed PNG header")
    if color_type not in {2, 6}:
        raise PngAuditError("unsupported PNG color type")
    has_alpha = color_type == 6
    alpha_min = alpha_max = None
    if has_alpha:
        channels = 4
        row_len = width * channels
        raw = zlib.decompress(bytes(idat))
        previous = [0] * row_len
        alpha_values = []
        pos = 0
        for _ in range(height):
            filter_type = raw[pos]
            pos += 1
            row = list(raw[pos : pos + row_len])
            pos += row_len
            recon = _unfilter(row, previous, filter_type, channels)
            alpha_values.extend(recon[3::4])
            previous = recon
        alpha_min = min(alpha_values)
        alpha_max = max(alpha_values)
    return {
        "width": width,
        "height": height,
        "hasAlpha": has_alpha,
        "alphaExtrema": [alpha_min, alpha_max] if has_alpha else None,
    }


def _unfilter(row: list[int], previous: list[int], filter_type: int, channels: int) -> list[int]:
    result = row[:]
    if filter_type == 0:
        return result
    if filter_type == 1:
        for index, value in enumerate(row):
            left = result[index - channels] if index >= channels else 0
            result[index] = (value + left) & 0xFF
        return result
    if filter_type == 2:
        for index, value in enumerate(row):
            result[index] = (value + previous[index]) & 0xFF
        return result
    if filter_type == 3:
        for index, value in enumerate(row):
            left = result[index - channels] if index >= channels else 0
            up = previous[index]
            result[index] = (value + ((left + up) // 2)) & 0xFF
        return result
    if filter_type == 4:
        for index, value in enumerate(row):
            left = result[index - channels] if index >= channels else 0
            up = previous[index]
            up_left = previous[index - channels] if index >= channels else 0
            result[index] = (value + _paeth(left, up, up_left)) & 0xFF
        return result
    raise PngAuditError(f"unsupported PNG filter {filter_type}")


def _paeth(left: int, up: int, up_left: int) -> int:
    estimate = left + up - up_left
    left_distance = abs(estimate - left)
    up_distance = abs(estimate - up)
    up_left_distance = abs(estimate - up_left)
    if left_distance <= up_distance and left_distance <= up_left_distance:
        return left
    if up_distance <= up_left_distance:
        return up
    return up_left


def _audit_png(path: Path, label: str, failures: list[str], prefix: str) -> dict[str, Any]:
    report = {"label": label, "path": str(path)}
    if not path.is_file():
        failures.append(f"{prefix}_MISSING")
        return report
    try:
        png = _read_png(path)
    except (OSError, PngAuditError, zlib.error, struct.error) as exc:
        failures.append(f"{prefix}_PNG_INVALID")
        report["error"] = str(exc)
        return report
    report.update(png)
    if (png["width"], png["height"]) != (EXPECTED_CANVAS["width"], EXPECTED_CANVAS["height"]):
        failures.append(f"{prefix}_CANVAS_DIMENSIONS_MISMATCH")
    if not png["hasAlpha"]:
        failures.append(f"{prefix}_MISSING_ALPHA_CHANNEL")
    elif png["alphaExtrema"][0] == 255:
        failures.append(f"{prefix}_NO_TRANSPARENT_PIXELS")
    elif png["alphaExtrema"][1] == 0:
        failures.append(f"{prefix}_NO_VISIBLE_PIXELS")
    return report


def audit_blueprint_package(manifest_path: Path | str) -> dict[str, Any]:
    manifest_path = Path(manifest_path).resolve()
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    base_dir = manifest_path.parent
    failures: list[str] = []

    for key, expected in EXPECTED_CANVAS.items():
        if manifest.get("canvas", {}).get(key) != expected:
            failures.append(f"CANVAS_{key.upper()}_MISMATCH")

    native = base_dir / manifest.get("nativeSource", "")
    if native.suffix.lower() not in {".kra", ".psb", ".psd", ".ora"} or not native.is_file():
        failures.append("NATIVE_SOURCE_MISSING_OR_UNSUPPORTED")

    composite_report = _audit_png(
        base_dir / manifest.get("composite", ""),
        "composite",
        failures,
        "COMPOSITE",
    )

    if manifest.get("neutralBody", {}).get("bakedOptionalOutfit") is not False:
        failures.append("NEUTRAL_BODY_HAS_BAKED_OPTIONAL_OUTFIT")

    pose_reports = []
    targets = {item.get("pose"): item for item in manifest.get("motionTargets", [])}
    for pose in sorted(REQUIRED_POSES):
        if pose not in targets:
            failures.append(f"MISSING_MOTION_TARGET_{pose.upper()}")
            continue
        pose_reports.append(
            _audit_png(base_dir / targets[pose].get("path", ""), pose, failures, f"MOTION_TARGET_{pose.upper()}")
        )

    joints = manifest.get("jointCenters", {})
    for joint in sorted(REQUIRED_JOINTS):
        value = joints.get(joint)
        if not isinstance(value, dict) or not isinstance(value.get("xy"), list) or len(value["xy"]) != 2:
            failures.append(f"MISSING_JOINT_{joint.upper()}")

    component_ids = {item.get("id") for item in manifest.get("bodyComponents", [])}
    for component in sorted(REQUIRED_COMPONENTS):
        if component not in component_ids:
            failures.append(f"MISSING_BODY_COMPONENT_{component.upper()}")

    hidden = set(manifest.get("hiddenSurfaces", []))
    for surface in sorted(REQUIRED_HIDDEN_SURFACES):
        if surface not in hidden:
            failures.append(f"MISSING_HIDDEN_SURFACE_{surface.upper()}")

    if not manifest.get("lowerLegFootPolicy"):
        failures.append("MISSING_LOWER_LEG_FOOT_POLICY")
    if not manifest.get("drawOrder"):
        failures.append("MISSING_DRAW_ORDER")
    if not manifest.get("ownershipTable"):
        failures.append("MISSING_OWNERSHIP_TABLE")
    if manifest.get("visualReview", {}).get("status") != "APPROVED":
        failures.append("VISUAL_REVIEW_NOT_APPROVED")

    return {
        "gateId": "LGO_SKELETAL_2D_SOURCE_BLUEPRINT_01",
        "status": "PASS" if not failures else "REJECT_BLUEPRINT_PACKAGE",
        "manifest": str(manifest_path),
        "expectedCanvas": EXPECTED_CANVAS,
        "failures": failures,
        "composite": composite_report,
        "motionTargets": pose_reports,
        "runtimePromotionAllowed": False,
    }


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("manifest", type=Path)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    report = audit_blueprint_package(args.manifest)
    text = json.dumps(report, ensure_ascii=False, indent=2) + "\n"
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(text, encoding="utf-8")
    print(text, end="")
    raise SystemExit(0 if report["status"] == "PASS" else 2)


if __name__ == "__main__":
    main()
