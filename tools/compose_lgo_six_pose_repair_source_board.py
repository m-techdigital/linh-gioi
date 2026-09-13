#!/usr/bin/env python3.12
"""Compose a source-space board for staged six-pose repair layers."""
from __future__ import annotations

import argparse
import json
from pathlib import Path

from stage_lgo_six_pose_repair_layers import read_rgba_png, write_rgba_png


POSES = ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"]
DEFAULT_CANVAS_SIZE = [1024, 1536]
BODY_FILENAMES = {
    "idle": "idle-base-unchanged.png",
    "run_contact_a": "run_contact_a-review.png",
    "run_a": "run_a-review.png",
    "run_contact_b": "run_contact_b-review.png",
    "run_b": "run_b-review.png",
    "jump_tuck": "jump_tuck-review.png",
}


def alpha_composite(base: bytearray, overlay: bytearray) -> bytearray:
    result = bytearray(base)
    for index in range(0, len(base), 4):
        oa = overlay[index + 3] / 255.0
        if oa <= 0:
            continue
        ba = result[index + 3] / 255.0
        out_a = oa + ba * (1 - oa)
        if out_a <= 0:
            result[index : index + 4] = b"\x00\x00\x00\x00"
            continue
        for channel in range(3):
            ov = overlay[index + channel] / 255.0
            bv = result[index + channel] / 255.0
            result[index + channel] = round(((ov * oa) + (bv * ba * (1 - oa))) / out_a * 255)
        result[index + 3] = round(out_a * 255)
    return result


def flatten_on_background(pixels: bytearray, background=(232, 232, 232, 255)) -> bytearray:
    bg = bytearray(background * (len(pixels) // 4))
    return alpha_composite(bg, pixels)


def downsample_nearest(pixels: bytearray, width: int, height: int, scale: int) -> tuple[int, int, bytearray]:
    if scale <= 1:
        return width, height, pixels
    out_w = width // scale
    out_h = height // scale
    result = bytearray(out_w * out_h * 4)
    for y in range(out_h):
        for x in range(out_w):
            src = ((y * scale) * width + (x * scale)) * 4
            dst = (y * out_w + x) * 4
            result[dst : dst + 4] = pixels[src : src + 4]
    return out_w, out_h, result


def paste(target: bytearray, target_width: int, source: bytearray, source_width: int, source_height: int, x0: int, y0: int) -> None:
    for y in range(source_height):
        src_start = y * source_width * 4
        dst_start = ((y0 + y) * target_width + x0) * 4
        target[dst_start : dst_start + source_width * 4] = source[src_start : src_start + source_width * 4]


def load_canvas(path: Path, canvas_size: list[int]) -> bytearray:
    width, height, pixels = read_rgba_png(path)
    if [width, height] != canvas_size:
        raise ValueError(f"wrong canvas {width}x{height}: {path}")
    return pixels


def _repair_slot_dirs(repair_plan: dict) -> list[tuple[str, str]]:
    return [(repair["slot"], repair["candidateDirectory"]) for repair in repair_plan.get("slotRepairs") or []]


def compose_pose(
    source_root: Path,
    body_root: Path,
    repair_plan: dict,
    pose: str,
    variant: str,
    canvas_size: list[int],
) -> tuple[bytearray, list[dict], list[str]]:
    body_path = body_root / BODY_FILENAMES[pose]
    image = load_canvas(body_path, canvas_size)
    records = [{"slot": "body", "pose": pose, "path": str(body_path), "state": "authority"}]
    missing = []
    for slot, directory in _repair_slot_dirs(repair_plan):
        for component in ("back", "front"):
            path = source_root / directory / variant / pose / f"{component}.png"
            if not path.exists():
                missing.append(f"{slot}/{pose}/{component} missing")
                records.append({"slot": slot, "pose": pose, "component": component, "state": "missing"})
                continue
            layer = load_canvas(path, canvas_size)
            image = alpha_composite(image, layer)
            records.append(
                {
                    "slot": slot,
                    "pose": pose,
                    "component": component,
                    "path": str(path),
                    "state": "draft",
                }
            )
    return flatten_on_background(image), records, missing


def compose_repair_board(
    source_root: Path | str,
    body_root: Path | str,
    repair_plan: dict,
    output: Path | str,
    report_path: Path | str,
    variant: str = "A",
    canvas_size: list[int] | None = None,
    panel_scale: int = 2,
) -> dict:
    source_root = Path(source_root).resolve()
    body_root = Path(body_root).resolve()
    canvas_size = canvas_size or DEFAULT_CANVAS_SIZE
    panels = []
    records_by_pose = {}
    missing = []
    for pose in POSES:
        panel, records, pose_missing = compose_pose(source_root, body_root, repair_plan, pose, variant, canvas_size)
        panel_w, panel_h, scaled = downsample_nearest(panel, canvas_size[0], canvas_size[1], panel_scale)
        panels.append((pose, scaled))
        records_by_pose[pose] = records
        missing.extend(pose_missing)
    columns = 2
    rows = 3
    board_w = panel_w * columns
    board_h = panel_h * rows
    board = bytearray([212, 212, 212, 255] * (board_w * board_h))
    for index, (_pose, panel) in enumerate(panels):
        paste(board, board_w, panel, panel_w, panel_h, (index % columns) * panel_w, (index // columns) * panel_h)
    output = Path(output)
    report_path = Path(report_path)
    output.parent.mkdir(parents=True, exist_ok=True)
    report_path.parent.mkdir(parents=True, exist_ok=True)
    write_rgba_png(output, board_w, board_h, board)
    report = {
        "status": "SOURCE_REPAIR_BOARD_FIX_REQUIRED" if missing else "SOURCE_REPAIR_BOARD_REVIEW_REQUIRED",
        "runtimePromotionAllowed": False,
        "sourceRoot": str(source_root),
        "bodyRoot": str(body_root),
        "output": str(output),
        "variant": variant,
        "sourceCanvas": canvas_size,
        "panelScale": panel_scale,
        "panelSize": [panel_w, panel_h],
        "missingLayers": missing,
        "poses": records_by_pose,
        "usage": "Source-space review board only; not visual acceptance and not Player/runtime proof.",
    }
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return report


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("source_root", type=Path)
    parser.add_argument("--body-root", type=Path, required=True)
    parser.add_argument("--repair-plan", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    parser.add_argument("--variant", choices=["A", "B"], default="A")
    parser.add_argument("--panel-scale", type=int, default=2)
    parser.add_argument("--canvas-size", default="1024x1536")
    args = parser.parse_args()
    canvas_size = [int(part) for part in args.canvas_size.lower().split("x", 1)]
    report = compose_repair_board(
        args.source_root,
        args.body_root,
        load_json(args.repair_plan),
        args.output,
        args.report,
        args.variant,
        canvas_size,
        args.panel_scale,
    )
    print(
        json.dumps(
            {
                "status": report["status"],
                "runtimePromotionAllowed": report["runtimePromotionAllowed"],
                "output": report["output"],
                "missingLayerCount": len(report["missingLayers"]),
            },
            ensure_ascii=False,
        )
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
