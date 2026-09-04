#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []

REQUIRED_ASSETS = [
    "client/Unity/Assets/Game/Art/Runtime/FinalLogin/Resources/LGOFinalLogin/login_background_spirit_gate_final_1920x1080.jpg",
    "client/Unity/Assets/Game/Art/Runtime/FinalLogin/Resources/LGOFinalLogin/logo_linh_gioi_online_final_420.png",
    "client/Unity/Assets/Game/Art/Runtime/FinalLogin/Resources/LGOFinalLogin/button_enter_world_final_384.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/login/login_background_spirit_gate_1920x1080.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/login/logo_linh_gioi_online_2048x1024.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/login/gate_keeper_npc_1024x1536.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/ui/buttons/button_enter_world_normal_1024x256.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/ui/buttons/button_enter_world_pressed_1024x256.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/ui/buttons/button_disabled_1024x256.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/ui/panels/server_selector_panel_1024x256.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/ui/panels/panel_main_1536x768.png",
    "client/Unity/Assets/Game/Art/Runtime/V3BA/Resources/LGOArtV3BA/ui/status/server_online_256x256.png",
]

RUNTIME_LIMITS = {
    "login_background_spirit_gate_final_1920x1080.jpg": 520 * 1024,
    "logo_linh_gioi_online_final_420.png": 220 * 1024,
    "button_enter_world_final_384.png": 130 * 1024,
    "login_background_spirit_gate_1920x1080.png": 220 * 1024,
    "logo_linh_gioi_online_2048x1024.png": 80 * 1024,
    "gate_keeper_npc_1024x1536.png": 80 * 1024,
    "button_enter_world_normal_1024x256.png": 40 * 1024,
    "button_enter_world_pressed_1024x256.png": 40 * 1024,
    "button_disabled_1024x256.png": 40 * 1024,
    "server_selector_panel_1024x256.png": 40 * 1024,
    "panel_main_1536x768.png": 40 * 1024,
    "server_online_256x256.png": 16 * 1024,
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
        "client/Unity/Assets/Game/Art/Runtime/LgoFinalLoginAssetRegistry.cs",
        "LGO_FINAL_LOGIN_RUNTIME_ART_CANDIDATE",
        "LoginBackgroundSpiritGate",
        "LogoLinhGioiOnline",
        "ButtonEnterWorldTexture",
    )
    require(
        "client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3BA.cs",
        "LGO_ART_V3BA_LOGIN_GATE_ENTRY_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL",
        "LoginBackgroundSpiritGate",
        "ButtonEnterWorldNormalTexture",
        "ServerSelectorPanelTexture",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "LGO Login Gate Entry Final Shell",
        "LGO Login Gate Entry Logo V3BA",
        "LGO Login Enter World CTA V3BA",
        "LgoFinalLoginAssetRegistry.LoginBackgroundSpiritGate",
        "LgoFinalLoginAssetRegistry.LogoLinhGioiOnline",
        "LgoFinalLoginAssetRegistry.ButtonEnterWorldTexture",
        "Bước qua Linh Môn",
        "Vào Thế Giới",
        "Khóa thử nghiệm",
    )
    require(
        "docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-PACK-v3b-a.md",
        "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1",
        "not claimed as final production art",
    )
    require("docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-MAPPING-v3b-a.csv", "runtime candidate not production final")
    require("docs/tasks/LOGIN-GATE-ENTRY-VISUAL-v1.md", "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1")
    require("docs/execution/checklists/LOGIN-GATE-ENTRY-VISUAL-CHECKLIST-v1.md", "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1")
    require("HANDOFF-LGO-LOGIN-GATE-ENTRY-VISUAL-v1.md", "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1")
    require("LGO-LOGIN-GATE-ENTRY-VISUAL-FINAL-REPORT-v1.md", "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1")
    check_assets()
    check_no_reference_import()
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
