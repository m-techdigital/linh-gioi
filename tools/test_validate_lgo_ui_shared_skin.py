#!/usr/bin/env python3
"""Regression tests for the Map01A shared UI governance validator."""
from __future__ import annotations

import shutil
import tempfile
import unittest
from pathlib import Path

import validate_lgo_ui_shared_skin as validator

ROOT = Path(__file__).resolve().parents[1]


class ValidateLgoUiSharedSkinTests(unittest.TestCase):
    def _copy_minimal_repo(self) -> tempfile.TemporaryDirectory[str]:
        temp = tempfile.TemporaryDirectory()
        dst = Path(temp.name)
        (dst / "client/Unity/Assets/Game/UI/Runtime").mkdir(parents=True)
        for path in (ROOT / "client/Unity/Assets/Game/UI/Runtime").glob("CongDongLamArrivalHud*.cs"):
            shutil.copy2(path, dst / "client/Unity/Assets/Game/UI/Runtime" / path.name)
        tests_src = ROOT / "client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs"
        tests_dst = dst / "client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs"
        tests_dst.parent.mkdir(parents=True)
        shutil.copy2(tests_src, tests_dst)
        shutil.copy2(ROOT / "AGENTS.md", dst / "AGENTS.md")
        return temp

    def test_current_repo_satisfies_shared_ui_governance(self) -> None:
        self.assertEqual([], validator.validate_root(ROOT))

    def test_rejects_missing_project_rule_for_shared_ui_and_right_detail(self) -> None:
        with self._copy_minimal_repo() as temp:
            agents = Path(temp) / "AGENTS.md"
            agents.write_text(agents.read_text(encoding="utf-8").replace("detail món đặt ở panel phải theo design đã chốt", "detail món có thể đặt tùy màn"), encoding="utf-8")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("AGENTS.md" in item and "detail món" in item for item in violations), violations)

    def test_rejects_inventory_detail_added_before_shared_content_columns(self) -> None:
        with self._copy_minimal_repo() as temp:
            inventory = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Inventory.cs"
            text = inventory.read_text(encoding="utf-8")
            text = text.replace(
                "body.Add(_inventoryGridPanel);",
                "body.Add(_inventoryDetailPanel);\n            body.Add(_inventoryGridPanel);",
                1,
            )
            text = text.replace("\n            body.Add(_inventoryDetailPanel);\n\n            ShowInventoryPage", "\n\n            ShowInventoryPage", 1)
            inventory.write_text(text, encoding="utf-8")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("Inventory detail panel must be added after" in item for item in violations), violations)


if __name__ == "__main__":
    unittest.main()
