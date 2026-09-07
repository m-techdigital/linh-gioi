using UnityEngine;

namespace LinhGioi.Foundation.Editor
{
    // Editor-only assembly inputs. New NPCs may reuse this preset, or bake a different selection.
    public sealed class NpcAppearanceRecipe : ScriptableObject
    {
        public string[] Slots;
        public Mesh[] Parts;
        public Material[] Materials;
        public string[] BoneNames;
        public Bounds Bounds;
    }
}
