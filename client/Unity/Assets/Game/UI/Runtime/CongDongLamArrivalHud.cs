using System;
using System.Collections.Generic;
using System.Threading;
using LinhGioi.Account;
using LinhGioi.Foundation;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    // Arrival UI follows Map01A reference 01/05/10; reuses shared touch and viewport policies.
    public sealed partial class CongDongLamArrivalHud : MonoBehaviour
    {
        // Approved five-tab shell: one balanced workspace plus one readable inspector.
        // Keep these widths centralized because every character-hub tab shares them.
        private const float InventoryDesktopMainColumnWidth = 600f;
        private const float InventoryDesktopDetailColumnWidth = 448f;
        private const float InventoryDesktopColumnGap = 12f;
        private const float InventoryCanonicalShellWidth = 1098f;
        private const float InventoryCanonicalShellHeight = 724f;
        private const float InventoryCanonicalBodyHorizontalInset = 38f;
        private const float InventoryGridCellBasisPercent = 18.2f;

        private CongDongLamMap01AArtPreview _scene;
        private VisualElement _root, _safe, _dialogue, _inventory, _combatBar, _quest, _questItemActions, _productShortcutActions, _questTabs, _vitalsPortrait, _dialoguePortrait;
        private VisualElement _playerHudCluster, _rightHudCluster, _minimap, _minimapCurrentMarker;
        private Label _questCategory, _questTitle, _questObjective, _questProgress, _questMessage;
        private Label _marker, _dialogueSpeaker, _dialogueQuestContext, _dialogueLine, _minimapTitle, _minimapStatus, _inventorySummary, _equipmentTitle, _equipmentDetail;
        private Button _talk, _outfit, _level, _gender, _slot, _itemLevel, _toggleSlot, _run, _jump, _basic, _skill;
        private Button _healthPotion, _manaPotion, _equipReward, _equipmentToggle, _equipmentVariant, _equipmentClass;
        private Button _menuShortcut, _questMissionsTab, _questPartyTab;
        private Button _dialogueContinue, _dialogueInformation, _dialogueClose, _npcTalk;
        private Button[] _equipmentRows;
        private IReadOnlyList<string> _equipmentSlotIds;
        private RuntimeTouchMovementPad _pad;
        private bool _touchJumpHeld;
        private VisualElement _vitals;
        private Label _vitalsName, _vitalsMeta;
        private UnityEngine.UIElements.ProgressBar _health, _mana;
        private Button _inventoryGender;
        private RuntimeViewportMetrics _metrics;
        private PanelSettings _ownedPanel;
        private bool _touch;
        private string _forcedLayoutProfile;
        private bool _reviewHotkeysEnabled;
        private IProductAuthClient _productAuthClient;
        private IProductAccountClient _productAccountClient;
        private IProductCharacterClient _productCharacterClient;
        private ProductAuthSessionState _productAuthSession;
        private ProductAccountRecoveryState _productRecoveryState;
        private CancellationTokenSource _productAuthCts;
        private bool _ownsProductAuthClient;
        private bool _productLoginInFlight;
        private bool _productRegisterInFlight;
        private bool _productRecoveryInFlight;
        private string _loadedProductCharacterName;

        public static void Attach(CongDongLamMap01AArtPreview scene,
            IProductAuthClient productAuthClient = null, ProductAuthSessionState authSession = null)
        {
            var host = new GameObject("Map01A Arrival HUD");
            host.transform.SetParent(scene.transform, false);
            var hud = host.AddComponent<CongDongLamArrivalHud>();
            hud._scene = scene;
            hud._productAuthClient = productAuthClient;
            hud._productAccountClient = productAuthClient as IProductAccountClient;
            hud._productCharacterClient = productAuthClient as IProductCharacterClient;
            hud._productAuthSession = authSession ?? new ProductAuthSessionState();
            hud._productRecoveryState = new ProductAccountRecoveryState();
            hud._productAuthCts = new CancellationTokenSource();
            var document = host.AddComponent<UIDocument>();
            hud._ownedPanel = Instantiate(RuntimePanelSettingsProvider.LoadOrCreate());
            document.panelSettings = hud._ownedPanel;
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "--lgo-map01a-device");
            var requestedProfile = index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
            hud._forcedLayoutProfile = requestedProfile == "pc" ? "desktop"
                : !string.IsNullOrWhiteSpace(requestedProfile) ? requestedProfile
                : Application.isMobilePlatform ? "mobile" : null;
            hud._touch = hud._forcedLayoutProfile == "mobile" || hud._forcedLayoutProfile == "tablet";
            hud._reviewHotkeysEnabled = ShouldEnableReviewHotkeysForArgs(args);
            hud.Build(document.rootVisualElement);
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
            RuntimeUiStyleSheetProvider.Attach(_root);
            RuntimeUiTypography.ApplyBodyFont(_root);
            _root.pickingMode = PickingMode.Ignore;
            _safe = new VisualElement { name = "Map01A Safe Hud", pickingMode = PickingMode.Ignore };
            _root.Add(_safe);

            _playerHudCluster = new VisualElement { name = "Map01A Player Status Cluster", pickingMode = PickingMode.Ignore };
            ApplyLgoGameplayPlayerZone(_playerHudCluster);
            _playerHudCluster.style.width = 292;
            _safe.Add(_playerHudCluster);

            _rightHudCluster = new VisualElement { name = "Map01A Right Hud Cluster", pickingMode = PickingMode.Ignore };
            ApplyLgoGameplayRightZone(_rightHudCluster);
            _rightHudCluster.style.width = 286;
            _safe.Add(_rightHudCluster);

            var title = new Label("CỘNG ĐỒNG LÂM  ·  KÊNH 1\nKhu an toàn  •  Lv1–3") { name = "Map01A Location Title" };
            ApplyLgoHudLocationChip(title); title.style.width = 286; title.style.marginBottom = 4; _rightHudCluster.Add(title);
            _vitals = new VisualElement { name = "Map01A Vitals", pickingMode = PickingMode.Ignore };
            ApplyLgoHudPlayerCard(_vitals); _vitals.style.width = 292;
            _vitalsPortrait = new VisualElement { name = "Map01A Player Portrait", pickingMode = PickingMode.Ignore };
            ApplyLgoHudPortrait(_vitalsPortrait);
            _vitals.Add(_vitalsPortrait);
            var vitalsContent = new VisualElement { name = "Map01A Vitals Content", pickingMode = PickingMode.Ignore };
            vitalsContent.style.flexGrow = 1;
            vitalsContent.style.marginLeft = 10;
            _vitalsName = new Label { name = "Map01A Player Name" };
            _vitalsName.style.fontSize = 15;
            _vitalsName.style.unityFontStyleAndWeight = FontStyle.Bold;
            _vitalsName.style.color = new Color(.96f, .91f, .76f, .98f);
            vitalsContent.Add(_vitalsName);
            _vitalsMeta = new Label { name = "Map01A Player Class Meta" };
            _vitalsMeta.style.fontSize = 11;
            _vitalsMeta.style.color = UiSubText;
            _vitalsMeta.style.marginTop = -1;
            vitalsContent.Add(_vitalsMeta);
            _health = MakeVital("Map01A Health", new Color(.67f, .16f, .15f));
            _mana = MakeVital("Map01A Mana", new Color(.12f, .37f, .64f));
            vitalsContent.Add(_health); vitalsContent.Add(_mana); _vitals.Add(vitalsContent); _playerHudCluster.Add(_vitals);
            _questTabs = new VisualElement { name = "Map01A Quest Tracker Tabs", pickingMode = PickingMode.Ignore };
            _questTabs.style.width = 286;
            _questTabs.style.flexDirection = FlexDirection.Row;
            _questTabs.style.height = 32;
            _questTabs.style.marginBottom = 4;
            _questMissionsTab = new Button { name = "Map01A Quest Tab Missions", text = "Nhiệm vụ" };
            _questPartyTab = new Button { name = "Map01A Quest Tab Party", text = "Đội" };
            ApplyLgoHudQuestTab(_questMissionsTab, selected: true, enabled: true, isLast: false);
            ApplyLgoHudQuestTab(_questPartyTab, selected: false, enabled: false, isLast: true);
            _questTabs.Add(_questMissionsTab);
            _questTabs.Add(_questPartyTab);
            _quest = new VisualElement { name = "Map01A Quest Tracker Body", pickingMode = PickingMode.Ignore };
            ApplyLgoHudQuestPanel(_quest);
            _quest.style.width = 286;
            _questCategory = LgoLabel("NHIỆM VỤ CHÍNH", 11, new Color(.45f, .90f, 1f, .96f), true);
            _questCategory.name = "Map01A Quest Category";
            _questTitle = LgoLabel("", 15, UiGold, true);
            _questTitle.name = "Map01A Quest Title";
            _questTitle.style.marginTop = 3;
            _questObjective = LgoLabel("", 13, UiText);
            _questObjective.name = "Map01A Quest Objective";
            _questObjective.style.whiteSpace = WhiteSpace.Normal;
            _questObjective.style.marginTop = 3;
            _questProgress = LgoLabel("", 12, new Color(.76f, 1f, .70f, .94f), true);
            _questProgress.name = "Map01A Quest Progress";
            _questProgress.style.marginTop = 4;
            _questMessage = LgoLabel("", 11, UiSubText);
            _questMessage.name = "Map01A Quest Interaction Message";
            _questMessage.style.whiteSpace = WhiteSpace.Normal;
            _questMessage.style.marginTop = 4;
            _quest.Add(_questCategory);
            _quest.Add(_questTitle);
            _quest.Add(_questObjective);
            _quest.Add(_questProgress);
            _quest.Add(_questMessage);
            _minimap = new VisualElement { name = "Map01A Minimap", pickingMode = PickingMode.Ignore }; ApplyLgoHudMapPanel(_minimap);
            _minimap.style.width = 286; _minimap.style.height = 104; _minimap.style.fontSize = 12; _minimap.style.marginBottom = 4;
            _minimapTitle = LgoLabel("BẢN ĐỒ KHU VỰC", 12, UiGold, true);
            _minimapTitle.name = "Map01A Minimap Title";
            _minimapTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
            _minimap.Add(_minimapTitle);
            var minimapRoute = new VisualElement { name = "Map01A Minimap Route", pickingMode = PickingMode.Ignore };
            minimapRoute.style.height = 42;
            minimapRoute.style.marginTop = 3;
            minimapRoute.style.position = Position.Relative;
            var routeLine = new VisualElement { name = "Map01A Minimap Route Line", pickingMode = PickingMode.Ignore };
            ApplyLgoHudMapRouteLine(routeLine);
            minimapRoute.Add(routeLine);
            var routeTrack = new VisualElement { name = "Map01A Minimap Route Track", pickingMode = PickingMode.Ignore };
            routeTrack.style.flexDirection = FlexDirection.Row;
            routeTrack.style.justifyContent = Justify.SpaceBetween;
            routeTrack.style.alignItems = Align.Center;
            routeTrack.style.height = 42;
            routeTrack.style.paddingLeft = routeTrack.style.paddingRight = 6;
            foreach (var label in new[] { "Hạ Vân", "Cổng", "Làng", "Rìa", "Suối" })
            {
                var routeNode = new VisualElement { name = "Map01A Minimap Node " + label, pickingMode = PickingMode.Ignore };
                ApplyLgoHudMapRouteNode(routeNode);
                var dot = new VisualElement { name = "Map01A Minimap Dot " + label, pickingMode = PickingMode.Ignore };
                ApplyLgoHudMapRouteDot(dot);
                var nodeLabel = LgoLabel(label, 9, UiSubText, true);
                nodeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                routeNode.Add(dot);
                routeNode.Add(nodeLabel);
                routeTrack.Add(routeNode);
            }
            minimapRoute.Add(routeTrack);
            _minimapCurrentMarker = new VisualElement { name = "Map01A Minimap Current Marker", pickingMode = PickingMode.Ignore };
            ApplyLgoHudMapCurrentMarker(_minimapCurrentMarker);
            minimapRoute.Add(_minimapCurrentMarker);
            _minimap.Add(minimapRoute);
            _minimapStatus = LgoLabel("", 10, UiSubText, true);
            _minimapStatus.name = "Map01A Minimap Status";
            _minimapStatus.style.unityTextAlign = TextAnchor.MiddleCenter;
            _minimap.Add(_minimapStatus);
            _rightHudCluster.Add(_minimap);
            _rightHudCluster.Add(_questTabs);
            _rightHudCluster.Add(_quest);
            _pad = new RuntimeTouchMovementPad { name = "LGO World Touch Movement Pad" }; ApplyLgoHudInfoPanel(_pad); ApplyLgoGameplayTouchZone(_pad);
            _pad.style.width = _pad.style.height = 112;
            _pad.style.display = _touch ? DisplayStyle.Flex : DisplayStyle.None;
            var nub = new VisualElement { name = "LGO World Touch Movement Nub", pickingMode = PickingMode.Ignore };
            RuntimeTouchMovementPad.ApplyCircularPresentation(
                _pad,
                nub,
                new Color(.016f, .052f, .092f, .84f),
                new Color(.10f, .45f, .70f, .96f),
                UiGoldBorder);
            _pad.Add(nub); _safe.Add(_pad);
            _talk = new Button(() => _scene.UseCurrentRouteAction()) { name = "Map01A Talk Action", text = "Tương tác · E" };
            ApplyLgoHudContextAction(_talk, _touch, minWidth: 150); ApplyLgoGameplayContextZone(_talk); _safe.Add(_talk);
            _npcTalk = new Button(() => _scene.UseNpcConversation()) { text = "Nói chuyện với Tiểu Đồng" };
            ApplyLgoHudContextAction(_npcTalk, _touch, minHeight: 48); ApplyLgoGameplayContextZone(_npcTalk); _safe.Add(_npcTalk);
            _outfit = new Button(() => _scene.CycleCharacterAvatarMode()) { text = "Trang bị · C" };
            ApplyLgoHudInfoPanel(_outfit); Place(_outfit, 16, null, _touch ? 90 : 90, null);
            _outfit.style.minHeight = _touch ? 56 : 42; _outfit.style.minWidth = 170; _safe.Add(_outfit);
            _level = new Button(() => _scene.CycleCharacterLevel()) { text = "Cấp trang bị · L" };
            ApplyLgoHudInfoPanel(_level); Place(_level, 16, null, _touch ? 152 : 138, null);
            _level.style.minHeight = _touch ? 52 : 40; _level.style.minWidth = 170; _safe.Add(_level);
            _gender = new Button(() => _scene.CycleCharacterGender()) { text = "Nam/Nữ · G" };
            ApplyLgoHudInfoPanel(_gender); Place(_gender, 16, null, _touch ? 210 : 184, null);
            _gender.style.minHeight = _touch ? 52 : 40; _gender.style.minWidth = 170; _safe.Add(_gender);
            _slot = new Button(() => _scene.CycleEquipmentSlot()) { text = "Chọn slot · V" };
            ApplyLgoHudInfoPanel(_slot); Place(_slot, 16, null, _touch ? 268 : 230, null);
            _slot.style.minHeight = _touch ? 52 : 40; _slot.style.minWidth = 170; _safe.Add(_slot);
            _itemLevel = new Button(() => _scene.CycleSelectedEquipmentItemLevel()) { text = "Đổi cấp item · M" };
            ApplyLgoHudInfoPanel(_itemLevel); Place(_itemLevel, 16, null, _touch ? 326 : 276, null);
            _itemLevel.style.minHeight = _touch ? 52 : 40; _itemLevel.style.minWidth = 170; _safe.Add(_itemLevel);
            _toggleSlot = new Button(() => _scene.ToggleEquipmentSlot()) { text = "Mặc/Cởi · B" };
            ApplyLgoHudInfoPanel(_toggleSlot); Place(_toggleSlot, 16, null, _touch ? 384 : 322, null);
            _toggleSlot.style.minHeight = _touch ? 52 : 40; _toggleSlot.style.minWidth = 170; _safe.Add(_toggleSlot);
            _combatBar = new VisualElement { name = "Map01A Combat Actions" }; ApplyLgoGameplayCombatZone(_combatBar);
            _combatBar.style.flexDirection = FlexDirection.Row;
            _combatBar.style.width = 330;
            _combatBar.style.height = _touch ? 76 : 66;
            _combatBar.style.justifyContent = Justify.FlexEnd;
            _combatBar.style.alignItems = Align.Center;
            _run = new Button(() => _scene.SetCharacterRun(!_scene.CharacterRunEnabled)) { name = "Map01A Run Action", text = "Chạy" };
            _run.tooltip = "Chạy · Shift";
            _jump = new Button { name = "Map01A Jump Action", text = "Nhảy" };
            _jump.tooltip = "Nhảy · W";
            _jump.RegisterCallback<PointerDownEvent>(evt => { _touchJumpHeld = true; _jump.CapturePointer(evt.pointerId); _scene.SetCharacterJumpHeld(true); });
            _jump.RegisterCallback<PointerUpEvent>(evt => { _touchJumpHeld = false; _jump.ReleasePointer(evt.pointerId); _scene.SetCharacterJumpHeld(false); });
            _jump.RegisterCallback<PointerCaptureOutEvent>(evt => { _touchJumpHeld = false; _scene.SetCharacterJumpHeld(false); });
            _basic = new Button(() => _scene.TriggerCharacterBasicAttack()) { name = "Map01A Basic Attack Action", text = "Đánh" };
            _basic.tooltip = "Đánh thường · Z";
            _skill = new Button(() => _scene.TriggerCharacterSkill()) { name = "Map01A Skill Action", text = "Kỹ năng" };
            _skill.tooltip = "Kỹ năng · X";
            ApplyLgoHudCombatAction(_run, _touch);
            ApplyLgoHudCombatAction(_jump, _touch);
            ApplyLgoHudCombatAction(_basic, _touch, primary: true);
            ApplyLgoHudCombatAction(_skill, _touch);
            foreach (var button in new[] { _run, _jump, _basic, _skill }) _combatBar.Add(button);
            AttachLgoHudActionIcon(_run, _scene.GetMap01AHudIconSprite("run"), _touch);
            AttachLgoHudActionIcon(_jump, _scene.GetMap01AHudIconSprite("jump"), _touch);
            AttachLgoHudActionIcon(_basic, _scene.GetMap01AHudIconSprite("attack"), _touch);
            AttachLgoHudActionIcon(_skill, _scene.GetMap01AHudIconSprite("skill"), _touch);
            _safe.Add(_combatBar);
            _productShortcutActions = new VisualElement { name = "Map01A Product Shortcut Actions", pickingMode = PickingMode.Ignore };
            ApplyLgoGameplaySecondaryZone(_productShortcutActions);
            _menuShortcut = new Button(ToggleMenu) { name = "Map01A Menu Shortcut", text = "Menu" };
            _menuShortcut.tooltip = "Mở menu nhân vật và hệ thống";
            ApplyLgoHudShortcutAction(_menuShortcut, _touch, true);
            _productShortcutActions.Add(_menuShortcut);
            AttachLgoHudActionIcon(_menuShortcut, _scene.GetMap01AHudIconSprite("menu"), _touch);
            _safe.Add(_productShortcutActions);
            BuildInventory();
            BuildCharacterSelect();
            BuildEntryScreen();
            BuildServerSelect();
            BuildRegister();
            BuildPasswordRecovery();
            BuildMenu();
            _dialogue = new VisualElement { name = "Map01A Dialogue Panel" }; ApplyLgoGlassPanel(_dialogue); ApplyLgoLayeredFrame(_dialogue); Place(_dialogue, 28, null, null, 22);
            _dialogue.style.paddingLeft = _dialogue.style.paddingRight = 14;
            _dialogue.style.paddingTop = _dialogue.style.paddingBottom = 10;
            _dialogue.style.fontSize = 17;
            _dialogue.style.flexDirection = FlexDirection.Column;
            var dialogueLayout = new VisualElement { name = "Map01A Dialogue Content Layout", pickingMode = PickingMode.Ignore };
            dialogueLayout.style.flexDirection = FlexDirection.Row;
            dialogueLayout.style.alignItems = Align.FlexEnd;
            _dialoguePortrait = new VisualElement { name = "Map01A Dialogue NPC Portrait", pickingMode = PickingMode.Ignore };
            ApplyLgoDialoguePortrait(_dialoguePortrait);
            dialogueLayout.Add(_dialoguePortrait);
            var dialogueContent = new VisualElement { name = "Map01A Dialogue Text And Actions" };
            dialogueContent.style.flexGrow = 1;
            var dialogueHeader = new VisualElement { name = "Map01A Dialogue Header" };
            ApplyLgoGlassPanel(dialogueHeader, true);
            dialogueHeader.style.paddingLeft = dialogueHeader.style.paddingRight = 10;
            dialogueHeader.style.paddingTop = dialogueHeader.style.paddingBottom = 8;
            _dialogueSpeaker = LgoTitleLabel("Hạ Vân", 18);
            _dialogueSpeaker.name = "Map01A Dialogue Speaker";
            dialogueHeader.Add(_dialogueSpeaker);
            _dialogueQuestContext = LgoSubtitleLabel("", 12);
            _dialogueQuestContext.name = "Map01A Dialogue Quest Context";
            _dialogueQuestContext.style.whiteSpace = WhiteSpace.Normal;
            _dialogueQuestContext.style.marginTop = 4;
            dialogueHeader.Add(_dialogueQuestContext);
            dialogueContent.Add(dialogueHeader);
            var dialogueBody = new VisualElement { name = "Map01A Dialogue Body" };
            ApplyLgoDetailCard(dialogueBody, 12, 10);
            dialogueBody.style.marginTop = 8;
            _dialogueLine = LgoLabel(_scene.DialogueText, 17, new Color(.95f, .91f, .78f, .98f));
            _dialogueLine.name = "Map01A Dialogue Line";
            _dialogueLine.style.whiteSpace = WhiteSpace.Normal; dialogueBody.Add(_dialogueLine);
            dialogueContent.Add(dialogueBody);
            var dialogueOptions = new VisualElement { name = "Map01A Dialogue Actions" }; dialogueOptions.style.flexDirection = FlexDirection.Row;
            dialogueOptions.style.flexWrap = Wrap.Wrap;
            dialogueOptions.style.marginTop = 8;
            _dialogueContinue = new Button(() => _scene.UseCurrentRouteAction()) { name = "Map01A Dialogue Continue", text = "Tiếp tục" };
            _dialogueInformation = new Button(() => _scene.ReadDialogueInformation()) { name = "Map01A Dialogue Information", text = "Hỏi việc tiếp theo" };
            _dialogueClose = new Button(() => _scene.CloseNpcDialogue()) { name = "Map01A Dialogue Close", text = "Để sau" };
            ApplyLgoDialoguePrimaryAction(_dialogueContinue);
            foreach (var option in new[] { _dialogueInformation, _dialogueClose })
            {
                ApplyLgoDialogueSecondaryAction(option);
            }
            foreach (var option in new[] { _dialogueContinue, _dialogueInformation, _dialogueClose }) dialogueOptions.Add(option);
            dialogueContent.Add(dialogueOptions);
            dialogueLayout.Add(dialogueContent);
            _dialogue.Add(dialogueLayout);
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
            ApplyLgoVitalBar(bar, color);
            return bar;
        }

        public static Rect CalculateDialoguePanelRect(Rect safePanelRect, bool touch)
        {
            var side = touch ? 12f : 28f;
            var rightReserve = touch ? 12f : 220f;
            var maxWidth = touch ? 840f : 860f;
            var width = Mathf.Min(maxWidth, Mathf.Max(0f, safePanelRect.width - side - rightReserve));
            return new Rect(safePanelRect.x + side, safePanelRect.y + 22f, width, 0f);
        }

        public static float CalculateInventoryShellHeight(Rect inventoryRect, bool touch, bool compactShell)
        {
            return Mathf.Min(inventoryRect.height, InventoryCanonicalShellHeight);
        }

        public static string QuestInteractionMessageForDisplay(string message, string objective, string progress)
        {
            var normalized = (message ?? "").Trim();
            if (normalized.Length == 0) return "";
            if (string.Equals(normalized, (objective ?? "").Trim(), StringComparison.Ordinal)
                || string.Equals(normalized, (progress ?? "").Trim(), StringComparison.Ordinal)) return "";
            return normalized;
        }

        public static bool ShouldBlockWorldInput(bool entryOpen, bool menuOpen, bool inventoryOpen,
            bool dialogueOpen, bool characterSelectOpen, bool serverSelectOpen = false)
        {
            return entryOpen || menuOpen || inventoryOpen || dialogueOpen || characterSelectOpen || serverSelectOpen;
        }

        public static bool ShouldEnableReviewHotkeysForArgs(string[] args)
        {
            return args != null && Array.IndexOf(args, "--lgo-map01a-review-hotkeys") >= 0;
        }

        private void HandleEscape()
        {
            if (_passwordRecoveryOpen) ClosePasswordRecovery();
            else if (_registerOpen) CloseRegister();
            else if (_serverSelectOpen) CloseServerSelect(false);
            else if (_menuOpen) CloseMenu();
            else if (_characterSelectOpen) CloseCharacterSelect();
            else if (_scene.DialogueOpen) _scene.CloseNpcDialogue();
            else if (_scene.InventoryOpen) _scene.ToggleInventory();
        }

        public static Rect CalculateInventoryModalRect(Rect safePanelRect, bool touch)
        {
            const float minimumMargin = 24f;
            const float opticalVerticalOffset = 18f;
            var width = Mathf.Min(InventoryCanonicalShellWidth,
                Mathf.Max(0f, safePanelRect.width - minimumMargin * 2f));
            var height = Mathf.Min(InventoryCanonicalShellHeight,
                Mathf.Max(0f, safePanelRect.height - minimumMargin * 2f));
            var x = safePanelRect.x + (safePanelRect.width - width) * .5f;
            var centeredY = (safePanelRect.height - height) * .5f + opticalVerticalOffset;
            var y = safePanelRect.y + Mathf.Clamp(centeredY, minimumMargin,
                Mathf.Max(minimumMargin, safePanelRect.height - height - minimumMargin));
            return new Rect(x, y, width, height);
        }

        public static Vector2 CalculateInventoryDesktopColumnWidths()
        {
            return new Vector2(InventoryDesktopMainColumnWidth, InventoryDesktopDetailColumnWidth);
        }

        public static Vector3 CalculateInventoryColumnWidths(float shellWidth)
        {
            var available = Mathf.Max(0f, shellWidth - InventoryCanonicalBodyHorizontalInset);
            var canonical = InventoryDesktopMainColumnWidth + InventoryDesktopDetailColumnWidth + InventoryDesktopColumnGap;
            var scale = canonical <= 0f ? 0f : Mathf.Min(1f, available / canonical);
            return new Vector3(
                InventoryDesktopMainColumnWidth * scale,
                InventoryDesktopDetailColumnWidth * scale,
                InventoryDesktopColumnGap * scale);
        }

        private void Layout()
        {
            _metrics = RuntimeViewportMetrics.FromRoot(_root, _forcedLayoutProfile);
            var layout = RuntimeUiLayoutProfile.FromViewport(_metrics);
            var r = _metrics.SafePanelRect;
            Place(_safe, r.x, null, r.y, null); _safe.style.width = r.width; _safe.style.height = r.height;
            LayoutEntryScreen(layout, r);
            var gameplayHud = RuntimeGameplayHudLayout.Calculate(new Rect(0, 0, r.width, r.height), layout);
            ApplyLgoGameplayHudRect(_playerHudCluster, gameplayHud.PlayerStatus);
            ApplyLgoGameplayHudRect(_rightHudCluster, gameplayHud.RightInfo);
            ApplyLgoGameplayHudRect(_combatBar, gameplayHud.Combat);
            ApplyLgoGameplayHudRect(_talk, gameplayHud.Context);
            ApplyLgoGameplayHudRect(_npcTalk, gameplayHud.Context);
            ApplyLgoGameplayHudRect(_pad, gameplayHud.TouchPad);
            ApplyLgoGameplayHudRect(_productShortcutActions, gameplayHud.SecondaryNav);
            _vitals.style.width = gameplayHud.PlayerStatus.width;
            var rightColumnWidth = gameplayHud.RightInfo.width;
            _quest.style.width = rightColumnWidth;
            _questTabs.style.width = rightColumnWidth;
            _minimap.style.width = rightColumnWidth;
            var dialogueRect = CalculateDialoguePanelRect(new Rect(0, 0, r.width, r.height), _touch);
            _dialogue.style.left = dialogueRect.x;
            _dialogue.style.right = StyleKeyword.Auto;
            _dialogue.style.width = dialogueRect.width;
            var inventoryRect = CalculateInventoryModalRect(new Rect(0, 0, r.width, r.height), _touch);
            _inventory.style.left = inventoryRect.x;
            _inventory.style.right = StyleKeyword.Auto;
            _inventory.style.top = inventoryRect.y;
            _inventory.style.bottom = StyleKeyword.Auto;
            _inventory.style.width = inventoryRect.width;
            _inventory.style.height = CalculateInventoryShellHeight(inventoryRect, _touch, IsInventoryCompactShellActive());
            PublishRuntimeUiMetrics(layout, inventoryRect);
            _talk.style.fontSize = layout.WorldTalkFontSize;
            if (_inventoryHeroPanel != null)
            {
                var body = _inventory.Q("Map01A Inventory Body");
                var columns = CalculateInventoryColumnWidths(inventoryRect.width);
                body.style.flexDirection = FlexDirection.Row;
                _inventoryHeroPanel.style.flexBasis = columns.x;
                _inventoryHeroPanel.style.marginRight = 0;
                _inventoryHeroPanel.style.marginBottom = 0;
                _inventoryGridPanel.style.flexBasis = columns.x;
                _inventoryGridPanel.style.marginRight = 0;
                _inventoryGridPanel.style.marginBottom = 0;
                _inventoryDetailPanel.style.flexBasis = columns.y;
                _inventoryDetailPanel.style.marginLeft = columns.z;
                _inventoryDetailPanel.style.marginRight = 0;
                _inventoryDetailPanel.style.marginBottom = 0;
                if (_skillsPanel != null)
                {
                    _skillsPanel.style.flexBasis = columns.x;
                    _skillsPanel.style.marginBottom = 0;
                    _potentialPanel.style.flexBasis = columns.x;
                    _potentialPanel.style.marginBottom = 0;
                    _spiritPetPanel.style.flexBasis = columns.x;
                    _spiritPetPanel.style.marginBottom = 0;
                    _hubPreviewDetailPanel.style.flexBasis = columns.y;
                    _hubPreviewDetailPanel.style.marginLeft = columns.z;
                    _hubPreviewDetailPanel.style.marginBottom = 0;
                }
            }
        }
        private void PublishRuntimeUiMetrics(RuntimeUiLayoutProfile layout, Rect characterHubShellRect)
        {
            if (_scene == null) return;
            var authority = Application.isMobilePlatform
                ? "device-runtime-unverified"
                : layout.InputClass == "touch" ? "macos-aspect-simulation" : "macos-player";
            _scene.SetRuntimeUiMetrics(RuntimeUiEvidenceMetrics.CreateSnapshot(
                _metrics.ScreenPixelWidth, _metrics.ScreenPixelHeight, _metrics.PanelWidth, _metrics.PanelHeight,
                _metrics.SafePanelRect, characterHubShellRect, layout.Name, layout.InputClass,
                RuntimePanelSettingsProvider.Describe(_ownedPanel), authority, RuntimeUiTheme.Current.minimumTouchTarget));
        }

        private void Update()
        {
            if (_scene == null || _root == null) return;
            var metrics = RuntimeViewportMetrics.FromRoot(_root, _forcedLayoutProfile);
            if (!_metrics.LayoutEquals(metrics)) Layout();
            UpdatePasswordRecoveryCooldown();
            if (!_scene.IsCapturing)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    HandleEscape();

                var worldInputBlocked = ShouldBlockWorldInput(
                    _entryOpen, _menuOpen, _scene.InventoryOpen, _scene.DialogueOpen, _characterSelectOpen,
                    _serverSelectOpen);
                if (!worldInputBlocked)
                {
                    var keyboard = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f)
                        - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
                    if (!_touch) _scene.SetCharacterRun(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                    _scene.MoveOnLane(Mathf.Abs(_pad.Value.x) > .01f ? _pad.Value.x : keyboard, Time.deltaTime);
                    if (Input.GetKeyDown(KeyCode.E)) _scene.UseCurrentRouteAction();
                    if (_reviewHotkeysEnabled)
                    {
                        if (!_scene.IsSourcePoseReviewActive && Input.GetKeyDown(KeyCode.C)) _scene.CycleCharacterAvatarMode();
                        if (Input.GetKeyDown(KeyCode.L)) _scene.CycleCharacterLevel();
                        if (Input.GetKeyDown(KeyCode.G)) _scene.CycleCharacterGender();
                        if (Input.GetKeyDown(KeyCode.V)) _scene.CycleEquipmentSlot();
                        if (Input.GetKeyDown(KeyCode.M)) _scene.CycleSelectedEquipmentItemLevel();
                        if (Input.GetKeyDown(KeyCode.B)) _scene.ToggleEquipmentSlot();
                        if (Input.GetKeyDown(KeyCode.F)) _scene.CycleSourcePoseClass();
                    }
                    if (Input.GetKeyDown(KeyCode.X)) _scene.TriggerCharacterSkill();
                    _scene.SetCharacterJumpHeld(_touchJumpHeld || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));
                    if (Input.GetKeyDown(KeyCode.Z)) _scene.TriggerCharacterBasicAttack();
                    if (Input.GetKeyDown(KeyCode.I)) _scene.ToggleInventory();
                    if (Input.GetKeyDown(KeyCode.H)) _scene.UseHealthPotion();
                    if (Input.GetKeyDown(KeyCode.K)) _scene.UseManaPotion();
                    if (Input.GetKeyDown(KeyCode.R)) _scene.EquipClassReward();
                }
                else
                {
                    if (!_touch) _scene.SetCharacterRun(false);
                    _scene.SetCharacterJumpHeld(false);
                }
            }
            _questTitle.text = _scene.QuestDisplayTitle;
            _questObjective.text = _scene.QuestObjectiveText;
            _questProgress.text = _scene.QuestProgressText;
            _questMessage.text = QuestInteractionMessageForDisplay(
                _scene.LastInteractionMessage, _scene.QuestObjectiveText, _scene.QuestProgressText);
            _questMessage.style.display = _scene.DialogueOpen || string.IsNullOrEmpty(_questMessage.text)
                ? DisplayStyle.None
                : DisplayStyle.Flex;
            _talk.SetEnabled(_scene.CanUseCurrentRouteAction);
            _talk.text = _scene.CurrentActionLabel + (_touch ? "" : " · E");
            _outfit.text = "Trang bị " + _scene.AvatarClassLabel + ": " + _scene.CharacterAvatarMode + (_touch ? "" : " · C");
            _level.text = "Cấp đồ: " + _scene.EquipmentLevelLabel + (_touch ? "" : " · L");
            _gender.text = "Thân: " + _scene.CharacterGender + (_touch ? "" : " · G");
            _slot.text = "Slot: " + _scene.EquipmentSlotLabel + (_touch ? "" : " · V");
            _itemLevel.text = "Đổi cấp item" + (_touch ? "" : " · M");
            _toggleSlot.text = (_scene.EquippedSlotCount == 10 ? "Cởi slot" : "Mặc/cởi") + (_touch ? "" : " · B");
            _run.text = _touch ? "" : "SHIFT";
            _jump.text = _touch ? "" : "W";
            _basic.text = _touch ? "" : "Z";
            _skill.text = ActiveCharacterHubProfile.Skills[0].Name + (_touch ? "" : " · X");
            _skill.SetEnabled(_scene.CanTriggerCharacterSkill);
            _minimapTitle.text = _scene.MinimapUnlocked ? "BẢN ĐỒ ĐÔNG LÂM" : "BẢN ĐỒ KHU VỰC";
            _minimapStatus.text = _scene.MinimapUnlocked ? "Đang ở: " + _scene.CurrentRouteNodeLabel : "Hoàn thành Q02 để mở tuyến đường";
            _minimapCurrentMarker.style.display = _scene.MinimapUnlocked ? DisplayStyle.Flex : DisplayStyle.None;
            _minimapCurrentMarker.style.left = new Length(7f + 86f * Mathf.Clamp01(_scene.CurrentRouteProgress), LengthUnit.Percent);
            _minimap.style.opacity = _scene.MinimapUnlocked ? 1f : .74f;
            _minimap.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _inventory.style.display = _scene.InventoryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            if (_inventoryBackdrop != null)
                _inventoryBackdrop.style.display = _scene.InventoryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateCharacterHubOpenAnimation(_scene.InventoryOpen);
            _quest.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _questTabs.style.display = _scene.InventoryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            foreach (var control in new[] { _outfit, _level, _gender, _slot, _itemLevel, _toggleSlot })
                control.style.display = DisplayStyle.None;
            _inventorySummary.text = _scene.InventorySummaryText;
            _inventoryHeroTitle.text = _scene.ActiveEquipmentClassLabel + " · " + (_scene.CharacterGender == "female" ? "Nữ" : "Nam");
            _inventoryHeroMeta.text = "HP " + _scene.PlayerHealth + "/100  ·  MP " + _scene.PlayerMana + "/100";
            _equipmentTitle.text = "TRANG BỊ · " + _scene.EquippedSlotCount + "/10 món đang mặc";
            if (_suppliesEmptyState != null)
                _suppliesEmptyState.text = _scene.HealthPotionCount <= 0 && _scene.ManaPotionCount <= 0 && !_scene.HasClassRewardItem
                    ? "Chưa nhận vật phẩm nhiệm vụ. Hoàn thành Q04 để nhận bình máu, bình linh lực và hộ uyển tân thủ."
                    : "Chọn vật phẩm để dùng hoặc trang bị. Vật phẩm chưa đủ điều kiện sẽ tạm khóa nhưng vẫn đọc rõ trạng thái.";
            _questItemActions.style.display = DisplayStyle.Flex;
            RefreshInventoryEquipmentTiles();
            RefreshInventoryDetailCard();
            var selectedSlot = _scene.SelectedEquipmentSlot;
            var selectedEquipped = _scene.IsEquipmentSlotEquipped(selectedSlot);
            var equipToggleText = selectedEquipped ? "Tháo" : "Trang bị";
            if (!_suppliesOpen)
            {
                _inventoryDetailPrimaryAction.text = equipToggleText;
                _equipmentToggle.text = equipToggleText;
                var hasVariant = _scene.HasEquipmentItemVariant(_scene.SelectedEquipmentSlot);
                _equipmentVariant.text = hasVariant ? "Đổi cấp món" : "";
                _equipmentVariant.style.display = hasVariant ? DisplayStyle.Flex : DisplayStyle.None;
                _equipmentVariant.SetEnabled(hasVariant);
            }
            _equipmentClass.style.display = _scene.CanCycleCharacterHubClass ? DisplayStyle.Flex : DisplayStyle.None;
            _equipmentClass.text = "Class · " + _scene.ActiveEquipmentClassLabel + "  ›";
            _equipmentClass.SetEnabled(_scene.CanCycleCharacterHubClass);
            RefreshCharacterHubClassProfile();
            _inventoryGender.style.display = _scene.CanCycleSourcePoseGender ? DisplayStyle.Flex : DisplayStyle.None;
            _inventoryGender.text = "Đổi giới · "
                + (_scene.CharacterGender == "female" ? "Nữ" : "Nam") + (_touch ? "" : " · G");
            _inventoryGender.SetEnabled(_scene.CanCycleSourcePoseGender);
            _vitals.style.display = _scene.InventoryOpen || _scene.DialogueOpen ? DisplayStyle.None : DisplayStyle.Flex;
            _vitalsName.text = string.IsNullOrWhiteSpace(_loadedProductCharacterName) ? "LụcThiên" : _loadedProductCharacterName;
            _vitalsMeta.text = _scene.ActiveEquipmentClassLabel + " · " + (_scene.CharacterGender == "female" ? "Nữ" : "Nam") + "  ·  Lv.1";
            var playerPortrait = _scene.GetCharacterAvatarThumbnailSprite();
            _vitalsPortrait.style.backgroundImage = playerPortrait == null ? StyleKeyword.None : new StyleBackground(playerPortrait);
            _health.value = _scene.PlayerHealth; _health.title = "HP " + _scene.PlayerHealth + "/100";
            _mana.value = _scene.PlayerMana; _mana.title = "MP " + _scene.PlayerMana + "/100";
            foreach (var supplyAction in new[] { _healthPotion, _manaPotion, _equipReward })
                supplyAction.SetEnabled(true);
            RefreshInventorySupplyRows();
            _dialogue.style.display = _scene.DialogueOpen ? DisplayStyle.Flex : DisplayStyle.None;
            var hudBlocked = _scene.DialogueOpen || _scene.InventoryOpen || _characterSelectOpen;
            _talk.style.display = hudBlocked || !_scene.CanUseCurrentRouteAction ? DisplayStyle.None : DisplayStyle.Flex;
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
            _dialogueSpeaker.text = _scene.DialogueSpeaker;
            _dialogueQuestContext.text = DialogueQuestContextText();
            _dialogueLine.text = _scene.DialogueText;
            var dialoguePortrait = _scene.GetCurrentDialogueNpcSprite();
            _dialoguePortrait.style.backgroundImage = dialoguePortrait == null ? StyleKeyword.None : new StyleBackground(dialoguePortrait);
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
        private void EnsureProductAuthClient()
        {
            if (_productAuthClient != null) return;
            var client = new AccountApiClient(ClientRuntimeConfig.LoadStreamingAssets());
            _productAuthClient = client;
            _productAccountClient = client;
            _productCharacterClient = client;
            _ownsProductAuthClient = true;
        }

        private void EnsureProductAccountClient()
        {
            if (_productAccountClient != null) return;
            EnsureProductAuthClient();
            _productAccountClient = _productAuthClient as IProductAccountClient;
            if (_productAccountClient == null)
                throw new InvalidOperationException("Product account client is unavailable.");
        }

        private void EnsureProductCharacterClient()
        {
            if (_productCharacterClient != null) return;
            EnsureProductAuthClient();
            _productCharacterClient = _productAuthClient as IProductCharacterClient;
            if (_productCharacterClient == null)
                throw new InvalidOperationException("Product character client is unavailable.");
        }

        private CancellationToken ProductAuthCancellationToken => _productAuthCts?.Token ?? CancellationToken.None;

        private void OnDestroy()
        {
            if (_productAuthCts != null)
            {
                _productAuthCts.Cancel();
                _productAuthCts.Dispose();
                _productAuthCts = null;
            }
            if (_ownsProductAuthClient && _productAuthClient is IDisposable disposable) disposable.Dispose();
            if (_ownedPanel != null) Destroy(_ownedPanel);
        }
        private void OnDisable() { _pad?.ResetInput(); }
    }
}
