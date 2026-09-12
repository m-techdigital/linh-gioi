using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _equipmentPage, _suppliesPage, _storagePanel, _inventoryFooter, _inventoryHeroPanel, _inventoryGridPanel, _inventoryDetailPanel;
        private Label _inventoryHeroTitle, _inventoryHeroMeta, _inventoryItemId, _inventoryItemState, _suppliesTitle, _storageState;
        private Button _bagTab, _characterInfoTab, _storageTab, _equipmentTab, _suppliesTab;
        private Button[] _equipmentTiles;
        private bool _characterInfoOpen, _suppliesOpen, _storageOpen;

        private static readonly Color InventoryGlass = new Color(.012f, .045f, .078f, .965f);
        private static readonly Color InventoryGlassRaised = new Color(.026f, .082f, .128f, .94f);
        private static readonly Color InventoryGold = new Color(.95f, .75f, .36f, .95f);
        private static readonly Color InventoryBlue = new Color(.10f, .35f, .58f, .96f);

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
            StyleFrame(button, new Color(.045f, .13f, .18f, .98f), new Color(.50f, .58f, .58f, .55f));
            button.style.color = new Color(.96f, .91f, .76f);
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

        private static void StyleFrame(VisualElement element, Color background, Color border)
        {
            element.style.backgroundColor = background;
            element.style.borderTopWidth = element.style.borderBottomWidth = 1;
            element.style.borderLeftWidth = element.style.borderRightWidth = 1;
            element.style.borderTopColor = element.style.borderBottomColor = border;
            element.style.borderLeftColor = element.style.borderRightColor = border;
        }

        private static Label InventoryLabel(string text, int size, Color color, bool bold = false)
        {
            var label = new Label(text);
            label.style.fontSize = size;
            label.style.color = color;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
            return label;
        }

        private VisualElement InventoryPanel(string name)
        {
            var panel = new VisualElement { name = name };
            StyleFrame(panel, InventoryGlassRaised, new Color(.58f, .47f, .27f, .50f));
            panel.style.paddingLeft = panel.style.paddingRight = 12;
            panel.style.paddingTop = panel.style.paddingBottom = 10;
            panel.style.minWidth = 0;
            return panel;
        }

        private void BuildInventory()
        {
            _inventory = new VisualElement { name = "Map01A Inventory" };
            Box(_inventory); Place(_inventory, 72, 72, 86, 72);
            _inventory.style.backgroundColor = InventoryGlass;
            _inventory.style.flexDirection = FlexDirection.Column;
            _inventory.style.paddingLeft = _inventory.style.paddingRight = 12;
            _inventory.style.paddingTop = _inventory.style.paddingBottom = 12;
            StyleFrame(_inventory, InventoryGlass, new Color(.78f, .62f, .32f, .70f));

            var header = InventoryRow("Map01A Inventory Header");
            header.style.alignItems = Align.Center;
            var titleGroup = new VisualElement();
            titleGroup.style.flexGrow = 1;
            titleGroup.Add(InventoryLabel("HÀNH TRANG", 26, InventoryGold, true));
            titleGroup.Add(InventoryLabel("Túi đồ và thông tin nhân vật dùng chung chi tiết món", 14, new Color(.73f, .85f, .88f, .88f)));
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
            _inventoryFooter = new VisualElement { name = "Map01A Inventory Footer" };
            _inventoryFooter.style.flexGrow = 1;
            _inventoryFooter.style.flexShrink = 0;
            _inventoryFooter.Add(InventoryLabel("Đang chọn", 15, new Color(.73f, .85f, .88f, .92f)));
            _equipmentDetail = InventoryLabel("", 22, InventoryGold, true);
            _equipmentDetail.style.marginTop = 6;
            _inventoryFooter.Add(_equipmentDetail);
            _inventoryItemId = InventoryLabel("", 14, new Color(.78f, .88f, .90f, .88f));
            _inventoryItemId.style.marginTop = 8;
            _inventoryFooter.Add(_inventoryItemId);
            _inventoryItemState = InventoryLabel("", 16, new Color(.91f, .93f, .84f, .96f));
            _inventoryItemState.style.marginTop = 12;
            _inventoryFooter.Add(_inventoryItemState);
            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            _inventoryFooter.Add(spacer);
            var actions = InventoryRow("Map01A Inventory Equipment Actions");
            actions.style.marginTop = 10;
            _equipmentToggle = InventoryButton(() => _scene.ToggleVoEquipmentSlot(), "LGO Equipment Inventory Toggle");
            _equipmentVariant = InventoryButton(() => _scene.CycleVoSelectedEquipmentItemLevel(), "LGO Equipment Inventory Variant");
            actions.Add(_equipmentToggle); actions.Add(_equipmentVariant);
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
            _inventoryHeroTitle = InventoryLabel("", 22, InventoryGold, true);
            _inventoryHeroMeta = InventoryLabel("", 15, new Color(.78f, .94f, .96f, .92f));
            _inventoryHeroPanel.Add(_inventoryHeroTitle);
            _inventoryHeroPanel.Add(_inventoryHeroMeta);
            var heroDivider = new VisualElement();
            heroDivider.style.height = 1; heroDivider.style.marginTop = heroDivider.style.marginBottom = 10;
            heroDivider.style.backgroundColor = new Color(.75f, .60f, .32f, .45f);
            _inventoryHeroPanel.Add(heroDivider);
            _equipmentTitle = InventoryLabel("", 16, new Color(.95f, .86f, .58f, .96f), true);
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
                var row = InventoryButton(() => _scene.SelectVoEquipmentSlot(slotId), "LGO Equipment Inventory Slot " + slotId);
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
            _storagePanel.Add(InventoryLabel("RƯƠNG ĐỒ", 22, InventoryGold, true));
            _storageState = InventoryLabel("Kho gửi/rút chưa kết nối model dữ liệu thật trong Map01A. Không tạo vật phẩm giả; khi có storage API/state sẽ dùng lại panel chi tiết bên phải để xem món đang chọn.", 16, new Color(.91f, .93f, .84f, .96f));
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
            for (var i = 0; i < _equipmentSlotIds.Count; i++)
            {
                var slotId = _equipmentSlotIds[i];
                var tile = InventoryButton(() => _scene.SelectVoEquipmentSlot(slotId), "Map01A Equipment Item Tile " + slotId);
                tile.style.flexGrow = 0;
                tile.style.flexBasis = new Length(31.5f, LengthUnit.Percent);
                tile.style.height = 72;
                tile.style.marginRight = 6;
                tile.style.marginBottom = 7;
                tile.style.fontSize = 15;
                tile.style.unityTextAlign = TextAnchor.MiddleCenter;
                _equipmentTiles[i] = tile;
                _equipmentPage.Add(tile);
            }
            scroll.Add(_equipmentPage);

            _suppliesPage = new VisualElement { name = "Map01A Supplies Page" };
            _suppliesPage.style.flexShrink = 0;
            _suppliesTitle = InventoryLabel("Vật phẩm nhiệm vụ", 18, InventoryGold, true);
            _suppliesPage.Add(_suppliesTitle);
            _inventorySummary = InventoryLabel("", 17, new Color(.91f, .93f, .84f, .96f));
            _inventorySummary.style.marginTop = 8; _inventorySummary.style.marginBottom = 14;
            _suppliesPage.Add(_inventorySummary);
            _questItemActions = new VisualElement { name = "Map01A Quest Item Actions" };
            _healthPotion = InventoryButton(() => _scene.UseHealthPotion(), "Map01A Health Potion", "Bình Máu Nhỏ");
            _manaPotion = InventoryButton(() => _scene.UseManaPotion(), "Map01A Mana Potion", "Bình Linh Lực Nhỏ");
            _equipReward = InventoryButton(() => _scene.EquipClassReward(), "Map01A Equip Reward", "Hộ Uyển Võ Tân Thủ");
            foreach (var button in new[] { _healthPotion, _manaPotion, _equipReward })
            {
                button.style.flexGrow = 0; button.style.flexBasis = StyleKeyword.Auto;
                button.style.marginBottom = 8; _questItemActions.Add(button);
            }
            _suppliesPage.Add(_questItemActions); scroll.Add(_suppliesPage);

            body.Add(_inventoryDetailPanel);

            ShowInventoryPage(false); ShowInventoryMode(false); _safe.Add(_inventory);
        }

        private void ShowInventoryMode(bool characterInfo)
        {
            _characterInfoOpen = characterInfo;
            _storageOpen = false;
            _inventoryGridPanel.style.display = characterInfo ? DisplayStyle.None : DisplayStyle.Flex;
            _inventoryHeroPanel.style.display = characterInfo ? DisplayStyle.Flex : DisplayStyle.None;
            _storagePanel.style.display = DisplayStyle.None;
            _inventoryDetailPanel.style.display = characterInfo || !_suppliesOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _inventoryFooter.style.display = characterInfo || !_suppliesOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _bagTab.style.backgroundColor = characterInfo ? new Color(.045f,.13f,.18f) : InventoryBlue;
            _characterInfoTab.style.backgroundColor = characterInfo ? InventoryBlue : new Color(.045f,.13f,.18f);
            _storageTab.style.backgroundColor = new Color(.045f,.13f,.18f);
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
            _storageTab.style.backgroundColor = InventoryBlue;
        }

        private void ShowInventoryPage(bool supplies)
        {
            _suppliesOpen = supplies;
            _equipmentPage.style.display = supplies ? DisplayStyle.None : DisplayStyle.Flex;
            _suppliesPage.style.display = supplies ? DisplayStyle.Flex : DisplayStyle.None;
            if (!_storageOpen)
            {
                _inventoryFooter.style.display = !_characterInfoOpen && supplies ? DisplayStyle.None : DisplayStyle.Flex;
                _inventoryDetailPanel.style.display = !_characterInfoOpen && supplies ? DisplayStyle.None : DisplayStyle.Flex;
            }
            _equipmentTab.style.backgroundColor = supplies ? new Color(.045f,.13f,.18f) : InventoryBlue;
            _suppliesTab.style.backgroundColor = supplies ? InventoryBlue : new Color(.045f,.13f,.18f);
        }
    }
}
