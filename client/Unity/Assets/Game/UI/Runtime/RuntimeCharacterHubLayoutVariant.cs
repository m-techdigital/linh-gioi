using UnityEngine;

namespace LinhGioi.UI
{
    /// <summary>
    /// Character Hub-specific responsive composition.
    /// The Hub is the UIF-02R pilot: responsive composition changes here do not
    /// implicitly rescale Auth, Character Select, gameplay HUD, or world UI.
    /// </summary>
    internal readonly struct RuntimeCharacterHubLayoutVariant
    {
        internal readonly string Name;
        internal readonly float MaximumShellHeight;
        internal readonly float InspectorShare;
        internal readonly float ColumnGapShare;
        internal readonly float TabsInsetShare;
        internal readonly float TabsWidthShare;
        internal readonly float TouchTargetScale;

        private RuntimeCharacterHubLayoutVariant(
            string name,
            float maximumShellHeight,
            float inspectorShare,
            float columnGapShare,
            float tabsInsetShare,
            float tabsWidthShare)
        {
            Name = name;
            MaximumShellHeight = maximumShellHeight;
            InspectorShare = inspectorShare;
            ColumnGapShare = columnGapShare;
            TabsInsetShare = tabsInsetShare;
            TabsWidthShare = tabsWidthShare;
            TouchTargetScale = 1f;
        }

        internal static RuntimeCharacterHubLayoutVariant FromWindow(
            string windowClass, string inputClass, int width, int height)
        {
            var adaptive = RuntimeUiAdaptiveProfile.FromWindow(windowClass, inputClass, width, height);
            var safeHeight = Mathf.Max(1f, height);

            if (adaptive.PresentationClass == "MobileLandscape")
            {
                return Create(
                    "MobileLandscape",
                    safeHeight,
                    RuntimeUiSizing.CharacterHubMobileLandscapeHeightOccupancy,
                    RuntimeUiSizing.CharacterHubMobileLandscapeInspectorShare,
                    RuntimeUiSizing.CharacterHubMobileLandscapeColumnGapShare,
                    RuntimeUiSizing.CharacterHubMobileLandscapeTabsInsetShare,
                    RuntimeUiSizing.CharacterHubMobileLandscapeTabsWidthShare);
            }
            if (adaptive.PresentationClass == "Tablet")
            {
                return Create(
                    "Tablet",
                    safeHeight,
                    RuntimeUiSizing.CharacterHubTabletHeightOccupancy,
                    RuntimeUiSizing.CharacterHubTabletInspectorShare,
                    RuntimeUiSizing.CharacterHubTabletColumnGapShare,
                    RuntimeUiSizing.CharacterHubTabletTabsInsetShare,
                    RuntimeUiSizing.CharacterHubTabletTabsWidthShare);
            }

            return Create(
                "Wide",
                safeHeight,
                RuntimeUiSizing.CharacterHubWideHeightOccupancy,
                RuntimeUiSizing.CharacterHubWideInspectorShare,
                RuntimeUiSizing.CharacterHubWideColumnGapShare,
                RuntimeUiSizing.CharacterHubWideTabsInsetShare,
                RuntimeUiSizing.CharacterHubWideTabsWidthShare);
        }

        private static RuntimeCharacterHubLayoutVariant Create(
            string name,
            float safeHeight,
            float heightOccupancy,
            float inspectorShare,
            float columnGapShare,
            float tabsInsetShare,
            float tabsWidthShare)
        {
            var maximumShellHeight = Mathf.Min(
                RuntimeUiSizing.CharacterHubCanonicalShellHeight,
                safeHeight * heightOccupancy);
            return new RuntimeCharacterHubLayoutVariant(
                name,
                maximumShellHeight,
                inspectorShare,
                columnGapShare,
                tabsInsetShare,
                tabsWidthShare);
        }

        internal Vector3 CalculateColumns(float shellWidth)
        {
            var bodyInset = shellWidth * RuntimeUiSizing.CharacterHubBodyHorizontalInsetShare;
            var available = Mathf.Max(0f, shellWidth - bodyInset);
            var gap = available * ColumnGapShare;
            var columnSpace = Mathf.Max(0f, available - gap);
            var detail = columnSpace * InspectorShare;
            var main = Mathf.Max(0f, columnSpace - detail);
            return new Vector3(main, detail, gap);
        }
    }
}
