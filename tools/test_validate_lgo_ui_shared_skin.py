#!/usr/bin/env python3
"""Regression tests for the Map01A shared UI governance validator."""
from __future__ import annotations

import shutil
import re
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
        shutil.copy2(
            ROOT / "client/Unity/Assets/Game/UI/Runtime/CharacterHubPotentialTopology.cs",
            dst / "client/Unity/Assets/Game/UI/Runtime/CharacterHubPotentialTopology.cs",
        )
        shutil.copy2(
            ROOT / "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
            dst / "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        )
        shutil.copy2(
            ROOT / "client/Unity/Assets/Game/UI/Runtime/CharacterHubClassCatalog.cs",
            dst / "client/Unity/Assets/Game/UI/Runtime/CharacterHubClassCatalog.cs",
        )
        tests_src = ROOT / "client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs"
        tests_dst = dst / "client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs"
        tests_dst.parent.mkdir(parents=True)
        shutil.copy2(tests_src, tests_dst)
        shutil.copy2(ROOT / "AGENTS.md", dst / "AGENTS.md")
        return temp

    def _skin_part_containing(self, root: Path, marker: str) -> Path:
        ui_dir = root / "client/Unity/Assets/Game/UI/Runtime"
        parts = list(ui_dir.glob("CongDongLamArrivalHud.Skin*.cs"))
        definition = re.compile(r"private\s+static\s+[^\n{;]*\b" + re.escape(marker) + r"\s*\(")
        owners = [path for path in parts if definition.search(path.read_text(encoding="utf-8"))]
        if len(owners) == 1:
            return owners[0]
        matches = [path for path in parts if marker in path.read_text(encoding="utf-8")]
        self.assertEqual(1, len(matches), f"Expected exactly one skin owner for {marker}: {matches}")
        return matches[0]

    def _replace_skin_marker(self, root: Path, marker: str, replacement: str) -> None:
        ui_dir = root / "client/Unity/Assets/Game/UI/Runtime"
        replaced = 0
        for path in ui_dir.glob("CongDongLamArrivalHud.Skin*.cs"):
            text = path.read_text(encoding="utf-8")
            count = text.count(marker)
            if count:
                path.write_text(text.replace(marker, replacement), encoding="utf-8")
                replaced += count
        self.assertGreater(replaced, 0, f"Expected shared skin marker {marker}")

    def test_split_skin_partials_form_one_shared_skin_owner(self) -> None:
        with self._copy_minimal_repo() as temp:
            root = Path(temp)
            skin_parts = sorted((root / "client/Unity/Assets/Game/UI/Runtime").glob("CongDongLamArrivalHud.Skin*.cs"))
            self.assertGreater(len(skin_parts), 1)
            self.assertEqual([], validator.validate_root(root))

    def test_all_equipment_surfaces_keep_shared_content_and_missing_state(self) -> None:
        markers = (
            "BindLgoItemIconContent(_equipmentRowIcons[index],",
            "BindLgoItemIconContent(_equipmentTileIcons[index],",
            "BindLgoItemIconContent(_characterHeroQuickIcons[i],",
            "BindLgoItemIconContent(_inventoryDetailIcon, thumbnail,",
            "BindLgoItemIconContent(_inventoryDetailIcon, itemSprite,",
        )
        for marker in markers:
            with self.subTest(surface=marker), self._copy_minimal_repo() as temp:
                inventory = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Inventory.cs"
                self.assertEqual([], validator.validate_root(Path(temp)), "The unmodified fixture must be valid before mutation.")
                text = inventory.read_text(encoding="utf-8")
                self.assertEqual(1, text.count(marker))
                inventory.write_text(text.replace(marker, marker.replace("BindLgoItemIconContent", "OneOffItemArt")), encoding="utf-8")
                violations = validator.validate_root(Path(temp))
                self.assertTrue(any(marker in item for item in violations), violations)

    def test_current_repo_satisfies_shared_ui_governance(self) -> None:
        self.assertEqual([], validator.validate_root(ROOT))

    def test_rejects_missing_project_rule_for_shared_ui_and_right_detail(self) -> None:
        with self._copy_minimal_repo() as temp:
            agents = Path(temp) / "AGENTS.md"
            agents.write_text(agents.read_text(encoding="utf-8").replace("detail món đặt ở panel phải theo design đã chốt", "detail món có thể đặt tùy màn"), encoding="utf-8")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("AGENTS.md" in item and "detail món" in item for item in violations), violations)

    def test_rejects_return_to_superseded_class_goal(self) -> None:
        with self._copy_minimal_repo() as temp:
            agents = Path(temp) / "AGENTS.md"
            agents.write_text(
                agents.read_text(encoding="utf-8").replace(
                    "Mọi objective/autopilot prompt cũ còn nhắc class, pose, wardrobe hoặc source art đã bị owner thay thế",
                    "Objective cũ có thể dùng làm fallback",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("objective/autopilot prompt cũ" in item for item in violations), violations)

    def test_rejects_reintroduced_legacy_static_class_renderer(self) -> None:
        with self._copy_minimal_repo() as temp:
            legacy = Path(temp) / "client/Unity/Assets/Game/World/Runtime/TwoDClassMixedLoadoutFitPreview.cs"
            legacy.parent.mkdir(parents=True)
            legacy.write_text("public sealed class TwoDClassMixedLoadoutFitPreview {}\n", encoding="utf-8")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("legacy static class renderer" in item for item in violations), violations)

    def test_rejects_class_specific_character_hub_api_in_ui(self) -> None:
        with self._copy_minimal_repo() as temp:
            inventory = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Inventory.cs"
            inventory.write_text(
                inventory.read_text(encoding="utf-8").replace(
                    "_scene.GetEquipmentItemId(slotId)",
                    "_scene.GetVoEquipmentItemId(slotId)",
                    1,
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("class-specific UI runtime branch" in item for item in violations), violations)

    def test_rejects_character_hub_class_refresh_that_rebuilds_shared_topology(self) -> None:
        with self._copy_minimal_repo() as temp:
            hub = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.CharacterHub.cs"
            hub.write_text(
                hub.read_text(encoding="utf-8").replace(
                    "BindCharacterHubProfile();",
                    "_skillsPanel?.RemoveFromHierarchy();\n            InitializeSkillsView(_characterHubBody);",
                    1,
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("stable shared topology" in item for item in violations), violations)

    def test_rejects_potential_topology_without_one_shared_artwork_template(self) -> None:
        with self._copy_minimal_repo() as temp:
            topology = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CharacterHubPotentialTopology.cs"
            topology.write_text(
                topology.read_text(encoding="utf-8").replace(
                    "Map01A Potential Topology Artwork",
                    "Map01A Potential Per Class Artwork",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("Map01A Potential Topology Artwork" in item for item in violations), violations)

    def test_rejects_spirit_pet_screen_that_skips_shared_roster_base(self) -> None:
        with self._copy_minimal_repo() as temp:
            self._replace_skin_marker(Path(temp), "ApplyLgoSpiritPetRoster", "ApplyLegacySpiritPetRoster")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoSpiritPetRoster" in item for item in violations), violations)

    def test_rejects_register_screen_that_skips_shared_panel_base(self) -> None:
        with self._copy_minimal_repo() as temp:
            register = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Register.cs"
            register.write_text(
                register.read_text(encoding="utf-8").replace(
                    "ApplyLgoRegisterPanel(_registerOverlay);",
                    "ApplyLgoFrame(_registerOverlay, Color.black, Color.yellow);",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoRegisterPanel" in item for item in violations), violations)

    def test_rejects_password_recovery_that_skips_shared_auth_flow_base(self) -> None:
        with self._copy_minimal_repo() as temp:
            recovery = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.PasswordRecovery.cs"
            recovery.write_text(
                recovery.read_text(encoding="utf-8").replace(
                    "ApplyLgoPasswordRecoveryPanel(_passwordRecoveryOverlay);",
                    "ApplyLgoFrame(_passwordRecoveryOverlay, Color.black, Color.yellow);",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoPasswordRecoveryPanel" in item for item in violations), violations)


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

    def test_rejects_missing_expandable_five_tab_character_hub_rule(self) -> None:
        with self._copy_minimal_repo() as temp:
            agents = Path(temp) / "AGENTS.md"
            agents.write_text(
                agents.read_text(encoding="utf-8").replace(
                    "Character hub dùng một hàng tab gọn có thể mở rộng",
                    "Character hub có thể tạo tab tùy màn",
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("Character hub dùng một hàng tab gọn" in item for item in violations), violations)

    def test_rejects_item_frame_reintroduced_around_full_body_character(self) -> None:
        with self._copy_minimal_repo() as temp:
            inventory = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Inventory.cs"
            text = inventory.read_text(encoding="utf-8").replace(
                "_characterHeroPortrait.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;",
                "ApplyLgoItemIcon(_characterHeroPortrait);\n            _characterHeroPortrait.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;",
                1,
            )
            inventory.write_text(text, encoding="utf-8")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoItemIcon(_characterHeroPortrait)" in item for item in violations), violations)

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

    def test_rejects_character_hub_that_skips_its_shared_shell(self) -> None:
        with self._copy_minimal_repo() as temp:
            inventory = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Inventory.cs"
            inventory.write_text(
                inventory.read_text(encoding="utf-8").replace(
                    "ApplyLgoCharacterHubShell(_inventory);",
                    "ApplyLgoModalShell(_inventory, 12);",
                    1,
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoCharacterHubShell(_inventory)" in item for item in violations), violations)

    def test_rejects_character_hub_that_drops_shared_chrome_or_motion(self) -> None:
        with self._copy_minimal_repo() as temp:
            self._replace_skin_marker(Path(temp), "ApplyLgoCharacterHubPanelSurface", "ApplyFlatPanelSurface")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoCharacterHubPanelSurface" in item for item in violations), violations)

    def test_rejects_character_hub_that_drops_shared_depth_or_interaction(self) -> None:
        with self._copy_minimal_repo() as temp:
            self._replace_skin_marker(Path(temp), "ApplyLgoCharacterHubSelectionState", "ApplyFlatSelectionState")
            self._replace_skin_marker(Path(temp), "ApplyLgoCharacterHubInteractiveMotion", "ApplyStaticButton")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoCharacterHubSelectionState" in item for item in violations), violations)
        self.assertTrue(any("ApplyLgoCharacterHubInteractiveMotion" in item for item in violations), violations)

    def test_rejects_character_hub_that_drops_shared_vector_frame(self) -> None:
        with self._copy_minimal_repo() as temp:
            self._replace_skin_marker(Path(temp), "RuntimeUiSkin.ApplyOrnamentedShellFrame", "ApplyPlainFrame")

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("RuntimeUiSkin.ApplyOrnamentedShellFrame" in item for item in violations), violations)

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
                    "ApplyLgoHudCombatAction(_run, _touch);",
                    "ApplyLgoButton(_run);",
                    1,
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoHudCombatAction(_run" in item for item in violations), violations)

    def test_rejects_hud_shortcuts_that_skip_shared_shortcut_base(self) -> None:
        with self._copy_minimal_repo() as temp:
            hud = Path(temp) / "client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs"
            hud.write_text(
                hud.read_text(encoding="utf-8").replace(
                    "ApplyLgoHudShortcutAction(_menuShortcut, _touch, true);",
                    "ApplyLgoDisabledAction(_menuShortcut);",
                    1,
                ),
                encoding="utf-8",
            )

            violations = validator.validate_root(Path(temp))

        self.assertTrue(any("ApplyLgoHudShortcutAction(_menuShortcut" in item for item in violations), violations)

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
