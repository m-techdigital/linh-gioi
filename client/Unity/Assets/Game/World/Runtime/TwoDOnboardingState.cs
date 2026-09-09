using System;
using UnityEngine;

namespace LinhGioi.World
{
    public enum TwoDOnboardingStep
    {
        FindGateKeeper,
        TalkToGateKeeper,
        GoToTrainingStone,
        ActivateTrainingStone,
        Complete
    }

    public enum TwoDOnboardingAction
    {
        None,
        Talk,
        Continue,
        Train
    }

    public sealed class TwoDOnboardingState
    {
        public const float FocusRange = 1.15f;
        public static readonly Vector2 PlayerStart = new Vector2(-3.6f, -1.25f);
        public static readonly Vector2 GateKeeperPosition = new Vector2(-1.35f, -0.55f);
        public static readonly Vector2 TrainingStonePosition = new Vector2(2.45f, -0.35f);

        public Vector2 PlayerPosition { get; private set; } = PlayerStart;
        public TwoDOnboardingStep Step { get; private set; } = TwoDOnboardingStep.FindGateKeeper;
        public TwoDOnboardingAction AvailableAction { get; private set; }
        public bool DialogueOpen { get; private set; }
        public string DialogueLine { get; private set; } = string.Empty;
        public string ObjectiveText { get; private set; } = "Tới gặp Người Giữ Cổng ở Cổng Linh Thành.";
        public string HintText { get; private set; } = "Di chuyển bằng WASD/phím mũi tên. Lại gần NPC để trò chuyện.";
        public string AreaText { get; private set; } = "Cổng Linh Thành";
        public string FeedbackText { get; private set; } = "Bình minh xanh phủ trên cổng thành.";

        public void Reset()
        {
            PlayerPosition = PlayerStart;
            Step = TwoDOnboardingStep.FindGateKeeper;
            AvailableAction = TwoDOnboardingAction.None;
            DialogueOpen = false;
            DialogueLine = string.Empty;
            ObjectiveText = "Tới gặp Người Giữ Cổng ở Cổng Linh Thành.";
            HintText = "Di chuyển bằng WASD/phím mũi tên. Lại gần NPC để trò chuyện.";
            AreaText = "Cổng Linh Thành";
            FeedbackText = "Bình minh xanh phủ trên cổng thành.";
            Refresh();
        }

        public void Move(Vector2 delta)
        {
            if (!IsFinite(delta)) return;
            PlayerPosition = ClampToPlayableArea(PlayerPosition + delta);
            Refresh();
        }

        public bool TryUseAction()
        {
            Refresh();
            switch (AvailableAction)
            {
                case TwoDOnboardingAction.Talk:
                    Step = TwoDOnboardingStep.TalkToGateKeeper;
                    DialogueOpen = true;
                    DialogueLine = "Chào mừng đến Linh Thành. Hãy chạm vào Bia Luyện Khí để ổn định linh lực đầu tiên.";
                    ObjectiveText = "Lắng nghe Người Giữ Cổng.";
                    HintText = "Bấm E hoặc nút hành động để nhận chỉ dẫn.";
                    FeedbackText = "Người Giữ Cổng cúi chào, ánh ngọc sáng nhẹ quanh cổng.";
                    AvailableAction = TwoDOnboardingAction.Continue;
                    return true;
                case TwoDOnboardingAction.Continue:
                    Step = TwoDOnboardingStep.GoToTrainingStone;
                    DialogueOpen = false;
                    DialogueLine = string.Empty;
                    ObjectiveText = "Tới Bia Luyện Khí trong sân phía đông.";
                    HintText = "Đi theo ánh ngọc tới bia đá.";
                    FeedbackText = "Một vệt sáng dẫn về phía Bia Luyện Khí.";
                    Refresh();
                    return true;
                case TwoDOnboardingAction.Train:
                    Step = TwoDOnboardingStep.Complete;
                    DialogueOpen = false;
                    DialogueLine = string.Empty;
                    ObjectiveText = "Hoàn tất nhập môn: Linh lực đã cộng hưởng.";
                    HintText = "Bạn đã sẵn sàng bước sâu hơn vào Linh Thành.";
                    FeedbackText = "Bia Luyện Khí sáng lên, một vòng linh quang lan dưới chân.";
                    AvailableAction = TwoDOnboardingAction.None;
                    return true;
                default:
                    return false;
            }
        }

        public void Refresh()
        {
            if (Step == TwoDOnboardingStep.Complete)
            {
                AvailableAction = TwoDOnboardingAction.None;
                AreaText = "Sân Luyện Khí";
                return;
            }

            if (Step == TwoDOnboardingStep.TalkToGateKeeper)
            {
                AvailableAction = TwoDOnboardingAction.Continue;
                AreaText = "Cổng Linh Thành";
                return;
            }

            var nearGateKeeper = Vector2.Distance(PlayerPosition, GateKeeperPosition) <= FocusRange;
            var nearTrainingStone = Vector2.Distance(PlayerPosition, TrainingStonePosition) <= FocusRange;

            if (Step == TwoDOnboardingStep.FindGateKeeper)
            {
                AvailableAction = nearGateKeeper ? TwoDOnboardingAction.Talk : TwoDOnboardingAction.None;
                AreaText = "Cổng Linh Thành";
                ObjectiveText = nearGateKeeper ? "Nói chuyện với Người Giữ Cổng." : "Tới gặp Người Giữ Cổng ở Cổng Linh Thành.";
                HintText = nearGateKeeper ? "Bấm E để trò chuyện." : "Di chuyển bằng WASD/phím mũi tên. Lại gần NPC để trò chuyện.";
                return;
            }

            AvailableAction = nearTrainingStone ? TwoDOnboardingAction.Train : TwoDOnboardingAction.None;
            AreaText = "Sân Luyện Khí";
            ObjectiveText = nearTrainingStone ? "Kích hoạt Bia Luyện Khí." : "Tới Bia Luyện Khí trong sân phía đông.";
            HintText = nearTrainingStone ? "Bấm E để cộng hưởng linh lực." : "Đi theo ánh ngọc tới bia đá.";
        }

        private static Vector2 ClampToPlayableArea(Vector2 value)
        {
            return new Vector2(Mathf.Clamp(value.x, -4.25f, 4.25f), Mathf.Clamp(value.y, -2.15f, 1.85f));
        }

        private static bool IsFinite(Vector2 value)
        {
            return IsFinite(value.x) && IsFinite(value.y);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
