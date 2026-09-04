#!/usr/bin/env python3
from __future__ import annotations

import csv
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"
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


def block_has_target(text: str, target: str, max_size: int, overridden: str) -> bool:
    marker = f"buildTarget: {target}"
    index = text.find(marker)
    if index < 0:
        return False
    block = text[index : text.find("  - serializedVersion:", index + 1)]
    if not block:
        block = text[index:]
    return f"maxTextureSize: {max_size}" in block and f"overridden: {overridden}" in block


def check_meta_profiles() -> None:
    if not MANIFEST.is_file():
        ERRORS.append("missing V3B runtime manifest")
        return
    with MANIFEST.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))
    for row in rows:
        role = row["role"]
        max_texture = int(row["runtime_max_texture_size"])
        meta_path = (ROOT / row["unity_path"]).with_suffix(Path(row["unity_path"]).suffix + ".meta")
        if not meta_path.is_file():
            ERRORS.append(f"missing meta file: {meta_path.relative_to(ROOT)}")
            continue
        text = meta_path.read_text(encoding="utf-8", errors="replace")
        expectations = (
            ("DefaultTexturePlatform", max_texture, "0"),
            ("Standalone", max_texture, "1"),
            ("Android", role_mobile_max(role, max_texture), "1"),
            ("iPhone", role_ios_max(role, max_texture), "1"),
        )
        for target, size, overridden in expectations:
            if not block_has_target(text, target, size, overridden):
                ERRORS.append(
                    f"{meta_path.relative_to(ROOT)} missing {target} override maxTextureSize={size} overridden={overridden}"
                )
        for marker in ("textureCompression: 1", "compressionQuality:", "LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL"):
            if marker not in text:
                ERRORS.append(f"{meta_path.relative_to(ROOT)} missing marker: {marker}")


def check_frozen() -> None:
    result = subprocess.run(
        [
            "git",
            "--no-pager",
            "diff",
            "--name-only",
            "--",
            "protocol",
            "gamedata/schemas",
            "docs/adr",
            "client/Unity/Assets/Game/UI/design-tokens.json",
        ],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "git frozen diff failed")
    elif result.stdout.strip():
        ERRORS.append("frozen contract/design-token surface changed")


def main() -> int:
    require(
        "docs/art/RUNTIME-ASSET-IMPORT-PROFILES.md",
        "LGO_RUNTIME_ASSET_IMPORT_PROFILES_READY",
        "Android",
        "iPhone",
        "Standalone",
        "no duplicate ad hoc runtime asset folders",
    )
    require(
        "docs/tasks/LGO-RUNTIME-ASSET-OPTIMIZATION-PASS-v1.0.md",
        "LGO_RUNTIME_ASSET_OPTIMIZATION_READY",
        "No production art claim",
        "No gameplay change",
    )
    require("tools/enforce_lgo_runtime_asset_import_profiles.py", "platformSettings", "DefaultTexturePlatform", "Android", "iPhone")
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_runtime_asset_import_profiles.py")
    check_meta_profiles()
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME ASSET IMPORT PROFILE VALIDATION FAILED", file=__import__("sys").stderr)
        for error in ERRORS:
            print(" - " + error, file=__import__("sys").stderr)
        return 1
    print("LGO_RUNTIME_ASSET_IMPORT_PROFILE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
