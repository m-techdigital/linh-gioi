namespace LinhGioi.World
{
    public static class OnboardingDialogueContent
    {
        public const float StoneFeedbackDuration = 9f;

        public static string StoneFeedback(float elapsedSeconds)
        {
            if (float.IsNaN(elapsedSeconds) || elapsedSeconds < 0f || elapsedSeconds >= StoneFeedbackDuration) return null;
            if (elapsedSeconds < 3f) return "Đá Luyện cộng hưởng với linh lực của bạn.";
            if (elapsedSeconds < 6f) return "Một luồng sáng dịu lan ra. Bạn cảm thấy bình tĩnh hơn.";
            return "Đây mới chỉ là bước khởi đầu.";
        }

        public static NpcDialogueSession CreateGateKeeperSession() => new NpcDialogueSession(
            "Người Giữ Cổng", new[]
            {
                "Chào mừng đến Linh Thành.",
                "Nếu mới tới, hãy ghé Đá Luyện ở Sân Luyện.",
                "Gần đây Âm Khí quanh cổng có dao động lạ. Đừng lo, chúng ta đang kiểm soát."
            }, "Tìm hiểu Linh Thành",
            "Linh Thành là nơi mọi người gặp gỡ, nghỉ chân và bắt đầu hành trình cùng nhau.\n\n"
            + "Trong thành, hãy tôn trọng nhau và giữ lối đi thông thoáng. Nếu cần hướng dẫn, bạn cứ quay lại gặp tôi.\n\n"
            + "Đá Luyện ở sân gần đây sẽ giúp bạn làm quen với cách tương tác. Sau đó, bạn có thể ghé sân nghỉ và khám phá tiếp.",
            "Đến Sân Luyện");
    }
}
