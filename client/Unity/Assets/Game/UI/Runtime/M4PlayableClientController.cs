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
        private Label _combatRangeStatus;
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
            _root.style.unityBackgroundImageTintColor = Color.white;
            var gateBackground = LgoVisualAssetRegistryV3B.LoginBackgroundSpiritGate ?? LgoVisualAssetRegistryV2.LoginBackgroundSpiritGate;
            if (gateBackground != null)
            {
                _root.style.backgroundImage = new StyleBackground(gateBackground);
                _root.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
            }

            BuildHeader();

            _mainShell = new VisualElement();
            _mainShell.style.flexDirection = FlexDirection.Row;
            _mainShell.style.flexWrap = Wrap.Wrap;
            _mainShell.style.width = Length.Percent(100);
            _mainShell.style.maxWidth = 1120;
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
            header.style.maxWidth = 1120;
            _root.Add(header);

            var brand = new VisualElement();
            brand.style.flexDirection = FlexDirection.Column;
            header.Add(brand);

            var logo = new VisualElement();
            logo.name = "LGO Login Gate Entry Logo V2";
            logo.style.width = 360;
            logo.style.height = 132;
            logo.style.marginBottom = 2;
            var logoTexture = LgoVisualAssetRegistryV2.LogoLinhGioiOnline;
            if (logoTexture != null) logo.style.backgroundImage = new StyleBackground(logoTexture);
            brand.Add(logo);

            var title = new Label("Linh Giới Online");
            title.style.fontSize = 24;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = RuntimeArtCatalog.Gold;
            brand.Add(title);

            var subtitle = new Label("Cổng Linh Giới - bản thử nghiệm nội bộ");
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
            _quitButton = NewQuietButton("Thoát", QuitPlayer);
            _quitButton.tooltip = "Esc mở menu phiên trong thế giới; Thoát đóng bản chơi thử.";
            right.Add(_quitButton);
            header.Add(right);
        }

        private void BuildAuthPanel()
        {
            _authPanel = NewPanel(620);
            ApplyV2PanelSkin(_authPanel);
            _mainShell.Add(_authPanel);
            _authPanel.Add(NewSectionTitle("Linh Môn"));
            _authPanel.Add(NewOrnamentRule(RuntimeArtCatalog.Spirit));
            var gateKeeper = new VisualElement();
            gateKeeper.name = "LGO Login Gate Keeper NPC V2";
            gateKeeper.style.width = 176;
            gateKeeper.style.height = 264;
            gateKeeper.style.alignSelf = Align.Center;
            gateKeeper.style.marginBottom = 8;
            var gateKeeperTexture = LgoVisualAssetRegistryV3B.GateKeeperNpcLoginTexture ?? LgoVisualAssetRegistryV2.GateKeeperNpcLoginTexture;
            if (gateKeeperTexture != null) gateKeeper.style.backgroundImage = new StyleBackground(gateKeeperTexture);
            _authPanel.Add(gateKeeper);

            var serverRow = new VisualElement();
            serverRow.name = "LGO Login Server Selector V2";
            serverRow.style.flexDirection = FlexDirection.Row;
            serverRow.style.alignItems = Align.Center;
            serverRow.style.marginBottom = 8;
            serverRow.style.paddingLeft = 10;
            serverRow.style.paddingRight = 10;
            serverRow.style.paddingTop = 8;
            serverRow.style.paddingBottom = 8;
            var serverPanel = LgoVisualAssetRegistryV2.ServerSelectorPanelTexture;
            if (serverPanel != null) serverRow.style.backgroundImage = new StyleBackground(serverPanel);
            serverRow.Add(NewIcon(LgoVisualAssetRegistryV2.IconServerTexture, "Máy chủ"));
            serverRow.Add(NewMutedLabel("Máy chủ thử nghiệm: Linh Môn 01"));
            serverRow.Add(NewIcon(LgoVisualAssetRegistryV2.ServerOnlineTexture, "Đang mở"));
            _authPanel.Add(serverRow);

            _authPanel.Add(NewMutedLabel("API: " + _config.apiBaseUrl));
            _devKey = NewTextField("Khóa thử nghiệm", DefaultDevKey);
            _authPanel.Add(_devKey);
            _loginButton = NewPrimaryButton("Vào Thế Giới", () => RunAsync(LoginAsync));
            _authPanel.Add(_loginButton);
            var utilities = NewButtonRow(
                NewIconButton("Tin", LgoVisualAssetRegistryV2.IconNoticeTexture, () => SetToast("Thông báo: bản thử nghiệm nội bộ.", RuntimeArtCatalog.Spirit)),
                NewIconButton("Tài khoản", LgoVisualAssetRegistryV2.IconAccountTexture, () => SetToast("Tài khoản dev sẽ được mở bằng khóa hiện tại.", RuntimeArtCatalog.Muted)),
                NewIconButton("Cài đặt", LgoVisualAssetRegistryV2.IconSettingsTexture, () => SetToast("Cài đặt nhanh có trong menu phiên.", RuntimeArtCatalog.Muted))
            );
            _authPanel.Add(utilities);
            _account = NewMutedLabel("Tài khoản: chưa kết nối");
            _account.style.marginTop = 10;
            _authPanel.Add(_account);
        }

        private void BuildLobbyPanel()
        {
            _lobbyPanel = NewPanel(840);
            ApplyV2PanelSkin(_lobbyPanel);
            _mainShell.Add(_lobbyPanel);
            _lobbyPanel.Add(NewSectionTitle("Điện Nhân Vật"));
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
            _selectedName = new Label("Chưa chọn nhân vật");
            _selectedName.style.fontSize = 19;
            _selectedName.style.unityFontStyleAndWeight = FontStyle.Bold;
            _selectedName.style.color = RuntimeArtCatalog.Gold;
            preview.Add(_selectedName);
            _selectedMeta = NewMutedLabel("Tạo một tu sĩ để bước vào Linh Giới.");
            preview.Add(_selectedMeta);
            content.Add(preview);

            var create = new VisualElement();
            create.style.marginTop = 14;
            create.style.paddingTop = 12;
            create.style.borderTopColor = RuntimeArtCatalog.SurfaceRaised;
            create.style.borderTopWidth = 1;
            _lobbyPanel.Add(create);

            create.Add(NewSectionTitle("Tạo Tu Sĩ"));
            _characterName = NewTextField("Tên nhân vật", "LinhGioiHero");
            _classId = NewTextField("Mã lớp tu luyện", DefaultClassId);
            _createButton = NewSecondaryButton("Tạo", () => RunAsync(CreateCharacterAsync));
            _enterWorldButton = NewPrimaryButton("Vào sân luyện", () => RunAsync(EnterWorldAsync));
            create.Add(_characterName);
            create.Add(_classId);
            create.Add(NewButtonRow(_createButton, _enterWorldButton));
        }

        private void BuildWorldHud()
        {
            _worldHud = NewPanel(760);
            _mainShell.Add(_worldHud);
            _worldHud.Add(NewSectionTitle("Sân Luyện An Toàn"));
            _worldHud.Add(NewOrnamentRule(RuntimeArtCatalog.Spirit));

            var topStrip = new VisualElement();
            topStrip.style.flexDirection = FlexDirection.Row;
            topStrip.style.flexWrap = Wrap.Wrap;
            topStrip.style.marginBottom = 10;
            _worldHud.Add(topStrip);

            topStrip.Add(NewBadge("Tài khoản", "đã kết nối"));
            topStrip.Add(NewBadge("Lưu vị trí", "API nội bộ"));
            topStrip.Add(NewBadge("Di chuyển", "WASD hoặc phím mũi tên"));
            topStrip.Add(NewBadge("Xoay", "Q / E"));
            topStrip.Add(NewBadge("Tương tác", "F hoặc Space"));
            topStrip.Add(NewBadge("Menu", "Esc"));

            _worldName = new Label("Chưa chọn nhân vật");
            _worldName.style.fontSize = 19;
            _worldName.style.unityFontStyleAndWeight = FontStyle.Bold;
            _worldName.style.color = RuntimeArtCatalog.Gold;
            _worldHud.Add(_worldName);

            _worldMeta = NewMutedLabel("Chọn nhân vật tại điện nhân vật.");
            _worldHud.Add(_worldMeta);

            _worldArea = NewStatusLabel("Khu vực: xem trước tại sảnh", RuntimeArtCatalog.Muted);
            _worldHud.Add(_worldArea);

            _worldStep = NewStatusLabel("Vòng hướng dẫn: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_worldStep);

            _worldDirection = NewStatusLabel("Chỉ dẫn: vào thế giới để hiện mốc sân luyện.", RuntimeArtCatalog.Gold);
            _worldHud.Add(_worldDirection);

            _worldPoseState = NewStatusLabel("Tư thế: nhân vật đứng yên / Người Giữ Cổng chờ / Bóng Tối đứng yên.", RuntimeArtCatalog.Muted);
            _worldHud.Add(_worldPoseState);

            _worldVfxState = NewStatusLabel("Hiệu ứng: yên tĩnh / cổng, mạch linh khí, chém gió, cảnh báo bóng đều chỉ là hình ảnh.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_worldVfxState);

            _skinSource = NewStatusLabel("Nguồn giao diện: asset runtime tối ưu, chưa phải art final.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_skinSource);

            _worldObjective = NewStatusLabel("Mục tiêu 1/2: trò chuyện với Người Giữ Cổng.", RuntimeArtCatalog.Gold);
            _worldHud.Add(_worldObjective);

            _interactionHint = NewStatusLabel("Di chuyển tới gần Người Giữ Cổng.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_interactionHint);

            _position = NewMutedLabel("x=0.00 y=0.00 z=0.00 yaw=0.0");
            _position.style.marginTop = 8;
            _position.style.backgroundColor = RuntimeArtCatalog.Background;
            _position.style.paddingLeft = 10;
            _position.style.paddingRight = 10;
            _position.style.paddingTop = 6;
            _position.style.paddingBottom = 6;
            _worldHud.Add(_position);

            _worldLandmarks = NewMutedLabel("Mốc sân luyện: Linh Môn phía nam / Người Giữ Cổng tây bắc / Đá Luyện phía bắc / Bia đọc mục tiêu phía đông / Bóng Tối xa phía đông.");
            _worldLandmarks.style.marginTop = 8;
            _worldHud.Add(_worldLandmarks);

            _toast = NewToast("Linh Môn đã sẵn sàng.");
            _worldHud.Add(_toast);

            BuildSessionMenuPanel();
            BuildSkillPreviewPanel();
            BuildLocalCombatPanel();

            _dialoguePanel = NewPreviewPanel();
            _dialoguePanel.style.marginTop = 10;
            _dialogueSpeaker = new Label("Người Giữ Cổng");
            _dialogueSpeaker.style.fontSize = 17;
            _dialogueSpeaker.style.unityFontStyleAndWeight = FontStyle.Bold;
            _dialogueSpeaker.style.color = RuntimeArtCatalog.Gold;
            _dialogueLine = NewMutedLabel("Đối thoại đã đóng.");
            _dialogueProgress = NewStatusLabel("Đối thoại: 0/3", RuntimeArtCatalog.Muted);
            _dialogueContinueButton = NewSecondaryButton("Tiếp tục", ContinueDialogue);
            _dialogueCloseButton = NewQuietButton("Đóng", CloseDialogue);
            _dialoguePanel.Add(_dialogueSpeaker);
            _dialoguePanel.Add(_dialogueLine);
            _dialoguePanel.Add(_dialogueProgress);
            _dialoguePanel.Add(NewButtonRow(_dialogueContinueButton, _dialogueCloseButton));
            _worldHud.Add(_dialoguePanel);
            SetDialogueVisible(false);

            _savePositionButton = NewPrimaryButton("Lưu vị trí", () => RunAsync(SavePositionAsync));
            _savePositionButton.tooltip = "Ghi vị trí hiện tại vào API nội bộ.";
            _backButton = NewSecondaryButton("Về điện nhân vật", BackToLobby);
            _backButton.tooltip = "Quay lại quản lý nhân vật mà không đóng bản chơi thử.";
            _worldHud.Add(NewButtonRow(_savePositionButton, _backButton, NewQuietButton("Thoát", QuitPlayer)));
        }

        private void BuildSessionMenuPanel()
        {
            _sessionMenuPanel = NewPreviewPanel();
            _sessionMenuPanel.name = "LGO Session Menu Overlay";
            _sessionMenuPanel.style.marginTop = 10;
            _sessionMenuPanel.style.backgroundColor = RuntimeArtCatalog.Background;
            _sessionMenuPanel.style.borderLeftColor = RuntimeArtCatalog.Gold;
            _sessionMenuPanel.style.borderLeftWidth = 2;
            _sessionMenuPanel.Add(NewSectionTitle("Menu phiên"));
            _sessionMenuStatus = NewMutedLabel("Đang tạm dừng trong sân luyện.");
            _sessionMenuPanel.Add(_sessionMenuStatus);
            _resumeButton = NewPrimaryButton("Tiếp tục", HideSessionMenu);
            _sessionSaveButton = NewSecondaryButton("Lưu vị trí", () => RunAsync(SavePositionAsync));
            _sessionBackButton = NewSecondaryButton("Về điện nhân vật", BackToLobby);
            _sessionQuitButton = NewQuietButton("Thoát", QuitPlayer);
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
            _skillPreviewPanel.Add(NewSectionTitle("Xem thử kỹ năng"));
            _skillPreviewPanel.Add(NewMutedLabel("Chỉ diễn tập hình ảnh cục bộ. Không tạo đối thủ, luật thời gian, tiến trình hay kết quả thật."));
            _previewWindSlashButton = NewSecondaryButton("Chém Gió", () => PreviewSkill("Wind Slash", "Chém Gió"));
            _previewShadowBindButton = NewSecondaryButton("Trói Bóng", () => PreviewSkill("Shadow Bind", "Trói Bóng"));
            _previewSpiritGuardButton = NewSecondaryButton("Hộ Linh", () => PreviewSkill("Spirit Guard", "Hộ Linh"));
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
            _combatRangeStatus = NewStatusLabel("Tầm đánh: chưa vào sân.", RuntimeArtCatalog.Muted);
            _combatVisualState = NewStatusLabel("Dấu hiệu mục tiêu: chưa chọn.", RuntimeArtCatalog.Gold);
            _combatFeedback = NewStatusLabel("Chưa phải chiến đấu thật.", RuntimeArtCatalog.Spirit);
            _combatCooldown = NewStatusLabel("Hồi chiêu: Sẵn sàng", RuntimeArtCatalog.Muted);
            _combatAuthority = NewStatusLabel("Mô phỏng cục bộ: chưa gửi ý định chiến đấu.", RuntimeArtCatalog.Spirit);
            _localCombatButton = NewSecondaryButton("Gửi ý định chiến đấu", TriggerLocalCombat);
            _localCombatButton.tooltip = "Kích hoạt phản hồi đánh thử cục bộ. Đánh thử cục bộ: xem vòng chọn mục tiêu, hit flash và nhịp hồi chiêu; không phải chiến đấu thật";
            ApplyCombatButtonSkin(_localCombatButton, CombatPlaceholderAssets.CombatButtonNormalTexture);
            _localCombatPanel.Add(_combatCooldownIcon);
            _localCombatPanel.Add(_combatTargetStatus);
            _localCombatPanel.Add(_combatRangeStatus);
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
            _settingsPanel.Add(NewSectionTitle("Cài đặt cục bộ"));
            _showPositionToggle = NewLocalSettingToggle("Hiện tọa độ", true, ApplyLocalSettings);
            _showHintsToggle = NewLocalSettingToggle("Hiện chỉ dẫn", true, ApplyLocalSettings);
            _focusModeToggle = NewLocalSettingToggle("Chế độ HUD gọn", false, ApplyLocalSettings);
            _settingsPanel.Add(_showPositionToggle);
            _settingsPanel.Add(_showHintsToggle);
            _settingsPanel.Add(_focusModeToggle);
            _sessionMenuPanel.Add(_settingsPanel);
        }

        private async Task LoginAsync()
        {
            SetBusy(true, "Đang mở Linh Môn...");
            var login = await _client.LoginDevAsync(Required(_devKey.value, DefaultDevKey), "M4 Playable Client", _shutdown.Token);
            _accountState = login.account;
            _account.text = "Tài khoản: " + Abbrev(_accountState.accountId) + " / " + _accountState.displayName;
            await RefreshCharactersAsync();
            ShowLobbyMode();
            SetBusy(false, "Điện Nhân Vật đã sẵn sàng.");
            SetToast("Tài khoản đã kết nối. Điện Nhân Vật đã mở.", RuntimeArtCatalog.Spirit);
        }

        private async Task RefreshCharactersAsync()
        {
            _characters = await _client.ListCharactersAsync(_accountState.accountId, _shutdown.Token);
            _characterList.Clear();
            _characterList.Add(NewMutedLabel(_characters.Length == 0 ? "Chưa có nhân vật." : "Chọn nhân vật."));
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
            SetBusy(true, "Đang tạo tu sĩ...");
            try
            {
                var created = await _client.CreateCharacterAsync(_accountState.accountId, Required(_characterName.value, "LinhGioiHero"), Required(_classId.value, DefaultClassId), _shutdown.Token);
                _selectedCharacter = created;
                await RefreshCharactersAsync();
                SetBusy(false, "Nhân vật đã sẵn sàng.");
                SetToast("Hồ sơ tu sĩ đã được tạo.", RuntimeArtCatalog.Gold);
            }
            catch (Exception exception)
            {
                SetApiError("tạo nhân vật", exception);
            }
        }

        private async Task EnterWorldAsync()
        {
            if (_selectedCharacter == null) return;
            SetBusy(true, "Đang vào sân luyện Linh Môn...");
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
            SetBusy(false, "Sân luyện đã sẵn sàng: làm theo Bước 1 rồi Bước 2.");
            SetToast("Linh Môn đã mở. Bước 1: trò chuyện với Người Giữ Cổng.", RuntimeArtCatalog.Spirit);
        }

        private async Task SavePositionAsync()
        {
            if (_selectedCharacter == null || _world == null) return;
            SetBusy(true, "Đang lưu vị trí vào API nội bộ...");
            var save = _world.BuildSaveRequest();
            _selectedCharacter = await _client.SaveCharacterPositionAsync(_selectedCharacter.characterId, save.x, save.y, save.z, save.yawDegrees, _shutdown.Token);
            UpdateSelectedPreview(_selectedCharacter);
            SetBusy(false, "Đã lưu vị trí gần " + _world.CurrentAreaLabel + ".");
            SetToast("Dấu ấn vị trí đã ghi gần " + _world.CurrentAreaLabel + ".", RuntimeArtCatalog.Gold);
        }

        private void BackToLobby()
        {
            SetSessionMenuVisible(false);
            ShowLobbyMode();
            SetBusy(false, "Đã quay lại Điện Nhân Vật.");
            SetToast("Đã quay lại Điện Nhân Vật.", RuntimeArtCatalog.Muted);
        }

        private void SelectCharacter(CharacterResponse character)
        {
            _selectedCharacter = character;
            UpdateSelectedPreview(character);
            _enterWorldButton.SetEnabled(character != null);
            _status.text = character == null ? "Tạo hoặc chọn tu sĩ" : "Đã chọn: " + character.name;
            SetToast(character == null ? "Đang chờ chọn tu sĩ." : "Đã chọn " + character.name + ".", RuntimeArtCatalog.Muted);
        }

        private void UpdateSelectedPreview(CharacterResponse character)
        {
            if (character == null)
            {
                _selectedName.text = "Chưa chọn nhân vật";
                _selectedMeta.text = "Tạo tu sĩ để bước vào Linh Giới.";
                _worldName.text = "Chưa chọn nhân vật";
                _worldMeta.text = "Chọn nhân vật tại điện nhân vật.";
                if (_worldArea != null) _worldArea.text = "Khu vực: xem trước tại sảnh";
                if (_worldStep != null) _worldStep.text = "Vòng hướng dẫn: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện.";
                if (_worldDirection != null) _worldDirection.text = "Chỉ dẫn: vào thế giới để hiện hướng dẫn Bước 1.";
                if (_worldLandmarks != null) _worldLandmarks.text = "Mốc sân luyện: Linh Môn phía nam / Người Giữ Cổng tây bắc / Đá Luyện phía bắc / Bia đọc mục tiêu phía đông / Bóng Tối xa phía đông.";
                if (_worldPoseState != null) _worldPoseState.text = "Tư thế: nhân vật đứng yên / Người Giữ Cổng chờ / Bóng Tối đứng yên.";
                if (_worldVfxState != null) _worldVfxState.text = "Hiệu ứng: yên tĩnh / cổng, mạch linh khí, chém gió, cảnh báo bóng đều chỉ là hình ảnh.";
                if (_skinSource != null) _skinSource.text = "Nguồn giao diện: asset runtime tối ưu, chưa phải art final.";
                if (_worldObjective != null) _worldObjective.text = "Mục tiêu 1/2: trò chuyện với Người Giữ Cổng.";
                if (_interactionHint != null) _interactionHint.text = "Di chuyển tới gần Người Giữ Cổng.";
                _position.text = "x=0.00 y=0.00 z=0.00 yaw=0.0";
                return;
            }
            _selectedName.text = character.name;
            _selectedMeta.text = "Lớp " + character.classId + " / " + Abbrev(character.characterId);
            _worldName.text = character.name;
            _worldMeta.text = "Lớp " + character.classId + " / " + Abbrev(character.characterId);
            _position.text = character.ToString();
        }

        private void RefreshWorldLoopLabels()
        {
            if (_world == null) return;
            if (_worldArea != null) _worldArea.text = "Khu vực: " + _world.CurrentAreaLabel;
            if (_worldStep != null) _worldStep.text = "Vòng hướng dẫn: " + _world.GuidedTrainingStepName;
            if (_worldDirection != null) _worldDirection.text = "Chỉ dẫn: " + _world.ObjectiveDirectionHint;
            if (_worldLandmarks != null) _worldLandmarks.text = _world.WorldLandmarkSummary;
            if (_worldPoseState != null) _worldPoseState.text = "Tư thế: nhân vật " + _world.PlayerPoseStateName + " / Người Giữ Cổng " + _world.GateKeeperPoseStateName + " / Bóng Tối " + _world.ShadowSlimeStateName + ".";
            if (_worldVfxState != null) _worldVfxState.text = "Hiệu ứng: " + _world.VfxFeedbackStateName + " / chỉ là phản hồi hình ảnh cục bộ.";
            if (_combatTargetStatus != null) _combatTargetStatus.text = _world.TargetDummyStatusText;
            if (_combatRangeStatus != null) _combatRangeStatus.text = _world.TargetDummyRangeText;
            if (_combatVisualState != null) _combatVisualState.text = _world.TargetDummyVisualStateText;
            if (_combatFeedback != null) _combatFeedback.text = _world.CombatFeedbackText;
            if (_combatCooldown != null) _combatCooldown.text = _world.CombatCooldownText;
            if (_combatAuthority != null) _combatAuthority.text = _world.CombatAuthorityText;
            RefreshCombatAssetUiState();
            if (_skinSource != null) _skinSource.text = "Nguồn giao diện: asset runtime tối ưu, chưa phải art final.";
            if (_worldObjective != null) _worldObjective.text = _world.ObjectiveText;
            if (_interactionHint != null) _interactionHint.text = _world.InteractionText;
            SetToast(_world.InteractionAcknowledged ? "Hoàn tất luyện tập. Hãy lưu vị trí hoặc về Điện Nhân Vật." : _world.InteractionText, RuntimeArtCatalog.Spirit);
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
                SetApiError("gửi yêu cầu API", exception);
            }
        }

        private void SetApiError(string action, Exception exception)
        {
            var message = "API bị chặn khi " + action + ": " + exception.Message;
            SetBusy(false, message);
            SetToast("API nội bộ chưa sẵn sàng hoặc từ chối yêu cầu. Kiểm tra server rồi thử lại.", RuntimeArtCatalog.Danger);
            if (_sessionMenuStatus != null)
                _sessionMenuStatus.text = "Lỗi API: kiểm tra server nội bộ rồi thử lại.";
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
            var sigil = new Label("LINH MÔN");
            sigil.style.color = RuntimeArtCatalog.Spirit;
            sigil.style.unityFontStyleAndWeight = FontStyle.Bold;
            preview.Add(sigil);
            return preview;
        }

        private static Label NewSectionTitle(string text)
        {
            var label = new Label(text);
            label.style.fontSize = 20;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.color = RuntimeArtCatalog.Text;
            label.style.marginBottom = 8;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
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
            button.style.minHeight = 58;
            button.style.fontSize = 16;
            var texture = LgoVisualAssetRegistryV3B.ButtonEnterWorldGoldTexture ?? LgoVisualAssetRegistryV2.ButtonPrimaryNormalTexture;
            if (texture != null) button.style.backgroundImage = new StyleBackground(texture);
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
            var texture = LgoVisualAssetRegistryV2.ButtonSecondaryTexture;
            if (texture != null) button.style.backgroundImage = new StyleBackground(texture);
            return button;
        }

        private static VisualElement NewIcon(Texture2D texture, string tooltip)
        {
            var icon = new VisualElement();
            icon.style.width = 28;
            icon.style.height = 28;
            icon.style.marginRight = 8;
            icon.style.marginLeft = 4;
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
            icon.tooltip = tooltip;
            return icon;
        }

        private static Button NewIconButton(string label, Texture2D texture, Action action)
        {
            var button = NewSecondaryButton(string.Empty, action);
            button.style.minWidth = 104;
            button.style.flexDirection = FlexDirection.Row;
            button.style.alignItems = Align.Center;
            button.Add(NewIcon(texture, label));
            var text = new Label(label);
            text.style.color = RuntimeArtCatalog.Text;
            text.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.Add(text);
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
            SetToast("Đã quay lại phiên chơi.", RuntimeArtCatalog.Muted);
        }

        private void SetSessionMenuVisible(bool visible)
        {
            if (_sessionMenuPanel == null) return;
            _sessionMenuPanel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_sessionMenuStatus != null)
                _sessionMenuStatus.text = visible ? "Đang tạm dừng trong sân luyện. Có thể tiếp tục, lưu vị trí, quay lại hoặc thoát." : "Phiên chơi đang hoạt động.";
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
                _combatCooldownIcon.style.borderTopColor = coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit;
                _combatCooldownIcon.style.borderLeftColor = coolingDown ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Spirit;
            }
            if (_localCombatButton != null)
            {
                ApplyCombatButtonSkin(_localCombatButton, coolingDown ? CombatPlaceholderAssets.CombatButtonCooldownTexture : CombatPlaceholderAssets.CombatButtonNormalTexture);
                _localCombatButton.text = coolingDown ? "Đang hồi chiêu" : "Tấn công thử";
                _localCombatButton.tooltip = coolingDown
                    ? "Bấm vẫn cho phản hồi từ chối hồi chiêu; đây là nguyên mẫu cục bộ, không phải chiến đấu thật."
                    : "Gửi ý định Chém Gió vào bia luyện tập; chỉ là phản hồi nguyên mẫu.";
            }
            var feedback = _world.CombatFeedbackText;
            var warning = feedback.Contains("Ngoài tầm") || feedback.Contains("Chưa chọn") || feedback.Contains("Đang hồi chiêu");
            ApplyStatusAccent(_combatRangeStatus, _world.TargetDummyRangeText.Contains("trong tầm") ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Danger);
            ApplyStatusAccent(_combatVisualState, coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit);
            ApplyStatusAccent(_combatFeedback, warning ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Gold);
            ApplyStatusAccent(_combatCooldown, coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit);
            ApplyStatusAccent(_combatAuthority, _world.CombatAuthorityText.Contains("từ chối") || _world.CombatAuthorityText.Contains("Từ chối") ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Spirit);
        }

        private void PreviewSkill(string previewName, string displayName)
        {
            if (_world == null) return;
            _world.PreviewSkillFeedback(previewName);
            RefreshWorldLoopLabels();
            SetToast("Xem thử: hiệu ứng " + displayName + " đã chạy trong sân an toàn.", RuntimeArtCatalog.Spirit);
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
            _dialogueProgress.text = "Đối thoại: " + _world.DialogueProgress;
            _dialogueContinueButton.text = _world.HasNextDialogueLine ? "Tiếp tục" : "Hoàn tất";
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
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
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
            button.tooltip = "Chọn nhân vật tu luyện";
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
            icon.style.borderTopWidth = 2;
            icon.style.borderLeftWidth = 2;
            icon.style.borderTopColor = RuntimeArtCatalog.Spirit;
            icon.style.borderLeftColor = RuntimeArtCatalog.Spirit;
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

        private static void ApplyV2PanelSkin(VisualElement panel)
        {
            var texture = LgoVisualAssetRegistryV3B.PanelMainDarkGoldTexture ?? LgoVisualAssetRegistryV2.PanelMainLargeTexture;
            if (texture == null) return;
            panel.style.backgroundImage = new StyleBackground(texture);
        }

        private static void ApplyCombatButtonSkin(Button button, Texture2D texture)
        {
            if (button == null || texture == null) return;
            button.style.backgroundImage = new StyleBackground(texture);
        }

        private static void ApplyStatusAccent(Label label, Color accent)
        {
            if (label == null) return;
            label.style.borderLeftColor = accent;
            label.style.color = accent;
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
