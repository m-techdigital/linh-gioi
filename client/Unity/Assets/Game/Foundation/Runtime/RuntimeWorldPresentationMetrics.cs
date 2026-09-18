using System;
using UnityEngine;

namespace LinhGioi.Foundation
{
    [Serializable]
    public sealed class RuntimeWorldPresentationMetrics
    {
        public string profileName;
        public float cameraOrthographicSize;
        public float cameraGroundOffsetY;
        public float actorWorldHeight;
        public float actorScreenHeightRatio;
        public float npcWorldHeight;
        public float npcScreenHeightRatio;
        public int interactionMarkerFontSize;
        public float interactionMarkerCharacterSize;

        public static float ScreenHeightRatio(Camera camera, Bounds worldBounds)
        {
            if (camera == null || worldBounds.size.y <= 0f) return 0f;

            var bottom = camera.WorldToViewportPoint(
                new Vector3(worldBounds.center.x, worldBounds.min.y, worldBounds.center.z));
            var top = camera.WorldToViewportPoint(
                new Vector3(worldBounds.center.x, worldBounds.max.y, worldBounds.center.z));
            return Mathf.Abs(top.y - bottom.y);
        }
    }
}
