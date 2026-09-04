from __future__ import annotations

import importlib.util
import json
import shutil
import tempfile
import unittest
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SPEC = importlib.util.spec_from_file_location("validate_gamedata", ROOT / "tools" / "validate_gamedata.py")
MODULE = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = MODULE
assert SPEC.loader is not None
SPEC.loader.exec_module(MODULE)


class GameDataPipelineTest(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.gd = Path(self.tmp.name) / "gamedata"
        shutil.copytree(ROOT / "gamedata", self.gd)

    def tearDown(self):
        self.tmp.cleanup()

    def validate(self):
        return MODULE.validate_gamedata(self.gd)

    def test_current_content_validates(self):
        result = self.validate()
        self.assertTrue(result.valid, "\n".join(result.errors))
        self.assertEqual(4, result.manifest["content_count"])
        self.assertEqual(1, result.manifest["gamedata_version"])

    def test_duplicate_id_is_rejected(self):
        shutil.copy(self.gd / "skills" / "wind_slash.yaml", self.gd / "skills" / "duplicate.yaml")
        result = self.validate()
        self.assertFalse(result.valid)
        self.assertTrue(any("duplicate id" in item for item in result.errors), result.errors)

    def test_skill_bound_is_rejected(self):
        path = self.gd / "skills" / "wind_slash.yaml"
        text = path.read_text().replace("cooldown_ms: 6000", "cooldown_ms: -1")
        path.write_text(text)
        result = self.validate()
        self.assertFalse(result.valid)
        self.assertTrue(any("cooldown_ms" in item for item in result.errors), result.errors)

    def test_skill_activation_rule_is_rejected(self):
        path = self.gd / "skills" / "wind_slash.yaml"
        path.write_text(path.read_text().replace("mode: instant", "mode: forbidden"))
        result = self.validate()
        self.assertFalse(result.valid)
        self.assertTrue(any("activation.mode" in item for item in result.errors), result.errors)

    def test_skill_cooldown_rule_is_rejected(self):
        path = self.gd / "skills" / "wind_slash.yaml"
        path.write_text(path.read_text().replace("skill_ms: 6000", "skill_ms: -1"))
        result = self.validate()
        self.assertFalse(result.valid)
        self.assertTrue(any("cooldown.skill_ms" in item for item in result.errors), result.errors)

    def test_skill_targeting_rule_is_rejected(self):
        path = self.gd / "skills" / "wind_slash.yaml"
        path.write_text(path.read_text().replace("max_range_m: 4.5", "max_range_m: 101"))
        result = self.validate()
        self.assertFalse(result.valid)
        self.assertTrue(any("targeting.max_range_m" in item for item in result.errors), result.errors)

    def test_invalid_class_reference_is_rejected(self):
        path = self.gd / "skills" / "wind_slash.yaml"
        path.write_text(path.read_text().replace("class.sword", "class.unknown"))
        result = self.validate()
        self.assertFalse(result.valid)
        self.assertTrue(any("unknown class" in item for item in result.errors), result.errors)

    def test_invalid_map_reference_is_rejected(self):
        path = self.gd / "events" / "shadow_invasion.yaml"
        path.write_text(path.read_text().replace("map.city.linh_thanh", "map.city.missing"))
        result = self.validate()
        self.assertFalse(result.valid)
        self.assertTrue(any("unknown map" in item for item in result.errors), result.errors)

    def test_same_source_compiles_byte_identically(self):
        first = self.validate()
        second = self.validate()
        self.assertTrue(first.valid)
        self.assertEqual(first.manifest, second.manifest)
        a = json.dumps(first.manifest, ensure_ascii=False, sort_keys=True, indent=2) + "\n"
        b = json.dumps(second.manifest, ensure_ascii=False, sort_keys=True, indent=2) + "\n"
        self.assertEqual(a.encode(), b.encode())


if __name__ == "__main__":
    unittest.main()
