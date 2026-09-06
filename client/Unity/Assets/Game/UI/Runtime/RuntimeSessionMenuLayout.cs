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
            panel.style.right = layout.SessionMenuRight;
            panel.style.top = layout.SessionMenuTop;
            panel.style.marginTop = 0;
            panel.style.width = layout.SessionMenuWidth;
            panel.style.maxWidth = layout.IsMobile || layout.IsTablet ? StyleKeyword.None : layout.SessionMenuWidth;
            panel.style.maxHeight = layout.SessionMenuMaxHeight;
            RuntimeUiSkin.ApplyPadding(panel, layout.SessionMenuPaddingHorizontal, layout.SessionMenuPaddingHorizontal, layout.SessionMenuPaddingTop, layout.SessionMenuPaddingBottom);
            panel.style.backgroundColor = RuntimeUiSkin.SessionMenuBackground(layout.IsMobile || layout.IsTablet);
        }

        internal static void ApplyActions(
            VisualElement actions,
            RuntimeUiLayoutProfile layout,
            params Button[] buttons)
        {
            if (actions == null) return;
            actions.style.justifyContent = layout.IsMobile ? Justify.SpaceBetween : Justify.Center;
            foreach (var button in buttons)
            {
                if (button == null) continue;
                button.style.marginRight = layout.IsMobile ? 0 : RuntimeUiSpacing.BaseButtonMarginRight;
                button.style.width = layout.IsMobile ? Length.Percent(48) : StyleKeyword.Auto;
                RuntimeUiSkin.ApplyButtonMetrics(button, 0, layout.IsMobile ? RuntimeUiSpacing.CharacterSelectedPrimaryMobileMinHeight : RuntimeUiSpacing.CompactButtonMinHeight);
            }
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
