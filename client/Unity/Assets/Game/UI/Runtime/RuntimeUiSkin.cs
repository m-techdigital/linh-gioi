using UnityEngine;
using UnityEngine.UIElements;
using LinhGioi.Art;

namespace LinhGioi.UI
{
    internal enum RuntimeUiButtonTier
    {
        Small,
        Compact,
        Standard,
        Primary,
        Hero
    }

    internal static class RuntimeUiSkin
    {
        internal const string FoundationMarker = "LGO Runtime UI Skin Foundation v1";

        internal static readonly Color DeepGlass = new Color(0.035f, 0.040f, 0.046f, 0.82f);
        internal static readonly Color DenseGlass = new Color(0.035f, 0.040f, 0.046f, 0.92f);
        internal static readonly Color BlueGlass = new Color(0.055f, 0.063f, 0.074f, 0.90f);
        internal static readonly Color IvoryText = new Color(0.89f, 0.87f, 0.82f, 1f);
        internal static readonly Color SoftLoginGlass = new Color(0.005f, 0.018f, 0.040f, 0.36f);
        internal static readonly Color LightGoldBorder = new Color(0.93f, 0.73f, 0.36f, 0.20f);
        internal static readonly Color MediumGoldBorder = new Color(0.93f, 0.73f, 0.36f, 0.48f);
        internal static readonly Color LightSpiritBorder = new Color(0.14f, 0.78f, 0.90f, 0.24f);
        internal static readonly Color MediumSpiritBorder = new Color(0.14f, 0.78f, 0.90f, 0.46f);
        internal const string SettingToggleStatePillName = "LGO Setting Row State Pill";

        internal static void ApplyRadius(VisualElement element, float radius)
        {
            element.style.borderTopLeftRadius = radius;
            element.style.borderTopRightRadius = radius;
            element.style.borderBottomLeftRadius = radius;
            element.style.borderBottomRightRadius = radius;
        }

        internal static void ApplyPadding(VisualElement element, float horizontal, float vertical)
        {
            element.style.paddingLeft = horizontal;
            element.style.paddingRight = horizontal;
            element.style.paddingTop = vertical;
            element.style.paddingBottom = vertical;
        }

        internal static void ApplyPadding(VisualElement element, float left, float right, float top, float bottom)
        {
            element.style.paddingLeft = left;
            element.style.paddingRight = right;
            element.style.paddingTop = top;
            element.style.paddingBottom = bottom;
        }

        internal static void ApplyMargin(VisualElement element, float left, float right, float top, float bottom)
        {
            element.style.marginLeft = left;
            element.style.marginRight = right;
            element.style.marginTop = top;
            element.style.marginBottom = bottom;
        }

        internal static void ApplyVerticalMargin(VisualElement element, float top, float bottom)
        {
            element.style.marginTop = top;
            element.style.marginBottom = bottom;
        }

