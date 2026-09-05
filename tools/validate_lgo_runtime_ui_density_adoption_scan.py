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


def require(rel: str, *markers: str) -> str:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")
    return text


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains direct style drift marker: {marker}")


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
        "RuntimeUiSkin.ApplyPadding(_root, RuntimeUiSpacing.EvidenceRootPaddingHorizontal",
        "RuntimeUiFactory.NewPanel(860)",
        "RuntimeUiSkin.ApplyText(header, RuntimeArtCatalog.Gold, 32, true)",
        "RuntimeUiSpacing.EvidenceLineMarginTop",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M5VisualEvidenceRunner.cs",
        "_root.style.paddingLeft = 24;",
        "panel.style.paddingLeft = 18;",
        "panel.style.borderLeftWidth = 3;",
        "label.style.fontSize = 18;",
        "footer.style.color = RuntimeArtCatalog.Muted;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "EvidenceRootPaddingHorizontal",
        "EvidencePanelPaddingHorizontal",
        "EvidenceLineMarginTop",
    )
    require(
        "docs/design/RUNTIME-UI-DENSITY-ADOPTION-SCAN-v1.0.md",
        "LGO_RUNTIME_UI_DENSITY_ADOPTION_SCAN_READY",
        "M5VisualEvidenceRunner",
        "UIShowcaseController",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-DENSITY-ADOPTION-SCAN-v1.0.md",
        "LGO_RUNTIME_UI_DENSITY_ADOPTION_SCAN_READY",
        "LGO-M5-VISUAL-EVIDENCE-RUNNER-SKIN-ADOPTION-EVIDENCE-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-M5-VISUAL-EVIDENCE-RUNNER-SKIN-ADOPTION-EVIDENCE-v1.0",
        "LGO_RUNTIME_UI_DENSITY_ADOPTION_SCAN_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-DENSITY-ADOPTION-SCAN v1.0",
        "LGO_RUNTIME_UI_DENSITY_ADOPTION_SCAN_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_density_adoption_scan",
        "validate_lgo_runtime_ui_density_adoption_scan.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI DENSITY ADOPTION SCAN VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_DENSITY_ADOPTION_SCAN_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
