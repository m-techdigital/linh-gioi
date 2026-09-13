#!/usr/bin/env python3.12
"""Create a review-only pose registration guide candidate from landmark overrides."""
from __future__ import annotations

import argparse
import copy
import hashlib
import json
from pathlib import Path


def file_sha256(path: Path | str) -> str:
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def apply_overrides(guide_path: Path | str, overrides_path: Path | str, output_path: Path | str) -> dict:
    guide_path = Path(guide_path)
    overrides_path = Path(overrides_path)
    output_path = Path(output_path)
    guide = json.loads(guide_path.read_text(encoding="utf-8"))
    overrides = json.loads(overrides_path.read_text(encoding="utf-8"))
    candidate = copy.deepcopy(guide)
    candidate["status"] = overrides.get("status", "CANDIDATE_REVIEW_ONLY")
    candidate["sourceGuide"] = str(guide_path)
    candidate["sourceGuideSha256"] = file_sha256(guide_path)
    candidate["landmarkOverrideSource"] = str(overrides_path)
    candidate["landmarkOverrideSourceSha256"] = file_sha256(overrides_path)
    candidate["statusDoesNotAuthorizeRuntime"] = True
    candidate["usage"] = (
        "Review-only guide candidate. Rerun measurement and visual overlay before asset authoring; "
        "do not treat overrides as runtime authority."
    )
    changed = []
    pose_overrides = overrides.get("overrides", {})
    for pose in candidate.get("poses", []):
        current = pose_overrides.get(pose.get("pose"), {})
        if not current:
            continue
        landmarks = {item["id"]: item for item in pose.get("landmarks", [])}
        for landmark_id, xy in current.items():
            if landmark_id not in landmarks:
                raise ValueError(f"{pose.get('pose')}: missing landmark {landmark_id}")
            before = list(landmarks[landmark_id]["xy"])
            landmarks[landmark_id]["xy"] = list(xy)
            landmarks[landmark_id]["status"] = "CANDIDATE_VISUAL_REVIEW_OVERRIDE"
            landmarks[landmark_id]["overrideReason"] = overrides.get("reason", "landmark override")
            changed.append(
                {
                    "pose": pose.get("pose"),
                    "landmark": landmark_id,
                    "before": before,
                    "after": list(xy),
                }
            )
    candidate["changedLandmarks"] = changed
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(json.dumps(candidate, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return candidate


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--guide", type=Path, required=True)
    parser.add_argument("--overrides", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = apply_overrides(args.guide, args.overrides, args.output)
    print(json.dumps({"status": result["status"], "changed": result["changedLandmarks"]}, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
