#!/usr/bin/env python3.12
"""Plan a grouped six-pose source repair batch from coverage evidence.

The output is a repair contract, not visual approval. It keeps the next
authoring pass tied to measurable source coverage and blocks the stopped
skeletal/generated-cutout loop from being resumed by accident.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path


REPAIR_SLOT_DIRECTORIES = {
    "outer_top": "outer-top-six-pose-source-repair-v1",
    "waist_belt": "waist-belt-six-pose-source-repair-v1",
    "shoulder_chest_guard": "shoulder-chest-guard-six-pose-source-repair-v1",
}

SLOT_OWNERSHIP_NOTES = {
    "outer_top": [
        "Chốt collar/neck seam thuộc outer_top hay inner_top trước khi export.",
        "Chốt hem overlap với waist_belt, không để cả hai slot cùng che một mép.",
    ],
    "waist_belt": [
        "Giữ belt là lớp vòng eo riêng; không dùng belt để vá mép áo.",
        "Dùng run draft hiện có làm tham chiếu, nhưng phải xuất lại A/B front/back đủ sáu pose.",
    ],
    "shoulder_chest_guard": [
        "Guard là overlay cứng trên ngực/vai, không thay đổi body silhouette.",
        "Nếu level/variant đổi silhouette lớn, tách family mới thay vì ép reuse.",
    ],
}

FORBIDDEN_METHOD_FAMILIES = [
    "skeletal_generated_cutout_runtime_probe",
    "polygon_body_mask_floating_panels",
    "ai_composite_geometry_authority",
    "per_pose_pixel_nudging",
    "runtime_offset_or_pose_scale_compensation",
]

FORBIDDEN_MARKDOWN_LINES = {
    "skeletal_generated_cutout_runtime_probe": "Không dùng skeletal generated-cutout hoặc Player probe cũ.",
    "polygon_body_mask_floating_panels": "Không dùng polygon/body-mask floating panels đã fail.",
    "ai_composite_geometry_authority": "Không lấy AI/composite làm geometry authority.",
    "per_pose_pixel_nudging": "Không chỉnh mò từng pose/pixel.",
    "runtime_offset_or_pose_scale_compensation": "Không dùng runtime offset hoặc pose-scale để che lỗi source.",
}


def _basename(path: str | None) -> str | None:
    return Path(path).name if path else None


def _sha256_payload(data: dict) -> str:
    payload = dict(data)
    payload.pop("planPayloadSha256", None)
    return hashlib.sha256(
        json.dumps(payload, ensure_ascii=False, sort_keys=True, separators=(",", ":")).encode()
    ).hexdigest()


def plan_repair_batch(coverage: dict, batch_id: str = "six-pose-source-repair-batch-v1") -> dict:
    source_root = coverage.get("sourceRoot")
    poses = list(coverage.get("poses") or [])
    variants = list(coverage.get("variants") or ["A", "B"])
    slots = coverage.get("slots") or {}
    repairs = []

    for slot in ("outer_top", "waist_belt", "shoulder_chest_guard"):
        record = slots.get(slot) or {}
        reuse_directories = [
            name
            for name in (
                _basename(record.get("path")),
                _basename(record.get("runDraftPath")),
            )
            if name
        ]
        covered_poses = list(record.get("coveredCandidatePoses") or [])
        missing_poses = list(record.get("missingPoses") or [pose for pose in poses if pose not in covered_poses])
        repairs.append(
            {
                "slot": slot,
                "candidateDirectory": REPAIR_SLOT_DIRECTORIES[slot],
                "reuseSourceDirectories": reuse_directories,
                "coveredCandidatePoses": covered_poses,
                "missingPoses": missing_poses,
                "requiredPoses": poses,
                "requiredVariants": variants,
                "requiredComponents": ["front", "back"],
                "requiredExportCount": len(poses) * len(variants) * 2,
                "slotOwnershipNotes": SLOT_OWNERSHIP_NOTES[slot],
                "acceptanceGate": "SIX_POSE_A_B_FRONT_BACK_SOURCE_REVIEW_REQUIRED",
            }
        )

    status = "SOURCE_REPAIR_BATCH_READY" if repairs else "NO_SOURCE_REPAIR_REQUIRED"
    plan = {
        "status": status,
        "batchId": batch_id,
        "runtimePromotionAllowed": False,
        "sourceRoot": source_root,
        "bodyRoot": coverage.get("bodyRoot"),
        "coverageStatus": coverage.get("status"),
        "coverageBlockingGates": list(coverage.get("blockingGates") or []),
        "slotRepairs": repairs,
        "sharedDependencies": [
            "jump_tuck_body_anatomy_acceptance",
            "source_canvas_profile_1024x1536_origin512_ground1484",
            "mixed_level_off_slot_review_board_before_player_pack",
        ],
        "forbiddenMethodFamilies": FORBIDDEN_METHOD_FAMILIES,
        "evidenceTargets": {
            "sourceBoard": "build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/source-board-v1/contact-sheet.png",
            "repairManifest": "build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/repair-batch-plan.json",
            "repairBrief": "build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/repair-batch-brief.md",
            "nativeRoundTrip": "build/pose-matched-layer-authoring-v1/six-pose-source-repair-batch-v1/native-roundtrip-v1.json",
        },
        "nextAllowedAction": (
            "Create candidate directories under sourceRoot, repair all missing slots as one source batch, "
            "then compose the source board before any runtime pack."
        ),
    }
    plan["planPayloadSha256"] = _sha256_payload(plan)
    return plan


def render_markdown(plan: dict) -> str:
    lines = [
        "# Six-pose source repair batch v1",
        "",
        f"Status: `{plan['status']}`",
        f"runtimePromotionAllowed={str(plan['runtimePromotionAllowed']).lower()}",
        "",
        "Source root:",
        "",
        f"- `{plan.get('sourceRoot')}`",
        "",
        "Evidence board target:",
        "",
        f"- `{plan['evidenceTargets']['sourceBoard']}`",
        "",
        "## Slot repair batch",
        "",
    ]
    for repair in plan["slotRepairs"]:
        lines.extend(
            [
                f"### {repair['slot']}",
                "",
                f"- Candidate directory: `{repair['candidateDirectory']}`",
                f"- Reuse/reference directories: `{', '.join(repair['reuseSourceDirectories']) or 'none'}`",
                f"- Covered candidate poses: `{', '.join(repair['coveredCandidatePoses']) or 'none'}`",
                f"- Missing poses: `{', '.join(repair['missingPoses']) or 'none'}`",
                f"- Required exports: `{repair['requiredExportCount']}` = A/B × six poses × front/back",
                "- Ownership notes:",
            ]
        )
        lines.extend([f"  - {note}" for note in repair["slotOwnershipNotes"]])
        lines.append("")

    lines.extend(
        [
            "## Shared dependencies",
            "",
        ]
    )
    lines.extend([f"- `{dep}`" for dep in plan["sharedDependencies"]])
    lines.extend(
        [
            "",
            "## Forbidden loops",
            "",
        ]
    )
    lines.extend([f"- {FORBIDDEN_MARKDOWN_LINES[item]}" for item in plan["forbiddenMethodFamilies"]])
    lines.extend(
        [
            "",
            "## Next allowed action",
            "",
            plan["nextAllowedAction"],
            "",
        ]
    )
    return "\n".join(lines)


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def write_outputs(plan: dict, output_json: Path, output_md: Path | None = None) -> None:
    output_json.parent.mkdir(parents=True, exist_ok=True)
    output_json.write_text(json.dumps(plan, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    if output_md:
        output_md.parent.mkdir(parents=True, exist_ok=True)
        output_md.write_text(render_markdown(plan), encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("coverage", type=Path)
    parser.add_argument("--batch-id", default="six-pose-source-repair-batch-v1")
    parser.add_argument("--output-json", type=Path, required=True)
    parser.add_argument("--output-md", type=Path)
    args = parser.parse_args()
    plan = plan_repair_batch(load_json(args.coverage), batch_id=args.batch_id)
    write_outputs(plan, args.output_json, args.output_md)
    print(json.dumps({"status": plan["status"], "slotRepairs": [item["slot"] for item in plan["slotRepairs"]]}, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
