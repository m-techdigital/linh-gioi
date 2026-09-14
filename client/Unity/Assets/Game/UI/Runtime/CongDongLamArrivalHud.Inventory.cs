using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _inventoryBackdrop, _inventoryItemsGrid, _inventoryFooter, _inventoryHeroPanel, _inventoryGridPanel, _inventoryDetailPanel, _characterHeroCard, _characterHeroPortrait, _characterHeroLoadoutStrip, _characterHeroLeftEquipmentRail, _characterHeroRightEquipmentRail, _characterStatStrip, _characterLoadoutMatrix, _inventoryBottomActions, _inventoryDetailStatsCard, _inventoryCategoryRail;
        private Label _inventoryModalTitle, _inventoryModalSubtitle, _inventoryHeroTitle, _inventoryHeroMeta, _characterHeroName, _characterHeroPower, _characterHeroLoadout, _inventoryCountBadge, _inventoryItemId, _inventoryItemState, _inventoryDetailHeader, _inventoryDetailIcon, _inventoryDetailRarity, _inventoryDetailSlotType, _inventoryDetailStateBadge, _inventoryDetailLevelChip, _inventoryDetailFitChip, _inventoryDetailStatsHeader, _inventoryDetailSetHeader, _inventoryDetailStatPrimary, _inventoryDetailStatFit, _suppliesTitle, _suppliesEmptyState;
        private Button _bagTab, _characterInfoTab, _skillsTab, _potentialTab, _spiritPetTab, _allItemsTab, _equipmentTab, _suppliesTab, _materialsTab, _otherItemsTab;
        private Button _inventoryDetailPrimaryAction, _inventoryDetailLockAction, _inventoryDetailSellAction;
        private TextField _inventorySearchField;
        private Button[] _equipmentTiles;
        private VisualElement[] _emptyBagSlots;
        private VisualElement[] _equipmentTileIcons, _equipmentRowIcons, _characterHeroQuickIcons;
        private Label[] _equipmentTileNames, _equipmentTileStates, _equipmentRowNames, _equipmentRowStates, _characterHeroQuickLevels;
        private Label _healthPotionName, _healthPotionCount, _healthPotionState, _manaPotionName, _manaPotionCount, _manaPotionState, _classRewardName, _classRewardCount, _classRewardState;
        private UnityEngine.UIElements.ProgressBar _characterHeroHealth, _characterHeroMana;
        private bool _characterInfoOpen, _suppliesOpen;
        private bool _inventoryWasOpen;
        private string _inventoryCategory = "all";
        private string _inventorySearchQuery = string.Empty;
        private string _selectedSupplyItemId = "health_potion";
        private readonly HashSet<string> _lockedEquipmentItemIds = new HashSet<string>();

        private bool IsInventoryCompactShellActive() => !_characterInfoOpen;

        private void RefreshInventoryShellMode()
        {
            if (_inventory == null) return;
            var compact = IsInventoryCompactShellActive();
            ApplyLgoInventoryCompactShell(_inventory, compact);
            var safe = _metrics.SafePanelRect;
            if (safe.width <= 0 || safe.height <= 0)
            {
                safe = _touch ? new Rect(0, 0, 800, 480) : new Rect(0, 0, 1600, 900);
            }
            var rect = CalculateInventoryModalRect(new Rect(0, 0, safe.width, safe.height), _touch);
            _inventory.style.height = CalculateInventoryShellHeight(rect, _touch, compact);
        }

        private Button InventoryButton(Action action, string name, string text = "")
        {
            var button = new Button(action) { name = name, text = text };
            ApplyLgoInventoryButtonBase(button, _touch);
            return button;
        }

        private static VisualElement InventoryRow(string name)
        {
            var row = new VisualElement { name = name };
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexShrink = 0;
            row.style.marginBottom = 6;
            return row;
        }

        private VisualElement InventoryPanel(string name)
        {
            var panel = new VisualElement { name = name };
            ApplyLgoInventoryPanelShell(panel);
            return panel;
        }

        private static Label InventoryBadge(string name, string text, Color color)
        {
            var badge = LgoLabel(text, 13, color, true);
            badge.name = name;
            ApplyLgoInventoryBadge(badge);
            return badge;
        }

        private Button InventoryCategory(Action action, string name, string categoryId, string label)
        {
            var button = InventoryButton(action, name);
            ApplyLgoInventoryCategoryItem(button, _touch);
            var icon = new VisualElement { name = "Map01A Inventory Category Icon " + categoryId };
            ApplyLgoInventoryCategoryIcon(icon);
            var sprite = _scene.GetMap01ABagCategoryIconSprite(categoryId);
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            var caption = LgoLabel(label, 12, UiText, true);
            caption.name = "Map01A Inventory Category Label " + categoryId;
            caption.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.Add(icon);
            button.Add(caption);
            return button;
        }


        private Button SupplyItemRow(Action action, string name, string itemId, out Label nameLabel, out Label countLabel, out Label stateLabel)
        {
            var row = InventoryButton(action, name);
            ApplyLgoInventoryBagGridCell(row);
            row.style.flexDirection = FlexDirection.Column;
            row.style.alignItems = Align.Center;
            row.style.unityTextAlign = TextAnchor.MiddleCenter;

            var icon = new VisualElement { name = "Map01A Supply Item Icon " + itemId };
            ApplyLgoItemIcon(icon);
            icon.style.width = 76;
            icon.style.height = 76;
            icon.style.marginTop = 0;
            icon.style.marginBottom = 4;
            icon.style.marginRight = 0;
            var sprite = _scene.GetMap01AItemThumbnailSprite(itemId);
            icon.style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
            icon.style.display = sprite == null ? DisplayStyle.None : DisplayStyle.Flex;
            row.Add(icon);

            var textGroup = new VisualElement { name = "Map01A Supply Item Text " + itemId };
            textGroup.style.flexGrow = 1;
            textGroup.style.minWidth = 0;
            textGroup.style.flexDirection = FlexDirection.Column;
            textGroup.style.alignItems = Align.Center;
            nameLabel = LgoLabel("", 11, UiText, true);
            nameLabel.name = "Map01A Supply Item Name " + itemId;
            nameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            nameLabel.style.display = DisplayStyle.None;
            stateLabel = LgoLabel("", 10, UiSubText);
            stateLabel.name = "Map01A Supply Item State " + itemId;
            stateLabel.style.display = DisplayStyle.None;
            textGroup.Add(nameLabel);
            textGroup.Add(stateLabel);

            countLabel = LgoLabel("", 12, UiGold, true);
            countLabel.name = "Map01A Supply Item Count " + itemId;
            ApplyLgoInventoryCountBadge(countLabel);
            countLabel.style.position = Position.Absolute;
            countLabel.style.right = 4;
            countLabel.style.bottom = 4;

            row.Add(textGroup); row.Add(countLabel);
            return row;
        }

        private void BuildInventory()
        {
            _inventoryBackdrop = new VisualElement { name = "Map01A Character Hub Backdrop", pickingMode = PickingMode.Ignore };
            Place(_inventoryBackdrop, 0, 0, 0, 0);
            ApplyLgoCharacterHubBackdrop(_inventoryBackdrop);
            _inventoryBackdrop.style.display = DisplayStyle.None;
            _inventory = new VisualElement { name = "Map01A Inventory" };
            ApplyLgoCharacterHubShell(_inventory); Place(_inventory, 72, 72, 86, 72);
            RuntimeUiTypography.ApplyBodyFont(_inventory);
            _inventory.style.flexDirection = FlexDirection.Column;
            _inventory.style.paddingLeft = _inventory.style.paddingRight = 12;
            _inventory.style.paddingTop = _inventory.style.paddingBottom = 12;

            var header = InventoryRow("Map01A Inventory Header");
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 2;
            var titleGroup = new VisualElement();
            titleGroup.style.flexGrow = 1;
            titleGroup.style.marginLeft = 34;
            _inventoryModalTitle = LgoTitleLabel("HÀNH TRANG", 26);
            _inventoryModalTitle.name = "Map01A Inventory Modal Title";
            ApplyLgoCharacterHubTitle(_inventoryModalTitle);
            _inventoryModalSubtitle = LgoSubtitleLabel("Túi đồ và thông tin nhân vật dùng chung chi tiết món", 13);
            _inventoryModalSubtitle.name = "Map01A Inventory Modal Subtitle";
            _inventoryModalSubtitle.style.display = DisplayStyle.None;
            titleGroup.Add(_inventoryModalTitle);
            titleGroup.Add(_inventoryModalSubtitle);
            _equipmentClass = InventoryButton(() => _scene.CycleCharacterHubClass(), "LGO Equipment Inventory Class");
            _equipmentClass.style.flexGrow = 0;
            _equipmentClass.style.flexBasis = 126;
            _equipmentClass.style.minHeight = 34;
            _equipmentClass.style.marginRight = 8;
            _equipmentClass.style.fontSize = 13;
            var close = InventoryButton(() => { if (_scene.InventoryOpen) _scene.ToggleInventory(); },
                "LGO Inventory Close", "×");
            close.tooltip = "Đóng hành trang (I / Esc)";
            ApplyLgoModalCloseButton(close, _touch);
            header.Add(titleGroup); header.Add(_equipmentClass); header.Add(close); _inventory.Add(header);
            var topOrnament = new VisualElement { name = "Map01A Inventory Modal Top Ornament" };
            ApplyLgoOrnamentRail(topOrnament);
            topOrnament.style.display = DisplayStyle.None;
            _inventory.Add(topOrnament);

            var mainTabs = InventoryRow("Map01A Inventory Main Tabs");
            mainTabs.style.width = 992;
            mainTabs.style.alignSelf = Align.FlexStart;
            mainTabs.style.marginLeft = 20;
            _characterInfoTab = InventoryButton(() => ShowInventoryMode(true), "Map01A Character Info Main Tab", "Nhân vật");
            _bagTab = InventoryButton(() => ShowInventoryMode(false), "Map01A Bag Main Tab", "Rương đồ");
            _skillsTab = InventoryButton(() => ShowCharacterHubPreviewMode(CharacterHubMode.Skills), "Map01A Skills Main Tab", "Kỹ năng");
            _potentialTab = InventoryButton(() => ShowCharacterHubPreviewMode(CharacterHubMode.Potential), "Map01A Potential Main Tab", "Tiềm năng");
            _spiritPetTab = InventoryButton(() => ShowCharacterHubPreviewMode(CharacterHubMode.SpiritPet), "Map01A Spirit Pet Main Tab", "Linh thú");
            foreach (var tab in new[] { _characterInfoTab, _bagTab, _skillsTab, _potentialTab, _spiritPetTab })
            {
                ApplyLgoInventoryMainTab(tab, _touch);
            }
            _spiritPetTab.style.marginRight = 0;
            mainTabs.Add(_characterInfoTab);
            mainTabs.Add(_bagTab);
            mainTabs.Add(_skillsTab);
            mainTabs.Add(_potentialTab);
            mainTabs.Add(_spiritPetTab);
            _inventory.Add(mainTabs);

            var body = new VisualElement { name = "Map01A Inventory Body" };
            body.style.flexDirection = FlexDirection.Row;
            body.style.flexGrow = 1;
            body.style.minHeight = 0;
            body.style.marginTop = 6;
            body.style.justifyContent = Justify.Center;
            _inventory.Add(body);

            var bottomOrnament = new VisualElement { name = "Map01A Inventory Modal Bottom Ornament" };
            ApplyLgoOrnamentRail(bottomOrnament);
            bottomOrnament.style.marginTop = 10;
            bottomOrnament.style.marginBottom = 0;
            bottomOrnament.style.display = DisplayStyle.None;
            _inventory.Add(bottomOrnament);

            _inventoryDetailPanel = InventoryPanel("Map01A Inventory Detail Panel");
            _inventoryDetailPanel.style.flexGrow = 0;
            _inventoryDetailPanel.style.flexBasis = InventoryDesktopDetailColumnWidth;
            _inventoryDetailPanel.style.marginLeft = InventoryDesktopColumnGap;
            ApplyLgoCharacterHubDetailCard(_inventoryDetailPanel);
            _inventoryFooter = new VisualElement { name = "Map01A Inventory Footer" };
            _inventoryFooter.style.flexGrow = 0;
            _inventoryFooter.style.flexShrink = 0;
            var detailScroll = new ScrollView(ScrollViewMode.Vertical)
            {
                name = "Map01A Inventory Detail Scroll",
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            RuntimeUiOverflowGuard.ApplyBoundedScroll(detailScroll, 900);
            detailScroll.style.flexGrow = 1;
            detailScroll.style.minHeight = 0;
            detailScroll.contentViewport.RegisterCallback<GeometryChangedEvent>(evt =>
                detailScroll.contentContainer.style.width = evt.newRect.width);
            detailScroll.Add(_inventoryFooter);
            _inventoryDetailPanel.Add(detailScroll);
            _inventoryDetailHeader = LgoLabel("CHI TIẾT MÓN", 15, new Color(.73f, .85f, .88f, .92f), true);
            _inventoryDetailHeader.name = "Map01A Inventory Detail Header";
            _inventoryDetailHeader.style.display = DisplayStyle.None;
            _inventoryFooter.Add(_inventoryDetailHeader);
            var detailHero = InventoryRow("Map01A Inventory Detail Hero");
            detailHero.style.alignItems = Align.Center;
            detailHero.style.marginTop = 8;
            detailHero.style.marginBottom = 4;
            _inventoryDetailIcon = LgoLabel("", 42, UiGold, true);
            _inventoryDetailIcon.name = "Map01A Inventory Detail Icon";
            ApplyLgoItemIcon(_inventoryDetailIcon);
            ApplyLgoCharacterHubHeroIconFrame(_inventoryDetailIcon);
            _inventoryDetailIcon.style.width = 112;
            _inventoryDetailIcon.style.height = 112;
            _inventoryDetailIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            _inventoryDetailIcon.style.marginTop = 0;
            _inventoryDetailIcon.style.marginBottom = 0;
            _inventoryDetailIcon.style.marginRight = 12;
            detailHero.Add(_inventoryDetailIcon);
            var detailHeroText = new VisualElement { name = "Map01A Inventory Detail Hero Text" };
            detailHeroText.style.flexGrow = 1;
            detailHeroText.style.minWidth = 0;
            detailHeroText.style.flexDirection = FlexDirection.Column;
            var detailTitleRow = InventoryRow("Map01A Inventory Detail Title Row");
            detailTitleRow.style.alignItems = Align.Center;
            detailTitleRow.style.marginBottom = 0;
            _equipmentDetail = LgoLabel("", 24, UiGold, true);
            _equipmentDetail.name = "Map01A Inventory Detail Item Name";
            _equipmentDetail.style.flexGrow = 1;
            _equipmentDetail.style.minWidth = 0;
            detailTitleRow.Add(_equipmentDetail);
            _inventoryDetailLevelChip = InventoryBadge("Map01A Inventory Detail Level Chip", "Lv1", new Color(.86f, .94f, .90f, .96f));
            _inventoryDetailLevelChip.style.marginRight = 0;
            detailTitleRow.Add(_inventoryDetailLevelChip);
            detailHeroText.Add(detailTitleRow);
            _inventoryDetailRarity = LgoLabel("", 16, new Color(.74f, .92f, 1f, .94f), true);
            _inventoryDetailRarity.name = "Map01A Inventory Detail Rarity";
            _inventoryDetailRarity.style.marginTop = 2;
            detailHeroText.Add(_inventoryDetailRarity);
            _inventoryDetailSlotType = LgoLabel("", 14, new Color(.88f, .94f, .92f, .94f), true);
            _inventoryDetailSlotType.name = "Map01A Inventory Detail Slot Type";
            _inventoryDetailSlotType.style.marginTop = 4;
            detailHeroText.Add(_inventoryDetailSlotType);
            _inventoryDetailStateBadge = LgoLabel("", 13, new Color(.12f, .08f, .03f, 1f), true);
            _inventoryDetailStateBadge.name = "Map01A Inventory Detail State Badge";
            ApplyLgoInventoryStateBadge(_inventoryDetailStateBadge);
            detailHeroText.Add(_inventoryDetailStateBadge);
            detailHero.Add(detailHeroText);
            _inventoryFooter.Add(detailHero);
            _inventoryFooter.Add(LgoDivider("Map01A Inventory Detail Divider"));
            _inventoryItemId = LgoLabel("", 14, new Color(.78f, .88f, .90f, .88f));
            _inventoryItemId.style.marginTop = 2;
            _inventoryItemId.style.display = DisplayStyle.None;
            _inventoryFooter.Add(_inventoryItemId);
            _inventoryItemState = LgoLabel("", 13, new Color(.91f, .93f, .84f, .96f));
            _inventoryItemState.style.marginTop = 8;
            _inventoryFooter.Add(_inventoryItemState);

            _inventoryDetailStatsCard = new VisualElement { name = "Map01A Inventory Detail Stats Card" };
            ApplyLgoInventoryStatsCard(_inventoryDetailStatsCard);
            _inventoryDetailStatsHeader = LgoLabel("THUỘC TÍNH", 14, new Color(.40f, .78f, 1f, .98f), true);
            _inventoryDetailStatsHeader.name = "Map01A Inventory Detail Stats Header";
            _inventoryDetailStatsCard.Add(_inventoryDetailStatsHeader);
            _inventoryDetailStatPrimary = LgoLabel("", 14, new Color(.76f, .92f, 1f, .96f), true);
            _inventoryDetailStatPrimary.name = "Map01A Inventory Detail Stat Primary";
            _inventoryDetailStatPrimary.style.marginTop = 6;
            _inventoryDetailStatsCard.Add(_inventoryDetailStatPrimary);
            var detailSetDivider = LgoDivider("Map01A Inventory Detail Set Divider");
            detailSetDivider.style.marginTop = 9;
            detailSetDivider.style.marginBottom = 8;
            _inventoryDetailStatsCard.Add(detailSetDivider);
            _inventoryDetailSetHeader = LgoLabel("BỘ TRANG BỊ HIỆN TẠI", 14, new Color(.40f, .78f, 1f, .98f), true);
            _inventoryDetailSetHeader.name = "Map01A Inventory Detail Set Header";
            _inventoryDetailStatsCard.Add(_inventoryDetailSetHeader);
            _inventoryDetailFitChip = InventoryBadge("Map01A Inventory Detail Fit Chip", "Võ · Nam", UiSubText);
            _inventoryDetailFitChip.style.marginTop = 6;
            _inventoryDetailFitChip.style.alignSelf = Align.FlexStart;
            _inventoryDetailStatsCard.Add(_inventoryDetailFitChip);
            _inventoryDetailStatFit = LgoLabel("", 13, new Color(.76f, 1f, .70f, .96f), true);
            _inventoryDetailStatFit.name = "Map01A Inventory Detail Stat Fit";
            _inventoryDetailStatFit.style.marginTop = 4;
            _inventoryDetailStatsCard.Add(_inventoryDetailStatFit);
            _inventoryFooter.Add(_inventoryDetailStatsCard);
            var actions = InventoryRow("Map01A Inventory Equipment Actions");
            actions.style.marginTop = 10;
            actions.style.marginBottom = 10;
            actions.style.flexShrink = 0;
            _inventoryDetailPrimaryAction = InventoryButton(UseInventoryDetailPrimaryAction, "Map01A Inventory Detail Primary Action");
            ApplyLgoCharacterHubPrimaryAction(_inventoryDetailPrimaryAction);
            _inventoryDetailPrimaryAction.style.minHeight = _touch ? 44 : 38;
            _inventoryDetailLockAction = InventoryButton(ToggleSelectedEquipmentLock, "Map01A Inventory Detail Lock Action", "Khóa");
            ApplyLgoCharacterHubGoldAction(_inventoryDetailLockAction);
            _inventoryDetailSellAction = InventoryButton(() => { }, "Map01A Inventory Detail Sell Action", "Bán");
            ApplyLgoButton(_inventoryDetailSellAction);
            _inventoryDetailSellAction.SetEnabled(false);
            _inventoryDetailSellAction.tooltip = "Tính năng bán vật phẩm chưa khả dụng";
            _equipmentToggle = InventoryButton(() => { _scene.ToggleVoEquipmentSlot(); RefreshInventoryEquipmentTiles(); RefreshInventoryDetailCard(); }, "LGO Equipment Inventory Toggle");
            _equipmentToggle.style.display = DisplayStyle.None;
            _equipmentVariant = InventoryButton(() => { _scene.CycleVoSelectedEquipmentItemLevel(); RefreshInventoryEquipmentTiles(); RefreshInventoryDetailCard(); }, "LGO Equipment Inventory Variant");
            _equipmentVariant.style.display = DisplayStyle.None;
            _equipmentVariant.style.minHeight = _touch ? 44 : 38;
            actions.Add(_inventoryDetailPrimaryAction); actions.Add(_inventoryDetailLockAction); actions.Add(_inventoryDetailSellAction); actions.Add(_equipmentToggle); actions.Add(_equipmentVariant);
            _inventoryDetailPanel.Add(actions);

            _inventoryGridPanel = InventoryPanel("Map01A Inventory Grid Panel");
            ApplyLgoInventoryContentFitPanel(_inventoryGridPanel);
            _inventoryGridPanel.style.flexBasis = InventoryDesktopMainColumnWidth;
            _inventoryGridPanel.style.marginRight = 0;
            body.Add(_inventoryGridPanel);
            var gridAccent = new VisualElement { name = "Map01A Inventory Grid Accent Rail" };
            ApplyLgoOrnamentRail(gridAccent);
            gridAccent.style.marginBottom = 10;
            _inventoryGridPanel.Add(gridAccent);

            var gridWorkspace = new VisualElement { name = "Map01A Inventory Grid Workspace" };
            gridWorkspace.style.flexDirection = FlexDirection.Row;
            gridWorkspace.style.flexGrow = 1;
            gridWorkspace.style.minHeight = 0;
            _inventoryGridPanel.Add(gridWorkspace);

            _inventoryCategoryRail = new VisualElement { name = "Map01A Inventory Category Rail" };
            _inventoryCategoryRail.style.flexDirection = FlexDirection.Column;
            _inventoryCategoryRail.style.flexGrow = 0;
            _inventoryCategoryRail.style.flexBasis = 108;
            _inventoryCategoryRail.style.marginRight = 10;
            gridWorkspace.Add(_inventoryCategoryRail);

            _allItemsTab = InventoryCategory(() => ShowInventoryCategory("all"), "Map01A All Items Category", "all", "Tất cả");
            _equipmentTab = InventoryCategory(() => ShowInventoryCategory("equipment"), "Map01A Equipment Tab", "equipment", "Trang bị");
            _suppliesTab = InventoryCategory(() => ShowInventoryCategory("items"), "Map01A Supplies Tab", "items", "Vật phẩm");
            _materialsTab = InventoryCategory(() => { }, "Map01A Materials Category", "materials", "Nguyên liệu");
            _otherItemsTab = InventoryCategory(() => { }, "Map01A Other Items Category", "other", "Khác");
            foreach (var categoryTab in new[] { _allItemsTab, _equipmentTab, _suppliesTab, _materialsTab, _otherItemsTab })
            {
                _inventoryCategoryRail.Add(categoryTab);
            }
            ApplyLgoDisabledAction(_materialsTab);
            ApplyLgoDisabledAction(_otherItemsTab);

            var gridContent = new VisualElement { name = "Map01A Inventory Grid Content" };
            gridContent.style.flexDirection = FlexDirection.Column;
            gridContent.style.flexGrow = 1;
            gridContent.style.minWidth = 0;
            gridWorkspace.Add(gridContent);

            var gridStatus = InventoryRow("Map01A Inventory Grid Status");
            gridStatus.style.alignItems = Align.Center;
            gridStatus.style.marginTop = 2;
            gridStatus.style.marginBottom = 8;
            _inventoryCountBadge = InventoryBadge("Map01A Inventory Count Badge", "56/120 ô", new Color(.86f, .94f, .90f, .96f));
            gridStatus.Add(_inventoryCountBadge);
            var equippedBadge = InventoryBadge("Map01A Inventory Equipped Badge", "10/10 đang mặc", new Color(.76f, 1f, .70f, .94f));
            equippedBadge.style.display = DisplayStyle.None;
            gridStatus.Add(equippedBadge);
            _inventorySearchField = new TextField { name = "Map01A Inventory Search" };
            _inventorySearchField.textEdition.placeholder = "Tìm vật phẩm...";
            _inventorySearchField.tooltip = "Tìm trong Rương đồ";
            ApplyLgoInventorySearchField(_inventorySearchField, _touch);
            _inventorySearchField.RegisterValueChangedCallback(evt =>
            {
                _inventorySearchQuery = NormalizeInventorySearch(evt.newValue);
                RefreshInventoryVisibility();
            });
            gridStatus.Add(_inventorySearchField);
            var gridStatusSpacer = new VisualElement();
            gridStatusSpacer.style.flexGrow = 1;
            gridStatus.Add(gridStatusSpacer);
            var sortBadge = InventoryBadge("Map01A Inventory Sort Badge", "Sắp xếp: mặc định", UiSubText);
            gridStatus.Add(sortBadge);
            gridContent.Add(gridStatus);

            var scroll = new ScrollView(ScrollViewMode.Vertical)
            {
                name = "LGO Inventory Scroll",
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            RuntimeUiOverflowGuard.ApplyBoundedScroll(scroll, 900);
            scroll.style.flexGrow = 1;
            scroll.style.flexShrink = 1;
            scroll.style.minHeight = 0;
            scroll.contentViewport.RegisterCallback<GeometryChangedEvent>(evt =>
                scroll.contentContainer.style.width = evt.newRect.width);
            gridContent.Add(scroll);

            _inventoryBottomActions = InventoryRow("Map01A Inventory Bottom Actions");
            _inventoryBottomActions.style.marginTop = 8;
            _inventoryBottomActions.style.marginBottom = 0;
            _inventoryBottomActions.style.justifyContent = Justify.FlexEnd;
            _inventoryBottomActions.style.alignItems = Align.Center;
            var sortButton = InventoryButton(() => { }, "Map01A Inventory Sort Action", "Sắp xếp");
            var splitButton = InventoryButton(() => { }, "Map01A Inventory Split Action", "Tách");
            var quickSellButton = InventoryButton(() => { }, "Map01A Inventory Quick Sell Action", "Bán nhanh");
            foreach (var actionButton in new[] { sortButton, splitButton, quickSellButton })
            {
                ApplyLgoInventoryToolbarAction(actionButton, _touch);
            }
            splitButton.style.display = DisplayStyle.None;
            foreach (var visibleAction in new[] { sortButton, quickSellButton })
            {
                visibleAction.style.flexGrow = 1;
                visibleAction.style.flexBasis = 0;
            }
            _inventoryBottomActions.Add(sortButton);
            _inventoryBottomActions.Add(splitButton);
            _inventoryBottomActions.Add(quickSellButton);
            gridContent.Add(_inventoryBottomActions);

            _inventoryHeroPanel = InventoryPanel("Map01A Inventory Character Panel");
            _inventoryHeroPanel.style.flexGrow = 0;
            _inventoryHeroPanel.style.flexBasis = InventoryDesktopMainColumnWidth;
            body.Add(_inventoryHeroPanel);
            _inventoryHeroTitle = LgoLabel("", 22, UiGold, true);
            _inventoryHeroMeta = LgoLabel("", 15, new Color(.78f, .94f, .96f, .92f));
            _inventoryHeroTitle.name = "Map01A Inventory Hero Title";
            _inventoryHeroMeta.name = "Map01A Inventory Hero Meta";
            _inventoryHeroTitle.style.display = DisplayStyle.None;
            _inventoryHeroMeta.style.display = DisplayStyle.None;
            _inventoryHeroPanel.Add(_inventoryHeroTitle);
            _inventoryHeroPanel.Add(_inventoryHeroMeta);
            _equipmentSlotIds = _scene.VoEquipmentSlotIds;

            _characterHeroCard = new VisualElement { name = "Map01A Character Hero Card" };
            _characterHeroCard.style.flexDirection = FlexDirection.Row;
            _characterHeroCard.style.alignItems = Align.Center;
            _characterHeroCard.style.justifyContent = Justify.Center;
            _characterHeroCard.style.height = 432;
            _characterHeroCard.style.minHeight = 432;
            _characterHeroCard.style.flexShrink = 0;
            _characterHeroCard.style.marginTop = 0;
            _characterHeroCard.style.marginBottom = 4;
            ApplyLgoCharacterHubDetailCard(_characterHeroCard, 12, 10);
            _characterHeroCard.style.paddingLeft = _characterHeroCard.style.paddingRight = 0;
            _characterHeroCard.style.paddingTop = _characterHeroCard.style.paddingBottom = 0;
            _characterHeroCard.style.backgroundColor = Color.clear;
            _characterHeroCard.style.borderTopWidth = 0;
            _characterHeroCard.style.borderBottomWidth = 0;
            _characterHeroCard.style.borderLeftWidth = 0;
            _characterHeroCard.style.borderRightWidth = 0;
            _inventoryHeroPanel.Add(_characterHeroCard);

            _characterHeroLeftEquipmentRail = new VisualElement { name = "Map01A Character Hero Left Equipment Rail" };
            _characterHeroLeftEquipmentRail.style.flexDirection = FlexDirection.Column;
            _characterHeroLeftEquipmentRail.style.flexGrow = 0;
            _characterHeroLeftEquipmentRail.style.flexShrink = 0;
            _characterHeroLeftEquipmentRail.style.width = 76;
            _characterHeroLeftEquipmentRail.style.height = 420;
            _characterHeroLeftEquipmentRail.style.justifyContent = Justify.SpaceBetween;
            _characterHeroLeftEquipmentRail.style.marginRight = 8;
            _characterHeroCard.Add(_characterHeroLeftEquipmentRail);

            _characterHeroPortrait = new VisualElement { name = "Map01A Character Hero Portrait" };
            _characterHeroPortrait.style.width = 400;
            _characterHeroPortrait.style.height = 428;
            _characterHeroPortrait.style.flexGrow = 0;
            _characterHeroPortrait.style.flexShrink = 0;
            _characterHeroPortrait.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            _characterHeroPortrait.style.marginRight = 8;
            _characterHeroPortrait.style.marginTop = 0;
            _characterHeroPortrait.style.marginBottom = 0;
            _characterHeroCard.Add(_characterHeroPortrait);

            _characterHeroRightEquipmentRail = new VisualElement { name = "Map01A Character Hero Right Equipment Rail" };
            _characterHeroRightEquipmentRail.style.flexDirection = FlexDirection.Column;
            _characterHeroRightEquipmentRail.style.flexGrow = 0;
            _characterHeroRightEquipmentRail.style.flexShrink = 0;
            _characterHeroRightEquipmentRail.style.width = 76;
            _characterHeroRightEquipmentRail.style.height = 420;
            _characterHeroRightEquipmentRail.style.justifyContent = Justify.SpaceBetween;
            _characterHeroRightEquipmentRail.style.marginRight = 0;
            _characterHeroCard.Add(_characterHeroRightEquipmentRail);

            var heroInfo = new VisualElement { name = "Map01A Character Hero Info" };
            heroInfo.style.flexGrow = 0;
            heroInfo.style.minWidth = 0;
            heroInfo.style.flexDirection = FlexDirection.Column;
            heroInfo.style.alignItems = Align.Center;
            heroInfo.style.marginTop = 4;
            _characterHeroName = LgoLabel("", 19, UiGold, true);
            _characterHeroName.name = "Map01A Character Hero Name";
            _characterHeroPower = LgoLabel("", 17, new Color(.96f, .91f, .76f, .96f), true);
            _characterHeroPower.name = "Map01A Character Hero Power";
            _characterHeroLoadout = LgoLabel("", 13, new Color(.76f, 1f, .70f, .94f), true);
            _characterHeroLoadout.name = "Map01A Character Hero Loadout";
            _characterHeroLoadout.style.display = DisplayStyle.None;
            _characterHeroLoadoutStrip = new VisualElement { name = "Map01A Character Hero Loadout Strip" };
            _characterHeroLoadoutStrip.style.display = DisplayStyle.None;
            _characterHeroQuickIcons = new VisualElement[_equipmentSlotIds.Count];
            _characterHeroQuickLevels = new Label[_equipmentSlotIds.Count];
            for (var heroIconIndex = 0; heroIconIndex < _characterHeroQuickIcons.Length; heroIconIndex++)
            {
                var slotId = _equipmentSlotIds[heroIconIndex];
                var quickIcon = InventoryButton(() => SelectInventoryEquipmentSlot(slotId), "Map01A Character Hero Quick Icon " + heroIconIndex);
                ApplyLgoItemIcon(quickIcon);
                quickIcon.style.flexGrow = 0;
                quickIcon.style.flexShrink = 0;
                quickIcon.style.flexBasis = 68;
                quickIcon.style.width = 68;
                quickIcon.style.height = 68;
                quickIcon.style.minHeight = 68;
                quickIcon.style.maxHeight = 68;
                quickIcon.style.marginTop = 0;
                quickIcon.style.marginBottom = 0;
                quickIcon.style.marginRight = 0;
                quickIcon.tooltip = EquipmentDisplayName(slotId);
                var levelBadge = LgoLabel("", 11, new Color(.98f, .99f, 1f, 1f), true);
                levelBadge.name = "Map01A Character Hero Quick Level " + heroIconIndex;
                ApplyLgoEquipmentLevelBadge(levelBadge);
                quickIcon.Add(levelBadge);
                _characterHeroQuickIcons[heroIconIndex] = quickIcon;
                _characterHeroQuickLevels[heroIconIndex] = levelBadge;
                if (heroIconIndex < 5) _characterHeroLeftEquipmentRail.Add(quickIcon);
                else _characterHeroRightEquipmentRail.Add(quickIcon);
            }
            heroInfo.Add(_characterHeroName);
            heroInfo.Add(_characterHeroPower);
            var heroVitalsBars = InventoryRow("Map01A Character Hero Vitals Bars");
            heroVitalsBars.style.width = 290;
            heroVitalsBars.style.marginTop = 2;
            heroVitalsBars.style.marginBottom = 2;
            _characterHeroHealth = MakeVital("Map01A Character Hero Health", new Color(.67f, .16f, .15f));
            _characterHeroMana = MakeVital("Map01A Character Hero Mana", new Color(.12f, .37f, .64f));
            _characterHeroHealth.style.flexGrow = 1;
            _characterHeroHealth.style.marginRight = 8;
            _characterHeroMana.style.flexGrow = 1;
            heroVitalsBars.Add(_characterHeroHealth);
            heroVitalsBars.Add(_characterHeroMana);
            heroInfo.Add(heroVitalsBars);
            heroInfo.Add(_characterHeroLoadout);
            heroInfo.Add(_characterHeroLoadoutStrip);
            _inventoryHeroPanel.Add(heroInfo);

            _characterStatStrip = new VisualElement { name = "Map01A Character Stat Strip" };
            _characterStatStrip.style.flexDirection = FlexDirection.Row;
            _characterStatStrip.style.display = DisplayStyle.None;
            _characterStatStrip.style.marginTop = 2;
            _characterStatStrip.style.marginBottom = 8;
            _characterStatStrip.Add(InventoryBadge("Map01A Character HP Badge", "HP 60/100", new Color(.95f, .70f, .70f, .96f)));
            _characterStatStrip.Add(InventoryBadge("Map01A Character MP Badge", "MP 50/100", new Color(.66f, .84f, 1f, .96f)));
            _characterStatStrip.Add(InventoryBadge("Map01A Character Gear Badge", "10/10 Lv1", new Color(.76f, 1f, .70f, .94f)));
            _inventoryHeroPanel.Add(_characterStatStrip);

            var heroDivider = LgoDivider("Map01A Character Hero Divider");
            heroDivider.style.display = DisplayStyle.None;
            _inventoryHeroPanel.Add(heroDivider);
            _equipmentTitle = LgoLabel("", 16, new Color(.95f, .86f, .58f, .96f), true);
            _equipmentTitle.name = "Map01A Character Equipment Summary";
            _equipmentTitle.style.display = DisplayStyle.None;
            _inventoryHeroPanel.Add(_equipmentTitle);

            _characterLoadoutMatrix = new VisualElement { name = "Map01A Character Loadout Matrix" };
            var equipmentSlots = _characterLoadoutMatrix;
            equipmentSlots.style.flexGrow = 1;
            equipmentSlots.style.minHeight = 0;
            equipmentSlots.style.flexDirection = FlexDirection.Row;
            equipmentSlots.style.flexWrap = Wrap.Wrap;
            equipmentSlots.style.marginTop = 8;
            var equipmentScroll = new ScrollView(ScrollViewMode.Vertical)
            {
                name = "Map01A Character Equipment Scroll",
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            RuntimeUiOverflowGuard.ApplyBoundedScroll(equipmentScroll, 900);
            equipmentScroll.style.flexGrow = 1;
            equipmentScroll.style.minHeight = 0;
            equipmentScroll.contentViewport.RegisterCallback<GeometryChangedEvent>(evt =>
                equipmentScroll.contentContainer.style.width = evt.newRect.width);
            equipmentScroll.Add(equipmentSlots);
            equipmentScroll.style.display = DisplayStyle.None;
            _inventoryHeroPanel.Add(equipmentScroll);
            _equipmentRows = new Button[_equipmentSlotIds.Count];
            _equipmentRowIcons = new VisualElement[_equipmentSlotIds.Count];
            _equipmentRowNames = new Label[_equipmentSlotIds.Count];
            _equipmentRowStates = new Label[_equipmentSlotIds.Count];
            for (var i = 0; i < _equipmentSlotIds.Count; i++)
            {
                var slotId = _equipmentSlotIds[i];
                var row = InventoryButton(() => SelectInventoryEquipmentSlot(slotId), "LGO Equipment Inventory Slot " + slotId);
                ApplyLgoInventoryItemRow(row, _touch);
                row.style.flexBasis = new Length(48, LengthUnit.Percent);
                row.style.height = 58;
                row.style.marginBottom = 6;
                row.style.marginRight = i % 2 == 0 ? 7 : 0;

                var icon = new VisualElement { name = "Map01A Character Info Slot Icon " + slotId };
                ApplyLgoItemIcon(icon);
                icon.style.width = 34;
                icon.style.height = 34;
                icon.style.marginLeft = 2;
                icon.style.marginRight = 7;
                row.Add(icon);

                var textGroup = new VisualElement { name = "Map01A Character Info Slot Text " + slotId };
                textGroup.style.flexGrow = 1;
                textGroup.style.minWidth = 0;
                textGroup.style.flexDirection = FlexDirection.Column;
                var nameLabel = LgoLabel("", 12, UiText, true);
                nameLabel.name = "Map01A Character Info Slot Name " + slotId;
                var stateLabel = LgoLabel("", 12, UiSubText);
                stateLabel.name = "Map01A Character Info Slot State " + slotId;
                textGroup.Add(nameLabel);
                textGroup.Add(stateLabel);
                row.Add(textGroup);

                _equipmentRows[i] = row;
                _equipmentRowIcons[i] = icon;
                _equipmentRowNames[i] = nameLabel;
                _equipmentRowStates[i] = stateLabel;
                equipmentSlots.Add(row);
            }

            var identity = InventoryRow("Map01A Inventory Identity");
            identity.style.marginTop = 4;
            _inventoryGender = InventoryButton(() => _scene.CycleVoAvatarGender(), "Map01A Inventory Gender");
            identity.Add(_inventoryGender); _inventoryHeroPanel.Add(identity);

            _inventoryItemsGrid = new VisualElement { name = "Map01A Inventory Items Grid" };
            _inventoryItemsGrid.style.flexDirection = FlexDirection.Row;
            _inventoryItemsGrid.style.flexWrap = Wrap.Wrap;
            _inventoryItemsGrid.style.flexShrink = 0;
            scroll.Add(_inventoryItemsGrid);
            _equipmentTiles = new Button[_equipmentSlotIds.Count];
            _equipmentTileIcons = new VisualElement[_equipmentSlotIds.Count];
            _equipmentTileNames = new Label[_equipmentSlotIds.Count];
            _equipmentTileStates = new Label[_equipmentSlotIds.Count];
            for (var i = 0; i < _equipmentSlotIds.Count; i++)
            {
                var slotId = _equipmentSlotIds[i];
                var tile = InventoryButton(() => SelectInventoryEquipmentSlot(slotId), "Map01A Equipment Item Tile " + slotId);
                ApplyLgoInventoryBagGridCell(tile);
                tile.style.fontSize = 12;
                tile.style.flexDirection = FlexDirection.Column;
                tile.style.unityTextAlign = TextAnchor.MiddleCenter;

                var icon = new VisualElement { name = "Map01A Equipment Item Icon " + slotId };
                ApplyLgoItemIcon(icon);
                icon.style.width = 76;
                icon.style.height = 76;
                icon.style.marginTop = 2;
                icon.style.marginBottom = 4;
                icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                icon.style.marginLeft = 0;
                icon.style.marginRight = 0;
                tile.Add(icon);

                var textGroup = new VisualElement { name = "Map01A Equipment Item Text " + slotId };
                textGroup.style.flexGrow = 0;
                textGroup.style.minWidth = 0;
                textGroup.style.alignItems = Align.Center;
                textGroup.style.flexDirection = FlexDirection.Column;
                var nameLabel = LgoLabel("", 12, UiText, true);
                nameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                nameLabel.name = "Map01A Equipment Item Name " + slotId;
                nameLabel.style.display = DisplayStyle.None;
                var stateLabel = LgoLabel("", 9, UiSubText);
                stateLabel.name = "Map01A Equipment Item State " + slotId;
                stateLabel.style.marginTop = 1;
                stateLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                stateLabel.style.display = DisplayStyle.None;
                textGroup.Add(nameLabel);
                textGroup.Add(stateLabel);
                tile.Add(textGroup);

                _equipmentTiles[i] = tile;
                _equipmentTileIcons[i] = icon;
                _equipmentTileNames[i] = nameLabel;
                _equipmentTileStates[i] = stateLabel;
                _inventoryItemsGrid.Add(tile);
            }
            _questItemActions = _inventoryItemsGrid;
            _healthPotion = SupplyItemRow(() => SelectInventorySupply("health_potion"), "Map01A Health Potion", "health_potion", out _healthPotionName, out _healthPotionCount, out _healthPotionState);
            _manaPotion = SupplyItemRow(() => SelectInventorySupply("mana_potion"), "Map01A Mana Potion", "mana_potion", out _manaPotionName, out _manaPotionCount, out _manaPotionState);
            _equipReward = SupplyItemRow(() => SelectInventorySupply("class_reward"), "Map01A Equip Reward", "class_reward", out _classRewardName, out _classRewardCount, out _classRewardState);
            _inventoryItemsGrid.Add(_healthPotion);
            _inventoryItemsGrid.Add(_manaPotion);
            _inventoryItemsGrid.Add(_equipReward);
            _emptyBagSlots = new VisualElement[7];
            for (var emptyIndex = 0; emptyIndex < _emptyBagSlots.Length; emptyIndex++)
            {
                var emptySlot = new VisualElement { name = $"Map01A Empty Bag Slot {emptyIndex + 1:00}" };
                ApplyLgoInventoryBagGridCell(emptySlot);
                ApplyLgoFrame(emptySlot, new Color(.020f, .060f, .088f, .58f), new Color(.50f, .58f, .58f, .32f));
                var emptyMark = LgoLabel("·", 10, new Color(.48f, .58f, .62f, .34f));
                emptyMark.style.unityTextAlign = TextAnchor.MiddleCenter;
                emptySlot.Add(emptyMark);
                _emptyBagSlots[emptyIndex] = emptySlot;
                _inventoryItemsGrid.Add(emptySlot);
            }
            _inventorySummary = LgoLabel("", 12, UiSubText);
            _inventorySummary.style.display = DisplayStyle.None;
            _inventoryItemsGrid.Add(_inventorySummary);
            _suppliesEmptyState = LgoLabel("", 15, new Color(.70f, .80f, .80f, .92f));
            _suppliesEmptyState.name = "Map01A Supplies Empty State";
            _suppliesEmptyState.style.display = DisplayStyle.None;
            _inventoryItemsGrid.Add(_suppliesEmptyState);

            InitializeCharacterHub(body);
            body.Add(_inventoryDetailPanel);

            ShowInventoryCategory("all"); ShowInventoryMode(false); _safe.Add(_inventoryBackdrop); _safe.Add(_inventory);
        }


        public void OpenInventoryReviewMode(string mode)
        {
            if (!_scene.InventoryOpen) _scene.ToggleInventory();
            if (mode == "character-info") ShowInventoryMode(true);
            else if (mode == "skills") ShowCharacterHubPreviewMode(CharacterHubMode.Skills);
            else if (mode == "potential") ShowCharacterHubPreviewMode(CharacterHubMode.Potential);
            else if (mode == "spirit-pet") ShowCharacterHubPreviewMode(CharacterHubMode.SpiritPet);
            else if (mode == "supplies") { ShowInventoryMode(false); ShowInventoryCategory("items"); }
            else ShowInventoryMode(false);
            Update();
        }

        private void SelectInventoryEquipmentSlot(string slotId)
        {
            _scene.SelectVoEquipmentSlot(slotId);
            _suppliesOpen = false;
            RefreshInventoryEquipmentTiles();
            RefreshInventoryDetailCard();
        }

        private void SelectInventorySupply(string itemId)
        {
            _selectedSupplyItemId = itemId;
            _suppliesOpen = true;
            RefreshInventoryEquipmentTiles();
            RefreshInventorySupplyRows();
            RefreshInventoryDetailCard();
        }

        private void UseInventoryDetailPrimaryAction()
        {
            if (_suppliesOpen)
            {
                if (_selectedSupplyItemId == "health_potion") _scene.UseHealthPotion();
                else if (_selectedSupplyItemId == "mana_potion") _scene.UseManaPotion();
                else if (_selectedSupplyItemId == "class_reward") _scene.EquipClassReward();
            }
            else _scene.ToggleVoEquipmentSlot();
            RefreshInventoryEquipmentTiles();
            RefreshInventoryDetailCard();
        }

        private void ToggleSelectedEquipmentLock()
        {
            if (_scene == null || _suppliesOpen) return;
            var itemId = _scene.GetVoEquipmentItemId(_scene.VoSelectedEquipmentSlot);
            if (string.IsNullOrEmpty(itemId)) return;
            if (!_lockedEquipmentItemIds.Remove(itemId)) _lockedEquipmentItemIds.Add(itemId);
            RefreshInventoryDetailCard();
        }

        private void ShowInventoryMode(bool characterInfo)
        {
            _activeCharacterHubPreviewMode = null;
            HideCharacterHubPreviewPanels();
            ShowInventoryCategory(characterInfo ? "equipment" : "all");
            _inventoryHeroPanel.style.flexGrow = 0;
            _inventoryHeroPanel.style.flexBasis = InventoryDesktopMainColumnWidth;
            _inventoryGridPanel.style.flexGrow = 0;
            _inventoryGridPanel.style.flexBasis = characterInfo ? 0 : InventoryDesktopMainColumnWidth;
            _characterInfoOpen = characterInfo;
            RefreshInventoryShellMode();
            RefreshInventoryModalHeader();
            RefreshInventoryEquipmentTiles();
            _inventoryGridPanel.style.display = characterInfo ? DisplayStyle.None : DisplayStyle.Flex;
            _inventoryHeroPanel.style.display = characterInfo ? DisplayStyle.Flex : DisplayStyle.None;
            _inventoryDetailPanel.style.display = DisplayStyle.Flex;
            _inventoryFooter.style.display = DisplayStyle.Flex;
            _inventoryDetailSellAction.style.display = characterInfo ? DisplayStyle.None : DisplayStyle.Flex;
            ApplyHubMainTabSelection(null, characterInfo, !characterInfo);
            AnimateLgoCharacterHubSwap(characterInfo ? _inventoryHeroPanel : _inventoryGridPanel);
            AnimateLgoCharacterHubSwap(_inventoryDetailPanel);
            RefreshInventoryDetailCard();
        }

        private void UpdateCharacterHubOpenAnimation(bool isOpen)
        {
            if (isOpen && !_inventoryWasOpen) AnimateLgoCharacterHubOpen(_inventory, _inventoryBackdrop);
            _inventoryWasOpen = isOpen;
        }

        private void RefreshInventoryModalHeader()
        {
            if (_inventoryModalTitle == null || _inventoryModalSubtitle == null) return;
            if (_characterInfoOpen)
            {
                _inventoryModalTitle.text = "THÔNG TIN NHÂN VẬT";
                _inventoryModalSubtitle.text = "Trang bị hiện tại và thông tin chiến đấu của LụcThiên";
            }
            else
            {
                _inventoryModalTitle.text = "HÀNH TRANG";
                _inventoryModalSubtitle.text = "Vật phẩm đang mang theo, phân loại và thông tin chi tiết";
            }
        }

        private void ShowInventoryPage(bool supplies)
        {
            ShowInventoryCategory(supplies ? "items" : "equipment");
        }

        private void ShowInventoryCategory(string category)
        {
            _inventoryCategory = category;
            _suppliesOpen = category == "items";
            RefreshInventoryEquipmentTiles();
            RefreshInventoryVisibility();
            _inventoryFooter.style.display = DisplayStyle.Flex;
            _inventoryDetailPanel.style.display = DisplayStyle.Flex;
            ApplyLgoSelectedTab(_allItemsTab, category == "all");
            ApplyLgoSelectedTab(_equipmentTab, category == "equipment");
            ApplyLgoSelectedTab(_suppliesTab, category == "items");
            RefreshInventoryDetailCard();
        }

        private static string NormalizeInventorySearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(decomposed.Length);
            foreach (var character in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark) continue;
                builder.Append(char.ToLowerInvariant(character));
            }
            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        private bool InventoryMatchesSearch(params string[] values)
        {
            if (string.IsNullOrEmpty(_inventorySearchQuery)) return true;
            foreach (var value in values)
            {
                if (NormalizeInventorySearch(value).IndexOf(_inventorySearchQuery, StringComparison.Ordinal) >= 0)
                    return true;
            }
            return false;
        }

        private void RefreshInventoryVisibility()
        {
            if (_equipmentTiles == null || _emptyBagSlots == null) return;
            var showEquipment = _inventoryCategory == "all" || _inventoryCategory == "equipment";
            var showSupplies = _inventoryCategory == "all" || _inventoryCategory == "items";
            var visibleItemCount = 0;

            for (var index = 0; index < _equipmentTiles.Length; index++)
            {
                var slotId = _equipmentSlotIds[index];
                var visible = showEquipment && InventoryMatchesSearch(
                    EquipmentDisplayName(slotId), EquipmentShortName(slotId), slotId, _scene.GetVoEquipmentItemId(slotId));
                _equipmentTiles[index].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
                if (visible) visibleItemCount++;
            }

            var healthVisible = showSupplies && InventoryMatchesSearch("Bình Máu Nhỏ", "health_potion", "hồi phục HP");
            var manaVisible = showSupplies && InventoryMatchesSearch("Bình Linh Lực Nhỏ", "mana_potion", "hồi phục MP");
            var rewardVisible = showSupplies && InventoryMatchesSearch("Hộ Uyển Võ Tân Thủ", "class_reward", "phần thưởng nhiệm vụ");
            _healthPotion.style.display = healthVisible ? DisplayStyle.Flex : DisplayStyle.None;
            _manaPotion.style.display = manaVisible ? DisplayStyle.Flex : DisplayStyle.None;
            _equipReward.style.display = rewardVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (healthVisible) visibleItemCount++;
            if (manaVisible) visibleItemCount++;
            if (rewardVisible) visibleItemCount++;

            var showEmptySlots = _inventoryCategory == "all" && string.IsNullOrEmpty(_inventorySearchQuery);
            foreach (var emptySlot in _emptyBagSlots)
                emptySlot.style.display = showEmptySlots ? DisplayStyle.Flex : DisplayStyle.None;

            if (_suppliesEmptyState != null)
            {
                _suppliesEmptyState.text = string.IsNullOrEmpty(_inventorySearchQuery)
                    ? "Chưa có vật phẩm trong nhóm này."
                    : "Không tìm thấy vật phẩm phù hợp.";
                _suppliesEmptyState.style.display = visibleItemCount == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            }
            if (_inventoryCountBadge != null)
            {
                _inventoryCountBadge.text = string.IsNullOrEmpty(_inventorySearchQuery)
                    ? "56/120 ô"
                    : visibleItemCount + " kết quả · 56/120 ô";
            }
        }

        private void RefreshInventorySupplyRows()
        {
            if (_healthPotion == null || _manaPotion == null || _equipReward == null) return;
            RefreshSupplyRow(_healthPotion, _healthPotionName, _healthPotionCount, _healthPotionState,
                "health_potion", "Bình Máu Nhỏ", _scene.HealthPotionCount, _scene.HealthPotionCount > 0 && _scene.PlayerHealth < 100 ? "Sẵn sàng" : "Thiếu điều kiện");
            RefreshSupplyRow(_manaPotion, _manaPotionName, _manaPotionCount, _manaPotionState,
                "mana_potion", "Bình Linh Lực Nhỏ", _scene.ManaPotionCount, _scene.ManaPotionCount > 0 && _scene.PlayerMana < 100 ? "Sẵn sàng" : "Thiếu điều kiện");
            RefreshSupplyRow(_equipReward, _classRewardName, _classRewardCount, _classRewardState,
                "class_reward", "Hộ Uyển Võ Tân Thủ", _scene.HasClassRewardItem ? 1 : 0,
                !_scene.HasClassRewardItem ? "Chưa nhận" : _scene.IsClassRewardEquipped ? "Đã trang bị" : "Có thể trang bị");
        }

        private void RefreshSupplyRow(Button button, Label nameLabel, Label countLabel, Label stateLabel, string itemId, string label, int count, string state)
        {
            var selected = _suppliesOpen && _selectedSupplyItemId == itemId;
            button.text = string.Empty;
            if (nameLabel != null) nameLabel.text = label;
            if (countLabel != null) countLabel.text = "x" + count;
            if (stateLabel != null) stateLabel.text = state;
            button.tooltip = label + " · x" + count + " · " + state;
            ApplyLgoCharacterHubSelectionState(button, selected);
            button.style.color = new Color(.70f, .80f, .80f, .92f);
            if (stateLabel != null) stateLabel.style.color = selected ? new Color(.90f, .96f, 1f, .96f) : UiSubText;
        }

        private void RefreshInventoryEquipmentTiles()
        {
            if (_scene == null || _equipmentRows == null || _equipmentTiles == null) return;
            for (var index = 0; index < _equipmentRows.Length; index++)
            {
                var slotId = _equipmentSlotIds[index];
                var equipped = _scene.IsVoEquipmentSlotEquipped(slotId);
                var level = _scene.GetVoEquipmentItemLevel(slotId);
                _equipmentRows[index].text = string.Empty;
                _equipmentRows[index].style.backgroundColor = slotId == _scene.VoSelectedEquipmentSlot
                    ? new Color(.16f, .48f, .50f, .96f)
                    : equipped ? new Color(.06f, .13f, .17f, .94f) : new Color(.035f, .055f, .065f, .82f);
                var thumbnail = _scene.GetVoEquipmentThumbnailSprite(slotId);
                if (_equipmentRowNames != null && index < _equipmentRowNames.Length)
                    _equipmentRowNames[index].text = (equipped ? "✓ " : "○ ") + EquipmentShortName(slotId);
                if (_equipmentRowStates != null && index < _equipmentRowStates.Length)
                    _equipmentRowStates[index].text = "Lv" + level;
                if (_equipmentRowIcons != null && index < _equipmentRowIcons.Length)
                {
                    _equipmentRowIcons[index].style.backgroundImage = thumbnail == null ? StyleKeyword.None : new StyleBackground(thumbnail);
                    _equipmentRowIcons[index].style.display = thumbnail == null ? DisplayStyle.None : DisplayStyle.Flex;
                }
                _equipmentTiles[index].text = string.Empty;
                _equipmentTiles[index].tooltip = EquipmentDisplayName(slotId) + " · Lv" + level
                    + " · " + (equipped ? "Đang mặc" : "Đã tháo");
                ApplyLgoCharacterHubSelectionState(_equipmentTiles[index], slotId == _scene.VoSelectedEquipmentSlot);
                if (_equipmentTileNames != null && index < _equipmentTileNames.Length)
                    _equipmentTileNames[index].text = EquipmentShortName(slotId) + " · Lv" + level;
                if (_equipmentTileStates != null && index < _equipmentTileStates.Length)
                    _equipmentTileStates[index].text = equipped ? "Đang mặc" : "Đã tháo";
                if (_equipmentTileIcons != null && index < _equipmentTileIcons.Length)
                {
                    _equipmentTileIcons[index].style.backgroundImage = thumbnail == null ? StyleKeyword.None : new StyleBackground(thumbnail);
                    _equipmentTileIcons[index].style.display = thumbnail == null ? DisplayStyle.None : DisplayStyle.Flex;
                }
            }
            RefreshCharacterHeroCard();
        }

        private void RefreshCharacterHeroCard()
        {
            if (_scene == null || _characterHeroCard == null) return;
            if (_characterHeroName != null) _characterHeroName.text = "LụcThiên";
            if (_characterHeroPower != null) _characterHeroPower.text = "Lv." + _scene.VoAvatarLevel + "  ·  LC 245.780";
            if (_characterHeroHealth != null)
            {
                _characterHeroHealth.value = _scene.PlayerHealth;
                _characterHeroHealth.title = "HP " + _scene.PlayerHealth + "/100";
            }
            if (_characterHeroMana != null)
            {
                _characterHeroMana.value = _scene.PlayerMana;
                _characterHeroMana.title = "MP " + _scene.PlayerMana + "/100";
            }
            if (_characterHeroLoadout != null) _characterHeroLoadout.text = "Trang bị " + _scene.VoEquippedSlotCount + "/10 · Lv" + _scene.VoAvatarLevel;
            if (_inventoryCountBadge != null && string.IsNullOrEmpty(_inventorySearchQuery))
                _inventoryCountBadge.text = "56/120 ô";

            var portraitTexture = _scene.GetCharacterHubAvatarPreviewTexture();
            if (_characterHeroPortrait != null)
            {
                if (portraitTexture != null) _characterHeroPortrait.style.backgroundImage = new StyleBackground(portraitTexture);
                else _characterHeroPortrait.style.backgroundImage = StyleKeyword.None;
                // Keep the equipment rails anchored around a fixed stage even while a
                // class portrait is loading or unavailable. Collapsing this element
                // pulls all ten equipment slots into the middle of the character tab.
                _characterHeroPortrait.style.display = DisplayStyle.Flex;
            }
            if (_characterHeroQuickIcons == null || _equipmentSlotIds == null) return;
            for (var i = 0; i < _characterHeroQuickIcons.Length; i++)
            {
                var slot = i < _equipmentSlotIds.Count ? _equipmentSlotIds[i] : null;
                var sprite = string.IsNullOrEmpty(slot) ? null : _scene.GetVoEquipmentThumbnailSprite(slot);
                _characterHeroQuickIcons[i].style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
                _characterHeroQuickIcons[i].style.display = sprite == null ? DisplayStyle.None : DisplayStyle.Flex;
                if (_characterHeroQuickLevels != null && i < _characterHeroQuickLevels.Length)
                    _characterHeroQuickLevels[i].text = "+" + _scene.GetVoEquipmentItemLevel(slot);
            }
        }

        private void RefreshInventoryDetailChips(string level, string fit)
        {
            if (_inventoryDetailLevelChip != null) _inventoryDetailLevelChip.text = level;
            if (_inventoryDetailFitChip != null) _inventoryDetailFitChip.text = fit;
        }

        private void RefreshInventoryDetailCard()
        {
            if (_scene == null || _inventoryDetailStateBadge == null) return;
            if (_suppliesOpen)
            {
                RefreshInventorySupplyDetailCard();
                return;
            }
            _inventoryDetailHeader.text = "CHI TIẾT MÓN";
            _inventoryDetailHeader.style.display = DisplayStyle.None;
            if (_inventoryDetailPrimaryAction != null) _inventoryDetailPrimaryAction.SetEnabled(true);
            var selectedSlot = _scene.VoSelectedEquipmentSlot;
            var selectedEquipped = _scene.IsVoEquipmentSlotEquipped(selectedSlot);
            var selectedLevel = _scene.GetVoEquipmentItemLevel(selectedSlot);
            var selectedName = EquipmentDisplayName(selectedSlot);
            var selectedItemId = _scene.GetVoEquipmentItemId(selectedSlot);
            var selectedLocked = _lockedEquipmentItemIds.Contains(selectedItemId);
            if (_inventoryDetailLockAction != null)
            {
                _inventoryDetailLockAction.style.display = DisplayStyle.Flex;
                _inventoryDetailLockAction.text = selectedLocked ? "Mở khóa" : "Khóa";
            }
            if (_inventoryDetailPrimaryAction != null) _inventoryDetailPrimaryAction.SetEnabled(!selectedLocked);
            var thumbnail = _scene.GetVoEquipmentThumbnailSprite(selectedSlot);
            _inventoryDetailIcon.text = "";
            _inventoryDetailIcon.style.backgroundImage = thumbnail == null ? StyleKeyword.None : new StyleBackground(thumbnail);
            _inventoryDetailIcon.style.display = thumbnail == null ? DisplayStyle.None : DisplayStyle.Flex;
            _equipmentDetail.text = selectedName;
            _inventoryDetailRarity.text = "Trang bị · Lv" + selectedLevel;
            _inventoryDetailSlotType.text = selectedName;
            _inventoryItemId.text = selectedItemId;
            _inventoryDetailStateBadge.text = selectedEquipped ? "ĐANG MẶC" : "ĐÃ THÁO";
            _inventoryItemState.text = selectedEquipped ? "Đang mặc trên nhân vật." : "Đã tháo khỏi nhân vật.";
            _inventoryItemState.style.display = _characterInfoOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _inventoryDetailStatsHeader.text = "THUỘC TÍNH";
            _inventoryDetailSetHeader.text = "BỘ TRANG BỊ HIỆN TẠI";
            _inventoryDetailStatPrimary.text = "Chưa có thuộc tính chiến đấu.";
            _inventoryDetailStatFit.text = _scene.VoEquippedSlotCount + "/10 món đang mặc\nDành cho " + _scene.ActiveEquipmentClassLabel + " · " + (_scene.VoAvatarGender == "female" ? "Nữ" : "Nam");
            RefreshInventoryDetailChips("Lv" + selectedLevel, _scene.ActiveEquipmentClassLabel + " · " + (_scene.VoAvatarGender == "female" ? "Nữ" : "Nam"));
            if (_inventoryDetailPrimaryAction != null)
                _inventoryDetailPrimaryAction.text = selectedEquipped ? "Tháo" : "Trang bị";
            if (_equipmentToggle != null)
                _equipmentToggle.text = selectedEquipped ? "Tháo món đang chọn" : "Mặc món đang chọn";
        }

        private void RefreshInventorySupplyDetailCard()
        {
            if (_inventoryDetailLockAction != null) _inventoryDetailLockAction.style.display = DisplayStyle.None;
            _inventoryDetailIcon.text = "";
            var itemSprite = _scene.GetMap01AItemThumbnailSprite(_selectedSupplyItemId);
            _inventoryDetailIcon.style.backgroundImage = itemSprite == null ? StyleKeyword.None : new StyleBackground(itemSprite);
            _inventoryDetailIcon.style.display = itemSprite == null ? DisplayStyle.None : DisplayStyle.Flex;
            _inventoryDetailHeader.text = "CHI TIẾT VẬT PHẨM";
            _inventoryDetailStatsHeader.text = "HIỆU QUẢ";
            _inventoryDetailSetHeader.text = "ĐIỀU KIỆN SỬ DỤNG";
            _inventoryDetailRarity.text = "Vật phẩm nhiệm vụ";
            if (_selectedSupplyItemId == "mana_potion")
            {
                _equipmentDetail.text = "Bình Linh Lực Nhỏ";
                _inventoryDetailSlotType.text = "Vật phẩm hồi phục";
                _inventoryItemId.text = "map01a_mana_potion_small";
                _inventoryDetailStateBadge.text = _scene.ManaPotionCount > 0 && _scene.PlayerMana < 100 ? "SẴN SÀNG" : "THIẾU ĐIỀU KIỆN";
                _inventoryItemState.text = "Số lượng: " + _scene.ManaPotionCount + " · MP " + _scene.PlayerMana + "/100.";
                _inventoryDetailStatPrimary.text = "Hồi MP +50";
                _inventoryDetailStatFit.text = "Dùng khi MP chưa đầy.";
                RefreshInventoryDetailChips("x" + _scene.ManaPotionCount, "Hồi phục MP");
                _inventoryDetailPrimaryAction.text = "Dùng bình linh lực";
                _inventoryDetailPrimaryAction.SetEnabled(_scene.ManaPotionCount > 0 && _scene.PlayerMana < 100);
            }
            else if (_selectedSupplyItemId == "class_reward")
            {
                _equipmentDetail.text = "Hộ Uyển Võ Tân Thủ";
                _inventoryDetailSlotType.text = "Trang bị nhiệm vụ";
                _inventoryItemId.text = "map01a_vo_wrist_guard_reward";
                _inventoryDetailStateBadge.text = !_scene.HasClassRewardItem ? "CHƯA NHẬN" : _scene.IsClassRewardEquipped ? "ĐÃ TRANG BỊ" : "CÓ THỂ TRANG BỊ";
                _inventoryItemState.text = !_scene.HasClassRewardItem ? "Hoàn thành nhánh Q07 để nhận." : _scene.IsClassRewardEquipped ? "Đã trang bị từ nhiệm vụ." : "Sẵn sàng trang bị từ chi tiết bên phải.";
                _inventoryDetailStatPrimary.text = "Phần thưởng nhiệm vụ";
                _inventoryDetailStatFit.text = "Nhận khi hoàn thành nhiệm vụ tân thủ.";
                RefreshInventoryDetailChips(_scene.HasClassRewardItem ? "x1" : "x0", "Nhiệm vụ");
                _inventoryDetailPrimaryAction.text = "Trang bị hộ uyển";
                _inventoryDetailPrimaryAction.SetEnabled(_scene.HasClassRewardItem && !_scene.IsClassRewardEquipped);
            }
            else
            {
                _selectedSupplyItemId = "health_potion";
                _equipmentDetail.text = "Bình Máu Nhỏ";
                _inventoryDetailSlotType.text = "Vật phẩm hồi phục";
                _inventoryItemId.text = "map01a_health_potion_small";
                _inventoryDetailStateBadge.text = _scene.HealthPotionCount > 0 && _scene.PlayerHealth < 100 ? "SẴN SÀNG" : "THIẾU ĐIỀU KIỆN";
                _inventoryItemState.text = "Số lượng: " + _scene.HealthPotionCount + " · HP " + _scene.PlayerHealth + "/100.";
                _inventoryDetailStatPrimary.text = "Hồi HP +50";
                _inventoryDetailStatFit.text = "Dùng khi HP chưa đầy.";
                RefreshInventoryDetailChips("x" + _scene.HealthPotionCount, "Hồi phục HP");
                _inventoryDetailPrimaryAction.text = "Dùng bình máu";
                _inventoryDetailPrimaryAction.SetEnabled(_scene.HealthPotionCount > 0 && _scene.PlayerHealth < 100);
            }
            if (_equipmentToggle != null) _equipmentToggle.style.display = DisplayStyle.None;
            if (_equipmentVariant != null) _equipmentVariant.style.display = DisplayStyle.None;
        }

    }
}
