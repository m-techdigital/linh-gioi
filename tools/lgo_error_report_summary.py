#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SUMMARY = ROOT / "build" / "lgo-playable-closure" / "latest-summary.json"


def classify(payload: dict[str, object]) -> str:
    status = str(payload.get("status", "")).upper()
    reason = str(payload.get("reason", "")).lower()
    if status == "PASS":
        return "PASS"
    if "unity_editor is not set" in reason or "not executable" in reason or status == "UNVERIFIED_ENVIRONMENT":
        return "UNVERIFIED_ENVIRONMENT"
    if "protocol" in reason or "schema" in reason or "adr" in reason or "design-token" in reason:
        return "CONTRACT_CHANGE_REQUIRED"
    return "FIX_REQUIRED"


def main() -> int:
    parser = argparse.ArgumentParser(description="Summarize the latest local Linh Gioi closure error state.")
    parser.add_argument("--summary", default=str(SUMMARY))
    args = parser.parse_args()
    path = Path(args.summary)
    if not path.is_file():
        print(json.dumps({"status": "NO_SUMMARY", "classification": "UNVERIFIED_ENVIRONMENT", "path": str(path)}, indent=2, sort_keys=True))
        return 0
    payload = json.loads(path.read_text(encoding="utf-8"))
    payload["classification"] = classify(payload)
    print(json.dumps(payload, indent=2, sort_keys=True))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
