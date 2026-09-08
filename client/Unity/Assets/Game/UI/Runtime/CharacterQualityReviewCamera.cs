using System;
using UnityEngine;

namespace LinhGioi.UI
{
    // Opt-in development capture only. Runs after the gameplay camera brain.
    [DefaultExecutionOrder(10000)]
    public sealed class CharacterQualityReviewCamera : MonoBehaviour
    {
        private Camera _camera;
        private Animator _actor;
        public float Angle { get; set; }
        public bool FaceCloseup { get; set; }
        private Transform _head;
        private float _headYaw;
        private Quaternion _diagnosticHeadRotation;
        // Explicit deformation review, not a gameplay look/talking animation.
        public float HeadYawDegrees
        {
            get => _headYaw;
            set
            {
                if (_headYaw == 0f && value != 0f) _diagnosticHeadRotation = _head.rotation;
                _headYaw = value;
            }
        }

        public void Configure(Camera camera, Animator actor)
        {
            _camera = camera;
            _actor = actor;
            if (!Debug.isDebugBuild || _camera == null || _actor == null || !_actor.isHuman)
                throw new InvalidOperationException("Character review requires a development Player and actual Humanoid actor.");
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-character-shadow-diagnostic") >= 0)
            {
                foreach (var light in FindObjectsByType<Light>(FindObjectsSortMode.None))
                    light.shadows = LightShadows.None;
                Debug.Log("LGO_CHARACTER_SHADOW_DIAGNOSTIC light_shadows=none not_visual_pass=true");
            }
            foreach (var renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
                renderer.enabled = renderer.transform.IsChildOf(actor.transform);
            foreach (var renderer in actor.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.updateWhenOffscreen = true;
                foreach (var material in renderer.sharedMaterials)
                {
                    var atlas = material.GetTexture("_BaseMap");
                    Debug.Log("LGO_CHARACTER_REVIEW_ATLAS material=" + material.name + " size="
                        + (atlas == null ? "none" : atlas.width + "x" + atlas.height));
                }
            }
            _head = actor.GetBoneTransform(HumanBodyBones.Head);
            actor.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0.24f, 0.27f, 0.30f);
            _camera.fieldOfView = 35f;
        }

        private void LateUpdate()
        {
            if (_actor == null) return;
            if (_headYaw != 0f) _head.rotation = Quaternion.AngleAxis(_headYaw, Vector3.up) * _diagnosticHeadRotation;
            _camera.fieldOfView = 35f;
            var target = FaceCloseup ? _head.position : _actor.transform.position + Vector3.up * 0.98f;
            var direction = Quaternion.AngleAxis(Angle, Vector3.up) * _actor.transform.forward;
            _camera.transform.position = target + direction * (FaceCloseup ? 0.85f : 3.65f) + Vector3.up * 0.05f;
            _camera.transform.LookAt(target);
        }
    }
}
