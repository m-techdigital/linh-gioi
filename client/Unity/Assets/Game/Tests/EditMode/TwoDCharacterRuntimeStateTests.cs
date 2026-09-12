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
        public void SourcePoseInventoryKeepsQuestPotionActionUsable()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("source inventory quest test");
            try
            {
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                scene.UseCurrentRouteAction(); scene.UseCurrentRouteAction();
                foreach (var steps in new[] { 13, 25, 53, 19 })
                {
                    for (var i = 0; i < steps; i++) scene.MoveOnLane(1, .1f);
                    scene.UseCurrentRouteAction();
                    if (scene.DialogueOpen) scene.UseCurrentRouteAction();
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
                var potion = root.Query<Button>().ToList().Single(button => button.text == "Dùng Máu");
                Assert.That(potion.parent.style.display.value, Is.EqualTo(DisplayStyle.Flex),
                    "Source-pose review must not hide the inventory actions needed to complete Q04");
                InvokeBoundButton(potion);
                Assert.That(scene.PlayerHealth, Is.EqualTo(100));
                Assert.That(scene.ActiveQuestId, Is.EqualTo("Q05"));
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
