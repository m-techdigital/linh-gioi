#!/usr/bin/env python3
from __future__ import annotations

import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def require(path: str, *markers: str) -> None:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return
    text = file_path.read_text(encoding="utf-8")
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{path} missing marker: {marker}")


def main() -> int:
    require(
        "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
        "RefreshWorldLabelPresentation",
        "SetWorldLabelActive",
        "GuidedTrainingStep.FindGateKeeper",
        "GuidedTrainingStep.FindTrainingStone",
        "PlaceholderVfxFeedbackState.TargetDummyHitFlash",
        "LocalCombatPrototypeState.WindSlashRangeM",
        "fontSize = 42",
        "characterSize = 0.042f",
    )
    require(
        "docs/tasks/LGO-WORLD-HUB-PROP-LABEL-RESPONSIVE-PASS-v1.0.md",
        "LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY",
        "presentation-only",
        "No gameplay",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hub_prop_label_responsive",
        "validate_lgo_world_hub_prop_label_responsive.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUB-PROP-LABEL-RESPONSIVE-PASS-v1.0",
        "LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUB-PROP-LABEL-RESPONSIVE-PASS v1.0",
        "LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY",
    )

    if ERRORS:
        print("LGO WORLD HUB PROP LABEL RESPONSIVE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
