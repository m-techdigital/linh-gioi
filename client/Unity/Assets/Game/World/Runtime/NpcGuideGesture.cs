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
            _animator.SetLookAtWeight(weight * 0.5f, 0f, 0.6f, 0f, 0.7f);
            if (weight <= 0f) return;
            var direction = Vector3.ProjectOnPlane(_destination - transform.position, Vector3.up).normalized;
            _animator.SetIKPosition(AvatarIKGoal.RightHand,
                _shoulder.position + direction * _reach - Vector3.up * (_reach * 0.15f));
            _animator.SetLookAtPosition(_destination + Vector3.up);
        }
    }
}
