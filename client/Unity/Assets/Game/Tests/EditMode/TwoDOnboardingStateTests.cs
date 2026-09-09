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
        public void RuntimeMapCatalogKeepsWorldHubAndDongMonRouteTogether()
        {
            var map = TwoDMapDesignCatalog.CreateDefault();

            Assert.That(map.WorldZones.Length, Is.GreaterThanOrEqualTo(10));
            Assert.That(map.LinhThanhDistricts.Length, Is.GreaterThanOrEqualTo(8));
            Assert.That(map.DongMonRoute.Length, Is.GreaterThanOrEqualTo(6));
            StringAssert.Contains("Linh Thành", map.WorldSnapshot);
            StringAssert.Contains("Đông Môn", map.WorldSnapshot);
            StringAssert.Contains("Người Giữ Cổng", map.TutorialRouteSnapshot);
            StringAssert.Contains("Bia Luyện Khí", map.TutorialRouteSnapshot);
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
                StringAssert.Contains("Mini Boss", controller.RuntimeMapSnapshot);
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
