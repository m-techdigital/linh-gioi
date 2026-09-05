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
using static LinhGioi.UI.RuntimeUiFactory;

namespace LinhGioi.UI
{
    public sealed class M4PlayableClientController : MonoBehaviour
    {
        private const string DefaultDevKey = "m4-playable-dev-key";
        private const string DefaultClassId = "class.sword";
        private const bool UseLoginOrnatePanelTexture = false;
        private const string LoginResponsiveScaleCleanupMarker = "LGO Login Responsive Scale Cleanup v1";
        private const string LoginCtaBackingBalanceMarker = "LGO Login CTA Backing Balance v1";
        private AccountApiClient _client;
        private ClientRuntimeConfig _config;
        private CancellationTokenSource _shutdown;
        private UIDocument _document;
        private VisualElement _root;
        private VisualElement _screenScrim;
        private VisualElement _header;
        private VisualElement _headerActions;
        private VisualElement _mainShell;
        private VisualElement _authPanel;
        private VisualElement _lobbyPanel;
        private VisualElement _worldHud;
        private VisualElement _worldDebugStrip;
        private VisualElement _worldGuidanceCard;
        private VisualElement _dialoguePanel;
        private VisualElement _sessionMenuPanel;
        private VisualElement _settingsPanel;
        private VisualElement _skillPreviewPanel;
        private VisualElement _localCombatPanel;
        private VisualElement _worldFooterActions;
        private VisualElement _characterActionRow;
        private VisualElement _characterList;
        private VisualElement _lobbyContent;
        private VisualElement _selectedPreview;
        private VisualElement _createPanel;
        private VisualElement _emptyCharacterCard;
        private VisualElement _loginStage;
        private VisualElement _loginControlColumn;
        private VisualElement _loginGateKeeper;
        private VisualElement _loginNpcGrounding;
        private VisualElement _loginLogo;
        private VisualElement _loginCard;
        private VisualElement _loginServerRow;
        private VisualElement _serverStatusIcon;
        private Label _loginServerText;
        private Label _lobbyIntro;
        private Label _createHint;
        private Label _emptyCharacterHint;
        private TextField _devKey;
        private TextField _characterName;
        private TextField _classId;
        private Label _status;
        private Label _account;
        private Label _selectedName;
        private Label _selectedMeta;
        private Label _selectedStatus;
        private Label _selectedObjective;
        private Label _selectedClassSummary;
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
        private Label _layoutProfileLabel;
        private Label _dialogueSpeaker;
        private Label _dialogueLine;
        private Label _dialogueProgress;
        private Label _sessionMenuStatus;
        private Label _loginHeroTitle;
        private Label _loginHeroCopy;
        private Button _loginButton;
        private Button _serverSwitchButton;
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
        private string _lastLayoutProfile;
        private string _forcedLayoutProfile;
        private bool _isMobileProfile;

        public static M4PlayableClientController Attach(GameObject host)
        {
            return host.GetComponent<M4PlayableClientController>() ?? host.AddComponent<M4PlayableClientController>();
        }

        private void Awake()
        {
            _shutdown = new CancellationTokenSource();
            _config = ClientRuntimeConfig.LoadStreamingAssets();
            _client = new AccountApiClient(_config);
            _forcedLayoutProfile = NormalizeLayoutProfile(GetArg("--lgo-device-profile"));
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
            ApplyResponsiveLayoutProfile(false);
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
            _root.style.paddingLeft = 28;
            _root.style.paddingRight = 28;
            _root.style.paddingTop = 16;
            _root.style.paddingBottom = 18;
            _root.style.alignItems = Align.Center;
            _root.style.justifyContent = Justify.SpaceBetween;
            _root.style.unityBackgroundImageTintColor = Color.white;
            var runtimeFont = LoadRuntimeFont();
            if (runtimeFont != null) _root.style.unityFont = runtimeFont;
            ApplyLoginBackdrop(true);
            AddScreenScrim();

            BuildHeader();

            _mainShell = new VisualElement();
            _mainShell.style.flexDirection = FlexDirection.Row;
            _mainShell.style.flexWrap = Wrap.Wrap;
            _mainShell.style.width = Length.Percent(100);
            _mainShell.style.maxWidth = 1180;
            _mainShell.style.alignContent = Align.FlexStart;
            _mainShell.style.alignItems = Align.FlexStart;
            _mainShell.style.justifyContent = Justify.Center;
            _mainShell.style.flexGrow = 1;
            _mainShell.style.marginTop = 0;
            _root.Add(_mainShell);

            BuildAuthPanel();
            BuildLobbyPanel();
            BuildWorldHud();
            ApplyResponsiveLayoutProfile(true);
            ShowAuthMode();
        }

        private void BuildHeader()
        {
            var header = new VisualElement();
            _header = header;
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.width = Length.Percent(100);
            header.style.maxWidth = 1180;
            header.style.minHeight = 76;
            _root.Add(header);

            var brand = new VisualElement();
            brand.style.flexDirection = FlexDirection.Column;
            brand.style.alignItems = Align.FlexStart;
            brand.style.width = 300;
            brand.style.height = 42;
            header.Add(brand);

            _status = new Label("S1 - Linh Giới / Ổn định");
            _status.tooltip = "Cổng phiên hiện tại: " + _config.apiBaseUrl;
            _status.style.color = RuntimeArtCatalog.Muted;
            _status.style.unityTextAlign = TextAnchor.MiddleRight;
            _status.style.marginTop = 6;
            _status.style.display = DisplayStyle.None;
            ApplyStatusChip(_status, RuntimeArtCatalog.Muted);

            var right = new VisualElement();
            _headerActions = right;
            right.style.flexDirection = FlexDirection.Row;
            right.style.alignItems = Align.Center;
            right.Add(_status);
            _quitButton = NewQuietButton("Thoát", QuitPlayer);
            _quitButton.tooltip = "Esc mở menu phiên trong thế giới; Thoát đóng phiên hiện tại.";
            _quitButton.style.marginTop = 0;
            _quitButton.style.display = DisplayStyle.None;
            right.Add(_quitButton);
            header.Add(right);
        }

        private void AddScreenScrim()
        {
            _screenScrim = new VisualElement();
            _screenScrim.name = "LGO Login Gate Entry Readability Scrim";
            _screenScrim.pickingMode = PickingMode.Ignore;
            _screenScrim.style.position = Position.Absolute;
            _screenScrim.style.left = 0;
            _screenScrim.style.right = 0;
            _screenScrim.style.top = 0;
            _screenScrim.style.bottom = 0;
            _screenScrim.style.backgroundColor = new Color(0.02f, 0.05f, 0.10f, 0.18f);
            _root.Add(_screenScrim);
        }

        private void ApplyLoginBackdrop(bool enabled)
        {
            if (enabled)
            {
                _root.style.backgroundColor = RuntimeArtCatalog.Background;
                var gateBackground = LgoVisualAssetRegistryV3B.LoginBackgroundSpiritGate ?? LgoVisualAssetRegistryV2.LoginBackgroundSpiritGate;
                if (gateBackground != null)
                {
                    _root.style.backgroundImage = new StyleBackground(gateBackground);
                    _root.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
                }
                if (_screenScrim != null)
                    _screenScrim.style.backgroundColor = new Color(0.02f, 0.05f, 0.10f, 0.18f);
                return;
            }

            _root.style.backgroundImage = StyleKeyword.None;
            _root.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
            if (_screenScrim != null)
                _screenScrim.style.backgroundColor = new Color(0.01f, 0.03f, 0.07f, 0.08f);
        }

        private static Font LoadRuntimeFont()
        {
            var builtInFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (builtInFont != null) return builtInFont;
            try
            {
                var font = Font.CreateDynamicFontFromOSFont(
                    new[] { "Helvetica Neue", "Arial", "DejaVu Sans", "Noto Sans" },
                    18);
                if (font != null) return font;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[LinhGioi] Runtime UI font fallback unavailable: " + exception.Message);
            }
            return null;
        }

