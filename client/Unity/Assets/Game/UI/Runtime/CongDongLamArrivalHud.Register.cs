using System;
using LinhGioi.Account;
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
        private Button _registerSubmitButton;
        private bool _registerAgreementAccepted;
        private bool _registerOpen;

        private void BuildRegister()
        {
            _registerOverlay = new VisualElement { name = "Map01A Register Overlay" };
            ApplyLgoRegisterPanel(_registerOverlay);

            var header = new VisualElement { name = "Map01A Register Header", pickingMode = PickingMode.Ignore };
            ApplyLgoRegisterHeader(header);
            var title = LgoTitleLabel("ĐĂNG KÝ TÀI KHOẢN", 28, TextAnchor.MiddleCenter);
            title.name = "Map01A Register Title";
            title.style.whiteSpace = WhiteSpace.NoWrap;
            header.Add(title);

            var subtitleRow = new VisualElement { name = "Map01A Register Subtitle Row", pickingMode = PickingMode.Ignore };
            ApplyLgoRegisterSubtitleRow(subtitleRow);
            var subtitleOrnamentLeft = new VisualElement { name = "Map01A Register Subtitle Ornament Left", pickingMode = PickingMode.Ignore };
            ApplyLgoOrnamentRail(subtitleOrnamentLeft);
            var subtitle = LgoSubtitleLabel("Bắt đầu hành trình tại Đông Lâm", 14, TextAnchor.MiddleCenter);
            subtitle.name = "Map01A Register Subtitle";
            var subtitleOrnamentRight = new VisualElement { name = "Map01A Register Subtitle Ornament Right", pickingMode = PickingMode.Ignore };
            ApplyLgoOrnamentRail(subtitleOrnamentRight);
            subtitleRow.Add(subtitleOrnamentLeft);
            subtitleRow.Add(subtitle);
            subtitleRow.Add(subtitleOrnamentRight);
            header.Add(subtitleRow);
            _registerOverlay.Add(header);

            _registerAccountField = MakeRegisterField(
                "Map01A Register Account Field", "Email đăng nhập", "account", false, string.Empty);
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
            ApplyLgoRegisterAgreementCheckFrame(box);
            _registerAgreementMark = new VisualElement { name = "Map01A Register Agreement Mark", pickingMode = PickingMode.Ignore };
            ApplyLgoRegisterAgreementMark(_registerAgreementMark);
            _registerAgreementMark.style.display = DisplayStyle.None;
            box.Add(_registerAgreementMark);
            agreement.Add(box);
            var agreementCopy = LgoLabel("Tôi đồng ý", 14, UiText);
            agreementCopy.name = "Map01A Register Agreement Copy";
            agreement.Add(agreementCopy);
            var agreementTerms = LgoLabel("Điều khoản sử dụng", 14, UiText);
            agreementTerms.name = "Map01A Register Agreement Terms";
            ApplyLgoRegisterTerms(agreementTerms);
            agreement.Add(agreementTerms);
            _registerOverlay.Add(agreement);

            _registerStatus = LgoSubtitleLabel("", 13, TextAnchor.MiddleCenter);
            _registerStatus.name = "Map01A Register Status";
            _registerStatus.style.minHeight = 24;
            _registerStatus.style.whiteSpace = WhiteSpace.NoWrap;
            _registerOverlay.Add(_registerStatus);

            _registerSubmitButton = new Button(SubmitRegister)
            {
                name = "Map01A Register Submit",
                text = "Tạo tài khoản"
            };
            ApplyLgoRegisterPrimary(_registerSubmitButton);
            _registerOverlay.Add(_registerSubmitButton);

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

        private async void SubmitRegister()
        {
            if (_productRegisterInFlight) return;
            var email = NormalizeProductEmail(_registerAccountField?.value);
            if (email == null || string.IsNullOrWhiteSpace(_registerPasswordField?.value)
                || string.IsNullOrWhiteSpace(_registerConfirmPasswordField?.value))
            {
                _registerStatus.text = "Nhập email hợp lệ và hai lần mật khẩu.";
                return;
            }
            if (_registerPasswordField.value.Length < 8 || _registerPasswordField.value.Length > 128)
            {
                _registerStatus.text = "Mật khẩu phải từ 8 đến 128 ký tự.";
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

            _productRegisterInFlight = true;
            _registerSubmitButton?.SetEnabled(false);
            _registerStatus.text = "Đang tạo tài khoản…";
            try
            {
                EnsureProductAccountClient();
                await _productAccountClient.RegisterAsync(email, _registerPasswordField.value, true,
                    ProductAuthCancellationToken);
                _registerPasswordField.SetValueWithoutNotify(string.Empty);
                _registerConfirmPasswordField.SetValueWithoutNotify(string.Empty);
                _registerOpen = false;
                _entryOpen = true;
                _entryAccountField?.SetValueWithoutNotify(email);
                if (_entryStatus != null) _entryStatus.text = "Tạo tài khoản thành công. Hãy đăng nhập.";
                UpdateRegisterScreen();
                UpdateEntryScreen();
            }
            catch (OperationCanceledException) { }
            catch (AccountApiException exception) when (exception.StatusCode == 409)
            {
                _registerStatus.text = "Email này đã được đăng ký.";
            }
            catch (AccountApiException exception) when (exception.StatusCode == 400)
            {
                _registerStatus.text = "Thông tin đăng ký chưa hợp lệ.";
            }
            catch (Exception)
            {
                _registerStatus.text = "Không thể kết nối dịch vụ đăng ký. Vui lòng thử lại.";
            }
            finally
            {
                _productRegisterInFlight = false;
                _registerSubmitButton?.SetEnabled(true);
            }
        }

        private static string NormalizeProductEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var normalized = value.Trim().ToLowerInvariant();
            if (normalized.Length < 3 || normalized.Length > 254) return null;
            var at = normalized.IndexOf('@');
            return at > 0 && at == normalized.LastIndexOf('@') && at < normalized.Length - 1 ? normalized : null;
        }

        private void OpenRegister()
        {
            _registerOpen = true;
            _serverSelectOpen = false;
            _passwordRecoveryOpen = false;
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
            AnimateLgoCharacterHubSwap(_registerOverlay);
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
