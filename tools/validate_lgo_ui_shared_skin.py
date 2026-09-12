#!/usr/bin/env python3
"""Guard Map01A runtime UI against per-screen duplicate skin systems."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
UI_DIR = ROOT / "client/Unity/Assets/Game/UI/Runtime"
SKIN = UI_DIR / "CongDongLamArrivalHud.Skin.cs"
PARTIALS = sorted(UI_DIR.glob("CongDongLamArrivalHud*.cs"))

REQUIRED_SKIN_MARKERS = [
    "ApplyLgoFrame",
    "ApplyLgoGlassPanel",
    "ApplyLgoModalShell",
    "ApplyLgoButton",
    "ApplyLgoSelectedTab",
]
FORBIDDEN_LOCAL_PATTERNS = [
    re.compile(r"private\s+static\s+readonly\s+Color\s+(?!Ui)[A-Za-z0-9_]*(Glass|Gold|Blue|Border|Text|SubText)"),
    re.compile(r"private\s+static\s+void\s+StyleFrame\s*\("),
    re.compile(r"private\s+static\s+Label\s+(?!LgoLabel\b)[A-Za-z0-9_]*Label\s*\("),
]
# Exact legacy snippets that previously caused each screen to grow its own skin.
FORBIDDEN_SNIPPETS = [
    "InventoryGlass",
    "InventoryGlassRaised",
    "InventoryGold",
    "InventoryBlue",
    "style.gap =",
]


def fail(message: str) -> int:
    print("LGO_UI_SHARED_SKIN_FAIL " + message, file=sys.stderr)
    return 1


def main() -> int:
    if not SKIN.is_file():
        return fail("missing CongDongLamArrivalHud.Skin.cs shared skin")
    skin_text = SKIN.read_text(encoding="utf-8", errors="replace")
    missing = [marker for marker in REQUIRED_SKIN_MARKERS if marker not in skin_text]
    if missing:
        return fail("skin_missing_markers=" + ",".join(missing))

    violations: list[str] = []
    for path in PARTIALS:
        text = path.read_text(encoding="utf-8", errors="replace")
        rel = path.relative_to(ROOT)
        if path == SKIN:
            continue
        for snippet in FORBIDDEN_SNIPPETS:
            if snippet in text:
                violations.append(f"{rel}: forbidden snippet {snippet}")
        for pattern in FORBIDDEN_LOCAL_PATTERNS:
            for match in pattern.finditer(text):
                violations.append(f"{rel}: local skin pattern {match.group(0)}")
    if violations:
        for item in violations:
            print(item, file=sys.stderr)
        return fail(f"violations={len(violations)}")
    print("LGO_UI_SHARED_SKIN_PASS partials=" + str(len(PARTIALS)))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
