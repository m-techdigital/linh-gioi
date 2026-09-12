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

                InvokeBoundButton(infoTab);
                Assert.That(root.Q("Map01A Inventory Grid Panel").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Inventory Character Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q("Map01A Inventory Detail Panel").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(body.IndexOf(root.Q("Map01A Inventory Detail Panel")), Is.GreaterThan(body.IndexOf(root.Q("Map01A Inventory Character Panel"))),
                    "Item detail must stay on the right side of character equipment slots.");

                InvokeBoundButton(root.Q<Button>("LGO Equipment Inventory Slot boots"));
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("boots"));
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
                Assert.That(root.Q<Label>("Map01A Storage State").text, Does.Contain("chưa kết nối"));
                Assert.That(root.Q<Button>("Map01A Storage Deposit").enabledSelf, Is.False);
                Assert.That(root.Q<Button>("Map01A Storage Withdraw").enabledSelf, Is.False);
                Assert.That(scene.VoSelectedEquipmentSlot, Is.EqualTo("boots"));
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
                Assert.That(root.Q("Map01A Inventory Footer").style.display.value, Is.EqualTo(DisplayStyle.None));
                var potion = root.Q<Button>("Map01A Health Potion");
                Assert.That(potion, Is.Not.Null);
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
                Assert.That(root.Q<UnityEngine.UIElements.ProgressBar>("Map01A Health").value, Is.EqualTo(60));
                Assert.That(root.Q<UnityEngine.UIElements.ProgressBar>("Map01A Mana").value, Is.EqualTo(50));
                Assert.That(root.Q<Button>("Map01A Inventory Gender").enabledSelf, Is.False,
                    "A missing female pack must not offer a renderer fallback");
                scene.ToggleInventory(); update.Invoke(hud, null);
                Assert.That(root.Q("Map01A Vitals").style.display.value, Is.EqualTo(DisplayStyle.None));
                scene.ToggleInventory(); scene.TalkToHaVan(); update.Invoke(hud, null);
                Assert.That(root.Q("Map01A Vitals").style.display.value, Is.EqualTo(DisplayStyle.None));
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
