#!/usr/bin/env python3.12
"""Audit a candidate source layer against a reusable slot envelope.

This gate is intentionally narrow: it checks source-space fit before a layer is
allowed into repair staging. It does not approve visual quality or runtime use.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import math
from pathlib import Path

from inspect_lgo_source_png_inventory import inspect_png


DEFAULT_PROFILE = {"canvasSize": [1024, 1536], "originX": 512, "groundY": 1484}


def _payload_sha256(data: dict) -> str:
    payload = dict(data)
    payload.pop("auditPayloadSha256", None)
    return hashlib.sha256(
        json.dumps(payload, ensure_ascii=False, sort_keys=True, separators=(",", ":")).encode()
    ).hexdigest()


def _distance(a: list[float], b: list[float]) -> float:
    return math.hypot(a[0] - b[0], a[1] - b[1])


def _bbox_size(bbox: list[int]) -> tuple[int, int]:
    return bbox[2] - bbox[0], bbox[3] - bbox[1]


def _bbox_center(bbox: list[int]) -> list[float]:
    return [(bbox[0] + bbox[2]) / 2, (bbox[1] + bbox[3]) / 2]


def _find_target(brief: dict, slot: str, pose: str) -> dict:
    for target in brief.get("targets") or []:
        if target.get("slot") == slot and target.get("pose") == pose:
            return target
    raise ValueError(f"missing target in brief: {slot}/{pose}")


def build_slot_envelope(target: dict, profile: dict | None = None) -> dict:
    """Build a source-space numeric envelope from the target slot guide."""
    profile = dict(profile or DEFAULT_PROFILE)
    guide = target.get("slotGuide") or {}
    line = guide.get("line")
    if not line or len(line) != 2:
        raise ValueError(f"slot target has no line guide: {target.get('slot')}/{target.get('pose')}")
    center = guide.get("center")
    if not center:
        center = [(line[0][0] + line[1][0]) / 2, (line[0][1] + line[1][1]) / 2]
    line_length = _distance(line[0], line[1])
    half_width = float(guide.get("halfWidthPx") or (line_length / 2))
    measurements = target.get("measurements") or {}
    torso_length = float(measurements.get("torsoLengthPx") or 0)

    # These limits are source-authoring guardrails, not runtime offsets. They
    # allow ornament/tail tolerance around the waist guide while rejecting raw
    # generated layers that are several body segments too large.
    max_bbox_width = round(line_length * 2.2)
    max_bbox_height = round(max(half_width * 2.4, torso_length * 0.34, 36))
    max_center_distance = round(max(half_width * 0.9, 36))

    return {
        "slot": target.get("slot"),
        "pose": target.get("pose"),
        "profile": profile,
        "guide": {
            "center": [round(float(center[0]), 2), round(float(center[1]), 2)],
            "line": line,
            "lineLengthPx": round(line_length, 2),
            "angleDegrees": guide.get("angleDegrees"),
            "halfWidthPx": round(half_width, 2),
        },
        "limits": {
            "maxBBoxWidthPx": max_bbox_width,
            "maxBBoxHeightPx": max_bbox_height,
            "maxCenterDistancePx": max_center_distance,
        },
        "runtimePromotionAllowed": False,
        "visualAccepted": False,
        "usage": "Pre-stage source-space fit gate. Passing this does not approve art quality or runtime promotion.",
    }


def audit_candidate_against_brief(
    brief: dict,
    candidate_png: Path | str,
    slot: str,
    pose: str,
    output: Path | str | None = None,
    profile: dict | None = None,
) -> dict:
    candidate_png = Path(candidate_png)
    target = _find_target(brief, slot, pose)
    envelope = build_slot_envelope(target, profile)
    info = inspect_png(candidate_png, candidate_png.parent, scan_alpha=True)
    failures: dict[str, dict] = {}

    if info.get("size") != envelope["profile"]["canvasSize"]:
        failures["canvasSize"] = {
            "actual": info.get("size"),
            "limit": envelope["profile"]["canvasSize"],
        }
    if not info.get("hasAlpha"):
        failures["hasAlpha"] = {"actual": False, "required": True}
    if not info.get("nonzeroAlpha"):
        failures["nonzeroAlpha"] = {"actual": info.get("nonzeroAlpha"), "required": ">0"}

    bbox = info.get("alphaBBox")
    candidate_record = {
        "path": str(candidate_png),
        "size": info.get("size"),
        "hasAlpha": info.get("hasAlpha"),
        "nonzeroAlpha": info.get("nonzeroAlpha"),
        "alphaBBox": bbox,
        "sha256": info.get("sha256"),
    }
    if bbox:
        width, height = _bbox_size(bbox)
        center = _bbox_center(bbox)
        center_distance = _distance(center, envelope["guide"]["center"])
        candidate_record.update(
            {
                "bboxWidthPx": width,
                "bboxHeightPx": height,
                "bboxCenter": [round(center[0], 2), round(center[1], 2)],
                "centerDistancePx": round(center_distance, 2),
            }
        )
        limits = envelope["limits"]
        if width > limits["maxBBoxWidthPx"]:
            failures["bboxWidth"] = {"actual": width, "limit": limits["maxBBoxWidthPx"]}
        if height > limits["maxBBoxHeightPx"]:
            failures["bboxHeight"] = {"actual": height, "limit": limits["maxBBoxHeightPx"]}
        if center_distance > limits["maxCenterDistancePx"]:
            failures["centerDistance"] = {
                "actual": round(center_distance, 2),
                "limit": limits["maxCenterDistancePx"],
            }

    source_candidate_allowed = not failures
    result = {
        "status": (
            "SLOT_ENVELOPE_FIT_PASS_REVIEW_REQUIRED"
            if source_candidate_allowed
            else "SLOT_ENVELOPE_FIT_FAIL"
        ),
        "slot": slot,
        "pose": pose,
        "runtimePromotionAllowed": False,
        "visualAccepted": False,
        "sourceCandidateAllowed": source_candidate_allowed,
        "envelope": envelope,
        "candidate": candidate_record,
        "failures": failures,
        "notes": [
            "This gate only checks numeric source-space fit.",
            "Visual art quality, occlusion and Player runtime review remain separate gates.",
        ],
    }
    result["auditPayloadSha256"] = _payload_sha256(result)
    if output:
        output = Path(output)
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return result


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("brief", type=Path)
    parser.add_argument("candidate_png", type=Path)
    parser.add_argument("--slot", required=True)
    parser.add_argument("--pose", required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()

    brief = json.loads(args.brief.read_text(encoding="utf-8"))
    result = audit_candidate_against_brief(
        brief,
        args.candidate_png,
        args.slot,
        args.pose,
        output=args.output,
    )
    print(json.dumps(result, ensure_ascii=False, indent=2))
    return 0 if result["sourceCandidateAllowed"] else 2


if __name__ == "__main__":
    raise SystemExit(main())
