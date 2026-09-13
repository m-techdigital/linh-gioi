using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _equipmentPage, _suppliesPage, _storagePanel, _inventoryFooter, _inventoryHeroPanel, _inventoryGridPanel, _inventoryDetailPanel;
        private Label _inventoryHeroTitle, _inventoryHeroMeta, _inventoryItemId, _inventoryItemState, _inventoryDetailHeader, _inventoryDetailIcon, _inventoryDetailRarity, _inventoryDetailSlotType, _inventoryDetailStateBadge, _inventoryDetailStatPrimary, _inventoryDetailStatFit, _suppliesTitle, _suppliesEmptyState, _storageState;
        private Button _bagTab, _characterInfoTab, _storageTab, _equipmentTab, _suppliesTab;
        private Button _inventoryDetailPrimaryAction;
        private Button[] _equipmentTiles;
        private VisualElement[] _equipmentTileIcons;
        private Label[] _equipmentTileNames, _equipmentTileStates;
        private bool _characterInfoOpen, _suppliesOpen, _storageOpen;
        private string _selectedSupplyItemId = "health_potion";

        private Button InventoryButton(Action action, string name, string text = "")
        {
            var button = new Button(action) { name = name, text = text };
            button.style.minHeight = _touch ? 58 : 46;
            button.style.minWidth = 0;
            button.style.flexGrow = 1;
            button.style.flexBasis = 0;
            button.style.marginRight = 4;
            button.style.marginLeft = 0;
            button.style.fontSize = 17;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            ApplyLgoButton(button);
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
            ApplyLgoGlassPanel(panel, true);
            panel.style.paddingLeft = panel.style.paddingRight = 12;
            panel.style.paddingTop = panel.style.paddingBottom = 10;
            panel.style.minWidth = 0;
            return panel;
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
            titleGroup.Add(LgoLabel("HÀNH TRANG", 26, UiGold, true));
            titleGroup.Add(LgoLabel("Túi đồ và thông tin nhân vật dùng chung chi tiết món", 14, new Color(.73f, .85f, .88f, .88f)));
            var close = InventoryButton(() => { if (_scene.InventoryOpen) _scene.ToggleInventory(); },
                "LGO Inventory Close", "×");
            close.tooltip = "Đóng hành trang (I / Esc)";
            close.style.flexGrow = 0; close.style.flexBasis = 54; close.style.minHeight = 48; close.style.fontSize = 30;
            header.Add(titleGroup); header.Add(close); _inventory.Add(header);

            var mainTabs = InventoryRow("Map01A Inventory Main Tabs");
            _bagTab = InventoryButton(() => ShowInventoryMode(false), "Map01A Bag Main Tab", "Hành trang");
            _characterInfoTab = InventoryButton(() => ShowInventoryMode(true), "Map01A Character Info Main Tab", "Thông tin");
            _storageTab = InventoryButton(ShowStorageMode, "Map01A Storage Main Tab", "Rương đồ");
            mainTabs.Add(_bagTab); mainTabs.Add(_characterInfoTab); mainTabs.Add(_storageTab); _inventory.Add(mainTabs);

            var body = new VisualElement { name = "Map01A Inventory Body" };
            body.style.flexDirection = FlexDirection.Row;
            body.style.flexGrow = 1;
            body.style.minHeight = 0;
            body.style.marginTop = 6;
            _inventory.Add(body);

            _inventoryDetailPanel = InventoryPanel("Map01A Inventory Detail Panel");
            _inventoryDetailPanel.style.flexGrow = 0;
            _inventoryDetailPanel.style.flexBasis = 300;
            _inventoryDetailPanel.style.marginLeft = 10;
            ApplyLgoDetailCard(_inventoryDetailPanel);
            _inventoryFooter = new VisualElement { name = "Map01A Inventory Footer" };
            _inventoryFooter.style.flexGrow = 1;
            _inventoryFooter.style.flexShrink = 0;
            _inventoryDetailHeader = LgoLabel("CHI TIẾT MÓN", 15, new Color(.73f, .85f, .88f, .92f), true);
            _inventoryDetailHeader.name = "Map01A Inventory Detail Header";
            _inventoryFooter.Add(_inventoryDetailHeader);
            _inventoryDetailIcon = LgoLabel("", 42, UiGold, true);
            _inventoryDetailIcon.name = "Map01A Inventory Detail Icon";
            ApplyLgoItemIcon(_inventoryDetailIcon);
            _inventoryDetailIcon.style.width = 78;
            _inventoryDetailIcon.style.height = 78;
            _inventoryDetailIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            _inventoryFooter.Add(_inventoryDetailIcon);
            _equipmentDetail = LgoLabel("", 24, UiGold, true);
            _equipmentDetail.style.marginTop = 6;
            _inventoryFooter.Add(_equipmentDetail);
            _inventoryDetailRarity = LgoLabel("", 15, new Color(.74f, .92f, 1f, .94f), true);
            _inventoryDetailRarity.name = "Map01A Inventory Detail Rarity";
            _inventoryDetailRarity.style.marginTop = 2;
            _inventoryFooter.Add(_inventoryDetailRarity);
            _inventoryDetailSlotType = LgoLabel("", 16, new Color(.88f, .94f, .92f, .94f), true);
            _inventoryDetailSlotType.name = "Map01A Inventory Detail Slot Type";
            _inventoryDetailSlotType.style.marginTop = 4;
            _inventoryFooter.Add(_inventoryDetailSlotType);
            _inventoryFooter.Add(LgoDivider("Map01A Inventory Detail Divider"));
            _inventoryItemId = LgoLabel("", 14, new Color(.78f, .88f, .90f, .88f));
            _inventoryItemId.style.marginTop = 2;
            _inventoryFooter.Add(_inventoryItemId);
            _inventoryDetailStateBadge = LgoLabel("", 15, new Color(.12f, .08f, .03f, 1f), true);
            _inventoryDetailStateBadge.name = "Map01A Inventory Detail State Badge";
            _inventoryDetailStateBadge.style.marginTop = 12;
            _inventoryDetailStateBadge.style.paddingLeft = 10;
            _inventoryDetailStateBadge.style.paddingRight = 10;
            _inventoryDetailStateBadge.style.paddingTop = 5;
            _inventoryDetailStateBadge.style.paddingBottom = 5;
            ApplyLgoFrame(_inventoryDetailStateBadge, UiGold, new Color(.98f, .86f, .48f, .92f));
            _inventoryFooter.Add(_inventoryDetailStateBadge);
            _inventoryItemState = LgoLabel("", 16, new Color(.91f, .93f, .84f, .96f));
            _inventoryItemState.style.marginTop = 12;
            _inventoryFooter.Add(_inventoryItemState);
            _inventoryDetailStatPrimary = LgoLabel("", 15, new Color(.76f, .92f, 1f, .96f), true);
            _inventoryDetailStatPrimary.name = "Map01A Inventory Detail Stat Primary";
            _inventoryDetailStatPrimary.style.marginTop = 10;
            _inventoryFooter.Add(_inventoryDetailStatPrimary);
            _inventoryDetailStatFit = LgoLabel("", 15, new Color(.76f, 1f, .70f, .96f), true);
            _inventoryDetailStatFit.name = "Map01A Inventory Detail Stat Fit";
            _inventoryDetailStatFit.style.marginTop = 4;
            _inventoryFooter.Add(_inventoryDetailStatFit);
            var actions = InventoryRow("Map01A Inventory Equipment Actions");
            actions.style.marginTop = 14;
            actions.style.marginBottom = 12;
            actions.style.flexShrink = 0;
            _inventoryDetailPrimaryAction = InventoryButton(UseInventoryDetailPrimaryAction, "Map01A Inventory Detail Primary Action");
            _inventoryDetailPrimaryAction.style.minHeight = 46;
            _equipmentToggle = InventoryButton(() => { _scene.ToggleVoEquipmentSlot(); RefreshInventoryEquipmentTiles(); RefreshInventoryDetailCard(); }, "LGO Equipment Inventory Toggle");
            _equipmentToggle.style.display = DisplayStyle.None;
            _equipmentVariant = InventoryButton(() => { _scene.CycleVoSelectedEquipmentItemLevel(); RefreshInventoryEquipmentTiles(); RefreshInventoryDetailCard(); }, "LGO Equipment Inventory Variant");
            _equipmentVariant.style.display = DisplayStyle.None;
            _equipmentVariant.style.minHeight = 46;
            actions.Add(_inventoryDetailPrimaryAction); actions.Add(_equipmentToggle); actions.Add(_equipmentVariant);
            _inventoryFooter.Add(actions); _inventoryDetailPanel.Add(_inventoryFooter);

            _inventoryGridPanel = InventoryPanel("Map01A Inventory Grid Panel");
            _inventoryGridPanel.style.flexGrow = 1;
            _inventoryGridPanel.style.marginRight = 10;
            body.Add(_inventoryGridPanel);
            var tabs = InventoryRow("Map01A Inventory Tabs");
            _equipmentTab = InventoryButton(() => ShowInventoryPage(false), "Map01A Equipment Tab", "Trang bị");
            _suppliesTab = InventoryButton(() => ShowInventoryPage(true), "Map01A Supplies Tab", "Vật phẩm");
            tabs.Add(_equipmentTab); tabs.Add(_suppliesTab); _inventoryGridPanel.Add(tabs);

            var scroll = new ScrollView(ScrollViewMode.Vertical)
            {
                name = "LGO Inventory Scroll",
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            RuntimeUiOverflowGuard.ApplyBoundedScroll(scroll, 900);
            scroll.style.flexGrow = 1;
            scroll.style.minHeight = 0;
            scroll.contentViewport.RegisterCallback<GeometryChangedEvent>(evt =>
                scroll.contentContainer.style.width = evt.newRect.width);
            _inventoryGridPanel.Add(scroll);

            _inventoryHeroPanel = InventoryPanel("Map01A Inventory Character Panel");
            _inventoryHeroPanel.style.flexGrow = 0;
            _inventoryHeroPanel.style.flexBasis = 360;
            body.Add(_inventoryHeroPanel);
            _inventoryHeroTitle = LgoLabel("", 22, UiGold, true);
            _inventoryHeroMeta = LgoLabel("", 15, new Color(.78f, .94f, .96f, .92f));
            _inventoryHeroPanel.Add(_inventoryHeroTitle);
            _inventoryHeroPanel.Add(_inventoryHeroMeta);
            var heroDivider = new VisualElement();
            heroDivider.style.height = 1; heroDivider.style.marginTop = heroDivider.style.marginBottom = 10;
            heroDivider.style.backgroundColor = new Color(.75f, .60f, .32f, .45f);
            _inventoryHeroPanel.Add(heroDivider);
            _equipmentTitle = LgoLabel("", 16, new Color(.95f, .86f, .58f, .96f), true);
            _inventoryHeroPanel.Add(_equipmentTitle);

            var equipmentSlots = new VisualElement { name = "Map01A Equipment Grid" };
            equipmentSlots.style.flexGrow = 1;
            equipmentSlots.style.minHeight = 0;
            equipmentSlots.style.flexDirection = FlexDirection.Row;
            equipmentSlots.style.flexWrap = Wrap.Wrap;
            equipmentSlots.style.marginTop = 8;
            _inventoryHeroPanel.Add(equipmentSlots);
            _equipmentSlotIds = _scene.VoEquipmentSlotIds;
            _equipmentRows = new Button[_equipmentSlotIds.Count];
            for (var i = 0; i < _equipmentSlotIds.Count; i++)
            {
                var slotId = _equipmentSlotIds[i];
                var row = InventoryButton(() => SelectInventoryEquipmentSlot(slotId), "LGO Equipment Inventory Slot " + slotId);
                row.style.flexGrow = 0;
                row.style.flexBasis = new Length(48, LengthUnit.Percent);
                row.style.height = 52;
                row.style.marginBottom = 6;
                row.style.marginRight = i % 2 == 0 ? 7 : 0;
                row.style.fontSize = 15;
                row.style.unityTextAlign = TextAnchor.MiddleCenter;
                _equipmentRows[i] = row;
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
            _storagePanel.Add(LgoLabel("RƯƠNG ĐỒ", 22, UiGold, true));
            _storageState = LgoLabel("Kho gửi/rút chưa kết nối model dữ liệu thật trong Map01A. Không tạo vật phẩm giả; khi có storage API/state sẽ dùng lại panel chi tiết bên phải để xem món đang chọn.", 16, new Color(.91f, .93f, .84f, .96f));
            _storageState.name = "Map01A Storage State";
            _storageState.style.marginTop = 10;
            _storagePanel.Add(_storageState);
            var storageActions = InventoryRow("Map01A Storage Actions");
            storageActions.style.marginTop = 16;
            var deposit = InventoryButton(() => { }, "Map01A Storage Deposit", "Gửi đồ");
            var withdraw = InventoryButton(() => { }, "Map01A Storage Withdraw", "Rút đồ");
            deposit.SetEnabled(false);
            withdraw.SetEnabled(false);
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
                tile.style.flexGrow = 0;
                tile.style.flexBasis = new Length(31.5f, LengthUnit.Percent);
                tile.style.height = 94;
                tile.style.marginRight = 6;
                tile.style.marginBottom = 7;
                tile.style.fontSize = 15;
                tile.style.flexDirection = FlexDirection.Row;
                tile.style.alignItems = Align.Center;
                tile.style.justifyContent = Justify.FlexStart;
                tile.style.unityTextAlign = TextAnchor.MiddleLeft;

                var icon = new VisualElement { name = "Map01A Equipment Item Icon " + slotId };
                ApplyLgoItemIcon(icon);
                icon.style.width = 56;
                icon.style.height = 56;
                icon.style.marginTop = 0;
                icon.style.marginBottom = 0;
                icon.style.marginLeft = 2;
                icon.style.marginRight = 8;
                tile.Add(icon);

                var textGroup = new VisualElement { name = "Map01A Equipment Item Text " + slotId };
                textGroup.style.flexGrow = 1;
                textGroup.style.minWidth = 0;
                textGroup.style.flexDirection = FlexDirection.Column;
                var nameLabel = LgoLabel("", 14, UiText, true);
                nameLabel.name = "Map01A Equipment Item Name " + slotId;
                var stateLabel = LgoLabel("", 12, UiSubText);
                stateLabel.name = "Map01A Equipment Item State " + slotId;
                stateLabel.style.marginTop = 2;
                textGroup.Add(nameLabel);
                textGroup.Add(stateLabel);
                tile.Add(textGroup);

                _equipmentTiles[i] = tile;
                _equipmentTileIcons[i] = icon;
                _equipmentTileNames[i] = nameLabel;
                _equipmentTileStates[i] = stateLabel;
                _equipmentPage.Add(tile);
            }
            scroll.Add(_equipmentPage);

            _suppliesPage = new VisualElement { name = "Map01A Supplies Page" };
            _suppliesPage.style.flexShrink = 0;
            _suppliesTitle = LgoLabel("Vật phẩm nhiệm vụ", 18, UiGold, true);
            _suppliesPage.Add(_suppliesTitle);
            _inventorySummary = LgoLabel("", 17, new Color(.91f, .93f, .84f, .96f));
            _inventorySummary.style.marginTop = 8; _inventorySummary.style.marginBottom = 10;
            _suppliesPage.Add(_inventorySummary);
            _suppliesEmptyState = LgoLabel("", 15, new Color(.70f, .80f, .80f, .92f));
            _suppliesEmptyState.name = "Map01A Supplies Empty State";
            _suppliesEmptyState.style.marginTop = 2;
            _suppliesEmptyState.style.marginBottom = 12;
            _suppliesEmptyState.style.paddingLeft = 10;
            _suppliesEmptyState.style.paddingRight = 10;
            _suppliesEmptyState.style.paddingTop = 8;
            _suppliesEmptyState.style.paddingBottom = 8;
            ApplyLgoFrame(_suppliesEmptyState, new Color(.020f, .060f, .088f, .86f), new Color(.50f, .58f, .58f, .55f));
            _suppliesPage.Add(_suppliesEmptyState);
            _questItemActions = new VisualElement { name = "Map01A Quest Item Actions" };
            _healthPotion = InventoryButton(() => SelectInventorySupply("health_potion"), "Map01A Health Potion", "Bình Máu Nhỏ");
            _manaPotion = InventoryButton(() => SelectInventorySupply("mana_potion"), "Map01A Mana Potion", "Bình Linh Lực Nhỏ");
            _equipReward = InventoryButton(() => SelectInventorySupply("class_reward"), "Map01A Equip Reward", "Hộ Uyển Võ Tân Thủ");
            foreach (var button in new[] { _healthPotion, _manaPotion, _equipReward })
            {
                button.style.flexGrow = 0; button.style.flexBasis = StyleKeyword.Auto;
                button.style.marginBottom = 8;
                button.style.color = new Color(.70f, .80f, .80f, .92f);
                button.style.unityTextAlign = TextAnchor.MiddleLeft;
                button.style.paddingLeft = 14;
                _questItemActions.Add(button);
            }
            _suppliesPage.Add(_questItemActions); scroll.Add(_suppliesPage);

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
            _characterInfoOpen = characterInfo;
            _storageOpen = false;
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
            _inventoryGridPanel.style.display = DisplayStyle.None;
            _inventoryHeroPanel.style.display = DisplayStyle.None;
            _storagePanel.style.display = DisplayStyle.Flex;
            _inventoryDetailPanel.style.display = DisplayStyle.None;
            _inventoryFooter.style.display = DisplayStyle.None;
            _bagTab.style.backgroundColor = new Color(.045f,.13f,.18f);
            _characterInfoTab.style.backgroundColor = new Color(.045f,.13f,.18f);
            ApplyLgoSelectedTab(_storageTab, true);
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
            StyleSupplyRow(_healthPotion, _selectedSupplyItemId == "health_potion");
            StyleSupplyRow(_manaPotion, _selectedSupplyItemId == "mana_potion");
            StyleSupplyRow(_equipReward, _selectedSupplyItemId == "class_reward");
        }

        private static void StyleSupplyRow(Button button, bool selected)
        {
            button.style.backgroundColor = selected ? new Color(.12f, .33f, .56f, .98f) : new Color(.045f, .13f, .18f, .98f);
            button.style.borderTopColor = button.style.borderBottomColor = selected ? new Color(.92f, .72f, .36f, .86f) : new Color(.50f, .58f, .58f, .55f);
            button.style.borderLeftColor = button.style.borderRightColor = selected ? new Color(.92f, .72f, .36f, .86f) : new Color(.50f, .58f, .58f, .55f);
            button.style.color = new Color(.70f, .80f, .80f, .92f);
        }

        private void RefreshInventoryEquipmentTiles()
        {
            if (_scene == null || _equipmentRows == null || _equipmentTiles == null) return;
            for (var index = 0; index < _equipmentRows.Length; index++)
            {
                var slotId = _equipmentSlotIds[index];
                var equipped = _scene.IsVoEquipmentSlotEquipped(slotId);
                var level = _scene.GetVoEquipmentItemLevel(slotId);
                _equipmentRows[index].text = (equipped ? "✓ " : "○ ") + EquipmentShortName(slotId)
                    + "\nLv" + level;
                _equipmentRows[index].style.backgroundColor = slotId == _scene.VoSelectedEquipmentSlot
                    ? new Color(.16f, .48f, .50f, .96f)
                    : equipped ? new Color(.06f, .13f, .17f, .94f) : new Color(.035f, .055f, .065f, .82f);
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
                    var thumbnail = _scene.GetVoEquipmentThumbnailSprite(slotId);
                    _equipmentTileIcons[index].style.backgroundImage = thumbnail == null ? StyleKeyword.None : new StyleBackground(thumbnail);
                    _equipmentTileIcons[index].style.display = thumbnail == null ? DisplayStyle.None : DisplayStyle.Flex;
                }
            }
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
            _inventoryDetailRarity.text = "Tinh phẩm · Lv" + selectedLevel + " · 10 slot chung";
            _inventoryDetailSlotType.text = selectedName;
            _inventoryItemId.text = _scene.GetVoEquipmentItemId(selectedSlot);
            _inventoryDetailStateBadge.text = selectedEquipped ? "ĐANG MẶC" : "ĐÃ THÁO";
            _inventoryItemState.text = selectedEquipped ? "Đang mặc trên nhân vật." : "Đã tháo khỏi nhân vật.";
            _inventoryDetailStatPrimary.text = EquipmentPrimaryStat(selectedSlot, selectedLevel);
            _inventoryDetailStatFit.text = "Khớp: " + _scene.EquipmentFitSummary;
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
            _inventoryDetailRarity.text = "Nhiệm vụ · Map01A · local-only";
            if (_selectedSupplyItemId == "mana_potion")
            {
                _equipmentDetail.text = "Bình Linh Lực Nhỏ";
                _inventoryDetailSlotType.text = "Vật phẩm hồi phục";
                _inventoryItemId.text = "map01a_mana_potion_small";
                _inventoryDetailStateBadge.text = _scene.ManaPotionCount > 0 && _scene.PlayerMana < 100 ? "CÓ THỂ DÙNG" : "TẠM KHÓA";
                _inventoryItemState.text = "Số lượng: " + _scene.ManaPotionCount + " · MP " + _scene.PlayerMana + "/100.";
                _inventoryDetailStatPrimary.text = "Hồi MP +50";
                _inventoryDetailStatFit.text = "Dùng khi MP chưa đầy.";
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
                _inventoryDetailStatPrimary.text = "Thủ +24 · Sinh lực +80";
                _inventoryDetailStatFit.text = "Trang bị nhiệm vụ dùng chung detail bên phải.";
                _inventoryDetailPrimaryAction.text = "Trang bị hộ uyển";
                _inventoryDetailPrimaryAction.SetEnabled(_scene.HasClassRewardItem && !_scene.IsClassRewardEquipped);
            }
            else
            {
                _selectedSupplyItemId = "health_potion";
                _equipmentDetail.text = "Bình Máu Nhỏ";
                _inventoryDetailSlotType.text = "Vật phẩm hồi phục";
                _inventoryItemId.text = "map01a_health_potion_small";
                _inventoryDetailStateBadge.text = _scene.HealthPotionCount > 0 && _scene.PlayerHealth < 100 ? "CÓ THỂ DÙNG" : "TẠM KHÓA";
                _inventoryItemState.text = "Số lượng: " + _scene.HealthPotionCount + " · HP " + _scene.PlayerHealth + "/100.";
                _inventoryDetailStatPrimary.text = "Hồi HP +50";
                _inventoryDetailStatFit.text = "Dùng khi HP chưa đầy.";
                _inventoryDetailPrimaryAction.text = "Dùng bình máu";
                _inventoryDetailPrimaryAction.SetEnabled(_scene.HealthPotionCount > 0 && _scene.PlayerHealth < 100);
            }
            if (_equipmentToggle != null) _equipmentToggle.style.display = DisplayStyle.None;
            if (_equipmentVariant != null) _equipmentVariant.style.display = DisplayStyle.None;
        }

        private static string EquipmentPrimaryStat(string slotId, int level)
        {
            var value = 18 + level * 7;
            if (slotId == "main_weapon") return "Công +" + (value + 28) + " · Chính xác +" + (level * 3 + 9);
            if (slotId == "boots") return "Tốc +" + (level * 2 + 6) + " · Né +" + (level * 3 + 8);
            if (slotId == "wrist_guard") return "Bạo kích +" + (level * 2 + 4) + " · Công +" + value;
            return "Thủ +" + value + " · Sinh lực +" + (level * 18 + 60);
        }
    }
}
