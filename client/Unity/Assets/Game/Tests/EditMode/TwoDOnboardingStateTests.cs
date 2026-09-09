using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests
{
    public sealed class TwoDOnboardingStateTests
    {
        [Test]
        public void FlowRequiresGateDialogueBeforeTrainingStone()
        {
            var state = new TwoDOnboardingState();
            state.Reset();

            state.Move(TwoDOnboardingState.TrainingStonePosition - TwoDOnboardingState.PlayerStart);
            Assert.AreEqual(TwoDOnboardingStep.FindGateKeeper, state.Step);
            Assert.AreEqual(TwoDOnboardingAction.None, state.AvailableAction);
            Assert.IsFalse(state.TryUseAction());

            state.Move(TwoDOnboardingState.GateKeeperPosition - state.PlayerPosition);
            Assert.AreEqual(TwoDOnboardingAction.Talk, state.AvailableAction);
            Assert.IsTrue(state.TryUseAction());
            Assert.AreEqual(TwoDOnboardingStep.TalkToGateKeeper, state.Step);
            StringAssert.Contains("Chào mừng đến Linh Thành", state.DialogueLine);

            Assert.IsTrue(state.TryUseAction());
            Assert.AreEqual(TwoDOnboardingStep.GoToTrainingStone, state.Step);
            Assert.IsFalse(state.DialogueOpen);

            state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
            Assert.AreEqual(TwoDOnboardingAction.Train, state.AvailableAction);
            Assert.IsTrue(state.TryUseAction());
            Assert.AreEqual(TwoDOnboardingStep.LearnJump, state.Step);
            StringAssert.Contains("Nhảy", state.ObjectiveText);

            Assert.IsTrue(state.TryUseJump());
            Assert.AreEqual(TwoDOnboardingStep.LearnDash, state.Step);
            Assert.IsTrue(state.TryUseDash());
            Assert.AreEqual(TwoDOnboardingStep.LearnClassSkill, state.Step);
            Assert.IsTrue(state.TryUseClassSkill());
            Assert.AreEqual(TwoDOnboardingStep.Complete, state.Step);
            StringAssert.Contains("Mở Linh Thành", state.ObjectiveText);
        }


        [Test]
        public void CompletingDongMonUnlocksLinhThanhPlazaLocally()
        {
            var state = new TwoDOnboardingState();
            state.Reset();

            Assert.IsFalse(state.LinhThanhUnlocked);

            state.Move(TwoDOnboardingState.GateKeeperPosition - state.PlayerPosition);
            state.TryUseAction();
            state.TryUseAction();
            state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
            state.TryUseAction();
            state.TryUseJump();
            state.TryUseDash();

            Assert.IsFalse(state.LinhThanhUnlocked);
            Assert.IsTrue(state.TryUseClassSkill());

            Assert.IsTrue(state.LinhThanhUnlocked);
            StringAssert.Contains("Mở Linh Thành", state.ObjectiveText);
            StringAssert.Contains("Quảng Trường", state.HintText);
            StringAssert.Contains("return-gate", state.CurrentRouteNodeId);
        }

        [Test]
        public void MovementIsClampedAndRejectsInvalidInput()
        {
            var state = new TwoDOnboardingState();
            state.Reset();

            state.Move(new Vector2(float.NaN, 1f));
            Assert.AreEqual(TwoDOnboardingState.PlayerStart, state.PlayerPosition);

            state.Move(new Vector2(99f, -99f));
            Assert.That(state.PlayerPosition.x, Is.EqualTo(4.25f).Within(0.0001f));
            Assert.That(state.PlayerPosition.y, Is.EqualTo(-2.15f).Within(0.0001f));
        }

        [Test]
        public void FocusActionChangesWithCurrentObjective()
        {
            var state = new TwoDOnboardingState();
            state.Reset();

            state.Move(TwoDOnboardingState.GateKeeperPosition - state.PlayerPosition);
            Assert.AreEqual(TwoDOnboardingAction.Talk, state.AvailableAction);

            state.TryUseAction();
            Assert.AreEqual(TwoDOnboardingAction.Continue, state.AvailableAction);

            state.TryUseAction();
            Assert.AreEqual(TwoDOnboardingStep.GoToTrainingStone, state.Step);
            Assert.AreEqual(TwoDOnboardingAction.None, state.AvailableAction);

            state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
            Assert.AreEqual(TwoDOnboardingAction.Train, state.AvailableAction);
        }



        [Test]
        public void CharacterBaseCatalogDefinesMaleFemaleLayeredDefaults()
        {
            var catalog = TwoDCharacterBaseCatalog.CreateDefault();

            Assert.That(catalog.Bases.Length, Is.EqualTo(2));
            Assert.That(catalog.MinimumAnchorCount, Is.GreaterThanOrEqualTo(20));
            StringAssert.Contains("male_base", catalog.Snapshot);
            StringAssert.Contains("female_base", catalog.Snapshot);
            StringAssert.Contains("HairFront", catalog.Snapshot);
            StringAssert.Contains("WeaponAnchor", catalog.Snapshot);
            StringAssert.Contains("grey shorts", catalog.Snapshot);
        }

        [Test]
        public void RuntimeControllerExposesCharacterBaseSnapshotForVisualEvidence()
        {
            var host = new GameObject("2D character base snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("male_base", controller.RuntimeCharacterBaseSnapshot);
                StringAssert.Contains("female_base", controller.RuntimeCharacterBaseSnapshot);
                StringAssert.Contains("LayeredCharacter", controller.RuntimeCharacterBaseSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void CharacterModuleCatalogDefinesInventoryPreviewSlots()
        {
            var modules = TwoDCharacterModuleCatalog.CreateDefault();

            Assert.That(modules.Items.Length, Is.GreaterThanOrEqualTo(10));
            StringAssert.Contains("hair_front_basic_male", modules.Snapshot);
            StringAssert.Contains("eyes_default", modules.Snapshot);
            StringAssert.Contains("top_common_male", modules.Snapshot);
            StringAssert.Contains("top_common_female", modules.Snapshot);
            StringAssert.Contains("pants_common_unisex", modules.Snapshot);
            StringAssert.Contains("PreviewFlow: select_icon -> inspect_item -> try_on -> cancel_or_apply", modules.Snapshot);
        }

        [Test]
        public void CharacterLoadoutSupportsTryOnCancelAndApply()
        {
            var modules = TwoDCharacterModuleCatalog.CreateDefault();
            var loadout = TwoDCharacterLoadout.CreateStarter("male_base", modules);

            StringAssert.Contains("top=top_common_male", loadout.Snapshot);
            Assert.IsTrue(loadout.TryPreview("top_vo_lv1_male", modules));
            StringAssert.Contains("preview=top_vo_lv1_male", loadout.Snapshot);
            StringAssert.Contains("status=TRYING_ON", loadout.Snapshot);

            loadout.CancelPreview();
            StringAssert.DoesNotContain("top_vo_lv1_male", loadout.Snapshot);
            StringAssert.Contains("status=EQUIPPED", loadout.Snapshot);

            Assert.IsTrue(loadout.TryPreview("top_vo_lv1_male", modules));
            loadout.ApplyPreview();
            StringAssert.Contains("top=top_vo_lv1_male", loadout.Snapshot);
            StringAssert.Contains("status=EQUIPPED", loadout.Snapshot);
        }

        [Test]
        public void RuntimeControllerExposesModularEquipmentSnapshotForVisualEvidence()
        {
            var host = new GameObject("2D modular equipment snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("LayeredEquipment", controller.RuntimeEquipmentSnapshot);
                StringAssert.Contains("hair_front_basic_male", controller.RuntimeEquipmentSnapshot);
                StringAssert.Contains("eyes_default", controller.RuntimeEquipmentSnapshot);
                StringAssert.Contains("top_common_male", controller.RuntimeEquipmentSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeControllerExposesInventoryTryOnSnapshotForVisualEvidence()
        {
            var host = new GameObject("2D inventory try-on snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("InventoryTryOn", controller.RuntimeInventoryTryOnSnapshot);
                StringAssert.Contains("select_icon -> inspect_item -> try_on -> cancel_or_apply", controller.RuntimeInventoryTryOnSnapshot);
                StringAssert.Contains("selected=top_kiem_lv1_male", controller.RuntimeInventoryTryOnSnapshot);
                StringAssert.Contains("preview=top_kiem_lv1_male", controller.RuntimeInventoryTryOnSnapshot);
                StringAssert.Contains("weapon_kiem_lv1_starter", controller.RuntimeInventoryTryOnSnapshot);
                StringAssert.Contains("status=TRYING_ON", controller.RuntimeInventoryTryOnSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void VisualCaptureManifestIncludesInventoryInputRuntimeSnapshot()
        {
            var resultType = typeof(TwoDOnboardingVisualCaptureRunner).GetNestedType("TwoDOnboardingVisualCaptureResult", System.Reflection.BindingFlags.NonPublic);

            Assert.IsNotNull(resultType);
            Assert.IsNotNull(resultType.GetField("runtimeInventoryInputSnapshot"));
            Assert.IsNotNull(resultType.GetField("runtimePlazaHubInputSnapshot"));
            Assert.IsNotNull(resultType.GetField("runtimeLinhThanhUnlockSnapshot"));
            Assert.IsNotNull(resultType.GetField("runtimeLinhThanhPlazaHubSnapshot"));
        }

        [Test]
        public void RuntimeControllerSupportsInventoryInputTryApplyCancel()
        {
            var host = new GameObject("2D inventory input test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                controller.ToggleInventoryPanel();
                StringAssert.Contains("InventoryInputState=Open", controller.RuntimeInventoryInputSnapshot);
                StringAssert.Contains("selected=top_kiem_lv1_male", controller.RuntimeInventoryInputSnapshot);

                Assert.IsTrue(controller.PreviewSelectedInventoryItem());
                StringAssert.Contains("InventoryInputState=Trying", controller.RuntimeInventoryInputSnapshot);
                StringAssert.Contains("preview=top_kiem_lv1_male", controller.RuntimeInventoryInputSnapshot);

                controller.ApplyInventoryPreview();
                StringAssert.Contains("InventoryInputState=Applied", controller.RuntimeInventoryInputSnapshot);
                StringAssert.Contains("top=top_kiem_lv1_male", controller.RuntimeInventoryInputSnapshot);

                controller.ToggleInventoryPanel();
                controller.PreviewSelectedInventoryItem();
                controller.CancelInventoryPreview();
                StringAssert.Contains("InventoryInputState=Cancelled", controller.RuntimeInventoryInputSnapshot);
                StringAssert.Contains("status=EQUIPPED", controller.RuntimeInventoryInputSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void VoLv1StarterModulesCoverMaleFemaleCoreOutfitSlots()
        {
            var modules = TwoDCharacterModuleCatalog.CreateDefault();

            foreach (var id in new[]
            {
                "top_vo_lv1_male", "top_vo_lv1_female", "waist_vo_lv1_unisex",
                "gloves_vo_lv1_unisex", "boots_vo_lv1_unisex", "pants_vo_lv1_unisex"
            })
            {
                Assert.IsTrue(modules.TryFind(id, out _), id);
            }
            StringAssert.Contains("class=Vo", modules.Snapshot);
            StringAssert.Contains("level=1", modules.Snapshot);
        }

        [Test]
        public void KiemLv1StarterModulesCoverMaleFemaleCoreOutfitAndWeaponSlots()
        {
            var modules = TwoDCharacterModuleCatalog.CreateDefault();

            foreach (var id in new[]
            {
                "top_kiem_lv1_male", "top_kiem_lv1_female", "pants_kiem_lv1_unisex",
                "waist_kiem_lv1_unisex", "gloves_kiem_lv1_unisex", "boots_kiem_lv1_unisex",
                "weapon_kiem_lv1_starter"
            })
            {
                Assert.IsTrue(modules.TryFind(id, out _), id);
            }
            StringAssert.Contains("class=Kiem", modules.Snapshot);
            StringAssert.Contains("sword_trail_seed", modules.Snapshot);
            StringAssert.Contains("base_male_torso", modules.Snapshot);
            StringAssert.Contains("base_female_torso", modules.Snapshot);
        }

        [Test]
        public void CharacterLoadoutCanMixVoPantsWithKiemTopAndWeapon()
        {
            var modules = TwoDCharacterModuleCatalog.CreateDefault();
            var loadout = TwoDCharacterLoadout.CreateStarter("male_base", modules);

            loadout.ApplyVoLv1Starter(modules);
            Assert.IsTrue(loadout.TryEquip("top_kiem_lv1_male", modules));
            Assert.IsTrue(loadout.TryEquip("weapon_kiem_lv1_starter", modules));

            StringAssert.Contains("top=top_kiem_lv1_male", loadout.Snapshot);
            StringAssert.Contains("pants=pants_vo_lv1_unisex", loadout.Snapshot);
            StringAssert.Contains("weapon=weapon_kiem_lv1_starter", loadout.Snapshot);
            StringAssert.Contains("status=EQUIPPED", loadout.Snapshot);
        }

        [Test]
        public void TrainingCompletionAppliesVoLv1StarterPreviewToRuntimeSnapshot()
        {
            var host = new GameObject("2D Vo Lv1 apply test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();

                StringAssert.Contains("top=top_vo_lv1_male", controller.RuntimeEquipmentSnapshot);
                StringAssert.Contains("waist=waist_vo_lv1_unisex", controller.RuntimeEquipmentSnapshot);
                StringAssert.Contains("class=Vo", controller.RuntimeEquipmentSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }



        [Test]
        public void TrainingStoneUnlocksJumpDashAndClassSkillBeforeCompletion()
        {
            var state = new TwoDOnboardingState();
            state.Reset();
            state.Move(TwoDOnboardingState.GateKeeperPosition - state.PlayerPosition);
            state.TryUseAction();
            state.TryUseAction();
            state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
            state.TryUseAction();

            Assert.AreEqual(TwoDOnboardingStep.LearnJump, state.Step);
            Assert.AreEqual(TwoDOnboardingAction.Jump, state.AvailableAction);
            Assert.IsTrue(state.TryUseJump());
            Assert.AreEqual("Jump", state.LastAnimationIntent);

            Assert.AreEqual(TwoDOnboardingStep.LearnDash, state.Step);
            Assert.AreEqual(TwoDOnboardingAction.Dash, state.AvailableAction);
            Assert.IsTrue(state.TryUseDash());
            Assert.AreEqual("Dash", state.LastAnimationIntent);

            Assert.AreEqual(TwoDOnboardingStep.LearnClassSkill, state.Step);
            Assert.AreEqual(TwoDOnboardingAction.Skill, state.AvailableAction);
            Assert.IsTrue(state.TryUseClassSkill());
            Assert.AreEqual("ClassSkill", state.LastAnimationIntent);
            Assert.AreEqual(TwoDOnboardingStep.Complete, state.Step);
        }

        [Test]
        public void LocomotionAnimationProfileDefinesSideScrollStates()
        {
            var profile = TwoDLocomotionAnimationProfile.CreateDefault();

            StringAssert.Contains("locomotion=side_scroll", profile.Snapshot);
            StringAssert.Contains("Idle", profile.Snapshot);
            StringAssert.Contains("Walk", profile.Snapshot);
            StringAssert.Contains("Run", profile.Snapshot);
            StringAssert.Contains("Jump", profile.Snapshot);
            StringAssert.Contains("Dash", profile.Snapshot);
            StringAssert.Contains("ClassSkill", profile.Snapshot);
            StringAssert.Contains("TrainingCompletePose", profile.Snapshot);
        }

        [Test]
        public void RuntimeControllerUpdatesAnimationSnapshotAfterMovementAndCompletion()
        {
            var host = new GameObject("2D locomotion animation snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();
                StringAssert.Contains("Idle", controller.RuntimeAnimationSnapshot);

                controller.State.Move(new Vector2(1.2f, 0f));
                controller.RefreshForSmoke();
                StringAssert.Contains("Walk", controller.RuntimeAnimationSnapshot);

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.RefreshForSmoke();
                StringAssert.Contains("ClassSkill", controller.RuntimeAnimationSnapshot);

                controller.State.TryUseJump();
                controller.RefreshForSmoke();
                StringAssert.Contains("Jump", controller.RuntimeAnimationSnapshot);

                controller.State.TryUseDash();
                controller.RefreshForSmoke();
                StringAssert.Contains("Dash", controller.RuntimeAnimationSnapshot);

                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();
                StringAssert.Contains("TrainingCompletePose", controller.RuntimeAnimationSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void ClassSkillDefeatsShadowSlimeBeforeTutorialCompletes()
        {
            var state = new TwoDOnboardingState();
            state.Reset();
            state.Move(TwoDOnboardingState.GateKeeperPosition - state.PlayerPosition);
            state.TryUseAction();
            state.TryUseAction();
            state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
            state.TryUseAction();
            state.TryUseJump();
            state.TryUseDash();

            Assert.AreEqual(TwoDOnboardingStep.LearnClassSkill, state.Step);
            Assert.IsTrue(state.ShadowSlimeVisible);
            Assert.IsFalse(state.ShadowSlimeDefeated);
            Assert.IsTrue(state.TryUseClassSkill());
            Assert.IsTrue(state.ShadowSlimeDefeated);
            StringAssert.Contains("Shadow Slime", state.FeedbackText);
            Assert.AreEqual(TwoDOnboardingStep.Complete, state.Step);
        }

        [Test]
        public void RuntimeControllerExposesShadowSlimeSnapshotForVisualEvidence()
        {
            var host = new GameObject("2D Shadow Slime snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.RefreshForSmoke();

                StringAssert.Contains("ShadowSlimeVisible=True", controller.RuntimeCombatSnapshot);
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();
                StringAssert.Contains("ShadowSlimeDefeated=True", controller.RuntimeCombatSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeMapCatalogKeepsWorldHubAndDongMonRouteTogether()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            Assert.That(map.WorldZones.Length, Is.GreaterThanOrEqualTo(10));
            Assert.That(map.LinhThanhDistricts.Length, Is.GreaterThanOrEqualTo(8));
            Assert.That(map.DongMonRoute.Length, Is.GreaterThanOrEqualTo(6));
            StringAssert.Contains("Linh Thành", map.WorldSnapshot);
            StringAssert.Contains("Đông Môn", map.WorldSnapshot);
            StringAssert.Contains("Chapter 1: Vết Nứt Đông Môn", map.LayerBudgetSnapshot);
            StringAssert.Contains("Sky/Fog", map.LayerBudgetSnapshot);
            StringAssert.Contains("Gameplay Plane", map.LayerBudgetSnapshot);
            StringAssert.Contains("Foreground", map.LayerBudgetSnapshot);
            Assert.That(map.DongMonLandmarks.Length, Is.GreaterThanOrEqualTo(6));
            StringAssert.Contains("Landmarks: Chapter 1: Vết Nứt Đông Môn", map.LandmarkSnapshot);
            StringAssert.Contains("gate-landmark", map.LandmarkSnapshot);
            StringAssert.Contains("wood-bridge", map.LandmarkSnapshot);
            StringAssert.Contains("spirit-waterfall", map.LandmarkSnapshot);
            StringAssert.Contains("song-linh", map.LandmarkSnapshot);
            StringAssert.Contains("outer-forest", map.LandmarkSnapshot);
            StringAssert.Contains("Người Giữ Cổng", map.TutorialRouteSnapshot);
            StringAssert.Contains("Bia Luyện Khí", map.TutorialRouteSnapshot);
            StringAssert.Contains("Shadow Slime", map.TutorialRouteSnapshot);
            StringAssert.Contains("Quay về Người Giữ Cổng", map.TutorialRouteSnapshot);
            StringAssert.Contains("Mini Boss", map.TutorialRouteSnapshot);
        }


        [Test]
        public void RuntimeMapCatalogKeepsWorldZoneNetwork()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            Assert.That(map.ZoneConnections.Length, Is.GreaterThanOrEqualTo(5));
            StringAssert.Contains("WorldMapNetwork: hub=linh-thanh", map.ZoneNetworkSnapshot);
            StringAssert.Contains("linh-thanh->east", map.ZoneNetworkSnapshot);
            StringAssert.Contains("linh-thanh->spirit-mountain", map.ZoneNetworkSnapshot);
            StringAssert.Contains("linh-thanh->underworld", map.ZoneNetworkSnapshot);
            StringAssert.Contains("LinhThanhHubRuntime:", map.ZoneNetworkSnapshot);
            StringAssert.Contains("east-gate:Đông Môn", map.ZoneNetworkSnapshot);
            StringAssert.Contains("plaza:Quảng Trường", map.ZoneNetworkSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesZoneNetworkSnapshot()
        {
            var host = new GameObject("2D zone network snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("WorldMapNetwork: hub=linh-thanh", controller.RuntimeZoneNetworkSnapshot);
                StringAssert.Contains("LinhThanhHubRuntime:", controller.RuntimeZoneNetworkSnapshot);
                StringAssert.Contains("linh-thanh->east", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhHubShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("HubShell: linh-thanh", map.LinhThanhHubShellSnapshot);
            StringAssert.Contains("district=east-gate", map.LinhThanhHubShellSnapshot);
            StringAssert.Contains("district=plaza", map.LinhThanhHubShellSnapshot);
            StringAssert.Contains("district=academy", map.LinhThanhHubShellSnapshot);
            StringAssert.Contains("district=market", map.LinhThanhHubShellSnapshot);
            StringAssert.Contains("HubShell: linh-thanh", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhHubShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh hub shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("HubShell: linh-thanh", controller.RuntimeLinhThanhHubShellSnapshot);
                StringAssert.Contains("district=plaza", controller.RuntimeLinhThanhHubShellSnapshot);
                StringAssert.Contains("district=market", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhPlazaShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("PlazaShell: district=plaza", map.LinhThanhPlazaShellSnapshot);
            StringAssert.Contains("social-spawn", map.LinhThanhPlazaShellSnapshot);
            StringAssert.Contains("event-board", map.LinhThanhPlazaShellSnapshot);
            StringAssert.Contains("guild-bulletin-preview", map.LinhThanhPlazaShellSnapshot);
            StringAssert.Contains("safe-no-trade-backend", map.LinhThanhPlazaShellSnapshot);
            StringAssert.Contains("PlazaShell: district=plaza", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhPlazaShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh plaza shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("PlazaShell: district=plaza", controller.RuntimeLinhThanhPlazaShellSnapshot);
                StringAssert.Contains("safe-no-trade-backend", controller.RuntimeLinhThanhPlazaShellSnapshot);
                StringAssert.Contains("event-board", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhAcademyShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("AcademyShell: district=academy", map.LinhThanhAcademyShellSnapshot);
            StringAssert.Contains("skill-hall=preview-only", map.LinhThanhAcademyShellSnapshot);
            StringAssert.Contains("class-trainer=locked", map.LinhThanhAcademyShellSnapshot);
            StringAssert.Contains("safe-no-skill-backend", map.LinhThanhAcademyShellSnapshot);
            StringAssert.Contains("AcademyShell: district=academy", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhAcademyShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh academy shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("AcademyShell: district=academy", controller.RuntimeLinhThanhAcademyShellSnapshot);
                StringAssert.Contains("safe-no-skill-backend", controller.RuntimeLinhThanhAcademyShellSnapshot);
                StringAssert.Contains("AcademyShell: district=academy", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhMarketShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("MarketShell: district=market", map.LinhThanhMarketShellSnapshot);
            StringAssert.Contains("vendor-row=preview-only", map.LinhThanhMarketShellSnapshot);
            StringAssert.Contains("auction-board=locked", map.LinhThanhMarketShellSnapshot);
            StringAssert.Contains("safe-no-trade-backend", map.LinhThanhMarketShellSnapshot);
            StringAssert.Contains("MarketShell: district=market", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhMarketShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh market shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("MarketShell: district=market", controller.RuntimeLinhThanhMarketShellSnapshot);
                StringAssert.Contains("safe-no-trade-backend", controller.RuntimeLinhThanhMarketShellSnapshot);
                StringAssert.Contains("MarketShell: district=market", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhSpiritTempleShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("SpiritTempleShell: district=spirit-temple", map.LinhThanhSpiritTempleShellSnapshot);
            StringAssert.Contains("blessing-altar=preview-only", map.LinhThanhSpiritTempleShellSnapshot);
            StringAssert.Contains("story-shrine=locked", map.LinhThanhSpiritTempleShellSnapshot);
            StringAssert.Contains("safe-no-buff-backend", map.LinhThanhSpiritTempleShellSnapshot);
            StringAssert.Contains("SpiritTempleShell: district=spirit-temple", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhSpiritTempleShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh spirit temple shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("SpiritTempleShell: district=spirit-temple", controller.RuntimeLinhThanhSpiritTempleShellSnapshot);
                StringAssert.Contains("safe-no-buff-backend", controller.RuntimeLinhThanhSpiritTempleShellSnapshot);
                StringAssert.Contains("SpiritTempleShell: district=spirit-temple", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhResidentialShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("ResidentialShell: district=residential", map.LinhThanhResidentialShellSnapshot);
            StringAssert.Contains("npc-home-row=preview-only", map.LinhThanhResidentialShellSnapshot);
            StringAssert.Contains("social-chat-node=locked", map.LinhThanhResidentialShellSnapshot);
            StringAssert.Contains("safe-no-housing-backend", map.LinhThanhResidentialShellSnapshot);
            StringAssert.Contains("ResidentialShell: district=residential", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhResidentialShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh residential shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("ResidentialShell: district=residential", controller.RuntimeLinhThanhResidentialShellSnapshot);
                StringAssert.Contains("safe-no-housing-backend", controller.RuntimeLinhThanhResidentialShellSnapshot);
                StringAssert.Contains("ResidentialShell: district=residential", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhForgeShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("ForgeShell: district=forge", map.LinhThanhForgeShellSnapshot);
            StringAssert.Contains("anvil-row=preview-only", map.LinhThanhForgeShellSnapshot);
            StringAssert.Contains("craft-board=locked", map.LinhThanhForgeShellSnapshot);
            StringAssert.Contains("safe-no-crafting-backend", map.LinhThanhForgeShellSnapshot);
            StringAssert.Contains("ForgeShell: district=forge", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhForgeShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh forge shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("ForgeShell: district=forge", controller.RuntimeLinhThanhForgeShellSnapshot);
                StringAssert.Contains("safe-no-crafting-backend", controller.RuntimeLinhThanhForgeShellSnapshot);
                StringAssert.Contains("ForgeShell: district=forge", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhGuildShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("GuildShell: district=guild", map.LinhThanhGuildShellSnapshot);
            StringAssert.Contains("guild-hall=preview-only", map.LinhThanhGuildShellSnapshot);
            StringAssert.Contains("guild-banner=local-preview", map.LinhThanhGuildShellSnapshot);
            StringAssert.Contains("safe-no-guild-backend", map.LinhThanhGuildShellSnapshot);
            StringAssert.Contains("GuildShell: district=guild", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhGuildShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh guild shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("GuildShell: district=guild", controller.RuntimeLinhThanhGuildShellSnapshot);
                StringAssert.Contains("safe-no-guild-backend", controller.RuntimeLinhThanhGuildShellSnapshot);
                StringAssert.Contains("GuildShell: district=guild", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhHarborShell()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("HarborShell: district=harbor", map.LinhThanhHarborShellSnapshot);
            StringAssert.Contains("spirit-boat=preview-only", map.LinhThanhHarborShellSnapshot);
            StringAssert.Contains("travel-board=locked", map.LinhThanhHarborShellSnapshot);
            StringAssert.Contains("safe-no-travel-backend", map.LinhThanhHarborShellSnapshot);
            StringAssert.Contains("HarborShell: district=harbor", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhHarborShellSnapshot()
        {
            var host = new GameObject("2D Linh Thanh harbor shell snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("HarborShell: district=harbor", controller.RuntimeLinhThanhHarborShellSnapshot);
                StringAssert.Contains("safe-no-travel-backend", controller.RuntimeLinhThanhHarborShellSnapshot);
                StringAssert.Contains("HarborShell: district=harbor", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }




        [Test]
        public void PlazaHubBoardPreviewRequiresLinhThanhUnlock()
        {
            var state = CompleteDongMonFlow();

            Assert.IsTrue(state.TryInspectPlazaHubBoard());
            Assert.IsTrue(state.PlazaHubPreviewOpen);
            Assert.IsFalse(state.PlazaHubNpcPreviewOpen);
            Assert.AreEqual("board-preview-open", state.PlazaHubInteractionId);
            Assert.AreEqual("Quảng Trường", state.AreaText);
            StringAssert.Contains("Nhiệm vụ cộng đồng", state.DialogueLine);
            Assert.AreEqual("Bảng Sự Kiện", state.DialogueSpeaker);
            StringAssert.Contains("local preview", state.FeedbackText);
        }

        [Test]
        public void PlazaHubNpcPreviewRequiresLinhThanhUnlockAndStaysLocalOnly()
        {
            var locked = new TwoDOnboardingState();
            locked.Reset();

            Assert.IsFalse(locked.TryTalkPlazaGateGuide());
            Assert.IsFalse(locked.TryTalkPlazaMerchantPreview());
            Assert.IsFalse(locked.PlazaHubNpcPreviewOpen);

            var state = CompleteDongMonFlow();

            Assert.IsTrue(state.TryTalkPlazaGateGuide());
            Assert.IsTrue(state.PlazaHubNpcPreviewOpen);
            Assert.AreEqual("npc-gate-guide-preview", state.PlazaHubInteractionId);
            Assert.AreEqual("plaza-gate-guide", state.CurrentRouteNodeId);
            Assert.AreEqual("Người Giữ Cổng", state.DialogueSpeaker);
            StringAssert.Contains("Quảng Trường", state.DialogueLine);
            StringAssert.Contains("chưa mở teleport", state.HintText);

            Assert.IsTrue(state.TryTalkPlazaMerchantPreview());
            Assert.IsTrue(state.PlazaHubNpcPreviewOpen);
            Assert.AreEqual("npc-merchant-preview", state.PlazaHubInteractionId);
            Assert.AreEqual("plaza-merchant", state.CurrentRouteNodeId);
            StringAssert.Contains("Hàng tân thủ", state.DialogueLine);
            Assert.AreEqual("Thương Nhân", state.DialogueSpeaker);
            StringAssert.Contains("chưa mở mua bán", state.DialogueLine);
            StringAssert.Contains("không tạo tiền tệ", state.HintText);
        }


        [Test]
        public void PlazaHubTargetSelectorCyclesAfterUnlockAndUsesSelectedTarget()
        {
            var locked = new TwoDOnboardingState();
            locked.Reset();

            Assert.IsFalse(locked.SelectNextPlazaHubTarget());
            Assert.AreEqual("locked", locked.SelectedPlazaHubTargetId);

            var state = CompleteDongMonFlow();

            Assert.IsTrue(state.SelectNextPlazaHubTarget());
            Assert.AreEqual("event-board", state.SelectedPlazaHubTargetId);
            Assert.AreEqual("Bảng Sự Kiện", state.SelectedPlazaHubTargetLabel);
            Assert.AreEqual("target-selected-event-board", state.PlazaHubInteractionId);
            Assert.IsFalse(state.DialogueOpen);

            Assert.IsTrue(state.SelectNextPlazaHubTarget());
            Assert.AreEqual("gate-guide", state.SelectedPlazaHubTargetId);
            Assert.AreEqual("Người Giữ Cổng", state.SelectedPlazaHubTargetLabel);

            Assert.IsTrue(state.SelectNextPlazaHubTarget());
            Assert.AreEqual("merchant-preview", state.SelectedPlazaHubTargetId);
            Assert.AreEqual("Thương Nhân", state.SelectedPlazaHubTargetLabel);

            Assert.IsTrue(state.TryUseSelectedPlazaHubTarget());
            Assert.AreEqual("npc-merchant-preview", state.PlazaHubInteractionId);
            Assert.AreEqual("Thương Nhân", state.DialogueSpeaker);
            StringAssert.Contains("Hàng tân thủ", state.DialogueLine);
        }

        [Test]
        public void RuntimeMapCatalogKeepsLinhThanhPlazaHubRuntime()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("PlazaHubRuntime", map.LinhThanhPlazaHubRuntimeSnapshot);
            StringAssert.Contains("npc=gate-guide", map.LinhThanhPlazaHubRuntimeSnapshot);
            StringAssert.Contains("npc=merchant-preview", map.LinhThanhPlazaHubRuntimeSnapshot);
            StringAssert.Contains("board=event-local-preview", map.LinhThanhPlazaHubRuntimeSnapshot);
            StringAssert.Contains("guild-bulletin=locked", map.LinhThanhPlazaHubRuntimeSnapshot);
            StringAssert.Contains("safe-local-no-backend", map.LinhThanhPlazaHubRuntimeSnapshot);
            StringAssert.Contains("safe-local-no-shop-backend", map.LinhThanhPlazaHubRuntimeSnapshot);
            StringAssert.Contains("PlazaHubRuntime", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesPlazaHubRuntimeAfterUnlock()
        {
            var host = new GameObject("2D plaza hub runtime test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("PlazaHubRuntime", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("unlocked=False", controller.RuntimeLinhThanhPlazaHubSnapshot);

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();

                StringAssert.Contains("unlocked=True", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("npc=gate-guide", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("npc=merchant-preview", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("board=event-local-preview", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("safe-local-no-backend", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("safe-local-no-shop-backend", controller.RuntimeLinhThanhPlazaHubSnapshot);
                Assert.IsTrue(controller.State.TryInspectPlazaHubBoard());
                controller.RefreshForSmoke();
                StringAssert.Contains("interaction=board-preview-open", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("Khu vực: Quảng Trường", controller.WorldHudSnapshot);
                StringAssert.Contains("Bảng Sự Kiện: Nhiệm vụ cộng đồng", controller.WorldHudSnapshot);

                Assert.IsTrue(controller.State.TryTalkPlazaMerchantPreview());
                controller.RefreshForSmoke();
                StringAssert.Contains("interaction=npc-merchant-preview", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("selected=merchant-preview", controller.RuntimeLinhThanhPlazaHubSnapshot);
                StringAssert.Contains("selected=merchant-preview", controller.RuntimePlazaHubInputSnapshot);
                StringAssert.Contains("controls=P select, E interact", controller.RuntimePlazaHubInputSnapshot);
                StringAssert.Contains("layout=spaced-social-triangle", controller.RuntimePlazaHubInputSnapshot);
                StringAssert.Contains("PlazaReadability", controller.RuntimePlazaReadabilitySnapshot);
                StringAssert.Contains("mode=label-rail", controller.RuntimePlazaReadabilitySnapshot);
                StringAssert.Contains("world-label-density=reduced", controller.RuntimePlazaReadabilitySnapshot);
                StringAssert.Contains("Thương Nhân: Hàng tân thủ", controller.WorldHudSnapshot);
                StringAssert.Contains("không tạo tiền tệ", controller.WorldHudSnapshot);
                StringAssert.Contains("LINH_THANH_PLAZA_HUB_RUNTIME", controller.ProductionSceneBeatSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeControllerSupportsPlazaHubTargetInput()
        {
            var host = new GameObject("2D plaza hub target selector input test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                Assert.IsFalse(controller.SelectNextPlazaHubTarget());
                StringAssert.Contains("unlocked=False", controller.RuntimePlazaHubInputSnapshot);

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();

                Assert.IsTrue(controller.SelectNextPlazaHubTarget());
                StringAssert.Contains("selected=event-board", controller.RuntimePlazaHubInputSnapshot);
                StringAssert.Contains("Bấm P để đổi mục tiêu", controller.WorldHudSnapshot);

                Assert.IsTrue(controller.SelectNextPlazaHubTarget());
                StringAssert.Contains("selected=gate-guide", controller.RuntimePlazaHubInputSnapshot);

                Assert.IsTrue(controller.SelectNextPlazaHubTarget());
                StringAssert.Contains("selected=merchant-preview", controller.RuntimePlazaHubInputSnapshot);

                Assert.IsTrue(controller.UseSelectedPlazaHubTarget());
                StringAssert.Contains("interaction=npc-merchant-preview", controller.RuntimePlazaHubInputSnapshot);
                StringAssert.Contains("Thương Nhân: Hàng tân thủ", controller.WorldHudSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesEastGateToPlazaTransitionPreview()
        {
            var host = new GameObject("2D hub transition preview test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("HubTransition", controller.RuntimeHubTransitionSnapshot);
                StringAssert.Contains("unlocked=False", controller.RuntimeHubTransitionSnapshot);
                Assert.IsFalse(controller.PreviewEastGateToPlazaTransition());

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();

                Assert.IsTrue(controller.PreviewEastGateToPlazaTransition());
                StringAssert.Contains("HubTransition", controller.RuntimeHubTransitionSnapshot);
                StringAssert.Contains("from=east-gate", controller.RuntimeHubTransitionSnapshot);
                StringAssert.Contains("to=plaza", controller.RuntimeHubTransitionSnapshot);
                StringAssert.Contains("mode=local-route-preview", controller.RuntimeHubTransitionSnapshot);
                StringAssert.Contains("safe-local-no-teleport-backend", controller.RuntimeHubTransitionSnapshot);
                StringAssert.Contains("Tuyến Đông Môn → Quảng Trường", controller.WorldHudSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeInventoryPanelOnlyShowsWhenOpened()
        {
            var host = new GameObject("2D inventory panel visibility test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                Assert.IsFalse(controller.RuntimeInventoryPanelVisible);

                controller.ToggleInventoryPanel();
                Assert.IsTrue(controller.RuntimeInventoryPanelVisible);

                controller.CancelInventoryPreview();
                Assert.IsFalse(controller.RuntimeInventoryPanelVisible);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesLinhThanhUnlockSnapshot()
        {
            var host = new GameObject("2D Linh Thanh unlock snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("LinhThanhUnlock", controller.RuntimeLinhThanhUnlockSnapshot);
                StringAssert.Contains("unlocked=False", controller.RuntimeLinhThanhUnlockSnapshot);

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();

                StringAssert.Contains("unlocked=True", controller.RuntimeLinhThanhUnlockSnapshot);
                StringAssert.Contains("unlock=plaza", controller.RuntimeLinhThanhUnlockSnapshot);
                StringAssert.Contains("safe-local-no-teleport", controller.RuntimeLinhThanhUnlockSnapshot);
                StringAssert.Contains("Mở Linh Thành", controller.WorldHudSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeMapCatalogKeepsDongMonParallaxPolish()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("ParallaxDepth", map.ParallaxDepthSnapshot);
            StringAssert.Contains("cloud-drift", map.ParallaxDepthSnapshot);
            StringAssert.Contains("mountain-silhouette", map.ParallaxDepthSnapshot);
            StringAssert.Contains("grass-leaf-motes", map.ParallaxDepthSnapshot);
        }

        [Test]
        public void RuntimeMapCatalogKeepsTerrainCollisionBandsForDongMon()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            Assert.That(map.DongMonCollisionBands.Length, Is.GreaterThanOrEqualTo(5));
            StringAssert.Contains("Collision: Chapter 1: Vết Nứt Đông Môn", map.CollisionSnapshot);
            StringAssert.Contains("ground-main", map.CollisionSnapshot);
            StringAssert.Contains("jump-gap", map.CollisionSnapshot);
            StringAssert.Contains("dash-lane", map.CollisionSnapshot);
            StringAssert.Contains("slime-arena", map.CollisionSnapshot);
        }

        [Test]
        public void RuntimeMapCatalogKeepsDongMonTileDefinitions()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            Assert.That(map.DongMonTileDefinitions.Length, Is.GreaterThanOrEqualTo(6));
            StringAssert.Contains("Chapter 1 Tilemap", map.TilemapSnapshot);
            StringAssert.Contains("tile_ground_grass", map.TilemapSnapshot);
            StringAssert.Contains("tile_platform_wood", map.TilemapSnapshot);
            StringAssert.Contains("tile_gap_marker", map.TilemapSnapshot);
        }

        [Test]
        public void RuntimeMapCatalogKeepsDongMonTileChunks()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            Assert.That(map.DongMonTileChunks.Length, Is.GreaterThanOrEqualTo(5));
            StringAssert.Contains("ChunkFlow", map.TilemapSnapshot);
            StringAssert.Contains("chunk_gate_entry", map.TilemapSnapshot);
            StringAssert.Contains("chunk_slime_arena", map.TilemapSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesTilemapSnapshotForVisualEvidence()
        {
            var host = new GameObject("2D tilemap snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("Chapter 1 Tilemap", controller.RuntimeTilemapSnapshot);
                StringAssert.Contains("tile_ground_grass", controller.RuntimeTilemapSnapshot);
                StringAssert.Contains("tile_platform_wood", controller.RuntimeTilemapSnapshot);
                StringAssert.Contains("tile_gap_marker", controller.RuntimeTilemapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesMapSnapshotForVisualEvidence()
        {
            var host = new GameObject("2D onboarding map snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("World", controller.RuntimeMapSnapshot);
                StringAssert.Contains("Linh Thành", controller.RuntimeMapSnapshot);
                StringAssert.Contains("Đông Môn", controller.RuntimeMapSnapshot);
                StringAssert.Contains("LayerBudget", controller.RuntimeMapSnapshot);
                StringAssert.Contains("Landmarks", controller.RuntimeMapSnapshot);
                StringAssert.Contains("Cầu Gỗ", controller.RuntimeMapSnapshot);
                StringAssert.Contains("Thác Nước", controller.RuntimeMapSnapshot);
                StringAssert.Contains("Chapter 1", controller.RuntimeMapSnapshot);
                StringAssert.Contains("Mini Boss", controller.RuntimeMapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesTerrainCollisionSnapshotForVisualEvidence()
        {
            var host = new GameObject("2D terrain collision snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("Collision: Chapter 1", controller.RuntimeTerrainCollisionSnapshot);
                StringAssert.Contains("jump-gap", controller.RuntimeTerrainCollisionSnapshot);
                StringAssert.Contains("dash-lane", controller.RuntimeTerrainCollisionSnapshot);
                StringAssert.Contains("slime-arena", controller.RuntimeTerrainCollisionSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void OnboardingStateExposesCurrentRouteNodeForMapProgress()
        {
            var state = new TwoDOnboardingState();
            state.Reset();

            Assert.AreEqual("spawn", state.CurrentRouteNodeId);
            state.Move(TwoDOnboardingState.GateKeeperPosition - state.PlayerPosition);
            Assert.AreEqual("gatekeeper", state.CurrentRouteNodeId);
            state.TryUseAction();
            Assert.AreEqual("gatekeeper", state.CurrentRouteNodeId);
            state.TryUseAction();
            state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
            Assert.AreEqual("training-stone", state.CurrentRouteNodeId);
            state.TryUseAction();
            Assert.AreEqual("jump", state.CurrentRouteNodeId);
            state.TryUseJump();
            Assert.AreEqual("dash", state.CurrentRouteNodeId);
            state.TryUseDash();
            Assert.AreEqual("shadow-slime", state.CurrentRouteNodeId);
            state.TryUseClassSkill();
            Assert.AreEqual("return-gate", state.CurrentRouteNodeId);
        }

        [Test]
        public void RuntimeControllerExposesRouteProgressSnapshotForVisualEvidence()
        {
            var host = new GameObject("2D route progress snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();
                StringAssert.Contains("current=spawn", controller.RuntimeRouteProgressSnapshot);

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.RefreshForSmoke();
                StringAssert.Contains("current=gatekeeper", controller.RuntimeRouteProgressSnapshot);

                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.RefreshForSmoke();
                StringAssert.Contains("current=shadow-slime", controller.RuntimeRouteProgressSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerBuildsReadableProceduralSceneBeats()
        {
            var host = new GameObject("2D onboarding scene beat test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                Assert.That(controller.ProductionSceneBeatCount, Is.GreaterThanOrEqualTo(18));
                StringAssert.Contains("Cổng Linh Thành", controller.ProductionSceneBeatSnapshot);
                StringAssert.Contains("Lối ngọc", controller.ProductionSceneBeatSnapshot);
                StringAssert.Contains("Lồng đèn", controller.ProductionSceneBeatSnapshot);
                StringAssert.Contains("Người Giữ Cổng", controller.ProductionSceneBeatSnapshot);
                StringAssert.Contains("Bia Luyện Khí", controller.ProductionSceneBeatSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerMaintainsCameraCapturedHudText()
        {
            var host = new GameObject("2D onboarding HUD test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();
                StringAssert.Contains("Linh Giới Online", controller.WorldHudSnapshot);
                StringAssert.Contains("Tới gặp Người Giữ Cổng", controller.WorldHudSnapshot);
                Assert.That(controller.WorldHudLineCount, Is.GreaterThanOrEqualTo(5));

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.RefreshForSmoke();
                StringAssert.Contains("Người Giữ Cổng", controller.WorldHudSnapshot);
                StringAssert.Contains("Chào mừng đến Linh Thành", controller.WorldHudSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        private static TwoDOnboardingState CompleteDongMonFlow()
        {
            var state = new TwoDOnboardingState();
            state.Reset();
            state.Move(TwoDOnboardingState.GateKeeperPosition - state.PlayerPosition);
            state.TryUseAction();
            state.TryUseAction();
            state.Move(TwoDOnboardingState.TrainingStonePosition - state.PlayerPosition);
            state.TryUseAction();
            state.TryUseJump();
            state.TryUseDash();
            state.TryUseClassSkill();
            return state;
        }

    }
}