        private void BuildAuthPanel()
        {
            _authPanel = new VisualElement();
            _authPanel.name = "LGO Login Gate Entry Final Shell";
            _authPanel.style.width = Length.Percent(100);
            _authPanel.style.maxWidth = 1180;
            _authPanel.style.flexGrow = 1;
            _authPanel.style.minHeight = 560;
            _authPanel.style.flexDirection = FlexDirection.Row;
            _authPanel.style.justifyContent = Justify.FlexStart;
            _authPanel.style.alignItems = Align.Center;
            _authPanel.style.marginTop = 4;
            _authPanel.style.paddingTop = 8;
            _authPanel.style.paddingBottom = 8;
            _authPanel.style.position = Position.Relative;
            _mainShell.Add(_authPanel);

            _loginStage = new VisualElement();
            _loginStage.name = "LGO Login Gate Entry NPC Composition Stage V3B";
            _loginStage.style.position = Position.Absolute;
            _loginStage.style.right = 64;
            _loginStage.style.bottom = -88;
            _loginStage.style.width = 304;
            _loginStage.style.minHeight = 460;
            _loginStage.style.alignItems = Align.Center;
            _loginStage.style.justifyContent = Justify.FlexEnd;
            _loginStage.style.opacity = 0.93f;
            _authPanel.Add(_loginStage);

            var npcGrounding = new VisualElement();
            _loginNpcGrounding = npcGrounding;
            npcGrounding.name = "LGO Login Gate Keeper Soft Grounding Glow V3B";
            npcGrounding.pickingMode = PickingMode.Ignore;
            npcGrounding.style.position = Position.Absolute;
            npcGrounding.style.width = 232;
            npcGrounding.style.height = 20;
            npcGrounding.style.bottom = 24;
            npcGrounding.style.backgroundColor = new Color(0.005f, 0.018f, 0.035f, 0.26f);
            npcGrounding.style.opacity = 0.78f;
            RuntimeUiSkin.ApplyRadius(npcGrounding, 110);
            npcGrounding.tooltip = "LGO Login NPC Grounding Shadow Balance v1";
            _loginStage.Add(npcGrounding);

            var gateKeeperTexture = LgoVisualAssetRegistryV3B.GateKeeperNpcLoginTexture ?? LgoVisualAssetRegistryV2.GateKeeperNpcLoginTexture;
            var gateKeeper = NewImageLayer("LGO Login Gate Keeper NPC V3B", gateKeeperTexture, ScaleMode.ScaleToFit);
            _loginGateKeeper = gateKeeper;
            gateKeeper.style.width = 292;
            gateKeeper.style.height = 438;
            _loginStage.Add(gateKeeper);

            var controlColumn = new VisualElement();
            _loginControlColumn = controlColumn;
            controlColumn.name = "LGO Login Gate Entry Control Column V3B Final";
            controlColumn.style.width = Length.Percent(58);
            controlColumn.style.maxWidth = 600;
            controlColumn.style.minWidth = 300;
            controlColumn.style.flexShrink = 1;
            controlColumn.style.alignItems = Align.Center;
            controlColumn.style.justifyContent = Justify.Center;
            controlColumn.style.paddingBottom = 12;
            controlColumn.style.marginLeft = 22;
            controlColumn.style.marginTop = 12;
            _authPanel.Add(controlColumn);

            var logoLockup = NewImageLayer("LGO Login Gate Entry V3B Final Logo Text Lockup", LgoVisualAssetRegistryV3B.LogoLinhGioiOnline, ScaleMode.ScaleToFit);
            _loginLogo = logoLockup;
            logoLockup.style.width = 500;
            logoLockup.style.height = 250;
            logoLockup.style.alignItems = Align.Center;
            logoLockup.style.justifyContent = Justify.Center;
            logoLockup.style.marginBottom = -12;
            logoLockup.style.opacity = 0.96f;
            controlColumn.Add(logoLockup);

            _loginHeroTitle = new Label("Bước qua Linh Môn");
            _loginHeroTitle.name = "LGO Login Gate Entry Hero Title";
            _loginHeroTitle.style.fontSize = 25;
            _loginHeroTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            _loginHeroTitle.style.color = RuntimeArtCatalog.Text;
            _loginHeroTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
            _loginHeroTitle.style.marginBottom = 4;
            _loginHeroTitle.style.display = DisplayStyle.None;
            controlColumn.Add(_loginHeroTitle);

            _loginHeroCopy = NewMutedLabel("\"Tu tiên không chỉ là sức mạnh, mà là hành trình trở về chính mình.\"");
            _loginHeroCopy.name = "LGO Login Gate Entry Hero Copy";
            _loginHeroCopy.style.fontSize = 15;
            _loginHeroCopy.style.maxWidth = 560;
            _loginHeroCopy.style.color = RuntimeArtCatalog.Text;
            _loginHeroCopy.style.unityTextAlign = TextAnchor.MiddleCenter;
            _loginHeroCopy.style.marginBottom = 12;
            _loginHeroCopy.style.display = DisplayStyle.None;
            controlColumn.Add(_loginHeroCopy);

            _loginCard = new VisualElement();
            _loginCard.name = "LGO Login Gate Entry Bottom CTA v3 Final Panel V3B";
            _loginCard.style.width = Length.Percent(100);
            _loginCard.style.maxWidth = 480;
            _loginCard.style.minHeight = 152;
            _loginCard.style.alignItems = Align.Center;
            _loginCard.style.justifyContent = Justify.Center;
            _loginCard.style.paddingLeft = 28;
            _loginCard.style.paddingRight = 28;
            _loginCard.style.paddingTop = 20;
            _loginCard.style.paddingBottom = 22;
            _loginCard.style.marginBottom = 18;
            _loginCard.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            RuntimeUiSkin.ApplyLoginCtaBacking(_loginCard);
            _loginCard.tooltip = LoginCtaBackingBalanceMarker;
            if (UseLoginOrnatePanelTexture && LgoVisualAssetRegistryV3B.PanelMainDarkGoldTexture != null)
            {
                _loginCard.style.backgroundImage = new StyleBackground(LgoVisualAssetRegistryV3B.PanelMainDarkGoldTexture);
            }
            controlColumn.Add(_loginCard);
            _loginCard.Add(NewLoginOrnamentRule("LGO Login CTA Lightweight Top Ornament v1"));

            var serverRow = new VisualElement();
            _loginServerRow = serverRow;
            serverRow.name = "LGO Login Server Selector V3B";
            serverRow.style.flexDirection = FlexDirection.Row;
            serverRow.style.alignItems = Align.Center;
            serverRow.style.justifyContent = Justify.SpaceBetween;
            serverRow.style.width = Length.Percent(100);
            serverRow.style.maxWidth = 436;
            serverRow.style.minHeight = 42;
            serverRow.style.paddingLeft = 22;
            serverRow.style.paddingRight = 22;
            serverRow.style.paddingTop = 7;
            serverRow.style.paddingBottom = 7;
            RuntimeUiSkin.ApplyServerSelectorFrame(serverRow);
            serverRow.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            var serverText = new Label("S1 - Linh Giới");
            _loginServerText = serverText;
            serverText.style.flexGrow = 1;
            serverText.style.fontSize = 18;
            serverText.style.unityFontStyleAndWeight = FontStyle.Bold;
            serverText.style.color = RuntimeArtCatalog.Text;
            serverText.style.unityTextAlign = TextAnchor.MiddleCenter;
            serverRow.Add(serverText);
            _serverStatusIcon = new VisualElement();
            _serverStatusIcon.name = "LGO Login Server Online Dot";
            _serverStatusIcon.style.width = 12;
            _serverStatusIcon.style.height = 12;
            _serverStatusIcon.style.borderTopLeftRadius = 6;
            _serverStatusIcon.style.borderTopRightRadius = 6;
            _serverStatusIcon.style.borderBottomLeftRadius = 6;
            _serverStatusIcon.style.borderBottomRightRadius = 6;
            _serverStatusIcon.style.backgroundColor = RuntimeArtCatalog.Spirit;
            _serverStatusIcon.tooltip = "Đang mở";
            serverRow.Add(_serverStatusIcon);
            _loginCard.Add(serverRow);

            var apiLabel = NewMutedLabel("Cổng phiên hiện tại: " + _config.apiBaseUrl);
            apiLabel.style.fontSize = 12;
            apiLabel.style.display = DisplayStyle.None;
            _loginCard.Add(apiLabel);
            _devKey = NewTextField("Khóa thử nghiệm", DefaultDevKey);
            _devKey.style.display = DisplayStyle.None;
            _loginCard.Add(_devKey);
            _loginButton = NewPrimaryButton("Vào Thế Giới", () => RunAsync(LoginAsync));
            _loginButton.name = "LGO Login Enter World CTA Final v2";
            _loginButton.style.width = Length.Percent(100);
            _loginButton.style.maxWidth = 436;
            _loginButton.style.minHeight = 58;
            _loginButton.style.fontSize = 22;
            _loginButton.style.marginTop = 12;
            _loginButton.style.marginRight = 0;
            _loginButton.style.color = new Color(0.12f, 0.06f, 0.01f, 1f);
            _loginButton.style.backgroundColor = Color.clear;
            _loginButton.tooltip = "Mở tài khoản thử nghiệm và đi tới Điện Nhân Vật.";
            _loginCard.Add(_loginButton);
            _loginCard.Add(NewLoginOrnamentRule("LGO Login CTA Lightweight Bottom Ornament v1"));
            _serverSwitchButton = NewQuietButton("Chọn Máy Chủ", () => SetToast("S1 - Linh Giới đang mở ổn định.", RuntimeArtCatalog.Spirit));
            _serverSwitchButton.name = "LGO Login Server Switch Secondary";
            _serverSwitchButton.style.minWidth = 160;
            _serverSwitchButton.style.minHeight = 32;
            _serverSwitchButton.style.marginTop = 3;
            _serverSwitchButton.style.marginRight = 0;
            _serverSwitchButton.style.display = DisplayStyle.None;
            _loginCard.Add(_serverSwitchButton);
            _account = NewMutedLabel("Tài khoản: chưa kết nối");
            _account.style.marginTop = 10;
            _account.style.unityTextAlign = TextAnchor.MiddleCenter;
            _account.style.fontSize = 13;
            _account.style.display = DisplayStyle.None;
            _loginCard.Add(_account);
        }

