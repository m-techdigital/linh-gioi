using System;
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

        internal static VisualElement NewSectionShell(string sigilText, string headingText, string sectionTitle, string elementName)
        {
            var shell = NewPreviewPanel(sigilText, headingText);
            if (!string.IsNullOrWhiteSpace(elementName)) shell.name = elementName;
            if (!string.IsNullOrWhiteSpace(sectionTitle)) shell.Add(NewSectionTitle(sectionTitle));
            return shell;
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

        internal static Label NewCompactStatusLabel(string text, Color color, int fontSize)
        {
            var label = NewStatusLabel(text, color);
            ApplyHudStatusCompact(label, fontSize);
            return label;
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
            return NewActionRow("LGO Runtime Action Row", Justify.FlexStart, 6, 0, buttons);
        }

        internal static VisualElement NewActionRow(string elementName, Justify justifyContent, float marginTop, float marginBottom, params Button[] buttons)
        {
            var row = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) row.name = elementName;
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.alignItems = Align.Center;
            row.style.justifyContent = justifyContent;
            row.style.marginTop = marginTop;
            row.style.marginBottom = marginBottom;
            foreach (var button in buttons) row.Add(button);
            return row;
        }

        internal static VisualElement NewIconStatusRow(string elementName, VisualElement icon, params VisualElement[] statusElements)
        {
            var row = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) row.name = elementName;
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.marginTop = 4;
            row.style.marginBottom = 6;
            row.style.paddingLeft = 4;
            row.style.paddingRight = 4;
            row.Add(icon);

            var statusColumn = new VisualElement();
            statusColumn.style.flexGrow = 1;
            statusColumn.style.marginLeft = 10;
            foreach (var statusElement in statusElements) statusColumn.Add(statusElement);
            row.Add(statusColumn);
            return row;
        }

        internal static TextField NewTextField(string label, string value)
        {
            var field = new TextField(label) { value = value };
            field.style.maxWidth = 420;
            field.style.marginTop = 8;
            field.style.color = RuntimeArtCatalog.Text;
            return field;
        }

        internal static void ApplyLobbyInputStyle(TextField field)
        {
            field.style.minHeight = 42;
            RuntimeUiSkin.ApplyPadding(field, 10, 4);
            RuntimeUiSkin.ApplyLobbyInputFrame(field);
        }

        internal static Button NewPrimaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.backgroundColor = RuntimeArtCatalog.Spirit;
            button.style.color = RuntimeArtCatalog.Background;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.minHeight = 58;
            button.style.fontSize = 16;
            button.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            var texture = LgoVisualAssetRegistryV3B.ButtonEnterWorldGoldTexture ?? LgoVisualAssetRegistryV2.ButtonPrimaryNormalTexture;
            if (texture != null)
            {
                button.style.backgroundColor = Color.clear;
                button.style.backgroundImage = new StyleBackground(texture);
            }
            return button;
        }

        internal static Button NewCompactPrimaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.minWidth = 144;
            button.style.minHeight = 44;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.fontSize = 14;
            RuntimeUiSkin.ApplyCompactActionFrame(button, new Color(0.03f, 0.22f, 0.34f, 0.92f), RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit);
            return button;
        }

        internal static Button NewQuietButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.minWidth = 88;
            button.style.backgroundColor = RuntimeArtCatalog.Background;
            button.style.color = RuntimeArtCatalog.Muted;
            return button;
        }

        internal static Button NewSecondaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            button.style.color = RuntimeArtCatalog.Text;
            var texture = LgoVisualAssetRegistryV2.ButtonSecondaryTexture;
            if (texture != null) button.style.backgroundImage = new StyleBackground(texture);
            return button;
        }

        internal static Button NewCompactSecondaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.minWidth = 132;
            button.style.minHeight = 42;
            button.style.fontSize = 14;
            RuntimeUiSkin.ApplyCompactActionFrame(button, new Color(0.04f, 0.13f, 0.22f, 0.92f), RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.SurfaceRaised, RuntimeArtCatalog.Gold);
            return button;
        }

        internal static Button NewIconButton(string label, Texture2D texture, Action action)
        {
            var button = NewSecondaryButton(string.Empty, action);
            button.style.minWidth = 112;
            button.style.minHeight = 48;
            button.style.flexDirection = FlexDirection.Row;
            button.style.alignItems = Align.Center;
            button.Add(NewIcon(texture, label));
            var text = new Label(label);
            text.style.color = RuntimeArtCatalog.Text;
            text.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.Add(text);
            return button;
        }

        internal static Toggle NewLocalSettingToggle(string label, bool value, Action changed)
        {
            var toggle = new Toggle(label) { value = value };
            toggle.style.minHeight = 42;
            toggle.style.marginTop = 7;
            toggle.style.marginBottom = 0;
            RuntimeUiSkin.ApplySettingToggleFrame(toggle, value ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Muted);
            var statePill = new Label();
            RuntimeUiSkin.ApplySettingToggleStatePill(statePill, value);
            toggle.Add(statePill);
            RuntimeUiSkin.ApplySettingToggleState(toggle, value);
            toggle.RegisterValueChangedCallback(evt =>
            {
                RuntimeUiSkin.ApplySettingToggleState(toggle, evt.newValue);
                changed();
            });
            return toggle;
        }

        internal static Button NewListButton(string name, string classId, Action action)
        {
            var button = NewSecondaryButton(name + "\nKiếm tu sơ nhập", action);
            button.style.minWidth = 230;
            button.style.minHeight = 58;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.style.paddingLeft = 14;
            RuntimeUiSkin.ApplyCompactActionFrame(button, new Color(0.03f, 0.15f, 0.25f, 0.88f), RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold);
            button.tooltip = "Chọn nhân vật tu luyện";
            return button;
        }

        internal static VisualElement NewRuntimeIcon(Texture2D texture, int size, string tooltip)
        {
            var icon = new VisualElement();
            icon.style.width = size;
            icon.style.height = size;
            icon.style.minWidth = size;
            icon.style.minHeight = size;
            RuntimeUiSkin.ApplyRuntimeIconFrame(icon, new Color(0.02f, 0.08f, 0.16f, 0.82f));
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = tooltip;
            return icon;
        }

        internal static VisualElement NewImageLayer(string elementName, Texture2D texture, ScaleMode scaleMode, string tooltip = null)
        {
            var layer = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) layer.name = elementName;
            layer.pickingMode = PickingMode.Ignore;
            layer.style.unityBackgroundScaleMode = scaleMode;
            if (texture != null) layer.style.backgroundImage = new StyleBackground(texture);
            if (!string.IsNullOrWhiteSpace(tooltip)) layer.tooltip = tooltip;
            return layer;
        }

        internal static VisualElement NewCombatCooldownIcon()
        {
            var icon = new VisualElement();
            icon.name = "LGO M6 Combat Cooldown Runtime Icon v0.46";
            icon.style.width = 52;
            icon.style.height = 52;
            icon.style.marginBottom = 0;
            RuntimeUiSkin.ApplyCombatCooldownIconFrame(icon);
            var texture = CombatPlaceholderAssets.CooldownReadyTexture;
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = "Sẵn sàng tấn công thử.";
            return icon;
        }

        internal static void ApplyCombatPanelSkin(VisualElement panel)
        {
            panel.style.backgroundImage = new StyleBackground();
            panel.style.backgroundColor = RuntimeUiSkin.BlueGlass;
            RuntimeUiSkin.ApplyEdgeFrame(panel, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold);
        }

        internal static void ApplyV2PanelSkin(VisualElement panel)
        {
            panel.style.backgroundImage = new StyleBackground();
            panel.style.backgroundColor = new Color(0.02f, 0.07f, 0.14f, 0.90f);
            RuntimeUiSkin.ApplyEdgeFrame(panel, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold);
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

        private static Button NewButton(string label, Action action)
        {
            var button = new Button(action) { text = label };
            button.style.minWidth = 132;
            button.style.minHeight = 44;
            button.style.marginTop = 8;
            button.style.marginRight = 8;
            RuntimeUiSkin.ApplyBaseButtonFrame(button);
            return button;
        }

        private static VisualElement NewIcon(Texture2D texture, string tooltip)
        {
            var icon = new VisualElement();
            icon.style.width = 28;
            icon.style.height = 28;
            icon.style.marginRight = 8;
            icon.style.marginLeft = 4;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = tooltip;
            return icon;
        }
    }
}
