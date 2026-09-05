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
    }
}
