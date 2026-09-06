using System;
using UnityEngine;
using UnityEngine.UIElements;
using LinhGioi.Art;

namespace LinhGioi.UI
{
    internal static class RuntimeUiFactory
    {
        internal const string FactoryMarker = "LGO Runtime UI Primitive Factory v1";

        internal static VisualElement NewPanel(float maxWidth)
        {
            var panel = new VisualElement();
            panel.style.maxWidth = maxWidth;
            panel.style.minWidth = RuntimeUiSpacing.PanelMinWidth;
            panel.style.width = Length.Percent(100);
            panel.style.marginRight = 0;
            panel.style.marginBottom = RuntimeUiSpacing.PanelMarginBottom;
            RuntimeUiSkin.ApplyPadding(panel, RuntimeUiSpacing.PanelPaddingHorizontal, RuntimeUiSpacing.PanelPaddingVertical);
            panel.style.backgroundColor = RuntimeArtCatalog.Surface;
            RuntimeUiSkin.ApplyPanelFrame(panel);
            return panel;
        }

        internal static VisualElement NewPreviewPanel(string sigilText = "LINH MÔN", string headingText = null)
        {
            var preview = new VisualElement();
            preview.style.minWidth = RuntimeUiSpacing.PreviewPanelMinWidth;
            preview.style.flexGrow = 1;
            RuntimeUiSkin.ApplyPadding(preview, RuntimeUiSpacing.PreviewPanelPaddingHorizontal, RuntimeUiSpacing.PreviewPanelPaddingVertical);
            RuntimeUiSkin.ApplyPreviewPanelFrame(preview);
            if (!string.IsNullOrWhiteSpace(sigilText))
            {
                var sigil = new Label(sigilText);
                RuntimeUiSkin.ApplyText(sigil, RuntimeArtCatalog.Gold, RuntimeUiTypography.SectionSigilFontSize, true);
                preview.Add(sigil);
            }
            if (!string.IsNullOrWhiteSpace(headingText))
            {
                var heading = new Label(headingText);
                RuntimeUiSkin.ApplyText(heading, RuntimeArtCatalog.Text, RuntimeUiTypography.SectionHeadingFontSize, true);
                heading.style.marginTop = RuntimeUiSpacing.PreviewPanelHeadingMarginTop;
                heading.style.marginBottom = RuntimeUiSpacing.PreviewPanelHeadingMarginBottom;
                preview.Add(heading);
            }
            return preview;
        }

        internal static VisualElement NewSectionShell(string sigilText, string headingText, string sectionTitle, string elementName)
        {
            var shell = NewPreviewPanel(sigilText, headingText);
            if (!string.IsNullOrWhiteSpace(elementName)) shell.name = elementName;
            if (!string.IsNullOrWhiteSpace(sectionTitle)) shell.Add(NewSectionTitle(sectionTitle));
            return shell;
        }

        internal static VisualElement NewSessionMenuShell(string title, string elementName)
        {
            var shell = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) shell.name = elementName;
            shell.style.flexDirection = FlexDirection.Column;
            shell.style.alignItems = Align.Stretch;
            var topRule = NewOrnamentRule(RuntimeArtCatalog.Gold);
            topRule.style.marginBottom = 8;
            shell.Add(topRule);
            var titleLabel = NewSectionTitle(title);
            titleLabel.style.color = RuntimeArtCatalog.Gold;
            titleLabel.name = "LGO Session Menu Title";
            titleLabel.style.marginBottom = 8;
            shell.Add(titleLabel);
            shell.Add(NewOrnamentRule(RuntimeArtCatalog.Gold));
            return shell;
        }

