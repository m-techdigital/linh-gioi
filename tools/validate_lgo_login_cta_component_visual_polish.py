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
        "LGO Login CTA Component Visual Polish v1",
        "LoginCtaComponentVisualPolishMarker",
        "RuntimeUiSkin.ApplyLoginCtaBacking(_loginCard);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "LoginCardMinHeight => IsMobile ? Mathf.RoundToInt(108f * MobileScale) : IsTablet ? 140 : 152",
        "LoginCardPaddingTop => IsMobile ? Mathf.RoundToInt(8f * MobileScale) : IsTablet ? 14 : 16",
        "LoginCardPaddingBottom => IsMobile ? Mathf.RoundToInt(9f * MobileScale) : IsTablet ? 14 : 16",
        "new Color(0.005f, 0.018f, 0.040f, 0.18f)",
        "new Color(0.005f, 0.018f, 0.040f, 0.24f)",
        "new Color(0.005f, 0.018f, 0.040f, 0.28f)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyLoginCtaBacking(VisualElement element)",
        "new Color(0.93f, 0.73f, 0.36f, 0.42f)",
        "element.style.borderTopWidth = 2;",
        "element.style.borderBottomWidth = 2;",
        "ApplyServerSelectorFrame(VisualElement element)",
        "new Color(0.003f, 0.015f, 0.035f, 0.82f)",
    )
    require(
        "docs/design/LOGIN-CTA-COMPONENT-VISUAL-POLISH-v1.0.md",
        "LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_READY",
        "does not add PNGs",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "docs/tasks/LGO-LOGIN-CTA-COMPONENT-VISUAL-POLISH-v1.0.md",
        "LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_READY",
        "LGO-LOGIN-CTA-COMPONENT-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-LOGIN-CTA-COMPONENT-EVIDENCE-REFRESH-v1.0",
        "LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-LOGIN-CTA-COMPONENT-VISUAL-POLISH v1.0",
        "LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "login_cta_component_visual_polish",
        "validate_lgo_login_cta_component_visual_polish.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO LOGIN CTA COMPONENT VISUAL POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
