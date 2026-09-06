using UnityEngine;
using UnityEngine.UIElements;
using LinhGioi.Art;

namespace LinhGioi.UI
{
    internal static class RuntimeCharacterHallResponsiveLayout
    {
        internal const string OwnerMarker = "LGO Character Hall Responsive Layout Helper v1";

        internal static void Apply(
            RuntimeUiLayoutProfile layout,
            VisualElement lobbyPanel,
            VisualElement lobbyHeaderBlock,
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
            // LGO Character Hall Selected Compact Header Contract v1: selected mobile follows the target sheet by prioritizing roster/hero/actions over repeated screen prose.
            SetDisplayed(lobbyHeaderBlock, !(layout.IsMobile && hasSelectedCharacter));
            ApplyIntro(layout, lobbyIntro, hasSelectedCharacter);
            RuntimeUiFactory.ApplyCharacterListResponsive(characterList, layout, width, hasSelectedCharacter);
            if (emptyCharacterCard != null)
                RuntimeUiFactory.ApplyEmptyCharacterCardDensity(emptyCharacterCard, layout.CharacterHallDensity);
            ApplyEmptyHint(layout, emptyCharacterHint);
            RuntimeUiFactory.ApplyCharacterHallContentResponsive(lobbyContent, layout);
            RuntimeUiFactory.ApplySelectedCharacterPreviewResponsive(selectedPreview, selectedName, layout, width, hasSelectedCharacter);
            RuntimeUiFactory.ApplyCharacterSelectionStage(lobbyContent, characterList, selectedPreview, layout, hasSelectedCharacter);
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
            VisualElement createBody,
            VisualElement createFooter,
            VisualElement characterActionRow,
            Button createButton,
            Button enterWorldButton)
        {
            var collapsed = hasSelectedCharacter && !createFormExpanded;
            SetDisplayed(createTitle, !collapsed);
            SetDisplayed(createHint, false);
            SetDisplayed(characterName, !collapsed);
            SetDisplayed(classId, false);
            createTitle.text = hasSelectedCharacter ? "Tạo thêm tu sĩ" : "Tạo nhân vật";
            createTitle.style.fontSize = RuntimeUiTypography.SectionTitleMobileFontSize;
            createTitle.style.whiteSpace = WhiteSpace.Normal;
            createTitle.style.marginBottom = 8;
            createPanel.style.position = Position.Relative;
            createPanel.style.left = StyleKeyword.Auto;
            createPanel.style.right = StyleKeyword.Auto;
            createPanel.style.top = StyleKeyword.Auto;
            createPanel.style.bottom = StyleKeyword.Auto;
            createPanel.style.width = Length.Percent(100);
            createPanel.style.minWidth = 0;
            createPanel.style.minHeight = 0;
            createPanel.style.maxHeight = StyleKeyword.None;
            createPanel.style.flexDirection = FlexDirection.Column;
            createPanel.style.flexWrap = Wrap.NoWrap;
            createPanel.style.flexGrow = 1;
            createPanel.style.flexShrink = 1;
            createPanel.style.alignItems = Align.Stretch;
            createPanel.style.alignSelf = Align.Stretch;
            createPanel.style.opacity = 1;
            createPanel.style.marginTop = 0;
            RuntimeUiSkin.ApplyPadding(createPanel, 0, 0);
            RuntimeUiSkin.ApplyFloatingActionBarFrame(createPanel);
            RuntimeUiOverflowGuard.ApplyModalBody(createBody);
            createBody.style.flexDirection = FlexDirection.Column;
            createBody.style.minWidth = 0;
            createBody.style.flexGrow = 1;
            createBody.style.width = Length.Percent(100);
            characterName.style.width = Length.Percent(100);
            characterName.style.minWidth = 0;
            RuntimeUiSkin.ApplyMargin(characterName, 0, 0, 0, 0);
            RuntimeUiOverflowGuard.ApplyModalFooter(createFooter, 8);
            createFooter.style.width = Length.Percent(100);
            createFooter.style.maxWidth = Length.Percent(100);
            createFooter.style.marginLeft = 0;
            createFooter.style.marginTop = StyleKeyword.Auto;
            RuntimeUiOverflowGuard.ApplyResponsiveColumns(characterActionRow, 2, 6, createButton, enterWorldButton);
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
            var cancelling = hasSelectedCharacter && createFormExpanded;
            var entering = hasSelectedCharacter && !createFormExpanded;
            createButton.text = entering ? "Tạo thêm" : "Tạo tu sĩ";
            enterWorldButton.text = cancelling ? "Hủy" : "Vào sân luyện";
            enterWorldButton.tooltip = cancelling ? "Đóng form và giữ nhân vật đang chọn."
                : hasSelectedCharacter ? "Bước qua Linh Môn vào sân luyện." : "Chọn hoặc tạo tu sĩ trước khi vào sân luyện.";
            createButton.style.display = DisplayStyle.Flex;
            enterWorldButton.style.display = DisplayStyle.Flex;
            createButton.style.opacity = 1;
            enterWorldButton.style.opacity = hasSelectedCharacter ? 1 : 0.46f;
            RuntimeUiFactory.ApplyCompactButtonPriority(createButton, !entering);
            RuntimeUiFactory.ApplyCompactButtonPriority(enterWorldButton, entering);
            RuntimeUiSkin.ApplyButtonTier(createButton, RuntimeUiButtonTier.Compact);
            RuntimeUiSkin.ApplyButtonTier(enterWorldButton, RuntimeUiButtonTier.Compact);
            characterActionRow.Clear();
            characterActionRow.Add(entering ? enterWorldButton : createButton);
            characterActionRow.Add(entering ? createButton : enterWorldButton);
            RuntimeUiOverflowGuard.ApplyResponsiveColumns(characterActionRow, 2, 6,
                entering ? enterWorldButton : createButton, entering ? createButton : enterWorldButton);
        }

