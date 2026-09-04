#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
V3_PACK = Path("/Users/minhdc/Projects/LGO-ArtPacks/LGO-ART-V3-HIGH-RES-VISUAL-TARGET-PACK")
FROZEN_PREFIXES = ("protocol/", "gamedata/schemas/", "docs/adr/")
FROZEN_EXACT = {"client/Unity/Assets/Game/UI/design-tokens.json"}
REQUIRED_DOCS = (
    "docs/art/v1/ART-V1-REFERENCE-ONLY-BOUNDARY.md",
    "docs/art/v2/ART-V2-STRUCTURAL-PLACEHOLDER-BOUNDARY.md",
    "docs/art/v3/ART-V3-HIGH-RES-VISUAL-TARGET.md",
    "docs/art/v3/ART-V3B-SEPARATED-RUNTIME-ASSET-REQUIREMENTS.md",
    "docs/art/v2/ART-V2-QUALITY-REVIEW.md",
    "docs/art/v2/ART-V2-USAGE-BOUNDARY.md",
)
REQUIRED_MARKERS = (
    "LGO_ART_V1_REFERENCE_ONLY_ACCEPTED",
    "experimental-source-only",
    "STRUCTURAL_RUNTIME_PLACEHOLDER_V2",
    "not final visual quality",
    "not production art",
    "LGO_ART_V3_HIGH_RES_VISUAL_TARGET_REFERENCE_ONLY",
    "ART_V3B_SEPARATED_RUNTIME_ASSET_PRODUCTION_REQUIRED",
)
V3_REFERENCE_IMAGES = (
    "01-login-gate-entry-high-res-target-reference.png",
    "02-high-res-runtime-asset-catalog-reference.png",
    "03-high-res-asset-pack-v3-reference.png",
)


def fail(message: str) -> None:
    print(f"LGO ART V3 COURSE CORRECTION VALIDATION FAILED: {message}", file=sys.stderr)
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
    combined = "\n".join(read(rel) for rel in REQUIRED_DOCS)
    for marker in REQUIRED_MARKERS:
        if marker not in combined:
            fail(f"missing marker: {marker}")


def check_v3_pack_reference_only() -> None:
    if not V3_PACK.is_dir():
        fail(f"missing V3 visual target pack: {V3_PACK}")
    for name in V3_REFERENCE_IMAGES:
        source = V3_PACK / "images/reference-only" / name
        if not source.is_file():
            fail(f"missing V3 reference image in pack: {source}")
        for unity_path in (ROOT / "client/Unity/Assets").rglob(name):
            fail(f"V3 reference poster imported into Unity: {unity_path.relative_to(ROOT)}")
        for repo_path in (ROOT / "docs/reference-art").rglob(name):
            fail(f"V3 reference poster copied into repo reference-art: {repo_path.relative_to(ROOT)}")


def check_frozen_status() -> None:
    for line in git_status_lines():
        rel = line[3:] if len(line) >= 4 else line
        if rel in FROZEN_EXACT or rel.startswith(FROZEN_PREFIXES):
            fail(f"frozen surface changed: {rel}")
        if "__pycache__/" in rel or rel.endswith(".pyc") or rel.endswith(".DS_Store"):
            fail(f"generated/cache artifact present: {rel}")


def main() -> int:
    check_docs()
    check_v3_pack_reference_only()
    check_frozen_status()
    print("LGO_ART_V3_COURSE_CORRECTION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
