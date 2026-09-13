using System;
using System.Collections.Generic;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    // Arrival UI follows Map01A reference 01/05/10; reuses shared touch and viewport policies.
    public sealed partial class CongDongLamArrivalHud : MonoBehaviour
    {
        private CongDongLamMap01AArtPreview _scene;
        private VisualElement _root, _safe, _dialogue, _inventory, _combatBar, _questItemActions, _productShortcutActions, _questTabs;
        private Label _quest, _marker, _dialogueSpeaker, _dialogueQuestContext, _dialogueLine, _minimap, _inventorySummary, _equipmentTitle, _equipmentDetail;
        private Button _talk, _outfit, _level, _gender, _slot, _itemLevel, _toggleSlot, _run, _jump, _basic, _skill;
        private Button _inventoryToggle, _characterSelectButton, _healthPotion, _manaPotion, _equipReward, _equipmentToggle, _equipmentVariant, _equipmentClass;
        private Button _skillsShortcut, _menuShortcut, _questMissionsTab, _questPartyTab;
        private Button _dialogueContinue, _dialogueInformation, _dialogueClose, _npcTalk;
        private Button[] _equipmentRows;
        private IReadOnlyList<string> _equipmentSlotIds;
        private RuntimeTouchMovementPad _pad;
        private bool _touchJumpHeld;
        private VisualElement _vitals;
        private Label _vitalsName;
        private UnityEngine.UIElements.ProgressBar _health, _mana;
        private Button _inventoryGender;
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
            RuntimeUiTypography.ApplyBodyFont(_root);
            _root.pickingMode = PickingMode.Ignore;
            _safe = new VisualElement { name = "Map01A Safe Hud", pickingMode = PickingMode.Ignore };
            _root.Add(_safe);
            var title = new Label("CỔNG ĐÔNG LÂM\nKhu an toàn • Lv1–3");
            Box(title); Place(title, 12, null, 12, null); _safe.Add(title);
            _vitals = new VisualElement { name = "Map01A Vitals", pickingMode = PickingMode.Ignore };
            Box(_vitals); Place(_vitals, 12, null, 92, null); _vitals.style.width = 240;
            _vitalsName = new Label(); _vitalsName.style.fontSize = 18; _vitals.Add(_vitalsName);
            _health = MakeVital("Map01A Health", new Color(.67f, .16f, .15f));
            _mana = MakeVital("Map01A Mana", new Color(.12f, .37f, .64f));
            _vitals.Add(_health); _vitals.Add(_mana); _safe.Add(_vitals);
            _questTabs = new VisualElement { name = "Map01A Quest Tracker Tabs", pickingMode = PickingMode.Ignore };
            Place(_questTabs, null, 12, 12, null);
            _questTabs.style.width = 260;
            _questTabs.style.flexDirection = FlexDirection.Row;
            _questTabs.style.height = 32;
            _questMissionsTab = new Button { name = "Map01A Quest Tab Missions", text = "Nhiệm vụ" };
            _questPartyTab = new Button { name = "Map01A Quest Tab Party", text = "Đội" };
            foreach (var tab in new[] { _questMissionsTab, _questPartyTab })
            {
                tab.style.position = Position.Relative;
                tab.style.left = tab.style.right = tab.style.top = tab.style.bottom = StyleKeyword.Auto;
                tab.style.flexGrow = 1;
                tab.style.flexBasis = 0;
                tab.style.minHeight = 30;
                tab.style.fontSize = 12;
                tab.style.marginRight = tab == _questMissionsTab ? 4 : 0;
                tab.style.whiteSpace = WhiteSpace.NoWrap;
                _questTabs.Add(tab);
            }
            ApplyLgoButton(_questMissionsTab);
            ApplyLgoDisabledAction(_questPartyTab);
            foreach (var tab in new[] { _questMissionsTab, _questPartyTab })
            {
                tab.style.minHeight = 30;
                tab.style.fontSize = 12;
                tab.style.paddingLeft = 8;
                tab.style.paddingRight = 8;
                tab.style.whiteSpace = WhiteSpace.NoWrap;
            }
            _safe.Add(_questTabs);
            _quest = new Label { name = "Map01A Quest Tracker Body" }; Box(_quest); Place(_quest, null, 12, 46, null);
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
            ApplyLgoButton(_talk); Place(_talk, null, 16, null, 24); _talk.style.minHeight = _touch ? 64 : 48; _talk.style.minWidth = 170; _safe.Add(_talk);
            _talk.style.whiteSpace = WhiteSpace.Normal;
            _npcTalk = new Button(() => _scene.UseNpcConversation()) { text = "Nói chuyện với Tiểu Đồng" };
            ApplyLgoButton(_npcTalk); Place(_npcTalk, null, 16, null, 100); _npcTalk.style.minHeight = 48; _safe.Add(_npcTalk);
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
                ApplyLgoButton(button);
                button.style.minHeight = _touch ? 64 : 48;
                button.style.minWidth = _touch ? 112 : 108;
                button.style.marginRight = 6;
                _combatBar.Add(button);
            }
            _safe.Add(_combatBar);
            _characterSelectButton = new Button(OpenCharacterSelect) { name = "Map01A Character Select Button", text = "Nhân vật" };
            ApplyLgoButton(_characterSelectButton);
            _characterSelectButton.style.minHeight = _touch ? 64 : 48; _characterSelectButton.style.minWidth = 150;
            _inventoryToggle = new Button(() => _scene.ToggleInventory()) { text = "Hành trang · I" };
            ApplyLgoButton(_inventoryToggle); Place(_inventoryToggle, null, _touch ? 408 : 410, null, 24);
            _inventoryToggle.style.minHeight = _touch ? 64 : 48; _inventoryToggle.style.minWidth = 180;
            var actionBar = new VisualElement { name = "Map01A Context Actions", pickingMode = PickingMode.Ignore };
            Place(actionBar, null, 16, null, 24); actionBar.style.flexDirection = FlexDirection.Row;
            actionBar.style.alignItems = Align.FlexEnd;
            foreach (var button in new[] { _characterSelectButton, _inventoryToggle, _talk })
            {
                button.style.position = Position.Relative;
                button.style.left = button.style.right = button.style.top = button.style.bottom = StyleKeyword.Auto;
                button.style.maxWidth = 260; button.style.whiteSpace = WhiteSpace.Normal;
                actionBar.Add(button);
            }
            _characterSelectButton.style.marginRight = 12;
            _inventoryToggle.style.marginRight = 12; _safe.Add(actionBar);
            _productShortcutActions = new VisualElement { name = "Map01A Product Shortcut Actions", pickingMode = PickingMode.Ignore };
            Place(_productShortcutActions, null, 16, null, _touch ? 162 : 92);
            _productShortcutActions.style.flexDirection = FlexDirection.Row;
            _productShortcutActions.style.alignItems = Align.FlexEnd;
            _skillsShortcut = new Button { name = "Map01A Skills Shortcut", text = "Kỹ năng · chưa mở" };
            _menuShortcut = new Button { name = "Map01A Menu Shortcut", text = "Menu · chưa mở" };
            foreach (var button in new[] { _skillsShortcut, _menuShortcut })
            {
                ApplyLgoDisabledAction(button);
                button.style.position = Position.Relative;
                button.style.left = button.style.right = button.style.top = button.style.bottom = StyleKeyword.Auto;
                button.style.minHeight = _touch ? 58 : 44;
                button.style.minWidth = _touch ? 138 : 124;
                button.style.maxWidth = 156;
                button.style.marginLeft = 10;
                _productShortcutActions.Add(button);
            }
            _safe.Add(_productShortcutActions);
            BuildInventory();
            BuildCharacterSelect();
            BuildEntryScreen();
            _dialogue = new VisualElement { name = "Map01A Dialogue Panel" }; ApplyLgoGlassPanel(_dialogue); Place(_dialogue, 142, 290, null, 20);
            _dialogue.style.paddingLeft = _dialogue.style.paddingRight = 14;
            _dialogue.style.paddingTop = _dialogue.style.paddingBottom = 10;
            _dialogue.style.fontSize = 20;
            _dialogue.style.flexDirection = FlexDirection.Column;
            var dialogueHeader = new VisualElement { name = "Map01A Dialogue Header" };
            ApplyLgoGlassPanel(dialogueHeader, true);
            dialogueHeader.style.paddingLeft = dialogueHeader.style.paddingRight = 10;
            dialogueHeader.style.paddingTop = dialogueHeader.style.paddingBottom = 8;
            _dialogueSpeaker = LgoLabel("Hạ Vân", 20, UiGold, true);
            dialogueHeader.Add(_dialogueSpeaker);
            _dialogueQuestContext = LgoLabel("", 14, new Color(.72f, .86f, .92f, .94f));
            _dialogueQuestContext.name = "Map01A Dialogue Quest Context";
            _dialogueQuestContext.style.whiteSpace = WhiteSpace.Normal;
            _dialogueQuestContext.style.marginTop = 4;
            dialogueHeader.Add(_dialogueQuestContext);
            _dialogue.Add(dialogueHeader);
            var dialogueBody = new VisualElement { name = "Map01A Dialogue Body" };
            ApplyLgoDetailCard(dialogueBody);
            dialogueBody.style.paddingLeft = dialogueBody.style.paddingRight = 12;
            dialogueBody.style.paddingTop = dialogueBody.style.paddingBottom = 10;
            dialogueBody.style.marginTop = 8;
            _dialogueLine = LgoLabel(_scene.DialogueText, 20, new Color(.95f, .91f, .78f, .98f));
            _dialogueLine.style.whiteSpace = WhiteSpace.Normal; dialogueBody.Add(_dialogueLine);
            _dialogue.Add(dialogueBody);
            var dialogueOptions = new VisualElement { name = "Map01A Dialogue Actions" }; dialogueOptions.style.flexDirection = FlexDirection.Row;
            dialogueOptions.style.flexWrap = Wrap.Wrap;
            dialogueOptions.style.marginTop = 8;
            _dialogueContinue = new Button(() => _scene.UseCurrentRouteAction()) { name = "Map01A Dialogue Continue", text = "Tiếp tục" };
            _dialogueInformation = new Button(() => _scene.ReadDialogueInformation()) { name = "Map01A Dialogue Information", text = "Hỏi việc tiếp theo" };
            _dialogueClose = new Button(() => _scene.CloseNpcDialogue()) { name = "Map01A Dialogue Close", text = "Để sau" };
            ApplyLgoButton(_dialogueContinue, true); _dialogueContinue.style.marginRight = 10; _dialogueContinue.style.flexGrow = 1;
            foreach (var option in new[] { _dialogueInformation, _dialogueClose })
            {
                ApplyLgoButton(option); option.style.minHeight = 44; option.style.marginRight = 10;
            }
            foreach (var option in new[] { _dialogueContinue, _dialogueInformation, _dialogueClose }) dialogueOptions.Add(option);
            _dialogue.Add(dialogueOptions);
            _safe.Add(_dialogue);
            _marker = new Label("!\nHạ Vân") { pickingMode = PickingMode.Ignore };
            _marker.style.position = Position.Absolute; _marker.style.color = new Color(1,.83f,.3f);
            _marker.style.width = 240; _marker.style.height = 64;
            _marker.style.whiteSpace = WhiteSpace.Normal;
            _marker.style.fontSize = 18; _marker.style.unityTextAlign = TextAnchor.MiddleCenter;
            _root.Insert(0, _marker);
            _root.RegisterCallback<GeometryChangedEvent>(_ => Layout());
            Layout();
        }
        private static UnityEngine.UIElements.ProgressBar MakeVital(string name, Color color)
        {
            var bar = new UnityEngine.UIElements.ProgressBar { name = name, lowValue = 0, highValue = 100 };
            bar.style.height = 22; bar.style.marginTop = 4; bar.style.fontSize = 15;
            bar.Q(className: "unity-progress-bar__progress").style.backgroundColor = color;
            bar.Q(className: "unity-progress-bar__background").style.backgroundColor = new Color(.03f,.05f,.07f);
            return bar;
        }

        private void Layout()
        {
            _metrics = RuntimeViewportMetrics.FromRoot(_root);
            var r = _metrics.SafePanelRect;
            Place(_safe, r.x, null, r.y, null); _safe.style.width = r.width; _safe.style.height = r.height;
            _quest.style.width = r.width < 900 ? 220 : 260;
            _questTabs.style.width = r.width < 900 ? 220 : 260;
            _minimap.style.left = r.width < 1100 ? 206 : 220;
            _minimap.style.width = r.width < 1100 ? 360 : 430;
            _dialogue.style.left = _touch ? 150 : 20;
            _combatBar.style.left = _touch ? 150 : 220;
            _inventory.style.left = r.width < 950 ? 14 : 56;
            _inventory.style.right = r.width < 950 ? 14 : 56;
            _inventory.style.top = r.width < 950 ? 70 : 82;
            _inventory.style.bottom = r.width < 950 ? 96 : 70;
            _inventory.style.width = StyleKeyword.Auto;
            _combatBar.style.bottom = r.width < 1300 ? 100 : 24;
            _talk.style.fontSize = _touch ? 20 : 18;
            if (_inventoryHeroPanel != null)
            {
                var stacked = r.width < 950;
                var body = _inventory.Q("Map01A Inventory Body");
                body.style.flexDirection = stacked ? FlexDirection.Column : FlexDirection.Row;
                _inventoryHeroPanel.style.flexBasis = stacked ? StyleKeyword.Auto : StyleKeyword.Auto;
                _inventoryHeroPanel.style.flexGrow = _characterInfoOpen ? 1 : 0;
                _inventoryHeroPanel.style.marginRight = 0;
                _inventoryHeroPanel.style.marginBottom = stacked ? 10 : 0;
                _inventoryGridPanel.style.flexGrow = _characterInfoOpen || _storageOpen ? 0 : 1;
                _inventoryGridPanel.style.marginRight = 0;
                _inventoryGridPanel.style.marginBottom = stacked ? 10 : 0;
                _storagePanel.style.flexGrow = _storageOpen ? 1 : 0;
                _storagePanel.style.marginRight = 0;
                _storagePanel.style.marginBottom = stacked ? 10 : 0;
                _inventoryDetailPanel.style.flexBasis = stacked ? StyleKeyword.Auto : 300;
                _inventoryDetailPanel.style.marginLeft = stacked ? 0 : 10;
                _inventoryDetailPanel.style.marginRight = 0;
                _inventoryDetailPanel.style.marginBottom = stacked ? 10 : 0;
            }
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
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (_characterSelectOpen) CloseCharacterSelect();
                    else if (_scene.DialogueOpen) _scene.CloseNpcDialogue();
                    else if (_scene.InventoryOpen) _scene.ToggleInventory();
                }
                if (!_scene.DialogueOpen && !_characterSelectOpen)
                {
                    if (!_scene.IsSourcePoseReviewActive && Input.GetKeyDown(KeyCode.C)) _scene.CycleVoAvatarMode();
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
                    if (Input.GetKeyDown(KeyCode.F)) _scene.CycleSourcePoseClass();
                }
                else _scene.SetVoJumpHeld(false);
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
            _minimap.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _characterSelectButton.text = "Nhân vật" + (_touch ? "" : " · P");
            _inventoryToggle.text = (_scene.InventoryOpen ? "Đóng hành trang" : "Hành trang") + (_touch ? "" : " · I");
            _inventory.style.display = _scene.InventoryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _quest.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _questTabs.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            foreach (var control in new[] { _outfit, _level, _gender, _slot, _itemLevel, _toggleSlot })
                control.style.display = DisplayStyle.None;
            _inventorySummary.text = _scene.InventorySummaryText;
            _inventoryHeroTitle.text = _scene.ActiveEquipmentClassLabel + " · " + (_scene.VoAvatarGender == "female" ? "Nữ" : "Nam");
            _inventoryHeroMeta.text = "LC review local · " + _scene.EquipmentFitSummary
                + "\nHP " + _scene.PlayerHealth + "/100  ·  MP " + _scene.PlayerMana + "/100";
            _equipmentTitle.text = "TRANG BỊ · " + _scene.VoEquippedSlotCount + "/10 món đang mặc";
            if (_suppliesEmptyState != null)
                _suppliesEmptyState.text = _scene.HealthPotionCount <= 0 && _scene.ManaPotionCount <= 0 && !_scene.HasClassRewardItem
                    ? "Chưa nhận vật phẩm nhiệm vụ. Hoàn thành Q04 để nhận bình máu, bình linh lực và hộ uyển tân thủ."
                    : "Chọn vật phẩm để dùng hoặc trang bị. Vật phẩm chưa đủ điều kiện sẽ tạm khóa nhưng vẫn đọc rõ trạng thái.";
            _questItemActions.style.display = DisplayStyle.Flex;
            RefreshInventoryEquipmentTiles();
            RefreshInventoryDetailCard();
            var selectedSlot = _scene.VoSelectedEquipmentSlot;
            var selectedEquipped = _scene.IsVoEquipmentSlotEquipped(selectedSlot);
            var equipToggleText = selectedEquipped ? "Tháo món đang chọn" : "Mặc món đang chọn";
            if (!_suppliesOpen)
            {
                _inventoryDetailPrimaryAction.text = equipToggleText;
                _equipmentToggle.text = equipToggleText;
                var hasVariant = _scene.HasVoEquipmentItemVariant(_scene.VoSelectedEquipmentSlot);
                _equipmentVariant.text = hasVariant ? "Đổi cấp món" : "";
                _equipmentVariant.style.display = hasVariant ? DisplayStyle.Flex : DisplayStyle.None;
                _equipmentVariant.SetEnabled(hasVariant);
            }
            _equipmentClass.style.display = _scene.IsSourcePoseReviewActive ? DisplayStyle.Flex : DisplayStyle.None;
            _equipmentClass.text = _scene.CanCycleSourcePoseClass
                ? "Đổi class: " + _scene.ActiveEquipmentClassLabel + (_touch ? "" : " · F")
                : "Class: " + _scene.ActiveEquipmentClassLabel;
            _equipmentClass.SetEnabled(_scene.CanCycleSourcePoseClass);
            _inventoryGender.style.display = _scene.IsSourcePoseReviewActive ? DisplayStyle.Flex : DisplayStyle.None;
            _inventoryGender.text = (_scene.CanCycleSourcePoseGender ? "Đổi giới · " : "Hiện có · ")
                + (_scene.VoAvatarGender == "female" ? "Nữ" : "Nam") + (_touch ? "" : " · G");
            _inventoryGender.SetEnabled(_scene.CanCycleSourcePoseGender);
            _vitals.style.display = _scene.InventoryOpen || _scene.DialogueOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _vitalsName.text = _scene.ActiveEquipmentClassLabel + " · " + (_scene.VoAvatarGender == "female" ? "Nữ" : "Nam");
            _health.value = _scene.PlayerHealth; _health.title = "HP " + _scene.PlayerHealth + "/100";
            _mana.value = _scene.PlayerMana; _mana.title = "MP " + _scene.PlayerMana + "/100";
            foreach (var supplyAction in new[] { _healthPotion, _manaPotion, _equipReward })
                supplyAction.SetEnabled(true);
            RefreshInventorySupplyRows();
            _dialogue.style.display = _scene.DialogueOpen ? DisplayStyle.Flex : DisplayStyle.None;
            var hudBlocked = _scene.DialogueOpen || _scene.InventoryOpen || _characterSelectOpen;
            _characterSelectButton.style.display = hudBlocked ? DisplayStyle.None : DisplayStyle.Flex;
            _inventoryToggle.style.display = hudBlocked ? DisplayStyle.None : DisplayStyle.Flex;
            _talk.style.display = hudBlocked ? DisplayStyle.None : DisplayStyle.Flex;
            _productShortcutActions.style.display = hudBlocked ? DisplayStyle.None : DisplayStyle.Flex;
            _combatBar.style.display = hudBlocked ? DisplayStyle.None : DisplayStyle.Flex;
            _pad.style.display = _touch && !hudBlocked ? DisplayStyle.Flex : DisplayStyle.None;
            _dialogueContinue.style.display = _scene.DialogueOpen ? DisplayStyle.Flex : DisplayStyle.None;
            _dialogueContinue.SetEnabled(_scene.CanUseCurrentRouteAction);
            _dialogueContinue.text = "Tiếp tục" + (_touch ? "" : " · E");
            _dialogueInformation.style.display = _scene.CanReadDialogueInformation ? DisplayStyle.Flex : DisplayStyle.None;
            _npcTalk.style.display = !_scene.DialogueOpen && _scene.CanTalkToCurrentNpc
                && _scene.CurrentRouteNodeId == "well-bridge" && _scene.CurrentActionLabel != "Trò chuyện"
                ? DisplayStyle.Flex : DisplayStyle.None;
            _talk.style.width = _scene.DialogueOpen ? new StyleLength(260) : new StyleLength(StyleKeyword.Auto);
            _dialogueSpeaker.text = _scene.DialogueSpeaker + " · " + _scene.DialogueProgress;
            _dialogueQuestContext.text = DialogueQuestContextText();
            _dialogueLine.text = _scene.DialogueText;
            _marker.text = "!\n" + _scene.CurrentRouteNodeLabel;
            _marker.style.display = _scene.DialogueOpen || _scene.InventoryOpen || _characterSelectOpen ? DisplayStyle.None : DisplayStyle.Flex;
            UpdateHudShellVisibility();
            var camera = Camera.main;
            if (camera != null)
            {
                var v = camera.WorldToViewportPoint(_scene.CurrentInteractionPosition);
                _marker.style.left = v.x * _metrics.PanelWidth - 120;
                _marker.style.top = (1-v.y) * _metrics.PanelHeight - 64;
            }
        }

        private string DialogueQuestContextText()
        {
            var tracker = _scene.QuestTrackerText ?? "";
            var firstLineEnd = tracker.IndexOf('\n');
            var questLine = firstLineEnd >= 0 ? tracker.Substring(0, firstLineEnd) : tracker;
            if (string.IsNullOrEmpty(questLine)) questLine = _scene.ActiveQuestId;
            return questLine + " · " + _scene.DialogueProgress;
        }

        private string EquipmentDisplayName(string slot)
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
                case "accessory": return "Phụ kiện " + _scene.ActiveEquipmentClassLabel;
                default: return slot;
            }
        }
        private string EquipmentShortName(string slot)
        {
            switch (slot)
            {
                case "main_weapon": return "Vũ khí";
                case "head_hair": return "Đầu tóc";
                case "light_armor": return "Giáp";
                case "accessory": return "Phụ kiện";
                default: return EquipmentDisplayName(slot);
            }
        }
        private void OnDestroy() { if (_ownedPanel != null) Destroy(_ownedPanel); }
        private void OnDisable() { _pad?.ResetInput(); }
    }
}
