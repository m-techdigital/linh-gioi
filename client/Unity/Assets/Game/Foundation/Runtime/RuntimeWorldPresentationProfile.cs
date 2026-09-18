using System;

namespace LinhGioi.Foundation
{
    public readonly struct RuntimeWorldPresentationProfile
    {
        private const int MobileMaxShortSide = 600;
        private const int MobileMaxLongSide = 1050;
        private const int TabletMaxShortSide = 900;
        private const int TabletMaxLongSide = 1450;

        public string Name { get; }
        public float CameraOrthographicSize { get; }
        public float CameraGroundOffsetY { get; }
        public float ActorMinScreenHeightRatio { get; }
        public float ActorMaxScreenHeightRatio { get; }
        public float NpcMinScreenHeightRatio { get; }
        public float NpcMaxScreenHeightRatio { get; }
        public int InteractionMarkerFontSize { get; }
        public float InteractionMarkerCharacterSize { get; }

        private RuntimeWorldPresentationProfile(string name, int width, int height)
        {
            Name = name;
            var presentation = RuntimePresentationScaleProfile.FromWindow(name, width, height);
            CameraOrthographicSize = 3.8f * presentation.WorldCameraScale;
            CameraGroundOffsetY = 1.5f;
            ActorMinScreenHeightRatio = .18f;
            ActorMaxScreenHeightRatio = .27f;
            NpcMinScreenHeightRatio = .16f;
            NpcMaxScreenHeightRatio = .27f;
            InteractionMarkerFontSize = 64;
            InteractionMarkerCharacterSize = .045f;
        }
        public static RuntimeWorldPresentationProfile FromScreen(
            string forcedProfile, int screenWidth, int screenHeight)
        {
            var forced = NormalizeProfile(forcedProfile);
            if (forced != null) return new RuntimeWorldPresentationProfile(forced, screenWidth, screenHeight);

            var width = screenWidth > 0 ? screenWidth : 1280;
            var height = screenHeight > 0 ? screenHeight : 720;
            var shortSide = Math.Min(width, height);
            var longSide = Math.Max(width, height);
            if (shortSide <= MobileMaxShortSide && longSide <= MobileMaxLongSide)
                return new RuntimeWorldPresentationProfile("mobile", width, height);
            if (shortSide <= TabletMaxShortSide && longSide <= TabletMaxLongSide)
                return new RuntimeWorldPresentationProfile("tablet", width, height);
            return new RuntimeWorldPresentationProfile("desktop", width, height);
        }

        public static string NormalizeProfile(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var normalized = value.Trim().ToLowerInvariant();
            if (normalized == "pc") return "desktop";
            return normalized == "mobile" || normalized == "tablet" || normalized == "desktop"
                ? normalized
                : null;
        }
    }
}
