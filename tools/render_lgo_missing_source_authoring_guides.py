#!/usr/bin/env python3.12
"""Render measured guide overlays for missing six-pose source authoring."""
from __future__ import annotations

import argparse
import json
from pathlib import Path

from compose_lgo_six_pose_repair_source_board import BODY_FILENAMES, downsample_nearest, flatten_on_background, paste
from stage_lgo_six_pose_repair_layers import read_rgba_png, write_rgba_png


POSES = ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"]
DEFAULT_CANVAS_SIZE = [1024, 1536]
COLORS = {
    "outer_top": (0, 136, 255, 255),
    "waist_belt": (255, 160, 0, 255),
    "shoulder_chest_guard": (210, 40, 255, 255),
    "center": (255, 40, 40, 255),
}


def _set_pixel(pixels: bytearray, width: int, height: int, x: int, y: int, color: tuple[int, int, int, int]) -> None:
    if 0 <= x < width and 0 <= y < height:
        index = (y * width + x) * 4
        pixels[index : index + 4] = bytes(color)


def draw_point(pixels: bytearray, width: int, height: int, xy: list[float], color: tuple[int, int, int, int], radius: int) -> None:
    cx, cy = int(round(xy[0])), int(round(xy[1]))
    r2 = radius * radius
    for y in range(cy - radius, cy + radius + 1):
        for x in range(cx - radius, cx + radius + 1):
            if (x - cx) * (x - cx) + (y - cy) * (y - cy) <= r2:
                _set_pixel(pixels, width, height, x, y, color)


def draw_line(
    pixels: bytearray,
    width: int,
    height: int,
    points: list[list[float]],
    color: tuple[int, int, int, int],
    thickness: int,
) -> None:
    if len(points) != 2:
        return
    x0, y0 = points[0]
    x1, y1 = points[1]
    steps = max(abs(x1 - x0), abs(y1 - y0), 1)
    radius = max(1, thickness // 2)
    for step in range(int(steps) + 1):
        t = step / steps
        x = x0 + (x1 - x0) * t
        y = y0 + (y1 - y0) * t
        draw_point(pixels, width, height, [x, y], color, radius)


def load_body(body_root: Path, pose: str, canvas_size: list[int]) -> tuple[int, int, bytearray]:
    path = body_root / BODY_FILENAMES[pose]
    width, height, pixels = read_rgba_png(path)
    if [width, height] != canvas_size:
        raise ValueError(f"wrong body canvas {width}x{height}: {path}")
    return width, height, pixels


def render_pose_guides(
    body_root: Path,
    pose: str,
    targets: list[dict],
    canvas_size: list[int],
) -> bytearray:
    width, height, pixels = load_body(body_root, pose, canvas_size)
    for target in targets:
        slot = target["slot"]
        guide = target.get("slotGuide") or {}
        color = COLORS.get(slot, (0, 0, 0, 255))
        for key in ("shoulderLine", "waistLine", "chestLine", "line", "torsoAxis"):
            if key in guide:
                draw_line(pixels, width, height, guide[key], color, max(3, width // 170))
        if "center" in guide:
            draw_point(pixels, width, height, guide["center"], COLORS["center"], max(4, width // 128))
    return pixels


def render_authoring_guides(
    brief: dict,
    body_root: Path | str,
    output: Path | str,
    report_path: Path | str,
    canvas_size: list[int] | None = None,
    panel_scale: int = 2,
) -> dict:
    body_root = Path(body_root).resolve()
    canvas_size = canvas_size or DEFAULT_CANVAS_SIZE
    targets_by_pose: dict[str, list[dict]] = {pose: [] for pose in POSES}
    for target in brief.get("targets") or []:
        targets_by_pose.setdefault(target["pose"], []).append(target)
    panels = []
    for pose in POSES:
        panel = flatten_on_background(render_pose_guides(body_root, pose, targets_by_pose.get(pose, []), canvas_size))
        panel_w, panel_h, scaled = downsample_nearest(panel, canvas_size[0], canvas_size[1], panel_scale)
        panels.append((pose, scaled))
    columns, rows = 2, 3
    board_w, board_h = panel_w * columns, panel_h * rows
    board = bytearray([232, 232, 232, 255] * (board_w * board_h))
    for index, (_pose, panel) in enumerate(panels):
        paste(board, board_w, panel, panel_w, panel_h, (index % columns) * panel_w, (index // columns) * panel_h)
    output = Path(output)
    report_path = Path(report_path)
    output.parent.mkdir(parents=True, exist_ok=True)
    report_path.parent.mkdir(parents=True, exist_ok=True)
    write_rgba_png(output, board_w, board_h, board)
    report = {
        "status": "MISSING_SOURCE_AUTHORING_GUIDE_BOARD_READY",
        "runtimePromotionAllowed": False,
        "output": str(output),
        "bodyRoot": str(body_root),
        "sourceCanvas": canvas_size,
        "panelScale": panel_scale,
        "panelSize": [panel_w, panel_h],
        "targetCount": sum(len(items) for items in targets_by_pose.values()),
        "targetsByPose": {
            pose: [{"slot": item["slot"], "slotGuide": item.get("slotGuide")} for item in items]
            for pose, items in targets_by_pose.items()
            if items
        },
        "usage": "Authoring guide overlay only. Draw/check source layers from these guides; do not pack this board.",
    }
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return report


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--brief", type=Path, required=True)
    parser.add_argument("--body-root", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    parser.add_argument("--panel-scale", type=int, default=2)
    parser.add_argument("--canvas-size", default="1024x1536")
    args = parser.parse_args()
    canvas_size = [int(part) for part in args.canvas_size.lower().split("x", 1)]
    report = render_authoring_guides(
        load_json(args.brief),
        args.body_root,
        args.output,
        args.report,
        canvas_size,
        args.panel_scale,
    )
    print(
        json.dumps(
            {
                "status": report["status"],
                "runtimePromotionAllowed": report["runtimePromotionAllowed"],
                "targetCount": report["targetCount"],
                "output": report["output"],
            },
            ensure_ascii=False,
        )
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
