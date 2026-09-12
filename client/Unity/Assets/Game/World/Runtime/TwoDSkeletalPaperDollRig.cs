using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.World
{
    public sealed class TwoDSkeletalPaperDollRig
    {
        public readonly struct BoneDefinition
        {
            public BoneDefinition(string id, string parentId, Vector2 pivot, string objectName)
            {
                Id = id;
                ParentId = parentId;
                Pivot = pivot;
                ObjectName = objectName;
            }

            public string Id { get; }
            public string ParentId { get; }
            public Vector2 Pivot { get; }
            public string ObjectName { get; }
        }

        private readonly Transform _root;
        private readonly Dictionary<string, Transform> _bones = new Dictionary<string, Transform>();
        private readonly Dictionary<string, Vector2> _pivots = new Dictionary<string, Vector2>();

        public TwoDSkeletalPaperDollRig(Transform root)
        {
            _root = root != null ? root : throw new ArgumentNullException(nameof(root));
        }

        public void Build(IReadOnlyList<BoneDefinition> definitions)
        {
            if (_bones.Count != 0) throw new InvalidOperationException("Rig already built");
            foreach (var definition in definitions)
            {
                if (string.IsNullOrEmpty(definition.Id) || _bones.ContainsKey(definition.Id))
                    throw new InvalidOperationException("Invalid or duplicate bone: " + definition.Id);
                _bones.Add(definition.Id, new GameObject(definition.ObjectName).transform);
                _pivots.Add(definition.Id, definition.Pivot);
            }
            foreach (var definition in definitions)
            {
                var bone = _bones[definition.Id];
                if (string.IsNullOrEmpty(definition.ParentId))
                {
                    bone.SetParent(_root, false);
                    bone.localPosition = definition.Pivot;
                    continue;
                }
                if (!_bones.TryGetValue(definition.ParentId, out var parent))
                    throw new InvalidOperationException("Missing parent bone: " + definition.ParentId);
                bone.SetParent(parent, false);
                bone.localPosition = definition.Pivot - _pivots[definition.ParentId];
            }
        }

        public Transform Bone(string id)
        {
            if (!_bones.TryGetValue(id, out var bone)) throw new InvalidOperationException("Missing bone: " + id);
            return bone;
        }

        public SpriteRenderer Attach(string boneId, string objectName, Sprite sprite, Vector2 worldCenter,
            Vector2 worldSize, int sortingOrder)
        {
            if (sprite == null || worldSize.x <= 0 || worldSize.y <= 0)
                throw new InvalidOperationException("Invalid paper-doll attachment: " + objectName);
            var host = new GameObject(objectName);
            host.transform.SetParent(Bone(boneId), false);
            host.transform.localPosition = worldCenter - _pivots[boneId];
            host.transform.localScale = new Vector3(worldSize.x / sprite.bounds.size.x,
                worldSize.y / sprite.bounds.size.y, 1);
            var renderer = host.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        public void ResetPose()
        {
            foreach (var bone in _bones.Values) bone.localRotation = Quaternion.identity;
        }

        public void SetLocalRotation(string boneId, float degrees)
        {
            Bone(boneId).localRotation = Quaternion.Euler(0, 0, degrees);
        }

        public float LocalRotationDegrees(string boneId)
        {
            var degrees = Bone(boneId).localEulerAngles.z;
            return degrees > 180 ? degrees - 360 : degrees;
        }
    }
}
