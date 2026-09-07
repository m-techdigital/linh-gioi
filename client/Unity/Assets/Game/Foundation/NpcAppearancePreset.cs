using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.Foundation
{
    // A baked appearance is shared by every NPC using it. Authoring parts stay in Editor assets.
    [CreateAssetMenu(menuName = "Linh Gioi/NPC Appearance")]
    public sealed class NpcAppearancePreset : ScriptableObject
    {
        public Mesh Mesh;
        public Material[] Materials;
        public string[] BoneNames;
        public Bounds Bounds;

        public void Apply(SkinnedMeshRenderer target)
        {
            if (target == null || Mesh == null || Materials == null || BoneNames == null ||
                Materials.Length != Mesh.subMeshCount || BoneNames.Length != Mesh.bindposeCount)
                throw new InvalidOperationException("Incomplete NPC appearance or incompatible mesh layout.");
            foreach (var material in Materials)
                if (material == null) throw new InvalidOperationException("NPC appearance has a missing material.");
            var available = new Dictionary<string, Transform>(StringComparer.Ordinal);
            foreach (var bone in target.bones)
                if (bone != null && !available.TryAdd(bone.name, bone))
                    throw new InvalidOperationException("Ambiguous NPC bone: " + bone.name);
            var mapped = new Transform[BoneNames.Length];
            for (var i = 0; i < mapped.Length; i++)
                if (!available.TryGetValue(BoneNames[i], out mapped[i]))
                    throw new InvalidOperationException("NPC rig is missing bone: " + BoneNames[i]);
            // Validate first, then apply atomically; never instantiate per-NPC meshes/materials.
            target.bones = mapped;
            target.sharedMesh = Mesh;
            target.sharedMaterials = Materials;
            target.localBounds = Bounds;
        }
    }
}
