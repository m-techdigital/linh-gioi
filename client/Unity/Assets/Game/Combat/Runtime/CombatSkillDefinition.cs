using System;

namespace LinhGioi.Combat
{
    [Serializable]
    public sealed class CombatSkillDefinition
    {
        public string id;
        public string classId;
        public int cooldownMs;
        public float damageCoefficient;
        public float rangeM;
        public string[] tags = new string[0];

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException("skill id must not be blank.");
            if (string.IsNullOrWhiteSpace(classId)) throw new InvalidOperationException($"skill {id} classId must not be blank.");
            if (cooldownMs < 0) throw new InvalidOperationException($"skill {id} cooldownMs must not be negative.");
            if (damageCoefficient < 0f) throw new InvalidOperationException($"skill {id} damageCoefficient must not be negative.");
            if (rangeM < 0f) throw new InvalidOperationException($"skill {id} rangeM must not be negative.");
        }
    }
}