        internal static VisualElement NewReadabilityRow(string title, string value, Color accent)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.marginTop = RuntimeUiSpacing.RowMarginTop;
            RuntimeUiSkin.ApplyPadding(row, RuntimeUiSpacing.ReadabilityRowPaddingHorizontal, RuntimeUiSpacing.ReadabilityRowPaddingVertical);
            RuntimeUiSkin.ApplyInsetRowFrame(row, accent);
            var titleLabel = new Label(title);
            titleLabel.style.minWidth = RuntimeUiSpacing.ReadabilityTitleMinWidth;
            titleLabel.style.marginRight = RuntimeUiSpacing.RowGap;
            RuntimeUiSkin.ApplyText(titleLabel, accent, 0, true);
            var valueLabel = new Label(value);
            valueLabel.style.flexGrow = 1;
            RuntimeUiSkin.ApplyText(valueLabel, RuntimeArtCatalog.Text);
            valueLabel.style.whiteSpace = WhiteSpace.Normal;
            row.Add(titleLabel);
            row.Add(valueLabel);
            return row;
        }

        internal static VisualElement NewWorldHudGroup(string name, Color accent)
        {
            var group = new VisualElement { name = name };
            RuntimeUiSkin.ApplyVerticalMargin(group, RuntimeUiSpacing.WorldHudGroupMarginVertical, RuntimeUiSpacing.WorldHudGroupMarginVertical);
            RuntimeUiSkin.ApplyPadding(group, RuntimeUiSpacing.WorldHudGroupPaddingHorizontal, RuntimeUiSpacing.WorldHudGroupPaddingVertical);
            RuntimeUiSkin.ApplyWorldHudGroupFrame(group, accent);
            return group;
        }

        internal static VisualElement NewWorldHudRoot(string name, float maxWidth)
        {
            var hud = NewPanel(maxWidth);
            hud.name = name;
            hud.style.maxWidth = maxWidth;
            hud.style.alignSelf = Align.FlexStart;
            RuntimeUiSkin.ApplyPadding(hud, RuntimeUiSpacing.WorldHudRootPaddingHorizontal, RuntimeUiSpacing.WorldHudRootPaddingVertical);
            RuntimeUiSkin.ApplyWorldHudRootFrame(hud);
            return hud;
        }

        internal static VisualElement NewCharacterHallPanel(RuntimeUiLayoutProfile layout)
        {
            var panel = NewPanel(RuntimeUiSizing.MainShellMaxWidth);
            panel.name = "LGO Character Hall V3B Composition Panel";
            RuntimeUiSkin.ApplyCharacterHallPanelFrame(panel, layout.IsMobile);
            panel.style.maxWidth = RuntimeUiSizing.CharacterHallPanelMaxWidth;
            panel.style.minHeight = RuntimeUiSizing.CharacterHallPanelMinHeight;
            RuntimeUiSkin.ApplyPadding(panel, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingTop, layout.LobbyPanelPaddingBottom);
            panel.style.alignSelf = Align.FlexStart;
            return panel;
        }

        internal static VisualElement NewCharacterCreatePanel(RuntimeUiLayoutProfile layout)
        {
            var panel = new VisualElement();
            panel.name = "LGO Character Hall Create Cultivator Panel V3B";
            panel.style.marginTop = layout.CreatePanelMarginTop;
            panel.style.position = Position.Relative;
            RuntimeUiSkin.ApplyPadding(panel, layout.CreatePanelPaddingHorizontal, layout.CreatePanelPaddingHorizontal, layout.CreatePanelPaddingTop, layout.CreatePanelPaddingBottom);
            panel.style.minHeight = RuntimeUiSizing.CharacterCreatePanelMinHeight;
            panel.style.maxHeight = RuntimeUiSizing.CharacterCreatePanelMaxHeight;
            RuntimeUiSkin.ApplyCharacterCreateFrame(panel);
            return panel;
        }

        internal static VisualElement NewCharacterHallContentRow(RuntimeUiLayoutProfile layout)
        {
            var row = new VisualElement();
            row.name = "LGO Character Hall Main Selection Grid V3B";
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.NoWrap;
            row.style.justifyContent = Justify.SpaceBetween;
            ApplyCharacterHallContentResponsive(row, layout);
            return row;
        }

        internal static void ApplyCharacterHallContentResponsive(VisualElement row, RuntimeUiLayoutProfile layout)
        {
            if (row == null) return;
            row.style.minHeight = 0;
            row.style.flexShrink = 1;
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.NoWrap;
            row.style.height = layout.IsMobile || layout.IsTablet
                ? StyleKeyword.Auto
                : Mathf.Clamp(layout.Height * 0.42f, 390f, 460f);
            RuntimeUiSkin.ApplyVerticalMargin(row, layout.LobbyContentMarginTop, layout.LobbyContentMarginBottom);
        }

        internal static VisualElement NewCharacterListPanel(RuntimeUiLayoutProfile layout)
        {
            // LGO Character Hall Bounded List Scroll Base v1: list/detail overflow belongs to a scroll body, not the screen shell.
            var list = new ScrollView(ScrollViewMode.Vertical);
            list.name = "LGO Character Hall Bounded List Scroll";
            list.verticalScrollerVisibility = ScrollerVisibility.Auto;
            list.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            list.style.minWidth = RuntimeUiSizing.CharacterListInitialMinWidth;
            list.style.maxWidth = RuntimeUiSizing.CharacterListMaxWidth;
            list.style.flexGrow = 0;
            list.style.minHeight = 0;
            RuntimeUiSkin.ApplyMargin(list, 0, layout.CharacterListMarginRight, 0, layout.CharacterListMarginBottom);
            ApplyCharacterListDensity(list, layout.CharacterHallDensity);
            RuntimeUiSkin.ApplyCharacterListFrame(list);
            RuntimeUiOverflowGuard.ApplyBoundedScroll(list, layout.CharacterListMaxHeight(false), 0f);
            list.contentViewport.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (evt.newRect.size != evt.oldRect.size) RevealSelectedListItem(list);
            });
            list.contentContainer.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (evt.newRect.size != evt.oldRect.size) RevealSelectedListItem(list);
            });
            return list;
        }

        internal static void ApplyCharacterListResponsive(VisualElement list, RuntimeUiLayoutProfile layout, int viewportWidth, bool hasSelectedCharacter = false)
        {
            if (list == null) return;
            list.style.minWidth = layout.IsMobile && hasSelectedCharacter ? 206 : layout.IsMobile ? 220 : RuntimeUiSizing.CharacterListInitialMinWidth;
            var listMaxWidth = layout.IsMobile
                ? hasSelectedCharacter ? layout.CharacterSelectedListMaxWidth : Mathf.Clamp(viewportWidth * 0.40f, 285f, 330f)
                : layout.CharacterSelectedListMaxWidth;
            list.style.width = listMaxWidth;
            list.style.maxWidth = listMaxWidth;
            list.style.flexGrow = hasSelectedCharacter ? 1 : 0;
            var listMaxHeight = layout.CharacterListMaxHeight(hasSelectedCharacter);
            list.style.height = StyleKeyword.Auto;
            list.style.maxHeight = listMaxHeight;
            list.style.overflow = Overflow.Hidden;
            list.style.marginRight = layout.CharacterListMarginRight;
            ApplyCharacterListDensity(list, layout.CharacterHallDensity);
            if (list is ScrollView scroll)
            {
                RuntimeUiOverflowGuard.ApplyBoundedScroll(scroll, listMaxHeight, 0f);
                scroll.verticalScrollerVisibility = ScrollerVisibility.Auto;
                scroll.contentContainer.style.width = Length.Percent(100);
            }
            list.style.width = listMaxWidth;
            list.style.maxWidth = listMaxWidth;
            list.style.flexGrow = hasSelectedCharacter ? 1 : 0;
        }

        internal static VisualElement NewSelectedCharacterPreviewPanel()
        {
            var preview = NewPreviewPanel(null);
            preview.name = "LGO Character Hall Selected Cultivator Card V3B";
            RuntimeUiSkin.ApplyCharacterPreviewFrame(preview);
            return preview;
        }

        internal static void ApplySelectedCharacterPreviewResponsive(VisualElement preview, Label selectedName, RuntimeUiLayoutProfile layout, int viewportWidth, bool hasSelectedCharacter = true)
        {
            if (preview == null) return;
            preview.style.display = DisplayStyle.Flex;
            preview.style.width = hasSelectedCharacter ? layout.CharacterSelectedPreviewMaxWidth : StyleKeyword.Auto;
            preview.style.maxWidth = layout.CharacterSelectedPreviewMaxWidth;
            preview.style.minWidth = layout.IsMobile ? 0 : RuntimeUiSpacing.PreviewPanelMinWidth;
            preview.style.flexGrow = layout.IsMobile ? 0 : 1;
            preview.style.flexBasis = layout.IsMobile && hasSelectedCharacter ? layout.CharacterSelectedPreviewMaxWidth : StyleKeyword.Auto;
            preview.style.height = layout.IsMobile || layout.IsTablet
                ? StyleKeyword.Auto
                : layout.CharacterSelectedPreviewHeight;
            RuntimeUiSkin.ApplyCharacterPreviewFrame(preview, layout.IsMobile && hasSelectedCharacter);
            if (selectedName != null)
            {
                selectedName.style.fontSize = layout.SelectedCharacterNameFontSize;
                selectedName.style.whiteSpace = WhiteSpace.Normal;
                selectedName.style.minWidth = 0;
                selectedName.style.maxWidth = Length.Percent(100);
            }
        }

        internal static VisualElement NewCharacterProfileHero(RuntimeUiLayoutProfile layout, VisualElement portrait, VisualElement copy)
        {
            var hero = new VisualElement();
            hero.name = "LGO Character Hall Selected Profile Hero V3B";
            hero.style.flexDirection = FlexDirection.Row;
            hero.style.alignItems = Align.Center;
            hero.style.marginBottom = layout.SelectedPreviewHeroMarginBottom;
            hero.Add(portrait);
            hero.Add(copy);
            return hero;
        }

        internal static void ApplyCharacterSelectionStage(VisualElement row, VisualElement roster, VisualElement preview, RuntimeUiLayoutProfile layout, bool selected)
        {
            if (row == null || roster == null || preview == null) return;
            var portrait = row.Q<VisualElement>("LGO Character Hall V3B Cultivator Portrait");
            if (portrait == null) return;
            // One retained image becomes the central stage; the profile card owns only text.
            if (portrait.parent != row)
            {
                portrait.RemoveFromHierarchy();
                row.Insert(row.IndexOf(preview), portrait);
            }
            portrait.style.display = DisplayStyle.Flex;

            row.style.height = layout.Height * (layout.IsMobile ? 0.58f : 0.48f);
            row.style.minWidth = 0;
            row.style.alignItems = Align.Stretch;
            ApplySelectionStageColumn(roster, 32);
            ApplySelectionStageColumn(portrait, 34);
            ApplySelectionStageColumn(preview, 32);
            portrait.style.height = Length.Percent(100);
            portrait.style.backgroundColor = Color.clear;
            portrait.style.borderTopWidth = 0;
            portrait.style.borderBottomWidth = 0;
            portrait.style.borderLeftWidth = 0;
            portrait.style.borderRightWidth = 0;
            portrait.pickingMode = PickingMode.Ignore;
            roster.style.height = Length.Percent(100);
            roster.style.maxHeight = Length.Percent(100);
            if (roster is ScrollView slots) slots.contentContainer.style.flexGrow = 1;
            preview.style.height = Length.Percent(100);
            preview.style.alignSelf = Align.Stretch;
            preview.style.maxHeight = Length.Percent(100);
            preview.style.overflow = Overflow.Hidden;
        }

        private static void ApplySelectionStageColumn(VisualElement element, float widthPercent)
        {
            element.style.width = Length.Percent(widthPercent);
            element.style.maxWidth = Length.Percent(widthPercent);
            element.style.minWidth = 0;
            element.style.flexGrow = 0;
            element.style.flexShrink = 1;
            element.style.flexBasis = StyleKeyword.Auto;
            element.style.marginLeft = 0;
            element.style.marginRight = 0;
        }

        internal static VisualElement NewCharacterPortraitFrame(RuntimeUiLayoutProfile layout, Texture2D portraitTexture, Texture2D fallbackTexture)
        {
            var portrait = NewImageLayer("LGO Character Hall V3B Cultivator Portrait", portraitTexture, ScaleMode.ScaleToFit);
            portrait.style.width = layout.CharacterPortraitWidth;
            portrait.style.height = layout.CharacterPortraitHeight;
            portrait.style.marginRight = layout.CharacterPortraitMarginRight;
            RuntimeUiSkin.ApplyCharacterPortraitFrame(portrait);
            if (portraitTexture == null && fallbackTexture != null)
                portrait.Add(NewRuntimeIcon(fallbackTexture, 58, "Hồ sơ tu sĩ"));
            return portrait;
        }

        internal static Label NewCharacterHallListHeading(string text)
        {
            var label = new Label(text);
            label.name = "LGO Character Hall V3B List Heading";
            label.style.whiteSpace = WhiteSpace.Normal;
            RuntimeUiSkin.ApplyCharacterHallListHeading(label);
            return label;
        }

        internal static VisualElement NewFlexibleColumn(string name = null)
        {
            var column = new VisualElement();
            if (!string.IsNullOrWhiteSpace(name)) column.name = name;
            column.style.flexGrow = 1;
            return column;
        }

        internal static VisualElement NewEmptyCharacterCard(RuntimeUiLayoutProfile layout, Label title, Label hint)
        {
            var density = layout.CharacterHallDensity;
            var card = new VisualElement();
            ApplyEmptyCharacterCardDensity(card, density);
            RuntimeUiSkin.ApplyEmptyCharacterCardFrame(card);
            card.Add(title);
            hint.style.marginTop = RuntimeUiSpacing.EmptyCharacterHintMarginTop;
            card.Add(hint);
            return card;
        }

        internal static void ApplyCharacterListDensity(VisualElement list, RuntimeUiDensityProfile density)
        {
            RuntimeUiSkin.ApplyPadding(list, density.ListPaddingHorizontal, density.ListPaddingHorizontal, density.ListPaddingVertical, density.ListPaddingVertical);
        }

        internal static void ApplyEmptyCharacterCardDensity(VisualElement card, RuntimeUiDensityProfile density)
        {
            card.style.marginTop = density.EmptyCardMarginTop;
            RuntimeUiSkin.ApplyPadding(card, density.EmptyCardPaddingHorizontal, density.EmptyCardPaddingHorizontal, density.EmptyCardPaddingVertical, density.EmptyCardPaddingVertical);
        }

        internal static void ApplyHudStatusCompact(Label label, int fontSize)
        {
            label.style.fontSize = fontSize;
            label.style.marginTop = RuntimeUiSpacing.CompactStatusMarginTop;
            RuntimeUiSkin.ApplyPadding(label, RuntimeUiSpacing.CompactStatusPaddingHorizontal, RuntimeUiSpacing.CompactStatusPaddingHorizontal, RuntimeUiSpacing.CompactStatusPaddingTop, RuntimeUiSpacing.CompactStatusPaddingBottom);
            RuntimeUiSkin.ApplyHudStatusCompactFrame(label);
        }

        internal static Label NewCompactStatusLabel(string text, Color color, int fontSize)
        {
            var label = NewStatusLabel(text, color);
            ApplyHudStatusCompact(label, fontSize);
            return label;
        }

        internal static Label NewHiddenStatusLabel(string text, Color color)
        {
            var label = NewStatusLabel(text, color);
            label.style.display = DisplayStyle.None;
            return label;
        }

        internal static Label NewCharacterHallStatusLabel(string text, Color color, RuntimeUiLayoutProfile layout) =>
            NewStatusLabel(text, color, layout.CharacterHallDensity);

        internal static Label NewHiddenMutedLabel(string text)
        {
            var label = NewMutedLabel(text);
            label.style.display = DisplayStyle.None;
            return label;
        }

        internal static void ApplyStatusAccent(Label label, Color accent)
        {
            RuntimeUiSkin.ApplyStatusAccent(label, accent);
        }

        internal static Label NewSectionTitle(string text)
        {
            var label = new Label(text);
            RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Gold, RuntimeUiTypography.SectionTitleFontSize, true, TextAnchor.MiddleCenter);
            label.style.marginBottom = RuntimeUiSpacing.SectionTitleMarginBottom;
            return label;
        }

        internal static VisualElement NewSectionHeaderBlock(string title, Color ornamentColor, string elementName = null)
        {
            var block = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) block.name = elementName;
            block.style.flexDirection = FlexDirection.Column;
            var titleLabel = NewSectionTitle(title);
            if (!string.IsNullOrWhiteSpace(elementName)) titleLabel.name = elementName + " Title";
            block.Add(titleLabel);
            block.Add(NewOrnamentRule(ornamentColor));
            return block;
        }

        internal static VisualElement NewBadgeStrip(string elementName, params (string title, string value)[] badges)
        {
            var strip = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) strip.name = elementName;
            strip.style.flexDirection = FlexDirection.Row;
            strip.style.flexWrap = Wrap.Wrap;
            strip.style.marginBottom = RuntimeUiSpacing.BadgeStripMarginBottom;
            foreach (var badge in badges) strip.Add(NewBadge(badge.title, badge.value));
            return strip;
        }

        internal static VisualElement NewBadge(string title, string value)
        {
            var badge = new VisualElement();
            RuntimeUiSkin.ApplyPadding(badge, RuntimeUiSpacing.BadgePaddingHorizontal, RuntimeUiSpacing.BadgePaddingVertical);
            badge.style.marginRight = RuntimeUiSpacing.RowGap;
            badge.style.marginBottom = RuntimeUiSpacing.BadgeMarginBottom;
            RuntimeUiSkin.ApplyBadgeFrame(badge);
            var titleLabel = new Label(title);
            RuntimeUiSkin.ApplyText(titleLabel, RuntimeArtCatalog.Gold, RuntimeUiTypography.BadgeTitleFontSize);
            var valueLabel = new Label(value);
            RuntimeUiSkin.ApplyText(valueLabel, RuntimeArtCatalog.Text, RuntimeUiTypography.BadgeValueFontSize);
            badge.Add(titleLabel);
            badge.Add(valueLabel);
            return badge;
        }

        internal static Label NewToast(string text)
        {
            var label = new Label(text);
            label.style.marginTop = RuntimeUiSpacing.ToastMarginTop;
            RuntimeUiSkin.ApplyPadding(label, RuntimeUiSpacing.ToastPaddingHorizontal, RuntimeUiSpacing.ToastPaddingVertical);
            label.style.whiteSpace = WhiteSpace.Normal;
            RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Text);
            RuntimeUiSkin.ApplyToastFrame(label, RuntimeArtCatalog.Gold);
            return label;
        }

        internal static void ApplyStatusChip(Label label, Color accent)
        {
            label.style.maxWidth = RuntimeUiSpacing.StatusChipMaxWidth;
            label.style.marginRight = RuntimeUiSpacing.RowGap;
            label.style.whiteSpace = WhiteSpace.Normal;
            RuntimeUiSkin.ApplyPadding(label, RuntimeUiSpacing.StatusChipPaddingHorizontal, RuntimeUiSpacing.StatusChipPaddingVertical);
            RuntimeUiSkin.ApplyStatusChipFrame(label, accent);
        }

        internal static Label NewMutedLabel(string text)
        {
            var label = new Label(text);
            RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Muted);
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        internal static VisualElement NewLoginOrnamentRule(string name)
        {
            var row = new VisualElement { name = name };
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.justifyContent = Justify.Center;
            row.style.width = Length.Percent(RuntimeUiSpacing.LoginOrnamentWidthPercent);
            row.style.height = RuntimeUiSpacing.LoginOrnamentHeight;
            row.style.marginTop = RuntimeUiSpacing.LoginOrnamentMarginTop;
            row.style.marginBottom = RuntimeUiSpacing.LoginOrnamentMarginBottom;
            row.Add(NewLoginOrnamentLine(RuntimeArtCatalog.Gold));
            return row;
        }

        internal static VisualElement NewOrnamentRule(Color color)
        {
            var rule = new VisualElement();
            rule.style.height = RuntimeUiSpacing.OrnamentRuleHeight;
            rule.style.marginBottom = RuntimeUiSpacing.OrnamentRuleMarginBottom;
            rule.style.backgroundColor = color;
            rule.style.opacity = 0.8f;
            return rule;
        }

        internal static Label NewStatusLabel(string text, Color color)
        {
            return NewStatusLabel(text, color, default);
        }

        internal static Label NewStatusLabel(string text, Color color, RuntimeUiDensityProfile density)
        {
            var label = new Label(text);
            RuntimeUiSkin.ApplyText(label, color);
            label.style.whiteSpace = WhiteSpace.Normal;
            if (density.StatusPaddingHorizontal > 0)
            {
                label.style.marginTop = density.StatusMarginTop;
                RuntimeUiSkin.ApplyPadding(label, density.StatusPaddingHorizontal, density.StatusPaddingVertical);
            }
            else
            {
                label.style.marginTop = RuntimeUiSpacing.StatusLabelMarginTop;
                RuntimeUiSkin.ApplyPadding(label, RuntimeUiSpacing.StatusLabelPaddingHorizontal, RuntimeUiSpacing.StatusLabelPaddingVertical);
            }
            RuntimeUiSkin.ApplyInsetRowFrame(label, color);
            return label;
        }

        internal static VisualElement NewButtonRow(params Button[] buttons)
        {
            return NewActionRow("LGO Runtime Action Row", Justify.FlexStart, 6, 0, buttons);
        }

        internal static VisualElement NewModalBody(string elementName)
        {
            var body = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) body.name = elementName;
            RuntimeUiOverflowGuard.ApplyModalBody(body);
            return body;
        }

        internal static VisualElement NewModalFooter(string elementName)
        {
            var footer = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) footer.name = elementName;
            RuntimeUiOverflowGuard.ApplyModalFooter(footer, 0);
            return footer;
        }

        internal static VisualElement NewActionRow(string elementName, Justify justifyContent, float marginTop, float marginBottom, params Button[] buttons)
        {
            var row = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) row.name = elementName;
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.alignItems = Align.Center;
            row.style.justifyContent = justifyContent;
            row.style.marginTop = marginTop;
            row.style.marginBottom = marginBottom;
            RuntimeUiOverflowGuard.ApplyBoundedActionRow(row);
            foreach (var button in buttons) row.Add(button);
            return row;
        }

        internal static VisualElement NewIconStatusRow(string elementName, VisualElement icon, params VisualElement[] statusElements)
        {
            var row = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) row.name = elementName;
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.marginTop = RuntimeUiSpacing.CompactStatusMarginTop;
            row.style.marginBottom = RuntimeUiSpacing.IconStatusRowMarginBottom;
            RuntimeUiSkin.ApplyPadding(
                row,
                RuntimeUiSpacing.IconStatusRowPaddingHorizontal,
                RuntimeUiSpacing.IconStatusRowPaddingHorizontal,
                RuntimeUiSpacing.IconStatusRowPaddingTop,
                RuntimeUiSpacing.IconStatusRowPaddingBottom);
            row.Add(icon);

            var statusColumn = new VisualElement();
            statusColumn.style.flexGrow = 1;
            statusColumn.style.marginLeft = RuntimeUiSpacing.StatusLabelPaddingHorizontal;
            foreach (var statusElement in statusElements) statusColumn.Add(statusElement);
            row.Add(statusColumn);
            return row;
        }

        internal static void ApplyCombatButtonSkin(Button button, Texture2D texture, bool coolingDown)
        {
            if (button == null) return;
            if (coolingDown)
            {
                button.style.backgroundImage = StyleKeyword.None;
                button.style.backgroundColor = new Color(0.018f, 0.055f, 0.070f, 0.76f);
                button.style.color = RuntimeArtCatalog.Muted;
                RuntimeUiSkin.ApplyRadius(button, RuntimeUiSizing.BaseButtonRadius);
                RuntimeUiSkin.ApplyEdgeFrame(
                    button,
                    new Color(0.93f, 0.73f, 0.36f, 0.42f),
                    new Color(0.14f, 0.78f, 0.90f, 0.28f),
                    new Color(0.93f, 0.73f, 0.36f, 0.16f),
                    new Color(0.14f, 0.78f, 0.90f, 0.18f),
                    1f,
                    1f);
            }
            else
            {
                if (texture != null) button.style.backgroundImage = new StyleBackground(texture);
                button.style.color = RuntimeArtCatalog.Text;
            }
            RuntimeUiSkin.ApplyButtonMetrics(
                button,
                coolingDown ? RuntimeUiSpacing.CombatButtonCooldownMinWidth : RuntimeUiSpacing.CombatButtonReadyMinWidth,
                RuntimeUiSpacing.CombatButtonMinHeight,
                coolingDown ? RuntimeUiSpacing.CombatButtonCooldownFontSize : RuntimeUiSpacing.CombatButtonReadyFontSize,
                true);
            RuntimeUiSkin.ApplyPadding(
                button,
                RuntimeUiSpacing.CombatButtonPaddingHorizontal,
                RuntimeUiSpacing.CombatButtonPaddingHorizontal,
                RuntimeUiSpacing.CombatButtonPaddingTop,
                RuntimeUiSpacing.CombatButtonPaddingBottom);
        }

        internal static TextField NewTextField(string label, string value)
        {
            var field = new TextField(label) { value = value };
            RuntimeUiSkin.ApplyInputMetrics(field, RuntimeUiSpacing.BaseInputMaxWidth, marginTop: RuntimeUiSpacing.BaseInputMarginTop);
            return field;
        }

        internal static TextField NewLobbyTextField(string label, string value, string tooltip)
        {
            var field = NewTextField(label, value);
            ApplyLobbyInputStyle(field);
            if (!string.IsNullOrWhiteSpace(tooltip)) field.tooltip = tooltip;
            return field;
        }

        internal static void ApplyLobbyInputStyle(TextField field)
        {
            RuntimeUiSkin.ApplyInputMetrics(field, minHeight: RuntimeUiSpacing.BaseInputMinHeight);
            RuntimeUiSkin.ApplyPadding(field, RuntimeUiSpacing.BaseInputPaddingHorizontal, RuntimeUiSpacing.BaseInputPaddingVertical);
            RuntimeUiSkin.ApplyLobbyInputFrame(field);
        }

        internal static Button NewPrimaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.backgroundColor = RuntimeArtCatalog.Spirit;
            button.style.color = RuntimeArtCatalog.Background;
            RuntimeUiSkin.ApplyButtonTier(button, RuntimeUiButtonTier.Hero);
            button.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            var texture = LgoVisualAssetRegistryV3B.ButtonEnterWorldGoldTexture;
            if (texture != null)
            {
                button.style.backgroundColor = Color.clear;
                button.style.backgroundImage = new StyleBackground(texture);
            }
            return button;
        }

        internal static Button NewCompactPrimaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            RuntimeUiSkin.ApplyButtonTier(button, RuntimeUiButtonTier.Primary);
            ApplyCompactButtonPriority(button, true);
            return button;
        }

        internal static Button NewQuietButton(string label, Action action)
        {
            var button = NewButton(label, action);
            RuntimeUiSkin.ApplyButtonTier(button, RuntimeUiButtonTier.Small);
            button.style.backgroundColor = RuntimeArtCatalog.Background;
            button.style.color = RuntimeArtCatalog.Muted;
            return button;
        }

        internal static Button NewSecondaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            ApplyCompactButtonPriority(button, false);
            return button;
        }

        internal static Button NewCompactSecondaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            RuntimeUiSkin.ApplyButtonTier(button, RuntimeUiButtonTier.Standard);
            ApplyCompactButtonPriority(button, false);
            return button;
        }

        internal static void ApplyCompactButtonPriority(Button button, bool primary)
        {
            button.style.unityFontStyleAndWeight = primary ? FontStyle.Bold : FontStyle.Normal;
            if (primary)
                RuntimeUiSkin.ApplyCompactActionFrame(button, new Color(0.08f, 0.12f, 0.16f, 0.96f), RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold);
            else
                RuntimeUiSkin.ApplyCompactActionFrame(button, RuntimeUiSkin.BlueGlass, RuntimeUiSkin.MediumGoldBorder, RuntimeUiSkin.MediumGoldBorder, RuntimeUiSkin.MediumGoldBorder, RuntimeUiSkin.MediumGoldBorder);
        }

        internal static Button NewIconButton(string label, Texture2D texture, Action action)
        {
            var button = NewSecondaryButton(string.Empty, action);
            RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSizing.IconButtonMinWidth, RuntimeUiSizing.IconButtonMinHeight);
            button.style.flexDirection = FlexDirection.Row;
            button.style.alignItems = Align.Center;
            button.Add(NewIcon(texture, label));
            var text = new Label(label);
            text.style.marginLeft = RuntimeUiSpacing.IconButtonTextGap;
            RuntimeUiSkin.ApplyText(text, RuntimeArtCatalog.Text, bold: true);
            button.Add(text);
            return button;
        }

        internal static Toggle NewLocalSettingToggle(string label, bool value, Action changed)
        {
            var toggle = new Toggle(label) { value = value };
            toggle.style.minHeight = RuntimeUiSpacing.SettingToggleMinHeight;
            toggle.style.marginTop = RuntimeUiSpacing.SettingToggleMarginTop;
            toggle.style.marginBottom = 0;
            RuntimeUiSkin.ApplySettingToggleFrame(toggle, value ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Muted);
            var statePill = new Label();
            RuntimeUiSkin.ApplySettingToggleStatePill(statePill, value);
            toggle.Add(statePill);
            RuntimeUiSkin.ApplySettingToggleState(toggle, value);
            toggle.RegisterValueChangedCallback(evt =>
            {
                RuntimeUiSkin.ApplySettingToggleState(toggle, evt.newValue);
                changed();
            });
            return toggle;
        }

        internal static Button NewListButton(string name, string classId, Action action)
        {
            var button = NewSecondaryButton(name + "\n" + classId, action);
            button.AddToClassList("lgo-list-item");
            RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSpacing.ListButtonMinWidth, RuntimeUiSpacing.ListButtonMinHeight);
            RuntimeUiOverflowGuard.ApplyButton(button);
            button.style.minWidth = 0;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.style.paddingLeft = RuntimeUiSpacing.ListButtonPaddingLeft;
            ApplyListButtonSelection(button, false);
            return button;
        }

        internal static void ApplyListButtonSelection(Button button, bool selected)
        {
            button.EnableInClassList("lgo-list-selected", selected);
            var border = selected ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.SurfaceRaised;
            RuntimeUiSkin.ApplyCompactActionFrame(button,
                selected ? new Color(0.08f, 0.09f, 0.11f, 0.92f) : RuntimeUiSkin.DeepGlass,
                border, border, border, border);
            button.tooltip = selected ? "Nhân vật đang chọn" : "Chọn nhân vật tu luyện";
            if (selected)
                button.schedule.Execute(() => RevealSelectedListItem(button.GetFirstAncestorOfType<ScrollView>()));
        }

        private static void RevealSelectedListItem(ScrollView list)
        {
            if (list == null || list.panel == null) return;
            var selected = list.Q<Button>(className: "lgo-list-selected");
            if (selected != null && selected.resolvedStyle.height > 0)
                list.ScrollTo(selected);
        }

        internal static VisualElement NewRuntimeIcon(Texture2D texture, int size, string tooltip)
        {
            var icon = new VisualElement();
            icon.style.width = size;
            icon.style.height = size;
            icon.style.minWidth = size;
            icon.style.minHeight = size;
            RuntimeUiSkin.ApplyRuntimeIconFrame(icon, new Color(0.02f, 0.08f, 0.16f, 0.82f));
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = tooltip;
            return icon;
        }

        internal static VisualElement NewWorldTouchControlsOverlay()
        {
            var overlay = new VisualElement { name = "LGO World Touch Controls Overlay" };
            overlay.pickingMode = PickingMode.Ignore;
            overlay.style.position = Position.Absolute;
            overlay.style.left = 0;
            overlay.style.right = 0;
            overlay.style.top = 0;
            overlay.style.bottom = 0;
            overlay.style.overflow = Overflow.Hidden;
            return overlay;
        }

        internal static VisualElement NewWorldTouchPad()
        {
            var pad = new RuntimeTouchMovementPad { name = "LGO World Touch Movement Pad" };
            pad.pickingMode = PickingMode.Position;
            pad.style.position = Position.Absolute;
            pad.style.alignItems = Align.Center;
            pad.style.justifyContent = Justify.Center;
            pad.style.backgroundColor = new Color(0.003f, 0.018f, 0.040f, 0.46f);
            RuntimeUiSkin.ApplyEdgeFrame(
                pad,
                new Color(0.14f, 0.78f, 0.90f, 0.46f),
                new Color(0.93f, 0.73f, 0.36f, 0.24f),
                new Color(0.14f, 0.78f, 0.90f, 0.20f),
                new Color(0.93f, 0.73f, 0.36f, 0.18f),
                2f,
                1f);

            var label = new Label("Di chuyển");
            label.name = "LGO World Touch Movement Label";
            RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Muted, 12f, true, TextAnchor.MiddleCenter);
            label.style.whiteSpace = WhiteSpace.NoWrap;
            label.pickingMode = PickingMode.Ignore;

            var nub = new VisualElement { name = "LGO World Touch Movement Nub" };
            nub.pickingMode = PickingMode.Ignore;
            nub.style.position = Position.Absolute;
            nub.style.backgroundColor = new Color(0.14f, 0.78f, 0.90f, 0.30f);
            RuntimeUiSkin.ApplyEdgeFrame(nub, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, 1f, 1f);
            pad.Add(label);
            pad.Add(nub);
            return pad;
        }

        internal static VisualElement NewWorldTouchActionCluster()
        {
            var cluster = new VisualElement { name = "LGO World Touch Action Cluster" };
            cluster.style.position = Position.Absolute;
            cluster.style.flexDirection = FlexDirection.Row;
            cluster.style.flexWrap = Wrap.Wrap;
            cluster.style.alignItems = Align.FlexEnd;
            cluster.style.justifyContent = Justify.FlexEnd;
            cluster.style.overflow = Overflow.Hidden;
            return cluster;
        }

        internal static Button NewWorldTouchActionButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.name = "LGO World Touch Action Button " + label;
            button.style.marginTop = 0;
            button.style.backgroundColor = new Color(0.015f, 0.060f, 0.105f, 0.82f);
            button.style.color = RuntimeArtCatalog.Text;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.whiteSpace = WhiteSpace.NoWrap;
            RuntimeUiSkin.ApplyEdgeFrame(
                button,
                RuntimeArtCatalog.Spirit,
                RuntimeArtCatalog.Gold,
                new Color(0.14f, 0.78f, 0.90f, 0.26f),
                new Color(0.93f, 0.73f, 0.36f, 0.32f),
                1f,
                1f);
            return button;
        }

        internal static VisualElement NewImageLayer(string elementName, Texture2D texture, ScaleMode scaleMode, string tooltip = null)
        {
            var layer = new VisualElement();
            if (!string.IsNullOrWhiteSpace(elementName)) layer.name = elementName;
            layer.pickingMode = PickingMode.Ignore;
            layer.style.unityBackgroundScaleMode = scaleMode;
            if (texture != null) layer.style.backgroundImage = new StyleBackground(texture);
            if (!string.IsNullOrWhiteSpace(tooltip)) layer.tooltip = tooltip;
            return layer;
        }

        internal static VisualElement NewCombatCooldownIcon()
        {
            var icon = new VisualElement();
            icon.name = "LGO M6 Combat Cooldown Runtime Icon v0.46";
            icon.style.width = RuntimeUiSpacing.CooldownIconSize;
            icon.style.height = RuntimeUiSpacing.CooldownIconSize;
            icon.style.marginBottom = 0;
            RuntimeUiSkin.ApplyCombatCooldownIconFrame(icon);
            var texture = CombatPlaceholderAssets.CooldownReadyTexture;
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = "Sẵn sàng tấn công thử.";
            return icon;
        }

        internal static void ApplyCombatPanelSkin(VisualElement panel)
        {
            panel.style.backgroundImage = new StyleBackground();
            panel.style.backgroundColor = RuntimeUiSkin.BlueGlass;
            RuntimeUiSkin.ApplyEdgeFrame(panel, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold);
        }

        internal static void ApplyV2PanelSkin(VisualElement panel)
        {
            panel.style.backgroundImage = new StyleBackground();
            panel.style.backgroundColor = new Color(0.02f, 0.07f, 0.14f, 0.90f);
            RuntimeUiSkin.ApplyEdgeFrame(panel, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Spirit, RuntimeArtCatalog.Gold);
        }

        private static VisualElement NewLoginOrnamentLine(Color color)
        {
            var line = new VisualElement();
            line.style.flexGrow = 1;
            line.style.height = RuntimeUiSpacing.HairlineHeight;
            line.style.backgroundColor = color;
            line.style.opacity = 0.64f;
            return line;
        }

        private static Button NewButton(string label, Action action)
        {
            var button = new Button(action) { text = label };
            RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSpacing.BaseButtonMinWidth, RuntimeUiSpacing.BaseButtonMinHeight);
            button.style.marginTop = RuntimeUiSpacing.BaseButtonMarginTop;
            button.style.marginRight = RuntimeUiSpacing.BaseButtonMarginRight;
            RuntimeUiSkin.ApplyBaseButtonFrame(button);
            return button;
        }

        private static VisualElement NewIcon(Texture2D texture, string tooltip)
        {
            var icon = new VisualElement();
            icon.style.width = RuntimeUiSpacing.RuntimeIconSmall;
            icon.style.height = RuntimeUiSpacing.RuntimeIconSmall;
            icon.style.marginRight = RuntimeUiSpacing.IconMarginRight;
            icon.style.marginLeft = RuntimeUiSpacing.IconMarginLeft;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = tooltip;
            return icon;
        }
    }
}
