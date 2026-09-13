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

        var report = new Report
        {
            status = failures.Count == 0 ? "PASS" : "FAIL",
            unityVersion = Application.unityVersion,
            assetPath = assetPath,
            skinnedMeshRendererCount = renderers.Length,
            animationClipCount = clips.Length,
            animationClips = clips,
            equipmentObjects = equipment,
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
