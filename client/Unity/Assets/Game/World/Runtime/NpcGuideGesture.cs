using UnityEngine;

namespace LinhGioi.World
{
    [RequireComponent(typeof(Animator))]
    public sealed class NpcGuideGesture : MonoBehaviour
    {
        private Animator _animator;
        private Transform _shoulder;
        private float _reach;
        private float _started = float.NegativeInfinity;
        private Vector3 _destination;
        private int _layer;
        private readonly Transform[] _fingers = new Transform[15];
        private readonly Quaternion[] _openFingers = new Quaternion[15];
        private const int FirstFinger = (int)HumanBodyBones.RightThumbProximal;
        private const float Duration = 2.2f;
        public bool Active => Time.time - _started < Duration;

        public void Initialize(int layer)
        {
            _animator = GetComponent<Animator>();
            _layer = layer;
            _shoulder = _animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            var elbow = _animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            var hand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            _reach = (Vector3.Distance(_shoulder.position, elbow.position) +
                Vector3.Distance(elbow.position, hand.position)) * 0.88f;
            CacheOpenHand();
        }

        private void CacheOpenHand()
        {
            var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer == null || renderer.sharedMesh == null) return;
            var bones = renderer.bones;
            var bind = renderer.sharedMesh.bindposes;
            for (var i = 0; i < _fingers.Length; i++)
            {
                var finger = _animator.GetBoneTransform((HumanBodyBones)(FirstFinger + i));
                if (finger == null) continue;
                var index = System.Array.IndexOf(bones, finger);
                var parent = System.Array.IndexOf(bones, finger.parent);
                if (index < 0 || parent < 0 || index >= bind.Length || parent >= bind.Length) continue;
                _fingers[i] = finger;
                _openFingers[i] = (bind[parent] * bind[index].inverse).rotation;
            }
        }

        public void Begin(Vector3 destination)
        {
            _destination = destination;
            _started = Time.time;
        }

        public void Cancel() => _started = float.NegativeInfinity;

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null || layerIndex != _layer) return;
            var elapsed = Time.time - _started;
            var weight = Active ? Mathf.SmoothStep(0f, 1f,
                Mathf.Min(elapsed / 0.35f, (Duration - elapsed) / 0.35f)) : 0f;
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, weight * 0.85f);
            _animator.SetLookAtWeight(weight * 0.5f, 0f, 0.6f, 0f, 0.7f);
            if (weight <= 0f) return;
            for (var i = 0; i < _fingers.Length; i++)
                if (_fingers[i] != null)
                    _animator.SetBoneLocalRotation((HumanBodyBones)(FirstFinger + i),
                        Quaternion.Slerp(_fingers[i].localRotation, _openFingers[i], weight * 0.9f));
            var direction = Vector3.ProjectOnPlane(_destination - transform.position, Vector3.up).normalized;
            // Keep the elbow below and outside the shoulder, away from the cape.
            // The hand still aims at the actual destination, not a fixed local gesture.
            _animator.SetIKHintPosition(AvatarIKHint.RightElbow,
                _shoulder.position + transform.right * (_reach * 0.35f)
                - Vector3.up * (_reach * 0.55f) + direction * (_reach * 0.20f));
            _animator.SetIKPosition(AvatarIKGoal.RightHand,
                _shoulder.position + direction * _reach - Vector3.up * (_reach * 0.15f));
            _animator.SetLookAtPosition(_destination + Vector3.up);
        }
    }
}
