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
                var host = new GameObject("inventory approved two-column test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                scene.ToggleInventory();
                var body = root.Q("Map01A Inventory Body");
                var bagTab = root.Q<Button>("Map01A Bag Main Tab");
                var infoTab = root.Q<Button>("Map01A Character Info Main Tab");

                Assert.That(root.Q("Map01A Inventory Bag Character Panel"), Is.Null,
                    "Approved Rương đồ must not restore the obsolete third character-preview column.");
                Assert.That(root.Q("Map01A Storage Panel"), Is.Null,
                    "The obsolete storage gate must not coexist with the approved Rương đồ flow.");
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(body.IndexOf(root.Q("Map01A Inventory Detail Panel")),
                    Is.GreaterThan(body.IndexOf(root.Q("Map01A Inventory Grid Panel"))),
                    "Item detail must stay on the right side of the bag grid.");
                Assert.That(root.Q("Map01A Inventory Category Rail").style.flexDirection.value, Is.EqualTo(FlexDirection.Column));
                Assert.That(root.Q("Map01A Inventory Category Chips"), Is.Null);
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile main_weapon").ClassListContains("lgo-inventory-grid-cell"), Is.True);
                Assert.That(root.Q<VisualElement>("Map01A Equipment Item Icon main_weapon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetVoEquipmentThumbnailSprite("main_weapon")));
                Assert.That(root.Q("Map01A Inventory Detail Panel").ClassListContains("lgo-detail-card"), Is.True);
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").ClassListContains("lgo-inventory-button-base"), Is.True);

                InvokeBoundButton(infoTab);
                Assert.That(root.Q<Label>("Map01A Inventory Modal Title").text, Is.EqualTo("THÔNG TIN NHÂN VẬT"));
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(body.IndexOf(root.Q("Map01A Inventory Detail Panel")),
                    Is.GreaterThan(body.IndexOf(root.Q("Map01A Inventory Character Panel"))),
                    "Selected equipment detail must stay in the shared right column.");
                var heroPanel = root.Q("Map01A Inventory Character Panel");
                var heroCard = root.Q("Map01A Character Hero Card");
                var heroPortrait = root.Q<VisualElement>("Map01A Character Hero Portrait");
                var heroInfo = root.Q("Map01A Character Hero Info");
                Assert.That(heroPortrait.style.backgroundImage.value.sprite, Is.EqualTo(scene.GetVoAvatarThumbnailSprite()));
                Assert.That(heroInfo.parent, Is.EqualTo(heroPanel),
                    "Approved character hierarchy keeps name/power below the actor instead of squeezing it into a third inner column.");
                Assert.That(heroPortrait.style.height.value.value, Is.GreaterThanOrEqualTo(340),
                    "The full-body preview should use the available character surface after removing the obsolete inner info column.");
                Assert.That(root.Q<Label>("Map01A Inventory Hero Title").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The character surface must not repeat class/gender above the approved full-body composition.");
                Assert.That(root.Q<Label>("Map01A Inventory Hero Meta").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Duplicate HP/MP copy above the actor steals the height required by the approved bottom identity strip.");
                Assert.That(root.Q("Map01A Character Stat Strip").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Character vitals must use one bottom identity stack instead of overlapping a second badge strip.");
                Assert.That(root.Q<Label>("Map01A Character Hero Vitals").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Label>("Map01A Character Hero Loadout").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Label>("Map01A Character Equipment Summary").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The bottom loadout line already carries the equipped count; a second left-aligned summary causes visual overlap.");
                Assert.That(heroCard.style.minHeight.value.value, Is.LessThanOrEqualTo(370));
                Assert.That(root.Q("Map01A Character Hero Left Equipment Rail").childCount, Is.EqualTo(5));
                Assert.That(root.Q("Map01A Character Hero Right Equipment Rail").childCount, Is.EqualTo(5));

                InvokeBoundButton(root.Q<Button>("Map01A Character Hero Quick Icon 7"));
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("boots"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Slot Type").text, Does.Contain("Giày"));
                InvokeBoundButton(root.Q<Button>("Map01A Inventory Detail Primary Action"));
                Assert.That(scene.IsVoEquipmentSlotEquipped("boots"), Is.False);
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
        public void CharacterHubUsesFiveCompactTabsAndOneSharedDetailColumn()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character hub five-tab test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                scene.ToggleInventory();

                var tabs = root.Q("Map01A Inventory Main Tabs");
                Assert.That(tabs.childCount, Is.EqualTo(5),
                    "Approved character hub has one compact row of five tabs, not parallel two/three-tab systems.");
                var expected = new[]
                {
                    ("Map01A Character Info Main Tab", "Nhân vật"),
                    ("Map01A Bag Main Tab", "Rương đồ"),
                    ("Map01A Skills Main Tab", "Kỹ năng"),
                    ("Map01A Potential Main Tab", "Tiềm năng"),
                    ("Map01A Spirit Pet Main Tab", "Linh thú")
                };
                foreach (var item in expected)
                {
                    var tab = root.Q<Button>(item.Item1);
                    Assert.That(tab, Is.Not.Null);
                    Assert.That(tab.text, Is.EqualTo(item.Item2));
                    Assert.That(tab.style.flexGrow.value, Is.EqualTo(0));
                    Assert.That(tab.style.flexBasis.value.value, Is.LessThanOrEqualTo(150),
                        "Hub tabs must stay compact so later tabs do not force another navigation base.");
                    Assert.That(tab.ClassListContains("lgo-inventory-main-tab"), Is.True);
                }
                Assert.That(root.Q("Map01A Storage Main Tab"), Is.Null,
                    "The obsolete third inventory/storage tab must not remain beside the approved character-hub tabs.");
                Assert.That(root.Q("Map01A Inventory Category Chips"), Is.Null,
                    "Approved Rương đồ uses an internal vertical category rail, not a second horizontal tab row.");
                var categoryRail = root.Q("Map01A Inventory Category Rail");
                Assert.That(categoryRail, Is.Not.Null);
                Assert.That(categoryRail.style.flexDirection.value, Is.EqualTo(FlexDirection.Column));
                Assert.That(root.Q("Map01A Inventory Bag Character Panel"), Is.Null,
                    "Rương đồ must remain a two-column grid/detail screen without the obsolete character-preview column.");
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));

                InvokeBoundButton(root.Q<Button>("Map01A Character Info Main Tab"));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));

                InvokeBoundButton(root.Q<Button>("Map01A Skills Main Tab"));
                Assert.That(root.Q("Map01A Skills Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Hub Preview Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));

                InvokeBoundButton(root.Q<Button>("Map01A Potential Main Tab"));
                Assert.That(root.Q("Map01A Potential Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Button>("Map01A Potential Add Point").enabledSelf, Is.False,
                    "Map01A must not create local fake potential progression before the real state contract exists.");

                InvokeBoundButton(root.Q<Button>("Map01A Spirit Pet Main Tab"));
                Assert.That(root.Q("Map01A Spirit Pet Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Button>("Map01A Spirit Pet Develop Action").enabledSelf, Is.False,
                    "Linh thú growth must remain visibly gated until its real progression state exists.");
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
                Assert.That(inventoryToggle.ClassListContains("lgo-hud-navigation-action"), Is.True,
                    "Inventory navigation must share the compact HUD navigation base.");
                Assert.That(characterSelect.ClassListContains("lgo-hud-navigation-action"), Is.True,
                    "Character navigation must share the same compact HUD navigation base.");

                var skills = root.Q<Button>("Map01A Skills Shortcut");
                var menu = root.Q<Button>("Map01A Menu Shortcut");
                Assert.That(skills, Is.Not.Null);
                Assert.That(menu, Is.Not.Null);
                Assert.That(skills.text, Is.EqualTo("Kỹ năng"));
                Assert.That(menu.text, Is.EqualTo("Menu"));
                Assert.That(skills.enabledSelf, Is.True, "Kỹ năng shortcut should open the approved shared character hub.");
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
                InvokeBoundButton(skills);
                Assert.That(scene.InventoryOpen, Is.True);
                Assert.That(root.Q("Map01A Skills Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));

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
                Assert.That(loginButton.ClassListContains("lgo-entry-auth-primary"), Is.True,
                    "Login must use the shared blue auth-action role from the approved entry hierarchy.");
                Assert.That(loginButton.text, Is.EqualTo("Đăng nhập"));
                var registerButton = root.Q<Button>("Map01A Entry Register Button");
                Assert.That(registerButton.text, Is.EqualTo("Đăng ký"));
                Assert.That(registerButton.ClassListContains("lgo-entry-auth-secondary"), Is.True,
                    "Register must use the shared gold-outline auth-action role instead of matching the login fill.");
                Assert.That(loginButton.style.backgroundColor.value.b, Is.GreaterThan(registerButton.style.backgroundColor.value.b),
                    "The primary login action should read as blue while register remains visually secondary.");
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
                Assert.That(loginButton.style.minHeight.value.value, Is.LessThanOrEqualTo(40),
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
        public void ObsoleteStorageTabIsRemovedWithoutChangingLoadout()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("obsolete storage flow removed test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                scene.ToggleInventory();
                InvokeBoundButton(root.Q<Button>("Map01A Equipment Item Tile boots"));
                var selected = scene.VoSelectedEquipmentSlot;

                Assert.That(root.Q("Map01A Storage Main Tab"), Is.Null);
                Assert.That(root.Q("Map01A Storage Panel"), Is.Null);
                Assert.That(root.Q<Button>("Map01A Bag Main Tab").text, Is.EqualTo("Rương đồ"));
                InvokeBoundButton(root.Q<Button>("Map01A Character Info Main Tab"));
                InvokeBoundButton(root.Q<Button>("Map01A Bag Main Tab"));
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo(selected),
                    "Switching approved tabs must preserve the actual selected/equipped state.");
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
                var host = new GameObject("five-tab review capture test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;

                var expected = new[]
                {
                    ("character-info", "Map01A Inventory Character Panel", "THÔNG TIN NHÂN VẬT"),
                    ("bag", "Map01A Inventory Grid Panel", "HÀNH TRANG"),
                    ("skills", "Map01A Skills Panel", "KỸ NĂNG"),
                    ("potential", "Map01A Potential Panel", "TIỀM NĂNG"),
                    ("spirit-pet", "Map01A Spirit Pet Panel", "LINH THÚ")
                };
                foreach (var view in expected)
                {
                    hud.OpenInventoryReviewMode(view.Item1);
                    Assert.That(scene.InventoryOpen, Is.True);
                    Assert.That(root.Q(view.Item2).style.display.value, Is.EqualTo(DisplayStyle.Flex), view.Item1);
                    Assert.That(root.Q<Label>("Map01A Inventory Modal Title").text, Is.EqualTo(view.Item3), view.Item1);
                    if (view.Item1 == "bag")
                    {
                        var itemsGrid = root.Q("Map01A Inventory Items Grid");
                        var equipmentItem = root.Q<Button>("Map01A Equipment Item Tile main_weapon");
                        var supplyItem = root.Q<Button>("Map01A Health Potion");
                        Assert.That(itemsGrid.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                        Assert.That(equipmentItem.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                            "Opening Rương đồ from another main tab must restore equipment in the approved Tất cả view.");
                        Assert.That(supplyItem.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                            "The approved Tất cả view must show actual inventory items alongside equipment.");
                        Assert.That(equipmentItem.parent, Is.EqualTo(supplyItem.parent),
                            "Tất cả must use one shared wrapping grid so item groups do not fall into separate off-screen pages.");
                        Assert.That(root.Q<Button>("Map01A All Items Category").style.backgroundColor.value.b,
                            Is.GreaterThan(root.Q<Button>("Map01A Equipment Tab").style.backgroundColor.value.b),
                            "The category rail must visibly select Tất cả when Rương đồ opens.");
                    }
                }
                Assert.That(root.Q<Button>("Map01A Skill Upgrade Action").enabledSelf, Is.False);
                Assert.That(root.Q<Button>("Map01A Potential Add Point").enabledSelf, Is.False);
                Assert.That(root.Q<Button>("Map01A Spirit Pet Develop Action").enabledSelf, Is.False);
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
                Assert.That(root.Q("Map01A Inventory Items Grid").style.display.value, Is.EqualTo(DisplayStyle.Flex));
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
                var playerCluster = root.Q("Map01A Player Status Cluster");
                var rightCluster = root.Q("Map01A Right Hud Cluster");
                Assert.That(playerCluster.ClassListContains("lgo-hud-composition"), Is.True);
                Assert.That(rightCluster.ClassListContains("lgo-hud-composition"), Is.True);
                Assert.That(root.Q("Map01A Vitals").parent, Is.EqualTo(playerCluster));
                foreach (var name in new[] { "Map01A Location Title", "Map01A Minimap", "Map01A Quest Tracker Tabs", "Map01A Quest Tracker Body" })
                    Assert.That(root.Q(name).parent, Is.EqualTo(rightCluster), name + " must stay inside the shared right-side HUD composition.");
                var locationTitle = root.Q("Map01A Location Title");
                Assert.That(locationTitle, Is.Not.Null,
                    "HUD location title needs a named shared info panel for visual audit and base-first reuse.");
                Assert.That(locationTitle.ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD location title must use the shared info-panel base instead of the legacy Box helper.");
                Assert.That(root.Q("Map01A Vitals").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD vitals must share the same info-panel base as the title and quest tracker.");
                Assert.That(root.Q("Map01A Quest Tracker Body").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD quest body must use the shared info-panel base instead of one-off panel styling.");
                Assert.That(root.Q("Map01A Quest Tracker Body").style.backgroundColor.value.a, Is.GreaterThanOrEqualTo(.9f),
                    "Quest tracking needs a readable shared glass background over bright map art.");
                Assert.That(root.Q("Map01A Minimap").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD minimap placeholder must use the shared info-panel base until a real minimap art pass replaces it.");
                Assert.That(root.Q("Map01A Minimap").style.backgroundColor.value.a, Is.GreaterThanOrEqualTo(.9f),
                    "Map and quest cards must remain readable over bright sky and share one HUD background token.");
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
                Assert.That(runAction.style.fontSize.value.value, Is.LessThanOrEqualTo(13f),
                    "Bottom HUD action buttons must stay compact on Player.");
                Assert.That(runAction.style.minHeight.value.value, Is.InRange(54f, 64f),
                    "Desktop combat icons must stay readable without growing to modal CTA scale.");
                Assert.That(skillAction.style.fontSize.value.value, Is.LessThanOrEqualTo(13f));
                var basicAttack = root.Q<Button>("Map01A Basic Attack Action");
                Assert.That(basicAttack.ClassListContains("lgo-hud-primary-combat-action"), Is.True,
                    "The primary attack must have an explicit shared emphasis role instead of four equal debug buttons.");
                Assert.That(basicAttack.style.minWidth.value.value, Is.GreaterThan(runAction.style.minWidth.value.value));
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
                    Assert.That(button.style.height.value.value, Is.InRange(60f, 76f));
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
                Assert.That(portrait.style.width.value.value, Is.GreaterThanOrEqualTo(104f),
                    "NPC portrait must carry the conversation visually instead of reading like a small inventory thumbnail.");
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
