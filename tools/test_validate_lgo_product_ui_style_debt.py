#!/usr/bin/env python3
import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
VALIDATOR = ROOT / "tools/validate_lgo_product_ui_style_debt.py"


class ProductUiStyleDebtValidatorTests(unittest.TestCase):
    def run_validator(self, source_text, budget):
        with tempfile.TemporaryDirectory() as tmp:
            tmp = Path(tmp)
            source_root = tmp / "Runtime"
            source_root.mkdir()
            (source_root / "CongDongLamArrivalHud.Skin.Fixture.cs").write_text(source_text, encoding="utf-8")
            budget_path = tmp / "budget.json"
            budget_path.write_text(json.dumps(budget), encoding="utf-8")
            return subprocess.run([
                sys.executable, str(VALIDATOR),
                "--source-root", str(source_root),
                "--budget", str(budget_path),
            ], cwd=ROOT, text=True, capture_output=True)

    def test_accepts_source_at_or_below_budget(self):
        budget = {"directStyleAssignments": 1, "numericStyleAssignments": 1,
                  "semanticPaletteDeclarations": 0, "skinPartialMaxLoc": 20, "skinPartialMaxMethods": 20}
        result = self.run_validator("class X { void M(){ x.style.width = 10; } }", budget)
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)

    def test_rejects_style_and_palette_regression(self):
        budget = {"directStyleAssignments": 0, "numericStyleAssignments": 0,
                  "semanticPaletteDeclarations": 0, "skinPartialMaxLoc": 20, "skinPartialMaxMethods": 20}
        source = """using UnityEngine;
class X {
 static readonly Color LocalGold = new Color(1f, .7f, .2f, 1f);
 void M(){ x.style.width = 10; }
}"""
        result = self.run_validator(source, budget)
        self.assertNotEqual(result.returncode, 0)
        output = result.stdout + result.stderr
        self.assertIn("directStyleAssignments", output)
        self.assertIn("numericStyleAssignments", output)
        self.assertIn("semanticPaletteDeclarations", output)

    def test_rejects_oversized_skin_partial(self):
        budget = {"directStyleAssignments": 0, "numericStyleAssignments": 0,
                  "semanticPaletteDeclarations": 0, "skinPartialMaxLoc": 3, "skinPartialMaxMethods": 20}
        source = "\n".join(["partial class CongDongLamArrivalHud {", "", "", "}"])
        result = self.run_validator(source, budget)
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("skinPartialMaxLoc", result.stdout + result.stderr)


if __name__ == "__main__":
    unittest.main()
