using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.Protocol.V1;
using UnityEngine;

namespace LinhGioi.World
{
    public static class M6UnityCombatIntentClientSmokeRunner
    {
        public static bool ShouldRun()
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m6-unity-combat-intent-client-smoke") return true;
            return false;
        }

        public static Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var resultPath = GetArg("--lgo-m6-unity-combat-intent-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m6-unity-combat-intent-client-result.json");
            var result = new M6UnityCombatIntentClientSmokeResult { status = "STARTED", resultPath = resultPath, unityVersion = Application.unityVersion };
            var exitCode = 99;
            try
            {
                shutdownToken.ThrowIfCancellationRequested();
                var world = new GameObject("LGO M6 Unity Combat Intent Client Smoke World").AddComponent<PlayableWorldController>();
                world.Enter(new CharacterResponse { characterId = "m6-unity-client-character", accountId = "m6-unity-client-account", name = "M6IntentHero", classId = "class.sword", x = 0f, y = 0.25f, z = 0f, yawDegrees = 0f });
                world.SetSmokePositionNearTargetDummy();
                var intent = world.BuildCombatIntentForLocalPreview(42, "unity-smoke-intent-42");
                world.MarkCombatIntentPending(intent);
                result.intentBuilt = intent.ProtocolVersion == 1 && intent.Sequence == 42 && intent.SkillId == "skill.sword.wind_slash" && intent.LocalPreviewOnly;
                result.pendingText = world.CombatAuthorityText;
                result.localPreviewTriggered = world.TriggerLocalCombatForSmoke();
                world.MarkCombatIntentAccepted(new CombatAccepted { Sequence = intent.Sequence, IntentId = intent.IntentId, ActorEntityId = intent.ActorEntityId, SkillId = intent.SkillId, CooldownMs = 1500, ServerTimeUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
                result.acceptedText = world.CombatAuthorityText;
                world.MarkCombatIntentRejected(new CombatRejected { Sequence = intent.Sequence, IntentId = intent.IntentId, Error = new ErrorInfo { Code = "combat_intent_rejected_smoke", Message = "smoke rejection", Retryable = true }, ServerTimeUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
                result.rejectedText = world.CombatAuthorityText;
                if (!result.intentBuilt) throw new InvalidOperationException("canonical CombatIntent was not built");
                if (!result.pendingText.Contains("Gửi ý định chiến đấu") || !result.pendingText.Contains("Đang xác thực") || !result.pendingText.Contains("Mô phỏng cục bộ")) throw new InvalidOperationException("pending Vietnamese combat intent text missing");
                if (!result.acceptedText.Contains("Máy chủ chấp nhận") || !result.acceptedText.Contains("Kết quả máy chủ")) throw new InvalidOperationException("accepted Vietnamese combat intent text missing");
                if (!result.rejectedText.Contains("Máy chủ từ chối")) throw new InvalidOperationException("rejected Vietnamese combat intent text missing");
                result.status = "PASS";
                result.marker = "M6_UNITY_COMBAT_INTENT_CLIENT_RUNTIME_SMOKE_PASS";
                exitCode = 0;
            }
            catch (Exception exception)
            {
                result.status = "FAIL";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = 15;
            }
            finally
            {
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log("[LinhGioi] M6 Unity combat intent client smoke status=" + result.status + " marker=" + result.marker + " result=" + resultPath);
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

        private static void WriteResult(string path, M6UnityCombatIntentClientSmokeResult result)
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
        private sealed class M6UnityCombatIntentClientSmokeResult
        {
            public string status;
            public string marker;
            public string resultPath;
            public string unityVersion;
            public bool intentBuilt;
            public bool localPreviewTriggered;
            public string pendingText;
            public string acceptedText;
            public string rejectedText;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
