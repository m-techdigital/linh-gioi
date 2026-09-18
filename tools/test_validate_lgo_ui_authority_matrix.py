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
    def run_validator(self, document, owner_root=None, strict=False):
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "matrix.json"
            path.write_text(json.dumps(document), encoding="utf-8")
            command = [sys.executable, str(SCRIPT), "--matrix", str(path)]
            if owner_root is not None:
                command.extend(["--owner-root", str(owner_root)])
            if strict:
                command.append("--require-authority-files")
            return subprocess.run(command, cwd=ROOT, text=True, capture_output=True)

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

    def test_strict_mode_requires_owner_root_for_owner_authority(self):
        item = surface()
        item["designAuthority"]["sha256"] = "0" * 64
        result = self.run_validator({"version": 1, "surfaces": [item]}, strict=True)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("owner root is required", result.stdout + result.stderr)

    def test_strict_mode_verifies_authority_hash_code_owner_and_runtime_evidence(self):
        import hashlib
        with tempfile.TemporaryDirectory() as tmp:
            owner_root = Path(tmp)
            authority = owner_root / "redesign-v5-entry/01-entry-login-CANONICAL.png"
            authority.parent.mkdir(parents=True)
            authority.write_bytes(b"owner-canonical")
            item = surface()
            item["designAuthority"]["sha256"] = hashlib.sha256(authority.read_bytes()).hexdigest()
            item["runtimeEvidence"] = ["docs/design/LGO-MAP01A-ENTRY-SCREEN-CONTRACT-v1.0.md"]
            result = self.run_validator({"version": 1, "surfaces": [item]},
                                        owner_root=owner_root, strict=True)
            self.assertEqual(result.returncode, 0, result.stdout + result.stderr)

            broken = json.loads(json.dumps(item))
            broken["codeOwners"] = ["client/Unity/Assets/Game/UI/Runtime/DOES-NOT-EXIST.cs"]
            result = self.run_validator({"version": 1, "surfaces": [broken]},
                                        owner_root=owner_root, strict=True)
            self.assertNotEqual(result.returncode, 0)
            self.assertIn("missing code owner", result.stdout + result.stderr)

            broken = json.loads(json.dumps(item))
            broken["runtimeEvidence"] = ["build/does-not-exist.png"]
            result = self.run_validator({"version": 1, "surfaces": [broken]},
                                        owner_root=owner_root, strict=True)
            self.assertNotEqual(result.returncode, 0)
            self.assertIn("missing runtime evidence", result.stdout + result.stderr)


if __name__ == "__main__":
    unittest.main()
