#!/usr/bin/env python3.12
"""Prepare a non-packable workspace for missing six-pose source authoring."""
from __future__ import annotations

import argparse
import hashlib
import json
import shutil
from datetime import datetime, timezone
from pathlib import Path

from compose_lgo_six_pose_repair_source_board import BODY_FILENAMES, flatten_on_background
from render_lgo_missing_source_authoring_guides import render_pose_guides
from stage_lgo_six_pose_repair_layers import transparent_pixels, write_rgba_png


DEFAULT_CANVAS_SIZE = [1024, 1536]


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def safe_name(value: str) -> str:
    return value.replace("/", "_").replace(" ", "_")


def slot_repair_index(repair_plan: dict) -> dict[str, dict]:
    return {repair["slot"]: repair for repair in repair_plan.get("slotRepairs") or []}


def select_source_reference(slot: str, pose: str, source_mapping: dict) -> str | None:
    slot_mapping = (source_mapping.get("slots") or {}).get(slot) or {}
    if pose in slot_mapping:
        return slot_mapping[pose]
    if "idle" in slot_mapping:
        return slot_mapping["idle"]
    if slot_mapping:
        return next(iter(slot_mapping.values()))
    return None


def copy_reference(source_root: Path, relative: str | None, target_dir: Path) -> dict | None:
    if not relative:
        return None
    source = source_root / relative
    if not source.exists():
        return {"relativePath": relative, "exists": False}
    dest = target_dir / "source-reference.png"
    shutil.copy2(source, dest)
    return {
        "relativePath": relative,
        "exists": True,
        "copiedTo": str(dest),
        "sha256": sha256(source),
    }


def write_blank_templates(target_dir: Path, variants: list[str], components: list[str], canvas_size: list[int]) -> list[dict]:
    width, height = canvas_size
    pixels = transparent_pixels(width, height)
    templates = []
    for variant in variants:
        for component in components:
            path = target_dir / "blank-layer-templates" / f"{variant}-{component}-transparent-template.png"
            write_rgba_png(path, width, height, pixels)
            templates.append(
                {
                    "variant": variant,
                    "component": component,
                    "path": str(path),
                    "canvas": canvas_size,
                    "usage": "Copy into a native editor layer, draw the source art, then export to the destination path listed in this manifest.",
                }
            )
    return templates


def destination_exports(source_root: Path, repair: dict, pose: str, variants: list[str], components: list[str]) -> list[dict]:
    candidate = source_root / repair["candidateDirectory"]
    return [
        {
            "variant": variant,
            "component": component,
            "path": str(candidate / variant / pose / f"{component}.png"),
        }
        for variant in variants
        for component in components
    ]


def prepare_target(
    target: dict,
    repair: dict,
    source_mapping: dict,
    body_root: Path,
    source_root: Path,
    workspace_root: Path,
    canvas_size: list[int],
) -> dict:
    slot = target["slot"]
    pose = target["pose"]
    target_dir = workspace_root / "targets" / safe_name(slot) / safe_name(pose)
    target_dir.mkdir(parents=True, exist_ok=True)

    body_source = body_root / BODY_FILENAMES[pose]
    body_reference = target_dir / "body-reference.png"
    shutil.copy2(body_source, body_reference)

    guide_overlay = target_dir / "guide-overlay-on-body.png"
    guide_pixels = flatten_on_background(render_pose_guides(body_root, pose, [target], canvas_size))
    write_rgba_png(guide_overlay, canvas_size[0], canvas_size[1], guide_pixels)

    variants = target.get("requiredVariants") or repair.get("requiredVariants") or ["A", "B"]
    components = target.get("requiredComponents") or repair.get("requiredComponents") or ["front", "back"]
    source_reference = copy_reference(
        source_root,
        select_source_reference(slot, pose, source_mapping),
        target_dir,
    )
    requires_jump_body = pose == "jump_tuck"
    guardrails = [
        "DO_NOT_PACK_WORKSPACE_FILES",
        "DO_NOT_USE_RUNTIME_OFFSET_OR_POSE_SCALE",
        "EXPORT_ONLY_CLEAN_RGBA_1024x1536_SOURCE_LAYERS",
    ]
    if requires_jump_body:
        guardrails.append("DO_NOT_SYNTHESIZE_JUMP_FROM_IDLE_OR_RUN")

    return {
        "slot": slot,
        "pose": pose,
        "targetDir": str(target_dir),
        "bodyReference": str(body_reference),
        "guideOverlay": str(guide_overlay),
        "sourceReference": source_reference,
        "slotGuide": target.get("slotGuide"),
        "measurements": target.get("measurements"),
        "requiresBodyAnatomyAcceptance": requires_jump_body,
        "templates": write_blank_templates(target_dir, variants, components, canvas_size),
        "destinationExports": destination_exports(source_root, repair, pose, variants, components),
        "guardrails": guardrails,
    }


