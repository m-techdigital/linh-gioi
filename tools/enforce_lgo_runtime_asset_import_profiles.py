#!/usr/bin/env python3
from __future__ import annotations

import csv
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"


def role_mobile_max(role: str, default_max: int) -> int:
    if role == "login_background":
        return min(default_max, 1024)
    if role.startswith("vfx_"):
        return min(default_max, 256)
    if role.startswith("combat_cooldown_"):
        return min(default_max, 128)
    return min(default_max, 512)


def role_ios_max(role: str, default_max: int) -> int:
    if role == "login_background":
        return min(default_max, 1536)
    if role.startswith("vfx_"):
        return min(default_max, 256)
    if role.startswith("combat_cooldown_"):
        return min(default_max, 128)
    return min(default_max, 768)


def platform_settings(role: str, default_max: int) -> str:
    android_max = role_mobile_max(role, default_max)
    iphone_max = role_ios_max(role, default_max)
    standalone_max = default_max
    return f"""platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: {default_max}
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 80
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  - serializedVersion: 3
    buildTarget: Standalone
    maxTextureSize: {standalone_max}
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 80
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 1
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  - serializedVersion: 3
    buildTarget: Android
    maxTextureSize: {android_max}
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 70
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 1
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  - serializedVersion: 3
    buildTarget: iPhone
    maxTextureSize: {iphone_max}
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 75
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 1
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0"""


def replace_platform_settings(text: str, block: str) -> str:
    if "  platformSettings: []" in text:
        return text.replace("  platformSettings: []", "  " + block.replace("\n", "\n  "), 1)
    pattern = re.compile(
        r"  platformSettings:\n(?:  - serializedVersion: 3\n(?:    .+\n)+)+",
        re.MULTILINE,
    )
    replacement = "  " + block.replace("\n", "\n  ") + "\n"
    updated, count = pattern.subn(replacement, text, count=1)
    if count:
        return updated
    raise ValueError("could not find platformSettings block")


def main() -> int:
    if not MANIFEST.is_file():
        raise SystemExit(f"missing manifest: {MANIFEST}")
    changed = 0
    with MANIFEST.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))
    for row in rows:
        unity_path = ROOT / row["unity_path"]
        meta_path = unity_path.with_suffix(unity_path.suffix + ".meta")
        if not meta_path.is_file():
            raise SystemExit(f"missing meta: {meta_path.relative_to(ROOT)}")
        max_texture = int(row["runtime_max_texture_size"])
        block = platform_settings(row["role"], max_texture)
        original = meta_path.read_text(encoding="utf-8")
        updated = replace_platform_settings(original, block)
        if updated != original:
            meta_path.write_text(updated, encoding="utf-8")
            changed += 1
    print(f"LGO_RUNTIME_ASSET_IMPORT_PROFILES_ENFORCED changed={changed}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
