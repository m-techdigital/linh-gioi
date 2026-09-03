using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Foundation;
using UnityEngine;

namespace LinhGioi.Account
{
    public static class M3BAccountCharacterSmokeRunner
    {
        private const int DefaultTimeoutMs = 20000;
        private const string DefaultCharacterName = "M3BHero";
        private const string DefaultClassId = "class.sword";
        private const string DefaultDevKey = "m3b-unity-dev-key";

        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M3B_ACCOUNT_CHARACTER_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m3b-account-character-smoke") return true;
            return false;
        }

        public static async Task RunFromCommandLineAsync(CancellationToken shutdownToken)
        {
            var resultPath = GetArg("--lgo-m3b-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m3b-account-character-result.json");
            var exitCode = 99;
            var result = new M3BAccountCharacterSmokeResult
            {
                status = "STARTED",
                startedAtUtc = DateTimeOffset.UtcNow.ToString("O"),
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                resultPath = resultPath
            };

            try
            {
                var timeoutMs = GetIntArg("--lgo-m3b-timeout-ms", DefaultTimeoutMs);
                var config = BuildConfig();
                using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(shutdownToken))
                using (var client = new AccountApiClient(config))
                {
                    timeout.CancelAfter(timeoutMs);
                    var token = timeout.Token;
                    var devKey = GetArg("--lgo-m3b-dev-key") ?? DefaultDevKey;
                    var displayName = GetArg("--lgo-m3b-display-name") ?? "M3B Unity Smoke";
                    var characterName = GetArg("--lgo-m3b-character-name") ?? DefaultCharacterName;
                    var classId = GetArg("--lgo-m3b-class-id") ?? DefaultClassId;
                    var expectExisting = HasArg("--lgo-m3b-expect-existing");
                    result.expectExisting = expectExisting;

                    var login = await client.LoginDevAsync(devKey, displayName, token);
                    Require(login != null && login.account != null, "dev login returned no account");
                    Require(StartsWith(login.account.accountId, "account.dev."), "dev login returned unexpected account id");

                    result.apiBaseUrl = config.apiBaseUrl;
                    result.accountId = login.account.accountId;
                    result.loginCreated = login.created;
                    if (expectExisting && login.created) throw new InvalidOperationException("expected persisted dev account to exist after API restart");
                    result.initialCharacterCount = login.characters == null ? 0 : login.characters.Length;

                    var listed = await client.ListCharactersAsync(result.accountId, token);
                    result.listedBeforeCreateCount = listed.Length;
                    var character = FindByName(listed, characterName);
                    result.reusedExistingCharacter = character != null;
                    if (expectExisting && character == null) throw new InvalidOperationException("expected persisted character to exist after API restart");
                    if (character == null)
                    {
                        character = await client.CreateCharacterAsync(result.accountId, characterName, classId, token);
                    }
                    Require(character != null, "character create/list returned null");
                    Require(StartsWith(character.characterId, "character."), "unexpected character id");
                    Require(character.accountId == result.accountId, "character account id does not match login account");
                    Require(character.name == characterName, "character name mismatch");
                    Require(character.classId == classId, "character class mismatch");

                    result.characterId = character.characterId;
                    result.characterName = character.name;
                    result.classId = character.classId;
                    result.entityId = character.entityId;

                    var saved = await client.SaveCharacterPositionAsync(character.characterId, 3.25f, 0.5f, -7.75f, 270.0f, token);
                    Require(saved.HasSamePosition(3.25f, 0.5f, -7.75f, 270.0f), "saved position mismatch");
                    var loaded = await client.LoadCharacterAsync(character.characterId, token);
                    Require(loaded.HasSamePosition(3.25f, 0.5f, -7.75f, 270.0f), "loaded position mismatch");
                    Require(loaded.entityId == character.entityId, "loaded entity id changed");

                    var after = await client.ListCharactersAsync(result.accountId, token);
                    Require(FindById(after, character.characterId) != null, "saved character missing from account list");
                    result.listedAfterCreateCount = after.Length;
                    result.x = loaded.x;
                    result.y = loaded.y;
                    result.z = loaded.z;
                    result.yawDegrees = loaded.yawDegrees;
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
                Debug.Log($"[LinhGioi] M3B account/character smoke status={result.status} result={resultPath}");
                Quit(exitCode);
            }
        }

        private static ClientRuntimeConfig BuildConfig()
        {
            var config = ClientRuntimeConfig.LoadStreamingAssets();
            var apiBaseUrl = GetArg("--lgo-m3b-api-url");
            if (!string.IsNullOrWhiteSpace(apiBaseUrl)) config.apiBaseUrl = apiBaseUrl;
            config.apiTimeoutSeconds = GetIntArg("--lgo-m3b-api-timeout-seconds", config.apiTimeoutSeconds);
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

        private static CharacterResponse FindById(CharacterResponse[] characters, string characterId)
        {
            if (characters == null) return null;
            foreach (var character in characters)
                if (character != null && string.Equals(character.characterId, characterId, StringComparison.Ordinal)) return character;
            return null;
        }

        private static bool StartsWith(string value, string prefix)
        {
            return !string.IsNullOrWhiteSpace(value) && value.StartsWith(prefix, StringComparison.Ordinal);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
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

        private static void WriteResult(string path, M3BAccountCharacterSmokeResult result)
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
        private sealed class M3BAccountCharacterSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string apiBaseUrl;
            public string accountId;
            public bool expectExisting;
            public bool loginCreated;
            public int initialCharacterCount;
            public int listedBeforeCreateCount;
            public bool reusedExistingCharacter;
            public int listedAfterCreateCount;
            public string characterId;
            public string characterName;
            public string classId;
            public long entityId;
            public float x;
            public float y;
            public float z;
            public float yawDegrees;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
