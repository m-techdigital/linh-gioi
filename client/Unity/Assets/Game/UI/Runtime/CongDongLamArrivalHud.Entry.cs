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
            _entryOverlay.style.backgroundColor = new Color(.010f, .026f, .050f, .34f);
            _entryOverlay.style.justifyContent = Justify.Center;
            _entryOverlay.style.alignItems = Align.Center;

            var leftBanner = new Label("ĐÔNG\nLÂM") { name = "Map01A Entry Left Banner" };
            leftBanner.style.position = Position.Absolute;
            leftBanner.style.left = 42;
            leftBanner.style.top = 90;
            leftBanner.style.width = 92;
            leftBanner.style.height = 260;
            leftBanner.style.unityTextAlign = TextAnchor.MiddleCenter;
            leftBanner.style.fontSize = 30;
            leftBanner.style.unityFontStyleAndWeight = FontStyle.Bold;
            leftBanner.style.color = new Color(.86f, .72f, .42f, .88f);
            ApplyLgoFrame(leftBanner, new Color(.035f, .080f, .130f, .72f), new Color(.80f, .62f, .32f, .55f));
            leftBanner.style.backgroundColor = new Color(.010f, .030f, .050f, .24f);
            leftBanner.style.borderTopWidth = leftBanner.style.borderBottomWidth = 0;
            leftBanner.style.borderLeftWidth = leftBanner.style.borderRightWidth = 0;
            _entryOverlay.Add(leftBanner);

            var notice = new VisualElement { name = "Map01A Entry Notice Panel" };
            notice.style.position = Position.Absolute;
            notice.style.left = 28;
            notice.style.bottom = 28;
            notice.style.width = 430;
            ApplyLgoStatusCard(notice, 14, 10);
            var noticeTitle = LgoLabel("Thông Báo", 17, UiGold, true);
            noticeTitle.name = "Map01A Entry Notice Title";
            noticeTitle.style.marginBottom = 6;
            notice.Add(noticeTitle);
            var noticeLine = new Label("• Máy chủ S1 · Đông Lâm đang hoạt động ổn định") { name = "Map01A Entry Notice Line" };
            noticeLine.style.color = new Color(.90f, .94f, .90f, .92f);
            noticeLine.style.fontSize = 14;
            notice.Add(noticeLine);
            _entryOverlay.Add(notice);

            var sideActions = new VisualElement { name = "Map01A Entry Side Actions" };
            sideActions.style.position = Position.Absolute;
            sideActions.style.right = 32;
            sideActions.style.top = 80;
            sideActions.style.width = 92;
            _entryOverlay.Add(sideActions);
            AddEntrySideAction(sideActions, "Thông Báo", "Thông báo máy chủ Đông Lâm đang mở ở góc trái dưới.", "notice");
            AddEntrySideAction(sideActions, "Hỗ Trợ", "Trung tâm hỗ trợ hiện chưa khả dụng.", "support");
            AddEntrySideAction(sideActions, "Cinematic", "Cinematic giới thiệu hiện chưa khả dụng.", "cinematic");
            AddEntrySideAction(sideActions, "Cài Đặt", "Cài đặt nâng cao hiện chưa khả dụng.", "menu");

            var panelGlow = new VisualElement { name = "Map01A Entry Panel Glow" };
            panelGlow.style.position = Position.Absolute;
            panelGlow.style.width = Length.Percent(42);
            panelGlow.style.minWidth = 540;
            panelGlow.style.maxWidth = 660;
            panelGlow.style.height = 610;
            panelGlow.style.alignSelf = Align.Center;
            ApplyLgoSoftGlow(panelGlow, .22f);
            _entryOverlay.Add(panelGlow);

            var panel = new VisualElement { name = "Map01A Entry Panel" };
            ApplyLgoEntryShell(panel);
            _entryOverlay.Add(panel);

            var brandCrest = new VisualElement { name = "Map01A Entry Brand Crest", pickingMode = PickingMode.Ignore };
            ApplyLgoEntryBrandCrest(brandCrest, _scene.GetMap01AHudIconSprite("crest"));
            panel.Add(brandCrest);

            var logo = new Label("LINH GIỚI") { name = "Map01A Entry Logo" };
            RuntimeUiTypography.ApplyHeadingFont(logo);
            logo.style.fontSize = 52;
            logo.style.letterSpacing = 4;
            logo.style.color = new Color(.96f, .98f, 1f, .98f);
            logo.style.unityTextAlign = TextAnchor.MiddleCenter;
            panel.Add(logo);

            var logoOnline = new Label("O  N  L  I  N  E") { name = "Map01A Entry Logo Online" };
            logoOnline.style.fontSize = 14;
            logoOnline.style.letterSpacing = 3;
            logoOnline.style.color = new Color(.90f, .94f, 1f, .94f);
            logoOnline.style.unityTextAlign = TextAnchor.MiddleCenter;
            logoOnline.style.marginTop = -5;
            logoOnline.style.marginBottom = 4;
            panel.Add(logoOnline);

            var subtitle = LgoSubtitleLabel("Kiếm trong tay — Chính nghĩa trong lòng", 16, TextAnchor.MiddleCenter);
            subtitle.name = "Map01A Entry Subtitle";
            subtitle.style.marginBottom = 8;
            panel.Add(subtitle);

            var motto = LgoLabel("Chính nghĩa trong lòng · Bước vào Cổng Đông Lâm", 15, UiGold, true);
            motto.name = "Map01A Entry Hero Motto";
            motto.style.unityTextAlign = TextAnchor.MiddleCenter;
            motto.style.marginBottom = 16;
            motto.style.display = DisplayStyle.None;
            panel.Add(motto);

            var controlCard = new VisualElement { name = "Map01A Entry Control Card" };
            ApplyLgoEntryControlCard(controlCard);
            panel.Add(controlCard);

            var ornamentTop = new VisualElement { name = "Map01A Entry Ornament Top" };
            ornamentTop.style.height = 2;
            ornamentTop.style.width = Length.Percent(62);
            ornamentTop.style.alignSelf = Align.Center;
            ornamentTop.style.marginBottom = 12;
            ornamentTop.style.backgroundColor = new Color(.95f, .75f, .36f, .70f);
            controlCard.Add(ornamentTop);

            var loginTitle = LgoTitleLabel("Đăng nhập", 20);
            loginTitle.name = "Map01A Entry Login Title";
            loginTitle.style.display = DisplayStyle.None;
            controlCard.Add(loginTitle);

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

            var authScope = LgoSubtitleLabel("Đăng nhập để tiếp tục hành trình tại Đông Lâm.", 13);
            authScope.name = "Map01A Entry Auth Scope";
            authScope.style.marginBottom = 10;
            authScope.style.display = DisplayStyle.None;
            controlCard.Add(authScope);

            var authActions = new VisualElement { name = "Map01A Entry Secondary Actions" };
            authActions.style.flexDirection = FlexDirection.Row;
            authActions.style.marginBottom = 10;
            var login = new Button(() =>
            {
                _entryStatus.text = string.IsNullOrWhiteSpace(_entryAccountField?.value)
                    || string.IsNullOrWhiteSpace(_entryPasswordField?.value)
                    ? "Nhập tài khoản và mật khẩu để đăng nhập."
                    : "Dịch vụ tài khoản hiện chưa khả dụng. Có thể chọn Bắt đầu để vào Đông Lâm.";
                _entryStatus.style.display = DisplayStyle.Flex;
            })
            {
                name = "Map01A Entry Login Button",
                text = "Đăng nhập"
            };
            ApplyLgoEntryAuthAction(login, true);
            login.style.flexGrow = 1;
            login.style.marginRight = 8;
            var register = new Button(() =>
            {
                _entryStatus.text = "Đăng ký tài khoản hiện chưa khả dụng.";
                _entryStatus.style.display = DisplayStyle.Flex;
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
            serverCard.style.flexDirection = FlexDirection.Row;
            serverCard.style.alignItems = Align.Center;
            serverCard.style.marginBottom = 8;
            ApplyLgoDetailCard(serverCard, 14, 10);
            var serverIcon = CreateLgoEntryIcon("Map01A Entry Server Icon", _scene.GetMap01AHudIconSprite("server"), 30);
            serverIcon.style.marginRight = 10;
            var serverName = LgoLabel("Máy chủ · S1 · Đông Lâm", 17, new Color(.86f, .94f, .90f, .96f), true);
            serverName.name = "Map01A Entry Server Name";
            serverName.style.flexGrow = 1;
            var serverState = LgoLabel("● Mượt", 16, new Color(.58f, 1f, .36f, .96f), true);
            serverState.name = "Map01A Entry Server State";
            serverState.style.whiteSpace = WhiteSpace.NoWrap;
            serverState.style.flexShrink = 0;
            serverState.style.minWidth = 68;
            serverState.style.marginRight = 8;
            var serverSwitch = new Button { name = "Map01A Entry Server Switch", text = "Đổi máy chủ" };
            ApplyLgoEntrySecondaryAction(serverSwitch, minWidth: 142);
            serverCard.Add(serverIcon);
            serverCard.Add(serverName);
            serverCard.Add(serverState);
            serverCard.Add(serverSwitch);
            controlCard.Add(serverCard);

            var brandSeal = LgoLabel("S1 · Đông Lâm · Khu an toàn", 14, UiGold, true);
            brandSeal.name = "Map01A Entry Brand Seal";
            brandSeal.style.unityTextAlign = TextAnchor.MiddleCenter;
            brandSeal.style.marginBottom = 8;
            brandSeal.style.display = DisplayStyle.None;
            panel.Add(brandSeal);

            _entryStatus = new Label("Sẵn sàng kết nối tới máy chủ Đông Lâm.") { name = "Map01A Entry Safety Note" };
            _entryStatus.style.fontSize = 14;
            _entryStatus.style.whiteSpace = WhiteSpace.Normal;
            _entryStatus.style.color = new Color(.72f, .86f, .92f, .90f);
            _entryStatus.style.marginBottom = 16;
            _entryStatus.style.display = DisplayStyle.None;
            panel.Add(_entryStatus);

            var primaryCta = new VisualElement { name = "Map01A Entry Primary Cta Row" };
            primaryCta.style.flexDirection = FlexDirection.Row;
            primaryCta.style.alignItems = Align.Center;
            primaryCta.style.justifyContent = Justify.Center;
            primaryCta.style.marginTop = 2;
            panel.Add(primaryCta);

            var ctaLeft = MakeEntryCtaOrnament("Map01A Entry Cta Ornament Left");
            var start = new Button(CloseEntryScreen)
            {
                name = "Map01A Entry Start Button",
                text = "Bắt đầu"
            };
            ApplyLgoEntryCtaAction(start, true);
            start.style.width = Length.Percent(64);
            start.style.maxWidth = 420;
            start.style.marginLeft = 12;
            start.style.marginRight = 12;
            var ctaRight = MakeEntryCtaOrnament("Map01A Entry Cta Ornament Right");
            primaryCta.Add(ctaLeft);
            primaryCta.Add(start);
            primaryCta.Add(ctaRight);

            var character = new Label("Nhân vật: LụcThiên · Cổng Đông Lâm") { name = "Map01A Entry Character Summary" };
            character.style.fontSize = 13;
            character.style.color = new Color(.90f, .86f, .72f, .96f);
            character.style.unityTextAlign = TextAnchor.MiddleCenter;
            character.style.marginTop = 12;
            character.style.display = DisplayStyle.None;
            panel.Add(character);

            _root.Add(_entryOverlay);
            UpdateEntryScreen();
        }

        private static VisualElement MakeEntryCtaOrnament(string name)
        {
            var rail = new VisualElement { name = name };
            rail.style.flexGrow = 1;
            rail.style.maxWidth = 82;
            rail.style.minWidth = 42;
            rail.style.marginTop = 2;
            ApplyLgoOrnamentRail(rail);
            return rail;
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

        private void CloseEntryScreen()
        {
            _entryOpen = false;
            UpdateEntryScreen();
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
