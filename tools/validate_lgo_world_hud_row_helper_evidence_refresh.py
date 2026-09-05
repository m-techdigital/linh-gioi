#!/usr/bin/env python3
from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing file: {rel}")
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def require(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")


def require_file(rel: str, min_bytes: int = 1) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing file: {rel}")
        return
    if path.stat().st_size < min_bytes:
        ERRORS.append(f"{rel} smaller than expected: {path.stat().st_size} bytes")


def check_manifest() -> None:
    path = ROOT / "build/visual-evidence/latest/visual-runtime-evidence-manifest.json"
    if not path.is_file():
        ERRORS.append("missing visual evidence manifest")
        return
    data = json.loads(path.read_text(encoding="utf-8"))
    checkpoints = data.get("checkpoints", [])
    names = {str(item.get("file", "")) for item in checkpoints}
    for name in ["world-hub.png", "npc-dialogue.png", "session-menu.png", "target-dummy-state.png"]:
        if name not in names:
            ERRORS.append(f"manifest missing screenshot: {name}")


def check_frozen() -> None:
    result = subprocess.run(
        [
            "git",
            "--no-pager",
            "diff",
            "--name-only",
            "--",
            "protocol",
            "gamedata/schemas",
            "docs/adr",
            "client/Unity/Assets/Game/UI/design-tokens.json",
        ],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "git frozen diff failed")
    elif result.stdout.strip():
        ERRORS.append("frozen surface changed")


def main() -> int:
    for screenshot in [
        "build/visual-evidence/latest/world-hub.png",
        "build/visual-evidence/latest/npc-dialogue.png",
        "build/visual-evidence/latest/session-menu.png",
        "build/visual-evidence/latest/target-dummy-state.png",
    ]:
        require_file(screenshot, 10_000)
    require_file("build/visual-evidence/latest/visual-runtime-evidence-review.md", 100)
    check_manifest()
    require(
        "docs/tasks/LGO-WORLD-HUD-ROW-HELPER-EVIDENCE-REFRESH-v1.0.md",
        "LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_READY",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUD-ROW-HELPER-EVIDENCE-REFRESH-v1.0",
        "LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUD-ROW-HELPER-EVIDENCE-REFRESH v1.0",
        "LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hud_row_helper_evidence_refresh",
        "validate_lgo_world_hud_row_helper_evidence_refresh.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUD ROW HELPER EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
