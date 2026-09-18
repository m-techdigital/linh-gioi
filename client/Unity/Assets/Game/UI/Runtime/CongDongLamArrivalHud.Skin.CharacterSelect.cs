using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private static void ApplyLgoCharacterSelectOverlay(VisualElement overlay)
        {
            overlay.AddToClassList(LgoCharacterSelectOverlayClass);
        }

        private static void ApplyLgoCharacterSelectBrand(VisualElement brand)
        {
            brand.AddToClassList(LgoCharacterSelectBrandClass);
        }

        private static void ApplyLgoCharacterSelectStage(VisualElement stage)
        {
            stage.AddToClassList(LgoCharacterSelectStageClass);
        }

        private static void ApplyLgoCharacterSelectPanel(VisualElement panel)
        {
            panel.AddToClassList(LgoCharacterSelectPanelClass);
            ApplyLgoModalChrome(panel, 14);
            panel.style.minWidth = 0;
            panel.style.minHeight = 0;
            panel.style.backgroundColor = new Color(.006f, .026f, .052f, .94f);
        }

        private static void ApplyLgoCharacterSelectDetail(VisualElement detail)
        {
            detail.AddToClassList(LgoCharacterSelectDetailClass);
        }

        private static void ApplyLgoCharacterSelectActions(VisualElement actions)
        {
            actions.AddToClassList(LgoCharacterSelectActionsClass);
        }

        private static void ApplyLgoCharacterSelectProfile(Button profile, bool selected)
        {
            profile.AddToClassList(LgoCharacterSelectProfileClass);
            ApplyLgoButton(profile);
            profile.style.position = Position.Relative;
            profile.style.flexGrow = 0;
            profile.style.flexShrink = 0;
            profile.style.height = 84;
            profile.style.marginBottom = 7;
            profile.style.paddingLeft = 92;
            profile.style.fontSize = 18;
            profile.style.unityTextAlign = TextAnchor.MiddleLeft;
            profile.style.whiteSpace = WhiteSpace.NoWrap;
            if (selected)
            {
                profile.style.backgroundColor = new Color(.035f, .17f, .30f, .98f);
                profile.style.borderLeftWidth = profile.style.borderRightWidth = 2;
                profile.style.borderTopWidth = profile.style.borderBottomWidth = 2;
                profile.style.borderLeftColor = profile.style.borderRightColor = UiGold;
                profile.style.borderTopColor = profile.style.borderBottomColor = UiGold;
            }
        }

        private static void ApplyLgoCharacterSelectEmptySlot(Button slot)
        {
            slot.AddToClassList(LgoCharacterSelectEmptySlotClass);
            ApplyLgoButton(slot);
            slot.style.position = Position.Relative;
            slot.style.flexGrow = 0;
            slot.style.flexShrink = 0;
            slot.style.height = 66;
            slot.style.marginBottom = 6;
            slot.style.paddingLeft = 84;
            slot.style.fontSize = 13;
            slot.style.color = UiSubText;
            slot.style.unityTextAlign = TextAnchor.MiddleLeft;
            slot.style.whiteSpace = WhiteSpace.Normal;
        }

        private static void ApplyLgoCharacterSelectIcon(VisualElement icon, Sprite sprite, float size)
        {
            icon.style.width = size;
            icon.style.height = size;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            icon.style.opacity = sprite == null ? .25f : .92f;
        }

        private static void ApplyLgoCharacterSelectSecondaryAction(Button button)
        {
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.flexShrink = 0;
            button.style.height = 44;
            button.style.fontSize = 12;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.marginBottom = 0;
        }

        private static void ApplyLgoCharacterSelectStatus(Label status)
        {
            status.style.flexGrow = 0;
            status.style.minHeight = 26;
            status.style.marginTop = 5;
            status.style.paddingTop = status.style.paddingBottom = 4;
            status.style.color = new Color(.70f, .88f, .96f, .92f);
            status.style.whiteSpace = WhiteSpace.Normal;
        }

        private static void ApplyLgoCharacterSelectAction(Button button, bool primary = false)
        {
            button.AddToClassList(primary ? LgoCharacterSelectEnterClass : LgoCharacterSelectUtilityClass);
            ApplyLgoButton(button, primary);
            button.style.flexGrow = primary ? 1 : 0;
            button.style.flexBasis = primary ? 0 : StyleKeyword.Auto;
            button.style.minWidth = primary ? 0 : 82;
            button.style.height = primary ? 46 : 40;
            button.style.fontSize = primary ? 16 : 12;
            button.style.whiteSpace = WhiteSpace.NoWrap;
        }

        private static void ApplyLgoCharacterSelectServerRow(VisualElement row)
        {
            row.AddToClassList(LgoCharacterSelectServerRowClass);
            ApplyLgoStatusCard(row, 9, 6);
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.flexGrow = 0;
            row.style.minHeight = 42;
        }

        private static void ApplyLgoCharacterSelectServerAction(Button button)
        {
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.height = 30;
            button.style.minWidth = 104;
            button.style.fontSize = 11;
            button.style.whiteSpace = WhiteSpace.NoWrap;
        }

        private static void ApplyLgoCharacterSelectNavigationAction(Button button)
        {
            ApplyLgoButton(button);
            button.style.minWidth = 120;
            button.style.height = 38;
            button.style.fontSize = 13;
            button.style.whiteSpace = WhiteSpace.NoWrap;
        }
    }
}
