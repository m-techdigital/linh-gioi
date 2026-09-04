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
                world.SetSmokePositionNearTargetDummy();
                result.attackTriggered = world.TriggerLocalCombatForSmoke();
                result.targetDummyHitAcknowledged = world.TargetDummyHitAcknowledged;
                result.combatFeedbackText = world.CombatFeedbackText;
                result.targetDummyStatusText = world.TargetDummyStatusText;
                result.cooldownText = world.CombatCooldownText;
                if (!result.attackTriggered || !result.targetDummyHitAcknowledged)
                    throw new InvalidOperationException("local combat target dummy hit was not acknowledged");
                if (!result.combatFeedbackText.Contains("Trúng mục tiêu") || !result.combatFeedbackText.Contains("Chỉ là mô phỏng cục bộ"))
                    throw new InvalidOperationException("Vietnamese local combat feedback marker missing");
                result.status = "PASS";
                result.marker = "M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS";
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
            public bool attackTriggered;
            public bool targetDummyHitAcknowledged;
            public string combatFeedbackText;
            public string targetDummyStatusText;
            public string cooldownText;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
