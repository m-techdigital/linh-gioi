using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private static readonly Color UiGlass = new Color(.010f, .034f, .064f, .92f);
        private static readonly Color UiGlassStrong = new Color(.016f, .052f, .092f, .96f);
        private static readonly Color UiGlassRaised = new Color(.022f, .072f, .118f, .94f);
        private static readonly Color UiGold = new Color(.95f, .75f, .36f, .96f);
        private static readonly Color UiGoldBorder = new Color(.78f, .62f, .32f, .70f);
        private static readonly Color UiBlue = new Color(.10f, .35f, .58f, .96f);
        private static readonly Color UiText = new Color(.96f, .91f, .76f, .96f);
        private static readonly Color UiSubText = new Color(.73f, .85f, .88f, .90f);
        private const string LgoInventoryButtonBaseClass = "lgo-inventory-button-base";
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
        private const string LgoInventoryFilterChipClass = "lgo-inventory-filter-chip";
        private const string LgoInventoryToolbarActionClass = "lgo-inventory-toolbar-action";
        private const string LgoInventoryGridCellClass = "lgo-inventory-grid-cell";
        private const string LgoInventorySearchFieldClass = "lgo-inventory-search-field";
        private const string LgoInventorySearchInputClass = "lgo-inventory-search-input";
        private const string LgoEntryTextFieldClass = "lgo-entry-text-field";
        private const string LgoEntryTextInputClass = "lgo-entry-text-input";
        private const string LgoEntryRememberActionClass = "lgo-entry-remember-action";
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
        private const string LgoDialoguePrimaryActionClass = "lgo-dialogue-primary-action";
        private const string LgoDialogueSecondaryActionClass = "lgo-dialogue-secondary-action";
        private const string LgoDialoguePortraitClass = "lgo-dialogue-portrait";
        private const string LgoEntryCtaActionClass = "lgo-entry-cta-action";
        private const string LgoEntryAuthPrimaryClass = "lgo-entry-auth-primary";
        private const string LgoEntryAuthSecondaryClass = "lgo-entry-auth-secondary";
        private const string LgoEntrySecondaryActionClass = "lgo-entry-secondary-action";
        private const string LgoEntrySideActionClass = "lgo-entry-side-action";
        private const string LgoEntryShellClass = "lgo-entry-shell";
        private const string LgoEntryControlCardClass = "lgo-entry-control-card";
        private const string LgoCharacterSelectCardClass = "lgo-character-select-card";
        private const string LgoCharacterSelectPrimaryActionClass = "lgo-character-select-primary-action";
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

        private static void ApplyLgoFrame(VisualElement element, Color background, Color border)
        {
            element.style.backgroundColor = background;
            element.style.borderTopWidth = element.style.borderBottomWidth = 1;
            element.style.borderLeftWidth = element.style.borderRightWidth = 1;
            element.style.borderTopColor = element.style.borderBottomColor = border;
            element.style.borderLeftColor = element.style.borderRightColor = border;
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

        private static void ApplyLgoLayeredFrame(VisualElement element)
        {
            element.AddToClassList(LgoLayeredFrameClass);
            AddLgoFrameCorner(element, "TL", true, false, false, true);
            AddLgoFrameCorner(element, "TR", true, true, false, false);
            AddLgoFrameCorner(element, "BL", false, false, true, true);
            AddLgoFrameCorner(element, "BR", false, true, true, false);
        }

        private static void ApplyLgoModalShell(VisualElement element, float padding = 12)
        {
            ApplyLgoGlassPanel(element);
            ApplyLgoLayeredFrame(element);
            element.style.flexDirection = FlexDirection.Column;
            element.style.paddingLeft = element.style.paddingRight = padding;
            element.style.paddingTop = element.style.paddingBottom = padding;
        }



        private static void ApplyLgoInputField(VisualElement element)
        {
            element.AddToClassList(LgoInputFieldClass);
            ApplyLgoFrame(element, new Color(.010f, .035f, .060f, .86f), new Color(.46f, .64f, .74f, .50f));
            element.style.paddingLeft = element.style.paddingRight = 16;
            element.style.paddingTop = element.style.paddingBottom = 2;
            element.style.borderTopWidth = element.style.borderBottomWidth = 2;
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
            element.style.height = 2;
            element.style.backgroundColor = new Color(.95f, .75f, .36f, .72f);
        }

        private static void ApplyLgoItemIcon(VisualElement icon)
        {
            icon.AddToClassList(LgoItemIconFrameClass);
            icon.style.width = 58;
            icon.style.height = 58;
            icon.style.marginTop = 8;
            icon.style.marginBottom = 4;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            ApplyLgoFrame(icon, new Color(.020f, .070f, .128f, .96f), new Color(.96f, .76f, .36f, .90f));
            icon.style.borderTopWidth = icon.style.borderBottomWidth = 2;
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
            button.EnableInClassList(LgoActionPrimaryClass, primary);
            button.EnableInClassList(LgoActionStandardClass, !primary);
            button.style.minHeight = primary ? 46 : 38;
            button.style.minWidth = 0;
            button.style.paddingLeft = button.style.paddingRight = primary ? 20 : 14;
            button.style.paddingTop = button.style.paddingBottom = primary ? 3 : 2;
            button.style.fontSize = primary ? 18 : 14;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.unityFontStyleAndWeight = primary ? FontStyle.Bold : FontStyle.Normal;
            button.style.color = primary ? new Color(.10f, .07f, .03f, 1f) : UiText;
            ApplyLgoFrame(button, primary ? UiGold : new Color(.038f, .118f, .172f, .98f), primary ? new Color(.98f, .86f, .48f, .94f) : new Color(.56f, .68f, .70f, .58f));
            if (primary)
            {
                button.style.borderTopWidth = 2;
                button.style.borderBottomWidth = 2;
                button.style.borderLeftWidth = 2;
                button.style.borderRightWidth = 2;
            }
        }

        private static void ApplyLgoInventoryPanelShell(VisualElement panel)
        {
            panel.AddToClassList(LgoInventoryPanelShellClass);
            ApplyLgoGlassPanel(panel, true);
            panel.style.paddingLeft = panel.style.paddingRight = 12;
            panel.style.paddingTop = panel.style.paddingBottom = 10;
            panel.style.minWidth = 0;
        }

        private static void ApplyLgoInventoryButtonBase(Button button, bool touch)
        {
            button.AddToClassList(LgoInventoryButtonBaseClass);
            ApplyLgoButton(button);
            button.style.minHeight = touch ? 44 : 38;
            button.style.minWidth = 0;
            button.style.flexGrow = 1;
            button.style.flexBasis = 0;
            button.style.marginRight = 4;
            button.style.marginLeft = 0;
            button.style.fontSize = 15;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyLgoInventoryItemRow(Button row, bool touch)
        {
            row.AddToClassList(LgoInventoryItemRowClass);
            row.text = string.Empty;
            row.style.flexGrow = 0;
            row.style.flexBasis = StyleKeyword.Auto;
            row.style.minHeight = touch ? 58 : 52;
            row.style.marginBottom = 8;
            row.style.paddingLeft = row.style.paddingRight = 12;
            row.style.paddingTop = row.style.paddingBottom = 7;
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.justifyContent = Justify.FlexStart;
            row.style.unityTextAlign = TextAnchor.MiddleLeft;
        }

        private static void ApplyLgoInventoryCountBadge(Label badge)
        {
            badge.AddToClassList(LgoInventoryCountBadgeClass);
            badge.style.flexGrow = 0;
            badge.style.marginLeft = 10;
            badge.style.paddingLeft = badge.style.paddingRight = 8;
            badge.style.paddingTop = badge.style.paddingBottom = 3;
            badge.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoFrame(badge, new Color(.020f, .060f, .088f, .90f), new Color(.92f, .72f, .36f, .55f));
        }

        private static void ApplyLgoInventoryBadge(Label badge)
        {
            badge.AddToClassList(LgoInventoryBadgeClass);
            badge.style.paddingLeft = badge.style.paddingRight = 10;
            badge.style.paddingTop = badge.style.paddingBottom = 4;
            badge.style.marginRight = 6;
            badge.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoFrame(badge, new Color(.018f, .060f, .096f, .88f), new Color(.72f, .62f, .38f, .62f));
        }

        private static void ApplyLgoInventoryStateBadge(Label badge)
        {
            badge.AddToClassList(LgoInventoryStateBadgeClass);
            badge.style.marginTop = 8;
            badge.style.alignSelf = Align.FlexStart;
            badge.style.paddingLeft = badge.style.paddingRight = 10;
            badge.style.paddingTop = badge.style.paddingBottom = 5;
            badge.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoFrame(badge, UiGold, new Color(.98f, .86f, .48f, .92f));
        }

        private static void ApplyLgoInventoryStatsCard(VisualElement card)
        {
            card.AddToClassList(LgoInventoryStatsCardClass);
            card.style.flexDirection = FlexDirection.Column;
            card.style.marginTop = 8;
            card.style.paddingLeft = card.style.paddingRight = 10;
            card.style.paddingTop = card.style.paddingBottom = 7;
            ApplyLgoFrame(card, new Color(.018f, .060f, .096f, .92f), new Color(.72f, .62f, .38f, .58f));
        }

        private static void ApplyLgoInventoryContentFitPanel(VisualElement panel)
        {
            panel.AddToClassList(LgoInventoryContentFitPanelClass);
            panel.style.alignSelf = Align.Stretch;
            panel.style.flexGrow = 1;
            panel.style.flexShrink = 0;
        }

        private static void ApplyLgoInventoryCompactShell(VisualElement shell, bool compact)
        {
            shell.EnableInClassList(LgoInventoryCompactShellClass, compact);
        }

        private static void ApplyLgoStatusCard(VisualElement card, float horizontalPadding = 10, float verticalPadding = 8)
        {
            card.AddToClassList(LgoStatusCardClass);
            card.style.paddingLeft = card.style.paddingRight = horizontalPadding;
            card.style.paddingTop = card.style.paddingBottom = verticalPadding;
            ApplyLgoFrame(card, new Color(.018f, .055f, .090f, .86f), new Color(.56f, .68f, .72f, .52f));
            ApplyLgoLayeredFrame(card);
        }

        private static void ApplyLgoSelectedTab(Button button, bool selected)
        {
            button.style.backgroundColor = selected ? UiBlue : new Color(.038f, .118f, .172f, .98f);
            button.style.color = selected ? new Color(.98f, .95f, .78f, .98f) : UiText;
            button.style.borderBottomWidth = selected ? 2 : 1;
            button.style.borderBottomColor = selected ? UiGold : new Color(.56f, .68f, .70f, .58f);
        }

        private static void ApplyLgoInventoryMainTab(Button button, bool touch)
        {
            button.AddToClassList(LgoInventoryMainTabClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 1;
            button.style.flexBasis = 0;
            button.style.minWidth = 0;
            button.style.minHeight = 50;
            button.style.fontSize = 16;
            button.style.marginRight = 8;
        }

        private static void ApplyLgoInventoryFilterChip(Button button, bool touch)
        {
            button.AddToClassList(LgoInventoryFilterChipClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.flexBasis = 100;
            button.style.minHeight = touch ? 36 : 28;
            button.style.marginRight = 6;
            button.style.marginBottom = 4;
            button.style.fontSize = 12;
        }

        private static void ApplyLgoInventoryToolbarAction(Button button, bool touch, bool disabled = true)
        {
            button.AddToClassList(LgoInventoryToolbarActionClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.flexBasis = 112;
            button.style.minHeight = touch ? 38 : 30;
            button.style.marginRight = 6;
            button.style.fontSize = 13;
            if (!disabled) return;
            button.SetEnabled(false);
            button.style.opacity = .58f;
            button.style.color = new Color(.70f, .78f, .78f, .82f);
        }

        private static void ApplyLgoModalCloseButton(Button button, bool touch)
        {
            button.AddToClassList(LgoModalCloseButtonClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.flexBasis = touch ? 46 : 42;
            button.style.minHeight = touch ? 42 : 38;
            button.style.fontSize = 22;
            button.style.marginRight = 0;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        private static void ApplyLgoInventoryGridCell(VisualElement cell)
        {
            cell.AddToClassList(LgoInventoryGridCellClass);
            cell.style.flexGrow = 0;
            cell.style.flexBasis = new Length(InventoryGridCellBasisPercent, LengthUnit.Percent);
            cell.style.height = 116;
            cell.style.marginRight = 6;
            cell.style.marginBottom = 7;
            cell.style.alignItems = Align.Center;
            cell.style.justifyContent = Justify.Center;
        }

        private static void ApplyLgoInventoryBagGridCell(VisualElement cell)
        {
            ApplyLgoInventoryGridCell(cell);
            cell.AddToClassList("lgo-inventory-bag-grid-cell");
            cell.style.flexBasis = new Length(18.2f, LengthUnit.Percent);
            cell.style.height = 92;
            cell.style.minHeight = 92;
            cell.style.maxHeight = 92;
        }

        private static void ApplyLgoInventoryCategoryItem(Button button, bool touch)
        {
            ApplyLgoInventoryFilterChip(button, touch);
            button.text = string.Empty;
            button.style.flexDirection = FlexDirection.Column;
            button.style.alignItems = Align.Center;
            button.style.justifyContent = Justify.Center;
            button.style.flexBasis = StyleKeyword.Auto;
            button.style.minWidth = 0;
            button.style.minHeight = 74;
            button.style.marginRight = 0;
            button.style.marginBottom = 6;
        }

        private static void ApplyLgoInventoryCategoryIcon(VisualElement icon)
        {
            icon.style.width = 46;
            icon.style.height = 46;
            icon.style.flexGrow = 0;
            icon.style.flexShrink = 0;
            icon.style.marginBottom = 2;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        private static void ApplyLgoSkillCategoryCard(Button button, bool touch)
        {
            ApplyLgoInventoryFilterChip(button, touch);
            button.AddToClassList("lgo-skill-category-card");
            button.text = string.Empty;
            button.style.flexDirection = FlexDirection.Column;
            button.style.alignItems = Align.Center;
            button.style.justifyContent = Justify.Center;
            button.style.flexBasis = StyleKeyword.Auto;
            button.style.height = 128;
            button.style.minHeight = 128;
            button.style.marginRight = 0;
            button.style.marginBottom = 8;
        }

        private static void ApplyLgoSkillIcon(VisualElement icon, float size)
        {
            icon.style.width = size;
            icon.style.height = size;
            icon.style.flexGrow = 0;
            icon.style.flexShrink = 0;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        private static void ApplyLgoSkillNode(Button node)
        {
            node.AddToClassList("lgo-skill-node");
            node.text = string.Empty;
            node.style.width = 104;
            node.style.minWidth = 104;
            node.style.maxWidth = 104;
            node.style.flexBasis = 104;
            node.style.flexGrow = 0;
            node.style.flexShrink = 0;
            node.style.height = 104;
            node.style.minHeight = 104;
            node.style.maxHeight = 104;
            node.style.paddingLeft = node.style.paddingRight = 4;
            node.style.paddingTop = node.style.paddingBottom = 4;
            node.style.flexDirection = FlexDirection.Column;
            node.style.alignItems = Align.Center;
            node.style.justifyContent = Justify.Center;
            node.style.borderTopLeftRadius = node.style.borderTopRightRadius = 52;
            node.style.borderBottomLeftRadius = node.style.borderBottomRightRadius = 52;
        }

        private static void ApplyLgoInventorySearchField(TextField field, bool touch)
        {
            field.AddToClassList(LgoInventorySearchFieldClass);
            ApplyLgoInputField(field);
            field.style.flexGrow = 1;
            field.style.flexShrink = 1;
            field.style.minWidth = touch ? 140 : 170;
            field.style.maxWidth = touch ? 210 : 250;
            field.style.height = touch ? 38 : 32;
            field.style.marginLeft = 8;
            field.style.marginRight = 8;
            field.style.paddingLeft = field.style.paddingRight = 10;
            field.style.fontSize = 13;
            ApplyLgoInventorySearchInnerField(field);
            field.RegisterCallback<AttachToPanelEvent>(_ => ApplyLgoInventorySearchInnerField(field));
        }

        private static void ApplyLgoInventorySearchInnerField(TextField field)
        {
            var input = ApplyLgoTextFieldInnerFrame(field, LgoInventorySearchInputClass);
            if (input == null) return;
            input.style.paddingLeft = input.style.paddingRight = 8;
            input.style.paddingTop = input.style.paddingBottom = 0;
            input.style.fontSize = 13;
            input.style.unityTextAlign = TextAnchor.MiddleLeft;
            input.style.height = Length.Percent(100);
        }

        private static VisualElement ApplyLgoTextFieldInnerFrame(TextField field, string semanticClass)
        {
            RuntimeUiSkin.ApplyLobbyInputInnerFrame(field);
            var input = field.Q(className: "unity-base-text-field__input")
                ?? field.Q(className: "unity-text-field__input")
                ?? field.Q("unity-text-input");
            if (input != null) input.AddToClassList(semanticClass);
            return input;
        }

        private static void ApplyLgoEntryTextField(TextField field)
        {
            field.AddToClassList(LgoEntryTextFieldClass);
            ApplyLgoInputField(field);
            field.style.height = 44;
            field.style.marginBottom = 10;
            field.style.paddingLeft = 16;
            field.style.paddingRight = 16;
            field.style.fontSize = 15;
            var input = ApplyLgoTextFieldInnerFrame(field, LgoEntryTextInputClass);
            if (input != null) input.style.fontSize = 15;
            field.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                var attachedInput = ApplyLgoTextFieldInnerFrame(field, LgoEntryTextInputClass);
                if (attachedInput != null) attachedInput.style.fontSize = 15;
            });
        }

        private static void ApplyLgoEntryRememberAction(Button button)
        {
            button.AddToClassList(LgoEntryRememberActionClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 1;
            button.style.flexBasis = 0;
            button.style.minHeight = 28;
            button.style.paddingLeft = button.style.paddingRight = 0;
            button.style.paddingTop = button.style.paddingBottom = 0;
            button.style.marginRight = 8;
            button.style.backgroundColor = Color.clear;
            button.style.borderTopWidth = button.style.borderRightWidth = 0;
            button.style.borderBottomWidth = button.style.borderLeftWidth = 0;
            button.style.justifyContent = Justify.FlexStart;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
        }

        private static void ApplyLgoEntryCtaAction(Button button, bool primary)
        {
            button.AddToClassList(LgoEntryCtaActionClass);
            ApplyLgoButton(button, primary);
            button.style.minWidth = primary ? 320 : 156;
            button.style.minHeight = primary ? 50 : 38;
            button.style.fontSize = primary ? 21 : 14;
            if (!primary) return;
            button.style.borderTopWidth = button.style.borderBottomWidth = 3;
            button.style.borderLeftWidth = button.style.borderRightWidth = 3;
            button.style.borderTopColor = button.style.borderBottomColor = new Color(1f, .86f, .50f, .98f);
            button.style.borderLeftColor = button.style.borderRightColor = new Color(.66f, .42f, .12f, .98f);
        }

        private static void ApplyLgoEntryAuthAction(Button button, bool primary)
        {
            ApplyLgoEntryCtaAction(button, false);
            button.EnableInClassList(LgoEntryAuthPrimaryClass, primary);
            button.EnableInClassList(LgoEntryAuthSecondaryClass, !primary);
            button.style.minHeight = 40;
            button.style.fontSize = 15;
            button.style.borderTopWidth = button.style.borderBottomWidth = 2;
            button.style.borderLeftWidth = button.style.borderRightWidth = 2;
            if (primary)
            {
                button.style.backgroundColor = new Color(.025f, .28f, .68f, .98f);
                button.style.borderTopColor = button.style.borderBottomColor = new Color(.28f, .78f, 1f, .98f);
                button.style.borderLeftColor = button.style.borderRightColor = new Color(.12f, .52f, .92f, .98f);
                button.style.color = new Color(.98f, .98f, .90f, 1f);
            }
            else
            {
                button.style.backgroundColor = new Color(.018f, .065f, .12f, .96f);
                button.style.borderTopColor = button.style.borderBottomColor = new Color(.95f, .75f, .36f, .92f);
                button.style.borderLeftColor = button.style.borderRightColor = new Color(.68f, .46f, .18f, .92f);
                button.style.color = new Color(.96f, .89f, .70f, .98f);
            }
        }

        private static void ApplyLgoEntryShell(VisualElement panel)
        {
            panel.AddToClassList(LgoEntryShellClass);
            panel.style.width = Length.Percent(36);
            panel.style.minWidth = 520;
            panel.style.maxWidth = 590;
            panel.style.paddingLeft = 16;
            panel.style.paddingRight = 16;
            panel.style.paddingTop = 14;
            panel.style.paddingBottom = 14;
            ApplyLgoModalShell(panel, 16);
            panel.style.backgroundColor = new Color(.010f, .030f, .058f, .08f);
            panel.style.borderTopWidth = panel.style.borderBottomWidth = 0;
            panel.style.borderLeftWidth = panel.style.borderRightWidth = 0;
        }

        private static void ApplyLgoEntryControlCard(VisualElement card)
        {
            card.AddToClassList(LgoEntryControlCardClass);
            ApplyLgoDetailCard(card, 18, 16);
            card.style.backgroundColor = new Color(.010f, .035f, .064f, .93f);
            card.style.borderTopWidth = card.style.borderBottomWidth = 2;
            card.style.borderLeftWidth = card.style.borderRightWidth = 2;
            card.style.marginTop = 2;
            card.style.marginBottom = 14;
        }

        private static VisualElement CreateLgoEntryIcon(string name, Sprite sprite, float size)
        {
            var icon = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            icon.style.width = icon.style.height = size;
            icon.style.flexShrink = 0;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            return icon;
        }

        private static void AttachLgoEntryFieldIcon(VisualElement field, Sprite sprite)
        {
            field.style.position = Position.Relative;
            field.style.paddingLeft = 50;
            var icon = CreateLgoEntryIcon(field.name + " Icon", sprite, 24);
            icon.style.position = Position.Absolute;
            icon.style.left = 14;
            icon.style.top = 9;
            field.Add(icon);
        }

        private static void AttachLgoEntrySideActionIcon(Button button, Sprite sprite)
        {
            var icon = CreateLgoEntryIcon(button.name + " Icon", sprite, 30);
            icon.style.position = Position.Absolute;
            icon.style.left = 19;
            icon.style.top = 5;
            button.Add(icon);
        }

        private static void ApplyLgoEntryBrandCrest(VisualElement crest, Sprite sprite)
        {
            crest.style.width = crest.style.height = 48;
            crest.style.alignSelf = Align.Center;
            crest.style.marginBottom = 2;
            crest.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            crest.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
        }

        private static void ApplyLgoEntrySecondaryAction(Button button, float minWidth = 0f, float marginRight = 0f)
        {
            button.AddToClassList(LgoEntrySecondaryActionClass);
            ApplyLgoDisabledAction(button);
            button.style.flexGrow = 0;
            button.style.minHeight = 34;
            button.style.fontSize = 13;
            if (minWidth > 0f) button.style.minWidth = minWidth;
            if (marginRight > 0f) button.style.marginRight = marginRight;
        }

        private static void ApplyLgoEntrySideAction(Button button)
        {
            button.AddToClassList(LgoEntrySideActionClass);
            ApplyLgoButton(button);
            button.style.position = Position.Relative;
            button.style.width = 68;
            button.style.height = 68;
            button.style.marginLeft = 12;
            button.style.marginBottom = 10;
            button.style.paddingLeft = button.style.paddingRight = 0;
            button.style.paddingTop = 38;
            button.style.paddingBottom = 5;
            button.style.fontSize = 11;
            button.style.opacity = .90f;
            button.style.whiteSpace = WhiteSpace.NoWrap;
            button.style.unityTextAlign = TextAnchor.LowerCenter;
            button.style.backgroundColor = new Color(.008f, .030f, .054f, .72f);
            button.style.borderTopLeftRadius = button.style.borderTopRightRadius = 34;
            button.style.borderBottomLeftRadius = button.style.borderBottomRightRadius = 34;
            button.style.borderTopWidth = button.style.borderBottomWidth = 2;
            button.style.borderLeftWidth = button.style.borderRightWidth = 2;
            button.style.borderTopColor = button.style.borderBottomColor = new Color(.82f, .67f, .36f, .80f);
            button.style.borderLeftColor = button.style.borderRightColor = new Color(.82f, .67f, .36f, .80f);
        }

        private static void ApplyLgoCharacterSelectCard(Button card)
        {
            card.AddToClassList(LgoCharacterSelectCardClass);
            card.style.flexGrow = 0;
            card.style.flexBasis = new Length(30.5f, LengthUnit.Percent);
            card.style.height = 116;
            card.style.marginRight = 8;
            card.style.marginBottom = 8;
            card.style.fontSize = 19;
            card.style.whiteSpace = WhiteSpace.Normal;
            card.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoButton(card);
        }

        private static void ApplyLgoCharacterSelectPrimaryAction(Button button)
        {
            button.AddToClassList(LgoCharacterSelectPrimaryActionClass);
            button.style.minWidth = 220;
            ApplyLgoButton(button, true);
        }

        private static void ApplyLgoMenuAction(Button button, bool primary = false)
        {
            button.AddToClassList(LgoMenuActionClass);
            ApplyLgoButton(button, primary);
            button.style.flexGrow = 0;
            button.style.flexShrink = 0;
            button.style.flexBasis = new Length(47f, LengthUnit.Percent);
            button.style.minWidth = 142;
            button.style.minHeight = 46;
            button.style.marginRight = 6;
            button.style.marginBottom = 6;
            button.style.fontSize = 13;
            button.style.whiteSpace = WhiteSpace.NoWrap;
        }

        private static void ApplyLgoDialoguePrimaryAction(Button button)
        {
            button.AddToClassList(LgoDialoguePrimaryActionClass);
            ApplyLgoButton(button, true);
            button.style.marginRight = 10;
            button.style.flexGrow = 1;
        }

        private static void ApplyLgoDialogueSecondaryAction(Button button)
        {
            button.AddToClassList(LgoDialogueSecondaryActionClass);
            ApplyLgoButton(button);
            button.style.minHeight = 44;
            button.style.marginRight = 10;
        }

        private static void ApplyLgoDialoguePortrait(VisualElement portrait)
        {
            portrait.AddToClassList(LgoDialoguePortraitClass);
            portrait.style.width = 112;
            portrait.style.height = 144;
            portrait.style.flexShrink = 0;
            portrait.style.marginRight = 12;
            portrait.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            ApplyLgoFrame(portrait, new Color(.018f, .060f, .096f, .72f), new Color(.90f, .70f, .36f, .72f));
        }

        private static void ApplyLgoHudInfoPanel(VisualElement element)
        {
            element.AddToClassList(LgoHudInfoPanelClass);
            element.style.backgroundColor = new Color(.012f, .042f, .074f, .86f);
            element.style.color = UiText;
            element.style.paddingLeft = element.style.paddingRight = 10;
            element.style.paddingTop = element.style.paddingBottom = 7;
            element.style.fontSize = 16;
            element.style.borderBottomWidth = 1;
            element.style.borderBottomColor = new Color(.72f, .60f, .34f, .42f);
        }

        private static void ApplyLgoHudLocationChip(VisualElement element)
        {
            element.AddToClassList(LgoHudLocationChipClass);
            ApplyLgoHudInfoPanel(element);
            element.style.fontSize = 14;
            element.style.unityFontStyleAndWeight = FontStyle.Bold;
            element.style.paddingTop = element.style.paddingBottom = 5;
        }

        private static void ApplyLgoHudComposition(VisualElement element)
        {
            element.AddToClassList(LgoHudCompositionClass);
            element.style.flexDirection = FlexDirection.Column;
            element.style.alignItems = Align.Stretch;
        }

        private static void ApplyLgoHudPlayerCard(VisualElement element)
        {
            element.AddToClassList(LgoHudPlayerCardClass);
            ApplyLgoHudInfoPanel(element);
            element.style.flexDirection = FlexDirection.Row;
            element.style.alignItems = Align.Center;
            element.style.height = 88;
            element.style.borderLeftWidth = 2;
            element.style.borderLeftColor = UiGoldBorder;
        }

        private static void ApplyLgoHudPortrait(VisualElement element)
        {
            element.AddToClassList(LgoHudPortraitClass);
            element.style.width = 58;
            element.style.height = 68;
            element.style.flexShrink = 0;
            element.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            ApplyLgoFrame(element, new Color(.018f, .060f, .096f, .92f), new Color(.90f, .70f, .36f, .72f));
        }

        private static void ApplyLgoHudMapPanel(VisualElement element)
        {
            element.AddToClassList(LgoHudMapPanelClass);
            ApplyLgoHudInfoPanel(element);
            element.style.backgroundColor = UiGlassStrong;
            element.style.borderTopWidth = 2;
            element.style.borderTopColor = UiGoldBorder;
        }

        private static void ApplyLgoHudMapRouteLine(VisualElement element)
        {
            element.AddToClassList(LgoHudMapRouteClass);
            element.style.position = Position.Absolute;
            element.style.left = 18;
            element.style.right = 18;
            element.style.top = 12;
            element.style.height = 2;
            element.style.backgroundColor = new Color(.22f, .58f, .78f, .62f);
        }

        private static void ApplyLgoHudMapRouteNode(VisualElement element)
        {
            element.AddToClassList(LgoHudMapNodeClass);
            element.style.width = 46;
            element.style.alignItems = Align.Center;
            element.style.flexShrink = 0;
        }

        private static void ApplyLgoHudMapRouteDot(VisualElement element)
        {
            element.style.width = element.style.height = 10;
            element.style.borderTopLeftRadius = element.style.borderTopRightRadius = 5;
            element.style.borderBottomLeftRadius = element.style.borderBottomRightRadius = 5;
            ApplyLgoFrame(element, new Color(.08f, .36f, .58f, 1f), new Color(.90f, .72f, .34f, .94f));
        }

        private static void ApplyLgoHudMapCurrentMarker(VisualElement element)
        {
            element.style.position = Position.Absolute;
            element.style.top = 4;
            element.style.width = element.style.height = 16;
            element.style.borderTopLeftRadius = element.style.borderTopRightRadius = 8;
            element.style.borderBottomLeftRadius = element.style.borderBottomRightRadius = 8;
            ApplyLgoFrame(element, new Color(.12f, .72f, 1f, .98f), new Color(1f, .84f, .42f, 1f));
            element.style.borderLeftWidth = element.style.borderRightWidth = 2;
            element.style.borderTopWidth = element.style.borderBottomWidth = 2;
        }

        private static void ApplyLgoHudQuestPanel(VisualElement element)
        {
            element.AddToClassList(LgoHudQuestPanelClass);
            ApplyLgoHudInfoPanel(element);
            element.style.backgroundColor = UiGlassStrong;
            element.style.fontSize = 15;
            element.style.borderLeftWidth = 2;
            element.style.borderLeftColor = new Color(.13f, .58f, .86f, .88f);
        }

        private static void ApplyLgoHudQuestTab(Button button, bool selected, bool enabled, bool isLast)
        {
            button.AddToClassList(LgoHudQuestTabClass);
            if (enabled) ApplyLgoButton(button); else ApplyLgoDisabledAction(button);
            button.style.position = Position.Relative;
            button.style.left = button.style.right = button.style.top = button.style.bottom = StyleKeyword.Auto;
            button.style.flexGrow = 1;
            button.style.flexBasis = 0;
            button.style.minHeight = 30;
            button.style.fontSize = 12;
            button.style.marginRight = isLast ? 0 : 4;
            button.style.paddingLeft = 8;
            button.style.paddingRight = 8;
            button.style.whiteSpace = WhiteSpace.NoWrap;
            if (selected) ApplyLgoSelectedTab(button, true);
        }

        private static void ApplyLgoHudCombatAction(Button button, bool touch, bool primary = false)
        {
            button.AddToClassList(LgoHudCombatActionClass);
            button.EnableInClassList(LgoHudPrimaryCombatActionClass, primary);
            ApplyLgoButton(button);
            var size = primary ? (touch ? 84 : 70) : (touch ? 68 : 58);
            button.style.minHeight = size;
            button.style.maxHeight = size;
            button.style.minWidth = size;
            button.style.maxWidth = size;
            button.style.flexShrink = 0;
            button.style.fontSize = touch ? 10 : 9;
            button.style.whiteSpace = WhiteSpace.NoWrap;
            button.style.marginRight = 8;
            button.style.paddingLeft = button.style.paddingRight = 0;
            button.style.paddingTop = primary ? (touch ? 50 : 42) : (touch ? 42 : 35);
            button.style.paddingBottom = 3;
            button.style.unityTextAlign = TextAnchor.LowerCenter;
            var radius = size * .5f;
            button.style.borderTopLeftRadius = button.style.borderTopRightRadius = radius;
            button.style.borderBottomLeftRadius = button.style.borderBottomRightRadius = radius;
            if (primary)
            {
                button.style.backgroundColor = new Color(.025f, .095f, .16f, .98f);
                button.style.borderTopWidth = button.style.borderBottomWidth = 3;
                button.style.borderLeftWidth = button.style.borderRightWidth = 3;
                button.style.borderTopColor = button.style.borderBottomColor = UiGold;
                button.style.borderLeftColor = button.style.borderRightColor = UiGoldBorder;
            }
        }

        private static void AttachLgoHudActionIcon(Button button, Sprite sprite, bool touch)
        {
            var combat = button.ClassListContains(LgoHudCombatActionClass);
            var primaryCombat = button.ClassListContains(LgoHudPrimaryCombatActionClass);
            var navigation = button.ClassListContains(LgoHudNavigationActionClass);
            var icon = new VisualElement
            {
                name = button.name + " Icon",
                pickingMode = PickingMode.Ignore,
            };
            icon.AddToClassList(LgoHudActionIconClass);
            icon.style.position = Position.Absolute;
            icon.style.left = combat ? (primaryCombat ? (touch ? 20 : 17) : (touch ? 18 : 14)) : navigation ? (touch ? 25 : 25) : (touch ? 10 : 8);
            icon.style.top = combat ? (primaryCombat ? (touch ? 9 : 7) : (touch ? 7 : 5)) : navigation ? 7 : (touch ? 14 : 10);
            icon.style.width = icon.style.height = combat ? (primaryCombat ? (touch ? 44 : 36) : (touch ? 36 : 30)) : navigation ? (touch ? 34 : 30) : (touch ? 28 : 24);
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            if (!combat && !navigation)
            {
                button.style.paddingLeft = touch ? 43 : 36;
                button.style.paddingRight = touch ? 10 : 8;
                button.style.unityTextAlign = TextAnchor.MiddleRight;
            }
            button.Add(icon);
        }

        private static void ApplyLgoHudNavigationAction(Button button, bool touch, bool enabled)
        {
            button.AddToClassList(LgoHudNavigationActionClass);
            ApplyLgoButton(button);
            button.style.position = Position.Relative;
            button.style.left = button.style.right = button.style.top = button.style.bottom = StyleKeyword.Auto;
            button.style.width = touch ? 84 : 80;
            button.style.minWidth = touch ? 84 : 80;
            button.style.maxWidth = touch ? 84 : 80;
            button.style.height = touch ? 76 : 68;
            button.style.minHeight = touch ? 76 : 68;
            button.style.maxHeight = touch ? 76 : 68;
            button.style.marginLeft = 6;
            button.style.paddingLeft = button.style.paddingRight = 0;
            button.style.paddingTop = touch ? 46 : 40;
            button.style.paddingBottom = 4;
            button.style.fontSize = touch ? 12 : 11;
            button.style.unityTextAlign = TextAnchor.LowerCenter;
            button.style.whiteSpace = WhiteSpace.NoWrap;
            button.style.backgroundColor = new Color(.008f, .032f, .058f, .82f);
            button.style.borderTopColor = button.style.borderBottomColor = UiGoldBorder;
            button.style.borderLeftColor = button.style.borderRightColor = new Color(.12f, .38f, .58f, .64f);
            button.style.borderTopLeftRadius = button.style.borderTopRightRadius = 10;
            button.style.borderBottomLeftRadius = button.style.borderBottomRightRadius = 10;
            if (!enabled)
            {
                button.SetEnabled(false);
                button.style.opacity = .52f;
            }
        }

        private static void ApplyLgoHudShortcutAction(Button button, bool touch, bool enabled = false)
        {
            button.AddToClassList(LgoHudShortcutActionClass);
            ApplyLgoHudNavigationAction(button, touch, enabled);
        }

        private static void ApplyLgoHudContextAction(Button button, bool touch, float minWidth = 170f, float? minHeight = null)
        {
            button.AddToClassList(LgoHudContextActionClass);
            ApplyLgoButton(button);
            button.style.minHeight = minHeight ?? (touch ? 64 : 48);
            button.style.minWidth = minWidth;
            button.style.whiteSpace = WhiteSpace.Normal;
        }

        private static void ApplyLgoDisabledAction(Button button)
        {
            ApplyLgoButton(button);
            button.SetEnabled(false);
            button.style.opacity = .58f;
            button.style.color = new Color(.70f, .78f, .78f, .82f);
        }
    }
}
