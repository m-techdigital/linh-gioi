#!/usr/bin/env python3
"""Report legacy UI validators that still point at removed M4 runtime UI code.

This is intentionally a report, not a failing gate: the historical validators are
kept for provenance, but current 2D UI work must not use them as proof or revive
M4/V3B UI just to satisfy stale checks.
"""
from __future__ import annotations

from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
LEGACY_TARGET = "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs"
PATTERN = "M4PlayableClientController.cs"


def main() -> int:
    target = ROOT / LEGACY_TARGET
    refs: list[Path] = []
    for path in sorted((ROOT / "tools").glob("validate_lgo_*.py")):
        text = path.read_text(encoding="utf-8", errors="replace")
        if PATTERN in text:
            refs.append(path.relative_to(ROOT))

    status = "PRESENT" if target.is_file() else "MISSING"
    classification = "CURRENT" if target.is_file() else "LEGACY_STALE_NOT_CURRENT_GATE"
    print("LGO_LEGACY_UI_VALIDATOR_REF_REPORT")
    print(f"target={LEGACY_TARGET}")
    print(f"target_status={status}")
    print(f"classification={classification}")
    print(f"legacy_validator_ref_count={len(refs)}")
    for ref in refs:
        print(f"legacy_validator_ref={ref}")
    if not refs:
        print("owner_note=Không còn validator UI cũ trỏ M4PlayableClientController.cs.")
    elif not target.is_file():
        print("owner_note=Các validator này là lịch sử/provenance; không dùng làm gate cho UI 2D mới và không phục hồi M4/V3B chỉ để làm xanh chúng.")
    else:
        print("owner_note=Target còn tồn tại; cần audit lại trước khi phân loại legacy.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
