using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
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
