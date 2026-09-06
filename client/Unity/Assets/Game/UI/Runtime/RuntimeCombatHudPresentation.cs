using LinhGioi.Art;
using UnityEngine.UIElements;
using static LinhGioi.UI.RuntimeUiFactory;

namespace LinhGioi.UI
{
    internal static class RuntimeCombatHudPresentation
    {
        internal const string OwnerMarker = "LGO Runtime Combat HUD Presentation Helper v1";

        internal static void ApplySkillReadiness(Button button, string label, float remainingSeconds, bool targetInRange)
        {
            if (button == null) return;
            var coolingDown = remainingSeconds > 0f;
            var text = coolingDown
                ? label + "\n" + System.Math.Max(0.1f, remainingSeconds).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "s"
                : targetInRange ? label : label + "\nLại gần";
            if (button.text != text) button.text = text;
            button.SetEnabled(!coolingDown);
            button.tooltip = coolingDown ? "Đang hồi Chém Gió." : targetInRange ? "Thi triển Chém Gió lên bia luyện." : "Ngoài tầm Chém Gió. Đến gần bia luyện rồi thử lại.";
        }

        internal static void ApplyAssetState(
            Label rangeStatus,
            Label visualState,
            Label feedbackStatus,
            Label cooldownStatus,
            Label authorityStatus,
            bool coolingDown,
            string targetRangeText,
            string feedbackText,
            string authorityText)
        {
            var warning = ContainsAny(feedbackText, "Ngoài tầm", "Chưa chọn", "Đang hồi chiêu");
            if (rangeStatus != null)
                rangeStatus.text = CompactRangeStatus(targetRangeText).Replace("/ sẵn sàng.", coolingDown ? "/ đang hồi." : "/ sẵn sàng.");
            ApplyStatusAccent(rangeStatus, ContainsAny(targetRangeText, "trong tầm") ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Danger);
            ApplyStatusAccent(visualState, coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit);
            if (feedbackStatus != null) feedbackStatus.text = CompactFeedbackStatus(feedbackText);
            ApplyStatusAccent(feedbackStatus, warning ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Gold);
            ApplyStatusAccent(cooldownStatus, coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit);
            ApplyStatusAccent(authorityStatus, ContainsAny(authorityText, "từ chối", "Từ chối") ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Spirit);
        }

        internal static string CompactTargetStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Bia luyện: chưa rõ";
            return value
                .Replace("Mục tiêu luyện tập: sức bền mô phỏng ", "Sức bền: ")
                .Replace(" - Chỉ là mô phỏng cục bộ.", " mô phỏng.")
                .Replace("Mục tiêu luyện tập", "Bia luyện");
        }

        internal static string CompactRangeStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Tầm: chưa rõ";
            return value
                .Replace("Tầm đánh: trong tầm ", "Tầm: ")
                .Replace(" / sẵn sàng gửi ý định.", " / sẵn sàng.")
                .Replace("Tầm đánh: ngoài tầm ", "Tầm: ngoài ")
                .Replace("Tầm đánh", "Tầm");
        }

        internal static string CompactFeedbackStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Diễn tập an toàn.";
            if (value.Contains("Trúng mục tiêu")) return "Trúng mục tiêu: +1 Tinh khí diễn tập.";
            if (value.Contains("Đang xem Trói Bóng")) return "Đang xem Trói Bóng: vòng cảnh báo an toàn.";
            if (value.Contains("Đang xem Chém Gió")) return "Đang xem Chém Gió: cung vàng đã hiện.";
            if (value.Contains("Đang xem Hộ Linh")) return "Đang xem Hộ Linh: mạch sáng bảo hộ.";
            return value.Replace("Mô phỏng cục bộ", "Cục bộ");
        }

        private static bool ContainsAny(string value, params string[] needles)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (var needle in needles)
                if (value.Contains(needle)) return true;
            return false;
        }
    }
}
