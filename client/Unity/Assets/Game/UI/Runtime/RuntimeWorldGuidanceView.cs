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
            Panel = RuntimeUiFactory.NewFlexibleColumn("LGO World Guidance Card V3B");
            Area = RuntimeUiFactory.NewWorldHudText("Khu vực: xem trước tại sảnh", RuntimeArtCatalog.Muted, RuntimeUiTypography.WorldAreaFontSize);
            Step = RuntimeUiFactory.NewWorldHudText("Tiến trình: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện.", RuntimeArtCatalog.Text, RuntimeUiTypography.WorldStepFontSize);
            Direction = RuntimeUiFactory.NewWorldHudText("Chỉ dẫn: vào sân để hiện mốc gần nhất.", RuntimeArtCatalog.Gold, RuntimeUiTypography.WorldDirectionFontSize);
            Objective = RuntimeUiFactory.NewWorldHudText("Mục tiêu: gặp Người Giữ Cổng.", RuntimeArtCatalog.Gold, RuntimeUiTypography.WorldObjectiveInitialFontSize);
            Objective.name = "LGO World Objective Touch Priority";
            Hint = RuntimeUiFactory.NewWorldHudText("Di chuyển tới gần Người Giữ Cổng.", RuntimeArtCatalog.Text, RuntimeUiTypography.WorldInteractionInitialFontSize);
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
