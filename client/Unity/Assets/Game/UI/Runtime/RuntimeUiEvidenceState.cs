namespace LinhGioi.UI
{
    internal readonly struct RuntimeUiEvidenceState
    {
        internal static readonly RuntimeUiEvidenceState None = new RuntimeUiEvidenceState(false, false, false);
        internal static readonly RuntimeUiEvidenceState EnterWorldTransition = new RuntimeUiEvidenceState(false, false, true);
        internal static readonly RuntimeUiEvidenceState CombatPanelFocus = new RuntimeUiEvidenceState(true, true, false);

        internal readonly bool ForceCombatPanel;
        internal readonly bool HideGuidanceCardOnCompact;
        internal readonly bool ShowEnterWorldTransition;

        private RuntimeUiEvidenceState(bool forceCombatPanel, bool hideGuidanceCardOnCompact, bool showEnterWorldTransition)
        {
            ForceCombatPanel = forceCombatPanel;
            HideGuidanceCardOnCompact = hideGuidanceCardOnCompact;
            ShowEnterWorldTransition = showEnterWorldTransition;
        }
    }
}
