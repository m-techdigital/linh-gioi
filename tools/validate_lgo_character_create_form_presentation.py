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
        "LGO Character Create Form Game Copy v1",
        "LGO Character Create Form Framed Input v1",
        "ApplyLobbyInputStyle",
        '"Danh xưng"',
        '"Tạo tu sĩ"',
        "Mạch tu luyện khởi đầu: Kiếm tu sơ nhập.",
    )
    require(
        "docs/tasks/LGO-CHARACTER-CREATE-FORM-PRESENTATION-PASS-v1.0.md",
        "LGO_CHARACTER_CREATE_FORM_PRESENTATION_READY",
        "No account semantics change",
        "No production art claim",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "character_create_form_presentation",
        "validate_lgo_character_create_form_presentation.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-CHARACTER-CREATE-FORM-PRESENTATION-PASS-v1.0",
        "LGO_CHARACTER_CREATE_FORM_PRESENTATION_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-CHARACTER-CREATE-FORM-PRESENTATION-PASS v1.0",
        "LGO_CHARACTER_CREATE_FORM_PRESENTATION_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO CHARACTER CREATE FORM PRESENTATION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_CHARACTER_CREATE_FORM_PRESENTATION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
