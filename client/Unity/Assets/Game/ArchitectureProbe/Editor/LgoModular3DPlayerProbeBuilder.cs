using System;
using System.IO;
using System.Linq;
using LinhGioi.ArchitectureProbe;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LinhGioi.ArchitectureProbe.Editor
{
    public static class LgoModular3DPlayerProbeBuilder
    {
        private const string TempRoot = "Assets/ArchitectureProbeTemp";
        private const string ModelPath = TempRoot + "/modular-3d-technical-probe.fbx";
        private const string ControllerPath = TempRoot + "/modular-3d-probe.controller";
        private const string ScenePath = TempRoot + "/modular-3d-probe.unity";

        [Serializable]
        private sealed class BuildEvidence
        {
            public string status;
            public string unityVersion;
            public string output;
            public ulong totalSizeBytes;
            public int errors;
            public int warnings;
            public int animationClipCount;
            public int skinnedMeshRendererCount;
            public bool runtimePromotionAllowed;
        }

        public static void Build()
        {
            AssetDatabase.ImportAsset(ModelPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            if (model == null) throw new FileNotFoundException("Temporary modular FBX is missing", ModelPath);
            var clips = AssetDatabase.LoadAllAssetsAtPath(ModelPath)
                .OfType<AnimationClip>()
                .Where(clip => !clip.name.StartsWith("__preview__", StringComparison.Ordinal))
                .ToArray();
            var required = new[] { "idle", "run", "jump", "attack", "return_to_idle" };
            foreach (var action in required)
                if (!clips.Any(clip => clip.name.IndexOf(action, StringComparison.OrdinalIgnoreCase) >= 0))
                    throw new InvalidOperationException("Missing animation clip: " + action);

            if (AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath) != null)
                AssetDatabase.DeleteAsset(ControllerPath);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            var stateMachine = controller.layers[0].stateMachine;
            foreach (var action in required)
            {
                var clip = clips.First(item => item.name.IndexOf(action, StringComparison.OrdinalIgnoreCase) >= 0);
                var state = stateMachine.AddState(action);
                state.motion = clip;
                if (action == "idle") stateMachine.defaultState = state;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var actor = PrefabUtility.InstantiatePrefab(model) as GameObject;
            if (actor == null) throw new InvalidOperationException("Could not instantiate modular FBX");
            actor.name = "Modular3DTechnicalActor";
            var animator = actor.GetComponent<Animator>();
            if (animator == null) animator = actor.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;

            var transforms = actor.GetComponentsInChildren<Transform>(true);
            var weapon = transforms.FirstOrDefault(item => item.name == "equipment__rigid_hand_item");
            var upper = transforms.FirstOrDefault(item => item.name == "equipment__upper");
            if (weapon == null || upper == null) throw new InvalidOperationException("Required equipment object missing");
            var runnerObject = new GameObject("Modular3DProbeRunner");
            runnerObject.AddComponent<LgoModular3DPlayerProbe>().Configure(animator, actor.transform, weapon, upper.gameObject);

            var renderers = actor.GetComponentsInChildren<Renderer>(true);
            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            var cameraObject = new GameObject("ProbeCamera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = Mathf.Max(1.5f, bounds.extents.y * 1.35f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.045f, 0.065f);
            camera.transform.position = bounds.center + new Vector3(0f, 0f, -8f);
            camera.transform.LookAt(bounds.center);
            camera.tag = "MainCamera";

            var lightObject = new GameObject("ProbeLight");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.4f;
            lightObject.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            var skinnedMeshRendererCount = actor.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length;
            var output = Environment.GetEnvironmentVariable("LGO_MODULAR3D_PLAYER_OUTPUT");
            if (string.IsNullOrWhiteSpace(output))
                output = Path.GetFullPath("build/character-model-architecture-review-01/modular-3d-player-probe/LgoModular3DProbe.app");
            var build = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = output,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.Development,
            });
            var summary = build.summary;
            var evidence = new BuildEvidence
            {
                status = summary.result == BuildResult.Succeeded ? "PASS" : "FAIL",
                unityVersion = Application.unityVersion,
                output = summary.outputPath,
                totalSizeBytes = summary.totalSize,
                errors = summary.totalErrors,
                warnings = summary.totalWarnings,
                animationClipCount = clips.Length,
                skinnedMeshRendererCount = skinnedMeshRendererCount,
                runtimePromotionAllowed = false,
            };
            var reportPath = Environment.GetEnvironmentVariable("LGO_MODULAR3D_BUILD_REPORT");
            if (!string.IsNullOrWhiteSpace(reportPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
                File.WriteAllText(reportPath, JsonUtility.ToJson(evidence, true) + Environment.NewLine);
            }
            if (summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Modular 3D Player probe build failed: " + summary.result);
        }
    }
}
