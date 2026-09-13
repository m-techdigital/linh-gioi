#!/usr/bin/env python3
"""Compare a draft LGO pose guide with macOS Vision body-pose proposals."""

from __future__ import annotations

import argparse
import hashlib
import json
import math
import statistics
import subprocess
from pathlib import Path


JOINTS = ("shoulder", "elbow", "wrist", "hip", "knee", "ankle")


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def distance(a: list[float], b: dict[str, float]) -> float:
    return math.hypot(a[0] - b["x"], a[1] - b["y"])


def compare_pose(pose: dict, vision: dict, minimum_confidence: float = 0.2) -> dict:
    manual = {item["id"]: item for item in pose["landmarks"]}
    detected = vision["joints"]
    orientations = []
    for near_side, far_side in (("right", "left"), ("left", "right")):
        score = 0.0
        compared = 0
        for joint in JOINTS:
            for role, side in (("near", near_side), ("far", far_side)):
                source = manual.get(f"{role}_{joint}")
                candidate = detected.get(f"{side}_{joint}")
                if source and candidate and candidate["confidence"] >= minimum_confidence:
                    score += distance(source["xy"], candidate)
                    compared += 1
        orientations.append((score / max(1, compared), near_side, far_side, compared))
    mean_delta, near_side, far_side, _count = min(orientations)

    records = []
    for joint in JOINTS:
        for role, side in (("near", near_side), ("far", far_side)):
            source = manual.get(f"{role}_{joint}")
            candidate = detected.get(f"{side}_{joint}")
            if not source or not candidate:
                continue
            delta = distance(source["xy"], candidate)
            radius = float(source.get("reviewRadiusPx", 0))
            usable = candidate["confidence"] >= minimum_confidence
            records.append({
                "landmark": f"{role}_{joint}", "visionJoint": f"{side}_{joint}",
                "manualXY": source["xy"], "visionXY": [candidate["x"], candidate["y"]],
                "visionConfidence": candidate["confidence"], "deltaPixels": delta,
                "usableForComparison": usable,
                "manualReviewRadiusPixels": radius,
                "withinManualReviewRadius": usable and radius > 0 and delta <= radius,
                "withinTwoPixelBindTolerance": usable and delta <= 2.0,
            })
    if "neck" in manual and "neck" in detected:
        candidate = detected["neck"]
        delta = distance(manual["neck"]["xy"], candidate)
        radius = float(manual["neck"].get("reviewRadiusPx", 0))
        usable = candidate["confidence"] >= minimum_confidence
        records.append({
            "landmark": "neck", "visionJoint": "neck", "manualXY": manual["neck"]["xy"],
            "visionXY": [candidate["x"], candidate["y"]], "visionConfidence": candidate["confidence"],
            "usableForComparison": usable,
            "deltaPixels": delta, "manualReviewRadiusPixels": radius,
            "withinManualReviewRadius": usable and radius > 0 and delta <= radius,
            "withinTwoPixelBindTolerance": usable and delta <= 2.0,
        })
    return {
        "pose": pose["pose"], "nearVisionSide": near_side, "farVisionSide": far_side,
        "orientationMeanDeltaPixels": mean_delta, "landmarks": records,
    }


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--guide", type=Path, required=True)
    parser.add_argument("--output-dir", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    parser.add_argument("--swift-tool", type=Path, default=Path(__file__).with_name("lgo_detect_body_pose.swift"))
    args = parser.parse_args()
    if args.output_dir.exists():
        raise FileExistsError("Refusing to overwrite Vision evidence: " + str(args.output_dir))
    guide = json.loads(args.guide.read_text())
    args.output_dir.mkdir(parents=True)
    comparisons, failures = [], []
    for pose in guide.get("poses", []):
        source = Path(pose["source"])
        output = args.output_dir / f"{pose['pose']}.json"
        result = subprocess.run(
            ["swift", str(args.swift_tool), str(source)], text=True, capture_output=True)
        if result.returncode:
            failures.append({"pose": pose["pose"], "exitCode": result.returncode, "stderr": result.stderr.strip()})
            continue
        output.write_text(result.stdout)
        comparisons.append(compare_pose(pose, json.loads(result.stdout)))
    landmarks = [item for pose in comparisons for item in pose["landmarks"]]
    usable = [item for item in landmarks if item["usableForComparison"]]
    deltas = [item["deltaPixels"] for item in usable]
    low_confidence = [item for item in landmarks if not item["usableForComparison"]]
    within_review = [item for item in landmarks if item["withinManualReviewRadius"]]
    within_two = [item for item in landmarks if item["withinTwoPixelBindTolerance"]]
    report = {
        "id": "lgo_macos_vision_pose_guide_audit_v1",
        "status": "VISION_REVIEW_ASSIST_ONLY_NOT_BIND_AUTHORITY",
        "guide": str(args.guide), "guideSha256": digest(args.guide),
        "guideStatus": guide.get("status"), "detectedPoseCount": len(comparisons),
        "requestedPoseCount": len(guide.get("poses", [])), "failures": failures,
        "summary": {
            "detectedLandmarkCount": len(landmarks),
            "comparedLandmarkCount": len(usable),
            "withinTwoPixelBindToleranceCount": len(within_two),
            "withinManualReviewRadiusCount": len(within_review),
            "unusableBelowMinimumConfidenceCount": len(low_confidence),
            "minimumConfidence": 0.2,
            "medianDeltaPixels": statistics.median(deltas) if deltas else None,
            "maxDeltaPixels": max(deltas) if deltas else None,
        },
        "comparisons": comparisons,
        "runtimeAuthority": False,
        "claimLimit": "Vision proposals can flag gross guide errors. They cannot approve a two-pixel bind profile, inferred or occluded joints, anatomy, or runtime promotion.",
    }
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n")
    print(json.dumps({"status": report["status"], **report["summary"]}))
    return 0 if comparisons else 2


if __name__ == "__main__":
    raise SystemExit(main())
