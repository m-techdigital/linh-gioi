using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal static class RuntimeLoginResponsiveLayout
    {
        internal static void Apply(
            RuntimeUiLayoutProfile layout,
            VisualElement root,
            VisualElement authPanel,
            VisualElement loginStage,
            VisualElement loginGateKeeper,
            VisualElement loginNpcGrounding,
            VisualElement loginNpcGroundingBloom,
            VisualElement loginControlColumn,
            VisualElement loginLogo,
            Label loginHeroTitle,
            Label loginHeroCopy,
            VisualElement loginCard,
            VisualElement loginServerRow,
            Label loginServerText,
            Button loginButton,
            Button serverSwitchButton)
        {
            RuntimeUiSkin.ApplyPadding(root, layout.RootPaddingHorizontal, layout.RootPaddingHorizontal, layout.RootPaddingTop, layout.RootPaddingBottom);
            ApplyPanel(layout, authPanel);
            ApplyStage(layout, loginStage, loginGateKeeper, loginNpcGrounding, loginNpcGroundingBloom);
            ApplyControls(layout, loginControlColumn, loginLogo, loginHeroTitle, loginHeroCopy, loginCard, loginServerRow, loginServerText, loginButton, serverSwitchButton);
        }

        private static void ApplyPanel(RuntimeUiLayoutProfile layout, VisualElement authPanel)
        {
            if (authPanel == null) return;
            authPanel.style.minHeight = layout.AuthPanelMinHeight;
            authPanel.style.flexDirection = layout.IsMobile ? FlexDirection.Column : FlexDirection.Row;
            authPanel.style.justifyContent = Justify.FlexStart;
            authPanel.style.alignItems = Align.Center;
            authPanel.style.marginTop = layout.AuthPanelMarginTop;
            RuntimeUiSkin.ApplyPadding(authPanel, 0, 0, layout.AuthPanelPaddingTop, layout.AuthPanelPaddingBottom);
        }

        private static void ApplyStage(
            RuntimeUiLayoutProfile layout,
            VisualElement loginStage,
            VisualElement loginGateKeeper,
            VisualElement loginNpcGrounding,
            VisualElement loginNpcGroundingBloom)
        {
            if (loginStage != null)
            {
                loginStage.style.display = layout.LoginStageDisplay;
                loginStage.style.width = layout.LoginStageWidth;
                loginStage.style.minHeight = layout.LoginStageMinHeight;
                loginStage.style.right = layout.LoginStageRight;
                loginStage.style.bottom = layout.LoginStageBottom;
                loginStage.tooltip = "LGO Login Responsive Scale Cleanup v1";
            }
            if (loginGateKeeper != null)
            {
                loginGateKeeper.style.width = layout.LoginGateKeeperWidth;
                loginGateKeeper.style.height = layout.LoginGateKeeperHeight;
            }
            ApplyGrounding(layout, loginNpcGrounding, false);
            ApplyGrounding(layout, loginNpcGroundingBloom, true);
        }

        private static void ApplyGrounding(RuntimeUiLayoutProfile layout, VisualElement element, bool bloom)
        {
            if (element == null) return;
            element.style.display = layout.LoginNpcGroundingDisplay;
            element.style.width = bloom ? layout.LoginNpcGroundingBloomWidth : layout.LoginNpcGroundingWidth;
            element.style.height = bloom ? layout.LoginNpcGroundingBloomHeight : layout.LoginNpcGroundingHeight;
            element.style.bottom = bloom ? layout.LoginNpcGroundingBloomBottom : layout.LoginNpcGroundingBottom;
            element.style.backgroundColor = bloom ? layout.LoginNpcGroundingBloomColor : layout.LoginNpcGroundingColor;
            element.style.opacity = bloom ? layout.LoginNpcGroundingBloomOpacity : layout.LoginNpcGroundingOpacity;
        }

        private static void ApplyControls(
            RuntimeUiLayoutProfile layout,
            VisualElement loginControlColumn,
            VisualElement loginLogo,
            Label loginHeroTitle,
            Label loginHeroCopy,
            VisualElement loginCard,
            VisualElement loginServerRow,
            Label loginServerText,
            Button loginButton,
            Button serverSwitchButton)
        {
            if (loginControlColumn != null)
            {
                loginControlColumn.style.width = layout.LoginControlColumnWidth;
                loginControlColumn.style.minWidth = layout.LoginControlColumnMinWidth;
                loginControlColumn.style.maxWidth = layout.LoginControlColumnMaxWidth;
                loginControlColumn.style.paddingBottom = layout.LoginControlColumnPaddingBottom;
                loginControlColumn.style.marginLeft = layout.LoginControlColumnMarginLeft;
                loginControlColumn.style.marginTop = layout.LoginControlColumnMarginTop;
            }
            if (loginLogo != null)
            {
                loginLogo.style.width = layout.LoginLogoWidth;
                loginLogo.style.height = layout.LoginLogoHeight;
                loginLogo.style.marginBottom = layout.LoginLogoMarginBottom;
            }
            if (loginHeroTitle != null)
            {
                loginHeroTitle.style.display = DisplayStyle.None;
                loginHeroTitle.style.fontSize = layout.LoginHeroTitleFontSize;
            }
            if (loginHeroCopy != null)
                loginHeroCopy.style.display = DisplayStyle.None;
            if (loginCard != null)
            {
                loginCard.style.maxWidth = layout.LoginCardWidth;
                loginCard.style.minHeight = layout.LoginCardMinHeight;
                RuntimeUiSkin.ApplyPadding(loginCard, layout.LoginCardPadding, layout.LoginCardPadding, layout.LoginCardPaddingTop, layout.LoginCardPaddingBottom);
                loginCard.style.marginBottom = layout.LoginCardMarginBottom;
                loginCard.style.backgroundColor = layout.LoginCardBackground;
                RuntimeUiSkin.ApplyLoginCtaSceneBlend(loginCard);
            }
            if (loginServerRow != null)
            {
                loginServerRow.style.maxWidth = layout.LoginServerRowMaxWidth;
                loginServerRow.style.minHeight = layout.LoginServerRowMinHeight;
                RuntimeUiSkin.ApplyPadding(loginServerRow, layout.LoginServerRowPaddingHorizontal, layout.LoginServerRowPaddingHorizontal, layout.LoginServerRowPaddingVertical, layout.LoginServerRowPaddingVertical);
            }
            if (loginServerText != null)
                loginServerText.style.fontSize = layout.LoginServerTextFontSize;
            if (loginButton != null)
            {
                loginButton.style.minHeight = layout.LoginButtonHeight;
                loginButton.style.fontSize = layout.LoginButtonFontSize;
                loginButton.style.marginTop = layout.LoginButtonMarginTop;
            }
            if (serverSwitchButton != null)
            {
                serverSwitchButton.style.display = DisplayStyle.None;
                serverSwitchButton.style.minHeight = RuntimeUiSizing.LoginServerSwitchMinHeight;
            }
        }
    }
}
