using System;
using LinhGioi.Art;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;
using static LinhGioi.UI.RuntimeUiFactory;

namespace LinhGioi.UI
{
    internal sealed class RuntimeNpcDialogueView
    {
        internal readonly VisualElement Panel, SpeakerHeader, Portrait, Body, Footer, ActionRow;
        internal readonly Label Speaker, Line, Progress;
        internal readonly ScrollView Scroll;
        internal readonly Button ContinueButton, CloseButton;

        internal RuntimeNpcDialogueView(RuntimeUiLayoutProfile layout, Action advance, Action close)
        {
            Panel = NewSectionShell(string.Empty, string.Empty, string.Empty, "LGO Dialogue Shell");
            SpeakerHeader = new VisualElement { name = "LGO Dialogue Speaker Header" };
            Portrait = NewRuntimeIcon(LgoVisualAssetRegistryV3B.GateKeeperPortrait, layout.DialogueSpeakerPortraitSize, "Người Giữ Cổng");
            Portrait.name = "LGO Dialogue Speaker Portrait V3B";
            Speaker = new Label("Người Giữ Cổng");
            RuntimeUiSkin.ApplyText(Speaker, RuntimeArtCatalog.Gold, RuntimeUiTypography.DialogueSpeakerInitialFontSize, true);
            Body = NewModalBody("LGO Dialogue Body");
            Footer = NewModalFooter("LGO Dialogue Footer");
            Scroll = new ScrollView(ScrollViewMode.Vertical) { name = "LGO Dialogue Line Scroll" };
            Line = NewMutedLabel("Đối thoại đã đóng.");
            Progress = NewStatusLabel("Đối thoại: 0/3", RuntimeArtCatalog.Muted);
            ContinueButton = NewCompactSecondaryButton("Tiếp tục", advance);
            CloseButton = NewQuietButton("Đóng", close);
            SpeakerHeader.Add(Portrait);
            SpeakerHeader.Add(Speaker);
            Panel.Add(SpeakerHeader);
            Scroll.Add(Line);
            Body.Add(Scroll);
            Panel.Add(Body);
            Footer.Add(Progress);
            ActionRow = NewActionRow("LGO Dialogue Action Row", Justify.FlexStart, 6, 0, ContinueButton, CloseButton);
            Footer.Add(ActionRow);
            Panel.Add(Footer);
            Panel.style.display = DisplayStyle.None;
            ApplyLayout(layout);
        }

        internal void ApplyLayout(RuntimeUiLayoutProfile layout) =>
            RuntimeWorldHudResponsiveLayout.ApplyDialogue(layout, Panel, SpeakerHeader, Portrait, Speaker,
                Body, Scroll, Footer, Line, Progress, ActionRow, ContinueButton, CloseButton);

        internal void Refresh(NpcDialogueSession session)
        {
            var resetReadingPosition = Panel.style.display != DisplayStyle.Flex || Line.text != session.Line;
            Panel.style.display = session.Active ? DisplayStyle.Flex : DisplayStyle.None;
            if (!session.Active) return;
            Panel.BringToFront();
            Speaker.text = session.Speaker;
            Line.text = session.Line;
            if (resetReadingPosition) Scroll.scrollOffset = Vector2.zero;
            Progress.text = "Đối thoại: " + session.Progress;
            ContinueButton.text = session.HasNext ? "Tiếp tục" : "Hoàn tất";
        }
    }
}
