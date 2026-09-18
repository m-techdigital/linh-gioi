using System;

namespace LinhGioi.Foundation
{
    [Serializable]
    public sealed class RuntimeUiRectSnapshot
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

    [Serializable]
    public sealed class RuntimeUiMetricsSnapshot
    {
        public string evidenceAuthority;
        public int screenWidth;
        public int screenHeight;
        public int panelWidth;
        public int panelHeight;
        public float safePanelX;
        public float safePanelY;
        public float safePanelWidth;
        public float safePanelHeight;
        public string layoutClass;
        public string inputClass;
        public string panelSettings;
        public float characterHubShellX;
        public float characterHubShellY;
        public float characterHubShellWidth;
        public float characterHubShellHeight;
        public float characterHubShellScreenHeightRatio;
        public int minimumTouchTargetPanelUnits;
        public float minimumTouchTargetScreenPixels;
        public RuntimeUiRectSnapshot gameplayPlayerStatus;
        public RuntimeUiRectSnapshot gameplayRightInfo;
        public RuntimeUiRectSnapshot gameplayCombat;
        public RuntimeUiRectSnapshot gameplayContext;
        public RuntimeUiRectSnapshot gameplaySecondaryNav;
        public RuntimeUiRectSnapshot gameplayTouchPad;
        public RuntimeUiRectSnapshot gameplayDialogue;
    }
}
