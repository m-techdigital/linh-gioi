using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _passwordRecoveryOverlay;
        private TextField _passwordRecoveryAccountField;
        private Label _passwordRecoveryStatus;
        private bool _passwordRecoveryOpen;

        private void BuildPasswordRecovery()
        {
            _passwordRecoveryOverlay = new VisualElement { name = "Map01A Password Recovery Overlay" };
            ApplyLgoAuthFlowPanel(_passwordRecoveryOverlay, 410);

            var title = LgoTitleLabel("KHÔI PHỤC MẬT KHẨU", 25, TextAnchor.MiddleCenter);
            title.name = "Map01A Password Recovery Title";
            title.style.whiteSpace = WhiteSpace.NoWrap;
            _passwordRecoveryOverlay.Add(title);

            var subtitle = LgoSubtitleLabel(
                "Nhận hướng dẫn bảo mật qua email đã đăng ký", 14, TextAnchor.MiddleCenter);
            subtitle.name = "Map01A Password Recovery Subtitle";
            subtitle.style.marginTop = 4;
            subtitle.style.marginBottom = 20;
            subtitle.style.whiteSpace = WhiteSpace.NoWrap;
            _passwordRecoveryOverlay.Add(subtitle);

            var guidance = LgoLabel(
                "Nhập tài khoản hoặc email liên kết với nhân vật của bạn.",
                14, new Color(.86f, .92f, .94f, .94f));
            guidance.name = "Map01A Password Recovery Guidance";
            guidance.style.marginBottom = 10;
            guidance.style.whiteSpace = WhiteSpace.NoWrap;
            _passwordRecoveryOverlay.Add(guidance);

            _passwordRecoveryAccountField = MakeEntryField(
                "Map01A Password Recovery Account Field", "Tài khoản / Email", "account", false);
            _passwordRecoveryOverlay.Add(_passwordRecoveryAccountField);

            _passwordRecoveryStatus = LgoSubtitleLabel("", 13, TextAnchor.MiddleCenter);
            _passwordRecoveryStatus.name = "Map01A Password Recovery Status";
            _passwordRecoveryStatus.style.minHeight = 36;
            _passwordRecoveryStatus.style.whiteSpace = WhiteSpace.NoWrap;
            _passwordRecoveryOverlay.Add(_passwordRecoveryStatus);

            var submit = new Button(SubmitPasswordRecoveryRequest)
            {
                name = "Map01A Password Recovery Submit",
                text = "Gửi hướng dẫn"
            };
            ApplyLgoAuthFlowPrimary(submit);
            _passwordRecoveryOverlay.Add(submit);

            var back = new Button(ClosePasswordRecovery)
            {
                name = "Map01A Password Recovery Back",
                text = "Quay lại đăng nhập"
            };
            ApplyLgoAuthFlowBack(back);
            _passwordRecoveryOverlay.Add(back);

            _entryPanel.Add(_passwordRecoveryOverlay);
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-password-recovery-capture") >= 0)
                OpenPasswordRecovery();
            else
                _passwordRecoveryOverlay.style.display = DisplayStyle.None;
        }

        private void SubmitPasswordRecoveryRequest()
        {
            _passwordRecoveryStatus.text = string.IsNullOrWhiteSpace(_passwordRecoveryAccountField?.value)
                ? "Nhập tài khoản hoặc email để nhận hướng dẫn."
                : "Dịch vụ khôi phục mật khẩu chưa kết nối. Vui lòng thử lại sau.";
        }

        private void OpenPasswordRecovery()
        {
            _passwordRecoveryOpen = true;
            _registerOpen = false;
            _serverSelectOpen = false;
            _characterSelectOpen = false;
            _entryOpen = true;
            _passwordRecoveryAccountField?.SetValueWithoutNotify(_entryAccountField?.value ?? string.Empty);
            if (_passwordRecoveryStatus != null) _passwordRecoveryStatus.text = string.Empty;
            UpdateCharacterSelectScreen();
            UpdateServerSelectScreen();
            UpdateRegisterScreen();
            UpdateEntryScreen();
            UpdatePasswordRecoveryScreen();
        }

        private void ClosePasswordRecovery()
        {
            _passwordRecoveryOpen = false;
            _entryOpen = true;
            UpdatePasswordRecoveryScreen();
            UpdateEntryScreen();
        }

        private void UpdatePasswordRecoveryScreen()
        {
            if (_passwordRecoveryOverlay == null) return;
            _passwordRecoveryOverlay.style.display = _passwordRecoveryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateEntryControlCardVisibility();
            UpdateHudShellVisibility();
        }
    }
}
