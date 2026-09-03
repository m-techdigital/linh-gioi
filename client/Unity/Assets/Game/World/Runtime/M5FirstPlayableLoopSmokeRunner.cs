using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.Foundation;
using UnityEngine;

namespace LinhGioi.World
{
    public static class M5FirstPlayableLoopSmokeRunner
    {
        private const int DefaultTimeoutMs = 25000;
        private const string DefaultCharacterName = "M5LoopHero";
        private const string DefaultClassId = "class.sword";
        private const string DefaultDevKey = "m5-first-playable-loop-dev-key";

        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M5_FIRST_PLAYABLE_LOOP_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m5-first-playable-loop-smoke") return true;
            return false;
        }

        public static async Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var resultPath = GetArg("--lgo-m5-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m5-first-playable-loop-result.json");
            var result = new M5FirstPlayableLoopSmokeResult
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
                    timeout.CancelAfter(GetIntArg("--lgo-m5-timeout-ms", DefaultTimeoutMs));
                    var token = timeout.Token;

                    var login = await client.LoginDevAsync(GetArg("--lgo-m5-dev-key") ?? DefaultDevKey, GetArg("--lgo-m5-display-name") ?? "M5 Loop Smoke", token);
                    Require(login != null && login.account != null, "dev login returned no account");
                    result.accountId = login.account.accountId;

                    var characterName = GetArg("--lgo-m5-character-name") ?? DefaultCharacterName;
                    var listed = await client.ListCharactersAsync(result.accountId, token);
                    var character = FindByName(listed, characterName);
                    if (character == null)
                        character = await client.CreateCharacterAsync(result.accountId, characterName, GetArg("--lgo-m5-class-id") ?? DefaultClassId, token);
                    Require(character != null, "character selection failed");

                    var loaded = await client.LoadCharacterAsync(character.characterId, token);
                    var world = new GameObject("LGO M5 First Playable Loop Smoke World").AddComponent<PlayableWorldController>();
                    world.Enter(loaded);
                    result.enteredWorld = true;
                    result.initialObjective = world.ObjectiveText;

                    world.SetSmokePositionNearTrainingStone();
                    result.nearInteractablePrompt = world.InteractionText;
                    Require(result.nearInteractablePrompt.Contains("Press F or Space"), "interaction prompt did not become available");

                    result.interactionTriggered = world.TriggerInteractionForSmoke();
                    Require(result.interactionTriggered, "interaction did not trigger");
                    Require(world.InteractionAcknowledged, "interaction acknowledgement flag missing");
                    Require(world.ObjectiveText.Contains("Objective complete"), "objective did not reach completion feedback");
                    result.finalObjective = world.ObjectiveText;
                    result.finalFeedback = world.InteractionText;

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
                Debug.Log($"[LinhGioi] M5 first playable loop smoke status={result.status} result={resultPath}");
                Quit(exitCode);
            }
        }

        private static ClientRuntimeConfig BuildConfig()
        {
            var config = ClientRuntimeConfig.LoadStreamingAssets();
            var apiBaseUrl = GetArg("--lgo-m5-api-url");
            if (!string.IsNullOrWhiteSpace(apiBaseUrl)) config.apiBaseUrl = apiBaseUrl;
            config.apiTimeoutSeconds = GetIntArg("--lgo-m5-api-timeout-seconds", config.apiTimeoutSeconds);
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

        private static void WriteResult(string path, M5FirstPlayableLoopSmokeResult result)
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
        private sealed class M5FirstPlayableLoopSmokeResult
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
            public string nearInteractablePrompt;
            public bool interactionTriggered;
            public string finalObjective;
            public string finalFeedback;
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
