using LinhGioi.Foundation;
using UnityEngine;

namespace LinhGioi.UI
{
    internal static class RuntimeUiEvidenceMetrics
    {
        internal static RuntimeUiMetricsSnapshot CreateGameplaySnapshot(
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
            int minimumTouchTargetPanelUnits,
            RuntimeGameplayHudLayout gameplayHud)
        {
            var snapshot = CreateSnapshot(screenWidth, screenHeight, panelWidth, panelHeight,
                safePanelRect, characterHubShellRect, layoutClass, inputClass, panelSettings,
                evidenceAuthority, minimumTouchTargetPanelUnits);
            snapshot.gameplayPlayerStatus = ToSnapshot(gameplayHud.PlayerStatus);
            snapshot.gameplayRightInfo = ToSnapshot(gameplayHud.RightInfo);
            snapshot.gameplayCombat = ToSnapshot(gameplayHud.Combat);
            snapshot.gameplayContext = ToSnapshot(gameplayHud.Context);
            snapshot.gameplaySecondaryNav = ToSnapshot(gameplayHud.SecondaryNav);
            snapshot.gameplayTouchPad = ToSnapshot(gameplayHud.TouchPad);
            snapshot.gameplayDialogue = ToSnapshot(gameplayHud.Dialogue);
            return snapshot;
        }

        private static RuntimeUiRectSnapshot ToSnapshot(Rect rect)
        {
            return new RuntimeUiRectSnapshot
            {
                x = rect.x,
                y = rect.y,
                width = rect.width,
                height = rect.height,
            };
        }

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
