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
    "REJECTED_SOURCE_VISUAL",
}


def _is_rejected_status(value: object) -> bool:
    status = str(value or "").upper()
    return (
        status in REJECTED_STATUSES
        or "REJECTED" in status
        or "WITHDRAWN" in status
    )


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
    authoring_rejected = {}
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
        if any(_is_rejected_status(value) for value in status_values):
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
    for path in sorted(source_root.rglob("authoring-selection.json")):
        data = _load_json(path)
        status_values = [
            data.get("visualReviewStatus", ""),
            data.get("selectionStatus", ""),
            data.get("status", ""),
        ]
        if any(_is_rejected_status(value) for value in status_values):
            relative = path.relative_to(source_root)
            directory = relative.parts[0] if relative.parts else str(path.parent)
            authoring_rejected[directory] = {
                "selection": str(relative),
                "statusValues": [str(value) for value in status_values if value],
                "runtimeEligible": data.get("runtimeEligible"),
            }
    return {
        "visualRejected": visual_rejected,
        "authoringRejected": authoring_rejected,
        "priorRejected": prior_rejected,
        "provenanceFiles": provenance_files,
    }


def _is_quarantined_reference(reference: str) -> bool:
    return "rejected-evidence" in {part.lower() for part in Path(str(reference)).parts}


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
        if _is_quarantined_reference(relative):
            failures.append(f"{slot}/{pose} selects quarantined rejected evidence")
            record["state"] = "rejected"
        if directory in index["visualRejected"]:
            failures.append(f"{slot}/{pose} selects visually rejected directory {directory}")
            record["state"] = "rejected"
        if directory in index["authoringRejected"]:
            failures.append(f"{slot}/{pose} selects authoring-rejected directory {directory}")
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
