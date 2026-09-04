#!/usr/bin/env python3
from __future__ import annotations

import csv
import hashlib
import uuid
import shutil
import struct
import zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PACK = Path("/Users/minhdc/Projects/LGO-ArtPacks/LGO-ART-PACK-v1.1")
DOC_ROOT = ROOT / "docs" / "reference-art" / "v1"
UNITY_ROOT = None
PNG_SIG = b"\x89PNG\r\n\x1a\n"


ASSETS = [
    ("03-LGO-ui-asset-sheet-v1.png", "ui/panels/panel_large_main.png", (35, 110, 380, 330), "UI panel", "9-slice candidate"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/panels/panel_medium.png", (410, 125, 710, 320), "UI panel", "9-slice candidate"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/panels/label_bar_long.png", (720, 105, 1065, 170), "UI label", "sprite single"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/buttons/button_blue_normal.png", (1130, 80, 1430, 160), "UI button", "text on sheet is ignored by runtime"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/buttons/button_gold_pressed.png", (1130, 170, 1430, 255), "UI button", "text on sheet is ignored by runtime"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/buttons/button_disabled.png", (1130, 270, 1430, 355), "UI button", "text on sheet is ignored by runtime"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/skill-icons/icon_skill_wind_slash.png", (45, 360, 250, 585), "Skill icon", "placeholder"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/skill-icons/icon_skill_shadow_bind.png", (265, 360, 475, 585), "Skill icon", "placeholder"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/skill-icons/icon_skill_spirit_guard.png", (500, 360, 710, 585), "Skill icon", "placeholder"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/cooldown/cooldown_ring_ready.png", (780, 365, 925, 555), "Cooldown", "ready"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/cooldown/cooldown_ring_half.png", (965, 365, 1110, 555), "Cooldown", "half"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/cooldown/cooldown_ring_full.png", (1185, 365, 1335, 555), "Cooldown", "full"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/feedback/feedback_damage.png", (35, 595, 205, 735), "Feedback", "damage"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/feedback/feedback_critical.png", (220, 590, 395, 735), "Feedback", "critical"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/feedback/feedback_block.png", (425, 600, 575, 735), "Feedback", "block"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/feedback/feedback_heal.png", (615, 600, 775, 735), "Feedback", "heal"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/markers/target_marker_red.png", (795, 590, 960, 790), "Target marker", "warning/hostile"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/markers/target_selected_blue.png", (990, 585, 1165, 790), "Target marker", "selected"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/markers/warning_telegraph_red.png", (1195, 590, 1500, 790), "Warning telegraph", "ground warning"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/hud/quest_hint_panel.png", (50, 800, 625, 955), "Quest hint", "contains reference English text; runtime copy remains Vietnamese"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/hud/hud_avatar_frame.png", (660, 805, 770, 935), "HUD avatar", "placeholder"),
    ("03-LGO-ui-asset-sheet-v1.png", "ui/hud/hud_hp_mp_bar.png", (740, 815, 1120, 930), "HUD bars", "placeholder"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/character/character_reference_full.png", (25, 105, 245, 535), "World character", "reference-quality placeholder"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/npc/gate_keeper_npc_full.png", (265, 65, 590, 550), "World NPC", "placeholder"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/gate/spirit_gate_full.png", (610, 80, 1000, 550), "World gate", "placeholder landmark"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/training-stone/training_stone_full.png", (1030, 120, 1285, 550), "World training stone", "placeholder landmark"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/dummy/dummy_idle.png", (35, 580, 225, 895), "Combat dummy", "idle"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/dummy/dummy_selected.png", (250, 545, 515, 900), "Combat dummy", "selected"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/dummy/dummy_hit.png", (535, 570, 760, 900), "Combat dummy", "hit"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/monsters/shadow_slime_alt.png", (1300, 425, 1515, 590), "Monster marker", "visual-only"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/vfx/wind_slash_large.png", (775, 585, 1280, 875), "VFX", "large slash"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/vfx/wind_slash_frame_01.png", (0, 930, 170, 1105), "VFX frame", "sequence 1"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/vfx/wind_slash_frame_02.png", (185, 930, 365, 1110), "VFX frame", "sequence 2"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/vfx/wind_slash_frame_03.png", (380, 930, 565, 1110), "VFX frame", "sequence 3"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/vfx/wind_slash_frame_04.png", (580, 930, 795, 1110), "VFX frame", "sequence 4"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/vfx/impact_spark_large.png", (1290, 605, 1600, 905), "VFX", "impact"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/props/spirit_orb.png", (805, 955, 965, 1125), "Prop", "spirit orb"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/props/spirit_orb_variant.png", (970, 970, 1115, 1125), "Prop", "orb variant"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/markers/selector_ring_blue.png", (1080, 930, 1260, 1125), "Selector ring", "blue"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/markers/selector_ring_gold.png", (250, 545, 515, 900), "Selector ring", "gold with dummy context"),
    ("04-LGO-world-combat-asset-sheet-v1.png", "world/props/lantern_prop.png", (1280, 890, 1510, 1150), "Prop", "lantern"),
]


