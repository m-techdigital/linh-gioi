using UnityEngine;

namespace LinhGioi.Foundation
{
    public sealed class NpcAppearanceInstance : MonoBehaviour
    {
        public NpcAppearancePreset Appearance;
        private void Awake()
        {
            if (Appearance != null) Appearance.Apply(GetComponentInChildren<SkinnedMeshRenderer>());
        }
    }
}
