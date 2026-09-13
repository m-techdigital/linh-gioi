#!/usr/bin/env python3.12
"""Audit repeated pose-clothing authoring method failures.

This is a process gate, not a visual approval. It prevents the sandbox from
creating another small candidate inside a method family that already failed
repeatedly without new measurement evidence or a different authoring contract.
"""
from __future__ import annotations

import argparse
import json
from pathlib import Path


METHOD_RULES = {
    "polygon_eye_fit": {
        "patterns": ["outer-top-run-four-material-v1", "outer-top-run-four-material-v2"],
        "lockAt": 2,
        "reason": "hand-drawn torso polygons adjusted by eye produced floating shield-like outer_top",
    },
    "body_mask_large_fill": {
        "patterns": ["outer-top-run-four-material-v3-bodymask"],
        "lockAt": 1,
        "reason": "large body-mask fill attached silhouette but generated sleeve/collar artifacts",
    },
    "ai_or_composite_patch": {
        "patterns": [
            "outer-top-material-idle-v5",
            "outer-top-material-idle-v6",
            "outer-top-material-idle-v8-inpaint-raw-rejected",
        ],
        "lockAt": 2,
        "reason": "AI/composite patch changed placement or carried background/glow into alpha",
    },
    "contour_pixel_nudge_no_anchor": {
        "patterns": ["outer-top-material-idle-v3", "outer-top-material-idle-v4", "outer-top-material-idle-v7"],
        "lockAt": 2,
        "reason": "collar/hem contour edits without measured anchors damaged neckline, alpha, or material continuity",
    },
}

ALLOWED_METHODS = {
    "measured_anchor_stack": "uses reviewed guide measurements and stack-level board before candidate promotion",
    "guide_or_body_fix": "fixes source landmarks/body candidate before garment authoring",
    "new_authoring_proof": "uses a different measurable authoring contract",
}


def existing_candidate_names(root: Path) -> set[str]:
    return {path.name for path in root.iterdir() if path.is_dir()}


def classify_methods(root: Path) -> dict:
    names = existing_candidate_names(root)
    families = {}
    for family, rule in METHOD_RULES.items():
        candidates = [pattern for pattern in rule["patterns"] if pattern in names]
        locked = len(candidates) >= rule["lockAt"]
        families[family] = {
            "candidateCount": len(candidates),
            "candidates": candidates,
            "repeatStatus": "LOCKED_AFTER_REPEATED_FAILURE" if locked else "NOT_LOCKED",
            "lockAt": rule["lockAt"],
            "reason": rule["reason"],
        }
    return families


def audit_method_repeats(root: Path | str, proposed_method_family: str | None = None) -> dict:
    root = Path(root)
    families = classify_methods(root)
    blocked_families = [
        family for family, info in families.items() if info["repeatStatus"] == "LOCKED_AFTER_REPEATED_FAILURE"
    ]
    proposed_blocked = proposed_method_family in blocked_families
    if proposed_method_family is None:
        status = "METHOD_REPEAT_AUDIT_REVIEW_REQUIRED" if blocked_families else "METHOD_REPEAT_AUDIT_CLEAR"
    elif proposed_blocked:
        status = "METHOD_REPEAT_BLOCKED"
    else:
        status = "METHOD_CHANGE_ALLOWED"
    return {
        "status": status,
        "root": str(root),
        "proposedMethodFamily": proposed_method_family,
        "blockedMethodFamilies": blocked_families,
        "allowedMethodFamilies": ALLOWED_METHODS,
        "methodFamilies": families,
        "usage": (
            "Run before creating a new pose-clothing candidate. If status is METHOD_REPEAT_BLOCKED, "
            "do not generate another candidate in that method family; switch to guide/body fix, "
            "measured anchor stack, or a new measurable proof."
        ),
    }


def write_audit(root: Path | str, output: Path | str, proposed_method_family: str | None = None) -> dict:
    output = Path(output)
    output.parent.mkdir(parents=True, exist_ok=True)
    result = audit_method_repeats(root, proposed_method_family)
    output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return result


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("root", type=Path)
    parser.add_argument("--proposed-method-family")
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = write_audit(args.root, args.output, args.proposed_method_family)
    print(json.dumps({"status": result["status"], "blocked": result["blockedMethodFamilies"]}, ensure_ascii=False))
    return 2 if result["status"] == "METHOD_REPEAT_BLOCKED" else 0


if __name__ == "__main__":
    raise SystemExit(main())
