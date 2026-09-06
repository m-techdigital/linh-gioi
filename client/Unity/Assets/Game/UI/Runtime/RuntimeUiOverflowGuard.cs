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
        internal const string ModalShellMarker = "LGO Runtime UI Modal Shell Base v1";

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

        internal static void ApplyModalBody(VisualElement body)
        {
            if (body == null) return;
            body.style.width = Length.Percent(100);
            body.style.maxWidth = Length.Percent(100);
            body.style.minHeight = 0;
            body.style.flexGrow = 1;
            body.style.flexShrink = 1;
            body.style.marginTop = 0;
            body.style.marginBottom = 0;
            body.style.overflow = Overflow.Hidden;
        }

        internal static void ApplyModalFooter(VisualElement footer, float marginTop)
        {
            if (footer == null) return;
            footer.style.width = Length.Percent(100);
            footer.style.maxWidth = Length.Percent(100);
            footer.style.minHeight = 0;
            footer.style.flexGrow = 0;
            footer.style.flexShrink = 0;
            footer.style.marginTop = marginTop;
            footer.style.marginBottom = 0;
            footer.style.overflow = Overflow.Hidden;
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
            scroller.style.minWidth = 0;
            scroller.style.maxWidth = 8;
            scroller.style.backgroundImage = StyleKeyword.None;
            scroller.style.backgroundColor = RuntimeUiSkin.DeepGlass;
            if (scroller.lowButton != null) scroller.lowButton.style.display = DisplayStyle.None;
            if (scroller.highButton != null) scroller.highButton.style.display = DisplayStyle.None;
            if (scroller.slider != null)
            {
                var slider = scroller.slider;
                ApplyCompactScrollerPart(slider, Color.clear);
                ApplyCompactScrollerPart(slider.Q<VisualElement>(className: Slider.dragContainerUssClassName), Color.clear);
                ApplyCompactScrollerPart(slider.Q<VisualElement>(className: Slider.trackerUssClassName), RuntimeUiSkin.LightGoldBorder);
                ApplyCompactScrollerPart(slider.Q<VisualElement>(className: Slider.draggerUssClassName), LinhGioi.Art.RuntimeArtCatalog.Gold);
                ApplyCompactScrollerPart(slider.Q<VisualElement>("unity-dragger-border"), Color.clear);
            }
        }

        private static void ApplyCompactScrollerPart(VisualElement part, Color background)
        {
            if (part == null) return;
            // Only constrain the cross-axis; Unity owns vertical thumb size and scroll position.
            part.style.width = Length.Percent(100);
            part.style.minWidth = 0;
            part.style.maxWidth = Length.Percent(100);
            part.style.left = 0;
            part.style.right = 0;
            part.style.marginLeft = 0;
            part.style.marginRight = 0;
            part.style.backgroundImage = StyleKeyword.None;
            part.style.backgroundColor = background;
            RuntimeUiSkin.ApplyEdgeFrame(part, Color.clear, Color.clear, Color.clear, Color.clear, 0, 0);
            RuntimeUiSkin.ApplyRadius(part, 3);
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
                // Column width already consumes the available space; Unity's default left margin would overflow it.
                button.style.marginLeft = 0;
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
            surface.style.translate = new Translate(
                placement == RuntimeUiOverlayPlacement.Center ? Length.Percent(-50) : new Length(0),
                verticalPlacement == RuntimeUiOverlayVerticalPlacement.Center ? Length.Percent(-50) : new Length(0));

            if (placement == RuntimeUiOverlayPlacement.Center)
            {
                surface.style.left = Length.Percent(50);
                surface.style.right = StyleKeyword.Auto;
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

            if (verticalPlacement == RuntimeUiOverlayVerticalPlacement.Center)
            {
                surface.style.top = Length.Percent(50);
                surface.style.bottom = StyleKeyword.Auto;
            }
            else if (verticalPlacement == RuntimeUiOverlayVerticalPlacement.Bottom)
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

        internal static void ApplyViewportBottomSafeOverlaySurface(
            VisualElement surface,
            RuntimeUiOverlayPlacement placement,
            float width,
            float maxHeight,
            float horizontalInset,
            float bottomInset)
        {
            // LGO Runtime UI Bottom Safe Overlay Base v1: bottom-docked surfaces share one safe inset path instead of per-screen bottom constants.
            ApplyViewportOverlaySurface(
                surface,
                placement,
                RuntimeUiOverlayVerticalPlacement.Bottom,
                width,
                maxHeight,
                horizontalInset,
                bottomInset);
        }
    }
}
