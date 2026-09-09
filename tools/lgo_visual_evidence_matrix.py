#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
VISUAL_DIR = ROOT / "build/2d-onboarding-visual"
MANIFEST = VISUAL_DIR / "twod-onboarding-visual-manifest.json"

LEGACY_VIEWS: list[dict[str, Any]] = [
    {
        "id": "login_gate_entry",
        "label": "Login / Gate Entry",
        "requiredEvidence": ["runtime screenshot", "capture log"],
        "nonClaim": "not production art",
    },
    {
        "id": "character_hall",
        "label": "Character Hall",
        "requiredEvidence": ["runtime screenshot", "capture log"],
        "nonClaim": "not production auth",
    },
    {
        "id": "world_hud",
        "label": "World HUD",
        "requiredEvidence": ["runtime screenshot", "capture log"],
        "nonClaim": "not MMO-scale gameplay",
    },
    {
        "id": "first_playable_loop",
        "label": "First Playable Loop",
        "requiredEvidence": ["runtime screenshot", "smoke log"],
        "nonClaim": "not full quest/content system",
    },
    {
        "id": "combat_readiness_hud",
        "label": "Combat Readiness HUD",
        "requiredEvidence": ["runtime screenshot", "combat smoke json"],
        "nonClaim": "not production combat",
    },
    {
        "id": "combat_placeholder_assets",
        "label": "Combat Placeholder Assets",
        "requiredEvidence": ["runtime screenshot", "contact sheet review aid"],
        "nonClaim": "not production art",
    },
]

TWO_D_ONBOARDING_VIEWS: list[dict[str, Any]] = [
    {
        "id": "two_d_initial",
        "label": "2D Đông Môn initial HUD/map read",
        "screenshot": "01-initial.bmp",
        "requiredManifest": ["status", "screenshotCount", "hudSnapshot", "runtimeTilemapSnapshot", "runtimeDongMonTilePaletteSnapshot", "runtimeDongMonTilePaletteSourceSnapshot", "runtimeDongMonAuthoredPassSnapshot"],
        "nonClaim": "not production art",
    },
    {
        "id": "two_d_gate_focus",
        "label": "2D Gate Keeper focus",
        "screenshot": "02-gate-focus.bmp",
        "requiredManifest": ["runtimeRouteProgressSnapshot", "runtimeCharacterBaseSnapshot"],
        "nonClaim": "not production social hub",
    },
    {
        "id": "two_d_skill_ready",
        "label": "2D Shadow Slime / skill ready",
        "screenshot": "07-skill-ready.bmp",
        "requiredManifest": ["runtimeCombatSnapshot", "runtimeAnimationSnapshot"],
        "nonClaim": "not production combat",
    },
    {
        "id": "two_d_inventory_try",
        "label": "2D inventory try-on preview",
        "screenshot": "09-inventory-try.bmp",
        "requiredManifest": ["runtimeInventoryTryOnSnapshot", "runtimeInventoryInputSnapshot"],
        "nonClaim": "not production inventory economy",
    },
    {
        "id": "two_d_inventory_applied",
        "label": "2D inventory apply state",
        "screenshot": "10-inventory-applied.bmp",
        "requiredManifest": ["runtimeInventoryInputSnapshot", "runtimeEquipmentSnapshot"],
        "nonClaim": "not persistent equipment save",
    },

    {
        "id": "two_d_plaza_transition_preview",
        "label": "2D East Gate to Plaza route preview",
        "screenshot": "14-plaza-transition-preview.bmp",
        "requiredManifest": ["runtimeHubTransitionSnapshot", "runtimeLinhThanhAcademyShellSnapshot", "runtimeLinhThanhMarketShellSnapshot", "runtimeLinhThanhSpiritTempleShellSnapshot", "runtimeLinhThanhResidentialShellSnapshot", "runtimeLinhThanhForgeShellSnapshot", "runtimeLinhThanhGuildShellSnapshot", "runtimeLinhThanhHarborShellSnapshot", "hudSnapshot"],
        "nonClaim": "not production teleport backend",
    },
    {
        "id": "two_d_plaza_board_preview",
        "label": "2D Plaza board local preview",
        "screenshot": "11-plaza-board-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhPlazaHubSnapshot", "hudSnapshot"],
        "nonClaim": "not production event backend",
    },
    {
        "id": "two_d_plaza_target_selector",
        "label": "2D Plaza target selector input",
        "screenshot": "12-plaza-target-selector.bmp",
        "requiredManifest": ["runtimePlazaHubInputSnapshot", "runtimePlazaReadabilitySnapshot", "hudSnapshot"],
        "nonClaim": "not production social interaction system",
    },
    {
        "id": "two_d_plaza_npc_preview",
        "label": "2D Plaza NPC merchant local preview",
        "screenshot": "13-plaza-npc-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhPlazaHubSnapshot", "runtimePlazaHubInputSnapshot", "runtimePlazaReadabilitySnapshot", "hudSnapshot"],
        "nonClaim": "not production shop backend",
    },
]


def all_views() -> list[dict[str, Any]]:
    return LEGACY_VIEWS + TWO_D_ONBOARDING_VIEWS


