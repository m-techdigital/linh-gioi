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

        private static void WritePack(string directory, string reviewSlot, string baseAtlas, string baseManifest,
            string[] components = null)
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
                        atlasRectTopLeft = new[] { index, 0, 1, 1 }, sourceCanvasRect = new[] { index * 4, 0, index * 4 + 4, 4 } };
                }
            var pack = new ReviewPack { reviewSlot = reviewSlot, basePoseAtlasSha256 = baseAtlas,
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

        [Test]
        public void SomersaultCompletesEarlierForSnappierJump()
        {
            Assert.That(TwoDSourcePoseReview.SomersaultDegrees(.40f), Is.LessThan(-350f));
        }
    }
}
