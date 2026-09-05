using UnityEngine;
using UnityEngine.UIElements;
using LinhGioi.Art;

namespace LinhGioi.UI
{
    internal static class RuntimeUiSkin
    {
        internal const string FoundationMarker = "LGO Runtime UI Skin Foundation v1";

        internal static readonly Color DeepGlass = new Color(0.0f, 0.014f, 0.034f, 0.70f);
        internal static readonly Color DenseGlass = new Color(0.0f, 0.016f, 0.040f, 0.82f);
        internal static readonly Color BlueGlass = new Color(0.02f, 0.07f, 0.14f, 0.86f);
        internal static readonly Color SoftLoginGlass = new Color(0.005f, 0.018f, 0.040f, 0.18f);
        internal static readonly Color LightGoldBorder = new Color(0.93f, 0.73f, 0.36f, 0.20f);
        internal static readonly Color MediumGoldBorder = new Color(0.93f, 0.73f, 0.36f, 0.48f);
        internal static readonly Color LightSpiritBorder = new Color(0.14f, 0.78f, 0.90f, 0.24f);
        internal static readonly Color MediumSpiritBorder = new Color(0.14f, 0.78f, 0.90f, 0.46f);

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
            ApplyEdgeFrame(element, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.SurfaceRaised, RuntimeArtCatalog.SurfaceRaised);
        }

        internal static void ApplyInsetRowFrame(VisualElement element, Color accent)
        {
            element.style.backgroundColor = RuntimeArtCatalog.Background;
            ApplyEdgeFrame(element, accent, RuntimeArtCatalog.SurfaceRaised, Color.clear, RuntimeArtCatalog.SurfaceRaised, 2f, 1f);
            element.style.borderRightWidth = 0;
        }

        internal static void ApplyLoginCtaBacking(VisualElement element)
        {
            element.style.backgroundColor = SoftLoginGlass;
            ApplyRadius(element, 18);
            ApplyEdgeFrame(
                element,
                new Color(0.14f, 0.78f, 0.90f, 0.10f),
                LightGoldBorder,
                new Color(0.93f, 0.73f, 0.36f, 0.12f),
                new Color(0.14f, 0.78f, 0.90f, 0.10f),
                1f,
                1f);
        }

        internal static void ApplyServerSelectorFrame(VisualElement element)
        {
            element.style.backgroundColor = DenseGlass;
            ApplyRadius(element, 8);
            element.style.borderTopColor = MediumGoldBorder;
            element.style.borderTopWidth = 1;
            element.style.borderBottomColor = LightSpiritBorder;
            element.style.borderBottomWidth = 1;
        }

        internal static void ApplyCompactActionFrame(Button button, Color background, Color left, Color top, Color right, Color bottom)
        {
            button.style.backgroundImage = new StyleBackground();
            button.style.backgroundColor = background;
            button.style.color = RuntimeArtCatalog.Text;
            ApplyEdgeFrame(button, left, top, right, bottom);
        }

