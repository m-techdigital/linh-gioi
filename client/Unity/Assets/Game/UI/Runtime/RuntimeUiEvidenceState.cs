namespace LinhGioi.UI
{
    internal readonly struct RuntimeUiEvidenceState
    {
        internal static readonly RuntimeUiEvidenceState None = new RuntimeUiEvidenceState(false, false);
        internal static readonly RuntimeUiEvidenceState CombatPanelFocus = new RuntimeUiEvidenceState(true, true);

        internal readonly bool ForceCombatPanel;
        internal readonly bool HideGuidanceCardOnCompact;

        private RuntimeUiEvidenceState(bool forceCombatPanel, bool hideGuidanceCardOnCompact)
        {
            ForceCombatPanel = forceCombatPanel;
            HideGuidanceCardOnCompact = hideGuidanceCardOnCompact;
        }
    }
}
