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
            Label selectedName,
            VisualElement createPanel,
            bool hasSelectedCharacter)
        {
            var width = layout.Width;
            var height = layout.Height;
            ApplyPanel(layout, width, height, lobbyPanel);
            ApplyIntro(layout, lobbyIntro);
            ApplyCreatePanelOrder(layout, lobbyPanel, lobbyContent, createPanel, hasSelectedCharacter);
            RuntimeUiFactory.ApplyCharacterListResponsive(characterList, layout, width);
            if (emptyCharacterCard != null)
                RuntimeUiFactory.ApplyEmptyCharacterCardDensity(emptyCharacterCard, layout.CharacterHallDensity);
            ApplyEmptyHint(layout, emptyCharacterHint);
            RuntimeUiFactory.ApplyCharacterHallContentResponsive(lobbyContent, layout);
            RuntimeUiFactory.ApplySelectedCharacterPreviewResponsive(selectedPreview, selectedName, layout, width);
            ApplyCreatePanel(layout, width, createPanel);
        }

        internal static void ApplyCreateFormState(
            RuntimeUiLayoutProfile layout,
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
            var compactStandaloneCreate = !hasSelectedCharacter && !isMobileProfile;
            var desktopStandaloneCreate = compactStandaloneCreate && !layout.IsTablet;
            if (createTitle != null)
            {
                createTitle.text = collapsed ? "Sẵn sàng" : hasSelectedCharacter ? "Tạo thêm tu sĩ" : "Khai mở tu sĩ";
                createTitle.style.display = collapsed && isMobileProfile ? DisplayStyle.None : DisplayStyle.Flex;
                createTitle.style.marginBottom = collapsed ? 2 : compactStandaloneCreate ? 4 : 8;
                createTitle.style.marginRight = desktopStandaloneCreate ? 18 : 0;
                createTitle.style.unityTextAlign = collapsed && !isMobileProfile ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;
                createTitle.style.width = compactStandaloneCreate && !desktopStandaloneCreate ? Length.Percent(100) : StyleKeyword.Auto;
            }
            if (createHint != null)
            {
                var showDesktopHint = !isMobileProfile && !layout.IsTablet && !collapsed && !compactStandaloneCreate;
                createHint.text = collapsed
                    ? "Tu sĩ đã sẵn sàng."
                    : hasSelectedCharacter
                        ? "Nhập danh xưng mới nếu muốn tạo thêm hồ sơ."
                        : "Danh xưng tu sĩ - Mạch khởi đầu: Kiếm tu sơ nhập.";
                createHint.style.display = showDesktopHint ? DisplayStyle.Flex : DisplayStyle.None;
            }
            if (characterName != null)
            {
                characterName.style.display = collapsed ? DisplayStyle.None : DisplayStyle.Flex;
                characterName.style.alignSelf = !hasSelectedCharacter && !isMobileProfile ? Align.Center : Align.Stretch;
                characterName.style.width = compactStandaloneCreate ? Mathf.Clamp(layout.Width * 0.32f, 280f, RuntimeUiSizing.CharacterNameFieldMaxWidth) : Length.Percent(100);
            }
            if (classId != null) classId.style.display = DisplayStyle.None;
            if (createPanel != null)
            {
                // LGO Character Hall Standalone Create Viewport Fit v1: desktop/tablet empty-state form shares one compact row so it stays inside the safe panel height.
                createPanel.style.flexDirection = collapsed && !isMobileProfile || compactStandaloneCreate ? FlexDirection.Row : FlexDirection.Column;
                createPanel.style.flexWrap = compactStandaloneCreate && !desktopStandaloneCreate ? Wrap.Wrap : Wrap.NoWrap;
                createPanel.style.alignItems = collapsed && !isMobileProfile || compactStandaloneCreate ? Align.Center : Align.Stretch;
                createPanel.style.alignSelf = !hasSelectedCharacter && !isMobileProfile ? Align.Center : Align.Stretch;
                createPanel.style.width = layout.IsMobile
                    ? collapsed ? Mathf.Clamp(layout.Width * 0.34f, 300f, 336f) : Mathf.Clamp(layout.Width * 0.36f, 320f, 360f)
                    : !hasSelectedCharacter && !isMobileProfile ? Length.Percent(RuntimeUiSizing.CharacterCreateStandalonePanelWidthPercent) : Length.Percent(100);
                createPanel.style.opacity = collapsed ? 0.72f : hasSelectedCharacter ? (isMobileProfile ? 0.82f : 0.88f) : 1f;
                createPanel.style.minHeight = collapsed ? (isMobileProfile ? 62 : 96) : desktopStandaloneCreate ? 100 : compactStandaloneCreate ? 104 : RuntimeUiSizing.CharacterCreatePanelMinHeight;
                createPanel.style.maxHeight = collapsed ? (isMobileProfile ? 72 : 108) : desktopStandaloneCreate ? 118 : compactStandaloneCreate ? 126 : RuntimeUiSizing.CharacterCreatePanelMaxHeight;
                if (layout.IsMobile && collapsed)
                {
                    // LGO Character Hall Selected Action Anchor v1: selected state becomes a stable action dock instead of a floating mid-screen card.
                    createPanel.style.position = Position.Absolute;
                    createPanel.style.left = StyleKeyword.Auto;
                    createPanel.style.right = 14;
                    createPanel.style.top = StyleKeyword.Auto;
                    createPanel.style.bottom = 28;
                }
                else if (!layout.IsMobile)
                {
                    createPanel.style.position = Position.Relative;
                    createPanel.style.left = 0;
                    createPanel.style.right = StyleKeyword.Auto;
                    createPanel.style.top = StyleKeyword.Auto;
                    createPanel.style.bottom = StyleKeyword.Auto;
                }
            }
            if (characterActionRow != null)
            {
                characterActionRow.style.marginLeft = collapsed && !isMobileProfile ? 18 : compactStandaloneCreate ? 12 : 0;
                characterActionRow.style.marginTop = collapsed && isMobileProfile ? 0 : compactStandaloneCreate ? 0 : 6;
                characterActionRow.style.flexGrow = collapsed && !isMobileProfile || compactStandaloneCreate ? 1 : 0;
                characterActionRow.style.justifyContent = !hasSelectedCharacter && !isMobileProfile ? Justify.Center : Justify.FlexStart;
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

        private static void ApplyCreatePanelOrder(
            RuntimeUiLayoutProfile layout,
            VisualElement lobbyPanel,
            VisualElement lobbyContent,
            VisualElement createPanel,
            bool hasSelectedCharacter)
        {
            if (layout.IsMobile || lobbyPanel == null || lobbyContent == null || createPanel == null) return;
            if (lobbyContent.parent != lobbyPanel || createPanel.parent != lobbyPanel) return;

            var createFirst = !hasSelectedCharacter && !layout.IsTablet;
            var contentIndex = lobbyPanel.IndexOf(lobbyContent);
            var createIndex = lobbyPanel.IndexOf(createPanel);
            if (contentIndex < 0 || createIndex < 0) return;
            if (createFirst && createIndex < contentIndex) return;
            if (!createFirst && createIndex > contentIndex) return;

            createPanel.RemoveFromHierarchy();
            contentIndex = lobbyPanel.IndexOf(lobbyContent);
            if (contentIndex < 0)
            {
                lobbyPanel.Add(createPanel);
                return;
            }

            var targetIndex = createFirst ? contentIndex : Mathf.Min(contentIndex + 1, lobbyPanel.childCount);
            lobbyPanel.Insert(targetIndex, createPanel);
        }
    }
}
