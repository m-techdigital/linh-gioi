using System.Collections.Generic;
using System.Linq;
using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests
{
    public sealed class DongMonIllustratedPreviewTests
    {
        [Test]
        public void Map01ARouteScrollsAcrossMultipleScreensWhileFeetRemainGrounded()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A route test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                var initialCameraX = Camera.main.transform.position.x;
                for (var step = 0; step < 120; step++) preview.MoveOnLane(1, .1f);

                Assert.That(preview.PlayerX, Is.GreaterThan(20f), "Map01A must span several 16:9 screens");
                Assert.That(Camera.main.transform.position.x, Is.GreaterThan(initialCameraX + 10f), "Camera must follow the route lane");
                Assert.That(preview.FootY, Is.EqualTo(preview.GroundY).Within(.001f));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AGeneratedModulesBuildReusableTerrainAndForegroundBands()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A module test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                Assert.That(GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                    .Count(renderer => renderer.name.StartsWith("Map01A authored terrain")), Is.GreaterThanOrEqualTo(10));
                Assert.That(GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                    .Count(renderer => renderer.name.StartsWith("Map01A foreground")), Is.GreaterThanOrEqualTo(4));
                var terrain = GameObject.Find("Map01A authored terrain 0").GetComponent<SpriteRenderer>();
                var foreground = GameObject.Find("Map01A foreground bamboo").GetComponent<SpriteRenderer>();
                Assert.That(terrain.sprite.texture.width, Is.EqualTo(1024));
                Assert.That(terrain.bounds.max.y, Is.EqualTo(preview.GroundY).Within(.001f));
                Assert.That(terrain.sortingOrder, Is.LessThan(0));
                Assert.That(foreground.sortingOrder, Is.GreaterThan(0));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01ABuildsSixConsistentNpcSpritesOnTheRouteGround()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A NPC test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                var npcs = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                    .Where(renderer => renderer.name.StartsWith("Map01A NPC ")).ToArray();
                Assert.That(npcs, Has.Length.EqualTo(6));
                Assert.That(npcs.Select(renderer => renderer.sprite.texture).Distinct().Count(), Is.EqualTo(1));
                Assert.That(npcs.All(renderer => Mathf.Abs(renderer.bounds.min.y - preview.GroundY) <= .001f), Is.True);
                Assert.That(npcs.Single(renderer => renderer.name == "Map01A NPC quan-thu-dong-lam").sortingOrder, Is.EqualTo(1));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01ABuildsSixRouteLandmarksBehindActors()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A landmark test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                var landmarks = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                    .Where(renderer => renderer.name.StartsWith("Map01A landmark ")).ToArray();
                Assert.That(landmarks, Has.Length.EqualTo(6));
                Assert.That(landmarks.Select(renderer => renderer.sprite.texture).Distinct().Count(), Is.EqualTo(1));
                Assert.That(landmarks.All(renderer => Mathf.Abs(renderer.bounds.min.y - preview.GroundY) <= .001f), Is.True);
                Assert.That(landmarks.All(renderer => renderer.sortingOrder < 1), Is.True, "Actors must render in front of route architecture");
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01ABuildsFourEnemiesOnlyAtCombatEdgeAndTwoWorldInteractables()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A combat art test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                var enemies = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                    .Where(renderer => renderer.name.StartsWith("Map01A enemy ")).ToArray();
                var interactables = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                    .Where(renderer => renderer.name.StartsWith("Map01A world interactable ")).ToArray();
                Assert.That(enemies, Has.Length.EqualTo(4));
                Assert.That(enemies.All(renderer => renderer.bounds.min.x >= 37f), Is.True,
                    "Beginner enemies must stay outside the village safe zone");
                Assert.That(interactables.Select(renderer => renderer.name), Is.EquivalentTo(new[] {
                    "Map01A world interactable common-chest",
                    "Map01A world interactable young-spirit-herb",
                }));
                Assert.That(enemies.Concat(interactables)
                    .All(renderer => Mathf.Abs(renderer.bounds.min.y - preview.GroundY) <= .001f), Is.True);
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01ACompletesQ01ToQ09ThroughNpcResourceCombatLootAndPortalActions()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A route action test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q01"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.DialogueOpen, Is.True);
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.HasMetHaVan, Is.True);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q02"));
                Assert.That(preview.CompletedQuestCount, Is.EqualTo(1));

                for (var step = 0; step < 13; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("grand-gate"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q03"));
                Assert.That(preview.MinimapUnlocked, Is.True);
                StringAssert.Contains("Đại Cổng", preview.MinimapRouteText);

                for (var step = 0; step < 25; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("quan-thu"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.DialogueOpen, Is.True);
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q04"));

                for (var step = 0; step < 53; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("village-square"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True, "Q04 first teaches inventory in the square");
                Assert.That(preview.InventoryOpen, Is.True);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q04"));

                for (var step = 0; step < 19; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("tong-phu"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.HasStarterSupplies, Is.True);
                Assert.That(preview.HealthPotionCount, Is.EqualTo(3));
                Assert.That(preview.ManaPotionCount, Is.EqualTo(2));
                Assert.That(preview.PlayerHealth, Is.EqualTo(60));
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q04"), "Q04 requires using a potion after receiving supplies");
                Assert.That(preview.UseHealthPotion(), Is.True);
                Assert.That(preview.HealthPotionCount, Is.EqualTo(2));
                Assert.That(preview.PlayerHealth, Is.EqualTo(100));
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q05"));

                for (var step = 0; step < 17; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("thanh-nhi"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True, "Thanh Nhi must hand off the gather objective");
                Assert.That(preview.UseCurrentRouteAction(), Is.True);

                for (var step = 0; step < 17; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("well-bridge"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.HasHarvestedSpiritHerb, Is.True);
                Assert.That(GameObject.Find("Map01A world interactable young-spirit-herb").GetComponent<SpriteRenderer>().enabled, Is.False);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q06"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.HasOpenedHiddenChest, Is.True);
                Assert.That(GameObject.Find("Map01A world interactable common-chest").GetComponent<SpriteRenderer>().color.a, Is.LessThan(1f));
                Assert.That(preview.IsQuestComplete("Q08"), Is.True, "Q08 is optional and must not block the main chain");

                for (var step = 0; step < 17; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("lao-tran"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.HasAcceptedCombatQuest, Is.True);

                for (var step = 0; step < 17; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("combat-edge"));
                for (var hit = 0; hit < 3; hit++)
                {
                    Assert.That(preview.TriggerVoSkill(), Is.True);
                    preview.AdvanceVoAnimation(.5f);
                }
                Assert.That(preview.HasDefeatedFirstEnemy, Is.True);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q07"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.HasLootedFirstEnemy, Is.True);
                Assert.That(preview.HasClassRewardItem, Is.True);
                Assert.That(preview.InventoryOpen, Is.True);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q07"), "Q07 requires equipping the class reward");
                Assert.That(preview.EquipClassReward(), Is.True);
                Assert.That(preview.IsClassRewardEquipped, Is.True);
                Assert.That(preview.ActiveQuestId, Is.EqualTo("Q09"));

                for (var step = 0; step < 17; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CurrentRouteNodeId, Is.EqualTo("portal-suoi-thanh-minh"));
                Assert.That(preview.UseCurrentRouteAction(), Is.True);
                Assert.That(preview.HasInspectedPortal, Is.True);
                Assert.That(preview.PortalUnlocked, Is.True);
                Assert.That(preview.CurrentActionLabel, Is.EqualTo("Đã mở"), "Completed portal label must fit mobile action button");
                Assert.That(preview.ActiveQuestId, Is.EqualTo("COMPLETE"));
                Assert.That(preview.CompletedQuestCount, Is.EqualTo(9));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AUsesReviewedVoAvatarAndCyclesBaseFullModular()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A Võ avatar test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                var avatar = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Where(renderer => renderer.name.StartsWith("Map01A Võ avatar ")).ToArray();
                Assert.That(avatar, Has.Length.EqualTo(96));
                Assert.That(avatar.Select(renderer => renderer.sprite.texture).Distinct().Count(), Is.EqualTo(4));
                Assert.That(avatar.All(renderer => renderer.sprite.texture.width == 1024), Is.True);
                Assert.That(preview.VoAvatarMode, Is.EqualTo("full"));
                Assert.That(avatar.Single(renderer => renderer.name == "Map01A Võ avatar lv001 male full").enabled, Is.True);
                Assert.That(avatar.Single(renderer => renderer.name == "Map01A Võ avatar lv001 male full").bounds.min.y,
                    Is.EqualTo(preview.GroundY).Within(.001f));

                preview.CycleVoAvatarMode();
                Assert.That(preview.VoAvatarMode, Is.EqualTo("base"));
                Assert.That(avatar.Single(renderer => renderer.name == "Map01A Võ avatar lv001 male base").enabled, Is.True);
                preview.CycleVoAvatarMode();
                Assert.That(preview.VoAvatarMode, Is.EqualTo("modular"));
                Assert.That(avatar.Count(renderer => renderer.enabled), Is.EqualTo(0));
                Assert.That(preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Count(renderer => renderer.enabled && renderer.name.StartsWith("Map01A Võ equipment component lv001 male ")),
                    Is.EqualTo(14));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AVoAvatarSupportsBothGendersAndTenToggleableEquipmentSlots()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A Võ two-gender equipment test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                var avatar = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Where(renderer => renderer.name.StartsWith("Map01A Võ avatar ")).ToArray();

                Assert.That(avatar, Has.Length.EqualTo(96), "four tiers of base + full + 10 slots for male and female");
                Assert.That(avatar.Select(renderer => renderer.sprite.texture).Distinct().Count(), Is.EqualTo(4));
                Assert.That(preview.VoAvatarGender, Is.EqualTo("male"));
                Assert.That(preview.VoEquippedSlotCount, Is.EqualTo(10));

                preview.CycleVoAvatarGender();
                Assert.That(preview.VoAvatarGender, Is.EqualTo("female"));
                Assert.That(avatar.Single(renderer => renderer.name == "Map01A Võ avatar lv001 female full").enabled, Is.True);

                preview.CycleVoAvatarMode();
                preview.CycleVoAvatarMode();
                Assert.That(preview.VoAvatarMode, Is.EqualTo("modular"));
                Assert.That(avatar.Count(renderer => renderer.enabled), Is.EqualTo(0));

                preview.CycleVoEquipmentSlot();
                Assert.That(preview.VoSelectedEquipmentSlot, Is.EqualTo("head_hair"));
                preview.ToggleVoEquipmentSlot();
                Assert.That(preview.VoEquippedSlotCount, Is.EqualTo(9));
                Assert.That(avatar.Single(renderer => renderer.name == "Map01A Võ avatar lv001 female slot head_hair").enabled, Is.False);
                preview.MoveOnLane(1, .1f);
                Assert.That(preview.VoAvatarUsesAlignedPaperDollMotion, Is.True);
                Assert.That(GameObject.Find("Map01A Võ motion frame").GetComponent<SpriteRenderer>().enabled, Is.False);
                Assert.That(avatar.Count(renderer => renderer.enabled), Is.EqualTo(0),
                    "modular mode must render every equipment slot through the shared bone attachment path");
                Assert.That(preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Count(renderer => renderer.enabled && renderer.name.StartsWith("Map01A Võ equipment component lv001 female ")),
                    Is.EqualTo(13));
                Assert.That(avatar.Single(renderer => renderer.name == "Map01A Võ avatar lv001 female slot head_hair").enabled, Is.False,
                    "an unequipped slot must not reappear from the full-frame sheet");
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AEveryVoEquipmentSlotTogglesAllOfItsBoneAttachments()
        {
            var host = new GameObject("Map01A Võ complete equipment matrix test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = host.AddComponent<TwoDOnboardingController>();
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                preview.CycleVoAvatarMode();
                preview.CycleVoAvatarMode();
                var slots = new[] { "main_weapon", "head_hair", "inner_top", "outer_tunic", "lower_garment",
                    "waist", "arm_guard", "boots", "light_armor", "accessory" };
                foreach (var slot in slots)
                {
                    Assert.That(preview.VoSelectedEquipmentSlot, Is.EqualTo(slot));
                    var affected = preview.GetComponentsInChildren<SpriteRenderer>(true)
                        .Count(renderer => renderer.name.StartsWith("Map01A Võ equipment component lv001 male ")
                            && renderer.name.Contains(" " + slot + " "));
                    Assert.That(affected, Is.GreaterThanOrEqualTo(1));
                    preview.ToggleVoEquipmentSlot();
                    Assert.That(preview.GetComponentsInChildren<SpriteRenderer>(true)
                        .Count(renderer => renderer.enabled
                            && renderer.name.StartsWith("Map01A Võ equipment component lv001 male ")),
                        Is.EqualTo(14 - affected));
                    preview.ToggleVoEquipmentSlot();
                    preview.CycleVoEquipmentSlot();
                }
            }
            finally
            {
                if (preview != null) UnityEngine.Object.DestroyImmediate(preview.gameObject);
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void Map01AVoModularLoadoutMixesEquipmentLevelsOnTheSharedRig()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A Võ mixed level equipment test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                preview.CycleVoAvatarMode();
                preview.CycleVoAvatarMode();
                for (var step = 0; step < 3; step++) preview.CycleVoEquipmentSlot();
                Assert.That(preview.VoSelectedEquipmentSlot, Is.EqualTo("outer_tunic"));
                for (var step = 0; step < 3; step++) preview.CycleVoSelectedEquipmentItemLevel();

                Assert.That(preview.VoAvatarLevel, Is.EqualTo(1), "Changing one item must not change the character progression preview");
                Assert.That(preview.VoSelectedEquipmentItemLevel, Is.EqualTo(30));
                Assert.That(GameObject.Find("Map01A Võ equipment component lv030 male outer_tunic center")
                    .GetComponent<SpriteRenderer>().enabled, Is.True);
                Assert.That(GameObject.Find("Map01A Võ equipment component lv001 male outer_tunic center")
                    .GetComponent<SpriteRenderer>().enabled, Is.False);
                Assert.That(GameObject.Find("Map01A Võ equipment component lv001 male waist center")
                    .GetComponent<SpriteRenderer>().enabled, Is.True, "Other slots must keep their own item levels");

                preview.SetVoRun(true);
                preview.MoveOnLane(1, .1f);
                Assert.That(preview.VoAvatarMotionState, Is.EqualTo("run"));
                Assert.That(GameObject.Find("Map01A Võ equipment component lv030 male outer_tunic center")
                    .GetComponent<SpriteRenderer>().enabled, Is.True, "Mixed-level equipment must stay attached during motion");
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AVoAvatarWalkAndSkillHaveRuntimeMotionStateAndVfx()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A Võ motion test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                Assert.That(preview.VoAvatarMotionState, Is.EqualTo("idle"));

                preview.MoveOnLane(1, .1f);
                Assert.That(preview.VoAvatarMotionState, Is.EqualTo("walk"));
                StringAssert.StartsWith("male_walk_", preview.VoAvatarMotionFrameId);
                Assert.That(preview.VoAvatarMotionScale, Is.EqualTo(Vector2.one));

                for (var step = 0; step < 175; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.CanTriggerVoSkill, Is.True);

                Assert.That(preview.TriggerVoSkill(), Is.True);
                Assert.That(preview.VoAvatarMotionState, Is.EqualTo("skill"));
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("male_lien_quyen_hit_a"));
                Assert.That(preview.VoSkillCastCount, Is.EqualTo(1));
                Assert.That(GameObject.Find("Map01A Võ skill").GetComponent<SpriteRenderer>().enabled, Is.True);
                Assert.That(preview.TriggerVoSkill(), Is.False, "Skill cannot restart during its active window");

                preview.AdvanceVoAnimation(.16f);
                Assert.That(preview.VoSkillHitCount, Is.EqualTo(1));
                Assert.That(preview.VoTrainingTargetHp, Is.EqualTo(65));
                StringAssert.Contains("-35 HP", preview.LastInteractionMessage);
                preview.AdvanceVoAnimation(.5f);
                Assert.That(preview.VoAvatarMotionState, Is.EqualTo("idle"));
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("male_idle"));
                Assert.That(GameObject.Find("Map01A Võ skill").GetComponent<SpriteRenderer>().enabled, Is.False);
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AVoLv1FullAvatarExposesRunJumpBasicAndLienQuyenKeyPoses()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A Võ extended motion test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);

                preview.SetVoRun(true);
                preview.MoveOnLane(1, .1f);
                Assert.That(preview.VoAvatarMotionState, Is.EqualTo("run"));
                StringAssert.StartsWith("male_run_", preview.VoAvatarMotionFrameId);
                preview.SetVoRun(false);
                preview.AdvanceVoAnimation(.5f);

                Assert.That(preview.TriggerVoJump(), Is.True);
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("male_jump_rise"));
                preview.MoveOnLane(1, .1f);
                Assert.That(preview.VoAvatarMotionState, Is.EqualTo("jump"), "air control must not cancel the jump clip");
                preview.AdvanceVoAnimation(.1f);
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("male_jump_apex"));
                preview.AdvanceVoAnimation(.5f);

                for (var step = 0; step < 175; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.TriggerVoBasicAttack(), Is.True);
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("male_basic_windup"));
                preview.AdvanceVoAnimation(.12f);
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("male_basic_impact"));
                preview.AdvanceVoAnimation(.4f);

                Assert.That(preview.TriggerVoSkill(), Is.True);
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("male_lien_quyen_hit_a"));
                preview.AdvanceVoAnimation(.3f);
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("male_lien_quyen_finish_b"));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AVoModularEquipmentUsesSlotSpecificPoseAttachments()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A Võ attachment profile test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                preview.CycleVoAvatarMode();
                preview.CycleVoAvatarMode();
                Assert.That(preview.VoAvatarMode, Is.EqualTo("modular"));
                var rig = preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Where(renderer => renderer.name.StartsWith("Map01A Võ rig male ")).ToArray();
                Assert.That(rig, Has.Length.EqualTo(10));
                Assert.That(rig.Count(renderer => renderer.enabled), Is.EqualTo(10));
                Assert.That(GameObject.Find("Map01A Võ avatar lv001 male base").GetComponent<SpriteRenderer>().enabled, Is.False);
                Assert.That(GameObject.Find("Map01A Võ avatar lv001 male slot arm_guard").GetComponent<SpriteRenderer>().enabled, Is.False);
                Assert.That(GameObject.Find("Map01A Võ avatar lv001 male slot boots").GetComponent<SpriteRenderer>().enabled, Is.False);
                var equipmentComponents = preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Where(renderer => renderer.name.StartsWith("Map01A Võ equipment component lv001 male ")).ToArray();
                Assert.That(equipmentComponents, Has.Length.EqualTo(14));
                Assert.That(equipmentComponents.Count(renderer => renderer.enabled), Is.EqualTo(14));

                var weapon = GameObject.Find("Map01A Võ equipment component lv001 male main_weapon center").transform;
                var boots = GameObject.Find("Map01A Võ equipment component lv001 male boots left").transform;
                var tunic = GameObject.Find("Map01A Võ equipment component lv001 male outer_tunic center").transform;
                var leftForearmBone = GameObject.Find("Map01A Võ bone male left-forearm-hand").transform;
                Assert.That(leftForearmBone.parent.name, Is.EqualTo("Map01A Võ bone male left-upper-arm"));
                Assert.That(boots.parent.name, Is.EqualTo("Map01A Võ bone male left-shin-foot"));
                var idleWeaponPosition = weapon.position;
                var idleWeaponRotation = weapon.eulerAngles.z;
                var idleBootsPosition = boots.position;
                var idleTunicRotation = tunic.eulerAngles.z;

                preview.SetVoRun(true);
                preview.MoveOnLane(1, .1f);
                Assert.That(weapon.position, Is.Not.EqualTo(idleWeaponPosition));
                Assert.That(weapon.eulerAngles.z, Is.Not.EqualTo(idleWeaponRotation));

                preview.SetVoRun(false);
                preview.AdvanceVoAnimation(.5f);
                Assert.That(preview.TriggerVoJump(), Is.True);
                Assert.That(boots.position, Is.Not.EqualTo(idleBootsPosition));

                preview.AdvanceVoAnimation(.6f);
                for (var step = 0; step < 175; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.TriggerVoBasicAttack(), Is.True);
                Assert.That(tunic.eulerAngles.z, Is.Not.EqualTo(idleTunicRotation));

                Assert.That(preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Count(renderer => renderer.enabled && renderer.name.Contains("lv001 male slot")), Is.EqualTo(0));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AVoAvatarCyclesLv1To30AndFemaleUsesRealMotionFrames()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A Võ progression test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                Assert.That(preview.VoAvatarLevel, Is.EqualTo(1));
                Assert.That(preview.VoAvatarAvailableLevels, Is.EqualTo(new[] { 1, 10, 20, 30 }));

                preview.CycleVoAvatarLevel();
                Assert.That(preview.VoAvatarLevel, Is.EqualTo(10));
                Assert.That(GameObject.Find("Map01A Võ avatar lv010 male full").GetComponent<SpriteRenderer>().enabled, Is.True);
                preview.CycleVoAvatarLevel();
                preview.CycleVoAvatarLevel();
                Assert.That(preview.VoAvatarLevel, Is.EqualTo(30));
                preview.MoveOnLane(1, .1f);
                Assert.That(preview.VoAvatarUsesAlignedPaperDollMotion, Is.True);
                Assert.That(GameObject.Find("Map01A Võ motion frame").GetComponent<SpriteRenderer>().enabled, Is.False,
                    "A higher-tier outfit must not be replaced by the Lv1 full-frame motion sheet");
                Assert.That(GameObject.Find("Map01A Võ avatar lv030 male full").GetComponent<SpriteRenderer>().enabled, Is.True);
                preview.AdvanceVoAnimation(.5f);

                preview.CycleVoAvatarGender();
                preview.CycleVoAvatarLevel();
                Assert.That(preview.VoAvatarLevel, Is.EqualTo(1));
                preview.MoveOnLane(1, .1f);
                StringAssert.StartsWith("female_walk_", preview.VoAvatarMotionFrameId);
                Assert.That(preview.VoAvatarMotionScale, Is.EqualTo(Vector2.one));
                Assert.That(GameObject.Find("Map01A Võ motion frame").GetComponent<SpriteRenderer>().enabled, Is.True);

                for (var step = 0; step < 175; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.TriggerVoSkill(), Is.True);
                Assert.That(preview.VoAvatarMotionFrameId, Is.EqualTo("female_lien_quyen_hit_a"));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AConfiguresAllTenRouteInteractionMarkers()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A marker test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                var markers = preview.GetComponentsInChildren<TextMesh>(true)
                    .Where(marker => marker.name.StartsWith("Map01A interaction ")).ToArray();
                Assert.That(markers, Has.Length.EqualTo(10));
                Assert.That(markers.Count(marker => marker.gameObject.activeSelf), Is.EqualTo(1), "Only the nearby route action should be emphasized");
                Assert.That(markers.Single(marker => marker.gameObject.activeSelf).name, Is.EqualTo("Map01A interaction spawn-ha-van"));

                for (var step = 0; step < 95; step++) preview.MoveOnLane(1, .1f);
                Assert.That(markers.Single(marker => marker.gameObject.activeSelf).name, Is.EqualTo("Map01A interaction village-square"));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AArtLoadsMovesFarLayerAndRestoresOldWorld()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A preview test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                var gate = GameObject.Find("LGO 2D Gate Roof").GetComponent<Renderer>();
                var previous = gate.enabled;
                var cameraSize = Camera.main.orthographicSize;
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                Assert.That(Camera.main.orthographicSize, Is.EqualTo(3.8f).Within(.001f),
                    "Map01A framing must keep characters readable instead of spending most of the viewport on empty sky");
                Assert.That(Camera.main.transform.position.y - preview.GroundY, Is.EqualTo(1.5f).Within(.001f));
                Assert.That(preview.PartCount, Is.EqualTo(2));
                Assert.That(preview.LayerCount, Is.EqualTo(3));
                Assert.That(preview.SourcePropCount, Is.EqualTo(4));
                var authoredDecor = preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Count(renderer => renderer.name.StartsWith("Map01A foreground "));
                Assert.That(authoredDecor, Is.GreaterThanOrEqualTo(20),
                    "The full route needs a reusable vegetation rhythm, not four isolated decorations");
                var sourceCrate = GameObject.Find("Map01A source market-crates").GetComponent<SpriteRenderer>();
                Assert.That(sourceCrate.sprite.texture.width, Is.EqualTo(256));
                Assert.That(sourceCrate.bounds.min.y, Is.EqualTo(preview.GroundY).Within(.001f));

                Assert.IsFalse(gate.enabled);
                var gateArt = GameObject.Find("Map01A grand-gate").GetComponent<SpriteRenderer>();
                Assert.That(gateArt.sprite.texture.width, Is.EqualTo(1024), "Scene atlas should exclude replaced terrain and NPC pixels");
                Assert.IsFalse(controller.enabled, "Old input must be suspended in Map01A");
                preview.MoveOnLane(-1, .1f);
                Assert.IsFalse(preview.TalkToHaVan(), "Out of range must reject talk");
                preview.MoveOnLane(1, .1f);
                Assert.IsTrue(preview.TalkToHaVan());
                var talkingX = preview.PlayerX;
                preview.MoveOnLane(1, .1f);
                Assert.That(preview.PlayerX, Is.EqualTo(talkingX), "Dialogue pauses movement");
                Assert.IsTrue(preview.TalkToHaVan());
                Assert.IsTrue(preview.HasMetHaVan);
                Assert.That(controller.State.Step, Is.EqualTo(TwoDOnboardingStep.FindGateKeeper), "Map01A talk must not trigger legacy quest");
                var before = gateArt.transform.position;
                for (var step = 0; step < 40; step++) preview.MoveOnLane(1, .1f);
                Assert.That(preview.FarOffset, Is.GreaterThan(0));
                Assert.That(preview.FootY, Is.EqualTo(preview.GroundY).Within(.001f));
                preview.Refresh();
                Assert.That(preview.FootY, Is.EqualTo(preview.GroundY).Within(.001f), "Grounding must not accumulate offsets");
                Assert.That(gateArt.transform.position, Is.EqualTo(before), "Architecture must stay fixed to ground");
                var camera = Camera.main;
                var previousAspect = camera.aspect;
                try
                {
                    foreach (var aspect in new[] { 1600f / 720f, 1024f / 768f, 1280f / 720f })
                    {
                        camera.aspect = aspect;
                        preview.Refresh();
                        var background = GameObject.Find("Map01A far-landscape").GetComponent<SpriteRenderer>().bounds;
                        var center = camera.transform.position;
                        var halfHeight = camera.orthographicSize;
                        Assert.That(background.min.x, Is.LessThanOrEqualTo(center.x - halfHeight * aspect));
                        Assert.That(background.max.x, Is.GreaterThanOrEqualTo(center.x + halfHeight * aspect));
                        Assert.That(background.min.y, Is.LessThanOrEqualTo(center.y - halfHeight));
                        Assert.That(background.max.y, Is.GreaterThanOrEqualTo(center.y + halfHeight));
                    }
                }
                finally { camera.aspect = previousAspect; }
                Object.DestroyImmediate(preview.gameObject);
                preview = null;
                Assert.That(gate.enabled, Is.EqualTo(previous));
                Assert.That(Camera.main.orthographicSize, Is.EqualTo(cameraSize));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void DraftAtlasLoadsAndPreviewPreservesInteractionAndRestoresRenderers()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Illustrated preview test");
            DongMonIllustratedPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                var gate = GameObject.Find("LGO 2D Gate Roof").GetComponent<Renderer>();
                var previous = gate.enabled;
                preview = DongMonIllustratedPreview.Attach(controller);
                Assert.That(preview.AtlasPartCount, Is.EqualTo(3));
                Assert.That(preview.GateSprite.rect.width, Is.EqualTo(1024));
                Assert.That(preview.GateSprite.texture.width, Is.EqualTo(2048));
                Assert.IsFalse(gate.enabled);
                var before = preview.FarOffset;
                controller.State.Move(TwoDOnboardingState.GateKeeperPosition + Vector2.left * 0.85f - controller.State.PlayerPosition);
                preview.Refresh();
                Assert.That(preview.FarOffset, Is.Not.EqualTo(before));
                Assert.That(controller.State.AvailableAction, Is.EqualTo(TwoDOnboardingAction.Talk));
                Assert.IsTrue(controller.State.TryUseAction());
                Assert.IsTrue(controller.State.DialogueOpen);
                Object.DestroyImmediate(preview.gameObject);
                preview = null;
                Assert.That(gate.enabled, Is.EqualTo(previous));
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }
    }
}
