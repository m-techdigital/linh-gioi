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


def check_no_forbidden_runtime_art() -> None:
    forbidden_paths = [
        ROOT / "client/Unity/Assets/Game/Art/Runtime/V3BA",
        ROOT / "client/Unity/Assets/Game/Art/Runtime/FinalLogin",
    ]
    for path in forbidden_paths:
        if path.exists():
            ERRORS.append(f"forbidden runtime art surface exists: {path.relative_to(ROOT)}")


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
        "LGO Login Gate Entry NPC Composition Stage V3B",
        "LGO Login Gate Keeper Soft Grounding Glow V3B",
        "LGO Login Gate Entry Control Column V3B Final",
        "LGO Login Gate Entry V3B Final Logo Text Lockup",
        "LGO Login Gate Entry Bottom CTA v3 Final Panel V3B",
        "position = Position.Relative",
        "position = Position.Absolute",
        "LgoVisualAssetRegistryV3B.GateKeeperNpcLoginTexture",
        "LgoVisualAssetRegistryV3B.LogoLinhGioiOnline",
        "LgoVisualAssetRegistryV3B.ButtonEnterWorldGoldTexture",
        "LgoVisualAssetRegistryV3B.PanelMainDarkGoldTexture",
        "Mathf.Clamp(width * 0.46f",
        "mobile ? DisplayStyle.None : DisplayStyle.Flex",
    )
    require(
        "docs/tasks/LGO-LOGIN-NPC-COMPOSITING-POLISH-v1.0.md",
        "LGO_LOGIN_NPC_COMPOSITING_POLISH_READY",
        "V3B runtime candidate",
        "no V3BA",
        "no reference poster import",
    )
    check_no_forbidden_runtime_art()
    check_frozen()
    if ERRORS:
        print("LGO LOGIN NPC COMPOSITING POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_LOGIN_NPC_COMPOSITING_POLISH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
