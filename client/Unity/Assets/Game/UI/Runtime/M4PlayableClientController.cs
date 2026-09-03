using System;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.Art;
using LinhGioi.Foundation;
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
        private Label _worldObjective;
        private Label _interactionHint;
        private Label _position;
        private Button _loginButton;
        private Button _createButton;
        private Button _enterWorldButton;
        private Button _savePositionButton;
        private Button _backButton;
        private Button _quitButton;
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
            if (Input.GetKeyDown(KeyCode.Escape)) QuitPlayer();
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

            var right = new VisualElement();
            right.style.flexDirection = FlexDirection.Row;
            right.style.alignItems = Align.Center;
            right.style.flexWrap = Wrap.Wrap;
            right.Add(_status);
            _quitButton = NewQuietButton("Quit", QuitPlayer);
            _quitButton.tooltip = "Exit visible player review";
            right.Add(_quitButton);
            header.Add(right);
        }

        private void BuildAuthPanel()
        {
            _authPanel = NewPanel(520);
            _mainShell.Add(_authPanel);
            _authPanel.Add(NewSectionTitle("Auth / Gate Entry"));
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
            _worldHud.Add(NewSectionTitle("World HUD"));

            var topStrip = new VisualElement();
            topStrip.style.flexDirection = FlexDirection.Row;
            topStrip.style.flexWrap = Wrap.Wrap;
            topStrip.style.marginBottom = 10;
            _worldHud.Add(topStrip);

            topStrip.Add(NewBadge("Account", "profile loaded"));
            topStrip.Add(NewBadge("API", "local persistence"));
            topStrip.Add(NewBadge("Move", "WASD/arrows + Q/E"));

            _worldName = new Label("No character selected");
            _worldName.style.fontSize = 19;
            _worldName.style.unityFontStyleAndWeight = FontStyle.Bold;
            _worldName.style.color = RuntimeArtCatalog.Gold;
            _worldHud.Add(_worldName);

            _worldMeta = NewMutedLabel("Select a character in the lobby.");
            _worldHud.Add(_worldMeta);

            _worldObjective = NewStatusLabel("Objective: enter the world and find the training stone.", RuntimeArtCatalog.Gold);
            _worldHud.Add(_worldObjective);

            _interactionHint = NewStatusLabel("Move near the Gate Keeper or Training Stone.", RuntimeArtCatalog.Spirit);
            _worldHud.Add(_interactionHint);

            _position = NewMutedLabel("x=0.00 y=0.00 z=0.00 yaw=0.0");
            _position.style.marginTop = 8;
            _position.style.backgroundColor = RuntimeArtCatalog.Background;
            _position.style.paddingLeft = 10;
            _position.style.paddingRight = 10;
            _position.style.paddingTop = 6;
            _position.style.paddingBottom = 6;
            _worldHud.Add(_position);

            _savePositionButton = NewPrimaryButton("Save Position", () => RunAsync(SavePositionAsync));
            _backButton = NewSecondaryButton("Back to Lobby", BackToLobby);
            _worldHud.Add(NewButtonRow(_savePositionButton, _backButton, NewQuietButton("Quit", QuitPlayer)));
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
            }
            catch (Exception exception)
            {
                SetBusy(false, "Create failed: " + exception.Message);
            }
        }

        private async Task EnterWorldAsync()
        {
            if (_selectedCharacter == null) return;
            SetBusy(true, "Entering world...");
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
            SetBusy(false, "World ready: " + Abbrev(loaded.characterId));
        }

        private async Task SavePositionAsync()
        {
            if (_selectedCharacter == null || _world == null) return;
            SetBusy(true, "Saving position...");
            var save = _world.BuildSaveRequest();
            _selectedCharacter = await _client.SaveCharacterPositionAsync(_selectedCharacter.characterId, save.x, save.y, save.z, save.yawDegrees, _shutdown.Token);
            UpdateSelectedPreview(_selectedCharacter);
            SetBusy(false, "Position saved");
        }

        private void BackToLobby()
        {
            ShowLobbyMode();
            SetBusy(false, "Lobby ready");
        }

        private void SelectCharacter(CharacterResponse character)
        {
            _selectedCharacter = character;
            UpdateSelectedPreview(character);
            _enterWorldButton.SetEnabled(character != null);
            _status.text = character == null ? "Create or select a cultivator" : "Selected: " + character.name;
        }

        private void UpdateSelectedPreview(CharacterResponse character)
        {
            if (character == null)
            {
                _selectedName.text = "No character selected";
                _selectedMeta.text = "Create a cultivator to enter the world.";
                _worldName.text = "No character selected";
                _worldMeta.text = "Select a character in the lobby.";
                if (_worldObjective != null) _worldObjective.text = "Objective: enter the world and find the training stone.";
                if (_interactionHint != null) _interactionHint.text = "Move near the Gate Keeper or Training Stone.";
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
            if (_worldObjective != null) _worldObjective.text = _world.ObjectiveText;
            if (_interactionHint != null) _interactionHint.text = _world.InteractionText;
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
            _loginButton.SetEnabled(!busy);
            if (_accountState != null)
            {
                _createButton.SetEnabled(!busy);
                _enterWorldButton.SetEnabled(!busy && _selectedCharacter != null);
                _savePositionButton.SetEnabled(!busy);
                _backButton.SetEnabled(!busy);
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
                SetBusy(false, "Error: " + exception.Message);
            }
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
            panel.style.borderTopColor = RuntimeArtCatalog.SurfaceRaised;
            panel.style.borderTopWidth = 1;
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
            return button;
        }

        private static Button NewListButton(string name, string classId, Action action)
        {
            var button = NewSecondaryButton(name + "\n" + classId, action);
            button.style.minWidth = 210;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
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

            var settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.name = "LGO Runtime Panel Settings";
            return settings;
        }
    }
}
