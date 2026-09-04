#!/usr/bin/env python3
from __future__ import annotations

import csv
import hashlib
import json
import shutil
import struct
import uuid
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PACK = Path("/Users/minhdc/Projects/LGO-ArtPacks/LGO-ART-V2-SEPARATED-RUNTIME-ASSETS")
DOC_ROOT = ROOT / "docs/reference-art/v2"
UNITY_ROOT = ROOT / "client/Unity/Assets/Game/Art/Runtime/V2/Resources/LGOArtV2"


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def png_size(path: Path) -> tuple[int, int]:
    with path.open("rb") as handle:
        header = handle.read(24)
    if header[:8] != b"\x89PNG\r\n\x1a\n" or header[12:16] != b"IHDR":
        raise ValueError(f"not a PNG: {path}")
    return struct.unpack(">II", header[16:24])


def guid_for(rel: str) -> str:
    return uuid.uuid5(uuid.NAMESPACE_URL, "linhgioi-art-v2:" + rel).hex


def unity_rel_for(pack_rel: str) -> str:
    rel = pack_rel.removeprefix("images/runtime-ready/")
    top, rest = rel.split("/", 1)
    top_map = {
        "login": "Login",
        "ui": "UI",
        "combat": "Combat",
        "world": "World",
        "vfx": "VFX",
    }
    return f"{top_map[top]}/{rest}"


def write_unity_meta(path: Path, unity_rel: str) -> None:
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
  maxTextureSize: 2048
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
  compressionQuality: 50
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
  userData: 
  assetBundleName: 
  assetBundleVariant: 
""",
        encoding="utf-8",
    )


def copy_tree(src: Path, dst: Path) -> None:
    for path in src.rglob("*"):
        if not path.is_file() or path.name == ".DS_Store":
            continue
        rel = path.relative_to(src)
        target = dst / rel
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(path, target)


def main() -> int:
    manifest_csv = PACK / "metadata/runtime-assets-v2-manifest.csv"
    if not manifest_csv.is_file():
        raise SystemExit("missing v2 manifest csv")

    copy_tree(PACK / "images/reference-only", DOC_ROOT / "reference-only")
    copy_tree(PACK / "images/runtime-ready", DOC_ROOT / "runtime-ready")
    copy_tree(PACK / "metadata", DOC_ROOT / "metadata")

    imported: list[dict[str, str]] = []
    with manifest_csv.open("r", encoding="utf-8", newline="") as handle:
        for row in csv.DictReader(handle):
            pack_rel = row["path"]
            source = PACK / pack_rel
            docs_target = ROOT / "docs/reference-art/v2" / pack_rel.removeprefix("images/")
            if not source.is_file():
                raise SystemExit(f"missing source asset: {pack_rel}")
            if sha256(source) != row["sha256"]:
                raise SystemExit(f"sha mismatch: {pack_rel}")
            width, height = png_size(source)
            if (str(width), str(height)) != (row["width"], row["height"]):
                raise SystemExit(f"dimension mismatch: {pack_rel}")
            unity_rel = unity_rel_for(pack_rel)
            unity_target = UNITY_ROOT / unity_rel
            unity_target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, unity_target)
            write_unity_meta(unity_target, unity_rel)
            imported.append({
                "source_path": pack_rel,
                "docs_path": docs_target.relative_to(ROOT).as_posix(),
                "unity_path": unity_target.relative_to(ROOT).as_posix(),
                "width": str(width),
                "height": str(height),
                "sha256": row["sha256"],
                "usage": row["usage"],
                "notes": row["notes"],
            })

    mapping = DOC_ROOT / "metadata/runtime-assets-v2-unity-import-mapping.csv"
    mapping.parent.mkdir(parents=True, exist_ok=True)
    with mapping.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(imported[0].keys()))
        writer.writeheader()
        writer.writerows(imported)
    (DOC_ROOT / "metadata/runtime-assets-v2-unity-import-mapping.json").write_text(
        json.dumps(imported, indent=2, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )
    print(f"LGO_ART_V2_INGEST_IMPORT_PASS assets={len(imported)} mapping={mapping}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
