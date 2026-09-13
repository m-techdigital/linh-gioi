#!/usr/bin/env python3.12
"""Validate a six-pose outfit surface contract before source authoring or Player pack."""
from __future__ import annotations

import argparse
import json
from pathlib import Path

EXPECTED_PROFILE = {
    "canvas": [1024, 1536],
    "originX": 512,
    "groundY": 1484,
    "unitScale": "1.70/1536",
}
POSES = ["idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck"]
SLOTS = {
    "main_weapon",
    "head_hair",
    "inner_top",
    "outer_top",
    "lower_body",
    "waist_belt",
    "arm_guard",
    "footwear",
    "shoulder_chest_guard",
    "class_accessory",
}
FAMILY_TYPES = {"rigid", "part_rigid", "cloth_body", "pose_authored"}
ROUTES = {"SLEEVELESS_PHAP_LV1", "SLEEVED_PHAP_LV1"}


def _family_report(family: dict, selected_route: str | None) -> dict:
    failures: list[str] = []
    slot = family.get("slotId")
    family_type = family.get("familyType")
    ownership = set(family.get("ownership") or [])
    pose_masks = family.get("poseMasks") or {}

    if slot not in SLOTS:
        failures.append("UNKNOWN_SLOT")
    if family_type not in FAMILY_TYPES:
        failures.append("UNKNOWN_FAMILY_TYPE")
    if not ownership:
        failures.append("OWNERSHIP_REQUIRED")

    if slot == "outer_top" and selected_route == "SLEEVED_PHAP_LV1":
        if "upper_arm_cloth" not in ownership:
            failures.append("SLEEVED_ROUTE_REQUIRES_UPPER_ARM_CLOTH_OWNERSHIP")
        missing_masks = [pose for pose in POSES if pose not in pose_masks]
        if missing_masks:
            failures.append("SLEEVED_ROUTE_REQUIRES_POSE_MASKS")
    if slot == "outer_top" and selected_route == "SLEEVELESS_PHAP_LV1":
        if "upper_arm_cloth" in ownership:
            failures.append("SLEEVELESS_ROUTE_MUST_NOT_OWN_UPPER_ARM_CLOTH")
        if family.get("routeDependency") != "IDLE_NATIVE_SOURCE_MUST_REMOVE_SLEEVES":
            failures.append("SLEEVELESS_ROUTE_REQUIRES_NATIVE_IDLE_SOURCE_DECISION")

    return {
        "slotId": slot,
        "familyType": family_type,
        "ownership": sorted(ownership),
        "failures": failures,
    }


def validate_contract(contract_path: Path | str) -> dict:
    contract_path = Path(contract_path).resolve()
    contract = json.loads(contract_path.read_text(encoding="utf-8"))
    global_failures: list[str] = []
    decision_gates: list[str] = []

    profile = contract.get("sourceSpaceProfile") or {}
    for key, expected in EXPECTED_PROFILE.items():
        if profile.get(key) != expected:
            global_failures.append(f"SOURCE_PROFILE_{key.upper()}_MISMATCH")

    if contract.get("poses") != POSES:
        global_failures.append("POSE_SET_MISMATCH")

    selected_route = contract.get("selectedRoute")
    if selected_route is None:
        decision_gates.append("ROUTE_SELECTION_REQUIRED")
    elif selected_route not in ROUTES:
        global_failures.append("UNKNOWN_SELECTED_ROUTE")

    if contract.get("runtimePromotionAllowed") is not False:
        global_failures.append("RUNTIME_PROMOTION_MUST_REMAIN_FALSE")

    families = [_family_report(family, selected_route) for family in contract.get("itemFamilies") or []]
    if not families:
        global_failures.append("NO_ITEM_FAMILIES")
    family_failures = sum(len(item["failures"]) for item in families)
    failure_count = len(global_failures) + family_failures

    if failure_count:
        status = "REJECT_OUTFIT_SURFACE_CONTRACT"
    elif decision_gates:
        status = "NEED_OWNER_DECISION"
    else:
        status = "PASS"

    return {
        "gateId": "LGO_OUTFIT_SURFACE_CONTRACT_V1",
        "status": status,
        "contract": str(contract_path),
        "selectedRoute": selected_route,
        "decisionGates": decision_gates,
        "globalFailures": global_failures,
        "families": families,
        "failureCount": failure_count,
        "runtimePromotionAllowed": False,
    }


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("contract", type=Path)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args(argv)
    report = validate_contract(args.contract)
    text = json.dumps(report, ensure_ascii=False, indent=2) + "\n"
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(text, encoding="utf-8")
    print(text, end="")
    return 0 if report["status"] in {"PASS", "NEED_OWNER_DECISION"} else 2


if __name__ == "__main__":
    raise SystemExit(main())
