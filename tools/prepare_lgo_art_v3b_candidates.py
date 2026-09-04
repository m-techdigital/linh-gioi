#!/usr/bin/env python3
from __future__ import annotations

import csv
import hashlib
import json
import shutil
import struct
import subprocess
import uuid
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DOC_ROOT = ROOT / "docs/reference-art/v3b/runtime-candidates"
UNITY_ROOT = ROOT / "client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B"
MANIFEST_CSV = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"
MANIFEST_JSON = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.json"

CANDIDATES = (
    {
        "role": "login_background",
        "source": "login/login_background_spirit_gate_1920x1080_v3b_candidate.png",
        "unity": "Login/login_background_spirit_gate_1920x1080_v3b_candidate.jpg",
        "min_width": 1920,
        "min_height": 1080,
        "alpha_required": "no",
        "runtime_max_texture_size": 2048,
        "runtime_source_size": 1920,
        "usage": "Login/Gate Entry background candidate; no baked UI/text.",
    },
    {
        "role": "login_panel",
        "source": "login/panel_main_dark_gold_v3b_candidate.png",
        "unity": "Login/panel_main_dark_gold_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 512,
        "alpha_required": "yes",
        "runtime_max_texture_size": 512,
        "runtime_source_size": 384,
        "usage": "Login/Gate Entry panel skin candidate; human edge QA still required.",
    },
    {
        "role": "enter_world_button",
        "source": "login/button_enter_world_gold_v3b_candidate.png",
        "unity": "Login/button_enter_world_gold_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 256,
        "alpha_required": "yes",
        "runtime_max_texture_size": 512,
        "runtime_source_size": 384,
        "usage": "Primary enter-world button skin candidate; player-facing Vietnamese text remains Unity-rendered.",
    },
    {
        "role": "gate_keeper_npc_login",
        "source": "login/gate_keeper_npc_login_v3b_candidate.png",
        "unity": "Login/gate_keeper_npc_login_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 1024,
        "alpha_required": "yes",
        "runtime_max_texture_size": 512,
        "runtime_source_size": 384,
        "usage": "High-resolution login Gate Keeper NPC candidate; transparent cutout, not production-final.",
    },
    {
        "role": "world_spirit_gate",
        "source": "world/gate/spirit_gate_v3b_candidate.png",
        "unity": "World/gate/spirit_gate_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 1024,
        "alpha_required": "yes",
        "runtime_max_texture_size": 512,
        "runtime_source_size": 384,
        "usage": "High-resolution world Spirit Gate candidate; transparent prop sprite, not production-final.",
    },
    {
        "role": "world_training_stone",
        "source": "world/training-stone/training_stone_v3b_candidate.png",
        "unity": "World/training-stone/training_stone_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 1024,
        "alpha_required": "yes",
        "runtime_max_texture_size": 512,
        "runtime_source_size": 384,
        "usage": "High-resolution Training Stone candidate; transparent interactable prop sprite, not production-final.",
    },
    {
        "role": "vfx_wind_slash_frame_01",
        "source": "vfx/wind-slash/wind_slash_frame_01_v3b_candidate.png",
        "unity": "VFX/wind-slash/wind_slash_frame_01_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 1024,
        "alpha_required": "yes",
        "runtime_max_texture_size": 256,
        "runtime_source_size": 192,
        "usage": "High-resolution Wind Slash VFX candidate; local feedback only, not production-final.",
    },
    {
        "role": "vfx_impact_spark",
        "source": "vfx/impact/impact_spark_v3b_candidate.png",
        "unity": "VFX/impact/impact_spark_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 1024,
        "alpha_required": "yes",
        "runtime_max_texture_size": 256,
        "runtime_source_size": 192,
        "usage": "High-resolution impact spark VFX candidate; local feedback only, not production-final.",
    },
    {
        "role": "combat_cooldown_ready",
        "source": "combat/cooldown/cooldown_ready_v3b_candidate.png",
        "unity": "Combat/cooldown/cooldown_ready_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 1024,
        "alpha_required": "yes",
        "runtime_max_texture_size": 128,
        "runtime_source_size": 128,
        "usage": "High-resolution cooldown ready ring candidate; no baked text/numbers, not production-final.",
    },
    {
        "role": "combat_cooldown_active",
        "source": "combat/cooldown/cooldown_active_v3b_candidate.png",
        "unity": "Combat/cooldown/cooldown_active_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 1024,
        "alpha_required": "yes",
        "runtime_max_texture_size": 128,
        "runtime_source_size": 128,
        "usage": "High-resolution cooldown active ring candidate; no baked text/numbers, not production-final.",
    },
    {
        "role": "combat_target_dummy_idle",
        "source": "combat/target-dummy/target_dummy_idle_v3b_candidate.png",
        "unity": "Combat/target-dummy/target_dummy_idle_v3b_candidate.png",
        "min_width": 1024,
        "min_height": 1024,
        "alpha_required": "yes",
        "runtime_max_texture_size": 512,
        "runtime_source_size": 384,
        "usage": "High-resolution target dummy idle candidate; selected/hit/recover still require clean separated V3B state assets.",
    },
)


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def png_header(path: Path) -> tuple[int, int, int]:
    with path.open("rb") as handle:
        header = handle.read(26)
    if header[:8] != b"\x89PNG\r\n\x1a\n" or header[12:16] != b"IHDR":
        raise ValueError(f"not a PNG: {path}")
    width, height = struct.unpack(">II", header[16:24])
    color_type = header[25]
    return width, height, color_type


