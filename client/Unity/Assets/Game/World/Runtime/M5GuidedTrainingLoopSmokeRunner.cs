using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.Foundation;
using UnityEngine;

namespace LinhGioi.World
{
    public static class M5GuidedTrainingLoopSmokeRunner
    {
        private const int DefaultTimeoutMs = 25000;
        private const string DefaultCharacterName = "M5GuidedHero";
        private const string DefaultClassId = "class.sword";
        private const string DefaultDevKey = "m5-guided-training-loop-dev-key";

        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M5_GUIDED_TRAINING_LOOP_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m5-guided-training-loop-smoke") return true;
            return false;
        }

        public static async Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var resultPath = GetArg("--lgo-m5-guided-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m5-guided-training-loop-result.json");
            var result = new M5GuidedTrainingLoopSmokeResult
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
                using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(shutdownToken))
                using (var client = new AccountApiClient(BuildConfig()))
                {
                    timeout.CancelAfter(GetIntArg("--lgo-m5-guided-timeout-ms", DefaultTimeoutMs));
                    var token = timeout.Token;

                    var login = await client.LoginDevAsync(GetArg("--lgo-m5-guided-dev-key") ?? DefaultDevKey, "M5 Guided Training Smoke", token);
                    Require(login != null && login.account != null, "dev login returned no account");
                    result.accountId = login.account.accountId;

                    var characterName = GetArg("--lgo-m5-guided-character-name") ?? DefaultCharacterName;
                    var listed = await client.ListCharactersAsync(result.accountId, token);
                    var character = FindByName(listed, characterName);
                    if (character == null)
                        character = await client.CreateCharacterAsync(result.accountId, characterName, GetArg("--lgo-m5-guided-class-id") ?? DefaultClassId, token);
                    Require(character != null, "character selection failed");

                    var loaded = await client.LoadCharacterAsync(character.characterId, token);
                    var world = new GameObject("LGO M5 Guided Training Loop Smoke World").AddComponent<PlayableWorldController>();
                    world.Enter(loaded);
                    result.enteredWorld = true;
                    result.initialObjective = world.ObjectiveText;
                    result.initialVfxFeedbackState = world.VfxFeedbackStateName;
                    Require(result.initialObjective.Contains("Gate Keeper"), "initial objective did not point to Gate Keeper");
                    Require(result.initialVfxFeedbackState == "PortalGatePulse", "initial portal VFX feedback did not appear");

                    world.SetSmokePositionNearGateKeeper();
                    result.gateKeeperPrompt = world.InteractionText;
                    Require(result.gateKeeperPrompt.Contains("Gate Keeper"), "Gate Keeper prompt did not become available");
                    result.gateKeeperInteractionTriggered = world.TriggerInteractionForSmoke();
                    Require(result.gateKeeperInteractionTriggered, "Gate Keeper interaction did not trigger");
                    while (world.DialogueActive)
                        world.ContinueDialogue();
                    result.afterGateKeeperObjective = world.ObjectiveText;
                    result.afterGateKeeperFeedback = world.InteractionText;
                    result.afterGateKeeperVfxFeedbackState = world.VfxFeedbackStateName;
                    Require(result.afterGateKeeperObjective.Contains("Training Stone"), "objective did not advance to Training Stone");
                    Require(world.GuidedTrainingStepName == "FindTrainingStone", "guided step did not advance after Gate Keeper");
                    Require(result.afterGateKeeperVfxFeedbackState == "WindSlashPreview", "Gate Keeper interaction did not trigger wind slash preview");

                    world.SetSmokePositionNearTrainingStone();
                    result.trainingStonePrompt = world.InteractionText;
                    Require(result.trainingStonePrompt.Contains("Training Stone") || result.trainingStonePrompt.Contains("spirit pulse"), "Training Stone prompt did not become available");
                    result.trainingStoneInteractionTriggered = world.TriggerInteractionForSmoke();
                    Require(result.trainingStoneInteractionTriggered, "Training Stone interaction did not trigger");
                    Require(world.InteractionAcknowledged, "final acknowledgement flag missing");
                    result.finalObjective = world.ObjectiveText;
                    result.finalFeedback = world.InteractionText;
                    result.finalVfxFeedbackState = world.VfxFeedbackStateName;
                    Require(result.finalObjective.Contains("Objective complete"), "objective did not complete");
                    Require(result.finalFeedback.Contains("Spirit pulse stabilized"), "final feedback did not show spirit pulse stabilization");
                    Require(result.finalVfxFeedbackState == "SpiritPulse", "Training Stone interaction did not trigger spirit pulse VFX feedback");

                    var save = world.BuildSaveRequest();
                    var saved = await client.SaveCharacterPositionAsync(loaded.characterId, save.x, save.y, save.z, save.yawDegrees, token);
                    Require(saved.HasSamePosition(save.x, save.y, save.z, save.yawDegrees), "existing save position behavior regressed");
                    result.savePositionStillWorks = true;
                    result.characterId = saved.characterId;
                    result.characterName = saved.name;
                    result.classId = saved.classId;
                    result.savedX = saved.x;
                    result.savedY = saved.y;
                    result.savedZ = saved.z;
                    result.savedYawDegrees = saved.yawDegrees;
                    result.status = "PASS";
                    exitCode = 0;
                }
            }
            catch (OperationCanceledException exception)
            {
                result.status = "TIMEOUT";
                result.exceptionType = exception.GetType().FullName;
                result.exceptionMessage = exception.Message;
                exitCode = 13;
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
                result.finishedAtUtc = DateTimeOffset.UtcNow.ToString("O");
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log($"[LinhGioi] M5 guided training loop smoke status={result.status} result={resultPath}");
                Quit(exitCode);
            }
        }

        private static ClientRuntimeConfig BuildConfig()
        {
            var config = ClientRuntimeConfig.LoadStreamingAssets();
            var apiBaseUrl = GetArg("--lgo-m5-guided-api-url");
            if (!string.IsNullOrWhiteSpace(apiBaseUrl)) config.apiBaseUrl = apiBaseUrl;
            config.apiTimeoutSeconds = GetIntArg("--lgo-m5-guided-api-timeout-seconds", config.apiTimeoutSeconds);
            config.Validate();
            return config;
        }

        private static CharacterResponse FindByName(CharacterResponse[] characters, string name)
        {
            if (characters == null) return null;
            foreach (var character in characters)
                if (character != null && string.Equals(character.name, name, StringComparison.Ordinal)) return character;
            return null;
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
        }

        private static int GetIntArg(string key, int fallback)
        {
            var raw = GetArg(key);
            if (string.IsNullOrWhiteSpace(raw)) return fallback;
            if (!int.TryParse(raw, out var value)) throw new InvalidOperationException(key + " must be an integer.");
            return value;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void WriteResult(string path, M5GuidedTrainingLoopSmokeResult result)
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
        private sealed class M5GuidedTrainingLoopSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string accountId;
            public string characterId;
            public string characterName;
            public string classId;
            public bool enteredWorld;
            public string initialObjective;
            public string initialVfxFeedbackState;
            public string gateKeeperPrompt;
            public bool gateKeeperInteractionTriggered;
            public string afterGateKeeperObjective;
            public string afterGateKeeperFeedback;
            public string afterGateKeeperVfxFeedbackState;
            public string trainingStonePrompt;
            public bool trainingStoneInteractionTriggered;
            public string finalObjective;
            public string finalFeedback;
            public string finalVfxFeedbackState;
            public bool savePositionStillWorks;
            public float savedX;
            public float savedY;
            public float savedZ;
            public float savedYawDegrees;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
