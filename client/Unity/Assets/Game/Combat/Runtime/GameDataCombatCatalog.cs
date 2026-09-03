using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LinhGioi.Combat
{
    public sealed class GameDataCombatCatalog
    {
        public const string DefaultSkillId = "skill.sword.wind_slash";
        public const string DefaultMonsterId = "monster.shadow.slime";

        public CombatSkillDefinition[] skills = new CombatSkillDefinition[0];
        public CombatantState[] monsters = new CombatantState[0];

        public CombatSkillDefinition GetSkill(string id)
        {
            var skill = skills.FirstOrDefault(item => string.Equals(item.id, id, StringComparison.Ordinal));
            if (skill == null) throw new InvalidOperationException($"Skill not found in GameData catalog: {id}.");
            return skill;
        }

        public CombatantState CreateMonster(string id, ulong entityId, Vector3 position)
        {
            var template = monsters.FirstOrDefault(item => string.Equals(item.contentId, id, StringComparison.Ordinal));
            if (template == null) throw new InvalidOperationException($"Monster not found in GameData catalog: {id}.");
            var instance = template.Clone();
            instance.entityId = entityId;
            instance.position = position;
            instance.currentHp = instance.maxHp;
            return instance;
        }

        public static GameDataCombatCatalog FromCompiledManifestJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Compiled GameData manifest JSON must not be blank.", nameof(json));
            var manifest = JsonUtility.FromJson<CompiledManifestDto>(json);
            if (manifest == null || manifest.documents == null) throw new InvalidOperationException("Compiled GameData manifest could not be parsed.");

            if (manifest.gamedata_version != 1)
                throw new InvalidOperationException($"Unsupported compiled GameData version: {manifest.gamedata_version}.");

            var catalog = new GameDataCombatCatalog
            {
                skills = manifest.documents
                    .Where(doc => string.Equals(doc.kind, "skills", StringComparison.Ordinal) && doc.data != null)
                    .Select(doc => new CombatSkillDefinition
                    {
                        id = doc.data.id,
                        classId = doc.data.class_id,
                        cooldownMs = doc.data.cooldown_ms,
                        damageCoefficient = doc.data.damage == null ? 0f : doc.data.damage.coefficient,
                        rangeM = doc.data.range_m,
                        tags = doc.data.tags ?? new string[0]
                    })
                    .ToArray(),
                monsters = manifest.documents
                    .Where(doc => string.Equals(doc.kind, "monsters", StringComparison.Ordinal) && doc.data != null)
                    .Select(doc => new CombatantState
                    {
                        entityId = 1UL,
                        contentId = doc.data.id,
                        displayName = doc.data.id,
                        level = doc.data.level,
                        maxHp = doc.data.max_hp,
                        currentHp = doc.data.max_hp,
                        attackPower = Mathf.Max(1, doc.data.level * 8),
                        defense = Mathf.Max(0, doc.data.level * 2),
                        position = Vector3.zero
                    })
                    .ToArray()
            };

            foreach (var skill in catalog.skills) skill.Validate();
            foreach (var monster in catalog.monsters) monster.Validate();
            RequireUnique(catalog.skills.Select(skill => skill.id), "skill");
            RequireUnique(catalog.monsters.Select(monster => monster.contentId), "monster");
            if (catalog.skills.Length == 0) throw new InvalidOperationException("Compiled GameData manifest contains no combat skills.");
            if (catalog.monsters.Length == 0) throw new InvalidOperationException("Compiled GameData manifest contains no combat monsters.");
            catalog.GetSkill(DefaultSkillId);
            catalog.CreateMonster(DefaultMonsterId, 9999UL, Vector3.zero);
            return catalog;
        }


        private static void RequireUnique(IEnumerable<string> ids, string kind)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var id in ids)
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                if (!seen.Add(id)) throw new InvalidOperationException($"Duplicate {kind} id in compiled GameData manifest: {id}.");
            }
        }

        [Serializable]
        private sealed class CompiledManifestDto
        {
            public int gamedata_version;
            public DocumentDto[] documents;
        }

        [Serializable]
        private sealed class DocumentDto
        {
            public string kind;
            public string source;
            public DataDto data;
        }

        [Serializable]
        private sealed class DataDto
        {
            public int schema_version;
            public string id;
            public string class_id;
            public int cooldown_ms;
            public DamageDto damage;
            public float range_m;
            public string[] tags;
            public int level;
            public int max_hp;
            public float move_speed;
            public string archetype;
        }

        [Serializable]
        private sealed class DamageDto
        {
            public float coefficient;
        }
    }
}
