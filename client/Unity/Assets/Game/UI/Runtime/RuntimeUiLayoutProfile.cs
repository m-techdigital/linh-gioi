using UnityEngine;

namespace LinhGioi.UI
{
    internal readonly struct RuntimeUiLayoutProfile
    {
        internal const int DefaultViewportWidth = 1280;
        internal const int DefaultViewportHeight = 720;
        internal const int MobileMaxWidth = 760;
        internal const int MobileMaxHeight = 520;
        internal const int TabletMaxWidth = 1100;
        internal const float MobileScaleBaseline = 520f;
        internal const float MobileScaleMin = 0.62f;
        internal const float MobileScaleMax = 0.86f;
        internal const float MobileLoginLogoWidthRatio = 0.43f;
        internal const float TabletLoginLogoWidthRatio = 0.32f;
        internal const float DesktopLoginLogoWidthRatio = 0.26f;
        internal const float MobileLoginCardWidthRatio = 0.46f;
        internal const float TabletLoginCardWidthRatio = 0.36f;
        internal const float LoginLogoAspect = 0.50f;

        internal readonly string Name;
        internal readonly int Width;
        internal readonly int Height;
        internal readonly int ShortSide;
        internal readonly bool IsMobile;
        internal readonly bool IsTablet;
        internal readonly float MobileScale;
        internal readonly float LoginLogoWidth;
        internal readonly float LoginLogoHeight;
        internal readonly float LoginCardWidth;
        internal readonly int LoginCardPadding;
        internal readonly int LoginButtonHeight;
        internal readonly int LoginButtonFontSize;

        private RuntimeUiLayoutProfile(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
            ShortSide = Mathf.Min(width, height);
            IsMobile = name == "mobile";
            IsTablet = name == "tablet";
            MobileScale = IsMobile ? Mathf.Clamp(ShortSide / MobileScaleBaseline, MobileScaleMin, MobileScaleMax) : 1f;
            LoginLogoWidth = IsMobile
                ? Mathf.Clamp(width * MobileLoginLogoWidthRatio, 260f, 356f)
                : IsTablet
                    ? Mathf.Clamp(width * TabletLoginLogoWidthRatio, 390f, 446f)
                    : Mathf.Clamp(width * DesktopLoginLogoWidthRatio, 470f, 504f);
            LoginLogoHeight = LoginLogoWidth * LoginLogoAspect;
            LoginCardWidth = IsMobile
                ? Mathf.Clamp(width * MobileLoginCardWidthRatio, 312f, 392f)
                : IsTablet
                    ? Mathf.Clamp(width * TabletLoginCardWidthRatio, 416f, 470f)
                    : 468f;
            LoginCardPadding = IsMobile ? Mathf.RoundToInt(14f * MobileScale) : IsTablet ? 22 : 28;
            LoginButtonHeight = IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.11f, 42f, 50f)) : IsTablet ? 54 : 58;
            LoginButtonFontSize = IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.047f, 17f, 21f)) : IsTablet ? 20 : 22;
        }

        internal static RuntimeUiLayoutProfile FromScreen(string forcedProfile, int screenWidth, int screenHeight)
        {
            var width = screenWidth > 0 ? screenWidth : DefaultViewportWidth;
            var height = screenHeight > 0 ? screenHeight : DefaultViewportHeight;
            var name = forcedProfile ?? (width <= MobileMaxWidth || height <= MobileMaxHeight ? "mobile" : width <= TabletMaxWidth ? "tablet" : "desktop");
            return new RuntimeUiLayoutProfile(name, width, height);
        }
    }
}
