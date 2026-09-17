#!/usr/bin/env python3
import argparse
import json
from pathlib import Path

from report_lgo_product_ui_style_debt import DEFAULT_SOURCE, scan

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_BUDGET = ROOT / "docs/design/LGO-UI-STYLE-DEBT-BUDGET-v1.0.json"
BUDGET_KEYS = ("directStyleAssignments", "numericStyleAssignments",
               "semanticPaletteDeclarations", "skinPartialMaxLoc", "skinPartialMaxMethods")


def validate(report, budget):
    errors = []
    metrics = report["metrics"]
    for key in BUDGET_KEYS:
        if key not in budget:
            errors.append("budget missing " + key)
            continue
        maximum = budget[key]
        if not isinstance(maximum, int) or maximum < 0:
            errors.append("budget " + key + " must be a non-negative integer")
            continue
        actual = metrics.get(key, 0)
        if actual > maximum:
            errors.append(f"{key} actual={actual} budget={maximum}")
    return errors


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--source-root", default=str(DEFAULT_SOURCE))
    parser.add_argument("--budget", default=str(DEFAULT_BUDGET))
    args = parser.parse_args()
    report = scan(args.source_root)
    budget = json.loads(Path(args.budget).read_text(encoding="utf-8"))
    errors = validate(report, budget)
    if errors:
        print("LGO_PRODUCT_UI_STYLE_DEBT_FAIL")
        for error in errors:
            print("- " + error)
        raise SystemExit(1)
    metrics = report["metrics"]
    print("LGO_PRODUCT_UI_STYLE_DEBT_PASS " + " ".join(
        f"{key}={metrics[key]}" for key in BUDGET_KEYS))


if __name__ == "__main__":
    main()