        internal static void ApplySelectedDetails(RuntimeUiLayoutProfile layout, bool hasSelectedCharacter, Label selectedStatus, Label selectedObjective)
        {
            // The form or selected profile owns this column; do not repeat onboarding prose above it.
            var showDetails = false;
            if (selectedStatus != null) selectedStatus.style.display = showDetails ? DisplayStyle.Flex : DisplayStyle.None;
            if (selectedObjective != null) selectedObjective.style.display = showDetails ? DisplayStyle.Flex : DisplayStyle.None;
        }

        internal static void ApplyCreatePreviewVisibility(VisualElement preview, bool editingInPreviewColumn)
        {
            if (preview != null)
            {
                preview.style.visibility = Visibility.Visible;
                SetDisplayed(preview.Q<VisualElement>("LGO Character Hall Selected Profile Hero V3B"), !editingInPreviewColumn);
            }
        }

        internal static void ApplyCreateValidationFeedback(Label title, Label hint, string error)
        {
            if (hint == null) return;
            hint.style.color = error == null ? RuntimeArtCatalog.Muted : RuntimeArtCatalog.Danger;
            if (error == null) return;
            if (title != null) title.style.display = DisplayStyle.None;
            hint.text = error;
            hint.style.minWidth = 0;
            hint.style.maxWidth = Length.Percent(100);
            hint.style.whiteSpace = WhiteSpace.Normal;
            hint.style.display = DisplayStyle.Flex;
        }

        private static void ApplyPanel(RuntimeUiLayoutProfile layout, int width, int height, VisualElement lobbyPanel)
        {
            if (lobbyPanel == null) return;
            // LGO Character Hall Mobile Full Safe Shell v1: mobile landscape follows the demo container bounds instead of a narrow fixed panel-space clamp.
            lobbyPanel.style.width = Length.Percent(100);
            lobbyPanel.style.maxWidth = layout.IsMobile ? Length.Percent(100)
                : layout.IsTablet ? RuntimeUiSizing.CharacterHallTabletPanelMaxWidth : RuntimeUiSizing.CharacterHallPanelMaxWidth;
            lobbyPanel.style.minHeight = layout.IsMobile ? layout.CharacterHallPanelMaxHeight : 0;
            lobbyPanel.style.maxHeight = layout.IsMobile ? layout.CharacterHallPanelMaxHeight
                : Mathf.Max(0, height - 2 * (layout.RootPaddingTop + layout.HeaderMinHeight(false)));
            RuntimeUiSkin.ApplyPadding(lobbyPanel, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingTop, layout.LobbyPanelPaddingBottom);
        }

        private static void ApplyIntro(RuntimeUiLayoutProfile layout, Label lobbyIntro, bool hasSelectedCharacter)
        {
            if (lobbyIntro == null) return;
            // LGO Character Hall Mobile Copy Density v1: mobile keeps intent, drops prose.
            lobbyIntro.text = layout.IsMobile ? "Chọn tu sĩ, rồi vào sân luyện." : "Chọn tu sĩ để bước qua Linh Môn. Hồ sơ sẽ được chuẩn bị cho phiên hiện tại.";
            lobbyIntro.style.display = layout.IsMobile && hasSelectedCharacter ? DisplayStyle.None : DisplayStyle.Flex;
            lobbyIntro.style.fontSize = layout.IsMobile ? RuntimeUiTypography.LobbyIntroMobileFontSize : RuntimeUiTypography.LobbyIntroDesktopFontSize;
            lobbyIntro.style.marginBottom = layout.LobbyIntroMarginBottom;
        }

        private static void SetDisplayed(VisualElement element, bool visible)
        {
            if (element != null) element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void ApplyEmptyHint(RuntimeUiLayoutProfile layout, Label emptyCharacterHint)
        {
            if (emptyCharacterHint == null) return;
            emptyCharacterHint.text = layout.IsMobile ? "Hồ sơ sẽ hiện tại đây." : "Sau khi tạo, hồ sơ sẽ xuất hiện tại đây để chọn và vào sân luyện.";
            emptyCharacterHint.style.fontSize = layout.IsMobile ? RuntimeUiTypography.EmptyCharacterHintMobileFontSize : RuntimeUiTypography.EmptyCharacterHintDesktopFontSize;
        }

    }
}
