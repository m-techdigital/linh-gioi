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
            Assert.IsNotNull(resultType.GetField("runtimeDongMonAuthoredDetailSourceSnapshot"));
            Assert.IsNotNull(resultType.GetField("runtimeDongMonNpcSpriteSourceSnapshot"));
            Assert.IsNotNull(resultType.GetField("runtimeDongMonInteractionMarkerSourceSnapshot"));
            Assert.IsNotNull(resultType.GetField("runtimeDongMonPlayerSceneFitSnapshot"));
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
        public void VoLv1ClassSliceLoadsPaperDollMotionAndSkillFromRuntimeResource()
        {
            var source = TwoDClassSliceCatalog.LoadVoLv1ClassSlice();
            var snapshot = TwoDClassSliceCatalog.LoadVoLv1ClassSliceSnapshot();

            Assert.IsNotNull(source);
            Assert.AreEqual("vo", source.classId);
            Assert.That(source.slots.Length, Is.EqualTo(5));
            Assert.That(source.motions.Length, Is.EqualTo(5));
            Assert.AreEqual("vo_lv1_first_skill", source.skill.id);
            Assert.AreEqual("Hand_R", source.skill.anchor);
            StringAssert.Contains("VoLv1ClassSlice", snapshot);
            StringAssert.Contains("resource=LGOClasses/VoLv1ClassSlice", snapshot);
            StringAssert.Contains("classId=vo", snapshot);
            StringAssert.Contains("OuterShirt:top_vo_lv1_male@Chest", snapshot);
            StringAssert.Contains("Gloves:gloves_vo_lv1_unisex@Hand_L,Hand_R", snapshot);
            StringAssert.Contains("ClassSkill:vo_lv1_first_skill", snapshot);
            StringAssert.Contains("skillAnchor=Hand_R", snapshot);
            StringAssert.Contains("safe-no-source-image=True", snapshot);
            StringAssert.Contains("safe-no-3d=True", snapshot);
        }

        [Test]
        public void RuntimeControllerExposesVoLv1ClassSliceWithEquippedMotionAndSkillCue()
        {
            var host = new GameObject("2D Vo Lv1 class slice runtime test host");
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

                StringAssert.Contains("VoLv1ClassSlice", controller.RuntimeVoLv1ClassSliceSnapshot);
                StringAssert.Contains("classId=vo", controller.RuntimeVoLv1ClassSliceSnapshot);
                StringAssert.Contains("slots=5", controller.RuntimeVoLv1ClassSliceSnapshot);
                StringAssert.Contains("top=top_vo_lv1_male", controller.RuntimeVoLv1ClassSliceSnapshot);
                StringAssert.Contains("runtimeAnimation=animation=TrainingCompletePose", controller.RuntimeVoLv1ClassSliceSnapshot);
                StringAssert.Contains("runtimeSkillCue=True", controller.RuntimeVoLv1ClassSliceSnapshot);
                Assert.IsNotNull(GameObject.Find("LGO 2D Vo Lv1 Skill Runtime Cue"));
                Assert.IsNotNull(GameObject.Find("LGO 2D Vo Lv1 Skill Runtime Cue Impact Palm"));
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
        public void RuntimeControllerExposesSelectedPlazaHubDetailForVisualEvidence()
        {
            var host = new GameObject("2D plaza selected detail test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("PlazaHubDetail", controller.RuntimePlazaHubDetailSnapshot);
                StringAssert.Contains("locked-until-unlock", controller.RuntimePlazaHubDetailSnapshot);

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
                Assert.IsTrue(controller.SelectNextPlazaHubTarget());
                Assert.IsTrue(controller.SelectNextPlazaHubTarget());
                Assert.IsTrue(controller.UseSelectedPlazaHubTarget());

                StringAssert.Contains("PlazaHubDetail", controller.RuntimePlazaHubDetailSnapshot);
                StringAssert.Contains("selected=merchant-preview", controller.RuntimePlazaHubDetailSnapshot);
                StringAssert.Contains("role=starter-gear-preview", controller.RuntimePlazaHubDetailSnapshot);
                StringAssert.Contains("detail=try-before-shop", controller.RuntimePlazaHubDetailSnapshot);
                StringAssert.Contains("safe-no-shop-backend", controller.RuntimePlazaHubDetailSnapshot);
                StringAssert.Contains("safe-local-no-backend", controller.RuntimePlazaHubDetailSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesPlazaHubLayoutAnchorsForVisualEvidence()
        {
            var host = new GameObject("2D plaza layout anchor test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("PlazaHubLayout", controller.RuntimePlazaHubLayoutSnapshot);
                StringAssert.Contains("anchor=social-spawn@center", controller.RuntimePlazaHubLayoutSnapshot);
                StringAssert.Contains("anchor=event-board@upper-mid", controller.RuntimePlazaHubLayoutSnapshot);
                StringAssert.Contains("anchor=gate-guide@left", controller.RuntimePlazaHubLayoutSnapshot);
                StringAssert.Contains("anchor=merchant-preview@right", controller.RuntimePlazaHubLayoutSnapshot);
                StringAssert.Contains("anchor=guild-locked@far-right", controller.RuntimePlazaHubLayoutSnapshot);
                StringAssert.Contains("safe-local-no-backend", controller.RuntimePlazaHubLayoutSnapshot);
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
        public void RuntimeMapCatalogKeepsDongMonAuthoredPass()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            StringAssert.Contains("DongMonAuthoredPass", map.DongMonAuthoredPassSnapshot);
            StringAssert.Contains("route-segments=5", map.DongMonAuthoredPassSnapshot);
            StringAssert.Contains("detail-density=readable", map.DongMonAuthoredPassSnapshot);
            StringAssert.Contains("collision-boundaries=from-bands", map.DongMonAuthoredPassSnapshot);
            StringAssert.Contains("no-random-decoration", map.DongMonAuthoredPassSnapshot);
            StringAssert.Contains("DongMonAuthoredPass", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeMapCatalogKeepsDongMonTilePalette()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            Assert.That(map.DongMonTilePalette.Length, Is.GreaterThanOrEqualTo(6));
            StringAssert.Contains("DongMonTilePalette", map.DongMonTilePaletteSnapshot);
            StringAssert.Contains("tile_ground_grass:earth-green:soft-grass-edge", map.DongMonTilePaletteSnapshot);
            StringAssert.Contains("tile_ground_stone:spirit-cyan:stone-step-line", map.DongMonTilePaletteSnapshot);
            StringAssert.Contains("tile_platform_wood:warm-wood:rope-rail", map.DongMonTilePaletteSnapshot);
            StringAssert.Contains("tile_slime_arena:violet-corruption:rune-boundary", map.DongMonTilePaletteSnapshot);
            StringAssert.Contains("safe-no-source-image", map.DongMonTilePaletteSnapshot);
            StringAssert.Contains("DongMonTilePalette", map.RuntimeSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesDongMonTilePaletteForVisualEvidence()
        {
            var host = new GameObject("2D Dong Mon tile palette snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonTilePalette", controller.RuntimeDongMonTilePaletteSnapshot);
                StringAssert.Contains("tile_dash_lane:spirit-cyan:wind-streak", controller.RuntimeDongMonTilePaletteSnapshot);
                StringAssert.Contains("safe-no-source-image", controller.RuntimeDongMonTilePaletteSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeMapCatalogLoadsDongMonTilePaletteSourceAsset()
        {
            var sourceSnapshot = TwoDMapDesignCatalog.LoadDongMonTilePaletteSourceSnapshot();

            StringAssert.Contains("DongMonTilePaletteSource", sourceSnapshot);
            StringAssert.Contains("resource=LGOMaps/DongMonTilePalette", sourceSnapshot);
            StringAssert.Contains("tile_dash_lane", sourceSnapshot);
            StringAssert.Contains("safe-no-source-image", sourceSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesDongMonTilePaletteSourceForVisualEvidence()
        {
            var host = new GameObject("2D Dong Mon tile palette source snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonTilePaletteSource", controller.RuntimeDongMonTilePaletteSourceSnapshot);
                StringAssert.Contains("resource=LGOMaps/DongMonTilePalette", controller.RuntimeDongMonTilePaletteSourceSnapshot);
                StringAssert.Contains("safe-runtime-resource", controller.RuntimeDongMonTilePaletteSourceSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeMapCatalogLoadsDongMonAuthoredChunkPlacementSourceAsset()
        {
            var sourceSnapshot = TwoDMapDesignCatalog.LoadDongMonChunkPlacementSourceSnapshot();

            StringAssert.Contains("DongMonChunkPlacementSource", sourceSnapshot);
            StringAssert.Contains("resource=LGOMaps/DongMonChunkPlacement", sourceSnapshot);
            StringAssert.Contains("chunk_gate_entry@-3.70,-2.02x4", sourceSnapshot);
            StringAssert.Contains("chunk_dash_lane@1.82,-1.02x4", sourceSnapshot);
            StringAssert.Contains("authored-placement", sourceSnapshot);
            StringAssert.Contains("safe-runtime-resource", sourceSnapshot);
        }

        [Test]
        public void RuntimeControllerExposesDongMonAuthoredChunkPlacementForVisualEvidence()
        {
            var host = new GameObject("2D Dong Mon authored chunk placement source snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonChunkPlacementSource", controller.RuntimeDongMonChunkPlacementSourceSnapshot);
                StringAssert.Contains("chunk_slime_arena@3.25,-1.36x2", controller.RuntimeDongMonChunkPlacementSourceSnapshot);
                StringAssert.Contains("safe-no-3d", controller.RuntimeDongMonChunkPlacementSourceSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }



        [Test]
        public void RuntimeMapCatalogLoadsDongMonNpcSpriteSourceAsset()
        {
            var npcs = TwoDMapDesignCatalog.LoadDongMonNpcSprites();
            var gateKeeper = TwoDMapDesignCatalog.LoadDongMonNpcSprite("gate_keeper");
            var sourceSnapshot = TwoDMapDesignCatalog.LoadDongMonNpcSpriteSourceSnapshot();

            Assert.That(npcs.Length, Is.EqualTo(1));
            Assert.IsNotNull(gateKeeper);
            Assert.That(gateKeeper.parts.Length, Is.EqualTo(13));
            StringAssert.Contains("DongMonNpcSpriteSource", sourceSnapshot);
            StringAssert.Contains("resource=LGOMaps/DongMonNpcSprites", sourceSnapshot);
            StringAssert.Contains("npcs=1", sourceSnapshot);
            StringAssert.Contains("gate_keeper_parts=13", sourceSnapshot);
            StringAssert.Contains("role=tutorial-guide", sourceSnapshot);
            StringAssert.Contains("silhouette=elder-robed-guardian-staff", sourceSnapshot);
            StringAssert.Contains("slots=robe,cloak,hat,staff,talisman", sourceSnapshot);
            StringAssert.Contains("style=primitive-shape-polish-v1", sourceSnapshot);
            Assert.That(gateKeeper.parts[0].shape, Is.EqualTo("ellipse"));
            StringAssert.Contains("authored-npc-sprite=True", sourceSnapshot);
            StringAssert.Contains("safe-runtime-resource=True", sourceSnapshot);
            StringAssert.Contains("safe-no-source-image=True", sourceSnapshot);
            StringAssert.Contains("safe-no-3d=True", sourceSnapshot);
            StringAssert.Contains("safe-local-no-backend=True", sourceSnapshot);
        }

        [Test]
        public void RuntimeControllerBuildsGateKeeperNpcFromSpriteResourceForVisualEvidence()
        {
            var host = new GameObject("2D Dong Mon gate keeper npc sprite source snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonNpcSpriteSource", controller.RuntimeDongMonNpcSpriteSourceSnapshot);
                StringAssert.Contains("gate_keeper_parts=13", controller.RuntimeDongMonNpcSpriteSourceSnapshot);
                StringAssert.Contains("safe-no-source-image=True", controller.RuntimeDongMonNpcSpriteSourceSnapshot);

                var npc = TwoDMapDesignCatalog.LoadDongMonNpcSprite("gate_keeper");
                foreach (var part in npc.parts)
                {
                    var renderer = GameObject.Find("LGO 2D Gate Keeper SpritePart " + part.id + " " + part.slot)
                        .GetComponent<SpriteRenderer>();
                    Assert.That(renderer.transform.localScale.x, Is.EqualTo(part.w));
                    Assert.That(renderer.transform.localScale.y, Is.EqualTo(part.h));
                    if (part.shape != "rect")
                        Assert.That(renderer.sprite.name, Is.EqualTo("LGO 2D Shape Sprite " + part.shape));
                }
                Assert.IsNotNull(GameObject.Find("LGO 2D Gate Keeper SpritePart staff staff"));
                Assert.IsNotNull(GameObject.Find("LGO 2D Gate Keeper SpritePart jade_talisman talisman"));
                Assert.IsNotNull(GameObject.Find("LGO 2D Gate Keeper SpritePart hat_brim hat"));
                StringAssert.Contains("style=primitive-shape-polish-v1", controller.RuntimeDongMonNpcSpriteSourceSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void NpcShapeSpritesPreserveFallbackSizeAndReuse()
        {
            var method = typeof(TwoDOnboardingController).GetMethod("ShapeSprite",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(method);
            var rectangle = (Sprite)method.Invoke(null, new object[] { "rect" });
            foreach (var shape in new[] { null, "", "unknown-shape" })
                Assert.AreSame(rectangle, method.Invoke(null, new object[] { shape }), "Fallback: " + shape);
            foreach (var shape in new[] { "ellipse", "diamond", "tapered" })
            {
                var sprite = (Sprite)method.Invoke(null, new object[] { shape });
                Assert.AreNotSame(rectangle, sprite);
                Assert.That(sprite.bounds.size.x, Is.EqualTo(1f).Within(0.001f));
                Assert.That(sprite.bounds.size.y, Is.EqualTo(1f).Within(0.001f));
                Assert.AreSame(sprite, method.Invoke(null, new object[] { shape }));
            }
        }

        [Test]
        public void RuntimeMapCatalogLoadsDongMonAuthoredDetailsSourceAsset()
        {
            var details = TwoDMapDesignCatalog.LoadDongMonAuthoredDetails();
            var sourceSnapshot = TwoDMapDesignCatalog.LoadDongMonAuthoredDetailsSourceSnapshot();

            Assert.That(details.Length, Is.EqualTo(7));
            StringAssert.Contains("DongMonAuthoredDetailSource", sourceSnapshot);
            StringAssert.Contains("resource=LGOMaps/DongMonAuthoredDetails", sourceSnapshot);
            StringAssert.Contains("details=7", sourceSnapshot);
            StringAssert.Contains("moss=True", sourceSnapshot);
            StringAssert.Contains("step=True", sourceSnapshot);
            StringAssert.Contains("rope=True", sourceSnapshot);
            StringAssert.Contains("spirit-dust=True", sourceSnapshot);
            StringAssert.Contains("rune=True", sourceSnapshot);
            StringAssert.Contains("authored-detail=True", sourceSnapshot);
            StringAssert.Contains("safe-no-source-image=True", sourceSnapshot);
            StringAssert.Contains("safe-runtime-resource=True", sourceSnapshot);
            StringAssert.Contains("safe-no-3d=True", sourceSnapshot);
        }

        [Test]
        public void RuntimeControllerBuildsDongMonAuthoredDetailsFromResourceForVisualEvidence()
        {
            var host = new GameObject("2D Dong Mon authored details source snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonAuthoredDetailSource", controller.RuntimeDongMonAuthoredDetailSourceSnapshot);
                StringAssert.Contains("details=7", controller.RuntimeDongMonAuthoredDetailSourceSnapshot);
                StringAssert.Contains("safe-no-source-image=True", controller.RuntimeDongMonAuthoredDetailSourceSnapshot);

                var detailObject = GameObject.Find("LGO 2D Authored Detail detail_bridge_rope rope");
                Assert.IsNotNull(detailObject);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerBuildsDongMonUnityTilemapLayerForVisualEvidence()
        {
            var host = new GameObject("2D Dong Mon Unity Tilemap test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonUnityTilemap", controller.RuntimeDongMonUnityTilemapSnapshot);
                StringAssert.Contains("renderer=TilemapRenderer", controller.RuntimeDongMonUnityTilemapSnapshot);
                StringAssert.Contains("cells=16", controller.RuntimeDongMonUnityTilemapSnapshot);
                StringAssert.Contains("source=LGOMaps/DongMonChunkPlacement", controller.RuntimeDongMonUnityTilemapSnapshot);
                StringAssert.Contains("safe-no-source-image", controller.RuntimeDongMonUnityTilemapSnapshot);

                var tilemapObject = GameObject.Find("LGO 2D Dong Mon Unity Tilemap");
                Assert.IsNotNull(tilemapObject);
                Assert.IsNotNull(tilemapObject.GetComponent("Tilemap"));
                Assert.IsNotNull(tilemapObject.GetComponent("TilemapRenderer"));
                Assert.IsNotNull(tilemapObject.GetComponentInParent<Grid>());
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesDongMonLabelReadabilityRailForVisualEvidence()
        {
            var host = new GameObject("2D Dong Mon label readability rail test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonReadability", controller.RuntimeDongMonReadabilitySnapshot);
                StringAssert.Contains("mode=route-label-rail", controller.RuntimeDongMonReadabilitySnapshot);
                StringAssert.Contains("world-label-density=reduced", controller.RuntimeDongMonReadabilitySnapshot);
                StringAssert.Contains("chips=gate,stone,jump,dash,slime", controller.RuntimeDongMonReadabilitySnapshot);
                StringAssert.Contains("avoids-hud-overlap", controller.RuntimeDongMonReadabilitySnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesDongMonAuthoredPassForVisualEvidence()
        {
            var host = new GameObject("2D Dong Mon authored pass snapshot test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonAuthoredPass", controller.RuntimeDongMonAuthoredPassSnapshot);
                StringAssert.Contains("detail-density=readable", controller.RuntimeDongMonAuthoredPassSnapshot);
                StringAssert.Contains("collision-boundaries=from-bands", controller.RuntimeDongMonAuthoredPassSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void DongMonInteractionMarkersLoadFromRuntimeResource()
        {
            var markers = TwoDMapDesignCatalog.LoadDongMonInteractionMarkers();
            var sourceSnapshot = TwoDMapDesignCatalog.LoadDongMonInteractionMarkerSourceSnapshot();

            Assert.That(markers.Length, Is.EqualTo(5));
            Assert.AreEqual("marker_gate_keeper", markers[0].id);
            Assert.AreEqual("Talk", markers[0].action);
            Assert.AreEqual("gatekeeper", markers[0].routeNodeId);
            StringAssert.Contains("DongMonInteractionMarkerSource", sourceSnapshot);
            StringAssert.Contains("resource=LGOMaps/DongMonInteractionMarkers", sourceSnapshot);
            StringAssert.Contains("markers=5", sourceSnapshot);
            StringAssert.Contains("marker_training_stone", sourceSnapshot);
            StringAssert.Contains("marker_shadow_slime", sourceSnapshot);
            StringAssert.Contains("safe-runtime-resource=True", sourceSnapshot);
            StringAssert.Contains("safe-no-source-image=True", sourceSnapshot);
        }


        [Test]
        public void RuntimeControllerExposesDongMonPlayerSceneFitForPcGroundingEvidence()
        {
            var host = new GameObject("2D Dong Mon player scene fit test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonPlayerSceneFit", controller.RuntimeDongMonPlayerSceneFitSnapshot);
                StringAssert.Contains("groundBandY=-1.58..-0.78", controller.RuntimeDongMonPlayerSceneFitSnapshot);
                StringAssert.Contains("footAnchor=bottom-center", controller.RuntimeDongMonPlayerSceneFitSnapshot);
                StringAssert.Contains("contactShadow=LayeredCharacter Shadow Slot Shadow", controller.RuntimeDongMonPlayerSceneFitSnapshot);
                StringAssert.Contains("playerSortOrder=2", controller.RuntimeDongMonPlayerSceneFitSnapshot);
                StringAssert.Contains("routeAnchors=gatekeeper,training-stone,jump,dash,shadow-slime", controller.RuntimeDongMonPlayerSceneFitSnapshot);
                StringAssert.Contains("safe-runtime-player-evidence=True", controller.RuntimeDongMonPlayerSceneFitSnapshot);

                Assert.IsNotNull(GameObject.Find("LGO 2D Player"));
                Assert.IsNotNull(GameObject.Find("LGO 2D Player LayeredCharacter Shadow Slot Shadow"));
                Assert.IsNotNull(GameObject.Find("LGO 2D Dong Mon Unity Tilemap"));
                StringAssert.Contains("cells=16", controller.RuntimeDongMonUnityTilemapSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerRendersDongMonInteractionMarkersForPlayerEvidence()
        {
            var host = new GameObject("2D Dong Mon interaction marker test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DongMonInteractionMarkerSource", controller.RuntimeDongMonInteractionMarkerSourceSnapshot);
                StringAssert.Contains("markers=5", controller.RuntimeDongMonInteractionMarkerSourceSnapshot);
                StringAssert.Contains("route=gatekeeper,training-stone,jump,dash,shadow-slime", controller.RuntimeDongMonInteractionMarkerSourceSnapshot);
                StringAssert.Contains("safe-no-3d=True", controller.RuntimeDongMonInteractionMarkerSourceSnapshot);

                Assert.IsNotNull(GameObject.Find("LGO 2D Interaction Marker marker_gate_keeper Ring"));
                Assert.IsNotNull(GameObject.Find("LGO 2D Interaction Marker marker_training_stone Ring"));
                Assert.IsNotNull(GameObject.Find("LGO 2D Interaction Marker marker_shadow_slime Ring"));
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


        [Test]
        public void RuntimeControllerExposesLinhThanhDistrictPreviewRailAfterUnlock()
        {
            var host = new GameObject("2D Linh Thanh district preview rail test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DistrictPreviewRail", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("locked-until-unlock", controller.RuntimeLinhThanhDistrictPreviewSnapshot);

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                StringAssert.Contains("DistrictPreviewRail", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("selected=academy", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("label=Học Viện", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("route=plaza->academy", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("safe-no-skill-backend", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("safe-no-district-backend", controller.RuntimeLinhThanhDistrictPreviewSnapshot);

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                StringAssert.Contains("selected=market", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("safe-no-trade-backend", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeControllerExposesLinhThanhDistrictDetailForAcademyAndMarket()
        {
            var host = new GameObject("2D Linh Thanh district detail test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                controller.RefreshForSmoke();

                StringAssert.Contains("DistrictDetail", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("locked-until-unlock", controller.RuntimeLinhThanhDistrictDetailSnapshot);

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                StringAssert.Contains("DistrictDetail", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("selected=academy", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("role=skill-learning-preview", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("detail=class-trainer-locked", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("safe-no-skill-backend", controller.RuntimeLinhThanhDistrictDetailSnapshot);

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                StringAssert.Contains("selected=market", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("role=starter-commerce-preview", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("detail=vendor-row-only", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("safe-no-trade-backend", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("safe-no-economy-backend", controller.RuntimeLinhThanhDistrictDetailSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }


        [Test]
        public void RuntimeControllerCyclesDistrictPreviewToSpiritTempleWithStoryGuard()
        {
            var host = new GameObject("2D Linh Thanh spirit temple preview test host");
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

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());

                StringAssert.Contains("DistrictPreviewRail", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("selected=spirit-temple", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("label=Đền Linh", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("route=plaza->spirit-temple", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("safe-no-buff-backend", controller.RuntimeLinhThanhDistrictPreviewSnapshot);

                StringAssert.Contains("DistrictDetail", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("selected=spirit-temple", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("role=story-blessing-preview", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("detail=altar-local-only", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("next=quest-buff-gate", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("safe-no-buff-backend", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("safe-no-district-backend", controller.RuntimeLinhThanhDistrictDetailSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerCyclesDistrictPreviewThroughForgeGuildAndHarborWithLockedGuards()
        {
            var host = new GameObject("2D Linh Thanh full district rail preview test host");
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

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                StringAssert.Contains("selected=forge", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("label=Khu Rèn", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("route=plaza->forge", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("safe-no-crafting-backend", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("role=crafting-preview", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("detail=anvil-row-only", controller.RuntimeLinhThanhDistrictDetailSnapshot);

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                StringAssert.Contains("selected=guild", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("label=Khu Bang Hội", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("route=plaza->guild", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("safe-no-guild-backend", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("role=social-guild-preview", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("detail=notice-board-locked", controller.RuntimeLinhThanhDistrictDetailSnapshot);

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                StringAssert.Contains("selected=harbor", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("label=Cảng Linh Thuyền", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("route=plaza->harbor", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("safe-no-travel-backend", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("safe-no-teleport-backend", controller.RuntimeLinhThanhDistrictPreviewSnapshot);
                StringAssert.Contains("role=travel-preview", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("detail=spirit-boat-locked", controller.RuntimeLinhThanhDistrictDetailSnapshot);
                StringAssert.Contains("next=world-route-gate", controller.RuntimeLinhThanhDistrictDetailSnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesDistrictRailReadableSelectedNodeCallout()
        {
            var host = new GameObject("2D Linh Thanh district rail readability test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                StringAssert.Contains("DistrictRailReadability", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
                StringAssert.Contains("locked-until-unlock", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);

                controller.State.Move(TwoDOnboardingState.GateKeeperPosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseAction();
                controller.State.Move(TwoDOnboardingState.TrainingStonePosition - controller.State.PlayerPosition);
                controller.State.TryUseAction();
                controller.State.TryUseJump();
                controller.State.TryUseDash();
                controller.State.TryUseClassSkill();
                controller.RefreshForSmoke();

                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());
                Assert.IsTrue(controller.SelectNextLinhThanhDistrictPreview());

                StringAssert.Contains("DistrictRailReadability", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
                StringAssert.Contains("mode=selected-node-callout", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
                StringAssert.Contains("selected=harbor", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
                StringAssert.Contains("label-follows-selected=True", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
                StringAssert.Contains("backplate=follows-selected", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
                StringAssert.Contains("callout-size=readable", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
                StringAssert.Contains("avoids-hud-overlap", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
                StringAssert.Contains("safe-local-no-backend", controller.RuntimeLinhThanhDistrictReadabilitySnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void RuntimeControllerExposesCompactMinimapReadabilitySnapshot()
        {
            var host = new GameObject("2D compact minimap readability test host");
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                StringAssert.Contains("MinimapReadability", controller.RuntimeMinimapReadabilitySnapshot);
                StringAssert.Contains("mode=compact-district-route", controller.RuntimeMinimapReadabilitySnapshot);
                StringAssert.Contains("route-text=short", controller.RuntimeMinimapReadabilitySnapshot);
                StringAssert.Contains("district-chips=academy,market,spirit,forge,guild,harbor", controller.RuntimeMinimapReadabilitySnapshot);
                StringAssert.Contains("world-links=icon-only", controller.RuntimeMinimapReadabilitySnapshot);
                StringAssert.Contains("selected-node-progress", controller.RuntimeMinimapReadabilitySnapshot);
                StringAssert.Contains("avoids-hud-overlap", controller.RuntimeMinimapReadabilitySnapshot);
                StringAssert.Contains("safe-local-no-backend", controller.RuntimeMinimapReadabilitySnapshot);
            }
            finally
            {
                Object.DestroyImmediate(host);
            }
        }

    }
}
