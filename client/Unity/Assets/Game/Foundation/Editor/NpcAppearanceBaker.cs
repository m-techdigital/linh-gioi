using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinhGioi.Foundation.Editor
{
    public static class NpcAppearanceBaker
    {
        private const string Root = "Assets/Game/Art/OnboardingCandidate/";

        public static void BakeKeeper()
        {
            ArrivalOutfitImporter.PrepareKeeperModel();
            BakeModular(Root + "GateKeeper.fbx", Root + "Resources/LGOGateKeeperCandidate.prefab", "Keeper");
        }

        private static void BakeModular(string modelPath, string prefabPath, string stem)
        {
            var partsRoot = Root + "Editor/" + stem + "Parts/";
            EnsureFolder(Root + "Editor"); EnsureFolder(partsRoot.TrimEnd('/'));
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(modelPath));
            try
            {
                PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                var renderer = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                var original = renderer.sharedMesh;
                var recipe = ScriptableObject.CreateInstance<NpcAppearanceRecipe>();
                var sourceSections = renderer.sharedMaterials.Select(material => ParseSection(material.name)).ToArray();
                recipe.Slots = sourceSections.Select(section => section.slot).Distinct().ToArray();
                recipe.Materials = new[] { AssetDatabase.LoadAssetAtPath<Material>(Root + "Keeper_Reconstruction.mat"),
                    AssetDatabase.LoadAssetAtPath<Material>(Root + (stem == "Arrival" ? "Arrival_Face.mat" : "Keeper_Reconstruction_Face.mat")) };
                if (recipe.Materials.Any(material => material == null))
                    throw new InvalidOperationException("Appearance requires both body and face materials.");
                recipe.BoneNames = renderer.bones.Select(b => b.name).ToArray();
                recipe.Bounds = renderer.localBounds;
                var selected = new List<int>[recipe.Slots.Length, 2];
                for (var slot = 0; slot < recipe.Slots.Length; slot++)
                    for (var sub = 0; sub < 2; sub++) selected[slot, sub] = new List<int>();
                for (var sub = 0; sub < original.subMeshCount; sub++)
                {
                    var section = sourceSections[sub];
                    selected[Array.IndexOf(recipe.Slots, section.slot), section.surface].AddRange(original.GetTriangles(sub));
                }
                recipe.Parts = new Mesh[recipe.Slots.Length];
                for (var slot = 0; slot < recipe.Slots.Length; slot++)
                {
                    var sections = Enumerable.Range(0, 2).Select(sub => selected[slot, sub].ToArray()).ToArray();
                    recipe.Parts[slot] = Save(Extract(original, sections, recipe.Slots[slot]), partsRoot + recipe.Slots[slot] + ".asset");
                    var expectedPoints = sections.SelectMany(section => section).Select(index => original.vertices[index]).Distinct().ToArray();
                    var actualPoints = recipe.Parts[slot].vertices.Distinct().ToArray();
                    if (expectedPoints.Except(actualPoints).Any() || actualPoints.Except(expectedPoints).Any())
                        throw new InvalidOperationException("NPC part did not preserve its tagged source vertices: " + recipe.Slots[slot]);

                }
                recipe = Save(recipe, partsRoot + stem + "Recipe.asset");
                var merged = Merge(recipe);
                if (merged.GetIndexCount(0) + merged.GetIndexCount(1) != Enumerable.Range(0, original.subMeshCount).Aggregate(0u, (sum, sub) => sum + original.GetIndexCount(sub)))
                    throw new InvalidOperationException("NPC bake lost geometry.");
                var preset = ScriptableObject.CreateInstance<NpcAppearancePreset>();
                MeshUtility.Optimize(merged);
                MeshUtility.SetMeshCompression(merged, ModelImporterMeshCompression.Off);
                preset.Mesh = Save(merged, Root + stem + "AppearanceMesh.asset");
                preset.Materials = recipe.Materials; preset.BoneNames = recipe.BoneNames; preset.Bounds = recipe.Bounds;
                preset = Save(preset, Root + stem + "Appearance.asset");
                preset.Apply(renderer);
                var binding = instance.GetComponent<NpcAppearanceInstance>();
                if (binding == null) binding = instance.AddComponent<NpcAppearanceInstance>();
                binding.Appearance = preset;
                // Reuse the canonical rig Avatar; wardrobe geometry never defines a new reference pose.
                var animator = instance.GetComponent<Animator>();
                if (animator == null) animator = instance.AddComponent<Animator>();
                animator.avatar = AssetDatabase.LoadAssetAtPath<Avatar>(Root + "SharedHumanoidAvatar.asset");
                animator.applyRootMotion = false;
                animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Root + "ArrivalLocomotion.controller");
                if (animator.runtimeAnimatorController == null || !animator.isHuman)
                    throw new InvalidOperationException("NPC bake requires the shared Humanoid locomotion controller.");
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                AssetDatabase.SaveAssets();
                var dependencies = AssetDatabase.GetDependencies(prefabPath, true);
                if (dependencies.Any(path => path.StartsWith(partsRoot, StringComparison.Ordinal) || path == modelPath))
                    throw new InvalidOperationException("Authoring meshes leaked into runtime NPC dependencies: " + string.Join(", ", dependencies.Where(path => path.StartsWith(partsRoot, StringComparison.Ordinal) || path == modelPath)));
                Debug.Log("LGO_NPC_APPEARANCE_BAKE_PASS actor=" + stem + " slots=" + recipe.Slots.Length + " runtime_renderers=1 materials=2 shared_preset=true authoring_dependencies=false vertices=" + preset.Mesh.vertexCount);
                foreach (var material in preset.Materials)
                {
                    var texture = material.GetTexture("_BaseMap");
                    Debug.Log("LGO_NPC_TEXTURE_MEMORY name=" + texture.name + " width=" + texture.width + " height=" + texture.height + " bytes=" + UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(texture));
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }

        internal static (string slot, int surface) ParseSection(string name)
        {
            // FBX section names carry explicit ownership; bones and position cannot identify clothing.
            var tokens = name.Split('_');
            if (tokens.Length != 3 || tokens[0] != "LGO" || string.IsNullOrEmpty(tokens[1]) ||
                (tokens[2] != "Body" && tokens[2] != "Face"))
                throw new InvalidOperationException("Missing wardrobe slot on source section: " + name);
            return (tokens[1], tokens[2] == "Face" ? 1 : 0);
        }

        internal static void BakePlayer(string modelPath, string prefabPath)
            => BakeModular(modelPath, prefabPath, "Arrival");

        public static Mesh Merge(NpcAppearanceRecipe recipe)
        {
            var vertices = new List<Vector3>(); var normals = new List<Vector3>(); var tangents = new List<Vector4>();
            var uv = new List<Vector2>(); var skin = new List<BoneWeight>();
            var sections = recipe.Materials.Select(_ => new List<int>()).ToArray();
            foreach (var part in recipe.Parts)
            {
                if (part == null || part.subMeshCount != sections.Length || part.bindposeCount != recipe.BoneNames.Length)
                    throw new InvalidOperationException("NPC module has an incompatible skeleton or material layout.");
                if (!part.bindposes.SequenceEqual(recipe.Parts[0].bindposes))
                    throw new InvalidOperationException("NPC modules must use the same bind pose.");
                var offset = vertices.Count;
                vertices.AddRange(part.vertices); normals.AddRange(part.normals); tangents.AddRange(part.tangents);
                uv.AddRange(part.uv); skin.AddRange(part.boneWeights);
                for (var sub = 0; sub < sections.Length; sub++) sections[sub].AddRange(part.GetTriangles(sub).Select(i => i + offset));
            }
            var mesh = new Mesh { name = "Shared assembled appearance", indexFormat = vertices.Count > 65535 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16 };
            mesh.SetVertices(vertices); mesh.SetNormals(normals); mesh.SetTangents(tangents); mesh.SetUVs(0, uv);
            mesh.boneWeights = skin.ToArray(); mesh.bindposes = recipe.Parts[0].bindposes;
            mesh.subMeshCount = sections.Length;
            for (var sub = 0; sub < sections.Length; sub++) mesh.SetTriangles(sections[sub], sub);
            mesh.bounds = recipe.Bounds;
            return mesh;
        }

        private static Mesh Extract(Mesh source, int[][] sections, string name)
        {
            var used = sections.SelectMany(s => s).Distinct().OrderBy(i => i).ToArray();
            if (used.Length == 0) throw new InvalidOperationException("Empty NPC module: " + name);
            var lookup = used.Select((old, index) => (old, index)).ToDictionary(p => p.old, p => p.index);
            var v = source.vertices; var n = source.normals; var t = source.tangents; var uv = source.uv; var w = source.boneWeights;
            var mesh = new Mesh { name = name, vertices = used.Select(i => v[i]).ToArray(), normals = used.Select(i => n[i]).ToArray(),
                tangents = used.Select(i => t[i]).ToArray(), uv = used.Select(i => uv[i]).ToArray(), boneWeights = used.Select(i => w[i]).ToArray(),
                bindposes = source.bindposes, subMeshCount = sections.Length };
            for (var sub = 0; sub < sections.Length; sub++) mesh.SetTriangles(sections[sub].Select(i => lookup[i]).ToArray(), sub);
            mesh.bounds = source.bounds;
            return mesh;
        }
        private static T Save<T>(T value, string path) where T : UnityEngine.Object
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing == null) { AssetDatabase.CreateAsset(value, path); return value; }
            if (value is Mesh source && existing is Mesh target)
            {
                // CopySerialized alone leaves stale native vertex buffers on existing Mesh assets.
                target.Clear(); target.indexFormat = source.indexFormat;
                target.vertices = source.vertices; target.normals = source.normals;
                target.tangents = source.tangents; target.uv = source.uv;
                target.bindposes = source.bindposes; target.boneWeights = source.boneWeights;
                target.subMeshCount = source.subMeshCount;
                for (var sub = 0; sub < source.subMeshCount; sub++) target.SetTriangles(source.GetTriangles(sub), sub);
                target.bounds = source.bounds; target.name = source.name;
                MeshUtility.SetMeshCompression(target, MeshUtility.GetMeshCompression(source));
            }
            else EditorUtility.CopySerialized(value, existing);
            UnityEngine.Object.DestroyImmediate(value); EditorUtility.SetDirty(existing); return existing;
        }
        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(System.IO.Path.GetDirectoryName(path).Replace('\\', '/'), System.IO.Path.GetFileName(path));
        }
    }
}
