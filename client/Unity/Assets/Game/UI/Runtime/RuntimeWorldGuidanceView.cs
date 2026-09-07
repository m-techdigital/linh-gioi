using LinhGioi.Art;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    internal sealed class RuntimeWorldGuidanceView
    {
        internal readonly VisualElement Panel;
        internal readonly Label Area;
        internal readonly Label Step;
        internal readonly Label Direction;
        internal readonly Label Objective;
        internal readonly Label Hint;

        internal RuntimeWorldGuidanceView()
        {
            Panel = RuntimeUiFactory.NewWorldHudGroup("LGO World Guidance Card V3B", RuntimeArtCatalog.Spirit);
            Area = RuntimeUiFactory.NewCompactStatusLabel("Khu vực: xem trước tại sảnh", RuntimeArtCatalog.Muted, RuntimeUiTypography.WorldAreaFontSize);
            Step = RuntimeUiFactory.NewCompactStatusLabel("Tiến trình: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện.", RuntimeArtCatalog.Spirit, RuntimeUiTypography.WorldStepFontSize);
            Direction = RuntimeUiFactory.NewCompactStatusLabel("Chỉ dẫn: vào sân để hiện mốc gần nhất.", RuntimeArtCatalog.Gold, RuntimeUiTypography.WorldDirectionFontSize);
            Objective = RuntimeUiFactory.NewCompactStatusLabel("Mục tiêu: gặp Người Giữ Cổng.", RuntimeArtCatalog.Gold, RuntimeUiTypography.WorldObjectiveInitialFontSize);
            Objective.name = "LGO World Objective Touch Priority";
            Hint = RuntimeUiFactory.NewCompactStatusLabel("Di chuyển tới gần Người Giữ Cổng.", RuntimeArtCatalog.Spirit, RuntimeUiTypography.WorldInteractionInitialFontSize);
            Hint.name = "LGO World Interaction Touch Hint";
            Panel.Add(Area);
            Panel.Add(Step);
            Panel.Add(Direction);
            Panel.Add(Objective);
            Panel.Add(Hint);
        }

        internal void ApplyTypography(RuntimeUiLayoutProfile layout)
        {
            Objective.style.fontSize = layout.IsMobile ? RuntimeUiTypography.WorldObjectiveMobileFontSize : RuntimeUiTypography.WorldObjectiveDesktopFontSize;
            Hint.style.fontSize = layout.IsMobile ? RuntimeUiTypography.WorldInteractionMobileFontSize : RuntimeUiTypography.WorldInteractionDesktopFontSize;
        }
    }
}
