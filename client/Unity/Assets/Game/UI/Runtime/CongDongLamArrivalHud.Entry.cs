using System;
using LinhGioi.Account;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _entryOverlay;
        private VisualElement _entryPanel;
        private VisualElement _entryControlCard;
        private VisualElement _entryBrandStage;
        private VisualElement _entryNoticePanel;
        private VisualElement _entrySideActions;
        private Label _entrySlogan;
        private Label _entrySignature;
        private Label _entryStatus;
        private TextField _entryAccountField, _entryPasswordField;
        private Button _entryLoginButton;
        private VisualElement _entryRememberMark;
        private bool _entryRememberAccount;
        private bool _entryOpen;
        private const string RememberedAccountKey = "lgo.map01a.entry.remembered-account";

        private void BuildEntryScreen()
        {
            _entryOpen = ShouldShowEntryOnLaunch();
            _entryOverlay = new VisualElement { name = "Map01A Entry Overlay" };
            _entryOverlay.AddToClassList("lgo-entry-overlay");
            _entryOverlay.style.position = Position.Absolute;
            _entryOverlay.style.left = 0;
            _entryOverlay.style.right = 0;
            _entryOverlay.style.top = 0;
            _entryOverlay.style.bottom = 0;
            _entryOverlay.style.backgroundColor = RuntimeUiTheme.WithAlpha(RuntimeUiTheme.Current.bg, .28f);

            _entrySlogan = new Label("Kiếm trong tay\n—  Chính nghĩa trong lòng  —") { name = "Map01A Entry Slogan" };
            ApplyLgoEntrySlogan(_entrySlogan);
            _entryOverlay.Add(_entrySlogan);

            _entryBrandStage = new VisualElement { name = "Map01A Entry Brand Stage", pickingMode = PickingMode.Ignore };
            ApplyLgoEntryBrandStage(_entryBrandStage);
            var brandCrest = new VisualElement { name = "Map01A Entry Brand Crest", pickingMode = PickingMode.Ignore };
            ApplyLgoEntryBrandCrest(brandCrest, _scene.GetMap01AHudIconSprite("crest"));
            _entryBrandStage.Add(brandCrest);
            var logo = new Label("LINH GIỚI") { name = "Map01A Entry Logo" };
            ApplyLgoEntryLogo(logo);
            _entryBrandStage.Add(logo);
            var logoOnline = new Label("O  N  L  I  N  E") { name = "Map01A Entry Logo Online" };
            ApplyLgoEntryLogoOnline(logoOnline);
            _entryBrandStage.Add(logoOnline);
            var brandRail = new VisualElement { name = "Map01A Entry Brand Ornament", pickingMode = PickingMode.Ignore };
            ApplyLgoOrnamentRail(brandRail);
            brandRail.style.width = 280;
            brandRail.style.alignSelf = Align.Center;
            _entryBrandStage.Add(brandRail);
            _entryOverlay.Add(_entryBrandStage);

            _entryNoticePanel = new VisualElement { name = "Map01A Entry Notice Panel" };
            ApplyLgoEntryNoticeCard(_entryNoticePanel);
            var noticeHeader = new VisualElement { name = "Map01A Entry Notice Header" };
            ApplyLgoEntryNoticeHeader(noticeHeader);
            var noticeIcon = CreateLgoEntryIcon("Map01A Entry Notice Icon", _scene.GetMap01AHudIconSprite("notice"), 22);
            noticeHeader.Add(noticeIcon);
            var noticeTitle = LgoLabel("Thông Báo", 18, UiGold, true);
            noticeTitle.name = "Map01A Entry Notice Title";
            noticeTitle.style.flexGrow = 1;
            noticeHeader.Add(noticeTitle);
            var noticeMore = new Button(() =>
            {
                if (_entryStatus != null) _entryStatus.text = "Danh sách thông báo đầy đủ hiện chưa khả dụng.";
            }) { name = "Map01A Entry Notice More", text = "Xem thêm  ›" };
            ApplyLgoEntryNoticeLink(noticeMore);
            noticeHeader.Add(noticeMore);
            _entryNoticePanel.Add(noticeHeader);

            var noticePrimaryRow = new VisualElement { name = "Map01A Entry Notice Primary Row" };
            ApplyLgoEntryNoticeRow(noticePrimaryRow);
            var noticeLine = LgoLabel("◆  Khai mở máy chủ S1 · Đông Lâm · 10:00 ngày 25/04", 14, UiText);
            noticeLine.name = "Map01A Entry Notice Line";
            noticeLine.style.flexGrow = 1;
            noticePrimaryRow.Add(noticeLine);
            var hotBadge = LgoLabel("Hot", 12, UiText, true);
            hotBadge.name = "Map01A Entry Notice Hot Badge";
            ApplyLgoEntryNoticeHotBadge(hotBadge);
            noticePrimaryRow.Add(hotBadge);
            _entryNoticePanel.Add(noticePrimaryRow);
            var noticeEvent = LgoLabel("◆  Sự kiện Tu Luyện Đăng Nhập · nhận quà cực phẩm", 14, UiSubText);
            noticeEvent.name = "Map01A Entry Notice Event";
            _entryNoticePanel.Add(noticeEvent);
            _entryOverlay.Add(_entryNoticePanel);

            _entrySideActions = new VisualElement { name = "Map01A Entry Side Actions" };
            ApplyLgoEntryUtilityRail(_entrySideActions);
            _entryOverlay.Add(_entrySideActions);
            AddEntrySideAction(_entrySideActions, "Thông Báo", "Thông báo máy chủ Đông Lâm đang mở ở góc trái dưới.", "notice");
            AddEntrySideAction(_entrySideActions, "Hỗ Trợ", "Trung tâm hỗ trợ hiện chưa khả dụng.", "support");
            AddEntrySideAction(_entrySideActions, "Cinematic", "Cinematic giới thiệu hiện chưa khả dụng.", "cinematic");
            AddEntrySideAction(_entrySideActions, "Cài Đặt", "Cài đặt nâng cao hiện chưa khả dụng.", "menu");

            _entrySignature = new Label("Cùng nhau\nKiến tạo thế giới lớn") { name = "Map01A Entry Signature" };
            ApplyLgoEntrySignature(_entrySignature);
            _entryOverlay.Add(_entrySignature);

            _entryPanel = new VisualElement { name = "Map01A Entry Panel" };
            ApplyLgoEntryShell(_entryPanel);
            _entryOverlay.Add(_entryPanel);

            _entryControlCard = new VisualElement { name = "Map01A Entry Control Card" };
            ApplyLgoEntryControlCard(_entryControlCard);
            _entryPanel.Add(_entryControlCard);

            _entryAccountField = MakeEntryField("Map01A Entry Account Field", "Tài khoản / Email / Số điện thoại", "account", false);
            _entryPasswordField = MakeEntryField("Map01A Entry Password Field", "Mật khẩu", "lock", true);
            var rememberedAccount = PlayerPrefs.GetString(RememberedAccountKey, string.Empty);
            _entryRememberAccount = !string.IsNullOrWhiteSpace(rememberedAccount);
            _entryAccountField.SetValueWithoutNotify(rememberedAccount);
            _entryAccountField.RegisterValueChangedCallback(evt =>
            {
                if (!_entryRememberAccount) return;
                if (string.IsNullOrWhiteSpace(evt.newValue)) PlayerPrefs.DeleteKey(RememberedAccountKey);
                else PlayerPrefs.SetString(RememberedAccountKey, evt.newValue.Trim());
                PlayerPrefs.Save();
            });
            _entryControlCard.Add(_entryAccountField);
            _entryControlCard.Add(_entryPasswordField);
            _entryControlCard.Add(MakeEntryAuthOptions());

            var authActions = new VisualElement { name = "Map01A Entry Auth Actions" };
            authActions.style.flexDirection = FlexDirection.Row;
            authActions.style.marginBottom = 14;
            _entryLoginButton = new Button(SubmitProductLogin)
            {
                name = "Map01A Entry Login Button",
                text = "Đăng nhập"
            };
            ApplyLgoEntryAuthAction(_entryLoginButton, true);
            _entryLoginButton.style.flexGrow = 1;
            _entryLoginButton.style.marginRight = 10;
            var register = new Button(OpenRegister)
            {
                name = "Map01A Entry Register Button",
                text = "Đăng ký"
            };
            ApplyLgoEntryAuthAction(register, false);
            register.style.flexGrow = 1;
            authActions.Add(_entryLoginButton);
            authActions.Add(register);
            _entryControlCard.Add(authActions);

            var serverCard = new VisualElement { name = "Map01A Entry Server Card" };
            ApplyLgoEntryServerCard(serverCard);
            var serverIcon = CreateLgoEntryIcon("Map01A Entry Server Icon", _scene.GetMap01AHudIconSprite("server"), 34);
            serverIcon.style.marginRight = 12;
            var serverName = LgoLabel("S1 · Đông Lâm", 18, new Color(.90f, .95f, .94f, .98f), true);
            serverName.name = "Map01A Entry Server Name";
            serverName.style.flexGrow = 1;
            var serverState = LgoLabel("● Mượt", 16, new Color(.58f, 1f, .36f, .96f), true);
            serverState.name = "Map01A Entry Server State";
            serverState.style.whiteSpace = WhiteSpace.NoWrap;
            serverState.style.flexShrink = 0;
            serverState.style.minWidth = 76;
            serverState.style.marginRight = 8;
            var serverSwitch = new Button(() => OpenServerSelect(ServerSelectReturnTarget.Entry))
            {
                name = "Map01A Entry Server Switch",
                text = "›"
            };
            ApplyLgoEntryServerSwitchAction(serverSwitch);
            serverSwitch.style.fontSize = 24;
            serverSwitch.style.paddingLeft = serverSwitch.style.paddingRight = 8;
            serverCard.Add(serverIcon);
            serverCard.Add(serverName);
            serverCard.Add(serverState);
            serverCard.Add(serverSwitch);
            _entryControlCard.Add(serverCard);

            var statusRow = new VisualElement { name = "Map01A Entry Status Row" };
            ApplyLgoEntryStatusRow(statusRow);
            var statusIcon = LgoLabel("i", 12, UiSubText, true);
            statusIcon.name = "Map01A Entry Status Icon";
            ApplyLgoEntryStatusIcon(statusIcon);
            statusRow.Add(statusIcon);
            _entryStatus = new Label("Sẵn sàng kết nối tới máy chủ Đông Lâm.") { name = "Map01A Entry Safety Note" };
            ApplyLgoEntryStatusLine(_entryStatus);
            statusRow.Add(_entryStatus);
            _entryControlCard.Add(statusRow);

            _root.Add(_entryOverlay);
            UpdateEntryScreen();
        }

        private async void SubmitProductLogin()
        {
            if (_productLoginInFlight) return;
            if (string.IsNullOrWhiteSpace(_entryAccountField?.value) || string.IsNullOrWhiteSpace(_entryPasswordField?.value))
            {
                _entryStatus.text = "Nhập tài khoản và mật khẩu để đăng nhập.";
                return;
            }

            _productLoginInFlight = true;
            _entryLoginButton?.SetEnabled(false);
            _entryStatus.text = "Đang xác thực…";
            try
            {
                EnsureProductAuthClient();
                var response = await _productAuthClient.LoginAsync(
                    _entryAccountField.value.Trim(), _entryPasswordField.value, ProductAuthCancellationToken);
                _productAuthSession.Set(response);
                _entryPasswordField.SetValueWithoutNotify(string.Empty);
                OpenCharacterSelect();
            }
            catch (OperationCanceledException) { }
            catch (AccountApiException exception) when (exception.StatusCode == 401)
            {
                _entryStatus.text = "Tài khoản hoặc mật khẩu không đúng.";
            }
            catch (Exception)
            {
                _entryStatus.text = "Không thể kết nối máy chủ đăng nhập. Vui lòng thử lại.";
            }
            finally
            {
                _productLoginInFlight = false;
                _entryLoginButton?.SetEnabled(true);
            }
        }

        private void AddEntrySideAction(VisualElement parent, string text, string status, string iconId)
        {
            var button = new Button(() =>
            {
                if (_entryStatus != null)
                {
                    _entryStatus.text = status;
                    _entryStatus.style.display = DisplayStyle.Flex;
                }
            })
            {
                name = "Map01A Entry Side Action " + text,
                text = text
            };
            ApplyLgoEntrySideAction(button);
            AttachLgoEntrySideActionIcon(button, _scene.GetMap01AHudIconSprite(iconId));
            parent.Add(button);
        }


        private VisualElement MakeEntryAuthOptions()
        {
            var row = new VisualElement { name = "Map01A Entry Auth Options" };
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.marginTop = -2;
            row.style.marginBottom = 10;

            var rememberWrap = new Button(ToggleRememberAccount) { name = "Map01A Entry Remember Action", text = string.Empty };
            rememberWrap.tooltip = "Lưu tên tài khoản trên thiết bị này";
            ApplyLgoEntryRememberAction(rememberWrap);
            rememberWrap.style.flexDirection = FlexDirection.Row;
            rememberWrap.style.alignItems = Align.Center;
            rememberWrap.style.flexGrow = 1;

            var rememberBox = new VisualElement { name = "Map01A Entry Remember Box" };
            rememberBox.style.width = 13;
            rememberBox.style.height = 13;
            rememberBox.style.marginRight = 6;
            ApplyLgoFrame(rememberBox, new Color(.025f, .075f, .080f, .92f), new Color(.86f, .78f, .48f, .88f));

            var rememberMark = new VisualElement { name = "Map01A Entry Remember Mark" };
            rememberMark.style.width = 7;
            rememberMark.style.height = 7;
            rememberMark.style.marginLeft = 3;
            rememberMark.style.marginTop = 3;
            rememberMark.style.backgroundColor = UiGold;
            rememberMark.style.display = _entryRememberAccount ? DisplayStyle.Flex : DisplayStyle.None;
            _entryRememberMark = rememberMark;
            rememberBox.Add(rememberMark);
            rememberWrap.Add(rememberBox);

            var remember = LgoLabel("Lưu tài khoản", 14, new Color(.86f, .92f, .88f, .92f), true);
            remember.name = "Map01A Entry Remember Account";
            rememberWrap.Add(remember);
            row.Add(rememberWrap);

            var forgot = new Button(OpenPasswordRecovery) { name = "Map01A Entry Forgot Password", text = "Quên mật khẩu" };
            ApplyLgoEntryTextLink(forgot);
            row.Add(forgot);
            return row;
        }

        private void ToggleRememberAccount()
        {
            if (!_entryRememberAccount && string.IsNullOrWhiteSpace(_entryAccountField?.value))
            {
                _entryStatus.text = "Nhập tên tài khoản trước khi bật Lưu tài khoản.";
                _entryStatus.style.display = DisplayStyle.Flex;
                return;
            }

            _entryRememberAccount = !_entryRememberAccount;
            if (_entryRememberAccount)
                PlayerPrefs.SetString(RememberedAccountKey, _entryAccountField.value.Trim());
            else
                PlayerPrefs.DeleteKey(RememberedAccountKey);
            PlayerPrefs.Save();
            if (_entryRememberMark != null)
                _entryRememberMark.style.display = _entryRememberAccount ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private TextField MakeEntryField(string fieldName, string placeholderText, string iconId, bool password)
        {
            var field = new TextField { name = fieldName, isPasswordField = password };
            field.textEdition.placeholder = placeholderText;
            ApplyLgoEntryTextField(field);
            AttachLgoEntryFieldIcon(field, _scene.GetMap01AHudIconSprite(iconId));
            if (password)
            {
                var reveal = new Button(() => field.isPasswordField = !field.isPasswordField)
                {
                    name = "Map01A Entry Password Reveal",
                    text = string.Empty,
                    tooltip = "Hiện hoặc ẩn mật khẩu"
                };
                ApplyLgoEntryPasswordReveal(reveal, _scene.GetMap01AHudIconSprite("eye"));
                field.style.paddingRight = 50;
                field.Add(reveal);
            }
            return field;
        }

        private void LayoutEntryScreen(RuntimeUiLayoutProfile layout, Rect safePanelRect)
        {
            if (_entryPanel == null || _entryBrandStage == null) return;
            var centerX = safePanelRect.x + safePanelRect.width * .5f;
            var cardWidth = Mathf.Min(layout.LoginCardWidth, Mathf.Max(0f, safePanelRect.width - 24f));
            var panelLeft = Mathf.Clamp(centerX - cardWidth * .5f, safePanelRect.x + 12f,
                Mathf.Max(safePanelRect.x + 12f, safePanelRect.xMax - cardWidth - 12f));
            Place(_entryPanel, panelLeft, null, safePanelRect.y + layout.EntryPanelTop, null);
            _entryPanel.style.width = cardWidth;
            _entryControlCard.style.width = Length.Percent(100);

            var brandWidth = Mathf.Min(layout.LoginLogoWidth, Mathf.Max(0f, safePanelRect.width - 32f));
            Place(_entryBrandStage, centerX - brandWidth * .5f, null, safePanelRect.y + layout.EntryBrandTop, null);
            _entryBrandStage.style.width = brandWidth;
            _entryBrandStage.style.height = layout.EntryBrandHeight;

            Place(_entrySlogan, safePanelRect.x + layout.EntrySloganLeft, null,
                safePanelRect.y + layout.EntrySloganTop, null);
            _entrySlogan.style.width = Mathf.Min(layout.EntrySloganWidth, safePanelRect.width * .30f);

            _entryNoticePanel.style.width = Mathf.Min(layout.EntryNoticeWidth, safePanelRect.width - 48f);
            Place(_entryNoticePanel, safePanelRect.x + 24f, null, null,
                Mathf.Max(18f, _metrics.PanelHeight - safePanelRect.yMax + layout.EntryNoticeBottom));

            Place(_entrySideActions, null, Mathf.Max(18f, _metrics.PanelWidth - safePanelRect.xMax + layout.EntryUtilityRight),
                safePanelRect.y + layout.EntryUtilityTop, null);
            Place(_entrySignature, null, Mathf.Max(24f, _metrics.PanelWidth - safePanelRect.xMax + layout.EntrySignatureRight),
                null, Mathf.Max(24f, _metrics.PanelHeight - safePanelRect.yMax + layout.EntrySignatureBottom));
        }

        private bool ShouldShowEntryOnLaunch() => ShouldShowEntryOnLaunchForArgs(
            Environment.GetCommandLineArgs(), _scene != null && _scene.IsCapturing);

        public static bool ShouldShowEntryOnLaunchForArgs(string[] args, bool sceneIsCapturing)
        {
            if (Array.IndexOf(args, "--lgo-map01a-skip-entry") >= 0) return false;
            if (Array.IndexOf(args, "--lgo-map01a-server-select-capture") >= 0) return true;
            if (Array.IndexOf(args, "--lgo-map01a-register-capture") >= 0) return true;
            if (Array.IndexOf(args, "--lgo-map01a-password-recovery-capture") >= 0) return true;
            if (Array.IndexOf(args, "--lgo-map01a-password-recovery-verify-capture") >= 0) return true;
            if (Array.IndexOf(args, "--lgo-map01a-password-recovery-new-password-capture") >= 0) return true;
            if (Array.IndexOf(args, "--lgo-map01a-character-select-capture") >= 0) return false;
            if (Array.IndexOf(args, "--lgo-map01a-inventory-tabs-capture") >= 0) return false;
            if (Array.IndexOf(args, "--lgo-map01a-menu-capture") >= 0) return false;
            if (sceneIsCapturing) return false;
            return true;
        }

        private void UpdateEntryScreen()
        {
            if (_entryOverlay == null) return;
            _entryOverlay.style.display = _entryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateEntryControlCardVisibility();
            if (_safe != null) _safe.style.display = _entryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            if (_marker != null) _marker.style.display = _entryOpen ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void UpdateEntryControlCardVisibility()
        {
            if (_entryControlCard == null) return;
            _entryControlCard.style.display = _serverSelectOpen || _registerOpen || _passwordRecoveryOpen
                ? DisplayStyle.None
                : DisplayStyle.Flex;
        }
    }
}
