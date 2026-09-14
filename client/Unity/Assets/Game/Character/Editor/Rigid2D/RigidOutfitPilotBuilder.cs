using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LinhGioi.Character.Editor
{
    public static class RigidOutfitPilotBuilder
    {
        private static readonly string[] TextureRoots =
        {
            "Assets/Game/Character/Runtime/Resources/LGORigidPilot/v1",
            "Assets/Game/Character/Runtime/Resources/LGORigidPilot/rigid-source-v2",
        };
        private const string GeneratedRoot = "Assets/Game/Generated/RigidOutfitPilot";
        private const string ScenePath = GeneratedRoot + "/RigidOutfitPilot.unity";

        public static void BuildMacOS()
        {
            ConfigureSourceTextures();
            RigidOutfitInvariantValidator.AssertProjectGate();
            Directory.CreateDirectory(Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, GeneratedRoot));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            new GameObject("LGO Rigid Outfit Pilot").AddComponent<RigidOutfitPilotPlayer>();
            EditorSceneManager.SaveScene(scene, ScenePath);

            var playerPath = RequireEnvironment("LGO_RIGID_PILOT_PLAYER_PATH");
            Directory.CreateDirectory(Path.GetDirectoryName(playerPath)!);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = playerPath,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.Development,
            });
            var output = Environment.GetEnvironmentVariable("LGO_RIGID_PILOT_BUILD_REPORT");
            if (!string.IsNullOrWhiteSpace(output))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(output)!);
                File.WriteAllText(output, JsonUtility.ToJson(new BuildEvidence
                {
                    status = report.summary.result == BuildResult.Succeeded ? "PASS" : "FIX_REQUIRED",
                    unityVersion = Application.unityVersion,
                    totalErrors = report.summary.totalErrors,
                    totalWarnings = report.summary.totalWarnings,
                    totalSizeBytes = report.summary.totalSize,
                    outputPath = report.summary.outputPath,
                    textureCount = AssetDatabase.FindAssets("t:Texture2D", TextureRoots).Length,
                    pixelsPerUnit = RigidOutfitPilotCatalog.PixelsPerUnit,
                }, true) + Environment.NewLine);
            }
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Rigid outfit pilot build failed: " + report.summary.result);
        }

        public static void ConfigureSourceTextures()
        {
            var textureGuids = AssetDatabase.FindAssets("t:Texture2D", TextureRoots);
            if (textureGuids.Length < 41) throw new InvalidOperationException("Rigid pilot source texture set is incomplete");
            foreach (var path in textureGuids.Select(AssetDatabase.GUIDToAssetPath))
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = RigidOutfitPilotCatalog.PixelsPerUnit;
                importer.spritePivot = new Vector2(.5f, .5f);
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 512;
                importer.SaveAndReimport();
            }
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Pilot Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, .75f, -10f);
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 3.45f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.025f, .12f, .15f);
        }

        private static string RequireEnvironment(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException("Missing environment variable " + name);
            return Path.GetFullPath(value);
        }

        [Serializable]
        private sealed class BuildEvidence
        {
            public string status;
            public string unityVersion;
            public int totalErrors;
            public int totalWarnings;
            public ulong totalSizeBytes;
            public string outputPath;
            public int textureCount;
            public float pixelsPerUnit;
        }
    }
}
