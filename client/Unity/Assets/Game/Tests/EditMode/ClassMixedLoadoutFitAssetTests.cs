using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests.EditMode
{
    public sealed class ClassMixedLoadoutFitAssetTests
    {
        private const string KiemResource = "LGOClasses/KiemMixedLoadoutFitPreview/";
        private const string PhapResource = "LGOClasses/PhapMixedLoadoutFitPreview/";
        private const string CoResource = "LGOClasses/CoMixedLoadoutFitPreview/";
        private const string LinhResource = "LGOClasses/LinhMixedLoadoutFitPreview/";

        [Test]
        public void ReviewPackUsesTwoBoundedAtlasesForAllFourLevelsAndBothGenders()
        {
            var male = Resources.Load<Texture2D>(KiemResource + "kiem-equipment-male-atlas");
            var female = Resources.Load<Texture2D>(KiemResource + "kiem-equipment-female-atlas");
            Assert.That(male, Is.Not.Null);
            Assert.That(female, Is.Not.Null);
            Assert.That(male.width, Is.EqualTo(1024));
            Assert.That(male.height, Is.EqualTo(1024));
            Assert.That(female.width, Is.EqualTo(1024));
            Assert.That(female.height, Is.EqualTo(1024));
            var phapMale = Resources.Load<Texture2D>(PhapResource + "phap-equipment-male-atlas");
            var phapFemale = Resources.Load<Texture2D>(PhapResource + "phap-equipment-female-atlas");
            Assert.That(phapMale, Is.Not.Null);
            Assert.That(phapFemale, Is.Not.Null);
            Assert.That(phapMale.width, Is.EqualTo(1024));
            Assert.That(phapFemale.height, Is.EqualTo(1024));
            Assert.That(Resources.Load<Texture2D>(CoResource + "co-equipment-male-atlas"), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(CoResource + "co-equipment-female-atlas"), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(LinhResource + "linh-equipment-male-atlas"), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(LinhResource + "linh-equipment-female-atlas"), Is.Not.Null);
        }

        [Test]
        public void PreviewUsesOneSharedRigWithTenInteractiveSlotsAndCrossLevelItems()
        {
            var root = new GameObject("Kiếm preview test root");
            TwoDClassMixedLoadoutFitPreview preview = null;
            try
            {
                var rig = new TwoDSkeletalPaperDollRig(root.transform);
                rig.Build(new[]
                {
                    Bone("male_torso-hips", "", 0, .7f), Bone("male_head", "male_torso-hips", 0, 1.32f),
                    Bone("male_left-thigh", "male_torso-hips", -.11f, .58f),
                    Bone("male_right-thigh", "male_torso-hips", .11f, .58f),
                    Bone("male_left-forearm-hand", "male_torso-hips", -.25f, .82f),
                    Bone("male_right-forearm-hand", "male_torso-hips", .25f, .82f),
                    Bone("male_left-shin-foot", "male_left-thigh", -.11f, .2f),
                    Bone("male_right-shin-foot", "male_right-thigh", .11f, .2f),
                    Bone("female_torso-hips", "", 0, .7f), Bone("female_head", "female_torso-hips", 0, 1.32f),
                    Bone("female_left-thigh", "female_torso-hips", -.11f, .58f),
                    Bone("female_right-thigh", "female_torso-hips", .11f, .58f),
                    Bone("female_left-forearm-hand", "female_torso-hips", -.25f, .82f),
                    Bone("female_right-forearm-hand", "female_torso-hips", .25f, .82f),
                    Bone("female_left-shin-foot", "female_left-thigh", -.11f, .2f),
                    Bone("female_right-shin-foot", "female_right-thigh", .11f, .2f)
                });
                preview = new TwoDClassMixedLoadoutFitPreview(root.transform, rig, "kiem");
                preview.SetActive(true, "male", "run");
                Assert.That(preview.VisibleSlotCount, Is.EqualTo(10));
                Assert.That(preview.VisibleComponentCount, Is.EqualTo(13));
                Assert.That(preview.AvailableLevels, Is.EqualTo(new[] { 1, 10, 20, 30 }));
                StringAssert.Contains("slots=10/10", preview.Snapshot);
                StringAssert.Contains("sharedSkeleton=lgo_humanoid_2d_v1", preview.Snapshot);

                preview.SetSlotVisible("outer_top", false);
                Assert.That(preview.VisibleSlotCount, Is.EqualTo(9));
                Assert.That(preview.VisibleComponentCount, Is.EqualTo(12));
                preview.SetSlotLevel("main_weapon", 30);
                Assert.That(preview.GetSlotLevel("main_weapon"), Is.EqualTo(30));
                StringAssert.Contains("lv030", preview.GetSlotItemId("main_weapon"));
                StringAssert.Contains("runtimeEligibleCount=0", preview.Snapshot);
            }
            finally
            {
                preview?.Dispose();
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Map01AUsesOneActorAndInventoryFlowAcrossClassTenSlotReviews()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager
                .GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A Kiếm inventory test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                preview.ActivateClassEquipmentReview("kiem");
                Assert.That(preview.ClassEquipmentPreviewActive, Is.True);
                Assert.That(preview.AvatarClassLabel, Does.StartWith("Kiếm"));
                Assert.That(preview.EquipmentFitSummary, Is.EqualTo("Kiếm · rig chung · 10 slot · cấp 1/10/20/30"));
                Assert.That(preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Count(renderer => renderer.enabled && renderer.name.StartsWith("Map01A Kiếm kiem-lv001-male-")),
                    Is.EqualTo(13));

                preview.SelectVoEquipmentSlot("outer_tunic");
                preview.ToggleVoEquipmentSlot();
                Assert.That(preview.VoEquippedSlotCount, Is.EqualTo(9));
                Assert.That(preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Any(renderer => renderer.enabled && renderer.name.Contains("male-outer_top")), Is.False);
                preview.ToggleVoEquipmentSlot();
                preview.CycleVoSelectedEquipmentItemLevel();
                Assert.That(preview.VoSelectedEquipmentItemLevel, Is.EqualTo(10));
                StringAssert.Contains("kiem-lv010-male-outer_top", preview.GetVoEquipmentItemId("outer_tunic"));

                preview.ActivateClassEquipmentReview("phap");
                Assert.That(preview.ActiveEquipmentClassId, Is.EqualTo("phap"));
                Assert.That(preview.AvatarClassLabel, Does.StartWith("Pháp"));
                StringAssert.Contains("phap-lv010-male-outer_top", preview.GetVoEquipmentItemId("outer_tunic"));

                preview.ActivateClassEquipmentReview("co");
                Assert.That(preview.ActiveEquipmentClassId, Is.EqualTo("co"));
                Assert.That(preview.AvatarClassLabel, Does.StartWith("Cơ"));
                StringAssert.Contains("co-lv010-male-outer_top", preview.GetVoEquipmentItemId("outer_tunic"));

                preview.ActivateClassEquipmentReview("linh");
                Assert.That(preview.ActiveEquipmentClassId, Is.EqualTo("linh"));
                Assert.That(preview.AvatarClassLabel, Does.StartWith("Linh"));
                StringAssert.Contains("linh-lv010-male-outer_top", preview.GetVoEquipmentItemId("outer_tunic"));

                preview.SetVoRun(true);
                preview.MoveOnLane(1, .1f);
                Assert.That(preview.VoAvatarMotionState, Is.EqualTo("run"));
                Assert.That(preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Any(renderer => renderer.enabled && renderer.name.Contains("linh-lv010-male-outer_top")), Is.True);
                Assert.That(preview.GetComponentsInChildren<SpriteRenderer>(true)
                    .Any(renderer => renderer.enabled && renderer.name.StartsWith("Map01A Võ equipment component")), Is.False,
                    "Class review must not render a parallel Võ wardrobe");
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
        public void ActivatingLegacyClassPreviewHidesAnyLoadedSourcePoseActor()
        {
            var beforeRoots = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager
                .GetActiveScene().GetRootGameObjects());
            var host = new GameObject("Map01A presentation precedence test");
            CongDongLamMap01AArtPreview preview = null;
            try
            {
                var controller = TwoDOnboardingController.Attach(host);
                preview = CongDongLamMap01AArtPreview.Attach(controller);
                var sourceReview = preview.gameObject.AddComponent<TwoDSourcePoseReview>();
                typeof(CongDongLamMap01AArtPreview)
                    .GetField("_sourcePoseReview", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(preview, sourceReview);

                Assert.That(sourceReview.PresentationVisible, Is.True);
                preview.ActivateClassEquipmentReview("kiem");

                Assert.That(sourceReview.PresentationVisible, Is.False,
                    "Only the selected presentation may remain visible");
            }
            finally
            {
                if (preview != null) Object.DestroyImmediate(preview.gameObject);
                Object.DestroyImmediate(host);
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!beforeRoots.Contains(root)) Object.DestroyImmediate(root);
            }
        }

        private static TwoDSkeletalPaperDollRig.BoneDefinition Bone(string id, string parent, float x, float y)
        {
            return new TwoDSkeletalPaperDollRig.BoneDefinition(id, parent, new Vector2(x, y), id);
        }
    }
}
