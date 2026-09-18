#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import subprocess
from pathlib import Path

from report_lgo_product_ui_style_debt import DEFAULT_SOURCE, scan
from validate_lgo_ui_authority_matrix import validate_document
from validate_lgo_ui_fidelity_gate_bundle import file_sha256

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_MATRIX = ROOT / "docs/design/LGO-UI-AUTHORITY-MATRIX-v1.0.json"
DEFAULT_CONFIG = ROOT / "docs/design/LGO-UI-FIDELITY-DEVICE-GATE-v1.0.json"
DEFAULT_BUDGET = ROOT / "docs/design/LGO-UI-STYLE-DEBT-BUDGET-v1.0.json"


def git_head(root: Path) -> str:
    return subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=root, text=True).strip()


def root_relative(root: Path, path: Path) -> str:
    return str(path.resolve().relative_to(root.resolve()))


def add_artifact(artifacts: dict[str, str], root: Path, path: Path) -> None:
    if not path.is_file():
        raise FileNotFoundError(path)
    artifacts[root_relative(root, path)] = file_sha256(path)
def build_bundle(root: Path, owner_root: Path, evidence_root: Path, review_path: Path,
                 matrix_path: Path, config_path: Path, budget_path: Path) -> dict:
    matrix = json.loads(matrix_path.read_text(encoding="utf-8"))
    errors = validate_document(matrix, owner_root, True)
    if errors:
        raise ValueError("authority matrix strict validation failed: " + "; ".join(errors))
    config = json.loads(config_path.read_text(encoding="utf-8"))
    budget = json.loads(budget_path.read_text(encoding="utf-8"))
    review = json.loads(review_path.read_text(encoding="utf-8"))
    if review.get("status") != "PASS":
        raise ValueError("visual review must be PASS before bundle creation")

    artifacts: dict[str, str] = {}
    for path in (matrix_path, config_path, budget_path, review_path):
        add_artifact(artifacts, root, path)

    required_profiles = config.get("requiredProfiles") or []
    for profile in required_profiles:
        profile_root = evidence_root / profile
        for rel in ("manifest.json", "character-hub/manifest.json", "quest/manifest.json"):
            add_artifact(artifacts, root, profile_root / rel)

    for sid, spec in (config.get("surfaceEvidence") or {}).items():
        if isinstance(spec, str):
            rel = spec
            profiles = required_profiles
        else:
            rel = spec["path"]
            profiles = spec.get("profiles") or required_profiles
        for profile in profiles:
            add_artifact(artifacts, root, evidence_root / profile / rel)
    style_report = scan(DEFAULT_SOURCE)
    metrics = {key: style_report["metrics"][key] for key in (
        "directStyleAssignments",
        "numericStyleAssignments",
        "semanticPaletteDeclarations",
        "skinPartialMaxLoc",
        "skinPartialMaxMethods",
    )}
    return {
        "version": 1,
        "sourceHead": git_head(root),
        "authorityMatrix": root_relative(root, matrix_path),
        "styleBudget": root_relative(root, budget_path),
        "gateConfig": root_relative(root, config_path),
        "review": root_relative(root, review_path),
        "evidenceRoot": root_relative(root, evidence_root),
        "styleMetrics": metrics,
        "artifacts": dict(sorted(artifacts.items())),
    }


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--owner-root", required=True)
    parser.add_argument("--evidence-root", required=True)
    parser.add_argument("--review", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--matrix", default=str(DEFAULT_MATRIX))
    parser.add_argument("--config", default=str(DEFAULT_CONFIG))
    parser.add_argument("--budget", default=str(DEFAULT_BUDGET))
    args = parser.parse_args()

    root = ROOT.resolve()
    bundle = build_bundle(
        root,
        Path(args.owner_root).resolve(),
        Path(args.evidence_root).resolve(),
        Path(args.review).resolve(),
        Path(args.matrix).resolve(),
        Path(args.config).resolve(),
        Path(args.budget).resolve(),
    )
    out = Path(args.out).resolve()
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(bundle, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(
        "LGO_UI_FIDELITY_GATE_BUNDLE_READY "
        f"sourceHead={bundle['sourceHead']} artifacts={len(bundle['artifacts'])} out={out}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
