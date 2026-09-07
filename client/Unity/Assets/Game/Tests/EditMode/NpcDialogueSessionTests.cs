using LinhGioi.World;
using NUnit.Framework;

namespace LinhGioi.Tests
{
    public sealed class NpcDialogueSessionTests
    {
        [Test]
        public void ReturningGuideCanCloseAndCompleteWithoutResettingIntroduction()
        {
            var introduction = OnboardingDialogueContent.CreateGateKeeperSession();
            introduction.Open();
            while (introduction.Active) introduction.Advance();
            var returning = OnboardingDialogueContent.CreateGateKeeperReturnSession();
            returning.Open();
            Assert.AreEqual("1/2", returning.Progress);
            StringAssert.Contains("Đá Luyện", returning.Line);
            returning.Close();
            Assert.IsTrue(introduction.Completed);
            Assert.IsFalse(returning.Completed);
            returning.Open();
            returning.Advance();
            Assert.AreEqual("2/2", returning.Progress);
            Assert.AreEqual("Khám phá tiếp", returning.CompletionAction);
            returning.Advance();
            Assert.IsTrue(returning.Completed);
            Assert.IsTrue(introduction.Completed);
            returning.Open();
            Assert.AreEqual("1/2", returning.Progress);
            Assert.IsTrue(introduction.Completed);
        }

        [TestCase(-1f, null)]
        [TestCase(0f, "Đá Luyện cộng hưởng với linh lực của bạn.")]
        [TestCase(2.99f, "Đá Luyện cộng hưởng với linh lực của bạn.")]
        [TestCase(3f, "Một luồng sáng dịu lan ra. Bạn cảm thấy bình tĩnh hơn.")]
        [TestCase(5.99f, "Một luồng sáng dịu lan ra. Bạn cảm thấy bình tĩnh hơn.")]
        [TestCase(6f, "Đây mới chỉ là bước khởi đầu.")]
        [TestCase(8.99f, "Đây mới chỉ là bước khởi đầu.")]
        [TestCase(9f, null)]
        [TestCase(float.PositiveInfinity, null)]
        [TestCase(float.NaN, null)]
        public void StoneNarrationUsesBoundedNonRewardFeedback(float elapsed, string expected)
        {
            Assert.AreEqual(expected, OnboardingDialogueContent.StoneFeedback(elapsed));
        }

        [Test]
        public void SessionSupportsInformationWithoutCompletingTheObjective()
        {
            var session = OnboardingDialogueContent.CreateGateKeeperSession();
            Assert.IsFalse(session.ReadInformation());
            session.Open();
            Assert.IsFalse(session.ReadInformation());
            session.Advance();
            session.Advance();
            var finalLine = session.Line;
            Assert.IsTrue(session.ReadInformation());
            StringAssert.Contains("gặp gỡ", session.Line);
            Assert.IsFalse(session.ReadInformation());
            Assert.IsFalse(session.Completed);
            Assert.IsTrue(session.Advance());
            Assert.IsFalse(session.ReadingInformation);
            Assert.AreEqual(finalLine, session.Line);
            Assert.IsTrue(session.Active);
            Assert.IsFalse(session.Completed);
            session.Advance();
            Assert.IsTrue(session.Completed);
            Assert.IsFalse(session.ReadInformation());
        }

        [Test]
        public void ClosingInformationResetsWithoutCompletingAndSessionsAreIndependent()
        {
            var session = OnboardingDialogueContent.CreateGateKeeperSession();
            session.Open(); session.Advance(); session.Advance(); session.ReadInformation();
            Assert.IsTrue(session.Close());
            Assert.IsFalse(session.Completed);
            Assert.IsFalse(session.ReadingInformation);
            session.Open();
            Assert.AreEqual("1/3", session.Progress);
            Assert.IsFalse(OnboardingDialogueContent.CreateGateKeeperSession().Active);
        }

        [Test]
        public void InformationAndCompletionActionsRejectIncompleteContent()
        {
            Assert.Throws<System.ArgumentException>(() => new NpcDialogueSession("NPC", new[] { "One" }, "Read", null));
            Assert.Throws<System.ArgumentException>(() => new NpcDialogueSession("NPC", new[] { "One" }, null, "Info"));
            Assert.Throws<System.ArgumentException>(() => new NpcDialogueSession("NPC", new[] { "One" }, "Read", " "));
            Assert.Throws<System.ArgumentException>(() => new NpcDialogueSession("NPC", new[] { "One" }, completionAction: " "));
        }

        [Test]
        public void PlayableGuideWelcomesPlayerToLinhThanh()
        {
            var host = new UnityEngine.GameObject("Guide content test");
            try
            {
                var world = host.AddComponent<PlayableWorldController>();
                world.Dialogue.Open();
                Assert.AreEqual("Chào mừng đến Linh Thành.", world.Dialogue.Line);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

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
