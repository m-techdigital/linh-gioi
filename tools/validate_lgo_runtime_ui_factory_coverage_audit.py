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
            ERRORS.append(f"{rel} still contains rejected duplicate marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "internal static VisualElement NewImageLayer",
        "layer.pickingMode = PickingMode.Ignore;",
        "layer.style.unityBackgroundScaleMode = scaleMode;",
        "layer.style.backgroundImage = new StyleBackground(texture);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "NewImageLayer(\"LGO Login Gate Keeper NPC V3B\"",
        "NewImageLayer(\"LGO Login Gate Entry V3B Final Logo Text Lockup\"",
        "NewImageLayer(\"LGO Character Hall V3B Cultivator Portrait\"",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "gateKeeper.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;",
        "logoLockup.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;",
        "portrait.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;",
    )
    require(
        "docs/design/RUNTIME-UI-FACTORY-COVERAGE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_READY",
        "NewImageLayer",
        "Sizing, margins, screen positioning",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-FACTORY-COVERAGE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_READY",
        "No gameplay",
        "LGO-RUNTIME-UI-IMAGE-LAYER-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_factory_coverage_audit",
        "validate_lgo_runtime_ui_factory_coverage_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-FACTORY-COVERAGE-AUDIT-v1.0",
        "LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_READY",
        "LGO-RUNTIME-UI-IMAGE-LAYER-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-FACTORY-COVERAGE-AUDIT v1.0",
        "LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI FACTORY COVERAGE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
