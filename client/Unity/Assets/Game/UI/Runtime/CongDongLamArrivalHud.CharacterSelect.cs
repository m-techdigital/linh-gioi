using LinhGioi.World;
using UnityEngine;
using System;
using LinhGioi.Account;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _characterSelectOverlay;
        private VisualElement _characterSelectPreview;
        private Label _characterSelectStatus;
        private Label _characterSelectDetailMeta;
        private Label _characterSelectStageName;
        private Button _characterSelectEnterButton;
        private readonly Button[] _characterSelectSlotButtons = new Button[3];
        private readonly CharacterResponse[] _characterSelectCharacters = new CharacterResponse[3];
        private CharacterResponse _selectedProductCharacter;
        private bool _characterSelectOpen;
        private bool _characterSelectLoading;

        private void BuildCharacterSelect()
        {
            _characterSelectOverlay = new VisualElement { name = "Map01A Character Select Overlay" };
            ApplyLgoCharacterSelectOverlay(_characterSelectOverlay);
            _characterSelectOverlay.style.backgroundColor = new Color(.006f, .020f, .040f, .12f);

            BuildCharacterSelectBrand();
            BuildCharacterSelectStage();
            BuildCharacterSelectAccountSurface();

            _root.Add(_characterSelectOverlay);
            var captureArgs = Environment.GetCommandLineArgs();
            _characterSelectOpen = ShouldShowCharacterSelectOnLaunch();
            if (ShouldSeedCharacterSelectCaptureForArgs(captureArgs))
                PrepareCharacterSelectCaptureSeed();
            UpdateCharacterSelectScreen();
        }

        private void BuildCharacterSelectBrand()
        {
            var brand = new VisualElement { name = "Map01A Character Select Brand" };
            ApplyLgoCharacterSelectBrand(brand);

            var logo = LgoTitleLabel("LINH GIỚI", 54, TextAnchor.MiddleLeft);
            logo.name = "Map01A Character Select Logo";
            RuntimeUiTypography.ApplyHeadingFont(logo);
            logo.style.color = new Color(.96f, .98f, 1f, .98f);
            logo.style.letterSpacing = 4;
            logo.style.whiteSpace = WhiteSpace.NoWrap;
            brand.Add(logo);

            var online = LgoSubtitleLabel("O  N  L  I  N  E", 12);
            online.name = "Map01A Character Select Online";
            online.style.letterSpacing = 3;
            online.style.marginTop = -7;
            brand.Add(online);

            var motto = LgoSubtitleLabel("Kiếm trong tay — Chính nghĩa trong lòng", 16);
            motto.name = "Map01A Character Select Motto";
            motto.style.marginTop = 6;
            motto.style.color = new Color(.96f, .98f, 1f, .92f);
            brand.Add(motto);
            _characterSelectOverlay.Add(brand);
        }

        private void BuildCharacterSelectStage()
        {
            var stage = new VisualElement { name = "Map01A Character Select Stage" };
            ApplyLgoCharacterSelectStage(stage);
            stage.style.alignItems = Align.Center;
            stage.style.justifyContent = Justify.FlexEnd;

            var glow = new VisualElement { name = "Map01A Character Select Stage Glow", pickingMode = PickingMode.Ignore };
            glow.style.position = Position.Absolute;
            glow.style.left = Length.Percent(33f);
            glow.style.right = Length.Percent(33f);
            glow.style.bottom = 10;
            glow.style.height = 48;
            ApplyLgoSoftGlow(glow, .34f);
            stage.Add(glow);

            _characterSelectPreview = new VisualElement { name = "Map01A Character Select Preview", pickingMode = PickingMode.Ignore };
            _characterSelectPreview.style.position = Position.Absolute;
            _characterSelectPreview.style.left = Length.Percent(18f);
            _characterSelectPreview.style.width = Length.Percent(78f);
            _characterSelectPreview.style.top = Length.Percent(1f);
            _characterSelectPreview.style.height = Length.Percent(94f);
            _characterSelectPreview.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            stage.Add(_characterSelectPreview);

            _characterSelectStageName = LgoTitleLabel("Chưa chọn nhân vật", 22, TextAnchor.MiddleCenter);
            _characterSelectStageName.name = "Map01A Character Select Stage Name";
            _characterSelectStageName.style.display = DisplayStyle.None;
            stage.Add(_characterSelectStageName);
            var title = LgoSubtitleLabel("Đệ tử Đông Lâm", 13, TextAnchor.MiddleCenter);
            title.name = "Map01A Character Select Stage Title";
            title.style.display = DisplayStyle.None;
            stage.Add(title);
            _characterSelectOverlay.Add(stage);

            var back = new Button(OpenEntryFromCharacterSelect)
            {
                name = "Map01A Character Select Back",
                text = "‹  Quay lại"
            };
            ApplyLgoCharacterSelectNavigationAction(back);
            back.AddToClassList("lgo-character-select-back");
            _characterSelectOverlay.Add(back);
        }

        private void BuildCharacterSelectAccountSurface()
        {
            var panel = RuntimeUiFactory.NewModalSurface("Map01A Character Select Account Panel");
            ApplyLgoCharacterSelectPanel(panel);

            var title = LgoTitleLabel("Chọn Nhân Vật", 28, TextAnchor.MiddleCenter);
            title.name = "Map01A Character Select Title";
            panel.Add(title);
            var subtitle = LgoSubtitleLabel("Chọn một nhân vật để tiếp tục hành trình", 13, TextAnchor.MiddleCenter);
            subtitle.name = "Map01A Character Select Subtitle";
            subtitle.style.marginBottom = 9;
            panel.Add(subtitle);

            for (var slotNumber = 1; slotNumber <= 3; slotNumber++)
            {
                var capturedSlot = slotNumber;
                var slot = new Button(() => SelectCharacterSlot(capturedSlot))
                {
                    name = "Map01A Character Slot " + capturedSlot,
                    text = "Chưa có nhân vật\nSlot " + capturedSlot
                };
                _characterSelectSlotButtons[capturedSlot - 1] = slot;
                BindCharacterSelectSlot(capturedSlot);
                panel.Add(slot);
            }

            var create = new Button(() => SetCharacterSelectStatus("Màn Tạo nhân vật đang chờ canonical design riêng."))
            {
                name = "Map01A Character Select Create",
                text = "+  Tạo nhân vật\nBắt đầu hành trình mới"
            };
            ApplyLgoCharacterSelectSecondaryAction(create);
            panel.Add(create);

            var detail = new VisualElement { name = "Map01A Character Select Detail" };
            ApplyLgoDetailCard(detail, 14, 10);
            ApplyLgoCharacterSelectDetail(detail);
            var detailTitle = LgoTitleLabel("Lộ trình Đông Lâm", 17);
            detailTitle.name = "Map01A Character Select Detail Title";
            detail.Add(detailTitle);
            _characterSelectDetailMeta = LgoSubtitleLabel("", 13);
            _characterSelectDetailMeta.name = "Map01A Character Select Detail Meta";
            _characterSelectDetailMeta.style.marginTop = 5;
            detail.Add(_characterSelectDetailMeta);
            var description = LgoSubtitleLabel("Hồ sơ hiện hành đã sẵn sàng tiếp tục tại Cổng Đông Lâm.", 12);
            description.name = "Map01A Character Select Detail Description";
            description.style.marginTop = 7;
            detail.Add(description);
            panel.Add(detail);

            _characterSelectStatus = LgoSubtitleLabel("Đang chờ dữ liệu nhân vật.", 12, TextAnchor.MiddleCenter);
            _characterSelectStatus.name = "Map01A Character Select Status";
            ApplyLgoCharacterSelectStatus(_characterSelectStatus);
            panel.Add(_characterSelectStatus);

            var actions = new VisualElement { name = "Map01A Character Select Actions" };
            ApplyLgoCharacterSelectActions(actions);
            var edit = new Button(() => SetCharacterSelectStatus("Chỉnh sửa nhân vật đang chờ screen design riêng."))
            {
                name = "Map01A Character Select Edit",
                text = "Chỉnh sửa"
            };
            var delete = new Button(() => SetCharacterSelectStatus("Xóa nhân vật cần xác nhận tài khoản và chưa khả dụng."))
            {
                name = "Map01A Character Select Delete",
                text = "Xóa"
            };
            _characterSelectEnterButton = new Button(EnterSelectedCharacter)
            {
                name = "Map01A Character Select Enter Game",
                text = "Vào game"
            };
            ApplyLgoCharacterSelectAction(edit);
            ApplyLgoCharacterSelectAction(delete);
            ApplyLgoCharacterSelectAction(_characterSelectEnterButton, true);
            edit.style.marginRight = 6;
            delete.style.marginRight = 10;
            actions.Add(edit);
            actions.Add(delete);
            actions.Add(_characterSelectEnterButton);
            panel.Add(actions);

            var server = new VisualElement { name = "Map01A Character Select Server Row" };
            ApplyLgoCharacterSelectServerRow(server);
            var serverIcon = new VisualElement { name = "Map01A Character Select Server Icon", pickingMode = PickingMode.Ignore };
            ApplyLgoCharacterSelectIcon(serverIcon, _scene.GetMap01AHudIconSprite("server"), 28);
            serverIcon.style.marginRight = 8;
            server.Add(serverIcon);
            var serverName = LgoSubtitleLabel("S1 · Đông Lâm  •  Mượt", 13);
            serverName.name = "Map01A Character Select Server Name";
            serverName.style.flexGrow = 1;
            serverName.style.whiteSpace = WhiteSpace.NoWrap;
            server.Add(serverName);
            var switchServer = new Button(() => OpenServerSelect(ServerSelectReturnTarget.CharacterSelect))
            {
                name = "Map01A Character Select Switch Server",
                text = "Đổi máy chủ"
            };
            ApplyLgoCharacterSelectServerAction(switchServer);
            server.Add(switchServer);

            _characterSelectOverlay.Add(panel);
            _characterSelectOverlay.Add(server);
            RefreshCharacterSelectContent();
        }

        private static void AddCharacterSelectProfileIcon(VisualElement parent, Sprite sprite, string suffix, float size = 64)
        {
            var icon = new VisualElement { name = "Map01A Character Profile Icon " + suffix, pickingMode = PickingMode.Ignore };
            ApplyLgoCharacterSelectIcon(icon, sprite, size);
            icon.style.position = Position.Absolute;
            icon.style.left = 10;
            icon.style.top = 8;
            parent.Add(icon);
        }

        private bool ShouldShowCharacterSelectOnLaunch()
        {
            return ShouldSeedCharacterSelectCaptureForArgs(Environment.GetCommandLineArgs());
        }

        public static bool ShouldSeedCharacterSelectCaptureForArgs(string[] args)
        {
            return args != null && (Array.IndexOf(args, "--lgo-map01a-character-select-capture") >= 0
                || Array.IndexOf(args, "--lgo-map01a-character-entry-capture") >= 0);
        }

        private static bool ShouldCaptureCharacterEntryForArgs(string[] args)
            => args != null && Array.IndexOf(args, "--lgo-map01a-character-entry-capture") >= 0;

        private void PrepareCharacterSelectCaptureSeed()
        {
            var entryCapture = ShouldCaptureCharacterEntryForArgs(Environment.GetCommandLineArgs());
            Array.Clear(_characterSelectCharacters, 0, _characterSelectCharacters.Length);
            _characterSelectCharacters[0] = entryCapture
                ? new CharacterResponse
                {
                    slot = 1, characterId = "capture.character.entry", accountId = "capture.account.review",
                    name = "LinhTu", classId = "linh", runtimeClassId = "linh", entityId = 1001,
                    runtimeState = new CharacterRuntimeStateResponse
                    {
                        mapId = Map01ACharacterEntryMapper.MapId, laneX = 18.5f, facing = -1,
                        updatedAtUnixMs = 1_700_000_000_000L
                    }
                }
                : new CharacterResponse
                {
                    slot = 1, characterId = "capture.character.1", accountId = "capture.account.review",
                    name = "KiếmTu", classId = "class.sword", runtimeClassId = "kiem", entityId = 1001
                };
            _characterSelectCharacters[2] = new CharacterResponse
            {
                slot = 3, characterId = "capture.character.3", accountId = "capture.account.review",
                name = "VõGia", classId = "class.martial", runtimeClassId = "vo", entityId = 1003
            };
            _selectedProductCharacter = _characterSelectCharacters[0];
            if (entryCapture)
            {
                _productAuthSession.Set(new ProductLoginResponse
                {
                    account = new AccountResponse
                    {
                        accountId = "capture.account.review", displayName = "capture@example.invalid"
                    },
                    accessToken = "capture-internal-token", expiresAtUnixMs = long.MaxValue
                });
                _productCharacterClient = new CharacterEntryCaptureClient(_selectedProductCharacter);
            }
            RefreshCharacterSelectSlots();
            RefreshCharacterSelectContent();
            SetCharacterSelectStatus(_selectedProductCharacter.name + " đang được chọn.");
        }

        private sealed class CharacterEntryCaptureClient : IProductCharacterClient
        {
            private readonly CharacterResponse _character;
            public CharacterEntryCaptureClient(CharacterResponse character)
                => _character = character ?? throw new ArgumentNullException(nameof(character));

            public System.Threading.Tasks.Task<CharacterResponse[]> ListProductCharactersAsync(
                string accessToken, System.Threading.CancellationToken cancellationToken)
                => System.Threading.Tasks.Task.FromResult(new[] { _character });

            public System.Threading.Tasks.Task<CharacterResponse> LoadProductCharacterAsync(
                string accessToken, string characterId, System.Threading.CancellationToken cancellationToken)
                => characterId == _character.characterId
                    ? System.Threading.Tasks.Task.FromResult(_character)
                    : System.Threading.Tasks.Task.FromException<CharacterResponse>(
                        new InvalidOperationException("Capture character id mismatch."));

            public System.Threading.Tasks.Task<CharacterResponse> SaveMap01AStateAsync(
                string accessToken, string characterId, float laneX, int facing,
                System.Threading.CancellationToken cancellationToken)
                => System.Threading.Tasks.Task.FromException<CharacterResponse>(
                    new NotSupportedException("Capture harness never persists runtime state."));
        }

        private void SelectCharacterSlot(int slotNumber)
        {
            if (slotNumber < 1 || slotNumber > _characterSelectCharacters.Length) return;
            var character = _characterSelectCharacters[slotNumber - 1];
            if (character == null)
            {
                SetCharacterSelectStatus("Slot " + slotNumber + " chưa có nhân vật. Hãy dùng Tạo nhân vật.");
                return;
            }
            _selectedProductCharacter = character;
            RefreshCharacterSelectContent();
            RefreshCharacterSelectSlots();
            SetCharacterSelectStatus(character.name + " đã chọn · S1 Đông Lâm.");
        }

        private void BindCharacterSelectSlot(int slotNumber)
        {
            var button = _characterSelectSlotButtons[slotNumber - 1];
            if (button == null) return;
            var character = _characterSelectCharacters[slotNumber - 1];
            button.RemoveFromClassList(LgoCharacterSelectProfileClass);
            button.RemoveFromClassList(LgoCharacterSelectEmptySlotClass);
            button.Clear();
            if (character == null)
            {
                button.text = "Chưa có nhân vật\nSlot " + slotNumber;
                ApplyLgoCharacterSelectEmptySlot(button);
                AddCharacterSelectProfileIcon(button, _scene.GetMap01AHudIconSprite("crest"), "Slot " + slotNumber, 48);
                return;
            }
            button.text = character.name;
            ApplyLgoCharacterSelectProfile(button, ReferenceEquals(character, _selectedProductCharacter));
            AddCharacterSelectProfileIcon(button, _scene.GetCharacterAvatarThumbnailSprite(), "Slot " + slotNumber);
            var meta = LgoSubtitleLabel("Slot " + slotNumber + " · " + ProductCharacterClassLabel(character) + " · S1 Đông Lâm", 12);
            meta.name = "Map01A Character Slot " + slotNumber + " Meta";
            meta.style.position = Position.Absolute;
            meta.style.left = 92;
            meta.style.right = 12;
            meta.style.top = 46;
            button.Add(meta);
        }

        private void RefreshCharacterSelectSlots()
        {
            for (var slotNumber = 1; slotNumber <= _characterSelectSlotButtons.Length; slotNumber++)
                BindCharacterSelectSlot(slotNumber);
        }

        private void SetCharacterSelectStatus(string message)
        {
            if (_characterSelectStatus != null) _characterSelectStatus.text = message;
        }

        private static string ProductCharacterClassLabel(CharacterResponse character)
        {
            if (character == null) return "Nhân vật";
            var classId = string.IsNullOrWhiteSpace(character.runtimeClassId)
                ? character.classId : character.runtimeClassId;
            switch (classId)
            {
                case "vo": case "class.martial": return "Võ";
                case "kiem": case "class.sword": return "Kiếm";
                case "phap": return "Pháp";
                case "co": return "Cơ";
                case "linh": return "Linh";
                default: return "Nhân vật";
            }
        }

        private void RefreshCharacterSelectContent()
        {
            if (_scene == null) return;
            var preview = _scene.GetCharacterAvatarThumbnailSprite();
            if (_characterSelectPreview != null)
                _characterSelectPreview.style.backgroundImage = preview == null ? StyleKeyword.None : new StyleBackground(preview);
            if (_characterSelectStageName != null)
                _characterSelectStageName.text = _selectedProductCharacter == null ? "Chưa chọn nhân vật" : _selectedProductCharacter.name;
            if (_characterSelectDetailMeta != null)
                _characterSelectDetailMeta.text = _selectedProductCharacter == null
                    ? "Chưa có hồ sơ được chọn."
                    : ProductCharacterClassLabel(_selectedProductCharacter) + " · Slot " + _selectedProductCharacter.slot + "\nS1 Đông Lâm";
            if (_characterSelectEnterButton != null)
                _characterSelectEnterButton.SetEnabled(!_characterSelectLoading && _selectedProductCharacter != null);
        }

        private async void OpenCharacterSelect()
        {
            if (_scene.InventoryOpen) _scene.ToggleInventory();
            if (_scene.DialogueOpen) _scene.CloseNpcDialogue();
            _entryOpen = false;
            UpdateEntryScreen();
            _characterSelectOpen = true;
            RefreshCharacterSelectContent();
            UpdateCharacterSelectScreen();
            await RefreshProductCharacterSlotsAsync();
        }

        private async System.Threading.Tasks.Task RefreshProductCharacterSlotsAsync()
        {
            Array.Clear(_characterSelectCharacters, 0, _characterSelectCharacters.Length);
            _selectedProductCharacter = null;
            RefreshCharacterSelectSlots();
            RefreshCharacterSelectContent();
            if (_productAuthSession == null || !_productAuthSession.IsAuthenticated)
            {
                SetCharacterSelectStatus("Đăng nhập để tải danh sách nhân vật.");
                return;
            }
            _characterSelectLoading = true;
            RefreshCharacterSelectContent();
            SetCharacterSelectStatus("Đang tải danh sách nhân vật…");
            try
            {
                EnsureProductCharacterClient();
                var listed = await _productCharacterClient.ListProductCharactersAsync(
                    _productAuthSession.AccessToken, ProductAuthCancellationToken);
                foreach (var character in listed ?? Array.Empty<CharacterResponse>())
                {
                    if (character == null || character.slot < 1 || character.slot > 3)
                        throw new InvalidOperationException("Character slot payload is invalid.");
                    if (_characterSelectCharacters[character.slot - 1] != null)
                        throw new InvalidOperationException("Character slot payload is duplicated.");
                    _characterSelectCharacters[character.slot - 1] = character;
                }
                for (var i = 0; i < _characterSelectCharacters.Length; i++)
                    if (_characterSelectCharacters[i] != null) { _selectedProductCharacter = _characterSelectCharacters[i]; break; }
                RefreshCharacterSelectSlots();
                RefreshCharacterSelectContent();
                SetCharacterSelectStatus(_selectedProductCharacter == null
                    ? "Chưa có nhân vật. Hãy dùng Tạo nhân vật."
                    : _selectedProductCharacter.name + " đang được chọn.");
            }
            catch (OperationCanceledException) { }
            catch (Exception)
            {
                SetCharacterSelectStatus("Không thể tải danh sách nhân vật. Vui lòng thử lại.");
            }
            finally
            {
                _characterSelectLoading = false;
                RefreshCharacterSelectContent();
            }
        }

        private async void EnterSelectedCharacter()
        {
            if (_characterSelectLoading || _selectedProductCharacter == null)
            {
                SetCharacterSelectStatus("Hãy chọn một nhân vật trước khi vào game.");
                return;
            }
            if (_productAuthSession == null || !_productAuthSession.IsAuthenticated)
            {
                SetCharacterSelectStatus("Phiên đăng nhập không còn hợp lệ.");
                return;
            }
            _characterSelectLoading = true;
            RefreshCharacterSelectContent();
            SetCharacterSelectStatus("Đang tải nhân vật…");
            try
            {
                EnsureProductCharacterClient();
                var loaded = await _productCharacterClient.LoadProductCharacterAsync(
                    _productAuthSession.AccessToken, _selectedProductCharacter.characterId, ProductAuthCancellationToken);
                if (loaded == null || loaded.characterId != _selectedProductCharacter.characterId)
                    throw new InvalidOperationException("Loaded character does not match selection.");
                var entryState = Map01ACharacterEntryMapper.Resolve(loaded);
                _scene.ApplyProductCharacterEntryState(entryState);
                _selectedProductCharacter = loaded;
                _loadedProductCharacterName = loaded.name;
                if (_vitalsName != null) _vitalsName.text = loaded.name;
                CloseCharacterSelect();
            }
            catch (OperationCanceledException) { }
            catch (Exception)
            {
                SetCharacterSelectStatus("Không thể tải nhân vật. Vui lòng thử lại.");
            }
            finally
            {
                _characterSelectLoading = false;
                RefreshCharacterSelectContent();
            }
        }

        private void OpenEntryFromCharacterSelect()
        {
            _characterSelectOpen = false;
            _entryOpen = true;
            UpdateCharacterSelectScreen();
            UpdateEntryScreen();
        }

        private void CloseCharacterSelect()
        {
            _characterSelectOpen = false;
            UpdateCharacterSelectScreen();
        }

        private void UpdateCharacterSelectScreen()
        {
            if (_characterSelectOverlay == null) return;
            _characterSelectOverlay.style.display = _characterSelectOpen ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateHudShellVisibility();
        }

        private void UpdateHudShellVisibility()
        {
            var hide = _entryOpen || _characterSelectOpen;
            if (_safe != null) _safe.style.display = hide ? DisplayStyle.None : DisplayStyle.Flex;
            if (_marker != null) _marker.style.display = hide ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
