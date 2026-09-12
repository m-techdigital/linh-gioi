using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private static readonly Color UiGlass = new Color(.012f, .045f, .078f, .94f);
        private static readonly Color UiGlassStrong = new Color(.020f, .060f, .100f, .96f);
        private static readonly Color UiGlassRaised = new Color(.026f, .082f, .128f, .94f);
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
            ApplyLgoFrame(element, new Color(.014f, .050f, .086f, .97f), new Color(.86f, .66f, .34f, .76f));
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

        private static void ApplyLgoItemIcon(Label label)
        {
            label.style.width = 58;
            label.style.height = 58;
            label.style.marginTop = 8;
            label.style.marginBottom = 4;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoFrame(label, new Color(.025f, .075f, .130f, .96f), new Color(.92f, .72f, .36f, .86f));
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
            button.style.minHeight = primary ? 56 : 46;
            button.style.minWidth = 0;
            button.style.fontSize = primary ? 24 : 17;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.unityFontStyleAndWeight = primary ? FontStyle.Bold : FontStyle.Normal;
            button.style.color = primary ? new Color(.10f, .07f, .03f, 1f) : UiText;
            ApplyLgoFrame(button, primary ? UiGold : new Color(.045f, .13f, .18f, .98f), primary ? new Color(.98f, .86f, .48f, .92f) : new Color(.50f, .58f, .58f, .55f));
        }

        private static void ApplyLgoSelectedTab(Button button, bool selected)
        {
            button.style.backgroundColor = selected ? UiBlue : new Color(.045f, .13f, .18f, .98f);
            button.style.color = selected ? new Color(.98f, .95f, .78f, .98f) : UiText;
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
