#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import subprocess
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]

SOURCE_GATES: list[dict[str, Any]] = [
    {
        "id": "package_hygiene",
        "kind": "command",
        "command": ["python3.12", "tools/validate_package_hygiene.py"],
        "marker": "PACKAGE HYGIENE VALIDATION PASS",
    },
    {
        "id": "continuous_mode",
        "kind": "command",
        "command": ["python3.12", "tools/validate_lgo_continuous_development_mode.py"],
        "marker": "LGO_CONTINUOUS_DEVELOPMENT_MODE_VALIDATION_PASS",
    },
    {
        "id": "playable_source",
        "kind": "command",
        "command": ["./tools/lgo_playable_closure_check.sh", "--source-only"],
        "marker": "LGO_PLAYABLE_CLOSURE_SOURCE_GATES_PASS",
    },
    {
        "id": "playable_package_ready",
        "kind": "command",
        "command": ["./tools/lgo_playable_closure_check.sh", "--package-ready"],
        "marker": "LGO_PLAYABLE_CLOSURE_PACKAGE_READY",
    },
]

RUNTIME_GATES: list[dict[str, Any]] = [
    {
        "id": "playable_runtime",
        "kind": "command",
        "command": ["./tools/lgo_playable_closure_check.sh", "--runtime"],
        "marker": "LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS",
    }
]

