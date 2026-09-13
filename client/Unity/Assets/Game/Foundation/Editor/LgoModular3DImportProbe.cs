using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class LgoModular3DImportProbe
{
    [Serializable]
    private sealed class Report
    {
        public string status;
        public string unityVersion;
        public string assetPath;
        public int skinnedMeshRendererCount;
        public int animationClipCount;
        public string[] animationClips;
        public string[] equipmentObjects;
        public int sampledClipCount;
        public int midClipSampleCount;
        public int skinnedMeshBakeCount;
        public int equipmentToggleCount;
        public float rootScaleMaxDrift;
        public float rigidItemScaleMaxDrift;
        public string[] failures;
        public bool runtimePromotionAllowed;
    }

    public static void Run()
    {
        const string assetPath = "Assets/ArchitectureProbeTemp/modular-3d-technical-probe.fbx";
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        var root = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        var failures = new System.Collections.Generic.List<string>();
        if (root == null)
        {
            failures.Add("UNITY_MODEL_PREFAB_MISSING");
        }

        var clips = AssetDatabase.LoadAllAssetsAtPath(assetPath)
            .OfType<AnimationClip>()
            .Where(clip => !clip.name.StartsWith("__preview__", StringComparison.Ordinal))
            .Select(clip => clip.name)
            .OrderBy(name => name)
            .ToArray();
        var required = new[] { "attack", "idle", "jump", "return_to_idle", "run" };
        foreach (var action in required)
        {
            if (!clips.Any(name => name.IndexOf(action, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                failures.Add("UNITY_ANIMATION_MISSING_" + action.ToUpperInvariant());
            }
        }

        var renderers = root == null ? Array.Empty<SkinnedMeshRenderer>() : root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (renderers.Length < 2)
        {
            failures.Add("UNITY_SKINNED_GARMENT_RENDERERS_MISSING");
        }
        var equipment = root == null
            ? Array.Empty<string>()
            : root.GetComponentsInChildren<Transform>(true)
                .Select(item => item.name)
                .Where(name => name.StartsWith("equipment__", StringComparison.Ordinal))
                .Distinct()
                .OrderBy(name => name)
                .ToArray();
        var requiredSlots = new[] { "upper", "lower", "footwear", "waist", "rigid_hand_item" };
        foreach (var slot in requiredSlots)
        {
            if (!equipment.Any(name => name.IndexOf(slot, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                failures.Add("UNITY_EQUIPMENT_SLOT_MISSING_" + slot.ToUpperInvariant());
            }
        }

        var sampledClipCount = 0;
        var midClipSampleCount = 0;
        var skinnedMeshBakeCount = 0;
        var equipmentToggleCount = 0;
        var rootScaleMaxDrift = 0f;
        var rigidItemScaleMaxDrift = 0f;
        if (root != null)
        {
            var instance = UnityEngine.Object.Instantiate(root);
            try
            {
                var initialRootScale = instance.transform.localScale;
                var weapon = instance.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(item => item.name == "equipment__rigid_hand_item");
                var initialWeaponScale = weapon == null ? Vector3.one : weapon.lossyScale;
                foreach (var clip in AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<AnimationClip>()
                    .Where(clip => !clip.name.StartsWith("__preview__", StringComparison.Ordinal)))
                {
                    sampledClipCount++;
                    foreach (var normalizedTime in new[] { 0.25f, 0.5f, 0.75f })
                    {
                        clip.SampleAnimation(instance, clip.length * normalizedTime);
                        midClipSampleCount++;
                        rootScaleMaxDrift = Mathf.Max(rootScaleMaxDrift, Vector3.Distance(initialRootScale, instance.transform.localScale));
                        if (weapon != null)
                        {
                            rigidItemScaleMaxDrift = Mathf.Max(rigidItemScaleMaxDrift, Vector3.Distance(initialWeaponScale, weapon.lossyScale));
                        }
                        foreach (var renderer in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                        {
                            var baked = new Mesh();
                            renderer.BakeMesh(baked);
                            if (baked.vertexCount > 0)
                            {
                                skinnedMeshBakeCount++;
                            }
                            UnityEngine.Object.DestroyImmediate(baked);
                        }
                    }
                }
                foreach (var equipmentObject in instance.GetComponentsInChildren<Transform>(true)
                    .Where(item => item.name.StartsWith("equipment__", StringComparison.Ordinal)))
                {
                    equipmentObject.gameObject.SetActive(false);
                    equipmentObject.gameObject.SetActive(true);
                    equipmentToggleCount++;
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }
        if (sampledClipCount < required.Length || midClipSampleCount < required.Length * 3)
        {
            failures.Add("UNITY_MID_CLIP_SAMPLES_MISSING");
        }
        if (skinnedMeshBakeCount < renderers.Length * required.Length * 3)
        {
            failures.Add("UNITY_SKINNED_MESH_BAKE_FAILED");
        }
        if (rootScaleMaxDrift > 0.001f)
        {
            failures.Add("UNITY_ROOT_SCALE_DRIFT");
        }
        if (rigidItemScaleMaxDrift > 0.001f)
        {
            failures.Add("UNITY_RIGID_ITEM_SCALE_DRIFT");
        }

        var report = new Report
        {
            status = failures.Count == 0 ? "PASS" : "FAIL",
            unityVersion = Application.unityVersion,
            assetPath = assetPath,
            skinnedMeshRendererCount = renderers.Length,
            animationClipCount = clips.Length,
            animationClips = clips,
            equipmentObjects = equipment,
            sampledClipCount = sampledClipCount,
            midClipSampleCount = midClipSampleCount,
            skinnedMeshBakeCount = skinnedMeshBakeCount,
            equipmentToggleCount = equipmentToggleCount,
            rootScaleMaxDrift = rootScaleMaxDrift,
            rigidItemScaleMaxDrift = rigidItemScaleMaxDrift,
            failures = failures.ToArray(),
            runtimePromotionAllowed = false,
        };
        var output = Environment.GetEnvironmentVariable("LGO_MODULAR3D_REPORT_PATH");
        if (string.IsNullOrEmpty(output))
        {
            output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../build/character-model-architecture-review-01/modular-3d-toolchain-probe-v1/unity-import-report.json"));
        }
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        File.WriteAllText(output, JsonUtility.ToJson(report, true) + Environment.NewLine);
        AssetDatabase.SaveAssets();
        EditorApplication.Exit(failures.Count == 0 ? 0 : 2);
    }
}