        internal static void ApplyBaseButtonFrame(Button button)
        {
            ApplyRadius(button, 8);
            ApplyEdgeFrame(button, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.SurfaceRaised, RuntimeArtCatalog.SurfaceRaised, 1f, 1f);
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        internal static void ApplyRuntimeIconFrame(VisualElement icon, Color background)
        {
            icon.style.backgroundColor = background;
            ApplyRadius(icon, 8);
            ApplyEdgeFrame(icon, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, 1f, 1f);
        }

        internal static void ApplySettingToggleFrame(Toggle toggle, Color accent)
        {
            ApplyPadding(toggle, 10, 5);
            toggle.style.backgroundColor = RuntimeArtCatalog.Background;
            ApplyEdgeFrame(toggle, accent, RuntimeArtCatalog.SurfaceRaised, Color.clear, RuntimeArtCatalog.SurfaceRaised, 2f, 1f);
            toggle.style.borderRightWidth = 0;
            toggle.style.color = RuntimeArtCatalog.Text;
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

        internal static void ApplyCharacterHallPanelFrame(VisualElement panel)
        {
            panel.style.backgroundColor = new Color(0.005f, 0.025f, 0.055f, 0.82f);
            ApplyEdgeFrame(panel, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold);
        }

        internal static void ApplyCharacterListFrame(VisualElement list)
        {
            list.style.backgroundColor = new Color(0.0f, 0.018f, 0.045f, 0.70f);
            ApplyEdgeFrame(list, new Color(0.93f, 0.73f, 0.36f, 0.70f), new Color(0.14f, 0.78f, 0.90f, 0.42f), Color.clear, Color.clear, 2f, 1f);
            list.style.borderRightWidth = 0;
            list.style.borderBottomWidth = 0;
        }

        internal static void ApplyCharacterPreviewFrame(VisualElement preview)
        {
            preview.style.backgroundColor = new Color(0.0f, 0.018f, 0.045f, 0.76f);
            ApplyEdgeFrame(preview, RuntimeArtCatalog.Spirit, new Color(0.93f, 0.73f, 0.36f, 0.68f), new Color(0.14f, 0.78f, 0.90f, 0.38f), RuntimeArtCatalog.SurfaceRaised, 2f, 1f);
        }

        internal static void ApplyCharacterCreateFrame(VisualElement panel)
        {
            panel.style.backgroundColor = new Color(0.0f, 0.018f, 0.045f, 0.66f);
            ApplyEdgeFrame(panel, new Color(0.14f, 0.78f, 0.90f, 0.42f), new Color(0.93f, 0.73f, 0.36f, 0.54f), Color.clear, Color.clear, 2f, 1f);
            panel.style.borderRightWidth = 0;
            panel.style.borderBottomWidth = 0;
        }

        internal static void ApplyCharacterPortraitFrame(VisualElement portrait)
        {
            portrait.style.backgroundColor = new Color(0.0f, 0.015f, 0.035f, 0.48f);
            ApplyEdgeFrame(portrait, Color.clear, RuntimeArtCatalog.Gold, Color.clear, RuntimeArtCatalog.Spirit, 0f, 1f);
            portrait.style.borderRightWidth = 0;
        }

        internal static void ApplyLobbyInputFrame(TextField field)
        {
            field.style.backgroundColor = DeepGlass;
            ApplyEdgeFrame(field, MediumSpiritBorder, MediumGoldBorder, LightSpiritBorder, new Color(0.93f, 0.73f, 0.36f, 0.28f));
        }

        internal static void ApplyPreviewPanelFrame(VisualElement preview)
        {
            preview.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            ApplyRadius(preview, 8);
            ApplyEdgeFrame(preview, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, Color.clear, Color.clear, 2f, 1f);
            preview.style.borderRightWidth = 0;
            preview.style.borderBottomWidth = 0;
        }

        internal static void ApplyWorldHudGroupFrame(VisualElement group, Color accent)
        {
            group.style.backgroundColor = DeepGlass;
            ApplyEdgeFrame(group, accent, RuntimeArtCatalog.Gold, RuntimeArtCatalog.SurfaceRaised, RuntimeArtCatalog.SurfaceRaised);
        }

        internal static void ApplyHudStatusCompactFrame(Label label)
        {
            label.style.backgroundColor = new Color(0.02f, 0.055f, 0.10f, 0.58f);
        }

        internal static void ApplySessionMenuFrame(VisualElement panel)
        {
            panel.style.backgroundColor = new Color(0.01f, 0.04f, 0.09f, 0.96f);
            ApplyEdgeFrame(panel, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold);
        }

        internal static Color SessionMenuBackground(bool compactProfile)
        {
            return compactProfile
                ? new Color(0.004f, 0.018f, 0.045f, 1.0f)
                : new Color(0.01f, 0.04f, 0.09f, 0.96f);
        }

        internal static Color WorldHudBackground(bool mobile, bool tablet, bool dialogueVisible)
        {
            if (mobile) return new Color(0.002f, 0.014f, 0.036f, dialogueVisible ? 0.82f : 0.66f);
            if (tablet) return new Color(0.004f, 0.018f, 0.044f, 0.78f);
            return RuntimeArtCatalog.Surface;
        }
    }
}
