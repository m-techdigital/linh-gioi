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
        public void CharacterHubCatalogDefinesFiveClassProfilesWithExplicitContentData()
        {
            var profiles = CharacterHubClassCatalog.Profiles;

            Assert.That(profiles, Is.Not.InstanceOf<CharacterHubClassProfile[]>(),
                "The shared catalog must not expose its mutable backing array.");
            Assert.That(profiles.Select(profile => profile.Id),
                Is.EqualTo(new[] { "vo", "kiem", "phap", "co", "linh" }));
            Assert.That(profiles.Select(profile => profile.Label),
                Is.EqualTo(new[] { "Võ", "Kiếm", "Pháp", "Cơ", "Linh" }));
            var expectedDefaultPotentials = new Dictionary<string, string>
            {
                ["vo"] = "Sinh lực",
                ["kiem"] = "Nhanh nhẹn",
                ["phap"] = "Linh lực",
                ["co"] = "Công",
                ["linh"] = "Linh lực"
            };
            foreach (var profile in profiles)
            {
                Assert.That(profile.Skills, Is.Not.InstanceOf<CharacterHubSkillPreview[]>(), profile.Id);
                Assert.That(profile.EquippedSkillIndices, Is.Not.InstanceOf<int[]>(), profile.Id);
                Assert.That(profile.SpiritPet.Skills,
                    Is.Not.InstanceOf<CharacterHubSpiritPetPreview.SkillPreview[]>(), profile.Id);
                Assert.That(profile.SpiritPet.Stats, Is.Not.InstanceOf<string[]>(), profile.Id);
                Assert.That(profile.SpiritPet.Stats.Count, Is.EqualTo(5), profile.Id);
                Assert.That(profile.Skills.Count, Is.EqualTo(9), profile.Id + " must fill the shared 3x3 skill path.");
                Assert.That(profile.Potentials.Count, Is.EqualTo(5), profile.Id + " must fill the shared potential diagram.");
                Assert.That(profile.EquippedSkillIndices.Count, Is.EqualTo(4));
                Assert.That(profile.SpiritPet, Is.Not.Null);
                Assert.That(profile.SpiritPet.Synergy, Is.Not.Empty);
                Assert.That(profile.SpiritPet.Skills.Count, Is.EqualTo(2),
                    profile.Id + " must bind both fixed spirit-pet skill rows.");
                Assert.That(profile.SpiritPet.Skills.All(skill => !string.IsNullOrEmpty(skill.IconId)
                    && !string.IsNullOrEmpty(skill.Description)), Is.True);
                Assert.That(profile.Skills.All(skill => !string.IsNullOrEmpty(skill.Description)), Is.True);
                Assert.That(profile.Potentials.All(potential => !string.IsNullOrEmpty(potential.Summary)
                    && !string.IsNullOrEmpty(potential.CurrentEffect)
                    && !string.IsNullOrEmpty(potential.NextEffect)), Is.True,
                    profile.Id + " must bind the shared Potential detail sections without embedding layout copy in one blob.");
                Assert.That(profile.DefaultPotentialName, Is.EqualTo(expectedDefaultPotentials[profile.Id]),
                    profile.Id + " must select its recommended starting Potential through profile data.");
                Assert.That(profile.Potentials.Any(potential => potential.Name == profile.DefaultPotentialName), Is.True);
                if (profile.Id != "kiem")
                    Assert.That(profile.Skills.All(skill => skill.IconCatalog == CharacterHubIconCatalog.Hud), Is.True,
                        profile.Id + " must use provenance-backed shared icons instead of pretending Kiếm art belongs to another class.");
            }
            for (var index = 1; index < profiles.Count; index++)
                Assert.That(ReferenceEquals(profiles[0].Potentials, profiles[index].Potentials), Is.True,
                    "The five generic potential definitions must be one immutable shared catalog; class profiles only bind recommendation and selection state.");
        }

        [Test]
        public void CharacterHubSelectionStateIsIsolatedPerClass()
        {
            var state = new CharacterHubSelectionState();
            var profiles = CharacterHubClassCatalog.Profiles;
            for (var index = 0; index < profiles.Count; index++)
            {
                var profile = profiles[index];
                var skill = profile.Skills[(index + 2) % profile.Skills.Count];
                var potential = profile.Potentials[(index + 1) % profile.Potentials.Count];
                state.SelectSkill(profile, skill.Id);
                state.SelectPotential(profile, potential.Name);
            }

            for (var index = 0; index < profiles.Count; index++)
            {
                var profile = profiles[index];
                Assert.That(state.SkillIdFor(profile),
                    Is.EqualTo(profile.Skills[(index + 2) % profile.Skills.Count].Id), profile.Id);
                Assert.That(state.PotentialNameFor(profile),
                    Is.EqualTo(profile.Potentials[(index + 1) % profile.Potentials.Count].Name), profile.Id);
            }

            var vo = CharacterHubClassCatalog.Get("vo");
            Assert.Throws<System.ArgumentException>(() => state.SelectSkill(vo, "kiem_skill_1"));
            Assert.Throws<System.ArgumentException>(() => state.SelectPotential(vo, "không tồn tại"));
        }

        [Test]
        public void SharedEquipmentContractReadsClassCompatibilityFromActiveActorData()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("shared equipment class-data test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                Assert.That(scene.IsClassRewardCompatible, Is.True);
                Assert.That(scene.ClassRewardItemId, Is.EqualTo("map01a_vo_wrist_guard_reward"));

                var sourcePose = new GameObject("active class-data actor").AddComponent<TwoDSourcePoseReview>();
                sourcePose.transform.SetParent(scene.transform, false);
                typeof(TwoDSourcePoseReview)
                    .GetField("<ClassId>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(sourcePose, "kiem");
                typeof(CongDongLamMap01AArtPreview)
                    .GetField("_sourcePoseReview", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(scene, sourcePose);

                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo("kiem"));
                Assert.That(scene.IsClassRewardCompatible, Is.False,
                    "The shared inventory flow must read compatibility from item/class data, not a per-class UI branch.");
                Assert.That(scene.EquipmentSlotIds.Count, Is.EqualTo(10));
                Assert.That(scene.SelectedEquipmentSlot, Is.EqualTo(scene.EquipmentSlotIds[0]));
                Assert.That(scene.GetEquipmentItemId(scene.SelectedEquipmentSlot),
                    Does.StartWith("kiem_"),
                    "Fallback item identity must be derived from active actor data, not fixed to Võ.");
            }
            finally
            {
                foreach (var item in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(item)) Object.DestroyImmediate(item);
            }
        }

        [Test]
        public void ProductPlayerRejectsTwoCharacterRendererAuthorities()
        {
            Assert.That(CongDongLamMap01AArtPreview.HasRendererAuthorityConflict(new[]
            {
                "Player", "--lgo-vo-registered", "--lgo-vo-pose-review-dir", "/tmp/pose"
            }), Is.True);
            Assert.That(CongDongLamMap01AArtPreview.HasRendererAuthorityConflict(new[]
            {
                "Player", "--lgo-vo-pose-review-dir", "/tmp/pose"
            }), Is.False);
            Assert.That(CongDongLamMap01AArtPreview.HasRendererAuthorityConflict(new[]
            {
                "Player", "--lgo-vo-registered", "--lgo-registered-capture",
                "--lgo-vo-pose-review-dir", "/tmp/pose"
            }), Is.False, "The isolated registered comparison capture remains explicit WIP evidence.");
        }

        [Test]
        public void CharacterHubDoesNotReviveLegacyStaticClassPreviewAndKeepsFixedActorStage()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character hub class binding test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var sourcePose = new GameObject("current source-pose actor").AddComponent<TwoDSourcePoseReview>();
                sourcePose.transform.SetParent(scene.transform, false);
                typeof(CongDongLamMap01AArtPreview)
                    .GetField("_sourcePoseReview", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(scene, sourcePose);
                typeof(CongDongLamMap01AArtPreview)
                    .GetMethod("RefreshVoAvatarMode", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(scene, null);
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                scene.ToggleInventory();
                typeof(CongDongLamArrivalHud).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(host.GetComponentInChildren<CongDongLamArrivalHud>(), null);
                var shell = root.Q("Map01A Inventory");
                Assert.That(scene.CanCycleCharacterHubClass, Is.False,
                    "A Character Hub without registered source-pose alternatives must not expose static class-fit packs.");
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo("vo"));
                Assert.That(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
                    .SelectMany(item => item.GetComponentsInChildren<Transform>(true))
                    .All(item => !item.name.Contains("ten-slot shared-rig review")), Is.True);
                var legacyAvatar = scene.GetComponentsInChildren<Transform>(true)
                    .Single(item => item.name == "Map01A Võ avatar");
                Assert.That(legacyAvatar.GetComponentsInChildren<SpriteRenderer>(true)
                    .All(renderer => renderer.forceRenderingOff), Is.True,
                    "A source-pose actor must be the sole character renderer; the atlas/rig avatar must never remain renderable in parallel.");
                Assert.That(root.Q("Map01A Inventory"), Is.SameAs(shell));
                var portrait = root.Q("Map01A Character Hero Portrait");
                Assert.That(portrait.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "The fixed character stage must not collapse while a class portrait is unavailable.");
                Assert.That(portrait.style.width.value.value, Is.EqualTo(400));
                Assert.That(root.Q("Map01A Character Hero Left Equipment Rail").style.width.value.value, Is.EqualTo(76));
                Assert.That(root.Q("Map01A Character Hero Right Equipment Rail").style.width.value.value, Is.EqualTo(76));
                foreach (var slot in scene.VoEquipmentSlotIds)
                    Assert.That(scene.GetVoEquipmentThumbnailSprite(slot),
                        Is.EqualTo(scene.GetMap01ACharacterEquipmentIconSprite(slot)),
                        "The current source-pose actor must keep the dedicated readable slot icon; "
                        + "it must not fall back to a cropped legacy renderer sprite for " + slot);
                Assert.That(root.Q<Button>("Map01A Inventory Gender").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "A single-gender source pack must not leave a disabled status button under the fixed actor stage.");
            }
            finally
            {
                foreach (var item in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(item)) Object.DestroyImmediate(item);
            }
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
        public void SharedStateRestoresOneValidatedLoadoutSnapshot()
        {
            var state = CreateState();

            state.SelectMode("modular");
            state.SetEquipmentState("boots", new[] { "weapon", "boots" });

            Assert.That(state.Mode, Is.EqualTo("modular"));
            Assert.That(state.SelectedEquipmentSlot, Is.EqualTo("boots"));
            Assert.That(state.IsEquipped("weapon"), Is.True);
            Assert.That(state.IsEquipped("hair"), Is.False);
            Assert.That(state.IsEquipped("boots"), Is.True);
            Assert.That(state.EquippedSlotCount, Is.EqualTo(2));
            Assert.That(() => state.SelectMode("unknown"), Throws.ArgumentException);
            Assert.That(() => state.SetEquipmentState("weapon", new[] { "unknown" }), Throws.ArgumentException);
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
            var characterScreenArgs = new[] { "LinhGioiOnline", "--lgo-map01a-character-screen-capture" };
            Assert.That(CongDongLamMap01AArtPreview.ShouldRunForArgs(characterScreenArgs), Is.True);
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
            var serverArgs = new[] { "LinhGioiOnline", "--lgo-map01a-server-select-capture" };
            Assert.That(CongDongLamMap01AArtPreview.ShouldRunForArgs(serverArgs), Is.True);
            Assert.That(CongDongLamMap01AArtPreview.IsMapQuestCaptureForArgs(serverArgs), Is.False);
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(serverArgs, sceneIsCapturing: true), Is.True,
                "Server select capture reuses the Entry scene layers while replacing only its control card.");
            var registerArgs = new[] { "LinhGioiOnline", "--lgo-map01a-register-capture" };
            Assert.That(CongDongLamMap01AArtPreview.ShouldRunForArgs(registerArgs), Is.True);
            Assert.That(CongDongLamMap01AArtPreview.IsMapQuestCaptureForArgs(registerArgs), Is.False);
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(registerArgs, sceneIsCapturing: true), Is.True,
                "Register capture reuses the Entry scene layers while replacing only its control card.");
            var recoveryArgs = new[] { "LinhGioiOnline", "--lgo-map01a-password-recovery-capture" };
            Assert.That(CongDongLamMap01AArtPreview.ShouldRunForArgs(recoveryArgs), Is.True);
            Assert.That(CongDongLamMap01AArtPreview.IsMapQuestCaptureForArgs(recoveryArgs), Is.False);
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(recoveryArgs, sceneIsCapturing: true), Is.True,
                "Password recovery capture reuses the Entry scene layers while replacing only its control card.");
            var menuArgs = new[] { "LinhGioiOnline", "--lgo-map01a-menu-capture" };
            Assert.That(CongDongLamMap01AArtPreview.ShouldRunForArgs(menuArgs), Is.True);
            Assert.That(CongDongLamArrivalHud.ShouldShowEntryOnLaunchForArgs(menuArgs, sceneIsCapturing: true), Is.False,
                "Menu capture should open the playable HUD directly instead of stacking behind entry login.");
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
                Assert.That(root.Q("Map01A Equipment Item Tile main_weapon").style.height.value.value, Is.EqualTo(84));
                Assert.That(root.Q<Label>("Map01A Equipment Item Name main_weapon").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q<Label>("Map01A Equipment Item State main_weapon").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q<VisualElement>("Map01A Inventory Category Icon all").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01ABagCategoryIconSprite("all")));
                var searchInput = root.Q<TextField>("Map01A Inventory Search")
                    .Q(className: "lgo-inventory-search-input");
                Assert.That(searchInput.style.unityTextAlign.value, Is.EqualTo(TextAnchor.MiddleLeft),
                    "The bag search placeholder must remain vertically centered instead of clipping against the top edge.");
                Assert.That(root.Q<Button>("Map01A Inventory Split Action").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q<Button>("Map01A Inventory Sort Action").style.flexGrow.value, Is.EqualTo(1));
                Assert.That(root.Q<Button>("Map01A Inventory Quick Sell Action").style.flexGrow.value, Is.EqualTo(1));
                Assert.That(root.Query<VisualElement>(className: "lgo-inventory-bag-grid-cell").ToList().Count, Is.EqualTo(25),
                    "The approved storage workspace keeps a stable five-by-five grid, including honest empty slots.");
                Assert.That(root.Q<Button>("Map01A All Items Category").style.minHeight.value.value, Is.EqualTo(100),
                    "Five vertical storage categories must fill the approved rail instead of leaving a large dead zone.");
                Assert.That(root.Q("Map01A Inventory Detail Panel").ClassListContains("lgo-detail-card"), Is.True);
                Assert.That(root.Q<Button>("Map01A Inventory Detail Primary Action").ClassListContains("lgo-inventory-button-base"), Is.True);
                var detailSellAction = root.Q<Button>("Map01A Inventory Detail Sell Action");
                Assert.That(detailSellAction, Is.Not.Null);
                Assert.That(detailSellAction.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Storage detail keeps the approved third action without pretending that selling is available.");
                Assert.That(detailSellAction.enabledSelf, Is.False);

                InvokeBoundButton(infoTab);
                Assert.That(root.Q<Label>("Map01A Inventory Modal Title").text, Is.EqualTo("THÔNG TIN NHÂN VẬT"));
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(detailSellAction.style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Character detail has exactly the two actions shown in its canonical screen.");
                Assert.That(body.IndexOf(root.Q("Map01A Inventory Detail Panel")),
                    Is.GreaterThan(body.IndexOf(root.Q("Map01A Inventory Character Panel"))),
                    "Selected equipment detail must stay in the shared right column.");
                var heroPanel = root.Q("Map01A Inventory Character Panel");
                var heroCard = root.Q("Map01A Character Hero Card");
                var heroPortrait = root.Q<VisualElement>("Map01A Character Hero Portrait");
                var heroInfo = root.Q("Map01A Character Hero Info");
                Assert.That(heroPortrait.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "The Character Hub keeps one fixed actor stage in headless tests; Player graphics populate it from the active actor layers.");
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
                Assert.That(root.Q<Label>("Map01A Character Hero Vitals"), Is.Null,
                    "The approved character screen uses readable HP/MP bars instead of retaining a hidden duplicate text system.");
                var heroHealth = root.Q<UnityEngine.UIElements.ProgressBar>("Map01A Character Hero Health");
                var heroMana = root.Q<UnityEngine.UIElements.ProgressBar>("Map01A Character Hero Mana");
                Assert.That(heroHealth, Is.Not.Null);
                Assert.That(heroMana, Is.Not.Null);
                Assert.That(heroHealth.ClassListContains("lgo-vital-bar"), Is.True,
                    "Character and gameplay vitals must reuse the shared progress-bar base.");
                Assert.That(heroMana.ClassListContains("lgo-vital-bar"), Is.True);
                Assert.That(heroHealth.value, Is.EqualTo(scene.PlayerHealth));
                Assert.That(heroMana.value, Is.EqualTo(scene.PlayerMana));
                Assert.That(root.Q<Label>("Map01A Character Hero Loadout").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The ten visible equipment slots already communicate loadout completeness; the approved identity footer keeps only name, power and vitals.");
                Assert.That(root.Q<Label>("Map01A Character Equipment Summary").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The bottom loadout line already carries the equipped count; a second left-aligned summary causes visual overlap.");
                Assert.That(heroCard.style.height.value.value, Is.InRange(425, 435),
                    "The canonical character workspace reserves the upper body for one readable full-body stage.");
                var leftRail = root.Q("Map01A Character Hero Left Equipment Rail");
                var rightRail = root.Q("Map01A Character Hero Right Equipment Rail");
                Assert.That(leftRail.childCount, Is.EqualTo(5));
                Assert.That(rightRail.childCount, Is.EqualTo(5));
                Assert.That(leftRail.style.height.value.value, Is.EqualTo(420));
                Assert.That(root.Q("Map01A Character Hero Quick Icon 0").style.height.value.value, Is.EqualTo(76),
                    "Equipment rail icons must keep square design slots instead of flex-collapsing into short rows.");
                for (var equipmentIndex = 0; equipmentIndex < scene.VoEquipmentSlotIds.Count; equipmentIndex++)
                {
                    var levelBadge = root.Q<Label>("Map01A Character Hero Quick Level " + equipmentIndex);
                    Assert.That(levelBadge, Is.Not.Null,
                        "Every equipped slot must expose its real item level like the approved character design.");
                    Assert.That(levelBadge.text, Is.EqualTo("+" + scene.GetVoEquipmentItemLevel(scene.VoEquipmentSlotIds[equipmentIndex])));
                    Assert.That(levelBadge.ClassListContains("lgo-equipment-level-badge"), Is.True);
                }
                StringAssert.StartsWith("Lv." + scene.VoAvatarLevel + "  ·  LC ",
                    root.Q<Label>("Map01A Character Hero Power").text,
                    "The identity footer must show the real avatar level beside combat power.");
                var detailHeroText = root.Q("Map01A Inventory Detail Hero Text");
                var detailTitleRow = root.Q("Map01A Inventory Detail Title Row");
                Assert.That(detailTitleRow.parent, Is.EqualTo(detailHeroText));
                Assert.That(root.Q("Map01A Inventory Detail Level Chip").parent, Is.EqualTo(detailTitleRow),
                    "Item level belongs in the inspector hero beside the item name.");
                Assert.That(root.Q("Map01A Inventory Detail State Badge").parent, Is.EqualTo(detailHeroText),
                    "Equipped state belongs in the inspector hero instead of a detached duplicate row.");
                Assert.That(root.Q("Map01A Inventory Detail Equipped Chip"), Is.Null,
                    "The approved inspector has one equipped-state badge, not two parallel state systems.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Stats Header").text, Is.EqualTo("THUỘC TÍNH"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Set Header").text, Is.EqualTo("BỘ TRANG BỊ HIỆN TẠI"));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Header").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The approved detail hierarchy starts with the selected item hero, without a redundant technical header.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Icon").style.width.value.value, Is.EqualTo(120));
                var lockAction = root.Q<Button>("Map01A Inventory Detail Lock Action");
                var primaryAction = root.Q<Button>("Map01A Inventory Detail Primary Action");
                Assert.That(lockAction, Is.Not.Null);
                Assert.That(lockAction.text, Is.EqualTo("Khóa"));
                InvokeBoundButton(lockAction);
                Assert.That(lockAction.text, Is.EqualTo("Mở khóa"));
                Assert.That(primaryAction.enabledSelf, Is.False,
                    "A locked equipment item cannot be equipped or removed until it is unlocked.");

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
                    Assert.That(tab.style.flexGrow.value, Is.EqualTo(1),
                        "The five canonical tabs must share their compact navigation rail equally.");
                    Assert.That(tab.style.flexBasis.value.value, Is.EqualTo(0),
                        "All five tabs must derive equal width from one reusable navigation base.");
                    Assert.That(tab.ClassListContains("lgo-inventory-main-tab"), Is.True);
                }
                Assert.That(tabs.style.width.value.value, Is.EqualTo(992).Within(1),
                    "The approved navigation rail is compact and leaves the close/title edge clear for future tabs.");
                var modal = root.Q("Map01A Inventory");
                var backdrop = root.Q("Map01A Character Hub Backdrop");
                var modalTitle = root.Q<Label>("Map01A Inventory Modal Title");
                var close = root.Q<Button>("LGO Inventory Close");
                Assert.That(modalTitle.style.fontSize.value.value, Is.EqualTo(30).Within(1),
                    "The five-tab shell title must match the canonical visual hierarchy.");
                Assert.That(modalTitle.style.color.value, Is.EqualTo(new Color(.98f, .98f, .94f, 1f)),
                    "The canonical title is light, while gold remains an accent color.");
                Assert.That(close.style.flexBasis.value.value, Is.EqualTo(52).Within(1),
                    "The close control must stay subordinate to the modal title.");
                Assert.That(close.style.borderTopWidth.value, Is.EqualTo(0),
                    "Character Hub chrome already draws its border inside the texture; a CSS border would create a nested frame.");
                Assert.That(modal.ClassListContains("lgo-ornamented-shell"), Is.True,
                    "The Character Hub must keep its decorative frame rendered inside the shell.");
                Assert.That(modal.style.borderTopWidth.value, Is.EqualTo(0));
                Assert.That(modal.style.borderRightWidth.value, Is.EqualTo(0));
                Assert.That(modal.style.borderBottomWidth.value, Is.EqualTo(0));
                Assert.That(modal.style.borderLeftWidth.value, Is.EqualTo(0),
                    "The shell ornament is the only frame; an outer CSS border would create a second box.");
                Assert.That(modal.style.backgroundColor.value.a, Is.GreaterThanOrEqualTo(.96f),
                    "The hub shell must hold contrast against every Map01A backdrop.");
                Assert.That(backdrop, Is.Not.Null);
                Assert.That(backdrop.style.backgroundColor.value.a, Is.GreaterThanOrEqualTo(.42f),
                    "The canonical hub dims the live world without replacing it with a second scene.");
                for (var itemIndex = 0; itemIndex < expected.Length; itemIndex++)
                {
                    var item = expected[itemIndex];
                    var tab = root.Q<Button>(item.Item1);
                    Assert.That(tab.style.minHeight.value.value, Is.EqualTo(52).Within(1));
                    Assert.That(tab.style.fontSize.value.value, Is.EqualTo(23).Within(1));
                    Assert.That(tab.style.borderTopWidth.value, Is.EqualTo(0),
                        "Tabs keep the ornamental border authored inside their shared texture without an outer CSS frame.");
                    Assert.That(tab.style.marginRight.value.value, Is.EqualTo(itemIndex == expected.Length - 1 ? 0 : 6).Within(1));
                }
                Assert.That(root.Q<Button>("Map01A Bag Main Tab").style.backgroundColor.value.b,
                    Is.GreaterThanOrEqualTo(.88f),
                    "Selected tabs must use the bright blue canonical state rather than the muted technical state.");
                Assert.That(root.Q("Map01A Inventory Body").style.flexDirection.value, Is.EqualTo(FlexDirection.Row),
                    "PC, tablet, and landscape mobile must preserve the canonical two-column composition.");
                Assert.That(root.Q("Map01A Inventory Header").style.marginBottom.value.value, Is.EqualTo(2).Within(1));
                Assert.That(root.Q("Map01A Inventory Modal Bottom Ornament").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The body must extend to the canonical lower frame instead of losing height to a decorative flow row.");
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
                var modalSubtitle = root.Q<Label>("Map01A Inventory Modal Subtitle");
                Assert.That(modalSubtitle.style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Approved character-hub screens use a compact title row; descriptive copy must not consume the shared body height.");
                StringAssert.DoesNotContain("phân loại dọc", modalSubtitle.text,
                    "Player-facing inventory copy must describe the feature rather than its implementation layout.");
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.alignSelf.value, Is.EqualTo(Align.Stretch),
                    "Rương đồ must use the full shared body height instead of ending as a short content-fit card.");
                foreach (var actionName in new[]
                {
                    "Map01A Inventory Sort Action", "Map01A Inventory Split Action", "Map01A Inventory Quick Sell Action"
                })
                    Assert.That(root.Q<Button>(actionName).enabledSelf, Is.False,
                        actionName + " must not expose an enabled dead click before its inventory model exists.");

                InvokeBoundButton(root.Q<Button>("Map01A Character Info Main Tab"));
                var characterPanel = root.Q("Map01A Inventory Character Panel");
                var equipmentDetail = root.Q("Map01A Inventory Detail Panel");
                Assert.That(characterPanel.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(equipmentDetail.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                var sharedColumns = CongDongLamArrivalHud.CalculateInventoryDesktopColumnWidths();
                Assert.That(sharedColumns.x, Is.EqualTo(600).Within(1));
                Assert.That(sharedColumns.y, Is.EqualTo(448).Within(1));
                Assert.That(root.Q("Map01A Character Hero Portrait").style.width.value.value, Is.EqualTo(400).Within(1));
                Assert.That(root.Q("Map01A Character Hero Portrait").style.height.value.value, Is.EqualTo(428).Within(1));
                Assert.That(root.Q("Map01A Character Hero Left Equipment Rail").style.width.value.value, Is.EqualTo(76).Within(1));
                var primaryDetailAction = root.Q<Button>("Map01A Inventory Detail Primary Action");
                var lockDetailAction = root.Q<Button>("Map01A Inventory Detail Lock Action");
                Assert.That(primaryDetailAction.style.backgroundColor.value.b, Is.GreaterThanOrEqualTo(.80f),
                    "The shared item inspector must use the approved bright-blue primary action state.");
                Assert.That(primaryDetailAction.style.color.value, Is.EqualTo(new Color(.98f, .99f, 1f, 1f)),
                    "Primary inspector actions must keep readable light text on the blue surface.");
                Assert.That(primaryDetailAction.style.borderTopWidth.value, Is.EqualTo(0),
                    "Textured action buttons must not add a second outer border.");
                Assert.That(lockDetailAction.style.borderTopWidth.value, Is.EqualTo(0),
                    "Gold action buttons must also keep only the decorative border inside their texture.");
                Assert.That(equipmentDetail.style.borderTopWidth.value, Is.EqualTo(0),
                    "The inspector owns one shared ornamental frame; an additional CSS outline must stay disabled.");
                for (var iconIndex = 0; iconIndex < scene.VoEquipmentSlotIds.Count; iconIndex++)
                {
                    var slotId = scene.VoEquipmentSlotIds[iconIndex];
                    var expectedIcon = scene.GetVoEquipmentThumbnailSprite(slotId);
                    Assert.That(expectedIcon, Is.Not.Null, "Missing dedicated UI icon for " + slotId);
                    Assert.That(expectedIcon, Is.EqualTo(scene.GetMap01ACharacterEquipmentIconSprite(slotId)),
                        "Baseline Võ must keep the approved readable UI icon atlas instead of a dark runtime clothing crop.");
                    Assert.That(root.Q("Map01A Character Hero Quick Icon " + iconIndex).style.backgroundImage.value.sprite,
                        Is.EqualTo(expectedIcon), "Character rail must use the dedicated readable UI atlas for " + slotId);
                }

                InvokeBoundButton(root.Q<Button>("Map01A Skills Main Tab"));
                Assert.That(root.Q("Map01A Skills Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Hub Preview Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Hub Preview Detail Hero").style.flexDirection.value, Is.EqualTo(FlexDirection.Row),
                    "Skills, Potential and Spirit Pet must share the canonical icon-plus-heading inspector hierarchy.");
                Assert.That(root.Q<Label>("Map01A Hub Preview Detail Name").style.fontSize.value.value, Is.EqualTo(28).Within(1));
                Assert.That(root.Q<ScrollView>("Map01A Hub Detail Scroll").style.flexGrow.value, Is.EqualTo(1),
                    "Context actions must remain docked to the lower edge of the shared detail column.");
                StringAssert.DoesNotContain("state", modalSubtitle.text);
                Assert.That(root.Q("Map01A Skill Path Stage 1"), Is.Not.Null,
                    "Approved skill screen must present a connected progression path instead of a generic item grid.");
                Assert.That(root.Q("Map01A Skill Path Stage 2"), Is.Not.Null);
                Assert.That(root.Q("Map01A Skill Path Stage 3"), Is.Not.Null);
                Assert.That(root.Q("Map01A Skill Stage 1 Connector 1"), Is.Not.Null);
                Assert.That(root.Q<Button>("Map01A Passive Skills Category").enabledSelf, Is.False,
                    "Unimplemented skill categories must be visibly gated instead of accepting dead clicks.");
                Assert.That(root.Q<Button>("Map01A Method Skills Category").enabledSelf, Is.False);
                var activeProfile = CharacterHubClassCatalog.Get(scene.ActiveEquipmentClassId);
                var firstSkill = activeProfile.Skills[0];
                var firstSkillNode = root.Q<Button>("Map01A Skill Node 0");
                Assert.That(firstSkillNode.ClassListContains("lgo-skill-node"), Is.True,
                    "All skill nodes must use the shared circular skill-node base.");
                Assert.That(firstSkillNode.style.width.value.value, Is.EqualTo(112).Within(1));
                Assert.That(root.Q<VisualElement>("Map01A Active Skills Category Icon"), Is.Not.Null);
                var firstExpectedIcon = firstSkill.IconCatalog == CharacterHubIconCatalog.Skill
                    ? scene.GetMap01ASkillIconSprite(firstSkill.IconId)
                    : scene.GetMap01AHudIconSprite(firstSkill.IconId);
                Assert.That(root.Q<VisualElement>("Map01A Skill Node 0 Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(firstExpectedIcon));
                Assert.That(root.Q<VisualElement>("Map01A Equipped Skill Icon 1").style.backgroundImage.value.sprite,
                    Is.EqualTo(firstExpectedIcon));
                Assert.That(root.Q<VisualElement>("Map01A Equipped Skill Icon 1").style.width.value.value,
                    Is.EqualTo(68), "The full-width equipped strip must keep readable icons without clipping its skill-points badge.");
                Assert.That(root.Q<Label>("Map01A Equipped Skill Heading").text, Is.EqualTo("Kỹ năng đã trang bị"));
                Assert.That(root.Query<VisualElement>(className: "lgo-equipped-skill-slot").ToList().Count, Is.EqualTo(4));
                for (var equippedIndex = 1; equippedIndex <= 4; equippedIndex++)
                    Assert.That(root.Q<Label>("Map01A Equipped Skill Number " + equippedIndex).text,
                        Is.EqualTo(equippedIndex.ToString()));
                Assert.That(root.Q<Label>("Map01A Skill Points Label").text, Is.EqualTo("Điểm kỹ năng"));
                Assert.That(root.Q<Button>("Map01A Skill Points Add").enabledSelf, Is.False,
                    "The approved points affordance stays visible but locked until progression has a real contract.");
                Assert.That(root.Q<Label>("Map01A Hub Detail Header").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The canonical inspector begins with the skill hero, without a redundant technical heading.");
                Assert.That(root.Q<Button>("Map01A Skill Equip Action"), Is.Not.Null);
                Assert.That(root.Q<Button>("Map01A Skill Equip Action").enabledSelf, Is.False);
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var hubDetailName = (Label)typeof(CongDongLamArrivalHud).GetField("_hubDetailName", flags).GetValue(hud);
                var hubDetailBody = (CharacterHubFactBlock)typeof(CongDongLamArrivalHud).GetField("_hubDetailBody", flags).GetValue(hud);
                var hubDetailStatus = (Label)typeof(CongDongLamArrivalHud).GetField("_hubDetailStatus", flags).GetValue(hud);
                var selectedSkill = activeProfile.Skills[2];
                var selectedSkillNode = root.Q<Button>("Map01A Skill Node 2");
                InvokeBoundButton(selectedSkillNode);
                Assert.That(hubDetailName.text, Is.EqualTo(selectedSkill.Name),
                    "Selecting a skill must update the shared detail-right panel instead of leaving the default skill visible.");
                Assert.That(selectedSkillNode.style.borderTopWidth.value, Is.EqualTo(0),
                    "The selected node must expose the same visible selection state used by its detail-right content.");
                Assert.That(firstSkillNode.style.borderTopWidth.value, Is.EqualTo(0));
                Assert.That(root.Q("Map01A Hub Preview Detail Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(selectedSkill.IconCatalog == CharacterHubIconCatalog.Skill
                        ? scene.GetMap01ASkillIconSprite(selectedSkill.IconId)
                        : scene.GetMap01AHudIconSprite(selectedSkill.IconId)));
                StringAssert.DoesNotContain("chờ dữ liệu", hubDetailBody.SourceText);
                StringAssert.DoesNotContain("chính thức", hubDetailStatus.text);

                InvokeBoundButton(root.Q<Button>("Map01A Potential Main Tab"));
                Assert.That(root.Q("Map01A Potential Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Potential Diagram Core"), Is.Not.Null,
                    "Approved potential screen must use one central meridian diagram rather than a repeated card grid.");
                Assert.That(root.Q("Map01A Potential Heading"), Is.Null,
                    "The canonical diagram does not repeat a technical heading or duplicate remaining-points badge above the orbit.");
                Assert.That(root.Q("Map01A Potential Topology Base"), Is.Not.Null);
                Assert.That(root.Q("Map01A Potential Topology Base").ClassListContains("lgo-potential-topology"), Is.True,
                    "The outer ring and connectors must come from one shared prebuilt topology behind the class data.");
                Assert.That(root.Q("Map01A Potential Diagram").style.height.value.value, Is.InRange(500, 525),
                    "The canonical meridian fills the main column; a short 400px canvas leaves a large dead zone below the graph.");
                Assert.That(root.Q("Map01A Potential Diagram").style.width.value.value,
                    Is.LessThanOrEqualTo(600),
                    "The shared potential template must fit the canonical main column so right-side value/add frames are never clipped by detail-right.");
                Assert.That(root.Q<Button>("Map01A Potential Node 2").ClassListContains("lgo-potential-node"), Is.True);
                for (var potentialIndex = 0; potentialIndex < 5; potentialIndex++)
                {
                    Assert.That(root.Q("Map01A Potential Node Add " + potentialIndex), Is.Null,
                        "The shared Potential artwork owns the add box and plus glyph; overlays bind data and interaction only.");
                }
                Assert.That(root.Q<VisualElement>("Map01A Potential Node 2 Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01APotentialIconSprite("vitality")));
                Assert.That(root.Q("Map01A Potential Topology Artwork"), Is.Not.Null,
                    "The canonical center and all fixed geometry must come from one shared actual-size template.");
                Assert.That(root.Q("Map01A Potential Core Figure"), Is.Null,
                    "The meditation figure is baked into the shared template and must not be rebuilt as a second runtime element.");
                Assert.That(root.Q("Map01A Potential Diagram Core").childCount, Is.EqualTo(0),
                    "The shared center marker must stay structural; the approved design has no duplicate technical label over the figure.");
                Assert.That(root.Q("Map01A Potential Core Icon"), Is.Null);
                var potentialFacts = root.Q("Map01A Potential Detail Facts");
                Assert.That(potentialFacts, Is.Not.Null,
                    "Potential uses one prebuilt detail template; classes only bind icon and values into it.");
                Assert.That(potentialFacts.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Label>("Map01A Potential Current Effect Heading").text,
                    Is.EqualTo("HIỆU QUẢ HIỆN TẠI"));
                Assert.That(root.Q<Label>("Map01A Potential Current Level").text,
                    Is.EqualTo("Cấp hiện tại:  250"),
                    "Potential level/value belongs in its canonical facts row, not in class-specific hero metadata.");
                Assert.That(root.Q<Label>("Map01A Potential Next Effect Heading").text,
                    Is.EqualTo("HIỆU QUẢ KHI CỘNG 1 ĐIỂM"));
                Assert.That(root.Q<CharacterHubFactBlock>("Map01A Potential Current Effect").SourceText,
                    Does.Contain("Sinh lực (HP)  +12.500"));
                Assert.That(root.Q<CharacterHubFactBlock>("Map01A Potential Next Effect").SourceText,
                    Does.Contain("Sinh lực (HP)  +50"));
                Assert.That(root.Q<Label>("Map01A Potential Current Level").style.fontSize.value.value,
                    Is.EqualTo(21).Within(1));
                Assert.That(root.Q<Label>("Map01A Potential Current Effect Heading").style.fontSize.value.value,
                    Is.EqualTo(17).Within(1));
                Assert.That(root.Q<CharacterHubFactBlock>("Map01A Potential Current Effect").style.fontSize.value.value,
                    Is.EqualTo(20).Within(1));
                Assert.That(root.Q<CharacterHubFactBlock>("Map01A Potential Next Effect").style.fontSize.value.value,
                    Is.EqualTo(20).Within(1));
                Assert.That(root.Q<Label>("Map01A Potential Cost").text,
                    Is.EqualTo("Tiêu hao  Điểm tiềm năng ×1"));
                Assert.That(root.Q("Map01A Potential Cost Row").style.minHeight.value.value,
                    Is.EqualTo(52).Within(1));
                Assert.That(root.Q("Map01A Potential Cost Row").style.flexShrink.value,
                    Is.EqualTo(0).Within(.01),
                    "The shared Potential cost row must keep its frame clear of the docked action row at every approved viewport.");
                Assert.That(root.Q<VisualElement>("Map01A Potential Cost Icon").style.backgroundImage.value.sprite,
                    Is.SameAs(scene.GetMap01APotentialIconSprite("core")),
                    "The shared cost row reuses the provenance-backed point icon instead of drawing a per-class placeholder.");
                Assert.That(root.Q<Label>("Map01A Potential Summary").parent.name,
                    Is.EqualTo("Map01A Hub Preview Detail Hero Copy"),
                    "The approved inspector keeps the selected Potential description beside its hero icon.");
                Assert.That(root.Q<Label>("Map01A Hub Preview Detail Meta").style.display.value,
                    Is.EqualTo(DisplayStyle.None));
                Assert.That(hubDetailStatus.style.display.value, Is.EqualTo(DisplayStyle.None),
                    "Potential lock state is already expressed by disabled actions; an extra review-status box is outside the canonical layout.");
                Assert.That(root.Q<Button>("Map01A Potential Add Point").enabledSelf, Is.False,
                    "Map01A must not create local fake potential progression before the real state contract exists.");
                Assert.That(root.Q<Button>("Map01A Potential Reset").enabledSelf, Is.False);
                Assert.That(root.Q<Button>("Map01A Potential Add Point").style.minHeight.value.value,
                    Is.EqualTo(52).Within(1));
                Assert.That(root.Q<Button>("Map01A Potential Add Point").style.fontSize.value.value,
                    Is.EqualTo(20).Within(1));
                InvokeBoundButton(root.Q<Button>("Map01A Potential Node 0"));
                Assert.That(hubDetailName.text, Is.EqualTo("Công"),
                    "Selecting a potential node must update detail-right without mutating progression state.");
                Assert.That(root.Q("Map01A Hub Preview Detail Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01APotentialIconSprite("attack")));
                Assert.That(root.Q<CharacterHubFactBlock>("Map01A Potential Current Effect").SourceText, Is.EqualTo("Công  +120"));
                Assert.That(root.Q<CharacterHubFactBlock>("Map01A Potential Next Effect").SourceText, Is.EqualTo("Công  +2"));
                StringAssert.DoesNotContain("state", modalSubtitle.text);
                StringAssert.DoesNotContain("local", hubDetailBody.SourceText);
                StringAssert.DoesNotContain("state", hubDetailBody.SourceText);

                InvokeBoundButton(root.Q<Button>("Map01A Spirit Pet Main Tab"));
                Assert.That(root.Q("Map01A Spirit Pet Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                var spiritPetTexture = root.Q("Map01A Spirit Pet Selected Roster Art").style.backgroundImage.value.texture;
                Assert.That(spiritPetTexture, Is.Not.Null,
                    "The selected spirit-pet roster entry must reuse the provenance-backed pet art, not a generic HUD crest.");
                Assert.That(spiritPetTexture.width, Is.EqualTo(spiritPetTexture.height),
                    "Roster/detail must use the authored portrait crop instead of shrinking the full wide hero into a small square.");
                Assert.That(root.Q("Map01A Spirit Pet Preview Art").style.backgroundImage.value.texture.width
                    / (float)root.Q("Map01A Spirit Pet Preview Art").style.backgroundImage.value.texture.height,
                    Is.GreaterThanOrEqualTo(1.45f), "The main hero must retain the approved wide source.");
                Assert.That(root.Q("Map01A Spirit Pet Locked Roster 1"), Is.Not.Null);
                Assert.That(root.Q<UnityEngine.UIElements.ProgressBar>("Map01A Spirit Pet Intimacy").value, Is.EqualTo(320));
                Assert.That(root.Q<UnityEngine.UIElements.ProgressBar>("Map01A Spirit Pet Growth").value, Is.EqualTo(180));
                var spiritBadges = root.Q("Map01A Spirit Pet Detail Badges");
                Assert.That(spiritBadges.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Label>("Map01A Spirit Pet Rarity Badge").text, Is.EqualTo("Tinh phẩm"));
                Assert.That(root.Q<Label>("Map01A Spirit Pet Role Badge").text, Is.EqualTo("Hỗ trợ"));
                Assert.That(root.Q<Label>("Map01A Spirit Pet State Badge").text, Is.EqualTo("Đang xuất chiến"));
                Assert.That(hubDetailStatus.text, Is.EqualTo("Tính năng bồi dưỡng đang khóa."),
                    "Deployment state belongs in the hero badge row instead of being duplicated in the status card.");
                Assert.That(root.Q<Button>("Map01A Spirit Pet Deploy Action").enabledSelf, Is.False);
                Assert.That(root.Q<Button>("Map01A Spirit Pet Develop Action").enabledSelf, Is.False,
                    "Linh thú growth must remain visibly gated until its real progression state exists.");
                StringAssert.DoesNotContain("Màn này", hubDetailBody.SourceText);
                StringAssert.DoesNotContain("state", hubDetailBody.SourceText);
                StringAssert.DoesNotContain("chính thức", hubDetailStatus.text);
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void CharacterHubClassRefreshRebindsOneStableSharedTopology()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character hub shared topology test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var refresh = typeof(CongDongLamArrivalHud).GetMethod("RefreshCharacterHubClassProfile", flags);

                var skillsPanel = root.Q("Map01A Skills Panel");
                var potentialPanel = root.Q("Map01A Potential Panel");
                var spiritPanel = root.Q("Map01A Spirit Pet Panel");
                var skillNode0 = root.Q<Button>("Map01A Skill Node 0");
                var potentialNode0 = root.Q<Button>("Map01A Potential Node 0");
                var potentialTopology = root.Q("Map01A Potential Topology Base");
                var potentialFacts = root.Q("Map01A Potential Detail Facts");
                var spiritSkillRow0 = root.Q("Map01A Spirit Pet Skill Row 0");
                Assert.That(potentialTopology, Is.Not.Null,
                    "The circles, outer ring and connectors must be one prebuilt shared topology behind class-bound icons.");
                Assert.That(potentialTopology.ClassListContains("lgo-potential-topology"), Is.True);
                var topologyArtwork = root.Q("Map01A Potential Topology Artwork");
                Assert.That(topologyArtwork, Is.Not.Null,
                    "Potential circles, connectors, node frames and meridian figure must come from one actual-size reusable artwork template.");
                Assert.That(topologyArtwork.style.backgroundImage.value.texture, Is.Not.Null,
                    "The shared Potential topology template must be loaded once; class data may only overlay icons, labels and state.");
                Assert.That(topologyArtwork.style.backgroundImage.value.texture.width, Is.EqualTo(600));
                Assert.That(topologyArtwork.style.backgroundImage.value.texture.height, Is.EqualTo(520));
                var topologyFrameCount = potentialTopology.GetType().GetProperty(
                    "PrebuiltNodeFrameCount", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.That(topologyFrameCount, Is.Not.Null,
                    "Potential topology must own the reusable node rings instead of asking every class-bound control to draw one.");
                Assert.That(topologyFrameCount.GetValue(null), Is.EqualTo(5));
                var topologyValueFrameCount = potentialTopology.GetType().GetProperty(
                    "PrebuiltValueFrameCount", BindingFlags.Static | BindingFlags.NonPublic);
                var topologyAddFrameCount = potentialTopology.GetType().GetProperty(
                    "PrebuiltAddFrameCount", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.That(topologyValueFrameCount?.GetValue(null), Is.EqualTo(5),
                    "Potential topology must prebuild all value boxes instead of styling one box per class-bound node.");
                Assert.That(topologyAddFrameCount?.GetValue(null), Is.EqualTo(5),
                    "Potential topology must prebuild all add boxes; the plus labels above it only bind interaction state.");
                var topologyAddGlyphCount = potentialTopology.GetType().GetProperty(
                    "PrebuiltAddGlyphCount", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.That(topologyAddGlyphCount?.GetValue(null), Is.EqualTo(5),
                    "Potential topology must prebuild one crisp plus glyph in every add box.");
                var topologyMeridianAnchorCount = potentialTopology.GetType().GetProperty(
                    "PrebuiltMeridianAnchorCount", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.That(topologyMeridianAnchorCount?.GetValue(null), Is.EqualTo(5),
                    "Potential topology must own the complete meridian spine; class data may only bind icons and values.");
                Assert.That(potentialNode0.ClassListContains("lgo-potential-node-overlay"), Is.True,
                    "Potential buttons are interaction/data overlays on the shared vector base.");
                Assert.That(potentialNode0.style.borderLeftWidth.value, Is.EqualTo(0));
                Assert.That(potentialNode0.style.borderTopWidth.value, Is.EqualTo(0));
                Assert.That(potentialNode0.style.borderTopLeftRadius.value.value, Is.EqualTo(0),
                    "Potential overlay must remain a geometry-free hit target; the reusable topology owns every node circle.");
                Assert.That(root.Q("Map01A Potential Node Add 0"), Is.Null,
                    "The shared Potential artwork already owns every add box and plus glyph; data overlays must not create duplicate geometry.");
                Assert.That(skillNode0.ClassListContains("lgo-potential-node"), Is.False,
                    "Skill and Potential must remain separate reusable components.");
                Assert.That(potentialNode0.ClassListContains("lgo-skill-node"), Is.False,
                    "Potential nodes must never reuse the Skill topology or its node chrome.");
                Assert.That(spiritSkillRow0, Is.Not.Null,
                    "Spirit pet skill rows must be prebuilt once and rebound from profile data.");
                Assert.That(root.Q("Map01A Spirit Pet Skill Row 1"), Is.Not.Null);
                Assert.That(root.Q<VisualElement>("Map01A Spirit Pet Skill Row 0 Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01ASkillIconSprite("ho_the")));

                var sourcePose = new GameObject("shared topology class actor").AddComponent<TwoDSourcePoseReview>();
                sourcePose.transform.SetParent(scene.transform, false);
                var classId = typeof(TwoDSourcePoseReview)
                    .GetField("<ClassId>k__BackingField", flags);
                typeof(CongDongLamMap01AArtPreview)
                    .GetField("_sourcePoseReview", flags)
                    .SetValue(scene, sourcePose);
                foreach (var profile in CharacterHubClassCatalog.Profiles)
                {
                    classId.SetValue(sourcePose, profile.Id);
                    refresh.Invoke(hud, null);

                    Assert.That(root.Q("Map01A Skills Panel"), Is.SameAs(skillsPanel), profile.Id);
                    Assert.That(root.Q("Map01A Potential Panel"), Is.SameAs(potentialPanel), profile.Id);
                    Assert.That(root.Q("Map01A Spirit Pet Panel"), Is.SameAs(spiritPanel), profile.Id);
                    Assert.That(root.Q("Map01A Spirit Pet Skill Row 0"), Is.SameAs(spiritSkillRow0), profile.Id);
                    Assert.That(root.Q<Button>("Map01A Skill Node 0"), Is.SameAs(skillNode0), profile.Id);
                    Assert.That(root.Q<Button>("Map01A Potential Node 0"), Is.SameAs(potentialNode0), profile.Id);
                    Assert.That(root.Q("Map01A Potential Topology Base"), Is.SameAs(potentialTopology), profile.Id);
                    Assert.That(root.Q("Map01A Potential Detail Facts"), Is.SameAs(potentialFacts), profile.Id,
                        "Class refresh must rebind the shared Potential detail template rather than rebuilding one per class.");
                    Assert.That(root.Query<Button>(className: "lgo-skill-node").ToList().Count, Is.EqualTo(9), profile.Id);
                    Assert.That(root.Query<Button>(className: "lgo-potential-node").ToList().Count, Is.EqualTo(5), profile.Id);
                    Assert.That(root.Q<Label>("Map01A Skill Node 0 Title").text,
                        Is.EqualTo(profile.Skills[0].Name), profile.Id);
                    Assert.That(root.Q<Label>("Map01A Potential Node 0 Title").text,
                        Is.EqualTo(profile.Potentials[0].Name), profile.Id);
                    var selectedPotentialIndex = profile.Potentials
                        .Select((potential, index) => new { potential.Name, Index = index })
                        .Single(item => item.Name == profile.DefaultPotentialName).Index;
                    for (var potentialIndex = 0; potentialIndex < profile.Potentials.Count; potentialIndex++)
                        Assert.That(root.Q<Button>("Map01A Potential Node " + potentialIndex)
                                .ClassListContains("lgo-character-hub-selected"),
                            Is.EqualTo(potentialIndex == selectedPotentialIndex),
                            profile.Id + " default Potential selection must be rebound without a class-specific UI branch.");
                    Assert.That(root.Q<Label>("Map01A Spirit Pet Hero Level").text,
                        Does.Contain(profile.Label), profile.Id);
                    Assert.That(root.Q<Label>("Map01A Spirit Pet Skill Row 0 Name").text,
                        Is.EqualTo("Thanh Vân Hộ Thể"), profile.Id);
                }
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void PotentialEvidenceBindingCoversFiveProfilesWithoutChangingRendererAuthority()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("potential five-profile evidence binding test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var bind = typeof(CongDongLamArrivalHud).GetMethod("BindCharacterHubEvidenceClass", flags);
                var clear = typeof(CongDongLamArrivalHud).GetMethod("ClearCharacterHubEvidenceClass", flags);
                Assert.That(bind, Is.Not.Null);
                Assert.That(clear, Is.Not.Null);
                var topology = root.Q("Map01A Potential Topology Base");
                var rendererClass = scene.ActiveEquipmentClassId;
                foreach (var profile in CharacterHubClassCatalog.Profiles)
                {
                    bind.Invoke(hud, new object[] { profile.Id });
                    Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(rendererClass), profile.Id);
                    Assert.That(root.Q("Map01A Potential Topology Base"), Is.SameAs(topology), profile.Id);
                    Assert.That(root.Q<Label>("Map01A Potential Recommendation").text,
                        Is.EqualTo(profile.Recommendation), profile.Id);
                    Assert.That(root.Q<Label>("Map01A Hub Preview Detail Name").text,
                        Is.EqualTo(profile.DefaultPotentialName), profile.Id);
                }
                clear.Invoke(hud, null);
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(rendererClass));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void CharacterHubFrameHierarchyRetainsOrnamentsWithoutLeakingIntoPotential()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character hub frame hierarchy test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Character Info Main Tab"));
                foreach (var name in new[] { "Map01A Inventory Detail Panel", "Map01A Hub Preview Detail Panel" })
                {
                    var panel = root.Q(name);
                    var frame = panel.Q(className: "lgo-hub-inspector-frame");
                    Assert.That(frame, Is.Not.Null, "The inspector must use the shared shell ornament, not a plain CSS outline.");
                    Assert.That(frame.ClassListContains("lgo-ornamented-shell"), Is.True);
                    Assert.That(frame.pickingMode, Is.EqualTo(PickingMode.Ignore));
                    Assert.That(frame.style.position.value, Is.EqualTo(Position.Absolute));
                    Assert.That(panel.style.borderTopWidth.value, Is.EqualTo(0));
                }
                InvokeBoundButton(root.Q<Button>("Map01A Skills Main Tab"));
                var hero = root.Q("Map01A Hub Preview Detail Icon");
                var heroFrame = hero.Q(className: "lgo-hub-hero-frame");
                Assert.That(heroFrame, Is.Not.Null);
                var count = root.Query<VisualElement>(className: "lgo-hub-ornament-frame").ToList().Count;
                for (var repeat = 0; repeat < 3; repeat++)
                {
                    InvokeBoundButton(root.Q<Button>("Map01A Potential Main Tab"));
                    Assert.That(heroFrame.style.display.value, Is.EqualTo(DisplayStyle.None),
                        "A Potential icon owns its independent circular frame; never stack a square hero frame over it.");
                    Assert.That(hero.Q("Map01A Potential Detail Shared Frame").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                    InvokeBoundButton(root.Q<Button>("Map01A Skills Main Tab"));
                    Assert.That(hero.Q(className: "lgo-hub-hero-frame"), Is.SameAs(heroFrame));
                    Assert.That(heroFrame.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                    Assert.That(root.Query<VisualElement>(className: "lgo-hub-ornament-frame").ToList().Count, Is.EqualTo(count));
                }
            }
            finally
            {
                foreach (var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(go)) Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void CharacterHubFrameSelectionTracksEquipmentAndSkillWithoutRebuilding()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character hub selected ornament test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Character Info Main Tab"));
                var first = root.Q<Button>("Map01A Character Hero Quick Icon 0");
                var second = root.Q<Button>("Map01A Character Hero Quick Icon 1");
                InvokeBoundButton(second);
                Assert.That(second.ClassListContains("lgo-character-hub-selected"), Is.True,
                    "The clicked equipment rail slot must expose the same selection as the inspector.");
                Assert.That(first.ClassListContains("lgo-character-hub-selected"), Is.False);
                var frame = second.Q(className: "lgo-hub-selection-frame");
                Assert.That(frame, Is.Not.Null);
                Assert.That(frame.style.opacity.value, Is.EqualTo(1));
                InvokeBoundButton(first);
                InvokeBoundButton(second);
                Assert.That(second.Q(className: "lgo-hub-selection-frame"), Is.SameAs(frame));
                Assert.That(second.Query<VisualElement>(className: "lgo-hub-selection-frame").ToList().Count, Is.EqualTo(1));
                InvokeBoundButton(root.Q<Button>("Map01A Skills Main Tab"));
                var skill = root.Q<Button>("Map01A Skill Node 2");
                InvokeBoundButton(skill);
                var skillFrame = skill.Q(className: "lgo-hub-selection-frame");
                Assert.That(skillFrame, Is.Not.Null, "Canonical selected skills use a square gold ornament outside the separate circular icon.");
                Assert.That(skillFrame.ClassListContains("lgo-ornamented-shell"), Is.True);
                Assert.That(skillFrame.style.opacity.value, Is.EqualTo(1));
                Assert.That(skill.IndexOf(skillFrame), Is.LessThan(skill.IndexOf(skill.Q<Label>(skill.name + " Level"))),
                    "Decorative borders must paint behind the level badge, never through its text.");
                Assert.That(second.IndexOf(frame), Is.LessThan(second.IndexOf(second.Q<Label>())),
                    "Equipment level badges must remain above the same shared frame layer.");
                Assert.That(skill.style.borderTopWidth.value, Is.EqualTo(0), "Do not retain the old blue CSS circle below the gold selection frame.");
                InvokeBoundButton(root.Q<Button>("Map01A Skill Node 0"));
                Assert.That(skillFrame.style.opacity.value, Is.EqualTo(0));
                Assert.That(skill.Q(className: "lgo-hub-selection-frame"), Is.SameAs(skillFrame));
            }
            finally
            {
                foreach (var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(go)) Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void CharacterHubPresentationIconsDoNotShrinkInCompositeControls()
        {
            var icon = new VisualElement();
            icon.style.flexGrow = 1;
            icon.style.flexShrink = 1;
            var apply = typeof(CongDongLamArrivalHud).GetMethod("ApplyLgoItemIcon",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(apply, Is.Not.Null);
            apply.Invoke(null, new object[] { icon });
            Assert.That(icon.style.flexShrink.value, Is.EqualTo(0),
                "HUD-catalog skill and roster artwork must not collapse to a horizontal strip.");
            Assert.That(icon.style.flexGrow.value, Is.EqualTo(0));
        }

        [Test]
        public void CharacterHubPresentationCharacterInspectorAndVitalsAreReadable()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character hub shared presentation test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Character Info Main Tab"));
                {
                    var inspector = root.Q("Map01A Inventory Detail Panel");
                    Assert.That(inspector.style.paddingLeft.value.value, Is.GreaterThanOrEqualTo(16));
                    Assert.That(inspector.style.paddingRight.value.value, Is.GreaterThanOrEqualTo(16));
                    var facts = root.Q("Map01A Inventory Detail Stats Card");
                    Assert.That(facts.style.borderLeftWidth.value, Is.EqualTo(0),
                        "Facts belong inside the existing inspector, not another framed box.");
                    foreach (var actionName in new[] { "Primary", "Lock" })
                        Assert.That(root.Q<Button>("Map01A Inventory Detail " + actionName + " Action").style.minHeight.value.value,
                            Is.GreaterThanOrEqualTo(48));
                    Assert.That(root.Q<Label>("Map01A Inventory Detail Stats Header").style.fontSize.value.value,
                        Is.GreaterThanOrEqualTo(18));
                    Assert.That(root.Q("Map01A Character Hero Vitals Bars").style.flexDirection.value,
                        Is.EqualTo(FlexDirection.Column));
                    Assert.That(root.Q("Map01A Character Hero Health").style.height.value.value,
                        Is.GreaterThanOrEqualTo(22));
                    Assert.That(root.Q<Label>("Map01A Character Hero Name").style.fontSize.value.value,
                        Is.GreaterThanOrEqualTo(24));
                    var portrait = root.Q("Map01A Character Hero Portrait");
                    Assert.That(portrait.style.width.value.value, Is.EqualTo(400));
                    Assert.That(portrait.style.height.value.value, Is.EqualTo(428),
                        "UI presentation must not resize or replace the frozen actor stage.");
                }
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void CharacterHubPresentationBagUsesTwentyFiveSquareSlotsAndOneState()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character hub bag presentation test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Bag Main Tab"));
                var cells = root.Q("Map01A Inventory Items Grid")
                    .Query<VisualElement>(className: "lgo-inventory-bag-grid-cell").ToList();
                Assert.That(cells.Count, Is.EqualTo(25),
                    "Approved Bag design has five columns and five rows, including empty slots.");
                var fit = typeof(CongDongLamArrivalHud).GetMethod("FitCharacterHubBagGrid",
                    BindingFlags.Static | BindingFlags.NonPublic);
                Assert.That(fit, Is.Not.Null, "Fit five columns to the actual scroll viewport, not the nominal panel.");
                Assert.That(fit.GetParameters().Length, Is.EqualTo(3), "The fifth row must also fit the viewport height.");
                foreach (var width in new[] { 438f, 446f, 456f })
                {
                    fit.Invoke(null, new object[] { root.Q("Map01A Inventory Items Grid"), width, 420f });
                    var rowWidth = 0f;
                    foreach (var cell in cells.Take(5))
                    {
                        rowWidth += cell.style.width.value.value + cell.style.marginRight.value.value;
                        Assert.That(cell.style.width.value.value, Is.EqualTo(cell.style.height.value.value));
                        Assert.That(cell.style.marginTop.value.value, Is.EqualTo(0));
                    }
                    Assert.That(rowWidth, Is.LessThanOrEqualTo(width), "Scrollbar reservation must not reduce the grid to four columns.");
                    Assert.That(5 * (cells[0].style.height.value.value + cells[0].style.marginBottom.value.value),
                        Is.LessThanOrEqualTo(420), "All five rows must remain visible above the fixed toolbar.");
                }
                foreach (var category in root.Q("Map01A Inventory Category Rail").Children())
                {
                    Assert.That(category.style.height.value.value, Is.EqualTo(100));
                    Assert.That(category.style.maxHeight.value.value, Is.EqualTo(100));
                }
                var icon = root.Q("Map01A Equipment Item Icon " + scene.EquipmentSlotIds[0]);
                Assert.That(icon.style.borderLeftWidth.value, Is.EqualTo(0),
                    "The grid cell owns its frame; artwork must not add a second nested rectangle.");
                Assert.That(root.Q("Map01A Inventory Category Icon all").style.height.value.value,
                    Is.GreaterThanOrEqualTo(60));
                Assert.That(root.Q<Label>("Map01A Inventory Detail Slot Type").style.display.value,
                    Is.EqualTo(DisplayStyle.None), "Do not repeat the generic equipment title below itself.");
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void SkillProgressionUsesOneCanonicalThreeByThreeTopology()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("skill shared topology test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Skills Main Tab"));

                var expected = new[] { 3, 3, 3 };
                for (var stageIndex = 0; stageIndex < expected.Length; stageIndex++)
                {
                    var stage = root.Q("Map01A Skill Path Stage " + (stageIndex + 1));
                    Assert.That(stage, Is.Not.Null);
                    Assert.That(stage.Query<Button>(className: "lgo-skill-node").ToList().Count,
                        Is.EqualTo(expected[stageIndex]),
                        "The shared Skill topology must follow the approved three-by-three grid; class data only fills its nine nodes.");
                }
                Assert.That(root.Q<ScrollView>("Map01A Hub Detail Scroll"), Is.Not.Null,
                    "Long detail copy must scroll without covering the fixed action row.");
                for (var index = 0; index < 9; index++)
                {
                    var node = root.Q<Button>("Map01A Skill Node " + index);
                    Assert.That(node.style.height.value.value, Is.EqualTo(112));
                    var icon = node.Q(node.name + " Icon");
                    Assert.That(icon.style.height.value.value, Is.EqualTo(96));
                    Assert.That(icon.style.borderLeftWidth.value, Is.EqualTo(0));
                    Assert.That(node.Q<Label>(node.name + " Title").style.display.value, Is.EqualTo(DisplayStyle.None));
                    var level = node.Q<Label>(node.name + " Level");
                    Assert.That(level.style.position.value, Is.EqualTo(Position.Absolute));
                    Assert.That(level.style.fontSize.value.value, Is.GreaterThanOrEqualTo(16));
                }
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void CharacterHubFactPresentationSeparatesValuesAndPreservesNodes()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("hub fact row integration");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Potential Main Tab"));
                var current = root.Q("Map01A Potential Current Effect");
                var rows = current.Query<VisualElement>(className: "lgo-character-hub-fact-row").ToList();
                Assert.That(rows.Count, Is.EqualTo(2), "Potential effects need prebuilt caption/value rows, not one multiline label.");
                Assert.That(rows[0].Q<Label>(rows[0].name + " Caption").text, Is.EqualTo("Sinh lực (HP)"));
                Assert.That(rows[0].Q<Label>(rows[0].name + " Value").text, Is.EqualTo("+12.500"));
                Assert.That(rows[1].Q<Label>(rows[1].name + " Value").text, Is.EqualTo("+250"));
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var activeClass = scene.ActiveEquipmentClassId;
                foreach (var profile in CharacterHubClassCatalog.Profiles)
                {
                    typeof(CongDongLamArrivalHud).GetField("_characterHubEvidenceClassId", flags).SetValue(hud, profile.Id);
                    typeof(CongDongLamArrivalHud).GetMethod("BindCharacterHubProfile", flags).Invoke(hud, null);
                    InvokeBoundButton(root.Q<Button>("Map01A Potential Node 0"));
                    Assert.That(current.Query<VisualElement>(className: "lgo-character-hub-fact-row").ToList(), Is.EqualTo(rows));
                    Assert.That(rows[0].Q<Label>(rows[0].name + " Value").text, Is.EqualTo("+120"));
                    Assert.That(rows[1].style.display.value, Is.EqualTo(DisplayStyle.None), "One-effect selection must hide the previous second effect.");
                    InvokeBoundButton(root.Q<Button>("Map01A Skills Main Tab"));
                    var skillFacts = root.Q("Map01A Hub Skill Facts");
                    Assert.That(skillFacts, Is.Not.Null);
                    foreach (var skill in profile.Skills)
                    {
                        typeof(CongDongLamArrivalHud).GetMethod("ShowSkillDetail", flags).Invoke(hud, new object[] { skill });
                        Assert.That(skillFacts.GetType().GetProperty("SourceText").GetValue(skillFacts), Is.EqualTo(skill.Description));
                    }
                    Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(activeClass), "Fact review must not change the actor class.");
                }
            }
            finally
            {
                foreach (var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(go)) Object.DestroyImmediate(go);
            }
        }

        [UnityEngine.TestTools.UnityTest]
        public System.Collections.IEnumerator CharacterHubFactPresentationWrappedValuesFitMeasuredRows()
        {
            var window = ScriptableObject.CreateInstance<UnityEditor.EditorWindow>();
            try
            {
                window.Show();
                var block = new CharacterHubFactBlock("Measured fact block", 4, 18, false);
                window.rootVisualElement.Add(block);
                typeof(CongDongLamArrivalHud).GetMethod("ApplyLgoSpiritPetStatRow", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new object[] { block.Rows[3] });
                block.Bind("Sát thương  320% Công\nPhạm vi  Hình quạt trước mặt\nHồi chiêu  12 giây\nGiảm Sát Thương  +12%");
                foreach (var width in new[] { 410, 300, 220 })
                {
                    block.style.width = width;
                    yield return null;
                    yield return null;
                    foreach (var row in block.Rows)
                    foreach (var copy in new[] { row.Caption, row.Value })
                    {
                        Assert.That(copy.contentRect.width, Is.GreaterThan(0));
                        var measured = copy.MeasureTextSize(copy.text, copy.contentRect.width,
                            VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
                        Assert.That(copy.contentRect.height + 1, Is.GreaterThanOrEqualTo(measured.y),
                            copy.name + " at width " + width + " must measure its actual wrapped text.");
                        Assert.That(copy.worldBound.yMin, Is.GreaterThanOrEqualTo(row.worldBound.yMin - 1));
                        Assert.That(copy.worldBound.yMax, Is.LessThanOrEqualTo(row.worldBound.yMax + 1));
                    }
                }
            }
            finally { window.Close(); }
        }

        [Test]
        public void CharacterHubFactPresentationPreservesUnknownAndOverflowText()
        {
            var type = typeof(CongDongLamArrivalHud).Assembly.GetType("LinhGioi.UI.CharacterHubFactBlock");
            Assert.That(type, Is.Not.Null, "A reusable source-preserving fact component is required.");
            var block = (VisualElement)System.Activator.CreateInstance(type, new object[] { "Fact test", 2, 18, false });
            var bind = type.GetMethod("Bind");
            var children = block.Children().ToArray();
            foreach (var text in new[] { "HP  +12.500\nPhòng thủ  +250", "Dòng văn bản không có giá trị", "HP  +1\n\nGhi chú\nDòng thứ tư", "", "\r\n" })
            {
                bind.Invoke(block, new object[] { text });
                Assert.That(type.GetProperty("SourceText").GetValue(block), Is.EqualTo(text));
                Assert.That(block.Children().ToArray(), Is.EqualTo(children), "Binding must not rebuild the row tree.");
                if (text.Contains("Dòng thứ tư"))
                {
                    var fallback = block.Q<Label>("Fact test Overflow Text");
                    Assert.That(fallback.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                    Assert.That(fallback.text, Is.EqualTo(text), "Over-budget content must remain readable, never truncated.");
                }
            }
            bind.Invoke(block, new object[] { "HP  +1" });
            var value = block.Q<Label>("Fact test Row 0 Value");
            Assert.That(value.text, Is.EqualTo("+1"));
            Assert.That(value.style.unityTextAlign.value, Is.EqualTo(TextAnchor.MiddleRight));
            Assert.That(value.style.whiteSpace.value, Is.EqualTo(WhiteSpace.Normal));
        }

        [Test]
        public void CharacterHubFactPresentationLockedActionsAndConnectorsAreExplicit()
        {
            var action = new Button();
            typeof(CongDongLamArrivalHud).GetMethod("ApplyLgoCharacterHubLockedAction", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { action, true });
            Assert.That(action.enabledSelf, Is.False);
            Assert.That(action.tooltip, Does.Contain("chưa khả dụng"));
            Assert.That(action.style.color.value.r, Is.GreaterThanOrEqualTo(.80f), "Disabled is not permission to make the label illegible.");
            var create = typeof(CongDongLamArrivalHud).GetMethod("CreateHubPathConnector", BindingFlags.Static | BindingFlags.NonPublic);
            var connector = (VisualElement)create.Invoke(null, new object[] { "test connector", false });
            Assert.That(connector.ClassListContains("lgo-skill-directional-connector"), Is.True);
            Assert.That(connector.style.height.value.value, Is.GreaterThanOrEqualTo(10));
            Assert.That(connector.pickingMode, Is.EqualTo(PickingMode.Ignore));
        }

        [Test]
        public void PotentialInspectorUsesExplicitReadingRhythmInsteadOfInheritedLabelSpacing()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("potential reading rhythm test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Potential Main Tab"));
                foreach (var label in root.Q("Map01A Potential Detail Facts").Query<Label>().ToList())
                {
                    Assert.That(label.ClassListContains("lgo-hub-copy"), Is.True, label.name);
                    Assert.That(label.style.paddingTop.value.value, Is.EqualTo(0), label.name);
                    Assert.That(label.style.paddingBottom.value.value, Is.EqualTo(0), label.name);
                    Assert.That(label.style.marginBottom.value.value, Is.EqualTo(0), label.name);
                }
                var cost = root.Q("Map01A Potential Cost Row");
                Assert.That(cost.ClassListContains("lgo-status-card"), Is.False,
                    "Cost is a section of the inspector, not a nested framed status card.");
                Assert.That(cost.style.minHeight.value.value, Is.EqualTo(52));
                Assert.That(root.Q("Map01A Potential Level Divider").style.marginBottom.value.value, Is.EqualTo(6));
            }
            finally
            {
                foreach (var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(go)) Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void PotentialFramesShareOneSpriteAndNeverBecomeClassData()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("shared potential frame test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Potential Main Tab"));
                var master = scene.GetMap01APotentialIconSprite("frame");
                Assert.That(master, Is.Not.Null, "The atlas must expose one independent frame asset");
                Assert.That(master.texture.width, Is.EqualTo(512));
                Assert.That(master.texture.height, Is.EqualTo(256));
                var topology = root.Q("Map01A Potential Topology Base");
                var frames = topology.Query<VisualElement>(className: "lgo-circular-icon-frame").ToList();
                Assert.That(frames.Count, Is.EqualTo(5));
                InvokeBoundButton(root.Q<Button>("Map01A Potential Node 0"));
                Assert.That(frames[0].style.opacity.value, Is.EqualTo(1f));
                for (var index = 1; index < 5; index++)
                    Assert.That(frames[index].style.opacity.value, Is.EqualTo(.62f).Within(.001f),
                        "The shared frame must display selection independently of the inner symbol");
                var bind = typeof(CongDongLamArrivalHud).GetMethod("BindCharacterHubEvidenceClass", BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var profile in CharacterHubClassCatalog.Profiles)
                {
                    bind.Invoke(hud, new object[] { profile.Id });
                    Assert.That(root.Q("Map01A Potential Topology Base"), Is.SameAs(topology));
                    for (var index = 0; index < 5; index++)
                    {
                        Assert.That(topology.Q("Map01A Potential Shared Frame " + index), Is.SameAs(frames[index]));
                        Assert.That(frames[index].style.backgroundImage.value.sprite, Is.SameAs(master));
                        Assert.That(root.Q("Map01A Potential Node " + index).Query<VisualElement>(className: "lgo-circular-icon-frame").ToList(), Is.Empty);
                    }
                }
                var detailFrame = root.Q("Map01A Potential Detail Shared Frame");
                Assert.That(detailFrame.style.backgroundImage.value.sprite, Is.SameAs(master));
                Assert.That(root.Q("Map01A Potential Cost Icon Shared Frame").style.backgroundImage.value.sprite, Is.SameAs(master));
                InvokeBoundButton(root.Q<Button>("Map01A Skills Main Tab"));
                Assert.That(detailFrame.style.display.value, Is.EqualTo(DisplayStyle.None));
            }
            finally
            {
                foreach (var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(go)) Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void PotentialPresentationSeparatesMedallionCaptionAndValueInOneTemplate()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("potential overlay presentation test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Potential Main Tab"));
                for (var index = 0; index < 5; index++)
                {
                    var node = root.Q("Map01A Potential Node " + index);
                    var icon = node.Q(node.name + " Icon");
                    var title = node.Q<Label>(node.name + " Title");
                    var value = node.Q<Label>(node.name + " Value");
                    Assert.That(icon.style.height.value.value, Is.EqualTo(108));
                    Assert.That(icon.style.top.value.value + icon.style.height.value.value, Is.LessThanOrEqualTo(title.style.top.value.value));
                    Assert.That(title.style.position.value, Is.EqualTo(Position.Absolute));
                    Assert.That(title.style.top.value.value, Is.EqualTo(116));
                    Assert.That(value.style.top.value.value, Is.EqualTo(142));
                    Assert.That(value.style.fontSize.value.value, Is.EqualTo(20));
                    Assert.That(node.style.height.value.value, Is.EqualTo(172));
                    Assert.That(title.style.top.value.value + title.style.height.value.value,
                        Is.LessThanOrEqualTo(value.style.top.value.value));
                    Assert.That(node.style.backgroundColor.value.a, Is.EqualTo(0));
                    Assert.That(node.style.borderLeftWidth.value, Is.EqualTo(0));
                }
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void SpiritPetUsesOneFixedHeroRosterAndStructuredDetailTemplate()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("spirit pet shared template test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                InvokeBoundButton(root.Q<Button>("Map01A Spirit Pet Main Tab"));

                var preview = root.Q("Map01A Spirit Pet Preview Art");
                Assert.That(preview.style.height.value.value, Is.EqualTo(346).Within(1));
                Assert.That(preview.style.flexShrink.value, Is.EqualTo(0).Within(.01),
                    "The canonical hero must keep its authored height instead of shrinking differently per viewport.");
                Assert.That(preview.style.borderLeftWidth.value, Is.EqualTo(0),
                    "The pet belongs to the main panel, without a second black framed box.");
                Assert.That(root.Q<Label>("Map01A Spirit Pet Identity").style.fontSize.value.value,
                    Is.GreaterThanOrEqualTo(24));
                foreach (var name in new[] { "Map01A Spirit Pet Intimacy", "Map01A Spirit Pet Growth" })
                    Assert.That(root.Q(name).style.height.value.value, Is.EqualTo(22));
                foreach (var card in root.Query<Button>(className: "lgo-spirit-pet-roster-card").ToList())
                {
                    Assert.That(card.style.height.value.value, Is.EqualTo(100));
                    var art = card.Children().First();
                    Assert.That(art.style.position.value, Is.EqualTo(Position.Absolute));
                    Assert.That(art.style.top.value.value, Is.EqualTo(2));
                    Assert.That(art.style.height.value.value, Is.EqualTo(76));
                    var caption = card.Q<Label>(card.name + " Level");
                    Assert.That(caption.style.bottom.value.value, Is.EqualTo(0));
                    Assert.That(caption.style.height.value.value, Is.EqualTo(22));
                }
                foreach (var label in root.Q("Map01A Spirit Pet Detail Facts").Query<Label>().ToList())
                    Assert.That(label.ClassListContains("lgo-hub-copy"), Is.True, label.name);
                Assert.That(root.Q<Label>("Map01A Spirit Pet Skill Row 1 Description").style.fontSize.value.value,
                    Is.GreaterThanOrEqualTo(16));
                var roster = root.Q("Map01A Spirit Pet Roster");
                Assert.That(roster.childCount, Is.EqualTo(4));
                Assert.That(roster.style.flexShrink.value, Is.EqualTo(0).Within(.01));
                Assert.That(root.Query<VisualElement>(className: "lgo-spirit-pet-stat-row").ToList().Count,
                    Is.EqualTo(5), "Five stat rows must be created once; profiles only bind their values.");
                Assert.That(root.Q("Map01A Spirit Pet Skill Row 0").style.minHeight.value.value,
                    Is.EqualTo(88).Within(1));
                Assert.That(root.Q<Label>("Map01A Spirit Pet Skill Row 0 Name").style.fontSize.value.value,
                    Is.GreaterThanOrEqualTo(16));
                Assert.That(root.Q<Label>("Map01A Hub Detail Status").style.display.value,
                    Is.EqualTo(DisplayStyle.None),
                    "The deployed badge and disabled actions already explain state; a duplicate status card is outside canonical.");
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
            var canonical = CongDongLamArrivalHud.CalculateInventoryModalRect(new Rect(0, 0, 1672, 941), touch: false);

            Assert.That(canonical.x, Is.EqualTo(287).Within(1));
            Assert.That(canonical.y, Is.EqualTo(127).Within(1));
            Assert.That(canonical.width, Is.EqualTo(1098));
            Assert.That(canonical.height, Is.EqualTo(724));
            Assert.That(CongDongLamArrivalHud.CalculateInventoryShellHeight(canonical, touch: false, compactShell: false),
                Is.EqualTo(724));
            Assert.That(CongDongLamArrivalHud.CalculateInventoryShellHeight(canonical, touch: true, compactShell: true),
                Is.EqualTo(724),
                "Every approved character-hub tab must keep one stable outer shell instead of jumping between heights.");

            var compact = CongDongLamArrivalHud.CalculateInventoryModalRect(new Rect(0, 0, 800, 480), touch: true);
            Assert.That(compact.x, Is.EqualTo(24));
            Assert.That(compact.y, Is.EqualTo(24));
            Assert.That(compact.width, Is.EqualTo(752));
            Assert.That(compact.height, Is.EqualTo(432));
            var compactColumns = CongDongLamArrivalHud.CalculateInventoryColumnWidths(compact.width);
            Assert.That(compactColumns.x / compactColumns.y, Is.EqualTo(600f / 448f).Within(.01f));
            Assert.That(compactColumns.x + compactColumns.y + compactColumns.z,
                Is.EqualTo(compact.width - 38).Within(.1f),
                "Small screens must scale both canonical columns together instead of stacking detail below content.");
        }

        [Test]
        public void CharacterNavigationOpensApprovedCharacterHubWithoutLegacyClassSelector()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("character select modal test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var open = root.Q<Button>("Map01A Character Select Button");
                Assert.That(open, Is.Not.Null);
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value, Is.EqualTo(DisplayStyle.None));

                InvokeBoundButton(open);
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The product Nhân vật shortcut must not reopen the legacy class/pose review selector.");
                Assert.That(scene.InventoryOpen, Is.True);
                Assert.That(root.Q("Map01A Inventory").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Label>("Map01A Inventory Modal Title").text, Is.EqualTo("THÔNG TIN NHÂN VẬT"));
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q01"));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void CharacterSelectUsesOneSavedProfileAndNeverMutatesClassSelection()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("canonical character select test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var open = typeof(CongDongLamArrivalHud).GetMethod("OpenCharacterSelect", flags);
                Assert.That(open, Is.Not.Null);
                var classBefore = scene.ActiveEquipmentClassId;

                open.Invoke(hud, null);

                var overlay = root.Q("Map01A Character Select Overlay");
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Character Select Stage"), Is.Not.Null);
                Assert.That(root.Q("Map01A Character Select Account Panel"), Is.Not.Null);
                Assert.That(root.Q<VisualElement>("Map01A Character Select Preview").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetVoAvatarThumbnailSprite()),
                    "The selected-character stage must reuse the current runtime avatar source.");

                var selected = root.Q<Button>("Map01A Character Saved Profile");
                Assert.That(selected, Is.Not.Null);
                Assert.That(selected.text, Does.Contain("LụcThiên"));
                Assert.That(selected.ClassListContains("lgo-character-select-profile"), Is.True);
                Assert.That(root.Query<Button>(className: "lgo-character-select-empty-slot").ToList().Count, Is.EqualTo(2));
                Assert.That(root.Q<Button>("Map01A Character Empty Slot 1").text, Does.Contain("Chưa có nhân vật"));
                Assert.That(root.Q<Button>("Map01A Character Empty Slot 2").text, Does.Contain("Chưa có nhân vật"));
                Assert.That(root.Q("Map01A Character Card Võ"), Is.Null);
                Assert.That(root.Q("Map01A Character Card Kiếm"), Is.Null);
                Assert.That(root.Q("Map01A Character Card Pháp"), Is.Null);

                InvokeBoundButton(selected);
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(classBefore),
                    "Selecting the saved profile must never cycle class/pose review source.");
                Assert.That(root.Q<Label>("Map01A Character Select Status").text, Does.Contain("đã chọn"));

                InvokeBoundButton(root.Q<Button>("Map01A Character Select Enter Game"));
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Product Shortcut Actions").style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Entering Map01A must restore the gameplay navigation instead of preserving the hidden overlay state.");
                open.Invoke(hud, null);
                InvokeBoundButton(root.Q<Button>("Map01A Character Select Back"));
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Entry Overlay").style.display.value, Is.EqualTo(DisplayStyle.Flex));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ServerSelectUsesOneRealServerAndReturnsToItsOpeningScreen()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("canonical server select test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var openCharacter = typeof(CongDongLamArrivalHud).GetMethod("OpenCharacterSelect", flags);
                Assert.That(openCharacter, Is.Not.Null);
                var classBefore = scene.ActiveEquipmentClassId;

                var entrySwitch = root.Q<Button>("Map01A Entry Server Switch");
                Assert.That(entrySwitch.enabledSelf, Is.True);
                InvokeBoundButton(entrySwitch);

                var overlay = root.Q("Map01A Server Select Overlay");
                Assert.That(overlay, Is.Not.Null);
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Query<Button>(className: "lgo-server-select-card").ToList().Count, Is.EqualTo(1));
                Assert.That(root.Q<Label>("Map01A Server Select Name").text, Is.EqualTo("S1 · Đông Lâm"));
                Assert.That(root.Q<Label>("Map01A Server Select State").text, Is.EqualTo("Mượt"));
                Assert.That(root.Q("Map01A Server Select Region Tabs"), Is.Null);
                Assert.That(root.Q("Map01A Server Select Fake Server"), Is.Null);

                InvokeBoundButton(root.Q<Button>("Map01A Server Select Back"));
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Entry Overlay").style.display.value, Is.EqualTo(DisplayStyle.Flex));

                openCharacter.Invoke(hud, null);
                InvokeBoundButton(root.Q<Button>("Map01A Character Select Switch Server"));
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                InvokeBoundButton(root.Q<Button>("Map01A Server Select Confirm"));
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(classBefore),
                    "Selecting a server must not mutate class, pose, wardrobe or gameplay source state.");
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void RegisterScreenValidatesLocallyAndReturnsToEntry()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("canonical register screen test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var classBefore = scene.ActiveEquipmentClassId;

                InvokeBoundButton(root.Q<Button>("Map01A Entry Register Button"));
                var overlay = root.Q("Map01A Register Overlay");
                Assert.That(overlay, Is.Not.Null);
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.Flex));

                var account = root.Q<TextField>("Map01A Register Account Field");
                var password = root.Q<TextField>("Map01A Register Password Field");
                var confirmation = root.Q<TextField>("Map01A Register Confirm Password Field");
                Assert.That(account, Is.Not.Null);
                Assert.That(password.isPasswordField, Is.True);
                Assert.That(confirmation.isPasswordField, Is.True);
                Assert.That(root.Query<Button>(className: "lgo-register-primary").ToList().Count, Is.EqualTo(1));
                Assert.That(root.Query<Button>(className: "lgo-register-password-reveal").ToList().Count, Is.EqualTo(2));
                InvokeBoundButton(root.Q<Button>("Map01A Register Password Reveal"));
                Assert.That(password.isPasswordField, Is.False);
                Assert.That(confirmation.isPasswordField, Is.True, "Password reveal actions must remain independent.");
                InvokeBoundButton(root.Q<Button>("Map01A Register Password Reveal"));

                var submit = root.Q<Button>("Map01A Register Submit");
                var status = root.Q<Label>("Map01A Register Status");
                InvokeBoundButton(submit);
                Assert.That(status.text, Is.EqualTo("Nhập đủ tài khoản và hai lần mật khẩu."));

                account.value = "luc-thien@example.test";
                password.value = "mat-khau-1";
                confirmation.value = "mat-khau-2";
                InvokeBoundButton(submit);
                Assert.That(status.text, Is.EqualTo("Hai mật khẩu chưa khớp."));

                confirmation.value = password.value;
                InvokeBoundButton(submit);
                Assert.That(status.text, Is.EqualTo("Bạn cần đồng ý Điều khoản sử dụng."));

                InvokeBoundButton(root.Q<Button>("Map01A Register Agreement"));
                InvokeBoundButton(submit);
                Assert.That(status.text, Is.EqualTo("Dịch vụ đăng ký chưa kết nối. Vui lòng thử lại sau."));
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value, Is.EqualTo(DisplayStyle.None));

                InvokeBoundButton(root.Q<Button>("Map01A Register Back"));
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Entry Overlay").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                InvokeBoundButton(root.Q<Button>("Map01A Entry Register Button"));
                Assert.That(password.value, Is.Empty);
                Assert.That(confirmation.value, Is.Empty, "Register must not retain either password after reopening.");
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(classBefore),
                    "Register validation must not mutate class, pose, wardrobe or gameplay state.");
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void PasswordRecoveryRequestValidatesLocallyAndReturnsToEntry()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("canonical password recovery request screen test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var classBefore = scene.ActiveEquipmentClassId;

                var entryAccount = root.Q<TextField>("Map01A Entry Account Field");
                entryAccount.value = "luc-thien@example.test";
                InvokeBoundButton(root.Q<Button>("Map01A Entry Forgot Password"));

                var overlay = root.Q("Map01A Password Recovery Overlay");
                Assert.That(overlay, Is.Not.Null);
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Entry Control Card").style.display.value, Is.EqualTo(DisplayStyle.None));

                var account = root.Q<TextField>("Map01A Password Recovery Account Field");
                var submit = root.Q<Button>("Map01A Password Recovery Submit");
                var status = root.Q<Label>("Map01A Password Recovery Status");
                Assert.That(account, Is.Not.Null);
                Assert.That(account.value, Is.EqualTo("luc-thien@example.test"));
                Assert.That(overlay.Query<Button>(className: "lgo-auth-flow-primary").ToList().Count, Is.EqualTo(1));
                Assert.That(root.Q<TextField>("Map01A Password Recovery Code Field"), Is.Null,
                    "Request screen must not absorb the later verification-code screen.");

                account.value = string.Empty;
                InvokeBoundButton(submit);
                Assert.That(status.text, Is.EqualTo("Nhập tài khoản hoặc email để nhận hướng dẫn."));
                account.value = "luc-thien@example.test";
                InvokeBoundButton(submit);
                Assert.That(status.text, Is.EqualTo(
                    "Dịch vụ khôi phục mật khẩu chưa kết nối. Vui lòng thử lại sau."));
                Assert.That(root.Q("Map01A Character Select Overlay").style.display.value, Is.EqualTo(DisplayStyle.None));

                InvokeBoundButton(root.Q<Button>("Map01A Password Recovery Back"));
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Entry Control Card").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(scene.ActiveEquipmentClassId, Is.EqualTo(classBefore),
                    "Password recovery must not mutate class, pose, wardrobe or gameplay state.");
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
                Assert.That(menu.enabledSelf, Is.True, "Menu shortcut should open the Map01A navigation menu.");
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
                InvokeBoundButton(menu);
                var menuOverlay = root.Q("Map01A Menu Overlay");
                Assert.That(menuOverlay, Is.Not.Null);
                Assert.That(menuOverlay.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Button>("Map01A Menu Character Action"), Is.Not.Null);
                Assert.That(root.Q<Button>("Map01A Menu Bag Action"), Is.Not.Null);
                Assert.That(root.Q<Button>("Map01A Menu Skills Action"), Is.Not.Null);
                var potentialAction = root.Q<Button>("Map01A Menu Potential Action");
                Assert.That(potentialAction, Is.Not.Null,
                    "The navigation menu must expose every approved character-hub tab, including Tiềm năng.");
                Assert.That(root.Q<Button>("Map01A Menu Spirit Pet Action"), Is.Not.Null);
                Assert.That(root.Q("Map01A Menu Panel").ClassListContains("lgo-layered-frame"), Is.True,
                    "Menu should reuse the shared modal frame instead of defining a second panel system.");
                Assert.That(root.Q<Button>("Map01A Menu Character Action").style.flexGrow.value, Is.EqualTo(0),
                    "Menu grid actions must keep a bounded row height instead of stretching into the panel body.");
                InvokeBoundButton(potentialAction);
                Assert.That(menuOverlay.style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(scene.InventoryOpen, Is.True);
                Assert.That(root.Q("Map01A Potential Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Menu Tiềm năng must route into the approved shared hub tab.");
                InvokeBoundButton(menu);
                var handleEscape = typeof(CongDongLamArrivalHud).GetMethod("HandleEscape",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.That(handleEscape, Is.Not.Null,
                    "Menu must share one escape handler with the other foreground workspaces.");
                handleEscape.Invoke(hud, null);
                Assert.That(menuOverlay.style.display.value, Is.EqualTo(DisplayStyle.None));
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
        public void GameplayWorldInputIsBlockedByEveryForegroundWorkspace()
        {
            Assert.That(CongDongLamArrivalHud.ShouldBlockWorldInput(
                entryOpen: false, menuOpen: false, inventoryOpen: false, dialogueOpen: false, characterSelectOpen: false),
                Is.False, "World input should remain available while the HUD is unobstructed.");
            Assert.That(CongDongLamArrivalHud.ShouldBlockWorldInput(true, false, false, false, false), Is.True,
                "Entry/login must block movement and combat behind it.");
            Assert.That(CongDongLamArrivalHud.ShouldBlockWorldInput(false, true, false, false, false), Is.True,
                "Menu must block movement and combat behind it.");
            Assert.That(CongDongLamArrivalHud.ShouldBlockWorldInput(false, false, true, false, false), Is.True,
                "The five-tab workspace must block movement and combat behind it.");
            Assert.That(CongDongLamArrivalHud.ShouldBlockWorldInput(false, false, false, true, false), Is.True,
                "NPC dialogue must block movement and combat behind it.");
            Assert.That(CongDongLamArrivalHud.ShouldBlockWorldInput(false, false, false, false, true), Is.True,
                "Character selection must block movement and combat behind it.");
        }

        [Test]
        public void EntryCredentialsUseRealFieldsWithoutPretendingToAuthenticate()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("entry credential fields test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var account = root.Q<TextField>("Map01A Entry Account Field");
                var password = root.Q<TextField>("Map01A Entry Password Field");

                Assert.That(account, Is.Not.Null, "The approved login must accept account input instead of rendering a placeholder panel.");
                Assert.That(password, Is.Not.Null, "The approved login must accept password input instead of rendering a placeholder panel.");
                Assert.That(account.textEdition.placeholder, Does.Contain("Tài khoản"));
                Assert.That(password.textEdition.placeholder, Does.Contain("Mật khẩu"));
                Assert.That(password.isPasswordField, Is.True);
                Assert.That(account.ClassListContains("lgo-entry-text-field"), Is.True);
                Assert.That(password.ClassListContains("lgo-entry-text-field"), Is.True);

                InvokeBoundButton(root.Q<Button>("Map01A Entry Login Button"));
                var status = root.Q<Label>("Map01A Entry Safety Note");
                Assert.That(status.text, Does.Contain("Nhập tài khoản"));

                account.value = "LụcThiên";
                password.value = "demo-secret";
                InvokeBoundButton(root.Q<Button>("Map01A Entry Login Button"));
                Assert.That(status.text, Does.Contain("chưa kết nối"));
                Assert.That(root.Q("Map01A Entry Overlay").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q01"));
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void EntryRememberAccountControlPersistsOnlyTheAccountLocally()
        {
            const string rememberKey = "lgo.map01a.entry.remembered-account";
            PlayerPrefs.DeleteKey(rememberKey);
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("entry remember account test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var account = root.Q<TextField>("Map01A Entry Account Field");
                var remember = root.Q<Button>("Map01A Entry Remember Action");

                Assert.That(remember, Is.Not.Null, "Lưu tài khoản must be an actual control instead of a checked-looking decoration.");
                Assert.That(remember.ClassListContains("lgo-entry-remember-action"), Is.True);
                account.value = "LụcThiên";
                InvokeBoundButton(remember);
                Assert.That(PlayerPrefs.GetString(rememberKey, ""), Is.EqualTo("LụcThiên"));
                Assert.That(root.Q("Map01A Entry Remember Mark").style.display.value, Is.EqualTo(DisplayStyle.Flex));

                account.value = "LụcThiên2";
                Assert.That(PlayerPrefs.GetString(rememberKey, ""), Is.EqualTo("LụcThiên2"));
                InvokeBoundButton(remember);
                Assert.That(PlayerPrefs.HasKey(rememberKey), Is.False);
                Assert.That(root.Q("Map01A Entry Remember Mark").style.display.value, Is.EqualTo(DisplayStyle.None));
            }
            finally
            {
                PlayerPrefs.DeleteKey(rememberKey);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void EntryScreenMatchesCanonicalSingleCtaLayoutWithoutChangingMapState()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("canonical entry screen test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                var overlay = root.Q("Map01A Entry Overlay");
                Assert.That(overlay, Is.Not.Null);
                Assert.That(overlay.style.backgroundColor.value.a, Is.LessThanOrEqualTo(.42f));

                var panel = root.Q("Map01A Entry Panel");
                var controlCard = root.Q("Map01A Entry Control Card");
                Assert.That(panel.ClassListContains("lgo-entry-shell"), Is.True);
                Assert.That(controlCard.ClassListContains("lgo-entry-control-card"), Is.True);
                Assert.That(controlCard.ClassListContains("lgo-layered-frame"), Is.True);
                Assert.That(panel.style.maxWidth.value.value, Is.InRange(590f, 620f));
                Assert.That(controlCard.style.minHeight.value.value, Is.GreaterThanOrEqualTo(350f));

                var accountField = root.Q<TextField>("Map01A Entry Account Field");
                var passwordField = root.Q<TextField>("Map01A Entry Password Field");
                Assert.That(accountField.textEdition.placeholder, Is.EqualTo("Tài khoản / Email / Số điện thoại"));
                Assert.That(passwordField.textEdition.placeholder, Is.EqualTo("Mật khẩu"));
                Assert.That(passwordField.isPasswordField, Is.True);
                Assert.That(accountField.ClassListContains("lgo-input-field"), Is.True);
                Assert.That(passwordField.ClassListContains("lgo-input-field"), Is.True);
                Assert.That(root.Q("Map01A Entry Account Field Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01AHudIconSprite("account")));
                Assert.That(root.Q("Map01A Entry Password Field Icon").style.backgroundImage.value.sprite,
                    Is.EqualTo(scene.GetMap01AHudIconSprite("lock")));
                var passwordReveal = root.Q<Button>("Map01A Entry Password Reveal");
                Assert.That(passwordReveal, Is.Not.Null);
                Assert.That(passwordReveal.ClassListContains("lgo-entry-password-reveal"), Is.True);
                Assert.That(passwordField.isPasswordField, Is.True);
                InvokeBoundButton(passwordReveal);
                Assert.That(passwordField.isPasswordField, Is.False);
                InvokeBoundButton(passwordReveal);
                Assert.That(passwordField.isPasswordField, Is.True);

                var authActions = root.Q("Map01A Entry Auth Actions");
                var loginButton = root.Q<Button>("Map01A Entry Login Button");
                var registerButton = root.Q<Button>("Map01A Entry Register Button");
                Assert.That(authActions, Is.Not.Null);
                Assert.That(loginButton.ClassListContains("lgo-entry-auth-primary"), Is.True);
                Assert.That(registerButton.ClassListContains("lgo-entry-auth-secondary"), Is.True);
                Assert.That(loginButton.text, Is.EqualTo("Đăng nhập"));
                Assert.That(registerButton.text, Is.EqualTo("Đăng ký"));
                Assert.That(loginButton.style.minHeight.value.value, Is.InRange(46f, 54f));
                Assert.That(root.Q("Map01A Entry Start Button"), Is.Null,
                    "The canonical design has one primary login CTA and no second Start action.");
                Assert.That(root.Q("Map01A Entry Primary Cta Row"), Is.Null);
                Assert.That(root.Q("Map01A Entry Cta Ornament Left"), Is.Null);

                var serverCard = root.Q("Map01A Entry Server Card");
                var serverState = root.Q<Label>("Map01A Entry Server State");
                var serverSwitch = root.Q<Button>("Map01A Entry Server Switch");
                Assert.That(serverCard.ClassListContains("lgo-entry-server-card"), Is.True);
                Assert.That(serverCard.ClassListContains("lgo-detail-card"), Is.True);
                Assert.That(root.Q<Label>("Map01A Entry Server Name").text, Is.EqualTo("S1 · Đông Lâm"));
                Assert.That(serverState.text, Is.EqualTo("● Mượt"));
                Assert.That(serverState.style.whiteSpace.value, Is.EqualTo(WhiteSpace.NoWrap));
                Assert.That(serverSwitch.text, Is.EqualTo("›"));
                Assert.That(serverSwitch.enabledSelf, Is.True);

                var status = root.Q<Label>("Map01A Entry Safety Note");
                Assert.That(status.ClassListContains("lgo-entry-status-line"), Is.True);
                Assert.That(status.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "The canonical card reserves a stable inline status line to avoid layout jumps.");
                InvokeBoundButton(loginButton);
                Assert.That(status.text, Does.Contain("Nhập tài khoản"));
                accountField.value = "LụcThiên";
                passwordField.value = "demo-secret";
                InvokeBoundButton(loginButton);
                Assert.That(status.text, Does.Contain("chưa kết nối"));
                Assert.That(overlay.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q01"));

                Assert.That(root.Q("Map01A Entry Notice Panel").ClassListContains("lgo-status-card"), Is.True);
                Assert.That(root.Q("Map01A Entry Notice Panel").style.width.value.value, Is.InRange(500f, 560f));
                foreach (var name in new[] { "Thông Báo", "Hỗ Trợ", "Cinematic", "Cài Đặt" })
                {
                    var sideAction = root.Q<Button>("Map01A Entry Side Action " + name);
                    Assert.That(sideAction, Is.Not.Null);
                    Assert.That(sideAction.ClassListContains("lgo-entry-side-action"), Is.True);
                    Assert.That(sideAction.text, Is.EqualTo(name));
                    Assert.That(sideAction.style.whiteSpace.value, Is.EqualTo(WhiteSpace.NoWrap));
                    Assert.That(sideAction.Q(sideAction.name + " Icon"), Is.Not.Null);
                }
                var sideActions = root.Q("Map01A Entry Side Actions");
                CollectionAssert.AreEqual(new[]
                {
                    "Map01A Entry Side Action Thông Báo",
                    "Map01A Entry Side Action Hỗ Trợ",
                    "Map01A Entry Side Action Cinematic",
                    "Map01A Entry Side Action Cài Đặt"
                }, sideActions.Children().Select(child => child.name).ToArray());
                Assert.That(root.Q("Map01A Safe Hud").style.display.value, Is.EqualTo(DisplayStyle.None));
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
        public void InventorySearchFiltersRealItemsAndKeepsDetailSelectionOnTheRight()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("inventory search test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene);
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                hud.OpenInventoryReviewMode("bag");

                var search = root.Q<TextField>("Map01A Inventory Search");
                Assert.That(search, Is.Not.Null);
                Assert.That(search.ClassListContains("lgo-input-field"), Is.True);
                Assert.That(search.ClassListContains("lgo-inventory-search-field"), Is.True);
                var searchInput = search.Q(className: "unity-base-text-field__input")
                    ?? search.Q(className: "unity-text-field__input")
                    ?? search.Q("unity-text-input");
                Assert.That(searchInput, Is.Not.Null);
                Assert.That(searchInput.ClassListContains("lgo-inventory-search-input"), Is.True,
                    "The actual TextField input must use the shared dark game skin instead of Unity's white default.");
                search.value = "binh mau";

                Assert.That(root.Q<Button>("Map01A Health Potion").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Button>("Map01A Mana Potion").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile main_weapon").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q<Label>("Map01A Inventory Count Badge").text, Is.EqualTo("1 kết quả · 56/120 ô"),
                    "Search feedback must show the number of real matching items instead of leaving the capacity badge unchanged.");
                var healthPotion = root.Q<Button>("Map01A Health Potion");
                Assert.That(healthPotion.style.backgroundColor.value.b, Is.LessThan(.3f),
                    "A filtered result must not look selected while the right-side detail still belongs to equipment.");
                Assert.That(healthPotion.ClassListContains("lgo-character-hub-selected"), Is.False);
                InvokeBoundButton(healthPotion);
                Assert.That(healthPotion.ClassListContains("lgo-character-hub-selected"), Is.True,
                    "Selecting the result must synchronize its shared semantic highlight with the right-side detail.");
                Assert.That(root.Q<Label>("Map01A Inventory Detail Item Name").text, Is.EqualTo("Bình Máu Nhỏ"));

                search.value = "";
                Assert.That(root.Q<Button>("Map01A Equipment Item Tile main_weapon").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<Label>("Map01A Inventory Count Badge").text, Is.EqualTo("56/120 ô"));
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
                Assert.That(root.Q<Label>("Map01A Player Name").text, Is.EqualTo("LụcThiên"),
                    "The player card must lead with the character identity from the approved HUD hierarchy, not only a class/debug label.");
                var playerMeta = root.Q<Label>("Map01A Player Class Meta");
                Assert.That(playerMeta, Is.Not.Null);
                Assert.That(playerMeta.text, Does.Contain(scene.ActiveEquipmentClassLabel));
                Assert.That(playerMeta.text, Does.Contain("Lv.1"));
                Assert.That(playerMeta.style.fontSize.value.value, Is.LessThan(root.Q<Label>("Map01A Player Name").style.fontSize.value.value),
                    "Class, gender and level must stay subordinate to the player name instead of sharing one flat line.");
                Assert.That(root.Q("Map01A Quest Tracker Body").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD quest body must use the shared info-panel base instead of one-off panel styling.");
                Assert.That(root.Q("Map01A Quest Tracker Body").style.backgroundColor.value.a, Is.GreaterThanOrEqualTo(.9f),
                    "Quest tracking needs a readable shared glass background over bright map art.");
                var touchPad = root.Q<RuntimeTouchMovementPad>("LGO World Touch Movement Pad");
                var touchNub = touchPad.Q<VisualElement>("LGO World Touch Movement Nub");
                Assert.That(touchPad.ClassListContains("lgo-touch-movement-pad"), Is.True);
                Assert.That(touchNub.ClassListContains("lgo-touch-movement-nub"), Is.True,
                    "The touch joystick must use the shared circular control instead of a square placeholder nub.");
                Assert.That(touchPad.style.borderTopLeftRadius.value.value, Is.GreaterThanOrEqualTo(50));
                Assert.That(touchNub.style.borderTopLeftRadius.value.value, Is.GreaterThanOrEqualTo(18));
                Assert.That(touchNub.style.marginLeft.value.value, Is.EqualTo(0));
                Assert.That(touchNub.style.marginTop.value.value, Is.EqualTo(0));
                var questCategory = root.Q<Label>("Map01A Quest Category");
                var questTitle = root.Q<Label>("Map01A Quest Title");
                var questObjective = root.Q<Label>("Map01A Quest Objective");
                var questProgress = root.Q<Label>("Map01A Quest Progress");
                Assert.That(questCategory.text, Is.EqualTo("NHIỆM VỤ CHÍNH"));
                Assert.That(questTitle.text, Does.StartWith("Q01 ·"));
                Assert.That(questObjective.text, Does.Contain("Hạ Vân"));
                Assert.That(questProgress.text, Does.Contain("0/9"));
                Assert.That(questTitle.style.fontSize.value.value, Is.GreaterThan(questObjective.style.fontSize.value.value),
                    "Quest title and objective need separate visual hierarchy instead of one flat multiline debug label.");
                Assert.That(CongDongLamArrivalHud.QuestInteractionMessageForDisplay(
                        "Cổng đường sang Suối Thanh Minh đã mở.",
                        "Portal Suối Thanh Minh đã mở.",
                        "Cổng đường sang Suối Thanh Minh đã mở."),
                    Is.Empty,
                    "Quest completion feedback must not repeat the same sentence already shown as tracker progress.");
                Assert.That(root.Q("Map01A Minimap").ClassListContains("lgo-hud-info-panel"), Is.True,
                    "HUD minimap placeholder must use the shared info-panel base until a real minimap art pass replaces it.");
                Assert.That(root.Q("Map01A Minimap").style.backgroundColor.value.a, Is.GreaterThanOrEqualTo(.9f),
                    "Map and quest cards must remain readable over bright sky and share one HUD background token.");
                var minimapTrack = root.Q("Map01A Minimap Route Track");
                Assert.That(minimapTrack, Is.Not.Null,
                    "Map01A HUD must render its deterministic route data instead of leaving a text-only minimap placeholder.");
                Assert.That(minimapTrack.childCount, Is.EqualTo(5));
                Assert.That(root.Q("Map01A Minimap Current Marker"), Is.Not.Null);
                Assert.That(root.Q<Label>("Map01A Minimap Status"), Is.Not.Null);
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
                Assert.That(root.Q("Map01A Quest Interaction Message").style.display.value, Is.EqualTo(DisplayStyle.None),
                    "The quest tracker must not repeat an NPC conversation status while the dialogue panel already identifies the speaker.");
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
                Assert.That(speaker.text, Is.EqualTo(scene.DialogueSpeaker),
                    "Dialogue progress belongs in the quest-context row and must not repeat beside the NPC name.");
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
                Assert.That(talk.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "The contextual action must be visible when the current route action is usable.");
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
                typeof(CongDongLamMap01AArtPreview).GetField("_routeX", flags).SetValue(scene, scene.PlayerX + 2f);
                scene.Refresh();
                update.Invoke(hud, null);
                Assert.That(inventory.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(combat.style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(scene.CanUseCurrentRouteAction, Is.False);
                Assert.That(talk.style.display.value, Is.EqualTo(DisplayStyle.None),
                    "A completed or out-of-range route action must leave the HUD instead of becoming a dead translucent button.");
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
