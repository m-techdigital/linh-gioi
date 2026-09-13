#!/usr/bin/env python3.12
"""Compose a review-only pose clothing stack board from registered layers."""
from __future__ import annotations

import argparse
import hashlib
import json
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

CANVAS = (1024, 1536)
FIX_STATES = {"missing", "rejected"}


def sha256(path: Path | str) -> str:
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def load_layer_image(path: Path | str) -> Image.Image:
    image = Image.open(path).convert("RGBA")
    if image.size != CANVAS:
        raise ValueError(f"Layer must use 1024x1536 canvas: {path}")
    return image


def layer_record(layer: dict) -> dict:
    record = {
        "slot": layer["slot"],
        "state": layer.get("state", "draft"),
        "path": layer.get("path"),
        "note": layer.get("note", ""),
    }
    if layer.get("path"):
        record["sha256"] = sha256(layer["path"])
    return record


def compose_pose(pose: str, layers: list[dict], header_height: int | None = None) -> tuple[Image.Image, list[dict]]:
    image = Image.new("RGBA", CANVAS, (232, 232, 232, 255))
    records = []
    for layer in layers:
        records.append(layer_record(layer))
        if not layer.get("path"):
            continue
        image.alpha_composite(load_layer_image(layer["path"]))
    header_height = header_height or (76 + 28 * sum(layer.get("state", "draft") in FIX_STATES or bool(layer.get("note")) for layer in layers))
    panel = Image.new("RGBA", (CANVAS[0], CANVAS[1] + header_height), (232, 232, 232, 255))
    panel.alpha_composite(image, (0, header_height))
    image = panel
    draw = ImageDraw.Draw(image)
    counts = {}
    for layer in layers:
        state = layer.get("state", "draft")
        counts[state] = counts.get(state, 0) + 1
    title = f"{pose}  " + " ".join(f"{key}:{value}" for key, value in sorted(counts.items()))
    draw.rectangle((12, 12, 920, 66), fill=(255, 255, 255, 224))
    draw.text((24, 28), title, fill=(0, 0, 0), font=ImageFont.load_default())
    y = 76
    for layer in layers:
        state = layer.get("state", "draft")
        if state in FIX_STATES or layer.get("note"):
            text = f"{layer['slot']}: {state} {layer.get('note', '')}".strip()
            draw.rectangle((12, y - 4, 1012, y + 22), fill=(255, 244, 210, 224))
            draw.text((24, y), text, fill=(150, 0, 0) if state in FIX_STATES else (0, 0, 0), font=ImageFont.load_default())
            y += 28
    return image, records


def summarize(records_by_pose: dict[str, list[dict]]) -> dict:
    summary = {"totalLayers": 0, "missing": 0, "rejected": 0, "draft": 0, "authority": 0, "accepted": 0}
    for records in records_by_pose.values():
        for record in records:
            summary["totalLayers"] += 1
            state = record["state"]
            summary[state] = summary.get(state, 0) + 1
    return summary


def compose_stack_board(manifest: dict, output: Path | str, report: Path | str) -> dict:
    poses = manifest["poses"]
    if not poses:
        raise ValueError("At least one pose is required")
    header_height = 76 + 28 * max(sum(layer.get("state", "draft") in FIX_STATES or bool(layer.get("note")) for layer in manifest["layers"].get(pose, [])) for pose in poses)
    panel_height = CANVAS[1] + header_height
    columns = min(2, len(poses))
    rows = math.ceil(len(poses) / columns)
    board = Image.new("RGBA", (CANVAS[0] * columns, panel_height * rows), (212, 212, 212, 255))
    records_by_pose = {}
    for index, pose in enumerate(poses):
        pose_image, records = compose_pose(pose, manifest["layers"].get(pose, []), header_height)
        records_by_pose[pose] = records
        board.alpha_composite(pose_image, ((index % columns) * CANVAS[0], (index // columns) * panel_height))
    output = Path(output)
    report = Path(report)
    output.parent.mkdir(parents=True, exist_ok=True)
    report.parent.mkdir(parents=True, exist_ok=True)
    board.save(output)
    summary = summarize(records_by_pose)
    status = "STACK_REVIEW_FIX_REQUIRED" if summary.get("missing", 0) or summary.get("rejected", 0) else "STACK_REVIEW_REQUIRED"
    result = {
        "status": status,
        "runtimeEligible": False,
        "output": str(output),
        "sourceCanvas": list(CANVAS),
        "sourcePixelsPerBoardPixel": 1,
        "panelSourceOffset": [0, header_height],
        "summary": summary,
        "poses": records_by_pose,
        "usage": "Review-only stack board. It is not source acceptance, pack evidence, or Player runtime proof.",
    }
    report.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return result


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--manifest", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    args = parser.parse_args()
    manifest = json.loads(args.manifest.read_text(encoding="utf-8"))
    result = compose_stack_board(manifest, args.output, args.report)
    print(json.dumps({"status": result["status"], "summary": result["summary"]}, ensure_ascii=False))
    return 2 if result["status"] == "STACK_REVIEW_FIX_REQUIRED" else 0


if __name__ == "__main__":
    raise SystemExit(main())
