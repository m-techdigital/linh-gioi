#!/usr/bin/env python3
import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SCRIPT = ROOT / "tools/validate_lgo_ui_authority_matrix.py"


def surface(kind="owner"):
    return {
        "id": "entry-login",
        "designAuthority": {"kind": kind, "relativePath": "redesign-v5-entry/01-entry-login-CANONICAL.png"},
        "runtimeEvidence": ["build/whole-flow-p0-v1/pc/entry/entry-login.png"],
        "codeOwners": ["client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Entry.cs"],
        "states": ["default", "validation"],
        "profiles": ["pc", "tablet", "mobile-simulation"],
        "knownGaps": ["visual fidelity"],
    }

class AuthorityMatrixValidatorTests(unittest.TestCase):
    def run_validator(self, document):
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "matrix.json"
            path.write_text(json.dumps(document), encoding="utf-8")
            return subprocess.run(
                [sys.executable, str(SCRIPT), "--matrix", str(path)],
                cwd=ROOT, text=True, capture_output=True)

    def test_accepts_complete_owner_authority_surface(self):
        result = self.run_validator({"version": 1, "surfaces": [surface()]})
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)

    def test_rejects_runtime_evidence_as_design_authority(self):
        result = self.run_validator({"version": 1, "surfaces": [surface("runtime")]})
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("runtime evidence cannot be design authority", result.stdout + result.stderr)

    def test_rejects_missing_states_profiles_or_code_owners(self):
        broken = surface()
        broken["states"] = []
        broken["profiles"] = []
        broken["codeOwners"] = []
        result = self.run_validator({"version": 1, "surfaces": [broken]})
        self.assertNotEqual(result.returncode, 0)
        output = result.stdout + result.stderr
        self.assertIn("states", output)
        self.assertIn("profiles", output)
        self.assertIn("codeOwners", output)

    def test_rejects_duplicate_surface_ids(self):
        item = surface()
        result = self.run_validator({"version": 1, "surfaces": [item, dict(item)]})
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("duplicate surface id", result.stdout + result.stderr)


if __name__ == "__main__":
    unittest.main()
