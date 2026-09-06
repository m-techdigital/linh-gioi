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
            VisualElement dialogueSpeakerHeader,
            VisualElement dialogueSpeakerPortrait,
            Label dialogueSpeaker,
            VisualElement dialogueBody,
            ScrollView dialogueLineScroll,
            VisualElement dialogueFooter,
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

            worldHud.style.minWidth = layout.WorldHudMinWidthFor(false);
            worldHud.style.maxWidth = layout.WorldHudMaxWidth(false);
            worldHud.style.maxHeight = mobile || tablet ? layout.WorldHudMaxHeight(false) : StyleKeyword.None;
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
                ApplyDialogueOverlay(layout, dialoguePanel);
                dialoguePanel.style.maxHeight = layout.DialoguePanelMaxHeight;
                dialoguePanel.style.height = layout.DialoguePanelMaxHeight;
                dialoguePanel.style.overflow = Overflow.Hidden;
                RuntimeUiSkin.ApplyPadding(dialoguePanel, layout.DialoguePanelPaddingHorizontal, layout.DialoguePanelPaddingVertical);
            }
            RuntimeUiOverflowGuard.ApplyModalBody(dialogueBody);
            RuntimeUiOverflowGuard.ApplyModalFooter(dialogueFooter, layout.DialogueContentGap);
            RuntimeUiOverflowGuard.ApplyBoundedScroll(dialogueLineScroll, layout.DialogueLineScrollMaxHeight, layout.DialogueLineScrollMinHeight);
            if (dialogueSpeakerHeader != null)
            {
                dialogueSpeakerHeader.style.flexDirection = FlexDirection.Row;
                dialogueSpeakerHeader.style.alignItems = Align.Center;
                dialogueSpeakerHeader.style.marginBottom = layout.DialogueContentGap;
                RuntimeUiOverflowGuard.ApplyBoundedActionRow(dialogueSpeakerHeader);
            }
            if (dialogueSpeakerPortrait != null)
            {
                dialogueSpeakerPortrait.style.width = layout.DialogueSpeakerPortraitSize;
                dialogueSpeakerPortrait.style.height = layout.DialogueSpeakerPortraitSize;
                dialogueSpeakerPortrait.style.minWidth = layout.DialogueSpeakerPortraitSize;
                dialogueSpeakerPortrait.style.minHeight = layout.DialogueSpeakerPortraitSize;
                dialogueSpeakerPortrait.style.marginRight = layout.DialogueContentGap;
                dialogueSpeakerPortrait.style.flexShrink = 0;
            }
            if (dialogueSpeaker != null)
            {
                dialogueSpeaker.style.fontSize = mobile ? RuntimeUiTypography.DialogueSpeakerMobileFontSize : RuntimeUiTypography.DialogueSpeakerDesktopFontSize;
                dialogueSpeaker.style.marginBottom = 0;
                dialogueSpeaker.style.minWidth = 0;
                dialogueSpeaker.style.flexShrink = 1;
                dialogueSpeaker.style.whiteSpace = WhiteSpace.Normal;
            }
            if (dialogueLine != null)
            {
                dialogueLine.style.fontSize = mobile ? RuntimeUiTypography.DialogueLineMobileFontSize : RuntimeUiTypography.DialogueLineDesktopFontSize;
                dialogueLine.style.minWidth = 0;
                dialogueLine.style.maxWidth = Length.Percent(100);
                dialogueLine.style.whiteSpace = WhiteSpace.Normal;
                dialogueLine.style.flexShrink = 1;
            }
            if (dialogueProgress != null)
            {
                dialogueProgress.style.fontSize = mobile ? RuntimeUiTypography.DialogueProgressMobileFontSize : RuntimeUiTypography.DialogueProgressDesktopFontSize;
                dialogueProgress.style.marginTop = layout.DialogueContentGap;
                dialogueProgress.style.marginBottom = layout.DialogueContentGap;
                RuntimeUiSkin.ApplyPadding(dialogueProgress, layout.DialogueProgressPaddingHorizontal, layout.DialogueProgressPaddingHorizontal, layout.DialogueProgressPaddingVertical, layout.DialogueProgressPaddingVertical);
            }
            if (dialogueActionRow != null)
            {
                dialogueActionRow.style.flexWrap = Wrap.NoWrap;
                dialogueActionRow.style.width = Length.Percent(100);
                dialogueActionRow.style.maxWidth = Length.Percent(100);
                dialogueActionRow.style.marginTop = 0;
                dialogueActionRow.style.marginBottom = 0;
            }
            if (dialogueContinueButton != null)
            {
                RuntimeUiSkin.ApplyButtonTier(dialogueContinueButton, mobile ? RuntimeUiButtonTier.Compact : RuntimeUiButtonTier.Standard);
                ApplyDialogueActionButton(dialogueContinueButton);
            }
            if (dialogueCloseButton != null)
            {
                RuntimeUiSkin.ApplyButtonTier(dialogueCloseButton, mobile ? RuntimeUiButtonTier.Compact : RuntimeUiButtonTier.Standard);
                ApplyDialogueActionButton(dialogueCloseButton);
            }
            if (dialogueActionRow != null)
                RuntimeUiOverflowGuard.ApplyResponsiveColumns(dialogueActionRow, 2, mobile ? 4 : 6, dialogueContinueButton, dialogueCloseButton);
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
            SetDisplayed(worldGuidanceCard, !(dialogueVisible || skillPreviewActive || evidenceHidesGuidance));
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

        private static void ApplyDialogueActionButton(Button button)
        {
            // LGO Dialogue Action Sizing Contract v1: action columns own width; labels never force overflow.
            button.style.marginRight = 0;
        }

        private static void ApplyDialogueOverlay(RuntimeUiLayoutProfile layout, VisualElement dialoguePanel)
        {
            // LGO Dialogue Viewport Overlay Contract v1: center within safe viewport; equal top/bottom insets prevent bottom pinning.
            RuntimeUiOverflowGuard.ApplyViewportOverlaySurface(
                dialoguePanel,
                RuntimeUiOverlayPlacement.Center,
                RuntimeUiOverlayVerticalPlacement.Stretch,
                layout.DialogueOverlayWidth,
                layout.DialoguePanelMaxHeight,
                layout.DialogueOverlayInsetHorizontal,
                layout.DialogueOverlayInsetVertical);
        }

        private static void SetElementVisibility(VisualElement element, bool visible)
        {
            if (element == null) return;
            element.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
