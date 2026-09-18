using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private static void ApplyLgoInventoryPanelShell(VisualElement panel)
        {
            panel.AddToClassList(LgoInventoryPanelShellClass);
            panel.AddToClassList("lgo-panel");
            panel.style.backgroundColor = new Color(.004f, .024f, .046f, .985f);
            ApplyLgoCharacterHubSectionFrame(panel);
            panel.style.paddingLeft = panel.style.paddingRight = 12;
            panel.style.paddingTop = panel.style.paddingBottom = 10;
            panel.style.minWidth = 0;
            ApplyLgoCharacterHubPanelSurface(panel);
        }

        private static void ApplyLgoInventoryButtonBase(Button button, bool touch)
        {
            button.AddToClassList(LgoInventoryButtonBaseClass);
            ApplyLgoButton(button);
            button.style.minHeight = touch ? 44 : 38;
            button.style.minWidth = 0;
            button.style.flexGrow = 1;
            button.style.flexBasis = 0;
            button.style.marginRight = 4;
            button.style.marginLeft = 0;
            button.style.fontSize = 15;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyLgoInventoryItemRow(Button row, bool touch)
        {
            row.AddToClassList(LgoInventoryItemRowClass);
            row.text = string.Empty;
            row.style.flexGrow = 0;
            row.style.flexBasis = StyleKeyword.Auto;
            row.style.minHeight = touch ? 58 : 52;
            row.style.marginBottom = 8;
            row.style.paddingLeft = row.style.paddingRight = 12;
            row.style.paddingTop = row.style.paddingBottom = 7;
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.justifyContent = Justify.FlexStart;
            row.style.unityTextAlign = TextAnchor.MiddleLeft;
        }

        private static void ApplyLgoInventoryCountBadge(Label badge)
        {
            badge.AddToClassList(LgoInventoryCountBadgeClass);
            badge.style.flexGrow = 0;
            badge.style.marginLeft = 10;
            badge.style.paddingLeft = badge.style.paddingRight = 8;
            badge.style.paddingTop = badge.style.paddingBottom = 3;
            badge.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoFrame(badge, new Color(.020f, .060f, .088f, .90f), new Color(.92f, .72f, .36f, .55f));
        }

        private static void ApplyLgoInventoryBadge(Label badge)
        {
            badge.AddToClassList(LgoInventoryBadgeClass);
            badge.AddToClassList("lgo-badge");
            badge.style.paddingLeft = badge.style.paddingRight = 10;
            badge.style.paddingTop = badge.style.paddingBottom = 4;
            badge.style.marginRight = 6;
            ApplyLgoFrame(badge, new Color(.018f, .060f, .096f, .88f), new Color(.72f, .62f, .38f, .62f));
        }

        private static void ApplyLgoInventoryStateBadge(Label badge)
        {
            badge.AddToClassList(LgoInventoryStateBadgeClass);
            badge.style.marginTop = 8;
            badge.style.alignSelf = Align.FlexStart;
            badge.style.paddingLeft = badge.style.paddingRight = 10;
            badge.style.paddingTop = badge.style.paddingBottom = 5;
            badge.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoFrame(badge, UiGold, new Color(.98f, .86f, .48f, .92f));
        }

        private static void ApplyLgoInventoryStatsCard(VisualElement card)
        {
            card.AddToClassList(LgoInventoryStatsCardClass);
            card.style.flexDirection = FlexDirection.Column;
            card.style.marginTop = 8;
            card.style.paddingLeft = card.style.paddingRight = 10;
            card.style.paddingTop = card.style.paddingBottom = 7;
            ApplyLgoFrame(card, new Color(.018f, .060f, .096f, .92f), new Color(.72f, .62f, .38f, .58f));
        }

        private static void ApplyLgoCharacterHubInspectorFacts(VisualElement facts)
        {
            ApplyLgoInventoryStatsCard(facts);
            RemoveLgoOuterBorder(facts);
            facts.style.backgroundColor = Color.clear;
            facts.style.paddingLeft = facts.style.paddingRight = 0;
            facts.style.paddingTop = facts.style.paddingBottom = 4;
        }

        internal static void ApplyLgoCharacterHubFactCopy(Label label, int size, bool accent)
        {
            label.AddToClassList("lgo-hub-copy");
            label.AddToClassList("lgo-hub-fact-copy");
            label.style.fontSize = size;
            label.style.color = accent ? new Color(.62f, 1f, .68f, 1f) : UiText;
            label.style.marginTop = label.style.marginBottom = 0;
            label.style.paddingTop = label.style.paddingBottom = 0;
            label.style.paddingLeft = label.style.paddingRight = 0;
            label.style.minWidth = 0;
            label.style.flexShrink = 1;
            label.style.whiteSpace = WhiteSpace.Normal;
        }

        internal static void ApplyLgoCharacterHubFactRow(VisualElement row, VisualElement marker,
            Label caption, Label value, int size, bool accent)
        {
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.flexShrink = 0;
            row.style.minWidth = 0;
            ApplyLgoCharacterHubFactCopy(caption, size, accent);
            ApplyLgoCharacterHubFactCopy(value, size, accent);
            caption.style.flexGrow = 1;
            caption.style.flexBasis = 0;
            // Give Yoga a definite value column before text measurement. A lone
            // percentage maxWidth measures one line, then wraps glyphs after layout.
            value.style.width = new Length(55, LengthUnit.Percent);
            value.style.maxWidth = StyleKeyword.None;
            value.style.flexShrink = 0;
            value.style.marginLeft = 10;
            value.style.unityTextAlign = TextAnchor.MiddleRight;
            value.style.unityFontStyleAndWeight = FontStyle.Bold;
            marker.style.width = 8;
            marker.style.height = 12;
            marker.style.marginRight = 10;
            marker.style.flexShrink = 0;
            marker.generateVisualContent += context =>
            {
                var p = context.painter2D;
                p.fillColor = accent ? new Color(.40f, .90f, .57f, 1f) : new Color(.28f, .73f, 1f, 1f);
                p.strokeColor = new Color(.70f, .90f, 1f, 1f);
                p.lineWidth = .6f;
                p.BeginPath();
                p.MoveTo(new Vector2(4, 1)); p.LineTo(new Vector2(7, 6));
                p.LineTo(new Vector2(4, 11)); p.LineTo(new Vector2(1, 6));
                p.ClosePath(); p.Fill(); p.Stroke();
            };
        }

        private static void ApplyLgoSkillDirectionalConnector(VisualElement connector, bool vertical)
        {
            connector.style.flexShrink = 0;
            connector.style.alignSelf = Align.Center;
            connector.style.width = vertical ? 12 : 40;
            connector.style.height = 12;
            const string marker = "lgo-skill-directional-connector";
            if (connector.ClassListContains(marker)) return;
            connector.AddToClassList(marker);
            connector.generateVisualContent += context =>
            {
                var p = context.painter2D;
                p.strokeColor = new Color(.20f, .58f, .82f, .85f);
                p.lineWidth = 1.6f;
                var start = vertical ? new Vector2(6, 1) : new Vector2(1, 6);
                var end = vertical ? new Vector2(6, 11) : new Vector2(39, 6);
                var along = (end - start).normalized * 4;
                var across = new Vector2(-along.y, along.x) * .65f;
                p.BeginPath(); p.MoveTo(start); p.LineTo(end);
                p.MoveTo(start + along + across); p.LineTo(start); p.LineTo(start + along - across);
                p.MoveTo(end - along + across); p.LineTo(end); p.LineTo(end - along - across);
                p.Stroke();
            };
        }

        private static void ApplyLgoCharacterHubCopyRhythm(VisualElement scope)
        {
            // Labels inherit theme padding/margins. Own them here so readable
            // text does not consume the inspector's action/cost budget twice.
            scope.Query<Label>().ForEach(label =>
            {
                label.AddToClassList("lgo-hub-copy");
                label.style.marginTop = label.style.marginBottom = 0;
                label.style.paddingTop = label.style.paddingBottom = 0;
                label.style.paddingLeft = label.style.paddingRight = 0;
                label.style.flexShrink = label.ClassListContains("lgo-hub-fact-copy") ? 1 : 0;
            });
        }

        private static void ApplyLgoCharacterHubReadingDivider(VisualElement divider)
        {
            divider.style.flexShrink = 0;
            divider.style.marginTop = divider.style.marginBottom = 6;
        }

        private static void ApplyLgoCharacterHubCostRow(VisualElement row)
        {
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.flexShrink = 0;
            row.style.minHeight = 52;
            row.style.marginTop = 8;
            row.style.paddingTop = row.style.paddingBottom = 4;
            row.style.borderTopWidth = 1;
            row.style.borderTopColor = new Color(.18f, .40f, .58f, .58f);
        }

        private static void ApplyLgoCharacterHubIdentityLabel(Label label, bool primary)
        {
            label.style.fontSize = primary ? 26 : 20;
            label.style.height = primary ? 34 : 28;
            label.style.flexShrink = 0;
            label.style.marginTop = label.style.marginBottom = 0;
            label.style.paddingTop = label.style.paddingBottom = 0;
            label.style.color = primary ? new Color(.96f, .98f, 1f, 1f) : UiGold;
        }

        private static void ApplyLgoCharacterHubVitalBar(UnityEngine.UIElements.ProgressBar bar)
        {
            bar.style.height = bar.style.minHeight = bar.style.maxHeight = 24;
            bar.style.fontSize = 16;
            bar.style.flexGrow = 0;
            bar.style.flexShrink = 0;
            bar.style.marginTop = bar.style.marginBottom = 0;
            bar.style.marginLeft = bar.style.marginRight = 0;
        }

        private static void ApplyLgoCharacterHubEquipmentSlot(Button slot)
        {
            ApplyLgoItemIcon(slot);
            slot.style.flexBasis = 76;
            slot.style.width = 76;
            slot.style.height = slot.style.minHeight = slot.style.maxHeight = 76;
            slot.style.marginTop = slot.style.marginBottom = 0;
            slot.style.marginLeft = slot.style.marginRight = 0;
            slot.style.paddingTop = slot.style.paddingBottom = 0;
            slot.style.paddingLeft = slot.style.paddingRight = 0;
        }

        private static void ApplyLgoInventoryContentFitPanel(VisualElement panel)
        {
            panel.AddToClassList(LgoInventoryContentFitPanelClass);
            panel.style.alignSelf = Align.Stretch;
            panel.style.flexGrow = 1;
            panel.style.flexShrink = 0;
        }

        private static void ApplyLgoInventoryCompactShell(VisualElement shell, bool compact)
        {
            shell.EnableInClassList(LgoInventoryCompactShellClass, compact);
        }

        private static void ApplyLgoStatusCard(VisualElement card, float horizontalPadding = 10, float verticalPadding = 8)
        {
            card.AddToClassList(LgoStatusCardClass);
            card.AddToClassList("lgo-status");
            card.style.paddingLeft = card.style.paddingRight = horizontalPadding;
            card.style.paddingTop = card.style.paddingBottom = verticalPadding;
            ApplyLgoFrame(card, new Color(.018f, .055f, .090f, .86f), new Color(.56f, .68f, .72f, .52f));
            ApplyLgoLayeredFrame(card);
        }

        private static void ApplyLgoSelectedTab(Button button, bool selected)
        {
            button.style.backgroundColor = selected
                ? new Color(.015f, .38f, .94f, .99f)
                : new Color(.012f, .060f, .118f, .99f);
            button.style.color = selected ? new Color(1f, 1f, .98f, 1f) : new Color(.78f, .84f, .91f, .96f);
            button.style.borderTopWidth = button.style.borderBottomWidth = selected ? 2 : 1;
            button.style.borderLeftWidth = button.style.borderRightWidth = selected ? 2 : 1;
            button.style.borderTopColor = button.style.borderLeftColor = button.style.borderRightColor = selected
                ? new Color(.22f, .82f, 1f, 1f)
                : new Color(.34f, .48f, .62f, .76f);
            button.style.borderBottomColor = selected ? new Color(.98f, .78f, .32f, 1f) : new Color(.34f, .48f, .62f, .76f);
        }

        private static void ApplyLgoInventoryMainTab(Button button, bool touch)
        {
            button.AddToClassList(LgoInventoryMainTabClass);
            button.AddToClassList("lgo-tab");
            ApplyLgoButton(button);
            button.style.minHeight = 52;
            button.style.fontSize = 23;
            button.style.marginRight = 6;
            ApplyLgoUiSkinSurface(button, ref _characterHubTabIdle, "character-hub-tab-idle");
            RemoveLgoOuterBorder(button);
            ApplyLgoInteractiveMotion(button);
        }

        private static void ApplyLgoCharacterHubTabState(Button button, bool selected)
        {
            ApplyLgoSelectedTab(button, selected);
            if (selected)
            {
                ApplyLgoUiSkinSurface(button, ref _characterHubTabSelected, "character-hub-tab-selected");
                button.experimental.animation.Start(
                    new StyleValues { opacity = .62f },
                    new StyleValues { opacity = 1f },
                    150);
            }
            else ApplyLgoUiSkinSurface(button, ref _characterHubTabIdle, "character-hub-tab-idle");
            RemoveLgoOuterBorder(button);
        }

        private static void ApplyLgoInventoryFilterChip(Button button, bool touch)
        {
            button.AddToClassList(LgoInventoryFilterChipClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.flexBasis = 100;
            button.style.minHeight = touch ? 36 : 28;
            button.style.marginRight = 6;
            button.style.marginBottom = 4;
            button.style.fontSize = 12;
        }

        private static void ApplyLgoInventoryToolbarAction(Button button, bool touch, bool disabled = true)
        {
            button.AddToClassList(LgoInventoryToolbarActionClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.flexBasis = 112;
            button.style.height = button.style.minHeight = button.style.maxHeight = 38;
            button.style.marginRight = 6;
            button.style.fontSize = 16;
            if (!disabled) return;
            button.SetEnabled(false);
            button.style.opacity = .58f;
            button.style.color = new Color(.70f, .78f, .78f, .82f);
        }

        private static void ApplyLgoCharacterHubPrimaryAction(Button button)
        {
            ApplyLgoButton(button);
            button.style.backgroundColor = new Color(.015f, .31f, .90f, .99f);
            button.style.color = new Color(.98f, .99f, 1f, 1f);
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopWidth = button.style.borderBottomWidth = 2;
            button.style.borderLeftWidth = button.style.borderRightWidth = 2;
            button.style.borderTopColor = button.style.borderLeftColor = button.style.borderRightColor = new Color(.22f, .82f, 1f, 1f);
            button.style.borderBottomColor = new Color(.98f, .78f, .32f, 1f);
            ApplyLgoUiSkinSurface(button, ref _characterHubActionBlue, "character-hub-action-blue");
            RemoveLgoOuterBorder(button);
            ApplyLgoInteractiveMotion(button);
        }

        private static void ApplyLgoCharacterHubInspectorAction(Button button)
        {
            button.style.minHeight = 52;
            button.style.fontSize = 20;
            button.style.paddingLeft = button.style.paddingRight = 14;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        private static void AnimateLgoCharacterHubOpen(VisualElement shell, VisualElement backdrop)
        {
            if (shell == null) return;
            shell.experimental.animation.Start(
                new StyleValues { opacity = .10f },
                new StyleValues { opacity = 1f },
                190);
            if (backdrop != null)
                backdrop.experimental.animation.Start(
                    new StyleValues { opacity = 0f },
                    new StyleValues { opacity = 1f },
                    160);
        }

        private static void ApplyLgoEquipmentLevelBadge(Label badge)
        {
            badge.AddToClassList("lgo-equipment-level-badge");
            badge.style.position = Position.Absolute;
            badge.style.right = 2;
            badge.style.bottom = 2;
            badge.style.paddingLeft = badge.style.paddingRight = 4;
            badge.style.paddingTop = badge.style.paddingBottom = 1;
            badge.style.backgroundColor = new Color(.005f, .018f, .035f, .92f);
            badge.style.borderTopLeftRadius = badge.style.borderTopRightRadius = 3;
            badge.style.borderBottomLeftRadius = badge.style.borderBottomRightRadius = 3;
            badge.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyLgoModalCloseButton(Button button, bool touch)
        {
            button.AddToClassList(LgoModalCloseButtonClass);
            ApplyLgoButton(button);
            button.style.flexGrow = 0;
            button.style.flexBasis = 52;
            button.style.minHeight = 52;
            button.style.fontSize = 27;
            button.style.marginRight = 0;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.backgroundColor = new Color(.012f, .050f, .090f, .99f);
            button.style.borderTopWidth = button.style.borderBottomWidth = 2;
            button.style.borderLeftWidth = button.style.borderRightWidth = 2;
            button.style.borderTopColor = button.style.borderBottomColor = UiGold;
            button.style.borderLeftColor = button.style.borderRightColor = new Color(.62f, .46f, .22f, .94f);
            ApplyLgoUiSkinSurface(button, ref _characterHubClose, "character-hub-close");
            RemoveLgoOuterBorder(button);
            ApplyLgoInteractiveMotion(button);
        }

        private static void ApplyLgoInventoryGridCell(VisualElement cell)
        {
            cell.AddToClassList(LgoInventoryGridCellClass);
            cell.style.flexGrow = 0;
            cell.style.flexBasis = new Length(InventoryGridCellBasisPercent, LengthUnit.Percent);
            cell.style.height = 116;
            cell.style.marginRight = 6;
            cell.style.marginBottom = 7;
            cell.style.alignItems = Align.Center;
            cell.style.justifyContent = Justify.Center;
        }

        private static void ApplyLgoInventoryBagGridCell(VisualElement cell)
        {
            ApplyLgoInventoryGridCell(cell);
            cell.AddToClassList("lgo-inventory-bag-grid-cell");
            cell.style.flexBasis = 84;
            cell.style.width = cell.style.minWidth = cell.style.maxWidth = 84;
            cell.style.height = cell.style.minHeight = cell.style.maxHeight = 84;
            cell.style.flexShrink = 0;
            cell.style.paddingLeft = cell.style.paddingRight = 4;
            cell.style.paddingTop = cell.style.paddingBottom = 4;
        }

        private static void FitCharacterHubBagGrid(VisualElement grid, float viewportWidth, float viewportHeight)
        {
            if (grid == null || viewportWidth <= 0 || viewportHeight <= 0) return;
            // Reserve all five gutters, including the last one. The width supplied
            // by ScrollView already excludes its scrollbar: never use panel width.
            const float gap = 6;
            var size = Mathf.Max(0, Mathf.Floor((Mathf.Min(viewportWidth, viewportHeight) - 5 * gap) / 5));
            foreach (var cell in grid.Children())
            {
                if (!cell.ClassListContains("lgo-inventory-bag-grid-cell")) continue;
                cell.style.flexBasis = size;
                cell.style.width = cell.style.minWidth = cell.style.maxWidth = size;
                cell.style.height = cell.style.minHeight = cell.style.maxHeight = size;
                cell.style.marginTop = cell.style.marginLeft = 0;
                cell.style.marginRight = cell.style.marginBottom = gap;
                foreach (var child in cell.Children())
                    if (child.ClassListContains("lgo-inventory-bag-icon"))
                        child.style.width = child.style.height = Mathf.Max(0, Mathf.Min(72, size - 12));
            }
        }

        private static void ApplyLgoInventoryBagIcon(VisualElement icon)
        {
            ApplyLgoItemIcon(icon);
            icon.AddToClassList("lgo-inventory-bag-icon");
            icon.style.width = icon.style.height = 72;
            icon.style.marginTop = icon.style.marginBottom = 0;
            icon.style.marginLeft = icon.style.marginRight = 0;
            icon.style.backgroundColor = Color.clear;
            RemoveLgoOuterBorder(icon);
        }

        private static void ApplyLgoCharacterHubUnavailableControl(Button button)
        {
            // Preserve geometry while making unavailable actions unmistakably non-interactive.
            button.AddToClassList("lgo-unavailable-control");
            button.SetEnabled(false);
            button.style.opacity = .48f;
            button.style.color = new Color(.88f, .92f, .94f, 1f);
        }

        private static void ApplyLgoInventoryCategoryItem(Button button, bool touch)
        {
            ApplyLgoInventoryFilterChip(button, touch);
            button.text = string.Empty;
            button.style.flexDirection = FlexDirection.Column;
            button.style.alignItems = Align.Center;
            button.style.justifyContent = Justify.Center;
            button.style.flexBasis = 100;
            button.style.flexShrink = 0;
            button.style.minWidth = 0;
            button.style.height = button.style.minHeight = button.style.maxHeight = 100;
            button.style.paddingTop = button.style.paddingBottom = 4;
            button.style.marginTop = 0;
            button.style.paddingLeft = button.style.paddingRight = 4;
            button.style.marginRight = 0;
            button.style.marginBottom = 6;
        }

        private static void ApplyLgoInventoryCategoryIcon(VisualElement icon)
        {
            icon.style.width = 64;
            icon.style.height = 64;
            icon.style.flexGrow = 0;
            icon.style.flexShrink = 0;
            icon.style.marginBottom = 2;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        private static void ApplyLgoSkillCategoryCard(Button button, bool touch)
        {
            ApplyLgoInventoryFilterChip(button, touch);
            button.AddToClassList("lgo-skill-category-card");
            button.text = string.Empty;
            button.style.flexDirection = FlexDirection.Column;
            button.style.alignItems = Align.Center;
            button.style.justifyContent = Justify.Center;
            button.style.flexBasis = StyleKeyword.Auto;
            button.style.flexShrink = 0;
            button.style.paddingLeft = button.style.paddingRight = 4;
            button.style.marginTop = 0;
            button.style.height = button.style.maxHeight = 128;
            button.style.minHeight = 128;
            button.style.marginRight = 0;
            button.style.marginBottom = 8;
        }

        private static void ApplyLgoSkillIcon(VisualElement icon, float size)
        {
            RemoveLgoOuterBorder(icon);
            icon.style.backgroundColor = Color.clear;
            icon.style.marginTop = icon.style.marginBottom = 0;
            icon.style.marginLeft = icon.style.marginRight = 0;
            icon.style.width = size;
            icon.style.height = size;
            icon.style.flexGrow = 0;
            icon.style.flexShrink = 0;
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        internal static VisualElement CreateLgoCircularIconFrame(string name, Sprite artwork, float size)
        {
            var frame = new VisualElement { name = name, pickingMode = PickingMode.Ignore };
            frame.AddToClassList("lgo-circular-icon-frame");
            ApplyLgoSkillIcon(frame, size);
            frame.style.position = Position.Absolute;
            frame.style.left = frame.style.top = 0;
            frame.style.backgroundImage = artwork == null ? StyleKeyword.None : new StyleBackground(artwork);
            return frame;
        }

        private static void BindLgoItemIconContent(VisualElement icon, Sprite content, string label, string itemId)
        {
            var notice = icon.Q<Label>(className: "lgo-item-artwork-notice");
            if (notice == null)
            {
                notice = LgoLabel("", 11, UiSubText);
                notice.name = icon.name + " Artwork Status";
                notice.AddToClassList("lgo-item-artwork-notice");
                notice.pickingMode = PickingMode.Ignore;
                notice.style.position = Position.Absolute;
                notice.style.left = notice.style.right = 2;
                notice.style.top = 2;
                notice.style.bottom = new Length(18, LengthUnit.Percent);
                notice.style.marginTop = notice.style.marginBottom = 0;
                notice.style.paddingTop = notice.style.paddingBottom = 0;
                notice.style.whiteSpace = WhiteSpace.Normal;
                notice.style.unityTextAlign = TextAnchor.MiddleCenter;
                icon.Add(notice);
            }
            icon.style.display = DisplayStyle.Flex;
            icon.style.backgroundImage = content == null ? StyleKeyword.None : new StyleBackground(content);
            icon.EnableInClassList("lgo-item-art-missing", content == null);
            notice.text = label + "\nChưa có ảnh";
            notice.style.display = content == null ? DisplayStyle.Flex : DisplayStyle.None;
            icon.tooltip = label + " · " + itemId + (content == null ? " · Chưa có ảnh vật phẩm được đăng ký" : "");
        }

        private static void BindLgoSkillIconLayers(VisualElement icon, Sprite content, Sprite frameArtwork,
            float size, string identity)
        {
            var frame = icon.Q(className: "lgo-skill-shared-frame");
            if (frame == null)
            {
                if (frameArtwork == null) throw new System.InvalidOperationException("Missing shared Skill frame");
                ApplyLgoSkillIcon(icon, size);
                frame = CreateLgoCircularIconFrame(icon.name + " Shared Skill Frame", frameArtwork, size);
                frame.AddToClassList("lgo-skill-shared-frame");
                icon.Add(frame);
                var notice = LgoLabel("", 12, UiSubText);
                notice.name = icon.name + " Missing Artwork";
                notice.pickingMode = PickingMode.Ignore;
                notice.AddToClassList("lgo-skill-artwork-notice");
                notice.style.position = Position.Absolute;
                notice.style.left = new Length(12, LengthUnit.Percent);
                notice.style.width = new Length(76, LengthUnit.Percent);
                notice.style.top = new Length(12, LengthUnit.Percent);
                notice.style.height = new Length(76, LengthUnit.Percent);
                notice.style.marginTop = notice.style.marginBottom = 0;
                notice.style.paddingTop = notice.style.paddingBottom = 0;
                notice.style.paddingLeft = notice.style.paddingRight = 0;
                notice.style.unityTextAlign = TextAnchor.MiddleCenter;
                icon.Add(notice);
            }
            icon.style.width = icon.style.height = size;
            frame.style.width = frame.style.height = size;
            icon.style.backgroundImage = content == null ? StyleKeyword.None : new StyleBackground(content);
            icon.EnableInClassList("lgo-skill-art-missing", content == null);
            var missing = icon.Q<Label>(icon.name + " Missing Artwork");
            missing.style.fontSize = Mathf.Clamp(size / 8, 9, 13);
            missing.text = string.IsNullOrEmpty(identity) ? "Chưa có icon" : identity + "\nChưa có icon";
            icon.tooltip = content == null ? identity + " · Artwork chưa được đăng ký" : identity;
            SetLgoSkillIconLayersVisible(icon, true);
        }

        private static void SetLgoSkillIconLayersVisible(VisualElement icon, bool visible)
        {
            var frame = icon.Q(className: "lgo-skill-shared-frame");
            if (frame != null) frame.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            var missing = icon.Q<Label>(icon.name + " Missing Artwork");
            if (missing != null) missing.style.display = visible && icon.ClassListContains("lgo-skill-art-missing")
                ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void ApplyLgoEquippedSkillSlot(VisualElement slot)
        {
            slot.AddToClassList("lgo-equipped-skill-slot");
            slot.style.width = 76;
            slot.style.height = 76;
            slot.style.flexGrow = 0;
            slot.style.flexShrink = 0;
            slot.style.alignItems = Align.Center;
            slot.style.justifyContent = Justify.Center;
            ApplyLgoFrame(slot, new Color(.015f, .060f, .105f, .96f), new Color(.64f, .72f, .82f, .72f));
        }

        private static void ApplyLgoSkillLevelBadge(Label label)
        {
            label.style.position = Position.Absolute;
            label.style.bottom = -8;
            label.style.height = 26;
            label.style.fontSize = 16;
            label.style.flexShrink = 0;
            label.style.marginTop = label.style.marginBottom = 0;
            label.style.paddingTop = label.style.paddingBottom = 0;
            label.style.paddingLeft = label.style.paddingRight = 8;
            label.style.backgroundColor = new Color(.01f, .035f, .06f, .96f);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyLgoCharacterHubLockedAction(Button button, bool primary)
        {
            if (primary) ApplyLgoCharacterHubPrimaryAction(button);
            else ApplyLgoGoldAction(button);
            ApplyLgoCharacterHubInspectorAction(button);
            button.style.color = primary ? new Color(.90f, .95f, 1f, 1f) : new Color(.20f, .16f, .10f, 1f);
            ApplyLgoCharacterHubUnavailableControl(button);
            button.tooltip = "Tính năng chưa khả dụng trong phiên bản này.";
        }

        private static void ApplyLgoSkillNode(Button node)
        {
            node.AddToClassList("lgo-skill-node");
            node.text = string.Empty;
            node.style.width = 112;
            node.style.minWidth = 112;
            node.style.maxWidth = 112;
            node.style.flexBasis = 112;
            node.style.marginTop = node.style.marginBottom = 0;
            node.style.marginLeft = node.style.marginRight = 0;
            node.style.flexGrow = 0;
            node.style.flexShrink = 0;
            node.style.height = 112;
            node.style.minHeight = 112;
            node.style.maxHeight = 112;
            node.style.paddingLeft = node.style.paddingRight = 4;
            node.style.paddingTop = node.style.paddingBottom = 4;
            node.style.flexDirection = FlexDirection.Column;
            node.style.alignItems = Align.Center;
            node.style.justifyContent = Justify.Center;
            node.style.borderTopLeftRadius = node.style.borderTopRightRadius = 56;
            node.style.borderBottomLeftRadius = node.style.borderBottomRightRadius = 56;
        }

        private static void ApplyLgoPotentialDataOverlay(Button node)
        {
            node.AddToClassList("lgo-potential-node");
            node.text = string.Empty;
            node.style.backgroundColor = Color.clear;
            node.style.borderLeftWidth = node.style.borderRightWidth = 0;
            node.style.borderTopWidth = node.style.borderBottomWidth = 0;
            node.style.width = node.style.minWidth = node.style.maxWidth = 140;
            node.style.height = node.style.minHeight = node.style.maxHeight = 172;
            node.style.flexBasis = 140;
            node.style.flexGrow = node.style.flexShrink = 0;
            node.style.paddingLeft = node.style.paddingRight = 0;
            node.style.paddingTop = node.style.paddingBottom = 0;
            node.style.flexDirection = FlexDirection.Column;
            node.style.alignItems = Align.Center;
            node.style.justifyContent = Justify.Center;
            ApplyLgoInteractiveMotion(node);
        }

        private static void ApplyLgoPotentialOverlayContent(VisualElement icon, Label title, Label value)
        {
            ApplyLgoSkillIcon(icon, CharacterHubPotentialTopology.NodeFrameSize);
            icon.style.position = Position.Absolute;
            icon.style.left = icon.style.top = CharacterHubPotentialTopology.NodeFrameInset;
            foreach (var label in new[] { title, value })
            {
                label.style.position = Position.Absolute;
                label.style.fontSize = 20;
                label.style.flexShrink = 0;
                label.style.whiteSpace = WhiteSpace.NoWrap;
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                label.style.marginTop = label.style.marginBottom = 0;
                label.style.marginLeft = label.style.marginRight = 0;
                label.style.paddingTop = label.style.paddingBottom = 0;
                label.style.paddingLeft = label.style.paddingRight = 0;
            }
            title.style.left = 0;
            title.style.top = 116;
            title.style.width = 124;
            title.style.height = 24;
            value.style.left = 20;
            value.style.top = 142;
            value.style.width = 82;
            value.style.height = 28;
        }

        private static void ApplyLgoSpiritPetHeroPreview(VisualElement preview)
        {
            preview.AddToClassList("lgo-spirit-pet-hero");
            ApplyLgoCharacterHubDetailCard(preview, 10, 8);
            preview.style.height = preview.style.minHeight = preview.style.maxHeight = 346;
            preview.style.backgroundColor = Color.clear;
            RemoveLgoOuterBorder(preview);
            preview.style.flexShrink = 0;
            preview.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        private static void ApplyLgoSpiritPetRoster(VisualElement roster)
        {
            roster.AddToClassList("lgo-spirit-pet-roster");
            roster.style.height = roster.style.minHeight = 100;
            roster.style.marginBottom = 0;
            roster.style.flexShrink = 0;
            roster.style.marginTop = 6;
            roster.style.justifyContent = Justify.SpaceBetween;
        }

        private static void ApplyLgoSpiritPetRosterCard(Button card, bool selected)
        {
            card.AddToClassList("lgo-spirit-pet-roster-card");
            ApplyLgoInventoryGridCell(card);
            card.style.flexBasis = new Length(23, LengthUnit.Percent);
            card.style.height = card.style.minHeight = card.style.maxHeight = 100;
            card.style.marginTop = card.style.marginBottom = 0;
            card.style.marginLeft = card.style.marginRight = 0;
            card.style.paddingTop = card.style.paddingBottom = 0;
            card.style.paddingLeft = card.style.paddingRight = 0;
            card.style.flexShrink = 0;
            card.style.flexDirection = FlexDirection.Column;
            card.style.alignItems = Align.Center;
            card.style.justifyContent = Justify.Center;
            ApplyLgoCharacterHubSelectionState(card, selected);
            card.EnableInClassList("lgo-spirit-pet-locked-roster", !selected);
        }

        private static void ApplyLgoSpiritPetStatRow(VisualElement row)
        {
            row.AddToClassList("lgo-spirit-pet-stat-row");
            row.style.alignItems = Align.Center;
            row.style.height = StyleKeyword.Auto;
            row.style.minHeight = 22;
            row.style.flexShrink = 0;
            row.style.marginBottom = 0;
        }

        private static void ApplyLgoSpiritPetSkillRow(VisualElement row)
        {
            row.AddToClassList("lgo-spirit-pet-skill-row");
            row.style.alignItems = Align.Center;
            row.style.minHeight = 64;
            row.style.flexShrink = 0;
            row.style.paddingTop = row.style.paddingBottom = 2;
            row.style.marginBottom = 0;
            row.style.borderBottomWidth = 1;
            row.style.borderBottomColor = new Color(.18f, .40f, .58f, .58f);
        }

        private static void ApplyLgoSpiritPetRosterContent(VisualElement art, Label name, Label level, bool selected)
        {
            ApplyLgoSkillIcon(art, 76);
            art.style.position = Position.Absolute;
            art.style.top = 2;
            art.style.left = Length.Percent(50);
            art.style.marginLeft = -38;
            name.style.display = DisplayStyle.None;
            level.style.position = Position.Absolute;
            level.style.left = level.style.right = level.style.bottom = 0;
            level.style.height = 22;
            level.style.fontSize = selected ? 16 : 14;
            level.style.whiteSpace = WhiteSpace.NoWrap;
            level.style.unityTextAlign = TextAnchor.MiddleCenter;
            level.style.backgroundColor = new Color(.008f, .030f, .050f, .94f);
        }

        private static void ApplyLgoInventorySearchField(TextField field, bool touch)
        {
            field.AddToClassList(LgoInventorySearchFieldClass);
            ApplyLgoInputField(field);
            field.style.flexGrow = 1;
            field.style.flexShrink = 1;
            field.style.minWidth = 156;
            field.style.maxWidth = 230;
            field.style.height = 38;
            field.style.marginLeft = 8;
            field.style.marginRight = 8;
            field.style.paddingLeft = field.style.paddingRight = 10;
            field.style.fontSize = 16;
            ApplyLgoInventorySearchInnerField(field);
            field.RegisterCallback<AttachToPanelEvent>(_ => ApplyLgoInventorySearchInnerField(field));
        }

        private static void ApplyLgoInventorySearchInnerField(TextField field)
        {
            var input = ApplyLgoTextFieldInnerFrame(field, LgoInventorySearchInputClass);
            if (input == null) return;
            input.style.paddingLeft = input.style.paddingRight = 8;
            input.style.paddingTop = input.style.paddingBottom = 0;
            input.style.fontSize = 16;
            input.style.unityTextAlign = TextAnchor.MiddleLeft;
            input.style.height = Length.Percent(100);
        }

    }
}
