using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal static class RuntimeWorldHudResponsiveLayout
    {
        internal const string Marker = "LGO Runtime World HUD Responsive Layout Helper v1";
        internal const string DialogueViewportMarker = "LGO World HUD Dialogue Viewport Polish v1";
        internal const string MobileHierarchyMarker = "LGO World HUD Mobile Hierarchy Polish v1";
        internal const string TopStatusMobileMarker = "LGO World Top Status Mobile Readability v1";

        internal static void ApplyHudPanel(
            RuntimeUiLayoutProfile layout,
            bool worldVisible,
            VisualElement worldHud,
            VisualElement worldGuidanceCard,
            VisualElement dialoguePanel,
            Label dialogueSpeaker,
            Label dialogueLine,
            Label dialogueProgress,
            Button dialogueContinueButton,
            Button dialogueCloseButton)
        {
            if (!worldVisible || worldHud == null) return;
            var mobile = layout.IsMobile;
            var tablet = layout.IsTablet;
            var dialogueVisible = dialoguePanel != null && dialoguePanel.style.display == DisplayStyle.Flex;

            worldHud.style.maxWidth = layout.WorldHudMaxWidth(dialogueVisible);
            worldHud.style.maxHeight = mobile || tablet ? layout.WorldHudMaxHeight(dialogueVisible) : StyleKeyword.None;
            RuntimeUiSkin.ApplyPadding(
                worldHud,
                dialogueVisible ? layout.WorldHudDialoguePaddingHorizontal : layout.WorldHudPaddingHorizontal,
                dialogueVisible ? layout.WorldHudDialoguePaddingVertical : layout.WorldHudPaddingVertical);
            worldHud.style.backgroundColor = RuntimeUiSkin.WorldHudBackground(mobile, tablet, dialogueVisible);

            if (worldGuidanceCard != null)
            {
                RuntimeUiSkin.ApplyVerticalMargin(worldGuidanceCard, layout.WorldGuidanceCardMarginVertical, layout.WorldGuidanceCardMarginVertical);
                RuntimeUiSkin.ApplyPadding(worldGuidanceCard, layout.WorldGuidanceCardPaddingHorizontal, layout.WorldGuidanceCardPaddingHorizontal, layout.WorldGuidanceCardPaddingVertical, layout.WorldGuidanceCardPaddingVertical);
            }

            if (dialoguePanel != null)
            {
                dialoguePanel.style.marginTop = layout.DialoguePanelMarginTop;
                RuntimeUiSkin.ApplyPadding(dialoguePanel, layout.DialoguePanelPaddingHorizontal, layout.DialoguePanelPaddingVertical);
            }
            if (dialogueSpeaker != null)
                dialogueSpeaker.style.fontSize = mobile ? RuntimeUiTypography.DialogueSpeakerMobileFontSize : RuntimeUiTypography.DialogueSpeakerDesktopFontSize;
            if (dialogueLine != null)
                dialogueLine.style.fontSize = mobile ? RuntimeUiTypography.DialogueLineMobileFontSize : RuntimeUiTypography.DialogueLineDesktopFontSize;
            if (dialogueProgress != null)
            {
                dialogueProgress.style.fontSize = mobile ? RuntimeUiTypography.DialogueProgressMobileFontSize : RuntimeUiTypography.DialogueProgressDesktopFontSize;
                RuntimeUiSkin.ApplyPadding(dialogueProgress, layout.DialogueProgressPaddingHorizontal, layout.DialogueProgressPaddingHorizontal, layout.DialogueProgressPaddingVertical, layout.DialogueProgressPaddingVertical);
            }
            if (dialogueContinueButton != null)
            {
                RuntimeUiSkin.ApplyButtonMetrics(
                    dialogueContinueButton,
                    mobile ? RuntimeUiSpacing.DialogueContinueMobileMinWidth : RuntimeUiSpacing.DialogueContinueDesktopMinWidth,
                    mobile ? RuntimeUiSpacing.DialogueButtonMobileMinHeight : RuntimeUiSpacing.DialogueButtonDesktopMinHeight);
            }
            if (dialogueCloseButton != null)
            {
                RuntimeUiSkin.ApplyButtonMetrics(
                    dialogueCloseButton,
                    mobile ? RuntimeUiSpacing.DialogueCloseMobileMinWidth : RuntimeUiSpacing.DialogueCloseDesktopMinWidth,
                    mobile ? RuntimeUiSpacing.DialogueButtonMobileMinHeight : RuntimeUiSpacing.DialogueButtonDesktopMinHeight);
            }
        }

        internal static void ApplyTopStatus(
            RuntimeUiLayoutProfile layout,
            bool worldVisible,
            int viewportWidth,
            VisualElement headerActions,
            Label status,
            Button quitButton)
        {
            var mobile = layout.IsMobile;
            var tablet = layout.IsTablet;
            if (headerActions != null)
            {
                headerActions.style.flexShrink = 1;
                headerActions.style.justifyContent = Justify.FlexEnd;
                headerActions.style.maxWidth = worldVisible && mobile
                    ? Mathf.Max(RuntimeUiSpacing.HeaderActionsMobileMaxWidthFloor, viewportWidth - RuntimeUiSpacing.HeaderActionsMobileViewportInset)
                    : tablet ? RuntimeUiSpacing.HeaderActionsTabletMaxWidth : RuntimeUiSpacing.HeaderActionsDesktopMaxWidth;
            }
            if (status != null)
            {
                status.style.fontSize = worldVisible && mobile
                    ? RuntimeUiTypography.TopStatusWorldMobileFontSize
                    : tablet ? RuntimeUiTypography.TopStatusTabletFontSize : RuntimeUiTypography.TopStatusDefaultFontSize;
                status.style.minHeight = worldVisible && mobile ? RuntimeUiSpacing.TopStatusWorldMobileMinHeight : RuntimeUiSpacing.TopStatusDefaultMinHeight;
                RuntimeUiSkin.ApplyPadding(status, layout.StatusPaddingHorizontal(worldVisible), layout.StatusPaddingHorizontal(worldVisible), layout.StatusPaddingVertical, layout.StatusPaddingVertical);
                status.style.maxWidth = worldVisible && mobile
                    ? Mathf.Clamp(viewportWidth * (RuntimeUiSpacing.TopStatusWorldMobileMaxWidthRatioPercent / 100f), RuntimeUiSpacing.TopStatusWorldMobileMinWidth, RuntimeUiSpacing.TopStatusWorldMobileMaxWidth)
                    : tablet ? RuntimeUiSpacing.TopStatusTabletMaxWidth : RuntimeUiSpacing.TopStatusDesktopMaxWidth;
                if (worldVisible && string.Equals(status.text, "Sẵn sàng: Bước 1 rồi Bước 2.", StringComparison.Ordinal))
                    status.text = "Sẵn sàng: Bước 1/2";
            }
            if (quitButton != null)
            {
                RuntimeUiSkin.ApplyButtonMetrics(
                    quitButton,
                    worldVisible && mobile ? RuntimeUiSpacing.HeaderQuitWorldMobileMinWidth : RuntimeUiSpacing.HeaderQuitDefaultMinWidth,
                    worldVisible && mobile ? RuntimeUiSpacing.HeaderQuitWorldMobileMinHeight : RuntimeUiSpacing.HeaderQuitDefaultMinHeight,
                    worldVisible && mobile ? RuntimeUiSpacing.HeaderQuitWorldMobileFontSize : RuntimeUiSpacing.HeaderQuitDefaultFontSize);
                quitButton.style.marginRight = 0;
            }
        }

        internal static void ApplyLocalVisibility(
            bool showPosition,
            bool showHints,
            bool focusMode,
            bool sessionVisible,
            bool dialogueVisible,
            bool mobileProfile,
            bool tabletProfile,
            bool forceCombatPanel,
            bool hideGuidanceCardOnCompact,
            VisualElement worldHud,
            VisualElement headerActions,
            Label layoutProfileLabel,
            VisualElement worldFooterActions,
            Label position,
            VisualElement worldDebugStrip,
            Label worldMeta,
            VisualElement worldGuidanceCard,
            Label worldArea,
            Label worldStep,
            Label worldDirection,
            Label interactionHint,
            Label worldLandmarks,
            Label worldPoseState,
            Label worldVfxState,
            Label skinSource,
            VisualElement skillPreviewPanel,
            VisualElement localCombatPanel,
            Label toast,
            Label combatVisualState,
            Label combatCooldown,
            Label combatAuthority)
        {
            var compactViewport = mobileProfile || tabletProfile;
            var auxiliaryVisible = !focusMode && !sessionVisible && !dialogueVisible && !compactViewport;
            var gameplayPanelVisible = !sessionVisible && !dialogueVisible && (!compactViewport || forceCombatPanel);
            var compactWorld = compactViewport || focusMode;
            var evidenceHidesGuidance = hideGuidanceCardOnCompact && compactViewport;
            SetElementVisibility(worldHud, !sessionVisible);
            SetElementVisibility(headerActions, !sessionVisible);
            SetDisplayed(layoutProfileLabel, false);
            SetDisplayed(worldFooterActions, !(sessionVisible || mobileProfile));
            SetDisplayed(position, showPosition && !focusMode);
            SetDisplayed(worldDebugStrip, !compactWorld);
            SetDisplayed(worldMeta, !compactWorld);
            SetDisplayed(worldGuidanceCard, !((dialogueVisible && compactViewport) || evidenceHidesGuidance));
            SetDisplayed(worldArea, !compactWorld);
            SetDisplayed(worldStep, showHints && !compactWorld);
            SetDisplayed(worldDirection, showHints && !(mobileProfile && !dialogueVisible));
            SetDisplayed(interactionHint, showHints);
            SetDisplayed(worldLandmarks, showHints && !compactWorld);
            SetDisplayed(worldPoseState, auxiliaryVisible);
            SetDisplayed(worldVfxState, auxiliaryVisible);
            SetDisplayed(skinSource, auxiliaryVisible);
            SetDisplayed(skillPreviewPanel, auxiliaryVisible);
            SetDisplayed(localCombatPanel, gameplayPanelVisible);
            SetDisplayed(toast, !compactWorld);
            SetDisplayed(combatVisualState, auxiliaryVisible);
            SetDisplayed(combatCooldown, auxiliaryVisible);
            SetDisplayed(combatAuthority, auxiliaryVisible);
        }

        private static void SetDisplayed(VisualElement element, bool visible)
        {
            if (element == null) return;
            element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void SetElementVisibility(VisualElement element, bool visible)
        {
            if (element == null) return;
            element.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