TWO_D_GATES: list[dict[str, Any]] = [
    {
        "id": "two_d_onboarding_smoke",
        "kind": "json_artifact",
        "path": "build/2d-onboarding/twod-onboarding-smoke.json",
        "requirements": {"status": "PASS", "finalStep": "Complete", "linhThanhUnlocked": True},
        "marker": "LGO_2D_ONBOARDING_SMOKE_PASS",
    },
    {
        "id": "two_d_player_build",
        "kind": "text_artifact",
        "path": "build/2d-onboarding-player/build-macos-player.log",
        "markers": ["LGO_MACOS_PLAYER_BUILD result=Succeeded", "errors=0", "Build Finished, Result: Success"],
        "marker": "LGO_MACOS_PLAYER_BUILD result=Succeeded",
    },
    {
        "id": "two_d_visual_capture",
        "kind": "json_artifact",
        "path": "build/2d-onboarding-visual/twod-onboarding-visual-manifest.json",
        "requirements": {"status": "PASS", "finalStep": "Complete"},
        "minimums": {"screenshotCount": 16},
        "contains": {
            "runtimeMapSnapshot": ["WorldMapNetwork: hub=linh-thanh", "LinhThanhHubRuntime:", "HubShell: linh-thanh", "district=plaza", "district=academy", "district=market", "PlazaShell: district=plaza", "AcademyShell: district=academy", "MarketShell: district=market", "SpiritTempleShell: district=spirit-temple", "ResidentialShell: district=residential", "ForgeShell: district=forge", "GuildShell: district=guild", "HarborShell: district=harbor", "safe-no-trade-backend", "safe-no-skill-backend", "safe-no-economy-backend", "safe-no-buff-backend", "safe-no-housing-backend", "safe-no-crafting-backend", "safe-no-guild-backend", "safe-no-travel-backend"],
            "runtimeLinhThanhUnlockSnapshot": ["LinhThanhUnlock", "unlocked=True", "unlock=plaza", "safe-local-no-teleport"],
            "runtimeLinhThanhPlazaHubSnapshot": ["PlazaHubRuntime", "unlocked=True", "npc=gate-guide", "npc=merchant-preview", "board=event-local-preview", "interaction=npc-merchant-preview", "safe-local-no-backend", "safe-local-no-shop-backend"],
            "runtimeLinhThanhDistrictPreviewSnapshot": ["DistrictPreviewRail", "unlocked=True", "selected=market", "label=Thương Phố", "route=plaza->market", "controls=M select-district", "safe-no-trade-backend", "safe-no-district-backend", "safe-local-no-backend"],
            "runtimeLinhThanhDistrictDetailSnapshot": ["DistrictDetail", "selected=market", "role=starter-commerce-preview", "detail=vendor-row-only", "safe-no-trade-backend", "safe-no-economy-backend", "safe-no-district-backend", "safe-local-no-backend"],
            "runtimeLinhThanhAcademyShellSnapshot": ["AcademyShell: district=academy", "skill-hall=preview-only", "class-trainer=locked", "safe-no-skill-backend", "safe-local-no-backend"],
            "runtimeLinhThanhMarketShellSnapshot": ["MarketShell: district=market", "vendor-row=preview-only", "auction-board=locked", "safe-no-trade-backend", "safe-no-economy-backend", "safe-local-no-backend"],
            "runtimeLinhThanhSpiritTempleShellSnapshot": ["SpiritTempleShell: district=spirit-temple", "blessing-altar=preview-only", "story-shrine=locked", "safe-no-buff-backend", "safe-local-no-backend"],
            "runtimeLinhThanhResidentialShellSnapshot": ["ResidentialShell: district=residential", "npc-home-row=preview-only", "social-chat-node=locked", "safe-no-housing-backend", "safe-local-no-backend"],
            "runtimeLinhThanhForgeShellSnapshot": ["ForgeShell: district=forge", "anvil-row=preview-only", "craft-board=locked", "safe-no-crafting-backend", "safe-local-no-backend"],
            "runtimeLinhThanhGuildShellSnapshot": ["GuildShell: district=guild", "guild-hall=preview-only", "guild-banner=local-preview", "safe-no-guild-backend", "safe-local-no-backend"],
            "runtimeLinhThanhHarborShellSnapshot": ["HarborShell: district=harbor", "spirit-boat=preview-only", "travel-board=locked", "safe-no-travel-backend", "safe-local-no-backend"],
            "runtimeHubTransitionSnapshot": ["HubTransition", "unlocked=True", "from=east-gate", "to=plaza", "mode=local-route-preview", "safe-local-no-teleport-backend"],
            "runtimeTilemapSnapshot": ["ChunkFlow", "chunk_gate_entry", "chunk_slime_arena"],
            "runtimeDongMonUnityTilemapSnapshot": ["DongMonUnityTilemap", "renderer=TilemapRenderer", "grid=Grid", "source=LGOMaps/DongMonChunkPlacement", "cells=16", "safe-no-source-image", "safe-no-3d"],
            "runtimeDongMonTilePaletteSnapshot": ["DongMonTilePalette", "tile_ground_grass:earth-green:soft-grass-edge", "tile_dash_lane:spirit-cyan:wind-streak", "tile_slime_arena:violet-corruption:rune-boundary", "safe-no-source-image"],
            "runtimeDongMonTilePaletteSourceSnapshot": ["DongMonTilePaletteSource", "resource=LGOMaps/DongMonTilePalette", "tile_dash_lane=True", "safe-no-source-image=True", "safe-runtime-resource=True"],
            "runtimeDongMonChunkPlacementSourceSnapshot": ["DongMonChunkPlacementSource", "resource=LGOMaps/DongMonChunkPlacement", "chunk_gate_entry@-3.70,-2.02x4", "chunk_dash_lane@1.82,-1.02x4", "authored-placement=True", "safe-runtime-resource=True", "safe-no-3d=True"],
            "runtimeDongMonAuthoredPassSnapshot": ["DongMonAuthoredPass", "route-segments=5", "detail-density=readable", "collision-boundaries=from-bands", "no-random-decoration"],
            "runtimeInventoryInputSnapshot": ["InventoryInputState=Applied"],
            "runtimePlazaHubInputSnapshot": ["PlazaHubInput", "selected=merchant-preview", "layout=spaced-social-triangle", "controls=P select, E interact", "interaction=npc-merchant-preview", "safe-local-no-shop-backend"],
            "runtimePlazaHubDetailSnapshot": ["PlazaHubDetail", "selected=merchant-preview", "role=starter-gear-preview", "detail=try-before-shop", "safe-no-shop-backend", "safe-local-no-backend"],
            "runtimePlazaHubLayoutSnapshot": ["PlazaHubLayout", "anchors=5", "anchor=social-spawn@center", "anchor=event-board@upper-mid", "anchor=gate-guide@left", "anchor=merchant-preview@right", "anchor=guild-locked@far-right", "safe-local-no-backend"],
            "runtimePlazaReadabilitySnapshot": ["PlazaReadability", "mode=label-rail", "world-label-density=reduced", "target-chips=event-board,gate-guide,merchant-preview", "safe-local-no-backend"],
            "runtimeDongMonReadabilitySnapshot": ["DongMonReadability", "mode=route-label-rail", "world-label-density=reduced", "chips=gate,stone,jump,dash,slime", "avoids-hud-overlap", "safe-local-no-backend"],
        },
        "marker": "LGO_RUNTIME_SMOKE_MATRIX_2D_PASS",
    },
]


