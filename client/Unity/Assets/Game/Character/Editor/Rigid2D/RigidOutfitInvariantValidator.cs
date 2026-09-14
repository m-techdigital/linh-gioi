using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinhGioi.Character.Editor
{
    public sealed class RigidOutfitInvariantReport
    {
        private readonly List<string> _issues = new();

        public bool Passed => _issues.Count == 0;
        public IReadOnlyList<string> Issues => _issues;
        public IReadOnlyList<string> IssueCodes => _issues
            .Select(issue => issue.Split('|')[0]).Distinct(StringComparer.Ordinal).ToArray();

        internal void Add(string code, string detail) => _issues.Add(code + "|" + detail);
        internal void Merge(RigidOutfitInvariantReport other) => _issues.AddRange(other._issues);
    }

    public static class RigidOutfitInvariantValidator
    {
        private static readonly string[] EquipmentPathTokens =
        {
            "equipment", "outfit", "weapon", "tunic", "sleeve", "boot", "waist", "glove", "cloth",
        };

        private static readonly string[] EquipmentSourceFiles =
        {
            "Assets/Game/Character/Runtime/Rigid2D/RigidOutfitModel.cs",
            "Assets/Game/Character/Runtime/Rigid2D/RigidOutfitBinder.cs",
            "Assets/Game/Character/Runtime/Rigid2D/RigidOutfitPilotCatalog.cs",
        };

        private static readonly string[] ForbiddenEquipmentLogicTokens =
        {
            "AnimatorStateInfo", "GetCurrentAnimatorStateInfo", "normalizedTime", "RigidMotionState",
            "frameIndex", "poseId", "elbowAngle", "kneeAngle",
        };

        public static RigidOutfitInvariantReport ValidateAnimationClips(IEnumerable<AnimationClip> clips)
        {
            var report = new RigidOutfitInvariantReport();
            foreach (var clip in clips ?? throw new ArgumentNullException(nameof(clips)))
            {
                if (clip == null) continue;
                if (ContainsToken(clip.name, "outfit") || ContainsToken(clip.name, "equipment"))
                    report.Add("OUTFIT_SPECIFIC_ANIMATION", clip.name);

                foreach (var binding in AnimationUtility.GetCurveBindings(clip))
                    ValidateBinding(report, clip, binding);
                foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                    ValidateBinding(report, clip, binding);
            }
            return report;
        }

        public static RigidOutfitInvariantReport ValidateHierarchy(GameObject root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            var report = new RigidOutfitInvariantReport();
            foreach (var component in root.GetComponentsInChildren<Component>(true).Where(component => component != null))
            {
                var type = component.GetType();
                if (type.Name == "SpriteSkin" || component is SkinnedMeshRenderer || component is MeshFilter)
                    report.Add("DEFORMING_RENDERER_COMPONENT", type.FullName + "@" + component.transform.name);
                if (component.transform.localScale != Vector3.one)
                    report.Add("NON_UNIT_LOCAL_SCALE", component.transform.name);
            }
            return report;
        }

        public static RigidOutfitInvariantReport ValidateEquipmentSources()
        {
            var report = new RigidOutfitInvariantReport();
            var projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            foreach (var relativePath in EquipmentSourceFiles)
            {
                var fullPath = Path.Combine(projectRoot, relativePath);
                if (!File.Exists(fullPath))
                {
                    report.Add("EQUIPMENT_SOURCE_MISSING", relativePath);
                    continue;
                }
                var text = File.ReadAllText(fullPath);
                foreach (var token in ForbiddenEquipmentLogicTokens.Where(text.Contains))
                    report.Add("POSE_DEPENDENT_EQUIPMENT_LOGIC", relativePath + ":" + token);
            }
            return report;
        }

        public static RigidOutfitInvariantReport ValidateProject()
        {
            var report = new RigidOutfitInvariantReport();
            var clips = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Game/Character" })
                .Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<AnimationClip>);
            report.Merge(ValidateAnimationClips(clips));
            report.Merge(ValidateEquipmentSources());

            var catalog = RigidOutfitPilotCatalog.LoadFromResources();
            foreach (var character in new[] { catalog.Male, catalog.Female })
            {
                var root = new GameObject("Invariant-" + character.Gender);
                try
                {
                    RigidOutfitPilotActor.Create(root.transform, character);
                    report.Merge(ValidateHierarchy(root));
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }
            return report;
        }

        public static void AssertProjectGate()
        {
            var report = ValidateProject();
            WriteReport(report);
            if (!report.Passed)
                throw new InvalidOperationException("Rigid outfit invariant gate failed:\n" + string.Join("\n", report.Issues));
        }

        public static void RunProjectGate()
        {
            AssertProjectGate();
            Debug.Log("LGO_RIGID_OUTFIT_INVARIANTS_PASS");
        }

        private static void ValidateBinding(RigidOutfitInvariantReport report, AnimationClip clip, EditorCurveBinding binding)
        {
            if (binding.propertyName.IndexOf("m_LocalScale", StringComparison.OrdinalIgnoreCase) >= 0 ||
                binding.propertyName.IndexOf("localScale", StringComparison.OrdinalIgnoreCase) >= 0)
                report.Add("ANIMATED_SCALE", clip.name + ":" + binding.path + ":" + binding.propertyName);
            if (binding.type == typeof(SpriteRenderer) && binding.propertyName.IndexOf("Sprite", StringComparison.OrdinalIgnoreCase) >= 0)
                report.Add("ANIMATED_SPRITE", clip.name + ":" + binding.path + ":" + binding.propertyName);
            if (EquipmentPathTokens.Any(token => ContainsPathSegment(binding.path, token)))
                report.Add("ANIMATION_BINDS_EQUIPMENT", clip.name + ":" + binding.path);
        }

        private static bool ContainsPathSegment(string path, string token) =>
            (path ?? string.Empty).Split('/').Any(segment =>
                segment.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);

        private static bool ContainsToken(string value, string token) =>
            (value ?? string.Empty).IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;

        private static void WriteReport(RigidOutfitInvariantReport report)
        {
            var output = Environment.GetEnvironmentVariable("LGO_RIGID_PILOT_INVARIANT_REPORT");
            if (string.IsNullOrWhiteSpace(output)) return;
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            var json = JsonUtility.ToJson(new SerializableReport
            {
                status = report.Passed ? "PASS" : "FIX_REQUIRED",
                issueCodes = report.IssueCodes.ToArray(),
                issues = report.Issues.ToArray(),
                rules = new[]
                {
                    "NO_SPRITESKIN_OR_MESH", "NO_ANIMATED_SPRITE", "NO_ANIMATED_SCALE",
                    "NO_ANIMATION_EQUIPMENT_BINDING", "NO_POSE_DEPENDENT_EQUIPMENT_LOGIC",
                },
            }, true);
            File.WriteAllText(output, json + Environment.NewLine);
        }

        [Serializable]
        private sealed class SerializableReport
        {
            public string status;
            public string[] issueCodes;
            public string[] issues;
            public string[] rules;
        }
    }
}
