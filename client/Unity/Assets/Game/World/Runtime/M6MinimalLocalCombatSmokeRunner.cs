using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using UnityEngine;

namespace LinhGioi.World
{
    public static class M6MinimalLocalCombatSmokeRunner
    {
        private const string V049SmokeMarker = "M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0";

        public static bool ShouldRun()
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m6-minimal-local-combat-smoke") return true;
            return false;
        }

        public static Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var resultPath = GetArg("--lgo-m6-combat-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m6-minimal-local-combat-result.json");
            var result = new M6MinimalLocalCombatSmokeResult
            {
                status = "STARTED",
                marker = "",
                resultPath = resultPath,
                unityVersion = Application.unityVersion
            };
            var exitCode = 99;
            try
            {
                shutdownToken.ThrowIfCancellationRequested();
                var world = new GameObject("LGO M6 Minimal Local Combat Smoke World").AddComponent<PlayableWorldController>();
                world.Enter(new CharacterResponse
                {
                    characterId = "m6-local-character",
                    accountId = "m6-local-account",
                    name = "M6LocalHero",
                    classId = "class.sword",
                    x = 0f,
                    y = 0.25f,
                    z = 0f,
                    yawDegrees = 0f
                });
                var baseTimeMs = 1700000000000L;
                result.executedChecks = 0;
                var noTarget = world.TryLocalCombatPrototypeWithoutTargetForSmoke(baseTimeMs);
                result.rejectedNoTarget = !noTarget.Accepted && noTarget.RejectedReason == "NO_TARGET";
                result.rejectedNoTargetReason = noTarget.RejectedReason;
                result.rejectedNoTargetRetryable = noTarget.RejectedMessage != null && noTarget.RejectedMessage.Error.Retryable;
                result.rejectedNoTargetSnapshotTargetValid = noTarget.Snapshot != null && noTarget.Snapshot.TargetValid;
                result.executedChecks++;

                world.SetSmokePosition(0f, 0.25f, -4f, 0f);
                var outOfRange = world.TryLocalCombatPrototypeAt(baseTimeMs + 1000);
                result.rejectedOutOfRange = !outOfRange && world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.RejectedReason == "OUT_OF_RANGE";
                result.rejectedOutOfRangeReason = world.LastLocalCombatOutcome != null ? world.LastLocalCombatOutcome.RejectedReason : "";
                result.rejectedOutOfRangeRetryable = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.RejectedMessage != null && world.LastLocalCombatOutcome.RejectedMessage.Error.Retryable;
                result.rejectedOutOfRangeSnapshotTargetValid = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.Snapshot != null && world.LastLocalCombatOutcome.Snapshot.TargetValid;
                result.executedChecks++;

                world.SetSmokePositionNearTargetDummy();
                result.attackTriggered = world.TryLocalCombatPrototypeAt(baseTimeMs + 2000);
                result.targetDummyHitAcknowledged = world.TargetDummyHitAcknowledged;
                result.combatFeedbackText = world.CombatFeedbackText;
                result.targetDummyStatusText = world.TargetDummyStatusText;
                result.cooldownText = world.CombatCooldownText;
                result.acceptedEffectAmount = world.LastLocalCombatOutcome != null ? world.LastLocalCombatOutcome.EffectAmount : 0;
                result.acceptedTargetHpAfter = world.LastLocalCombatOutcome != null ? world.LastLocalCombatOutcome.TargetHpAfter : 0;
                result.acceptedTargetState = world.LocalCombatTargetStateName;
                result.acceptedIntentId = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.Intent != null ? world.LastLocalCombatOutcome.Intent.IntentId : "";
                result.acceptedSequence = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.AcceptedMessage != null ? world.LastLocalCombatOutcome.AcceptedMessage.Sequence : 0;
                result.acceptedCooldownMs = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.AcceptedMessage != null ? world.LastLocalCombatOutcome.AcceptedMessage.CooldownMs : 0;
                result.acceptedOutcome = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.ResultMessage != null ? world.LastLocalCombatOutcome.ResultMessage.Outcome : "";
                result.acceptedSnapshotTargetValid = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.Snapshot != null && world.LastLocalCombatOutcome.Snapshot.TargetValid;
                result.executedChecks++;

                result.cooldownBlockedAfterRepeatedInput = !world.TryLocalCombatPrototypeAt(baseTimeMs + 2500);
                result.cooldownBlockedFeedbackText = world.CombatFeedbackText;
                result.rejectedCooldownReason = world.LastLocalCombatOutcome != null ? world.LastLocalCombatOutcome.RejectedReason : "";
                result.rejectedCooldownRetryable = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.RejectedMessage != null && world.LastLocalCombatOutcome.RejectedMessage.Error.Retryable;
                result.rejectedCooldownRemainingMs = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.Snapshot != null ? world.LastLocalCombatOutcome.Snapshot.CooldownRemainingMs : 0;
                result.rejectedCooldownSnapshotTargetValid = world.LastLocalCombatOutcome != null && world.LastLocalCombatOutcome.Snapshot != null && world.LastLocalCombatOutcome.Snapshot.TargetValid;
                result.executedChecks++;

                world.RecoverLocalCombatCooldownForSmoke();
                result.cooldownRecoveredText = world.CombatCooldownText;
                result.attackAfterCooldownRecovered = world.TryLocalCombatPrototypeAt(baseTimeMs + 9000);
                result.feedbackAfterCooldownRecovered = world.CombatFeedbackText;
                result.executedChecks++;

                result.nonClaims = "local-only prototype; not production combat; not production art; no loot/reward/economy/db/auth/social/liveops";
                if (result.executedChecks < 5)
                    throw new InvalidOperationException("local combat smoke executed zero or insufficient checks");
                if (!result.rejectedNoTarget || !result.rejectedOutOfRange)
                    throw new InvalidOperationException("local combat rejected path coverage missing");
                if (!result.attackTriggered || !result.targetDummyHitAcknowledged)
                    throw new InvalidOperationException("local combat target dummy hit was not acknowledged");
                if (result.acceptedEffectAmount != LocalCombatPrototypeState.WindSlashPlaceholderAmount || result.acceptedTargetHpAfter != LocalCombatPrototypeState.TargetDummyMaxHp - LocalCombatPrototypeState.WindSlashPlaceholderAmount)
                    throw new InvalidOperationException("accepted local combat result did not use current GameData placeholder values");
                if (!result.combatFeedbackText.Contains("Trúng mục tiêu") || !result.combatFeedbackText.Contains("Chỉ là mô phỏng cục bộ"))
                    throw new InvalidOperationException("Vietnamese local combat feedback marker missing");
                if (!result.cooldownBlockedAfterRepeatedInput || result.rejectedCooldownReason != "COOLDOWN_ACTIVE" || !result.cooldownBlockedFeedbackText.Contains("Chưa thể tấn công") || !result.cooldownBlockedFeedbackText.Contains("Đang hồi chiêu"))
                    throw new InvalidOperationException("repeated attack did not produce deterministic cooldown block feedback");
                if (result.acceptedSequence == 0 || result.acceptedCooldownMs != LocalCombatPrototypeState.WindSlashCooldownMs || result.acceptedOutcome != "LOCAL_PLACEHOLDER_HIT" || !result.acceptedSnapshotTargetValid)
                    throw new InvalidOperationException("accepted local combat diagnostic evidence is incomplete");
                if (result.rejectedNoTargetRetryable || result.rejectedOutOfRangeRetryable || !result.rejectedCooldownRetryable || result.rejectedCooldownRemainingMs == 0 || !result.rejectedCooldownSnapshotTargetValid)
                    throw new InvalidOperationException("rejected local combat diagnostic evidence is incomplete");
                if (!result.cooldownRecoveredText.Contains("Sẵn sàng") || !result.attackAfterCooldownRecovered || !result.feedbackAfterCooldownRecovered.Contains("Trúng mục tiêu"))
                    throw new InvalidOperationException("cooldown recovery did not restore deterministic local attack feedback");
                result.status = "PASS";
                result.marker = V049SmokeMarker;
                result.legacyMarker = "M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS";
                exitCode = 0;
            }
            catch (Exception exception)
            {
                result.status = "FAIL";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = 14;
            }
            finally
            {
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log("[LinhGioi] M6 minimal local combat smoke status=" + result.status + " marker=" + result.marker + " result=" + resultPath);
                Quit(exitCode);
            }
            return Task.CompletedTask;
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
        }

        private static void WriteResult(string path, M6MinimalLocalCombatSmokeResult result)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, JsonUtility.ToJson(result, true));
        }

        private static void Quit(int exitCode)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(exitCode);
