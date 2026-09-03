using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace LinhGioi.Foundation.Editor
{
    public static class M0LinuxPlayerEvidenceBuilder
    {
        public static void BuildLinuxPlayerSmoke()
        {
            BuildPlayerSmoke(BuildTarget.StandaloneLinux64, "LGO_LINUX_PLAYER_BUILD", "Builds/LinuxPlayer/LinhGioiM0PlayerSmoke.x86_64");
        }

        public static void BuildMacOSPlayerSmoke()
        {
            BuildPlayerSmoke(BuildTarget.StandaloneOSX, "LGO_MACOS_PLAYER_BUILD", "Builds/MacOSPlayer/LinhGioiM0PlayerSmoke.app");
        }

        public static void BuildHostPlayerSmoke()
        {
#if UNITY_EDITOR_OSX
            BuildMacOSPlayerSmoke();
#elif UNITY_EDITOR_LINUX
            BuildLinuxPlayerSmoke();
#else
            throw new PlatformNotSupportedException("Host player smoke build is only wired for macOS and Linux editors.");
#endif
        }

        private static void BuildPlayerSmoke(BuildTarget target, string marker, string defaultOutputPath)
        {
            var outputPath = GetOption("--lgo-player-output") ?? Environment.GetEnvironmentVariable("LGO_PLAYER_OUTPUT");
            if (string.IsNullOrWhiteSpace(outputPath)) outputPath = defaultOutputPath;

            outputPath = Path.GetFullPath(outputPath);
            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(outputDirectory)) Directory.CreateDirectory(outputDirectory);

            M0ProjectGenerator.EnsureGeneratedFoundation();
            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            if (scenes.Length == 0)
                throw new InvalidOperationException("No enabled Unity scenes are configured for player smoke build.");

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            var summaryLine = $"{marker} result={summary.result} target={target} output={summary.outputPath} totalSize={summary.totalSize} errors={summary.totalErrors} warnings={summary.totalWarnings}";
            UnityEngine.Debug.Log(summaryLine);
            Console.WriteLine(summaryLine);

            if (summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"Player build failed: target={target}; result={summary.result}; errors={summary.totalErrors}; warnings={summary.totalWarnings}");
            if (!PlayerOutputExists(outputPath, target))
                throw new FileNotFoundException($"Player build reported success but executable/app output was not found for target={target}.", outputPath);
        }

        private static bool PlayerOutputExists(string outputPath, BuildTarget target)
        {
            if (File.Exists(outputPath)) return true;
            if (target == BuildTarget.StandaloneOSX && Directory.Exists(outputPath)) return true;
            return false;
        }

        private static string GetOption(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == key) return args[i + 1];
            }
            return null;
        }
    }
}
