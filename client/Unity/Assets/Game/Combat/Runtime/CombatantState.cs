using System;
using UnityEngine;

namespace LinhGioi.Combat
{
    [Serializable]
    public sealed class CombatantState
    {
        public ulong entityId;
        public string contentId;
        public string displayName;
        public int level;
        public int maxHp;
        public int currentHp;
        public int attackPower;
        public int defense;
        public Vector3 position;

        public bool IsDefeated => currentHp <= 0;

        public CombatantState Clone()
        {
            return new CombatantState
            {
                entityId = entityId,
                contentId = contentId,
                displayName = displayName,
                level = level,
                maxHp = maxHp,
                currentHp = currentHp,
                attackPower = attackPower,
                defense = defense,
                position = position
            };
        }

        public void Validate()
        {
            if (entityId == 0UL) throw new InvalidOperationException("entityId must be positive.");
            if (string.IsNullOrWhiteSpace(contentId)) throw new InvalidOperationException("contentId must not be blank.");
            if (string.IsNullOrWhiteSpace(displayName)) throw new InvalidOperationException("displayName must not be blank.");
            if (level < 1) throw new InvalidOperationException("level must be positive.");
            if (maxHp < 1) throw new InvalidOperationException("maxHp must be positive.");
            if (currentHp < 0 || currentHp > maxHp) throw new InvalidOperationException("currentHp must be between 0 and maxHp.");
            if (attackPower < 0) throw new InvalidOperationException("attackPower must not be negative.");
            if (defense < 0) throw new InvalidOperationException("defense must not be negative.");
        }
    }
}