def jpeg_size(path: Path) -> tuple[int, int]:
    data = path.read_bytes()
    if data[:2] != b"\xff\xd8":
        raise ValueError(f"not a JPEG: {path}")
    i = 2
    while i < len(data) - 9:
        if data[i] != 0xFF:
            i += 1
            continue
        marker = data[i + 1]
        i += 2
        if marker in (0xD8, 0xD9):
            continue
        segment_length = int.from_bytes(data[i:i + 2], "big")
        if 0xC0 <= marker <= 0xC3:
            height = int.from_bytes(data[i + 3:i + 5], "big")
            width = int.from_bytes(data[i + 5:i + 7], "big")
            return width, height
        i += segment_length
    raise ValueError(f"JPEG size not found: {path}")


def image_header(path: Path) -> tuple[int, int, int]:
    if path.suffix.lower() in {".jpg", ".jpeg"}:
        width, height = jpeg_size(path)
        return width, height, 2
    return png_header(path)


def prepare_unity_runtime_copy(source: Path, target: Path, max_side: int) -> None:
    target.parent.mkdir(parents=True, exist_ok=True)
    if target.suffix.lower() in {".jpg", ".jpeg"}:
        result = subprocess.run(
            ["sips", "-s", "format", "jpeg", "-s", "formatOptions", "55", str(source), "--out", str(target)],
            cwd=ROOT,
            text=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            check=False,
        )
        if result.returncode != 0:
            raise SystemExit(result.stderr.strip() or f"failed to create jpeg runtime asset: {target}")
        return
    shutil.copy2(source, target)
    width, height, _ = png_header(target)
    if max(width, height) <= max_side:
        return
    result = subprocess.run(
        ["sips", "-Z", str(max_side), str(target)],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        raise SystemExit(result.stderr.strip() or f"failed to downscale runtime asset: {target}")


def guid_for(rel: str) -> str:
    return uuid.uuid5(uuid.NAMESPACE_URL, "linhgioi-art-v3b:" + rel).hex


def write_unity_meta(path: Path, unity_rel: str, max_texture_size: int) -> None:
    meta = path.with_suffix(path.suffix + ".meta")
    meta.write_text(
        f"""fileFormatVersion: 2
guid: {guid_for(unity_rel)}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: {max_texture_size}
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 80
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 0
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings: []
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: 5e97eb03825dee720800000000000000
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
  spritePackingTag: 
  pSDRemoveMatte: 0
  pSDShowRemoveMatteOption: 0
  userData: LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL
  assetBundleName: 
  assetBundleVariant: 
""",
        encoding="utf-8",
    )


def write_folder_meta(path: Path) -> None:
    if path == UNITY_ROOT.parent or not path.is_dir():
        return
    rel = path.relative_to(UNITY_ROOT.parent).as_posix()
    meta = path.with_suffix(".meta")
    if meta.is_file():
        return
    meta.write_text(
        f"""fileFormatVersion: 2
guid: {guid_for(rel + '/')}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
""",
        encoding="utf-8",
    )


def main() -> int:
    MANIFEST_CSV.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for candidate in CANDIDATES:
        doc_path = DOC_ROOT / candidate["source"]
        unity_path = UNITY_ROOT / candidate["unity"]
        if not doc_path.is_file():
            raise SystemExit(f"missing candidate source: {doc_path}")
        width, height, color_type = png_header(doc_path)
        if width < int(candidate["min_width"]) or height < int(candidate["min_height"]):
            raise SystemExit(f"candidate too small: {doc_path} {width}x{height}")
        if candidate["alpha_required"] == "yes" and color_type not in (4, 6):
            raise SystemExit(f"candidate missing PNG alpha channel: {doc_path}")
        prepare_unity_runtime_copy(doc_path, unity_path, int(candidate["runtime_source_size"]))
        runtime_width, runtime_height, runtime_color_type = image_header(unity_path)
        if candidate["alpha_required"] == "yes" and runtime_color_type not in (4, 6):
            raise SystemExit(f"Unity runtime asset missing PNG alpha channel: {unity_path}")
        for parent in reversed(unity_path.parents):
            if UNITY_ROOT.parent in parent.parents or parent == UNITY_ROOT.parent:
                write_folder_meta(parent)
        max_texture_size = int(candidate["runtime_max_texture_size"])
        write_unity_meta(unity_path, candidate["unity"], max_texture_size)
        rows.append(
            {
                "role": candidate["role"],
                "docs_path": str(doc_path.relative_to(ROOT)),
                "unity_path": str(unity_path.relative_to(ROOT)),
                "width": width,
                "height": height,
                "runtime_width": runtime_width,
                "runtime_height": runtime_height,
                "color_type": color_type,
                "alpha_required": candidate["alpha_required"],
                "runtime_max_texture_size": max_texture_size,
                "runtime_source_size": int(candidate["runtime_source_size"]),
                "sha256": sha256(doc_path),
                "unity_sha256": sha256(unity_path),
                "classification": "LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL",
                "usage": candidate["usage"],
            }
        )
    with MANIFEST_CSV.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(rows[0].keys()), lineterminator="\n")
        writer.writeheader()
        writer.writerows(rows)
    MANIFEST_JSON.write_text(json.dumps(rows, indent=2) + "\n", encoding="utf-8")
    print(f"LGO_ART_V3B_CANDIDATE_PREPARED assets={len(rows)} manifest={MANIFEST_CSV.relative_to(ROOT)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
