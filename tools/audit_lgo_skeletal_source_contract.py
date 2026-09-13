#!/usr/bin/env python3
"""Reject invalid skeletal source components before layer import or runtime work."""

import argparse
import json
from pathlib import Path

from audit_lgo_skeletal_blueprint_package import PngAuditError, _read_png


EXPECTED_CANVAS = {"width": 1024, "height": 1536, "originX": 512, "groundY": 1484}
SLOT_OWNERSHIP = {
    "body": {"head", "neck", "torso", "upper_arm", "forearm", "hand", "pelvis", "thigh", "shin", "foot"},
    "upper": {"neck", "torso", "upper_arm", "forearm"},
    "lower": {"pelvis", "thigh", "shin"},
    "footwear": {"foot", "shin"},
    "waist": {"pelvis", "waist"},
    "rigid_hand_item": {"hand_socket"},
}


def _component_report(component, base_dir, canvas):
    failures = []
    image_path = (base_dir / component.get("path", "")).resolve()
    slot = component.get("slot")
    ownership = set(component.get("ownership", []))

    if slot not in SLOT_OWNERSHIP:
        failures.append("UNKNOWN_SLOT")
    elif not ownership or not ownership <= SLOT_OWNERSHIP[slot]:
        failures.append("OWNERSHIP_CROSSES_SLOT_BOUNDARY")

    report = {
        "id": component.get("id"),
        "path": str(image_path),
        "slot": slot,
        "ownership": sorted(ownership),
        "failures": failures,
    }
    if not image_path.is_file():
        failures.append("SOURCE_FILE_MISSING")
        return report

    try:
        png = _read_png(image_path)
        report["width"] = png["width"]
        report["height"] = png["height"]
        report["hasAlpha"] = png["hasAlpha"]
        if (png["width"], png["height"]) != (canvas["width"], canvas["height"]):
            failures.append("CANVAS_DIMENSIONS_MISMATCH")
        if not png["hasAlpha"]:
            failures.append("MISSING_ALPHA_CHANNEL")
        else:
            alpha_min, alpha_max = png["alphaExtrema"]
            report["alphaExtrema"] = [alpha_min, alpha_max]
            if alpha_min == 255:
                failures.append("NO_TRANSPARENT_PIXELS")
            if alpha_max == 0:
                failures.append("NO_VISIBLE_PIXELS")
    except (OSError, PngAuditError, ValueError):
        failures.append("IMAGE_DECODE_FAILED")
    return report


def audit_source_contract(manifest_path):
    manifest_path = Path(manifest_path).resolve()
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    canvas = manifest.get("canvas", {})
    global_failures = []
    for key, expected in EXPECTED_CANVAS.items():
        if canvas.get(key) != expected:
            global_failures.append(f"CANVAS_{key.upper()}_MISMATCH")

    components = [
        _component_report(component, manifest_path.parent, canvas)
        for component in manifest.get("components", [])
    ]
    if not components:
        global_failures.append("NO_COMPONENTS")
    failure_count = len(global_failures) + sum(len(item["failures"]) for item in components)
    return {
        "gateId": "LGO_SKELETAL_SOURCE_CONTRACT_01",
        "status": "PASS" if failure_count == 0 else "REJECT_SOURCE_CONTRACT",
        "manifest": str(manifest_path),
        "expectedCanvas": EXPECTED_CANVAS,
        "globalFailures": global_failures,
        "components": components,
        "failureCount": failure_count,
        "runtimePromotionAllowed": False,
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("manifest", type=Path)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    report = audit_source_contract(args.manifest)
    text = json.dumps(report, indent=2) + "\n"
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(text, encoding="utf-8")
    print(text, end="")
    raise SystemExit(0 if report["status"] == "PASS" else 2)


if __name__ == "__main__":
    main()
