using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal enum RuntimeUiOverlayPlacement
    {
        Left,
        Center,
        Right
    }

    internal enum RuntimeUiOverlayVerticalPlacement
    {
        Top,
        Center,
        Bottom,
        Stretch
    }

    internal static class RuntimeUiOverflowGuard
    {
        internal const string Marker = "LGO Runtime UI Overflow Guard v1";
        internal const string OverlayPlacementMarker = "LGO Runtime UI Overlay Placement Base v1";

        internal static void ApplyBoundedActionRow(VisualElement row)
        {
            if (row == null) return;
            row.style.width = Length.Percent(100);
            row.style.maxWidth = Length.Percent(100);
            row.style.minWidth = 0;
            row.style.overflow = Overflow.Hidden;
        }

        internal static void ApplyButton(Button button)
        {
            if (button == null) return;
            button.style.flexShrink = 1;
            button.style.maxWidth = Length.Percent(100);
            button.style.overflow = Overflow.Hidden;
        }

        internal static void ApplyBoundedScroll(ScrollView scroll, float maxHeight, float minHeight = 0f)
        {
            if (scroll == null) return;
            scroll.verticalScrollerVisibility = ScrollerVisibility.Auto;
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.style.width = Length.Percent(100);
            scroll.style.maxWidth = Length.Percent(100);
            scroll.style.maxHeight = maxHeight;
            scroll.style.minHeight = minHeight > 0f ? minHeight : 0;
            scroll.style.flexShrink = 1;
            scroll.style.overflow = Overflow.Hidden;
            scroll.contentContainer.style.minWidth = 0;
            scroll.contentContainer.style.maxWidth = Length.Percent(100);
            ApplyCompactScrollerSkin(scroll.verticalScroller);
        }

        private static void ApplyCompactScrollerSkin(Scroller scroller)
        {
            if (scroller == null) return;
            scroller.style.width = 8;
            scroller.style.backgroundColor = new Color(0.01f, 0.03f, 0.07f, 0.22f);
            if (scroller.lowButton != null) scroller.lowButton.style.display = DisplayStyle.None;
            if (scroller.highButton != null) scroller.highButton.style.display = DisplayStyle.None;
            if (scroller.slider != null)
            {
                scroller.slider.style.minWidth = 6;
                scroller.slider.style.backgroundColor = new Color(0.14f, 0.78f, 0.90f, 0.20f);
            }
        }

        internal static void ApplyResponsiveColumns(VisualElement row, int columns, float gap, params Button[] buttons)
        {
            if (row == null) return;
            columns = Mathf.Max(1, columns);
            ApplyBoundedActionRow(row);
            row.style.flexDirection = columns == 1 ? FlexDirection.Column : FlexDirection.Row;
            row.style.flexWrap = Wrap.NoWrap;
            var widthPercent = Mathf.Max(0f, (100f - (gap * (columns - 1))) / columns);
            for (var i = 0; i < buttons.Length; i++)
            {
                var button = buttons[i];
                if (button == null) continue;
                ApplyButton(button);
                button.style.minWidth = 0;
                button.style.width = Length.Percent(widthPercent);
                button.style.flexBasis = 0;
                button.style.flexGrow = 1;
                button.style.marginRight = columns == 1 || i % columns == columns - 1 ? 0 : gap;
                button.style.marginTop = columns == 1 && i > 0 ? gap : 0;
            }
        }

        internal static void ApplyViewportOverlaySurface(
            VisualElement surface,
            RuntimeUiOverlayPlacement placement,
            RuntimeUiOverlayVerticalPlacement verticalPlacement,
            float width,
            float maxHeight,
            float horizontalInset,
            float verticalInset)
        {
            if (surface == null) return;
            surface.style.position = Position.Absolute;
            surface.style.width = width;
            surface.style.minWidth = 0;
            surface.style.maxWidth = width;
            surface.style.maxHeight = maxHeight;
            surface.style.overflow = Overflow.Hidden;
            surface.style.marginTop = 0;
            surface.style.marginBottom = 0;
            surface.style.alignSelf = Align.Center;
            surface.style.flexShrink = 0;

            if (placement == RuntimeUiOverlayPlacement.Center)
            {
                surface.style.left = horizontalInset;
                surface.style.right = horizontalInset;
            }
            else if (placement == RuntimeUiOverlayPlacement.Left)
            {
                surface.style.left = horizontalInset;
                surface.style.right = StyleKeyword.Auto;
            }
            else
            {
                surface.style.left = StyleKeyword.Auto;
                surface.style.right = horizontalInset;
            }

            if (verticalPlacement == RuntimeUiOverlayVerticalPlacement.Bottom)
            {
                surface.style.top = StyleKeyword.Auto;
                surface.style.bottom = verticalInset;
            }
            else
            {
                surface.style.top = verticalInset;
                surface.style.bottom = verticalPlacement == RuntimeUiOverlayVerticalPlacement.Stretch ? verticalInset : StyleKeyword.Auto;
            }
            surface.style.height = verticalPlacement == RuntimeUiOverlayVerticalPlacement.Stretch ? maxHeight : StyleKeyword.Auto;
        }
    }
}
