#!/usr/bin/env python3.12
"""Write a lightweight slot-ownership area audit for source-pose surfaces."""
from __future__ import annotations

import argparse
import json
from pathlib import Path

POSES = ("idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck")
SLOTS = (
    "main_weapon", "inner_top", "lower_body", "outer_top", "waist_belt",
    "footwear", "arm_guard", "shoulder_chest_guard", "head_hair", "class_accessory",
)

DEFAULT_RATIO_LIMITS = {
    "class_accessory": 0.12,
    "head_hair": 0.16,
    "outer_top": 0.30,
}


def _alpha_count(path: Path) -> int:
    from PIL import Image

    alpha = Image.open(path).convert("RGBA").getchannel("A")
    histogram = alpha.histogram()
    return sum(histogram[9:])


def audit_surface(surface: Path, ratio_limits: dict[str, float] | None = None) -> dict:
    ratio_limits = ratio_limits or DEFAULT_RATIO_LIMITS
    rows = []
    for pose in POSES:
        counts = {}
        missing = []
        for slot in SLOTS:
            path = surface / slot / f"{pose}.png"
            if not path.is_file():
                missing.append(f"{slot}/{pose}.png")
                counts[slot] = 0
                continue
            counts[slot] = _alpha_count(path)
        total = sum(counts.values())
        ratios = {slot: (counts[slot] / total if total else 0.0) for slot in SLOTS}
        flags = sorted(slot for slot, limit in ratio_limits.items() if ratios.get(slot, 0.0) > limit)
        rows.append({
            "pose": pose,
            "totalSlotPixels": total,
            "counts": counts,
            "ratios": ratios,
            "flags": flags,
            "missing": missing,
            "topSlots": [
                {"slot": slot, "pixels": pixels, "ratio": ratios[slot]}
                for slot, pixels in sorted(counts.items(), key=lambda item: item[1], reverse=True)[:5]
            ],
        })
    return {
        "status": "SOURCE_OWNERSHIP_AUDIT",
        "surface": str(surface.resolve()),
        "poses": list(POSES),
        "slots": list(SLOTS),
        "ratioLimits": ratio_limits,
        "rows": rows,
    }


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--surface", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args(argv)
    payload = audit_surface(args.surface.resolve())
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    flagged = sum(1 for row in payload["rows"] if row["flags"] or row["missing"])
    print(f"LGO_SOURCE_SLOT_AREA_AUDIT_WRITTEN output={args.output} flaggedRows={flagged}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
