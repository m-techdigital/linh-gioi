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
        "requiredManifest": ["status", "screenshotCount", "hudSnapshot", "runtimeTilemapSnapshot", "runtimeDongMonUnityTilemapSnapshot", "runtimeDongMonReadabilitySnapshot", "runtimeMinimapReadabilitySnapshot", "runtimeDongMonTilePaletteSnapshot", "runtimeDongMonTilePaletteSourceSnapshot", "runtimeDongMonChunkPlacementSourceSnapshot", "runtimeDongMonAuthoredDetailSourceSnapshot", "runtimeDongMonNpcSpriteSourceSnapshot", "runtimeDongMonPlayerSceneFitSnapshot", "runtimeDongMonAuthoredPassSnapshot"],
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
        "requiredManifest": ["runtimeCombatSnapshot", "runtimeAnimationSnapshot", "runtimeVoLv1PaperDollAtlasSnapshot"],
        "nonClaim": "not production combat",
    },
    {
        "id": "two_d_vo_motion_complete",
        "label": "2D Võ Lv1 paper-doll motion complete",
        "screenshot": "08-complete.bmp",
        "requiredManifest": ["runtimeVoLv1ClassSliceSnapshot", "runtimeVoLv1PaperDollAtlasSnapshot", "runtimeVoLv1LevelBandFunctionProbeSnapshot", "runtimeAnimationSnapshot"],
        "nonClaim": "not production class art",
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
        "id": "two_d_linh_thanh_district_preview",
        "label": "2D Linh Thanh district preview rail",
        "screenshot": "15-district-preview-rail.bmp",
        "requiredManifest": ["runtimeLinhThanhDistrictPreviewSnapshot", "runtimeLinhThanhDistrictDetailSnapshot", "runtimeLinhThanhAcademyShellSnapshot", "runtimeLinhThanhMarketShellSnapshot", "hudSnapshot"],
        "nonClaim": "not production district travel backend",
    },
    {
        "id": "two_d_linh_thanh_market_preview",
        "label": "2D Linh Thanh market district preview",
        "screenshot": "16-district-market-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhDistrictPreviewSnapshot", "runtimeLinhThanhDistrictDetailSnapshot", "runtimeLinhThanhMarketShellSnapshot", "hudSnapshot"],
        "nonClaim": "not production market economy backend",
    },
    {
        "id": "two_d_linh_thanh_spirit_temple_preview",
        "label": "2D Linh Thanh spirit temple district preview",
        "screenshot": "17-district-spirit-temple-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhDistrictPreviewSnapshot", "runtimeLinhThanhDistrictDetailSnapshot", "runtimeLinhThanhSpiritTempleShellSnapshot", "hudSnapshot"],
        "nonClaim": "not production blessing or story backend",
    },
    {
        "id": "two_d_linh_thanh_forge_preview",
        "label": "2D Linh Thanh forge district preview",
        "screenshot": "18-district-forge-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhDistrictPreviewSnapshot", "runtimeLinhThanhDistrictDetailSnapshot", "runtimeLinhThanhForgeShellSnapshot", "hudSnapshot"],
        "nonClaim": "not production crafting backend",
    },
    {
        "id": "two_d_linh_thanh_guild_preview",
        "label": "2D Linh Thanh guild district preview",
        "screenshot": "19-district-guild-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhDistrictPreviewSnapshot", "runtimeLinhThanhDistrictDetailSnapshot", "runtimeLinhThanhGuildShellSnapshot", "hudSnapshot"],
        "nonClaim": "not production guild backend",
    },
    {
        "id": "two_d_linh_thanh_harbor_preview",
        "label": "2D Linh Thanh harbor district preview",
        "screenshot": "20-district-harbor-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhDistrictPreviewSnapshot", "runtimeLinhThanhDistrictDetailSnapshot", "runtimeLinhThanhDistrictReadabilitySnapshot", "runtimeLinhThanhHarborShellSnapshot", "hudSnapshot"],
        "nonClaim": "not production travel or teleport backend",
    },
    {
        "id": "two_d_plaza_board_preview",
        "label": "2D Plaza board local preview",
        "screenshot": "11-plaza-board-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhPlazaHubSnapshot", "runtimePlazaHubDetailSnapshot", "runtimePlazaHubLayoutSnapshot", "hudSnapshot"],
        "nonClaim": "not production event backend",
    },
    {
        "id": "two_d_plaza_target_selector",
        "label": "2D Plaza target selector input",
        "screenshot": "12-plaza-target-selector.bmp",
        "requiredManifest": ["runtimePlazaHubInputSnapshot", "runtimePlazaHubDetailSnapshot", "runtimePlazaHubLayoutSnapshot", "runtimePlazaReadabilitySnapshot", "hudSnapshot"],
        "nonClaim": "not production social interaction system",
    },
    {
        "id": "two_d_plaza_npc_preview",
        "label": "2D Plaza NPC merchant local preview",
        "screenshot": "13-plaza-npc-preview.bmp",
        "requiredManifest": ["runtimeLinhThanhPlazaHubSnapshot", "runtimePlazaHubInputSnapshot", "runtimePlazaHubDetailSnapshot", "runtimePlazaHubLayoutSnapshot", "runtimePlazaReadabilitySnapshot", "hudSnapshot"],
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
    unity_tilemap_snapshot = str(manifest.get("runtimeDongMonUnityTilemapSnapshot", ""))
    for token in ("DongMonUnityTilemap", "renderer=TilemapRenderer", "grid=Grid", "source=LGOMaps/DongMonChunkPlacement", "cells=16", "safe-no-source-image", "safe-no-3d"):
        if token not in unity_tilemap_snapshot:
            failures.append(f"runtimeDongMonUnityTilemapSnapshot missing {token!r}")
    dong_mon_readability_snapshot = str(manifest.get("runtimeDongMonReadabilitySnapshot", ""))
    for token in ("DongMonReadability", "mode=route-label-rail", "world-label-density=reduced", "chips=gate,stone,jump,dash,slime", "avoids-hud-overlap", "safe-local-no-backend"):
        if token not in dong_mon_readability_snapshot:
            failures.append(f"runtimeDongMonReadabilitySnapshot missing {token!r}")
    minimap_readability_snapshot = str(manifest.get("runtimeMinimapReadabilitySnapshot", ""))
    for token in ("MinimapReadability", "mode=compact-district-route", "route-text=short", "district-chips=academy,market,spirit,forge,guild,harbor", "world-links=icon-only", "selected-node-progress", "avoids-hud-overlap", "safe-local-no-backend"):
        if token not in minimap_readability_snapshot:
            failures.append(f"runtimeMinimapReadabilitySnapshot missing {token!r}")
    district_preview_snapshot = str(manifest.get("runtimeLinhThanhDistrictPreviewSnapshot", ""))
    for token in ("DistrictPreviewRail", "unlocked=True", "selected=harbor", "label=Cảng Linh Thuyền", "route=plaza->harbor", "controls=M select-district", "safe-no-travel-backend", "safe-no-teleport-backend", "safe-no-district-backend", "safe-local-no-backend"):
        if token not in district_preview_snapshot:
            failures.append(f"runtimeLinhThanhDistrictPreviewSnapshot missing {token!r}")
    district_detail_snapshot = str(manifest.get("runtimeLinhThanhDistrictDetailSnapshot", ""))
    for token in ("DistrictDetail", "selected=harbor", "role=travel-preview", "detail=spirit-boat-locked", "next=world-route-gate", "safe-no-travel-backend", "safe-no-teleport-backend", "safe-no-district-backend", "safe-local-no-backend"):
        if token not in district_detail_snapshot:
            failures.append(f"runtimeLinhThanhDistrictDetailSnapshot missing {token!r}")

    district_readability_snapshot = str(manifest.get("runtimeLinhThanhDistrictReadabilitySnapshot", ""))
    for token in ("DistrictRailReadability", "mode=selected-node-callout", "selected=harbor", "label-follows-selected=True", "backplate=follows-selected", "callout-size=readable", "avoids-hud-overlap", "safe-local-no-backend"):
        if token not in district_readability_snapshot:
            failures.append(f"runtimeLinhThanhDistrictReadabilitySnapshot missing {token!r}")

    plaza_detail_snapshot = str(manifest.get("runtimePlazaHubDetailSnapshot", ""))
    for token in ("PlazaHubDetail", "selected=merchant-preview", "role=starter-gear-preview", "detail=try-before-shop", "safe-no-shop-backend", "safe-local-no-backend"):
        if token not in plaza_detail_snapshot:
            failures.append(f"runtimePlazaHubDetailSnapshot missing {token!r}")

    vo_class_snapshot = str(manifest.get("runtimeVoLv1ClassSliceSnapshot", ""))
    for token in ("compat=base_male_torso->OuterShirt", "anchorSet=Chest,Hips,Hand_L,Hand_R,Foot_L,Foot_R", "source=runtime-authored-json", "runtimeArtPolicy=replace-primitive-with-approved-spritesheet"):
        if token not in vo_class_snapshot:
            failures.append(f"runtimeVoLv1ClassSliceSnapshot missing {token!r}")

    inventory_input_snapshot = str(manifest.get("runtimeInventoryInputSnapshot", ""))
    for token in ("selectedCompat=True", "selectedSlot=OuterShirt", "selectedAnchor=Chest", "fitProfile=base_male_torso", "source=runtime-authored-catalog"):
        if token not in inventory_input_snapshot:
            failures.append(f"runtimeInventoryInputSnapshot missing {token!r}")

    plaza_layout_snapshot = str(manifest.get("runtimePlazaHubLayoutSnapshot", ""))
    for token in ("PlazaHubLayout", "anchors=5", "anchor=social-spawn@center", "anchor=event-board@upper-mid", "anchor=gate-guide@left", "anchor=merchant-preview@right", "anchor=guild-locked@far-right", "safe-local-no-backend"):
        if token not in plaza_layout_snapshot:
            failures.append(f"runtimePlazaHubLayoutSnapshot missing {token!r}")

    palette_snapshot = str(manifest.get("runtimeDongMonTilePaletteSnapshot", ""))
    for token in ("DongMonTilePalette", "tile_ground_grass:earth-green:soft-grass-edge", "tile_dash_lane:spirit-cyan:wind-streak", "safe-no-source-image"):
        if token not in palette_snapshot:
            failures.append(f"runtimeDongMonTilePaletteSnapshot missing {token!r}")
    source_snapshot = str(manifest.get("runtimeDongMonTilePaletteSourceSnapshot", ""))
    for token in ("DongMonTilePaletteSource", "resource=LGOMaps/DongMonTilePalette", "tile_dash_lane=True", "safe-no-source-image=True", "safe-runtime-resource=True"):
        if token not in source_snapshot:
            failures.append(f"runtimeDongMonTilePaletteSourceSnapshot missing {token!r}")
    animation_snapshot = str(manifest.get("runtimeAnimationSnapshot", ""))
    for token in ("motionClip=TrainingCompletePose", "motionFrame=vo_complete_01", "paperDollPose=vo_lv1_training_complete"):
        if token not in animation_snapshot:
            failures.append(f"runtimeAnimationSnapshot missing {token!r}")

    chunk_source_snapshot = str(manifest.get("runtimeDongMonChunkPlacementSourceSnapshot", ""))
    for token in ("DongMonChunkPlacementSource", "resource=LGOMaps/DongMonChunkPlacement", "chunk_gate_entry@-3.70,-2.02x4", "chunk_dash_lane@1.82,-1.02x4", "authored-placement=True", "safe-runtime-resource=True", "safe-no-3d=True"):
        if token not in chunk_source_snapshot:
            failures.append(f"runtimeDongMonChunkPlacementSourceSnapshot missing {token!r}")
    detail_source_snapshot = str(manifest.get("runtimeDongMonAuthoredDetailSourceSnapshot", ""))
    for token in ("DongMonAuthoredDetailSource", "resource=LGOMaps/DongMonAuthoredDetails", "details=7", "moss=True", "step=True", "rope=True", "spirit-dust=True", "rune=True", "authored-detail=True", "safe-runtime-resource=True", "safe-no-source-image=True", "safe-no-3d=True"):
        if token not in detail_source_snapshot:
            failures.append(f"runtimeDongMonAuthoredDetailSourceSnapshot missing {token!r}")
    npc_sprite_snapshot = str(manifest.get("runtimeDongMonNpcSpriteSourceSnapshot", ""))
    for token in ("DongMonNpcSpriteSource", "resource=LGOMaps/DongMonNpcSprites", "npcs=1", "gate_keeper_parts=13", "role=tutorial-guide", "silhouette=elder-robed-guardian-staff", "slots=robe,cloak,hat,staff,talisman", "authored-npc-sprite=True", "safe-runtime-resource=True", "safe-no-source-image=True", "safe-no-3d=True", "safe-local-no-backend=True"):
        if token not in npc_sprite_snapshot:
            failures.append(f"runtimeDongMonNpcSpriteSourceSnapshot missing {token!r}")
    player_scene_fit_snapshot = str(manifest.get("runtimeDongMonPlayerSceneFitSnapshot", ""))
    for token in ("DongMonPlayerSceneFit", "DongMonPlayerGroundingSource", "authored-player-grounding=True", "routeAnchor=shadow-slime lane=combat-lane y=-1.16 sortBand=combat-front", "visitedGrounding=shadow-slime lane=combat-lane playerSort=6 shadowSort=5", "safe-runtime-player-evidence=True"):
        if token not in player_scene_fit_snapshot:
            failures.append(f"runtimeDongMonPlayerSceneFitSnapshot missing {token!r}")

    authored_snapshot = str(manifest.get("runtimeDongMonAuthoredPassSnapshot", ""))
    for token in ("DongMonAuthoredPass", "route-segments=5", "detail-density=readable", "collision-boundaries=from-bands", "no-random-decoration"):
        if token not in authored_snapshot:
            failures.append(f"runtimeDongMonAuthoredPassSnapshot missing {token!r}")

    vo_paperdoll_snapshot = str(manifest.get("runtimeVoLv1PaperDollAtlasSnapshot", ""))
    for token in ("VoLv1PaperDollAtlas", "cellSource=runtime-generated-atlas-cell", "part=chest_panel cell=torso_outer_vo_lv1", "part=hand_wrap_r cell=glove_r_vo_lv1", "skillCue=vo_lv1_first_skill_trail cell=skill_vo_lv1_palm_trail", "approvedRuntimeArt=True", "VoLv1ApprovedRuntimeArt", "levelBand=1-30", "cells=11", "approvedCell=chest_panel cell=torso_outer_vo_lv1", "approvedCell=inner_shadow cell=base_torso_male", "approvedCell=pants_shadow_l cell=pants_vo_lv1", "approvedCell=pants_shadow_r cell=pants_vo_lv1", "approvedCell=hand_wrap_r cell=glove_r_vo_lv1", "approvedCell=vo_lv1_first_skill_trail cell=skill_vo_lv1_palm_trail",
            "approvedCell=sash_red_core cell=waist_vo_lv1",
            "approvedCell=weapon_staff cell=staff_vo_lv1", "productionAtlas=vo-lv1-starter-atlas-v1", "importMode=layered-psb-or-spritesheet", "texturePolicy=approved-original-2d-art-only", "requiredCell=torso_outer_vo_lv1", "requiredCell=skill_vo_lv1_palm_burst", "rigJoint=Hand_R", "motionClip=vo_lv1_first_skill", "replacementGate=preserve-runtime-fit-contract", "poseOffsets=", "pose=vo_jump_lift", "pose=vo_dash_stretch", "currentPose=vo_lv1_training_complete", "safe-runtime-motion=True", "active=True"):
        if token not in vo_paperdoll_snapshot:
            failures.append(f"runtimeVoLv1PaperDollAtlasSnapshot missing {token!r}")

    vo_anchor_gizmo_snapshot = str(manifest.get("runtimeVoLv1AnchorGizmoSnapshot", ""))
    for token in ("VoLv1AnchorGizmo", "visible=True", "slot=OuterShirt anchor=Chest", "slot=Gloves anchor=Hand_R", "slot=Boots anchor=Foot_L", "pivotPolicy=bottom-center-foot-anchor", "visibleWhen=inventory-or-class-training", "safe-runtime-gizmo=True", "safe-no-source-image=True", "safe-no-3d=True"):
        if token not in vo_anchor_gizmo_snapshot:
            failures.append(f"runtimeVoLv1AnchorGizmoSnapshot missing {token!r}")

    vo_function_probe_snapshot = str(manifest.get("runtimeVoLv1LevelBandFunctionProbeSnapshot", ""))
    for token in ("VoLv1LevelBandFunctionProbe", "classId=vo", "levelBand=1-30", "highTier31Plus=False", "paperDollSlots=OuterShirt,PantsOrSkirt,Waist,Gloves,Boots,Weapon", "tryOn=True", "applyEquipment=True", "motionStates=Idle,Jump,Dash,ClassSkill,TrainingCompletePose", "skill=vo_lv1_first_skill target=shadow-slime", "grounding=visited-shadow-slime", "safe-no-source-image=True", "safe-no-3d=True"):
        if token not in vo_function_probe_snapshot:
            failures.append(f"runtimeVoLv1LevelBandFunctionProbeSnapshot missing {token!r}")

    vo_runtime_fit_snapshot = str(manifest.get("runtimeVoLv1RuntimeFitSnapshot", ""))
    for token in ("VoLv1RuntimeFit", "fitStatus=ANCHOR_ALIGNED", "slot=OuterShirt item=top_vo_lv1_male anchor=Chest pivot=bottom-center", "slot=Gloves item=gloves_vo_lv1_unisex anchor=Hand_R", "slot=Boots item=boots_vo_lv1_unisex anchor=Foot_L", "skill=vo_lv1_first_skill anchor=Hand_R", "scenePlane=dong-mon-gameplay-plane", "runtimeCheck=slot-bounds-follow-paperdoll-pose", "safe-runtime-fit=True", "safe-no-source-image=True", "safe-no-3d=True"):
        if token not in vo_runtime_fit_snapshot:
            failures.append(f"runtimeVoLv1RuntimeFitSnapshot missing {token!r}")

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
