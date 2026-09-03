using System;

namespace LinhGioi.Combat
{
    [Serializable]
    public sealed class CombatActionRequest
    {
        public CombatActionKind kind;
        public ulong sourceEntityId;
        public ulong targetEntityId;
        public string actionId;
        public int sequence;
        public long nowMs;

        public static CombatActionRequest BasicAttack(ulong sourceEntityId, ulong targetEntityId, int sequence, long nowMs)
        {
            return new CombatActionRequest
            {
                kind = CombatActionKind.BasicAttack,
                sourceEntityId = sourceEntityId,
                targetEntityId = targetEntityId,
                actionId = OfflineCombatSimulator.BasicAttackActionId,
                sequence = sequence,
                nowMs = nowMs
            };
        }

        public static CombatActionRequest Skill(ulong sourceEntityId, ulong targetEntityId, string skillId, int sequence, long nowMs)
        {
            return new CombatActionRequest
            {
                kind = CombatActionKind.Skill,
                sourceEntityId = sourceEntityId,
                targetEntityId = targetEntityId,
                actionId = skillId,
                sequence = sequence,
                nowMs = nowMs
            };
        }
    }
}
