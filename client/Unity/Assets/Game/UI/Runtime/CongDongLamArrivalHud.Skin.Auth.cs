using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private static VisualElement ApplyLgoTextFieldInnerFrame(TextField field, string semanticClass)
        {
            RuntimeUiSkin.ApplyLobbyInputInnerFrame(field);
            var input = field.Q(className: "unity-base-text-field__input")
                ?? field.Q(className: "unity-text-field__input")
                ?? field.Q("unity-text-input");
            if (input != null) input.AddToClassList(semanticClass);
            return input;
        }

        private static void ApplyLgoEntrySlogan(Label label)
        {
            label.AddToClassList(LgoEntrySloganClass);
            RuntimeUiTypography.ApplyHeadingFont(label);
            label.style.fontSize = 20;
            label.style.color = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.text, .92f);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyLgoEntryBrandStage(VisualElement stage)
        {
            stage.AddToClassList(LgoEntryBrandStageClass);
        }

        private static void ApplyLgoEntryLogo(Label label)
        {
            RuntimeUiTypography.ApplyHeadingFont(label);
            label.style.fontSize = 82;
            label.style.letterSpacing = 6;
            label.style.color = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.text, .98f);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyLgoEntryLogoOnline(Label label)
        {
            label.style.fontSize = 16;
            label.style.letterSpacing = 5;
            label.style.color = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.text, .92f);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyLgoEntrySignature(Label label)
        {
            label.AddToClassList(LgoEntrySignatureClass);
            RuntimeUiTypography.ApplyHeadingFont(label);
            label.style.fontSize = 18;
            label.style.unityFontStyleAndWeight = FontStyle.Italic;
            label.style.color = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.text, .90f);
            label.style.unityTextAlign = TextAnchor.LowerRight;
        }

        private static void ApplyLgoEntryNoticeCard(VisualElement panel)
        {
            panel.AddToClassList(LgoEntryNoticeClass);
            ApplyLgoStatusCard(panel, 18, 12);
        }

        private static void ApplyLgoEntryUtilityRail(VisualElement rail)
        {
            rail.AddToClassList(LgoEntryUtilityRailClass);
        }

        private static void ApplyLgoEntryNoticeHeader(VisualElement header)
        {
            header.AddToClassList("lgo-entry-notice-header");
        }

        private static void ApplyLgoEntryNoticeRow(VisualElement row)
        {
            row.AddToClassList("lgo-entry-notice-row");
        }

        private static void ApplyLgoEntryNoticeLink(Button button)
        {
            button.AddToClassList("lgo-entry-notice-link");
            ApplyLgoButton(button);
            button.style.color = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.text, .94f);
        }

        private static void ApplyLgoEntryNoticeHotBadge(Label badge)
        {
            badge.AddToClassList("lgo-entry-notice-hot");
            badge.AddToClassList("lgo-badge");
            badge.style.backgroundColor = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.danger, .90f);
        }

        private static void ApplyLgoEntryStatusRow(VisualElement row)
        {
            row.AddToClassList(LgoEntryStatusRowClass);
        }

        private static void ApplyLgoEntryStatusIcon(Label icon)
        {
            icon.AddToClassList("lgo-entry-status-icon");
            icon.style.borderTopColor = icon.style.borderRightColor = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.muted, .82f);
            icon.style.borderBottomColor = icon.style.borderLeftColor = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.muted, .82f);
        }

        private static void ApplyLgoEntryTextField(TextField field)
        {
            field.AddToClassList(LgoEntryTextFieldClass);
            ApplyLgoInputField(field);
            field.style.height = 54;
            field.style.marginBottom = 14;
            field.style.paddingLeft = 16;
            field.style.paddingRight = 16;
            field.style.fontSize = 15;
            var input = ApplyLgoTextFieldInnerFrame(field, LgoEntryTextInputClass);
            if (input != null) input.style.fontSize = 15;
            field.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                var attachedInput = ApplyLgoTextFieldInnerFrame(field, LgoEntryTextInputClass);
                if (attachedInput != null) attachedInput.style.fontSize = 15;
            });
        }

        private static void ApplyLgoEntryPasswordReveal(Button button, Sprite sprite)
        {
            button.AddToClassList(LgoEntryPasswordRevealClass);
            ApplyLgoButton(button);
            button.style.position = Position.Absolute;
            button.style.right = 12;
            button.style.top = 11;
            button.style.width = 30;
            button.style.height = 30;
            button.style.minWidth = 30;
            button.style.minHeight = 30;
            button.style.paddingLeft = button.style.paddingRight = 3;
            button.style.paddingTop = button.style.paddingBottom = 3;
            button.style.backgroundColor = Color.clear;
            button.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            button.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            button.style.borderTopWidth = button.style.borderRightWidth = 0;
            button.style.borderBottomWidth = button.style.borderLeftWidth = 0;
        }

        private static void ApplyLgoEntryRememberAction(Button button)
        {
            button.AddToClassList(LgoEntryRememberActionClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 1;
            button.style.flexBasis = 0;
            button.style.minHeight = 28;
            button.style.paddingLeft = button.style.paddingRight = 0;
            button.style.paddingTop = button.style.paddingBottom = 0;
            button.style.marginRight = 8;
            button.style.backgroundColor = Color.clear;
            button.style.borderTopWidth = button.style.borderRightWidth = 0;
            button.style.borderBottomWidth = button.style.borderLeftWidth = 0;
            button.style.justifyContent = Justify.FlexStart;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
        }

        private static void ApplyLgoEntryCtaAction(Button button, bool primary)
        {
            button.AddToClassList(LgoEntryCtaActionClass);
            ApplyLgoButton(button, primary);
            button.style.minWidth = primary ? 320 : 156;
            button.style.minHeight = primary ? 50 : 38;
            button.style.fontSize = primary ? 21 : 14;
            if (!primary) return;
            button.style.borderTopWidth = button.style.borderBottomWidth = 3;
            button.style.borderLeftWidth = button.style.borderRightWidth = 3;
            button.style.borderTopColor = button.style.borderBottomColor = new Color(1f, .86f, .50f, .98f);
            button.style.borderLeftColor = button.style.borderRightColor = new Color(.66f, .42f, .12f, .98f);
        }

        private static void ApplyLgoEntryAuthAction(Button button, bool primary)
        {
            ApplyLgoEntryCtaAction(button, false);
            button.EnableInClassList(LgoEntryAuthPrimaryClass, primary);
            button.EnableInClassList(LgoEntryAuthSecondaryClass, !primary);
            button.style.minHeight = 50;
            button.style.fontSize = 17;
            button.style.borderTopWidth = button.style.borderBottomWidth = 2;
            button.style.borderLeftWidth = button.style.borderRightWidth = 2;
            if (primary)
            {
                button.style.backgroundColor = new Color(.025f, .28f, .68f, .98f);
                button.style.borderTopColor = button.style.borderBottomColor = new Color(.28f, .78f, 1f, .98f);
                button.style.borderLeftColor = button.style.borderRightColor = new Color(.12f, .52f, .92f, .98f);
                button.style.color = new Color(.98f, .98f, .90f, 1f);
            }
            else
            {
                button.style.backgroundColor = new Color(.018f, .065f, .12f, .96f);
                button.style.borderTopColor = button.style.borderBottomColor = new Color(.95f, .75f, .36f, .92f);
                button.style.borderLeftColor = button.style.borderRightColor = new Color(.68f, .46f, .18f, .92f);
                button.style.color = new Color(.96f, .89f, .70f, .98f);
            }
        }

        private static void ApplyLgoEntryShell(VisualElement panel)
        {
            panel.AddToClassList(LgoEntryShellClass);
        }

        private static void ApplyLgoEntryControlCard(VisualElement card)
        {
            card.AddToClassList(LgoEntryControlCardClass);
            ApplyLgoDetailCard(card, 28, 24);
            card.style.backgroundColor = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.bg, .93f);
            card.style.borderTopWidth = card.style.borderBottomWidth = 2;
            card.style.borderLeftWidth = card.style.borderRightWidth = 2;
        }

        private static void ApplyLgoEntryServerCard(VisualElement card)
        {
            card.AddToClassList(LgoEntryServerCardClass);
            ApplyLgoDetailCard(card, 12, 10);
            card.style.flexDirection = FlexDirection.Row;
            card.style.alignItems = Align.Center;
            card.style.minHeight = 66;
            card.style.marginBottom = 10;
            card.style.backgroundColor = new Color(.008f, .035f, .064f, .96f);
        }

        private static void ApplyLgoEntryStatusLine(Label label)
        {
            label.AddToClassList(LgoEntryStatusLineClass);
            label.style.minHeight = 24;
            label.style.fontSize = 13;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.color = new Color(.72f, .86f, .92f, .90f);
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.paddingLeft = 6;
            label.style.paddingRight = 6;
        }

        private static void ApplyLgoEntryServerSwitchAction(Button button)
        {
            button.AddToClassList(LgoEntrySecondaryActionClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.minWidth = 40;
            button.style.minHeight = 34;
            button.style.paddingLeft = button.style.paddingRight = 8;
            button.style.fontSize = 24;
            button.style.opacity = 1;
        }

        private static void ApplyLgoServerSelectPanel(VisualElement panel)
        {
            panel.AddToClassList(LgoServerSelectPanelClass);
            ApplyLgoEntryControlCard(panel);
        }

        private static void ApplyLgoServerSelectTitleRow(VisualElement row)
        {
            row.AddToClassList(LgoServerSelectTitleRowClass);
        }

        private static void ApplyLgoServerSelectCard(Button card)
        {
            card.AddToClassList(LgoServerSelectCardClass);
            card.AddToClassList(LgoServerSelectSelectedClass);
            ApplyLgoButton(card);
            ApplyLgoLayeredFrame(card);
            card.style.backgroundColor = new Color(.015f, .20f, .48f, .96f);
            card.style.borderTopWidth = card.style.borderBottomWidth = 2;
            card.style.borderLeftWidth = card.style.borderRightWidth = 2;
            card.style.borderTopColor = card.style.borderBottomColor = new Color(.32f, .80f, 1f, .98f);
            card.style.borderLeftColor = card.style.borderRightColor = UiGold;
        }

        private static void ApplyLgoServerSelectStatusRow(VisualElement row)
        {
            row.AddToClassList(LgoServerSelectStatusRowClass);
        }

        private static void ApplyLgoServerSelectAction(Button button, bool primary)
        {
            button.AddToClassList(LgoServerSelectActionClass);
            ApplyLgoEntryAuthAction(button, primary);
            button.style.minHeight = 58;
            button.style.fontSize = 18;
            if (primary)
            {
                button.style.backgroundColor = new Color(.84f, .58f, .18f, .98f);
                button.style.borderTopColor = button.style.borderBottomColor = new Color(1f, .86f, .50f, .98f);
                button.style.color = new Color(.13f, .08f, .025f, 1f);
            }
        }

        private static void ApplyLgoRegisterPanel(VisualElement panel)
        {
            panel.AddToClassList(LgoRegisterPanelClass);
            ApplyLgoAuthFlowPanel(panel, 0);
        }

        private static void ApplyLgoRegisterHeader(VisualElement header)
        {
            header.AddToClassList(LgoRegisterHeaderClass);
        }

        private static void ApplyLgoRegisterSubtitleRow(VisualElement row)
        {
            row.AddToClassList(LgoRegisterSubtitleRowClass);
        }

        private static void ApplyLgoAuthFlowPanel(VisualElement panel, float minHeight)
        {
            panel.AddToClassList(LgoAuthFlowPanelClass);
            ApplyLgoEntryControlCard(panel);
            ApplyLgoCharacterHubSurface(panel, ref _characterHubSurface, "character-hub-surface");
            RemoveLgoOuterBorder(panel);
            ApplyLgoCharacterHubFiligreeFrame(panel);
            if (minHeight > 0f) panel.style.minHeight = minHeight;
        }

        private static void ApplyLgoRegisterAgreement(Button button)
        {
            button.AddToClassList(LgoRegisterAgreementClass);
            ApplyLgoEntryRememberAction(button);
            button.style.flexGrow = StyleKeyword.Null;
            button.style.flexBasis = StyleKeyword.Null;
            button.style.minHeight = StyleKeyword.Null;
            button.style.marginRight = StyleKeyword.Null;
        }

        private static void ApplyLgoRegisterAgreementCheckFrame(VisualElement box)
        {
            box.AddToClassList("lgo-register-agreement-box");
            ApplyLgoFrame(box, new Color(.025f, .075f, .080f, .92f), new Color(.86f, .78f, .48f, .88f));
        }

        private static void ApplyLgoRegisterAgreementMark(VisualElement mark)
        {
            mark.AddToClassList("lgo-register-agreement-mark");
            mark.style.backgroundColor = UiGold;
        }

        private static void ApplyLgoRegisterTerms(Label label)
        {
            label.AddToClassList(LgoRegisterTermsClass);
            label.style.color = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.spirit, .96f);
        }

        private static void ApplyLgoRegisterPrimary(Button button)
        {
            button.AddToClassList(LgoRegisterPrimaryClass);
            ApplyLgoAuthFlowPrimary(button);
        }

        private static void ApplyLgoAuthFlowPrimary(Button button)
        {
            button.AddToClassList(LgoAuthFlowPrimaryClass);
            ApplyLgoCharacterHubGoldAction(button);
            button.style.flexGrow = StyleKeyword.Null;
            button.style.height = StyleKeyword.Null;
            button.style.minHeight = StyleKeyword.Null;
            button.style.fontSize = StyleKeyword.Null;
            button.style.marginTop = StyleKeyword.Null;
            button.style.whiteSpace = StyleKeyword.Null;
        }

        private static void ApplyLgoRegisterBack(Button button)
        {
            button.AddToClassList(LgoRegisterBackClass);
            ApplyLgoAuthFlowBack(button);
        }

        private static void ApplyLgoAuthFlowBack(Button button)
        {
            button.AddToClassList(LgoAuthFlowBackClass);
            ApplyLgoButton(button);
            button.style.flexGrow = StyleKeyword.Null;
            button.style.alignSelf = StyleKeyword.Null;
            button.style.minHeight = StyleKeyword.Null;
            button.style.marginTop = StyleKeyword.Null;
            button.style.paddingLeft = button.style.paddingRight = StyleKeyword.Null;
            button.style.whiteSpace = StyleKeyword.Null;
            button.style.backgroundColor = Color.clear;
            button.style.borderTopWidth = button.style.borderRightWidth = 0;
            button.style.borderBottomWidth = button.style.borderLeftWidth = 0;
            button.style.color = new Color(.72f, .88f, 1f, .96f);
        }

        private static void ApplyLgoRegisterPasswordReveal(Button button, Sprite sprite)
        {
            button.AddToClassList(LgoRegisterPasswordRevealClass);
            ApplyLgoEntryPasswordReveal(button, sprite);
        }

        private static VisualElement CreateLgoEntryIcon(string name, Sprite sprite, float size)
        {
            var icon = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            icon.style.width = icon.style.height = size;
            icon.style.flexShrink = 0;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            return icon;
        }

        private static void AttachLgoEntryFieldIcon(VisualElement field, Sprite sprite)
        {
            field.style.position = Position.Relative;
            field.style.paddingLeft = 50;
            var icon = CreateLgoEntryIcon(field.name + " Icon", sprite, 24);
            icon.style.position = Position.Absolute;
            icon.style.left = 14;
            icon.style.top = 9;
            field.Add(icon);
        }

        private static void AttachLgoEntrySideActionIcon(Button button, Sprite sprite)
        {
            var icon = CreateLgoEntryIcon(button.name + " Icon", sprite, 30);
            icon.style.position = Position.Absolute;
            icon.style.left = 19;
            icon.style.top = 5;
            button.Add(icon);
        }

        private static void ApplyLgoEntryBrandCrest(VisualElement crest, Sprite sprite)
        {
            crest.style.width = crest.style.height = 42;
            crest.style.alignSelf = Align.Center;
            crest.style.marginBottom = 2;
            crest.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            crest.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
        }

        private static void ApplyLgoEntryTextLink(Button button)
        {
            button.AddToClassList(LgoEntryTextLinkClass);
            ApplyLgoButton(button);
            button.style.color = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.text, .94f);
        }

        private static void ApplyLgoEntrySideAction(Button button)
        {
            button.AddToClassList(LgoEntrySideActionClass);
            button.AddToClassList("lgo-utility-action");
            ApplyLgoButton(button);
            button.style.width = 72;
            button.style.height = 72;
            button.style.marginLeft = 8;
            button.style.marginBottom = 12;
            button.style.paddingLeft = button.style.paddingRight = 0;
            button.style.paddingTop = 40;
            button.style.paddingBottom = 5;
            button.style.fontSize = 12;
            button.style.opacity = .90f;
            button.style.backgroundColor = new Color(.008f, .030f, .054f, .72f);
            button.style.borderTopLeftRadius = button.style.borderTopRightRadius = 36;
            button.style.borderBottomLeftRadius = button.style.borderBottomRightRadius = 36;
            button.style.borderTopWidth = button.style.borderBottomWidth = 2;
            button.style.borderLeftWidth = button.style.borderRightWidth = 2;
            button.style.borderTopColor = button.style.borderBottomColor = new Color(.82f, .67f, .36f, .80f);
            button.style.borderLeftColor = button.style.borderRightColor = new Color(.82f, .67f, .36f, .80f);
        }

        private static void ApplyLgoCharacterSelectPanel(VisualElement panel)
        {
            ApplyLgoModalShell(panel, 14);
            panel.style.minWidth = 0;
            panel.style.minHeight = 0;
            panel.style.backgroundColor = new Color(.006f, .026f, .052f, .94f);
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
            button.style.height = 34;
            button.style.fontSize = 13;
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
            ApplyLgoButton(button, primary);
            button.style.flexGrow = 1;
            button.style.flexBasis = 0;
            button.style.minWidth = 0;
            button.style.height = 38;
            button.style.fontSize = 13;
            button.style.whiteSpace = WhiteSpace.NoWrap;
        }

        private static void ApplyLgoCharacterSelectServerRow(VisualElement row)
        {
            ApplyLgoStatusCard(row, 9, 6);
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.flexGrow = 0;
            row.style.minHeight = 42;
            row.style.marginTop = 7;
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
