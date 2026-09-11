using LinhGioi.World;
using NUnit.Framework;

namespace LinhGioi.Tests.EditMode
{
    public sealed class TwoDPaperDollPoseSamplerTests
    {
        [Test]
        public void LocomotionAlternatesTheSameSharedRigPose()
        {
            Assert.That(TwoDPaperDollPoseSampler.Sample("walk", 0f, 0f), Is.EqualTo(0f).Within(.001f));
            Assert.That(TwoDPaperDollPoseSampler.Sample("walk", .25f, 0f), Is.EqualTo(.72f).Within(.001f));
            Assert.That(TwoDPaperDollPoseSampler.Sample("walk", .75f, 0f), Is.EqualTo(-.72f).Within(.001f));
            Assert.That(TwoDPaperDollPoseSampler.Sample("run", .25f, 0f), Is.EqualTo(.9f).Within(.001f));
            Assert.That(TwoDPaperDollPoseSampler.Sample("run", .75f, 0f), Is.EqualTo(-.9f).Within(.001f));
        }

        [TestCase("jump")]
        [TestCase("basic_attack")]
        [TestCase("skill")]
        public void ActionsEaseFromRestToPoseAndBack(string motion)
        {
            Assert.That(TwoDPaperDollPoseSampler.Sample(motion, 0f, 0f), Is.EqualTo(0f).Within(.001f));
            Assert.That(TwoDPaperDollPoseSampler.Sample(motion, 0f, .5f), Is.InRange(.6f, .8f));
            Assert.That(TwoDPaperDollPoseSampler.Sample(motion, 0f, 1f), Is.EqualTo(0f).Within(.001f));
        }

        [Test]
        public void UnknownOrIdleMotionKeepsBindPose()
        {
            Assert.That(TwoDPaperDollPoseSampler.Sample("idle", .25f, .5f), Is.Zero);
            Assert.That(TwoDPaperDollPoseSampler.Sample("unknown", .25f, .5f), Is.Zero);
        }
    }
}
