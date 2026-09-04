using System;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.Art;
using LinhGioi.Foundation;
using LinhGioi.Protocol.V1;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed class M4PlayableClientController : MonoBehaviour
    {
        private const string DefaultDevKey = "m4-playable-dev-key";
        private const string DefaultClassId = "class.sword";
        private AccountApiClient _client;
        private ClientRuntimeConfig _config;
        private CancellationTokenSource _shutdown;
        private UIDocument _document;
        private VisualElement _root;
        private VisualElement _mainShell;
        private VisualElement _authPanel;
        private VisualElement _lobbyPanel;
        private VisualElement _worldHud;
        private VisualElement _dialoguePanel;
        private VisualElement _sessionMenuPanel;
        private VisualElement _settingsPanel;
        private VisualElement _skillPreviewPanel;
        private VisualElement _localCombatPanel;
        private VisualElement _characterList;
        private TextField _devKey;
        private TextField _characterName;
        private TextField _classId;
        private Label _status;
        private Label _account;
        private Label _selectedName;
        private Label _selectedMeta;
        private Label _worldName;
        private Label _worldMeta;
        private Label _worldArea;
        private Label _worldStep;
        private Label _worldDirection;
        private Label _worldLandmarks;
        private Label _worldPoseState;
        private Label _worldVfxState;
        private Label _combatTargetStatus;
        private Label _combatVisualState;
        private Label _combatFeedback;
        private Label _combatCooldown;
        private Label _combatAuthority;
        private Label _skinSource;
        private VisualElement _combatCooldownIcon;
        private Label _worldObjective;
        private Label _interactionHint;
        private Label _position;
        private Label _toast;
        private Label _dialogueSpeaker;
        private Label _dialogueLine;
        private Label _dialogueProgress;
        private Label _sessionMenuStatus;
        private Button _loginButton;
        private Button _createButton;
        private Button _enterWorldButton;
        private Button _savePositionButton;
        private Button _backButton;
        private Button _quitButton;
        private Button _dialogueContinueButton;
        private Button _dialogueCloseButton;
        private Button _previewWindSlashButton;
        private Button _previewShadowBindButton;
        private Button _previewSpiritGuardButton;
        private Button _localCombatButton;
        private Button _resumeButton;
        private Button _sessionSaveButton;
        private Button _sessionBackButton;
        private Button _sessionQuitButton;
        private Toggle _showPositionToggle;
        private Toggle _showHintsToggle;
        private Toggle _focusModeToggle;
        private AccountResponse _accountState;
        private CharacterResponse[] _characters = Array.Empty<CharacterResponse>();
        private CharacterResponse _selectedCharacter;
        private PlayableWorldController _world;

        public static M4PlayableClientController Attach(GameObject host)
        {
            return host.GetComponent<M4PlayableClientController>() ?? host.AddComponent<M4PlayableClientController>();
        }

        private void Awake()
        {
            _shutdown = new CancellationTokenSource();
            _config = ClientRuntimeConfig.LoadStreamingAssets();
            _client = new AccountApiClient(_config);
            _document = gameObject.AddComponent<UIDocument>();
            _document.panelSettings = ResolvePanelSettings();
            BuildUi();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_worldHud != null && _worldHud.style.display == DisplayStyle.Flex)
                    ToggleSessionMenu();
                else
                    QuitPlayer();
            }
            if (_world != null && _position != null) _position.text = _world.FormatPosition();
        }

        private void OnDestroy()
        {
            _shutdown?.Cancel();
            _shutdown?.Dispose();
            _client?.Dispose();
        }

        private void BuildUi()
        {
            _root = _document.rootVisualElement;
            _root.Clear();
            _root.style.flexGrow = 1;
            _root.style.backgroundColor = RuntimeArtCatalog.Background;
            _root.style.color = RuntimeArtCatalog.Text;
            _root.style.paddingLeft = 18;
            _root.style.paddingRight = 18;
            _root.style.paddingTop = 16;
            _root.style.paddingBottom = 16;
            _root.style.alignItems = Align.Center;
            _root.style.unityBackgroundImageTintColor = RuntimeArtCatalog.Background;

            BuildHeader();

            _mainShell = new VisualElement();
            _mainShell.style.flexDirection = FlexDirection.Row;
            _mainShell.style.flexWrap = Wrap.Wrap;
            _mainShell.style.width = Length.Percent(100);
            _mainShell.style.maxWidth = 960;
            _mainShell.style.alignContent = Align.FlexStart;
            _mainShell.style.justifyContent = Justify.Center;
            _mainShell.style.marginTop = 12;
            _root.Add(_mainShell);

            BuildAuthPanel();
            BuildLobbyPanel();
            BuildWorldHud();
            ShowAuthMode();
        }

        private void BuildHeader()
        {
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.flexWrap = Wrap.Wrap;
            header.style.width = Length.Percent(100);
            header.style.maxWidth = 960;
            _root.Add(header);

            var brand = new VisualElement();
            brand.style.flexDirection = FlexDirection.Column;
            header.Add(brand);

            var title = new Label("Linh Gioi Online");
            title.style.fontSize = 30;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = RuntimeArtCatalog.Gold;
            brand.Add(title);

            var subtitle = new Label("Spirit Gate Playable Shell");
            subtitle.style.color = RuntimeArtCatalog.Spirit;
            subtitle.style.fontSize = 13;
            brand.Add(subtitle);

            _status = new Label("API: " + _config.apiBaseUrl);
            _status.style.color = RuntimeArtCatalog.Muted;
            _status.style.unityTextAlign = TextAnchor.MiddleRight;
            _status.style.marginTop = 6;
            ApplyStatusChip(_status, RuntimeArtCatalog.Muted);

            var right = new VisualElement();
            right.style.flexDirection = FlexDirection.Row;
            right.style.alignItems = Align.Center;
            right.style.flexWrap = Wrap.Wrap;
            right.Add(_status);
            _quitButton = NewQuietButton("Quit", QuitPlayer);
            _quitButton.tooltip = "Esc opens the session menu in world; Quit closes the player review";
            right.Add(_quitButton);
            header.Add(right);
        }

        private void BuildAuthPanel()
        {
            _authPanel = NewPanel(520);
            _mainShell.Add(_authPanel);
            _authPanel.Add(NewSectionTitle("Auth / Gate Entry"));
            _authPanel.Add(NewOrnamentRule(RuntimeArtCatalog.Spirit));
            _authPanel.Add(NewMutedLabel("API status: " + _config.apiBaseUrl));
            _devKey = NewTextField("Dev key", DefaultDevKey);
            _authPanel.Add(_devKey);
            _loginButton = NewPrimaryButton("Open Gate", () => RunAsync(LoginAsync));
            _authPanel.Add(_loginButton);
            _account = NewMutedLabel("Account: not connected");
            _account.style.marginTop = 10;
            _authPanel.Add(_account);
        }

        private void BuildLobbyPanel()
        {
            _lobbyPanel = NewPanel(840);
            _mainShell.Add(_lobbyPanel);
            _lobbyPanel.Add(NewSectionTitle("Character Hall"));
            _lobbyPanel.Add(NewOrnamentRule(RuntimeArtCatalog.Gold));

            var content = new VisualElement();
            content.style.flexDirection = FlexDirection.Row;
            content.style.flexWrap = Wrap.Wrap;
            content.style.justifyContent = Justify.SpaceBetween;
            _lobbyPanel.Add(content);

            _characterList = new VisualElement();
            _characterList.style.minWidth = 220;
            _characterList.style.flexGrow = 1;
            _characterList.style.marginRight = 14;
            content.Add(_characterList);

            var preview = NewPreviewPanel();
            _selectedName = new Label("No character selected");
            _selectedName.style.fontSize = 19;
            _selectedName.style.unityFontStyleAndWeight = FontStyle.Bold;
            _selectedName.style.color = RuntimeArtCatalog.Gold;
            preview.Add(_selectedName);
            _selectedMeta = NewMutedLabel("Create a cultivator to enter the world.");
            preview.Add(_selectedMeta);
            content.Add(preview);

            var create = new VisualElement();
            create.style.marginTop = 14;
            create.style.paddingTop = 12;
            create.style.borderTopColor = RuntimeArtCatalog.SurfaceRaised;
            create.style.borderTopWidth = 1;
            _lobbyPanel.Add(create);

            create.Add(NewSectionTitle("Create Cultivator"));
            _characterName = NewTextField("Character name", "LinhGioiHero");
            _classId = NewTextField("Class ID", DefaultClassId);
            _createButton = NewSecondaryButton("Create", () => RunAsync(CreateCharacterAsync));
            _enterWorldButton = NewPrimaryButton("Enter World", () => RunAsync(EnterWorldAsync));
            create.Add(_characterName);
            create.Add(_classId);
            create.Add(NewButtonRow(_createButton, _enterWorldButton));
        }

        private void BuildWorldHud()
        {
            _worldHud = NewPanel(760);
            _mainShell.Add(_worldHud);
            _worldHud.Add(NewSectionTitle("World HUD / Safe Training Yard"));
            _worldHud.Add(NewOrnamentRule(RuntimeArtCatalog.Spirit));

            var topStrip = new VisualElement();
            topStrip.style.flexDirection = FlexDirection.Row;
            topStrip.style.flexWrap = Wrap.Wrap;
            topStrip.style.marginBottom = 10;
            _worldHud.Add(topStrip);

            topStrip.Add(NewBadge("Account", "profile loaded"));
            topStrip.Add(NewBadge("Persistence", "local dev API"));
            topStrip.Add(NewBadge("Move", "WASD or arrows"));
            topStrip.Add(NewBadge("Turn", "Q / E"));
            topStrip.Add(NewBadge("Interact", "F or Space"));
            topStrip.Add(NewBadge("Menu", "Esc"));

            _worldName = new Label("No character selected");
            _worldName.style.fontSize = 19;
            _worldName.style.unityFontStyleAndWeight = FontStyle.Bold;
            _worldName.style.color = RuntimeArtCatalog.Gold;
            _worldHud.Add(_worldName);

            _worldMeta = NewMutedLabel("Select a character in the lobby.");
            _worldHud.Add(_worldMeta);

            _worldArea = NewStatusLabel("Area: Lobby preview", RuntimeArtCatalog.Muted);
            _worldHud.Add(_worldArea);

            _worldStep = NewStatusLabel("Guided loop: Step 1 Gate Keeper / Step 2 Training Stone.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_worldStep);

            _worldDirection = NewStatusLabel("Direction: enter the world to reveal yard landmarks.", RuntimeArtCatalog.Gold);
            _worldHud.Add(_worldDirection);

            _worldPoseState = NewStatusLabel("Pose: player idle / Gate Keeper idle / Shadow Slime idle.", RuntimeArtCatalog.Muted);
            _worldHud.Add(_worldPoseState);

            _worldVfxState = NewStatusLabel("VFX: Quiet / portal, spirit pulse, wind slash preview, shadow bind warning are visual-only.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_worldVfxState);

            _skinSource = NewStatusLabel("UI skin source: v0.20 component sheet / window popup sheet.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_skinSource);

            _worldObjective = NewStatusLabel("Objective 1/2: talk to the Gate Keeper.", RuntimeArtCatalog.Gold);
            _worldHud.Add(_worldObjective);

            _interactionHint = NewStatusLabel("Move near the Gate Keeper.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_interactionHint);

            _position = NewMutedLabel("x=0.00 y=0.00 z=0.00 yaw=0.0");
            _position.style.marginTop = 8;
            _position.style.backgroundColor = RuntimeArtCatalog.Background;
            _position.style.paddingLeft = 10;
            _position.style.paddingRight = 10;
            _position.style.paddingTop = 6;
            _position.style.paddingBottom = 6;
            _worldHud.Add(_position);

            _worldLandmarks = NewMutedLabel("Landmarks: Spirit Gate south / Gate Keeper northwest / Training Stone north / Readability Dummy east / Shadow Slime far east.");
            _worldLandmarks.style.marginTop = 8;
            _worldHud.Add(_worldLandmarks);

            _toast = NewToast("Spirit Gate shell ready.");
            _worldHud.Add(_toast);

            BuildSessionMenuPanel();
            BuildSkillPreviewPanel();
            BuildLocalCombatPanel();

            _dialoguePanel = NewPreviewPanel();
            _dialoguePanel.style.marginTop = 10;
            _dialogueSpeaker = new Label("Gate Keeper");
            _dialogueSpeaker.style.fontSize = 17;
            _dialogueSpeaker.style.unityFontStyleAndWeight = FontStyle.Bold;
            _dialogueSpeaker.style.color = RuntimeArtCatalog.Gold;
            _dialogueLine = NewMutedLabel("Dialogue closed.");
            _dialogueProgress = NewStatusLabel("Dialogue: 0/3", RuntimeArtCatalog.Muted);
            _dialogueContinueButton = NewSecondaryButton("Continue", ContinueDialogue);
            _dialogueCloseButton = NewQuietButton("Close", CloseDialogue);
            _dialoguePanel.Add(_dialogueSpeaker);
            _dialoguePanel.Add(_dialogueLine);
            _dialoguePanel.Add(_dialogueProgress);
            _dialoguePanel.Add(NewButtonRow(_dialogueContinueButton, _dialogueCloseButton));
            _worldHud.Add(_dialoguePanel);
            SetDialogueVisible(false);

            _savePositionButton = NewPrimaryButton("Save Position", () => RunAsync(SavePositionAsync));
            _savePositionButton.tooltip = "Persist this character position to the local dev API";
            _backButton = NewSecondaryButton("Back to Character Hall", BackToLobby);
            _backButton.tooltip = "Back to Lobby / return to character management without leaving the player";
            _worldHud.Add(NewButtonRow(_savePositionButton, _backButton, NewQuietButton("Quit", QuitPlayer)));
        }

        private void BuildSessionMenuPanel()
        {
            _sessionMenuPanel = NewPreviewPanel();
            _sessionMenuPanel.name = "LGO Session Menu Overlay";
            _sessionMenuPanel.style.marginTop = 10;
            _sessionMenuPanel.style.backgroundColor = RuntimeArtCatalog.Background;
            _sessionMenuPanel.style.borderLeftColor = RuntimeArtCatalog.Gold;
            _sessionMenuPanel.style.borderLeftWidth = 2;
            _sessionMenuPanel.Add(NewSectionTitle("Session Menu"));
            _sessionMenuStatus = NewMutedLabel("Paused in safe training yard.");
            _sessionMenuPanel.Add(_sessionMenuStatus);
            _resumeButton = NewPrimaryButton("Resume", HideSessionMenu);
            _sessionSaveButton = NewSecondaryButton("Save Position", () => RunAsync(SavePositionAsync));
            _sessionBackButton = NewSecondaryButton("Back to Character Hall", BackToLobby);
            _sessionQuitButton = NewQuietButton("Quit", QuitPlayer);
            _sessionMenuPanel.Add(NewButtonRow(_resumeButton, _sessionSaveButton, _sessionBackButton, _sessionQuitButton));
            BuildLocalSettingsPanel();
            _worldHud.Add(_sessionMenuPanel);
            SetSessionMenuVisible(false);
        }

        private void BuildSkillPreviewPanel()
        {
            _skillPreviewPanel = NewPreviewPanel();
            _skillPreviewPanel.name = "LGO Skill Preview Sandbox";
            _skillPreviewPanel.style.marginTop = 10;
            _skillPreviewPanel.Add(NewSectionTitle("Skill Preview Sandbox"));
            _skillPreviewPanel.Add(NewMutedLabel("Local visual rehearsal only. No opponent, timing rule, or progression result is created."));
            _previewWindSlashButton = NewSecondaryButton("Preview Wind Slash", () => PreviewSkill("Wind Slash"));
            _previewShadowBindButton = NewSecondaryButton("Preview Shadow Bind", () => PreviewSkill("Shadow Bind"));
            _previewSpiritGuardButton = NewSecondaryButton("Preview Spirit Guard", () => PreviewSkill("Spirit Guard"));
            _skillPreviewPanel.Add(NewButtonRow(_previewWindSlashButton, _previewShadowBindButton, _previewSpiritGuardButton));
            _worldHud.Add(_skillPreviewPanel);
        }

        private void BuildLocalCombatPanel()
        {
            _localCombatPanel = NewPreviewPanel();
            _localCombatPanel.name = "LGO M6 Minimal Local Combat";
            _localCombatPanel.style.marginTop = 10;
            ApplyCombatPanelSkin(_localCombatPanel);
            _localCombatPanel.Add(NewSectionTitle("Luyện mục tiêu cục bộ"));
            _localCombatPanel.Add(NewMutedLabel("Nhãn nguyên mẫu cục bộ: Tấn công thử chỉ kiểm tra khả năng đọc mục tiêu, hit flash và hồi chiêu. Không có sát thương thật, phần thưởng, kinh nghiệm, hay chiến đấu máy chủ."));
            _combatCooldownIcon = NewCombatCooldownIcon();
            _combatTargetStatus = NewStatusLabel("Mục tiêu luyện tập: chưa vào sân.", RuntimeArtCatalog.Gold);
            _combatVisualState = NewStatusLabel("Dấu hiệu mục tiêu: chưa chọn.", RuntimeArtCatalog.Gold);
            _combatFeedback = NewStatusLabel("Chưa phải chiến đấu thật.", RuntimeArtCatalog.Spirit);
            _combatCooldown = NewStatusLabel("Hồi chiêu: Sẵn sàng", RuntimeArtCatalog.Muted);
            _combatAuthority = NewStatusLabel("Mô phỏng cục bộ: chưa gửi ý định chiến đấu.", RuntimeArtCatalog.Spirit);
            _localCombatButton = NewSecondaryButton("Gửi ý định chiến đấu", TriggerLocalCombat);
            _localCombatButton.tooltip = "Kích hoạt phản hồi đánh thử cục bộ. Đánh thử cục bộ: xem vòng chọn mục tiêu, hit flash và nhịp hồi chiêu; không phải chiến đấu thật";
            ApplyCombatButtonSkin(_localCombatButton, CombatPlaceholderAssets.CombatButtonNormalTexture);
            _localCombatPanel.Add(_combatCooldownIcon);
            _localCombatPanel.Add(_combatTargetStatus);
            _localCombatPanel.Add(_combatVisualState);
            _localCombatPanel.Add(_combatFeedback);
            _localCombatPanel.Add(_combatCooldown);
            _localCombatPanel.Add(_combatAuthority);
            _localCombatPanel.Add(NewButtonRow(_localCombatButton));
            _worldHud.Add(_localCombatPanel);
        }

        private void BuildLocalSettingsPanel()
        {
            _settingsPanel = NewPreviewPanel();
            _settingsPanel.name = "LGO Local Settings Foundation";
            _settingsPanel.style.marginTop = 8;
            _settingsPanel.Add(NewSectionTitle("Local Settings"));
            _showPositionToggle = NewLocalSettingToggle("Show position readout", true, ApplyLocalSettings);
            _showHintsToggle = NewLocalSettingToggle("Show guidance hints", true, ApplyLocalSettings);
            _focusModeToggle = NewLocalSettingToggle("Focus HUD mode", false, ApplyLocalSettings);
            _settingsPanel.Add(_showPositionToggle);
            _settingsPanel.Add(_showHintsToggle);
            _settingsPanel.Add(_focusModeToggle);
            _sessionMenuPanel.Add(_settingsPanel);
        }

        private async Task LoginAsync()
        {
            SetBusy(true, "Opening spirit gate...");
            var login = await _client.LoginDevAsync(Required(_devKey.value, DefaultDevKey), "M4 Playable Client", _shutdown.Token);
            _accountState = login.account;
            _account.text = "Account: " + Abbrev(_accountState.accountId) + " / " + _accountState.displayName;
            await RefreshCharactersAsync();
            ShowLobbyMode();
            SetBusy(false, "Lobby ready");
            SetToast("Account linked. Character Hall opened.", RuntimeArtCatalog.Spirit);
        }

        private async Task RefreshCharactersAsync()
        {
            _characters = await _client.ListCharactersAsync(_accountState.accountId, _shutdown.Token);
            _characterList.Clear();
            _characterList.Add(NewMutedLabel(_characters.Length == 0 ? "No character yet" : "Select a character"));
            if (_characters.Length == 0)
            {
                SelectCharacter(null);
                return;
            }
            foreach (var character in _characters)
            {
                var captured = character;
                _characterList.Add(NewListButton(character.name, character.classId, () => SelectCharacter(captured)));
            }
            SelectCharacter(_characters[0]);
        }

        private async Task CreateCharacterAsync()
        {
            if (_accountState == null) return;
            SetBusy(true, "Creating cultivator...");
            try
            {
                var created = await _client.CreateCharacterAsync(_accountState.accountId, Required(_characterName.value, "LinhGioiHero"), Required(_classId.value, DefaultClassId), _shutdown.Token);
                _selectedCharacter = created;
                await RefreshCharactersAsync();
                SetBusy(false, "Character ready");
                SetToast("Cultivator record prepared.", RuntimeArtCatalog.Gold);
            }
            catch (Exception exception)
            {
                SetApiError("create character", exception);
            }
        }

        private async Task EnterWorldAsync()
        {
            if (_selectedCharacter == null) return;
            SetBusy(true, "Entering Spirit Gate training yard...");
            var loaded = await _client.LoadCharacterAsync(_selectedCharacter.characterId, _shutdown.Token);
            _selectedCharacter = loaded;
            if (_world == null)
            {
                _world = gameObject.AddComponent<PlayableWorldController>();
                _world.PositionChanged += () => _position.text = _world.FormatPosition();
                _world.InteractionStateChanged += RefreshWorldLoopLabels;
            }
            _world.Enter(loaded);
            RefreshWorldLoopLabels();
            UpdateSelectedPreview(loaded);
            ShowWorldMode();
            SetBusy(false, "Training yard ready: follow Step 1 then Step 2.");
            SetToast("Spirit Gate opened. Step 1: talk to the Gate Keeper.", RuntimeArtCatalog.Spirit);
        }

        private async Task SavePositionAsync()
        {
            if (_selectedCharacter == null || _world == null) return;
            SetBusy(true, "Saving position to local dev API...");
            var save = _world.BuildSaveRequest();
            _selectedCharacter = await _client.SaveCharacterPositionAsync(_selectedCharacter.characterId, save.x, save.y, save.z, save.yawDegrees, _shutdown.Token);
            UpdateSelectedPreview(_selectedCharacter);
            SetBusy(false, "Position saved near " + _world.CurrentAreaLabel + ".");
            SetToast("Position seal recorded near " + _world.CurrentAreaLabel + ".", RuntimeArtCatalog.Gold);
        }

        private void BackToLobby()
        {
            SetSessionMenuVisible(false);
            ShowLobbyMode();
            SetBusy(false, "Returned to Character Hall.");
            SetToast("Returned to Character Hall.", RuntimeArtCatalog.Muted);
        }

        private void SelectCharacter(CharacterResponse character)
        {
            _selectedCharacter = character;
            UpdateSelectedPreview(character);
            _enterWorldButton.SetEnabled(character != null);
            _status.text = character == null ? "Create or select a cultivator" : "Selected: " + character.name;
            SetToast(character == null ? "Awaiting cultivator selection." : "Selected " + character.name + ".", RuntimeArtCatalog.Muted);
        }

        private void UpdateSelectedPreview(CharacterResponse character)
        {
            if (character == null)
            {
                _selectedName.text = "No character selected";
                _selectedMeta.text = "Create a cultivator to enter the world.";
                _worldName.text = "No character selected";
                _worldMeta.text = "Select a character in the lobby.";
                if (_worldArea != null) _worldArea.text = "Area: Lobby preview";
                if (_worldStep != null) _worldStep.text = "Guided loop: Step 1 Gate Keeper / Step 2 Training Stone.";
                if (_worldDirection != null) _worldDirection.text = "Direction: enter the world to reveal Step 1 guidance.";
                if (_worldLandmarks != null) _worldLandmarks.text = "Landmarks: Spirit Gate south / Gate Keeper northwest / Training Stone north / Readability Dummy east / Shadow Slime far east.";
                if (_worldPoseState != null) _worldPoseState.text = "Pose: player idle / Gate Keeper idle / Shadow Slime idle.";
                if (_worldVfxState != null) _worldVfxState.text = "VFX: Quiet / portal, spirit pulse, wind slash preview, shadow bind warning are visual-only.";
                if (_skinSource != null) _skinSource.text = "UI skin source: v0.20 component sheet / window popup sheet.";
                if (_worldObjective != null) _worldObjective.text = "Objective 1/2: talk to the Gate Keeper.";
                if (_interactionHint != null) _interactionHint.text = "Move near the Gate Keeper.";
                _position.text = "x=0.00 y=0.00 z=0.00 yaw=0.0";
                return;
            }
            _selectedName.text = character.name;
            _selectedMeta.text = "Class " + character.classId + " / " + Abbrev(character.characterId);
            _worldName.text = character.name;
            _worldMeta.text = "Class " + character.classId + " / " + Abbrev(character.characterId);
            _position.text = character.ToString();
        }

        private void RefreshWorldLoopLabels()
        {
            if (_world == null) return;
            if (_worldArea != null) _worldArea.text = "Area: " + _world.CurrentAreaLabel;
            if (_worldStep != null) _worldStep.text = "Guided loop: " + _world.GuidedTrainingStepName;
            if (_worldDirection != null) _worldDirection.text = "Direction: " + _world.ObjectiveDirectionHint;
            if (_worldLandmarks != null) _worldLandmarks.text = _world.WorldLandmarkSummary;
            if (_worldPoseState != null) _worldPoseState.text = "Pose: player " + _world.PlayerPoseStateName + " / Gate Keeper " + _world.GateKeeperPoseStateName + " / Shadow Slime " + _world.ShadowSlimeStateName + ".";
            if (_worldVfxState != null) _worldVfxState.text = "VFX: " + _world.VfxFeedbackStateName + " / visual-only local feedback.";
            if (_combatTargetStatus != null) _combatTargetStatus.text = _world.TargetDummyStatusText;
            if (_combatVisualState != null) _combatVisualState.text = _world.TargetDummyVisualStateText;
            if (_combatFeedback != null) _combatFeedback.text = _world.CombatFeedbackText;
            if (_combatCooldown != null) _combatCooldown.text = _world.CombatCooldownText;
            if (_combatAuthority != null) _combatAuthority.text = _world.CombatAuthorityText;
            RefreshCombatAssetUiState();
            if (_skinSource != null) _skinSource.text = "UI skin source: v0.20 component sheet / window popup sheet.";
            if (_worldObjective != null) _worldObjective.text = _world.ObjectiveText;
            if (_interactionHint != null) _interactionHint.text = _world.InteractionText;
            SetToast(_world.InteractionAcknowledged ? "Training complete. Save position or return to Character Hall." : _world.InteractionText, RuntimeArtCatalog.Spirit);
            RefreshDialoguePanel();
            ApplyLocalSettings();
        }

        private void ShowAuthMode()
        {
            _authPanel.style.display = DisplayStyle.Flex;
            _lobbyPanel.style.display = DisplayStyle.None;
            _worldHud.style.display = DisplayStyle.None;
            SetLobbyControls(false);
        }

        private void ShowLobbyMode()
        {
            _authPanel.style.display = DisplayStyle.None;
            _lobbyPanel.style.display = DisplayStyle.Flex;
            _worldHud.style.display = DisplayStyle.None;
            SetLobbyControls(true);
        }

        private void ShowWorldMode()
        {
            _authPanel.style.display = DisplayStyle.None;
            _lobbyPanel.style.display = DisplayStyle.None;
            _worldHud.style.display = DisplayStyle.Flex;
            SetSessionMenuVisible(false);
            _savePositionButton.SetEnabled(true);
            _backButton.SetEnabled(true);
        }

        private void SetLobbyControls(bool enabled)
        {
            _characterName.SetEnabled(enabled);
            _classId.SetEnabled(enabled);
            _createButton.SetEnabled(enabled);
            _enterWorldButton.SetEnabled(enabled && _selectedCharacter != null);
        }

        private void SetBusy(bool busy, string message)
        {
            _status.text = message;
            ApplyStatusChip(_status, busy ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Muted);
            _loginButton.SetEnabled(!busy);
            if (_accountState != null)
            {
                _createButton.SetEnabled(!busy);
                _enterWorldButton.SetEnabled(!busy && _selectedCharacter != null);
                _savePositionButton.SetEnabled(!busy);
                _backButton.SetEnabled(!busy);
                if (_sessionSaveButton != null) _sessionSaveButton.SetEnabled(!busy);
                if (_sessionBackButton != null) _sessionBackButton.SetEnabled(!busy);
            }
        }

        private void RunAsync(Func<Task> action)
        {
            _ = RunSafelyAsync(action);
        }

        private async Task RunSafelyAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                SetApiError("API request", exception);
            }
        }

        private void SetApiError(string action, Exception exception)
        {
            var message = "API blocked during " + action + ": " + exception.Message;
            SetBusy(false, message);
            SetToast("Local API unavailable or rejected the request. Check server, then retry.", RuntimeArtCatalog.Danger);
            if (_sessionMenuStatus != null)
                _sessionMenuStatus.text = "API error: check local server, then retry the same action.";
        }

        private static VisualElement NewPanel(float maxWidth)
        {
            var panel = new VisualElement();
            panel.style.maxWidth = maxWidth;
            panel.style.minWidth = 300;
            panel.style.width = Length.Percent(100);
            panel.style.marginRight = 0;
            panel.style.marginBottom = 12;
            panel.style.paddingLeft = 16;
            panel.style.paddingRight = 16;
            panel.style.paddingTop = 14;
            panel.style.paddingBottom = 14;
            panel.style.backgroundColor = RuntimeArtCatalog.Surface;
            panel.style.borderTopLeftRadius = 8;
            panel.style.borderTopRightRadius = 8;
            panel.style.borderBottomLeftRadius = 8;
            panel.style.borderBottomRightRadius = 8;
            panel.style.borderLeftColor = RuntimeArtCatalog.Spirit;
            panel.style.borderLeftWidth = 2;
            panel.style.borderTopColor = RuntimeArtCatalog.Gold;
            panel.style.borderTopWidth = 1;
            panel.style.borderRightColor = RuntimeArtCatalog.SurfaceRaised;
            panel.style.borderRightWidth = 1;
            panel.style.borderBottomColor = RuntimeArtCatalog.SurfaceRaised;
            panel.style.borderBottomWidth = 1;
            return panel;
        }

        private static VisualElement NewPreviewPanel()
        {
            var preview = new VisualElement();
            preview.style.minWidth = 220;
            preview.style.flexGrow = 1;
            preview.style.paddingLeft = 14;
            preview.style.paddingRight = 14;
            preview.style.paddingTop = 12;
            preview.style.paddingBottom = 12;
            preview.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            preview.style.borderTopLeftRadius = 8;
            preview.style.borderTopRightRadius = 8;
            preview.style.borderBottomLeftRadius = 8;
            preview.style.borderBottomRightRadius = 8;
            preview.style.borderLeftColor = RuntimeArtCatalog.Gold;
            preview.style.borderLeftWidth = 2;
            preview.style.borderTopColor = RuntimeArtCatalog.Spirit;
            preview.style.borderTopWidth = 1;
            var sigil = new Label("SPIRIT GATE");
            sigil.style.color = RuntimeArtCatalog.Spirit;
            sigil.style.unityFontStyleAndWeight = FontStyle.Bold;
            preview.Add(sigil);
            return preview;
        }

        private static Label NewSectionTitle(string text)
        {
            var label = new Label(text);
            label.style.fontSize = 17;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.color = RuntimeArtCatalog.Text;
            label.style.marginBottom = 8;
            return label;
        }

        private static Label NewMutedLabel(string text)
        {
            var label = new Label(text);
            label.style.color = RuntimeArtCatalog.Muted;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        private static Label NewStatusLabel(string text, Color color)
        {
            var label = new Label(text);
            label.style.color = color;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.marginTop = 8;
            label.style.paddingLeft = 10;
            label.style.paddingRight = 10;
            label.style.paddingTop = 6;
            label.style.paddingBottom = 6;
            label.style.backgroundColor = RuntimeArtCatalog.Background;
            label.style.borderLeftColor = color;
            label.style.borderLeftWidth = 2;
            label.style.borderTopColor = RuntimeArtCatalog.SurfaceRaised;
            label.style.borderTopWidth = 1;
            label.style.borderBottomColor = RuntimeArtCatalog.SurfaceRaised;
            label.style.borderBottomWidth = 1;
            return label;
        }

        private static TextField NewTextField(string label, string value)
        {
            var field = new TextField(label) { value = value };
            field.style.maxWidth = 420;
            field.style.marginTop = 8;
            field.style.color = RuntimeArtCatalog.Text;
            return field;
        }

        private static Button NewPrimaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.backgroundColor = RuntimeArtCatalog.Spirit;
            button.style.color = RuntimeArtCatalog.Background;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            return button;
        }

        private static Button NewQuietButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.minWidth = 88;
            button.style.backgroundColor = RuntimeArtCatalog.Background;
            button.style.color = RuntimeArtCatalog.Muted;
            return button;
        }

        private static Button NewSecondaryButton(string label, Action action)
        {
            var button = NewButton(label, action);
            button.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            button.style.color = RuntimeArtCatalog.Text;
            return button;
        }

        private static void QuitPlayer()
        {
            Application.Quit();
        }

        private void ToggleSessionMenu()
        {
            var visible = _sessionMenuPanel != null && _sessionMenuPanel.style.display == DisplayStyle.Flex;
            SetSessionMenuVisible(!visible);
        }

        private void HideSessionMenu()
        {
            SetSessionMenuVisible(false);
            SetToast("Session resumed.", RuntimeArtCatalog.Muted);
        }

        private void SetSessionMenuVisible(bool visible)
        {
            if (_sessionMenuPanel == null) return;
            _sessionMenuPanel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_sessionMenuStatus != null)
                _sessionMenuStatus.text = visible ? "Paused in safe training yard. Resume, save, return, or quit." : "Session active.";
            ApplyLocalSettings();
        }

        private void ApplyLocalSettings()
        {
            var showPosition = _showPositionToggle == null || _showPositionToggle.value;
            var showHints = _showHintsToggle == null || _showHintsToggle.value;
            var focusMode = _focusModeToggle != null && _focusModeToggle.value;
            if (_position != null) _position.style.display = showPosition && !focusMode ? DisplayStyle.Flex : DisplayStyle.None;
            if (_worldDirection != null) _worldDirection.style.display = showHints ? DisplayStyle.Flex : DisplayStyle.None;
            if (_interactionHint != null) _interactionHint.style.display = showHints ? DisplayStyle.Flex : DisplayStyle.None;
            if (_worldLandmarks != null) _worldLandmarks.style.display = showHints && !focusMode ? DisplayStyle.Flex : DisplayStyle.None;
            if (_worldPoseState != null) _worldPoseState.style.display = focusMode ? DisplayStyle.None : DisplayStyle.Flex;
            if (_worldVfxState != null) _worldVfxState.style.display = focusMode ? DisplayStyle.None : DisplayStyle.Flex;
            if (_skinSource != null) _skinSource.style.display = focusMode ? DisplayStyle.None : DisplayStyle.Flex;
            if (_skillPreviewPanel != null) _skillPreviewPanel.style.display = focusMode ? DisplayStyle.None : DisplayStyle.Flex;
            if (_localCombatPanel != null) _localCombatPanel.style.display = DisplayStyle.Flex;
        }

        private void TriggerLocalCombat()
        {
            if (_world == null) return;
            ApplyCombatButtonSkin(_localCombatButton, CombatPlaceholderAssets.CombatButtonPressedTexture);
            var intent = _world.BuildCombatIntentForLocalPreview(1, "unity-local-preview-1");
            _world.MarkCombatIntentPending(intent);
            _world.TryLocalCombatPrototype();
            RefreshWorldLoopLabels();
            SetToast(_world.CombatFeedbackText, RuntimeArtCatalog.Gold);
            RefreshCombatAssetUiState();
        }

        private void RefreshCombatAssetUiState()
        {
            if (_world == null) return;
            var coolingDown = _world.LocalCombatCoolingDown;
            if (_combatCooldownIcon != null)
            {
                var texture = coolingDown ? CombatPlaceholderAssets.CooldownActiveTexture : CombatPlaceholderAssets.CooldownReadyTexture;
                if (texture != null) _combatCooldownIcon.style.backgroundImage = new StyleBackground(texture);
                _combatCooldownIcon.tooltip = coolingDown ? "Hồi chiêu mô phỏng đang chạy." : "Sẵn sàng tấn công thử.";
            }
            if (_localCombatButton != null)
            {
                ApplyCombatButtonSkin(_localCombatButton, coolingDown ? CombatPlaceholderAssets.CombatButtonCooldownTexture : CombatPlaceholderAssets.CombatButtonNormalTexture);
            }
        }

        private void PreviewSkill(string previewName)
        {
            if (_world == null) return;
            _world.PreviewSkillFeedback(previewName);
            RefreshWorldLoopLabels();
            SetToast("Preview only: " + previewName + " feedback played in the safe yard.", RuntimeArtCatalog.Spirit);
        }

        private void ContinueDialogue()
        {
            if (_world == null) return;
            _world.ContinueDialogue();
            RefreshWorldLoopLabels();
        }

        private void CloseDialogue()
        {
            if (_world == null) return;
            _world.CloseDialogue();
            RefreshWorldLoopLabels();
        }

        private void RefreshDialoguePanel()
        {
            if (_world == null || _dialoguePanel == null)
            {
                SetDialogueVisible(false);
                return;
            }
            SetDialogueVisible(_world.DialogueActive);
            if (!_world.DialogueActive) return;
            _dialogueSpeaker.text = _world.DialogueSpeaker;
            _dialogueLine.text = _world.DialogueLine;
            _dialogueProgress.text = "Dialogue: " + _world.DialogueProgress;
            _dialogueContinueButton.text = _world.HasNextDialogueLine ? "Continue" : "Finish";
        }

        private void SetDialogueVisible(bool visible)
        {
            if (_dialoguePanel != null) _dialoguePanel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static Button NewButton(string label, Action action)
        {
            var button = new Button(action) { text = label };
            button.style.minWidth = 132;
            button.style.minHeight = 44;
            button.style.marginTop = 8;
            button.style.marginRight = 8;
            button.style.borderTopLeftRadius = 8;
            button.style.borderTopRightRadius = 8;
            button.style.borderBottomLeftRadius = 8;
            button.style.borderBottomRightRadius = 8;
            button.style.borderTopColor = RuntimeArtCatalog.Gold;
            button.style.borderTopWidth = 1;
            button.style.borderLeftColor = RuntimeArtCatalog.Spirit;
            button.style.borderLeftWidth = 1;
            button.style.borderRightColor = RuntimeArtCatalog.SurfaceRaised;
            button.style.borderRightWidth = 1;
            button.style.borderBottomColor = RuntimeArtCatalog.SurfaceRaised;
            button.style.borderBottomWidth = 1;
            return button;
        }

        private static Toggle NewLocalSettingToggle(string label, bool value, Action changed)
        {
            var toggle = new Toggle(label) { value = value };
            toggle.style.marginTop = 6;
            toggle.style.color = RuntimeArtCatalog.Text;
            toggle.RegisterValueChangedCallback(_ => changed());
            return toggle;
        }

        private static Button NewListButton(string name, string classId, Action action)
        {
            var button = NewSecondaryButton(name + "\n" + classId, action);
            button.style.minWidth = 210;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.tooltip = "Select cultivator";
            return button;
        }

        private static VisualElement NewButtonRow(params Button[] buttons)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.marginTop = 6;
            foreach (var button in buttons) row.Add(button);
            return row;
        }

        private static VisualElement NewCombatCooldownIcon()
        {
            var icon = new VisualElement();
            icon.name = "LGO M6 Combat Cooldown Runtime Icon v0.46";
            icon.style.width = 44;
            icon.style.height = 44;
            icon.style.marginBottom = 8;
            icon.style.backgroundColor = RuntimeArtCatalog.Surface;
            var texture = CombatPlaceholderAssets.CooldownReadyTexture;
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = "Sẵn sàng tấn công thử.";
            return icon;
        }

        private static void ApplyCombatPanelSkin(VisualElement panel)
        {
            var texture = CombatPlaceholderAssets.CombatPanelTexture;
            if (texture == null) return;
            panel.style.backgroundImage = new StyleBackground(texture);
        }

        private static void ApplyCombatButtonSkin(Button button, Texture2D texture)
        {
            if (button == null || texture == null) return;
            button.style.backgroundImage = new StyleBackground(texture);
        }

        private static VisualElement NewBadge(string title, string value)
        {
            var badge = new VisualElement();
            badge.style.paddingLeft = 10;
            badge.style.paddingRight = 10;
            badge.style.paddingTop = 6;
            badge.style.paddingBottom = 6;
            badge.style.marginRight = 8;
            badge.style.marginBottom = 8;
            badge.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            badge.style.borderTopLeftRadius = 8;
            badge.style.borderTopRightRadius = 8;
            badge.style.borderBottomLeftRadius = 8;
            badge.style.borderBottomRightRadius = 8;
            badge.style.borderLeftColor = RuntimeArtCatalog.Spirit;
            badge.style.borderLeftWidth = 1;
            badge.style.borderTopColor = RuntimeArtCatalog.Gold;
            badge.style.borderTopWidth = 1;
            var titleLabel = new Label(title);
            titleLabel.style.color = RuntimeArtCatalog.Gold;
            titleLabel.style.fontSize = 11;
            var valueLabel = new Label(value);
            valueLabel.style.color = RuntimeArtCatalog.Text;
            valueLabel.style.fontSize = 12;
            badge.Add(titleLabel);
            badge.Add(valueLabel);
            return badge;
        }

        private static VisualElement NewOrnamentRule(Color color)
        {
            var rule = new VisualElement();
            rule.style.height = 2;
            rule.style.marginBottom = 10;
            rule.style.backgroundColor = color;
            rule.style.opacity = 0.8f;
            return rule;
        }

        private static Label NewToast(string text)
        {
            var label = new Label(text);
            label.style.marginTop = 10;
            label.style.paddingLeft = 12;
            label.style.paddingRight = 12;
            label.style.paddingTop = 8;
            label.style.paddingBottom = 8;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.color = RuntimeArtCatalog.Text;
            label.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            label.style.borderTopLeftRadius = 8;
            label.style.borderTopRightRadius = 8;
            label.style.borderBottomLeftRadius = 8;
            label.style.borderBottomRightRadius = 8;
            label.style.borderLeftColor = RuntimeArtCatalog.Gold;
            label.style.borderLeftWidth = 2;
            return label;
        }

        private static void ApplyStatusChip(Label label, Color accent)
        {
            label.style.paddingLeft = 10;
            label.style.paddingRight = 10;
            label.style.paddingTop = 6;
            label.style.paddingBottom = 6;
            label.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            label.style.borderTopLeftRadius = 8;
            label.style.borderTopRightRadius = 8;
            label.style.borderBottomLeftRadius = 8;
            label.style.borderBottomRightRadius = 8;
            label.style.borderLeftColor = accent;
            label.style.borderLeftWidth = 2;
        }

        private void SetToast(string text, Color accent)
        {
            if (_toast == null) return;
            _toast.text = text;
            _toast.style.borderLeftColor = accent;
        }

        private static string Required(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static string Abbrev(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= 18) return value;
            return value.Substring(0, 8) + ".." + value.Substring(value.Length - 6);
        }

        private static PanelSettings ResolvePanelSettings()
        {
            var existingDocuments = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
            foreach (var document in existingDocuments)
                if (document != null && document.panelSettings != null) return document.panelSettings;

            var resourceSettings = Resources.Load<PanelSettings>("LGORuntimePanelSettings");
            if (resourceSettings != null) return resourceSettings;

            var settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.name = "LGO Runtime Panel Settings";
            return settings;
        }
    }
}
