using LinhGioi.Foundation;
using UnityEngine;

namespace LinhGioi.UI
{
    internal static class RuntimeUiEvidenceMetrics
    {
        internal static RuntimeUiMetricsSnapshot CreateSnapshot(
            int screenWidth,
            int screenHeight,
            int panelWidth,
            int panelHeight,
            Rect safePanelRect,
            Rect characterHubShellRect,
            string layoutClass,
            string inputClass,
            string panelSettings,
            string evidenceAuthority,
            int minimumTouchTargetPanelUnits)
        {
            var safeScreenHeight = Mathf.Max(1, screenHeight);
            var safePanelHeight = Mathf.Max(1, panelHeight);
            var panelToScreenY = safeScreenHeight / (float)safePanelHeight;
            return new RuntimeUiMetricsSnapshot
            {
                evidenceAuthority = evidenceAuthority ?? "unknown",
                screenWidth = Mathf.Max(1, screenWidth),
                screenHeight = safeScreenHeight,
                panelWidth = Mathf.Max(1, panelWidth),
                panelHeight = safePanelHeight,
                safePanelX = safePanelRect.x,
                safePanelY = safePanelRect.y,
                safePanelWidth = safePanelRect.width,
                safePanelHeight = safePanelRect.height,
                layoutClass = layoutClass ?? "unknown",
                inputClass = inputClass ?? "unknown",
                panelSettings = panelSettings ?? "unknown",
                characterHubShellX = characterHubShellRect.x,
                characterHubShellY = characterHubShellRect.y,
                characterHubShellWidth = characterHubShellRect.width,
                characterHubShellHeight = characterHubShellRect.height,
                characterHubShellScreenHeightRatio = characterHubShellRect.height * panelToScreenY / safeScreenHeight,
                minimumTouchTargetPanelUnits = minimumTouchTargetPanelUnits,
                minimumTouchTargetScreenPixels = minimumTouchTargetPanelUnits * panelToScreenY,
            };
        }
    }
}
