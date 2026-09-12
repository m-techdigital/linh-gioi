#!/usr/bin/env python3.12
"""Guard the owner-review source-pose catalog from exposing unapproved class art."""
from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

import launch_lgo_source_pose_review as launcher  # noqa: E402


def fail(message: str) -> int:
    print("LGO_OWNER_REVIEW_CATALOG_FAIL " + message, file=sys.stderr)
    return 1


def main() -> int:
    if tuple(launcher.CLASSES) != ("vo",):
        return fail("interactive catalog must stay locked to Võ until another class has source-pose visual approval")
    source = (ROOT / "tools/launch_lgo_source_pose_review.py").read_text(encoding="utf-8")
    required = "until each has non-base source-pose class art"
    if required not in source:
        return fail("launcher is missing the non-base class-art gate comment")
    for class_id in ("kiem", "phap", "co", "linh"):
        if class_id not in launcher.PACK_SUFFIXES:
            return fail("audit pack suffix missing for " + class_id)
    print("LGO_OWNER_REVIEW_CATALOG_PASS classes=vo audit_only=kiem,phap,co,linh")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
