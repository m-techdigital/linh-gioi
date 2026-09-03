namespace LinhGioi.Combat
{
    public enum CombatActionStatus
    {
        Applied,
        RejectedInvalidRequest,
        RejectedSourceDefeated,
        RejectedTargetDefeated,
        RejectedOutOfRange,
        RejectedCooldown,
        RejectedUnknownSkill,
        Victory
    }
}
