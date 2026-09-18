using System;
using UnityEngine;

namespace LinhGioi.UI
{
    /// <summary>
    /// Semantic presentation profile derived from the available runtime window.
    /// This profile owns responsive bounds and density intent; it is not a global
    /// instruction to scale every screen by one multiplier.
    /// </summary>
    internal readonly struct RuntimeUiAdaptiveProfile
    {
        internal readonly string PresentationClass;
        internal readonly string DensityMode;
        internal readonly string InputClass;
        internal readonly float AspectRatio;
        internal readonly float SpacingScaleMin;
        internal readonly float SpacingScaleMax;
        internal readonly float TypographyScaleMin;
        internal readonly float TypographyScaleMax;
        internal readonly float TouchTargetScale;
        private RuntimeUiAdaptiveProfile(
            string presentationClass,
            string densityMode,
            string inputClass,
            float aspectRatio,
            float spacingScaleMin,
            float spacingScaleMax,
            float typographyScaleMin,
            float typographyScaleMax)
        {
            PresentationClass = presentationClass;
            DensityMode = densityMode;
            InputClass = inputClass;
            AspectRatio = aspectRatio;
            SpacingScaleMin = spacingScaleMin;
            SpacingScaleMax = spacingScaleMax;
            TypographyScaleMin = typographyScaleMin;
            TypographyScaleMax = typographyScaleMax;
            TouchTargetScale = 1f;
        }
        internal static RuntimeUiAdaptiveProfile FromWindow(
            string windowClass, string inputClass, int width, int height)
        {
            var safeWidth = Mathf.Max(1, width);
            var safeHeight = Mathf.Max(1, height);
            var aspect = Mathf.Max(safeWidth, safeHeight) / (float)Mathf.Min(safeWidth, safeHeight);
            var touch = string.Equals(inputClass, "touch", StringComparison.Ordinal);
            var normalizedClass = (windowClass ?? string.Empty).Trim().ToLowerInvariant();

            if (touch && (normalizedClass == "mobile" || normalizedClass == "compact" || normalizedClass == "narrow"))
            {
                return new RuntimeUiAdaptiveProfile(
                    "MobileLandscape", "compact", "touch", aspect,
                    .88f, 1f, .94f, 1f);
            }

            if (touch || normalizedClass == "tablet" || normalizedClass == "regular")
            {
                return new RuntimeUiAdaptiveProfile(
                    "Tablet", "compact", touch ? "touch" : "pointer", aspect,
                    .94f, 1f, .97f, 1f);
            }
            return new RuntimeUiAdaptiveProfile(
                "PCWide", "comfortable", touch ? "touch" : "pointer", aspect,
                1f, 1f, 1f, 1f);
        }
    }
}
