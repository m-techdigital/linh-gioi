using System;
using UnityEngine;

namespace LinhGioi.World
{
    // Shared bone-space transition. Rendering/equipment refresh never advances time.
    public sealed class TwoDPoseTransition
    {
        private readonly Transform[] _bones;
        private readonly Vector3[] _lastPosition, _fromPosition;
        private readonly Quaternion[] _lastRotation, _fromRotation;
        private readonly float _duration;
        private string _state;
        private float _age, _delta;
        public TwoDPoseTransition(Transform[] bones, float duration = .12f)
        {
            if (bones == null || bones.Length == 0 || duration <= 0) throw new ArgumentException("Valid bones and duration required");
            _bones = (Transform[])bones.Clone();
            _duration = duration;
            _lastPosition = new Vector3[bones.Length]; _fromPosition = new Vector3[bones.Length];
            _lastRotation = new Quaternion[bones.Length]; _fromRotation = new Quaternion[bones.Length];
        }
        public void Advance(float seconds)
        {
            _delta = Mathf.Max(0, seconds);
            _age += _delta;
        }
        public void Blend(string state)
        {
            if (_state == null) _age = _duration;
            else if (_state != state)
            {
                Array.Copy(_lastPosition, _fromPosition, _bones.Length);
                Array.Copy(_lastRotation, _fromRotation, _bones.Length);
                // Large explicit advances (replay/settling) may span the transition.
                _age = _delta >= _duration ? _duration : 0;
            }
            _state = state;
            var weight = Mathf.SmoothStep(0, 1, Mathf.Clamp01(_age / _duration));
            for (var i = 0; i < _bones.Length; i++)
            {
                var bone = _bones[i];
                if (weight < 1)
                {
                    bone.localPosition = Vector3.Lerp(_fromPosition[i], bone.localPosition, weight);
                    bone.localRotation = Quaternion.Slerp(_fromRotation[i], bone.localRotation, weight);
                }
                _lastPosition[i] = bone.localPosition; _lastRotation[i] = bone.localRotation;
            }
        }
    }
}