def verify_current() -> dict[str, Any]:
    if not MANIFEST.is_file():
        return {
            "status": "UNVERIFIED_ENVIRONMENT",
            "reason": "visual manifest missing",
            "manifest": str(MANIFEST.relative_to(ROOT)),
            "markers": [],
        }
    try:
        manifest = json.loads(MANIFEST.read_text(encoding="utf-8", errors="replace"))
    except json.JSONDecodeError as exc:
        return {"status": "FAIL", "reason": f"invalid manifest json: {exc}", "markers": []}

    failures: list[str] = []
    if manifest.get("status") != "PASS":
        failures.append(f"manifest status expected PASS got {manifest.get('status')!r}")
    if manifest.get("finalStep") != "Complete":
        failures.append(f"finalStep expected Complete got {manifest.get('finalStep')!r}")
    screenshot_count = manifest.get("screenshotCount")
    if not isinstance(screenshot_count, (int, float)) or screenshot_count < len(TWO_D_ONBOARDING_VIEWS):
        failures.append(f"screenshotCount expected at least {len(TWO_D_ONBOARDING_VIEWS)} got {screenshot_count!r}")
    tilemap_snapshot = str(manifest.get("runtimeTilemapSnapshot", ""))
    for token in ("Chapter 1 Tilemap", "ChunkFlow", "chunk_gate_entry", "chunk_slime_arena"):
        if token not in tilemap_snapshot:
            failures.append(f"runtimeTilemapSnapshot missing {token!r}")
    palette_snapshot = str(manifest.get("runtimeDongMonTilePaletteSnapshot", ""))
    for token in ("DongMonTilePalette", "tile_ground_grass:earth-green:soft-grass-edge", "tile_dash_lane:spirit-cyan:wind-streak", "safe-no-source-image"):
        if token not in palette_snapshot:
            failures.append(f"runtimeDongMonTilePaletteSnapshot missing {token!r}")
    source_snapshot = str(manifest.get("runtimeDongMonTilePaletteSourceSnapshot", ""))
    for token in ("DongMonTilePaletteSource", "resource=LGOMaps/DongMonTilePalette", "tile_dash_lane=True", "safe-no-source-image=True", "safe-runtime-resource=True"):
        if token not in source_snapshot:
            failures.append(f"runtimeDongMonTilePaletteSourceSnapshot missing {token!r}")
    authored_snapshot = str(manifest.get("runtimeDongMonAuthoredPassSnapshot", ""))
    for token in ("DongMonAuthoredPass", "route-segments=5", "detail-density=readable", "collision-boundaries=from-bands", "no-random-decoration"):
        if token not in authored_snapshot:
            failures.append(f"runtimeDongMonAuthoredPassSnapshot missing {token!r}")

    view_results: list[dict[str, Any]] = []
    for view in TWO_D_ONBOARDING_VIEWS:
        screenshot = VISUAL_DIR / str(view["screenshot"])
        missing_fields = [field for field in view["requiredManifest"] if not str(manifest.get(field, ""))]
        exists = screenshot.is_file()
        if not exists:
            failures.append(f"{view['id']} missing screenshot {view['screenshot']}")
        if missing_fields:
            failures.append(f"{view['id']} missing manifest fields {', '.join(missing_fields)}")
        view_results.append(
            {
                "id": view["id"],
                "screenshot": str(screenshot.relative_to(ROOT)),
                "screenshotExists": exists,
                "missingManifestFields": missing_fields,
                "nonClaim": view["nonClaim"],
            }
        )

    markers = [] if failures else ["LGO_VISUAL_EVIDENCE_MATRIX_2D_CURRENT_PASS"]
    return {
        "status": "FAIL" if failures else "PASS",
        "reason": "; ".join(failures) if failures else "current 2D visual evidence manifest and screenshots verified",
        "manifest": str(MANIFEST.relative_to(ROOT)),
        "viewResults": view_results,
        "markers": markers,
    }


def main() -> int:
    parser = argparse.ArgumentParser(description="List or verify the Linh Gioi visual evidence matrix.")
    parser.add_argument("--json", action="store_true")
    parser.add_argument("--verify-current", action="store_true")
    args = parser.parse_args()
    if args.verify_current:
        payload = verify_current()
        if args.json:
            print(json.dumps(payload, indent=2, sort_keys=True, ensure_ascii=False))
        else:
            for view in payload.get("viewResults", []):
                print(f"{view['id']}: {view['screenshot']} exists={view['screenshotExists']} [{view['nonClaim']}]")
            for marker in payload.get("markers", []):
                print(marker)
        return 0 if payload["status"] == "PASS" else 1

    payload = {
        "marker": "LGO_VISUAL_EVIDENCE_MATRIX_READY",
        "views": all_views(),
        "visualGate": ["./tools/lgo_playable_closure_check.sh", "--visual-evidence"],
        "current2DVerify": ["python3.12", "tools/lgo_visual_evidence_matrix.py", "--verify-current"],
    }
    if args.json:
        print(json.dumps(payload, indent=2, sort_keys=True, ensure_ascii=False))
    else:
        for view in payload["views"]:
            print(f"{view['id']}: {view['label']} [{view['nonClaim']}]")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
