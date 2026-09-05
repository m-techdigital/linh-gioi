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
            ERRORS.append(f"{rel} still contains forbidden marker: {marker}")


def require_png(rel: str, min_size: int = 12_000) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing screenshot: {rel}")
        return
    if path.stat().st_size < min_size:
        ERRORS.append(f"screenshot too small: {rel} size={path.stat().st_size}")


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
        "client/Unity/Assets/Game/UI/Runtime/M5VisualEvidenceRunner.cs",
        "LGO M5 Visual Evidence Reusable Runtime Panel v1",
        "RuntimeUiFactory.NewPanel(860)",
        "RuntimeUiSkin.ApplyText",
    )
    require(
        "tools/run_m5_visual_evidence_review.sh",
        'OUT_DIR="$ROOT/build/visual-evidence/m5-latest"',
        "--lgo-m5-visual-evidence-review",
    )
    reject(
        "tools/run_m5_visual_evidence_review.sh",
        'OUT_DIR="$ROOT/build/visual-evidence"',
    )
    require(
        "docs/tasks/LGO-M5-VISUAL-EVIDENCE-RUNNER-SKIN-ADOPTION-EVIDENCE-v1.0.md",
        "LGO_M5_VISUAL_EVIDENCE_RUNNER_SKIN_ADOPTION_EVIDENCE_READY",
        "build/visual-evidence/m5-latest",
        "No `VISUAL_RUNTIME_PASS` claim",
        "visual acceptance and UI quality review must use the current runtime harness",
        "No human visual acceptance claim from M5 compatibility screenshots",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-VISUAL-EVIDENCE-OUTPUT-ISOLATION-AUDIT-v1.0",
        "LGO_M5_VISUAL_EVIDENCE_RUNNER_SKIN_ADOPTION_EVIDENCE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-M5-VISUAL-EVIDENCE-RUNNER-SKIN-ADOPTION-EVIDENCE v1.0",
        "LGO_M5_VISUAL_EVIDENCE_RUNNER_SKIN_ADOPTION_EVIDENCE_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "m5_visual_evidence_runner_skin_adoption_evidence",
        "validate_lgo_m5_visual_evidence_runner_skin_adoption_evidence.py",
    )
    require(
        "build/visual-evidence/m5-latest/visual-evidence-summary.txt",
        "LGO_PLAYABLE_VISUAL_EVIDENCE_READY",
        "humanVisualAcceptancePending=true",
    )
    require(
        "build/dev-loop/m5-visual-evidence-runner-skin-adoption.log",
        "LGO_PLAYABLE_VISUAL_EVIDENCE_READY",
    )
    require_png("build/visual-evidence/m5-latest/gate-entry.png")
    require_png("build/visual-evidence/m5-latest/character-hall.png")
    require_png("build/visual-evidence/m5-latest/world-hud.png")
    check_frozen()
    if ERRORS:
        print("LGO M5 VISUAL EVIDENCE RUNNER SKIN ADOPTION EVIDENCE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_M5_VISUAL_EVIDENCE_RUNNER_SKIN_ADOPTION_EVIDENCE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
