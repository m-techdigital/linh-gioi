using LinhGioi.World;
using NUnit.Framework;

namespace LinhGioi.Tests.EditMode
{
    public sealed class TwoDSourcePoseReviewTests
    {
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
