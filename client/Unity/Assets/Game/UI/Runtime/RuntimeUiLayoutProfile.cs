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
        internal const int CompactMaxShortSide = 600;
        internal const int ShortHeightMax = 620;
        internal const int NarrowMaxShortSide = 760;
        internal const int NarrowMaxLongSide = 1180;
        internal const int RegularMaxShortSide = 980;
        internal const int RegularMaxLongSide = 1500;
        internal const float MobileScaleBaseline = 520f;
        internal const float MobileScaleMin = 0.50f;
        internal const float MobileScaleMax = 0.86f;
        internal const float MobileLoginLogoWidthRatio = 0.58f;
        internal const float TabletLoginLogoWidthRatio = 0.58f;
        internal const float DesktopLoginLogoWidthRatio = 0.54f;
        internal const float MobileLoginCardWidthRatio = 0.70f;
        internal const float TabletLoginCardWidthRatio = 0.74f;
        internal const float LoginLogoAspect = 0.50f;

        internal readonly string Name;
        internal readonly int Width;
        internal readonly int Height;
        internal readonly int ShortSide;
        internal readonly bool IsMobile;
        internal readonly bool IsTablet;
        internal readonly string LayoutClass;
        internal readonly string InputClass;
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
        internal StyleLength MainShellMaxWidth(bool worldVisible) => worldVisible || IsMobile
            ? new StyleLength(Length.Percent(100))
            : new StyleLength(IsTablet ? 980f : RuntimeUiSizing.MainShellMaxWidth);
        internal Justify MainShellJustifyContent(bool worldVisible) => worldVisible || IsMobile ? Justify.FlexStart : Justify.Center;
        internal int HeaderMinHeight(bool authVisible) => authVisible && IsMobile ? 8 : IsMobile ? 34 : 76;
        internal int AuthPanelMinHeight => IsMobile
            ? Mathf.Max(0, Height - 18)
            : IsTablet
                ? Mathf.RoundToInt(Mathf.Clamp(Height - 22f, 300f, 500f))
                : Mathf.RoundToInt(Mathf.Clamp(Height - 24f, 320f, 560f));
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
        internal float LoginMobileControlColumnWidth => Mathf.Clamp(Width * 0.42f, 310f, 380f);
        internal float LoginMobileControlColumnInsetHorizontal => Mathf.Max(RootPaddingHorizontal, (Width - LoginMobileControlColumnWidth) * 0.5f);
        internal float LoginMobileControlColumnTop => Mathf.Clamp(Height * 0.08f, 28f, 48f);
        internal float LoginMobileControlColumnMaxHeight => Mathf.Clamp(Height * 0.44f, 210f, 280f);
        internal Length LoginControlColumnWidth => IsMobile ? new Length(LoginMobileControlColumnWidth) : IsTablet ? Length.Percent(56) : Length.Percent(54);
        internal int LoginControlColumnMinWidth => IsMobile ? 0 : 300;
        internal int LoginControlColumnMaxWidth => IsMobile ? 500 : IsTablet ? 540 : 600;
        internal int LoginControlColumnPaddingBottom => IsMobile ? 0 : 12;
        internal int LoginControlColumnMarginLeft => IsMobile ? 0 : IsTablet ? 8 : 22;
        internal int LoginControlColumnMarginTop => IsMobile ? 0 : IsTablet ? 2 : 12;
        internal int LoginLogoMarginBottom => IsMobile ? Mathf.RoundToInt(-10f * MobileScale) : IsTablet ? -8 : -10;
        internal int LoginHeroTitleFontSize => IsTablet ? 23 : 25;
        internal int LoginCardMinHeight => IsMobile ? Mathf.RoundToInt(84f * MobileScale) : IsTablet ? 132 : 140;
        internal int LoginCardPaddingTop => IsMobile ? Mathf.RoundToInt(5f * MobileScale) : IsTablet ? 14 : 16;
        internal int LoginCardPaddingBottom => IsMobile ? Mathf.RoundToInt(5f * MobileScale) : IsTablet ? 14 : 16;
        internal int LoginCardMarginBottom => IsMobile ? 0 : 18;
        internal Color LoginCardBackground => IsMobile
            ? new Color(0.005f, 0.018f, 0.040f, 0.18f)
            : IsTablet
                ? new Color(0.005f, 0.018f, 0.040f, 0.36f)
                : new Color(0.005f, 0.018f, 0.040f, 0.42f);
        internal StyleLength LoginServerRowMaxWidth => IsMobile ? new StyleLength(Length.Percent(100)) : new StyleLength(436f);
        internal int LoginServerRowMinHeight => IsMobile ? Mathf.RoundToInt(34f * MobileScale) : IsTablet ? 40 : 42;
        internal int LoginServerRowPaddingHorizontal => IsMobile ? 14 : 22;
        internal int LoginServerRowPaddingVertical => IsMobile ? 3 : 7;
        internal int LoginServerTextFontSize => IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.055f, 11f, 15f)) : IsTablet ? 18 : 19;
        internal int LoginButtonMarginTop => IsMobile ? Mathf.RoundToInt(2f * MobileScale) : 8;

        internal int LobbyIntroMarginBottom => IsMobile ? 6 : 10;
        internal int LobbyContentMarginTop => 4;
        internal int LobbyContentMarginBottom => IsMobile ? 8 : 10;
        internal int CharacterListMarginRight => IsMobile ? 10 : 14;
        internal int CharacterListMarginBottom => 8;
        internal int EmptyCharacterCardMarginTop => IsMobile ? 8 : 10;
        internal int SelectedPreviewHeroMarginBottom => 10;
        internal int CharacterPortraitMarginRight => 12;
        internal float CharacterSelectedPreviewMaxWidth => IsMobile
            ? Mathf.Clamp(Width * 0.36f, 300f, 360f)
            : IsTablet ? Mathf.Clamp(Width * 0.34f, 390f, 430f) : Mathf.Clamp(Width * 0.30f, 520f, 580f);
        internal float CharacterSelectedPreviewHeight => IsMobile || IsTablet ? 0f : Mathf.Clamp(Height * 0.42f, 390f, 460f);
        internal float CharacterSelectedListMaxWidth => IsMobile ? Mathf.Clamp(Width * 0.30f, 236f, 270f) : IsTablet ? 340 : RuntimeUiSizing.CharacterListMaxWidth;
        internal int CharacterPortraitWidth => IsMobile ? 128 : IsTablet ? 156 : 178;
        internal int CharacterPortraitHeight => IsMobile ? 172 : IsTablet ? 214 : 244;
        internal int SelectedCharacterNameFontSize => IsMobile ? 17 : IsTablet ? RuntimeUiTypography.SelectedCharacterNameTabletFontSize : 23;
        internal int LobbyPanelPaddingHorizontal => IsMobile ? 12 : 18;
        internal int LobbyPanelPaddingTop => IsMobile ? 8 : 16;
        internal int LobbyPanelPaddingBottom => IsMobile ? 8 : 18;
        internal float CharacterHallPanelVerticalInset => IsMobile ? OverlayBottomInset : Mathf.Clamp(Height * 0.04f, 34f, 58f);
        internal float CharacterHallPanelMaxHeight => IsMobile ? Mathf.Max(360f, Height - CharacterHallPanelVerticalInset * 2f) : 0f;
        internal RuntimeUiDensityProfile CharacterHallDensity => RuntimeUiDensityProfile.CharacterHall(this);
        internal int CreatePanelPaddingHorizontal => IsMobile ? 12 : 16;
        internal int CreatePanelPaddingTop => IsMobile ? 8 : 12;
        internal int CreatePanelPaddingBottom => IsMobile ? 8 : 14;
        internal int CreatePanelMarginTop => IsMobile ? 0 : 10;
        internal float CharacterHallCreateOverlayWidth => Mathf.Clamp(Width * 0.36f, 320f, 360f);
        internal float CharacterHallCreateOverlayRight => Mathf.Clamp(Width * 0.08f, 12f, 28f);
        internal float CharacterHallCreateOverlayTop => Mathf.Clamp(Height * 0.24f, 112f, 140f);
        internal float CharacterHallSelectedDockWidth => Mathf.Clamp(Width * 0.34f, 300f, 336f);
        internal float CharacterHallSelectedDockRight => Mathf.Clamp(Width * 0.06f, 14f, 28f);
        internal float OverlayBottomInset => IsMobile ? Mathf.Clamp(Height * 0.085f, 36f, 52f) : Mathf.Clamp(Height * 0.06f, 34f, 64f);
        internal float CharacterHallSelectedDockBottom => OverlayBottomInset;
        internal float WorldHudMinWidth => WorldHudMinWidthFor(false);

        internal float WorldHudMinWidthFor(bool dialogueVisible)
        {
            if (IsMobile) return dialogueVisible ? 232f : 180f;
            return 300f;
        }
        internal int WorldHudPaddingHorizontal => IsMobile ? 6 : IsTablet ? 12 : 10;
        internal int WorldHudPaddingVertical => IsMobile ? 4 : IsTablet ? 10 : 8;
        internal int WorldHudDialoguePaddingHorizontal => IsMobile ? 6 : WorldHudPaddingHorizontal;
        internal int WorldHudDialoguePaddingVertical => IsMobile ? 4 : WorldHudPaddingVertical;
        internal int WorldNameMarginTop => 6;
        internal int PositionChipMarginTop => 8;
        internal int WorldLandmarksMarginTop => 8;
        internal int SkillPreviewPanelMarginTop => 10;
        internal int LocalCombatPanelMarginTop => 8;
        internal int SettingsPanelMarginTop => 12;
        internal int WorldGuidanceCardPaddingHorizontal => 8;
        internal int WorldGuidanceCardMarginVertical => IsMobile ? 4 : 8;
        internal int WorldGuidanceCardPaddingVertical => IsMobile ? 4 : 7;
        internal int DialoguePanelPaddingHorizontal => IsMobile ? 10 : 14;
        internal int DialoguePanelPaddingVertical => IsMobile ? 9 : 12;
        internal int DialoguePanelMarginVertical => Mathf.RoundToInt(Mathf.Clamp(Height * (IsMobile ? 0.012f : 0.014f), IsMobile ? 6f : 10f, IsMobile ? 10f : 16f));
        internal int DialoguePanelMarginTop => DialoguePanelMarginVertical;
        internal float DialogueOverlayWidth => IsMobile
            ? Mathf.Clamp(Width * 0.42f, 330f, 430f)
            : IsTablet
                ? Mathf.Clamp(Width * 0.46f, 500f, 680f)
                : Mathf.Clamp(Width * 0.34f, 420f, 560f);
        internal float DialoguePanelMaxHeight => IsMobile
            ? Mathf.Clamp(Height * 0.50f, 210f, 270f)
            : IsTablet
                ? Mathf.Clamp(Height * 0.48f, 300f, 390f)
                : Mathf.Clamp(Height * 0.52f, 360f, 520f);
        internal float DialogueOverlayInsetHorizontal => Mathf.Max(12f, (Width - DialogueOverlayWidth) * 0.5f);
        internal float DialogueOverlayInsetVertical => Mathf.Max(DialoguePanelMarginVertical, (Height - DialoguePanelMaxHeight) * 0.5f);
        internal int DialogueContentGap => IsMobile ? 6 : IsTablet ? 8 : 10;
        internal float DialogueLineScrollMinHeight => IsMobile
            ? Mathf.Clamp(Height * 0.08f, 38f, 46f)
            : IsTablet
                ? Mathf.Clamp(Height * 0.10f, 78f, 104f)
                : Mathf.Clamp(Height * 0.10f, 92f, 128f);
        internal float DialogueLineScrollMaxHeight => IsMobile
            ? Mathf.Clamp(Height * 0.12f, 50f, 64f)
            : IsTablet
                ? Mathf.Clamp(Height * 0.16f, 108f, 148f)
                : Mathf.Clamp(Height * 0.16f, 124f, 176f);
        internal int DialogueProgressPaddingHorizontal => 10;
        internal int DialogueProgressPaddingVertical => IsMobile ? 4 : 5;
        internal int DialogueSpeakerPortraitSize => IsMobile ? 34 : IsTablet ? 38 : 42;
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
            ? Mathf.Clamp(Width * 0.54f, 180f, 214f)
            : IsTablet
                ? Mathf.Clamp(Width * 0.31f, 360f, 420f)
                : 342f;

        internal float WorldHudMaxWidth(bool dialogueVisible)
        {
            if (IsMobile)
                return dialogueVisible ? Mathf.Clamp(Width * 0.68f, 260f, 320f) : Mathf.Clamp(Width * 0.54f, 180f, 214f);
            if (IsTablet)
                return dialogueVisible ? Mathf.Clamp(Width * 0.30f, 350f, 400f) : Mathf.Clamp(Width * 0.31f, 360f, 420f);
            return dialogueVisible ? 390f : 342f;
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

        internal float SessionMenuInsetHorizontal => Mathf.Max(IsMobile ? 12f : 24f, (Width - SessionMenuWidth) * 0.5f);

        internal float SessionMenuRight => SessionMenuInsetHorizontal;

        internal float SessionMenuLeft => SessionMenuInsetHorizontal;

        internal float SessionMenuInsetVertical => IsMobile ? Mathf.Clamp(Height * 0.06f, 24f, 42f) : IsTablet ? 118f : 120f;

        internal float SessionMenuTop => SessionMenuInsetVertical;

        internal float SessionMenuMaxHeight => IsMobile ? Mathf.Max(240f, Height - SessionMenuInsetVertical * 2f) : IsTablet ? 430f : 500f;
        internal bool SessionMenuShowsSettings => !IsMobile && !IsTablet && Height >= 1180;
        internal int SessionMenuStatusMarginBottom => 10;
        internal int SessionMenuPaddingHorizontal => IsMobile ? 12 : IsTablet ? 16 : 22;
        internal int SessionMenuPaddingTop => IsMobile ? 10 : IsTablet ? 14 : 18;
        internal int SessionMenuPaddingBottom => IsMobile ? 10 : IsTablet ? 14 : 20;

        private RuntimeUiLayoutProfile(string name, int width, int height, string layoutClass = null, string inputClass = null)
        {
            Name = name;
            Width = width;
            Height = height;
            ShortSide = Mathf.Min(width, height);
            IsMobile = name == "mobile";
            IsTablet = name == "tablet";
            LayoutClass = layoutClass ?? name;
            InputClass = inputClass ?? (IsMobile || IsTablet ? "touch" : "pointer");
            MobileScale = IsMobile ? Mathf.Clamp(ShortSide / MobileScaleBaseline, MobileScaleMin, MobileScaleMax) : 1f;
            LoginLogoWidth = IsMobile
                ? Mathf.Clamp(Mathf.Min(width * MobileLoginLogoWidthRatio, height * 0.65f), 170f, 260f)
                : IsTablet
                    ? Mathf.Clamp(Mathf.Min(width * TabletLoginLogoWidthRatio, height * 0.78f), 220f, 320f)
                    : Mathf.Clamp(Mathf.Min(width * DesktopLoginLogoWidthRatio, height * 0.70f), 260f, 360f);
            LoginLogoHeight = LoginLogoWidth * LoginLogoAspect;
            LoginCardWidth = IsMobile
                ? Mathf.Clamp(width * MobileLoginCardWidthRatio, 300f, 360f)
                : IsTablet
                    ? Mathf.Clamp(width * TabletLoginCardWidthRatio, 330f, 420f)
                    : 424f;
            LoginCardPadding = IsMobile ? Mathf.RoundToInt(13f * MobileScale) : IsTablet ? 20 : 24;
            LoginButtonHeight = IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.13f, 26f, 36f)) : IsTablet ? 50 : 52;
            LoginButtonFontSize = IsMobile ? Mathf.RoundToInt(Mathf.Clamp(ShortSide * 0.055f, 11f, 15f)) : IsTablet ? 19 : 20;
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

        internal static RuntimeUiLayoutProfile FromViewport(RuntimeViewportMetrics viewport)
        {
            var name = viewport.LayoutClass == "compact" || viewport.LayoutClass == "narrow"
                ? "mobile"
                : viewport.LayoutClass == "regular" ? "tablet" : "desktop";
            if (viewport.LayoutClass == "mobile" || viewport.LayoutClass == "tablet" || viewport.LayoutClass == "desktop")
                name = viewport.LayoutClass;
            return new RuntimeUiLayoutProfile(
                name,
                Mathf.Max(1, Mathf.RoundToInt(viewport.SafePanelRect.width)),
                Mathf.Max(1, Mathf.RoundToInt(viewport.SafePanelRect.height)),
                viewport.LayoutClass,
                viewport.InputClass);
        }
    }
}