        private void BuildLobbyPanel()
        {
            _lobbyPanel = NewPanel(840);
            _lobbyPanel.name = "LGO Character Hall V3B Composition Panel";
            RuntimeUiSkin.ApplyCharacterHallPanelFrame(_lobbyPanel);
            _lobbyPanel.style.maxWidth = 800;
            _lobbyPanel.style.minHeight = 452;
            _lobbyPanel.style.paddingLeft = 18;
            _lobbyPanel.style.paddingRight = 18;
            _lobbyPanel.style.paddingTop = 16;
            _lobbyPanel.style.paddingBottom = 18;
            _lobbyPanel.style.alignSelf = Align.FlexStart;
            _mainShell.Add(_lobbyPanel);
            _lobbyPanel.Add(NewSectionTitle("Điện Nhân Vật"));
            _lobbyPanel.Add(NewOrnamentRule(RuntimeArtCatalog.Gold));
            var lobbyIntro = NewMutedLabel("Chọn tu sĩ để bước qua Linh Môn. Hồ sơ sẽ được chuẩn bị cho phiên hiện tại.");
            _lobbyIntro = lobbyIntro;
            lobbyIntro.style.marginBottom = 10;
            lobbyIntro.style.unityTextAlign = TextAnchor.MiddleCenter;
            _lobbyPanel.Add(lobbyIntro);

            _lobbyContent = new VisualElement();
            _lobbyContent.name = "LGO Character Hall Main Selection Grid V3B";
            _lobbyContent.style.flexDirection = FlexDirection.Row;
            _lobbyContent.style.flexWrap = Wrap.NoWrap;
            _lobbyContent.style.justifyContent = Justify.SpaceBetween;
            _lobbyContent.style.marginTop = 4;
            _lobbyContent.style.marginBottom = 10;
            _lobbyPanel.Add(_lobbyContent);

            _characterList = new VisualElement();
            _characterList.style.minWidth = 280;
            _characterList.style.maxWidth = 390;
            _characterList.style.flexGrow = 1;
            _characterList.style.marginRight = 14;
            _characterList.style.marginBottom = 8;
            _characterList.style.paddingLeft = 14;
            _characterList.style.paddingRight = 14;
            _characterList.style.paddingTop = 12;
            _characterList.style.paddingBottom = 12;
            RuntimeUiSkin.ApplyCharacterListFrame(_characterList);
            _lobbyContent.Add(_characterList);

            _selectedPreview = NewPreviewPanel("TU SĨ", "Hồ sơ đang chọn");
            _selectedPreview.name = "LGO Character Hall Selected Cultivator Card V3B";
            _selectedPreview.style.maxWidth = 350;
            RuntimeUiSkin.ApplyCharacterPreviewFrame(_selectedPreview);
            var profileHero = new VisualElement();
            profileHero.style.flexDirection = FlexDirection.Row;
            profileHero.style.alignItems = Align.Center;
            profileHero.style.marginBottom = 10;
            var portraitTexture = LgoVisualAssetRegistryV3B.PlayerMaleCultivatorTexture;
            var portrait = NewImageLayer("LGO Character Hall V3B Cultivator Portrait", portraitTexture, ScaleMode.ScaleToFit);
            portrait.style.width = 92;
            portrait.style.height = 128;
            portrait.style.marginRight = 12;
            RuntimeUiSkin.ApplyCharacterPortraitFrame(portrait);
            if (portraitTexture == null)
                portrait.Add(NewRuntimeIcon(LgoVisualAssetRegistryV2.IconAccountTexture, 58, "Hồ sơ tu sĩ"));
            profileHero.Add(portrait);
            var profileCopy = new VisualElement();
            profileCopy.style.flexGrow = 1;
            _selectedName = new Label("Chưa chọn nhân vật");
            _selectedName.style.fontSize = 21;
            _selectedName.style.unityFontStyleAndWeight = FontStyle.Bold;
            _selectedName.style.color = RuntimeArtCatalog.Gold;
            profileCopy.Add(_selectedName);
            _selectedMeta = NewMutedLabel("Tạo một tu sĩ để bước vào Linh Giới.");
            profileCopy.Add(_selectedMeta);
            profileHero.Add(profileCopy);
            _selectedPreview.Add(profileHero);
            _selectedStatus = NewStatusLabel("Trạng thái: Chọn tu sĩ trước khi vào sân luyện.", RuntimeArtCatalog.Spirit);
            _selectedObjective = NewStatusLabel("Mục tiêu: Bước qua Linh Môn, kiểm tra HUD, rồi lưu vị trí.", RuntimeArtCatalog.Gold);
            _selectedClassSummary = NewStatusLabel("Mạch tu luyện: Kiếm tu sơ nhập / vai trò cân bằng.", RuntimeArtCatalog.Muted);
            _selectedPreview.Add(_selectedStatus);
            _selectedPreview.Add(_selectedObjective);
            _selectedPreview.Add(_selectedClassSummary);
            _lobbyContent.Add(_selectedPreview);

            _createPanel = new VisualElement();
            _createPanel.name = "LGO Character Hall Create Cultivator Panel V3B";
            _createPanel.style.marginTop = 10;
            _createPanel.style.position = Position.Relative;
            _createPanel.style.paddingLeft = 16;
            _createPanel.style.paddingRight = 16;
            _createPanel.style.paddingTop = 12;
            _createPanel.style.paddingBottom = 14;
            _createPanel.style.minHeight = 122;
            RuntimeUiSkin.ApplyCharacterCreateFrame(_createPanel);
            _createPanel.style.maxHeight = 210;
            _lobbyPanel.Add(_createPanel);

            _createPanel.Add(NewSectionTitle("Tạo Tu Sĩ"));
            var createHint = NewStatusLabel("Mạch tu luyện khởi đầu: Kiếm tu sơ nhập.", RuntimeArtCatalog.Muted);
            _createHint = createHint;
            createHint.name = "LGO Character Create Form Game Copy v1";
            _createPanel.Add(createHint);
            _characterName = NewTextField("Danh xưng", "LinhGioiHero");
            _characterName.name = "LGO Character Create Form Framed Input v1";
            _characterName.style.maxWidth = 360;
            ApplyLobbyInputStyle(_characterName);
            _classId = NewTextField("Mã lớp tu luyện", DefaultClassId);
            _classId.style.display = DisplayStyle.None;
            _createButton = NewCompactSecondaryButton("Tạo tu sĩ", () => RunAsync(CreateCharacterAsync));
            _enterWorldButton = NewCompactPrimaryButton("Vào sân luyện", () => RunAsync(EnterWorldAsync));
            _createPanel.Add(_characterName);
            _createPanel.Add(_classId);
            _characterActionRow = NewActionRow("LGO Character Hall Action Row", Justify.FlexStart, 6, 0, _createButton, _enterWorldButton);
            _createPanel.Add(_characterActionRow);
        }

        private void BuildWorldHud()
        {
            _worldHud = NewPanel(390);
            _worldHud.name = "LGO World HUD Action Shell V3B Skin v1";
            _worldHud.style.maxWidth = 390;
            _worldHud.style.alignSelf = Align.FlexStart;
            _worldHud.style.paddingLeft = 12;
            _worldHud.style.paddingRight = 12;
            _worldHud.style.paddingTop = 10;
            _worldHud.style.paddingBottom = 10;
            _mainShell.Add(_worldHud);
            _worldHud.Add(NewSectionTitle("Sân Luyện An Toàn"));
            _worldHud.Add(NewOrnamentRule(RuntimeArtCatalog.Spirit));

            _worldDebugStrip = new VisualElement();
            _worldDebugStrip.style.flexDirection = FlexDirection.Row;
            _worldDebugStrip.style.flexWrap = Wrap.Wrap;
            _worldDebugStrip.style.marginBottom = 10;
            _worldDebugStrip.style.display = DisplayStyle.None;
            _worldHud.Add(_worldDebugStrip);

            _worldDebugStrip.Add(NewBadge("Tài khoản", "đã kết nối"));
            _worldDebugStrip.Add(NewBadge("Lưu vị trí", "phiên hiện tại"));
            _worldDebugStrip.Add(NewBadge("Di chuyển", "WASD hoặc phím mũi tên"));
            _worldDebugStrip.Add(NewBadge("Xoay", "Q / E"));
            _worldDebugStrip.Add(NewBadge("Tương tác", "F hoặc Space"));
            _worldDebugStrip.Add(NewBadge("Menu", "Esc"));

            _layoutProfileLabel = NewStatusLabel("Bố cục: desktop / HUD tinh gọn.", RuntimeArtCatalog.Muted);
            _layoutProfileLabel.style.display = DisplayStyle.None;
            _worldHud.Add(_layoutProfileLabel);

            _worldName = new Label("Chưa chọn nhân vật");
            _worldName.style.fontSize = 17;
            _worldName.style.unityFontStyleAndWeight = FontStyle.Bold;
            _worldName.style.color = RuntimeArtCatalog.Gold;
            _worldName.style.marginTop = 6;
            _worldHud.Add(_worldName);

            _worldMeta = NewMutedLabel("Chọn nhân vật tại điện nhân vật.");
            _worldMeta.style.fontSize = 12;
            _worldHud.Add(_worldMeta);

            var guidanceCard = NewWorldHudGroup("LGO World Guidance Card V3B", RuntimeArtCatalog.Spirit);
            _worldGuidanceCard = guidanceCard;
            _worldArea = NewStatusLabel("Khu vực: xem trước tại sảnh", RuntimeArtCatalog.Muted);
            ApplyHudStatusCompact(_worldArea, 12);
            guidanceCard.Add(_worldArea);

            _worldStep = NewStatusLabel("Tiến trình: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện.", RuntimeArtCatalog.Spirit);
            ApplyHudStatusCompact(_worldStep, 13);
            guidanceCard.Add(_worldStep);

            _worldDirection = NewStatusLabel("Chỉ dẫn: vào sân để hiện mốc gần nhất.", RuntimeArtCatalog.Gold);
            ApplyHudStatusCompact(_worldDirection, 13);
            guidanceCard.Add(_worldDirection);

            _worldPoseState = NewStatusLabel("Tư thế: nhân vật đứng yên / Người Giữ Cổng chờ / Bóng Tối đứng yên.", RuntimeArtCatalog.Muted);
            _worldPoseState.style.display = DisplayStyle.None;
            _worldHud.Add(_worldPoseState);

            _worldVfxState = NewStatusLabel("Hiệu ứng: yên tĩnh / cổng, mạch linh khí, chém gió, cảnh báo bóng đều chỉ là hình ảnh.", RuntimeArtCatalog.Spirit);
            _worldVfxState.style.display = DisplayStyle.None;
            _worldHud.Add(_worldVfxState);

            _skinSource = NewStatusLabel("Nguồn giao diện: asset runtime tối ưu, chưa phải art final.", RuntimeArtCatalog.Spirit);
            _skinSource.style.display = DisplayStyle.None;
            _worldHud.Add(_skinSource);

            _worldObjective = NewStatusLabel("Mục tiêu: gặp Người Giữ Cổng.", RuntimeArtCatalog.Gold);
            _worldObjective.name = "LGO World Objective Touch Priority";
            ApplyHudStatusCompact(_worldObjective, 14);
            guidanceCard.Add(_worldObjective);

            _interactionHint = NewStatusLabel("Di chuyển tới gần Người Giữ Cổng.", RuntimeArtCatalog.Spirit);
            _interactionHint.name = "LGO World Interaction Touch Hint";
            ApplyHudStatusCompact(_interactionHint, 13);
            guidanceCard.Add(_interactionHint);
            _worldHud.Add(guidanceCard);

            _position = NewMutedLabel("x=0.00 y=0.00 z=0.00 yaw=0.0");
            _position.style.marginTop = 8;
            _position.style.backgroundColor = RuntimeArtCatalog.Background;
            _position.style.paddingLeft = 10;
            _position.style.paddingRight = 10;
            _position.style.paddingTop = 6;
            _position.style.paddingBottom = 6;
            _position.style.display = DisplayStyle.None;
            _worldHud.Add(_position);

            _worldLandmarks = NewMutedLabel("Mốc sân luyện: Linh Môn phía nam / Người Giữ Cổng tây bắc / Đá Luyện phía bắc / Bia đọc mục tiêu phía đông / Bóng Tối xa phía đông.");
            _worldLandmarks.style.marginTop = 8;
            _worldLandmarks.style.display = DisplayStyle.None;
            _worldHud.Add(_worldLandmarks);

            _toast = NewToast("Linh Môn đã sẵn sàng.");
            _worldHud.Add(_toast);

            BuildSessionMenuPanel();
            BuildSkillPreviewPanel();
            BuildLocalCombatPanel();

            _dialoguePanel = NewSectionShell("ĐỐI THOẠI", "Người Giữ Cổng", string.Empty, "LGO Dialogue Shell");
            _dialoguePanel.style.marginTop = 10;
            _dialogueSpeaker = new Label("Người Giữ Cổng");
            _dialogueSpeaker.style.fontSize = 17;
            _dialogueSpeaker.style.unityFontStyleAndWeight = FontStyle.Bold;
            _dialogueSpeaker.style.color = RuntimeArtCatalog.Gold;
            _dialogueLine = NewMutedLabel("Đối thoại đã đóng.");
            _dialogueLine.style.fontSize = 16;
            _dialogueProgress = NewStatusLabel("Đối thoại: 0/3", RuntimeArtCatalog.Muted);
            _dialogueContinueButton = NewCompactSecondaryButton("Tiếp tục", ContinueDialogue);
            _dialogueCloseButton = NewQuietButton("Đóng", CloseDialogue);
            _dialoguePanel.Add(_dialogueSpeaker);
            _dialoguePanel.Add(_dialogueLine);
            _dialoguePanel.Add(_dialogueProgress);
            _dialoguePanel.Add(NewActionRow("LGO Dialogue Action Row", Justify.FlexStart, 6, 0, _dialogueContinueButton, _dialogueCloseButton));
            _worldHud.Add(_dialoguePanel);
            SetDialogueVisible(false);

            _savePositionButton = NewCompactPrimaryButton("Lưu vị trí", () => RunAsync(SavePositionAsync));
            _savePositionButton.tooltip = "Ghi vị trí hiện tại cho phiên thử nghiệm.";
            _backButton = NewCompactSecondaryButton("Về điện nhân vật", BackToLobby);
            _backButton.tooltip = "Quay lại quản lý nhân vật mà không đóng phiên hiện tại.";
            _worldFooterActions = NewActionRow("LGO World Action Footer V3B", Justify.FlexStart, 6, 0, _savePositionButton, _backButton);
            _worldHud.Add(_worldFooterActions);
        }

