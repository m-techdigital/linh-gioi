using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace LinhGioi.Foundation
{
    [Serializable]
    public sealed class RuntimeFrameTimingSnapshot
    {
        public int sampleCount;
        public float averageFrameMs;
        public float maxFrameMs;
    }

    [Serializable]
    public sealed class RuntimeTextureMemoryAuditResult
    {
        public string status;
        public string evidenceAuthority;
        public string platform;
        public int screenWidth;
        public int screenHeight;
        public int systemMemoryMb;
        public int graphicsMemoryMb;
        public string graphicsDevice;
        public int targetFrameRate;
        public int vSyncCount;
        public RuntimeTextureMemorySnapshot initialLoadedTextures;
        public RuntimeTextureMemorySnapshot allResourcesTextures;
        public RuntimeFrameTimingSnapshot initialFrameTiming;
        public RuntimeFrameTimingSnapshot allResourcesFrameTiming;
    }

    public static class RuntimeTextureMemoryAuditRunner
    {
        private const string Flag = "--lgo-runtime-texture-memory-audit";
        private const string ResultArg = "--lgo-runtime-texture-memory-result";

        public static bool ShouldRunForArgs(string[] args)
        {
            return args != null && Array.IndexOf(args, Flag) >= 0;
        }

        public static bool ShouldRun()
        {
            return ShouldRunForArgs(Environment.GetCommandLineArgs());
        }

        public static void RunFromCommandLine()
        {
            var host = new GameObject("LGO Runtime Texture Memory Audit");
            UnityEngine.Object.DontDestroyOnLoad(host);
            host.AddComponent<RuntimeTextureMemoryAuditBehaviour>();
        }

        internal static string ResultPathForArgs(string[] args)
        {
            if (args != null)
            {
                var index = Array.IndexOf(args, ResultArg);
                if (index >= 0 && index + 1 < args.Length && !string.IsNullOrWhiteSpace(args[index + 1]))
                    return args[index + 1];
            }
            return Path.Combine(Application.persistentDataPath, "lgo-runtime-texture-memory-audit.json");
        }
    }

    internal sealed class RuntimeTextureMemoryAuditBehaviour : MonoBehaviour
    {
        private IEnumerator Start()
        {
            Application.runInBackground = true;
            var args = Environment.GetCommandLineArgs();
            var result = new RuntimeTextureMemoryAuditResult
            {
                status = "RUNNING",
                evidenceAuthority = Application.isMobilePlatform ? "device-runtime" : "macos-player",
                platform = Application.platform.ToString(),
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                systemMemoryMb = SystemInfo.systemMemorySize,
                graphicsMemoryMb = SystemInfo.graphicsMemorySize,
                graphicsDevice = SystemInfo.graphicsDeviceName,
                targetFrameRate = Application.targetFrameRate,
                vSyncCount = QualitySettings.vSyncCount,
            };

            yield return SampleFrames(60, timing => result.initialFrameTiming = timing);
            var residentIds = RuntimeTextureMemoryAudit.CaptureLoadedTextureInstanceIds();
            var initial = Resources.FindObjectsOfTypeAll<Texture2D>();
            result.initialLoadedTextures = RuntimeTextureMemoryAudit.Summarize(initial, residentIds);

            var allResources = Resources.LoadAll<Texture2D>("");
            yield return null;
            yield return new WaitForEndOfFrame();
            result.allResourcesTextures = RuntimeTextureMemoryAudit.Summarize(allResources, residentIds);
            yield return SampleFrames(60, timing => result.allResourcesFrameTiming = timing);

            result.status = "PASS";
            var path = RuntimeTextureMemoryAuditRunner.ResultPathForArgs(args);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(path, JsonUtility.ToJson(result, true));
            Debug.Log($"LGO_RUNTIME_TEXTURE_MEMORY_AUDIT_PASS result={path} textures={result.allResourcesTextures.textureCount} estimatedBytes={result.allResourcesTextures.totalEstimatedStorageBytes} profilerBytes={result.allResourcesTextures.totalProfilerRuntimeBytes}");
            Application.Quit(0);
        }

        private static IEnumerator SampleFrames(int sampleCount, Action<RuntimeFrameTimingSnapshot> done)
        {
            var total = 0f;
            var maximum = 0f;
            for (var index = 0; index < sampleCount; index++)
            {
                yield return null;
                var frameMs = Mathf.Max(0f, Time.unscaledDeltaTime * 1000f);
                total += frameMs;
                maximum = Mathf.Max(maximum, frameMs);
            }
            done(new RuntimeFrameTimingSnapshot
            {
                sampleCount = sampleCount,
                averageFrameMs = sampleCount <= 0 ? 0f : total / sampleCount,
                maxFrameMs = maximum,
            });
        }
    }
}
