using System;
using System.IO;
using UnityEngine;

namespace LinhGioi.Combat
{
    public static class OfflineCombatSmokeRunner
    {
        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M1_OFFLINE_COMBAT_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m1-offline-combat-smoke") return true;
            return false;
        }

        public static void RunFromCommandLine()
        {
            var resultPath = GetArg("--lgo-m1-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m1-offline-combat-result.json");
            var exitCode = 99;
            var wrapper = new OfflineCombatSmokeResult
            {
                status = "STARTED",
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                resultPath = resultPath,
                startedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            };

            try
            {
                var manifestPath = GetArg("--lgo-gamedata-manifest") ?? Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "..", "gamedata", "compiled", "gamedata-manifest.json"));
                wrapper.gamedataManifestPath = manifestPath;
                var catalog = GameDataCombatCatalog.FromCompiledManifestJson(File.ReadAllText(manifestPath));
                wrapper.result = M1OfflineCombatScenario.RunDeterministicDuel(catalog);
                wrapper.status = wrapper.result.status;
                exitCode = wrapper.result.status == "PASS" ? 0 : 12;
            }
            catch (Exception exception)
            {
                wrapper.status = "FAIL";
                wrapper.exceptionType = exception.GetType().FullName;
                wrapper.exceptionMessage = exception.Message;
                exitCode = 14;
            }
            finally
            {
                wrapper.finishedAtUtc = DateTimeOffset.UtcNow.ToString("O");
                wrapper.exitCode = exitCode;
                WriteResult(resultPath, wrapper);
                Debug.Log($"[LinhGioi] M1 offline combat smoke status={wrapper.status} result={resultPath}");
                Quit(exitCode);
            }
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
        }

        private static void WriteResult(string path, OfflineCombatSmokeResult result)
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
        private sealed class OfflineCombatSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string gamedataManifestPath;
            public M1OfflineCombatRunResult result;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
