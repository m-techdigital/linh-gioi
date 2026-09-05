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
        "docs/design/RUNTIME-UI-FACTORY-SPLIT-REVIEW-v1.0.md",
        "LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY",
        "Do not split `M4PlayableClientController` into screen-level factories yet.",
        "Approved Next Split",
        "Runtime visual style remains in `RuntimeUiSkin`",
        "LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS-v1.0",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-FACTORY-SPLIT-REVIEW-v1.0.md",
        "LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY",
        "No broad screen-level split yet",
        "Leaf-level stateless UI factory split is allowed next",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_factory_split_review",
        "validate_lgo_runtime_ui_factory_split_review.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-FACTORY-SPLIT-REVIEW-v1.0",
        "LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-FACTORY-SPLIT-REVIEW v1.0",
        "LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI FACTORY SPLIT REVIEW VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
