using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private static void ApplyLgoCharacterHubMainTabs(VisualElement tabs)
        {
            tabs.AddToClassList(LgoCharacterHubMainTabsClass);
            tabs.style.alignSelf = Align.FlexStart;
        }

        private static void ApplyLgoCharacterHubMainTabsLayout(
            VisualElement tabs, float shellWidth, RuntimeCharacterHubLayoutVariant variant)
        {
            if (tabs == null) return;
            tabs.style.width = shellWidth * variant.TabsWidthShare;
            tabs.style.marginLeft = shellWidth * variant.TabsInsetShare;
        }

        private static void ApplyLgoCharacterHubInspectorColumn(VisualElement panel)
        {
            panel.AddToClassList(LgoCharacterHubInspectorColumnClass);
            panel.style.flexGrow = 0;
            panel.style.flexBasis = InventoryDesktopDetailColumnWidth;
            panel.style.marginLeft = InventoryDesktopColumnGap;
            ApplyLgoCharacterHubDetailCard(panel);
            ApplyLgoCharacterHubInspectorFrame(panel);
        }

        private static void ApplyLgoCharacterHubActionRow(VisualElement row, Button primary, Button secondary)
        {
            row.AddToClassList(LgoCharacterHubActionRowClass);
            row.style.marginTop = 6;
            foreach (var action in new[] { primary, secondary })
            {
                action.style.flexGrow = 1;
                action.style.flexBasis = 0;
                action.style.marginRight = 6;
                ApplyLgoCharacterHubLockedAction(action, action == primary);
                row.Add(action);
            }
        }
    }
}
