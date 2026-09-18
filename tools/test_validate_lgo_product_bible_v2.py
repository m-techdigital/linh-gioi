#!/usr/bin/env python3
from __future__ import annotations

import copy
import json
import unittest
from pathlib import Path

from validate_lgo_product_bible_v2 import load_contract, validate_contract, validate_repository

ROOT = Path(__file__).resolve().parents[1]


class ProductBibleV2ValidatorTests(unittest.TestCase):
    def test_repository_closes_founder_alpha_and_id_conflicts(self):
        errors = validate_repository(ROOT)
        self.assertEqual([], errors, "\n".join(errors))

    def test_five_canonical_new_write_ids_are_exact_and_ordered(self):
        contract = load_contract(ROOT / "docs/LGO-PRODUCT-BIBLE-v2.json")
        self.assertEqual(
            ["vo", "kiem", "phap", "co", "linh"],
            contract["classIdentity"]["canonicalNewWriteIds"],
        )
        self.assertEqual(
            {"class.martial": "vo", "class.sword": "kiem"},
            contract["classIdentity"]["legacyReadAliases"],
        )

    def test_two_class_founder_alpha_means_combat_staging_not_identity_catalog(self):
        contract = load_contract(ROOT / "docs/LGO-PRODUCT-BIBLE-v2.json")
        self.assertEqual(["vo", "kiem"], contract["founderAlpha"]["combatCompleteStageIds"])
        self.assertEqual(5, len(contract["classIdentity"]["canonicalNewWriteIds"]))
        self.assertFalse(contract["founderAlpha"]["limitsIdentityCatalog"])

    def test_city_and_map01a_ids_are_distinct_machine_id_kinds(self):
        contract = load_contract(ROOT / "docs/LGO-PRODUCT-BIBLE-v2.json")
        ids = contract["mapMachineIds"]
        self.assertEqual("content-zone", ids["map.city.linh_thanh"]["kind"])
        self.assertEqual("runtime-playable-map", ids["map-01a-cong-dong-lam"]["kind"])
        self.assertFalse(ids["map-01a-cong-dong-lam"]["aliasOfCityId"])

    def test_validator_rejects_identity_catalog_collapsed_to_two(self):
        contract = load_contract(ROOT / "docs/LGO-PRODUCT-BIBLE-v2.json")
        broken = copy.deepcopy(contract)
        broken["classIdentity"]["canonicalNewWriteIds"] = ["vo", "kiem"]
        errors = validate_contract(broken)
        self.assertTrue(any("canonicalNewWriteIds" in error for error in errors), errors)


if __name__ == "__main__":
    unittest.main()
