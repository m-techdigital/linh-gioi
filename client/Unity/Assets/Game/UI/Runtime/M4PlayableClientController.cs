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
    public sealed partial class M4PlayableClientController : MonoBehaviour
    {
        private const string DefaultDevKey = "m4-playable-dev-key";
        private const string DefaultClassId = "class.sword";
        private const bool UseLoginOrnatePanelTexture = false;
        private const string LoginCtaBackingBalanceMarker = "LGO Login CTA Backing Balance v1";
        private const string LoginCtaComponentVisualPolishMarker = "LGO Login CTA Component Visual Polish v1";
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
        private VisualElement _loginNpcGroundingBloom;
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
        private Label _skillPreviewStatus;
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
        private Label _createTitle;
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
        private bool _createFormExpanded = true;
        private RuntimeUiEvidenceState _evidenceState;

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

        private RuntimeUiLayoutProfile CurrentLayoutProfile()
        {
            return RuntimeUiLayoutProfile.FromScreen(_forcedLayoutProfile, Screen.width, Screen.height);
        }

        private void BuildUi()
        {
            _root = _document.rootVisualElement;
            _root.Clear();
            _root.style.flexGrow = 1;
            _root.style.backgroundColor = RuntimeArtCatalog.Background;
            _root.style.color = RuntimeArtCatalog.Text;
            var initialLayout = CurrentLayoutProfile();
            RuntimeUiSkin.ApplyPadding(_root, initialLayout.RootPaddingHorizontal, initialLayout.RootPaddingHorizontal, initialLayout.RootPaddingTop, initialLayout.RootPaddingBottom);
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
            _mainShell.style.maxWidth = RuntimeUiSizing.MainShellMaxWidth;
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
            header.style.maxWidth = RuntimeUiSizing.MainShellMaxWidth;
            header.style.minHeight = RuntimeUiSizing.HeaderMinHeight;
            _root.Add(header);

            var brand = new VisualElement();
            brand.style.flexDirection = FlexDirection.Column;
            brand.style.alignItems = Align.FlexStart;
            brand.style.width = RuntimeUiSizing.HeaderBrandWidth;
            brand.style.height = RuntimeUiSizing.HeaderBrandHeight;
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
                var gateBackground = LgoVisualAssetRegistryV3B.LoginBackgroundSpiritGate;
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
            var layout = CurrentLayoutProfile();
            _authPanel = new VisualElement();
            _authPanel.name = "LGO Login Gate Entry Final Shell";
            _authPanel.style.width = Length.Percent(100);
            _authPanel.style.maxWidth = RuntimeUiSizing.MainShellMaxWidth;
            _authPanel.style.flexGrow = 1;
            _authPanel.style.minHeight = RuntimeUiSizing.LoginAuthPanelMinHeight;
            _authPanel.style.flexDirection = FlexDirection.Row;
            _authPanel.style.justifyContent = Justify.FlexStart;
            _authPanel.style.alignItems = Align.Center;
            _authPanel.style.marginTop = layout.AuthPanelMarginTop;
            RuntimeUiSkin.ApplyPadding(_authPanel, 0, 0, layout.AuthPanelPaddingTop, layout.AuthPanelPaddingBottom);
            _authPanel.style.position = Position.Relative;
            _mainShell.Add(_authPanel);

            _loginStage = new VisualElement();
            _loginStage.name = "LGO Login Gate Entry NPC Composition Stage V3B";
            _loginStage.style.position = Position.Absolute;
            _loginStage.style.right = 64;
            _loginStage.style.bottom = -88;
            _loginStage.style.width = RuntimeUiSizing.LoginNpcStageWidth;
            _loginStage.style.minHeight = RuntimeUiSizing.LoginNpcStageMinHeight;
            _loginStage.style.alignItems = Align.Center;
            _loginStage.style.justifyContent = Justify.FlexEnd;
            _loginStage.style.opacity = 0.93f;
            _authPanel.Add(_loginStage);

            var npcGrounding = new VisualElement();
            _loginNpcGrounding = npcGrounding;
            npcGrounding.name = "LGO Login Gate Keeper Anchored Grounding Shadow V3B";
            npcGrounding.pickingMode = PickingMode.Ignore;
            npcGrounding.style.position = Position.Absolute;
            npcGrounding.style.width = RuntimeUiSizing.LoginNpcGroundingWidth;
            npcGrounding.style.height = RuntimeUiSizing.LoginNpcGroundingHeight;
            npcGrounding.style.bottom = 24;
            npcGrounding.style.backgroundColor = new Color(0.005f, 0.018f, 0.035f, 0.26f);
            npcGrounding.style.opacity = 0.78f;
            RuntimeUiSkin.ApplyRadius(npcGrounding, RuntimeUiSizing.LoginNpcGroundingRadius);
            npcGrounding.tooltip = "LGO Login NPC Grounding Shadow Balance v1";
            _loginStage.Add(npcGrounding);

            var npcGroundingBloom = new VisualElement();
            _loginNpcGroundingBloom = npcGroundingBloom;
            npcGroundingBloom.name = "LGO Login Gate Keeper Foot Bloom V3B";
            npcGroundingBloom.pickingMode = PickingMode.Ignore;
            npcGroundingBloom.style.position = Position.Absolute;
            npcGroundingBloom.style.width = RuntimeUiSizing.LoginNpcGroundingWidth;
            npcGroundingBloom.style.height = 8;
            npcGroundingBloom.style.bottom = 34;
            npcGroundingBloom.style.backgroundColor = new Color(0.10f, 0.72f, 0.95f, 0.26f);
            RuntimeUiSkin.ApplyRadius(npcGroundingBloom, RuntimeUiSizing.LoginNpcGroundingRadius);
            _loginStage.Add(npcGroundingBloom);

            var gateKeeperTexture = LgoVisualAssetRegistryV3B.GateKeeperNpcLoginTexture;
            var gateKeeper = NewImageLayer("LGO Login Gate Keeper NPC V3B", gateKeeperTexture, ScaleMode.ScaleToFit);
            _loginGateKeeper = gateKeeper;
            gateKeeper.style.width = RuntimeUiSizing.LoginGateKeeperWidth;
            gateKeeper.style.height = RuntimeUiSizing.LoginGateKeeperHeight;
            _loginStage.Add(gateKeeper);

            var controlColumn = new VisualElement();
            _loginControlColumn = controlColumn;
            controlColumn.name = "LGO Login Gate Entry Control Column V3B Final";
            controlColumn.style.width = Length.Percent(58);
            controlColumn.style.maxWidth = RuntimeUiSizing.LoginControlColumnMaxWidth;
            controlColumn.style.minWidth = RuntimeUiSizing.LoginControlColumnMinWidth;
            controlColumn.style.flexShrink = 1;
            controlColumn.style.alignItems = Align.Center;
            controlColumn.style.justifyContent = Justify.Center;
            controlColumn.style.paddingBottom = layout.LoginControlColumnPaddingBottom;
            controlColumn.style.marginLeft = layout.LoginControlColumnMarginLeft;
            controlColumn.style.marginTop = layout.LoginControlColumnMarginTop;
            _authPanel.Add(controlColumn);

            var logoLockup = NewImageLayer("LGO Login Gate Entry V3B Final Logo Text Lockup", LgoVisualAssetRegistryV3B.LogoLinhGioiOnline, ScaleMode.ScaleToFit);
            _loginLogo = logoLockup;
            logoLockup.style.width = layout.LoginLogoWidth;
            logoLockup.style.height = layout.LoginLogoHeight;
            logoLockup.style.alignItems = Align.Center;
            logoLockup.style.justifyContent = Justify.Center;
            logoLockup.style.marginBottom = layout.LoginLogoMarginBottom;
            logoLockup.style.opacity = 0.96f;
            controlColumn.Add(logoLockup);

            _loginHeroTitle = new Label("Bước qua Linh Môn");
            _loginHeroTitle.name = "LGO Login Gate Entry Hero Title";
            RuntimeUiSkin.ApplyText(_loginHeroTitle, RuntimeArtCatalog.Text, RuntimeUiTypography.LoginHeroTitleFontSize, true, TextAnchor.MiddleCenter);
            _loginHeroTitle.style.marginBottom = RuntimeUiSpacing.LoginOrnamentMarginBottom;
            _loginHeroTitle.style.display = DisplayStyle.None;
            controlColumn.Add(_loginHeroTitle);

            _loginHeroCopy = NewMutedLabel("\"Tu tiên không chỉ là sức mạnh, mà là hành trình trở về chính mình.\"");
            _loginHeroCopy.name = "LGO Login Gate Entry Hero Copy";
            RuntimeUiSkin.ApplyText(_loginHeroCopy, RuntimeArtCatalog.Text, RuntimeUiTypography.LoginHeroCopyFontSize, false, TextAnchor.MiddleCenter);
            _loginHeroCopy.style.maxWidth = RuntimeUiSizing.LoginControlColumnMaxWidth - 40;
            _loginHeroCopy.style.marginBottom = RuntimeUiSpacing.PanelMarginBottom;
            _loginHeroCopy.style.display = DisplayStyle.None;
            controlColumn.Add(_loginHeroCopy);

            _loginCard = new VisualElement();
            _loginCard.name = "LGO Login Gate Entry Bottom CTA v3 Final Panel V3B";
            _loginCard.style.width = Length.Percent(100);
            _loginCard.style.maxWidth = layout.LoginCardWidth;
            _loginCard.style.minHeight = layout.LoginCardMinHeight;
            _loginCard.style.alignItems = Align.Center;
            _loginCard.style.justifyContent = Justify.Center;
            RuntimeUiSkin.ApplyPadding(_loginCard, layout.LoginCardPadding, layout.LoginCardPadding, layout.LoginCardPaddingTop, layout.LoginCardPaddingBottom);
            _loginCard.style.marginBottom = layout.LoginCardMarginBottom;
            _loginCard.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            RuntimeUiSkin.ApplyLoginCtaBacking(_loginCard);
            RuntimeUiSkin.ApplyLoginCtaSceneBlend(_loginCard);
            _loginCard.tooltip = LoginCtaBackingBalanceMarker + " / " + LoginCtaComponentVisualPolishMarker;
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
            serverRow.style.maxWidth = layout.LoginServerRowMaxWidth;
            serverRow.style.minHeight = layout.LoginServerRowMinHeight;
            RuntimeUiSkin.ApplyPadding(serverRow, layout.LoginServerRowPaddingHorizontal, layout.LoginServerRowPaddingHorizontal, layout.LoginServerRowPaddingVertical, layout.LoginServerRowPaddingVertical);
            RuntimeUiSkin.ApplyServerSelectorFrame(serverRow);
            serverRow.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            var serverText = new Label("S1 - Linh Giới");
            _loginServerText = serverText;
            serverText.style.flexGrow = 1;
            RuntimeUiSkin.ApplyText(serverText, RuntimeArtCatalog.Text, RuntimeUiTypography.LoginServerTextInitialFontSize, true, TextAnchor.MiddleCenter);
            serverRow.Add(serverText);
            _serverStatusIcon = new VisualElement();
            _serverStatusIcon.name = "LGO Login Server Online Dot";
            _serverStatusIcon.style.width = RuntimeUiSizing.LoginServerStatusDotSize;
            _serverStatusIcon.style.height = RuntimeUiSizing.LoginServerStatusDotSize;
            RuntimeUiSkin.ApplyRadius(_serverStatusIcon, RuntimeUiSizing.LoginServerStatusDotRadius);
            _serverStatusIcon.style.backgroundColor = RuntimeArtCatalog.Spirit;
            _serverStatusIcon.tooltip = "Đang mở";
            serverRow.Add(_serverStatusIcon);
            _loginCard.Add(serverRow);

            var apiLabel = NewMutedLabel("Cổng phiên hiện tại: " + _config.apiBaseUrl);
            apiLabel.style.fontSize = RuntimeUiTypography.LoginApiLabelFontSize;
            apiLabel.style.display = DisplayStyle.None;
            _loginCard.Add(apiLabel);
            _devKey = NewTextField("Khóa thử nghiệm", DefaultDevKey);
            _devKey.style.display = DisplayStyle.None;
            _loginCard.Add(_devKey);
            _loginButton = NewPrimaryButton("Vào Thế Giới", () => RunAsync(LoginAsync));
            _loginButton.name = "LGO Login Enter World CTA Final v2";
            _loginButton.style.width = Length.Percent(100);
            _loginButton.style.maxWidth = RuntimeUiSizing.LoginButtonMaxWidth;
            _loginButton.style.minHeight = layout.LoginButtonHeight;
            _loginButton.style.fontSize = layout.LoginButtonFontSize;
            _loginButton.style.marginTop = layout.LoginButtonMarginTop;
            _loginButton.style.marginRight = 0;
            RuntimeUiSkin.ApplyLoginEnterButtonFrame(_loginButton);
            _loginButton.tooltip = "Mở tài khoản thử nghiệm và đi tới Điện Nhân Vật.";
            _loginCard.Add(_loginButton);
            _loginCard.Add(NewLoginOrnamentRule("LGO Login CTA Lightweight Bottom Ornament v1"));
            _serverSwitchButton = NewQuietButton("Chọn Máy Chủ", () => SetToast("S1 - Linh Giới đang mở ổn định.", RuntimeArtCatalog.Spirit));
            _serverSwitchButton.name = "LGO Login Server Switch Secondary";
            _serverSwitchButton.style.minWidth = RuntimeUiSizing.LoginServerSwitchMinWidth;
            _serverSwitchButton.style.minHeight = RuntimeUiSizing.LoginServerSwitchMinHeight;
            _serverSwitchButton.style.marginTop = 3;
            _serverSwitchButton.style.marginRight = 0;
            _serverSwitchButton.style.display = DisplayStyle.None;
            _loginCard.Add(_serverSwitchButton);
            _account = NewMutedLabel("Tài khoản: chưa kết nối");
            _account.style.marginTop = 10;
            _account.style.unityTextAlign = TextAnchor.MiddleCenter;
            _account.style.fontSize = RuntimeUiTypography.LoginAccountStatusFontSize;
            _account.style.display = DisplayStyle.None;
            _loginCard.Add(_account);
        }

        private void BuildLobbyPanel()
        {
            var layout = CurrentLayoutProfile();
            _lobbyPanel = NewCharacterHallPanel(layout);
            _mainShell.Add(_lobbyPanel);
            _lobbyPanel.Add(NewSectionHeaderBlock("Điện Nhân Vật", RuntimeArtCatalog.Gold, "LGO Character Hall Header Block"));
            var lobbyIntro = NewMutedLabel("Chọn tu sĩ để bước qua Linh Môn. Hồ sơ sẽ được chuẩn bị cho phiên hiện tại.");
            _lobbyIntro = lobbyIntro;
            lobbyIntro.style.marginBottom = layout.LobbyIntroMarginBottom;
            lobbyIntro.style.unityTextAlign = TextAnchor.MiddleCenter;
            _lobbyPanel.Add(lobbyIntro);

            _lobbyContent = NewCharacterHallContentRow(layout);
            _lobbyPanel.Add(_lobbyContent);

            _characterList = NewCharacterListPanel(layout);
            _lobbyContent.Add(_characterList);

            _selectedPreview = NewSelectedCharacterPreviewPanel();
            var portraitTexture = LgoVisualAssetRegistryV3B.PlayerMaleCultivatorTexture;
            var portrait = NewCharacterPortraitFrame(layout, portraitTexture, null);
            var profileCopy = NewFlexibleColumn("LGO Character Hall Selected Profile Copy V3B");
            _selectedName = new Label("Chưa chọn nhân vật");
            RuntimeUiSkin.ApplyText(_selectedName, RuntimeArtCatalog.Gold, RuntimeUiTypography.SelectedCharacterNameFontSize, true);
            profileCopy.Add(_selectedName);
            _selectedMeta = NewMutedLabel("Tạo một tu sĩ để bước vào Linh Giới.");
            profileCopy.Add(_selectedMeta);
            var profileHero = NewCharacterProfileHero(layout, portrait, profileCopy);
            _selectedPreview.Add(profileHero);
            _selectedStatus = NewCharacterHallStatusLabel("Trạng thái: Chọn tu sĩ trước khi vào sân luyện.", RuntimeArtCatalog.Spirit, layout);
            _selectedObjective = NewCharacterHallStatusLabel("Mục tiêu: Bước qua Linh Môn, kiểm tra HUD, rồi lưu vị trí.", RuntimeArtCatalog.Gold, layout);
            _selectedClassSummary = NewCharacterHallStatusLabel("Mạch: Kiếm tu sơ nhập.", RuntimeArtCatalog.Muted, layout);
            _selectedClassSummary.name = "LGO Character Hall Collapsed Class Summary v1";
            _selectedClassSummary.style.display = DisplayStyle.None;
            _selectedPreview.Add(_selectedStatus);
            _selectedPreview.Add(_selectedObjective);
            _selectedPreview.Add(_selectedClassSummary);
            _lobbyContent.Add(_selectedPreview);

            _createPanel = NewCharacterCreatePanel(layout);
            _lobbyPanel.Add(_createPanel);

            _createTitle = NewSectionTitle("Tạo Tu Sĩ");
            _createPanel.Add(_createTitle);
            var createHint = NewMutedLabel("Danh xưng tu sĩ - Mạch khởi đầu: Kiếm tu sơ nhập.");
            _createHint = createHint;
            createHint.name = "LGO Character Create Form Muted Game Copy v2";
            createHint.style.maxWidth = RuntimeUiSizing.CharacterNameFieldMaxWidth;
            createHint.style.alignSelf = Align.Center;
            createHint.style.unityTextAlign = TextAnchor.MiddleCenter;
            createHint.style.marginBottom = 6;
            _createPanel.Add(createHint);
            _characterName = NewLobbyTextField("", "LinhGioiHero", "Nhập danh xưng tu sĩ.");
            _characterName.name = "LGO Character Create Form Framed Input v1";
            _characterName.style.maxWidth = RuntimeUiSizing.CharacterNameFieldMaxWidth;
            _classId = NewTextField("Mã lớp tu luyện", DefaultClassId);
            _classId.style.display = DisplayStyle.None;
            _createButton = NewCompactSecondaryButton("Tạo tu sĩ", OnCreateCharacterAction);
            _enterWorldButton = NewCompactPrimaryButton("Vào sân luyện", () => RunAsync(EnterWorldAsync));
            _createPanel.Add(_characterName);
            _createPanel.Add(_classId);
            _characterActionRow = NewActionRow("LGO Character Hall Action Row", Justify.FlexStart, 6, 0, _createButton, _enterWorldButton);
            _createPanel.Add(_characterActionRow);
        }

        private void BuildWorldHud()
        {
            var layout = CurrentLayoutProfile();
            _worldHud = NewWorldHudRoot("LGO World HUD Action Shell V3B Skin v1", 390);
            _mainShell.Add(_worldHud);
            _worldHud.Add(NewSectionHeaderBlock("Sân Luyện An Toàn", RuntimeArtCatalog.Spirit, "LGO World HUD Header Block"));

            _worldDebugStrip = NewBadgeStrip(
                "LGO World Debug Badge Strip",
                ("Tài khoản", "đã kết nối"),
                ("Lưu vị trí", "phiên hiện tại"),
                ("Di chuyển", "WASD hoặc phím mũi tên"),
                ("Xoay", "Q / E"),
                ("Tương tác", "F hoặc Space"),
                ("Menu", "Esc"));
            _worldDebugStrip.style.display = DisplayStyle.None;
            _worldHud.Add(_worldDebugStrip);

            _layoutProfileLabel = NewHiddenStatusLabel("Bố cục: desktop / HUD tinh gọn.", RuntimeArtCatalog.Muted);
            _worldHud.Add(_layoutProfileLabel);

            _worldName = new Label("Chưa chọn nhân vật");
            RuntimeUiSkin.ApplyText(_worldName, RuntimeArtCatalog.Gold, RuntimeUiTypography.WorldNameInitialFontSize, true);
            _worldName.style.marginTop = layout.WorldNameMarginTop;
            _worldHud.Add(_worldName);

            _worldMeta = NewMutedLabel("Chọn nhân vật tại điện nhân vật.");
            _worldMeta.style.fontSize = RuntimeUiTypography.WorldMetaFontSize;
            _worldHud.Add(_worldMeta);

            var guidanceCard = NewWorldHudGroup("LGO World Guidance Card V3B", RuntimeArtCatalog.Spirit);
            _worldGuidanceCard = guidanceCard;
            _worldArea = NewCompactStatusLabel("Khu vực: xem trước tại sảnh", RuntimeArtCatalog.Muted, RuntimeUiTypography.WorldAreaFontSize);
            guidanceCard.Add(_worldArea);

            _worldStep = NewCompactStatusLabel("Tiến trình: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện.", RuntimeArtCatalog.Spirit, RuntimeUiTypography.WorldStepFontSize);
            guidanceCard.Add(_worldStep);

            _worldDirection = NewCompactStatusLabel("Chỉ dẫn: vào sân để hiện mốc gần nhất.", RuntimeArtCatalog.Gold, RuntimeUiTypography.WorldDirectionFontSize);
            guidanceCard.Add(_worldDirection);

            _worldPoseState = NewHiddenStatusLabel("Tư thế: nhân vật đứng yên / Người Giữ Cổng chờ / Bóng Tối đứng yên.", RuntimeArtCatalog.Muted);
            _worldHud.Add(_worldPoseState);

            _worldVfxState = NewHiddenStatusLabel("Hiệu ứng: yên tĩnh / cổng, mạch linh khí, chém gió, cảnh báo bóng đều chỉ là hình ảnh.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_worldVfxState);

            _skinSource = NewHiddenStatusLabel("Nguồn giao diện: asset runtime tối ưu, chưa phải art final.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_skinSource);

            _worldObjective = NewCompactStatusLabel("Mục tiêu: gặp Người Giữ Cổng.", RuntimeArtCatalog.Gold, RuntimeUiTypography.WorldObjectiveInitialFontSize);
            _worldObjective.name = "LGO World Objective Touch Priority";
            guidanceCard.Add(_worldObjective);

            _interactionHint = NewCompactStatusLabel("Di chuyển tới gần Người Giữ Cổng.", RuntimeArtCatalog.Spirit, RuntimeUiTypography.WorldInteractionInitialFontSize);
            _interactionHint.name = "LGO World Interaction Touch Hint";
            guidanceCard.Add(_interactionHint);
            _worldHud.Add(guidanceCard);

            _position = NewMutedLabel("x=0.00 y=0.00 z=0.00 yaw=0.0");
            _position.style.marginTop = layout.PositionChipMarginTop;
            _position.style.backgroundColor = RuntimeArtCatalog.Background;
            RuntimeUiSkin.ApplyPadding(_position, layout.PositionChipPaddingHorizontal, layout.PositionChipPaddingHorizontal, layout.PositionChipPaddingVertical, layout.PositionChipPaddingVertical);
            _position.style.display = DisplayStyle.None;
            _worldHud.Add(_position);

            _worldLandmarks = NewMutedLabel("Mốc sân luyện: Linh Môn phía nam / Người Giữ Cổng tây bắc / Đá Luyện phía bắc / Bia đọc mục tiêu phía đông / Bóng Tối xa phía đông.");
            _worldLandmarks.style.marginTop = layout.WorldLandmarksMarginTop;
            _worldLandmarks.style.display = DisplayStyle.None;
            _worldHud.Add(_worldLandmarks);

            _toast = NewToast("Linh Môn đã sẵn sàng.");
            _worldHud.Add(_toast);

            BuildSessionMenuPanel();
            BuildSkillPreviewPanel();
            BuildLocalCombatPanel();

            _dialoguePanel = NewSectionShell("ĐỐI THOẠI", "Người Giữ Cổng", string.Empty, "LGO Dialogue Shell");
            _dialoguePanel.style.marginTop = layout.DialoguePanelMarginTop;
            _dialogueSpeaker = new Label("Người Giữ Cổng");
            RuntimeUiSkin.ApplyText(_dialogueSpeaker, RuntimeArtCatalog.Gold, RuntimeUiTypography.DialogueSpeakerInitialFontSize, true);
            _dialogueLine = NewMutedLabel("Đối thoại đã đóng.");
            _dialogueLine.style.fontSize = RuntimeUiTypography.DialogueLineDesktopFontSize;
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
            var layout = CurrentLayoutProfile();
            _sessionMenuPanel = NewSectionShell("PHIÊN", "Tạm dừng cục bộ", "Menu phiên", "LGO Session Menu Overlay");
            RuntimeSessionMenuLayout.ApplyPanel(_sessionMenuPanel, layout);
            RuntimeUiSkin.ApplySessionMenuFrame(_sessionMenuPanel);
            _sessionMenuStatus = NewMutedLabel("Đang tạm dừng trong sân luyện.");
            _sessionMenuStatus.style.unityTextAlign = TextAnchor.MiddleCenter;
            _sessionMenuStatus.style.marginBottom = layout.SessionMenuStatusMarginBottom;
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
            var layout = CurrentLayoutProfile();
            _skillPreviewPanel = NewSectionShell("KỸ NĂNG", "Diễn tập an toàn", "Xem thử kỹ năng", "LGO Skill Preview Sandbox");
            _skillPreviewPanel.style.marginTop = layout.SkillPreviewPanelMarginTop;
            _skillPreviewPanel.Add(NewMutedLabel("Chọn kỹ năng để thấy tư thế, vòng cảnh báo và mạch linh khí ngay trong sân luyện."));
            _skillPreviewStatus = NewCompactStatusLabel("Đang xem: chưa chọn kỹ năng.", RuntimeArtCatalog.Muted, RuntimeUiSpacing.CombatRangeStatusFontSize);
            _skillPreviewPanel.Add(_skillPreviewStatus);
            _previewWindSlashButton = NewSecondaryButton("Chém Gió", () => PreviewSkill("Wind Slash", "Chém Gió"));
            _previewShadowBindButton = NewSecondaryButton("Trói Bóng", () => PreviewSkill("Shadow Bind", "Trói Bóng"));
            _previewSpiritGuardButton = NewSecondaryButton("Hộ Linh", () => PreviewSkill("Spirit Guard", "Hộ Linh"));
            RuntimeUiSkin.ApplyButtonMetrics(_previewWindSlashButton, RuntimeUiSpacing.SkillPreviewButtonMinWidth, RuntimeUiSpacing.CompactButtonMinHeight, RuntimeUiSpacing.SkillPreviewButtonFontSize);
            RuntimeUiSkin.ApplyButtonMetrics(_previewShadowBindButton, RuntimeUiSpacing.SkillPreviewButtonMinWidth, RuntimeUiSpacing.CompactButtonMinHeight, RuntimeUiSpacing.SkillPreviewButtonFontSize);
            RuntimeUiSkin.ApplyButtonMetrics(_previewSpiritGuardButton, RuntimeUiSpacing.SkillPreviewButtonMinWidth, RuntimeUiSpacing.CompactButtonMinHeight, RuntimeUiSpacing.SkillPreviewButtonFontSize);
            _skillPreviewPanel.Add(NewActionRow("LGO Skill Preview Action Row", Justify.FlexStart, 6, 0, _previewWindSlashButton, _previewShadowBindButton, _previewSpiritGuardButton));
            ApplySkillPreviewButtonState(null);
            _worldHud.Add(_skillPreviewPanel);
        }

        private void BuildLocalCombatPanel()
        {
            var layout = CurrentLayoutProfile();
            _localCombatPanel = NewSectionShell("LUYỆN TẬP", "Bia luyện", "Bia luyện", "LGO World Combat Action Shell V3B");
            _localCombatPanel.style.marginTop = layout.LocalCombatPanelMarginTop;
            RuntimeUiSkin.ApplyPadding(_localCombatPanel, layout.LocalCombatPanelPaddingHorizontal, layout.LocalCombatPanelPaddingHorizontal, layout.LocalCombatPanelPaddingVertical, layout.LocalCombatPanelPaddingVertical);
            ApplyCombatPanelSkin(_localCombatPanel);
            var combatNote = NewHiddenMutedLabel("Nhãn nguyên mẫu cục bộ: đọc mục tiêu, hit flash và hồi chiêu. Không có sát thương, phần thưởng hay chiến đấu máy chủ.");
            _localCombatPanel.Add(combatNote);
            _combatCooldownIcon = NewCombatCooldownIcon();
            _combatTargetStatus = NewCompactStatusLabel("Bia luyện: chưa vào sân.", RuntimeArtCatalog.Gold, RuntimeUiSpacing.CombatStatusFontSize);
            _combatRangeStatus = NewCompactStatusLabel("Tầm: chưa vào sân.", RuntimeArtCatalog.Muted, RuntimeUiSpacing.CombatRangeStatusFontSize);
            _combatVisualState = NewHiddenStatusLabel("Dấu hiệu mục tiêu: chưa chọn.", RuntimeArtCatalog.Gold);
            _combatFeedback = NewCompactStatusLabel("Diễn tập an toàn.", RuntimeArtCatalog.Spirit, RuntimeUiSpacing.CombatStatusFontSize);
            _combatCooldown = NewHiddenStatusLabel("Hồi chiêu: Sẵn sàng", RuntimeArtCatalog.Muted);
            _combatAuthority = NewHiddenStatusLabel("Mô phỏng cục bộ: chưa gửi ý định chiến đấu.", RuntimeArtCatalog.Spirit);
            _localCombatButton = NewCompactSecondaryButton("Tấn công thử", TriggerLocalCombat);
            _localCombatButton.name = "LGO World Touch Primary Combat Button";
            _localCombatButton.tooltip = "Kích hoạt phản hồi đánh thử cục bộ. Đánh thử cục bộ: xem vòng chọn mục tiêu, hit flash và nhịp hồi chiêu; không phải chiến đấu thật";
            ApplyCombatButtonSkin(_localCombatButton, CombatPlaceholderAssets.CombatButtonNormalTexture, false);
            var combatRow = NewIconStatusRow("LGO World Combat Readiness Row V3B", _combatCooldownIcon, _combatTargetStatus, _combatRangeStatus);
            _localCombatPanel.Add(combatRow);
            _localCombatPanel.Add(_combatFeedback);
            _localCombatPanel.Add(NewActionRow("LGO Local Combat Action Row", Justify.FlexStart, RuntimeUiSpacing.CombatActionRowMarginTop, RuntimeUiSpacing.CombatActionRowMarginBottom, _localCombatButton));
            _worldHud.Add(_localCombatPanel);
        }

        private void BuildLocalSettingsPanel()
        {
            var layout = CurrentLayoutProfile();
            _settingsPanel = NewPreviewPanel();
            _settingsPanel.name = "LGO Local Settings Foundation";
            _settingsPanel.style.marginTop = layout.SettingsPanelMarginTop;
            RuntimeUiSkin.ApplyPadding(_settingsPanel, layout.SettingsPanelPaddingHorizontal, layout.SettingsPanelPaddingHorizontal, layout.SettingsPanelPaddingTop, layout.SettingsPanelPaddingBottom);
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
            _characterList.Add(NewCharacterHallListHeading(_characters.Length == 0 ? "Chưa có nhân vật. Tạo tu sĩ đầu tiên." : "Danh sách tu sĩ"));
            if (_characters.Length == 0)
            {
                var layout = CurrentLayoutProfile();
                var emptyTitle = NewStatusLabel("Tạo tu sĩ đầu tiên", RuntimeArtCatalog.Gold);
                var empty = NewMutedLabel("Sau khi tạo, hồ sơ sẽ xuất hiện tại đây để chọn và vào sân luyện.");
                var emptyCard = NewEmptyCharacterCard(layout, emptyTitle, empty);
                _emptyCharacterCard = emptyCard;
                _emptyCharacterHint = empty;
                _characterList.Add(emptyCard);
                _characterList.Add(NewReadabilityRow("Bước 1", "Đặt danh xưng tu sĩ bên dưới.", RuntimeArtCatalog.Spirit));
                _characterList.Add(NewReadabilityRow("Bước 2", "Tạo hồ sơ rồi vào sân luyện.", RuntimeArtCatalog.Gold));
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
            _characterList.Add(NewReadabilityRow("Sẵn sàng", "Chọn hồ sơ rồi bước qua Linh Môn.", RuntimeArtCatalog.Spirit));
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
            _createFormExpanded = character == null;
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
                _selectedMeta.text = "Kiếm tu sơ nhập / chờ tạo hồ sơ";
                if (_selectedStatus != null) _selectedStatus.text = "Trạng thái: Chờ hồ sơ tu sĩ.";
                if (_selectedObjective != null) _selectedObjective.text = "Mục tiêu: Tạo hoặc chọn tu sĩ để vào sân luyện.";
                if (_selectedClassSummary != null)
                {
                    _selectedClassSummary.text = "Mạch: Kiếm tu sơ nhập.";
                    _selectedClassSummary.style.display = DisplayStyle.None;
                }
                ApplyCharacterCreateFormState();
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
            _selectedMeta.text = "Kiếm tu sơ nhập / sẵn sàng qua Linh Môn";
            if (_selectedStatus != null) _selectedStatus.text = "Trạng thái: Sẵn sàng bước qua Linh Môn.";
            if (_selectedObjective != null) _selectedObjective.text = "Mục tiêu: Vào sân luyện, gặp Người Giữ Cổng, rồi lưu vị trí.";
            if (_selectedClassSummary != null)
            {
                _selectedClassSummary.text = "Mạch: Kiếm tu sơ nhập.";
                _selectedClassSummary.style.display = DisplayStyle.None;
            }
            ApplyCharacterCreateFormState();
            _worldName.text = "Tu sĩ: " + character.name;
            _worldMeta.text = "Kiếm tu sơ nhập / phiên hiện tại";
            _position.text = character.ToString();
        }

        private void OnCreateCharacterAction()
        {
            if (_selectedCharacter != null && !_createFormExpanded)
            {
                // LGO Character Hall Selected Create Collapse v1: selected state protects Enter World as the primary path.
                _createFormExpanded = true;
                ApplyCharacterCreateFormState();
                SetToast("Nhập danh xưng mới để tạo thêm tu sĩ.", RuntimeArtCatalog.Muted);
                return;
            }
            RunAsync(CreateCharacterAsync);
        }

        private void ApplyCharacterCreateFormState()
        {
            RuntimeCharacterHallResponsiveLayout.ApplyCreateFormState(
                CurrentLayoutProfile(),
                _isMobileProfile,
                _selectedCharacter != null,
                _createFormExpanded,
                _createTitle,
                _createHint,
                _characterName,
                _classId,
                _createPanel,
                _characterActionRow);
            ApplyCharacterHallActionHierarchy();
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
            if (_combatTargetStatus != null) _combatTargetStatus.text = RuntimeCombatHudPresentation.CompactTargetStatus(_world.TargetDummyStatusText);
            if (_combatRangeStatus != null) _combatRangeStatus.text = RuntimeCombatHudPresentation.CompactRangeStatus(_world.TargetDummyRangeText);
            if (_combatVisualState != null) _combatVisualState.text = _world.TargetDummyVisualStateText;
            if (_combatFeedback != null) _combatFeedback.text = _world.CombatFeedbackText;
            if (_combatCooldown != null) _combatCooldown.text = _world.CombatCooldownText;
            if (_combatAuthority != null) _combatAuthority.text = _world.CombatAuthorityText;
            RefreshCombatAssetUiState();
            if (_skinSource != null) _skinSource.text = "Nguồn giao diện: asset runtime tối ưu, chưa phải art final.";
            if (_worldObjective != null) _worldObjective.text = _world.ObjectiveText;
            if (_interactionHint != null) _interactionHint.text = _world.InteractionActionText;
            if (_status != null) _status.text = WorldTopStatusText();
            if (_evidenceState.ShowEnterWorldTransition)
            {
                if (_worldStep != null) _worldStep.text = "Tiến trình: Linh Môn đang mở";
                if (_worldDirection != null) _worldDirection.text = "Chỉ dẫn: ổn định linh khí trước khi vào sân luyện.";
                if (_worldObjective != null) _worldObjective.text = "Mục tiêu: đang bước qua Linh Môn.";
                if (_interactionHint != null) _interactionHint.text = "Đang nhập giới. Chuẩn bị nhận quyền điều khiển.";
                if (_status != null) _status.text = "Đang vào Linh Môn";
            }
            SetToast(_world.InteractionAcknowledged ? "Hoàn tất luyện tập. Hãy lưu vị trí hoặc về Điện Nhân Vật." : _world.InteractionText, RuntimeArtCatalog.Spirit);
            RefreshDialoguePanel();
            ApplyLocalSettings();
        }

        private void ShowAuthMode()
        {
            SetDisplayed(_authPanel, true);
            SetDisplayed(_lobbyPanel, false);
            SetDisplayed(_worldHud, false);
            SetDisplayed(_status, false);
            SetDisplayed(_quitButton, false);
            _mainShell.style.justifyContent = Justify.Center;
            ApplyLoginBackdrop(true);
            SetLobbyControls(false);
        }

        private void ShowLobbyMode()
        {
            SetDisplayed(_authPanel, false);
            SetDisplayed(_lobbyPanel, true);
            SetDisplayed(_worldHud, false);
            SetDisplayed(_status, true);
            SetDisplayed(_quitButton, true);
            SetLobbyControls(true);
            ApplyResponsiveLayoutProfile(true);
            _mainShell.style.justifyContent = Justify.Center;
            ApplyLoginBackdrop(true);
        }

        private void ShowWorldMode()
        {
            SetDisplayed(_authPanel, false);
            SetDisplayed(_lobbyPanel, false);
            SetDisplayed(_worldHud, true);
            SetDisplayed(_status, true);
            SetDisplayed(_quitButton, true);
            SetSessionMenuVisible(false);
            _savePositionButton.SetEnabled(true);
            _backButton.SetEnabled(true);
            ApplyResponsiveLayoutProfile(true);
            _mainShell.style.justifyContent = Justify.FlexStart;
            ApplyLoginBackdrop(false);
        }

        private void SetLobbyControls(bool enabled)
        {
            _characterName.SetEnabled(enabled && (_selectedCharacter == null || _createFormExpanded));
            _classId.SetEnabled(enabled);
            _createButton.SetEnabled(enabled);
            _enterWorldButton.SetEnabled(enabled && _selectedCharacter != null);
        }

        private void SetBusy(bool busy, string message)
        {
            _status.text = FormatTopStatusMessage(message);
            ApplyStatusChip(_status, busy ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Muted);
            ApplyLoginButtonState(busy ? null : LgoVisualAssetRegistryV3B.ButtonEnterWorldGoldTexture);
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
            var worldVisible = _worldHud != null && _worldHud.style.display == DisplayStyle.Flex;
            if ((worldVisible || !string.Equals(_lastLayoutProfile, "desktop", StringComparison.Ordinal)) && message == "Sẵn sàng: Bước 1 rồi Bước 2.")
                return "Sẵn sàng: Bước 1/2";
            return message;
        }

        private string WorldTopStatusText()
        {
            if (_world == null) return "Sẵn sàng";
            if (_isMobileProfile)
            {
                var mobileSkillPreview = MobileSkillPreviewTopStatusText(_world.CombatFeedbackText);
                if (mobileSkillPreview != null) return mobileSkillPreview;
                return _world.InteractionAcknowledged ? "Hoàn tất" : _world.GuidedTrainingStepName.Replace("Bước 1: tìm Người Giữ Cổng", "Bước 1/2").Replace("Bước 2: ổn định Đá Luyện", "Bước 2/2");
            }
            return _world.InteractionAcknowledged ? "Hoàn tất hướng dẫn" : "Sẵn sàng: " + _world.GuidedTrainingStepName;
        }

        private static string MobileSkillPreviewTopStatusText(string feedbackText)
        {
            if (string.IsNullOrWhiteSpace(feedbackText)) return null;
            if (feedbackText.Contains("Đang xem Trói Bóng")) return "Xem Trói Bóng";
            if (feedbackText.Contains("Đang xem Chém Gió")) return "Xem Chém Gió";
            if (feedbackText.Contains("Đang xem Hộ Linh")) return "Xem Hộ Linh";
            return null;
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
            var visible = IsDisplayed(_sessionMenuPanel);
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
            SetDisplayed(_sessionMenuPanel, visible);
            if (visible) _sessionMenuPanel.BringToFront();
            if (_sessionMenuStatus != null)
                _sessionMenuStatus.text = visible ? "Phiên đang tạm dừng. Chọn tiếp tục, lưu vị trí, quay lại hoặc thoát." : "Phiên chơi đang hoạt động.";
            // LGO Session Menu Focus Cleanup v1: pause overlay owns focus; restore dialogue state when returning.
            if (visible)
            {
                SetDisplayed(_dialoguePanel, false);
                RuntimeSessionMenuLayout.ApplyFocusScrim(_screenScrim, true);
            }
            else
            {
                RuntimeSessionMenuLayout.ApplyFocusScrim(_screenScrim, false);
                RefreshDialoguePanel();
            }
            ApplyLocalSettings();
        }

        private void ApplyLocalSettings()
        {
            var showPosition = _showPositionToggle == null || _showPositionToggle.value;
            var showHints = _showHintsToggle == null || _showHintsToggle.value;
            var focusMode = _focusModeToggle != null && _focusModeToggle.value;
            var sessionVisible = IsDisplayed(_sessionMenuPanel);
            var dialogueVisible = IsDisplayed(_dialoguePanel);
            RuntimeWorldHudResponsiveLayout.ApplyLocalVisibility(
                showPosition,
                showHints,
                focusMode,
                sessionVisible,
                dialogueVisible,
                _isMobileProfile,
                string.Equals(_lastLayoutProfile, "tablet", StringComparison.Ordinal),
                _evidenceState.ForceCombatPanel,
                _evidenceState.HideGuidanceCardOnCompact,
                _worldHud,
                _headerActions,
                _layoutProfileLabel,
                _worldFooterActions,
                _position,
                _worldDebugStrip,
                _worldMeta,
                _worldGuidanceCard,
                _worldArea,
                _worldStep,
                _worldDirection,
                _interactionHint,
                _worldLandmarks,
                _worldPoseState,
                _worldVfxState,
                _skinSource,
                _skillPreviewPanel,
                _localCombatPanel,
                _toast,
                _combatVisualState,
                _combatCooldown,
                _combatAuthority);
        }

        private void ApplyResponsiveLayoutProfile(bool force)
        {
            var layout = CurrentLayoutProfile();
            var width = layout.Width;
            var profile = layout.Name;
            if (!force && string.Equals(_lastLayoutProfile, profile, StringComparison.Ordinal)) return;
            _lastLayoutProfile = profile;

            var mobile = layout.IsMobile;
            var tablet = layout.IsTablet;
            var worldVisible = _worldHud != null && _worldHud.style.display == DisplayStyle.Flex;
            var authVisible = _authPanel != null && _authPanel.style.display == DisplayStyle.Flex;
            _isMobileProfile = mobile;
            RuntimeLoginResponsiveLayout.Apply(
                layout,
                _root,
                _authPanel,
                _loginStage,
                _loginGateKeeper,
                _loginNpcGrounding,
                _loginNpcGroundingBloom,
                _loginControlColumn,
                _loginLogo,
                _loginHeroTitle,
                _loginHeroCopy,
                _loginCard,
                _loginServerRow,
                _loginServerText,
                _loginButton,
                _serverSwitchButton);

            _mainShell.style.maxWidth = worldVisible ? Length.Percent(100) : mobile ? 720 : tablet ? 980 : 1180;
            _mainShell.style.justifyContent = worldVisible || mobile ? Justify.FlexStart : Justify.Center;

            if (_header != null)
                _header.style.minHeight = layout.HeaderMinHeight(authVisible);

            RuntimeCharacterHallResponsiveLayout.Apply(
                layout,
                _lobbyPanel,
                _lobbyIntro,
                _characterList,
                _emptyCharacterCard,
                _emptyCharacterHint,
                _lobbyContent,
                _selectedPreview,
                _selectedName,
                _createPanel);
            ApplyCharacterCreateFormState();
            ApplyCharacterHallActionHierarchy();

            // LGO Mobile World Viewport Evidence Fit v1: keep the HUD proportional so scene actors remain reviewable.
            _worldHud.style.minWidth = layout.WorldHudMinWidth;
            _worldHud.style.maxWidth = layout.WorldHudBaseMaxWidth;
            RuntimeUiSkin.ApplyPadding(_worldHud, layout.WorldHudPaddingHorizontal, layout.WorldHudPaddingVertical);
            if (_worldName != null)
                _worldName.style.fontSize = mobile ? RuntimeUiTypography.WorldNameMobileFontSize : RuntimeUiTypography.WorldNameDesktopFontSize;
            if (_worldObjective != null)
                _worldObjective.style.fontSize = mobile ? RuntimeUiTypography.WorldObjectiveMobileFontSize : RuntimeUiTypography.WorldObjectiveDesktopFontSize;
            if (_interactionHint != null)
                _interactionHint.style.fontSize = mobile ? RuntimeUiTypography.WorldInteractionMobileFontSize : RuntimeUiTypography.WorldInteractionDesktopFontSize;
            if (_sessionMenuPanel != null)
            {
                // LGO Session Menu Compact Focus Frame v1: compact profiles let the pause panel own the viewport.
                RuntimeSessionMenuLayout.ApplyPanel(_sessionMenuPanel, layout);
            }
            if (_settingsPanel != null)
            {
                _settingsPanel.style.display = mobile || tablet ? DisplayStyle.None : DisplayStyle.Flex;
            }
            RuntimeWorldHudResponsiveLayout.ApplyHudPanel(
                layout,
                worldVisible,
                _worldHud,
                _worldGuidanceCard,
                _dialoguePanel,
                _dialogueSpeaker,
                _dialogueLine,
                _dialogueProgress,
                _dialogueContinueButton,
                _dialogueCloseButton);
            if (_layoutProfileLabel != null)
            {
                _layoutProfileLabel.text = mobile
                    ? "Bố cục: mobile / HUD gọn, ưu tiên mục tiêu và nút chính."
                    : tablet
                        ? "Bố cục: tablet / HUD gọn, ưu tiên chỉ dẫn và cảnh quan."
                        : "Bố cục: desktop / HUD đầy đủ.";
            }
            RuntimeWorldHudResponsiveLayout.ApplyTopStatus(layout, worldVisible, width, _headerActions, _status, _quitButton);

            if (_focusModeToggle != null && mobile && !_focusModeToggle.value)
                _focusModeToggle.value = true;
            ApplyLocalSettings();
        }

        private void ApplyCharacterHallActionHierarchy()
        {
            RuntimeCharacterHallResponsiveLayout.ApplyActionHierarchy(
                _isMobileProfile,
                _selectedCharacter != null,
                _createFormExpanded,
                _characterActionRow,
                _createButton,
                _enterWorldButton);
        }

        private void TriggerLocalCombat()
        {
            if (_world == null) return;
            ApplyCombatButtonSkin(_localCombatButton, CombatPlaceholderAssets.CombatButtonPressedTexture, false);
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
            RuntimeCombatHudPresentation.ApplyAssetState(
                _combatCooldownIcon,
                _localCombatButton,
                _combatRangeStatus,
                _combatVisualState,
                _combatFeedback,
                _combatCooldown,
                _combatAuthority,
                _world.LocalCombatCoolingDown,
                _world.TargetDummyRangeText,
                _world.CombatFeedbackText,
                _world.CombatAuthorityText);
        }

        private void PreviewSkill(string previewName, string displayName)
        {
            if (_world == null) return;
            _world.PreviewSkillFeedback(previewName);
            RefreshWorldLoopLabels();
            ApplySkillPreviewButtonState(previewName);
            if (_skillPreviewStatus != null)
            {
                _skillPreviewStatus.text = "Đang xem: " + displayName;
                _skillPreviewStatus.style.color = RuntimeArtCatalog.Spirit;
            }
            SetToast("Diễn tập " + displayName + ": hiệu ứng đã hiện trong sân an toàn.", RuntimeArtCatalog.Spirit);
        }

        private void ApplySkillPreviewButtonState(string activePreviewName)
        {
            ApplySkillPreviewButtonState(_previewWindSlashButton, activePreviewName == "Wind Slash");
            ApplySkillPreviewButtonState(_previewShadowBindButton, activePreviewName == "Shadow Bind");
            ApplySkillPreviewButtonState(_previewSpiritGuardButton, activePreviewName == "Spirit Guard");
        }

        private static void ApplySkillPreviewButtonState(Button button, bool active)
        {
            if (button == null) return;
            RuntimeUiSkin.ApplyCompactActionFrame(
                button,
                active ? new Color(0.02f, 0.25f, 0.30f, 0.94f) : new Color(0.03f, 0.10f, 0.18f, 0.90f),
                RuntimeArtCatalog.Spirit,
                active ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.SurfaceRaised,
                active ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.SurfaceRaised,
                active ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Gold);
            button.style.color = active ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Text;
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
            SetDisplayed(_dialoguePanel, visible);
            SetDisplayed(_localCombatPanel, !visible);
        }

        private static bool IsDisplayed(VisualElement element)
        {
            return element != null && element.style.display == DisplayStyle.Flex;
        }

        private static void SetDisplayed(VisualElement element, bool visible)
        {
            if (element == null) return;
            element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ApplyLoginButtonState(Texture2D texture)
        {
            if (_loginButton == null || texture == null) return;
            _loginButton.style.backgroundImage = new StyleBackground(texture);
            RuntimeUiSkin.ApplyLoginEnterButtonFrame(_loginButton);
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
