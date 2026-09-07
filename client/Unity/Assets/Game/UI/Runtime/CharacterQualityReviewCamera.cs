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

        public void Configure(Camera camera, Animator actor)
        {
            _camera = camera;
            _actor = actor;
            if (!Debug.isDebugBuild || _camera == null || _actor == null || !_actor.isHuman)
                throw new InvalidOperationException("Character review requires a development Player and actual Humanoid actor.");
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
            actor.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0.24f, 0.27f, 0.30f);
            _camera.fieldOfView = 35f;
        }

        private void LateUpdate()
        {
            if (_actor == null) return;
            _camera.fieldOfView = 35f;
            var target = _actor.transform.position + Vector3.up * 0.98f;
            var direction = Quaternion.AngleAxis(Angle, Vector3.up) * _actor.transform.forward;
            _camera.transform.position = target + direction * 3.65f + Vector3.up * 0.05f;
            _camera.transform.LookAt(target);
        }
    }
}
