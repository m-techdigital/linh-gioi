using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.Combat
{
    public sealed class OfflineCombatSimulator
    {
        public const string BasicAttackActionId = "basic.attack";
        public const float BasicAttackRangeM = 2.2f;
        public const int BasicAttackCooldownMs = 700;

        private readonly Dictionary<ulong, CombatantState> _combatants = new Dictionary<ulong, CombatantState>();
        private readonly Dictionary<string, CombatSkillDefinition> _skills = new Dictionary<string, CombatSkillDefinition>(StringComparer.Ordinal);
        private readonly Dictionary<string, long> _nextReadyAtMs = new Dictionary<string, long>(StringComparer.Ordinal);
        private readonly List<CombatActionResult> _history = new List<CombatActionResult>();

        public IReadOnlyList<CombatActionResult> History => _history;

        public void AddCombatant(CombatantState combatant)
        {
            if (combatant == null) throw new ArgumentNullException(nameof(combatant));
            combatant.Validate();
            if (_combatants.ContainsKey(combatant.entityId)) throw new InvalidOperationException($"Duplicate combatant entityId={combatant.entityId}.");
            _combatants.Add(combatant.entityId, combatant.Clone());
        }

        public void AddSkill(CombatSkillDefinition skill)
        {
            if (skill == null) throw new ArgumentNullException(nameof(skill));
            skill.Validate();
            if (_skills.ContainsKey(skill.id)) throw new InvalidOperationException($"Duplicate skill id={skill.id}.");
            _skills.Add(skill.id, skill);
        }

        public CombatantState GetCombatant(ulong entityId)
        {
            if (!_combatants.TryGetValue(entityId, out var combatant)) throw new KeyNotFoundException($"Combatant not found: {entityId}.");
            return combatant.Clone();
        }

        public CombatActionResult Execute(CombatActionRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!_combatants.TryGetValue(request.sourceEntityId, out var source)) throw new KeyNotFoundException($"Source combatant not found: {request.sourceEntityId}.");
            if (!_combatants.TryGetValue(request.targetEntityId, out var target)) throw new KeyNotFoundException($"Target combatant not found: {request.targetEntityId}.");

            var actionId = string.IsNullOrWhiteSpace(request.actionId) ? BasicAttackActionId : request.actionId;
            var result = NewResult(request, target, actionId);
            if (request.sequence <= 0) return Reject(result, CombatActionStatus.RejectedInvalidRequest, "sequence_must_be_positive");
            if (request.nowMs < 0L) return Reject(result, CombatActionStatus.RejectedInvalidRequest, "now_ms_must_be_non_negative");
            if (request.sourceEntityId == request.targetEntityId) return Reject(result, CombatActionStatus.RejectedInvalidRequest, "source_and_target_must_differ");
            if (!IsSupportedKind(request.kind)) return Reject(result, CombatActionStatus.RejectedInvalidRequest, "unsupported_action_kind");
            if (request.kind == CombatActionKind.Skill && string.IsNullOrWhiteSpace(request.actionId)) return Reject(result, CombatActionStatus.RejectedInvalidRequest, "skill_id_required");
            if (source.IsDefeated) return Reject(result, CombatActionStatus.RejectedSourceDefeated, "source_defeated");
            if (target.IsDefeated) return Reject(result, CombatActionStatus.RejectedTargetDefeated, "target_defeated");

            var coefficient = 1f;
            var range = BasicAttackRangeM;
            var cooldownMs = BasicAttackCooldownMs;
            if (request.kind == CombatActionKind.Skill)
            {
                if (!_skills.TryGetValue(actionId, out var skill)) return Reject(result, CombatActionStatus.RejectedUnknownSkill, "unknown_skill");
                coefficient = skill.damageCoefficient;
                range = skill.rangeM;
                cooldownMs = skill.cooldownMs;
            }

            var distance = Vector3.Distance(source.position, target.position);
            if (distance > range) return Reject(result, CombatActionStatus.RejectedOutOfRange, $"distance={distance:0.00};range={range:0.00}");

            var cooldownKey = source.entityId + ":" + actionId;
            if (_nextReadyAtMs.TryGetValue(cooldownKey, out var nextReadyAtMs) && request.nowMs < nextReadyAtMs)
            {
                result.nextReadyAtMs = nextReadyAtMs;
                return Reject(result, CombatActionStatus.RejectedCooldown, $"next_ready_at_ms={nextReadyAtMs}");
            }

            var rawDamage = Mathf.RoundToInt(source.attackPower * coefficient);
            var damage = Mathf.Max(1, rawDamage - target.defense);
            target.currentHp = Mathf.Max(0, target.currentHp - damage);

            result.damage = damage;
            result.targetHpAfter = target.currentHp;
            result.targetDefeated = target.IsDefeated;
            result.nextReadyAtMs = request.nowMs + cooldownMs;
            result.reason = target.IsDefeated ? "target_defeated" : "applied";
            result.status = target.IsDefeated ? CombatActionStatus.Victory : CombatActionStatus.Applied;
            _nextReadyAtMs[cooldownKey] = result.nextReadyAtMs;
            _history.Add(result);
            return result;
        }

        private static CombatActionResult NewResult(CombatActionRequest request, CombatantState target, string actionId)
        {
            return new CombatActionResult
            {
                status = CombatActionStatus.Applied,
                sequence = request.sequence,
                sourceEntityId = request.sourceEntityId,
                targetEntityId = request.targetEntityId,
                actionId = actionId,
                targetHpBefore = target.currentHp,
                targetHpAfter = target.currentHp,
                appliedAtMs = request.nowMs,
                reason = string.Empty
            };
        }

        private static bool IsSupportedKind(CombatActionKind kind)
        {
            return kind == CombatActionKind.BasicAttack || kind == CombatActionKind.Skill;
        }

        private CombatActionResult Reject(CombatActionResult result, CombatActionStatus status, string reason)
        {
            result.status = status;
            result.reason = reason;
            _history.Add(result);
            return result;
        }
    }
}
