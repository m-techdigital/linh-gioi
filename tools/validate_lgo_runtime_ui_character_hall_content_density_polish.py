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
            ERRORS.append(f"{rel} still contains dense copy marker: {marker}")


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
    controller = require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "LGO Character Hall Collapsed Class Summary v1",
        "_selectedClassSummary.style.display = DisplayStyle.None;",
        "_selectedMeta.text = \"Kiếm tu sơ nhập / chờ tạo hồ sơ\";",
        "_selectedMeta.text = \"Kiếm tu sơ nhập / sẵn sàng qua Linh Môn\";",
        "Mục tiêu: Tạo hoặc chọn tu sĩ để vào sân luyện.",
        "Mục tiêu: Vào sân luyện, gặp Người Giữ Cổng, rồi lưu vị trí.",
    )
    if controller.count("_selectedClassSummary.style.display = DisplayStyle.None;") < 2:
        ERRORS.append("selected class summary should stay collapsed in empty and selected states")
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "Mạch tu luyện: Kiếm tu sơ nhập / vai trò cân bằng.",
        "Mạch tu luyện: Kiếm tu sơ nhập đã sẵn sàng cho phiên hiện tại.",
    )
    require(
        "docs/design/RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-POLISH-v1.0.md",
        "LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_READY",
        "The selected character card now keeps two visible status rows",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-POLISH-v1.0.md",
        "LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_READY",
        "LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-POLISH v1.0",
        "LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_character_hall_content_density_polish",
        "validate_lgo_runtime_ui_character_hall_content_density_polish.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI CHARACTER HALL CONTENT DENSITY POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
