#!/usr/bin/env python3
"""Audit OpenRaster AUTHOR layers for real registered-canvas content."""

import argparse
import io
import json
import zipfile
from pathlib import Path
from xml.etree import ElementTree

from PIL import Image


EXPECTED_CANVAS = (1024, 1536)


def audit_ora_author_layers(source):
    source = Path(source).resolve()
    layers = []
    global_failures = []
    with zipfile.ZipFile(source) as archive:
        root = ElementTree.fromstring(archive.read("stack.xml"))
        canvas = (int(root.attrib.get("w", 0)), int(root.attrib.get("h", 0)))
        if canvas != EXPECTED_CANVAS:
            global_failures.append("DOCUMENT_CANVAS_MISMATCH")
        for node in root.findall(".//layer"):
            name = node.attrib.get("name", "")
            if not name.startswith("AUTHOR -"):
                continue
            failures = []
            layer_path = node.attrib.get("src", "")
            with Image.open(io.BytesIO(archive.read(layer_path))) as image:
                rgba = image.convert("RGBA")
                alpha = rgba.getchannel("A")
                non_transparent = sum(1 for value in alpha.getdata() if value)
                if rgba.size != EXPECTED_CANVAS:
                    failures.append("LAYER_CANVAS_MISMATCH")
                if non_transparent == 0:
                    failures.append("EMPTY_AUTHOR_LAYER")
                bbox = alpha.getbbox()
                layers.append({
                    "name": name,
                    "path": layer_path,
                    "size": list(rgba.size),
                    "alphaBbox": list(bbox) if bbox else None,
                    "nonTransparentPixels": non_transparent,
                    "failures": failures,
                })
    if not layers:
        global_failures.append("NO_AUTHOR_LAYERS")
    failure_count = len(global_failures) + sum(len(layer["failures"]) for layer in layers)
    return {
        "gateId": "LGO_ORA_AUTHOR_LAYER_CONTENT_01",
        "source": str(source),
        "documentName": root.attrib.get("name"),
        "canvas": list(canvas),
        "status": "PASS" if failure_count == 0 else "REJECT_EMPTY_AUTHORING_LAYERS",
        "globalFailures": global_failures,
        "layers": layers,
        "failureCount": failure_count,
        "runtimePromotionAllowed": False,
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("source", type=Path)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    report = audit_ora_author_layers(args.source)
    text = json.dumps(report, indent=2) + "\n"
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(text, encoding="utf-8")
    print(text, end="")
    raise SystemExit(0 if report["status"] == "PASS" else 2)


if __name__ == "__main__":
    main()