#else
            Application.Quit(exitCode);
#endif
        }

        [Serializable]
        private sealed class M6MinimalLocalCombatSmokeResult
        {
            public string status;
            public string marker;
            public string resultPath;
            public string unityVersion;
            public int executedChecks;
            public bool rejectedNoTarget;
            public string rejectedNoTargetReason;
            public bool rejectedNoTargetRetryable;
            public bool rejectedNoTargetSnapshotTargetValid;
            public bool rejectedOutOfRange;
            public string rejectedOutOfRangeReason;
            public bool rejectedOutOfRangeRetryable;
            public bool rejectedOutOfRangeSnapshotTargetValid;
            public bool attackTriggered;
            public bool targetDummyHitAcknowledged;
            public string acceptedIntentId;
            public uint acceptedSequence;
            public uint acceptedCooldownMs;
            public string acceptedOutcome;
            public bool acceptedSnapshotTargetValid;
            public int acceptedEffectAmount;
            public int acceptedTargetHpAfter;
            public string acceptedTargetState;
            public string combatFeedbackText;
            public string targetDummyStatusText;
            public string cooldownText;
            public bool cooldownBlockedAfterRepeatedInput;
            public string rejectedCooldownReason;
            public bool rejectedCooldownRetryable;
            public uint rejectedCooldownRemainingMs;
            public bool rejectedCooldownSnapshotTargetValid;
            public string cooldownBlockedFeedbackText;
            public string cooldownRecoveredText;
            public bool attackAfterCooldownRecovered;
            public string feedbackAfterCooldownRecovered;
            public string legacyMarker;
            public string nonClaims;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
