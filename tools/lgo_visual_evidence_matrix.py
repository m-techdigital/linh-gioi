#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

VIEWS = [
    {
        "id": "login_gate_entry",
        "label": "Login / Gate Entry",
        "requiredEvidence": ["runtime screenshot", "capture log"],
        "nonClaim": "not production art",
    },
    {
        "id": "character_hall",
        "label": "Character Hall",
        "requiredEvidence": ["runtime screenshot", "capture log"],
        "nonClaim": "not production auth",
    },
    {
        "id": "world_hud",
        "label": "World HUD",
        "requiredEvidence": ["runtime screenshot", "capture log"],
        "nonClaim": "not MMO-scale gameplay",
    },
    {
        "id": "first_playable_loop",
        "label": "First Playable Loop",
        "requiredEvidence": ["runtime screenshot", "smoke log"],
        "nonClaim": "not full quest/content system",
    },
    {
        "id": "combat_readiness_hud",
        "label": "Combat Readiness HUD",
        "requiredEvidence": ["runtime screenshot", "combat smoke json"],
        "nonClaim": "not production combat",
    },
    {
        "id": "combat_placeholder_assets",
        "label": "Combat Placeholder Assets",
        "requiredEvidence": ["runtime screenshot", "contact sheet review aid"],
        "nonClaim": "not production art",
    },
]


def main() -> int:
    parser = argparse.ArgumentParser(description="List the Linh Gioi visual evidence matrix.")
    parser.add_argument("--json", action="store_true")
    args = parser.parse_args()
    payload = {
        "marker": "LGO_VISUAL_EVIDENCE_MATRIX_READY",
        "views": VIEWS,
        "visualGate": ["./tools/lgo_playable_closure_check.sh", "--visual-evidence"],
    }
    if args.json:
        print(json.dumps(payload, indent=2, sort_keys=True))
    else:
        for view in VIEWS:
            print(f"{view['id']}: {view['label']} [{view['nonClaim']}]")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
