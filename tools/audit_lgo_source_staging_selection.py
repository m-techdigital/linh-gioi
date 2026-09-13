#!/usr/bin/env python3.12
"""Audit source staging selection against provenance rejection markers."""
from __future__ import annotations

import argparse
import json
from pathlib import Path


REJECTED_STATUSES = {
    "VISUAL_REJECTED",
    "VISUAL_REJECTED_PRESERVED_FOR_LESSONS",
    "OWNER_REJECTED_VISUAL",
}


def _load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8")) if path.exists() else {}


def _directory_from_reference(reference: str, source_root: Path | None = None) -> str:
    text = str(reference)
    path = Path(text)
    if path.is_absolute() and source_root:
        try:
            relative = path.resolve().relative_to(source_root)
            return relative.parts[0] if relative.parts else str(relative)
        except ValueError:
            pass
    if "/" in text:
        parts = Path(text).parts
        return parts[0] if parts else text
    return path.name


def collect_rejection_index(source_root: Path) -> dict:
    visual_rejected = {}
    prior_rejected = {}
    provenance_files = []
    for path in sorted(source_root.rglob("material-provenance.json")):
        data = _load_json(path)
        directory = str(path.parent.relative_to(source_root))
        provenance_files.append(str(path.relative_to(source_root)))
        status_values = [
            str(data.get("visualReviewStatus", "")),
            str(data.get("selectionStatus", "")),
            str(data.get("status", "")),
            str(data.get("reviewFinding", "")),
        ]
        if any(value in REJECTED_STATUSES or value.startswith("VISUAL_REJECTED") for value in status_values):
            visual_rejected[directory] = {
                "provenance": str(path.relative_to(source_root)),
                "statusValues": [value for value in status_values if value],
            }
        prior = data.get("priorRejected")
        if isinstance(prior, str):
            prior = [prior]
        for item in prior or []:
            rejected_directory = _directory_from_reference(item, source_root)
            prior_rejected.setdefault(rejected_directory, []).append(str(path.relative_to(source_root)))
    return {
        "visualRejected": visual_rejected,
        "priorRejected": prior_rejected,
        "provenanceFiles": provenance_files,
    }


def _selected_entries(selection: dict) -> list[tuple[str, str, str]]:
    slots = selection.get("slots", selection)
    entries = []
    for slot, poses in slots.items():
        for pose, relative in poses.items():
            entries.append((slot, pose, relative))
    return entries


def audit_selection(source_root: Path | str, selection: dict) -> dict:
    source_root = Path(source_root).resolve()
    index = collect_rejection_index(source_root)
    failures = []
    records = []
    for slot, pose, relative in _selected_entries(selection):
        directory = _directory_from_reference(relative, source_root)
        record = {
            "slot": slot,
            "pose": pose,
            "path": relative,
            "directory": directory,
            "state": "review_only",
        }
        if directory in index["visualRejected"]:
            failures.append(f"{slot}/{pose} selects visually rejected directory {directory}")
            record["state"] = "rejected"
        if directory in index["priorRejected"]:
            failures.append(f"{slot}/{pose} selects prior-rejected directory {directory}")
            record["state"] = "rejected"
            record["priorRejectedBy"] = index["priorRejected"][directory]
        records.append(record)
    return {
        "status": "SOURCE_STAGING_SELECTION_REJECTED" if failures else "SOURCE_STAGING_SELECTION_REVIEW_ONLY",
        "runtimePromotionAllowed": False,
        "sourceRoot": str(source_root),
        "records": records,
        "failures": failures,
        "rejectionIndex": index,
        "usage": "Selection provenance audit only. Review-only sources still require layer, board and visual gates.",
    }


def write_audit(source_root: Path | str, selection_path: Path | str, output: Path | str) -> dict:
    selection = _load_json(Path(selection_path))
    report = audit_selection(source_root, selection)
    output = Path(output)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return report


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("source_root", type=Path)
    parser.add_argument("--selection", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    report = write_audit(args.source_root, args.selection, args.output)
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
