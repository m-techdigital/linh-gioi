using System;
using System.Collections.Generic;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    // Arrival UI follows Map01A reference 01/05/10; reuses shared touch and viewport policies.
    public sealed class CongDongLamArrivalHud : MonoBehaviour
    {
        private CongDongLamMap01AArtPreview _scene;
        private VisualElement _root, _safe, _dialogue, _inventory, _combatBar, _questItemActions;
        private Label _quest, _marker, _dialogueSpeaker, _dialogueLine, _minimap, _inventorySummary, _equipmentTitle, _equipmentDetail;
        private Button _talk, _outfit, _level, _gender, _slot, _itemLevel, _toggleSlot, _run, _jump, _basic, _skill;
        private Button _inventoryToggle, _healthPotion, _manaPotion, _equipReward, _equipmentToggle, _equipmentVariant;
        private Button[] _equipmentRows;
        private IReadOnlyList<string> _equipmentSlotIds;
        private RuntimeTouchMovementPad _pad;
        private bool _touchJumpHeld;
        private RuntimeViewportMetrics _metrics;
        private PanelSettings _ownedPanel;
        private bool _touch;
        public static void Attach(CongDongLamMap01AArtPreview scene)
        {
            var host = new GameObject("Map01A Arrival HUD");
            host.transform.SetParent(scene.transform, false);
            var hud = host.AddComponent<CongDongLamArrivalHud>();
            hud._scene = scene;
            var document = host.AddComponent<UIDocument>();
            hud._ownedPanel = Instantiate(RuntimePanelSettingsProvider.LoadOrCreate());
            hud._ownedPanel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            hud._ownedPanel.referenceResolution = new Vector2Int(1600, 900);
            hud._ownedPanel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            hud._ownedPanel.match = 1f;
            document.panelSettings = hud._ownedPanel;
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-map01a-device");
            hud._touch = Application.isMobilePlatform || (index >= 0 && index + 1 < args.Length && args[index + 1] != "pc");
            hud.Build(document.rootVisualElement);
        }
        private static void Box(VisualElement e)
        {
            e.style.backgroundColor = new Color(.025f, .065f, .10f, .9f);
            e.style.color = new Color(.95f, .91f, .78f);
            e.style.paddingLeft = e.style.paddingRight = 12;
            e.style.paddingTop = e.style.paddingBottom = 8;
            e.style.fontSize = 20;
        }
        private static void Place(VisualElement e, float? left, float? right, float? top, float? bottom)
        {
            e.style.position = Position.Absolute;
            if (left.HasValue) e.style.left = left.Value;
            if (right.HasValue) e.style.right = right.Value;
            if (top.HasValue) e.style.top = top.Value;
            if (bottom.HasValue) e.style.bottom = bottom.Value;
        }
        private void Build(VisualElement root)
        {
            _root = root;
            _root.style.flexGrow = 1;
            _root.pickingMode = PickingMode.Ignore;
            _safe = new VisualElement { pickingMode = PickingMode.Ignore };
            _root.Add(_safe);
            var title = new Label("CỔNG ĐÔNG LÂM\nKhu an toàn • Lv1–3");
            Box(title); Place(title, 12, null, 12, null); _safe.Add(title);
            _quest = new Label(); Box(_quest); Place(_quest, null, 12, 12, null);
            _quest.style.width = 260; _quest.style.whiteSpace = WhiteSpace.Normal; _safe.Add(_quest);
            _minimap = new Label(); Box(_minimap); Place(_minimap, 220, null, 12, null);
            _minimap.style.width = 430; _minimap.style.fontSize = 16;
            _minimap.style.unityTextAlign = TextAnchor.MiddleCenter; _safe.Add(_minimap);
            _pad = new RuntimeTouchMovementPad(); Box(_pad); Place(_pad, 16, null, null, 16);
            _pad.style.width = _pad.style.height = 112;
            _pad.style.display = _touch ? DisplayStyle.Flex : DisplayStyle.None;
            _pad.style.borderTopLeftRadius = _pad.style.borderTopRightRadius = 52;
            _pad.style.borderBottomLeftRadius = _pad.style.borderBottomRightRadius = 52;
            var nub = new VisualElement { name = "LGO World Touch Movement Nub", pickingMode = PickingMode.Ignore };
            nub.style.width = nub.style.height = 36;
            nub.style.backgroundColor = new Color(.4f,.72f,.8f,.9f);
            nub.style.marginLeft = nub.style.marginTop = 22;
            _pad.Add(nub); _safe.Add(_pad);
            _talk = new Button(() => _scene.UseCurrentRouteAction()) { text = "Tương tác · E" };
            Box(_talk); Place(_talk, null, 16, null, 24); _talk.style.minHeight = _touch ? 64 : 48; _talk.style.minWidth = 170; _safe.Add(_talk);
            _outfit = new Button(() => _scene.CycleVoAvatarMode()) { text = "Trang bị Võ · C" };
            Box(_outfit); Place(_outfit, 16, null, _touch ? 90 : 90, null);
            _outfit.style.minHeight = _touch ? 56 : 42; _outfit.style.minWidth = 170; _safe.Add(_outfit);
            _level = new Button(() => _scene.CycleVoAvatarLevel()) { text = "Cấp trang bị · L" };
            Box(_level); Place(_level, 16, null, _touch ? 152 : 138, null);
            _level.style.minHeight = _touch ? 52 : 40; _level.style.minWidth = 170; _safe.Add(_level);
            _gender = new Button(() => _scene.CycleVoAvatarGender()) { text = "Nam/Nữ · G" };
            Box(_gender); Place(_gender, 16, null, _touch ? 210 : 184, null);
            _gender.style.minHeight = _touch ? 52 : 40; _gender.style.minWidth = 170; _safe.Add(_gender);
            _slot = new Button(() => _scene.CycleVoEquipmentSlot()) { text = "Chọn slot · V" };
            Box(_slot); Place(_slot, 16, null, _touch ? 268 : 230, null);
            _slot.style.minHeight = _touch ? 52 : 40; _slot.style.minWidth = 170; _safe.Add(_slot);
            _itemLevel = new Button(() => _scene.CycleVoSelectedEquipmentItemLevel()) { text = "Đổi cấp item · M" };
            Box(_itemLevel); Place(_itemLevel, 16, null, _touch ? 326 : 276, null);
            _itemLevel.style.minHeight = _touch ? 52 : 40; _itemLevel.style.minWidth = 170; _safe.Add(_itemLevel);
            _toggleSlot = new Button(() => _scene.ToggleVoEquipmentSlot()) { text = "Mặc/Cởi · B" };
            Box(_toggleSlot); Place(_toggleSlot, 16, null, _touch ? 384 : 322, null);
            _toggleSlot.style.minHeight = _touch ? 52 : 40; _toggleSlot.style.minWidth = 170; _safe.Add(_toggleSlot);
            _combatBar = new VisualElement(); Place(_combatBar, _touch ? 150 : 220, null, null, 24);
            _combatBar.style.flexDirection = FlexDirection.Row;
            _run = new Button(() => _scene.SetVoRun(!_scene.VoRunEnabled)) { text = "Chạy" };
            _jump = new Button { text = "Nhảy" };
            _jump.RegisterCallback<PointerDownEvent>(evt => { _touchJumpHeld = true; _jump.CapturePointer(evt.pointerId); _scene.SetVoJumpHeld(true); });
            _jump.RegisterCallback<PointerUpEvent>(evt => { _touchJumpHeld = false; _jump.ReleasePointer(evt.pointerId); _scene.SetVoJumpHeld(false); });
            _jump.RegisterCallback<PointerCaptureOutEvent>(evt => { _touchJumpHeld = false; _scene.SetVoJumpHeld(false); });
            _basic = new Button(() => _scene.TriggerVoBasicAttack()) { text = "Đánh" };
            _skill = new Button(() => _scene.TriggerVoSkill()) { text = "Liên Quyền" };
            foreach (var button in new[] { _run, _jump, _basic, _skill })
            {
                Box(button);
                button.style.minHeight = _touch ? 64 : 48;
                button.style.minWidth = _touch ? 112 : 108;
                button.style.marginRight = 6;
                _combatBar.Add(button);
            }
            _safe.Add(_combatBar);
            _inventoryToggle = new Button(() => _scene.ToggleInventory()) { text = "Hành trang · I" };
            Box(_inventoryToggle); Place(_inventoryToggle, null, _touch ? 408 : 410, null, 24);
            _inventoryToggle.style.minHeight = _touch ? 64 : 48; _inventoryToggle.style.minWidth = 180; _safe.Add(_inventoryToggle);
            _inventory = new VisualElement(); Box(_inventory); Place(_inventory, _touch ? 150 : 20, null, null, _touch ? 150 : 82);
            _inventory.style.width = _touch ? 410 : 470;
            _inventory.style.maxHeight = _touch ? 610 : 650;
            _inventorySummary = new Label(); _inventorySummary.style.whiteSpace = WhiteSpace.Normal; _inventory.Add(_inventorySummary);
            _equipmentTitle = new Label("TRANG BỊ VÕ · 10 SLOT");
            _equipmentTitle.style.color = new Color(1f, .78f, .25f);
            _equipmentTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            _equipmentTitle.style.marginTop = 8;
            _inventory.Add(_equipmentTitle);
            var equipmentGrid = new VisualElement();
            equipmentGrid.style.flexDirection = FlexDirection.Row;
            var equipmentLeft = new VisualElement();
            var equipmentRight = new VisualElement();
            equipmentLeft.style.flexGrow = equipmentRight.style.flexGrow = 1;
            equipmentLeft.style.marginRight = 6;
            equipmentGrid.Add(equipmentLeft);
            equipmentGrid.Add(equipmentRight);
            _equipmentSlotIds = _scene.VoEquipmentSlotIds;
            _equipmentRows = new Button[_equipmentSlotIds.Count];
            for (var index = 0; index < _equipmentSlotIds.Count; index++)
            {
                var slotId = _equipmentSlotIds[index];
                var row = new Button(() => _scene.SelectVoEquipmentSlot(slotId))
                    { name = "LGO Equipment Inventory Slot " + slotId };
                row.style.width = Length.Percent(100);
                row.style.height = _touch ? 42 : 36;
                row.style.marginBottom = 5;
                row.style.unityTextAlign = TextAnchor.MiddleLeft;
                _equipmentRows[index] = row;
                (index % 2 == 0 ? equipmentLeft : equipmentRight).Add(row);
            }
            _inventory.Add(equipmentGrid);
            _equipmentDetail = new Label();
            _equipmentDetail.style.whiteSpace = WhiteSpace.Normal;
            _equipmentDetail.style.marginTop = 5;
            _equipmentDetail.style.marginBottom = 5;
            _inventory.Add(_equipmentDetail);
            var equipmentActions = new VisualElement();
            equipmentActions.style.flexDirection = FlexDirection.Row;
            equipmentActions.style.flexWrap = Wrap.Wrap;
            _equipmentToggle = new Button(() => _scene.ToggleVoEquipmentSlot())
                { name = "LGO Equipment Inventory Toggle" };
            _equipmentVariant = new Button(() => _scene.CycleVoSelectedEquipmentItemLevel())
                { name = "LGO Equipment Inventory Variant", text = "Đổi cấp món" };
            foreach (var button in new[] { _equipmentToggle, _equipmentVariant })
            {
                button.style.minHeight = 40;
                button.style.minWidth = _touch ? 174 : 205;
                button.style.marginRight = 6;
                button.style.backgroundColor = new Color(.07f, .17f, .21f, .98f);
                button.style.color = new Color(.98f, .86f, .55f);
                equipmentActions.Add(button);
            }
            _inventory.Add(equipmentActions);
            _questItemActions = new VisualElement(); _questItemActions.style.flexDirection = FlexDirection.Row;
            _questItemActions.style.flexWrap = Wrap.Wrap;
            _healthPotion = new Button(() => _scene.UseHealthPotion()) { text = "Dùng Máu" };
            _manaPotion = new Button(() => _scene.UseManaPotion()) { text = "Dùng Linh Lực" };
            _equipReward = new Button(() => _scene.EquipClassReward()) { text = "Trang bị Hộ Uyển" };
            foreach (var button in new[] { _healthPotion, _manaPotion, _equipReward })
            {
                button.style.minHeight = 38;
                button.style.marginRight = 6;
                _questItemActions.Add(button);
            }
            _inventory.Add(_questItemActions); _safe.Add(_inventory);
            _dialogue = new VisualElement(); Box(_dialogue); Place(_dialogue, 142, 204, null, 20);
            _dialogueSpeaker = new Label("Hạ Vân");
            _dialogue.Add(_dialogueSpeaker);
            _dialogueLine = new Label(_scene.DialogueText); _dialogueLine.style.whiteSpace = WhiteSpace.Normal; _dialogue.Add(_dialogueLine);
            _safe.Add(_dialogue);
            _marker = new Label("!\nHạ Vân") { pickingMode = PickingMode.Ignore };
            _marker.style.position = Position.Absolute; _marker.style.color = new Color(1,.83f,.3f);
            _marker.style.width = 96; _marker.style.height = 56;
            _marker.style.fontSize = 18; _marker.style.unityTextAlign = TextAnchor.MiddleCenter;
            _root.Add(_marker);
            _root.RegisterCallback<GeometryChangedEvent>(_ => Layout());
            Layout();
        }
        private void Layout()
        {
            _metrics = RuntimeViewportMetrics.FromRoot(_root);
            var r = _metrics.SafePanelRect;
            Place(_safe, r.x, null, r.y, null); _safe.style.width = r.width; _safe.style.height = r.height;
            _quest.style.width = r.width < 900 ? 220 : 260;
            _minimap.style.left = r.width < 1100 ? 206 : 220;
            _minimap.style.width = r.width < 1100 ? 360 : 430;
            _dialogue.style.left = _touch ? 150 : 20;
            _combatBar.style.left = _touch ? 150 : 220;
            if (_scene.IsSourcePoseReviewActive)
            {
                _inventory.style.left = 16;
                _inventory.style.top = 72;
                _inventory.style.bottom = StyleKeyword.Auto;
            }
            if (r.width < 1300)
            {
                _inventoryToggle.style.left = 200;
                _inventoryToggle.style.right = StyleKeyword.Auto;
                _inventoryToggle.style.top = _touch ? 390 : 324;
                _inventoryToggle.style.bottom = StyleKeyword.Auto;
                _inventoryToggle.style.minWidth = 170;
            }
            else
            {
                _inventoryToggle.style.left = StyleKeyword.Auto;
                _inventoryToggle.style.right = _touch ? 408 : 410;
                _inventoryToggle.style.top = StyleKeyword.Auto;
                _inventoryToggle.style.bottom = 24;
                _inventoryToggle.style.minWidth = 180;
            }
            _talk.style.fontSize = _touch ? 20 : 18;
        }
        private void Update()
        {
            if (_scene == null || _root == null) return;
            var metrics = RuntimeViewportMetrics.FromRoot(_root);
            if (!_metrics.LayoutEquals(metrics)) Layout();
            if (!_scene.IsCapturing)
            {
                var keyboard = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f)
                    - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
                if (!_touch) _scene.SetVoRun(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                _scene.MoveOnLane(Mathf.Abs(_pad.Value.x) > .01f ? _pad.Value.x : keyboard, Time.deltaTime);
                if (Input.GetKeyDown(KeyCode.E)) _scene.UseCurrentRouteAction();
                if (Input.GetKeyDown(KeyCode.C)) _scene.CycleVoAvatarMode();
                if (Input.GetKeyDown(KeyCode.L)) _scene.CycleVoAvatarLevel();
                if (Input.GetKeyDown(KeyCode.G)) _scene.CycleVoAvatarGender();
                if (Input.GetKeyDown(KeyCode.V)) _scene.CycleVoEquipmentSlot();
                if (Input.GetKeyDown(KeyCode.M)) _scene.CycleVoSelectedEquipmentItemLevel();
                if (Input.GetKeyDown(KeyCode.B)) _scene.ToggleVoEquipmentSlot();
                if (Input.GetKeyDown(KeyCode.X)) _scene.TriggerVoSkill();
                _scene.SetVoJumpHeld(_touchJumpHeld || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));
                if (Input.GetKeyDown(KeyCode.Z)) _scene.TriggerVoBasicAttack();
                if (Input.GetKeyDown(KeyCode.I)) _scene.ToggleInventory();
                if (Input.GetKeyDown(KeyCode.H)) _scene.UseHealthPotion();
                if (Input.GetKeyDown(KeyCode.K)) _scene.UseManaPotion();
                if (Input.GetKeyDown(KeyCode.R)) _scene.EquipClassReward();
            }
            _quest.text = _scene.QuestTrackerText
                + (string.IsNullOrEmpty(_scene.LastInteractionMessage) ? "" : "\n" + _scene.LastInteractionMessage);
            _talk.SetEnabled(_scene.CanUseCurrentRouteAction);
            _talk.text = _scene.CurrentActionLabel + (_touch ? "" : " · E");
            _outfit.text = "Trang bị " + _scene.AvatarClassLabel + ": " + _scene.VoAvatarMode + (_touch ? "" : " · C");
            _level.text = "Cấp đồ: " + _scene.EquipmentLevelLabel + (_touch ? "" : " · L");
            _gender.text = "Thân: " + _scene.VoAvatarGender + (_touch ? "" : " · G");
            _slot.text = "Slot: " + _scene.EquipmentSlotLabel + (_touch ? "" : " · V");
            _itemLevel.text = "Đổi cấp item" + (_touch ? "" : " · M");
            _toggleSlot.text = (_scene.VoEquippedSlotCount == 10 ? "Cởi slot" : "Mặc/cởi") + (_touch ? "" : " · B");
            _run.text = (_scene.VoRunEnabled ? "Đang chạy" : "Chạy") + (_touch ? "" : " · Shift");
            _jump.text = (_scene.VoSomersaultEnabled ? "Nhảy lộn" : "Nhảy") + (_touch ? "" : " · ↑/W");
            _basic.text = _scene.VoAvatarMotionState == "basic_attack" ? "Đang đánh..." : "Đánh" + (_touch ? "" : " · Z");
            _skill.text = _scene.VoAvatarMotionState == "skill" ? "Đang thi triển..." : _scene.SkillLabel + (_touch ? "" : " · X");
            _skill.SetEnabled(_scene.CanTriggerVoSkill);
            _minimap.text = _scene.MinimapRouteText;
            _inventoryToggle.text = (_scene.InventoryOpen ? "Đóng hành trang" : "Hành trang") + (_touch ? "" : " · I");
            _inventory.style.display = _scene.InventoryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            var compactReview = _scene.IsSourcePoseReviewActive && _scene.InventoryOpen;
            foreach (var control in new[] { _outfit, _level, _gender, _slot, _itemLevel, _toggleSlot })
                control.style.display = compactReview ? DisplayStyle.None : DisplayStyle.Flex;
            _inventorySummary.text = _scene.IsSourcePoseReviewActive
                ? "Chọn từng món để xem thông tin và tháo/mặc trực tiếp trên nhân vật."
                : _scene.InventorySummaryText;
            _equipmentTitle.text = "TRANG BỊ " + (_scene.KiemPreviewActive ? "KIẾM" : "VÕ") + " · 10 SLOT";
            _questItemActions.style.display = _scene.IsSourcePoseReviewActive ? DisplayStyle.None : DisplayStyle.Flex;
            for (var index = 0; index < _equipmentRows.Length; index++)
            {
                var slotId = _equipmentSlotIds[index];
                var equipped = _scene.IsVoEquipmentSlotEquipped(slotId);
                _equipmentRows[index].text = (equipped ? "✓ " : "○ ") + EquipmentDisplayName(slotId)
                    + " · Lv" + _scene.GetVoEquipmentItemLevel(slotId);
                _equipmentRows[index].style.backgroundColor = slotId == _scene.VoSelectedEquipmentSlot
                    ? new Color(.16f, .48f, .50f, .96f) : new Color(.06f, .13f, .17f, .94f);
            }
            var selectedEquipped = _scene.IsVoEquipmentSlotEquipped(_scene.VoSelectedEquipmentSlot);
            _equipmentDetail.text = EquipmentDisplayName(_scene.VoSelectedEquipmentSlot)
                + "\n" + _scene.GetVoEquipmentItemId(_scene.VoSelectedEquipmentSlot)
                + "\nKhớp: " + _scene.EquipmentFitSummary
                + "\nTrạng thái: " + (selectedEquipped ? "ĐANG MẶC" : "ĐÃ THÁO");
            _equipmentToggle.text = selectedEquipped ? "Tháo món đang chọn" : "Mặc món đang chọn";
            var hasVariant = _scene.HasVoEquipmentItemVariant(_scene.VoSelectedEquipmentSlot);
            _equipmentVariant.text = hasVariant ? "Đổi cấp món" : "Chưa có cấp khác";
            _equipmentVariant.SetEnabled(hasVariant);
            _healthPotion.SetEnabled(_scene.HealthPotionCount > 0 && _scene.PlayerHealth < 100);
            _manaPotion.SetEnabled(_scene.ManaPotionCount > 0 && _scene.PlayerMana < 100);
            _equipReward.SetEnabled(_scene.HasClassRewardItem && !_scene.IsClassRewardEquipped);
            _dialogue.style.display = _scene.DialogueOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _dialogueSpeaker.text = _scene.DialogueSpeaker;
            _dialogueLine.text = _scene.DialogueText;
            _marker.text = "!\n" + _scene.CurrentRouteNodeLabel;
            var camera = Camera.main;
            if (camera != null)
            {
                var v = camera.WorldToViewportPoint(_scene.CurrentInteractionPosition);
                _marker.style.left = v.x * _metrics.PanelWidth - 48;
                _marker.style.top = (1-v.y) * _metrics.PanelHeight - 40;
            }
        }
        private static string EquipmentDisplayName(string slot)
        {
            switch (slot)
            {
                case "main_weapon": return "Vũ khí chính";
                case "head_hair": return "Đầu & tóc";
                case "inner_top": return "Áo trong";
                case "outer_tunic": return "Áo ngoài";
                case "lower_garment": return "Quần";
                case "waist": return "Đai lưng";
                case "arm_guard": return "Hộ uyển";
                case "boots": return "Giày";
                case "light_armor": return "Giáp vai/ngực";
                case "accessory": return "Phụ kiện Võ";
                default: return slot;
            }
        }
        private void OnDestroy() { if (_ownedPanel != null) Destroy(_ownedPanel); }
        private void OnDisable() { _pad?.ResetInput(); }
    }
}