        internal static void ApplyText(Label label, Color color, float fontSize = 0f, bool bold = false, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            label.style.color = color;
            label.style.unityTextAlign = alignment;
            if (fontSize > 0f) label.style.fontSize = fontSize;
            if (bold) label.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        internal static void ApplyStatusAccent(Label label, Color accent)
        {
            if (label == null) return;
            if (accent == RuntimeArtCatalog.Spirit) accent = IvoryText;
            label.style.borderLeftColor = accent;
            label.style.color = accent;
        }

        internal static void ApplyButtonMetrics(Button button, float minWidth = 0f, float minHeight = 0f, float fontSize = 0f, bool bold = false, WhiteSpace whiteSpace = WhiteSpace.NoWrap)
        {
            if (minWidth > 0f) button.style.minWidth = minWidth;
            if (minHeight > 0f) button.style.minHeight = minHeight;
            if (fontSize > 0f) button.style.fontSize = fontSize;
            if (bold) button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.whiteSpace = whiteSpace;
            RuntimeUiOverflowGuard.ApplyButton(button);
        }

        internal static void ApplyButtonTier(Button button, RuntimeUiButtonTier tier, bool bold = false)
        {
            var minWidth = tier == RuntimeUiButtonTier.Small ? RuntimeUiSpacing.ButtonTierSmallMinWidth :
                tier == RuntimeUiButtonTier.Compact ? RuntimeUiSpacing.ButtonTierCompactMinWidth :
                tier == RuntimeUiButtonTier.Standard ? RuntimeUiSpacing.ButtonTierStandardMinWidth :
                tier == RuntimeUiButtonTier.Primary ? RuntimeUiSpacing.ButtonTierPrimaryMinWidth : 0;
            var minHeight = tier == RuntimeUiButtonTier.Small ? RuntimeUiSpacing.ButtonTierSmallMinHeight :
                tier == RuntimeUiButtonTier.Compact ? RuntimeUiSpacing.ButtonTierCompactMinHeight :
                tier == RuntimeUiButtonTier.Standard ? RuntimeUiSpacing.ButtonTierStandardMinHeight :
                tier == RuntimeUiButtonTier.Primary ? RuntimeUiSpacing.ButtonTierPrimaryMinHeight : RuntimeUiSpacing.ButtonTierHeroMinHeight;
            var fontSize = tier == RuntimeUiButtonTier.Small ? RuntimeUiSpacing.ButtonTierSmallFontSize :
                tier == RuntimeUiButtonTier.Compact ? RuntimeUiSpacing.ButtonTierCompactFontSize :
                tier == RuntimeUiButtonTier.Standard ? RuntimeUiSpacing.ButtonTierStandardFontSize :
                tier == RuntimeUiButtonTier.Primary ? RuntimeUiSpacing.ButtonTierPrimaryFontSize : RuntimeUiSpacing.ButtonTierHeroFontSize;
            ApplyButtonMetrics(button, minWidth, minHeight, fontSize, bold || tier == RuntimeUiButtonTier.Primary || tier == RuntimeUiButtonTier.Hero);
        }

        internal static void ApplyInputMetrics(TextField field, float maxWidth = 0f, float minHeight = 0f, float marginTop = 0f)
        {
            if (maxWidth > 0f) field.style.maxWidth = maxWidth;
            if (minHeight > 0f) field.style.minHeight = minHeight;
            if (marginTop > 0f) field.style.marginTop = marginTop;
            field.style.color = RuntimeArtCatalog.Text;
        }

        internal static void ApplyEdgeFrame(VisualElement element, Color left, Color top, Color right, Color bottom, float leftWidth = 2f, float otherWidth = 1f)
        {
            element.style.borderLeftColor = left;
            element.style.borderLeftWidth = leftWidth;
            element.style.borderTopColor = top;
            element.style.borderTopWidth = otherWidth;
            element.style.borderRightColor = right;
            element.style.borderRightWidth = otherWidth;
            element.style.borderBottomColor = bottom;
            element.style.borderBottomWidth = otherWidth;
        }

        internal static void ApplyPanelFrame(VisualElement element)
        {
            ApplyRadius(element, 8);
            ApplyEdgeFrame(element, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, 1f, 1f);
        }

        internal static void ApplyOrnamentedShellFrame(VisualElement element)
        {
            ApplyPanelFrame(element);
            ApplyRadius(element, 4);
            const string ornamentClass = "lgo-ornamented-shell";
            if (element.ClassListContains(ornamentClass)) return;
            element.AddToClassList(ornamentClass);
            // Draw inside the shell padding; no extra layout children or input interception.
            element.generateVisualContent += context =>
            {
                var width = element.resolvedStyle.width;
                var height = element.resolvedStyle.height;
                if (float.IsNaN(width) || float.IsNaN(height) || width < 48 || height < 48) return;
                var painter = context.painter2D;
                painter.lineWidth = 1;
                painter.strokeColor = RuntimeArtCatalog.Gold;
                DrawShellCorner(painter, new Vector2(3, 3), 1, 1);
                DrawShellCorner(painter, new Vector2(width - 3, 3), -1, 1);
                DrawShellCorner(painter, new Vector2(3, height - 3), 1, -1);
                DrawShellCorner(painter, new Vector2(width - 3, height - 3), -1, -1);
            };
        }

        private static void DrawShellCorner(Painter2D painter, Vector2 origin, float x, float y)
        {
            Vector2 Point(float a, float b) => origin + new Vector2(a * x, b * y);
            painter.BeginPath();
            painter.MoveTo(Point(0, 18));
            painter.LineTo(Point(0, 7));
            painter.LineTo(Point(7, 0));
            painter.LineTo(Point(18, 0));
            painter.MoveTo(Point(4, 22));
            painter.LineTo(Point(4, 11));
            painter.LineTo(Point(11, 4));
            painter.LineTo(Point(22, 4));
            painter.Stroke();
            painter.BeginPath();
            painter.MoveTo(Point(3, 3));
            painter.LineTo(Point(10, 6));
            painter.LineTo(Point(6, 10));
            painter.ClosePath();
            painter.Stroke();
        }

        internal static void ApplyInsetRowFrame(VisualElement element, Color accent)
        {
            element.style.backgroundColor = DeepGlass;
            ApplyEdgeFrame(element, accent, RuntimeArtCatalog.SurfaceRaised, Color.clear, RuntimeArtCatalog.SurfaceRaised, 2f, 1f);
            element.style.borderRightWidth = 0;
        }

        internal static void ApplyLoginCtaBacking(VisualElement element)
        {
            element.style.backgroundColor = SoftLoginGlass;
            ApplyPanelFrame(element);
        }

        internal static void ApplyLoginCtaSceneBlend(VisualElement element)
        {
            element.style.backgroundColor = DeepGlass;
            ApplyPanelFrame(element);
        }

        internal static void ApplyServerSelectorFrame(VisualElement element)
        {
            element.style.backgroundColor = DenseGlass;
            ApplyPanelFrame(element);
        }

        internal static void ApplyLoginEnterButtonFrame(Button button)
        {
            ApplyBaseButtonFrame(button);
            ApplyCompactActionFrame(button, BlueGlass, RuntimeArtCatalog.Gold,
                RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold);
        }

        internal static void ApplyCompactActionFrame(Button button, Color background, Color left, Color top, Color right, Color bottom)
        {
            button.style.backgroundImage = new StyleBackground();
            button.style.backgroundColor = background;
            button.style.color = RuntimeArtCatalog.Text;
            ApplyEdgeFrame(button, left, top, right, bottom, 1f, 1f);
        }

        internal static void ApplyBaseButtonFrame(Button button)
        {
            ApplyRadius(button, 8);
            ApplyEdgeFrame(button, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, 1f, 1f);
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        internal static void ApplyRuntimeIconFrame(VisualElement icon, Color background)
        {
            icon.style.backgroundColor = background;
            ApplyRadius(icon, 8);
            ApplyEdgeFrame(icon, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, 1f, 1f);
        }

        internal static void ApplySettingToggleFrame(Toggle toggle, Color accent)
        {
            ApplyPadding(toggle, RuntimeUiSpacing.SettingTogglePaddingHorizontal, RuntimeUiSpacing.SettingTogglePaddingVertical);
            ApplyRadius(toggle, 8);
            toggle.style.minHeight = RuntimeUiSpacing.SettingToggleMinHeight;
            toggle.style.flexDirection = FlexDirection.Row;
            toggle.style.alignItems = Align.Center;
            toggle.style.justifyContent = Justify.SpaceBetween;
            toggle.style.backgroundColor = DenseGlass;
            ApplyEdgeFrame(toggle, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, 1f, 1f);
            toggle.style.color = RuntimeArtCatalog.Text;
            toggle.style.fontSize = RuntimeUiSpacing.SettingToggleFontSize;
            toggle.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        internal static void ApplySettingToggleState(Toggle toggle, bool enabled)
        {
            var border = enabled ? MediumGoldBorder : LightGoldBorder;
            ApplyEdgeFrame(toggle, border, border, border, border, 1f, 1f);
            toggle.style.backgroundColor = enabled ? BlueGlass : DeepGlass;
            var pill = toggle.Q<Label>(SettingToggleStatePillName);
            if (pill != null) ApplySettingToggleStatePill(pill, enabled);
        }

        internal static void ApplySettingToggleStatePill(Label pill, bool enabled)
        {
            pill.name = SettingToggleStatePillName;
            pill.text = enabled ? "Bật" : "Tắt";
            pill.style.minWidth = RuntimeUiSpacing.SettingTogglePillMinWidth;
            pill.style.marginLeft = RuntimeUiSpacing.SettingTogglePillMarginLeft;
            ApplyPadding(
                pill,
                RuntimeUiSpacing.SettingTogglePillPaddingHorizontal,
                RuntimeUiSpacing.SettingTogglePillPaddingHorizontal,
                RuntimeUiSpacing.SettingTogglePillPaddingTop,
                RuntimeUiSpacing.SettingTogglePillPaddingBottom);
            pill.style.unityTextAlign = TextAnchor.MiddleCenter;
            pill.style.unityFontStyleAndWeight = FontStyle.Bold;
            pill.style.fontSize = RuntimeUiSpacing.SettingTogglePillFontSize;
            pill.style.backgroundColor = enabled ? BlueGlass : DeepGlass;
            pill.style.color = enabled ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Muted;
            ApplyRadius(pill, RuntimeUiSpacing.SettingTogglePillRadius);
            ApplyEdgeFrame(pill, enabled ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Muted, LightGoldBorder, Color.clear, Color.clear, 1f, 1f);
        }

        internal static void ApplyBadgeFrame(VisualElement badge)
        {
            badge.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            ApplyRadius(badge, 8);
            ApplyEdgeFrame(badge, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, Color.clear, Color.clear, 1f, 1f);
            badge.style.borderRightWidth = 0;
            badge.style.borderBottomWidth = 0;
        }

        internal static void ApplyToastFrame(Label label, Color accent)
        {
            label.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            ApplyRadius(label, 8);
            ApplyEdgeFrame(label, accent, Color.clear, Color.clear, Color.clear, 2f, 0f);
        }

        internal static void ApplyStatusChipFrame(Label label, Color accent)
        {
            label.style.backgroundColor = BlueGlass;
            ApplyRadius(label, 8);
            ApplyEdgeFrame(label, accent, accent, accent, accent, 1f, 1f);
        }

        internal static void ApplyCharacterHallPanelFrame(VisualElement panel, bool lightProfile = false)
        {
            // LGO Character Hall No Stretched Texture v1: keep ornate assets role-sized; the large shell uses code-side glass.
            panel.style.backgroundImage = StyleKeyword.None;
            // LGO Character Hall Mobile Light Shell v1: compact screens keep the reference art visible behind the bounded shell.
            panel.style.backgroundColor = DenseGlass;
            ApplyOrnamentedShellFrame(panel);
        }

        internal static void ApplySubtleNestedFrame(VisualElement element, Color accent, float alpha = 0.32f)
        {
            element.style.backgroundColor = new Color(0.0f, 0.020f, 0.050f, 0.62f);
            ApplyRadius(element, 10);
            ApplyEdgeFrame(
                element,
                new Color(accent.r, accent.g, accent.b, alpha),
                new Color(0.93f, 0.73f, 0.36f, alpha * 0.70f),
                new Color(0.14f, 0.78f, 0.90f, alpha * 0.45f),
                new Color(0.93f, 0.73f, 0.36f, alpha * 0.36f),
                1f,
                1f);
        }

        internal static void ApplyCharacterListFrame(VisualElement list)
        {
            ApplyFloatingActionBarFrame(list);
        }

        internal static void ApplyCharacterPreviewFrame(VisualElement preview, bool lightProfile = false)
        {
            ApplyFloatingActionBarFrame(preview);
        }

        internal static void ApplyCharacterCreateFrame(VisualElement panel)
        {
            ApplySubtleNestedFrame(panel, RuntimeArtCatalog.Spirit, 0.30f);
            panel.style.backgroundColor = new Color(0.0f, 0.018f, 0.042f, 0.60f);
            panel.style.borderTopColor = new Color(0.93f, 0.73f, 0.36f, 0.38f);
            panel.style.borderBottomColor = new Color(0.14f, 0.78f, 0.90f, 0.26f);
        }

        internal static void ApplyFloatingActionBarFrame(VisualElement panel)
        {
            panel.style.backgroundImage = StyleKeyword.None;
            panel.style.backgroundColor = Color.clear;
            ApplyRadius(panel, 8);
            panel.style.borderLeftWidth = 0;
            panel.style.borderRightWidth = 0;
            panel.style.borderTopWidth = 0;
            panel.style.borderBottomWidth = 0;
        }

        internal static void ApplyCharacterPortraitFrame(VisualElement portrait)
        {
            portrait.style.backgroundColor = new Color(0.0f, 0.015f, 0.035f, 0.48f);
            ApplyRadius(portrait, 10);
            ApplyEdgeFrame(portrait, Color.clear, RuntimeArtCatalog.Gold, Color.clear, RuntimeArtCatalog.Spirit, 0f, 1f);
            portrait.style.borderRightWidth = 0;
        }

        internal static void ApplyCharacterHallListHeading(Label label)
        {
            ApplyText(label, RuntimeArtCatalog.Spirit, RuntimeUiTypography.BadgeValueFontSize, true);
            ApplyPadding(label, 12, 8);
            label.style.backgroundColor = new Color(0.02f, 0.07f, 0.14f, 0.72f);
            label.style.marginBottom = 8;
            ApplyRadius(label, 8);
            ApplyEdgeFrame(label, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, Color.clear, RuntimeArtCatalog.SurfaceRaised, 2f, 1f);
            label.style.borderRightWidth = 0;
        }

        internal static void ApplyLobbyInputFrame(TextField field)
        {
            field.style.backgroundColor = DeepGlass;
            ApplyRadius(field, 4);
            ApplyEdgeFrame(field, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, MediumGoldBorder, 1f, 1f);
            field.labelElement.style.color = RuntimeArtCatalog.Gold;
            field.labelElement.style.fontSize = RuntimeUiTypography.BadgeValueFontSize;
            field.labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;
            ApplyLobbyInputInnerFrame(field);
            field.RegisterCallback<AttachToPanelEvent>(_ => ApplyLobbyInputInnerFrame(field));
        }

        internal static void ApplyLobbyInputInnerFrame(TextField field)
        {
            if (field == null) return;
            // LGO Character Hall Lobby Text Field Inner Skin v1: style TextField's actual input child, not only the root frame.
            var input = field.Q(className: "unity-base-text-field__input")
                ?? field.Q(className: "unity-text-field__input")
                ?? field.Q("unity-text-input");
            if (input == null) return;
            input.style.backgroundColor = BlueGlass;
            input.style.color = RuntimeArtCatalog.Text;
            input.style.unityFontStyleAndWeight = FontStyle.Normal;
            input.style.borderTopWidth = 0;
            input.style.borderRightWidth = 0;
            input.style.borderBottomWidth = 0;
            input.style.borderLeftWidth = 0;
        }

        internal static void ApplyEmptyCharacterCardFrame(VisualElement card)
        {
            ApplySubtleNestedFrame(card, RuntimeArtCatalog.Spirit, 0.30f);
            card.style.borderRightWidth = 0;
            card.style.borderBottomWidth = 0;
        }

        internal static void ApplyPreviewPanelFrame(VisualElement preview)
        {
            preview.style.backgroundColor = DenseGlass;
            ApplyPanelFrame(preview);
        }

        internal static void ApplyWorldHudGroupFrame(VisualElement group, Color accent)
        {
            group.style.backgroundColor = DeepGlass;
            ApplyPanelFrame(group);
        }

        internal static void ApplyWorldHudRootFrame(VisualElement hud)
        {
            hud.style.backgroundColor = DeepGlass;
            hud.style.backgroundImage = StyleKeyword.None;
            ApplyPanelFrame(hud);
        }

        internal static void ApplyHudStatusCompactFrame(Label label)
        {
            label.style.backgroundColor = new Color(0.02f, 0.055f, 0.10f, 0.58f);
        }

        internal static void ApplySessionMenuFrame(VisualElement panel)
        {
            panel.style.backgroundColor = new Color(0.01f, 0.04f, 0.09f, 0.96f);
            ApplyOrnamentedShellFrame(panel);
        }

        internal static void ApplyLocalSettingsPanelFrame(VisualElement panel)
        {
            panel.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
        }

        internal static void ApplyCombatCooldownIconFrame(VisualElement icon)
        {
            icon.style.backgroundColor = RuntimeArtCatalog.Surface;
            icon.style.borderTopWidth = 2;
            icon.style.borderLeftWidth = 2;
            ApplyCombatCooldownIconState(icon, false);
        }

        internal static void ApplyCombatCooldownIconState(VisualElement icon, bool coolingDown)
        {
            icon.style.borderTopColor = coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit;
            icon.style.borderLeftColor = coolingDown ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Spirit;
        }

        internal static Color SessionMenuBackground(bool compactProfile)
        {
            return compactProfile
                ? new Color(DenseGlass.r, DenseGlass.g, DenseGlass.b, 1.0f)
                : new Color(DenseGlass.r, DenseGlass.g, DenseGlass.b, 0.96f);
        }

        internal static Color WorldHudBackground(bool mobile, bool tablet, bool dialogueVisible)
        {
            var alpha = mobile ? (dialogueVisible ? 0.80f : 0.60f) : tablet ? 0.72f : 0.68f;
            return new Color(DeepGlass.r, DeepGlass.g, DeepGlass.b, alpha);
        }
    }
}
