using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal readonly struct RuntimeUiLayoutProfile
    {
        internal const int DefaultViewportWidth = 1280;
        internal const int DefaultViewportHeight = 720;
        internal const int MobileMaxShortSide = 600;
        internal const int MobileMaxLongSide = 1050;
        internal const int TabletMaxShortSide = 900;
        internal const int TabletMaxLongSide = 1450;
        internal const float MobileScaleBaseline = 520f;
        internal const float MobileScaleMin = 0.62f;
        internal const float MobileScaleMax = 0.86f;
        internal const float MobileLoginLogoWidthRatio = 0.43f;
        internal const float TabletLoginLogoWidthRatio = 0.32f;
        internal const float DesktopLoginLogoWidthRatio = 0.26f;
        internal const float MobileLoginCardWidthRatio = 0.43f;
        internal const float TabletLoginCardWidthRatio = 0.34f;
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

        internal int RootPaddingHorizontal => IsMobile ? 12 : IsTablet ? 18 : 28;
        internal int RootPaddingTop => IsMobile ? 10 : 16;
        internal int RootPaddingBottom => IsMobile ? 12 : 18;
        internal int HeaderMinHeight(bool authVisible) => authVisible && IsMobile ? 8 : IsMobile ? 34 : 76;
        internal int AuthPanelMinHeight => IsMobile ? 0 : IsTablet ? 500 : 560;
        internal int AuthPanelMarginTop => IsMobile ? 0 : 4;
        internal int AuthPanelPaddingTop => IsMobile ? 0 : 8;
        internal int AuthPanelPaddingBottom => 8;
        internal DisplayStyle LoginStageDisplay => IsMobile ? DisplayStyle.None : DisplayStyle.Flex;
        internal int LoginStageWidth => IsTablet ? 262 : 304;
        internal int LoginStageMinHeight => IsTablet ? 388 : 460;
        internal int LoginStageRight => IsTablet ? 12 : 64;
        internal int LoginStageBottom => IsTablet ? -52 : -118;
        internal int LoginGateKeeperWidth => IsTablet ? 240 : 282;
        internal int LoginGateKeeperHeight => IsTablet ? 360 : 423;
        internal DisplayStyle LoginNpcGroundingDisplay => IsMobile ? DisplayStyle.None : DisplayStyle.Flex;
        internal int LoginNpcGroundingWidth => IsTablet ? 232 : 272;
        internal int LoginNpcGroundingHeight => IsTablet ? 26 : 32;
        internal int LoginNpcGroundingBottom => IsTablet ? 38 : 42;
        internal int LoginNpcGroundingBloomWidth => IsTablet ? 166 : 196;
        internal int LoginNpcGroundingBloomHeight => IsTablet ? 8 : 10;
        internal int LoginNpcGroundingBloomBottom => IsTablet ? 48 : 55;
        internal Color LoginNpcGroundingColor => IsTablet
            ? new Color(0.005f, 0.018f, 0.035f, 0.34f)
            : new Color(0.005f, 0.018f, 0.035f, 0.40f);
        internal Color LoginNpcGroundingBloomColor => IsTablet
            ? new Color(0.10f, 0.72f, 0.95f, 0.24f)
            : new Color(0.10f, 0.72f, 0.95f, 0.30f);
        internal float LoginNpcGroundingOpacity => IsTablet ? 0.86f : 0.92f;
        internal float LoginNpcGroundingBloomOpacity => IsTablet ? 0.72f : 0.80f;
        internal Length LoginControlColumnWidth => IsMobile ? Length.Percent(100) : IsTablet ? Length.Percent(56) : Length.Percent(54);
        internal int LoginControlColumnMinWidth => IsMobile ? 0 : 300;
        internal int LoginControlColumnMaxWidth => IsMobile ? 500 : IsTablet ? 540 : 600;
        internal int LoginControlColumnPaddingBottom => IsMobile ? 0 : 12;
        internal int LoginControlColumnMarginLeft => IsMobile ? 0 : IsTablet ? 8 : 22;
        internal int LoginControlColumnMarginTop => IsMobile ? 0 : IsTablet ? 2 : 12;
        internal int LoginLogoMarginBottom => IsMobile ? Mathf.RoundToInt(-10f * MobileScale) : IsTablet ? -8 : -10;
        internal int LoginHeroTitleFontSize => IsTablet ? 23 : 25;
        internal int LoginCardMinHeight => IsMobile ? Mathf.RoundToInt(108f * MobileScale) : IsTablet ? 140 : 152;
        internal int LoginCardPaddingTop => IsMobile ? Mathf.RoundToInt(8f * MobileScale) : IsTablet ? 14 : 16;
        internal int LoginCardPaddingBottom => IsMobile ? Mathf.RoundToInt(9f * MobileScale) : IsTablet ? 14 : 16;
        internal int LoginCardMarginBottom => IsMobile ? 0 : 18;
        internal Color LoginCardBackground => IsMobile
            ? new Color(0.005f, 0.018f, 0.040f, 0.18f)
            : IsTablet
                ? new Color(0.005f, 0.018f, 0.040f, 0.36f)
                : new Color(0.005f, 0.018f, 0.040f, 0.42f);
        internal StyleLength LoginServerRowMaxWidth => IsMobile ? new StyleLength(Length.Percent(100)) : new StyleLength(436f);
        internal int LoginServerRowMinHeight => IsMobile ? Mathf.RoundToInt(42f * MobileScale) : IsTablet ? 40 : 42;
        internal int LoginServerRowPaddingHorizontal => IsMobile ? 14 : 22;
        internal int LoginServerRowPaddingVertical => IsMobile ? 6 : 7;
        internal int LoginServerTextFontSize => IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.042f, 16f, 19f)) : IsTablet ? 18 : 19;
        internal int LoginButtonMarginTop => IsMobile ? Mathf.RoundToInt(5f * MobileScale) : 8;

        internal int LobbyIntroMarginBottom => IsMobile ? 6 : 10;
        internal int LobbyContentMarginTop => 4;
        internal int LobbyContentMarginBottom => IsMobile ? 8 : 10;
        internal int CharacterListMarginRight => IsMobile ? 10 : 14;
        internal int CharacterListMarginBottom => 8;
        internal int EmptyCharacterCardMarginTop => IsMobile ? 8 : 10;
        internal int SelectedPreviewHeroMarginBottom => 10;
        internal int CharacterPortraitMarginRight => 12;
        internal int LobbyPanelPaddingHorizontal => IsMobile ? 12 : 18;
        internal int LobbyPanelPaddingTop => IsMobile ? 8 : 16;
        internal int LobbyPanelPaddingBottom => IsMobile ? 8 : 18;
        internal RuntimeUiDensityProfile CharacterHallDensity => RuntimeUiDensityProfile.CharacterHall(this);
        internal int CreatePanelPaddingHorizontal => IsMobile ? 12 : 16;
        internal int CreatePanelPaddingTop => IsMobile ? 8 : 12;
        internal int CreatePanelPaddingBottom => IsMobile ? 8 : 14;
        internal int CreatePanelMarginTop => IsMobile ? 0 : 10;
        internal float WorldHudMinWidth => IsMobile ? 238f : 300f;
        internal int WorldHudPaddingHorizontal => IsMobile ? 8 : 12;
        internal int WorldHudPaddingVertical => IsMobile ? 6 : 10;
        internal int WorldHudDialoguePaddingHorizontal => IsMobile ? 7 : WorldHudPaddingHorizontal;
        internal int WorldHudDialoguePaddingVertical => IsMobile ? 5 : WorldHudPaddingVertical;
        internal int WorldNameMarginTop => 6;
        internal int PositionChipMarginTop => 8;
        internal int WorldLandmarksMarginTop => 8;
        internal int SkillPreviewPanelMarginTop => 10;
        internal int LocalCombatPanelMarginTop => 8;
        internal int SettingsPanelMarginTop => 12;
        internal int WorldGuidanceCardPaddingHorizontal => 8;
        internal int WorldGuidanceCardMarginVertical => IsMobile ? 6 : 8;
        internal int WorldGuidanceCardPaddingVertical => IsMobile ? 5 : 7;
        internal int DialoguePanelPaddingHorizontal => IsMobile ? 10 : 14;
        internal int DialoguePanelPaddingVertical => IsMobile ? 9 : 12;
        internal int DialoguePanelMarginTop => IsMobile ? 6 : IsTablet ? 8 : 10;
        internal int DialogueProgressPaddingHorizontal => 10;
        internal int DialogueProgressPaddingVertical => IsMobile ? 4 : 5;
        internal int StatusPaddingHorizontal(bool worldVisible) => worldVisible && IsMobile ? 14 : 18;
        internal int StatusPaddingVertical => 6;
        internal int PositionChipPaddingHorizontal => 10;
        internal int PositionChipPaddingVertical => 6;
        internal int LocalCombatPanelPaddingHorizontal => 12;
        internal int LocalCombatPanelPaddingVertical => 10;
        internal int SettingsPanelPaddingHorizontal => 14;
        internal int SettingsPanelPaddingTop => 12;
        internal int SettingsPanelPaddingBottom => 14;

        internal float WorldHudBaseMaxWidth => IsMobile
            ? Mathf.Clamp(Width * 0.28f, 238f, 272f)
            : IsTablet
                ? Mathf.Clamp(Width * 0.31f, 360f, 420f)
                : 390f;

        internal float WorldHudMaxWidth(bool dialogueVisible)
        {
            if (IsMobile)
                return dialogueVisible ? Mathf.Clamp(Width * 0.26f, 248f, 286f) : Mathf.Clamp(Width * 0.26f, 236f, 258f);
            if (IsTablet)
                return dialogueVisible ? Mathf.Clamp(Width * 0.30f, 350f, 400f) : Mathf.Clamp(Width * 0.31f, 360f, 420f);
            return 390f;
        }

        internal float WorldHudMaxHeight(bool dialogueVisible)
        {
            if (IsMobile) return Mathf.Max(260f, Height - 34f);
            if (IsTablet) return Mathf.Max(420f, Height - 80f);
            return 0f;
        }

        internal float SessionMenuWidth => IsMobile
            ? Mathf.Min(Mathf.Max(Width - 36f, 260f), 440f)
            : IsTablet
                ? Mathf.Clamp(Width * 0.62f, 620f, 820f)
                : Mathf.Clamp(Width * 0.50f, 760f, 960f);

        internal float SessionMenuRight => IsMobile ? 18f : Mathf.Max(24f, (Width - SessionMenuWidth) * 0.5f);

        internal float SessionMenuLeft => IsMobile ? 18f : Mathf.Max(24f, (Width - SessionMenuWidth) * 0.5f);

        internal float SessionMenuTop => IsMobile ? Mathf.Max(8f, Height * 0.06f) : IsTablet ? 118f : 120f;

        internal float SessionMenuMaxHeight => IsMobile ? Mathf.Max(240f, Height - 70f) : IsTablet ? 430f : 500f;
        internal int SessionMenuStatusMarginBottom => 10;
        internal int SessionMenuPaddingHorizontal => IsMobile ? 12 : IsTablet ? 16 : 22;
        internal int SessionMenuPaddingTop => IsMobile ? 10 : IsTablet ? 14 : 18;
        internal int SessionMenuPaddingBottom => IsMobile ? 10 : IsTablet ? 14 : 20;

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
                ? Mathf.Clamp(width * MobileLoginCardWidthRatio, 300f, 360f)
                : IsTablet
                    ? Mathf.Clamp(width * TabletLoginCardWidthRatio, 390f, 438f)
                    : 424f;
            LoginCardPadding = IsMobile ? Mathf.RoundToInt(13f * MobileScale) : IsTablet ? 20 : 24;
            LoginButtonHeight = IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.10f, 40f, 48f)) : IsTablet ? 50 : 52;
            LoginButtonFontSize = IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.044f, 16f, 20f)) : IsTablet ? 19 : 20;
        }

        internal static RuntimeUiLayoutProfile FromScreen(string forcedProfile, int screenWidth, int screenHeight, int layoutWidth = 0, int layoutHeight = 0)
        {
            var screenTargetWidth = screenWidth > 0 ? screenWidth : DefaultViewportWidth;
            var screenTargetHeight = screenHeight > 0 ? screenHeight : DefaultViewportHeight;
            var width = layoutWidth > 0 ? layoutWidth : screenTargetWidth;
            var height = layoutHeight > 0 ? layoutHeight : screenTargetHeight;
            var screenShortSide = Mathf.Min(screenTargetWidth, screenTargetHeight);
            var screenLongSide = Mathf.Max(screenTargetWidth, screenTargetHeight);
            var name = forcedProfile ?? (screenShortSide <= MobileMaxShortSide && screenLongSide <= MobileMaxLongSide
                ? "mobile"
                : screenShortSide <= TabletMaxShortSide && screenLongSide <= TabletMaxLongSide ? "tablet" : "desktop");
            return new RuntimeUiLayoutProfile(name, width, height);
        }
    }
}
