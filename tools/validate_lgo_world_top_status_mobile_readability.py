#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def require(path: str, *markers: str) -> None:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return
    text = file_path.read_text(encoding="utf-8", errors="replace")
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
        ERRORS.append("frozen surface changed")


def main() -> int:
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeWorldHudResponsiveLayout.ApplyTopStatus(layout, worldVisible, width, _headerActions, _status, _quitButton)",
        "FormatTopStatusMessage",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeWorldHudResponsiveLayout.cs",
        "LGO Runtime World HUD Responsive Layout Helper v1",
        "LGO World Top Status Mobile Readability v1",
        "ApplyTopStatus",
        "headerActions",
        'Sẵn sàng: Bước 1/2',
        "RuntimeUiSpacing.TopStatusWorldMobileMaxWidthRatioPercent",
        "RuntimeUiSpacing.TopStatusWorldMobileMinWidth",
        "RuntimeUiSpacing.TopStatusWorldMobileMaxWidth",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        'if (_isMobileProfile)',
        'Replace("Bước 1: tìm Người Giữ Cổng", "Bước 1/2")',
        'Replace("Bước 2: ổn định Đá Luyện", "Bước 2/2")',
        '"Hoàn tất"',
    )
    require(
        "docs/tasks/LGO-WORLD-TOP-STATUS-MOBILE-READABILITY-PASS-v1.0.md",
        "LGO_WORLD_TOP_STATUS_MOBILE_READABILITY_READY",
        "No gameplay change",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_top_status_mobile_readability",
        "validate_lgo_world_top_status_mobile_readability.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-TOP-STATUS-MOBILE-EVIDENCE-REFRESH-v1.0",
        "LGO_WORLD_TOP_STATUS_MOBILE_READABILITY_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-TOP-STATUS-MOBILE-READABILITY-PASS v1.0",
        "LGO_WORLD_TOP_STATUS_MOBILE_READABILITY_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD TOP STATUS MOBILE READABILITY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_WORLD_TOP_STATUS_MOBILE_READABILITY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
