using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests.EditMode
{
    public sealed class TwoDSourcePoseReviewTests
    {
        [Serializable] private sealed class ReviewPack
        {
            public string status = "REVIEW_ONLY";
            public bool runtimeEligible;
            public int samplingDivisor = 4;
            public int[] jumpPivotSource = { 512, 768 };
            public string reviewSlot;
            public string itemId;
            public string fitFamily;
            public string gender;
            public int unlockLevel;
            public string basePoseAtlasSha256;
            public string basePoseManifestSha256;
            public ReviewPart[] sprites;
        }
        [Serializable] private sealed class ReviewPart
        {
            public string id;
            public string componentId;
            public int order;
            public string sourceSpaceProfile = "lgo_character_canvas_1024x1536_v1";
            public int[] atlasRectTopLeft;
            public int[] sourceCanvasRect;
        }

        [Test]
        public void ClassSwitchLoadsAndRemovesOptionalGenderWithoutLegacyFallback()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-class-genders-" + Guid.NewGuid().ToString("N"));
            var before = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            var host = new GameObject("class catalog review");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null);
                var preview = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var type = typeof(CongDongLamMap01AArtPreview);
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var male = new GameObject("male review").AddComponent<TwoDSourcePoseReview>();
                male.transform.SetParent(preview.transform);
                male.ReloadPack(directory);
                type.GetField("_sourcePoseReview", flags).SetValue(preview, male);
                var optionType = type.GetNestedType("SourcePoseClassOption", BindingFlags.NonPublic);
                var options = (System.Collections.IList)type.GetField("_sourcePoseClassOptions", flags).GetValue(preview);
                foreach (var femalePath in new[] { "-", directory })
                {
                    var option = Activator.CreateInstance(optionType, true);
                    optionType.GetField("Id").SetValue(option, femalePath == "-" ? "single" : "pair");
                    optionType.GetField("MalePrimary").SetValue(option, directory);
                    optionType.GetField("FemalePrimary").SetValue(option, femalePath);
                    options.Add(option);
                }
                preview.CycleSourcePoseClass();
                preview.CycleVoAvatarGender();
                Assert.That(preview.VoAvatarGender, Is.EqualTo("female"));
                Assert.That(preview.IsSourcePoseReviewActive, Is.True);
                type.GetField("_sourcePoseClassSwitchReadyAt", flags).SetValue(preview, 0f);
                preview.CycleSourcePoseClass();
                Assert.That(preview.VoAvatarGender, Is.EqualTo("male"));
                Assert.That(preview.IsSourcePoseReviewActive, Is.True);
                Assert.That(type.GetField("_femaleSourcePoseReview", flags).GetValue(preview), Is.Null);
                Assert.That(preview.CanCycleSourcePoseClass, Is.True);
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void SingleGenderReviewCannotSwitchBackToLegacyPresentation()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-single-gender-" + Guid.NewGuid().ToString("N"));
            var before = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            var host = new GameObject("single gender review");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null);
                var preview = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var male = new GameObject("male review").AddComponent<TwoDSourcePoseReview>();
                male.transform.SetParent(preview.transform);
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(male, new object[] { directory });
                typeof(CongDongLamMap01AArtPreview).GetField("_sourcePoseReview", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(preview, male);
                preview.CycleVoAvatarGender();
                Assert.That(preview.VoAvatarGender, Is.EqualTo("male"), "Missing female source must never select the legacy female renderer");
                Assert.That(preview.IsSourcePoseReviewActive, Is.True);
                var female = new GameObject("female review").AddComponent<TwoDSourcePoseReview>();
                female.transform.SetParent(preview.transform);
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(female, new object[] { directory });
                typeof(CongDongLamMap01AArtPreview).GetField("_femaleSourcePoseReview", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(preview, female);
                preview.CycleVoAvatarGender();
                Assert.That(preview.VoAvatarGender, Is.EqualTo("female"));
                Assert.That(preview.IsSourcePoseReviewActive, Is.True);
            }
            finally
            {
                foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (!before.Contains(root)) UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void OptionalOuterTopPackSharesFrameFacingAndSomersaultRoot()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-pose-overlay-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("pose overlay test");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null);
                var overlay = Path.Combine(directory, "outer-top-review");
                Directory.CreateDirectory(overlay);
                WritePack(overlay, "outer_top", Hash(Path.Combine(directory, "atlas-review.png")),
                    Hash(Path.Combine(directory, "atlas-review.json")));

                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { directory });

                var renderers = root.GetComponentsInChildren<SpriteRenderer>();
                Assert.That(review.transform.localPosition, Is.EqualTo(Vector3.zero),
                    "Latest pose/wardrobe stack must occupy the main actor position, not a parallel comparison lane");
                Assert.That(renderers.Length, Is.EqualTo(2), "Optional outer-top review pack must render beside its source body");
                Assert.That(renderers[0].transform.parent, Is.SameAs(renderers[1].transform.parent));
                review.Apply("run", 0f, -1);
                Assert.That(renderers[0].sprite.rect.x, Is.EqualTo(renderers[1].sprite.rect.x));
                Assert.That(renderers[0].transform.localScale.x, Is.LessThan(0));
                Assert.That(renderers[1].transform.localScale.x, Is.LessThan(0));
                review.Apply("jump", 0f, 1, .2f);
                Assert.That(renderers[0].transform.parent.localRotation,
                    Is.EqualTo(renderers[1].transform.parent.localRotation));
                Assert.That(renderers[1].sortingOrder, Is.GreaterThan(renderers[0].sortingOrder));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void OuterTopReviewCanBeRemovedWithoutHidingTheSourceBody()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-pose-overlay-toggle-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("pose overlay toggle test");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null);
                var overlay = Path.Combine(directory, "outer-top-review");
                Directory.CreateDirectory(overlay);
                WritePack(overlay, "outer_top", Hash(Path.Combine(directory, "atlas-review.png")),
                    Hash(Path.Combine(directory, "atlas-review.json")));
                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { directory });
                var renderers = root.GetComponentsInChildren<SpriteRenderer>();
                var setVisible = typeof(TwoDSourcePoseReview).GetMethod("SetSlotVisible");
                Assert.That(setVisible, Is.Not.Null, "Pose review needs one generic slot toggle for the ten-item wardrobe");
                setVisible.Invoke(review, new object[] { "outer_top", false });
                Assert.That(renderers.Single(renderer => renderer.sortingOrder == 24).enabled, Is.True);
                Assert.That(renderers.Single(renderer => renderer.sortingOrder > 24).enabled, Is.False);
                setVisible.Invoke(review, new object[] { "outer_top", true });
                Assert.That(renderers.Single(renderer => renderer.sortingOrder > 24).enabled, Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void WholePosePresentationCanHideForTheOtherGenderWithoutDestroyingSlotState()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-pose-gender-toggle-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("pose gender toggle test");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null);
                var overlay = Path.Combine(directory, "outer-top-review");
                Directory.CreateDirectory(overlay);
                WritePack(overlay, "outer_top", Hash(Path.Combine(directory, "atlas-review.png")),
                    Hash(Path.Combine(directory, "atlas-review.json")));
                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { directory });

                review.SetPresentationVisible(false);
                Assert.That(review.PresentationVisible, Is.False);
                Assert.That(root.GetComponentsInChildren<SpriteRenderer>().All(renderer => renderer.forceRenderingOff), Is.True);
                review.SetPresentationVisible(true);
                Assert.That(review.PresentationVisible, Is.True);
                Assert.That(root.GetComponentsInChildren<SpriteRenderer>().All(renderer => !renderer.forceRenderingOff), Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void FemalePackUsesItsOwnFitFamilyInTheSameSourcePoseRuntime()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-pose-female-family-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("female source pose test");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null, gender: "female", fitFamily: "common_female_v1");
                var overlay = Path.Combine(directory, "outer-top-review");
                Directory.CreateDirectory(overlay);
                WritePack(overlay, "outer_top", Hash(Path.Combine(directory, "atlas-review.png")),
                    Hash(Path.Combine(directory, "atlas-review.json")), gender: "female", fitFamily: "common_female_v1");
                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { directory });

                Assert.That(review.GenderId, Is.EqualTo("female"));
                Assert.That(review.GenderLabel, Is.EqualTo("nữ"));
                Assert.That(root.GetComponentsInChildren<SpriteRenderer>().Length, Is.EqualTo(2));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void HighDensityDivisorTwoOverlayLoadsOnTheLockedDivisorFourBody()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-pose-hd-overlay-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("pose HD overlay test");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null);
                var overlay = Path.Combine(directory, "outer-top-review");
                Directory.CreateDirectory(overlay);
                WritePack(overlay, "outer_top", Hash(Path.Combine(directory, "atlas-review.png")),
                    Hash(Path.Combine(directory, "atlas-review.json")), divisor: 2);

                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { directory });

                Assert.That(root.GetComponentsInChildren<SpriteRenderer>().Length, Is.EqualTo(2));
                Assert.That(review.CurrentFrame, Is.EqualTo("idle"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void MultipleItemSlotsShareOneBodyFrameAndToggleIndependently()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-pose-ten-slot-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("pose ten-slot test");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null);
                foreach (var pair in new[] { ("outer-top-review", "outer_top"), ("waist-belt-review", "waist_belt") })
                {
                    var overlay = Path.Combine(directory, pair.Item1);
                    Directory.CreateDirectory(overlay);
                    WritePack(overlay, pair.Item2, Hash(Path.Combine(directory, "atlas-review.png")),
                        Hash(Path.Combine(directory, "atlas-review.json")),
                        pair.Item2 == "waist_belt" ? new[] { "front", "back" } : null);
                }
                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { directory });
                review.Apply("run", 0.51f / TwoDSourcePoseTimeline.RunCyclesPerSecond, -1);
                var renderers = root.GetComponentsInChildren<SpriteRenderer>();
                Assert.That(renderers.Length, Is.EqualTo(4), "One slot can own multiple front/back components without adding an actor");
                review.SetSlotVisible("waist_belt", false);
                Assert.That(renderers.Count(renderer => renderer.enabled), Is.EqualTo(2));
                review.SetSlotVisible("outer_top", false);
                Assert.That(renderers.Count(renderer => renderer.enabled), Is.EqualTo(1));
                Assert.That(renderers.Single(renderer => renderer.sortingOrder == 24).enabled, Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void VisibleBoundsAndTransformDiagnosticsDescribeTheDisplayedPoseActor()
        {
            var directory = Path.Combine(Application.temporaryCachePath, "lgo-pose-bounds-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("pose bounds test");
            try
            {
                Directory.CreateDirectory(directory);
                WritePack(directory, null, null, null);
                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { directory });

                review.Apply("idle", 0, 1);
                Assert.That(review.VisibleWorldBounds().size.y, Is.GreaterThan(0));
                Assert.That(review.PoseRootScale, Is.EqualTo(Vector3.one));
                Assert.That(review.PoseRootRotationDegrees, Is.EqualTo(0).Within(.001f));
                review.Apply("jump", 0, 1, .2f);
                Assert.That(review.PoseRootScale, Is.EqualTo(Vector3.one));
                Assert.That(Mathf.Abs(review.PoseRootRotationDegrees), Is.GreaterThan(1));
                review.SetPresentationVisible(false);
                Assert.That(review.VisibleWorldBounds().size, Is.EqualTo(Vector3.zero));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        [Test]
        public void ReloadPackReplacesClassOnTheSameActorWithoutLeavingOldRenderers()
        {
            var first = Path.Combine(Application.temporaryCachePath, "lgo-pose-reload-a-" + Guid.NewGuid().ToString("N"));
            var second = Path.Combine(Application.temporaryCachePath, "lgo-pose-reload-b-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("pose reload test");
            try
            {
                Directory.CreateDirectory(first);
                Directory.CreateDirectory(second);
                WritePack(first, null, null, null);
                WritePack(second, null, null, null);
                foreach (var pair in new[] { (first, "vo_male_lv001_outer_top"), (second, "phap_male_lv001_outer_top") })
                {
                    var overlay = Path.Combine(pair.Item1, "outer-top-review");
                    Directory.CreateDirectory(overlay);
                    WritePack(overlay, "outer_top", Hash(Path.Combine(pair.Item1, "atlas-review.png")),
                        Hash(Path.Combine(pair.Item1, "atlas-review.json")));
                    var path = Path.Combine(overlay, "atlas-review.json");
                    File.WriteAllText(path, File.ReadAllText(path).Replace("vo_male_lv001_outer_top", pair.Item2));
                }
                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { first });
                Assert.That(review.ClassId, Is.EqualTo("vo"));
                Assert.That(root.GetComponentsInChildren<SpriteRenderer>(true).Length, Is.EqualTo(2));

                review.ReloadPack(second);

                Assert.That(review.ClassId, Is.EqualTo("phap"));
                Assert.That(root.GetComponentsInChildren<SpriteRenderer>(true).Length, Is.EqualTo(2));
                Assert.That(review.GetSlotItemId("outer_top"), Does.StartWith("phap_"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(first)) Directory.Delete(first, true);
                if (Directory.Exists(second)) Directory.Delete(second, true);
            }
        }

        [Test]
        public void SameSlotCanSwitchLevelVariantWithoutAddingAnotherActor()
        {
            var primary = Path.Combine(Application.temporaryCachePath, "lgo-pose-level-primary-" + Guid.NewGuid().ToString("N"));
            var alternate = Path.Combine(Application.temporaryCachePath, "lgo-pose-level-alt-" + Guid.NewGuid().ToString("N"));
            var root = new GameObject("pose level variant test");
            try
            {
                Directory.CreateDirectory(primary);
                WritePack(primary, null, null, null);
                var primarySlot = Path.Combine(primary, "outer-top-review");
                Directory.CreateDirectory(primarySlot);
                WritePack(primarySlot, "outer_top", Hash(Path.Combine(primary, "atlas-review.png")),
                    Hash(Path.Combine(primary, "atlas-review.json")), level: 1);

                Directory.CreateDirectory(alternate);
                File.Copy(Path.Combine(primary, "atlas-review.png"), Path.Combine(alternate, "atlas-review.png"));
                File.Copy(Path.Combine(primary, "atlas-review.json"), Path.Combine(alternate, "atlas-review.json"));
                var alternateSlot = Path.Combine(alternate, "outer-top-review");
                Directory.CreateDirectory(alternateSlot);
                WritePack(alternateSlot, "outer_top", Hash(Path.Combine(primary, "atlas-review.png")),
                    Hash(Path.Combine(primary, "atlas-review.json")), level: 10);

                var review = root.AddComponent<TwoDSourcePoseReview>();
                typeof(TwoDSourcePoseReview).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(review, new object[] { primary });
                review.LoadItemVariants(alternate);

                var renderers = root.GetComponentsInChildren<SpriteRenderer>();
                Assert.That(renderers.Length, Is.EqualTo(3), "Variants share the existing body actor/root");
                Assert.That(renderers.Count(renderer => renderer.enabled), Is.EqualTo(2));
                Assert.That(review.SetSlotItemLevel("outer_top", 10), Is.True);
                Assert.That(renderers.Count(renderer => renderer.enabled), Is.EqualTo(2));
                Assert.That(review.GetSlotItemLevel("outer_top"), Is.EqualTo(10));
                Assert.That(review.NextCompleteItemLevel(1), Is.EqualTo(10));
                Assert.That(review.NextCompleteItemLevel(10), Is.EqualTo(1));
                Assert.That(review.NextSlotItemLevel("outer_top", 1), Is.EqualTo(10));
                Assert.That(review.HasCompleteItemLevel(10), Is.True);
                Assert.That(review.GetCompleteItemLevels(), Is.EqualTo(new[] { 1, 10 }));
                review.SetSlotVisible("outer_top", false);
                Assert.That(renderers.Count(renderer => renderer.enabled), Is.EqualTo(1));
                Assert.That(review.SetSlotItemLevel("outer_top", 1), Is.True);
                Assert.That(renderers.Count(renderer => renderer.enabled), Is.EqualTo(1), "Level switch preserves unequipped state");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                if (Directory.Exists(primary)) Directory.Delete(primary, true);
                if (Directory.Exists(alternate)) Directory.Delete(alternate, true);
            }
        }

        private static void WritePack(string directory, string reviewSlot, string baseAtlas, string baseManifest,
            string[] components = null, int level = 1, int divisor = 4, string gender = null, string fitFamily = null)
        {
            var ids = new[] { "idle", "run_contact_a", "run_a", "run_contact_b", "run_b", "jump_tuck" };
            components = components ?? new[] { "main" };
            var texture = new Texture2D(Mathf.NextPowerOfTwo(ids.Length * components.Length), 1, TextureFormat.RGBA32, false);
            try
            {
                for (var i = 0; i < ids.Length * components.Length; i++) texture.SetPixel(i, 0, new Color(i / 12f, .5f, .7f, 1));
                texture.Apply();
                File.WriteAllBytes(Path.Combine(directory, "atlas-review.png"), texture.EncodeToPNG());
            }
            finally { UnityEngine.Object.DestroyImmediate(texture); }
            var parts = new ReviewPart[ids.Length * components.Length];
            for (var component = 0; component < components.Length; component++)
                for (var i = 0; i < ids.Length; i++)
                {
                    var index = component * ids.Length + i;
                    parts[index] = new ReviewPart { id = ids[i], componentId = components[component], order = 25 + component,
                        atlasRectTopLeft = new[] { index, 0, 1, 1 }, sourceCanvasRect = new[] { index * divisor, 0, index * divisor + divisor, divisor } };
                }
            var family = fitFamily ?? (reviewSlot == null ? null : "vo_male_v3");
            var pack = new ReviewPack { reviewSlot = reviewSlot, basePoseAtlasSha256 = baseAtlas,
                itemId = reviewSlot == null ? null : (gender == "female" ? "linh_female_lv" : "vo_male_lv") + level.ToString("000") + "_" + reviewSlot,
                fitFamily = family, gender = gender, unlockLevel = level, samplingDivisor = divisor,
                basePoseManifestSha256 = baseManifest, sprites = parts };
            File.WriteAllText(Path.Combine(directory, "atlas-review.json"), JsonUtility.ToJson(pack, true));
        }

        private static string Hash(string path)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
        }

        [Test]
        public void ExplicitSeekSettlesMovementInsteadOfStartingANewStop()
        {
            var timeline = new TwoDSourcePoseTimeline();
            timeline.Select("run", 0);
            timeline.Select("run", .4f);
            timeline.Advance(2);
            Assert.That(timeline.Select("idle", .5f), Is.EqualTo("idle"));
            timeline.Advance(1f / 30);
            Assert.That(timeline.Select("run", .6f), Is.EqualTo("run_start"));
            Assert.That(timeline.Select("idle", .7f), Is.EqualTo("run_stop"));
            timeline.Advance(2);
            Assert.That(timeline.Select("idle", .8f), Is.EqualTo("idle"));
        }

        [Test]
        public void EntryAndExitAreOutsideTheFourPoseLoop()
        {
            var timeline = new TwoDSourcePoseTimeline();
            Assert.That(timeline.Select("idle", 0), Is.EqualTo("idle"));
            Assert.That(timeline.Select("run", 1), Is.EqualTo("run_start"));
            Assert.That(timeline.Select("run", 1.13f), Is.EqualTo("run_contact_a"));
            Assert.That(timeline.Select("run", 1.30f), Is.EqualTo("run_a"));
            Assert.That(timeline.Select("run", 1.47f), Is.EqualTo("run_contact_b"));
            Assert.That(timeline.Select("run", 1.64f), Is.EqualTo("run_b"));
            Assert.That(timeline.Select("run", 1.80f), Is.EqualTo("run_contact_a"));
            Assert.That(timeline.Select("idle", 2), Is.EqualTo("run_stop"));
            Assert.That(timeline.Select("idle", 2.13f), Is.EqualTo("idle"));
        }

        [Test]
        public void RepeatedRefreshDoesNotExtendEntryAndRestartBeginsAtContactA()
        {
            var timeline = new TwoDSourcePoseTimeline();
            for (var i = 0; i < 20; i++) Assert.That(timeline.Select("run", 10), Is.EqualTo("run_start"));
            Assert.That(timeline.Select("run", 10.13f), Is.EqualTo("run_contact_a"));
            Assert.That(timeline.Select("idle", 10.2f), Is.EqualTo("run_stop"));
            Assert.That(timeline.Select("run", 10.25f), Is.EqualTo("run_start"));
            Assert.That(timeline.Select("run", 10.38f), Is.EqualTo("run_contact_a"));
        }

        [Test]
        public void JumpInterruptsRunWithoutPlayingStopAfterLanding()
        {
            var timeline = new TwoDSourcePoseTimeline();
            timeline.Select("run", 0);
            timeline.Select("jump", .2f);
            Assert.That(timeline.Select("idle", .7f), Is.EqualTo("idle"));
        }

        [Test]
        public void RunReviewUsesGroundedContactFramesWhenAvailable()
        {
            Assert.That(TwoDSourcePoseReview.SelectRunFrame(0.00f, true), Is.EqualTo("run_contact_a"));
            Assert.That(TwoDSourcePoseReview.SelectRunFrame(0.24f, true), Is.EqualTo("run_contact_a"));
            Assert.That(TwoDSourcePoseReview.SelectRunFrame(0.25f, true), Is.EqualTo("run_a"));
            Assert.That(TwoDSourcePoseReview.SelectRunFrame(0.50f, true), Is.EqualTo("run_contact_b"));
            Assert.That(TwoDSourcePoseReview.SelectRunFrame(0.75f, true), Is.EqualTo("run_b"));
        }

        [Test]
        public void RunLoopRepeatsAllFourPhasesWithoutIdleOrJump()
        {
            var expected = new[] { "run_contact_a", "run_a", "run_contact_b", "run_b" };
            for (var i = 0; i < 12; i++)
                Assert.That(TwoDSourcePoseReview.SelectRunFrame(i * .25f + .01f, true), Is.EqualTo(expected[i % 4]));
        }

        [TestCase("linh_male_lv001_main_weapon", "linh", "Linh")]
        [TestCase("kiem_male_lv001_main_weapon", "kiem", "Kiếm")]
        [TestCase("vo_male_lv001_main_weapon", "vo", "Võ")]
        public void PoseReviewDerivesDisplayedClassFromLoadedItem(string itemId, string classId, string label)
        {
            Assert.That(TwoDSourcePoseReview.ResolveClassId(itemId), Is.EqualTo(classId));
            Assert.That(TwoDSourcePoseReview.ResolveClassLabel(itemId), Is.EqualTo(label));
        }

        [Test]
        public void SomersaultCompletesEarlierForSnappierJump()
        {
            Assert.That(TwoDSourcePoseReview.SomersaultDegrees(.40f), Is.LessThan(-350f));
        }
    }
}
