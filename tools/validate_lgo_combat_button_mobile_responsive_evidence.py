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


def require_binary(rel: str) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing evidence file: {rel}")
        return
    if path.stat().st_size < 64 * 1024:
        ERRORS.append(f"evidence file too small for visual review: {rel}")


def check_manifest(profile: str) -> None:
    rel = f"build/visual-evidence/profiles/{profile}/visual-runtime-evidence-manifest.json"
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing manifest: {rel}")
        return
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        ERRORS.append(f"invalid manifest json {rel}: {exc}")
        return
    checkpoints = {
        item.get("id"): item
        for item in data.get("checkpoints", [])
        if isinstance(item, dict)
    }
    checkpoint = checkpoints.get("target-dummy-state")
    if not checkpoint:
        ERRORS.append(f"{rel} missing target-dummy-state checkpoint")
        return
    expectation = checkpoint.get("expectation", "")
    for marker in ("cooldown ring", "combat button fit", "local-only combat copy"):
        if marker not in expectation:
            ERRORS.append(f"{rel} target-dummy expectation missing marker: {marker}")
    if checkpoint.get("status") != "CAPTURED":
        ERRORS.append(f"{rel} target-dummy-state not captured")


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
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiEvidenceState.CombatPanelFocus",
        "RuntimeUiEvidenceState.None",
        "evidenceHidesGuidance",
        "LGO Combat Button Mobile Responsive Evidence v1",
        "(!compactViewport || _evidenceState.ForceCombatPanel)",
        '_localCombatButton.text = coolingDown ? "Hồi chiêu" : "Tấn công thử";',
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiEvidenceState.cs",
        "internal readonly struct RuntimeUiEvidenceState",
        "CombatPanelFocus",
        "ForceCombatPanel",
        "HideGuidanceCardOnCompact",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/VisualRuntimeEvidenceRunner.cs",
        "target-dummy-state.png",
        "combat button fit",
    )
    require(
        "docs/tasks/LGO-COMBAT-BUTTON-MOBILE-RESPONSIVE-EVIDENCE-v1.0.md",
        "LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_READY",
        "build/visual-evidence/profiles/mobile/target-dummy-state.png",
        "No combat mechanic change",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-COMBAT-BUTTON-MOBILE-RESPONSIVE-EVIDENCE-v1.0",
        "LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-COMBAT-BUTTON-MOBILE-RESPONSIVE-EVIDENCE v1.0",
        "LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "combat_button_mobile_responsive_evidence",
        "validate_lgo_combat_button_mobile_responsive_evidence.py",
    )
    for profile in ("desktop", "tablet", "mobile"):
        require_binary(f"build/visual-evidence/profiles/{profile}/target-dummy-state.png")
        check_manifest(profile)
    check_frozen()
    if ERRORS:
        print("LGO COMBAT BUTTON MOBILE RESPONSIVE EVIDENCE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
