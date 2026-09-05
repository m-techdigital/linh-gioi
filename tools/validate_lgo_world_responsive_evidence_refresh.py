#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(path: str) -> str:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return ""
    return file_path.read_text(encoding="utf-8", errors="replace")


def require(path: str, *markers: str) -> None:
    text = read(path)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{path} missing marker: {marker}")


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
        ERRORS.append("frozen contract/design-token surface changed")


def main() -> int:
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "LGO Mobile World Viewport Evidence Fit v1",
        "layout.WorldHudBaseMaxWidth",
        "layout.WorldHudMinWidth",
        "_worldObjective.style.fontSize = mobile ? 14 : 15",
        "_interactionHint.style.fontSize = mobile ? 14 : 15",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "Mathf.Clamp(Width * 0.28f, 238f, 272f)",
    )
    require(
        "docs/tasks/LGO-WORLD-RESPONSIVE-EVIDENCE-REFRESH-v1.0.md",
        "LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_READY",
        "No gameplay change",
        "No new runtime image import",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_responsive_evidence_refresh",
        "validate_lgo_world_responsive_evidence_refresh.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-RESPONSIVE-EVIDENCE-REFRESH-v1.0",
        "LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-RESPONSIVE-EVIDENCE-REFRESH v1.0",
        "LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD RESPONSIVE EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
