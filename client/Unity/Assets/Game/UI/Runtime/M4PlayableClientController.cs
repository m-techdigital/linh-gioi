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
        private TextField _devKey;
        private TextField _characterName;
        private TextField _classId;
        private Label _status;
        private Label _account;
        private Label _position;
        private Button _loginButton;
        private Button _createButton;
        private Button _enterWorldButton;
        private Button _savePositionButton;
        private Button _backButton;
        private VisualElement _characterList;
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
            _root.style.flexGrow = 1;
            _root.style.paddingLeft = 22;
            _root.style.paddingRight = 22;
            _root.style.paddingTop = 18;
            _root.style.paddingBottom = 18;
            _root.style.backgroundColor = RuntimeArtCatalog.Background;
            _root.style.color = RuntimeArtCatalog.Text;

            var title = new Label("Linh Gioi Online");
            title.style.fontSize = 28;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = RuntimeArtCatalog.Gold;
            _root.Add(title);

            var identity = new Label("M4 Visual Placeholder Foundation");
            identity.style.color = RuntimeArtCatalog.Spirit;
            identity.style.marginBottom = 8;
            _root.Add(identity);

            _status = new Label("API: " + _config.apiBaseUrl);
            _status.style.marginBottom = 10;
            _root.Add(_status);

            _account = new Label("Not logged in");
            _root.Add(_account);

            _devKey = NewTextField("Dev key", DefaultDevKey);
            _root.Add(_devKey);
            _loginButton = NewButton("Login", () => RunAsync(LoginAsync));
            _root.Add(_loginButton);

            _characterList = new VisualElement();
            _characterList.style.marginTop = 14;
            _root.Add(_characterList);

            _characterName = NewTextField("Character name", "LinhGioiHero");
            _classId = NewTextField("Class ID", DefaultClassId);
            _createButton = NewButton("Create", () => RunAsync(CreateCharacterAsync));
            _enterWorldButton = NewButton("Enter World", () => RunAsync(EnterWorldAsync));
            _savePositionButton = NewButton("Save Position", () => RunAsync(SavePositionAsync));
            _backButton = NewButton("Back to Lobby", BackToLobby);
            _position = new Label("x=0.00 y=0.00 z=0.00 yaw=0.0");

            _root.Add(_characterName);
            _root.Add(_classId);
            _root.Add(_createButton);
            _root.Add(_enterWorldButton);
            _root.Add(_position);
            _root.Add(_savePositionButton);
            _root.Add(_backButton);
            SetLobbyControls(false);
        }

        private async Task LoginAsync()
        {
            SetBusy(true, "Logging in...");
            var login = await _client.LoginDevAsync(Required(_devKey.value, DefaultDevKey), "M4 Playable Client", _shutdown.Token);
            _accountState = login.account;
            _account.text = "Account: " + _accountState.accountId + " / " + _accountState.displayName;
            await RefreshCharactersAsync();
            SetLobbyControls(true);
            SetBusy(false, "Login ready");
        }

        private async Task RefreshCharactersAsync()
        {
            _characters = await _client.ListCharactersAsync(_accountState.accountId, _shutdown.Token);
            _characterList.Clear();
            if (_characters.Length == 0)
            {
                _characterList.Add(new Label("No character yet"));
                _selectedCharacter = null;
                _enterWorldButton.SetEnabled(false);
                return;
            }
            foreach (var character in _characters)
            {
                var captured = character;
                var button = NewButton(character.name + " / " + character.classId, () => SelectCharacter(captured));
                _characterList.Add(button);
            }
            SelectCharacter(_characters[0]);
        }

        private async Task CreateCharacterAsync()
        {
            if (_accountState == null) return;
            SetBusy(true, "Creating character...");
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
            }
            _world.Enter(loaded);
            _characterList.style.display = DisplayStyle.None;
            _characterName.style.display = DisplayStyle.None;
            _classId.style.display = DisplayStyle.None;
            _createButton.style.display = DisplayStyle.None;
            _enterWorldButton.style.display = DisplayStyle.None;
            _savePositionButton.style.display = DisplayStyle.Flex;
            _backButton.style.display = DisplayStyle.Flex;
            _position.style.display = DisplayStyle.Flex;
            SetBusy(false, "World ready: " + loaded.characterId);
        }

        private async Task SavePositionAsync()
        {
            if (_selectedCharacter == null || _world == null) return;
            SetBusy(true, "Saving position...");
            var save = _world.BuildSaveRequest();
            _selectedCharacter = await _client.SaveCharacterPositionAsync(_selectedCharacter.characterId, save.x, save.y, save.z, save.yawDegrees, _shutdown.Token);
            SetBusy(false, "Position saved");
        }

        private void BackToLobby()
        {
            _characterList.style.display = DisplayStyle.Flex;
            _characterName.style.display = DisplayStyle.Flex;
            _classId.style.display = DisplayStyle.Flex;
            _createButton.style.display = DisplayStyle.Flex;
            _enterWorldButton.style.display = DisplayStyle.Flex;
            _savePositionButton.style.display = DisplayStyle.None;
            _backButton.style.display = DisplayStyle.None;
            _position.style.display = DisplayStyle.None;
            SetBusy(false, "Lobby ready");
        }

        private void SelectCharacter(CharacterResponse character)
        {
            _selectedCharacter = character;
            _enterWorldButton.SetEnabled(character != null);
            _status.text = character == null ? "Select a character" : "Selected: " + character.name + " at " + character;
        }

        private void SetLobbyControls(bool enabled)
        {
            _characterName.SetEnabled(enabled);
            _classId.SetEnabled(enabled);
            _createButton.SetEnabled(enabled);
            _enterWorldButton.SetEnabled(enabled && _selectedCharacter != null);
            _savePositionButton.style.display = DisplayStyle.None;
            _backButton.style.display = DisplayStyle.None;
            _position.style.display = DisplayStyle.None;
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

        private static TextField NewTextField(string label, string value)
        {
            var field = new TextField(label) { value = value };
            field.style.maxWidth = 420;
            field.style.marginTop = 8;
            return field;
        }

        private static Button NewButton(string label, Action action)
        {
            var button = new Button(action) { text = label };
            button.style.maxWidth = 220;
            button.style.minHeight = 36;
            button.style.marginTop = 8;
            button.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;
            button.style.color = RuntimeArtCatalog.Text;
            return button;
        }

        private static string Required(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
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
