using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using UnityEngine;

namespace LinhGioi.World
{
    public static class M5LightweightDialogueSmokeRunner
    {
        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M5_LIGHTWEIGHT_DIALOGUE_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m5-lightweight-dialogue-smoke") return true;
            return false;
        }

        public static Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var resultPath = GetArg("--lgo-m5-dialogue-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m5-lightweight-dialogue-result.json");
            var result = new M5LightweightDialogueSmokeResult
            {
                status = "STARTED",
                startedAtUtc = DateTimeOffset.UtcNow.ToString("O"),
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                resultPath = resultPath
            };
            var exitCode = 99;
            try
            {
                shutdownToken.ThrowIfCancellationRequested();
                var character = new CharacterResponse
                {
                    characterId = "character.m5.lightweight.dialogue",
                    accountId = "account.m5.lightweight.dialogue",
                    name = "DialogueHero",
                    classId = "class.sword",
                    entityId = 1001,
                    x = 0f,
                    y = 0.25f,
                    z = 0f,
                    yawDegrees = 0f
                };
                var world = new GameObject("LGO M5 Lightweight Dialogue Smoke World").AddComponent<PlayableWorldController>();
                world.Enter(character);
                world.SetSmokePositionNearGateKeeper();
                result.initialObjective = world.ObjectiveText;
                result.gateKeeperPrompt = world.InteractionText;
                result.openedDialogue = world.TriggerInteractionForSmoke();
                Require(result.openedDialogue, "Gate Keeper interaction did not open dialogue");
                Require(world.DialogueActive, "dialogue panel state was not active");
                result.firstLine = world.DialogueLine;
                result.firstProgress = world.DialogueProgress;
                world.ContinueDialogue();
                result.secondLine = world.DialogueLine;
                result.secondProgress = world.DialogueProgress;
                while (world.DialogueActive)
                    world.ContinueDialogue();
                result.dialogueCompleted = world.DialogueCompleted;
                result.afterDialogueObjective = world.ObjectiveText;
                result.afterDialogueFeedback = world.InteractionText;
                Require(result.dialogueCompleted, "dialogue did not complete");
                Require(result.afterDialogueObjective.Contains("Đá Luyện"), "dialogue close did not advance objective");
                var save = world.BuildSaveRequest();
                result.savePositionStillWorks = Math.Abs(save.x + 3f) < 0.001f;
                Require(result.savePositionStillWorks, "save position request regressed");
                result.status = "PASS";
                exitCode = 0;
            }
            catch (Exception exception)
            {
                result.status = exception is OperationCanceledException ? "TIMEOUT" : "FAIL";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = exception is OperationCanceledException ? 13 : 14;
            }
            finally
            {
                result.finishedAtUtc = DateTimeOffset.UtcNow.ToString("O");
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log($"[LinhGioi] M5 lightweight dialogue smoke status={result.status} result={resultPath}");
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

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void WriteResult(string path, M5LightweightDialogueSmokeResult result)
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
        private sealed class M5LightweightDialogueSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string initialObjective;
            public string gateKeeperPrompt;
            public bool openedDialogue;
            public string firstLine;
            public string firstProgress;
            public string secondLine;
            public string secondProgress;
            public bool dialogueCompleted;
            public string afterDialogueObjective;
            public string afterDialogueFeedback;
            public bool savePositionStillWorks;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
