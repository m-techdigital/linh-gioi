using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal static class RuntimeUiOverflowGuard
    {
        internal const string Marker = "LGO Runtime UI Overflow Guard v1";

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
    }
}