        private void BuildSessionMenuPanel()
        {
            _sessionMenuPanel = NewSectionShell("PHIÊN", "Tạm dừng cục bộ", "Menu phiên", "LGO Session Menu Overlay");
            _sessionMenuPanel.style.position = Position.Absolute;
            _sessionMenuPanel.style.left = 420;
            _sessionMenuPanel.style.right = 84;
            _sessionMenuPanel.style.top = 132;
            _sessionMenuPanel.style.marginTop = 0;
            _sessionMenuPanel.style.maxWidth = 960;
            _sessionMenuPanel.style.paddingLeft = 22;
            _sessionMenuPanel.style.paddingRight = 22;
            _sessionMenuPanel.style.paddingTop = 18;
            _sessionMenuPanel.style.paddingBottom = 20;
            RuntimeUiSkin.ApplySessionMenuFrame(_sessionMenuPanel);
            _sessionMenuStatus = NewMutedLabel("Đang tạm dừng trong sân luyện.");
            _sessionMenuStatus.style.unityTextAlign = TextAnchor.MiddleCenter;
            _sessionMenuStatus.style.marginBottom = 10;
            _sessionMenuPanel.Add(_sessionMenuStatus);
            _sessionMenuPanel.Add(NewReadabilityRow("Vị trí", "Sân Luyện An Toàn / gần Linh Môn", RuntimeArtCatalog.Spirit));
            _sessionMenuPanel.Add(NewReadabilityRow("Mục tiêu", "Tiếp tục luyện tập, lưu dấu ấn, hoặc quay về Điện Nhân Vật.", RuntimeArtCatalog.Gold));
            _resumeButton = NewCompactPrimaryButton("Tiếp tục", HideSessionMenu);
            _sessionSaveButton = NewCompactSecondaryButton("Lưu vị trí", () => RunAsync(SavePositionAsync));
            _sessionBackButton = NewCompactSecondaryButton("Về điện nhân vật", BackToLobby);
            _sessionQuitButton = NewQuietButton("Thoát", QuitPlayer);
            var sessionActions = NewActionRow("LGO Session Menu Action Row", Justify.Center, 6, 12, _resumeButton, _sessionSaveButton, _sessionBackButton, _sessionQuitButton);
            _sessionMenuPanel.Add(sessionActions);
            BuildLocalSettingsPanel();
            _root.Add(_sessionMenuPanel);
            SetSessionMenuVisible(false);
        }

        private void BuildSkillPreviewPanel()
        {
            _skillPreviewPanel = NewSectionShell("KỸ NĂNG", "Diễn tập hình ảnh", "Xem thử kỹ năng", "LGO Skill Preview Sandbox");
            _skillPreviewPanel.style.marginTop = 10;
            _skillPreviewPanel.Add(NewMutedLabel("Chỉ diễn tập hình ảnh cục bộ. Không tạo đối thủ, luật thời gian, tiến trình hay kết quả thật."));
            _previewWindSlashButton = NewSecondaryButton("Chém Gió", () => PreviewSkill("Wind Slash", "Chém Gió"));
            _previewShadowBindButton = NewSecondaryButton("Trói Bóng", () => PreviewSkill("Shadow Bind", "Trói Bóng"));
            _previewSpiritGuardButton = NewSecondaryButton("Hộ Linh", () => PreviewSkill("Spirit Guard", "Hộ Linh"));
            _skillPreviewPanel.Add(NewActionRow("LGO Skill Preview Action Row", Justify.FlexStart, 6, 0, _previewWindSlashButton, _previewShadowBindButton, _previewSpiritGuardButton));
            _worldHud.Add(_skillPreviewPanel);
        }

        private void BuildLocalCombatPanel()
        {
            _localCombatPanel = NewSectionShell("LUYỆN TẬP", "Bia luyện", "Bia luyện", "LGO World Combat Action Shell V3B");
            _localCombatPanel.style.marginTop = 8;
            _localCombatPanel.style.paddingLeft = 12;
            _localCombatPanel.style.paddingRight = 12;
            _localCombatPanel.style.paddingTop = 10;
            _localCombatPanel.style.paddingBottom = 10;
            ApplyCombatPanelSkin(_localCombatPanel);
            var combatNote = NewMutedLabel("Nhãn nguyên mẫu cục bộ: đọc mục tiêu, hit flash và hồi chiêu. Không có sát thương, phần thưởng hay chiến đấu máy chủ.");
            combatNote.style.display = DisplayStyle.None;
            _localCombatPanel.Add(combatNote);
            _combatCooldownIcon = NewCombatCooldownIcon();
            _combatTargetStatus = NewStatusLabel("Bia luyện: chưa vào sân.", RuntimeArtCatalog.Gold);
            ApplyHudStatusCompact(_combatTargetStatus, 13);
            _combatRangeStatus = NewStatusLabel("Tầm: chưa vào sân.", RuntimeArtCatalog.Muted);
            ApplyHudStatusCompact(_combatRangeStatus, 12);
            _combatVisualState = NewStatusLabel("Dấu hiệu mục tiêu: chưa chọn.", RuntimeArtCatalog.Gold);
            _combatFeedback = NewStatusLabel("Chưa phải chiến đấu thật.", RuntimeArtCatalog.Spirit);
            ApplyHudStatusCompact(_combatFeedback, 13);
            _combatCooldown = NewStatusLabel("Hồi chiêu: Sẵn sàng", RuntimeArtCatalog.Muted);
            _combatAuthority = NewStatusLabel("Mô phỏng cục bộ: chưa gửi ý định chiến đấu.", RuntimeArtCatalog.Spirit);
            _localCombatButton = NewCompactSecondaryButton("Tấn công thử", TriggerLocalCombat);
            _localCombatButton.name = "LGO World Touch Primary Combat Button";
            _localCombatButton.tooltip = "Kích hoạt phản hồi đánh thử cục bộ. Đánh thử cục bộ: xem vòng chọn mục tiêu, hit flash và nhịp hồi chiêu; không phải chiến đấu thật";
            ApplyCombatButtonSkin(_localCombatButton, CombatPlaceholderAssets.CombatButtonNormalTexture);
            var combatRow = NewIconStatusRow("LGO World Combat Readiness Row V3B", _combatCooldownIcon, _combatTargetStatus, _combatRangeStatus);
            _localCombatPanel.Add(combatRow);
            _localCombatPanel.Add(_combatFeedback);
            _combatVisualState.style.display = DisplayStyle.None;
            _combatCooldown.style.display = DisplayStyle.None;
            _combatAuthority.style.display = DisplayStyle.None;
            _localCombatPanel.Add(NewActionRow("LGO Local Combat Action Row", Justify.FlexStart, 6, 0, _localCombatButton));
            _worldHud.Add(_localCombatPanel);
        }

