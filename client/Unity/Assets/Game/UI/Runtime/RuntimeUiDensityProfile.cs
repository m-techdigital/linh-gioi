using UnityEngine;

namespace LinhGioi.UI
{
    internal readonly struct RuntimeUiDensityProfile
    {
        internal const string ComponentDensityMarker = "LGO Runtime UI Component Density Base v1";

        internal readonly int StatusMarginTop;
        internal readonly int StatusPaddingHorizontal;
        internal readonly int StatusPaddingVertical;
        internal readonly int ListPaddingHorizontal;
        internal readonly int ListPaddingVertical;
        internal readonly int EmptyCardMarginTop;
        internal readonly int EmptyCardPaddingHorizontal;
        internal readonly int EmptyCardPaddingVertical;

        private RuntimeUiDensityProfile(
            int statusMarginTop,
            int statusPaddingHorizontal,
            int statusPaddingVertical,
            int listPaddingHorizontal,
            int listPaddingVertical,
            int emptyCardMarginTop,
            int emptyCardPaddingHorizontal,
            int emptyCardPaddingVertical)
        {
            StatusMarginTop = statusMarginTop;
            StatusPaddingHorizontal = statusPaddingHorizontal;
            StatusPaddingVertical = statusPaddingVertical;
            ListPaddingHorizontal = listPaddingHorizontal;
            ListPaddingVertical = listPaddingVertical;
            EmptyCardMarginTop = emptyCardMarginTop;
            EmptyCardPaddingHorizontal = emptyCardPaddingHorizontal;
            EmptyCardPaddingVertical = emptyCardPaddingVertical;
        }

        internal static RuntimeUiDensityProfile CharacterHall(RuntimeUiLayoutProfile layout)
        {
            var mobile = layout.IsMobile;
            return new RuntimeUiDensityProfile(
                statusMarginTop: mobile ? RuntimeUiSpacing.StatusLabelMobileDensityMarginTop : RuntimeUiSpacing.StatusLabelMarginTop,
                statusPaddingHorizontal: mobile ? RuntimeUiSpacing.StatusLabelMobileDensityPaddingHorizontal : RuntimeUiSpacing.StatusLabelPaddingHorizontal,
                statusPaddingVertical: mobile ? RuntimeUiSpacing.StatusLabelMobileDensityPaddingVertical : RuntimeUiSpacing.StatusLabelPaddingVertical,
                listPaddingHorizontal: RuntimeUiSpacing.CharacterListDensityPaddingHorizontal,
                listPaddingVertical: mobile ? RuntimeUiSpacing.CharacterListMobileDensityPaddingVertical : RuntimeUiSpacing.CharacterListDesktopDensityPaddingVertical,
                emptyCardMarginTop: mobile ? RuntimeUiSpacing.EmptyCharacterCardMobileDensityMarginTop : RuntimeUiSpacing.EmptyCharacterCardDesktopDensityMarginTop,
                emptyCardPaddingHorizontal: mobile ? RuntimeUiSpacing.EmptyCharacterCardMobileDensityPaddingHorizontal : RuntimeUiSpacing.EmptyCharacterCardDesktopDensityPaddingHorizontal,
                emptyCardPaddingVertical: mobile ? RuntimeUiSpacing.EmptyCharacterCardMobileDensityPaddingVertical : RuntimeUiSpacing.EmptyCharacterCardDesktopDensityPaddingVertical);
        }
    }
}
