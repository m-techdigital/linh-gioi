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
                var body = root.Q("Map01A Inventory Body");
                Assert.That(body.IndexOf(root.Q("Map01A Inventory Detail Panel")), Is.GreaterThan(body.IndexOf(root.Q("Map01A Inventory Grid Panel"))),
                    "Item detail must stay on the right side of the bag grid.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Header").text, Does.Contain("CHI TIẾT"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail State Badge").text, Does.Contain("ĐANG MẶC"));
                var weaponThumbnail = scene.GetVoEquipmentThumbnailSprite("main_weapon");
                Assert.That(weaponThumbnail, Is.Not.Null);
                Assert.That(weaponThumbnail.rect.height, Is.GreaterThan(weaponThumbnail.rect.width),
                    "Weapon inventory thumbnail should use the tight vertical runtime component crop, not the wide transparent slot sheet.");
                var detailIcon = root.Q<Label>("Map01A Inventory Detail Icon");
                Assert.That(detailIcon.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Inventory detail must show real runtime equipment art instead of hiding behind fake icons.");
                Assert.That(detailIcon.text, Is.Empty,
                    "Inventory must not present emoji/text badges as final item art.");
                Assert.That(detailIcon.style.width.value.value, Is.GreaterThanOrEqualTo(72),
                    "Right-side item detail card should present a larger hero thumbnail than grid tiles.");
                var weaponTileIcon = root.Q<VisualElement>("Map01A Equipment Item Icon main_weapon");
                Assert.That(weaponTileIcon, Is.Not.Null,
                    "Equipment grid tiles must show the same real runtime thumbnail art, not text-only placeholders.");
                Assert.That(weaponTileIcon.style.backgroundImage.value.sprite, Is.EqualTo(weaponThumbnail));
                Assert.That(weaponTileIcon.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(weaponTileIcon.style.width.value.value, Is.GreaterThanOrEqualTo(64),
                    "Grid item thumbnails should be the visual anchor of the tile.");
                var mainWeaponTile = root.Q<Button>("Map01A Equipment Item Tile main_weapon");
                Assert.That(mainWeaponTile.style.height.value.value, Is.InRange(104, 116),
                    "Inventory equipment tiles should read as compact square-ish game item cells, not long technical rows.");
                Assert.That(mainWeaponTile.style.flexBasis.value.value, Is.LessThanOrEqualTo(17f),
                    "Inventory equipment tiles should use a dense 5-6 column bag grid close to the owner bag references.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Rarity").text, Does.Contain("Lv"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Stat Primary").text, Is.EqualTo("Chưa có thuộc tính chiến đấu được công bố."));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Stat Fit").text, Does.Contain("Dành cho"));
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action"), Is.Not.Null);
                Assert.That(root.Q<Button>("LGO Equipment Inventory Variant").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Unavailable variant actions should not draw a disabled dead button in the narrow detail card.");
                var detailActions = root.Q("Map01A Inventory Equipment Actions");
                Assert.That(detailActions, Is.Not.Null);
                Assert.That(detailActions.style.marginBottom.value.value, Is.GreaterThanOrEqualTo(10),
                    "Right-side detail actions need breathing room above the bottom edge in Player layout.");
                Assert.That(root.Q<Button>("Map01A Bag Main Tab").style.minHeight.value.value, Is.LessThanOrEqualTo(38),
                    "Inventory top tabs should stay compact like the owner reference, not inherit oversized web-button height.");
                Assert.That(root.Q<Button>("Map01A Equipment Tab").style.minHeight.value.value, Is.LessThanOrEqualTo(38),
                    "Inventory sub-tabs should share the compact game-tab density.");
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile main_weapon").text, Is.Empty,
                    "Equipment tile Button.text must stay empty so UIToolkit does not draw text over the runtime thumbnail and child labels.");
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile boots").text, Is.Empty);
                Assert.That(root.Q<Label>("Map01A Equipment Item Name main_weapon").text, Does.Contain("Vũ khí"));
                Assert.That(root.Q<Label>("Map01A Equipment Item Name boots").text, Does.Contain("Giày"));
                var emptyBagSlot = root.Q("Map01A Empty Bag Slot 01");
                Assert.That(emptyBagSlot, Is.Not.Null,
                    "Bag layout should reserve empty inventory cells so the screen reads as a game bag grid, not a sparse debug list.");
                Assert.That(emptyBagSlot.style.flexBasis.value.value, Is.LessThanOrEqualTo(17f));

                InvokeBoundButton(infoTab);
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(body.IndexOf(root.Q("Map01A Inventory Detail Panel")), Is.GreaterThan(body.IndexOf(root.Q("Map01A Inventory Character Panel"))),
                    "Item detail must stay on the right side of character equipment slots.");

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
                Assert.That(root.Q<Label>("Map01A Character Select Title").text, Does.Contain("Chọn Nhân Vật"));
                foreach (var label in new[] { "Võ", "Kiếm", "Pháp", "Cơ", "Linh" })
                    Assert.That(root.Q<Button>("Map01A Character Card " + label), Is.Not.Null);
                var phap = root.Q<Button>("Map01A Character Card Pháp");
                Assert.That(phap.enabledSelf, Is.False, "Pháp must stay visible but disabled while source promotion is held out");
                Assert.That(phap.text, Does.Contain("đang audit"));
                Assert.That(root.Q<Label>("Map01A Character Select Scope").text, Does.Contain("review"));
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

                var skills = root.Q<Button>("Map01A Skills Shortcut");
                var menu = root.Q<Button>("Map01A Menu Shortcut");
                Assert.That(skills, Is.Not.Null);
                Assert.That(menu, Is.Not.Null);
                Assert.That(skills.text, Is.EqualTo("Kỹ năng"));
                Assert.That(menu.text, Is.EqualTo("Menu"));
                Assert.That(skills.enabledSelf, Is.False, "Kỹ năng shortcut must stay visibly gated until the real screen exists.");
                Assert.That(menu.enabledSelf, Is.False, "Menu shortcut must stay visibly gated until the real screen exists.");
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
                Assert.That(root.Q<Button>("Map01A Entry Login Button"), Is.Not.Null);
                Assert.That(root.Q("Map01A Entry Account Field"), Is.Not.Null);
                var accountPlaceholder = root.Q<Label>("Map01A Entry Account Placeholder").text;
                Assert.That(accountPlaceholder, Does.Contain("Tài khoản"));
                Assert.That(accountPlaceholder, Does.Not.Contain("👤"), "Login must not use temporary emoji glyphs as field icons.");
                Assert.That(root.Q("Map01A Entry Password Field"), Is.Not.Null);
                var passwordPlaceholder = root.Q<Label>("Map01A Entry Password Placeholder").text;
                Assert.That(passwordPlaceholder, Does.Contain("Mật khẩu"));
                Assert.That(passwordPlaceholder, Does.Not.Contain("🔒"), "Login must not use temporary emoji glyphs as field icons.");
                Assert.That(root.Q<Label>("Map01A Entry Auth Scope").text, Does.Contain("trải nghiệm"));
                Assert.That(root.Q<Label>("Map01A Entry Brand Seal").text, Does.Contain("Đông Lâm"));
                Assert.That(root.Q<Label>("Map01A Entry Server Name").text, Does.Contain("S1"));
                Assert.That(root.Q<Label>("Map01A Entry Server State").text, Does.Contain("Mượt"));
                var serverSwitch = root.Q<Button>("Map01A Entry Server Switch");
                Assert.That(serverSwitch, Is.Not.Null,
                    "Entry/login should reserve the design server-switch affordance without opening production server routing.");
                Assert.That(serverSwitch.enabledSelf, Is.False);
                Assert.That(serverSwitch.text, Does.Contain("Đổi máy chủ"));
                Assert.That(serverSwitch.text, Does.Not.Contain("chưa mở"),
                    "Disabled design affordances should read like game UI, not debug placeholder copy.");
                var start = root.Q<Button>("Map01A Entry Start Button");
                Assert.That(start, Is.Not.Null);
                Assert.That(root.Q("Map01A Entry Primary Cta Row"), Is.Not.Null,
                    "Entry reference uses the start action as its own gold CTA row instead of burying it beside secondary auth actions.");
                Assert.That(root.Q("Map01A Entry Secondary Actions"), Is.Not.Null,
                    "Secondary login actions should stay visually subordinate to the primary start CTA.");
                var loginButton = root.Q<Button>("Map01A Entry Login Button");
                Assert.That(loginButton.text, Does.Contain("Vào nhanh"));
                Assert.That(loginButton.text, Does.Not.Contain("dev"), "Entry/login surface must not expose developer wording to the player.");
                Assert.That(start.style.minHeight.value.value, Is.GreaterThan(loginButton.style.minHeight.value.value));
                Assert.That(start.style.minHeight.value.value, Is.LessThanOrEqualTo(50),
                    "Primary login CTA should feel like a polished game button, not an oversized web form control.");
                Assert.That(start.style.fontSize.value.value, Is.LessThanOrEqualTo(21),
                    "Primary CTA typography must stay below the oversized prototype style.");
                Assert.That(loginButton.style.minHeight.value.value, Is.LessThanOrEqualTo(38),
                    "Secondary login actions should be compact links/buttons under the main CTA.");
                Assert.That(start.style.maxWidth.value.value, Is.GreaterThan(300));
                Assert.That(root.Q<Label>("Map01A Entry Login Title").text, Does.Contain("Đăng nhập"));
                Assert.That(root.Q<Label>("Map01A Entry Hero Motto").text, Does.Contain("Chính nghĩa"));
                Assert.That(overlay.style.backgroundColor.value.a, Is.LessThanOrEqualTo(.66f),
                    "Entry/login should keep the Đông Lâm scene visible behind the glass layer instead of blacking it out.");
                Assert.That(root.Q("Map01A Entry Panel Glow"), Is.Not.Null,
                    "Entry/login needs a reusable visual depth layer so it does not read like a flat HTML form.");
                Assert.That(root.Q("Map01A Entry Cta Ornament Left"), Is.Not.Null,
                    "Primary CTA should carry game-style ornament rails instead of being only a plain text button.");
                Assert.That(root.Q("Map01A Entry Cta Ornament Right"), Is.Not.Null);
                Assert.That(root.Q("Map01A Entry Control Card"), Is.Not.Null,
                    "Login fields, auth options and server selection should sit inside one design card, matching the owner reference hierarchy.");
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
                Assert.That(forgotPassword.text, Does.Not.Contain("chưa mở"),
                    "Disabled design affordances should avoid exposing unfinished-state copy on the main login surface.");
                Assert.That(supportLink.text, Does.Not.Contain("chưa mở"));
                Assert.That(start.text, Does.Contain("Bắt đầu"));
                Assert.That(root.Q<Label>("Map01A Entry Safety Note").text, Does.Contain("local"));
                foreach (var name in new[] { "Thông Báo", "Cài Đặt", "Hỗ Trợ" })
                {
                    var sideAction = root.Q<Button>("Map01A Entry Side Action " + name);
                    Assert.That(sideAction, Is.Not.Null);
                    Assert.That(sideAction.enabledSelf, Is.False, "Entry/login side actions must not be clickable dead buttons.");
                    Assert.That(sideAction.text, Is.EqualTo(name));
                    Assert.That(sideAction.style.whiteSpace.value, Is.EqualTo(WhiteSpace.NoWrap),
                        "Entry side actions must stay compact and must not wrap into two-line placeholders.");
                    Assert.That(sideAction.resolvedStyle.fontSize, Is.LessThanOrEqualTo(13f));
                    Assert.That(sideAction.resolvedStyle.height, Is.LessThanOrEqualTo(42f));
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
                Assert.That(root.Q("Map01A Inventory Character Panel").style.flexGrow.value, Is.EqualTo(1),
                    "Tab content must fill its column immediately without requiring a viewport resize.");
                Assert.That(scene.InventoryOpen, Is.True);
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                var characterWeaponIcon = root.Q("Map01A Character Info Slot Icon main_weapon");
                Assert.That(characterWeaponIcon, Is.Not.Null,
                    "Character-info equipment slots should reuse runtime item thumbnails instead of staying as text-only cells.");
                Assert.That(characterWeaponIcon.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                var modalTitle = root.Q<Label>("Map01A Inventory Modal Title");
                var modalSubtitle = root.Q<Label>("Map01A Inventory Modal Subtitle");
                Assert.That(modalTitle, Is.Not.Null, "Inventory modal needs a named title so each main tab can present its own screen.");
                Assert.That(modalSubtitle, Is.Not.Null, "Inventory modal needs a named subtitle so tab context is visible in Player captures.");
                Assert.That(modalTitle.text, Is.EqualTo("THÔNG TIN"),
                    "Switching to character info must not leave the modal titled HÀNH TRANG.");
                Assert.That(modalSubtitle.text, Does.Contain("trang bị đang mặc"));

                hud.OpenInventoryReviewMode("supplies");
                Assert.That(modalTitle.text, Is.EqualTo("HÀNH TRANG"));
                Assert.That(root.Q("Map01A Supplies Page").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Supplies List Card"), Is.Not.Null,
                    "Supplies should present a left-side list card beside the right detail card, not a flat full-width technical list.");
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Supplies must keep the right-side detail panel instead of becoming a left-only technical list.");
                Assert.That(root.Q<Label>("Map01A Supplies Empty State").text, Does.Contain("Chưa nhận"));
                Assert.That(root.Q<Button>("Map01A Health Potion").text, Is.Empty,
                    "Supply rows must be composed cards, not plain Button.text labels that look like temporary debug UI.");
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
                Assert.That(root.Q("Map01A Inventory Character Panel").style.flexGrow.value, Is.EqualTo(1),
                    "Tab content must fill its column immediately without requiring a viewport resize.");
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
                Assert.That(root.Q<Label>("Map01A Inventory Detail Slot Type").text, Is.EqualTo("Vật phẩm hồi phục"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail State Badge").text, Is.EqualTo("CÓ THỂ DÙNG"));
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
                Assert.That(root.Q("Map01A Quest Tracker Tabs"), Is.Not.Null,
                    "HUD quest tracker should expose the Nhiệm Vụ/Đội tab structure from the product reference instead of staying as a plain text block.");
                Assert.That(root.Q<Button>("Map01A Quest Tab Missions").text, Does.Contain("Nhiệm vụ"));
                var questMissionTab = root.Q<Button>("Map01A Quest Tab Missions");
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
                Assert.That(runAction.style.whiteSpace.value, Is.EqualTo(WhiteSpace.NoWrap),
                    "Bottom HUD action buttons must not wrap into oversized blocks.");
                Assert.That(runAction.resolvedStyle.fontSize, Is.LessThanOrEqualTo(13f),
                    "Bottom HUD action buttons must stay compact on Player.");
                Assert.That(runAction.resolvedStyle.height, Is.LessThanOrEqualTo(44f),
                    "Bottom HUD action buttons must stay below modal CTA height.");
                Assert.That(skillAction.resolvedStyle.fontSize, Is.LessThanOrEqualTo(13f));
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
                var actionRow = root.Q("Map01A Dialogue Actions");
                Assert.That(actionRow, Is.Not.Null, "Dialogue panel must use a named action row instead of loose buttons.");
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
