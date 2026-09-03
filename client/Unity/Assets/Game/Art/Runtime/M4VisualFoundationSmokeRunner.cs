using System;
using System.IO;
using LinhGioi.Foundation;
using UnityEngine;

namespace LinhGioi.Art
{
    public static class M4VisualFoundationSmokeRunner
    {
        public static bool ShouldRun()
        {
            if (string.Equals(Environment.GetEnvironmentVariable("LGO_M4_VISUAL_FOUNDATION_SMOKE"), "1", StringComparison.Ordinal)) return true;
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--lgo-m4-visual-foundation-smoke") return true;
            return false;
        }

        public static void RunFromCommandLine()
        {
            var resultPath = GetArg("--lgo-m4-visual-result") ?? Path.Combine(Application.persistentDataPath, "lgo-m4-visual-foundation-result.json");
            var result = new M4VisualFoundationSmokeResult
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
                Require(RuntimeArtCatalog.Version == "0.10.0", "runtime art catalog version mismatch");
                Require(!string.IsNullOrWhiteSpace(RuntimeArtCatalog.HeroPlaceholder), "hero placeholder path missing");
                Require(RuntimeArtCatalog.Spirit.a > 0.99f, "spirit color alpha invalid");
                var material = RuntimeArtCatalog.CreateMaterial("LGO Visual Smoke Material", RuntimeArtCatalog.Spirit);
                Require(material != null, "material creation failed");
                result.heroPlaceholder = RuntimeArtCatalog.HeroPlaceholder;
                result.trainingGroundTile = RuntimeArtCatalog.TrainingGroundTile;
                result.status = "PASS";
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
                result.finishedAtUtc = DateTimeOffset.UtcNow.ToString("O");
                result.exitCode = exitCode;
                WriteResult(resultPath, result);
                Debug.Log($"[LinhGioi] M4 visual foundation smoke status={result.status} result={resultPath}");
                Quit(exitCode);
            }
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

        private static void WriteResult(string path, M4VisualFoundationSmokeResult result)
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
        private sealed class M4VisualFoundationSmokeResult
        {
            public string status;
            public string startedAtUtc;
            public string finishedAtUtc;
            public string unityVersion;
            public string platform;
            public string resultPath;
            public string heroPlaceholder;
            public string trainingGroundTile;
            public string exceptionType;
            public string exceptionMessage;
            public int exitCode;
        }
    }
}
