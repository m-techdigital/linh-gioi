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


    def test_rejects_missing_uploaded_design_reference_scope(self) -> None:
        with self._copy_minimal_repo() as temp:
            agents = Path(temp) / "AGENTS.md"
            agents.write_text(
                agents.read_text(encoding="utf-8").replace(
                    "Design demo owner upload là visual reference cho toàn bộ login/entry, character select, HUD, inventory/bag, storage/chest, fashion/wardrobe, dialog và item-detail",
                    "Design demo chỉ tham khảo tùy màn",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("Design demo owner upload" in item for item in violations), violations)

    def test_rejects_missing_all_uploaded_designs_and_base_first_rule(self) -> None:
        with self._copy_minimal_repo() as temp:
            agents = Path(temp) / "AGENTS.md"
            agents.write_text(
                agents.read_text(encoding="utf-8").replace(
                    "Áp dụng cho toàn bộ design owner mới upload, không chỉ một màn riêng lẻ",
                    "Áp dụng tùy màn đang sửa",
                ).replace(
                    "Nếu UI/UX giống nhau, cập nhật base/shared component trước rồi mới bind data/state/action từng màn",
                    "Nếu UI giống nhau có thể copy nhanh theo màn",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("toàn bộ design owner mới upload" in item for item in violations), violations)
        self.assertTrue(any("cập nhật base/shared component trước" in item for item in violations), violations)

    def test_rejects_missing_separate_inventory_info_storage_flow_rule(self) -> None:
        with self._copy_minimal_repo() as temp:
            agents = Path(temp) / "AGENTS.md"
            agents.write_text(
                agents.read_text(encoding="utf-8").replace(
                    "Hành trang, Thông tin nhân vật và Rương đồ là các tab/flow riêng",
                    "Hành trang có thể gộp chung tùy nhanh",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("Hành trang, Thông tin" in item for item in violations), violations)

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

    def test_rejects_hud_action_buttons_that_skip_shared_skin(self) -> None:
        with self._copy_minimal_repo() as temp:
            hud = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs"
            hud.write_text(
                hud.read_text(encoding="utf-8").replace(
                    "ApplyLgoHudContextAction(_talk",
                    "ApplyLgoButton(_talk",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoHudContextAction(_talk" in item for item in violations), violations)


    def test_rejects_hud_combat_actions_that_skip_shared_action_base(self) -> None:
        with self._copy_minimal_repo() as temp:
            hud = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs"
            hud.write_text(
                hud.read_text(encoding="utf-8").replace(
                    "ApplyLgoHudCombatAction(button, _touch);",
                    "ApplyLgoButton(button);",
                    1,
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoHudCombatAction(button" in item for item in violations), violations)

    def test_rejects_hud_shortcuts_that_skip_shared_shortcut_base(self) -> None:
        with self._copy_minimal_repo() as temp:
            hud = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs"
            hud.write_text(
                hud.read_text(encoding="utf-8").replace(
                    "ApplyLgoHudShortcutAction(button, _touch);",
                    "ApplyLgoDisabledAction(button);",
                    1,
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoHudShortcutAction(button" in item for item in violations), violations)

    def test_rejects_dialogue_panel_that_skips_shared_skin(self) -> None:
        with self._copy_minimal_repo() as temp:
            hud = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs"
            hud.write_text(
                hud.read_text(encoding="utf-8").replace(
                    "ApplyLgoGlassPanel(_dialogue);",
                    "Box(_dialogue);",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoGlassPanel(_dialogue" in item for item in violations), violations)


    def test_rejects_new_ui_partial_that_creates_parallel_modal_or_dialog_skin(self) -> None:
        with self._copy_minimal_repo() as temp:
            rogue = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.RogueDialog.cs"
            rogue.write_text(
                """using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private static void StyleModalDialog(VisualElement panel)
        {
            panel.style.backgroundColor = new Color(.01f, .04f, .07f, .95f);
            panel.style.borderTopWidth = panel.style.borderBottomWidth = 1;
        }
    }
}
""",
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("parallel modal/dialog/card/tab/button/detail/panel skin helper" in item for item in violations), violations)


    def test_rejects_reusing_allowed_helper_name_outside_owning_partial(self) -> None:
        with self._copy_minimal_repo() as temp:
            rogue = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.RogueInventory.cs"
            rogue.write_text(
                """using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement InventoryPanel(string name)
        {
            var panel = new VisualElement { name = name };
            panel.style.backgroundColor = new Color(.01f, .04f, .07f, .95f);
            panel.style.borderTopWidth = panel.style.borderBottomWidth = 1;
            return panel;
        }
    }
}
""",
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("helper InventoryPanel is only allowed" in item for item in violations), violations)



if __name__ == "__main__":
    unittest.main()
