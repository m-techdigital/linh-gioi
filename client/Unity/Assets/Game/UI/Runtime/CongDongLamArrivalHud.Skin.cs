using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        // Semantic product colors come from design-tokens.json through RuntimeUiTheme.
        // Role-specific alpha belongs here; RGB palette ownership does not.
        private static Color UiGlass => RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.bg, .92f);
        private static Color UiGlassStrong => RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.bg, .96f);
        private static Color UiGlassRaised => RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.surface, .94f);
        private static Color UiGold => RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.gold, .96f);
        private static Color UiGoldBorder => RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.gold, .70f);
        private static Color UiBlue => RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.surfaceRaised, .96f);
        private static Color UiText => RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.text, .96f);
        private static Color UiSubText => RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.muted, .90f);
        private const string LgoInventoryButtonBaseClass = "lgo-inventory-button-base";
        private const string LgoCharacterHubShellClass = "lgo-character-hub-shell";
        private const string LgoCharacterHubTitleClass = "lgo-character-hub-title";
        private const string LgoInventoryPanelShellClass = "lgo-inventory-panel-shell";
        private const string LgoInventoryItemRowClass = "lgo-inventory-item-row";
        private const string LgoInventoryCountBadgeClass = "lgo-inventory-count-badge";
        private const string LgoInventoryBadgeClass = "lgo-inventory-badge";
        private const string LgoInventoryStateBadgeClass = "lgo-inventory-state-badge";
        private const string LgoInventoryStatsCardClass = "lgo-inventory-stats-card";
        private const string LgoInventoryContentFitPanelClass = "lgo-inventory-content-fit-panel";
        private const string LgoInventoryCompactShellClass = "lgo-inventory-compact-shell";
        private const string LgoStatusCardClass = "lgo-status-card";
        private const string LgoDetailCardClass = "lgo-detail-card";
        private const string LgoInventoryMainTabClass = "lgo-inventory-main-tab";
        private const string LgoCharacterHubMainTabsClass = "lgo-character-hub-main-tabs";
        private const string LgoCharacterHubInspectorColumnClass = "lgo-character-hub-inspector-column";
        private const string LgoCharacterHubActionRowClass = "lgo-character-hub-action-row";
        private const string LgoInventoryFilterChipClass = "lgo-inventory-filter-chip";
        private const string LgoInventoryToolbarActionClass = "lgo-inventory-toolbar-action";
        private const string LgoInventoryGridCellClass = "lgo-inventory-grid-cell";
        private const string LgoInventorySearchFieldClass = "lgo-inventory-search-field";
        private const string LgoInventorySearchInputClass = "lgo-inventory-search-input";
        private const string LgoEntryTextFieldClass = "lgo-entry-text-field";
        private const string LgoEntryTextInputClass = "lgo-entry-text-input";
        private const string LgoEntryRememberActionClass = "lgo-entry-remember-action";
        private const string LgoEntryPasswordRevealClass = "lgo-entry-password-reveal";
        private const string LgoModalCloseButtonClass = "lgo-modal-close-button";
        private const string LgoHudCombatActionClass = "lgo-hud-combat-action";
        private const string LgoHudPrimaryCombatActionClass = "lgo-hud-primary-combat-action";
        private const string LgoHudActionIconClass = "lgo-hud-action-icon";
        private const string LgoHudNavigationActionClass = "lgo-hud-navigation-action";
        private const string LgoHudShortcutActionClass = "lgo-hud-shortcut-action";
        private const string LgoHudContextActionClass = "lgo-hud-context-action";
        private const string LgoHudQuestTabClass = "lgo-hud-quest-tab";
        private const string LgoHudInfoPanelClass = "lgo-hud-info-panel";
        private const string LgoHudPlayerCardClass = "lgo-hud-player-card";
        private const string LgoHudPortraitClass = "lgo-hud-portrait";
        private const string LgoHudLocationChipClass = "lgo-hud-location-chip";
        private const string LgoHudMapPanelClass = "lgo-hud-map-panel";
        private const string LgoHudMapRouteClass = "lgo-hud-map-route";
        private const string LgoHudMapNodeClass = "lgo-hud-map-node";
        private const string LgoHudQuestPanelClass = "lgo-hud-quest-panel";
        private const string LgoHudCompositionClass = "lgo-hud-composition";
        private const string LgoGameplayPlayerZoneClass = "lgo-gameplay-player-zone";
        private const string LgoGameplayRightZoneClass = "lgo-gameplay-right-zone";
        private const string LgoGameplayCombatZoneClass = "lgo-gameplay-combat-zone";
        private const string LgoGameplayContextZoneClass = "lgo-gameplay-context-zone";
        private const string LgoGameplayTouchZoneClass = "lgo-gameplay-touch-zone";
        private const string LgoGameplaySecondaryZoneClass = "lgo-gameplay-secondary-zone";
        private const string LgoDialoguePrimaryActionClass = "lgo-dialogue-primary-action";
        private const string LgoDialogueSecondaryActionClass = "lgo-dialogue-secondary-action";
        private const string LgoDialoguePortraitClass = "lgo-dialogue-portrait";
        private const string LgoEntryCtaActionClass = "lgo-entry-cta-action";
        private const string LgoEntryAuthPrimaryClass = "lgo-entry-auth-primary";
        private const string LgoEntryAuthSecondaryClass = "lgo-entry-auth-secondary";
        private const string LgoEntrySecondaryActionClass = "lgo-entry-secondary-action";
        private const string LgoEntryTextLinkClass = "lgo-entry-text-link";
        private const string LgoEntrySideActionClass = "lgo-entry-side-action";
        private const string LgoEntryShellClass = "lgo-entry-shell";
        private const string LgoEntrySloganClass = "lgo-entry-slogan";
        private const string LgoEntryBrandStageClass = "lgo-entry-brand-stage";
        private const string LgoEntrySignatureClass = "lgo-entry-signature";
        private const string LgoEntryNoticeClass = "lgo-entry-notice";
        private const string LgoEntryUtilityRailClass = "lgo-entry-utility-rail";
        private const string LgoEntryStatusRowClass = "lgo-entry-status-row";
        private const string LgoEntryControlCardClass = "lgo-entry-control-card";
        private const string LgoEntryServerCardClass = "lgo-entry-server-card";
        private const string LgoEntryStatusLineClass = "lgo-entry-status-line";
        private const string LgoServerSelectPanelClass = "lgo-server-select-panel";
        private const string LgoServerSelectCardClass = "lgo-server-select-card";
        private const string LgoServerSelectSelectedClass = "lgo-server-select-selected";
        private const string LgoServerSelectTitleRowClass = "lgo-server-select-title-row";
        private const string LgoServerSelectStatusRowClass = "lgo-server-select-status-row";
        private const string LgoServerSelectActionClass = "lgo-server-select-action";
        private const string LgoRegisterPanelClass = "lgo-register-panel";
        private const string LgoRegisterHeaderClass = "lgo-register-header";
        private const string LgoRegisterSubtitleRowClass = "lgo-register-subtitle-row";
        private const string LgoRegisterAgreementClass = "lgo-register-agreement";
        private const string LgoRegisterTermsClass = "lgo-register-terms";
        private const string LgoRegisterPrimaryClass = "lgo-register-primary";
        private const string LgoRegisterBackClass = "lgo-register-back";
        private const string LgoRegisterPasswordRevealClass = "lgo-register-password-reveal";
        private const string LgoAuthFlowPanelClass = "lgo-auth-flow-panel";
        private const string LgoAuthFlowHeaderClass = "lgo-auth-flow-header";
        private const string LgoAuthFlowSubtitleRowClass = "lgo-auth-flow-subtitle-row";
        private const string LgoAuthFlowPrimaryClass = "lgo-auth-flow-primary";
        private const string LgoAuthFlowBackClass = "lgo-auth-flow-back";
        private const string LgoPasswordRecoveryPanelClass = "lgo-password-recovery-panel";
        private const string LgoPasswordRecoveryRuleClass = "lgo-password-recovery-rule";
        private const string LgoPasswordRecoveryVerifyFooterClass = "lgo-password-recovery-verify-footer";
        private const string LgoCharacterSelectOverlayClass = "lgo-character-select-overlay";
        private const string LgoCharacterSelectBrandClass = "lgo-character-select-brand";
        private const string LgoCharacterSelectStageClass = "lgo-character-select-stage";
        private const string LgoCharacterSelectPanelClass = "lgo-character-select-panel";
        private const string LgoCharacterSelectProfileClass = "lgo-character-select-profile";
        private const string LgoCharacterSelectEmptySlotClass = "lgo-character-select-empty-slot";
        private const string LgoCharacterSelectDetailClass = "lgo-character-select-detail";
        private const string LgoCharacterSelectActionsClass = "lgo-character-select-actions";
        private const string LgoCharacterSelectUtilityClass = "lgo-character-select-utility";
        private const string LgoCharacterSelectEnterClass = "lgo-character-select-enter";
        private const string LgoCharacterSelectServerRowClass = "lgo-character-select-server-row";
        private const string LgoMenuActionClass = "lgo-menu-action";
        private const string LgoActionButtonClass = "lgo-action-button";
        private const string LgoActionPrimaryClass = "lgo-action-primary";
        private const string LgoActionStandardClass = "lgo-action-standard";
        private const string LgoInputFieldClass = "lgo-input-field";
        private const string LgoOrnamentRailClass = "lgo-ornament-rail";
        private const string LgoItemIconFrameClass = "lgo-item-icon-frame";
        private const string LgoTitleLabelClass = "lgo-title-label";
        private const string LgoSubtitleLabelClass = "lgo-subtitle-label";
        private const string LgoLayeredFrameClass = "lgo-layered-frame";
        private const string LgoFrameCornerClass = "lgo-frame-corner";
        private const string LgoVitalBarClass = "lgo-vital-bar";
        private const string LgoCharacterHubInteractiveMotionClass = "lgo-character-hub-interactive-motion";
        private const string LgoCharacterHubSelectedClass = "lgo-character-hub-selected";
        private const string LgoCharacterHubSkinRoot = "LGOMaps/CongDongLamMap01AUiSkin/";
        private static Texture2D _characterHubSurface, _characterHubPanelSurface, _characterHubTabIdle,
            _characterHubTabSelected, _characterHubActionBlue, _characterHubActionGold, _characterHubClose,
            _characterHubPotentialTopology;

        private static Texture2D LoadLgoCharacterHubTexture(ref Texture2D cache, string name)
        {
            if (cache == null) cache = Resources.Load<Texture2D>(LgoCharacterHubSkinRoot + name);
            return cache;
        }

        private static void ApplyLgoCharacterHubSurface(VisualElement element, ref Texture2D cache, string name)
        {
            var texture = LoadLgoCharacterHubTexture(ref cache, name);
            if (texture == null) return;
            element.style.backgroundImage = new StyleBackground(texture);
            element.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        }

        private static void RemoveLgoOuterBorder(VisualElement element)
        {
            element.style.borderTopWidth = element.style.borderBottomWidth = 0;
            element.style.borderLeftWidth = element.style.borderRightWidth = 0;
        }

        private static void ApplyLgoCharacterHubPanelSurface(VisualElement element)
        {
            ApplyLgoCharacterHubSurface(element, ref _characterHubPanelSurface, "character-hub-panel-surface");
        }

        private static Texture2D LoadLgoCharacterHubPotentialTopology()
            => LoadLgoCharacterHubTexture(ref _characterHubPotentialTopology, "character-hub-potential-topology");


        private static void ApplyLgoFrameColors(VisualElement element, Color background, Color border)
        {
            element.style.backgroundColor = background;
            element.style.borderTopColor = element.style.borderBottomColor = border;
            element.style.borderLeftColor = element.style.borderRightColor = border;
        }

        private static void ApplyLgoFrame(VisualElement element, Color background, Color border)
        {
            ApplyLgoFrameColors(element, background, border);
            element.style.borderTopWidth = element.style.borderBottomWidth = 1;
            element.style.borderLeftWidth = element.style.borderRightWidth = 1;
        }

        private static void ApplyLgoGlassPanel(VisualElement element, bool raised = false)
        {
            ApplyLgoFrame(element, raised ? UiGlassRaised : UiGlass, UiGoldBorder);
            element.style.borderTopWidth = 2;
            element.style.borderBottomWidth = 2;
            element.style.color = UiText;
        }

        private static bool HasDirectChildNamed(VisualElement element, string name)
        {
            foreach (var child in element.Children())
                if (child.name == name) return true;
            return false;
        }

        private static void AddLgoFrameCorner(VisualElement element, string suffix, bool top, bool right, bool bottom, bool left)
        {
            var name = "LGO Layered Frame Corner " + suffix;
            if (HasDirectChildNamed(element, name)) return;
            var corner = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            corner.AddToClassList(LgoFrameCornerClass);
            corner.style.position = Position.Absolute;
            corner.style.width = 18;
            corner.style.height = 18;
            if (top) corner.style.top = 3;
            if (right) corner.style.right = 3;
            if (bottom) corner.style.bottom = 3;
            if (left) corner.style.left = 3;
            var color = new Color(.96f, .76f, .36f, .86f);
            corner.style.borderTopColor = color;
            corner.style.borderRightColor = color;
            corner.style.borderBottomColor = color;
            corner.style.borderLeftColor = color;
            corner.style.borderTopWidth = top ? 2 : 0;
            corner.style.borderRightWidth = right ? 2 : 0;
            corner.style.borderBottomWidth = bottom ? 2 : 0;
            corner.style.borderLeftWidth = left ? 2 : 0;
            element.Add(corner);
        }

        private static void ApplyLgoCharacterHubFiligreeFrame(VisualElement element)
        {
            RuntimeUiSkin.ApplyOrnamentedShellFrame(element);
        }

        private static void ApplyLgoCharacterHubSectionFrame(VisualElement element)
        {
            element.style.borderTopWidth = element.style.borderBottomWidth = 1;
            element.style.borderLeftWidth = element.style.borderRightWidth = 1;
            element.style.borderTopColor = element.style.borderBottomColor = new Color(.34f, .48f, .58f, .48f);
            element.style.borderLeftColor = element.style.borderRightColor = new Color(.54f, .48f, .32f, .42f);
        }

        private static void ApplyLgoCharacterHubInsetFrame(VisualElement element)
        {
            element.style.borderTopWidth = element.style.borderBottomWidth = 1;
            element.style.borderLeftWidth = element.style.borderRightWidth = 1;
            element.style.borderTopColor = element.style.borderLeftColor = new Color(.94f, .74f, .34f, .86f);
            element.style.borderBottomColor = element.style.borderRightColor = new Color(.40f, .27f, .10f, .92f);
        }

        private static void ApplyLgoLayeredFrame(VisualElement element)
        {
            element.AddToClassList(LgoLayeredFrameClass);
            AddLgoFrameCorner(element, "TL", true, false, false, true);
            AddLgoFrameCorner(element, "TR", true, true, false, false);
            AddLgoFrameCorner(element, "BL", false, false, true, true);
            AddLgoFrameCorner(element, "BR", false, true, true, false);
        }

        private static void SetLgoFrameCornerVisibility(VisualElement element, bool visible)
        {
            foreach (var child in element.Children())
                if (child.ClassListContains(LgoFrameCornerClass))
                    child.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void ApplyLgoModalShell(VisualElement element, float padding = 12)
        {
            element.AddToClassList("lgo-modal");
            ApplyLgoGlassPanel(element);
            ApplyLgoLayeredFrame(element);
            element.style.paddingLeft = element.style.paddingRight = padding;
            element.style.paddingTop = element.style.paddingBottom = padding;
        }

        private static void ApplyLgoCharacterHubShell(VisualElement element)
        {
            element.AddToClassList(LgoCharacterHubShellClass);
            ApplyLgoModalShell(element, 12);
            SetLgoFrameCornerVisibility(element, false);
            ApplyLgoCharacterHubFiligreeFrame(element);
            element.style.backgroundColor = new Color(.004f, .024f, .052f, .98f);
            element.style.borderTopWidth = element.style.borderBottomWidth = 0;
            element.style.borderLeftWidth = element.style.borderRightWidth = 0;
            element.style.borderTopColor = element.style.borderBottomColor = new Color(.96f, .72f, .28f, .94f);
            element.style.borderLeftColor = element.style.borderRightColor = new Color(.70f, .48f, .16f, .92f);
            element.style.paddingTop = 0;
            element.style.paddingBottom = 0;
            ApplyLgoCharacterHubSurface(element, ref _characterHubSurface, "character-hub-surface");
        }

        private static void ApplyLgoCharacterHubTitle(Label label)
        {
            label.AddToClassList(LgoCharacterHubTitleClass);
            label.style.fontSize = 30;
            label.style.color = new Color(.98f, .98f, .94f, 1f);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        private static void ApplyLgoCharacterHubBackdrop(VisualElement backdrop)
        {
            backdrop.style.backgroundColor = new Color(.002f, .018f, .040f, .48f);
        }



        private static void ApplyLgoInputField(VisualElement element)
        {
            element.AddToClassList(LgoInputFieldClass);
            element.AddToClassList("lgo-input");
            ApplyLgoFrameColors(element, new Color(.010f, .035f, .060f, .86f), new Color(.46f, .64f, .74f, .50f));
            element.style.paddingLeft = element.style.paddingRight = 16;
            element.style.paddingTop = element.style.paddingBottom = 2;
            element.style.color = UiSubText;
        }

        private static void ApplyLgoDetailCard(VisualElement element, float horizontalPadding = 0, float verticalPadding = 0)
        {
            element.AddToClassList(LgoDetailCardClass);
            element.style.paddingLeft = element.style.paddingRight = horizontalPadding;
            element.style.paddingTop = element.style.paddingBottom = verticalPadding;
            ApplyLgoFrame(element, new Color(.012f, .040f, .074f, .97f), new Color(.90f, .70f, .36f, .82f));
            ApplyLgoLayeredFrame(element);
            element.style.borderTopWidth = 2;
            element.style.color = UiText;
        }

        private static void ApplyLgoCharacterHubDetailCard(
            VisualElement element, float horizontalPadding = 18, float verticalPadding = 14)
        {
            ApplyLgoDetailCard(element, horizontalPadding, verticalPadding);
            SetLgoFrameCornerVisibility(element, false);
            ApplyLgoCharacterHubSectionFrame(element);
        }

        private static VisualElement LgoDivider(string name)
        {
            var divider = new VisualElement { name = name };
            divider.style.height = 1;
            divider.style.marginTop = 10;
            divider.style.marginBottom = 10;
            divider.style.backgroundColor = new Color(.75f, .60f, .32f, .45f);
            return divider;
        }

        private static void ApplyLgoSoftGlow(VisualElement element, float alpha = .16f)
        {
            ApplyLgoFrame(element, new Color(.10f, .32f, .52f, alpha * .18f), new Color(.95f, .75f, .36f, alpha));
            element.style.opacity = .46f;
        }

        private static void ApplyLgoOrnamentRail(VisualElement element)
        {
            element.AddToClassList(LgoOrnamentRailClass);
            element.AddToClassList("lgo-ornament");
            element.style.backgroundColor = new Color(.95f, .75f, .36f, .72f);
        }

        private static void ApplyLgoItemIcon(VisualElement icon)
        {
            icon.AddToClassList(LgoItemIconFrameClass);
            icon.AddToClassList("lgo-icon-frame");
            icon.style.width = 58;
            icon.style.height = 58;
            icon.style.marginTop = 8;
            icon.style.marginBottom = 4;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            ApplyLgoFrame(icon, new Color(.020f, .070f, .128f, .96f), new Color(.96f, .76f, .36f, .90f));
            icon.style.borderTopWidth = icon.style.borderBottomWidth = 2;
        }

        private static VisualElement EnsureLgoCharacterHubOrnament(VisualElement owner, string roleClass)
        {
            foreach (var child in owner.Children())
                if (child.ClassListContains(roleClass)) return child;
            var frame = new VisualElement { name = owner.name + " Ornament", pickingMode = PickingMode.Ignore };
            frame.AddToClassList("lgo-hub-ornament-frame");
            frame.AddToClassList(roleClass);
            frame.style.position = Position.Absolute;
            frame.style.left = frame.style.right = frame.style.top = frame.style.bottom = 0;
            frame.style.backgroundColor = Color.clear;
            // Reuse the shell's authored vector corner/edge; no duplicated raster
            // borders and no decorative element participates in input or layout.
            RuntimeUiSkin.ApplyOrnamentedShellFrame(frame);
            owner.Insert(0, frame); // Labels and item artwork paint above decoration.
            return frame;
        }

        private static void ApplyLgoCharacterHubInspectorFrame(VisualElement panel)
        {
            RemoveLgoOuterBorder(panel);
            EnsureLgoCharacterHubOrnament(panel, "lgo-hub-inspector-frame");
        }

        private static void ApplyLgoCharacterHubIconSelection(VisualElement icon, bool selected)
        {
            var skill = icon.ClassListContains("lgo-skill-node");
            var frame = EnsureLgoCharacterHubOrnament(icon, "lgo-hub-selection-frame");
            frame.style.opacity = selected ? 1f : skill ? 0f : .38f;
            icon.style.backgroundColor = skill ? Color.clear
                : selected ? new Color(.10f, .075f, .025f, .48f) : new Color(.010f, .040f, .070f, .94f);
            RemoveLgoOuterBorder(icon);
            icon.style.overflow = Overflow.Visible;
            if (icon is Button button) ApplyLgoCharacterHubInteractiveMotion(button);
        }

        private static void ApplyLgoCharacterHubHeroIconFrame(VisualElement icon, bool visible = true)
        {
            icon.style.backgroundColor = visible ? new Color(.012f, .045f, .082f, .98f) : Color.clear;
            RemoveLgoOuterBorder(icon);
            EnsureLgoCharacterHubOrnament(icon, "lgo-hub-hero-frame").style.display
                = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void ApplyLgoCharacterHubInteractiveMotion(Button button)
        {
            if (button == null || button.ClassListContains(LgoCharacterHubInteractiveMotionClass)) return;
            button.AddToClassList(LgoCharacterHubInteractiveMotionClass);
            button.RegisterCallback<PointerEnterEvent>(_ =>
            {
                if (!button.enabledSelf) return;
                button.experimental.animation.Start(
                    new StyleValues { opacity = .84f },
                    new StyleValues { opacity = 1f },
                    105);
            });
            button.RegisterCallback<PointerDownEvent>(_ =>
            {
                if (!button.enabledSelf) return;
                button.experimental.animation.Start(
                    new StyleValues { opacity = 1f },
                    new StyleValues { opacity = .68f },
                    65);
            });
            button.RegisterCallback<PointerUpEvent>(_ =>
            {
                if (!button.enabledSelf) return;
                button.experimental.animation.Start(
                    new StyleValues { opacity = .68f },
                    new StyleValues { opacity = 1f },
                    115);
            });
        }

        private static void ApplyLgoCharacterHubSelectionState(VisualElement element, bool selected)
        {
            element.EnableInClassList(LgoCharacterHubSelectedClass, selected);
            if (element.ClassListContains("lgo-skill-node") || element.ClassListContains(LgoItemIconFrameClass)
                || element.ClassListContains("lgo-inventory-bag-grid-cell"))
            {
                ApplyLgoCharacterHubIconSelection(element, selected);
                return;
            }
            element.style.backgroundColor = selected
                ? new Color(.025f, .21f, .39f, .98f)
                : new Color(.012f, .050f, .088f, .94f);
            element.style.borderTopWidth = element.style.borderBottomWidth = selected ? 2 : 1;
            element.style.borderLeftWidth = element.style.borderRightWidth = selected ? 2 : 1;
            element.style.borderTopColor = element.style.borderLeftColor = selected
                ? new Color(.22f, .84f, 1f, 1f)
                : new Color(.30f, .49f, .64f, .66f);
            element.style.borderBottomColor = element.style.borderRightColor = selected
                ? new Color(1f, .76f, .26f, 1f)
                : new Color(.18f, .30f, .40f, .72f);
            if (element is Button button) ApplyLgoCharacterHubInteractiveMotion(button);
        }

        private static void ApplyLgoVitalBar(UnityEngine.UIElements.ProgressBar bar, Color fillColor)
        {
            bar.AddToClassList(LgoVitalBarClass);
            bar.style.height = 18;
            bar.style.marginTop = 3;
            bar.style.fontSize = 12;
            bar.style.color = new Color(.98f, .96f, .88f, .98f);
            var fill = bar.Q(className: "unity-progress-bar__progress");
            if (fill != null) fill.style.backgroundColor = fillColor;
            var background = bar.Q(className: "unity-progress-bar__background");
            if (background != null)
            {
                background.style.backgroundColor = new Color(.018f, .040f, .060f, .96f);
                background.style.borderTopWidth = background.style.borderBottomWidth = 1;
                background.style.borderLeftWidth = background.style.borderRightWidth = 1;
                background.style.borderTopColor = background.style.borderBottomColor = new Color(.54f, .66f, .70f, .58f);
                background.style.borderLeftColor = background.style.borderRightColor = new Color(.54f, .66f, .70f, .58f);
            }
        }

        private static Label LgoLabel(string text, int size, Color color, bool bold = false)
        {
            var label = new Label(text);
            label.style.fontSize = size;
            label.style.color = color;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
            return label;
        }

        private static Label LgoTitleLabel(string text, int size = 20, TextAnchor align = TextAnchor.MiddleLeft)
        {
            var label = LgoLabel(text, size, UiGold, true);
            label.AddToClassList(LgoTitleLabelClass);
            label.style.unityTextAlign = align;
            return label;
        }

        private static Label LgoSubtitleLabel(string text, int size = 13, TextAnchor align = TextAnchor.MiddleLeft)
        {
            var label = LgoLabel(text, size, UiSubText);
            label.AddToClassList(LgoSubtitleLabelClass);
            label.style.unityTextAlign = align;
            return label;
        }

        private static void ApplyLgoButton(Button button, bool primary = false)
        {
            button.AddToClassList(LgoActionButtonClass);
            button.AddToClassList("lgo-action");
            button.EnableInClassList(LgoActionPrimaryClass, primary);
            button.EnableInClassList(LgoActionStandardClass, !primary);
            button.style.minHeight = primary ? 46 : 38;
            button.style.minWidth = 0;
            button.style.paddingLeft = button.style.paddingRight = primary ? 20 : 14;
            button.style.paddingTop = button.style.paddingBottom = primary ? 3 : 2;
            button.style.fontSize = primary ? 18 : 14;
            button.style.unityFontStyleAndWeight = primary ? FontStyle.Bold : FontStyle.Normal;
            button.style.color = primary ? new Color(.10f, .07f, .03f, 1f) : UiText;
            ApplyLgoFrameColors(button, primary ? UiGold : new Color(.038f, .118f, .172f, .98f), primary ? new Color(.98f, .86f, .48f, .94f) : new Color(.56f, .68f, .70f, .58f));
        }

    }
}
