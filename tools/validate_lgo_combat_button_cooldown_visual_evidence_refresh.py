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


def require(rel: str, *markers: str) -> str:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")
    return text


def require_png(rel: str) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing screenshot: {rel}")
        return
    with path.open("rb") as handle:
        if handle.read(8) != b"\x89PNG\r\n\x1a\n":
            ERRORS.append(f"not a PNG screenshot: {rel}")


def check_manifest() -> None:
    manifest_path = ROOT / "build/visual-evidence/latest/visual-runtime-evidence-manifest.json"
    if not manifest_path.is_file():
        ERRORS.append("missing visual runtime manifest")
        return
    try:
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        ERRORS.append(f"invalid visual runtime manifest json: {exc}")
        return
    checkpoint_files = {checkpoint.get("file") for checkpoint in manifest.get("checkpoints", [])}
    for screenshot in ("target-dummy-state.png", "world-hub.png", "session-menu.png"):
        if screenshot not in checkpoint_files:
            ERRORS.append(f"manifest missing checkpoint: {screenshot}")
        require_png(f"build/visual-evidence/latest/{screenshot}")


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
    require(
        "docs/tasks/LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-EVIDENCE-REFRESH-v1.0.md",
        "LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY",
        "No `VISUAL_RUNTIME_PASS` claim",
        "LGO-RUNTIME-UI-STATE-DOC-COMPACTION-AUDIT-v1.0",
    )
    require(
        "build/visual-evidence/latest/visual-runtime-evidence-review-vi.md",
        "EVIDENCE_CAPTURED_FOR_REVIEW",
        "target-dummy-state.png",
        "không tự claim `VISUAL_RUNTIME_PASS`",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "combat_button_cooldown_visual_evidence_refresh",
        "validate_lgo_combat_button_cooldown_visual_evidence_refresh.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-EVIDENCE-REFRESH-v1.0",
        "LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY",
        "LGO-RUNTIME-UI-STATE-DOC-COMPACTION-AUDIT-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-EVIDENCE-REFRESH v1.0",
        "LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY",
    )
    check_manifest()
    check_frozen()
    if ERRORS:
        print("LGO COMBAT BUTTON COOLDOWN VISUAL EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
