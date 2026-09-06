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
            Label worldHudHeaderTitle,
            VisualElement worldGuidanceCard,
            VisualElement skillPreviewPanel,
            VisualElement localCombatPanel,
            VisualElement dialoguePanel,
            Label dialogueSpeaker,
            Label dialogueLine,
            Label dialogueProgress,
            VisualElement dialogueActionRow,
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

            if (worldHudHeaderTitle != null)
            {
                worldHudHeaderTitle.style.fontSize = mobile
                    ? RuntimeUiTypography.SectionTitleMobileFontSize
                    : tablet ? RuntimeUiTypography.SectionTitleFontSize : 18;
                worldHudHeaderTitle.style.maxWidth = Length.Percent(100);
                worldHudHeaderTitle.style.minWidth = 0;
                worldHudHeaderTitle.style.flexShrink = 1;
                worldHudHeaderTitle.style.whiteSpace = mobile ? WhiteSpace.Normal : WhiteSpace.NoWrap;
                worldHudHeaderTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
            }

            if (worldGuidanceCard != null)
            {
                ApplyMobileHudChildConstraint(layout, worldGuidanceCard);
                RuntimeUiSkin.ApplyVerticalMargin(worldGuidanceCard, layout.WorldGuidanceCardMarginVertical, layout.WorldGuidanceCardMarginVertical);
                RuntimeUiSkin.ApplyPadding(worldGuidanceCard, layout.WorldGuidanceCardPaddingHorizontal, layout.WorldGuidanceCardPaddingHorizontal, layout.WorldGuidanceCardPaddingVertical, layout.WorldGuidanceCardPaddingVertical);
            }
            ApplyMobileHudChildConstraint(layout, skillPreviewPanel);
            ApplyMobileHudChildConstraint(layout, localCombatPanel);

            if (dialoguePanel != null)
            {
                ApplyMobileHudChildConstraint(layout, dialoguePanel);
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
            if (dialogueActionRow != null)
            {
                dialogueActionRow.style.flexWrap = mobile ? Wrap.NoWrap : Wrap.Wrap;
                dialogueActionRow.style.width = mobile ? Length.Percent(100) : StyleKeyword.Auto;
                dialogueActionRow.style.maxWidth = mobile ? Length.Percent(100) : StyleKeyword.None;
            }
            if (dialogueContinueButton != null)
            {
                RuntimeUiSkin.ApplyButtonMetrics(
                    dialogueContinueButton,
                    mobile ? RuntimeUiSpacing.DialogueContinueMobileMinWidth : RuntimeUiSpacing.DialogueContinueDesktopMinWidth,
                    mobile ? RuntimeUiSpacing.DialogueButtonMobileMinHeight : RuntimeUiSpacing.DialogueButtonDesktopMinHeight);
                if (mobile) ApplyMobileDialogueActionButton(dialogueContinueButton, 4);
            }
            if (dialogueCloseButton != null)
            {
                RuntimeUiSkin.ApplyButtonMetrics(
                    dialogueCloseButton,
                    mobile ? RuntimeUiSpacing.DialogueCloseMobileMinWidth : RuntimeUiSpacing.DialogueCloseDesktopMinWidth,
                    mobile ? RuntimeUiSpacing.DialogueButtonMobileMinHeight : RuntimeUiSpacing.DialogueButtonDesktopMinHeight);
                if (mobile) ApplyMobileDialogueActionButton(dialogueCloseButton, 0);
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
                status.style.flexShrink = worldVisible && mobile ? 0 : 1;
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
            bool skillPreviewActive,
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
            var defaultGameplayPanelVisible = !sessionVisible && !dialogueVisible && !compactViewport && !focusMode;
            var combatPanelVisible = !sessionVisible && !dialogueVisible && (forceCombatPanel || defaultGameplayPanelVisible);
            var skillPreviewPanelVisible = !sessionVisible && !dialogueVisible && !mobileProfile && (skillPreviewActive || defaultGameplayPanelVisible);
            var compactWorld = compactViewport || focusMode;
            var evidenceHidesGuidance = hideGuidanceCardOnCompact && compactViewport;
            SetElementVisibility(worldHud, !sessionVisible);
            SetElementVisibility(headerActions, !sessionVisible);
            SetDisplayed(layoutProfileLabel, false);
            SetDisplayed(worldFooterActions, !(sessionVisible || mobileProfile || dialogueVisible || skillPreviewActive || forceCombatPanel));
            SetDisplayed(position, showPosition && !focusMode);
            SetDisplayed(worldDebugStrip, !compactWorld);
            SetDisplayed(worldMeta, !compactWorld);
            SetDisplayed(worldGuidanceCard, !((dialogueVisible && compactViewport) || skillPreviewActive || evidenceHidesGuidance));
            SetDisplayed(worldArea, !compactWorld);
            SetDisplayed(worldStep, showHints && !compactWorld);
            SetDisplayed(worldDirection, showHints && !(mobileProfile && !dialogueVisible));
            SetDisplayed(interactionHint, showHints);
            SetDisplayed(worldLandmarks, showHints && !compactWorld);
            SetDisplayed(worldPoseState, auxiliaryVisible);
            SetDisplayed(worldVfxState, auxiliaryVisible);
            SetDisplayed(skinSource, auxiliaryVisible);
            SetDisplayed(skillPreviewPanel, skillPreviewPanelVisible);
            SetDisplayed(localCombatPanel, combatPanelVisible);
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

        private static void ApplyMobileHudChildConstraint(RuntimeUiLayoutProfile layout, VisualElement element)
        {
            if (!layout.IsMobile || element == null) return;
            element.style.minWidth = 0;
            element.style.width = Length.Percent(100);
            element.style.maxWidth = Length.Percent(100);
            element.style.flexShrink = 1;
        }

        private static void ApplyMobileDialogueActionButton(Button button, int marginRight)
        {
            // LGO Mobile Dialogue Action Row Fit v1: two dialogue actions stay in one touchable row inside the HUD parent.
            button.style.width = StyleKeyword.Auto;
            button.style.maxWidth = StyleKeyword.None;
            button.style.flexBasis = StyleKeyword.Auto;
            button.style.flexGrow = 1;
            button.style.flexShrink = 1;
            button.style.marginRight = marginRight;
        }

        private static void SetElementVisibility(VisualElement element, bool visible)
        {
            if (element == null) return;
            element.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
