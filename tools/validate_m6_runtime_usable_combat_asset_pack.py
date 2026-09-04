#!/usr/bin/env python3
from __future__ import annotations

import json
import struct
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

ASSET_DIR = Path("docs/reference-art/v0.45.0/runtime-assets")
REQUIRED_ASSETS = [
    "target-dummy-idle-v0450.png",
    "target-dummy-selected-v0450.png",
    "target-dummy-hit-v0450.png",
    "target-dummy-recover-v0450.png",
    "skill-wind-slash-frame-01-v0450.png",
    "skill-wind-slash-frame-02-v0450.png",
    "skill-wind-slash-frame-03-v0450.png",
    "skill-wind-slash-frame-04-v0450.png",
    "skill-impact-spark-v0450.png",
    "cooldown-ring-ready-v0450.png",
    "cooldown-ring-cooldown-v0450.png",
    "target-marker-selected-v0450.png",
    "warning-telegraph-circle-v0450.png",
    "combat-button-normal-v0450.png",
    "combat-button-pressed-v0450.png",
    "combat-button-cooldown-v0450.png",
    "combat-panel-9slice-v0450.png",
]
REQUIRED_DOCS = {
    "LGO-M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-v0.45.0-CODEX-USAGE.md": ["transparent placeholder", "not final production art"],
    "README-LGO-M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-v0.45.0.md": ["transparent PNG placeholder", "No production art"],
    "docs/reference-art/v0.45.0/CODEX-USAGE.md": ["Player-facing UI text must remain Vietnamese", "do not add new combat mechanics"],
    "docs/reference-art/v0.45.0/README.md": ["transparent PNG placeholder assets", "No protocol/GameData/schema changes"],
    "docs/art/LGO-M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-v0.45.0.md": ["not production art", "Do not bake text into sprites"],
    "docs/design/LGO-M6-COMBAT-ASSET-IMPORT-RULES-v0.45.0.md": ["Texture Type: Sprite", "Existing cooldown/combat state remains authoritative"],
    "docs/tasks/M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-v0.45.0.md": ["M6_RUNTIME_USABLE_COMBAT_ASSET_PACK_INGEST_PASS_v0.45.0"],
    "M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-FINAL-REPORT-v0.45.0.md": ["M6_RUNTIME_USABLE_COMBAT_ASSET_PACK_INGEST_SOURCE_CLOSED_v0.45.0"],
    "HANDOFF-LG-M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-v0.45.0.md": ["Unity import/wiring belongs to v0.46"],
    "LGO-M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-v0.45.0-DELETIONS.txt": ["DELETED", "none"],
}
FROZEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FORBIDDEN_STATUS_FRAGMENTS = ("__pycache__/", ".pyc", ".DS_Store", "__MACOSX/")


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(
        ["git", "--no-pager", *args],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        errors.append("git command failed: git --no-pager " + " ".join(args) + " " + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def read(path: str) -> str:
    target = ROOT / path
    if not target.is_file():
        errors.append(f"missing file: {path}")
        return ""
    return target.read_text(encoding="utf-8", errors="replace")


def validate_png(path: Path) -> None:
    data = path.read_bytes()
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        errors.append(f"not a PNG: {path.relative_to(ROOT).as_posix()}")
        return
    if len(data) < 33 or data[12:16] != b"IHDR":
        errors.append(f"missing PNG IHDR: {path.relative_to(ROOT).as_posix()}")
        return
    width, height, bit_depth, color_type = struct.unpack(">IIBB", data[16:26])
    if width <= 0 or height <= 0:
        errors.append(f"invalid PNG dimensions: {path.relative_to(ROOT).as_posix()}")
    if bit_depth not in (8, 16):
        errors.append(f"unexpected PNG bit depth {bit_depth}: {path.relative_to(ROOT).as_posix()}")
    if color_type not in (4, 6):
        errors.append(f"PNG lacks alpha channel: {path.relative_to(ROOT).as_posix()}")


def main() -> int:
    manifest_path = ROOT / "LGO-M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-v0.45.0-MANIFEST.json"
    if not manifest_path.is_file():
        errors.append("missing v0.45 manifest")
        manifest_files: set[str] = set()
    else:
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
        if manifest.get("type") != "runtime-usable-placeholder-assets":
            errors.append("manifest type must be runtime-usable-placeholder-assets")
        if "not production art" not in manifest.get("status", ""):
            errors.append("manifest must retain non-production status")
        manifest_files = set(manifest.get("files", []))

    for name in REQUIRED_ASSETS:
        rel = ASSET_DIR / name
        target = ROOT / rel
        if not target.is_file():
            errors.append(f"missing runtime asset: {rel.as_posix()}")
        else:
            validate_png(target)
        if rel.as_posix() not in manifest_files:
            errors.append(f"manifest missing runtime asset: {rel.as_posix()}")

    preview = ROOT / "docs/reference-art/v0.45.0/preview/lgo-m6-runtime-combat-assets-preview-v0450.png"
    if not preview.is_file():
        errors.append("missing preview reference image")
    else:
        validate_png(preview)

    for path, markers in REQUIRED_DOCS.items():
        content = read(path)
        for marker in markers:
            if marker not in content:
                errors.append(f"{path} missing marker: {marker}")

    deletion_lines = read("LGO-M6-RUNTIME-USABLE-COMBAT-ASSET-PACK-v0.45.0-DELETIONS.txt").splitlines()
    if deletion_lines[:2] != ["DELETED", "none"]:
        errors.append("v0.45 deletion manifest must be exactly DELETED then none")

    for path in git_lines("diff", "--name-only"):
        if path == "client/Unity/Assets/Game/UI/design-tokens.json":
            errors.append(f"frozen surface modified: {path}")
        for prefix in FROZEN_PREFIXES:
            if path.startswith(prefix):
                errors.append(f"frozen surface modified: {path}")

    for line in git_lines("status", "--short", "--untracked-files=all"):
        status = line[:2]
        if "D" in status:
            continue
        path = line[3:] if len(line) >= 4 else line
        if any(fragment in path for fragment in FORBIDDEN_STATUS_FRAGMENTS):
            errors.append(f"forbidden cache/source artifact present: {path}")

    if errors:
        print("M6 RUNTIME-USABLE COMBAT ASSET PACK VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6_RUNTIME_USABLE_COMBAT_ASSET_PACK_INGEST_PASS_v0.45.0")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
