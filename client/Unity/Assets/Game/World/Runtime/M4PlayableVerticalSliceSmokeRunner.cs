using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.Foundation;
using UnityEngine;

namespace LinhGioi.World
{
    public static class M4PlayableVerticalSliceSmokeRunner
    {
        private const int DefaultTimeoutMs = 25000;
        private const string DefaultCharacterName = "M4VerticalHero";
        private const string DefaultClassId = "class.sword";
        private const string DefaultDevKey = "m4-playable-dev-key";

        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M4_PLAYABLE_VERTICAL_SLICE_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m4-playable-vertical-slice-smoke") return true;
            return false;
        }

        public static async Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var resultPath = GetArg("--lgo-m4-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m4-playable-vertical-slice-result.json");
            var result = new M4PlayableVerticalSliceSmokeResult
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
                    timeout.CancelAfter(GetIntArg("--lgo-m4-timeout-ms", DefaultTimeoutMs));
                    var token = timeout.Token;
                    var characterName = GetArg("--lgo-m4-character-name") ?? DefaultCharacterName;
                    var classId = GetArg("--lgo-m4-class-id") ?? DefaultClassId;
                    var expectExisting = HasArg("--lgo-m4-expect-existing");
                    result.expectExisting = expectExisting;

                    var login = await client.LoginDevAsync(GetArg("--lgo-m4-dev-key") ?? DefaultDevKey, GetArg("--lgo-m4-display-name") ?? "M4 Playable Smoke", token);
                    Require(login != null && login.account != null, "dev login returned no account");
                    result.accountId = login.account.accountId;
                    result.loginCreated = login.created;
                    result.initialCharacterCount = login.characters == null ? 0 : login.characters.Length;
                    if (expectExisting && login.created) throw new InvalidOperationException("expected persisted dev account to exist after restart");

                    var listed = await client.ListCharactersAsync(result.accountId, token);
                    result.listedBeforeCreateCount = listed.Length;
                    var character = FindByName(listed, characterName);
                    result.reusedExistingCharacter = character != null;
                    if (expectExisting && character == null) throw new InvalidOperationException("expected persisted character to be listed after restart");
                    if (character == null) character = await client.CreateCharacterAsync(result.accountId, characterName, classId, token);
                    Require(character != null, "character selection failed");
                    Require(character.classId == classId, "class id mismatch");

                    var loaded = await client.LoadCharacterAsync(character.characterId, token);
                    Require(loaded != null, "selected character failed to load");
                    result.initialX = loaded.x;
                    result.initialY = loaded.y;
                    result.initialZ = loaded.z;
                    result.initialYawDegrees = loaded.yawDegrees;

                    var world = new GameObject("LGO M4 Smoke World").AddComponent<PlayableWorldController>();
                    world.Enter(loaded);
                    if (expectExisting)
                    {
                        Require(loaded.HasSamePosition(4.5f, 0.25f, -2.75f, 135.0f), "restart loaded position mismatch");
                    }
                    else
                    {
                        world.SetSmokePosition(4.5f, 0.25f, -2.75f, 135.0f);
                        var save = world.BuildSaveRequest();
                        var saved = await client.SaveCharacterPositionAsync(loaded.characterId, save.x, save.y, save.z, save.yawDegrees, token);
                        Require(saved.HasSamePosition(4.5f, 0.25f, -2.75f, 135.0f), "saved world position mismatch");
                    }

                    var after = await client.LoadCharacterAsync(loaded.characterId, token);
                    result.characterId = after.characterId;
                    result.characterName = after.name;
                    result.classId = after.classId;
                    result.entityId = after.entityId;
                    result.savedX = after.x;
                    result.savedY = after.y;
                    result.savedZ = after.z;
                    result.savedYawDegrees = after.yawDegrees;
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
                Debug.Log($"[LinhGioi] M4 playable vertical slice smoke status={result.status} result={resultPath}");
                Quit(exitCode);
            }
        }

        private static ClientRuntimeConfig BuildConfig()
        {
            var config = ClientRuntimeConfig.LoadStreamingAssets();
            var apiBaseUrl = GetArg("--lgo-m4-api-url");
            if (!string.IsNullOrWhiteSpace(apiBaseUrl)) config.apiBaseUrl = apiBaseUrl;
            config.apiTimeoutSeconds = GetIntArg("--lgo-m4-api-timeout-seconds", config.apiTimeoutSeconds);
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

        private static bool HasArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == key) return true;
            return false;
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

        private static void WriteResult(string path, M4PlayableVerticalSliceSmokeResult result)
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
        private sealed class M4PlayableVerticalSliceSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string accountId;
            public bool expectExisting;
            public bool loginCreated;
            public int initialCharacterCount;
            public int listedBeforeCreateCount;
            public bool reusedExistingCharacter;
            public string characterId;
            public string characterName;
            public string classId;
            public long entityId;
            public float initialX;
            public float initialY;
            public float initialZ;
            public float initialYawDegrees;
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
