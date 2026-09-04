#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

SOURCE_GATES = [
    {
        "id": "package_hygiene",
        "command": ["python3.12", "tools/validate_package_hygiene.py"],
        "marker": "PACKAGE HYGIENE VALIDATION PASS",
    },
    {
        "id": "continuous_mode",
        "command": ["python3.12", "tools/validate_lgo_continuous_development_mode.py"],
        "marker": "LGO_CONTINUOUS_DEVELOPMENT_MODE_VALIDATION_PASS",
    },
    {
        "id": "playable_source",
        "command": ["./tools/lgo_playable_closure_check.sh", "--source-only"],
        "marker": "LGO_PLAYABLE_CLOSURE_SOURCE_GATES_PASS",
    },
    {
        "id": "playable_package_ready",
        "command": ["./tools/lgo_playable_closure_check.sh", "--package-ready"],
        "marker": "LGO_PLAYABLE_CLOSURE_PACKAGE_READY",
    },
]

RUNTIME_GATES = [
    {
        "id": "playable_runtime",
        "command": ["./tools/lgo_playable_closure_check.sh", "--runtime"],
        "marker": "LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS",
    }
]


def run_gate(gate: dict[str, object]) -> dict[str, object]:
    command = list(gate["command"])
    result = subprocess.run(command, cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, check=False)
    output = result.stdout or ""
    marker = str(gate["marker"])
    return {
        "id": gate["id"],
        "command": command,
        "returnCode": result.returncode,
        "marker": marker,
        "markerObserved": marker in output,
        "status": "PASS" if result.returncode == 0 and marker in output else "FAIL",
    }


def main() -> int:
    parser = argparse.ArgumentParser(description="List or run the Linh Gioi runtime smoke matrix.")
    parser.add_argument("--phase", choices=("source", "runtime", "all"), default="source")
    parser.add_argument("--list", action="store_true")
    parser.add_argument("--json", action="store_true")
    args = parser.parse_args()

    gates = []
    if args.phase in ("source", "all"):
        gates.extend(SOURCE_GATES)
    if args.phase in ("runtime", "all"):
        gates.extend(RUNTIME_GATES)

    if args.list:
        payload = {"phase": args.phase, "gates": gates}
        print(json.dumps(payload, indent=2, sort_keys=True) if args.json else "\n".join(f"{gate['id']}: {' '.join(gate['command'])}" for gate in gates))
        return 0

    results = [run_gate(gate) for gate in gates]
    print(json.dumps({"phase": args.phase, "results": results}, indent=2, sort_keys=True))
    if any(result["status"] != "PASS" for result in results):
        return 1
    print("LGO_RUNTIME_SMOKE_MATRIX_RUN_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
