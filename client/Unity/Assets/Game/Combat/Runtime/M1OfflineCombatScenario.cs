using System;
using UnityEngine;

namespace LinhGioi.Combat
{
    public static class M1OfflineCombatScenario
    {
        public const ulong PlayerEntityId = 1001UL;
        public const ulong EnemyEntityId = 2001UL;

        public static CombatantState CreateSwordAdept(Vector3 position)
        {
            return new CombatantState
            {
                entityId = PlayerEntityId,
                contentId = "class.sword",
                displayName = "Kiếm tu tập sự",
                level = 1,
                maxHp = 160,
                currentHp = 160,
                attackPower = 42,
                defense = 4,
                position = position
            };
        }

        public static OfflineCombatSimulator CreateSimulator(GameDataCombatCatalog catalog)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            var simulator = new OfflineCombatSimulator();
            simulator.AddSkill(catalog.GetSkill(GameDataCombatCatalog.DefaultSkillId));
            simulator.AddCombatant(CreateSwordAdept(Vector3.zero));
            simulator.AddCombatant(catalog.CreateMonster(GameDataCombatCatalog.DefaultMonsterId, EnemyEntityId, new Vector3(1.5f, 0f, 0f)));
            return simulator;
        }

        public static M1OfflineCombatRunResult RunDeterministicDuel(GameDataCombatCatalog catalog)
        {
            var simulator = CreateSimulator(catalog);
            var sequence = 1;
            var nowMs = 1000L;
            CombatActionResult last = null;

            while (sequence <= 12)
            {
                var request = sequence % 2 == 1
                    ? CombatActionRequest.Skill(PlayerEntityId, EnemyEntityId, GameDataCombatCatalog.DefaultSkillId, sequence, nowMs)
                    : CombatActionRequest.BasicAttack(PlayerEntityId, EnemyEntityId, sequence, nowMs);
                last = simulator.Execute(request);
                if (last.status == CombatActionStatus.Victory) break;
                nowMs += sequence % 2 == 1 ? 6000L : 800L;
                sequence++;
            }

            var player = simulator.GetCombatant(PlayerEntityId);
            var enemy = simulator.GetCombatant(EnemyEntityId);
            return new M1OfflineCombatRunResult
            {
                status = enemy.IsDefeated ? "PASS" : "INCOMPLETE",
                playerEntityId = PlayerEntityId,
                enemyEntityId = EnemyEntityId,
                enemyContentId = GameDataCombatCatalog.DefaultMonsterId,
                skillId = GameDataCombatCatalog.DefaultSkillId,
                actionsExecuted = simulator.History.Count,
                initialPlayerHp = player.maxHp,
                finalPlayerHp = player.currentHp,
                initialEnemyHp = enemy.maxHp,
                finalEnemyHp = enemy.currentHp,
                targetDefeated = enemy.IsDefeated,
                lastActionStatus = last == null ? string.Empty : last.status.ToString(),
                lastActionDamage = last == null ? 0 : last.damage
            };
        }
    }

    [Serializable]
    public sealed class M1OfflineCombatRunResult
    {
        public string status;
        public ulong playerEntityId;
        public ulong enemyEntityId;
        public string enemyContentId;
        public string skillId;
        public int actionsExecuted;
        public int initialPlayerHp;
        public int finalPlayerHp;
        public int initialEnemyHp;
        public int finalEnemyHp;
        public bool targetDefeated;
        public string lastActionStatus;
        public int lastActionDamage;
    }
}
