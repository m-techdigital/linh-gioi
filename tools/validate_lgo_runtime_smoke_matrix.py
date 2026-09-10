#!/usr/bin/env python3
from __future__ import annotations

import os
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


def check_executable(rel: str) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing executable: {rel}")
    elif not os.access(path, os.X_OK):
        ERRORS.append(f"not executable: {rel}")


def check_list_output() -> None:
    result = subprocess.run(["python3.12", "tools/lgo_runtime_smoke_matrix.py", "--phase", "source", "--list"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "matrix list failed")
        return
    for marker in ("package_hygiene", "continuous_mode", "playable_source", "playable_package_ready"):
        if marker not in result.stdout:
            ERRORS.append(f"matrix list missing gate: {marker}")
    two_d = subprocess.run(["python3.12", "tools/lgo_runtime_smoke_matrix.py", "--phase", "two-d", "--list", "--json"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if two_d.returncode != 0:
        ERRORS.append(two_d.stderr.strip() or "two-d matrix list failed")
        return
    for marker in ("two_d_onboarding_smoke", "two_d_player_build", "two_d_visual_capture", "runtimeMapSnapshot", "WorldMapNetwork", "LinhThanhHubRuntime", "HubShell", "district=plaza", "district=academy", "PlazaShell", "AcademyShell", "MarketShell", "SpiritTempleShell", "ResidentialShell", "ForgeShell", "GuildShell", "HarborShell", "safe-no-trade-backend", "safe-no-skill-backend", "safe-no-economy-backend", "safe-no-buff-backend", "safe-no-housing-backend", "safe-no-crafting-backend", "safe-no-guild-backend", "safe-no-travel-backend", "runtimeLinhThanhUnlockSnapshot", "LinhThanhUnlock", "unlock=plaza", "runtimeLinhThanhPlazaHubSnapshot", "PlazaHubRuntime", "runtimeLinhThanhAcademyShellSnapshot",
        "runtimeLinhThanhMarketShellSnapshot", "runtimeLinhThanhSpiritTempleShellSnapshot", "runtimeLinhThanhResidentialShellSnapshot", "runtimeLinhThanhForgeShellSnapshot", "runtimeLinhThanhGuildShellSnapshot", "runtimeLinhThanhHarborShellSnapshot", "npc=merchant-preview", "board=event-local-preview", "interaction=npc-merchant-preview", "safe-local-no-shop-backend", "runtimePlazaHubInputSnapshot", "PlazaHubInput", "layout=spaced-social-triangle", "runtimePlazaHubDetailSnapshot", "PlazaHubDetail", "detail=try-before-shop", "runtimePlazaHubLayoutSnapshot", "PlazaHubLayout", "anchor=social-spawn@center", "anchor=event-board@upper-mid", "anchor=merchant-preview@right", "anchor=guild-locked@far-right", "runtimePlazaReadabilitySnapshot", "PlazaReadability", "mode=label-rail", "world-label-density=reduced", "runtimeDongMonUnityTilemapSnapshot", "DongMonUnityTilemap", "renderer=TilemapRenderer", "grid=Grid",
        "controls=P select, E interact", "runtimeTilemapSnapshot", "ChunkFlow", "runtimeDongMonTilePaletteSnapshot", "DongMonTilePalette", "tile_dash_lane:spirit-cyan:wind-streak", "runtimeDongMonTilePaletteSourceSnapshot", "DongMonTilePaletteSource", "resource=LGOMaps/DongMonTilePalette", "runtimeDongMonAuthoredDetailSourceSnapshot", "DongMonAuthoredDetailSource", "resource=LGOMaps/DongMonAuthoredDetails", "details=7", "authored-detail=True", "runtimeDongMonNpcSpriteSourceSnapshot", "DongMonNpcSpriteSource", "resource=LGOMaps/DongMonNpcSprites", "gate_keeper_parts=13", "authored-npc-sprite=True", "runtimeDongMonInteractionMarkerSourceSnapshot", "DongMonInteractionMarkerSource", "resource=LGOMaps/DongMonInteractionMarkers", "markers=5", "route=gatekeeper,training-stone,jump,dash,shadow-slime", "runtimeDongMonPlayerSceneFitSnapshot", "DongMonPlayerSceneFit", "footAnchor=bottom-center", "safe-runtime-player-evidence=True", "runtimeVoLv1ClassSliceSnapshot", "VoLv1ClassSlice", "resource=LGOClasses/VoLv1ClassSlice", "classId=vo", "slots=5", "ClassSkill:vo_lv1_first_skill", "top=top_vo_lv1_male", "runtimeSkillCue=True", "runtimeVoLv1PaperDollAtlasSnapshot", "VoLv1PaperDollAtlas", "resource=LGOClasses/VoLv1PaperDollAtlas", "slot=OuterShirt", "anchor=Chest", "skillCue=vo_lv1_first_skill_trail", "poseOffsets=", "pose=vo_jump_lift", "pose=vo_dash_stretch", "currentPose=vo_lv1_training_complete", "active=True", "pose=vo_lv1_training_complete", "safe-runtime-motion=True", "runtimeAnimationSnapshot", "motionClip=TrainingCompletePose", "motionFrame=vo_complete_01", "paperDollPose=vo_lv1_training_complete", "runtimeDongMonAuthoredPassSnapshot", "DongMonAuthoredPass", "detail-density=readable", "runtimeMinimapReadabilitySnapshot", "MinimapReadability", "mode=compact-district-route", "route-text=short", "district-chips=academy,market,spirit,forge,guild,harbor", "world-links=icon-only", "selected-node-progress", "safe-local-no-backend"):
        if marker not in two_d.stdout:
            ERRORS.append(f"two-d matrix list missing: {marker}")


def main() -> int:
    require(
        "docs/tasks/LGO-RUNTIME-SMOKE-MATRIX-v1.0.md",
        "LGO_RUNTIME_SMOKE_MATRIX_READY",
        "No gameplay implementation",
        "validate_lgo_runtime_smoke_matrix.py",
    )
    require(
        "docs/execution/LGO-RUNTIME-SMOKE-MATRIX-v1.0.md",
        "LGO_RUNTIME_SMOKE_MATRIX_READY",
        "LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS",
        "2D Onboarding Gates",
        "two_d_onboarding_smoke",
        "runtimeMapSnapshot",
        "runtimeLinhThanhUnlockSnapshot",
        "LinhThanhUnlock",
        "unlock=plaza",
        "runtimeLinhThanhPlazaHubSnapshot",
        "PlazaHubRuntime",
        "runtimeLinhThanhAcademyShellSnapshot",
        "npc=merchant-preview",
        "board=event-local-preview",
        "interaction=npc-merchant-preview",
        "safe-local-no-shop-backend",
        "WorldMapNetwork",
        "LinhThanhHubRuntime",
        "HubShell",
        "district=plaza",
        "district=academy",
        "PlazaShell",
        "AcademyShell",
        "MarketShell",
        "SpiritTempleShell",
        "ResidentialShell",
        "ForgeShell",
        "GuildShell",
        "HarborShell",
        "safe-no-trade-backend",
        "safe-no-skill-backend",
        "safe-no-economy-backend",
        "safe-no-buff-backend",
        "safe-no-housing-backend",
        "safe-no-crafting-backend",
        "safe-no-guild-backend",
        "safe-no-travel-backend",
        "runtimeTilemapSnapshot",
        "ChunkFlow",
        "runtimeDongMonTilePaletteSnapshot",
        "DongMonTilePalette",
        "runtimeDongMonTilePaletteSourceSnapshot",
        "DongMonTilePaletteSource",
        "resource=LGOMaps/DongMonTilePalette",
        "runtimeDongMonAuthoredPassSnapshot",
        "runtimeLinhThanhDistrictDetailSnapshot",
        "role=starter-commerce-preview",
        "detail=vendor-row-only",
        "16-district-market-preview",
        "17-district-spirit-temple-preview",
        "18-district-forge-preview",
        "19-district-guild-preview",
        "20-district-harbor-preview",
        "runtimeLinhThanhDistrictPreviewSnapshot",
        "runtimeLinhThanhDistrictReadabilitySnapshot",
        "DistrictRailReadability",
        "mode=selected-node-callout",
        "label-follows-selected=True",
        "backplate=follows-selected",
        "callout-size=readable",
        "runtimeMinimapReadabilitySnapshot",
        "MinimapReadability",
        "mode=compact-district-route",
        "route-text=short",
        "district-chips=academy,market,spirit,forge,guild,harbor",
        "world-links=icon-only",
        "selected-node-progress",
        "safe-local-no-backend",
        "avoids-hud-overlap",
        "DistrictPreviewRail",
        "selected=academy",
        "selected=spirit-temple",
        "selected=harbor",
        "label=Đền Linh",
        "label=Cảng Linh Thuyền",
        "route=plaza->academy",
        "route=plaza->spirit-temple",
        "route=plaza->harbor",
        "role=story-blessing-preview",
        "role=travel-preview",
        "detail=altar-local-only",
        "detail=spirit-boat-locked",
        "next=quest-buff-gate",
        "next=world-route-gate",
        "safe-no-teleport-backend",
        "safe-no-district-backend",
        "runtimePlazaHubDetailSnapshot",
        "PlazaHubDetail",
        "detail=try-before-shop",
        "runtimePlazaHubLayoutSnapshot",
        "PlazaHubLayout",
        "anchor=social-spawn@center",
        "anchor=event-board@upper-mid",
        "anchor=merchant-preview@right",
        "anchor=guild-locked@far-right",
        "runtimeDongMonUnityTilemapSnapshot",
        "DongMonUnityTilemap",
        "renderer=TilemapRenderer",
        "grid=Grid",
        "DongMonAuthoredPass",
        "detail-density=readable",
        "LGO_RUNTIME_SMOKE_MATRIX_2D_PASS",
        "UNVERIFIED_ENVIRONMENT",
        "Do not mask failures",
    )
    require(
        "tools/lgo_runtime_smoke_matrix.py",
        "SOURCE_GATES",
        "RUNTIME_GATES",
        "LGO_RUNTIME_SMOKE_MATRIX_RUN_PASS",
        "LGO_RUNTIME_SMOKE_MATRIX_2D_PASS",
        "TWO_D_GATES",
        "runtimeMapSnapshot",
        "runtimeLinhThanhUnlockSnapshot",
        "LinhThanhUnlock",
        "unlock=plaza",
        "runtimeLinhThanhPlazaHubSnapshot",
        "PlazaHubRuntime",
        "runtimeLinhThanhAcademyShellSnapshot",
        "npc=merchant-preview",
        "board=event-local-preview",
        "interaction=npc-merchant-preview",
        "safe-local-no-shop-backend",
        "WorldMapNetwork",
        "LinhThanhHubRuntime",
        "HubShell",
        "district=plaza",
        "district=academy",
        "PlazaShell",
        "AcademyShell",
        "MarketShell",
        "SpiritTempleShell",
        "ResidentialShell",
        "ForgeShell",
        "GuildShell",
        "HarborShell",
        "safe-no-trade-backend",
        "safe-no-skill-backend",
        "safe-no-economy-backend",
        "safe-no-buff-backend",
        "safe-no-housing-backend",
        "safe-no-crafting-backend",
        "safe-no-guild-backend",
        "safe-no-travel-backend",
        "runtimeTilemapSnapshot",
        "ChunkFlow",
        "runtimeDongMonTilePaletteSnapshot",
        "DongMonTilePalette",
        "runtimeDongMonTilePaletteSourceSnapshot",
        "DongMonTilePaletteSource",
        "resource=LGOMaps/DongMonTilePalette",
        "runtimeDongMonAuthoredDetailSourceSnapshot",
        "DongMonAuthoredDetailSource",
        "resource=LGOMaps/DongMonAuthoredDetails",
        "details=7",
        "authored-detail=True",
        "runtimeDongMonNpcSpriteSourceSnapshot",
        "DongMonNpcSpriteSource",
        "resource=LGOMaps/DongMonNpcSprites",
        "gate_keeper_parts=13",
        "authored-npc-sprite=True",
        "runtimeDongMonInteractionMarkerSourceSnapshot", "DongMonInteractionMarkerSource", "resource=LGOMaps/DongMonInteractionMarkers", "markers=5", "route=gatekeeper,training-stone,jump,dash,shadow-slime", "runtimeDongMonPlayerSceneFitSnapshot", "DongMonPlayerSceneFit", "footAnchor=bottom-center", "safe-runtime-player-evidence=True", "runtimeVoLv1ClassSliceSnapshot", "VoLv1ClassSlice", "resource=LGOClasses/VoLv1ClassSlice", "classId=vo", "slots=5", "ClassSkill:vo_lv1_first_skill", "top=top_vo_lv1_male", "runtimeSkillCue=True", "runtimeVoLv1PaperDollAtlasSnapshot", "VoLv1PaperDollAtlas", "resource=LGOClasses/VoLv1PaperDollAtlas", "slot=OuterShirt", "anchor=Chest", "skillCue=vo_lv1_first_skill_trail", "poseOffsets=", "pose=vo_jump_lift", "pose=vo_dash_stretch", "currentPose=vo_lv1_training_complete", "active=True", "pose=vo_lv1_training_complete", "safe-runtime-motion=True", "runtimeAnimationSnapshot", "motionClip=TrainingCompletePose", "motionFrame=vo_complete_01", "paperDollPose=vo_lv1_training_complete", "runtimeDongMonAuthoredPassSnapshot",
        "DongMonAuthoredPass",
        "detail-density=readable",
        "runtimeMinimapReadabilitySnapshot",
        "MinimapReadability",
        "mode=compact-district-route",
        "route-text=short",
        "district-chips=academy,market,spirit,forge,guild,harbor",
        "world-links=icon-only",
        "selected-node-progress",
        "safe-local-no-backend",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_lgo_runtime_smoke_matrix.py",
    )
    check_executable("tools/lgo_runtime_smoke_matrix.py")
    check_executable("tools/validate_lgo_runtime_smoke_matrix.py")
    check_list_output()
    if ERRORS:
        print("LGO RUNTIME SMOKE MATRIX VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_SMOKE_MATRIX_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
