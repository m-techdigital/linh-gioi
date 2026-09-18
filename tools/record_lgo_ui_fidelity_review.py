#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
from pathlib import Path

from validate_lgo_ui_fidelity_gate_bundle import file_sha256

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_CONFIG = ROOT / "docs/design/LGO-UI-FIDELITY-DEVICE-GATE-v1.0.json"


def build_review(evidence_root: Path, config: dict, status: str, reviewed_by: str, notes: list[str]) -> dict:
    if status in {"PASS", "FIX_REQUIRED"} and not reviewed_by.strip():
        raise ValueError("reviewed-by is required for PASS/FIX_REQUIRED")
    required_profiles = config.get("requiredProfiles") or []
    artifacts: dict[str, str] = {}
    for sid, spec in (config.get("surfaceEvidence") or {}).items():
        if isinstance(spec, str):
            rel = spec
            profiles = required_profiles
        else:
            rel = spec["path"]
            profiles = spec.get("profiles") or required_profiles
        for profile in profiles:
            path = evidence_root / profile / rel
            if not path.is_file():
                raise FileNotFoundError(path)
            artifacts[f"{sid}:{profile}"] = file_sha256(path)
    return {
        "version": 1,
        "status": status,
        "reviewedBy": reviewed_by,
        "reviewedArtifacts": len(artifacts),
        "artifacts": dict(sorted(artifacts.items())),
        "notes": notes,
    }


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--evidence-root", required=True)
    parser.add_argument("--config", default=str(DEFAULT_CONFIG))
    parser.add_argument("--out", required=True)
    parser.add_argument("--status", choices=("PENDING", "PASS", "FIX_REQUIRED"), default="PENDING")
    parser.add_argument("--reviewed-by", default="")
    parser.add_argument("--note", action="append", default=[])
    args = parser.parse_args()

    config = json.loads(Path(args.config).read_text(encoding="utf-8"))
    review = build_review(
        Path(args.evidence_root).resolve(),
        config,
        args.status,
        args.reviewed_by,
        args.note,
    )
    out = Path(args.out).resolve()
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(review, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(
        "LGO_UI_FIDELITY_REVIEW_RECORDED "
        f"status={review['status']} artifacts={review['reviewedArtifacts']} out={out}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
