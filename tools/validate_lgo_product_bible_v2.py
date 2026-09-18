#!/usr/bin/env python3
from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CONTRACT = ROOT / "docs/LGO-PRODUCT-BIBLE-v2.json"
CANONICAL_CLASSES = ["vo", "kiem", "phap", "co", "linh"]
LEGACY_ALIASES = {"class.martial": "vo", "class.sword": "kiem"}


def load_contract(path: Path = CONTRACT) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def _require_text(root: Path, rel: str, markers: list[str], errors: list[str]) -> None:
    path = root / rel
    if not path.is_file():
        errors.append(f"missing authority source: {rel}")
        return
    text = path.read_text(encoding="utf-8", errors="replace")
    for marker in markers:
        if marker not in text:
            errors.append(f"{rel} missing authority marker: {marker}")


def validate_contract(contract: dict) -> list[str]:
    errors: list[str] = []
    if contract.get("version") != 2:
        errors.append("version must be 2")
    identity = contract.get("classIdentity") or {}
    if identity.get("canonicalNewWriteIds") != CANONICAL_CLASSES:
        errors.append("classIdentity.canonicalNewWriteIds must be exactly vo, kiem, phap, co, linh")
    if identity.get("legacyReadAliases") != LEGACY_ALIASES:
        errors.append("classIdentity.legacyReadAliases must preserve class.martial→vo and class.sword→kiem")
    alpha = contract.get("founderAlpha") or {}
    if alpha.get("combatCompleteStageIds") != ["vo", "kiem"]:
        errors.append("founderAlpha.combatCompleteStageIds must be exactly vo, kiem")
    if alpha.get("limitsIdentityCatalog") is not False:
        errors.append("founderAlpha.limitsIdentityCatalog must be false")
    maps = contract.get("mapMachineIds") or {}
    city = maps.get("map.city.linh_thanh") or {}
    map01a = maps.get("map-01a-cong-dong-lam") or {}
    if city.get("kind") != "content-zone":
        errors.append("map.city.linh_thanh must be a content-zone ID")
    if map01a.get("kind") != "runtime-playable-map":
        errors.append("map-01a-cong-dong-lam must be a runtime-playable-map ID")
    if map01a.get("aliasOfCityId") is not False:
        errors.append("map-01a-cong-dong-lam must explicitly not alias the city ID")
    if not isinstance(contract.get("supersessionRegister"), list) or not contract["supersessionRegister"]:
        errors.append("supersessionRegister must be non-empty")
    return errors


def validate_repository(root: Path = ROOT) -> list[str]:
    errors: list[str] = []
    if not (root / "docs/LGO-PRODUCT-BIBLE-v2.json").is_file():
        return ["missing docs/LGO-PRODUCT-BIBLE-v2.json"]
    contract = load_contract(root / "docs/LGO-PRODUCT-BIBLE-v2.json")
    errors.extend(validate_contract(contract))
    _require_text(root, "docs/LGO-PRODUCT-BIBLE-v2.md", [
        "Founder Alpha staging is capability staging, not identity-schema staging",
        "map.city.linh_thanh",
        "map-01a-cong-dong-lam",
        "Canonical new-write IDs",
        "Supersession register",
    ], errors)
    _require_text(root, "docs/00-VISION.md", [
        "2 combat-complete Paths/classes at Founder Alpha launch",
        "five canonical product identities",
    ], errors)
    _require_text(root, "docs/02-GDD.md", [
        "Canonical product identity/new-write IDs",
        "`vo`, `kiem`, `phap`, `co`, `linh`",
        "Founder Alpha combat-complete staging is Võ + Kiếm",
    ], errors)
    _require_text(root, "docs/12-CONTENT-ID-REGISTRY.md", [
        "Product/account class identity IDs",
        "Runtime playable map IDs",
        "not aliases",
    ], errors)
    _require_text(root, "server/api/src/main/java/com/linhgioi/server/api/persistence/CharacterClassCompatibility.java", [
        'Set.of("vo", "kiem", "phap", "co", "linh")',
        '"class.martial".equals(classId)',
        '"class.sword".equals(classId)',
    ], errors)
    _require_text(root, "server/api/src/main/java/com/linhgioi/server/api/persistence/CharacterRuntimeState.java", [
        'MAP01A_ID = "map-01a-cong-dong-lam"',
    ], errors)
    _require_text(root, "client/Unity/Assets/Game/World/Runtime/Map01ACharacterEntryMapper.cs", [
        'MapId = "map-01a-cong-dong-lam"',
    ], errors)
    _require_text(root, "gamedata/registry.yaml", [
        "map.city.linh_thanh",
        "class.sword",
        "class.martial",
    ], errors)
    _require_text(root, "tools/lgo_playable_closure_check.sh", [
        "product_bible_v2",
        "validate_lgo_product_bible_v2.py",
    ], errors)
    _require_text(root, "docs/design/LGO-2D-SOCIAL-ACTION-DIRECTION-LOCK-v0.1.md", [
        "## 5 class trong 2D",
        "Đánh Shadow Slime",
    ], errors)
    return errors


def main() -> int:
    errors = validate_repository(ROOT)
    if errors:
        print("LGO_PRODUCT_BIBLE_V2_FAIL", file=sys.stderr)
        for error in errors:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_PRODUCT_BIBLE_V2_PASS canonical=vo,kiem,phap,co,linh alpha=vo,kiem map01a=runtime-playable-map")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
