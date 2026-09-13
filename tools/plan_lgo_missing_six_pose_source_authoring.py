#!/usr/bin/env python3.12
"""Create a measured authoring brief for missing six-pose repair source."""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path


GUARDRAILS = [
    "do_not_synthesize_missing_pose",
    "do_not_use_runtime_offset_or_pose_scale",
    "do_not_pack_preview_or_full_alpha_composite",
    "do_not_resume_skeletal_generated_cutout_path",
]


def _parse_missing_failure(failure: str) -> tuple[str, str] | None:
    parts = failure.split("/")
    if len(parts) < 4 or not failure.endswith(" missing"):
        return None
    slot, _variant, pose, _filename = parts[:4]
    return slot, pose


def _payload_sha256(data: dict) -> str:
    payload = dict(data)
    payload.pop("briefPayloadSha256", None)
    return hashlib.sha256(
        json.dumps(payload, ensure_ascii=False, sort_keys=True, separators=(",", ":")).encode()
    ).hexdigest()


def _target_components(slot: str) -> list[str]:
    return ["front", "back"]


def _authoring_note(slot: str, pose: str) -> str:
    if pose == "jump_tuck":
        return "Jump source requires accepted jump body/anatomy review before drawing final garment fit."
    if slot == "outer_top":
        return "Use outer_top shoulder/waist guide to draw collar, torso body and hem ownership in source space."
    if slot == "shoulder_chest_guard":
        return "Use guard shoulder/chest guide; keep it as rigid overlay without changing body silhouette."
    if slot == "waist_belt":
        return "Use waist line/angle as source-space placement guide; do not use belt to hide outer hem errors."
    return "Use slot guide as source-space drawing target."


def plan_missing_source_authoring(repair_audit: dict, measurements: dict) -> dict:
    missing_pairs = []
    seen = set()
    for failure in repair_audit.get("failures") or []:
        parsed = _parse_missing_failure(failure)
        if parsed and parsed not in seen:
            seen.add(parsed)
            missing_pairs.append(parsed)

    targets = []
    poses = measurements.get("poses") or {}
    for slot, pose in missing_pairs:
        pose_measurement = poses.get(pose)
        if not pose_measurement:
            raise ValueError(f"Missing measurement pose: {pose}")
        slot_guide = (pose_measurement.get("slotGuides") or {}).get(slot)
        if not slot_guide:
            raise ValueError(f"Missing slot guide: {slot}/{pose}")
        targets.append(
            {
                "slot": slot,
                "pose": pose,
                "requiredVariants": ["A", "B"],
                "requiredComponents": _target_components(slot),
                "slotGuide": slot_guide,
                "measurements": pose_measurement.get("measurements", {}),
                "authoringNote": _authoring_note(slot, pose),
                "dimensionUse": (
                    "Use these source-space coordinates to draw/check layer shape on the 1024x1536 canvas; "
                    "they are not runtime offsets, not per-pose scale normalization, and not visual approval."
                ),
            }
        )

    status = "MISSING_SOURCE_AUTHORING_BRIEF_READY" if targets else "NO_MISSING_SOURCE_TARGETS"
    result = {
        "status": status,
        "runtimePromotionAllowed": False,
        "repairAuditStatus": repair_audit.get("status"),
        "measurementStatus": measurements.get("status"),
        "guideStatus": measurements.get("guideStatus"),
        "guideSanity": measurements.get("guideSanity", {}).get("status"),
        "proportionSanity": measurements.get("proportionSanity", {}).get("status"),
        "targetCount": len(targets),
        "targets": targets,
        "guardrails": GUARDRAILS,
        "usage": "Measured source-authoring brief only. It does not create source art and does not permit runtime packing.",
    }
    result["briefPayloadSha256"] = _payload_sha256(result)
    return result


def render_markdown(plan: dict) -> str:
    lines = [
        "# Missing six-pose source authoring brief",
        "",
        f"Status: `{plan['status']}`",
        "runtimePromotionAllowed=false",
        "",
        "Các số đo dưới đây dùng để vẽ/kiểm layer trong source-space 1024×1536; không phải runtime offset, không phải scale riêng từng pose và không phải visual approval.",
        "",
        "## Targets",
        "",
    ]
    for target in plan["targets"]:
        guide = target["slotGuide"]
        detail = []
        if "center" in guide:
            detail.append(f"center `{guide['center']}`")
        if "angleDegrees" in guide:
            detail.append(f"angle `{guide['angleDegrees']}`")
        if "line" in guide:
            detail.append(f"line `{guide['line']}`")
        if "shoulderLine" in guide:
            detail.append(f"shoulderLine `{guide['shoulderLine']}`")
        if "waistLine" in guide:
            detail.append(f"waistLine `{guide['waistLine']}`")
        lines.extend(
            [
                f"### {target['slot']} / {target['pose']}",
                "",
                f"- Required: A/B × {', '.join(target['requiredComponents'])}",
                f"- Guide: {'; '.join(detail)}",
                f"- Note: {target['authoringNote']}",
                "",
            ]
        )
    lines.extend(["## Guardrails", ""])
    lines.extend([f"- `{item}`" for item in plan["guardrails"]])
    lines.append("")
    return "\n".join(lines)


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def write_outputs(plan: dict, output_json: Path, output_md: Path | None) -> None:
    output_json.parent.mkdir(parents=True, exist_ok=True)
    output_json.write_text(json.dumps(plan, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    if output_md:
        output_md.parent.mkdir(parents=True, exist_ok=True)
        output_md.write_text(render_markdown(plan), encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--repair-audit", type=Path, required=True)
    parser.add_argument("--measurements", type=Path, required=True)
    parser.add_argument("--output-json", type=Path, required=True)
    parser.add_argument("--output-md", type=Path)
    args = parser.parse_args()
    plan = plan_missing_source_authoring(load_json(args.repair_audit), load_json(args.measurements))
    write_outputs(plan, args.output_json, args.output_md)
    print(
        json.dumps(
            {
                "status": plan["status"],
                "targetCount": plan["targetCount"],
                "runtimePromotionAllowed": plan["runtimePromotionAllowed"],
            },
            ensure_ascii=False,
        )
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
