using System;
using UnityEngine;

namespace LinhGioi.Foundation
{
    // Five secondary joints, shared mesh/materials, no per-vertex CPU simulation.
    // Garment joints are intentionally outside the canonical Humanoid mapping.
    [DefaultExecutionOrder(100)]
    public sealed class WardrobeDrapeMotion : MonoBehaviour
    {
        private sealed class Joint
        {
            public Transform Bone;
            public Quaternion RestInActor;
            public Vector3 DirectionInActor;
            public Quaternion Rotation;
        }

        private readonly Joint[] _cape = new Joint[3];
        private readonly Joint[] _hem = new Joint[2];
        private Animator _animator;
        private Vector3 _previousPosition;
        private bool _ready;
        private Transform _hips;
        private Quaternion _hipsRestInActor;
        private Quaternion _facing;
        private Transform _chest;
        private Quaternion _chestRestInActor;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
            _hipsRestInActor = Quaternion.Inverse(transform.rotation) * _hips.rotation;
            _chest = _animator.GetBoneTransform(HumanBodyBones.UpperChest);
            _chestRestInActor = Quaternion.Inverse(transform.rotation) * _chest.rotation;
            var all = GetComponentsInChildren<Transform>(true);
            Joint Bind(string name, Vector3 direction)
            {
                var bone = Array.Find(all, value => value.name == name);
                if (bone == null) throw new InvalidOperationException("Missing garment joint: " + name);
                return new Joint { Bone = bone,
                    RestInActor = Quaternion.Inverse(transform.rotation) * bone.rotation,
                    DirectionInActor = transform.InverseTransformDirection(direction),
                    Rotation = bone.rotation };
            }
            var capeRoot = Array.Find(all, value => value.name == "drape_cape_0");
            var capeNext = Array.Find(all, value => value.name == "drape_cape_1");
            if (capeRoot == null || capeNext == null) throw new InvalidOperationException("Incomplete cape chain");
            var capeDirection = (capeNext.position - capeRoot.position).normalized;
            for (var i = 0; i < 3; i++) _cape[i] = Bind("drape_cape_" + i, capeDirection);
            for (var i = 0; i < 2; i++)
            {
                var knee = _animator.GetBoneTransform(i == 0 ? HumanBodyBones.LeftLowerLeg : HumanBodyBones.RightLowerLeg);
                var ankle = _animator.GetBoneTransform(i == 0 ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
                _hem[i] = Bind("drape_hem_" + (i == 0 ? "l" : "r"), (ankle.position - knee.position).normalized);
            }
            _previousPosition = transform.position;
            _ready = true;
        }

        private void LateUpdate()
        {
            if (!_ready) return;
            var dt = Mathf.Min(Time.deltaTime, .05f);
            var velocity = dt > .00001f ? (transform.position - _previousPosition) / dt : Vector3.zero;
            _previousPosition = transform.position;
            var speed = Mathf.Min(velocity.magnitude, 4f);
            var settling = 1f - Mathf.Exp(-14f * dt);
            var hipDelta = _hips.rotation * Quaternion.Inverse(transform.rotation * _hipsRestInActor);
            var forward = Vector3.ProjectOnPlane(hipDelta * transform.forward, transform.up).normalized;
            _facing = Quaternion.LookRotation(forward, transform.up);
            for (var i = 0; i < 3; i++)
            {
                // Lower segments hang under gravity instead of inheriting torso lean.
                var direction = (-transform.up - forward * (.15f + speed * .055f)).normalized;
                var torsoDelta = _chest.rotation * Quaternion.Inverse(transform.rotation * _chestRestInActor);
                var attached = torsoDelta * transform.rotation * _cape[i].RestInActor;
                Aim(_cape[i], direction, settling, attached, i == 0 ? 0f : i == 1 ? .65f : 1f);
            }
            for (var i = 0; i < 2; i++)
            {
                var knee = _animator.GetBoneTransform(i == 0 ? HumanBodyBones.LeftLowerLeg : HumanBodyBones.RightLowerLeg);
                var thigh = _animator.GetBoneTransform(i == 0 ? HumanBodyBones.LeftUpperLeg : HumanBodyBones.RightUpperLeg);
                var leg = (knee.position - thigh.position).normalized;
                Aim(_hem[i], (-transform.up * .3f + leg * .7f).normalized, settling);
            }
        }

        private void Aim(Joint joint, Vector3 direction, float settling, Quaternion attached = default, float free = 1f)
        {
            var rest = _facing * joint.RestInActor;
            var target = Quaternion.FromToRotation(_facing * joint.DirectionInActor, direction) * rest;
            if (free < 1f) target = Quaternion.Slerp(attached, target, free);
            joint.Rotation = Quaternion.Slerp(joint.Rotation, target, settling);
            joint.Bone.rotation = joint.Rotation;
        }
    }
}
