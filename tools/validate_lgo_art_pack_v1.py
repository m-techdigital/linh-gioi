#!/usr/bin/env python3
from __future__ import annotations

import csv
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MARKERS = (
    "LGO_ART_V1_REFERENCE_ONLY_ACCEPTED",
    "LGO_ART_V1_EXPERIMENTAL_SLICE_REVIEW_REQUIRED",
    "MISSING_RUNTIME_ASSET",
)
FROZEN_PREFIXES = (
    "protocol/",
    "gamedata/schemas/",
    "docs/adr/",
    "client/Unity/Assets/Game/UI/design-tokens.json",
)
FORBIDDEN_UNITY_NAMES = {
    "01-LGO-visual-direction-board-v1.png",
    "02-LGO-screen-mockup-pack-v1.png",
    "03-LGO-ui-asset-sheet-v1.png",
    "04-LGO-world-combat-asset-sheet-v1.png",
}
REQUIRED_EXPERIMENTAL_ASSETS = (
    "ui/panels/panel_large_main.png",
    "ui/buttons/button_blue_normal.png",
    "ui/skill-icons/icon_skill_wind_slash.png",
    "ui/cooldown/cooldown_ring_ready.png",
    "ui/markers/target_selected_blue.png",
    "world/npc/gate_keeper_npc_full.png",
    "world/gate/spirit_gate_full.png",
    "world/training-stone/training_stone_full.png",
    "world/dummy/dummy_idle.png",
    "world/dummy/dummy_selected.png",
    "world/dummy/dummy_hit.png",
    "world/vfx/wind_slash_frame_01.png",
    "world/vfx/impact_spark_large.png",
    "world/monsters/shadow_slime_alt.png",
)


def fail(message: str) -> None:
    print(f"LGO ART PACK V1 VALIDATION FAILED: {message}", file=sys.stderr)
    raise SystemExit(1)


def read(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        fail(f"missing file: {rel}")
    return path.read_text(encoding="utf-8", errors="replace")


def git_status_lines() -> list[str]:
    result = subprocess.run(
        ["git", "--no-pager", "status", "--short", "--untracked-files=all"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        fail(result.stderr.strip() or "git status failed")
    return result.stdout.splitlines()


def check_docs() -> None:
    docs = [
        "docs/art/v1/README.md",
        "docs/art/v1/IMAGE-CLASSIFICATION.md",
        "docs/art/v1/ASSET-MAPPING.md",
        "docs/art/v1/CODEX-USAGE.md",
        "docs/art/v1/IMPORT-BOUNDARY.md",
        "docs/art/v1/ASSET-QUALITY-REVIEW.md",
        "docs/art/v1/RUNTIME-ASSET-BOUNDARY.md",
        "docs/art/v1/ART-V2-SEPARATED-ASSET-REQUIREMENTS.md",
        "docs/reference-art/v1/runtime-asset-pack/INDEX.md",
        "docs/reference-art/v1/runtime-asset-pack/IMPORT-NOTES.md",
    ]
    combined = "\n".join(read(rel) for rel in docs)
    for marker in MARKERS:
        if marker not in combined:
            fail(f"missing decision marker: {marker}")


def check_mapping() -> None:
    mapping = ROOT / "docs/reference-art/v1/runtime-asset-pack/MAPPING.csv"
    if not mapping.is_file():
        fail("missing runtime asset mapping csv")
    with mapping.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))
    if len(rows) < 41:
        fail(f"expected at least 41 experimental slice records, found {len(rows)}")
    outputs = {row["output_file"] for row in rows}
    for rel in REQUIRED_EXPERIMENTAL_ASSETS:
        if rel not in outputs:
            fail(f"mapping missing expected experimental slice: {rel}")
        doc_asset = ROOT / "docs/reference-art/v1/runtime-asset-pack" / rel
        if not doc_asset.is_file():
            fail(f"missing experimental docs slice: {doc_asset.relative_to(ROOT)}")


def check_unity_boundaries() -> None:
    unity_root = ROOT / "client/Unity/Assets/Game/Art/V1"
    if unity_root.exists():
        for path in unity_root.rglob("*.png"):
            if path.name in FORBIDDEN_UNITY_NAMES:
                fail(f"forbidden unsliced/reference image imported to Unity: {path.relative_to(ROOT)}")
            fail(f"experimental Art v1 slice imported to Unity runtime path: {path.relative_to(ROOT)}")
    if (ROOT / "client/Unity/Assets/Game/Art/Runtime/ArtPackV1Assets.cs").exists():
        fail("experimental ArtPackV1Assets runtime loader must not exist")
    combat = read("client/Unity/Assets/Game/Art/Runtime/CombatPlaceholderAssets.cs")
    for marker in ("Version = \"0.46.0\"", "ResourceRoot = \"CombatPlaceholders/\"", "target-dummy-idle-v0450"):
        if marker not in combat:
            fail(f"CombatPlaceholderAssets missing marker: {marker}")
    if "ArtPackV1Assets" in combat:
        fail("CombatPlaceholderAssets must not reference experimental ArtPackV1Assets")


def check_frozen_surface() -> None:
    for line in git_status_lines():
        rel = line[3:] if len(line) >= 4 else line
        if rel.startswith(FROZEN_PREFIXES):
            fail(f"frozen surface changed: {rel}")


def main() -> int:
    check_docs()
    check_mapping()
    check_unity_boundaries()
    check_frozen_surface()
    print("LGO_ART_PACK_V1_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
