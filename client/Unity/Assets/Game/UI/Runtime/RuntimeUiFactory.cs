using UnityEngine;
using UnityEngine.UIElements;
using LinhGioi.Art;

namespace LinhGioi.UI
{
    internal static class RuntimeUiFactory
    {
        internal const string FactoryMarker = "LGO Runtime UI Primitive Factory v1";

        internal static VisualElement NewPanel(float maxWidth)
        {
            var panel = new VisualElement();
            panel.style.maxWidth = maxWidth;
            panel.style.minWidth = 300;
            panel.style.width = Length.Percent(100);
            panel.style.marginRight = 0;
            panel.style.marginBottom = 12;
            RuntimeUiSkin.ApplyPadding(panel, 16, 14);
            panel.style.backgroundColor = RuntimeArtCatalog.Surface;
            RuntimeUiSkin.ApplyPanelFrame(panel);
            return panel;
        }

        internal static VisualElement NewPreviewPanel(string sigilText = "LINH MÔN", string headingText = null)
        {
            var preview = new VisualElement();
            preview.style.minWidth = 220;
            preview.style.flexGrow = 1;
            RuntimeUiSkin.ApplyPadding(preview, 14, 12);
            RuntimeUiSkin.ApplyPreviewPanelFrame(preview);
            var sigil = new Label(sigilText);
            sigil.style.color = RuntimeArtCatalog.Spirit;
            sigil.style.unityFontStyleAndWeight = FontStyle.Bold;
            sigil.style.fontSize = 11;
            preview.Add(sigil);
            if (!string.IsNullOrWhiteSpace(headingText))
            {
                var heading = new Label(headingText);
                heading.style.color = RuntimeArtCatalog.Text;
                heading.style.unityFontStyleAndWeight = FontStyle.Bold;
                heading.style.fontSize = 15;
                heading.style.marginTop = 2;
                heading.style.marginBottom = 6;
                preview.Add(heading);
            }
            return preview;
        }

        internal static VisualElement NewReadabilityRow(string title, string value, Color accent)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.marginTop = 8;
            RuntimeUiSkin.ApplyPadding(row, 10, 7);
            RuntimeUiSkin.ApplyInsetRowFrame(row, accent);
            var titleLabel = new Label(title);
            titleLabel.style.minWidth = 86;
            titleLabel.style.marginRight = 8;
            titleLabel.style.color = accent;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            var valueLabel = new Label(value);
            valueLabel.style.flexGrow = 1;
            valueLabel.style.color = RuntimeArtCatalog.Text;
            valueLabel.style.whiteSpace = WhiteSpace.Normal;
            row.Add(titleLabel);
            row.Add(valueLabel);
            return row;
        }

        internal static VisualElement NewWorldHudGroup(string name, Color accent)
        {
            var group = new VisualElement { name = name };
            group.style.marginTop = 8;
            group.style.marginBottom = 8;
            RuntimeUiSkin.ApplyPadding(group, 8, 7);
            RuntimeUiSkin.ApplyWorldHudGroupFrame(group, accent);
            return group;
        }

        internal static void ApplyHudStatusCompact(Label label, int fontSize)
        {
            label.style.fontSize = fontSize;
            label.style.marginTop = 4;
            label.style.paddingLeft = 8;
            label.style.paddingRight = 8;
            label.style.paddingTop = 5;
            label.style.paddingBottom = 5;
            RuntimeUiSkin.ApplyHudStatusCompactFrame(label);
        }

        internal static Label NewSectionTitle(string text)
        {
            var label = new Label(text);
            label.style.fontSize = 20;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.color = RuntimeArtCatalog.Text;
            label.style.marginBottom = 8;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            return label;
        }

        internal static Label NewMutedLabel(string text)
        {
            var label = new Label(text);
            label.style.color = RuntimeArtCatalog.Muted;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        internal static VisualElement NewLoginOrnamentRule(string name)
        {
            var row = new VisualElement { name = name };
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.justifyContent = Justify.Center;
            row.style.width = Length.Percent(86);
            row.style.height = 8;
            row.style.marginTop = 1;
            row.style.marginBottom = 4;
            row.Add(NewLoginOrnamentLine(RuntimeArtCatalog.Gold));
            return row;
        }

        internal static Label NewStatusLabel(string text, Color color)
        {
            var label = new Label(text);
            label.style.color = color;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.marginTop = 8;
            RuntimeUiSkin.ApplyPadding(label, 10, 6);
            RuntimeUiSkin.ApplyInsetRowFrame(label, color);
            return label;
        }

        internal static VisualElement NewButtonRow(params Button[] buttons)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.marginTop = 6;
            foreach (var button in buttons) row.Add(button);
            return row;
        }

        private static VisualElement NewLoginOrnamentLine(Color color)
        {
            var line = new VisualElement();
            line.style.flexGrow = 1;
            line.style.height = 1;
            line.style.backgroundColor = color;
            line.style.opacity = 0.64f;
            return line;
        }
    }
}
