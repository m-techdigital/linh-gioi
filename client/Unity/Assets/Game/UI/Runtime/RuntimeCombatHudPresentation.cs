using LinhGioi.Art;
using UnityEngine.UIElements;
using static LinhGioi.UI.RuntimeUiFactory;

namespace LinhGioi.UI
{
    internal static class RuntimeCombatHudPresentation
    {
        internal const string OwnerMarker = "LGO Runtime Combat HUD Presentation Helper v1";

        internal static void ApplyAssetState(
            VisualElement cooldownIcon,
            Button localCombatButton,
            Label rangeStatus,
            Label visualState,
            Label feedbackStatus,
            Label cooldownStatus,
            Label authorityStatus,
            bool coolingDown,
            string targetRangeText,
            string feedbackText,
            string cooldownText,
            string authorityText)
        {
            if (cooldownIcon != null)
            {
                var texture = coolingDown ? CombatPlaceholderAssets.CooldownActiveTexture : CombatPlaceholderAssets.CooldownReadyTexture;
                if (texture != null) cooldownIcon.style.backgroundImage = new StyleBackground(texture);
                cooldownIcon.tooltip = coolingDown ? "Hồi chiêu mô phỏng đang chạy." : "Sẵn sàng tấn công thử.";
                RuntimeUiSkin.ApplyCombatCooldownIconState(cooldownIcon, coolingDown);
            }
            if (localCombatButton != null)
            {
                ApplyCombatButtonSkin(localCombatButton, coolingDown ? CombatPlaceholderAssets.CombatButtonCooldownTexture : CombatPlaceholderAssets.CombatButtonNormalTexture, coolingDown);
                var previewingSkill = ContainsAny(feedbackText, "Đang xem");
                localCombatButton.text = coolingDown ? CompactCooldownButtonText(cooldownText) : previewingSkill ? "Thử bia luyện" : "Tấn công thử";
                localCombatButton.tooltip = coolingDown
                    ? "Đang hồi chiêu: bấm vẫn cho phản hồi từ chối hồi chiêu; đây là nguyên mẫu cục bộ, không phải chiến đấu thật."
                    : previewingSkill
                        ? "Rời xem thử kỹ năng để gửi ý định Chém Gió vào bia luyện tập; chỉ là phản hồi nguyên mẫu."
                        : "Gửi ý định Chém Gió vào bia luyện tập; chỉ là phản hồi nguyên mẫu.";
            }

            var warning = ContainsAny(feedbackText, "Ngoài tầm", "Chưa chọn", "Đang hồi chiêu");
            ApplyStatusAccent(rangeStatus, ContainsAny(targetRangeText, "trong tầm") ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Danger);
            ApplyStatusAccent(visualState, coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit);
            if (feedbackStatus != null) feedbackStatus.text = CompactFeedbackStatus(feedbackText);
            ApplyStatusAccent(feedbackStatus, warning ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Gold);
            ApplyStatusAccent(cooldownStatus, coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit);
            ApplyStatusAccent(authorityStatus, ContainsAny(authorityText, "từ chối", "Từ chối") ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Spirit);
        }

        private static string CompactCooldownButtonText(string cooldownText)
        {
            if (string.IsNullOrWhiteSpace(cooldownText)) return "Hồi chiêu";
            var marker = "còn ";
            var index = cooldownText.IndexOf(marker, System.StringComparison.Ordinal);
            return index < 0 ? "Hồi chiêu" : "Hồi " + cooldownText.Substring(index + marker.Length).TrimEnd('.');
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
