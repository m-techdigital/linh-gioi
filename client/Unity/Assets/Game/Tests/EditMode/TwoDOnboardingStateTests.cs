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
            StringAssert.Contains("Hoàn tất nhập môn", state.ObjectiveText);
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
            StringAssert.Contains("Người Giữ Cổng", map.TutorialRouteSnapshot);
            StringAssert.Contains("Bia Luyện Khí", map.TutorialRouteSnapshot);
            StringAssert.Contains("Shadow Slime", map.TutorialRouteSnapshot);
            StringAssert.Contains("Quay về Người Giữ Cổng", map.TutorialRouteSnapshot);
            StringAssert.Contains("Mini Boss", map.TutorialRouteSnapshot);
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
                StringAssert.Contains("Chapter 1", controller.RuntimeMapSnapshot);
                StringAssert.Contains("Mini Boss", controller.RuntimeMapSnapshot);
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

    }
}
