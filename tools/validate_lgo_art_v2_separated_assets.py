#!/usr/bin/env python3
from __future__ import annotations

import csv
import hashlib
import json
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PACK = Path("/Users/minhdc/Projects/LGO-ArtPacks/LGO-ART-V2-SEPARATED-RUNTIME-ASSETS")
DOC_ROOT = ROOT / "docs/reference-art/v2"
UNITY_ROOT = ROOT / "client/Unity/Assets/Game/Art/Runtime/V2/Resources/LGOArtV2"
FORBIDDEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FORBIDDEN_EXACT = {"client/Unity/Assets/Game/UI/design-tokens.json"}
FORBIDDEN_STATUS = ("__pycache__/", ".pyc", ".DS_Store", "client/Unity/Library/", "client/Unity/Temp/", "client/Unity/Logs/", "/target/")
REQUIRED_DOCS = (
    "docs/art/v2/LGO-ART-V2-SEPARATED-ASSET-PACK.md",
    "docs/art/v2/LGO-ART-V2-ASSET-QUALITY-RULES.md",
    "docs/art/v2/ART-V2-QUALITY-REVIEW.md",
    "docs/art/v2/ART-V2-USAGE-BOUNDARY.md",
    "docs/art/v3/ART-V3-HIGH-RES-ASSET-REQUIREMENTS.md",
    "docs/art/v2/LGO-ART-V2-CODEX-USAGE.md",
    "docs/art/v2/LGO-ART-V2-UNITY-IMPORT-MAPPING.md",
    "HANDOFF-LGO-ART-V2-SEPARATED-ASSETS.md",
    "LGO-ART-V2-SEPARATED-ASSETS-CHANGED-FILES.txt",
    "LGO-ART-V2-SEPARATED-ASSETS-DELETIONS.txt",
)
REQUIRED_MARKERS = (
    "LGO_ART_V1_AUTO_SLICE_STOPPED_SWITCHED_TO_V2",
    "LGO_ART_V2_SEPARATED_ASSETS_INGESTED",
    "LGO_ART_V2_UNITY_IMPORT_CLOSED",
    "LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1",
    "STRUCTURAL_RUNTIME_PLACEHOLDER_V2",
    "not final visual quality",
    "not production art",
    "ART V3",
)


def fail(message: str) -> None:
    print(f"LGO ART V2 SEPARATED ASSETS VALIDATION FAILED: {message}", file=sys.stderr)
    raise SystemExit(1)


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(["git", "--no-pager", *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        fail(result.stderr.strip() or "git failed")
    return result.stdout.splitlines()


def unity_rel_for(pack_rel: str) -> str:
    rel = pack_rel.removeprefix("images/runtime-ready/")
    top, rest = rel.split("/", 1)
    return {"login": "Login", "ui": "UI", "combat": "Combat", "world": "World", "vfx": "VFX"}[top] + "/" + rest


def read(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        fail(f"missing file: {rel}")
    return path.read_text(encoding="utf-8", errors="replace")


def check_manifest() -> None:
    manifest_csv = PACK / "metadata/runtime-assets-v2-manifest.csv"
    manifest_json = PACK / "metadata/runtime-assets-v2-manifest.json"
    if not manifest_csv.is_file() or not manifest_json.is_file():
        fail("missing source pack manifest")
    json.loads(manifest_json.read_text(encoding="utf-8"))
    with manifest_csv.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))
    if len(rows) < 60:
        fail(f"expected at least 60 runtime-ready assets, found {len(rows)}")
    for row in rows:
        for key in ("path", "width", "height", "sha256", "usage"):
            if not row.get(key):
                fail(f"manifest row missing {key}: {row}")
        if not row["path"].startswith("images/runtime-ready/"):
            fail(f"manifest runtime row outside runtime-ready: {row['path']}")
        source = PACK / row["path"]
        docs = DOC_ROOT / row["path"].removeprefix("images/")
        unity = UNITY_ROOT / unity_rel_for(row["path"])
        for path in (source, docs, unity):
            if not path.is_file():
                fail(f"missing asset: {path}")
            if sha256(path) != row["sha256"]:
                fail(f"sha mismatch: {path}")
        meta = unity.with_suffix(unity.suffix + ".meta")
        if not meta.is_file():
            fail(f"missing Unity meta: {meta.relative_to(ROOT)}")
        meta_text = meta.read_text(encoding="utf-8", errors="replace")
        for marker in ("TextureImporter", "spriteMode: 1", "textureType: 8", "alphaIsTransparency: 1", "enableMipMap: 0"):
            if marker not in meta_text:
                fail(f"{meta.relative_to(ROOT)} missing importer marker: {marker}")


def check_reference_boundary() -> None:
    reference = DOC_ROOT / "reference-only"
    if not reference.is_dir():
        fail("missing docs reference-only folder")
    for path in reference.rglob("*.png"):
        rel_name = path.name
        for unity_path in (ROOT / "client/Unity/Assets").rglob(rel_name):
            fail(f"reference-only image imported to Unity: {unity_path.relative_to(ROOT)}")
    for path in UNITY_ROOT.rglob("*.png"):
        if "reference" in path.name or "contact-sheet" in path.name or "sheet" in path.name:
            fail(f"reference/composite-like image imported to Unity: {path.relative_to(ROOT)}")


def check_docs_and_code() -> None:
    combined = "\n".join(read(rel) for rel in REQUIRED_DOCS if (ROOT / rel).exists())
    extra = []
    for rel in (
        "docs/tasks/LGO-ART-V2-UNITY-IMPORT.md",
        "docs/art/v2/LGO-ART-V2-UNITY-IMPORT-REPORT.md",
        "docs/tasks/LOGIN-GATE-ENTRY-VISUAL-v1.md",
        "docs/design/LOGIN-GATE-ENTRY-VISUAL-SPEC-v1.md",
        "LGO-LOGIN-GATE-ENTRY-VISUAL-FINAL-REPORT-v1.md",
    ):
        if (ROOT / rel).is_file():
            extra.append(read(rel))
    combined = combined + "\n" + "\n".join(extra)
    for marker in REQUIRED_MARKERS[:2]:
        if marker not in combined:
            fail(f"missing marker: {marker}")
    registry = ROOT / "client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV2.cs"
    if registry.is_file():
        text = registry.read_text(encoding="utf-8", errors="replace")
        for marker in ("ResourceRoot = \"LGOArtV2/\"", "STRUCTURAL_RUNTIME_PLACEHOLDER_V2", "LoginBackgroundSpiritGate", "ButtonPrimaryNormalTexture", "DummyIdle"):
            if marker not in text:
                fail(f"registry missing marker: {marker}")


def check_git_hygiene() -> None:
    for line in git_lines("status", "--short", "--untracked-files=all"):
        rel = line[3:] if len(line) >= 4 else line
        if rel in FORBIDDEN_EXACT or rel.startswith(FORBIDDEN_PREFIXES):
            fail(f"frozen surface changed: {rel}")
        if any(fragment in rel for fragment in FORBIDDEN_STATUS):
            fail(f"forbidden generated/cache artifact present: {rel}")


def main() -> int:
    check_manifest()
    check_reference_boundary()
    check_docs_and_code()
    check_git_hygiene()
    print("LGO_ART_V2_SEPARATED_ASSETS_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
