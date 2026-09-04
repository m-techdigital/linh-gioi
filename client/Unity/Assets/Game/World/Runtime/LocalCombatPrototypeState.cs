using System;
using LinhGioi.Protocol.V1;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class LocalCombatPrototypeState
    {
        public const ulong ActorEntityId = 1001UL;
        public const ulong TargetDummyEntityId = 2001UL;
        public const string WindSlashSkillId = "skill.sword.wind_slash";
        public const int WindSlashCooldownMs = 6000;
        public const int WindSlashPlaceholderAmount = 12;
        public const float WindSlashRangeM = 4.5f;
        public const int TargetDummyMaxHp = 120;
        public const string SmokePassMarker = "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0";

        private int _targetHp = TargetDummyMaxHp;
        private long _cooldownReadyAtMs;
        private bool _targetSelected;

        public LocalCombatTargetState TargetState { get; private set; } = LocalCombatTargetState.Idle;
        public string LastRejectedReason { get; private set; } = "";
        public int LastEffectAmount { get; private set; }
        public uint LastAcceptedSequence { get; private set; }
        public int TargetHp => _targetHp;
        public bool TargetSelected => _targetSelected;
        public bool CooldownActive(long nowMs) => nowMs < _cooldownReadyAtMs;
        public long CooldownRemainingMs(long nowMs) => Math.Max(0, _cooldownReadyAtMs - nowMs);

        public void Reset()
        {
            _targetHp = TargetDummyMaxHp;
            _cooldownReadyAtMs = 0;
            _targetSelected = false;
            TargetState = LocalCombatTargetState.Idle;
            LastRejectedReason = "";
            LastEffectAmount = 0;
            LastAcceptedSequence = 0;
        }

        public void SetTargetSelected(bool selected)
        {
            _targetSelected = selected;
            if (!selected)
            {
                TargetState = LocalCombatTargetState.Idle;
                return;
            }
            if (TargetState == LocalCombatTargetState.Idle)
                TargetState = LocalCombatTargetState.Selected;
        }

        public LocalCombatPrototypeOutcome TryWindSlash(CombatIntent intent, float distanceM, long nowMs)
        {
            if (!_targetSelected || intent.TargetEntityId == 0)
                return Reject(intent, "NO_TARGET", "Không có mục tiêu hợp lệ.");
            if (intent.TargetEntityId != TargetDummyEntityId)
                return Reject(intent, "INVALID_TARGET", "Mục tiêu không hợp lệ.");
            if (intent.SkillId != WindSlashSkillId)
                return Reject(intent, "UNKNOWN_SKILL", "Kỹ năng không có trong nguyên mẫu.");
            if (distanceM > WindSlashRangeM)
                return Reject(intent, "OUT_OF_RANGE", "Mục tiêu ngoài tầm kỹ năng.");
            if (CooldownActive(nowMs))
                return Reject(intent, "COOLDOWN_ACTIVE", "Kỹ năng đang hồi chiêu.");

            LastRejectedReason = "";
            LastEffectAmount = WindSlashPlaceholderAmount;
            LastAcceptedSequence = intent.Sequence;
            _targetHp = Math.Max(0, _targetHp - WindSlashPlaceholderAmount);
            _cooldownReadyAtMs = nowMs + WindSlashCooldownMs;
            TargetState = LocalCombatTargetState.Hit;
            return LocalCombatPrototypeOutcome.FromAccepted(intent, BuildAccepted(intent, nowMs), BuildResult(intent, nowMs), BuildSnapshot(intent, nowMs), _targetHp, WindSlashPlaceholderAmount);
        }

        public void MarkRecover()
        {
            TargetState = _targetSelected ? LocalCombatTargetState.Recover : LocalCombatTargetState.Idle;
        }

        public void MarkIdleIfReady(long nowMs)
        {
            if (!CooldownActive(nowMs))
                TargetState = _targetSelected ? LocalCombatTargetState.Selected : LocalCombatTargetState.Idle;
        }

        public void ForceCooldownReady()
        {
            _cooldownReadyAtMs = 0;
            MarkIdleIfReady(0);
        }

        private LocalCombatPrototypeOutcome Reject(CombatIntent intent, string code, string message)
        {
            LastRejectedReason = code;
            LastEffectAmount = 0;
            return LocalCombatPrototypeOutcome.FromRejected(intent, new CombatRejected
            {
                Sequence = intent.Sequence,
                IntentId = intent.IntentId,
                Error = new ErrorInfo { Code = code, Message = message, Retryable = code == "COOLDOWN_ACTIVE" },
                ServerTimeUnixMs = intent.ClientTimeUnixMs,
                Snapshot = BuildSnapshot(intent, intent.ClientTimeUnixMs)
            }, code);
        }

        private CombatAccepted BuildAccepted(CombatIntent intent, long nowMs)
        {
            return new CombatAccepted
            {
                Sequence = intent.Sequence,
                IntentId = intent.IntentId,
                ActorEntityId = intent.ActorEntityId,
                SkillId = intent.SkillId,
                CooldownMs = WindSlashCooldownMs,
                ServerTimeUnixMs = nowMs,
                Snapshot = BuildSnapshot(intent, nowMs)
            };
        }

        private CombatResult BuildResult(CombatIntent intent, long nowMs)
        {
            return new CombatResult
            {
                Sequence = intent.Sequence,
                IntentId = intent.IntentId,
                ActorEntityId = intent.ActorEntityId,
                TargetEntityId = intent.TargetEntityId,
                SkillId = intent.SkillId,
                EffectAmount = WindSlashPlaceholderAmount,
                Outcome = "LOCAL_PLACEHOLDER_HIT",
                ServerTimeUnixMs = nowMs,
                Snapshot = BuildSnapshot(intent, nowMs)
            };
        }

        private CombatStateSnapshot BuildSnapshot(CombatIntent intent, long nowMs)
        {
            long remainingMs = CooldownRemainingMs(nowMs);
            uint cooldownRemainingMs = remainingMs > int.MaxValue ? int.MaxValue : (uint)remainingMs;
            return new CombatStateSnapshot
            {
                ActorEntityId = intent.ActorEntityId,
                TargetEntityId = intent.TargetEntityId,
                ActiveSkillId = intent.SkillId,
                CooldownRemainingMs = cooldownRemainingMs,
                TargetValid = _targetSelected && intent.TargetEntityId == TargetDummyEntityId,
                ServerTimeUnixMs = nowMs
            };
        }
    }

    public enum LocalCombatTargetState
    {
        Idle,
        Selected,
        Telegraph,
        Hit,
        Recover
    }

    public sealed class LocalCombatPrototypeOutcome
    {
        public bool Accepted { get; private set; }
        public string RejectedReason { get; private set; }
        public CombatIntent Intent { get; private set; }
        public CombatAccepted AcceptedMessage { get; private set; }
        public CombatRejected RejectedMessage { get; private set; }
        public CombatResult ResultMessage { get; private set; }
        public CombatStateSnapshot Snapshot { get; private set; }
        public int TargetHpAfter { get; private set; }
        public int EffectAmount { get; private set; }

        public static LocalCombatPrototypeOutcome FromAccepted(CombatIntent intent, CombatAccepted accepted, CombatResult result, CombatStateSnapshot snapshot, int hpAfter, int effectAmount)
        {
            return new LocalCombatPrototypeOutcome
            {
                Accepted = true,
                RejectedReason = "",
                Intent = intent,
                AcceptedMessage = accepted,
                ResultMessage = result,
                Snapshot = snapshot,
                TargetHpAfter = hpAfter,
                EffectAmount = effectAmount
            };
        }

        public static LocalCombatPrototypeOutcome FromRejected(CombatIntent intent, CombatRejected rejected, string reason)
        {
            return new LocalCombatPrototypeOutcome
            {
                Accepted = false,
                RejectedReason = reason,
                Intent = intent,
                RejectedMessage = rejected,
                Snapshot = rejected.Snapshot,
                TargetHpAfter = LocalCombatPrototypeState.TargetDummyMaxHp,
                EffectAmount = 0
            };
        }
    }
}
