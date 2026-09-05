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
            panel.style.minWidth = RuntimeUiSpacing.PanelMinWidth;
            panel.style.width = Length.Percent(100);
            panel.style.marginRight = 0;
            panel.style.marginBottom = RuntimeUiSpacing.PanelMarginBottom;
            RuntimeUiSkin.ApplyPadding(panel, RuntimeUiSpacing.PanelPaddingHorizontal, RuntimeUiSpacing.PanelPaddingVertical);
            panel.style.backgroundColor = RuntimeArtCatalog.Surface;
            RuntimeUiSkin.ApplyPanelFrame(panel);
            return panel;
        }

        internal static VisualElement NewPreviewPanel(string sigilText = "LINH MÔN", string headingText = null)
        {
            var preview = new VisualElement();
            preview.style.minWidth = RuntimeUiSpacing.PreviewPanelMinWidth;
            preview.style.flexGrow = 1;
            RuntimeUiSkin.ApplyPadding(preview, RuntimeUiSpacing.PreviewPanelPaddingHorizontal, RuntimeUiSpacing.PreviewPanelPaddingVertical);
            RuntimeUiSkin.ApplyPreviewPanelFrame(preview);
            var sigil = new Label(sigilText);
            RuntimeUiSkin.ApplyText(sigil, RuntimeArtCatalog.Spirit, 11, true);
            preview.Add(sigil);
            if (!string.IsNullOrWhiteSpace(headingText))
            {
                var heading = new Label(headingText);
                RuntimeUiSkin.ApplyText(heading, RuntimeArtCatalog.Text, 15, true);
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
            row.style.marginTop = RuntimeUiSpacing.RowMarginTop;
            RuntimeUiSkin.ApplyPadding(row, RuntimeUiSpacing.ReadabilityRowPaddingHorizontal, RuntimeUiSpacing.ReadabilityRowPaddingVertical);
            RuntimeUiSkin.ApplyInsetRowFrame(row, accent);
            var titleLabel = new Label(title);
            titleLabel.style.minWidth = RuntimeUiSpacing.ReadabilityTitleMinWidth;
            titleLabel.style.marginRight = RuntimeUiSpacing.RowGap;
            RuntimeUiSkin.ApplyText(titleLabel, accent, 0, true);
            var valueLabel = new Label(value);
            valueLabel.style.flexGrow = 1;
            RuntimeUiSkin.ApplyText(valueLabel, RuntimeArtCatalog.Text);
            valueLabel.style.whiteSpace = WhiteSpace.Normal;
            row.Add(titleLabel);
            row.Add(valueLabel);
            return row;
        }

        internal static VisualElement NewWorldHudGroup(string name, Color accent)
        {
            var group = new VisualElement { name = name };
            RuntimeUiSkin.ApplyVerticalMargin(group, RuntimeUiSpacing.WorldHudGroupMarginVertical, RuntimeUiSpacing.WorldHudGroupMarginVertical);
            RuntimeUiSkin.ApplyPadding(group, RuntimeUiSpacing.WorldHudGroupPaddingHorizontal, RuntimeUiSpacing.WorldHudGroupPaddingVertical);
            RuntimeUiSkin.ApplyWorldHudGroupFrame(group, accent);
            return group;
        }

        internal static VisualElement NewWorldHudRoot(string name, float maxWidth)
        {
            var hud = NewPanel(maxWidth);
            hud.name = name;
            hud.style.maxWidth = maxWidth;
            hud.style.alignSelf = Align.FlexStart;
            RuntimeUiSkin.ApplyPadding(hud, RuntimeUiSpacing.WorldHudRootPaddingHorizontal, RuntimeUiSpacing.WorldHudRootPaddingVertical);
            return hud;
        }

        internal static VisualElement NewCharacterHallPanel(RuntimeUiLayoutProfile layout)
        {
            var panel = NewPanel(840);
            panel.name = "LGO Character Hall V3B Composition Panel";
            RuntimeUiSkin.ApplyCharacterHallPanelFrame(panel);
            panel.style.maxWidth = 800;
            panel.style.minHeight = 452;
            RuntimeUiSkin.ApplyPadding(panel, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingTop, layout.LobbyPanelPaddingBottom);
            panel.style.alignSelf = Align.FlexStart;
            return panel;
        }

        internal static void ApplyHudStatusCompact(Label label, int fontSize)
        {
            label.style.fontSize = fontSize;
            label.style.marginTop = RuntimeUiSpacing.CompactStatusMarginTop;
            RuntimeUiSkin.ApplyPadding(label, RuntimeUiSpacing.CompactStatusPaddingHorizontal, RuntimeUiSpacing.CompactStatusPaddingHorizontal, RuntimeUiSpacing.CompactStatusPaddingTop, RuntimeUiSpacing.CompactStatusPaddingBottom);
            RuntimeUiSkin.ApplyHudStatusCompactFrame(label);
        }

        internal static Label NewCompactStatusLabel(string text, Color color, int fontSize)
        {
            var label = NewStatusLabel(text, color);
            ApplyHudStatusCompact(label, fontSize);
            return label;
        }

        internal static void ApplyStatusAccent(Label label, Color accent)
        {
            if (label == null) return;
            label.style.borderLeftColor = accent;
            label.style.color = accent;
        }

        internal static Label NewSectionTitle(string text)
        {
            var label = new Label(text);
            RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Text, 20, true, TextAnchor.MiddleCenter);
            label.style.marginBottom = 8;
            return label;
        }

        internal static VisualElement NewSectionHeaderBlock(string title, Color ornamentColor, string elementName = null)
        {
            var block = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) block.name = elementName;
            block.style.flexDirection = FlexDirection.Column;
            block.Add(NewSectionTitle(title));
            block.Add(NewOrnamentRule(ornamentColor));
            return block;
        }

        internal static VisualElement NewBadgeStrip(string elementName, params (string title, string value)[] badges)
        {
            var strip = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) strip.name = elementName;
            strip.style.flexDirection = FlexDirection.Row;
            strip.style.flexWrap = Wrap.Wrap;
            strip.style.marginBottom = RuntimeUiSpacing.BadgeStripMarginBottom;
            foreach (var badge in badges) strip.Add(NewBadge(badge.title, badge.value));
            return strip;
        }

        internal static VisualElement NewBadge(string title, string value)
        {
            var badge = new VisualElement();
            RuntimeUiSkin.ApplyPadding(badge, RuntimeUiSpacing.BadgePaddingHorizontal, RuntimeUiSpacing.BadgePaddingVertical);
            badge.style.marginRight = RuntimeUiSpacing.RowGap;
            badge.style.marginBottom = RuntimeUiSpacing.BadgeMarginBottom;
            RuntimeUiSkin.ApplyBadgeFrame(badge);
            var titleLabel = new Label(title);
            RuntimeUiSkin.ApplyText(titleLabel, RuntimeArtCatalog.Gold, 11);
            var valueLabel = new Label(value);
            RuntimeUiSkin.ApplyText(valueLabel, RuntimeArtCatalog.Text, 12);
            badge.Add(titleLabel);
            badge.Add(valueLabel);
            return badge;
        }

        internal static Label NewToast(string text)
        {
            var label = new Label(text);
            label.style.marginTop = RuntimeUiSpacing.ToastMarginTop;
            RuntimeUiSkin.ApplyPadding(label, RuntimeUiSpacing.ToastPaddingHorizontal, RuntimeUiSpacing.ToastPaddingVertical);
            label.style.whiteSpace = WhiteSpace.Normal;
            RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Text);
            RuntimeUiSkin.ApplyToastFrame(label, RuntimeArtCatalog.Gold);
            return label;
        }

        internal static void ApplyStatusChip(Label label, Color accent)
        {
            label.style.maxWidth = 380;
            label.style.marginRight = RuntimeUiSpacing.RowGap;
            label.style.whiteSpace = WhiteSpace.Normal;
            RuntimeUiSkin.ApplyPadding(label, 14, 6);
            RuntimeUiSkin.ApplyStatusChipFrame(label, accent);
        }

        internal static Label NewMutedLabel(string text)
        {
            var label = new Label(text);
            RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Muted);
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

        internal static VisualElement NewOrnamentRule(Color color)
        {
            var rule = new VisualElement();
            rule.style.height = 2;
            rule.style.marginBottom = 10;
            rule.style.backgroundColor = color;
            rule.style.opacity = 0.8f;
            return rule;
        }

        internal static Label NewStatusLabel(string text, Color color)
        {
            var label = new Label(text);
            RuntimeUiSkin.ApplyText(label, color);
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.marginTop = RuntimeUiSpacing.StatusLabelMarginTop;
            RuntimeUiSkin.ApplyPadding(label, RuntimeUiSpacing.StatusLabelPaddingHorizontal, RuntimeUiSpacing.StatusLabelPaddingVertical);
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
            row.style.marginTop = RuntimeUiSpacing.CompactStatusMarginTop;
            row.style.marginBottom = 6;
            RuntimeUiSkin.ApplyPadding(row, 4, 4, 0, 0);
            row.Add(icon);

            var statusColumn = new VisualElement();
            statusColumn.style.flexGrow = 1;
            statusColumn.style.marginLeft = RuntimeUiSpacing.StatusLabelPaddingHorizontal;
            foreach (var statusElement in statusElements) statusColumn.Add(statusElement);
            row.Add(statusColumn);
            return row;
        }

        internal static void ApplyCombatButtonSkin(Button button, Texture2D texture, bool coolingDown)
        {
            if (button == null) return;
            if (texture != null) button.style.backgroundImage = new StyleBackground(texture);
            RuntimeUiSkin.ApplyButtonMetrics(button, coolingDown ? 142 : 132, 44, coolingDown ? 13 : 14, true);
            RuntimeUiSkin.ApplyPadding(button, 14, 14, 0, 0);
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
            RuntimeUiSkin.ApplyButtonMetrics(button, minHeight: 58, fontSize: 16, bold: true);
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
            RuntimeUiSkin.ApplyButtonMetrics(button, 144, 44, 14, true);
            RuntimeUiSkin.ApplyCompactActionFrame(button, new Color(0.03f, 0.22f, 0.34f, 0.92f), RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit);
            return button;
        }

        internal static Button NewQuietButton(string label, Action action)
        {
            var button = NewButton(label, action);
            RuntimeUiSkin.ApplyButtonMetrics(button, minWidth: 88);
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
            RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSpacing.BaseButtonMinWidth, RuntimeUiSpacing.CompactButtonMinHeight, RuntimeUiSpacing.CompactButtonFontSize);
            RuntimeUiSkin.ApplyCompactActionFrame(button, new Color(0.04f, 0.13f, 0.22f, 0.92f), RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.SurfaceRaised, RuntimeArtCatalog.Gold);
            return button;
        }

        internal static Button NewIconButton(string label, Texture2D texture, Action action)
        {
            var button = NewSecondaryButton(string.Empty, action);
            RuntimeUiSkin.ApplyButtonMetrics(button, 112, 48);
            button.style.flexDirection = FlexDirection.Row;
            button.style.alignItems = Align.Center;
            button.Add(NewIcon(texture, label));
            var text = new Label(label);
            RuntimeUiSkin.ApplyText(text, RuntimeArtCatalog.Text, bold: true);
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
            RuntimeUiSkin.ApplyButtonMetrics(button, 230, 58);
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
            icon.style.width = RuntimeUiSpacing.CooldownIconSize;
            icon.style.height = RuntimeUiSpacing.CooldownIconSize;
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
            RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSpacing.BaseButtonMinWidth, RuntimeUiSpacing.BaseButtonMinHeight);
            button.style.marginTop = RuntimeUiSpacing.BaseButtonMarginTop;
            button.style.marginRight = RuntimeUiSpacing.BaseButtonMarginRight;
            RuntimeUiSkin.ApplyBaseButtonFrame(button);
            return button;
        }

        private static VisualElement NewIcon(Texture2D texture, string tooltip)
        {
            var icon = new VisualElement();
            icon.style.width = RuntimeUiSpacing.RuntimeIconSmall;
            icon.style.height = RuntimeUiSpacing.RuntimeIconSmall;
            icon.style.marginRight = RuntimeUiSpacing.IconMarginRight;
            icon.style.marginLeft = RuntimeUiSpacing.IconMarginLeft;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = tooltip;
            return icon;
        }
    }
}
