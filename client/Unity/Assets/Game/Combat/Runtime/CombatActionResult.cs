using System;

namespace LinhGioi.Combat
{
    [Serializable]
    public sealed class CombatActionResult
    {
        public CombatActionStatus status;
        public int sequence;
        public ulong sourceEntityId;
        public ulong targetEntityId;
        public string actionId;
        public int damage;
        public int targetHpBefore;
        public int targetHpAfter;
        public bool targetDefeated;
        public long appliedAtMs;
        public long nextReadyAtMs;
        public string reason;

        public bool Applied => status == CombatActionStatus.Applied || status == CombatActionStatus.Victory;
    }
}
