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
                localCombatButton.text = coolingDown ? "Hồi chiêu" : "Tấn công thử";
                localCombatButton.tooltip = coolingDown
                    ? "Đang hồi chiêu: bấm vẫn cho phản hồi từ chối hồi chiêu; đây là nguyên mẫu cục bộ, không phải chiến đấu thật."
                    : "Gửi ý định Chém Gió vào bia luyện tập; chỉ là phản hồi nguyên mẫu.";
            }

            var warning = ContainsAny(feedbackText, "Ngoài tầm", "Chưa chọn", "Đang hồi chiêu");
            ApplyStatusAccent(rangeStatus, ContainsAny(targetRangeText, "trong tầm") ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Danger);
            ApplyStatusAccent(visualState, coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit);
            ApplyStatusAccent(feedbackStatus, warning ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Gold);
            ApplyStatusAccent(cooldownStatus, coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit);
            ApplyStatusAccent(authorityStatus, ContainsAny(authorityText, "từ chối", "Từ chối") ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Spirit);
        }

        internal static string CompactTargetStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Bia luyện: chưa rõ";
            return value.Replace("Mục tiêu luyện tập", "Bia luyện");
        }

        internal static string CompactRangeStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Tầm: chưa rõ";
            return value.Replace("Tầm đánh", "Tầm");
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
