#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []

REQUIRED_ASSETS = [
    "client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/login_background_spirit_gate_1920x1080_v3b_candidate.jpg",
    "client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/logo_linh_gioi_online_v3b_light_runtime_candidate.png",
    "client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/panel_main_dark_gold_v3b_candidate.png",
    "client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/button_enter_world_gold_v3b_candidate.png",
    "client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/gate_keeper_npc_login_v3b_candidate.png",
]

RUNTIME_LIMITS = {
    "login_background_spirit_gate_1920x1080_v3b_candidate.jpg": 560 * 1024,
    "logo_linh_gioi_online_v3b_light_runtime_candidate.png": 300 * 1024,
    "panel_main_dark_gold_v3b_candidate.png": 140 * 1024,
    "button_enter_world_gold_v3b_candidate.png": 90 * 1024,
    "gate_keeper_npc_login_v3b_candidate.png": 190 * 1024,
}


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


def check_assets() -> None:
    for rel in REQUIRED_ASSETS:
        path = ROOT / rel
        if not path.is_file():
            ERRORS.append(f"missing runtime asset: {rel}")
            continue
        if not path.with_name(path.name + ".meta").is_file():
            ERRORS.append(f"missing Unity meta: {rel}.meta")
        limit = RUNTIME_LIMITS.get(path.name)
        if limit is not None and path.stat().st_size > limit:
            ERRORS.append(f"asset exceeds login budget: {rel} {path.stat().st_size} > {limit}")


def check_no_reference_import() -> None:
    imported = [
        path
        for path in (ROOT / "client/Unity/Assets").rglob("*")
        if path.is_file() and ("reference-only" in path.as_posix() or "catalog-reference" in path.name or "target-reference" in path.name)
    ]
    for path in imported:
        ERRORS.append(f"reference-only image imported into Unity: {path.relative_to(ROOT)}")


def check_no_v3ba_runtime() -> None:
    forbidden_paths = [
        ROOT / "client/Unity/Assets/Game/Art/Runtime/V3BA",
        ROOT / "client/Unity/Assets/Game/Art/Runtime/FinalLogin",
        ROOT / "client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3BA.cs",
        ROOT / "client/Unity/Assets/Game/Art/Runtime/LgoFinalLoginAssetRegistry.cs",
    ]
    for path in forbidden_paths:
        if path.exists():
            ERRORS.append(f"V3BA runtime surface must be removed: {path.relative_to(ROOT)}")
    forbidden_markers = ("V3BA", "LGOArtV3BA", "LgoVisualAssetRegistryV3BA", "LgoFinalLoginAssetRegistry")
    text_files = [
        ROOT / "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        ROOT / "client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3B.cs",
    ]
    for path in text_files:
        if not path.is_file():
            ERRORS.append(f"missing file: {path.relative_to(ROOT)}")
            continue
        text = path.read_text(encoding="utf-8", errors="replace")
        for marker in forbidden_markers:
            if marker in text:
                ERRORS.append(f"{path.relative_to(ROOT)} still references forbidden login art marker: {marker}")


def check_frozen() -> None:
    result = subprocess.run(
        ["git", "--no-pager", "diff", "--name-only", "--", "protocol", "gamedata/schemas", "docs/adr", "client/Unity/Assets/Game/UI/design-tokens.json"],
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
        "client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3B.cs",
        "LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL",
        "LoginBackgroundSpiritGate",
        "LogoLinhGioiOnline",
        "PanelMainDarkGoldTexture",
        "ButtonEnterWorldGoldTexture",
        "GateKeeperNpcLoginTexture",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "LGO Login Gate Entry Final Shell",
        "LGO Login Gate Entry V3B Final Logo",
        "LGO Login Gate Keeper NPC V3B",
        "LGO Login Gate Entry Control Column V3B",
        "LGO Login Gate Entry Bottom CTA v3",
        "LGO Login Enter World CTA Final v2",
        "LGO Login Server Online Dot",
        "LGO Login Server Switch Secondary",
        "LgoVisualAssetRegistryV3B.LoginBackgroundSpiritGate",
        "LgoVisualAssetRegistryV3B.PanelMainDarkGoldTexture",
        "LgoVisualAssetRegistryV3B.ButtonEnterWorldGoldTexture",
        "LgoVisualAssetRegistryV3B.GateKeeperNpcLoginTexture",
        "Bước qua Linh Môn",
        "Vào Thế Giới",
        "Khóa thử nghiệm",
        "style.display = DisplayStyle.None",
    )
    require(
        "docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-PACK-v3b-runtime.md",
        "LGO_LOGIN_GATE_ENTRY_V3B_RUNTIME_ONLY",
        "Runtime Login Assets",
        "V3B assets are runtime candidates, not final production art",
    )
    require("docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-MAPPING-v3b-runtime.csv", "V3B runtime candidate not production final")
    require("docs/tasks/LOGIN-GATE-ENTRY-VISUAL-v1.md", "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1")
    require("docs/execution/checklists/LOGIN-GATE-ENTRY-VISUAL-CHECKLIST-v1.md", "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1")
    require("HANDOFF-LGO-LOGIN-GATE-ENTRY-VISUAL-v1.md", "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1")
    require("LGO-LOGIN-GATE-ENTRY-VISUAL-FINAL-REPORT-v1.md", "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1")
    check_assets()
    check_no_reference_import()
    check_no_v3ba_runtime()
    check_frozen()
    if ERRORS:
        print("LGO LOGIN GATE ENTRY VISUAL VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_LOGIN_GATE_ENTRY_VISUAL_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
