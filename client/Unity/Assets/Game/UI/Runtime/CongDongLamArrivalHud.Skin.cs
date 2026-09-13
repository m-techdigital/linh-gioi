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
        private const string LgoInventoryMainTabClass = "lgo-inventory-main-tab";
        private const string LgoInventoryFilterChipClass = "lgo-inventory-filter-chip";
        private const string LgoInventoryToolbarActionClass = "lgo-inventory-toolbar-action";
        private const string LgoInventoryGridCellClass = "lgo-inventory-grid-cell";
        private const string LgoModalCloseButtonClass = "lgo-modal-close-button";
        private const string LgoHudCombatActionClass = "lgo-hud-combat-action";
        private const string LgoHudShortcutActionClass = "lgo-hud-shortcut-action";
        private const string LgoHudContextActionClass = "lgo-hud-context-action";
        private const string LgoHudQuestTabClass = "lgo-hud-quest-tab";
        private const string LgoDialoguePrimaryActionClass = "lgo-dialogue-primary-action";
        private const string LgoDialogueSecondaryActionClass = "lgo-dialogue-secondary-action";
        private const string LgoEntrySecondaryActionClass = "lgo-entry-secondary-action";
        private const string LgoEntrySideActionClass = "lgo-entry-side-action";
        private const string LgoCharacterSelectCardClass = "lgo-character-select-card";
        private const string LgoCharacterSelectPrimaryActionClass = "lgo-character-select-primary-action";

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

        private static void ApplyLgoModalShell(VisualElement element, float padding = 12)
        {
            ApplyLgoGlassPanel(element);
            element.style.flexDirection = FlexDirection.Column;
            element.style.paddingLeft = element.style.paddingRight = padding;
            element.style.paddingTop = element.style.paddingBottom = padding;
        }



        private static void ApplyLgoInputField(VisualElement element)
        {
            ApplyLgoFrame(element, new Color(.010f, .035f, .060f, .86f), new Color(.46f, .64f, .74f, .50f));
            element.style.color = UiSubText;
        }

        private static void ApplyLgoDetailCard(VisualElement element)
        {
            ApplyLgoFrame(element, new Color(.012f, .040f, .074f, .97f), new Color(.90f, .70f, .36f, .82f));
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
            element.style.height = 2;
            element.style.backgroundColor = new Color(.95f, .75f, .36f, .72f);
        }

        private static void ApplyLgoItemIcon(VisualElement icon)
        {
            icon.style.width = 58;
            icon.style.height = 58;
            icon.style.marginTop = 8;
            icon.style.marginBottom = 4;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            ApplyLgoFrame(icon, new Color(.020f, .070f, .128f, .96f), new Color(.96f, .76f, .36f, .90f));
            icon.style.borderTopWidth = icon.style.borderBottomWidth = 2;
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

        private static void ApplyLgoButton(Button button, bool primary = false)
        {
            button.style.minHeight = primary ? 48 : 38;
            button.style.minWidth = 0;
            button.style.fontSize = primary ? 20 : 14;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.unityFontStyleAndWeight = primary ? FontStyle.Bold : FontStyle.Normal;
            button.style.color = primary ? new Color(.10f, .07f, .03f, 1f) : UiText;
            ApplyLgoFrame(button, primary ? UiGold : new Color(.038f, .118f, .172f, .98f), primary ? new Color(.98f, .86f, .48f, .94f) : new Color(.56f, .68f, .70f, .58f));
            if (primary)
            {
                button.style.borderTopWidth = 2;
                button.style.borderBottomWidth = 2;
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
            button.style.flexGrow = 0;
            button.style.flexBasis = 128;
            button.style.minHeight = touch ? 38 : 32;
            button.style.fontSize = 13;
            button.style.marginRight = 4;
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
            cell.style.flexBasis = new Length(15.8f, LengthUnit.Percent);
            cell.style.height = 108;
            cell.style.marginRight = 6;
            cell.style.marginBottom = 7;
            cell.style.alignItems = Align.Center;
            cell.style.justifyContent = Justify.Center;
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
            ApplyLgoDisabledAction(button);
            button.style.height = 38;
            button.style.marginBottom = 10;
            button.style.fontSize = 13;
            button.style.whiteSpace = WhiteSpace.NoWrap;
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

        private static void ApplyLgoHudCombatAction(Button button, bool touch)
        {
            button.AddToClassList(LgoHudCombatActionClass);
            ApplyLgoButton(button);
            button.style.minHeight = touch ? 48 : 38;
            button.style.minWidth = touch ? 104 : 88;
            button.style.maxWidth = touch ? 136 : 122;
            button.style.fontSize = touch ? 14 : 13;
            button.style.whiteSpace = WhiteSpace.NoWrap;
            button.style.marginRight = 6;
        }

        private static void ApplyLgoHudShortcutAction(Button button, bool touch)
        {
            button.AddToClassList(LgoHudShortcutActionClass);
            ApplyLgoDisabledAction(button);
            button.style.position = Position.Relative;
            button.style.left = button.style.right = button.style.top = button.style.bottom = StyleKeyword.Auto;
            button.style.minHeight = touch ? 46 : 36;
            button.style.minWidth = touch ? 110 : 92;
            button.style.maxWidth = 118;
            button.style.fontSize = 13;
            button.style.whiteSpace = WhiteSpace.NoWrap;
            button.style.marginLeft = 8;
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
