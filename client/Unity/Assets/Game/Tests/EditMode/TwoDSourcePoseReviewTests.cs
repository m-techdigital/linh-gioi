using LinhGioi.World;
using NUnit.Framework;

namespace LinhGioi.Tests.EditMode
{
    public sealed class TwoDSourcePoseReviewTests
    {
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
