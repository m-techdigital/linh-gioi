using UnityEngine;

namespace LinhGioi.UI
{
    /// <summary>
    /// Entry/Auth-specific responsive composition.
    /// Named profiles own surface families; no whole-screen scalar is exposed.
    /// </summary>
    internal readonly struct RuntimeAuthLayoutVariant
    {
        internal readonly string Name;
        internal readonly float EntrySurfaceWidth;
        internal readonly float ServerSurfaceWidth;
        internal readonly float RegisterSurfaceWidth;
        internal readonly float RecoverySurfaceWidth;
        internal readonly float PanelTop;
        internal readonly float BrandTop;
        internal readonly float BrandWidth;
        internal readonly float BrandHeight;
        internal readonly float SloganLeft;
        internal readonly float SloganTop;
        internal readonly float SloganWidth;
        internal readonly float NoticeWidth;
        internal readonly bool ShowNotice;
        internal readonly bool ShowPeripheralChrome;

        private RuntimeAuthLayoutVariant(
            string name,
            float entrySurfaceWidth,
            float serverSurfaceWidth,
            float registerSurfaceWidth,
            float recoverySurfaceWidth,
            float panelTop,
            float brandTop,
            float brandWidth,
            float brandHeight,
            float sloganLeft,
            float sloganTop,
            float sloganWidth,
            float noticeWidth,
            bool showNotice,
            bool showPeripheralChrome)
        {
            Name = name;
            EntrySurfaceWidth = entrySurfaceWidth;
            ServerSurfaceWidth = serverSurfaceWidth;
            RegisterSurfaceWidth = registerSurfaceWidth;
            RecoverySurfaceWidth = recoverySurfaceWidth;
            PanelTop = panelTop;
            BrandTop = brandTop;
            BrandWidth = brandWidth;
            BrandHeight = brandHeight;
            SloganLeft = sloganLeft;
            SloganTop = sloganTop;
            SloganWidth = sloganWidth;
            NoticeWidth = noticeWidth;
            ShowNotice = showNotice;
            ShowPeripheralChrome = showPeripheralChrome;
        }

        internal static RuntimeAuthLayoutVariant FromWindow(string windowClass, int width, int height)
        {
            var normalized = (windowClass ?? string.Empty).Trim().ToLowerInvariant();
            var touchProfile = normalized == "mobile" || normalized == "compact" || normalized == "narrow"
                || normalized == "tablet" || normalized == "regular";
            var adaptive = RuntimeUiAdaptiveProfile.FromWindow(
                windowClass, touchProfile ? "touch" : "pointer", width, height);
            var w = Mathf.Max(1f, width);
            var h = Mathf.Max(1f, height);

            if (adaptive.PresentationClass == "MobileLandscape")
            {
                var entry = Mathf.Clamp(w * .23f, 440f, 490f);
                return new RuntimeAuthLayoutVariant(
                    "MobileLandscape",
                    entry,
                    Mathf.Clamp(w * .24f, 460f, 505f),
                    Mathf.Clamp(w * .22f, 430f, 470f),
                    Mathf.Clamp(w * .21f, 420f, 455f),
                    Mathf.Clamp(h * .25f, 224f, 250f),
                    28f,
                    entry * .86f,
                    172f,
                    0f,
                    0f,
                    0f,
                    545f,
                    false,
                    false);
            }

            if (adaptive.PresentationClass == "Tablet")
            {
                var entry = Mathf.Clamp(w * .40f, 480f, 515f);
                return new RuntimeAuthLayoutVariant(
                    "Tablet",
                    entry,
                    Mathf.Clamp(w * .43f, 515f, 555f),
                    Mathf.Clamp(w * .40f, 480f, 515f),
                    Mathf.Clamp(w * .38f, 455f, 490f),
                    Mathf.Clamp(h * .30f, 276f, 300f),
                    38f,
                    entry * .92f,
                    198f,
                    Mathf.Clamp(w * .09f, 72f, 112f),
                    24f,
                    250f,
                    260f,
                    true,
                    false);
            }

            var desktopEntry = Mathf.Clamp(w * .345f, 530f, 590f);
            return new RuntimeAuthLayoutVariant(
                "PCWide",
                desktopEntry,
                Mathf.Clamp(w * .37f, 570f, 620f),
                Mathf.Clamp(w * .34f, 520f, 570f),
                Mathf.Clamp(w * .32f, 490f, 540f),
                Mathf.Clamp(h * .34f, 300f, 320f),
                58f,
                desktopEntry * .96f,
                232f,
                Mathf.Clamp(w * .13f, 64f, 218f),
                28f,
                340f,
                545f,
                true,
                true);
        }
    }
}
