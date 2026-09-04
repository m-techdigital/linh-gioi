#!/usr/bin/env python3
from __future__ import annotations

import hashlib
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

SOURCE_DIR = ROOT / "docs/reference-art/v0.45.0/runtime-assets"
UNITY_DIR = ROOT / "client/Unity/Assets/Game/Art/Combat/Placeholders/Resources/CombatPlaceholders"
ASSETS = [
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
REQUIRED_MARKERS = {
    "client/Unity/Assets/Game/Art/Runtime/CombatPlaceholderAssets.cs": [
        "ResourceRoot = \"CombatPlaceholders/\"",
        "TargetDummyIdle",
        "CombatButtonCooldownTexture",
        "Version = \"0.46.0\"",
    ],
    "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs": [
        "CreateBillboardSprite",
        "CombatPlaceholderAssets.TargetDummyIdle",
        "CombatPlaceholderAssets.ImpactSpark",
        "CombatPlaceholderAssets.WarningTelegraphCircle",
    ],
    "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs": [
        "ApplyCombatPanelSkin",
        "ApplyCombatButtonSkin",
        "NewCombatCooldownIcon",
        "Gửi ý định chiến đấu",
    ],
    "docs/tasks/M6-UNITY-COMBAT-PLACEHOLDER-ASSET-IMPORT-v0.46.0.md": [
        "M6_UNITY_COMBAT_PLACEHOLDER_ASSET_IMPORT_PASS_v0.46.0",
    ],
    "M6-UNITY-COMBAT-PLACEHOLDER-ASSET-IMPORT-FINAL-REPORT-v0.46.0.md": [
        "not production art",
        "No combat mechanic was added",
    ],
    "HANDOFF-LG-M6-UNITY-COMBAT-PLACEHOLDER-ASSET-IMPORT-v0.46.0.md": [
        "Player-facing combat labels remain Vietnamese",
    ],
    "LGO-M6-UNITY-COMBAT-PLACEHOLDER-ASSET-IMPORT-v0.46.0-DELETIONS.txt": [
        "DELETED",
        "none",
    ],
}
FROZEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FORBIDDEN_STATUS_FRAGMENTS = ("__pycache__/", ".pyc", ".DS_Store", "__MACOSX/")


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


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


def require(path: str, markers: list[str]) -> None:
    target = ROOT / path
    if not target.is_file():
        errors.append(f"missing file: {path}")
        return
    content = target.read_text(encoding="utf-8", errors="replace")
    for marker in markers:
        if marker not in content:
            errors.append(f"{path} missing marker: {marker}")


def main() -> int:
    for name in ASSETS:
        source = SOURCE_DIR / name
        imported = UNITY_DIR / name
        if not source.is_file():
            errors.append(f"missing source asset: {source.relative_to(ROOT).as_posix()}")
            continue
        if not imported.is_file():
            errors.append(f"missing Unity imported asset: {imported.relative_to(ROOT).as_posix()}")
            continue
        if sha256(source) != sha256(imported):
            errors.append(f"Unity imported asset does not match source reference: {name}")
        meta = Path(str(imported) + ".meta")
        if not meta.is_file():
            errors.append(f"missing Unity meta: {meta.relative_to(ROOT).as_posix()}")
        else:
            meta_text = meta.read_text(encoding="utf-8", errors="replace")
            for marker in ["TextureImporter", "spriteMode: 1", "textureType: 8", "alphaIsTransparency: 1", "enableMipMap: 0"]:
                if marker not in meta_text:
                    errors.append(f"{meta.relative_to(ROOT).as_posix()} missing importer marker: {marker}")

    for folder in [
        "client/Unity/Assets/Game/Art/Combat.meta",
        "client/Unity/Assets/Game/Art/Combat/Placeholders.meta",
        "client/Unity/Assets/Game/Art/Combat/Placeholders/Resources.meta",
        "client/Unity/Assets/Game/Art/Combat/Placeholders/Resources/CombatPlaceholders.meta",
    ]:
        if not (ROOT / folder).is_file():
            errors.append(f"missing Unity folder meta: {folder}")

    for path, markers in REQUIRED_MARKERS.items():
        require(path, markers)

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
        print("M6 UNITY COMBAT PLACEHOLDER ASSET IMPORT VALIDATION FAILED", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("M6_UNITY_COMBAT_PLACEHOLDER_ASSET_IMPORT_PASS_v0.46.0")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
