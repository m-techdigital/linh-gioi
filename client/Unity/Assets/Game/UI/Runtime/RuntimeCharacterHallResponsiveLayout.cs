using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal static class RuntimeCharacterHallResponsiveLayout
    {
        internal const string OwnerMarker = "LGO Character Hall Responsive Layout Helper v1";

        internal static void Apply(
            RuntimeUiLayoutProfile layout,
            VisualElement lobbyPanel,
            Label lobbyIntro,
            VisualElement characterList,
            VisualElement emptyCharacterCard,
            Label emptyCharacterHint,
            VisualElement lobbyContent,
            VisualElement selectedPreview,
            VisualElement createPanel)
        {
            var width = layout.Width;
            var height = layout.Height;
            ApplyPanel(layout, width, height, lobbyPanel);
            ApplyIntro(layout, lobbyIntro);
            RuntimeUiFactory.ApplyCharacterListResponsive(characterList, layout, width);
            if (emptyCharacterCard != null)
                RuntimeUiFactory.ApplyEmptyCharacterCardDensity(emptyCharacterCard, layout.CharacterHallDensity);
            ApplyEmptyHint(layout, emptyCharacterHint);
            RuntimeUiFactory.ApplyCharacterHallContentResponsive(lobbyContent, layout);
            RuntimeUiFactory.ApplySelectedCharacterPreviewResponsive(selectedPreview, layout, width);
            ApplyCreatePanel(layout, width, createPanel);
        }

        private static void ApplyPanel(RuntimeUiLayoutProfile layout, int width, int height, VisualElement lobbyPanel)
        {
            if (lobbyPanel == null) return;
            lobbyPanel.style.maxWidth = layout.IsMobile
                ? Mathf.Min(width - 40f, 780f)
                : layout.IsTablet ? RuntimeUiSizing.CharacterHallTabletPanelMaxWidth : RuntimeUiSizing.CharacterHallPanelMaxWidth;
            lobbyPanel.style.minHeight = layout.IsMobile ? Mathf.Max(292f, height - 48f) : 410;
            RuntimeUiSkin.ApplyPadding(lobbyPanel, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingTop, layout.LobbyPanelPaddingBottom);
        }

        private static void ApplyIntro(RuntimeUiLayoutProfile layout, Label lobbyIntro)
        {
            if (lobbyIntro == null) return;
            // LGO Character Hall Mobile Copy Density v1: mobile keeps intent, drops prose.
            lobbyIntro.text = layout.IsMobile ? "Chọn tu sĩ, rồi vào sân luyện." : "Chọn tu sĩ để bước qua Linh Môn. Hồ sơ sẽ được chuẩn bị cho phiên hiện tại.";
            lobbyIntro.style.fontSize = layout.IsMobile ? RuntimeUiTypography.LobbyIntroMobileFontSize : RuntimeUiTypography.LobbyIntroDesktopFontSize;
            lobbyIntro.style.marginBottom = layout.LobbyIntroMarginBottom;
        }

        private static void ApplyEmptyHint(RuntimeUiLayoutProfile layout, Label emptyCharacterHint)
        {
            if (emptyCharacterHint == null) return;
            emptyCharacterHint.text = layout.IsMobile ? "Hồ sơ sẽ hiện tại đây." : "Sau khi tạo, hồ sơ sẽ xuất hiện tại đây để chọn và vào sân luyện.";
            emptyCharacterHint.style.fontSize = layout.IsMobile ? RuntimeUiTypography.EmptyCharacterHintMobileFontSize : RuntimeUiTypography.EmptyCharacterHintDesktopFontSize;
        }

        private static void ApplyCreatePanel(RuntimeUiLayoutProfile layout, int width, VisualElement createPanel)
        {
            if (createPanel == null) return;
            createPanel.style.position = layout.IsMobile ? Position.Absolute : Position.Relative;
            createPanel.style.left = layout.IsMobile ? Mathf.Clamp(width * 0.45f, 350f, 390f) : 0;
            createPanel.style.right = layout.IsMobile ? 12 : StyleKeyword.Auto;
            createPanel.style.top = layout.IsMobile ? 132 : StyleKeyword.Auto;
            RuntimeUiSkin.ApplyPadding(createPanel, layout.CreatePanelPaddingHorizontal, layout.CreatePanelPaddingHorizontal, layout.CreatePanelPaddingTop, layout.CreatePanelPaddingBottom);
            createPanel.style.marginTop = layout.CreatePanelMarginTop;
            createPanel.style.maxHeight = layout.IsMobile ? 174 : RuntimeUiSizing.CharacterCreatePanelMaxHeight;
        }
    }
}