        private void BuildLocalSettingsPanel()
        {
            _settingsPanel = NewPreviewPanel();
            _settingsPanel.name = "LGO Local Settings Foundation";
            _settingsPanel.style.marginTop = 12;
            _settingsPanel.style.paddingLeft = 14;
            _settingsPanel.style.paddingRight = 14;
            _settingsPanel.style.paddingTop = 12;
            _settingsPanel.style.paddingBottom = 14;
            _settingsPanel.style.minHeight = 108;
            RuntimeUiSkin.ApplyLocalSettingsPanelFrame(_settingsPanel);
            _settingsPanel.Add(NewSectionTitle("Tùy chỉnh hiển thị"));
            _settingsPanel.Add(NewMutedLabel("Các lựa chọn này chỉ đổi cách xem trong phiên hiện tại."));
            _showPositionToggle = NewLocalSettingToggle("Tọa độ", false, ApplyLocalSettings);
            _showHintsToggle = NewLocalSettingToggle("Chỉ dẫn", true, ApplyLocalSettings);
            _focusModeToggle = NewLocalSettingToggle("HUD gọn", true, ApplyLocalSettings);
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
            _characterList.Add(NewStatusLabel(_characters.Length == 0 ? "Chưa có nhân vật. Tạo tu sĩ đầu tiên." : "Danh sách tu sĩ", RuntimeArtCatalog.Spirit));
            if (_characters.Length == 0)
            {
                var emptyCard = new VisualElement();
                _emptyCharacterCard = emptyCard;
                emptyCard.style.marginTop = 10;
                emptyCard.style.paddingLeft = 14;
                emptyCard.style.paddingRight = 14;
                emptyCard.style.paddingTop = 14;
                emptyCard.style.paddingBottom = 14;
                RuntimeUiSkin.ApplyEmptyCharacterCardFrame(emptyCard);
                emptyCard.Add(NewStatusLabel("Tạo tu sĩ đầu tiên", RuntimeArtCatalog.Gold));
                var empty = NewMutedLabel("Sau khi tạo, hồ sơ sẽ xuất hiện tại đây để chọn và vào sân luyện.");
                _emptyCharacterHint = empty;
                empty.style.marginTop = 6;
                emptyCard.Add(empty);
                _characterList.Add(emptyCard);
                SelectCharacter(null);
                return;
            }
            _emptyCharacterCard = null;
            _emptyCharacterHint = null;
            foreach (var character in _characters)
            {
                var captured = character;
                _characterList.Add(NewListButton(character.name, "Kiếm tu sơ nhập", () => SelectCharacter(captured)));
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
            SetBusy(false, "Sẵn sàng: Bước 1 rồi Bước 2.");
            SetToast("Linh Môn đã mở. Bước 1: trò chuyện với Người Giữ Cổng.", RuntimeArtCatalog.Spirit);
        }

        private async Task SavePositionAsync()
        {
            if (_selectedCharacter == null || _world == null) return;
            SetBusy(true, "Đang lưu vị trí phiên hiện tại...");
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
            ApplyCharacterHallActionHierarchy();
        }

        private void UpdateSelectedPreview(CharacterResponse character)
        {
            if (character == null)
            {
                _selectedName.text = "Chưa chọn nhân vật";
                _selectedMeta.text = "Tạo tu sĩ để bước vào Linh Giới.";
                if (_selectedStatus != null) _selectedStatus.text = "Trạng thái: Đang chờ hồ sơ tu sĩ.";
                if (_selectedObjective != null) _selectedObjective.text = "Mục tiêu: Tạo tu sĩ, chọn hồ sơ, rồi vào sân luyện.";
                if (_selectedClassSummary != null) _selectedClassSummary.text = "Mạch tu luyện: Kiếm tu sơ nhập đã sẵn sàng cho phiên hiện tại.";
                _worldName.text = "Chưa chọn nhân vật";
                _worldMeta.text = "Chọn nhân vật tại điện nhân vật.";
                if (_worldArea != null) _worldArea.text = "Khu vực: xem trước tại sảnh";
                if (_worldStep != null) _worldStep.text = "Tiến trình: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện.";
                if (_worldDirection != null) _worldDirection.text = "Chỉ dẫn: vào sân để hiện hướng dẫn Bước 1.";
                if (_worldLandmarks != null) _worldLandmarks.text = "Mốc sân luyện: Linh Môn phía nam / Người Giữ Cổng tây bắc / Đá Luyện phía bắc / Bia đọc mục tiêu phía đông / Bóng Tối xa phía đông.";
                if (_worldPoseState != null) _worldPoseState.text = "Tư thế: nhân vật đứng yên / Người Giữ Cổng chờ / Bóng Tối đứng yên.";
                if (_worldVfxState != null) _worldVfxState.text = "Hiệu ứng: yên tĩnh / cổng, mạch linh khí, chém gió, cảnh báo bóng đều chỉ là hình ảnh.";
                if (_skinSource != null) _skinSource.text = "Nguồn giao diện: asset runtime tối ưu, chưa phải art final.";
                if (_worldObjective != null) _worldObjective.text = "Mục tiêu: gặp Người Giữ Cổng.";
                if (_interactionHint != null) _interactionHint.text = "Di chuyển tới gần Người Giữ Cổng.";
                _position.text = "x=0.00 y=0.00 z=0.00 yaw=0.0";
                return;
            }
            _selectedName.text = character.name;
            _selectedMeta.text = "Sẵn sàng qua Linh Môn";
            if (_selectedStatus != null) _selectedStatus.text = "Trạng thái: Sẵn sàng bước qua Linh Môn.";
            if (_selectedObjective != null) _selectedObjective.text = "Mục tiêu: Vào sân luyện, gặp Người Giữ Cổng, rồi lưu vị trí.";
            if (_selectedClassSummary != null) _selectedClassSummary.text = "Mạch tu luyện: Kiếm tu sơ nhập / vai trò cân bằng.";
            _worldName.text = "Tu sĩ: " + character.name;
            _worldMeta.text = "Kiếm tu sơ nhập / phiên hiện tại";
            _position.text = character.ToString();
        }

        private void RefreshWorldLoopLabels()
        {
            if (_world == null) return;
            if (_worldArea != null) _worldArea.text = "Khu vực: " + _world.CurrentAreaLabel;
            if (_worldStep != null) _worldStep.text = "Tiến trình: " + _world.GuidedTrainingStepName;
            if (_worldDirection != null) _worldDirection.text = "Chỉ dẫn: " + _world.ObjectiveDirectionHint;
            if (_worldLandmarks != null) _worldLandmarks.text = _world.WorldLandmarkSummary;
            if (_worldPoseState != null) _worldPoseState.text = "Tư thế: nhân vật " + _world.PlayerPoseStateName + " / Người Giữ Cổng " + _world.GateKeeperPoseStateName + " / Bóng Tối " + _world.ShadowSlimeStateName + ".";
            if (_worldVfxState != null) _worldVfxState.text = "Hiệu ứng: " + _world.VfxFeedbackStateName + " / chỉ là phản hồi hình ảnh cục bộ.";
            if (_combatTargetStatus != null) _combatTargetStatus.text = CompactCombatTargetStatus(_world.TargetDummyStatusText);
            if (_combatRangeStatus != null) _combatRangeStatus.text = CompactCombatRangeStatus(_world.TargetDummyRangeText);
            if (_combatVisualState != null) _combatVisualState.text = _world.TargetDummyVisualStateText;
            if (_combatFeedback != null) _combatFeedback.text = _world.CombatFeedbackText;
            if (_combatCooldown != null) _combatCooldown.text = _world.CombatCooldownText;
            if (_combatAuthority != null) _combatAuthority.text = _world.CombatAuthorityText;
            RefreshCombatAssetUiState();
            if (_skinSource != null) _skinSource.text = "Nguồn giao diện: asset runtime tối ưu, chưa phải art final.";
            if (_worldObjective != null) _worldObjective.text = _world.ObjectiveText;
            if (_interactionHint != null) _interactionHint.text = _world.InteractionActionText;
            SetToast(_world.InteractionAcknowledged ? "Hoàn tất luyện tập. Hãy lưu vị trí hoặc về Điện Nhân Vật." : _world.InteractionText, RuntimeArtCatalog.Spirit);
            RefreshDialoguePanel();
            ApplyLocalSettings();
        }

        private void ShowAuthMode()
        {
            _authPanel.style.display = DisplayStyle.Flex;
            _lobbyPanel.style.display = DisplayStyle.None;
            _worldHud.style.display = DisplayStyle.None;
            _status.style.display = DisplayStyle.None;
            _quitButton.style.display = DisplayStyle.None;
            _mainShell.style.justifyContent = Justify.Center;
            ApplyLoginBackdrop(true);
            SetLobbyControls(false);
        }

        private void ShowLobbyMode()
        {
            _authPanel.style.display = DisplayStyle.None;
            _lobbyPanel.style.display = DisplayStyle.Flex;
            _worldHud.style.display = DisplayStyle.None;
            _status.style.display = DisplayStyle.Flex;
            _quitButton.style.display = DisplayStyle.Flex;
            SetLobbyControls(true);
            ApplyResponsiveLayoutProfile(true);
            _mainShell.style.justifyContent = Justify.Center;
            ApplyLoginBackdrop(true);
        }

        private void ShowWorldMode()
        {
            _authPanel.style.display = DisplayStyle.None;
            _lobbyPanel.style.display = DisplayStyle.None;
            _worldHud.style.display = DisplayStyle.Flex;
            _status.style.display = DisplayStyle.Flex;
            _quitButton.style.display = DisplayStyle.Flex;
            SetSessionMenuVisible(false);
            _savePositionButton.SetEnabled(true);
            _backButton.SetEnabled(true);
            ApplyResponsiveLayoutProfile(true);
            _mainShell.style.justifyContent = Justify.FlexStart;
            ApplyLoginBackdrop(false);
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
            _status.text = FormatTopStatusMessage(message);
            ApplyStatusChip(_status, busy ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Muted);
            ApplyLoginButtonState(busy ? LgoVisualAssetRegistryV2.ButtonDisabledTexture : LgoVisualAssetRegistryV3B.ButtonEnterWorldGoldTexture ?? LgoVisualAssetRegistryV2.ButtonPrimaryNormalTexture);
            if (_serverStatusIcon != null)
            {
                _serverStatusIcon.style.backgroundColor = busy ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit;
            }
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

        private string FormatTopStatusMessage(string message)
        {
            if (!string.Equals(_lastLayoutProfile, "desktop", StringComparison.Ordinal) && message == "Sẵn sàng: Bước 1 rồi Bước 2.")
                return "Sẵn sàng: Bước 1/2";
            return message;
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
                SetApiError("mở phiên", exception);
            }
        }

        private void SetApiError(string action, Exception exception)
        {
            var message = "Phiên hiện tại bị chặn khi " + action + ": " + exception.Message;
            SetBusy(false, message);
            SetToast("Phiên hiện tại chưa sẵn sàng hoặc từ chối yêu cầu. Kiểm tra kết nối rồi thử lại.", RuntimeArtCatalog.Danger);
            if (_sessionMenuStatus != null)
                _sessionMenuStatus.text = "Phiên bị gián đoạn: kiểm tra kết nối rồi thử lại.";
        }

        private static string NormalizeLayoutProfile(string value)
        {
            if (string.Equals(value, "mobile", StringComparison.OrdinalIgnoreCase)) return "mobile";
            if (string.Equals(value, "tablet", StringComparison.OrdinalIgnoreCase)) return "tablet";
            if (string.Equals(value, "desktop", StringComparison.OrdinalIgnoreCase)) return "desktop";
            return null;
        }

        private static string GetArg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == key) return args[i + 1];
            return null;
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
            if (visible) _sessionMenuPanel.BringToFront();
            if (_sessionMenuStatus != null)
                _sessionMenuStatus.text = visible ? "Phiên đang tạm dừng. Chọn tiếp tục, lưu vị trí, quay lại hoặc thoát." : "Phiên chơi đang hoạt động.";
            // LGO Session Menu Focus Cleanup v1: pause overlay owns focus; restore dialogue state when returning.
            if (visible)
            {
                if (_dialoguePanel != null) _dialoguePanel.style.display = DisplayStyle.None;
            }
            else
            {
                RefreshDialoguePanel();
            }
            ApplyLocalSettings();
        }

