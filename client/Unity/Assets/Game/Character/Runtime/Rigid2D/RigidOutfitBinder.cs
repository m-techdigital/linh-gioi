using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LinhGioi.Character
{
    public sealed class RigidEquippedBundle
    {
        internal RigidEquippedBundle(string slotId, string bundleId, string fitFingerprint,
            IReadOnlyList<SpriteRenderer> renderers)
        {
            SlotId = slotId;
            BundleId = bundleId;
            FitFingerprint = fitFingerprint;
            Renderers = renderers;
        }

        public string SlotId { get; }
        public string BundleId { get; }
        public string FitFingerprint { get; }
        public IReadOnlyList<SpriteRenderer> Renderers { get; }
        public string SpriteFingerprint => RigidFitProfile.Sha256(string.Join("\n", Renderers
            .OrderBy(renderer => renderer.name, StringComparer.Ordinal)
            .Select(renderer => renderer.name + "|" + (renderer.sprite == null ? 0 : renderer.sprite.GetInstanceID()))));
    }

    public sealed class RigidOutfitBinder
    {
        private readonly RigidCharacterSkeleton _skeleton;
        private readonly Dictionary<string, RigidEquippedBundle> _slots = new(StringComparer.Ordinal);

        public RigidOutfitBinder(RigidCharacterSkeleton skeleton)
        {
            _skeleton = skeleton ?? throw new ArgumentNullException(nameof(skeleton));
        }

        public RigidEquippedBundle Equip(string slotId, RigidVisualBundle bundle, RigidFitProfile fitProfile)
        {
            if (string.IsNullOrWhiteSpace(slotId)) throw new ArgumentException("slotId is required", nameof(slotId));
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (fitProfile == null) throw new ArgumentNullException(nameof(fitProfile));
            var fitFingerprint = fitProfile.Fingerprint;
            var fits = bundle.Parts.ToDictionary(part => part.PartId, part => fitProfile.Require(part.PartId), StringComparer.Ordinal);

            Unequip(slotId);
            var renderers = new List<SpriteRenderer>(bundle.Parts.Count);
            foreach (var part in bundle.Parts)
            {
                var fit = fits[part.PartId];
                var visual = new GameObject(part.PartId);
                visual.transform.SetParent(_skeleton[part.TargetBone], false);
                visual.transform.localPosition = fit.LocalPosition;
                visual.transform.localRotation = Quaternion.Euler(0f, 0f, fit.LocalRotationDegrees);
                visual.transform.localScale = Vector3.one;
                var renderer = visual.AddComponent<SpriteRenderer>();
                renderer.sprite = part.Sprite;
                renderer.sortingOrder = part.SortingOrder;
                renderers.Add(renderer);
            }

            var equipped = new RigidEquippedBundle(slotId, bundle.BundleId, fitFingerprint, renderers);
            _slots.Add(slotId, equipped);
            return equipped;
        }

        public void Unequip(string slotId)
        {
            if (!_slots.Remove(slotId, out var equipped)) return;
            foreach (var renderer in equipped.Renderers.Where(renderer => renderer != null))
            {
                if (Application.isPlaying) UnityEngine.Object.Destroy(renderer.gameObject);
                else UnityEngine.Object.DestroyImmediate(renderer.gameObject);
            }
        }
    }
}
