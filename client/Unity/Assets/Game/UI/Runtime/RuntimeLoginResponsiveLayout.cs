using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal static class RuntimeLoginResponsiveLayout
    {
        internal static void Apply(
            RuntimeUiLayoutProfile layout,
            RuntimeViewportMetrics viewport,
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
            ApplySafeRootPadding(layout, viewport, root);
            ApplyPanel(layout, authPanel);
            ApplyStage(layout, loginStage, loginGateKeeper, loginNpcGrounding, loginNpcGroundingBloom);
            ApplyControls(layout, loginControlColumn, loginLogo, loginHeroTitle, loginHeroCopy, loginCard, loginServerRow, loginServerText, loginButton, serverSwitchButton);
        }

        private static void ApplySafeRootPadding(RuntimeUiLayoutProfile layout, RuntimeViewportMetrics viewport, VisualElement root)
        {
            if (root == null) return;
            var safe = viewport.SafePanelRect;
            var leftInset = Mathf.Max(0f, safe.xMin);
            var topInset = Mathf.Max(0f, safe.yMin);
            var rightInset = Mathf.Max(0f, viewport.PanelWidth - safe.xMax);
            var bottomInset = Mathf.Max(0f, viewport.PanelHeight - safe.yMax);
            RuntimeUiSkin.ApplyPadding(
                root,
                Mathf.RoundToInt(leftInset) + layout.RootPaddingHorizontal,
                Mathf.RoundToInt(rightInset) + layout.RootPaddingHorizontal,
                Mathf.RoundToInt(topInset) + layout.RootPaddingTop,
                Mathf.RoundToInt(bottomInset) + layout.RootPaddingBottom);
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
                RuntimeUiOverflowGuard.ApplyViewportOverlaySurface(
                    loginStage,
                    RuntimeUiOverlayPlacement.Right,
                    RuntimeUiOverlayVerticalPlacement.Bottom,
                    layout.LoginStageWidth,
                    layout.LoginStageMinHeight,
                    layout.LoginStageRight,
                    layout.LoginStageBottom);
                loginStage.style.minHeight = layout.LoginStageMinHeight;
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
                loginControlColumn.style.justifyContent = Justify.Center;
                if (layout.IsMobile)
                {
                    // LGO Login Mobile Centered Control Overlay v1: mobile login controls share the viewport overlay base instead of a left/top absolute branch.
                    RuntimeUiOverflowGuard.ApplyViewportOverlaySurface(
                        loginControlColumn,
                        RuntimeUiOverlayPlacement.Center,
                        RuntimeUiOverlayVerticalPlacement.Top,
                        layout.LoginMobileControlColumnWidth,
                        layout.LoginMobileControlColumnMaxHeight,
                        layout.LoginMobileControlColumnInsetHorizontal,
                        layout.LoginMobileControlColumnTop);
                }
                else
                {
                    loginControlColumn.style.position = Position.Relative;
                    loginControlColumn.style.top = StyleKeyword.Auto;
                    loginControlColumn.style.left = StyleKeyword.Auto;
                    loginControlColumn.style.right = StyleKeyword.Auto;
                }
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
