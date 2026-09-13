using System.Linq;
using LinhGioi.ArchitectureProbe;
using NUnit.Framework;

namespace LinhGioi.Tests.EditMode
{
    public sealed class LgoModular3DPlayerProbeTests
    {
        [Test]
        public void Scenario_CoversRequiredActionOrderAndSwapOccursDuringRun()
        {
            var scenario = LgoModular3DPlayerProbe.Scenario;
            CollectionAssert.AreEqual(
                new[] { "idle", "run", "jump", "run", "attack", "return_to_idle", "run" },
                scenario.Select(step => step.State).ToArray());
            Assert.That(LgoModular3DPlayerProbe.EquipmentSwapAtSeconds, Is.GreaterThan(scenario[1].AtSeconds));
            Assert.That(LgoModular3DPlayerProbe.EquipmentSwapAtSeconds, Is.LessThan(scenario[2].AtSeconds));
            Assert.That(LgoModular3DPlayerProbe.QuitAtSeconds, Is.GreaterThan(scenario.Last().AtSeconds + 2f));
        }

        [Test]
        public void Percentile95_IsDeterministicAndUsesUpperRank()
        {
            Assert.That(LgoModular3DPlayerProbe.Percentile95(new[] { 5f, 1f, 4f, 3f, 2f }), Is.EqualTo(5f));
            Assert.That(LgoModular3DPlayerProbe.Percentile95(new[] { 3f }), Is.EqualTo(3f));
            Assert.That(LgoModular3DPlayerProbe.Percentile95(new float[0]), Is.EqualTo(0f));
        }
    }
}