def read_png(path: Path) -> tuple[int, int, bytes]:
    data = path.read_bytes()
    if not data.startswith(PNG_SIG):
        raise ValueError(f"not a png: {path}")
    pos = len(PNG_SIG)
    width = height = None
    color = None
    rows = b""
    while pos < len(data):
        length = struct.unpack(">I", data[pos:pos + 4])[0]
        kind = data[pos + 4:pos + 8]
        payload = data[pos + 8:pos + 8 + length]
        pos += 12 + length
        if kind == b"IHDR":
            width, height, bit_depth, color, compression, filter_method, interlace = struct.unpack(">IIBBBBB", payload)
            if bit_depth != 8 or color != 6 or compression != 0 or filter_method != 0 or interlace != 0:
                raise ValueError(f"unsupported PNG format: {path}")
        elif kind == b"IDAT":
            rows += payload
        elif kind == b"IEND":
            break
    if width is None or height is None:
        raise ValueError(f"missing IHDR: {path}")
    raw = zlib.decompress(rows)
    stride = width * 4
    out = bytearray(height * stride)
    prev = bytearray(stride)
    i = 0
    for y in range(height):
        filter_type = raw[i]
        i += 1
        cur = bytearray(raw[i:i + stride])
        i += stride
        for x in range(stride):
            left = cur[x - 4] if x >= 4 else 0
            up = prev[x]
            up_left = prev[x - 4] if x >= 4 else 0
            if filter_type == 1:
                cur[x] = (cur[x] + left) & 0xFF
            elif filter_type == 2:
                cur[x] = (cur[x] + up) & 0xFF
            elif filter_type == 3:
                cur[x] = (cur[x] + ((left + up) // 2)) & 0xFF
            elif filter_type == 4:
                p = left + up - up_left
                pa = abs(p - left)
                pb = abs(p - up)
                pc = abs(p - up_left)
                cur[x] = (cur[x] + (left if pa <= pb and pa <= pc else up if pb <= pc else up_left)) & 0xFF
            elif filter_type != 0:
                raise ValueError(f"unsupported PNG filter {filter_type}: {path}")
        out[y * stride:(y + 1) * stride] = cur
        prev = cur
    return width, height, bytes(out)


def write_png(path: Path, width: int, height: int, pixels: bytes) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    rows = bytearray()
    stride = width * 4
    for y in range(height):
        rows.append(0)
        rows.extend(pixels[y * stride:(y + 1) * stride])
    compressed = zlib.compress(bytes(rows), level=9)

    def chunk(kind: bytes, payload: bytes) -> bytes:
        return struct.pack(">I", len(payload)) + kind + payload + struct.pack(">I", zlib.crc32(kind + payload) & 0xFFFFFFFF)

    png = PNG_SIG
    png += chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
    png += chunk(b"IDAT", compressed)
    png += chunk(b"IEND", b"")
    path.write_bytes(png)


def crop(pixels: bytes, src_w: int, box: tuple[int, int, int, int]) -> tuple[int, int, bytes]:
    left, top, right, bottom = box
    width = right - left
    height = bottom - top
    out = bytearray(width * height * 4)
    for y in range(height):
        src_start = ((top + y) * src_w + left) * 4
        dst_start = y * width * 4
        out[dst_start:dst_start + width * 4] = pixels[src_start:src_start + width * 4]
    return width, height, bytes(out)


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def guid_for(rel: str) -> str:
    return uuid.uuid5(uuid.NAMESPACE_URL, "linhgioi-art-v1:" + rel).hex


def write_unity_meta(path: Path, rel: str) -> None:
    meta = path.with_suffix(path.suffix + ".meta")
    meta.write_text(
        f"""fileFormatVersion: 2
guid: {guid_for(rel)}
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


def main() -> int:
    for sub in ("reference-only", "runtime-asset-source"):
        (DOC_ROOT / sub).mkdir(parents=True, exist_ok=True)
    for name in ("01-LGO-visual-direction-board-v1.png", "02-LGO-screen-mockup-pack-v1.png"):
        shutil.copy2(PACK / "images" / name, DOC_ROOT / "reference-only" / name)
    for name in ("03-LGO-ui-asset-sheet-v1.png", "04-LGO-world-combat-asset-sheet-v1.png"):
        shutil.copy2(PACK / "images" / name, DOC_ROOT / "runtime-asset-source" / name)

    decoded: dict[str, tuple[int, int, bytes]] = {}
    rows: list[dict[str, str]] = []
    for sheet, rel, box, usage, notes in ASSETS:
        if sheet not in decoded:
            decoded[sheet] = read_png(PACK / "images" / sheet)
        src_w, _src_h, pixels = decoded[sheet]
        width, height, data = crop(pixels, src_w, box)
        doc_path = DOC_ROOT / "runtime-asset-pack" / rel
        write_png(doc_path, width, height, data)
        rows.append({
            "source_sheet": sheet,
            "crop_box": f"{box[0]} {box[1]} {box[2]} {box[3]}",
            "output_file": rel,
            "intended_usage": usage,
            "unity_path": "",
            "notes": notes,
            "sha256": sha256(doc_path),
        })

    mapping = DOC_ROOT / "runtime-asset-pack" / "MAPPING.csv"
    mapping.parent.mkdir(parents=True, exist_ok=True)
    with mapping.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(rows[0].keys()))
        writer.writeheader()
        writer.writerows(rows)
    print(f"LGO_ART_PACK_V1_EXPERIMENTAL_SLICE_PASS assets={len(rows)} mapping={mapping}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
