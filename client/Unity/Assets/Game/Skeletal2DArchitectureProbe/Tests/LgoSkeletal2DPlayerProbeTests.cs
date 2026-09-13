using System.Linq;
using LinhGioi.ArchitectureProbe;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests
{
    public sealed class LgoSkeletal2DPlayerProbeTests
    {
        [Test]
        public void PreserveChainLengths_UsesTargetDirectionsWithoutChangingBindProportions()
        {
            var bind = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(0f, 2f),
                new Vector2(0f, 5f),
            };
            var target = new[]
            {
                new Vector2(10f, 20f),
                new Vector2(14f, 20f),
                new Vector2(14f, 21f),
            };

            var solved = LgoSkeletal2DPlayerProbe.PreserveChainLengths(bind, target);

            Assert.That(solved[0], Is.EqualTo(target[0]));
            Assert.That(Vector2.Distance(solved[0], solved[1]), Is.EqualTo(2f).Within(0.00001f));
            Assert.That(Vector2.Distance(solved[1], solved[2]), Is.EqualTo(3f).Within(0.00001f));
            Assert.That(solved[1].y, Is.EqualTo(20f).Within(0.00001f));
            Assert.That(solved[2].x, Is.EqualTo(solved[1].x).Within(0.00001f));
        }

        [Test]
        public void PreserveChainLengths_RejectsCollapsedTargetDirection()
        {
            var bind = new[] { Vector2.zero, Vector2.right };
            var target = new[] { Vector2.one, Vector2.one };

            Assert.Throws<System.ArgumentException>(() =>
                LgoSkeletal2DPlayerProbe.PreserveChainLengths(bind, target));
        }

        [Test]
        public void Scenario_ContainsFullRequiredTransitionSequence()
        {
            var states = LgoSkeletal2DPlayerProbe.Scenario.Select(step => step.State).ToArray();

            CollectionAssert.AreEqual(new[]
            {
                "idle", "run", "jump", "fall", "land", "run",
                "attack", "return_to_idle", "run",
            }, states);
        }

        [Test]
        public void MeasureCutoutAxis_UsesAlphaEndpointCentersForSlantedPart()
        {
            const int width = 12, height = 12;
            var pixels = new Color32[width * height];
            for (var y = 1; y <= 10; y++)
            {
                var x = 2 + y / 3;
                pixels[y * width + x] = new Color32(255, 255, 255, 255);
                pixels[y * width + x + 1] = new Color32(255, 255, 255, 128);
            }

            var measured = LgoSkeletal2DPlayerProbe.MeasureCutoutAxis(pixels, width, height, .2f);

            Assert.That(measured.TopCenter.y, Is.GreaterThan(measured.BottomCenter.y));
            Assert.That(measured.TopCenter.x, Is.GreaterThan(measured.BottomCenter.x));
            Assert.That(measured.TopToBottom.magnitude, Is.GreaterThan(7f));
        }

        [Test]
        public void MeasureCutoutAxis_RejectsEmptyAlpha()
        {
            Assert.Throws<System.ArgumentException>(() =>
                LgoSkeletal2DPlayerProbe.MeasureCutoutAxis(new Color32[16], 4, 4));
        }

        [Test]
        public void RegisteredUniformScale_MapsMeasuredSourceAxisToBindLength()
        {
            var scale = LgoSkeletal2DPlayerProbe.RegisteredUniformScale(300f, 600f, .75f);

            Assert.That(scale, Is.EqualTo(1.5f).Within(.000001f));
            Assert.That((300f / 600f) * scale, Is.EqualTo(.75f).Within(.000001f));
        }

        [Test]
        public void DistanceToOpaqueAlpha_MeasuresNearestVisibleSourcePixel()
        {
            var pixels = new Color32[25];
            pixels[4 * 5 + 3] = new Color32(255, 255, 255, 255);

            var distance = LgoSkeletal2DPlayerProbe.DistanceToOpaqueAlpha(
                pixels, 5, 5, new Vector2(.5f, .5f));

            Assert.That(distance, Is.EqualTo(5f).Within(.000001f));
        }

        [Test]
        public void CaptureLabel_KeepsSequenceForRepeatedStates()
        {
            Assert.That(LgoSkeletal2DPlayerProbe.CaptureLabel(0, "idle"), Is.EqualTo("01-idle.bmp"));
            Assert.That(LgoSkeletal2DPlayerProbe.CaptureLabel(1, "run"), Is.EqualTo("02-run.bmp"));
            Assert.That(LgoSkeletal2DPlayerProbe.CaptureLabel(8, "run"), Is.EqualTo("09-run.bmp"));
        }

        [Test]
        public void BindProfile_ExposesDraftStatusAndDerivesFeetFromGroundPolicy()
        {
            const string json = @"{
              ""status"":""DRAFT_GUIDE_REQUIRES_REVIEW"",""guideId"":""test-guide"",
              ""sourceSpaceProfile"":""lgo_character_canvas_1024x1536_v1"",""sourceCanvas"":[1024,1536],
              ""coordinateOrigin"":""top_left"",""originX"":512,""groundY"":1484,
              ""poses"":[{""pose"":""idle"",""landmarks"":[
                {""id"":""crown"",""xy"":[520,77]},{""id"":""neck"",""xy"":[510,263]},
                {""id"":""near_shoulder"",""xy"":[403,322]},{""id"":""near_elbow"",""xy"":[351,519]},{""id"":""near_wrist"",""xy"":[349,721]},
                {""id"":""far_shoulder"",""xy"":[585,342]},{""id"":""far_elbow"",""xy"":[640,554]},{""id"":""far_wrist"",""xy"":[686,728]},
                {""id"":""near_hip"",""xy"":[480,718]},{""id"":""near_knee"",""xy"":[407,1002]},{""id"":""near_ankle"",""xy"":[337,1351]},
                {""id"":""far_hip"",""xy"":[589,720]},{""id"":""far_knee"",""xy"":[628,1017]},{""id"":""far_ankle"",""xy"":[645,1350]}
              ]}] }";

            var profile = LgoSkeletal2DBindProfile.Parse(json);
            var segments = profile.CreateReviewSegments();

            Assert.That(profile.RuntimeAuthority, Is.False);
            Assert.That(segments.Single(segment => segment.Id == "near_shin").End, Is.EqualTo(new Vector2(337, 1484)));
            Assert.That(segments.Single(segment => segment.Id == "pelvis").Start, Is.EqualTo(new Vector2(534.5f, 719f)));
        }
    }
}
