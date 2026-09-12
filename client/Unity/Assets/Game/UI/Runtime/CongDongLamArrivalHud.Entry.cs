using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _entryOverlay;
        private Label _entryStatus;
        private bool _entryOpen;

        private void BuildEntryScreen()
        {
            _entryOpen = ShouldShowEntryOnLaunch();
            _entryOverlay = new VisualElement { name = "Map01A Entry Overlay" };
            _entryOverlay.style.position = Position.Absolute;
            _entryOverlay.style.left = 0;
            _entryOverlay.style.right = 0;
            _entryOverlay.style.top = 0;
            _entryOverlay.style.bottom = 0;
            _entryOverlay.style.backgroundColor = new Color(.010f, .026f, .050f, .94f);
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
            _entryOverlay.Add(leftBanner);

            var notice = new VisualElement { name = "Map01A Entry Notice Panel" };
            notice.style.position = Position.Absolute;
            notice.style.left = 28;
            notice.style.bottom = 28;
            notice.style.width = 430;
            notice.style.paddingLeft = 14;
            notice.style.paddingRight = 14;
            notice.style.paddingTop = 10;
            notice.style.paddingBottom = 10;
            ApplyLgoFrame(notice, new Color(.018f, .055f, .090f, .82f), new Color(.56f, .68f, .72f, .42f));
            notice.Add(new Label("Thông Báo") { name = "Map01A Entry Notice Title" });
            var noticeLine = new Label("• Cổng Đông Lâm mở bản review 2D · UI còn trong giai đoạn chỉnh mỹ thuật") { name = "Map01A Entry Notice Line" };
            noticeLine.style.color = new Color(.90f, .94f, .90f, .92f);
            noticeLine.style.fontSize = 14;
            notice.Add(noticeLine);
            _entryOverlay.Add(notice);

            var sideActions = new VisualElement { name = "Map01A Entry Side Actions" };
            sideActions.style.position = Position.Absolute;
            sideActions.style.right = 32;
            sideActions.style.top = 80;
            sideActions.style.width = 132;
            _entryOverlay.Add(sideActions);
            AddEntrySideAction(sideActions, "Thông Báo");
            AddEntrySideAction(sideActions, "Cài Đặt");
            AddEntrySideAction(sideActions, "Hỗ Trợ");

            var panel = new VisualElement { name = "Map01A Entry Panel" };
            panel.style.width = Length.Percent(44);
            panel.style.minWidth = 500;
            panel.style.maxWidth = 720;
            panel.style.paddingLeft = 24;
            panel.style.paddingRight = 24;
            panel.style.paddingTop = 22;
            panel.style.paddingBottom = 22;
            ApplyLgoModalShell(panel, 24);
            _entryOverlay.Add(panel);

            var logo = new Label("LINH GIỚI ONLINE") { name = "Map01A Entry Logo" };
            logo.style.unityFontStyleAndWeight = FontStyle.Bold;
            logo.style.fontSize = 42;
            logo.style.color = new Color(.96f, .98f, 1f, .98f);
            logo.style.unityTextAlign = TextAnchor.MiddleCenter;
            panel.Add(logo);

            var subtitle = new Label("Kiếm trong tay — Chính nghĩa trong lòng") { name = "Map01A Entry Subtitle" };
            subtitle.style.fontSize = 16;
            subtitle.style.color = new Color(.72f, .86f, .92f, .92f);
            subtitle.style.unityTextAlign = TextAnchor.MiddleCenter;
            subtitle.style.marginBottom = 18;
            panel.Add(subtitle);

            var loginTitle = new Label("Đăng nhập") { name = "Map01A Entry Login Title" };
            loginTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            loginTitle.style.fontSize = 22;
            loginTitle.style.color = new Color(.95f, .75f, .36f, .96f);
            panel.Add(loginTitle);

            panel.Add(MakeEntryField("Map01A Entry Account Field", "Map01A Entry Account Placeholder", "👤  Tài khoản / Email / Số điện thoại"));
            panel.Add(MakeEntryField("Map01A Entry Password Field", "Map01A Entry Password Placeholder", "🔒  Mật khẩu"));

            var authScope = new Label("review local: form hiển thị theo UI/UX, chưa gửi tài khoản hoặc mật khẩu thật.")
            {
                name = "Map01A Entry Auth Scope"
            };
            authScope.style.fontSize = 13;
            authScope.style.color = new Color(.66f, .82f, .90f, .88f);
            authScope.style.marginBottom = 10;
            panel.Add(authScope);

            var serverCard = new VisualElement { name = "Map01A Entry Server Card" };
            serverCard.style.backgroundColor = new Color(.010f, .035f, .060f, .82f);
            serverCard.style.paddingLeft = 14;
            serverCard.style.paddingRight = 14;
            serverCard.style.paddingTop = 10;
            serverCard.style.paddingBottom = 10;
            serverCard.style.marginBottom = 8;
            ApplyLgoFrame(serverCard, new Color(.010f, .035f, .060f, .82f), new Color(.36f, .60f, .72f, .48f));
            var server = new Label("Máy chủ  S1 · Đông Lâm        ● Mượt") { name = "Map01A Entry Server" };
            server.style.fontSize = 17;
            server.style.color = new Color(.86f, .94f, .90f, .96f);
            serverCard.Add(server);
            panel.Add(serverCard);

            _entryStatus = new Label("Tài khoản dev/local dùng cho review UI. Chưa mở mật khẩu, đăng ký hoặc production auth.") { name = "Map01A Entry Safety Note" };
            _entryStatus.style.fontSize = 14;
            _entryStatus.style.whiteSpace = WhiteSpace.Normal;
            _entryStatus.style.color = new Color(.72f, .86f, .92f, .90f);
            _entryStatus.style.marginBottom = 16;
            panel.Add(_entryStatus);

            var actions = new VisualElement { name = "Map01A Entry Actions" };
            actions.style.flexDirection = FlexDirection.Row;
            actions.style.justifyContent = Justify.Center;
            panel.Add(actions);

            var login = new Button(() => _entryStatus.text = "Đăng nhập dev/local đã sẵn sàng cho preview; không gửi mật khẩu hoặc token thật.")
            {
                name = "Map01A Entry Login Button",
                text = "Đăng nhập dev"
            };
            StyleEntryButton(login, false);
            login.style.marginRight = 12;
            actions.Add(login);

            var start = new Button(CloseEntryScreen)
            {
                name = "Map01A Entry Start Button",
                text = "Bắt đầu"
            };
            StyleEntryButton(start, true);
            actions.Add(start);

            var character = new Label("Nhân vật: LụcThiên · Cổng Đông Lâm · dùng runtime Map01A hiện hành") { name = "Map01A Entry Character Summary" };
            character.style.fontSize = 15;
            character.style.color = new Color(.90f, .86f, .72f, .96f);
            character.style.unityTextAlign = TextAnchor.MiddleCenter;
            character.style.marginTop = 18;
            panel.Add(character);

            _root.Add(_entryOverlay);
            UpdateEntryScreen();
        }

        private static void AddEntrySideAction(VisualElement parent, string text)
        {
            var button = new Button(() => { }) { text = text };
            button.style.height = 44;
            button.style.marginBottom = 12;
            button.style.fontSize = 15;
            ApplyLgoButton(button);
            parent.Add(button);
        }

        private static VisualElement MakeEntryField(string fieldName, string placeholderName, string placeholderText)
        {
            var field = new VisualElement { name = fieldName };
            field.style.height = 48;
            field.style.marginBottom = 10;
            field.style.paddingLeft = 16;
            field.style.paddingRight = 16;
            field.style.justifyContent = Justify.Center;
            ApplyLgoFrame(field, new Color(.010f, .035f, .060f, .86f), new Color(.46f, .64f, .74f, .50f));

            var placeholder = new Label(placeholderText) { name = placeholderName };
            placeholder.style.fontSize = 17;
            placeholder.style.color = new Color(.74f, .82f, .88f, .82f);
            field.Add(placeholder);
            return field;
        }

        private static void StyleEntryButton(Button button, bool primary)
        {
            button.style.minWidth = primary ? 190 : 160;
            ApplyLgoButton(button, primary);
        }

        private bool ShouldShowEntryOnLaunch() => ShouldShowEntryOnLaunchForArgs(
            Environment.GetCommandLineArgs(), _scene != null && _scene.IsCapturing);

        public static bool ShouldShowEntryOnLaunchForArgs(string[] args, bool sceneIsCapturing)
        {
            if (Array.IndexOf(args, "--lgo-map01a-skip-entry") >= 0) return false;
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
