using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace LinhGioi.Character
{
    public readonly struct RigidBoneDefinition
    {
        public RigidBoneDefinition(RigidBoneId id, RigidBoneId? parent, Vector2 bindPosition)
        {
            Id = id;
            Parent = parent;
            BindPosition = bindPosition;
        }

        public RigidBoneId Id { get; }
        public RigidBoneId? Parent { get; }
        public Vector2 BindPosition { get; }
    }

    public sealed class RigidSkeletonDefinition
    {
        private RigidSkeletonDefinition(IEnumerable<RigidBoneDefinition> bones)
        {
            Bones = bones.ToArray();
        }

        public IReadOnlyList<RigidBoneDefinition> Bones { get; }

        public static RigidSkeletonDefinition ChibiSideView()
        {
            static RigidBoneDefinition Bone(RigidBoneId id, RigidBoneId? parent, float x, float y) =>
                new(id, parent, new Vector2(x, y));

            return new RigidSkeletonDefinition(new[]
            {
                Bone(RigidBoneId.CharacterRoot, null, 0f, 0f),
                Bone(RigidBoneId.Pelvis, RigidBoneId.CharacterRoot, 0f, 2.30f),
                Bone(RigidBoneId.Torso, RigidBoneId.Pelvis, 0f, 2.72f),
                Bone(RigidBoneId.Chest, RigidBoneId.Torso, 0f, 3.28f),
                Bone(RigidBoneId.Neck, RigidBoneId.Chest, .04f, 3.85f),
                Bone(RigidBoneId.Head, RigidBoneId.Neck, .04f, 3.96f),
                Bone(RigidBoneId.ShoulderL, RigidBoneId.Chest, -.15f, 3.72f),
                Bone(RigidBoneId.UpperArmL, RigidBoneId.ShoulderL, -.15f, 3.72f),
                Bone(RigidBoneId.ElbowL, RigidBoneId.UpperArmL, -.295f, 2.983f),
                Bone(RigidBoneId.LowerArmL, RigidBoneId.ElbowL, -.295f, 2.983f),
                Bone(RigidBoneId.WristL, RigidBoneId.LowerArmL, -.169f, 2.101f),
                Bone(RigidBoneId.HandL, RigidBoneId.WristL, -.169f, 2.101f),
                Bone(RigidBoneId.ShoulderR, RigidBoneId.Chest, -.05f, 3.70f),
                Bone(RigidBoneId.UpperArmR, RigidBoneId.ShoulderR, -.05f, 3.70f),
                Bone(RigidBoneId.ElbowR, RigidBoneId.UpperArmR, .076f, 2.963f),
                Bone(RigidBoneId.LowerArmR, RigidBoneId.ElbowR, .076f, 2.963f),
                Bone(RigidBoneId.WristR, RigidBoneId.LowerArmR, .234f, 2.05f),
                Bone(RigidBoneId.HandR, RigidBoneId.WristR, .234f, 2.05f),
                Bone(RigidBoneId.HipL, RigidBoneId.Pelvis, .12f, 2.30f),
                Bone(RigidBoneId.UpperLegL, RigidBoneId.HipL, .12f, 2.30f),
                Bone(RigidBoneId.KneeL, RigidBoneId.UpperLegL, .09f, 1.342f),
                Bone(RigidBoneId.LowerLegL, RigidBoneId.KneeL, .09f, 1.342f),
                Bone(RigidBoneId.AnkleL, RigidBoneId.LowerLegL, .06f, .435f),
                Bone(RigidBoneId.FootL, RigidBoneId.AnkleL, .06f, .435f),
                Bone(RigidBoneId.HipR, RigidBoneId.Pelvis, -.10f, 2.30f),
                Bone(RigidBoneId.UpperLegR, RigidBoneId.HipR, -.10f, 2.30f),
                Bone(RigidBoneId.KneeR, RigidBoneId.UpperLegR, -.07f, 1.342f),
                Bone(RigidBoneId.LowerLegR, RigidBoneId.KneeR, -.07f, 1.342f),
                Bone(RigidBoneId.AnkleR, RigidBoneId.LowerLegR, -.04f, .435f),
                Bone(RigidBoneId.FootR, RigidBoneId.AnkleR, -.04f, .435f),
            });
        }
    }

    public sealed class RigidCharacterSkeleton
    {
        private readonly Dictionary<RigidBoneId, Transform> _bones;

        private RigidCharacterSkeleton(Transform characterRoot, Dictionary<RigidBoneId, Transform> bones)
        {
            CharacterRoot = characterRoot;
            _bones = bones;
        }

        public Transform CharacterRoot { get; }
        public IReadOnlyCollection<RigidBoneId> BoneIds => _bones.Keys;
        public IEnumerable<Transform> AllTransforms => _bones.Values.Distinct();
        public Transform this[RigidBoneId id] => _bones[id];

        public static RigidCharacterSkeleton Create(Transform characterRoot, RigidSkeletonDefinition definition)
        {
            if (characterRoot == null) throw new ArgumentNullException(nameof(characterRoot));
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (definition.Bones.Select(bone => bone.Id).Distinct().Count() != definition.Bones.Count)
                throw new ArgumentException("Skeleton bone IDs must be unique", nameof(definition));
            var required = Enum.GetValues(typeof(RigidBoneId)).Cast<RigidBoneId>().ToArray();
            if (required.Except(definition.Bones.Select(bone => bone.Id)).Any())
                throw new ArgumentException("Skeleton definition does not provide all canonical bones", nameof(definition));

            characterRoot.localRotation = Quaternion.identity;
            characterRoot.localScale = Vector3.one;
            if (characterRoot.GetComponent<SortingGroup>() == null) characterRoot.gameObject.AddComponent<SortingGroup>();

            var bones = new Dictionary<RigidBoneId, Transform> { [RigidBoneId.CharacterRoot] = characterRoot };
            var positions = definition.Bones.ToDictionary(bone => bone.Id, bone => bone.BindPosition);
            foreach (var bone in definition.Bones.Where(bone => bone.Id != RigidBoneId.CharacterRoot))
            {
                if (bone.Parent == null || !bones.TryGetValue(bone.Parent.Value, out var parent))
                    throw new ArgumentException("Skeleton parents must precede children: " + bone.Id, nameof(definition));
                var transform = new GameObject(bone.Id.ToString()).transform;
                transform.SetParent(parent, false);
                transform.localPosition = bone.BindPosition - positions[bone.Parent.Value];
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                bones.Add(bone.Id, transform);
            }
            return new RigidCharacterSkeleton(characterRoot, bones);
        }
    }
}
