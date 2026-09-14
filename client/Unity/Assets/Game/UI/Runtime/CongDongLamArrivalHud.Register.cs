using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _registerOverlay;
        private TextField _registerAccountField;
        private TextField _registerPasswordField;
        private TextField _registerConfirmPasswordField;
        private VisualElement _registerAgreementMark;
        private Label _registerStatus;
        private bool _registerAgreementAccepted;
        private bool _registerOpen;

        private void BuildRegister()
        {
            _registerOverlay = new VisualElement { name = "Map01A Register Overlay" };
            ApplyLgoRegisterPanel(_registerOverlay);

            var title = LgoTitleLabel("ĐĂNG KÝ TÀI KHOẢN", 25, TextAnchor.MiddleCenter);
            title.name = "Map01A Register Title";
            title.style.whiteSpace = WhiteSpace.NoWrap;
            _registerOverlay.Add(title);

            var subtitle = LgoSubtitleLabel("Bắt đầu hành trình tại Đông Lâm", 14, TextAnchor.MiddleCenter);
            subtitle.name = "Map01A Register Subtitle";
            subtitle.style.marginTop = 3;
            subtitle.style.marginBottom = 14;
            _registerOverlay.Add(subtitle);

            _registerAccountField = MakeRegisterField(
                "Map01A Register Account Field", "Tài khoản / Email", "account", false, string.Empty);
            _registerPasswordField = MakeRegisterField(
                "Map01A Register Password Field", "Mật khẩu", "lock", true, "Map01A Register Password Reveal");
            _registerConfirmPasswordField = MakeRegisterField(
                "Map01A Register Confirm Password Field", "Nhập lại mật khẩu", "lock", true,
                "Map01A Register Confirm Password Reveal");
            _registerOverlay.Add(_registerAccountField);
            _registerOverlay.Add(_registerPasswordField);
            _registerOverlay.Add(_registerConfirmPasswordField);

            var agreement = new Button(ToggleRegisterAgreement)
            {
                name = "Map01A Register Agreement",
                text = string.Empty,
                tooltip = "Đồng ý Điều khoản sử dụng"
            };
            ApplyLgoRegisterAgreement(agreement);
            var box = new VisualElement { name = "Map01A Register Agreement Box", pickingMode = PickingMode.Ignore };
            box.style.width = box.style.height = 18;
            box.style.marginRight = 10;
            ApplyLgoFrame(box, new Color(.025f, .075f, .080f, .92f), new Color(.86f, .78f, .48f, .88f));
            _registerAgreementMark = new VisualElement { name = "Map01A Register Agreement Mark", pickingMode = PickingMode.Ignore };
            _registerAgreementMark.style.width = _registerAgreementMark.style.height = 10;
            _registerAgreementMark.style.marginLeft = _registerAgreementMark.style.marginTop = 3;
            _registerAgreementMark.style.backgroundColor = UiGold;
            _registerAgreementMark.style.display = DisplayStyle.None;
            box.Add(_registerAgreementMark);
            agreement.Add(box);
            agreement.Add(LgoLabel("Tôi đồng ý Điều khoản sử dụng", 14, new Color(.88f, .93f, .95f, .96f)));
            _registerOverlay.Add(agreement);

            _registerStatus = LgoSubtitleLabel("", 13, TextAnchor.MiddleCenter);
            _registerStatus.name = "Map01A Register Status";
            _registerStatus.style.minHeight = 24;
            _registerStatus.style.whiteSpace = WhiteSpace.NoWrap;
            _registerOverlay.Add(_registerStatus);

            var submit = new Button(SubmitRegister)
            {
                name = "Map01A Register Submit",
                text = "Tạo tài khoản"
            };
            ApplyLgoRegisterPrimary(submit);
            _registerOverlay.Add(submit);

            var back = new Button(CloseRegister)
            {
                name = "Map01A Register Back",
                text = "Quay lại đăng nhập"
            };
            ApplyLgoRegisterBack(back);
            _registerOverlay.Add(back);

            _entryPanel.Add(_registerOverlay);
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--lgo-map01a-register-capture") >= 0)
                OpenRegister();
            else
                _registerOverlay.style.display = DisplayStyle.None;
        }

        private TextField MakeRegisterField(string name, string placeholder, string iconId, bool password, string revealName)
        {
            var field = new TextField { name = name, isPasswordField = password };
            field.textEdition.placeholder = placeholder;
            ApplyLgoEntryTextField(field);
            AttachLgoEntryFieldIcon(field, _scene.GetMap01AHudIconSprite(iconId));
            if (!password) return field;
            var reveal = new Button(() => field.isPasswordField = !field.isPasswordField)
            {
                name = revealName,
                text = string.Empty,
                tooltip = "Hiện hoặc ẩn mật khẩu"
            };
            ApplyLgoRegisterPasswordReveal(reveal, _scene.GetMap01AHudIconSprite("eye"));
            field.style.paddingRight = 50;
            field.Add(reveal);
            return field;
        }

        private void ToggleRegisterAgreement()
        {
            _registerAgreementAccepted = !_registerAgreementAccepted;
            if (_registerAgreementMark != null)
                _registerAgreementMark.style.display = _registerAgreementAccepted ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void SubmitRegister()
        {
            if (string.IsNullOrWhiteSpace(_registerAccountField?.value)
                || string.IsNullOrWhiteSpace(_registerPasswordField?.value)
                || string.IsNullOrWhiteSpace(_registerConfirmPasswordField?.value))
            {
                _registerStatus.text = "Nhập đủ tài khoản và hai lần mật khẩu.";
                return;
            }
            if (_registerPasswordField.value != _registerConfirmPasswordField.value)
            {
                _registerStatus.text = "Hai mật khẩu chưa khớp.";
                return;
            }
            if (!_registerAgreementAccepted)
            {
                _registerStatus.text = "Bạn cần đồng ý Điều khoản sử dụng.";
                return;
            }
            _registerStatus.text = "Dịch vụ đăng ký chưa kết nối. Vui lòng thử lại sau.";
        }

        private void OpenRegister()
        {
            _registerOpen = true;
            _serverSelectOpen = false;
            _characterSelectOpen = false;
            _entryOpen = true;
            _registerAgreementAccepted = false;
            if (_registerAgreementMark != null) _registerAgreementMark.style.display = DisplayStyle.None;
            if (_registerStatus != null) _registerStatus.text = string.Empty;
            _registerPasswordField?.SetValueWithoutNotify(string.Empty);
            _registerConfirmPasswordField?.SetValueWithoutNotify(string.Empty);
            UpdateCharacterSelectScreen();
            UpdateServerSelectScreen();
            UpdateEntryScreen();
            UpdateRegisterScreen();
        }

        private void CloseRegister()
        {
            _registerOpen = false;
            _entryOpen = true;
            UpdateRegisterScreen();
            UpdateEntryScreen();
        }

        private void UpdateRegisterScreen()
        {
            if (_registerOverlay == null) return;
            _registerOverlay.style.display = _registerOpen ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateEntryControlCardVisibility();
            UpdateHudShellVisibility();
        }
    }
}
