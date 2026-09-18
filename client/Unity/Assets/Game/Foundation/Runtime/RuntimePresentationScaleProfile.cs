using System;

namespace LinhGioi.Foundation
{
    /// <summary>
    /// Derived presentation density shared by UI and world framing.
    /// It keeps 16:9 as the canonical landscape baseline and progressively
    /// exposes more playfield on wider phone windows without device-specific pixels.
    /// </summary>
    public readonly struct RuntimePresentationScaleProfile
    {
        public const float CanonicalLandscapeAspect = 16f / 9f;
        public const float MobileBaseUiContentScale = .90f;
        public const float MinimumMobileUiContentScale = .82f;
        public const float MobileAspectCompressionExponent = .25f;

        public string Name { get; }
        public float AspectRatio { get; }
        public float UiContentScale { get; }
        public float WorldCameraScale { get; }

        private RuntimePresentationScaleProfile(string name, float aspectRatio, float uiContentScale)
        {
            Name = name;
            AspectRatio = aspectRatio;
            UiContentScale = uiContentScale;
            WorldCameraScale = 1f / (float)Math.Sqrt(Math.Max(.0001f, uiContentScale));
        }

        public static RuntimePresentationScaleProfile FromWindow(string profileName, int width, int height)
        {
            var name = NormalizeProfile(profileName);
            var safeWidth = Math.Max(1, width);
            var safeHeight = Math.Max(1, height);
            var aspect = Math.Max(safeWidth, safeHeight) / (float)Math.Min(safeWidth, safeHeight);
            if (!string.Equals(name, "mobile", StringComparison.Ordinal))
                return new RuntimePresentationScaleProfile(name, aspect, 1f);

            var boundedAspect = Math.Max(CanonicalLandscapeAspect, aspect);
            var aspectScale = (float)Math.Pow(CanonicalLandscapeAspect / boundedAspect,
                MobileAspectCompressionExponent);
            var rawScale = MobileBaseUiContentScale * aspectScale;
            var scale = Math.Max(MinimumMobileUiContentScale,
                Math.Min(MobileBaseUiContentScale, rawScale));
            return new RuntimePresentationScaleProfile(name, aspect, scale);
        }

        private static string NormalizeProfile(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "desktop";
            var normalized = value.Trim().ToLowerInvariant();
            if (normalized == "pc") return "desktop";
            return normalized == "mobile" || normalized == "tablet" || normalized == "desktop"
                ? normalized
                : "desktop";
        }
    }
}
