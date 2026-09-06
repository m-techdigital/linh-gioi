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

        internal static void ApplyCreateFormState(
            bool isMobileProfile,
            bool hasSelectedCharacter,
            bool createFormExpanded,
            Label createTitle,
            Label createHint,
            TextField characterName,
            TextField classId,
            VisualElement createPanel,
            VisualElement characterActionRow)
        {
            var collapsed = hasSelectedCharacter && !createFormExpanded;
            if (createTitle != null)
            {
                createTitle.text = collapsed ? "Sẵn sàng" : hasSelectedCharacter ? "Tạo thêm tu sĩ" : "Khai mở tu sĩ";
                createTitle.style.marginBottom = collapsed ? 2 : 8;
                createTitle.style.unityTextAlign = collapsed && !isMobileProfile ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;
            }
            if (createHint != null)
            {
                createHint.text = collapsed
                    ? "Tu sĩ đã sẵn sàng."
                    : hasSelectedCharacter
                        ? "Nhập danh xưng mới nếu muốn tạo thêm hồ sơ."
                        : "Đặt danh xưng, chọn mạch khởi đầu, rồi bước qua Linh Môn.";
                createHint.style.display = (!isMobileProfile && !collapsed) ? DisplayStyle.Flex : DisplayStyle.None;
            }
            if (characterName != null)
            {
                characterName.style.display = collapsed ? DisplayStyle.None : DisplayStyle.Flex;
                characterName.style.alignSelf = !hasSelectedCharacter && !isMobileProfile ? Align.Center : Align.Stretch;
                characterName.style.width = !hasSelectedCharacter && !isMobileProfile ? Length.Percent(64) : Length.Percent(100);
            }
            if (classId != null) classId.style.display = DisplayStyle.None;
            if (createPanel != null)
            {
                createPanel.style.flexDirection = collapsed && !isMobileProfile ? FlexDirection.Row : FlexDirection.Column;
                createPanel.style.alignItems = collapsed && !isMobileProfile ? Align.Center : Align.Stretch;
                createPanel.style.alignSelf = !hasSelectedCharacter && !isMobileProfile ? Align.Center : Align.Stretch;
                createPanel.style.width = !hasSelectedCharacter && !isMobileProfile ? Length.Percent(82) : Length.Percent(100);
                createPanel.style.opacity = collapsed ? 0.72f : hasSelectedCharacter ? (isMobileProfile ? 0.82f : 0.88f) : 1f;
                createPanel.style.minHeight = collapsed ? (isMobileProfile ? 76 : 96) : RuntimeUiSizing.CharacterCreatePanelMinHeight;
                createPanel.style.maxHeight = collapsed ? (isMobileProfile ? 86 : 108) : RuntimeUiSizing.CharacterCreatePanelMaxHeight;
            }
            if (characterActionRow != null)
            {
                characterActionRow.style.marginLeft = collapsed && !isMobileProfile ? 18 : 0;
                characterActionRow.style.flexGrow = collapsed && !isMobileProfile ? 1 : 0;
            }
        }

        internal static void ApplyActionHierarchy(
            bool isMobileProfile,
            bool hasSelectedCharacter,
            bool createFormExpanded,
            VisualElement characterActionRow,
            Button createButton,
            Button enterWorldButton)
        {
            if (characterActionRow == null || createButton == null || enterWorldButton == null) return;
            var mobileSelected = isMobileProfile && hasSelectedCharacter;
            characterActionRow.Clear();
            if (hasSelectedCharacter)
            {
                // LGO Character Hall Mobile Selected CTA Hierarchy v1: enter-world owns the selected state on every profile.
                enterWorldButton.text = "Vào sân luyện";
                RuntimeUiSkin.ApplyButtonMetrics(
                    enterWorldButton,
                    mobileSelected ? RuntimeUiSpacing.CharacterSelectedPrimaryMobileMinWidth : RuntimeUiSpacing.CharacterActionButtonMinWidth,
                    mobileSelected ? RuntimeUiSpacing.CharacterSelectedPrimaryMobileMinHeight : RuntimeUiSpacing.CharacterActionButtonMinHeight,
                    mobileSelected ? RuntimeUiSpacing.CharacterSelectedPrimaryMobileFontSize : RuntimeUiSpacing.CharacterEnterWorldButtonFontSize,
                    true);
                enterWorldButton.style.marginTop = RuntimeUiSpacing.CharacterSelectedPrimaryMobileMarginTop;
                enterWorldButton.style.opacity = 1f;
                enterWorldButton.tooltip = "Bước qua Linh Môn vào sân luyện.";
                createButton.text = createFormExpanded ? "Tạo tu sĩ" : "Tạo thêm";
                RuntimeUiSkin.ApplyButtonMetrics(
                    createButton,
                    mobileSelected ? RuntimeUiSpacing.CharacterSelectedSecondaryMobileMinWidth : RuntimeUiSpacing.CharacterActionButtonMinWidth,
                    mobileSelected ? RuntimeUiSpacing.CharacterSelectedSecondaryMobileMinHeight : RuntimeUiSpacing.CharacterActionButtonMinHeight,
                    mobileSelected ? RuntimeUiSpacing.CharacterSelectedSecondaryMobileFontSize : RuntimeUiSpacing.CharacterCreateButtonFontSize);
                createButton.style.opacity = 0.82f;
                characterActionRow.Add(enterWorldButton);
                characterActionRow.Add(createButton);
                return;
            }

            createButton.text = "Tạo tu sĩ";
            RuntimeUiSkin.ApplyButtonMetrics(
                createButton,
                RuntimeUiSpacing.CharacterActionButtonMinWidth,
                RuntimeUiSpacing.CharacterActionButtonMinHeight,
                RuntimeUiSpacing.CharacterCreateButtonFontSize);
            createButton.style.opacity = 1f;
            enterWorldButton.text = "Vào sân luyện";
            RuntimeUiSkin.ApplyButtonMetrics(
                enterWorldButton,
                RuntimeUiSpacing.CharacterActionButtonMinWidth,
                RuntimeUiSpacing.CharacterActionButtonMinHeight,
                RuntimeUiSpacing.CharacterEnterWorldButtonFontSize,
                true);
            enterWorldButton.style.opacity = 0.46f;
            enterWorldButton.tooltip = "Chọn hoặc tạo tu sĩ trước khi vào sân luyện.";
            characterActionRow.Add(createButton);
            characterActionRow.Add(enterWorldButton);
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
