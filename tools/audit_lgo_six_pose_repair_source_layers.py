#!/usr/bin/env python3.12
"""Gate six-pose repair layer exports before any source board or runtime pack."""
from __future__ import annotations

import argparse
import json
from pathlib import Path

from inspect_lgo_source_png_inventory import inspect_png


DEFAULT_CANVAS_SIZE = [1024, 1536]


def _required_paths(repair: dict) -> list[tuple[str, str, str, str, Path]]:
    root = Path(repair["candidateDirectory"])
    paths = []
    for variant in repair.get("requiredVariants") or ["A", "B"]:
        for pose in repair.get("requiredPoses") or []:
            for component in repair.get("requiredComponents") or ["front", "back"]:
                paths.append((variant, pose, component, f"{variant}/{pose}/{component}.png", root / variant / pose / f"{component}.png"))
    return paths


def _audit_file(
    source_root: Path,
    slot: str,
    variant: str,
    pose: str,
    component: str,
    path: Path,
    canvas_size: list[int],
) -> tuple[dict | None, list[str]]:
    failures = []
    if not path.exists():
        return None, [f"{slot}/{variant}/{pose}/{component}.png missing"]
    try:
        record = inspect_png(path, source_root, scan_alpha=True)
    except Exception as exc:
        return None, [f"{slot}/{variant}/{pose}/{component}.png unreadable PNG: {exc}"]
    if record["size"] != canvas_size:
        failures.append(f"{slot}/{variant}/{pose}/{component}.png wrong canvas {record['size']}")
    if not record["hasAlpha"]:
        failures.append(f"{slot}/{variant}/{pose}/{component}.png missing alpha")
    total_pixels = (record["size"][0] or 0) * (record["size"][1] or 0)
    if record.get("nonzeroAlpha") == total_pixels and total_pixels:
        failures.append(f"{slot}/{variant}/{pose}/{component}.png full-canvas alpha composite")
    if component == "front" and record.get("nonzeroAlpha") == 0:
        failures.append(f"{slot}/{variant}/{pose}/{component}.png empty front layer")
    return record, failures


def audit_repair_layers(
    source_root: Path | str,
    repair_plan: dict,
    canvas_size: list[int] | None = None,
) -> dict:
    source_root = Path(source_root).resolve()
    canvas_size = canvas_size or DEFAULT_CANVAS_SIZE
    report = {
        "status": "SOURCE_REPAIR_LAYER_READY_FOR_VISUAL_BOARD",
        "runtimePromotionAllowed": False,
        "sourceRoot": str(source_root),
        "canvasSize": canvas_size,
        "slots": {},
        "failures": [],
    }
    for repair in repair_plan.get("slotRepairs") or []:
        slot = repair["slot"]
        records = []
        missing = []
        present = 0
        for variant, pose, component, _relative, relative_path in _required_paths(repair):
            record, failures = _audit_file(
                source_root,
                slot,
                variant,
                pose,
                component,
                source_root / relative_path,
                canvas_size,
            )
            if record:
                present += 1
                records.append(record)
            if failures:
                missing.extend(failures)
        required_count = len(_required_paths(repair))
        report["slots"][slot] = {
            "candidateDirectory": repair["candidateDirectory"],
            "present": present,
            "requiredCount": required_count,
            "files": records,
            "failures": missing,
        }
        report["failures"].extend(missing)
    if report["failures"]:
        report["status"] = "SOURCE_REPAIR_LAYER_GAP"
    return report


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def write_audit(
    source_root: Path | str,
    repair_plan_path: Path | str,
    output: Path | str,
    canvas_size: list[int] | None = None,
) -> dict:
    report = audit_repair_layers(source_root, load_json(Path(repair_plan_path)), canvas_size)
    output = Path(output)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return report


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("source_root", type=Path)
    parser.add_argument("--repair-plan", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--canvas-size", default="1024x1536")
    args = parser.parse_args()
    canvas_size = [int(part) for part in args.canvas_size.lower().split("x", 1)]
    report = write_audit(args.source_root, args.repair_plan, args.output, canvas_size)
    print(
        json.dumps(
            {
                "status": report["status"],
                "runtimePromotionAllowed": report["runtimePromotionAllowed"],
                "failureCount": len(report["failures"]),
            },
            ensure_ascii=False,
        )
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
