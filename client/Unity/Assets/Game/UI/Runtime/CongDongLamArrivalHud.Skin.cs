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

        private static void ApplyLgoSelectedTab(Button button, bool selected)
        {
            button.style.backgroundColor = selected ? UiBlue : new Color(.038f, .118f, .172f, .98f);
            button.style.color = selected ? new Color(.98f, .95f, .78f, .98f) : UiText;
            button.style.borderBottomWidth = selected ? 2 : 1;
            button.style.borderBottomColor = selected ? UiGold : new Color(.56f, .68f, .70f, .58f);
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
