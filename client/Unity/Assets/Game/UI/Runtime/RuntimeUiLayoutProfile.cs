using UnityEngine;

namespace LinhGioi.UI
{
    internal readonly struct RuntimeUiLayoutProfile
    {
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
            MobileScale = IsMobile ? Mathf.Clamp(ShortSide / 520f, 0.62f, 0.86f) : 1f;
            LoginLogoWidth = IsMobile
                ? Mathf.Clamp(width * 0.43f, 260f, 356f)
                : IsTablet
                    ? Mathf.Clamp(width * 0.32f, 390f, 446f)
                    : Mathf.Clamp(width * 0.26f, 470f, 504f);
            LoginLogoHeight = LoginLogoWidth * 0.50f;
            LoginCardWidth = IsMobile
                ? Mathf.Clamp(width * 0.46f, 312f, 392f)
                : IsTablet
                    ? Mathf.Clamp(width * 0.36f, 416f, 470f)
                    : 468f;
            LoginCardPadding = IsMobile ? Mathf.RoundToInt(14f * MobileScale) : IsTablet ? 22 : 28;
            LoginButtonHeight = IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.11f, 42f, 50f)) : IsTablet ? 54 : 58;
            LoginButtonFontSize = IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.047f, 17f, 21f)) : IsTablet ? 20 : 22;
        }

        internal static RuntimeUiLayoutProfile FromScreen(string forcedProfile, int screenWidth, int screenHeight)
        {
            var width = screenWidth > 0 ? screenWidth : 1280;
            var height = screenHeight > 0 ? screenHeight : 720;
            var name = forcedProfile ?? (width <= 760 || height <= 520 ? "mobile" : width <= 1100 ? "tablet" : "desktop");
            return new RuntimeUiLayoutProfile(name, width, height);
        }
    }
}
