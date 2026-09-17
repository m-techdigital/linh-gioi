#!/usr/bin/env python3
import argparse
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_SOURCE = ROOT / "client/Unity/Assets/Game/UI/Runtime"
STYLE_ASSIGN = re.compile(r"\.style\.\w+\s*=")
NUMERIC_STYLE = re.compile(r"\.style\.\w+\s*=.*?(?<![A-Za-z_])[-+]?(?:\d+(?:\.\d*)?|\.\d+)f?(?![A-Za-z_])")
PALETTE_DECL = re.compile(r"(?m)^\s*(?:(?:private|internal|public)\s+)?static\s+readonly\s+Color\s+\w+\s*=\s*new\s+Color")
METHOD_DECL = re.compile(r"(?m)^\s*(?:private|internal|public|protected)\s+(?:static\s+)?(?:[\w<>\[\],.?]+)\s+\w+\s*\(")


def scan(source_root):
    source_root = Path(source_root)
    rows = []
    totals = {"directStyleAssignments": 0, "numericStyleAssignments": 0,
              "semanticPaletteDeclarations": 0, "skinPartialMaxLoc": 0, "skinPartialMaxMethods": 0}
    for path in sorted(source_root.glob("*.cs")):
        text = path.read_text(encoding="utf-8", errors="replace")
        lines = text.splitlines()
        direct = sum(1 for line in lines if STYLE_ASSIGN.search(line))
        numeric = sum(1 for line in lines if NUMERIC_STYLE.search(line))
        palette = len(PALETTE_DECL.findall(text))
        methods = len(METHOD_DECL.findall(text))
        row = {"file": path.name, "loc": len(lines), "methods": methods,
               "directStyleAssignments": direct, "numericStyleAssignments": numeric,
               "semanticPaletteDeclarations": palette}
        rows.append(row)
        for key in ("directStyleAssignments", "numericStyleAssignments", "semanticPaletteDeclarations"):
            totals[key] += row[key]
        if path.name.startswith("CongDongLamArrivalHud.Skin"):
            totals["skinPartialMaxLoc"] = max(totals["skinPartialMaxLoc"], len(lines))
            totals["skinPartialMaxMethods"] = max(totals["skinPartialMaxMethods"], methods)
    return {"metrics": totals, "files": rows}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--source-root", default=str(DEFAULT_SOURCE))
    parser.add_argument("--output")
    args = parser.parse_args()
    report = scan(args.source_root)
    payload = json.dumps(report, ensure_ascii=False, indent=2) + "\n"
    if args.output:
        Path(args.output).write_text(payload, encoding="utf-8")
    else:
        print(payload, end="")


if __name__ == "__main__":
    main()
