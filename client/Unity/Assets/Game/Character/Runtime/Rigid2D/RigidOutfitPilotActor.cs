using System;
using System.Linq;
using UnityEngine;

namespace LinhGioi.Character
{
    public sealed class RigidOutfitPilotActor
    {
        private readonly RigidCharacterAnimator _animator;
        private readonly RigidEquippedBundle _body;
        private readonly RigidEquippedBundle _outfit;
        private readonly RigidEquippedBundle _weapon;

        private RigidOutfitPilotActor(Transform root, RigidCharacterSkeleton skeleton,
            RigidCharacterAnimator animator, RigidEquippedBundle body, RigidEquippedBundle outfit,
            RigidEquippedBundle weapon)
        {
            Root = root;
            Skeleton = skeleton;
            _animator = animator;
            _body = body;
            _outfit = outfit;
            _weapon = weapon;
        }

        public Transform Root { get; }
        public RigidCharacterSkeleton Skeleton { get; }
        public string SpriteFingerprint => RigidFitProfile.Sha256(
            _body.SpriteFingerprint + "|" + _outfit.SpriteFingerprint + "|" + _weapon.SpriteFingerprint);
        public string FitFingerprint => RigidFitProfile.Sha256(
            _body.FitFingerprint + "|" + _outfit.FitFingerprint + "|" + _weapon.FitFingerprint);
        public bool HasUnitScale => Root.GetComponentsInChildren<Transform>(true).All(transform => transform.localScale == Vector3.one);
        public bool HasForbiddenRendererComponents => Root.GetComponentsInChildren<Component>(true).Any(component =>
            component is MeshFilter || component is SkinnedMeshRenderer ||
            component.GetType().Name.Equals("SpriteSkin", StringComparison.Ordinal));

        public static RigidOutfitPilotActor Create(Transform root, RigidOutfitPilotCharacter character)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (character == null) throw new ArgumentNullException(nameof(character));
            var skeleton = RigidCharacterSkeleton.Create(root, character.Skeleton);
            var binder = new RigidOutfitBinder(skeleton);
            var body = binder.Equip("body", character.Body, character.BodyFit);
            var outfit = binder.Equip("outfit", character.Outfit, character.OutfitFit);
            var weapon = binder.Equip("weapon", character.Weapon, character.WeaponFit);
            return new RigidOutfitPilotActor(root, skeleton, new RigidCharacterAnimator(skeleton), body, outfit, weapon);
        }

        public void Apply(RigidMotionState state, float normalizedTime) =>
            _animator.Apply(RigidMotionLibrary.Sample(state, normalizedTime));
    }
}
