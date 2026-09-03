using LinhGioi.Combat;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests
{
    public sealed class M1OfflineCombatTests
    {
        private const string CompiledManifestJson = @"{
  ""gamedata_version"": 1,
  ""documents"": [
    {""kind"": ""monsters"", ""source"": ""gamedata/monsters/shadow_slime.yaml"", ""data"": {""schema_version"": 1, ""id"": ""monster.shadow.slime"", ""level"": 1, ""max_hp"": 120, ""move_speed"": 2.5, ""archetype"": ""melee""}},
    {""kind"": ""skills"", ""source"": ""gamedata/skills/wind_slash.yaml"", ""data"": {""schema_version"": 1, ""id"": ""skill.sword.wind_slash"", ""class_id"": ""class.sword"", ""cooldown_ms"": 6000, ""damage"": {""coefficient"": 1.35}, ""range_m"": 4.5, ""tags"": [""sword"", ""wind"", ""melee""]}}
  ]
}";

        [Test]
        public void CatalogReadsSkillAndMonsterFromCompiledGameData()
        {
            var catalog = GameDataCombatCatalog.FromCompiledManifestJson(CompiledManifestJson);
            var skill = catalog.GetSkill(GameDataCombatCatalog.DefaultSkillId);
            var monster = catalog.CreateMonster(GameDataCombatCatalog.DefaultMonsterId, 77UL, Vector3.zero);

            Assert.AreEqual("class.sword", skill.classId);
            Assert.AreEqual(6000, skill.cooldownMs);
            Assert.AreEqual(1.35f, skill.damageCoefficient, 0.001f);
            Assert.AreEqual(120, monster.maxHp);
            Assert.AreEqual(77UL, monster.entityId);
        }


        [Test]
        public void CatalogRejectsDuplicateSkillIds()
        {
            var duplicateJson = CompiledManifestJson.Replace(
                @"{""kind"": ""monsters"", ""source"": ""gamedata/monsters/shadow_slime.yaml"", ""data"": {""schema_version"": 1, ""id"": ""monster.shadow.slime"", ""level"": 1, ""max_hp"": 120, ""move_speed"": 2.5, ""archetype"": ""melee""}}",
                @"{""kind"": ""skills"", ""source"": ""gamedata/skills/wind_slash_copy.yaml"", ""data"": {""schema_version"": 1, ""id"": ""skill.sword.wind_slash"", ""class_id"": ""class.sword"", ""cooldown_ms"": 6000, ""damage"": {""coefficient"": 1.35}, ""range_m"": 4.5, ""tags"": [""sword"", ""wind""]}}"
            );

            Assert.Throws<System.InvalidOperationException>(() => GameDataCombatCatalog.FromCompiledManifestJson(duplicateJson));
        }

        [Test]
        public void CatalogRejectsMissingDefaultSkill()
        {
            var missingDefaultJson = CompiledManifestJson.Replace("skill.sword.wind_slash", "skill.sword.other_slash");

            Assert.Throws<System.InvalidOperationException>(() => GameDataCombatCatalog.FromCompiledManifestJson(missingDefaultJson));
        }

        [Test]
        public void InvalidSkillRequestIsRejectedWithoutChangingHp()
        {
            var simulator = M1OfflineCombatScenario.CreateSimulator(GameDataCombatCatalog.FromCompiledManifestJson(CompiledManifestJson));
            var rejected = simulator.Execute(CombatActionRequest.Skill(M1OfflineCombatScenario.PlayerEntityId, M1OfflineCombatScenario.EnemyEntityId, string.Empty, 1, 1000L));

            Assert.AreEqual(CombatActionStatus.RejectedInvalidRequest, rejected.status);
            Assert.AreEqual(120, rejected.targetHpAfter);
            Assert.AreEqual("skill_id_required", rejected.reason);
        }

        [Test]
        public void WindSlashDealsDeterministicGameDataDrivenDamage()
        {
            var simulator = M1OfflineCombatScenario.CreateSimulator(GameDataCombatCatalog.FromCompiledManifestJson(CompiledManifestJson));
            var result = simulator.Execute(CombatActionRequest.Skill(M1OfflineCombatScenario.PlayerEntityId, M1OfflineCombatScenario.EnemyEntityId, GameDataCombatCatalog.DefaultSkillId, 1, 1000L));

            Assert.AreEqual(CombatActionStatus.Applied, result.status);
            Assert.AreEqual(55, result.damage);
            Assert.AreEqual(65, result.targetHpAfter);
            Assert.AreEqual(7000L, result.nextReadyAtMs);
        }

        [Test]
        public void SkillCooldownRejectsEarlyRepeatWithoutChangingHp()
        {
            var simulator = M1OfflineCombatScenario.CreateSimulator(GameDataCombatCatalog.FromCompiledManifestJson(CompiledManifestJson));
            simulator.Execute(CombatActionRequest.Skill(M1OfflineCombatScenario.PlayerEntityId, M1OfflineCombatScenario.EnemyEntityId, GameDataCombatCatalog.DefaultSkillId, 1, 1000L));
            var rejected = simulator.Execute(CombatActionRequest.Skill(M1OfflineCombatScenario.PlayerEntityId, M1OfflineCombatScenario.EnemyEntityId, GameDataCombatCatalog.DefaultSkillId, 2, 2000L));

            Assert.AreEqual(CombatActionStatus.RejectedCooldown, rejected.status);
            Assert.AreEqual(65, rejected.targetHpAfter);
            Assert.AreEqual(7000L, rejected.nextReadyAtMs);
        }

        [Test]
        public void OutOfRangeActionIsRejected()
        {
            var catalog = GameDataCombatCatalog.FromCompiledManifestJson(CompiledManifestJson);
            var simulator = new OfflineCombatSimulator();
            simulator.AddSkill(catalog.GetSkill(GameDataCombatCatalog.DefaultSkillId));
            simulator.AddCombatant(M1OfflineCombatScenario.CreateSwordAdept(Vector3.zero));
            simulator.AddCombatant(catalog.CreateMonster(GameDataCombatCatalog.DefaultMonsterId, M1OfflineCombatScenario.EnemyEntityId, new Vector3(99f, 0f, 0f)));

            var rejected = simulator.Execute(CombatActionRequest.Skill(M1OfflineCombatScenario.PlayerEntityId, M1OfflineCombatScenario.EnemyEntityId, GameDataCombatCatalog.DefaultSkillId, 1, 1000L));

            Assert.AreEqual(CombatActionStatus.RejectedOutOfRange, rejected.status);
            Assert.AreEqual(120, rejected.targetHpAfter);
        }

        [Test]
        public void DeterministicDuelDefeatsStarterMonster()
        {
            var result = M1OfflineCombatScenario.RunDeterministicDuel(GameDataCombatCatalog.FromCompiledManifestJson(CompiledManifestJson));

            Assert.AreEqual("PASS", result.status);
            Assert.IsTrue(result.targetDefeated);
            Assert.AreEqual(120, result.initialEnemyHp);
            Assert.AreEqual(0, result.finalEnemyHp);
            Assert.AreEqual(160, result.initialPlayerHp);
            Assert.AreEqual(160, result.finalPlayerHp);
            Assert.AreEqual("Victory", result.lastActionStatus);
        }
    }
}
