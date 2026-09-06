using LinhGioi.World;
using NUnit.Framework;

namespace LinhGioi.Tests
{
    public sealed class NpcDialogueSessionTests
    {
        [Test]
        public void CloseDoesNotCompleteAndReopenStartsAtFirstLine()
        {
            var session = new NpcDialogueSession("NPC", new[] { "One", "Two", "Three" });
            Assert.IsTrue(session.Open());
            Assert.IsTrue(session.Advance());
            Assert.AreEqual("2/3", session.Progress);
            Assert.IsTrue(session.Close());
            Assert.IsFalse(session.Completed);
            Assert.IsFalse(session.Active);
            Assert.IsFalse(session.Advance());
            Assert.IsTrue(session.Open());
            Assert.AreEqual("One", session.Line);
            Assert.AreEqual("1/3", session.Progress);
        }

        [Test]
        public void CompletionOccursOnlyAfterLastLineAndResetClearsIt()
        {
            var session = new NpcDialogueSession("NPC", new[] { "One", "Two" });
            Assert.IsFalse(session.Advance());
            session.Open();
            Assert.IsTrue(session.HasNext);
            session.Advance();
            Assert.IsFalse(session.Completed);
            Assert.IsFalse(session.HasNext);
            session.Advance();
            Assert.IsTrue(session.Completed);
            Assert.IsFalse(session.Active);
            Assert.IsFalse(session.Advance());
            Assert.IsFalse(session.Close());
            session.Reset();
            Assert.IsFalse(session.Completed);
            Assert.AreEqual("0/2", session.Progress);
        }

        [Test]
        public void RepeatedOpenDoesNotRestartAnActiveConversation()
        {
            var lines = new[] { "One", "Two" };
            var session = new NpcDialogueSession("NPC", lines);
            lines[1] = "Mutated";
            session.Open();
            session.Advance();
            Assert.IsFalse(session.Open());
            Assert.AreEqual("Two", session.Line);
        }
    }
}
