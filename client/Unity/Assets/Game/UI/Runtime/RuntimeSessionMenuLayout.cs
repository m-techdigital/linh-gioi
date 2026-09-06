using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal static class RuntimeSessionMenuLayout
    {
        internal const string Marker = "LGO Runtime Session Menu Layout Helper v1";

        internal static void ApplyPanel(VisualElement panel, RuntimeUiLayoutProfile layout)
        {
            if (panel == null) return;
            panel.style.position = Position.Absolute;
            panel.style.left = layout.SessionMenuLeft;
            panel.style.right = layout.IsMobile ? StyleKeyword.Auto : layout.SessionMenuRight;
            panel.style.top = layout.SessionMenuTop;
            panel.style.marginTop = 0;
            panel.style.width = layout.SessionMenuWidth;
            panel.style.maxWidth = layout.IsMobile ? layout.SessionMenuWidth : StyleKeyword.None;
            panel.style.maxHeight = layout.SessionMenuMaxHeight;
            panel.style.height = layout.SessionMenuShowsSettings ? layout.SessionMenuMaxHeight : StyleKeyword.Auto;
            panel.style.overflow = Overflow.Hidden;
            RuntimeUiSkin.ApplyPadding(panel, layout.SessionMenuPaddingHorizontal, layout.SessionMenuPaddingHorizontal, layout.SessionMenuPaddingTop, layout.SessionMenuPaddingBottom);
            panel.style.backgroundColor = RuntimeUiSkin.SessionMenuBackground(layout.IsMobile || layout.IsTablet);
        }

        internal static ScrollView NewContentScroll(RuntimeUiLayoutProfile layout)
        {
            var scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.name = "LGO Session Menu Viewport Content";
            scroll.verticalScrollerVisibility = ScrollerVisibility.Auto;
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            ApplyContentScroll(scroll, layout);
            return scroll;
        }

        internal static void ApplyContentScroll(ScrollView scroll, RuntimeUiLayoutProfile layout)
        {
            if (scroll == null) return;
            scroll.style.flexGrow = 1;
            scroll.style.minHeight = 0;
            scroll.style.marginTop = layout.IsMobile ? 6 : 10;
            scroll.style.marginBottom = 0;
            scroll.style.overflow = Overflow.Hidden;
            scroll.contentContainer.style.flexGrow = 1;
            scroll.contentContainer.style.minHeight = 0;
        }

        internal static void ApplyActions(
            VisualElement actions,
            RuntimeUiLayoutProfile layout,
            params Button[] buttons)
        {
            if (actions == null) return;
            actions.style.justifyContent = layout.IsMobile ? Justify.SpaceBetween : Justify.Center;
            if (layout.IsMobile)
            {
                actions.style.marginTop = 4;
                actions.style.marginBottom = 0;
            }
            foreach (var button in buttons)
            {
                if (button == null) continue;
                button.style.marginRight = layout.IsMobile ? 0 : RuntimeUiSpacing.BaseButtonMarginRight;
                button.style.width = layout.IsMobile ? Length.Percent(48) : StyleKeyword.Auto;
                RuntimeUiSkin.ApplyButtonMetrics(
                    button,
                    0,
                    layout.IsMobile ? RuntimeUiSpacing.SessionActionMobileMinHeight : RuntimeUiSpacing.CompactButtonMinHeight,
                    layout.IsMobile ? RuntimeUiSpacing.SessionActionMobileFontSize : RuntimeUiSpacing.CompactButtonFontSize);
            }
        }

        internal static void ApplyStatus(Label status, RuntimeUiLayoutProfile layout)
        {
            if (status == null) return;
            status.style.display = layout.IsMobile ? DisplayStyle.None : DisplayStyle.Flex;
        }

        internal static void ApplyDetails(VisualElement locationRow, VisualElement objectiveRow, RuntimeUiLayoutProfile layout)
        {
            if (locationRow != null) locationRow.style.display = layout.IsMobile ? DisplayStyle.None : DisplayStyle.Flex;
            if (objectiveRow != null) objectiveRow.style.display = layout.IsMobile ? DisplayStyle.None : DisplayStyle.Flex;
        }

        internal static void ApplyFocusScrim(VisualElement scrim, bool visible)
        {
            if (scrim == null) return;
            scrim.style.backgroundColor = visible
                ? new Color(0.01f, 0.025f, 0.055f, 0.52f)
                : new Color(0.01f, 0.03f, 0.07f, 0.08f);
        }
    }
}