def run_command_gate(gate: dict[str, Any]) -> dict[str, Any]:
    command = list(gate["command"])
    result = subprocess.run(command, cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    output = result.stdout or ""
    marker = str(gate["marker"])
    observed = marker in output
    return {
        "id": gate["id"],
        "kind": gate.get("kind", "command"),
        "command": command,
        "returnCode": result.returncode,
        "marker": marker,
        "markerObserved": observed,
        "status": "PASS" if result.returncode == 0 and observed else "FAIL",
    }


def run_json_artifact_gate(gate: dict[str, Any]) -> dict[str, Any]:
    path = ROOT / str(gate["path"])
    result: dict[str, Any] = {
        "id": gate["id"],
        "kind": gate["kind"],
        "path": str(gate["path"]),
        "marker": gate["marker"],
    }
    if not path.is_file():
        result.update({"status": "UNVERIFIED_ENVIRONMENT", "reason": "artifact missing"})
        return result
    try:
        payload = json.loads(path.read_text(encoding="utf-8", errors="replace"))
    except json.JSONDecodeError as exc:
        result.update({"status": "FAIL", "reason": f"invalid json: {exc}"})
        return result
    failures: list[str] = []
    for key, expected in gate.get("requirements", {}).items():
        if payload.get(key) != expected:
            failures.append(f"{key} expected {expected!r} got {payload.get(key)!r}")
    for key, minimum in gate.get("minimums", {}).items():
        value = payload.get(key)
        if not isinstance(value, (int, float)) or value < minimum:
            failures.append(f"{key} expected >= {minimum!r} got {value!r}")
    for key, tokens in gate.get("contains", {}).items():
        value = str(payload.get(key, ""))
        for token in tokens:
            if token not in value:
                failures.append(f"{key} missing token {token!r}")
    result["observed"] = {key: payload.get(key) for key in sorted(set(gate.get("requirements", {})) | set(gate.get("minimums", {})) | set(gate.get("contains", {})))}
    if failures:
        result.update({"status": "FAIL", "reason": "; ".join(failures)})
    else:
        result.update({"status": "PASS", "markerObserved": True})
    return result


def run_text_artifact_gate(gate: dict[str, Any]) -> dict[str, Any]:
    path = ROOT / str(gate["path"])
    result: dict[str, Any] = {
        "id": gate["id"],
        "kind": gate["kind"],
        "path": str(gate["path"]),
        "marker": gate["marker"],
    }
    if not path.is_file():
        result.update({"status": "UNVERIFIED_ENVIRONMENT", "reason": "artifact missing"})
        return result
    text = path.read_text(encoding="utf-8", errors="replace")
    missing = [marker for marker in gate.get("markers", []) if marker not in text]
    if missing:
        result.update({"status": "FAIL", "reason": "missing markers: " + ", ".join(missing), "markerObserved": False})
    else:
        result.update({"status": "PASS", "markerObserved": True})
    return result


def run_gate(gate: dict[str, Any]) -> dict[str, Any]:
    kind = gate.get("kind", "command")
    if kind == "command":
        return run_command_gate(gate)
    if kind == "json_artifact":
        return run_json_artifact_gate(gate)
    if kind == "text_artifact":
        return run_text_artifact_gate(gate)
    return {"id": gate.get("id", "unknown"), "kind": kind, "status": "FAIL", "reason": "unknown gate kind"}


def selected_gates(phase: str) -> list[dict[str, Any]]:
    gates: list[dict[str, Any]] = []
    if phase in ("source", "all"):
        gates.extend(SOURCE_GATES)
    if phase in ("runtime", "all"):
        gates.extend(RUNTIME_GATES)
    if phase in ("two-d", "all"):
        gates.extend(TWO_D_GATES)
    return gates


def main() -> int:
    parser = argparse.ArgumentParser(description="List or run the Linh Gioi runtime smoke matrix.")
    parser.add_argument("--phase", choices=("source", "runtime", "two-d", "all"), default="source")
    parser.add_argument("--list", action="store_true")
    parser.add_argument("--json", action="store_true")
    args = parser.parse_args()

    gates = selected_gates(args.phase)

    if args.list:
        payload = {"phase": args.phase, "gates": gates}
        print(json.dumps(payload, indent=2, sort_keys=True) if args.json else "\n".join(f"{gate['id']}: {' '.join(gate.get('command', [gate.get('path', '')]))}" for gate in gates))
        return 0

    results = [run_gate(gate) for gate in gates]
    print(json.dumps({"phase": args.phase, "results": results}, indent=2, sort_keys=True, ensure_ascii=False))
    if any(result["status"] != "PASS" for result in results):
        return 1
    if args.phase == "two-d":
        print("LGO_RUNTIME_SMOKE_MATRIX_2D_PASS")
    print("LGO_RUNTIME_SMOKE_MATRIX_RUN_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
