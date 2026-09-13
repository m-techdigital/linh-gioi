#!/usr/bin/env python3.12
"""Audit pose-matched layer authoring coverage before any runtime pack.

This tool reports source coverage only. A complete file matrix is not visual
approval, and idle candidate previews are not enough to promote runtime assets.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

POSES = ("idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck")
VARIANTS = ("A", "B")
COMPONENTS = ("front", "back")

DEFAULT_SLOT_LAYOUT = {
    "inner_top": {
        "kind": "six_pose_native_front_back",
        "directory": "inner-top-native-v2",
        "report": "authoring-report.json",
    },
    "class_accessory": {
        "kind": "six_pose_native_front_back",
        "directory": "accessory-native-v1",
        "report": "authoring-report.json",
    },
    "outer_top": {
        "kind": "idle_material_candidate",
        "directory": "outer-top-material-idle-v2",
        "report": "material-provenance.json",
    },
    "waist_belt": {
        "kind": "idle_material_candidate",
        "directory": "waist-belt-material-idle-v2",
        "runDirectory": "waist-belt-run-four-material-v1",
        "report": "material-provenance.json",
    },
    "shoulder_chest_guard": {
        "kind": "idle_material_candidate",
        "directory": "shoulder-chest-guard-material-idle-v4",
        "report": "material-provenance.json",
    },
}


def digest(path: Path) -> str | None:
    path = Path(path)
    return hashlib.sha256(path.read_bytes()).hexdigest() if path.exists() else None


def load_json(path: Path) -> dict:
    return json.loads(path.read_text()) if path.exists() else {}


def audit_native_slot(slot_root: Path, report_name: str) -> dict:
    coverage, missing = {}, []
    present = 0
    required = len(VARIANTS) * len(POSES) * len(COMPONENTS)
    for variant in VARIANTS:
        coverage[variant] = {}
        for pose in POSES:
            coverage[variant][pose] = {}
            for component in COMPONENTS:
                path = slot_root / variant / pose / f"{component}.png"
                exists = path.exists()
                present += int(exists)
                if not exists:
                    missing.append(f"{variant}/{pose}/{component}")
                coverage[variant][pose][component] = {
                    "exists": exists,
                    "path": str(path),
                    "sha256": digest(path),
                }
    report = load_json(slot_root / report_name)
    return {
        "kind": "six_pose_native_front_back",
        "path": str(slot_root),
        "exists": slot_root.exists(),
        "present": present,
        "requiredCount": required,
        "missing": missing,
        "poseCoverage": coverage,
        "candidateStatus": report.get("status"),
        "sourceStatus": report.get("sourceStatus"),
        "runtimeEligible": report.get("runtimeEligible"),
    }


def audit_idle_candidate(slot_root: Path, report_name: str, run_root: Path | None = None) -> dict:
    exports = sorted(
        path
        for pattern in ("*export.png", "*material.png")
        for path in slot_root.glob(pattern)
        if path.is_file()
    )
    covered = ["idle"] if exports else []
    run_records = []
    if run_root and run_root.exists():
        for pose in POSES:
            for path in sorted(run_root.glob(f"{pose}-*export.png")):
                covered.append(pose)
                run_records.append({"pose": pose, "path": str(path), "sha256": digest(path)})
    report = load_json(slot_root / report_name)
    covered = sorted(set(covered), key=POSES.index)
    return {
        "kind": "idle_material_candidate",
        "path": str(slot_root),
        "exists": slot_root.exists(),
        "idleCandidateFiles": [
            {"path": str(path), "sha256": digest(path)} for path in exports
        ],
        "poseCoverage": {"idle": bool(exports)},
        "runDraftPath": str(run_root) if run_root else None,
        "runDraftFiles": run_records,
        "coveredCandidatePoses": covered,
        "missingPoses": [pose for pose in POSES if pose not in covered],
        "missingVariants": list(VARIANTS),
        "candidateStatus": report.get("status"),
        "selectionStatus": report.get("selectionStatus"),
        "reviewFinding": report.get("reviewFinding"),
        "runtimeEligible": report.get("runtimeEligible"),
    }


def audit_source_root(source_root: Path | str, body_root: Path | str | None = None) -> dict:
    source_root = Path(source_root).resolve()
    body_root = Path(body_root).resolve() if body_root else None
    result = {
        "status": "SOURCE_COVERAGE_INCOMPLETE",
        "runtimeEligible": False,
        "sourceRoot": str(source_root),
        "bodyRoot": str(body_root) if body_root else None,
        "poses": list(POSES),
        "variants": list(VARIANTS),
        "slots": {},
        "blockingGates": [],
    }
    for slot, config in DEFAULT_SLOT_LAYOUT.items():
        slot_root = source_root / config["directory"]
        if config["kind"] == "six_pose_native_front_back":
            record = audit_native_slot(slot_root, config["report"])
            if record["present"] != record["requiredCount"]:
                result["blockingGates"].append(f"{slot}: missing six-pose A/B components")
        else:
            run_root = source_root / config["runDirectory"] if config.get("runDirectory") else None
            record = audit_idle_candidate(slot_root, config["report"], run_root)
            if len(record["coveredCandidatePoses"]) > 1:
                result["blockingGates"].append(
                    f"{slot}: draft covers {len(record['coveredCandidatePoses'])}/6 poses but lacks A/B source acceptance"
                )
            elif record["idleCandidateFiles"]:
                result["blockingGates"].append(
                    f"{slot}: only idle candidate, no six-pose A/B source acceptance"
                )
            else:
                result["blockingGates"].append(f"{slot}: missing idle candidate")
        result["slots"][slot] = record
    result["blockingGates"].extend(
        [
            "outer_top collar/hem ownership unresolved",
            "jump body anatomy candidate not accepted",
        ]
    )
    return result


def payload_sha256(data: dict) -> str:
    payload = dict(data)
    payload.pop("auditPayloadSha256", None)
    return hashlib.sha256(
        json.dumps(payload, ensure_ascii=False, sort_keys=True, separators=(",", ":")).encode()
    ).hexdigest()


def write_audit(source_root: Path | str, body_root: Path | str | None, output: Path | str) -> dict:
    output = Path(output)
    output.parent.mkdir(parents=True, exist_ok=True)
    result = audit_source_root(source_root, body_root)
    result["auditPayloadSha256"] = payload_sha256(result)
    output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n")
    return result


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("source_root", type=Path)
    parser.add_argument("--body-root", type=Path)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = write_audit(args.source_root, args.body_root, args.output)
    print(json.dumps({"status": result["status"], "blockingGates": result["blockingGates"]}, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
