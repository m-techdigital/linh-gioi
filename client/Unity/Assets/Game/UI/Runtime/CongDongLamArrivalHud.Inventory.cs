using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _equipmentPage, _suppliesPage, _storagePanel, _inventoryFooter, _inventoryHeroPanel, _inventoryGridPanel, _inventoryDetailPanel, _storageGateCard, _characterHeroCard, _characterHeroPortrait, _characterHeroLoadoutStrip, _characterStatStrip, _characterLoadoutMatrix, _inventoryBottomActions, _inventoryDetailStatsCard;
        private Label _inventoryModalTitle, _inventoryModalSubtitle, _inventoryHeroTitle, _inventoryHeroMeta, _characterHeroName, _characterHeroPower, _characterHeroVitals, _characterHeroLoadout, _inventoryCountBadge, _inventoryItemId, _inventoryItemState, _inventoryDetailHeader, _inventoryDetailIcon, _inventoryDetailRarity, _inventoryDetailSlotType, _inventoryDetailStateBadge, _inventoryDetailLevelChip, _inventoryDetailEquippedChip, _inventoryDetailFitChip, _inventoryDetailStatPrimary, _inventoryDetailStatFit, _suppliesTitle, _suppliesEmptyState, _storageGateTitle, _storageState, _storageGateSafety;
        private Button _bagTab, _characterInfoTab, _storageTab, _equipmentTab, _suppliesTab;
        private Button _inventoryDetailPrimaryAction;
        private Button[] _equipmentTiles;
        private VisualElement[] _equipmentTileIcons, _equipmentRowIcons, _characterHeroQuickIcons;
        private Label[] _equipmentTileNames, _equipmentTileStates, _equipmentRowNames, _equipmentRowStates;
        private Label _healthPotionName, _healthPotionCount, _healthPotionState, _manaPotionName, _manaPotionCount, _manaPotionState, _classRewardName, _classRewardCount, _classRewardState;
        private bool _characterInfoOpen, _suppliesOpen, _storageOpen;
        private string _selectedSupplyItemId = "health_potion";

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


        private Button SupplyItemRow(Action action, string name, string itemId, out Label nameLabel, out Label countLabel, out Label stateLabel)
        {
            var row = InventoryButton(action, name);
            ApplyLgoInventoryItemRow(row, _touch);

            var textGroup = new VisualElement { name = "Map01A Supply Item Text " + itemId };
            textGroup.style.flexGrow = 1;
            textGroup.style.minWidth = 0;
            textGroup.style.flexDirection = FlexDirection.Column;
            nameLabel = LgoLabel("", 15, UiText, true);
            nameLabel.name = "Map01A Supply Item Name " + itemId;
            stateLabel = LgoLabel("", 12, UiSubText);
            stateLabel.name = "Map01A Supply Item State " + itemId;
            stateLabel.style.marginTop = 2;
            textGroup.Add(nameLabel);
            textGroup.Add(stateLabel);

            countLabel = LgoLabel("", 14, UiGold, true);
            countLabel.name = "Map01A Supply Item Count " + itemId;
            ApplyLgoInventoryCountBadge(countLabel);

            row.Add(textGroup);
            row.Add(countLabel);
            return row;
        }

        private void BuildInventory()
        {
            _inventory = new VisualElement { name = "Map01A Inventory" };
            Box(_inventory); Place(_inventory, 72, 72, 86, 72);
            _inventory.style.flexDirection = FlexDirection.Column;
            _inventory.style.paddingLeft = _inventory.style.paddingRight = 12;
            _inventory.style.paddingTop = _inventory.style.paddingBottom = 12;
            ApplyLgoGlassPanel(_inventory);

            var header = InventoryRow("Map01A Inventory Header");
            header.style.alignItems = Align.Center;
            var titleGroup = new VisualElement();
            titleGroup.style.flexGrow = 1;
            _inventoryModalTitle = LgoLabel("HÀNH TRANG", 20, UiGold, true);
            _inventoryModalTitle.name = "Map01A Inventory Modal Title";
            _inventoryModalSubtitle = LgoLabel("Túi đồ và thông tin nhân vật dùng chung chi tiết món", 13, new Color(.73f, .85f, .88f, .88f));
            _inventoryModalSubtitle.name = "Map01A Inventory Modal Subtitle";
            titleGroup.Add(_inventoryModalTitle);
            titleGroup.Add(_inventoryModalSubtitle);
            var close = InventoryButton(() => { if (_scene.InventoryOpen) _scene.ToggleInventory(); },
                "LGO Inventory Close", "×");
            close.tooltip = "Đóng hành trang (I / Esc)";
            ApplyLgoModalCloseButton(close, _touch);
            header.Add(titleGroup); header.Add(close); _inventory.Add(header);

            var mainTabs = InventoryRow("Map01A Inventory Main Tabs");
            _bagTab = InventoryButton(() => ShowInventoryMode(false), "Map01A Bag Main Tab", "Hành trang");
            _characterInfoTab = InventoryButton(() => ShowInventoryMode(true), "Map01A Character Info Main Tab", "Thông tin");
            _storageTab = InventoryButton(ShowStorageMode, "Map01A Storage Main Tab", "Rương đồ");
            foreach (var tab in new[] { _bagTab, _characterInfoTab, _storageTab })
            {
                ApplyLgoInventoryMainTab(tab, _touch);
            }
            mainTabs.Add(_bagTab); mainTabs.Add(_characterInfoTab); mainTabs.Add(_storageTab); _inventory.Add(mainTabs);

            var body = new VisualElement { name = "Map01A Inventory Body" };
            body.style.flexDirection = FlexDirection.Row;
            body.style.flexGrow = 1;
            body.style.minHeight = 0;
            body.style.marginTop = 6;
            body.style.justifyContent = Justify.Center;
            _inventory.Add(body);

            _inventoryDetailPanel = InventoryPanel("Map01A Inventory Detail Panel");
            _inventoryDetailPanel.style.flexGrow = 0;
            _inventoryDetailPanel.style.flexBasis = 300;
            _inventoryDetailPanel.style.marginLeft = 10;
            ApplyLgoDetailCard(_inventoryDetailPanel);
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
            _inventoryFooter.Add(_inventoryDetailHeader);
            _inventoryDetailIcon = LgoLabel("", 42, UiGold, true);
            _inventoryDetailIcon.name = "Map01A Inventory Detail Icon";
            ApplyLgoItemIcon(_inventoryDetailIcon);
            _inventoryDetailIcon.style.width = 72;
            _inventoryDetailIcon.style.height = 72;
            _inventoryDetailIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            _inventoryDetailIcon.style.alignSelf = Align.Center;
            _inventoryFooter.Add(_inventoryDetailIcon);
            _equipmentDetail = LgoLabel("", 18, UiGold, true);
            _equipmentDetail.name = "Map01A Inventory Detail Item Name";
            _equipmentDetail.style.marginTop = 4;
            _inventoryFooter.Add(_equipmentDetail);
            _inventoryDetailRarity = LgoLabel("", 14, new Color(.74f, .92f, 1f, .94f), true);
            _inventoryDetailRarity.name = "Map01A Inventory Detail Rarity";
            _inventoryDetailRarity.style.marginTop = 2;
            _inventoryFooter.Add(_inventoryDetailRarity);
            _inventoryDetailSlotType = LgoLabel("", 14, new Color(.88f, .94f, .92f, .94f), true);
            _inventoryDetailSlotType.name = "Map01A Inventory Detail Slot Type";
            _inventoryDetailSlotType.style.marginTop = 4;
            _inventoryFooter.Add(_inventoryDetailSlotType);
            _inventoryFooter.Add(LgoDivider("Map01A Inventory Detail Divider"));
            _inventoryItemId = LgoLabel("", 14, new Color(.78f, .88f, .90f, .88f));
            _inventoryItemId.style.marginTop = 2;
            _inventoryItemId.style.display = DisplayStyle.None;
            _inventoryFooter.Add(_inventoryItemId);
            _inventoryDetailStateBadge = LgoLabel("", 13, new Color(.12f, .08f, .03f, 1f), true);
            _inventoryDetailStateBadge.name = "Map01A Inventory Detail State Badge";
            ApplyLgoInventoryStateBadge(_inventoryDetailStateBadge);
            _inventoryFooter.Add(_inventoryDetailStateBadge);
            _inventoryItemState = LgoLabel("", 13, new Color(.91f, .93f, .84f, .96f));
            _inventoryItemState.style.marginTop = 8;
            _inventoryFooter.Add(_inventoryItemState);

            _inventoryDetailStatsCard = new VisualElement { name = "Map01A Inventory Detail Stats Card" };
            ApplyLgoInventoryStatsCard(_inventoryDetailStatsCard);
            var detailChips = InventoryRow("Map01A Inventory Detail Chip Row");
            detailChips.style.marginBottom = 6;
            _inventoryDetailLevelChip = InventoryBadge("Map01A Inventory Detail Level Chip", "Lv1", new Color(.86f, .94f, .90f, .96f));
            _inventoryDetailEquippedChip = InventoryBadge("Map01A Inventory Detail Equipped Chip", "Đang mặc", new Color(.76f, 1f, .70f, .94f));
            _inventoryDetailFitChip = InventoryBadge("Map01A Inventory Detail Fit Chip", "Võ · Nam", UiSubText);
            detailChips.Add(_inventoryDetailLevelChip);
            detailChips.Add(_inventoryDetailEquippedChip);
            _inventoryDetailStatsCard.Add(detailChips);
            _inventoryDetailStatsCard.Add(_inventoryDetailFitChip);
            _inventoryDetailStatPrimary = LgoLabel("", 13, new Color(.76f, .92f, 1f, .96f), true);
            _inventoryDetailStatPrimary.name = "Map01A Inventory Detail Stat Primary";
            _inventoryDetailStatPrimary.style.marginTop = 8;
            _inventoryDetailStatsCard.Add(_inventoryDetailStatPrimary);
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
            _inventoryDetailPrimaryAction.style.minHeight = _touch ? 44 : 38;
            _equipmentToggle = InventoryButton(() => { _scene.ToggleVoEquipmentSlot(); RefreshInventoryEquipmentTiles(); RefreshInventoryDetailCard(); }, "LGO Equipment Inventory Toggle");
            _equipmentToggle.style.display = DisplayStyle.None;
            _equipmentVariant = InventoryButton(() => { _scene.CycleVoSelectedEquipmentItemLevel(); RefreshInventoryEquipmentTiles(); RefreshInventoryDetailCard(); }, "LGO Equipment Inventory Variant");
            _equipmentVariant.style.display = DisplayStyle.None;
            _equipmentVariant.style.minHeight = _touch ? 44 : 38;
            actions.Add(_inventoryDetailPrimaryAction); actions.Add(_equipmentToggle); actions.Add(_equipmentVariant);
            _inventoryDetailPanel.Add(actions);

            _inventoryGridPanel = InventoryPanel("Map01A Inventory Grid Panel");
            _inventoryGridPanel.style.flexGrow = 0;
            _inventoryGridPanel.style.flexBasis = 720;
            _inventoryGridPanel.style.marginRight = 8;
            body.Add(_inventoryGridPanel);
            var gridAccent = new VisualElement { name = "Map01A Inventory Grid Accent Rail" };
            ApplyLgoOrnamentRail(gridAccent);
            gridAccent.style.marginBottom = 10;
            _inventoryGridPanel.Add(gridAccent);
            var tabs = InventoryRow("Map01A Inventory Category Chips");
            tabs.style.alignItems = Align.Center;
            tabs.style.flexWrap = Wrap.Wrap;
            _equipmentTab = InventoryButton(() => ShowInventoryPage(false), "Map01A Equipment Tab", "Trang bị");
            _suppliesTab = InventoryButton(() => ShowInventoryPage(true), "Map01A Supplies Tab", "Vật phẩm");
            foreach (var categoryTab in new[] { _equipmentTab, _suppliesTab })
            {
                ApplyLgoInventoryFilterChip(categoryTab, _touch);
            }
            tabs.Add(_equipmentTab);
            tabs.Add(_suppliesTab);
            var consumableChip = InventoryBadge("Map01A Inventory Category Chip Consumable", "Tiêu hao", UiSubText);
            var materialChip = InventoryBadge("Map01A Inventory Category Chip Material", "Nguyên liệu", UiSubText);
            var otherChip = InventoryBadge("Map01A Inventory Category Chip Other", "Khác", UiSubText);
            foreach (var passiveChip in new[] { consumableChip, materialChip, otherChip })
            {
                passiveChip.style.opacity = .58f;
                passiveChip.style.marginBottom = 4;
            }
            tabs.Add(consumableChip);
            tabs.Add(materialChip);
            tabs.Add(otherChip);
            _inventoryGridPanel.Add(tabs);

            var gridStatus = InventoryRow("Map01A Inventory Grid Status");
            gridStatus.style.alignItems = Align.Center;
            gridStatus.style.marginTop = 2;
            gridStatus.style.marginBottom = 8;
            _inventoryCountBadge = InventoryBadge("Map01A Inventory Count Badge", "56/120 ô", new Color(.86f, .94f, .90f, .96f));
            gridStatus.Add(_inventoryCountBadge);
            var equippedBadge = InventoryBadge("Map01A Inventory Equipped Badge", "10/10 đang mặc", new Color(.76f, 1f, .70f, .94f));
            gridStatus.Add(equippedBadge);
            var gridStatusSpacer = new VisualElement();
            gridStatusSpacer.style.flexGrow = 1;
            gridStatus.Add(gridStatusSpacer);
            var sortBadge = InventoryBadge("Map01A Inventory Sort Badge", "Sắp xếp: mặc định", UiSubText);
            gridStatus.Add(sortBadge);
            _inventoryGridPanel.Add(gridStatus);

            var scroll = new ScrollView(ScrollViewMode.Vertical)
            {
                name = "LGO Inventory Scroll",
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            RuntimeUiOverflowGuard.ApplyBoundedScroll(scroll, 900);
            scroll.style.flexGrow = 0;
            scroll.style.flexShrink = 0;
            scroll.style.maxHeight = 244;
            scroll.style.minHeight = 0;
            scroll.contentViewport.RegisterCallback<GeometryChangedEvent>(evt =>
                scroll.contentContainer.style.width = evt.newRect.width);
            _inventoryGridPanel.Add(scroll);

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
            _inventoryBottomActions.Add(sortButton);
            _inventoryBottomActions.Add(splitButton);
            _inventoryBottomActions.Add(quickSellButton);
            _inventoryGridPanel.Add(_inventoryBottomActions);

            _inventoryHeroPanel = InventoryPanel("Map01A Inventory Character Panel");
            _inventoryHeroPanel.style.flexGrow = 0;
            _inventoryHeroPanel.style.flexBasis = 720;
            body.Add(_inventoryHeroPanel);
            _inventoryHeroTitle = LgoLabel("", 22, UiGold, true);
            _inventoryHeroMeta = LgoLabel("", 15, new Color(.78f, .94f, .96f, .92f));
            _inventoryHeroPanel.Add(_inventoryHeroTitle);
            _inventoryHeroPanel.Add(_inventoryHeroMeta);

            _characterHeroCard = new VisualElement { name = "Map01A Character Hero Card" };
            _characterHeroCard.style.flexDirection = FlexDirection.Row;
            _characterHeroCard.style.alignItems = Align.Center;
            _characterHeroCard.style.marginTop = 10;
            _characterHeroCard.style.marginBottom = 8;
            ApplyLgoDetailCard(_characterHeroCard, 12, 10);
            _inventoryHeroPanel.Add(_characterHeroCard);

            _characterHeroPortrait = new VisualElement { name = "Map01A Character Hero Portrait" };
            ApplyLgoItemIcon(_characterHeroPortrait);
            _characterHeroPortrait.style.width = 86;
            _characterHeroPortrait.style.height = 110;
            _characterHeroPortrait.style.marginRight = 12;
            _characterHeroPortrait.style.marginTop = 0;
            _characterHeroPortrait.style.marginBottom = 0;
            _characterHeroCard.Add(_characterHeroPortrait);

            var heroInfo = new VisualElement { name = "Map01A Character Hero Info" };
            heroInfo.style.flexGrow = 1;
            heroInfo.style.minWidth = 0;
            heroInfo.style.flexDirection = FlexDirection.Column;
            _characterHeroName = LgoLabel("", 19, UiGold, true);
            _characterHeroName.name = "Map01A Character Hero Name";
            _characterHeroPower = LgoLabel("", 17, new Color(.96f, .91f, .76f, .96f), true);
            _characterHeroPower.name = "Map01A Character Hero Power";
            _characterHeroVitals = LgoLabel("", 13, UiSubText);
            _characterHeroVitals.name = "Map01A Character Hero Vitals";
            _characterHeroLoadout = LgoLabel("", 13, new Color(.76f, 1f, .70f, .94f), true);
            _characterHeroLoadout.name = "Map01A Character Hero Loadout";
            _characterHeroLoadoutStrip = new VisualElement { name = "Map01A Character Hero Loadout Strip" };
            _characterHeroLoadoutStrip.style.flexDirection = FlexDirection.Row;
            _characterHeroLoadoutStrip.style.marginTop = 8;
            _characterHeroQuickIcons = new VisualElement[4];
            for (var heroIconIndex = 0; heroIconIndex < _characterHeroQuickIcons.Length; heroIconIndex++)
            {
                var quickIcon = new VisualElement { name = "Map01A Character Hero Quick Icon " + heroIconIndex };
                ApplyLgoItemIcon(quickIcon);
                quickIcon.style.width = 34;
                quickIcon.style.height = 34;
                quickIcon.style.marginTop = 0;
                quickIcon.style.marginBottom = 0;
                quickIcon.style.marginRight = 6;
                _characterHeroQuickIcons[heroIconIndex] = quickIcon;
                _characterHeroLoadoutStrip.Add(quickIcon);
            }
            heroInfo.Add(_characterHeroName);
            heroInfo.Add(_characterHeroPower);
            heroInfo.Add(_characterHeroVitals);
            heroInfo.Add(_characterHeroLoadout);
            heroInfo.Add(_characterHeroLoadoutStrip);
            _characterHeroCard.Add(heroInfo);

            _characterStatStrip = new VisualElement { name = "Map01A Character Stat Strip" };
            _characterStatStrip.style.flexDirection = FlexDirection.Row;
            _characterStatStrip.style.marginTop = 2;
            _characterStatStrip.style.marginBottom = 8;
            _characterStatStrip.Add(InventoryBadge("Map01A Character HP Badge", "HP 60/100", new Color(.95f, .70f, .70f, .96f)));
            _characterStatStrip.Add(InventoryBadge("Map01A Character MP Badge", "MP 50/100", new Color(.66f, .84f, 1f, .96f)));
            _characterStatStrip.Add(InventoryBadge("Map01A Character Gear Badge", "10/10 Lv1", new Color(.76f, 1f, .70f, .94f)));
            _inventoryHeroPanel.Add(_characterStatStrip);

            var heroDivider = LgoDivider("Map01A Character Hero Divider");
            _inventoryHeroPanel.Add(heroDivider);
            _equipmentTitle = LgoLabel("", 16, new Color(.95f, .86f, .58f, .96f), true);
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
            _inventoryHeroPanel.Add(equipmentScroll);
            _equipmentSlotIds = _scene.VoEquipmentSlotIds;
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
            _equipmentClass = InventoryButton(() => _scene.CycleSourcePoseClass(), "LGO Equipment Inventory Class");
            _inventoryGender = InventoryButton(() => _scene.CycleVoAvatarGender(), "Map01A Inventory Gender");
            identity.Add(_equipmentClass); identity.Add(_inventoryGender); _inventoryHeroPanel.Add(identity);

            _storagePanel = InventoryPanel("Map01A Storage Panel");
            _storagePanel.style.flexGrow = 1;
            _storagePanel.style.marginRight = 0;
            body.Add(_storagePanel);
            _storageGateCard = InventoryPanel("Map01A Storage Gate Card");
            _storageGateCard.style.flexGrow = 0;
            _storageGateCard.style.marginTop = 8;
            _storageGateCard.style.marginBottom = 12;
            _storageGateCard.style.paddingLeft = _storageGateCard.style.paddingRight = 18;
            _storageGateCard.style.paddingTop = _storageGateCard.style.paddingBottom = 16;
            _storagePanel.Add(_storageGateCard);
            _storageGateTitle = LgoLabel("Kho gửi/rút chưa mở", 22, UiGold, true);
            _storageGateTitle.name = "Map01A Storage Gate Title";
            _storageGateCard.Add(_storageGateTitle);
            _storageState = LgoLabel("Rương đồ chưa khả dụng trong bản trải nghiệm này. Bạn vẫn có thể xem và sử dụng đồ trong Hành trang.", 16, new Color(.91f, .93f, .84f, .96f));
            _storageState.name = "Map01A Storage State";
            _storageState.style.marginTop = 10;
            _storageGateCard.Add(_storageState);
            _storageGateSafety = LgoLabel("Đồ trong Hành trang và trang bị đang mặc được giữ nguyên.", 15, new Color(.76f, 1f, .70f, .96f), true);
            _storageGateSafety.name = "Map01A Storage Gate Safety";
            _storageGateSafety.style.marginTop = 10;
            _storageGateCard.Add(_storageGateSafety);
            var storageActions = InventoryRow("Map01A Storage Actions");
            storageActions.style.marginTop = 16;
            var deposit = InventoryButton(() => { }, "Map01A Storage Deposit", "Gửi đồ");
            var withdraw = InventoryButton(() => { }, "Map01A Storage Withdraw", "Rút đồ");
            ApplyLgoDisabledAction(deposit);
            ApplyLgoDisabledAction(withdraw);
            storageActions.Add(deposit);
            storageActions.Add(withdraw);
            _storagePanel.Add(storageActions);

            _equipmentPage = new VisualElement { name = "Map01A Equipment Page" };
            _equipmentPage.style.flexDirection = FlexDirection.Row;
            _equipmentPage.style.flexWrap = Wrap.Wrap;
            _equipmentPage.style.flexShrink = 0;
            _equipmentTiles = new Button[_equipmentSlotIds.Count];
            _equipmentTileIcons = new VisualElement[_equipmentSlotIds.Count];
            _equipmentTileNames = new Label[_equipmentSlotIds.Count];
            _equipmentTileStates = new Label[_equipmentSlotIds.Count];
            for (var i = 0; i < _equipmentSlotIds.Count; i++)
            {
                var slotId = _equipmentSlotIds[i];
                var tile = InventoryButton(() => SelectInventoryEquipmentSlot(slotId), "Map01A Equipment Item Tile " + slotId);
                ApplyLgoInventoryGridCell(tile);
                tile.style.fontSize = 12;
                tile.style.flexDirection = FlexDirection.Column;
                tile.style.unityTextAlign = TextAnchor.MiddleCenter;

                var icon = new VisualElement { name = "Map01A Equipment Item Icon " + slotId };
                ApplyLgoItemIcon(icon);
                icon.style.width = 64;
                icon.style.height = 64;
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
                var stateLabel = LgoLabel("", 9, UiSubText);
                stateLabel.name = "Map01A Equipment Item State " + slotId;
                stateLabel.style.marginTop = 1;
                stateLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                textGroup.Add(nameLabel);
                textGroup.Add(stateLabel);
                tile.Add(textGroup);

                _equipmentTiles[i] = tile;
                _equipmentTileIcons[i] = icon;
                _equipmentTileNames[i] = nameLabel;
                _equipmentTileStates[i] = stateLabel;
                _equipmentPage.Add(tile);
            }
            for (var emptyIndex = 1; emptyIndex <= 4; emptyIndex++)
            {
                var emptySlot = new VisualElement { name = $"Map01A Empty Bag Slot {emptyIndex:00}" };
                ApplyLgoInventoryGridCell(emptySlot);
                ApplyLgoFrame(emptySlot, new Color(.020f, .060f, .088f, .58f), new Color(.50f, .58f, .58f, .32f));
                var emptyMark = LgoLabel("", 10, new Color(.48f, .58f, .62f, .34f));
                emptyMark.text = "·";
                emptyMark.style.unityTextAlign = TextAnchor.MiddleCenter;
                emptySlot.Add(emptyMark);
                _equipmentPage.Add(emptySlot);
            }
            scroll.Add(_equipmentPage);

            _suppliesPage = new VisualElement { name = "Map01A Supplies Page" };
            _suppliesPage.style.flexShrink = 0;
            var suppliesListCard = new VisualElement { name = "Map01A Supplies List Card" };
            suppliesListCard.style.paddingLeft = suppliesListCard.style.paddingRight = 14;
            suppliesListCard.style.paddingTop = suppliesListCard.style.paddingBottom = 12;
            suppliesListCard.style.marginTop = 2;
            suppliesListCard.style.marginBottom = 8;
            ApplyLgoDetailCard(suppliesListCard);
            _suppliesPage.Add(suppliesListCard);
            _suppliesTitle = LgoLabel("Vật phẩm", 20, UiGold, true);
            suppliesListCard.Add(_suppliesTitle);
            _inventorySummary = LgoLabel("", 15, new Color(.91f, .93f, .84f, .96f));
            _inventorySummary.style.marginTop = 8; _inventorySummary.style.marginBottom = 10;
            suppliesListCard.Add(_inventorySummary);
            _suppliesEmptyState = LgoLabel("", 15, new Color(.70f, .80f, .80f, .92f));
            _suppliesEmptyState.name = "Map01A Supplies Empty State";
            _suppliesEmptyState.style.marginTop = 2;
            _suppliesEmptyState.style.marginBottom = 12;
            ApplyLgoStatusCard(_suppliesEmptyState);
            suppliesListCard.Add(_suppliesEmptyState);
            _questItemActions = new VisualElement { name = "Map01A Quest Item Actions" };
            _healthPotion = SupplyItemRow(() => SelectInventorySupply("health_potion"), "Map01A Health Potion", "health_potion", out _healthPotionName, out _healthPotionCount, out _healthPotionState);
            _manaPotion = SupplyItemRow(() => SelectInventorySupply("mana_potion"), "Map01A Mana Potion", "mana_potion", out _manaPotionName, out _manaPotionCount, out _manaPotionState);
            _equipReward = SupplyItemRow(() => SelectInventorySupply("class_reward"), "Map01A Equip Reward", "class_reward", out _classRewardName, out _classRewardCount, out _classRewardState);
            _questItemActions.Add(_healthPotion);
            _questItemActions.Add(_manaPotion);
            _questItemActions.Add(_equipReward);
            suppliesListCard.Add(_questItemActions); scroll.Add(_suppliesPage);

            body.Add(_inventoryDetailPanel);

            ShowInventoryPage(false); ShowInventoryMode(false); _safe.Add(_inventory);
        }


        public void OpenInventoryReviewMode(string mode)
        {
            if (!_scene.InventoryOpen) _scene.ToggleInventory();
            if (mode == "character-info") ShowInventoryMode(true);
            else if (mode == "storage") ShowStorageMode();
            else if (mode == "supplies") { ShowInventoryMode(false); ShowInventoryPage(true); }
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

        private void ShowInventoryMode(bool characterInfo)
        {
            if (characterInfo) ShowInventoryPage(false);
            _inventoryHeroPanel.style.flexGrow = 0;
            _inventoryHeroPanel.style.flexBasis = characterInfo ? 720 : 360;
            _inventoryGridPanel.style.flexGrow = 0;
            _inventoryGridPanel.style.flexBasis = characterInfo ? 0 : 720;
            _storagePanel.style.flexGrow = 0;
            _characterInfoOpen = characterInfo;
            _storageOpen = false;
            RefreshInventoryModalHeader();
            RefreshInventoryEquipmentTiles();
            _inventoryGridPanel.style.display = characterInfo ? DisplayStyle.None : DisplayStyle.Flex;
            _inventoryHeroPanel.style.display = characterInfo ? DisplayStyle.Flex : DisplayStyle.None;
            _storagePanel.style.display = DisplayStyle.None;
            _inventoryDetailPanel.style.display = DisplayStyle.Flex;
            _inventoryFooter.style.display = DisplayStyle.Flex;
            ApplyLgoSelectedTab(_bagTab, !characterInfo);
            ApplyLgoSelectedTab(_characterInfoTab, characterInfo);
            _storageTab.style.backgroundColor = new Color(.045f,.13f,.18f);
            RefreshInventoryDetailCard();
        }

        private void ShowStorageMode()
        {
            _characterInfoOpen = false;
            _storageOpen = true;
            _inventoryHeroPanel.style.flexGrow = 0;
            _inventoryGridPanel.style.flexGrow = 0;
            _storagePanel.style.flexGrow = 1;
            RefreshInventoryModalHeader();
            _inventoryGridPanel.style.display = DisplayStyle.None;
            _inventoryHeroPanel.style.display = DisplayStyle.None;
            _storagePanel.style.display = DisplayStyle.Flex;
            _inventoryDetailPanel.style.display = DisplayStyle.None;
            _inventoryFooter.style.display = DisplayStyle.None;
            _bagTab.style.backgroundColor = new Color(.045f,.13f,.18f);
            _characterInfoTab.style.backgroundColor = new Color(.045f,.13f,.18f);
            ApplyLgoSelectedTab(_storageTab, true);
        }


        private void RefreshInventoryModalHeader()
        {
            if (_inventoryModalTitle == null || _inventoryModalSubtitle == null) return;
            if (_storageOpen)
            {
                _inventoryModalTitle.text = "RƯƠNG ĐỒ";
                _inventoryModalSubtitle.text = "Kho gửi/rút chưa mở trong bản trải nghiệm";
            }
            else if (_characterInfoOpen)
            {
                _inventoryModalTitle.text = "THÔNG TIN";
                _inventoryModalSubtitle.text = "Xem nhân vật, trang bị đang mặc và chi tiết món bên phải";
            }
            else
            {
                _inventoryModalTitle.text = "HÀNH TRANG";
                _inventoryModalSubtitle.text = "Túi đồ, vật phẩm và chi tiết món bên phải";
            }
        }

        private void ShowInventoryPage(bool supplies)
        {
            _suppliesOpen = supplies;
            RefreshInventoryEquipmentTiles();
            _equipmentPage.style.display = supplies ? DisplayStyle.None : DisplayStyle.Flex;
            _suppliesPage.style.display = supplies ? DisplayStyle.Flex : DisplayStyle.None;
            if (!_storageOpen)
            {
                _inventoryFooter.style.display = DisplayStyle.Flex;
                _inventoryDetailPanel.style.display = DisplayStyle.Flex;
            }
            ApplyLgoSelectedTab(_equipmentTab, !supplies);
            ApplyLgoSelectedTab(_suppliesTab, supplies);
            RefreshInventoryDetailCard();
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
            var selected = _selectedSupplyItemId == itemId;
            button.text = string.Empty;
            if (nameLabel != null) nameLabel.text = label;
            if (countLabel != null) countLabel.text = "x" + count;
            if (stateLabel != null) stateLabel.text = state;
            button.style.backgroundColor = selected ? new Color(.12f, .33f, .56f, .98f) : new Color(.045f, .13f, .18f, .98f);
            button.style.borderTopColor = button.style.borderBottomColor = selected ? new Color(.92f, .72f, .36f, .86f) : new Color(.50f, .58f, .58f, .55f);
            button.style.borderLeftColor = button.style.borderRightColor = selected ? new Color(.92f, .72f, .36f, .86f) : new Color(.50f, .58f, .58f, .55f);
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
                _equipmentTiles[index].style.backgroundColor = slotId == _scene.VoSelectedEquipmentSlot
                    ? new Color(.12f, .33f, .56f, .98f)
                    : equipped ? new Color(.045f, .12f, .18f, .96f) : new Color(.025f, .040f, .052f, .78f);
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
            var gender = _scene.VoAvatarGender == "female" ? "Nữ" : "Nam";
            if (_characterHeroName != null) _characterHeroName.text = "LụcThiên · " + _scene.ActiveEquipmentClassLabel + " " + gender;
            if (_characterHeroPower != null) _characterHeroPower.text = "LC 245.780";
            if (_characterHeroVitals != null) _characterHeroVitals.text = "HP " + _scene.PlayerHealth + "/100  ·  MP " + _scene.PlayerMana + "/100";
            if (_characterHeroLoadout != null) _characterHeroLoadout.text = "Trang bị " + _scene.VoEquippedSlotCount + "/10 · Lv" + _scene.VoAvatarLevel;
            if (_inventoryCountBadge != null) _inventoryCountBadge.text = "56/120 ô";

            var portraitSprite = _scene.GetVoEquipmentThumbnailSprite(_scene.VoSelectedEquipmentSlot)
                ?? _scene.GetVoEquipmentThumbnailSprite("main_weapon");
            if (_characterHeroPortrait != null)
            {
                _characterHeroPortrait.style.backgroundImage = portraitSprite == null ? StyleKeyword.None : new StyleBackground(portraitSprite);
                _characterHeroPortrait.style.display = portraitSprite == null ? DisplayStyle.None : DisplayStyle.Flex;
            }
            if (_characterHeroQuickIcons == null || _equipmentSlotIds == null) return;
            for (var i = 0; i < _characterHeroQuickIcons.Length; i++)
            {
                var slot = i < _equipmentSlotIds.Count ? _equipmentSlotIds[i] : null;
                var sprite = string.IsNullOrEmpty(slot) ? null : _scene.GetVoEquipmentThumbnailSprite(slot);
                _characterHeroQuickIcons[i].style.backgroundImage = sprite == null ? StyleKeyword.None : new StyleBackground(sprite);
                _characterHeroQuickIcons[i].style.display = sprite == null ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        private void RefreshInventoryDetailChips(string level, string state, string fit)
        {
            if (_inventoryDetailLevelChip != null) _inventoryDetailLevelChip.text = level;
            if (_inventoryDetailEquippedChip != null) _inventoryDetailEquippedChip.text = state;
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
            if (_inventoryDetailPrimaryAction != null) _inventoryDetailPrimaryAction.SetEnabled(true);
            var selectedSlot = _scene.VoSelectedEquipmentSlot;
            var selectedEquipped = _scene.IsVoEquipmentSlotEquipped(selectedSlot);
            var selectedLevel = _scene.GetVoEquipmentItemLevel(selectedSlot);
            var selectedName = EquipmentDisplayName(selectedSlot);
            var thumbnail = _scene.GetVoEquipmentThumbnailSprite(selectedSlot);
            _inventoryDetailIcon.text = "";
            _inventoryDetailIcon.style.backgroundImage = thumbnail == null ? StyleKeyword.None : new StyleBackground(thumbnail);
            _inventoryDetailIcon.style.display = thumbnail == null ? DisplayStyle.None : DisplayStyle.Flex;
            _equipmentDetail.text = selectedName + " · Lv" + selectedLevel;
            _inventoryDetailRarity.text = "Trang bị · Lv" + selectedLevel;
            _inventoryDetailSlotType.text = selectedName;
            _inventoryItemId.text = _scene.GetVoEquipmentItemId(selectedSlot);
            _inventoryDetailStateBadge.text = selectedEquipped ? "ĐANG MẶC" : "ĐÃ THÁO";
            _inventoryItemState.text = selectedEquipped ? "Đang mặc trên nhân vật." : "Đã tháo khỏi nhân vật.";
            _inventoryDetailStatPrimary.text = "Chưa có thuộc tính chiến đấu được công bố.";
            _inventoryDetailStatFit.text = "Dành cho " + _scene.ActiveEquipmentClassLabel + " · " + (_scene.VoAvatarGender == "female" ? "Nữ" : "Nam");
            RefreshInventoryDetailChips("Lv" + selectedLevel, selectedEquipped ? "Đang mặc" : "Đã tháo", _scene.ActiveEquipmentClassLabel + " · " + (_scene.VoAvatarGender == "female" ? "Nữ" : "Nam"));
            if (_inventoryDetailPrimaryAction != null)
                _inventoryDetailPrimaryAction.text = selectedEquipped ? "Tháo món đang chọn" : "Mặc món đang chọn";
            if (_equipmentToggle != null)
                _equipmentToggle.text = selectedEquipped ? "Tháo món đang chọn" : "Mặc món đang chọn";
        }

        private void RefreshInventorySupplyDetailCard()
        {
            _inventoryDetailIcon.text = "";
            _inventoryDetailIcon.style.backgroundImage = StyleKeyword.None;
            _inventoryDetailIcon.style.display = DisplayStyle.None;
            _inventoryDetailHeader.text = "CHI TIẾT VẬT PHẨM";
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
                RefreshInventoryDetailChips("x" + _scene.ManaPotionCount, _inventoryDetailStateBadge.text, "Hồi phục MP");
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
                RefreshInventoryDetailChips(_scene.HasClassRewardItem ? "x1" : "x0", _inventoryDetailStateBadge.text, "Nhiệm vụ");
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
                RefreshInventoryDetailChips("x" + _scene.HealthPotionCount, _inventoryDetailStateBadge.text, "Hồi phục HP");
                _inventoryDetailPrimaryAction.text = "Dùng bình máu";
                _inventoryDetailPrimaryAction.SetEnabled(_scene.HealthPotionCount > 0 && _scene.PlayerHealth < 100);
            }
            if (_equipmentToggle != null) _equipmentToggle.style.display = DisplayStyle.None;
            if (_equipmentVariant != null) _equipmentVariant.style.display = DisplayStyle.None;
        }

    }
}