        private void ApplyLocalSettings()
        {
            var showPosition = _showPositionToggle == null || _showPositionToggle.value;
            var showHints = _showHintsToggle == null || _showHintsToggle.value;
            var focusMode = _focusModeToggle != null && _focusModeToggle.value;
            var sessionVisible = _sessionMenuPanel != null && _sessionMenuPanel.style.display == DisplayStyle.Flex;
            var dialogueVisible = _dialoguePanel != null && _dialoguePanel.style.display == DisplayStyle.Flex;
            var compactViewport = _isMobileProfile || string.Equals(_lastLayoutProfile, "tablet", StringComparison.Ordinal);
            var auxiliaryVisible = !focusMode && !sessionVisible && !dialogueVisible && !compactViewport;
            var gameplayPanelVisible = !sessionVisible && !dialogueVisible && !compactViewport;
            var compactWorld = compactViewport || focusMode;
            if (_worldHud != null) _worldHud.style.visibility = sessionVisible && compactViewport ? Visibility.Hidden : Visibility.Visible;
            if (_headerActions != null) _headerActions.style.visibility = sessionVisible && compactViewport ? Visibility.Hidden : Visibility.Visible;
            if (_layoutProfileLabel != null) _layoutProfileLabel.style.display = DisplayStyle.None;
            if (_worldFooterActions != null) _worldFooterActions.style.display = sessionVisible || _isMobileProfile ? DisplayStyle.None : DisplayStyle.Flex;
            if (_position != null) _position.style.display = showPosition && !focusMode ? DisplayStyle.Flex : DisplayStyle.None;
            if (_worldDebugStrip != null) _worldDebugStrip.style.display = compactWorld ? DisplayStyle.None : DisplayStyle.Flex;
            if (_worldMeta != null) _worldMeta.style.display = compactWorld ? DisplayStyle.None : DisplayStyle.Flex;
            if (_worldGuidanceCard != null) _worldGuidanceCard.style.display = dialogueVisible && compactViewport ? DisplayStyle.None : DisplayStyle.Flex;
            if (_worldArea != null) _worldArea.style.display = compactWorld ? DisplayStyle.None : DisplayStyle.Flex;
            if (_worldStep != null) _worldStep.style.display = showHints && !compactWorld ? DisplayStyle.Flex : DisplayStyle.None;
            if (_worldDirection != null) _worldDirection.style.display = showHints && !(_isMobileProfile && !dialogueVisible) ? DisplayStyle.Flex : DisplayStyle.None;
            if (_interactionHint != null) _interactionHint.style.display = showHints ? DisplayStyle.Flex : DisplayStyle.None;
            if (_worldLandmarks != null) _worldLandmarks.style.display = showHints && !compactWorld ? DisplayStyle.Flex : DisplayStyle.None;
            if (_worldPoseState != null) _worldPoseState.style.display = auxiliaryVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_worldVfxState != null) _worldVfxState.style.display = auxiliaryVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_skinSource != null) _skinSource.style.display = auxiliaryVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_skillPreviewPanel != null) _skillPreviewPanel.style.display = auxiliaryVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_localCombatPanel != null) _localCombatPanel.style.display = gameplayPanelVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_toast != null) _toast.style.display = compactWorld ? DisplayStyle.None : DisplayStyle.Flex;
            if (_combatVisualState != null) _combatVisualState.style.display = auxiliaryVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_combatCooldown != null) _combatCooldown.style.display = auxiliaryVisible ? DisplayStyle.Flex : DisplayStyle.None;
            if (_combatAuthority != null) _combatAuthority.style.display = auxiliaryVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ApplyResponsiveLayoutProfile(bool force)
        {
            var layout = RuntimeUiLayoutProfile.FromScreen(_forcedLayoutProfile, Screen.width, Screen.height);
            var width = layout.Width;
            var height = layout.Height;
            var profile = layout.Name;
            if (!force && string.Equals(_lastLayoutProfile, profile, StringComparison.Ordinal)) return;
            _lastLayoutProfile = profile;

            var mobile = layout.IsMobile;
            var tablet = layout.IsTablet;
            var worldVisible = _worldHud != null && _worldHud.style.display == DisplayStyle.Flex;
            var authVisible = _authPanel != null && _authPanel.style.display == DisplayStyle.Flex;
            var loginLogoWidth = layout.LoginLogoWidth;
            var loginLogoHeight = layout.LoginLogoHeight;
            var loginCardWidth = layout.LoginCardWidth;
            var loginCardPadding = layout.LoginCardPadding;
            var loginButtonHeight = layout.LoginButtonHeight;
            var loginButtonFont = layout.LoginButtonFontSize;
            _isMobileProfile = mobile;
            _root.style.paddingLeft = layout.RootPaddingHorizontal;
            _root.style.paddingRight = layout.RootPaddingHorizontal;
            _root.style.paddingTop = layout.RootPaddingTop;
            _root.style.paddingBottom = layout.RootPaddingBottom;

            _mainShell.style.maxWidth = worldVisible ? Length.Percent(100) : mobile ? 720 : tablet ? 980 : 1180;
            _mainShell.style.justifyContent = worldVisible || mobile ? Justify.FlexStart : Justify.Center;

            if (_header != null)
                _header.style.minHeight = layout.HeaderMinHeight(authVisible);
            if (_authPanel != null)
            {
                _authPanel.style.minHeight = layout.AuthPanelMinHeight;
                _authPanel.style.flexDirection = mobile ? FlexDirection.Column : FlexDirection.Row;
                _authPanel.style.justifyContent = Justify.FlexStart;
                _authPanel.style.alignItems = Align.Center;
                _authPanel.style.marginTop = layout.AuthPanelMarginTop;
                _authPanel.style.paddingTop = layout.AuthPanelPaddingTop;
            }
            if (_loginStage != null)
            {
                _loginStage.style.display = layout.LoginStageDisplay;
                _loginStage.style.width = layout.LoginStageWidth;
                _loginStage.style.minHeight = layout.LoginStageMinHeight;
                _loginStage.style.right = layout.LoginStageRight;
                _loginStage.style.bottom = layout.LoginStageBottom;
                _loginStage.tooltip = LoginResponsiveScaleCleanupMarker;
            }
            if (_loginGateKeeper != null)
            {
                _loginGateKeeper.style.width = layout.LoginGateKeeperWidth;
                _loginGateKeeper.style.height = layout.LoginGateKeeperHeight;
            }
            if (_loginNpcGrounding != null)
            {
                _loginNpcGrounding.style.display = layout.LoginNpcGroundingDisplay;
                _loginNpcGrounding.style.width = layout.LoginNpcGroundingWidth;
                _loginNpcGrounding.style.height = layout.LoginNpcGroundingHeight;
                _loginNpcGrounding.style.bottom = layout.LoginNpcGroundingBottom;
                _loginNpcGrounding.style.backgroundColor = layout.LoginNpcGroundingColor;
                _loginNpcGrounding.style.opacity = layout.LoginNpcGroundingOpacity;
            }
            if (_loginControlColumn != null)
            {
                _loginControlColumn.style.width = layout.LoginControlColumnWidth;
                _loginControlColumn.style.minWidth = layout.LoginControlColumnMinWidth;
                _loginControlColumn.style.maxWidth = layout.LoginControlColumnMaxWidth;
                _loginControlColumn.style.paddingBottom = layout.LoginControlColumnPaddingBottom;
                _loginControlColumn.style.marginLeft = layout.LoginControlColumnMarginLeft;
                _loginControlColumn.style.marginTop = layout.LoginControlColumnMarginTop;
            }
            if (_loginLogo != null)
            {
                _loginLogo.style.width = loginLogoWidth;
                _loginLogo.style.height = loginLogoHeight;
                _loginLogo.style.marginBottom = layout.LoginLogoMarginBottom;
            }
            if (_loginHeroTitle != null)
            {
                _loginHeroTitle.style.display = DisplayStyle.None;
                _loginHeroTitle.style.fontSize = layout.LoginHeroTitleFontSize;
            }
            if (_loginHeroCopy != null)
                _loginHeroCopy.style.display = DisplayStyle.None;
            if (_loginCard != null)
            {
                _loginCard.style.maxWidth = loginCardWidth;
                _loginCard.style.minHeight = layout.LoginCardMinHeight;
                _loginCard.style.paddingLeft = loginCardPadding;
                _loginCard.style.paddingRight = loginCardPadding;
                _loginCard.style.paddingTop = layout.LoginCardPaddingTop;
                _loginCard.style.paddingBottom = layout.LoginCardPaddingBottom;
                _loginCard.style.marginBottom = layout.LoginCardMarginBottom;
                _loginCard.style.backgroundColor = layout.LoginCardBackground;
            }
            if (_loginServerRow != null)
            {
                _loginServerRow.style.maxWidth = layout.LoginServerRowMaxWidth;
                _loginServerRow.style.minHeight = layout.LoginServerRowMinHeight;
                _loginServerRow.style.paddingLeft = layout.LoginServerRowPaddingHorizontal;
                _loginServerRow.style.paddingRight = layout.LoginServerRowPaddingHorizontal;
                _loginServerRow.style.paddingTop = layout.LoginServerRowPaddingVertical;
                _loginServerRow.style.paddingBottom = layout.LoginServerRowPaddingVertical;
            }
            if (_loginServerText != null)
                _loginServerText.style.fontSize = layout.LoginServerTextFontSize;
            if (_loginButton != null)
            {
                _loginButton.style.minHeight = loginButtonHeight;
                _loginButton.style.fontSize = loginButtonFont;
                _loginButton.style.marginTop = layout.LoginButtonMarginTop;
            }
            if (_serverSwitchButton != null)
            {
                _serverSwitchButton.style.display = DisplayStyle.None;
                _serverSwitchButton.style.minHeight = 32;
            }

            _lobbyPanel.style.maxWidth = mobile ? Mathf.Min(width - 40f, 780f) : tablet ? 790 : 800;
            _lobbyPanel.style.minHeight = mobile ? Mathf.Max(292f, height - 48f) : 410;
            _lobbyPanel.style.paddingLeft = mobile ? 12 : 18;
            _lobbyPanel.style.paddingRight = mobile ? 12 : 18;
            _lobbyPanel.style.paddingTop = mobile ? 8 : 16;
            _lobbyPanel.style.paddingBottom = mobile ? 8 : 18;
            if (_lobbyIntro != null)
            {
                // LGO Character Hall Mobile Copy Density v1: mobile keeps intent, drops prose.
                _lobbyIntro.text = mobile ? "Chọn tu sĩ, rồi vào sân luyện." : "Chọn tu sĩ để bước qua Linh Môn. Hồ sơ sẽ được chuẩn bị cho phiên hiện tại.";
                _lobbyIntro.style.fontSize = mobile ? 13 : 14;
                _lobbyIntro.style.marginBottom = mobile ? 6 : 10;
            }
            _characterList.style.minWidth = mobile ? 220 : 280;
            _characterList.style.maxWidth = mobile ? Mathf.Clamp(width * 0.40f, 285f, 330f) : tablet ? 370 : 390;
            _characterList.style.marginRight = mobile ? 10 : 14;
            _characterList.style.paddingTop = mobile ? 8 : 12;
            _characterList.style.paddingBottom = mobile ? 8 : 12;
            if (_emptyCharacterCard != null)
            {
                _emptyCharacterCard.style.marginTop = mobile ? 8 : 10;
                _emptyCharacterCard.style.paddingLeft = mobile ? 12 : 14;
                _emptyCharacterCard.style.paddingRight = mobile ? 12 : 14;
                _emptyCharacterCard.style.paddingTop = mobile ? 10 : 14;
                _emptyCharacterCard.style.paddingBottom = mobile ? 10 : 14;
            }
            if (_emptyCharacterHint != null)
            {
                _emptyCharacterHint.text = mobile ? "Hồ sơ sẽ hiện tại đây." : "Sau khi tạo, hồ sơ sẽ xuất hiện tại đây để chọn và vào sân luyện.";
                _emptyCharacterHint.style.fontSize = mobile ? 13 : 14;
            }
            if (_lobbyContent != null)
            {
                _lobbyContent.style.flexDirection = FlexDirection.Row;
                _lobbyContent.style.flexWrap = Wrap.NoWrap;
                _lobbyContent.style.marginBottom = mobile ? 8 : 10;
            }
            if (_selectedPreview != null)
            {
                _selectedPreview.style.display = mobile ? DisplayStyle.None : DisplayStyle.Flex;
                _selectedPreview.style.maxWidth = mobile ? Mathf.Clamp(width * 0.48f, 300f, 390f) : tablet ? 334 : 350;
            }
            if (_createPanel != null)
            {
                _createPanel.style.position = mobile ? Position.Absolute : Position.Relative;
                _createPanel.style.left = mobile ? Mathf.Clamp(width * 0.45f, 350f, 390f) : 0;
                _createPanel.style.right = mobile ? 12 : StyleKeyword.Auto;
                _createPanel.style.top = mobile ? 112 : StyleKeyword.Auto;
                _createPanel.style.paddingLeft = mobile ? 12 : 16;
                _createPanel.style.paddingRight = mobile ? 12 : 16;
                _createPanel.style.paddingTop = mobile ? 8 : 12;
                _createPanel.style.paddingBottom = mobile ? 8 : 14;
                _createPanel.style.marginTop = mobile ? 0 : 10;
                _createPanel.style.maxHeight = mobile ? 174 : 210;
            }
            if (_createHint != null) _createHint.style.display = mobile ? DisplayStyle.None : DisplayStyle.Flex;
            ApplyCharacterHallActionHierarchy();

            // LGO Mobile World Viewport Evidence Fit v1: keep the HUD proportional so scene actors remain reviewable.
            _worldHud.style.minWidth = layout.WorldHudMinWidth;
            _worldHud.style.maxWidth = layout.WorldHudBaseMaxWidth;
            _worldHud.style.paddingLeft = mobile ? 8 : 12;
            _worldHud.style.paddingRight = mobile ? 8 : 12;
            _worldHud.style.paddingTop = mobile ? 6 : 10;
            _worldHud.style.paddingBottom = mobile ? 6 : 10;
            if (_worldName != null)
                _worldName.style.fontSize = mobile ? 16 : 17;
            if (_worldObjective != null)
                _worldObjective.style.fontSize = mobile ? 14 : 15;
            if (_interactionHint != null)
                _interactionHint.style.fontSize = mobile ? 14 : 15;
            if (_sessionMenuPanel != null)
            {
                // LGO Session Menu Compact Focus Frame v1: compact profiles let the pause panel own the viewport.
                _sessionMenuPanel.style.left = layout.SessionMenuLeft;
                _sessionMenuPanel.style.right = layout.SessionMenuRight;
                _sessionMenuPanel.style.top = layout.SessionMenuTop;
                _sessionMenuPanel.style.maxWidth = mobile || tablet ? StyleKeyword.None : 960;
                _sessionMenuPanel.style.maxHeight = layout.SessionMenuMaxHeight;
                _sessionMenuPanel.style.paddingLeft = mobile ? 12 : tablet ? 16 : 22;
                _sessionMenuPanel.style.paddingRight = mobile ? 12 : tablet ? 16 : 22;
                _sessionMenuPanel.style.paddingTop = mobile ? 10 : tablet ? 14 : 18;
                _sessionMenuPanel.style.paddingBottom = mobile ? 10 : tablet ? 14 : 20;
                _sessionMenuPanel.style.backgroundColor = RuntimeUiSkin.SessionMenuBackground(mobile || tablet);
            }
            if (_settingsPanel != null)
            {
                _settingsPanel.style.display = mobile || tablet ? DisplayStyle.None : DisplayStyle.Flex;
            }
            ApplyWorldPanelViewportPolish(layout, worldVisible);
            if (_layoutProfileLabel != null)
            {
                _layoutProfileLabel.text = mobile
                    ? "Bố cục: mobile / HUD gọn, ưu tiên mục tiêu và nút chính."
                    : tablet
                        ? "Bố cục: tablet / HUD gọn, ưu tiên chỉ dẫn và cảnh quan."
                        : "Bố cục: desktop / HUD đầy đủ.";
            }
            ApplyTopStatusResponsive(mobile, tablet, worldVisible, width);

            if (_focusModeToggle != null && mobile && !_focusModeToggle.value)
                _focusModeToggle.value = true;
            ApplyLocalSettings();
        }

        private void ApplyWorldPanelViewportPolish(RuntimeUiLayoutProfile layout, bool worldVisible)
        {
            if (!worldVisible || _worldHud == null) return;
            var mobile = layout.IsMobile;
            var tablet = layout.IsTablet;
            var dialogueVisible = _dialoguePanel != null && _dialoguePanel.style.display == DisplayStyle.Flex;

            // LGO World HUD Dialogue Viewport Polish v1: mobile dialogue keeps buttons inside the visible viewport.
            _worldHud.style.maxWidth = layout.WorldHudMaxWidth(dialogueVisible);
            _worldHud.style.maxHeight = mobile || tablet ? layout.WorldHudMaxHeight(dialogueVisible) : StyleKeyword.None;
            _worldHud.style.paddingLeft = mobile && dialogueVisible ? 7 : mobile ? 8 : 12;
            _worldHud.style.paddingRight = mobile && dialogueVisible ? 7 : mobile ? 8 : 12;
            _worldHud.style.paddingTop = mobile && dialogueVisible ? 5 : mobile ? 6 : 10;
            _worldHud.style.paddingBottom = mobile && dialogueVisible ? 5 : mobile ? 6 : 10;
            _worldHud.style.backgroundColor = RuntimeUiSkin.WorldHudBackground(mobile, tablet, dialogueVisible);
            if (_worldGuidanceCard != null)
            {
                // LGO World HUD Mobile Hierarchy Polish v1: normal mobile keeps only objective and interaction priority.
                _worldGuidanceCard.style.marginTop = mobile ? 6 : 8;
                _worldGuidanceCard.style.marginBottom = mobile ? 6 : 8;
                _worldGuidanceCard.style.paddingTop = mobile ? 5 : 7;
                _worldGuidanceCard.style.paddingBottom = mobile ? 5 : 7;
            }

            if (_dialoguePanel != null)
            {
                _dialoguePanel.style.marginTop = mobile ? 6 : tablet ? 8 : 10;
                _dialoguePanel.style.paddingLeft = mobile ? 10 : 14;
                _dialoguePanel.style.paddingRight = mobile ? 10 : 14;
                _dialoguePanel.style.paddingTop = mobile ? 9 : 12;
                _dialoguePanel.style.paddingBottom = mobile ? 9 : 12;
            }
            if (_dialogueSpeaker != null)
                _dialogueSpeaker.style.fontSize = mobile ? 15 : 17;
            if (_dialogueLine != null)
                _dialogueLine.style.fontSize = mobile ? 14 : 16;
            if (_dialogueProgress != null)
            {
                _dialogueProgress.style.fontSize = mobile ? 12 : 13;
                _dialogueProgress.style.paddingTop = mobile ? 4 : 5;
                _dialogueProgress.style.paddingBottom = mobile ? 4 : 5;
            }
            if (_dialogueContinueButton != null)
            {
                _dialogueContinueButton.style.minHeight = mobile ? 38 : 42;
                _dialogueContinueButton.style.minWidth = mobile ? 116 : 132;
            }
            if (_dialogueCloseButton != null)
            {
                _dialogueCloseButton.style.minHeight = mobile ? 38 : 42;
                _dialogueCloseButton.style.minWidth = mobile ? 90 : 104;
            }
        }

        private void ApplyTopStatusResponsive(bool mobile, bool tablet, bool worldVisible, int viewportWidth)
        {
            // LGO World Top Status Mobile Readability v1: top chips scale by profile and avoid long text on narrow world views.
            if (_headerActions != null)
            {
                _headerActions.style.flexShrink = 1;
                _headerActions.style.justifyContent = Justify.FlexEnd;
                _headerActions.style.maxWidth = worldVisible && mobile ? Mathf.Max(320f, viewportWidth - 24f) : tablet ? 430 : 520;
            }
            if (_status != null)
            {
                _status.style.fontSize = worldVisible && mobile ? 13 : tablet ? 13 : 14;
                _status.style.minHeight = worldVisible && mobile ? 34 : 32;
                _status.style.paddingLeft = worldVisible && mobile ? 14 : 18;
                _status.style.paddingRight = worldVisible && mobile ? 14 : 18;
                _status.style.maxWidth = worldVisible && mobile ? Mathf.Clamp(viewportWidth * 0.28f, 180f, 260f) : tablet ? 270 : 360;
                if (worldVisible && mobile && _status.text.StartsWith("Sẵn sàng:", StringComparison.Ordinal))
                    _status.text = "Sẵn sàng: Bước 1/2";
                else if (worldVisible && tablet && _status.text == "Sẵn sàng: Bước 1 rồi Bước 2.")
                    _status.text = "Sẵn sàng: Bước 1/2";
            }
            if (_quitButton != null)
            {
                _quitButton.style.minHeight = worldVisible && mobile ? 34 : 36;
                _quitButton.style.minWidth = worldVisible && mobile ? 78 : 88;
                _quitButton.style.fontSize = worldVisible && mobile ? 13 : 14;
                _quitButton.style.marginRight = 0;
            }
        }

        private void ApplyCharacterHallActionHierarchy()
        {
            if (_characterActionRow == null || _createButton == null || _enterWorldButton == null) return;
            var mobileSelected = _isMobileProfile && _selectedCharacter != null;
            _characterActionRow.Clear();
            if (mobileSelected)
            {
                // LGO Character Hall Mobile Selected CTA Hierarchy v1: enter-world owns the selected state.
                _enterWorldButton.text = "Vào sân luyện";
                _enterWorldButton.style.minHeight = 48;
                _enterWorldButton.style.minWidth = 154;
                _enterWorldButton.style.fontSize = 17;
                _enterWorldButton.style.marginTop = 8;
                _enterWorldButton.style.opacity = 1f;
                _enterWorldButton.tooltip = "Bước qua Linh Môn vào sân luyện.";
                _createButton.text = "Tạo thêm";
                _createButton.style.minHeight = 38;
                _createButton.style.minWidth = 124;
                _createButton.style.fontSize = 14;
                _createButton.style.opacity = 0.82f;
                _characterActionRow.Add(_enterWorldButton);
                _characterActionRow.Add(_createButton);
                return;
            }

            _createButton.text = "Tạo tu sĩ";
            _createButton.style.minHeight = 44;
            _createButton.style.minWidth = 132;
            _createButton.style.fontSize = 14;
            _createButton.style.opacity = 1f;
            _enterWorldButton.text = "Vào sân luyện";
            _enterWorldButton.style.minHeight = 44;
            _enterWorldButton.style.minWidth = 132;
            _enterWorldButton.style.fontSize = 15;
            _enterWorldButton.style.opacity = _selectedCharacter == null ? 0.46f : 1f;
            _enterWorldButton.tooltip = _selectedCharacter == null ? "Chọn hoặc tạo tu sĩ trước khi vào sân luyện." : "Bước qua Linh Môn vào sân luyện.";
            _characterActionRow.Add(_createButton);
            _characterActionRow.Add(_enterWorldButton);
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
                RuntimeUiSkin.ApplyCombatCooldownIconState(_combatCooldownIcon, coolingDown);
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

        private static string CompactCombatTargetStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Bia luyện: chưa rõ";
            return value.Replace("Mục tiêu luyện tập", "Bia luyện");
        }

        private static string CompactCombatRangeStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Tầm: chưa rõ";
            return value.Replace("Tầm đánh", "Tầm");
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
            if (_localCombatPanel != null) _localCombatPanel.style.display = visible ? DisplayStyle.None : DisplayStyle.Flex;
        }

        internal async Task CaptureEvidenceLoginAsync()
        {
            await LoginAsync();
        }

        internal async Task CaptureEvidenceCreateCharacterIfNeededAsync(string characterName)
        {
            if (_selectedCharacter != null) return;
            _characterName.value = Required(characterName, "EvidenceHero");
            _classId.value = DefaultClassId;
            await CreateCharacterAsync();
        }

        internal async Task CaptureEvidenceEnterWorldAsync()
        {
            await EnterWorldAsync();
        }

        internal void CaptureEvidenceNearGateKeeperPrompt()
        {
            if (_world == null) return;
            _world.SetSmokePositionNearGateKeeper();
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceNearTrainingStonePrompt()
        {
            if (_world == null) return;
            _world.SetSmokePositionNearTrainingStone();
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceOpenDialogue()
        {
            if (_world == null) return;
            _world.SetSmokePositionNearGateKeeper();
            _world.TriggerInteractionForSmoke();
            RefreshWorldLoopLabels();
        }

        internal void CaptureEvidenceTargetDummyState()
        {
            if (_world == null) return;
            _world.SetSmokePositionNearTargetDummy();
            _world.TriggerLocalCombatForSmoke();
            RefreshWorldLoopLabels();
            RefreshCombatAssetUiState();
        }

        internal void CaptureEvidenceOpenSessionMenu()
        {
            SetSessionMenuVisible(true);
        }

        private void ApplyLoginButtonState(Texture2D texture)
        {
            if (_loginButton == null || texture == null) return;
            _loginButton.style.backgroundImage = new StyleBackground(texture);
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
            RuntimeUiSkin.ApplyBadgeFrame(badge);
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
            RuntimeUiSkin.ApplyToastFrame(label, RuntimeArtCatalog.Gold);
            return label;
        }

        private static void ApplyStatusChip(Label label, Color accent)
        {
            label.style.paddingLeft = 14;
            label.style.paddingRight = 14;
            label.style.paddingTop = 6;
            label.style.paddingBottom = 6;
            label.style.maxWidth = 380;
            label.style.marginRight = 8;
            label.style.whiteSpace = WhiteSpace.Normal;
            RuntimeUiSkin.ApplyStatusChipFrame(label, accent);
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
