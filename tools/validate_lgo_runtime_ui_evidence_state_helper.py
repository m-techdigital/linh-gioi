#!/usr/bin/env python3
from __future__ import annotations

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


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiEvidenceState.cs",
        "internal readonly struct RuntimeUiEvidenceState",
        "RuntimeUiEvidenceState None",
        "RuntimeUiEvidenceState CombatPanelFocus",
        "ForceCombatPanel",
        "HideGuidanceCardOnCompact",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiEvidenceState.cs.meta",
        "guid:",
        "MonoImporter:",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private RuntimeUiEvidenceState _evidenceState;",
        "RuntimeUiEvidenceState.None",
        "RuntimeUiEvidenceState.CombatPanelFocus",
        "evidenceHidesGuidance",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_forceCombatPanelForEvidence",
    )
    require(
        "docs/design/RUNTIME-UI-EVIDENCE-STATE-HELPER-REVIEW-v1.0.md",
        "LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY",
        "Evidence state must not alter account, character, world, combat, protocol, or GameData semantics",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-EVIDENCE-STATE-HELPER-REVIEW-v1.0.md",
        "LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY",
        "No gameplay change",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_evidence_state_helper",
        "validate_lgo_runtime_ui_evidence_state_helper.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-EVIDENCE-STATE-HELPER-REVIEW-v1.0",
        "LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-EVIDENCE-STATE-HELPER-REVIEW v1.0",
        "LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI EVIDENCE STATE HELPER VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
