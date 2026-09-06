using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal readonly struct RuntimeViewportMetrics
    {
        internal const string Marker = "LGO Runtime Viewport Metrics Canonical Owner v1";

        internal readonly int ScreenPixelWidth;
        internal readonly int ScreenPixelHeight;
        internal readonly Rect SafeAreaPixels;
        internal readonly int PanelWidth;
        internal readonly int PanelHeight;
        internal readonly Rect SafePanelRect;
        internal readonly float AspectRatio;
        internal readonly string Orientation;
        internal readonly string LayoutClass;
        internal readonly string InputClass;

        private RuntimeViewportMetrics(
            int screenPixelWidth,
            int screenPixelHeight,
            Rect safeAreaPixels,
            int panelWidth,
            int panelHeight,
            Rect safePanelRect,
            string forcedProfile)
        {
            ScreenPixelWidth = screenPixelWidth;
            ScreenPixelHeight = screenPixelHeight;
            SafeAreaPixels = safeAreaPixels;
            PanelWidth = panelWidth;
            PanelHeight = panelHeight;
            SafePanelRect = safePanelRect;
            AspectRatio = panelHeight > 0 ? panelWidth / (float)panelHeight : 1f;
            Orientation = panelWidth >= panelHeight ? "landscape" : "portrait";
            LayoutClass = ResolveLayoutClass(panelWidth, panelHeight, forcedProfile);
            InputClass = ResolveInputClass(forcedProfile);
        }

        internal static RuntimeViewportMetrics FromRoot(VisualElement root, string forcedProfile = null)
        {
            var screenWidth = Mathf.Max(1, Screen.width);
            var screenHeight = Mathf.Max(1, Screen.height);
            var panelWidth = ResolvePanelSize(root != null ? root.resolvedStyle.width : 0f, screenWidth);
            var panelHeight = ResolvePanelSize(root != null ? root.resolvedStyle.height : 0f, screenHeight);
            var safeAreaPixels = Screen.safeArea;
            if (safeAreaPixels.width <= 0f || safeAreaPixels.height <= 0f)
                safeAreaPixels = new Rect(0f, 0f, screenWidth, screenHeight);
            var safePanelRect = ConvertSafeAreaToPanel(safeAreaPixels, screenWidth, screenHeight, panelWidth, panelHeight);
            return new RuntimeViewportMetrics(screenWidth, screenHeight, safeAreaPixels, panelWidth, panelHeight, safePanelRect, forcedProfile);
        }

        internal bool LayoutEquals(RuntimeViewportMetrics other)
        {
            return ScreenPixelWidth == other.ScreenPixelWidth
                && ScreenPixelHeight == other.ScreenPixelHeight
                && PanelWidth == other.PanelWidth
                && PanelHeight == other.PanelHeight
                && Mathf.RoundToInt(SafePanelRect.x) == Mathf.RoundToInt(other.SafePanelRect.x)
                && Mathf.RoundToInt(SafePanelRect.y) == Mathf.RoundToInt(other.SafePanelRect.y)
                && Mathf.RoundToInt(SafePanelRect.width) == Mathf.RoundToInt(other.SafePanelRect.width)
                && Mathf.RoundToInt(SafePanelRect.height) == Mathf.RoundToInt(other.SafePanelRect.height)
                && string.Equals(LayoutClass, other.LayoutClass, StringComparison.Ordinal)
                && string.Equals(InputClass, other.InputClass, StringComparison.Ordinal);
        }

        internal string DebugSummary()
        {
            return "screen=" + ScreenPixelWidth + "x" + ScreenPixelHeight
                + " panel=" + PanelWidth + "x" + PanelHeight
                + " safePixels=" + RectSummary(SafeAreaPixels)
                + " safePanel=" + RectSummary(SafePanelRect)
                + " aspect=" + AspectRatio.ToString("0.###")
                + " layout=" + LayoutClass
                + " input=" + InputClass;
        }

        private static int ResolvePanelSize(float resolvedSize, int fallback)
        {
            return !float.IsNaN(resolvedSize) && resolvedSize > 0f
                ? Mathf.RoundToInt(resolvedSize)
                : fallback;
        }

        private static Rect ConvertSafeAreaToPanel(Rect safeAreaPixels, int screenWidth, int screenHeight, int panelWidth, int panelHeight)
        {
            var scaleX = panelWidth / (float)Mathf.Max(1, screenWidth);
            var scaleY = panelHeight / (float)Mathf.Max(1, screenHeight);
            var x = Mathf.Clamp(safeAreaPixels.xMin * scaleX, 0f, panelWidth);
            var y = Mathf.Clamp((screenHeight - safeAreaPixels.yMax) * scaleY, 0f, panelHeight);
            var width = Mathf.Clamp(safeAreaPixels.width * scaleX, 0f, panelWidth - x);
            var height = Mathf.Clamp(safeAreaPixels.height * scaleY, 0f, panelHeight - y);
            return new Rect(x, y, width, height);
        }

        private static string ResolveLayoutClass(int panelWidth, int panelHeight, string forcedProfile)
        {
            if (!string.IsNullOrWhiteSpace(forcedProfile)) return forcedProfile;
            var shortSide = Mathf.Min(panelWidth, panelHeight);
            var longSide = Mathf.Max(panelWidth, panelHeight);
            if (shortSide <= RuntimeUiLayoutProfile.CompactMaxShortSide || panelHeight <= RuntimeUiLayoutProfile.ShortHeightMax)
                return "compact";
            if (shortSide <= RuntimeUiLayoutProfile.NarrowMaxShortSide && longSide <= RuntimeUiLayoutProfile.NarrowMaxLongSide)
                return "narrow";
            if (shortSide <= RuntimeUiLayoutProfile.RegularMaxShortSide && longSide <= RuntimeUiLayoutProfile.RegularMaxLongSide)
                return "regular";
            return "wide";
        }

        private static string ResolveInputClass(string forcedProfile)
        {
            if (string.Equals(forcedProfile, "mobile", StringComparison.Ordinal) || string.Equals(forcedProfile, "tablet", StringComparison.Ordinal))
                return "touch";
            return Application.isMobilePlatform ? "touch" : "pointer";
        }

        private static string RectSummary(Rect rect)
        {
            return Mathf.RoundToInt(rect.x) + "," + Mathf.RoundToInt(rect.y) + " "
                + Mathf.RoundToInt(rect.width) + "x" + Mathf.RoundToInt(rect.height);
        }
    }
}
