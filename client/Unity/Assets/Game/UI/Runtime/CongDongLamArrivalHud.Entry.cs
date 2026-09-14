using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _entryOverlay;
        private Label _entryStatus;
        private TextField _entryAccountField, _entryPasswordField;
        private VisualElement _entryRememberMark;
        private bool _entryRememberAccount;
        private bool _entryOpen;
        private const string RememberedAccountKey = "lgo.map01a.entry.remembered-account";

        private void BuildEntryScreen()
        {
            _entryOpen = ShouldShowEntryOnLaunch();
            _entryOverlay = new VisualElement { name = "Map01A Entry Overlay" };
            _entryOverlay.style.position = Position.Absolute;
            _entryOverlay.style.left = 0;
            _entryOverlay.style.right = 0;
            _entryOverlay.style.top = 0;
            _entryOverlay.style.bottom = 0;
            _entryOverlay.style.backgroundColor = new Color(.010f, .026f, .050f, .28f);
            _entryOverlay.style.justifyContent = Justify.Center;
            _entryOverlay.style.alignItems = Align.Center;

            var notice = new VisualElement { name = "Map01A Entry Notice Panel" };
            notice.style.position = Position.Absolute;
            notice.style.left = 24;
            notice.style.bottom = 28;
            notice.style.width = 530;
            ApplyLgoStatusCard(notice, 18, 12);
            var noticeTitle = LgoLabel("Thông Báo", 18, UiGold, true);
            noticeTitle.name = "Map01A Entry Notice Title";
            noticeTitle.style.marginBottom = 8;
            notice.Add(noticeTitle);
            var noticeLine = new Label("• Máy chủ S1 · Đông Lâm đang hoạt động ổn định") { name = "Map01A Entry Notice Line" };
            noticeLine.style.color = new Color(.90f, .94f, .90f, .92f);
            noticeLine.style.fontSize = 14;
            noticeLine.style.marginBottom = 4;
            notice.Add(noticeLine);
            var noticeEvent = new Label("• Tu luyện đăng nhập · nhận quà theo lịch sự kiện") { name = "Map01A Entry Notice Event" };
            noticeEvent.style.color = new Color(.80f, .88f, .92f, .88f);
            noticeEvent.style.fontSize = 14;
            notice.Add(noticeEvent);
            _entryOverlay.Add(notice);

            var sideActions = new VisualElement { name = "Map01A Entry Side Actions" };
            sideActions.style.position = Position.Absolute;
            sideActions.style.right = 24;
            sideActions.style.top = 72;
            sideActions.style.width = 88;
            _entryOverlay.Add(sideActions);
            AddEntrySideAction(sideActions, "Thông Báo", "Thông báo máy chủ Đông Lâm đang mở ở góc trái dưới.", "notice");
            AddEntrySideAction(sideActions, "Hỗ Trợ", "Trung tâm hỗ trợ hiện chưa khả dụng.", "support");
            AddEntrySideAction(sideActions, "Cinematic", "Cinematic giới thiệu hiện chưa khả dụng.", "cinematic");
            AddEntrySideAction(sideActions, "Cài Đặt", "Cài đặt nâng cao hiện chưa khả dụng.", "menu");

            var panelGlow = new VisualElement { name = "Map01A Entry Panel Glow" };
            panelGlow.style.position = Position.Absolute;
            panelGlow.style.width = Length.Percent(40);
            panelGlow.style.minWidth = 570;
            panelGlow.style.maxWidth = 650;
            panelGlow.style.height = 650;
            panelGlow.style.alignSelf = Align.Center;
            ApplyLgoSoftGlow(panelGlow, .24f);
            _entryOverlay.Add(panelGlow);

            var panel = new VisualElement { name = "Map01A Entry Panel" };
            ApplyLgoEntryShell(panel);
            _entryOverlay.Add(panel);

            var brandCrest = new VisualElement { name = "Map01A Entry Brand Crest", pickingMode = PickingMode.Ignore };
            ApplyLgoEntryBrandCrest(brandCrest, _scene.GetMap01AHudIconSprite("crest"));
            panel.Add(brandCrest);

            var logo = new Label("LINH GIỚI") { name = "Map01A Entry Logo" };
            RuntimeUiTypography.ApplyHeadingFont(logo);
            logo.style.fontSize = 72;
            logo.style.letterSpacing = 5;
            logo.style.color = new Color(.96f, .98f, 1f, .98f);
            logo.style.unityTextAlign = TextAnchor.MiddleCenter;
            logo.style.marginTop = -8;
            panel.Add(logo);

            var logoOnline = new Label("O  N  L  I  N  E") { name = "Map01A Entry Logo Online" };
            logoOnline.style.fontSize = 14;
            logoOnline.style.letterSpacing = 4;
            logoOnline.style.color = new Color(.90f, .94f, 1f, .94f);
            logoOnline.style.unityTextAlign = TextAnchor.MiddleCenter;
            logoOnline.style.marginTop = -8;
            logoOnline.style.marginBottom = 4;
            panel.Add(logoOnline);

            var subtitle = LgoSubtitleLabel("Kiếm trong tay — Chính nghĩa trong lòng", 16, TextAnchor.MiddleCenter);
            subtitle.name = "Map01A Entry Subtitle";
            subtitle.style.marginBottom = 4;
            panel.Add(subtitle);

            var controlCard = new VisualElement { name = "Map01A Entry Control Card" };
            ApplyLgoEntryControlCard(controlCard);
            panel.Add(controlCard);

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
            controlCard.Add(_entryAccountField);
            controlCard.Add(_entryPasswordField);
            controlCard.Add(MakeEntryAuthOptions());

            var authActions = new VisualElement { name = "Map01A Entry Auth Actions" };
            authActions.style.flexDirection = FlexDirection.Row;
            authActions.style.marginBottom = 14;
            var login = new Button(() =>
            {
                _entryStatus.text = string.IsNullOrWhiteSpace(_entryAccountField?.value)
                    || string.IsNullOrWhiteSpace(_entryPasswordField?.value)
                    ? "Nhập tài khoản và mật khẩu để đăng nhập."
                    : "Dịch vụ đăng nhập chưa kết nối. Vui lòng thử lại sau.";
            })
            {
                name = "Map01A Entry Login Button",
                text = "Đăng nhập"
            };
            ApplyLgoEntryAuthAction(login, true);
            login.style.flexGrow = 1;
            login.style.marginRight = 10;
            var register = new Button(() =>
            {
                _entryStatus.text = "Đăng ký tài khoản hiện chưa khả dụng.";
            })
            {
                name = "Map01A Entry Register Button",
                text = "Đăng ký"
            };
            ApplyLgoEntryAuthAction(register, false);
            register.style.flexGrow = 1;
            authActions.Add(login);
            authActions.Add(register);
            controlCard.Add(authActions);

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
            var serverSwitch = new Button { name = "Map01A Entry Server Switch", text = "›" };
            ApplyLgoEntrySecondaryAction(serverSwitch, minWidth: 40);
            serverSwitch.style.fontSize = 24;
            serverSwitch.style.paddingLeft = serverSwitch.style.paddingRight = 8;
            serverCard.Add(serverIcon);
            serverCard.Add(serverName);
            serverCard.Add(serverState);
            serverCard.Add(serverSwitch);
            controlCard.Add(serverCard);

            _entryStatus = new Label("Sẵn sàng kết nối tới máy chủ Đông Lâm.") { name = "Map01A Entry Safety Note" };
            ApplyLgoEntryStatusLine(_entryStatus);
            controlCard.Add(_entryStatus);

            _root.Add(_entryOverlay);
            UpdateEntryScreen();
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

            var forgot = new Button { name = "Map01A Entry Forgot Password", text = "Quên mật khẩu" };
            ApplyLgoEntrySecondaryAction(forgot);
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

        private bool ShouldShowEntryOnLaunch() => ShouldShowEntryOnLaunchForArgs(
            Environment.GetCommandLineArgs(), _scene != null && _scene.IsCapturing);

        public static bool ShouldShowEntryOnLaunchForArgs(string[] args, bool sceneIsCapturing)
        {
            if (Array.IndexOf(args, "--lgo-map01a-skip-entry") >= 0) return false;
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
            if (_safe != null) _safe.style.display = _entryOpen ? DisplayStyle.None : DisplayStyle.Flex;
            if (_marker != null) _marker.style.display = _entryOpen ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