def prepare_authoring_workspace(
    brief: dict,
    repair_plan: dict,
    source_mapping: dict,
    body_root: Path | str,
    source_root: Path | str,
    workspace_root: Path | str,
    canvas_size: list[int] | None = None,
) -> dict:
    body_root = Path(body_root).resolve()
    source_root = Path(source_root).resolve()
    workspace_root = Path(workspace_root).resolve()
    canvas_size = canvas_size or DEFAULT_CANVAS_SIZE
    workspace_root.mkdir(parents=True, exist_ok=True)
    (workspace_root / "DO-NOT-PACK.md").write_text(
        "Authoring workspace only. Do not pack, copy or promote these template/reference files as runtime source layers.\n",
        encoding="utf-8",
    )

    repairs = slot_repair_index(repair_plan)
    targets = []
    failures = []
    for target in brief.get("targets") or []:
        repair = repairs.get(target["slot"])
        if not repair:
            failures.append(f"missing repair plan for slot {target['slot']}")
            continue
        targets.append(
            prepare_target(target, repair, source_mapping, body_root, source_root, workspace_root, canvas_size)
        )

    report = {
        "status": "SOURCE_AUTHORING_WORKSPACE_READY" if not failures else "SOURCE_AUTHORING_WORKSPACE_INCOMPLETE",
        "runtimePromotionAllowed": False,
        "workspaceRoot": str(workspace_root),
        "sourceRoot": str(source_root),
        "bodyRoot": str(body_root),
        "canvasSize": canvas_size,
        "targetCount": len(targets),
        "targets": targets,
        "failures": failures,
        "createdAt": datetime.now(timezone.utc).isoformat(),
        "usage": "Use this as a Krita/manual authoring package. It creates references and blank templates only; it does not satisfy repair-layer audit or runtime promotion.",
    }
    (workspace_root / "authoring-workspace-manifest.json").write_text(
        json.dumps(report, ensure_ascii=False, indent=2) + "\n",
        encoding="utf-8",
    )
    (workspace_root / "README.md").write_text(render_readme(report), encoding="utf-8")
    return report


def render_readme(report: dict) -> str:
    lines = [
        "# Six-pose source authoring workspace",
        "",
        "Do not pack this workspace. It contains references, guide overlays and blank templates only.",
        "",
        f"Status: `{report['status']}`",
        f"Runtime promotion allowed: `{str(report['runtimePromotionAllowed']).lower()}`",
        f"Target count: `{report['targetCount']}`",
        "",
        "Workflow:",
        "",
        "1. Open a target folder under `targets/<slot>/<pose>/`.",
        "2. Use `body-reference.png`, `guide-overlay-on-body.png` and `source-reference.png` when present.",
        "3. Copy a transparent template into Krita or another native editor layer.",
        "4. Draw/export clean RGBA source layers to each `destinationExports` path in the manifest.",
        "5. Run repair-layer audit and source board review before any Player pack.",
        "",
        "Targets:",
        "",
    ]
    for target in report.get("targets") or []:
        jump = " — jump body acceptance required" if target.get("requiresBodyAnatomyAcceptance") else ""
        lines.append(f"- `{target['slot']}` / `{target['pose']}`{jump}")
    lines.append("")
    return "\n".join(lines)


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--brief", type=Path, required=True)
    parser.add_argument("--repair-plan", type=Path, required=True)
    parser.add_argument("--source-mapping", type=Path, required=True)
    parser.add_argument("--body-root", type=Path, required=True)
    parser.add_argument("--source-root", type=Path, required=True)
    parser.add_argument("--workspace-root", type=Path, required=True)
    parser.add_argument("--canvas-size", default="1024x1536")
    args = parser.parse_args()
    canvas_size = [int(part) for part in args.canvas_size.lower().split("x", 1)]
    report = prepare_authoring_workspace(
        load_json(args.brief),
        load_json(args.repair_plan),
        load_json(args.source_mapping),
        args.body_root,
        args.source_root,
        args.workspace_root,
        canvas_size,
    )
    print(
        json.dumps(
            {
                "status": report["status"],
                "runtimePromotionAllowed": report["runtimePromotionAllowed"],
                "targetCount": report["targetCount"],
                "workspaceRoot": report["workspaceRoot"],
            },
            ensure_ascii=False,
        )
    )
    return 0 if report["status"] == "SOURCE_AUTHORING_WORKSPACE_READY" else 1


if __name__ == "__main__":
    raise SystemExit(main())
