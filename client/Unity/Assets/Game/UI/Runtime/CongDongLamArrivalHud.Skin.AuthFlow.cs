using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private static void ApplyLgoAuthFlowHeader(VisualElement header)
        {
            header.AddToClassList(LgoAuthFlowHeaderClass);
        }

        private static void ApplyLgoAuthFlowSubtitleRow(VisualElement row)
        {
            row.AddToClassList(LgoAuthFlowSubtitleRowClass);
        }

        private static void ApplyLgoPasswordRecoveryPanel(VisualElement panel)
        {
            panel.AddToClassList(LgoPasswordRecoveryPanelClass);
            ApplyLgoAuthFlowPanel(panel, 0);
        }

        private static void ApplyLgoPasswordRecoveryRule(Label label)
        {
            label.AddToClassList(LgoPasswordRecoveryRuleClass);
        }

        private static void ApplyLgoPasswordRecoveryVerifyFooter(VisualElement footer)
        {
            footer.AddToClassList(LgoPasswordRecoveryVerifyFooterClass);
        }
    }
}
