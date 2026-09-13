using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using LinhGioi.UI;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.Tests.EditMode
{
    public sealed class TwoDCharacterRuntimeStateTests
    {
        private static TwoDCharacterRuntimeState CreateState()
        {
            return new TwoDCharacterRuntimeState(
                new[] { "full", "base", "modular" },
                new[] { "male", "female" },
                new[] { 1, 10, 20, 30 },
                new[] { "weapon", "hair", "boots" });
        }

        [Test]
        public void SharedStateOwnsPresentationSelectionAndLoadout()
        {
            var state = CreateState();

            Assert.That(state.Mode, Is.EqualTo("full"));
            Assert.That(state.Gender, Is.EqualTo("male"));
            Assert.That(state.Level, Is.EqualTo(1));
            Assert.That(state.EquippedSlotCount, Is.EqualTo(3));

            state.CycleMode();
            state.CycleGender();
            state.CycleLevel();
            state.CycleEquipmentSlot();
            state.ToggleSelectedEquipmentSlot();

            Assert.That(state.Mode, Is.EqualTo("base"));
            Assert.That(state.Gender, Is.EqualTo("female"));
            Assert.That(state.Level, Is.EqualTo(10));
            Assert.That(state.SelectedEquipmentSlot, Is.EqualTo("hair"));
            Assert.That(state.IsEquipped("hair"), Is.False);
            Assert.That(state.EquippedSlotCount, Is.EqualTo(2));

            state.SetPresentation(0, 3, 2, 2);
            state.EquipAllExcept("boots");
            Assert.That(state.Mode, Is.EqualTo("modular"));
            Assert.That(state.Gender, Is.EqualTo("male"));
            Assert.That(state.Level, Is.EqualTo(30));
            Assert.That(state.SelectedEquipmentSlot, Is.EqualTo("boots"));
            Assert.That(state.IsEquipped("boots"), Is.False);
        }

        [Test]
        public void InventoryCanSelectAnyEquipmentSlotDirectly()
        {
            var state = CreateState();

            state.SelectEquipmentSlot("boots");

            Assert.That(state.SelectedEquipmentSlot, Is.EqualTo("boots"));
            state.ToggleSelectedEquipmentSlot();
            Assert.That(state.IsEquipped("boots"), Is.False);
            Assert.That(() => state.SelectEquipmentSlot("unknown"), Throws.ArgumentException);
        }

        [Test]
        public void EntryCaptureFlagRunsPreviewWithoutSuppressingEntryOverlay()
        {
            var args = new[] { "LinhGioiOnline", "--lgo-map01a-entry-capture" };

            Assert.That(CongDongLamMap01AArtPreview.ShouldRunForArgs(args), Is.True);
            Assert.That(CongDongLamMap01AArtPreview.IsMapQuestCaptureForArgs(args), Is.False,
                "Entry capture must not enter the quest capture clock because that hides the entry overlay.");
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(args, sceneIsCapturing: false), Is.True);
            var inventoryTabArgs = new[] { "LinhGioiOnline", "--lgo-map01a-inventory-tabs-capture" };
            Assert.That(CongDongLamMap01AArtPreview.ShouldRunForArgs(inventoryTabArgs), Is.True);
            Assert.That(CongDongLamMap01AArtPreview.IsMapQuestCaptureForArgs(inventoryTabArgs), Is.False,
                "Inventory tab capture must not advance the quest capture route.");
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(inventoryTabArgs, sceneIsCapturing: true), Is.False);
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(inventoryTabArgs, sceneIsCapturing: false), Is.False,
                "Inventory tab capture should open Map01A HUD directly instead of stacking behind entry login.");

            var characterArgs = new[] { "LinhGioiOnline", "--lgo-map01a-character-select-capture" };
            Assert.That(CongDongLamMap01AArtPreview.ShouldRunForArgs(characterArgs), Is.True);
            Assert.That(CongDongLamMap01AArtPreview.IsMapQuestCaptureForArgs(characterArgs), Is.False,
                "Character select capture must not advance the quest capture route.");
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(characterArgs, sceneIsCapturing: true), Is.False,
                "Character select capture should open character modal directly instead of stacking over entry login.");
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(
                new[] { "LinhGioiOnline", "--lgo-map01a-art-capture" }, sceneIsCapturing: true), Is.False);
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(
                new[] { "LinhGioiOnline", "--lgo-map01a-skip-entry" }, sceneIsCapturing: false), Is.False);
        }

        [Test]
        public void RuntimeInventoryExposesTenClickableEquipmentRowsAndToggle()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("inventory pointer test");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                var scene = CongDongLamMap01AArtPreview.Attach(controller);
                CongDongLamArrivalHud.Attach(scene);
                var document = host.GetComponentInChildren<UIDocument>();
                var root = document.rootVisualElement;
                var rows = root.Query<Button>().ToList()
                    .Where(button => button.name.StartsWith("LGO Equipment Inventory Slot ")).ToArray();
                Assert.That(rows, Has.Length.EqualTo(10));
                Assert.That(root.Q<Button>("LGO Equipment Inventory Class"), Is.Not.Null);

                InvokeBoundButton(root.Q<Button>("LGO Equipment Inventory Slot outer_tunic"));
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("outer_tunic"));

                InvokeBoundButton(root.Q<Button>("LGO Equipment Inventory Toggle"));
                Assert.That(scene.IsVoEquipmentSlotEquipped("outer_tunic"), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void InventoryKeepsActionsOutsideScrollAndClosingPreservesEquipment()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("inventory fixed actions test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var scroll = root.Q<ScrollView>("LGO Inventory Scroll");
                Assert.That(scroll, Is.Not.Null);
                Assert.That(scroll.Contains(root.Q<Button>("LGO Equipment Inventory Class")), Is.False,
                    "Class switch must stay visible when the item list scrolls");
                Assert.That(scroll.Contains(root.Q<Button>("LGO Equipment Inventory Toggle")), Is.False,
                    "Equip action must stay outside scrolling content");
                scene.ToggleInventory();
                InvokeBoundButton(root.Q<Button>("LGO Equipment Inventory Slot outer_tunic"));
                InvokeBoundButton(root.Q<Button>("LGO Equipment Inventory Toggle"));
                var close = root.Q<Button>("LGO Inventory Close");
                Assert.That(close, Is.Not.Null);
                InvokeBoundButton(close);
                Assert.That(scene.InventoryOpen, Is.False);
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("outer_tunic"));
                Assert.That(scene.IsVoEquipmentSlotEquipped("outer_tunic"), Is.False);
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void InventorySeparatesBagAndCharacterInfoTabsWithSharedSelection()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("inventory main tabs test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                scene.ToggleInventory();
                var bagTab = root.Q<Button>("Map01A Bag Main Tab");
                var infoTab = root.Q<Button>("Map01A Character Info Main Tab");
                Assert.That(bagTab, Is.Not.Null);
                Assert.That(infoTab, Is.Not.Null);
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Bag Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Bag Character Art").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetVoAvatarThumbnailSprite()),
                    "Bag character column should use one complete avatar sprite, never a detached equipment component.");
                Assert.That(root.Q("Map01A Inventory Detail Panel").ClassListContains("lgo-detail-card"), Is.True,
                    "Inventory detail surfaces must inherit the shared detail-card foundation.");
                var body = root.Q("Map01A Inventory Body");
                Assert.That(body.IndexOf(root.Q("Map01A Inventory Detail Panel")), Is.GreaterThan(body.IndexOf(root.Q("Map01A Inventory Grid Panel"))),
                    "Item detail must stay on the right side of the bag grid.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Header").text, Does.Contain("CHI TIẾT"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail State Badge").text, Does.Contain("ĐANG MẶC"));
                var weaponThumbnail = scene.GetVoEquipmentThumbnailSprite("main_weapon");
                Assert.That(weaponThumbnail, Is.Not.Null);
                Assert.That(weaponThumbnail.rect.width, Is.GreaterThan(weaponThumbnail.rect.height * 2f),
                    "Weapon inventory thumbnail should use the complete aggregate slot art, not one narrow forearm component.");
                var detailIcon = root.Q<Label>("Map01A Inventory Detail Icon");
                Assert.That(detailIcon.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Inventory detail must show real runtime equipment art instead of hiding behind fake icons.");
                Assert.That(detailIcon.ClassListContains("lgo-item-icon-frame"), Is.True,
                    "Inventory detail icon must use the shared item-icon frame base before final dedicated item art is available.");
                Assert.That(detailIcon.text, Is.Empty,
                    "Inventory must not present emoji/text badges as final item art.");
                Assert.That(detailIcon.style.width.value.value, Is.GreaterThanOrEqualTo(72),
                    "Right-side item detail card should present a larger hero thumbnail than grid tiles.");
                Assert.That(root.Q<Button>("LGO Inventory Close").ClassListContains("lgo-inventory-button-base"), Is.True,
                    "Every inventory button must start from the shared inventory button base before semantic helpers override density.");
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").ClassListContains("lgo-inventory-button-base"), Is.True,
                    "Inventory detail actions must not use a separate one-off button foundation.");
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile main_weapon").ClassListContains("lgo-inventory-button-base"), Is.True,
                    "Inventory grid item buttons must share the inventory button base before applying grid-cell styling.");
                Assert.That(root.Q<Button>("LGO Inventory Close").style.flexBasis.value.value, Is.LessThanOrEqualTo(44),
                    "Inventory close button should be compact like an RPG modal control, not a large debug square.");
                Assert.That(root.Q<Button>("LGO Inventory Close").style.fontSize.value.value, Is.LessThanOrEqualTo(24),
                    "Inventory close button glyph should not dominate the modal header.");
                Assert.That(root.Q<Label>("Map01A Inventory Modal Title").style.fontSize.value.value, Is.LessThanOrEqualTo(20),
                    "Inventory modal title should match the compact game UI hierarchy instead of oversized debug headings.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Item Name").style.fontSize.value.value, Is.LessThanOrEqualTo(18),
                    "Right-side item title should be readable but not oversized compared with owner RPG inventory references.");
                var weaponTileIcon = root.Q<VisualElement>("Map01A Equipment Item Icon main_weapon");
                Assert.That(weaponTileIcon, Is.Not.Null,
                    "Equipment grid tiles must show the same real runtime thumbnail art, not text-only placeholders.");
                Assert.That(weaponTileIcon.style.backgroundImage.value.sprite, Is.EqualTo(weaponThumbnail));
                Assert.That(weaponTileIcon.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(weaponTileIcon.ClassListContains("lgo-item-icon-frame"), Is.True,
                    "Equipment grid thumbnails must use the same shared item-icon frame base as the detail panel.");
                Assert.That(weaponTileIcon.style.width.value.value, Is.InRange(58, 72),
                    "Grid item thumbnails should be compact like RPG bag icons, not oversized crops that make the UI look rough.");
                var mainWeaponTile = root.Q<Button>("Map01A Equipment Item Tile main_weapon");
                Assert.That(mainWeaponTile.style.height.value.value, Is.LessThanOrEqualTo(116),
                    "Inventory equipment tiles should stay compact and proportional to the owner bag references.");
                Assert.That(mainWeaponTile.style.flexBasis.value.value, Is.LessThanOrEqualTo(14.5f),
                    "Inventory equipment tiles should use a dense 6-7 column bag grid close to the owner bag references, not wide table cards.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Rarity").text, Does.Contain("Lv"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Stat Primary").text, Is.EqualTo("Chưa có thuộc tính chiến đấu được công bố."));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Stat Fit").text, Does.Contain("Dành cho"));
                Assert.That(root.Q("Map01A Inventory Detail Stats Card"), Is.Not.Null,
                    "Right-side item detail should group item facts in a card, not leave loose debug labels down the panel.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Level Chip"), Is.Not.Null,
                    "Right-side item detail should expose level as a compact chip for RPG inventory scanning.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Equipped Chip"), Is.Not.Null,
                    "Right-side item detail should expose equip state as a compact chip for RPG inventory scanning.");
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action"), Is.Not.Null);
                Assert.That(root.Q<Button>("LGO Equipment Inventory Variant").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Unavailable variant actions should not draw a disabled dead button in the narrow detail card.");
                var detailActions = root.Q("Map01A Inventory Equipment Actions");
                Assert.That(detailActions, Is.Not.Null);
                Assert.That(detailActions.style.marginBottom.value.value, Is.GreaterThanOrEqualTo(10),
                    "Right-side detail actions need breathing room above the bottom edge in Player layout.");
                Assert.That(root.Q<Button>("Map01A Bag Main Tab").style.minHeight.value.value, Is.LessThanOrEqualTo(34),
                    "Inventory top tabs should stay compact like the owner reference, not inherit oversized web-button height.");
                Assert.That(root.Q<Button>("Map01A Bag Main Tab").style.flexBasis.value.value, Is.LessThanOrEqualTo(138),
                    "Inventory top tabs should not look like large desktop form buttons.");
                Assert.That(root.Q<Button>("Map01A Bag Main Tab").ClassListContains("lgo-inventory-main-tab"), Is.True,
                    "Inventory main tabs must use the shared base style so future screens do not hand-tune button size in multiple places.");
                Assert.That(root.Q<Button>("Map01A Equipment Tab").style.minHeight.value.value, Is.LessThanOrEqualTo(32),
                    "Inventory sub-tabs should share the compact game-tab density.");
                Assert.That(root.Q<Button>("Map01A Equipment Tab").style.flexBasis.value.value, Is.LessThanOrEqualTo(112),
                    "Inventory sub-tabs should be compact filter chips, not oversized buttons.");
                Assert.That(root.Q<Button>("Map01A Equipment Tab").ClassListContains("lgo-inventory-filter-chip"), Is.True,
                    "Inventory filter chips must use a shared base style instead of local per-button overrides.");
                Assert.That(root.Q("Map01A Inventory Category Chips"), Is.Not.Null,
                    "Bag category controls should read like compact RPG filter chips, not a pair of full-width debug table tabs.");
                Assert.That(root.Q<Label>("Map01A Inventory Count Badge").ClassListContains("lgo-inventory-badge"), Is.True,
                    "Inventory count badge must share the reusable inventory badge base instead of hand-styling status chips per section.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Level Chip").ClassListContains("lgo-inventory-badge"), Is.True,
                    "Detail chips must share the same inventory badge base as grid and character status badges.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail State Badge").ClassListContains("lgo-inventory-state-badge"), Is.True,
                    "The selected-item state must use one reusable semantic badge instead of local padding and frame values.");
                Assert.That(root.Q("Map01A Inventory Detail Stats Card").ClassListContains("lgo-inventory-stats-card"), Is.True,
                    "Item facts must use one reusable stats-card primitive so bag and character details stay visually aligned.");
                var inventoryGridPanel = root.Q("Map01A Inventory Grid Panel");
                Assert.That(inventoryGridPanel.ClassListContains("lgo-inventory-panel-shell"), Is.True,
                    "Inventory content panels must share a shell base instead of each flow styling a flat panel separately.");
                Assert.That(root.Q("Map01A Inventory Detail Panel").ClassListContains("lgo-inventory-panel-shell"), Is.True,
                    "Inventory detail panel must share the same shell base as bag/character/storage panels.");
                Assert.That(root.Q("Map01A Inventory Detail Panel").ClassListContains("lgo-layered-frame"), Is.True,
                    "Inventory detail should use the shared layered-frame primitive instead of staying as a flat debug rectangle.");
                Assert.That(root.Q("Map01A Storage Gate Card").ClassListContains("lgo-inventory-panel-shell"), Is.True,
                    "Storage gate card must not introduce a parallel flat panel style.");
                Assert.That(inventoryGridPanel.ClassListContains("lgo-inventory-content-fit-panel"), Is.True,
                    "Sparse bag content must use a shared fit-to-content panel base instead of leaving a large blank debug area.");
                Assert.That(inventoryGridPanel.style.alignSelf.value, Is.EqualTo(Align.FlexStart),
                    "Sparse bag content should anchor to its own content height so the lower half of the modal is not an empty debug box.");
                Assert.That(inventoryGridPanel.style.flexGrow.value, Is.EqualTo(0),
                    "Bag grid must not stretch across the whole modal because that turns item cells into wide table cards.");
                var gridDesktopWidth = (float)typeof(CongDongLamArrivalHud).GetField("InventoryDesktopMainColumnWidth", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
                var detailDesktopWidth = (float)typeof(CongDongLamArrivalHud).GetField("InventoryDesktopDetailColumnWidth", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
                var desktopColumnGap = (float)typeof(CongDongLamArrivalHud).GetField("InventoryDesktopColumnGap", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
                Assert.That(gridDesktopWidth, Is.InRange(800, 840),
                    "Desktop bag grid should fill the modal beside detail instead of leaving large empty side gutters.");
                Assert.That(detailDesktopWidth, Is.InRange(320, 340),
                    "Right-side detail should remain readable while the grid gets enough desktop width.");
                Assert.That(desktopColumnGap, Is.InRange(12, 14),
                    "Grid/detail gap should be a deliberate shared desktop gutter, not a tiny accidental seam.");
                Assert.That(root.Q<Button>("Map01A Equipment Tab").style.flexGrow.value, Is.EqualTo(0),
                    "Equipment category chip should not stretch across the full bag width.");
                Assert.That(root.Q<Button>("Map01A Supplies Tab").style.flexGrow.value, Is.EqualTo(0),
                    "Supplies category chip should not stretch across the full bag width.");
                Assert.That(root.Q("Map01A Inventory Category Chip Consumable"), Is.Not.Null,
                    "Bag shell should reserve compact category chips for later item groups without opening fake inventory data.");
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile main_weapon").text, Is.Empty,
                    "Equipment tile Button.text must stay empty so UIToolkit does not draw text over the runtime thumbnail and child labels.");
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile boots").text, Is.Empty);
                Assert.That(root.Q<Label>("Map01A Equipment Item Name main_weapon").text, Does.Contain("Vũ khí"));
                Assert.That(root.Q<Label>("Map01A Equipment Item Name boots").text, Does.Contain("Giày"));
                var emptyBagSlot = root.Q("Map01A Empty Bag Slot 01");
                Assert.That(emptyBagSlot, Is.Not.Null,
                    "Bag layout should reserve empty inventory cells so the screen reads as a game bag grid, not a sparse debug list.");
                Assert.That(emptyBagSlot.style.flexBasis.value.value, Is.LessThanOrEqualTo(14.5f));
                Assert.That(root.Q("Map01A Empty Bag Slot 05"), Is.Null,
                    "Demo bag should reserve a few empty cells without filling half the modal with dead empty boxes far from the owner RPG references.");
                Assert.That(root.Q<Label>("Map01A Inventory Count Badge"), Is.Not.Null,
                    "Bag tab should show an inventory capacity badge like a real bag screen, not only a raw item grid.");
                Assert.That(root.Q("Map01A Inventory Bottom Actions"), Is.Not.Null,
                    "Bag tab needs a bottom action bar so the modal reads as game inventory instead of a debug table.");
                Assert.That(root.Q<Button>("Map01A Inventory Sort Action").style.flexGrow.value, Is.EqualTo(0),
                    "Bottom inventory actions should be compact toolbar actions, not full-width disabled debug bars.");
                Assert.That(root.Q<Button>("Map01A Inventory Quick Sell Action").style.flexBasis.value.value, Is.LessThanOrEqualTo(118),
                    "Bottom inventory actions should stay proportional to the owner RPG bag references.");
                Assert.That(root.Q<Button>("Map01A Inventory Quick Sell Action").ClassListContains("lgo-inventory-toolbar-action"), Is.True,
                    "Inventory toolbar buttons must use one reusable base so button density stays consistent across bag/storage flows.");
                Assert.That(root.Q<Label>("Map01A Equipment Item Name main_weapon").style.fontSize.value.value, Is.LessThanOrEqualTo(12),
                    "Equipment item labels should stay understated so the grid does not read as a debug table.");
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile main_weapon").ClassListContains("lgo-inventory-grid-cell"), Is.True,
                    "Equipment grid cells must use the same base as empty bag cells to avoid patchwork sizing.");
                Assert.That(root.Q<ScrollView>("LGO Inventory Scroll").style.flexGrow.value, Is.EqualTo(0),
                    "Demo bag content should not stretch the scroll view into a large empty debug table area when item rows are sparse.");
                Assert.That(root.Q<ScrollView>("LGO Inventory Scroll").style.maxHeight.value.value, Is.LessThanOrEqualTo(300),
                    "Bag grid should stay visually grouped around the current demo rows instead of filling the modal with blank table space.");
                Assert.That(root.Q("Map01A Inventory Grid Accent Rail"), Is.Not.Null,
                    "Inventory panels need a shared ornament rail to reduce flat debug-panel presentation.");
                Assert.That(root.Q("Map01A Inventory Modal Top Ornament"), Is.Not.Null,
                    "Inventory modal shell needs a shared top ornament rail so it does not read as one flat HTML block.");
                Assert.That(root.Q("Map01A Inventory Modal Top Ornament").ClassListContains("lgo-ornament-rail"), Is.True,
                    "Inventory modal top ornament must reuse the shared ornament-rail base instead of inline styling.");
                Assert.That(root.Q("Map01A Inventory Modal Bottom Ornament"), Is.Not.Null,
                    "Inventory modal shell needs a shared bottom ornament rail to close the visual frame.");
                Assert.That(root.Q("Map01A Inventory Modal Bottom Ornament").ClassListContains("lgo-ornament-rail"), Is.True,
                    "Inventory modal bottom ornament must reuse the shared ornament-rail base instead of a second rail style.");
                Assert.That(root.Q("Map01A Inventory").ClassListContains("lgo-inventory-compact-shell"), Is.True,
                    "Bag inventory should use a shared compact shell so the modal background does not leave a large empty lower half on Player.");
                Assert.That(root.Q("Map01A Inventory").style.height.value.value, Is.GreaterThanOrEqualTo(650),
                    "Bag inventory shell should keep enough shared compact height for the item-detail action stack to breathe.");
                Assert.That(root.Q("Map01A Inventory").style.height.value.value, Is.LessThanOrEqualTo(680),
                    "Bag inventory shell should be shorter than the full desktop safe-area when current content is sparse.");

                InvokeBoundButton(infoTab);
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory").ClassListContains("lgo-inventory-compact-shell"), Is.False,
                    "Character-info needs the regular bounded shell because its loadout matrix can scroll taller than the sparse bag grid.");
                Assert.That(root.Q("Map01A Inventory").style.height.value.value, Is.GreaterThanOrEqualTo(700),
                    "Switching from sparse bag to character-info must restore the regular modal height so the 10-slot loadout is not cut off in Player.");
                Assert.That(body.IndexOf(root.Q("Map01A Inventory Detail Panel")), Is.GreaterThan(body.IndexOf(root.Q("Map01A Inventory Character Panel"))),
                    "Item detail must stay on the right side of character equipment slots.");
                Assert.That(root.Q("Map01A Character Hero Card"), Is.Not.Null,
                    "Character-info must start with a rich hero/equipment overview card instead of a plain technical slot table.");
                Assert.That(root.Q("Map01A Character Hero Portrait"), Is.Not.Null,
                    "The hero card needs a visible portrait/equipment frame so the screen reads like character UI.");
                Assert.That(root.Q<Label>("Map01A Character Hero Name").text, Does.Contain("LụcThiên"));
                Assert.That(root.Q<Label>("Map01A Character Hero Power").text, Does.Contain("LC"));
                Assert.That(root.Q("Map01A Character Hero Loadout Strip"), Is.Not.Null,
                    "The hero card should summarize equipped-slot state before the detailed list.");
                Assert.That(root.Q("Map01A Character Stat Strip"), Is.Not.Null,
                    "Character-info needs a compact stat strip so it reads like the owner reference character panel.");
                Assert.That(root.Q("Map01A Character Loadout Matrix"), Is.Not.Null,
                    "Character-info needs a named loadout matrix instead of an anonymous wrapped technical list.");
                Assert.That(root.Q<Button>("LGO Equipment Inventory Slot main_weapon").ClassListContains("lgo-inventory-item-row"), Is.True,
                    "Character equipment rows must reuse the inventory item-row base instead of maintaining a parallel row skin.");

                InvokeBoundButton(root.Q<Button>("LGO Equipment Inventory Slot boots"));
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("boots"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Slot Type").text, Does.Contain("Giày"));
                Assert.That(scene.GetVoEquipmentThumbnailSprite("boots"), Is.Not.Null);
                Assert.That(root.Q<Label>("Map01A Inventory Detail Icon").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Icon").text, Is.Empty);
                Assert.That(root.Q<Label>("Map01A Inventory Detail State Badge").text, Does.Contain("ĐANG MẶC"));
                InvokeBoundButton(root.Q<Button>("LGO Equipment Inventory Toggle"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail State Badge").text, Does.Contain("ĐÃ THÁO"));
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").text, Does.Contain("Mặc"));
                InvokeBoundButton(bagTab);
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("boots"));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void InventoryModalUsesBoundedDesktopShellInsteadOfFullWidthOverlay()
        {
            var desktop = CongDongLamArrivalHud.CalculateInventoryModalRect(new Rect(0, 0, 1600, 900), touch: false);

            Assert.That(desktop.width, Is.InRange(1240, 1320),
                "Desktop inventory modal should be bounded near the owner RPG modal references instead of spanning the whole screen.");
            Assert.That(desktop.x, Is.GreaterThanOrEqualTo(120),
                "Desktop inventory modal should leave balanced map backdrop margins rather than a full-screen debug overlay.");
            Assert.That(desktop.y, Is.EqualTo(82));
            Assert.That(desktop.height, Is.EqualTo(748));

            var compact = CongDongLamArrivalHud.CalculateInventoryModalRect(new Rect(0, 0, 800, 480), touch: true);
            Assert.That(compact.x, Is.EqualTo(14));
            Assert.That(compact.width, Is.EqualTo(772),
                "Compact/touch inventory should keep safe side margins instead of using the desktop bounded shell.");
        }

        [Test]
        public void CharacterSelectModalUsesSharedSkinAndDoesNotAdvanceQuest()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character select modal test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Entry Start Button"));
                var open = root.Q<Button>("Map01A Character Select Button");
                Assert.That(open, Is.Not.Null);
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value, Is.EqualTo(DisplayStyle.None));

                InvokeBoundButton(open);
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Safe Hud").style.display.value, Is.EqualTo(DisplayStyle.None));
                var selectTitle = root.Q<Label>("Map01A Character Select Title");
                Assert.That(selectTitle.text, Does.Contain("Chọn Nhân Vật"));
                Assert.That(selectTitle.ClassListContains("lgo-title-label"), Is.True,
                    "Repeated screen titles must use the shared title-label base so modal typography stays consistent.");
                Assert.That(selectTitle.style.fontSize.value.value, Is.LessThanOrEqualTo(28),
                    "Character-select title should be game UI scale, not oversized prototype typography.");
                foreach (var label in new[] { "Võ", "Kiếm", "Pháp", "Cơ", "Linh" })
                {
                    var card = root.Q<Button>("Map01A Character Card " + label);
                    Assert.That(card, Is.Not.Null);
                    Assert.That(card.ClassListContains("lgo-character-select-card"), Is.True,
                        "Character select cards must use the shared card base instead of local one-off sizing.");
                }
                var close = root.Q<Button>("Map01A Character Select Close");
                Assert.That(close.ClassListContains("lgo-character-select-primary-action"), Is.True,
                    "Character select primary action must use a shared modal CTA base for consistent density.");
                var phap = root.Q<Button>("Map01A Character Card Pháp");
                Assert.That(phap.enabledSelf, Is.False, "Pháp must stay visible but disabled while source promotion is held out");
                Assert.That(phap.text, Does.Contain("đang audit"));
                Assert.That(root.Q<Label>("Map01A Character Select Scope").text, Does.Contain("review"));
                Assert.That(root.Q<Label>("Map01A Character Select Scope").ClassListContains("lgo-subtitle-label"), Is.True,
                    "Repeated modal subtitles must use the shared subtitle-label base instead of local one-off typography.");
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q01"));

                InvokeBoundButton(root.Q<Button>("Map01A Character Select Close"));
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Safe Hud").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q01"));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void GameplayHudShowsProductShortcutGateWithoutDeadClicks()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("gameplay shortcut gate test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Entry Start Button"));

                var shortcutBar = root.Q("Map01A Product Shortcut Actions");
                Assert.That(shortcutBar, Is.Not.Null);
                Assert.That(shortcutBar.style.display.value, Is.EqualTo(DisplayStyle.Flex));

                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var talk = (Button)typeof(CongDongLamArrivalHud).GetField("_talk", flags).GetValue(hud);
                var inventoryToggle = (Button)typeof(CongDongLamArrivalHud).GetField("_inventoryToggle", flags).GetValue(hud);
                var characterSelect = root.Q<Button>("Map01A Character Select Button");
                Assert.That(talk.ClassListContains("lgo-hud-context-action"), Is.True,
                    "HUD context actions must use a shared context-action base instead of local one-off sizing.");
                Assert.That(inventoryToggle.ClassListContains("lgo-hud-context-action"), Is.True,
                    "Inventory/context actions must share one base for consistent Player density.");
                Assert.That(characterSelect.ClassListContains("lgo-hud-context-action"), Is.True,
                    "Character/info context actions must not create a parallel HUD button style.");

                var skills = root.Q<Button>("Map01A Skills Shortcut");
                var menu = root.Q<Button>("Map01A Menu Shortcut");
                Assert.That(skills, Is.Not.Null);
                Assert.That(menu, Is.Not.Null);
                Assert.That(skills.text, Is.EqualTo("Kỹ năng"));
                Assert.That(menu.text, Is.EqualTo("Menu"));
                Assert.That(skills.enabledSelf, Is.False, "Kỹ năng shortcut must stay visibly gated until the real screen exists.");
                Assert.That(menu.enabledSelf, Is.False, "Menu shortcut must stay visibly gated until the real screen exists.");
                Assert.That(skills.ClassListContains("lgo-hud-shortcut-action"), Is.True,
                    "HUD product shortcuts must use the shared shortcut base instead of local one-off sizing.");
                Assert.That(menu.ClassListContains("lgo-hud-shortcut-action"), Is.True,
                    "All HUD product shortcuts must share the same base style for consistent Player density.");
                Assert.That(skills.style.whiteSpace.value, Is.EqualTo(WhiteSpace.NoWrap),
                    "HUD product shortcuts must not wrap into tall two-line buttons.");
                Assert.That(skills.resolvedStyle.fontSize, Is.LessThanOrEqualTo(13f),
                    "HUD product shortcuts must stay compact and must not inherit modal/button CTA typography.");
                Assert.That(skills.resolvedStyle.height, Is.LessThanOrEqualTo(42f),
                    "HUD product shortcuts must stay compact on Player.");

                InvokeBoundButton(root.Q<Button>("Map01A Character Select Button"));
                Assert.That(shortcutBar.style.display.value, Is.EqualTo(DisplayStyle.None));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void EntryScreenSeparatesDevLoginAndStartWithoutChangingMapState()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("entry screen test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var overlay = root.Q("Map01A Entry Overlay");
                Assert.That(overlay, Is.Not.Null);
                Assert.That(root.Q("Map01A Entry Notice Panel").ClassListContains("lgo-status-card"), Is.True,
                    "Entry notices must use the shared status-card foundation instead of a screen-local frame and padding skin.");
                Assert.That(root.Q<Button>("Map01A Entry Login Button"), Is.Not.Null);
                Assert.That(root.Q("Map01A Entry Account Field"), Is.Not.Null);
                var accountPlaceholder = root.Q<Label>("Map01A Entry Account Placeholder").text;
                Assert.That(accountPlaceholder, Does.Contain("Tài khoản"));
                Assert.That(accountPlaceholder, Does.Not.Contain("👤"), "Login must not use temporary emoji glyphs as field icons.");
                Assert.That(root.Q("Map01A Entry Password Field"), Is.Not.Null);
                var passwordPlaceholder = root.Q<Label>("Map01A Entry Password Placeholder").text;
                Assert.That(passwordPlaceholder, Does.Contain("Mật khẩu"));
                Assert.That(passwordPlaceholder, Does.Not.Contain("🔒"), "Login must not use temporary emoji glyphs as field icons.");
                Assert.That(root.Q("Map01A Entry Account Field").ClassListContains("lgo-input-field"), Is.True,
                    "Entry input fields must use the shared input-field base instead of login-only frame sizing.");
                Assert.That(root.Q("Map01A Entry Password Field").ClassListContains("lgo-input-field"), Is.True,
                    "Password field must share the same input-field base as account field.");
                Assert.That(root.Q("Map01A Entry Account Field Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01AHudIconSprite("account")));
                Assert.That(root.Q("Map01A Entry Password Field Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01AHudIconSprite("lock")));
                Assert.That(root.Q<Label>("Map01A Entry Account Placeholder").style.fontSize.value.value, Is.LessThanOrEqualTo(15),
                    "Entry placeholder text must stay compact against the owner reference instead of using oversized form typography.");
                Assert.That(root.Q("Map01A Entry Account Field").style.paddingLeft.value.value, Is.GreaterThanOrEqualTo(16),
                    "Entry input fields need shared inner spacing so they read as game UI controls rather than thin web-form rectangles.");
                Assert.That(root.Q("Map01A Entry Account Field").style.borderBottomWidth.value, Is.GreaterThanOrEqualTo(2),
                    "Entry input fields need a stronger shared frame instead of the default one-pixel web-form border.");
                Assert.That(root.Q<Label>("Map01A Entry Auth Scope").text, Does.Contain("trải nghiệm"));
                Assert.That(root.Q<Label>("Map01A Entry Brand Seal").text, Does.Contain("Đông Lâm"));
                Assert.That(root.Q<Label>("Map01A Entry Server Name").text, Does.Contain("S1"));
                Assert.That(root.Q<Label>("Map01A Entry Server State").text, Does.Contain("Mượt"));
                Assert.That(root.Q("Map01A Entry Server Card").ClassListContains("lgo-detail-card"), Is.True,
                    "Entry server summary must inherit the shared detail-card foundation.");
                var serverSwitch = root.Q<Button>("Map01A Entry Server Switch");
                Assert.That(serverSwitch, Is.Not.Null,
                    "Entry/login should reserve the design server-switch affordance without opening production server routing.");
                Assert.That(serverSwitch.enabledSelf, Is.False);
                Assert.That(serverSwitch.text, Does.Contain("Đổi máy chủ"));
                Assert.That(serverSwitch.ClassListContains("lgo-entry-secondary-action"), Is.True,
                    "Entry server switch must use the shared entry secondary-action base instead of local inline sizing.");
                Assert.That(serverSwitch.text, Does.Not.Contain("chưa mở"),
                    "Disabled design affordances should read like game UI, not debug placeholder copy.");
                var start = root.Q<Button>("Map01A Entry Start Button");
                Assert.That(start, Is.Not.Null);
                Assert.That(start.ClassListContains("lgo-action-button"), Is.True,
                    "All Map01A buttons must pass through the shared action-button base before screen-specific role styling.");
                Assert.That(start.ClassListContains("lgo-action-primary"), Is.True,
                    "Primary CTAs must use the shared primary action class instead of login-only button styling.");
                Assert.That(start.ClassListContains("lgo-entry-cta-action"), Is.True,
                    "Entry CTAs must use the shared entry CTA base instead of a login-only StyleEntryButton helper.");
                Assert.That(root.Q("Map01A Entry Primary Cta Row"), Is.Not.Null,
                    "Entry reference uses the start action as its own gold CTA row instead of burying it beside secondary auth actions.");
                Assert.That(root.Q("Map01A Entry Secondary Actions"), Is.Not.Null,
                    "Secondary login actions should stay visually subordinate to the primary start CTA.");
                var loginButton = root.Q<Button>("Map01A Entry Login Button");
                Assert.That(loginButton.ClassListContains("lgo-action-button"), Is.True,
                    "Secondary login actions must still inherit the shared action-button base.");
                Assert.That(loginButton.ClassListContains("lgo-action-standard"), Is.True,
                    "Secondary login actions should use the standard shared action role, then entry-specific sizing.");
                Assert.That(loginButton.ClassListContains("lgo-entry-cta-action"), Is.True,
                    "Entry secondary CTAs must share the same entry CTA base as the primary start button.");
                Assert.That(loginButton.text, Is.EqualTo("Đăng nhập"));
                Assert.That(root.Q<Button>("Map01A Entry Register Button").text, Is.EqualTo("Đăng ký"));
                Assert.That(loginButton.text, Does.Not.Contain("dev"), "Entry/login surface must not expose developer wording to the player.");
                Assert.That(start.style.minHeight.value.value, Is.GreaterThan(loginButton.style.minHeight.value.value));
                Assert.That(start.style.minHeight.value.value, Is.LessThanOrEqualTo(50),
                    "Primary login CTA should feel like a polished game button, not an oversized web form control.");
                Assert.That(start.style.fontSize.value.value, Is.LessThanOrEqualTo(21),
                    "Primary CTA typography must stay below the oversized prototype style.");
                Assert.That(start.style.paddingLeft.value.value, Is.GreaterThanOrEqualTo(18),
                    "Primary CTA should have shared horizontal padding so the button reads like a game control rather than raw text in a box.");
                Assert.That(start.style.borderLeftWidth.value, Is.GreaterThanOrEqualTo(2),
                    "Primary CTA should carry the same stronger frame on every edge, not only top and bottom.");
                Assert.That(loginButton.style.minHeight.value.value, Is.LessThanOrEqualTo(38),
                    "Secondary login actions should be compact links/buttons under the main CTA.");
                Assert.That(start.style.maxWidth.value.value, Is.GreaterThan(300));
                var entryLoginTitle = root.Q<Label>("Map01A Entry Login Title");
                Assert.That(entryLoginTitle.text, Does.Contain("Đăng nhập"));
                Assert.That(entryLoginTitle.ClassListContains("lgo-title-label"), Is.True,
                    "Entry/login title must use the shared title-label base instead of login-only typography.");
                Assert.That(entryLoginTitle.style.fontSize.value.value, Is.LessThanOrEqualTo(20),
                    "Entry/login form title should stay compact against the owner reference rather than using oversized web-form typography.");
                Assert.That(root.Q<Label>("Map01A Entry Hero Motto").text, Does.Contain("Chính nghĩa"));
                Assert.That(overlay.style.backgroundColor.value.a, Is.LessThanOrEqualTo(.54f),
                    "Entry/login should keep the Đông Lâm scene visible behind the glass layer instead of blacking it out.");
                var entrySideNav = root.Q<Button>("Map01A Entry Side Action Thông Báo");
                Assert.That(entrySideNav.ClassListContains("lgo-entry-side-action"), Is.True,
                    "Entry side actions must share one reusable side-action base.");
                Assert.That(entrySideNav.enabledSelf, Is.True,
                    "Entry side actions are navigation affordances and should not look disabled like placeholder debug controls.");
                Assert.That(entrySideNav.style.opacity.value, Is.GreaterThanOrEqualTo(.82f),
                    "Entry side actions should read as available controls instead of dim disabled blocks.");
                Assert.That(root.Q("Map01A Entry Panel Glow"), Is.Not.Null,
                    "Entry/login needs a reusable visual depth layer so it does not read like a flat HTML form.");
                Assert.That(root.Q("Map01A Entry Cta Ornament Left"), Is.Not.Null,
                    "Primary CTA should carry game-style ornament rails instead of being only a plain text button.");
                Assert.That(root.Q("Map01A Entry Cta Ornament Right"), Is.Not.Null);
                Assert.That(root.Q("Map01A Entry Cta Ornament Left").ClassListContains("lgo-ornament-rail"), Is.True,
                    "Repeated ornamental rails must use a shared base class so login/dialog/card polish does not fork.");
                Assert.That(root.Q("Map01A Entry Cta Ornament Right").ClassListContains("lgo-ornament-rail"), Is.True);
                Assert.That(root.Q("Map01A Entry Control Card"), Is.Not.Null,
                    "Login fields, auth options and server selection should sit inside one design card, matching the owner reference hierarchy.");
                Assert.That(root.Q("Map01A Entry Control Card").ClassListContains("lgo-entry-control-card"), Is.True);
                Assert.That(root.Q("Map01A Entry Control Card").ClassListContains("lgo-layered-frame"), Is.True);
                Assert.That(root.Q("Map01A Entry Brand Crest"), Is.Not.Null,
                    "Entry brand needs a reusable crest treatment above the logo hierarchy.");
                var entryPanel = root.Q("Map01A Entry Panel");
                Assert.That(entryPanel.ClassListContains("lgo-entry-shell"), Is.True,
                    "Entry/login panel sizing must go through a shared entry-shell base instead of screen-local width/padding values.");
                Assert.That(entryPanel.ClassListContains("lgo-layered-frame"), Is.True,
                    "Entry/login shell must use the shared layered-frame primitive instead of a flat web-form rectangle.");
                Assert.That(root.Q("Map01A Entry Server Card").ClassListContains("lgo-layered-frame"), Is.True,
                    "Entry detail cards must share the layered-frame primitive so card depth does not fork per screen.");
                Assert.That(entryPanel.style.maxWidth.value.value, Is.LessThanOrEqualTo(620),
                    "Entry/login panel should stay compact so the screen reads as a game login card instead of a wide web form.");
                Assert.That(root.Q("Map01A Entry Panel Glow").style.maxWidth.value.value, Is.LessThanOrEqualTo(660),
                    "Entry/login glow should frame the compact shell instead of widening the black rectangle behind the form.");
                Assert.That(root.Q<Label>("Map01A Entry Logo").style.fontSize.value.value, Is.InRange(48, 54),
                    "Entry logo should carry the visual hierarchy of the owner reference while remaining inside the compact shell.");
                Assert.That(root.Q<Label>("Map01A Entry Logo Online").text, Does.Contain("O  N  L  I  N  E"),
                    "Entry brand should preserve the stacked logo hierarchy from the approved reference.");
                Assert.That(root.Q("Map01A Entry Ornament Top"), Is.Not.Null,
                    "Entry/login needs a simple shared ornament separator instead of a plain blocky form stack.");
                Assert.That(root.Q<Label>("Map01A Entry Safety Note").text, Does.Not.Contain("production auth"),
                    "Runtime login copy should be player-facing and must not expose implementation wording on the main screen.");
                Assert.That(root.Q("Map01A Entry Auth Options"), Is.Not.Null);
                Assert.That(root.Q("Map01A Entry Remember Box"), Is.Not.Null,
                    "Remember-account state should use a UI element box, not a temporary checkbox glyph.");
                var rememberText = root.Q<Label>("Map01A Entry Remember Account").text;
                Assert.That(rememberText, Does.Contain("Lưu tài khoản"));
                Assert.That(rememberText, Does.Not.Contain("☑"), "Entry/login must not use temporary checkbox glyphs as UI art.");
                var forgotPassword = root.Q<Button>("Map01A Entry Forgot Password");
                var supportLink = root.Q<Button>("Map01A Entry Support Link");
                Assert.That(forgotPassword.enabledSelf, Is.False);
                Assert.That(supportLink.enabledSelf, Is.False);
                Assert.That(forgotPassword.ClassListContains("lgo-entry-secondary-action"), Is.True,
                    "Entry auth links must share the secondary-action base instead of per-link sizing.");
                Assert.That(supportLink.ClassListContains("lgo-entry-secondary-action"), Is.True,
                    "Entry auth links must share the same base for future auth/help states.");
                Assert.That(forgotPassword.text, Does.Not.Contain("chưa mở"),
                    "Disabled design affordances should avoid exposing unfinished-state copy on the main login surface.");
                Assert.That(supportLink.text, Does.Not.Contain("chưa mở"));
                Assert.That(start.text, Does.Contain("Bắt đầu"));
                Assert.That(root.Q<Label>("Map01A Entry Safety Note").text, Does.Contain("local"));
                foreach (var name in new[] { "Thông Báo", "Cài Đặt", "Hỗ Trợ", "Cinematic" })
                {
                    var sideAction = root.Q<Button>("Map01A Entry Side Action " + name);
                    Assert.That(sideAction, Is.Not.Null);
                    Assert.That(sideAction.enabledSelf, Is.True, "Entry/login side actions should be active navigation affordances with status feedback.");
                    Assert.That(sideAction.ClassListContains("lgo-entry-side-action"), Is.True,
                        "Entry side actions must use the shared entry side-action base instead of local inline sizing.");
                    Assert.That(sideAction.text, Is.EqualTo(name));
                    Assert.That(sideAction.style.whiteSpace.value, Is.EqualTo(WhiteSpace.NoWrap),
                        "Entry side actions must stay compact and must not wrap into two-line placeholders.");
                    Assert.That(sideAction.resolvedStyle.fontSize, Is.LessThanOrEqualTo(13f));
                    Assert.That(sideAction.style.height.value.value, Is.InRange(56f, 68f));
                    Assert.That(sideAction.Q(sideAction.name + " Icon"), Is.Not.Null);
                }
                Assert.That(root.Q("Map01A Safe Hud").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Entry/login must not leave the in-game HUD visible behind the modal.");

                InvokeBoundButton(start);
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Safe Hud").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q01"));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void InventoryStorageTabShowsExplicitGateWithoutChangingLoadout()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("inventory storage gate test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                scene.ToggleInventory();
                InvokeBoundButton(root.Q<Button>("LGO Equipment Inventory Slot boots"));
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("boots"));

                var storageTab = root.Q<Button>("Map01A Storage Main Tab");
                Assert.That(storageTab, Is.Not.Null);
                InvokeBoundButton(storageTab);
                Assert.That(root.Q("Map01A Storage Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                var storageGate = root.Q("Map01A Storage Gate Card");
                Assert.That(storageGate, Is.Not.Null,
                    "Rương đồ must render as an intentional locked-state card, not as an empty broken panel.");
                Assert.That(root.Q<Label>("Map01A Storage Gate Title").text, Does.Contain("Kho gửi/rút"));
                Assert.That(root.Q<Label>("Map01A Storage State").text, Does.Contain("chưa khả dụng"));
                Assert.That(root.Q<Label>("Map01A Storage State").text, Does.Not.Contain("API"));
                Assert.That(root.Q<Label>("Map01A Storage Gate Safety").text, Does.Contain("được giữ nguyên"));
                var deposit = root.Q<Button>("Map01A Storage Deposit");
                var withdraw = root.Q<Button>("Map01A Storage Withdraw");
                Assert.That(deposit.enabledSelf, Is.False);
                Assert.That(withdraw.enabledSelf, Is.False);
                Assert.That(deposit.style.opacity.value, Is.EqualTo(.58f),
                    "Locked storage actions should use the shared disabled-action skin, not raw inactive buttons.");
                Assert.That(withdraw.style.opacity.value, Is.EqualTo(.58f));
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("boots"));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }


        [Test]
        public void InventoryReviewCaptureCanOpenCharacterInfoAndStorageTabsWithoutInput()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("inventory review capture tab test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;

                hud.OpenInventoryReviewMode("character-info");
                Assert.That(root.Q("Map01A Inventory Character Panel").style.flexGrow.value, Is.EqualTo(0),
                    "Character info must keep the bounded two-column width instead of stretching like the obsolete full-width layout.");
                Assert.That(root.Q("Map01A Inventory Character Panel").style.flexBasis.value.value, Is.InRange(800, 840));
                Assert.That(scene.InventoryOpen, Is.True);
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                var characterWeaponIcon = root.Q("Map01A Character Info Slot Icon main_weapon");
                Assert.That(characterWeaponIcon, Is.Not.Null,
                    "Character-info equipment slots should reuse runtime item thumbnails instead of staying as text-only cells.");
                Assert.That(characterWeaponIcon.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(characterWeaponIcon.ClassListContains("lgo-item-icon-frame"), Is.True,
                    "Character-info equipment thumbnails must not carry a parallel icon-frame style.");
                var heroPortraitSprite = root.Q<VisualElement>("Map01A Character Hero Portrait").style.backgroundImage.value.sprite;
                Assert.That(heroPortraitSprite, Is.EqualTo(scene.GetVoAvatarThumbnailSprite()),
                    "Character summary should use stable complete avatar art instead of a detached equipment part.");
                Assert.That(heroPortraitSprite, Is.Not.EqualTo(scene.GetVoEquipmentThumbnailSprite("main_weapon")),
                    "Character summary must never present the selected weapon or limb crop as the character portrait.");
                var modalTitle = root.Q<Label>("Map01A Inventory Modal Title");
                var modalSubtitle = root.Q<Label>("Map01A Inventory Modal Subtitle");
                Assert.That(modalTitle, Is.Not.Null, "Inventory modal needs a named title so each main tab can present its own screen.");
                Assert.That(modalSubtitle, Is.Not.Null, "Inventory modal needs a named subtitle so tab context is visible in Player captures.");
                Assert.That(modalTitle.text, Is.EqualTo("THÔNG TIN"),
                    "Switching to character info must not leave the modal titled HÀNH TRANG.");
                Assert.That(modalSubtitle.text, Does.Contain("trang bị đang mặc"));
                Assert.That(modalTitle.ClassListContains("lgo-title-label"), Is.True,
                    "Inventory modal titles must inherit the shared title-label base so tab screens do not fork typography.");
                Assert.That(modalSubtitle.ClassListContains("lgo-subtitle-label"), Is.True,
                    "Inventory modal subtitles must inherit the shared subtitle-label base instead of local one-off typography.");

                hud.OpenInventoryReviewMode("supplies");
                Assert.That(modalTitle.text, Is.EqualTo("HÀNH TRANG"));
                Assert.That(root.Q("Map01A Supplies Page").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                var suppliesListCard = root.Q("Map01A Supplies List Card");
                Assert.That(suppliesListCard, Is.Not.Null,
                    "Supplies should present a left-side list card beside the right detail card, not a flat full-width technical list.");
                Assert.That(suppliesListCard.IndexOf(root.Q("Map01A Quest Item Actions")), Is.LessThan(suppliesListCard.IndexOf(root.Q("Map01A Supplies Empty State"))),
                    "Supply item rows must appear before explanatory empty-state copy so they remain visible in the bounded Player inventory viewport.");
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Supplies must keep the right-side detail panel instead of becoming a left-only technical list.");
                Assert.That(root.Q<Label>("Map01A Supplies Empty State").text, Does.Contain("Chưa nhận"));
                Assert.That(root.Q<Label>("Map01A Supplies Empty State").ClassListContains("lgo-status-card"), Is.True,
                    "Inventory empty states must share the same status-card foundation as entry notices.");
                Assert.That(root.Q<Button>("Map01A Health Potion").text, Is.Empty,
                    "Supply rows must be composed cards, not plain Button.text labels that look like temporary debug UI.");
                Assert.That(root.Q<Button>("Map01A Health Potion").ClassListContains("lgo-inventory-item-row"), Is.True,
                    "Supply rows must share the inventory item row base instead of each item tab styling rows separately.");
                Assert.That(root.Q<Label>("Map01A Supply Item Count health_potion").ClassListContains("lgo-inventory-count-badge"), Is.True,
                    "Supply count badges must use the shared count badge base so item grids and lists stay visually consistent.");
                Assert.That(root.Q<Label>("Map01A Supply Item Name health_potion").text, Is.EqualTo("Bình Máu Nhỏ"));
                Assert.That(root.Q<Label>("Map01A Supply Item Count health_potion").text, Does.Contain("x0"));
                Assert.That(root.Q<Label>("Map01A Supply Item State health_potion").text, Does.Contain("Thiếu điều kiện"));
                Assert.That(root.Q<Label>("Map01A Supply Item Name mana_potion").text, Is.EqualTo("Bình Linh Lực Nhỏ"));
                Assert.That(root.Q<Label>("Map01A Supply Item Name class_reward").text, Is.EqualTo("Hộ Uyển Võ Tân Thủ"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Header").text, Is.EqualTo("CHI TIẾT VẬT PHẨM"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Slot Type").text, Is.EqualTo("Vật phẩm hồi phục"));
                Assert.That(root.Q<Button>("Map01A Health Potion").enabledSelf, Is.True,
                    "Supply rows stay selectable so clicking an item can show detail without consuming it.");
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").text, Is.EqualTo("Dùng bình máu"));
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").enabledSelf, Is.False,
                    "The right-side action, not the selectable item row, is disabled when the supply cannot be used.");
                Assert.That(root.Q<Button>("Map01A Health Potion").style.color.value, Is.EqualTo(new Color(.70f, .80f, .80f, .92f)),
                    "Supply rows must stay readable in Player captures instead of fading into the dark panel.");

                InvokeBoundButton(root.Q<Button>("Map01A Mana Potion"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Slot Type").text, Is.EqualTo("Vật phẩm hồi phục"));
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").text, Is.EqualTo("Dùng bình linh lực"));
                Assert.That(root.Q<Button>("Map01A Mana Potion").style.backgroundColor.value, Is.EqualTo(new Color(.12f, .33f, .56f, .98f)),
                    "Selected supply row must be visibly highlighted like equipment item rows.");
                Assert.That(root.Q<Button>("Map01A Health Potion").style.backgroundColor.value, Is.Not.EqualTo(new Color(.12f, .33f, .56f, .98f)),
                    "Only the selected supply row should use the selected-row background.");
                Assert.That(root.Q<Label>("Map01A Supply Item State mana_potion").text, Does.Contain("Thiếu điều kiện"));

                var detailScroll = root.Q<ScrollView>("Map01A Inventory Detail Scroll");
                Assert.That(detailScroll, Is.Not.Null);
                Assert.That(detailScroll.Contains(root.Q("Map01A Inventory Equipment Actions")), Is.False,
                    "Item actions must remain visible outside the scrollable description.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Rarity").text, Does.Not.Contain("Tinh phẩm"));
                var equippedBeforeTabSwitch = scene.VoEquippedSlotCount;
                hud.OpenInventoryReviewMode("character-info");
                Assert.That(root.Q("Map01A Inventory Character Panel").style.flexGrow.value, Is.EqualTo(0));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.flexBasis.value.value, Is.InRange(800, 840),
                    "Returning from supplies must restore the bounded character-info column without waiting for geometry changes.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Header").text, Is.EqualTo("CHI TIẾT MÓN"),
                    "Character info must clear the consumable action before showing equipped items.");
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").text, Does.Contain("Tháo"));
                Assert.That(scene.VoEquippedSlotCount, Is.EqualTo(equippedBeforeTabSwitch),
                    "Navigating between tabs must never equip, remove or consume an item.");

                hud.OpenInventoryReviewMode("storage");
                Assert.That(root.Q("Map01A Storage Panel").style.flexGrow.value, Is.EqualTo(1));
                Assert.That(root.Q("Map01A Storage Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(modalTitle.text, Is.EqualTo("RƯƠNG ĐỒ"));
                Assert.That(modalSubtitle.text, Does.Contain("Kho gửi/rút"));
                Assert.That(root.Q<Button>("Map01A Storage Deposit").enabledSelf, Is.False);
                Assert.That(root.Q<Button>("Map01A Storage Withdraw").enabledSelf, Is.False);
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void SourcePoseInventoryKeepsQuestPotionActionUsable()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("source inventory quest test");
            try
            {
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                scene.UseCurrentRouteAction(); FinishDialogue(scene);
                foreach (var steps in new[] { 13, 25, 53, 19 })
                {
                    for (var i = 0; i < steps; i++) scene.MoveOnLane(1, .1f);
                    scene.UseCurrentRouteAction();
                    FinishDialogue(scene);
                }
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q04"));
                Assert.That(scene.HealthPotionCount, Is.EqualTo(3));
                var review = new GameObject("source presentation").AddComponent<TwoDSourcePoseReview>();
                review.transform.SetParent(scene.transform);
                typeof(CongDongLamMap01AArtPreview).GetField("_sourcePoseReview", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(scene, review);
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                typeof(CongDongLamArrivalHud).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(hud, null);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Supplies Tab"));
                Assert.That(root.Q("Map01A Supplies Page").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Footer").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                var potion = root.Q<Button>("Map01A Health Potion");
                Assert.That(potion, Is.Not.Null);
                Assert.That(potion.parent.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Source-pose review must keep the inventory item rows visible for Q04");
                InvokeBoundButton(potion);
                Assert.That(root.Q<Label>("Map01A Inventory Detail Header").text, Is.EqualTo("CHI TIẾT VẬT PHẨM"));
                Assert.That(root.Q("Map01A Supply Item Icon health_potion").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01AItemThumbnailSprite("health_potion")));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01AItemThumbnailSprite("health_potion")));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Slot Type").text, Is.EqualTo("Vật phẩm hồi phục"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail State Badge").text, Is.EqualTo("SẴN SÀNG"));
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").text, Is.EqualTo("Dùng bình máu"));
                InvokeBoundButton(root.Q<Button>("Map01A Inventory Detail Primary Action"));
                Assert.That(scene.PlayerHealth, Is.EqualTo(100));
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q05"));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void SourceGameplayHudKeepsVitalsAndHidesLegacyModeButtonsWhenInventoryCloses()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("source gameplay HUD test");
            try
            {
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var review = new GameObject("source presentation").AddComponent<TwoDSourcePoseReview>();
                review.transform.SetParent(scene.transform);
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                typeof(CongDongLamMap01AArtPreview).GetField("_sourcePoseReview", flags).SetValue(scene, review);
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var update = typeof(CongDongLamArrivalHud).GetMethod("Update", flags);
                if (scene.InventoryOpen) scene.ToggleInventory();
                update.Invoke(hud, null);
                foreach (var field in new[] { "_outfit", "_level", "_gender", "_slot", "_itemLevel", "_toggleSlot" })
                    Assert.That(((Button)typeof(CongDongLamArrivalHud).GetField(field, flags).GetValue(hud)).style.display.value,
                        Is.EqualTo(DisplayStyle.None), "Old review control reappeared: " + field);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var locationTitle = root.Q("Map01A Location Title");
                Assert.That(locationTitle, Is.Not.Null,
                    "HUD location title needs a named shared info panel for visual audit and base-first reuse.");
                Assert.That(locationTitle.ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD location title must use the shared info-panel base instead of the legacy Box helper.");
                Assert.That(root.Q("Map01A Vitals").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD vitals must share the same info-panel base as the title and quest tracker.");
                Assert.That(root.Q("Map01A Quest Tracker Body").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD quest body must use the shared info-panel base instead of one-off panel styling.");
                Assert.That(root.Q("Map01A Minimap").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD minimap placeholder must use the shared info-panel base until a real minimap art pass replaces it.");
                Assert.That(root.Q("LGO World Touch Movement Pad").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "Touch movement pad shell must reuse HUD info-panel base rather than the legacy Box helper.");
                Assert.That(root.Q("Map01A Quest Tracker Tabs"), Is.Not.Null,
                    "HUD quest tracker should expose the Nhiệm Vụ/Đội tab structure from the product reference instead of staying as a plain text block.");
                Assert.That(root.Q<Button>("Map01A Quest Tab Missions").text, Does.Contain("Nhiệm vụ"));
                var questMissionTab = root.Q<Button>("Map01A Quest Tab Missions");
                Assert.That(questMissionTab.ClassListContains("lgo-hud-quest-tab"), Is.True,
                    "Quest tracker tabs must use the shared HUD quest-tab base instead of local inline sizing.");
                Assert.That(root.Q<Button>("Map01A Quest Tab Party").ClassListContains("lgo-hud-quest-tab"), Is.True,
                    "All quest tracker tabs must share the same base style for future quest/team states.");
                Assert.That(questMissionTab.style.whiteSpace.value, Is.EqualTo(WhiteSpace.NoWrap),
                    "Quest tab labels must not wrap in the visible Player HUD.");
                Assert.That(questMissionTab.resolvedStyle.fontSize, Is.LessThanOrEqualTo(13f),
                    "HUD quest tabs must stay compact and must not inherit oversized primary CTA typography.");
                Assert.That(questMissionTab.resolvedStyle.height, Is.LessThanOrEqualTo(36f),
                    "HUD quest tabs must stay compact so the tracker does not look oversized on Player.");
                Assert.That(root.Q<Button>("Map01A Quest Tab Party").enabledSelf, Is.False,
                    "Đội is a visible roadmap affordance, not a clickable dead team feature.");
                var runAction = root.Q<Button>("Map01A Run Action");
                var skillAction = root.Q<Button>("Map01A Skill Action");
                Assert.That(runAction, Is.Not.Null);
                Assert.That(skillAction, Is.Not.Null);
                Assert.That(runAction.ClassListContains("lgo-hud-combat-action"), Is.True,
                    "Bottom HUD action buttons must use the shared HUD combat-action base instead of one-off sizing.");
                Assert.That(skillAction.ClassListContains("lgo-hud-combat-action"), Is.True,
                    "All combat actions must share the same base style for consistent Player density.");
                Assert.That(runAction.style.whiteSpace.value, Is.EqualTo(WhiteSpace.NoWrap),
                    "Bottom HUD action buttons must not wrap into oversized blocks.");
                Assert.That(runAction.resolvedStyle.fontSize, Is.LessThanOrEqualTo(13f),
                    "Bottom HUD action buttons must stay compact on Player.");
                Assert.That(runAction.resolvedStyle.height, Is.InRange(54f, 64f),
                    "Desktop combat icons must stay readable without growing to modal CTA scale.");
                Assert.That(skillAction.resolvedStyle.fontSize, Is.LessThanOrEqualTo(13f));
                foreach (var binding in new[]
                {
                    ("Map01A Run Action", "run"),
                    ("Map01A Jump Action", "jump"),
                    ("Map01A Basic Attack Action", "attack"),
                    ("Map01A Skill Action", "skill"),
                    ("Map01A Character Select Button", "character"),
                    ("Map01A Inventory Toggle", "inventory"),
                    ("Map01A Skills Shortcut", "skills"),
                    ("Map01A Menu Shortcut", "menu"),
                })
                {
                    var icon = root.Q(binding.Item1 + " Icon");
                    Assert.That(icon, Is.Not.Null, binding.Item1 + " must use the shared HUD icon base.");
                    Assert.That(icon.ClassListContains("lgo-hud-action-icon"), Is.True);
                    Assert.That(icon.style.backgroundImage.value.sprite, Is.EqualTo(scene.GetMap01AHudIconSprite(binding.Item2)));
                }
                var navigation = root.Q("Map01A Product Shortcut Actions");
                foreach (var name in new[]
                {
                    "Map01A Character Select Button", "Map01A Inventory Toggle",
                    "Map01A Skills Shortcut", "Map01A Menu Shortcut",
                })
                {
                    var button = root.Q<Button>(name);
                    Assert.That(button.parent, Is.EqualTo(navigation), name + " must stay in the shared bottom navigation group.");
                    Assert.That(button.ClassListContains("lgo-hud-navigation-action"), Is.True);
                    Assert.That(button.resolvedStyle.height, Is.InRange(60f, 76f));
                }
                Assert.That(root.Q<Button>("Map01A Talk Action").parent, Is.Not.EqualTo(navigation),
                    "Context interaction must not be mixed into product navigation.");
                Assert.That(root.Q<UnityEngine.UIElements.ProgressBar>("Map01A Health").value, Is.EqualTo(60));
                Assert.That(root.Q<UnityEngine.UIElements.ProgressBar>("Map01A Mana").value, Is.EqualTo(50));
                Assert.That(root.Q<Button>("Map01A Inventory Gender").enabledSelf, Is.False,
                    "A missing female pack must not offer a renderer fallback");
                scene.ToggleInventory(); update.Invoke(hud, null);
                Assert.That(root.Q("Map01A Vitals").style.display.value, Is.EqualTo(DisplayStyle.None));
                foreach (var field in new[] { "_outfit", "_level", "_gender", "_slot", "_itemLevel", "_toggleSlot" })
                    Assert.That(((Button)typeof(CongDongLamArrivalHud).GetField(field, flags).GetValue(hud)).style.display.value,
                        Is.EqualTo(DisplayStyle.None), "Review/debug controls must not bleed behind inventory: " + field);
                scene.ToggleInventory(); scene.TalkToHaVan(); update.Invoke(hud, null);
                Assert.That(root.Q("Map01A Vitals").style.display.value, Is.EqualTo(DisplayStyle.None));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void GameplayHudUsesCompactReferenceHierarchyAndBoundedDialogue()
        {
            var desktopDialogue = CongDongLamArrivalHud.CalculateDialoguePanelRect(new Rect(0, 0, 1280, 720), touch: false);
            Assert.That(desktopDialogue.width, Is.InRange(760f, 880f));
            Assert.That(desktopDialogue.xMin, Is.GreaterThanOrEqualTo(20f));
            Assert.That(desktopDialogue.xMax, Is.LessThan(1080f),
                "Desktop dialogue must leave the right-side quest/navigation column readable like the owner HUD reference.");

            var mobileDialogue = CongDongLamArrivalHud.CalculateDialoguePanelRect(new Rect(0, 0, 800, 480), touch: true);
            Assert.That(mobileDialogue.xMin, Is.GreaterThanOrEqualTo(12f));
            Assert.That(mobileDialogue.xMax, Is.LessThanOrEqualTo(788f));

            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("HUD reference hierarchy test");
            try
            {
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var playerCard = root.Q("Map01A Vitals");
                var portrait = root.Q("Map01A Player Portrait");
                var quest = root.Q("Map01A Quest Tracker Body");
                var map = root.Q("Map01A Minimap");
                var dialogue = root.Q("Map01A Dialogue Panel");
                var combatActions = root.Q("Map01A Combat Actions");

                Assert.That(playerCard, Is.Not.Null);
                Assert.That(portrait, Is.Not.Null, "The player HUD card needs a real runtime portrait area, not a text-only debug block.");
                Assert.That(quest.resolvedStyle.fontSize, Is.LessThanOrEqualTo(16f));
                Assert.That(map.resolvedStyle.fontSize, Is.LessThanOrEqualTo(14f));
                Assert.That(dialogue.ClassListContains("lgo-layered-frame"), Is.True,
                    "Dialogue must reuse the shared layered frame rather than remain a flat full-width strip.");
                Assert.That(root.Q<Label>("Map01A Dialogue Line").resolvedStyle.fontSize, Is.LessThanOrEqualTo(18f));
                Assert.That(combatActions.style.width.value.value, Is.GreaterThanOrEqualTo(330f),
                    "A right-anchored combat row needs an explicit width or its children overflow off-screen.");
                Assert.That(combatActions.style.height.value.value, Is.GreaterThanOrEqualTo(48f));
                Assert.That(combatActions.style.bottom.value.value, Is.GreaterThanOrEqualTo(150f),
                    "Combat controls must sit above the shortcut and context rows instead of being painted underneath them.");
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }


        [Test]
        public void DialoguePanelShowsQuestContextAndProgressInsideConversation()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("dialogue quest context test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;

                scene.UseCurrentRouteAction();
                typeof(CongDongLamArrivalHud).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(host.GetComponentInChildren<CongDongLamArrivalHud>(), null);
                var header = root.Q("Map01A Dialogue Header");
                Assert.That(header, Is.Not.Null, "Dialogue panel must have a separate design header for speaker/progress metadata.");
                var body = root.Q("Map01A Dialogue Body");
                Assert.That(body, Is.Not.Null, "Dialogue panel must frame the spoken line separately from action buttons.");
                Assert.That(body.ClassListContains("lgo-detail-card"), Is.True,
                    "Dialogue content must inherit the shared detail-card foundation.");
                Assert.That(body.ClassListContains("lgo-layered-frame"), Is.True,
                    "Dialogue body must use the shared layered-frame primitive so conversation panels do not stay flat.");
                var actionRow = root.Q("Map01A Dialogue Actions");
                Assert.That(actionRow, Is.Not.Null, "Dialogue panel must use a named action row instead of loose buttons.");
                var portrait = root.Q("Map01A Dialogue NPC Portrait");
                Assert.That(portrait, Is.Not.Null, "Dialogue must reserve a shared portrait area for the active NPC.");
                Assert.That(portrait.ClassListContains("lgo-dialogue-portrait"), Is.True);
                Assert.That(portrait.style.backgroundImage.value.sprite, Is.EqualTo(scene.GetCurrentDialogueNpcSprite()));
                var speaker = root.Q<Label>("Map01A Dialogue Speaker");
                Assert.That(speaker, Is.Not.Null, "Dialogue speaker needs a named shared title label for UI audit and style reuse.");
                Assert.That(speaker.ClassListContains("lgo-title-label"), Is.True,
                    "Dialogue speaker/title must inherit the shared title-label base instead of a dialogue-only label style.");
                var context = root.Q<Label>("Map01A Dialogue Quest Context");
                Assert.That(context, Is.Not.Null, "Dialogue panel must show quest context for NPC conversations.");
                Assert.That(context.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(context.text, Does.Contain("Q01"));
                Assert.That(context.text, Does.Contain("Đường Hội Tụ"));
                Assert.That(context.text, Does.Contain(scene.DialogueProgress));
                var continueButton = root.Q<Button>("Map01A Dialogue Continue");
                Assert.That(continueButton, Is.Not.Null,
                    "Dialogue panel must expose its own continue action after the world HUD actions are hidden.");
                Assert.That(continueButton.parent, Is.SameAs(actionRow));
                Assert.That(continueButton.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(continueButton.ClassListContains("lgo-dialogue-primary-action"), Is.True,
                    "Dialogue continue must use the shared dialogue primary action base instead of one-off inline sizing.");
                Assert.That(root.Q<Button>("Map01A Dialogue Information").ClassListContains("lgo-dialogue-secondary-action"), Is.True,
                    "Dialogue secondary actions must use the shared dialogue action base instead of local sizing.");
                Assert.That(root.Q<Button>("Map01A Dialogue Close").ClassListContains("lgo-dialogue-secondary-action"), Is.True,
                    "All dialogue secondary actions must share the same reusable base style.");
                Assert.That(continueButton.style.minHeight.value.value, Is.GreaterThan(root.Q<Button>("Map01A Dialogue Close").style.minHeight.value.value),
                    "The main continue action should read as the primary dialogue CTA.");
                var firstLine = scene.DialogueText;
                InvokeBoundButton(continueButton);
                Assert.That(scene.DialogueText, Is.Not.EqualTo(firstLine));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        private static void FinishDialogue(CongDongLamMap01AArtPreview scene)
        {
            for (var page = 0; scene.DialogueOpen && page < 8; page++) scene.UseCurrentRouteAction();
            Assert.That(scene.DialogueOpen, Is.False);
        }

        [Test]
        public void DialogueChoiceButtonsCancelOrReadInformationWithoutAcceptingQuest()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("dialogue choices test");
            try
            {
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                scene.UseCurrentRouteAction();
                InvokeBoundButton(root.Q<Button>("Map01A Dialogue Close"));
                Assert.That(scene.DialogueOpen, Is.False);
                Assert.That(scene.HasMetHaVan, Is.False);
                scene.UseCurrentRouteAction(); scene.UseCurrentRouteAction(); scene.UseCurrentRouteAction();
                var choiceLine = scene.DialogueText;
                Assert.That(scene.CanReadDialogueInformation, Is.True);
                InvokeBoundButton(root.Q<Button>("Map01A Dialogue Information"));
                Assert.That(scene.DialogueText, Is.Not.EqualTo(choiceLine));
                scene.UseCurrentRouteAction();
                Assert.That(scene.DialogueText, Is.EqualTo(choiceLine));
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q01"));
                scene.UseCurrentRouteAction();
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q02"));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        private static void InvokeBoundButton(Button button)
        {
            var callback = typeof(Clickable).GetField("clicked", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(button.clickable) as System.Action;
            Assert.That(callback, Is.Not.Null, "Runtime inventory button must own an actionable callback");
            callback();
        }

        [Test]
        public void DialogueHidesUnderlyingActionsAndRestoresThemAfterContinue()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("dialogue action visibility test");
            try
            {
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var update = typeof(CongDongLamArrivalHud).GetMethod("Update", flags);
                var inventory = (Button)typeof(CongDongLamArrivalHud).GetField("_inventoryToggle", flags).GetValue(hud);
                var combat = (VisualElement)typeof(CongDongLamArrivalHud).GetField("_combatBar", flags).GetValue(hud);
                var talk = (Button)typeof(CongDongLamArrivalHud).GetField("_talk", flags).GetValue(hud);
                update.Invoke(hud, null);
                foreach (var field in new[] { "_outfit", "_level", "_gender", "_slot", "_itemLevel", "_toggleSlot" })
                    Assert.That(((Button)typeof(CongDongLamArrivalHud).GetField(field, flags).GetValue(hud)).style.display.value,
                        Is.EqualTo(DisplayStyle.None), "Review/debug controls must not appear in owner-facing gameplay HUD: " + field);
                InvokeBoundButton(inventory);
                update.Invoke(hud, null);
                foreach (var field in new[] { "_outfit", "_level", "_gender", "_slot", "_itemLevel", "_toggleSlot" })
                    Assert.That(((Button)typeof(CongDongLamArrivalHud).GetField(field, flags).GetValue(hud)).style.display.value,
                        Is.EqualTo(DisplayStyle.None), "Review/debug controls must not bleed behind inventory: " + field);
                InvokeBoundButton(inventory);
                update.Invoke(hud, null);
                InvokeBoundButton(talk);
                Assert.That(scene.DialogueOpen, Is.True);
                update.Invoke(hud, null);
                Assert.That(inventory.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(combat.style.display.value, Is.EqualTo(DisplayStyle.None));
                InvokeBoundButton(talk);
                FinishDialogue(scene);
                Assert.That(scene.DialogueOpen, Is.False);
                update.Invoke(hud, null);
                Assert.That(inventory.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(combat.style.display.value, Is.EqualTo(DisplayStyle.Flex));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void SharedStateOwnsExclusiveActionTimingAndLocomotionHold()
        {
            var state = CreateState();

            state.SetRun(true);
            state.HoldMovement(.14f);
            Assert.That(state.MotionState, Is.EqualTo("run"));
            state.Advance(.1f);
            Assert.That(state.MotionState, Is.EqualTo("run"));
            state.Advance(.1f);
            Assert.That(state.MotionState, Is.EqualTo("idle"));

            Assert.That(state.TryStartAction("jump", .55f), Is.True);
            Assert.That(state.TryStartAction("skill", .42f), Is.False);
            state.Advance(.11f);
            Assert.That(state.MotionState, Is.EqualTo("jump"));
            Assert.That(state.ActionProgress, Is.EqualTo(.2f).Within(.001f));
            state.Advance(.5f);
            Assert.That(state.MotionState, Is.EqualTo("idle"));

            Assert.That(state.TryStartAction("skill", .42f), Is.True);
            state.Advance(.16f);
            Assert.That(state.ActionRemaining, Is.EqualTo(.26f).Within(.001f));
        }
    }
}
