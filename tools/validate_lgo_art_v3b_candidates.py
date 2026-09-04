#!/usr/bin/env python3
from __future__ import annotations

import csv
import hashlib
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"
UNITY_ROOT = ROOT / "client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B"
FROZEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FROZEN_EXACT = {"client/Unity/Assets/Game/UI/design-tokens.json"}
REQUIRED_DOCS = (
    "docs/art/v3/ART-V3B-SEPARATED-RUNTIME-ASSET-REQUIREMENTS.md",
    "docs/art/v3/ART-V3B-RUNTIME-CANDIDATE-QUALITY-REVIEW.md",
)
REQUIRED_CODE_MARKERS = (
    "LgoVisualAssetRegistryV3B",
    "LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL",
    "LoginBackgroundSpiritGate",
    "GateKeeperNpcLoginTexture",
    "ButtonEnterWorldGoldTexture",
    "SpiritGate",
    "TrainingStone",
    "TreeCherry",
    "TreePine",
    "LanternProp",
    "RockMoss",
    "BannerCultivation",
    "BridgeWood",
    "ShadowSlime",
    "WindSlashFrame01",
    "ImpactSpark",
    "CooldownReady",
    "CooldownActive",
    "TargetDummyIdle",
    "TargetDummySelected",
    "TargetDummyHit",
    "TargetDummyRecover",
)


def fail(message: str) -> None:
    print(f"LGO ART V3B CANDIDATES VALIDATION FAILED: {message}", file=sys.stderr)
    raise SystemExit(1)


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


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


def read(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        fail(f"missing file: {rel}")
    return path.read_text(encoding="utf-8", errors="replace")


def check_manifest() -> None:
    if not MANIFEST.is_file():
        fail("missing V3B runtime candidate manifest")
    with MANIFEST.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))
    if len(rows) < 3:
        fail(f"expected at least 3 V3B candidates, found {len(rows)}")
    for row in rows:
        for key in ("docs_path", "unity_path", "width", "height", "runtime_width", "runtime_height", "alpha_required", "runtime_max_texture_size", "runtime_source_size", "sha256", "unity_sha256", "classification"):
            if not row.get(key):
                fail(f"manifest row missing {key}: {row}")
        if row["classification"] != "LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL":
            fail(f"wrong classification: {row}")
        min_width = 512 if row["role"] == "login_logo" or row["role"].startswith("combat_target_dummy_") or row["role"].startswith("world_tree_") or row["role"] in ("world_lantern_prop", "world_rock_moss", "world_bridge_wood", "world_banner_cultivation", "world_shadow_slime") else 1024
        if int(row["width"]) < min_width or int(row["height"]) < 256:
            fail(f"candidate below minimum visual target size: {row}")
        runtime_max = int(row["runtime_max_texture_size"])
        runtime_source_size = int(row["runtime_source_size"])
        runtime_width = int(row["runtime_width"])
        runtime_height = int(row["runtime_height"])
        if runtime_max not in (128, 256, 512, 1024, 2048):
            fail(f"unsupported runtime texture budget: {row}")
        if runtime_source_size not in (128, 192, 224, 256, 288, 384, 480, 512, 768, 1024, 1920):
            fail(f"unsupported runtime source size: {row}")
        if max(runtime_width, runtime_height) > runtime_source_size:
            fail(f"Unity runtime source exceeds role size: {row}")
        if row["role"] == "login_background" and runtime_max > 2048:
            fail(f"login background runtime texture budget too large: {row}")
        if row["role"] != "login_background" and runtime_max > 1024:
            fail(f"non-background runtime texture budget too large: {row}")
        if "cooldown" in row["role"] and runtime_max > 512:
            fail(f"cooldown runtime texture budget too large: {row}")
        docs = ROOT / row["docs_path"]
        unity = ROOT / row["unity_path"]
        if not docs.is_file() or not unity.is_file():
            fail(f"missing candidate pair: {row}")
        if sha256(unity) != row["unity_sha256"]:
            fail(f"Unity runtime sha mismatch: {row}")
        meta = unity.with_suffix(unity.suffix + ".meta")
        if not meta.is_file():
            fail(f"missing Unity meta: {meta.relative_to(ROOT)}")
        meta_text = meta.read_text(encoding="utf-8", errors="replace")
        for marker in ("TextureImporter", "spriteMode: 1", "textureType: 8", f"maxTextureSize: {runtime_max}", "LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL"):
            if marker not in meta_text:
                fail(f"meta missing marker {marker}: {meta.relative_to(ROOT)}")


def check_no_reference_posters() -> None:
    forbidden_names = {
        "01-login-gate-entry-high-res-target-reference.png",
        "02-high-res-runtime-asset-catalog-reference.png",
        "03-high-res-asset-pack-v3-reference.png",
    }
    for path in list(UNITY_ROOT.rglob("*.png")) + list(UNITY_ROOT.rglob("*.jpg")):
        if path.name in forbidden_names or "reference" in path.name or "contact-sheet" in path.name:
            fail(f"reference/poster imported into V3B runtime path: {path.relative_to(ROOT)}")


def check_docs_and_code() -> None:
    combined_docs = "\n".join(read(rel) for rel in REQUIRED_DOCS)
    for marker in ("LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL", "not production art", "not final visual quality"):
        if marker not in combined_docs:
            fail(f"missing docs marker: {marker}")
    registry = read("client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3B.cs")
    ui = read("client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs")
    combined_code = registry + "\n" + ui
    for marker in REQUIRED_CODE_MARKERS:
        if marker not in combined_code:
            fail(f"missing code marker: {marker}")


def check_frozen_status() -> None:
    for line in git_status_lines():
        rel = line[3:] if len(line) >= 4 else line
        if rel in FROZEN_EXACT or rel.startswith(FROZEN_PREFIXES):
            fail(f"frozen surface changed: {rel}")


def main() -> int:
    check_manifest()
    check_no_reference_posters()
    check_docs_and_code()
    check_frozen_status()
    print("LGO_ART_V3B_CANDIDATES_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
