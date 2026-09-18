using System;
using LinhGioi.Account;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class CongDongLamArrivalHud
    {
        private VisualElement _passwordRecoveryOverlay, _passwordRecoveryVerifyFooter;
        private Label _passwordRecoveryTitle, _passwordRecoverySubtitle, _passwordRecoveryGuidance;
        private Label _passwordRecoveryMaskedEmail, _passwordRecoveryRule, _passwordRecoveryStatus;
        private TextField _passwordRecoveryAccountField, _passwordRecoveryCodeField;
        private TextField _passwordRecoveryNewPasswordField, _passwordRecoveryConfirmPasswordField;
        private Button _passwordRecoverySubmit, _passwordRecoveryVerifySubmit;
        private Button _passwordRecoveryNewPasswordSubmit, _passwordRecoveryResend, _passwordRecoveryBack;
        private bool _passwordRecoveryOpen;

        private void BuildPasswordRecovery()
        {
            _passwordRecoveryOverlay = RuntimeUiFactory.NewModalSurface("Map01A Password Recovery Overlay");
            ApplyLgoPasswordRecoveryPanel(_passwordRecoveryOverlay);

            var header = new VisualElement { name = "Map01A Password Recovery Header", pickingMode = PickingMode.Ignore };
            ApplyLgoAuthFlowHeader(header);
            _passwordRecoveryTitle = LgoTitleLabel("KHÔI PHỤC MẬT KHẨU", 28, TextAnchor.MiddleCenter);
            _passwordRecoveryTitle.name = "Map01A Password Recovery Title";
            _passwordRecoveryTitle.style.whiteSpace = WhiteSpace.NoWrap;
            header.Add(_passwordRecoveryTitle);

            var subtitleRow = new VisualElement { name = "Map01A Password Recovery Subtitle Row", pickingMode = PickingMode.Ignore };
            ApplyLgoAuthFlowSubtitleRow(subtitleRow);
            var ornamentLeft = new VisualElement { name = "Map01A Password Recovery Ornament Left", pickingMode = PickingMode.Ignore };
            ApplyLgoOrnamentRail(ornamentLeft);
            _passwordRecoverySubtitle = LgoSubtitleLabel(
                "Nhận hướng dẫn bảo mật qua email đã đăng ký", 14, TextAnchor.MiddleCenter);
            _passwordRecoverySubtitle.name = "Map01A Password Recovery Subtitle";
            var ornamentRight = new VisualElement { name = "Map01A Password Recovery Ornament Right", pickingMode = PickingMode.Ignore };
            ApplyLgoOrnamentRail(ornamentRight);
            subtitleRow.Add(ornamentLeft);
            subtitleRow.Add(_passwordRecoverySubtitle);
            subtitleRow.Add(ornamentRight);
            header.Add(subtitleRow);
            _passwordRecoveryOverlay.Add(header);

            _passwordRecoveryGuidance = LgoLabel(
                "Nhập email đã dùng để đăng ký tài khoản.", 14, new Color(.86f, .92f, .94f, .94f));
            _passwordRecoveryGuidance.name = "Map01A Password Recovery Guidance";
            _passwordRecoveryGuidance.style.marginBottom = 10;
            _passwordRecoveryOverlay.Add(_passwordRecoveryGuidance);

            _passwordRecoveryAccountField = MakeEntryField(
                "Map01A Password Recovery Account Field", "Email đăng ký", "account", false);
            _passwordRecoveryOverlay.Add(_passwordRecoveryAccountField);
            _passwordRecoveryMaskedEmail = LgoSubtitleLabel("", 13, TextAnchor.MiddleCenter);
            _passwordRecoveryMaskedEmail.name = "Map01A Password Recovery Masked Email";
            _passwordRecoveryMaskedEmail.style.marginBottom = 8;
            _passwordRecoveryOverlay.Add(_passwordRecoveryMaskedEmail);

            _passwordRecoveryCodeField = MakeEntryField(
                "Map01A Password Recovery Code Field", "Mã xác minh 6 số", "lock", false);
            _passwordRecoveryOverlay.Add(_passwordRecoveryCodeField);

            _passwordRecoveryNewPasswordField = MakeRegisterField(
                "Map01A Password Recovery New Password Field", "Mật khẩu mới", "lock", true,
                "Map01A Password Recovery New Password Reveal");
            _passwordRecoveryConfirmPasswordField = MakeRegisterField(
                "Map01A Password Recovery Confirm Password Field", "Nhập lại mật khẩu mới", "lock", true,
                "Map01A Password Recovery Confirm Password Reveal");
            _passwordRecoveryOverlay.Add(_passwordRecoveryNewPasswordField);
            _passwordRecoveryOverlay.Add(_passwordRecoveryConfirmPasswordField);

            _passwordRecoveryRule = LgoSubtitleLabel("", 13, TextAnchor.MiddleCenter);
            _passwordRecoveryRule.name = "Map01A Password Recovery Rule";
            ApplyLgoPasswordRecoveryRule(_passwordRecoveryRule);
            _passwordRecoveryOverlay.Add(_passwordRecoveryRule);

            _passwordRecoveryStatus = LgoSubtitleLabel("", 13, TextAnchor.MiddleCenter);
            _passwordRecoveryStatus.name = "Map01A Password Recovery Status";
            _passwordRecoveryStatus.style.minHeight = 36;
            _passwordRecoveryStatus.style.whiteSpace = WhiteSpace.Normal;
            _passwordRecoveryOverlay.Add(_passwordRecoveryStatus);

            _passwordRecoverySubmit = new Button(SubmitPasswordRecoveryRequest)
            { name = "Map01A Password Recovery Submit", text = "Gửi mã xác minh" };
            ApplyLgoAuthFlowPrimary(_passwordRecoverySubmit);
            _passwordRecoveryOverlay.Add(_passwordRecoverySubmit);
            _passwordRecoveryVerifySubmit = new Button(SubmitPasswordRecoveryVerification)
            { name = "Map01A Password Recovery Verify Submit", text = "Xác minh" };
            ApplyLgoAuthFlowPrimary(_passwordRecoveryVerifySubmit);
            _passwordRecoveryOverlay.Add(_passwordRecoveryVerifySubmit);

            _passwordRecoveryNewPasswordSubmit = new Button(SubmitPasswordRecoveryReset)
            { name = "Map01A Password Recovery New Password Submit", text = "Cập nhật mật khẩu" };
            ApplyLgoAuthFlowPrimary(_passwordRecoveryNewPasswordSubmit);
            _passwordRecoveryOverlay.Add(_passwordRecoveryNewPasswordSubmit);

            _passwordRecoveryVerifyFooter = new VisualElement { name = "Map01A Password Recovery Verify Footer" };
            ApplyLgoPasswordRecoveryVerifyFooter(_passwordRecoveryVerifyFooter);
            var resendPrompt = LgoSubtitleLabel("Chưa nhận được mã?", 13, TextAnchor.MiddleCenter);
            resendPrompt.name = "Map01A Password Recovery Resend Prompt";
            resendPrompt.AddToClassList("lgo-password-recovery-resend-prompt");
            _passwordRecoveryVerifyFooter.Add(resendPrompt);
            _passwordRecoveryResend = new Button(ResendPasswordRecoveryCode)
            { name = "Map01A Password Recovery Resend", text = "Gửi lại mã" };
            ApplyLgoAuthFlowBack(_passwordRecoveryResend);
            _passwordRecoveryVerifyFooter.Add(_passwordRecoveryResend);
            _passwordRecoveryOverlay.Add(_passwordRecoveryVerifyFooter);
            _passwordRecoveryBack = new Button(HandlePasswordRecoveryBack)
            { name = "Map01A Password Recovery Back", text = "Quay lại đăng nhập" };
            ApplyLgoAuthFlowBack(_passwordRecoveryBack);
            _passwordRecoveryOverlay.Add(_passwordRecoveryBack);

            _entryPanel.Add(_passwordRecoveryOverlay);
            var captureArgs = Environment.GetCommandLineArgs();
            var captureRequested = Array.IndexOf(captureArgs, "--lgo-map01a-password-recovery-capture") >= 0
                || Array.IndexOf(captureArgs, "--lgo-map01a-password-recovery-verify-capture") >= 0
                || Array.IndexOf(captureArgs, "--lgo-map01a-password-recovery-new-password-capture") >= 0;
            if (captureRequested)
            {
                OpenPasswordRecovery();
                PreparePasswordRecoveryCaptureStage(captureArgs);
            }
            else
                _passwordRecoveryOverlay.style.display = DisplayStyle.None;
            RefreshPasswordRecoveryStage();
        }

        private void PreparePasswordRecoveryCaptureStage(string[] args)
        {
            var verifyCapture = Array.IndexOf(args, "--lgo-map01a-password-recovery-verify-capture") >= 0;
            var newPasswordCapture = Array.IndexOf(args, "--lgo-map01a-password-recovery-new-password-capture") >= 0;
            if (!verifyCapture && !newPasswordCapture) return;
            const string reviewEmail = "owner-review@example.test";
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _productRecoveryState.Begin(reviewEmail);
            _passwordRecoveryAccountField?.SetValueWithoutNotify(reviewEmail);
            _productRecoveryState.AcceptRequest(new PasswordRecoveryRequestResponse
            {
                challengeId = "capture-challenge",
                expiresAtUnixMs = now + 600_000,
                resendAvailableAtUnixMs = now + 45_000
            });
            if (newPasswordCapture)
            {
                _productRecoveryState.AcceptVerification(new PasswordRecoveryVerifyResponse
                {
                    resetToken = "capture-grant",
                    expiresAtUnixMs = now + 600_000
                });
            }
            ClearPasswordRecoverySecrets();
            if (_passwordRecoveryStatus != null) _passwordRecoveryStatus.text = string.Empty;
            RefreshPasswordRecoveryStage();
        }

        private async void SubmitPasswordRecoveryRequest()
        {
            if (_productRecoveryInFlight) return;
            var email = NormalizeProductEmail(_passwordRecoveryAccountField?.value);
            if (email == null)
            {
                _passwordRecoveryStatus.text = "Nhập email đăng ký hợp lệ.";
                return;
            }
            _productRecoveryInFlight = true;
            SetPasswordRecoveryActionsEnabled(false);
            _passwordRecoveryStatus.text = "Đang gửi mã xác minh…";
            try
            {
                EnsureProductAccountClient();
                _productRecoveryState.Begin(email);
                var response = await _productAccountClient.RequestPasswordRecoveryAsync(
                    email, ProductAuthCancellationToken);
                _productRecoveryState.AcceptRequest(response);
                _passwordRecoveryAccountField.SetValueWithoutNotify(email);
                _passwordRecoveryStatus.text = "Nếu email hợp lệ, mã xác minh đã được gửi.";
                RefreshPasswordRecoveryStage();
            }
            catch (OperationCanceledException) { }
            catch (AccountApiException exception) when (exception.StatusCode == 429)
            {
                _passwordRecoveryStatus.text = "Vui lòng chờ trước khi gửi lại mã.";
            }
            catch (AccountApiException exception) when (exception.StatusCode == 503)
            {
                _passwordRecoveryStatus.text = "Dịch vụ gửi mã hiện chưa khả dụng.";
            }
            catch (AccountApiException exception) when (exception.StatusCode == 400)
            {
                _passwordRecoveryStatus.text = "Email khôi phục chưa hợp lệ.";
            }
            catch (Exception)
            {
                _passwordRecoveryStatus.text = "Không thể kết nối dịch vụ khôi phục. Vui lòng thử lại.";
            }
            finally
            {
                _productRecoveryInFlight = false;
                SetPasswordRecoveryActionsEnabled(true);
                UpdatePasswordRecoveryCooldown();
            }
        }

        private async void SubmitPasswordRecoveryVerification()
        {
            if (_productRecoveryInFlight) return;
            var code = _passwordRecoveryCodeField?.value?.Trim();
            if (string.IsNullOrWhiteSpace(code) || code.Length != 6 || !int.TryParse(code, out _))
            {
                _passwordRecoveryStatus.text = "Nhập mã xác minh gồm đúng 6 chữ số.";
                return;
            }
            _productRecoveryInFlight = true;
            SetPasswordRecoveryActionsEnabled(false);
            _passwordRecoveryStatus.text = "Đang xác minh mã…";
            try
            {
                EnsureProductAccountClient();
                var response = await _productAccountClient.VerifyPasswordRecoveryAsync(
                    _productRecoveryState.ChallengeId, code, ProductAuthCancellationToken);
                _productRecoveryState.AcceptVerification(response);
                _passwordRecoveryCodeField.SetValueWithoutNotify(string.Empty);
                _passwordRecoveryStatus.text = string.Empty;
                RefreshPasswordRecoveryStage();
            }
            catch (OperationCanceledException) { }
            catch (AccountApiException exception) when (exception.StatusCode == 401)
            {
                _passwordRecoveryStatus.text = "Mã xác minh không hợp lệ hoặc đã hết hạn.";
            }
            catch (Exception)
            {
                _passwordRecoveryStatus.text = "Không thể xác minh mã lúc này. Vui lòng thử lại.";
            }
            finally
            {
                _productRecoveryInFlight = false;
                SetPasswordRecoveryActionsEnabled(true);
                UpdatePasswordRecoveryCooldown();
            }
        }

        private async void SubmitPasswordRecoveryReset()
        {
            if (_productRecoveryInFlight) return;
            var password = _passwordRecoveryNewPasswordField?.value;
            var confirmation = _passwordRecoveryConfirmPasswordField?.value;
            if (string.IsNullOrEmpty(password) || password.Length < 8 || password.Length > 128)
            {
                _passwordRecoveryStatus.text = "Mật khẩu phải từ 8 đến 128 ký tự.";
                return;
            }
            if (password != confirmation)
            {
                _passwordRecoveryStatus.text = "Hai mật khẩu chưa khớp.";
                return;
            }
            _productRecoveryInFlight = true;
            SetPasswordRecoveryActionsEnabled(false);
            _passwordRecoveryStatus.text = "Đang cập nhật mật khẩu…";
            var email = _productRecoveryState.Email;
            try
            {
                EnsureProductAccountClient();
                await _productAccountClient.ResetPasswordAsync(
                    _productRecoveryState.ResetToken, password, ProductAuthCancellationToken);
                ClearPasswordRecoverySecrets();
                _productRecoveryState.Clear();
                _passwordRecoveryOpen = false;
                _entryOpen = true;
                if (!string.IsNullOrWhiteSpace(email)) _entryAccountField?.SetValueWithoutNotify(email);
                if (_entryStatus != null) _entryStatus.text = "Mật khẩu đã cập nhật. Hãy đăng nhập.";
                UpdatePasswordRecoveryScreen();
                UpdateEntryScreen();
            }
            catch (OperationCanceledException) { }
            catch (AccountApiException exception) when (exception.StatusCode == 401)
            {
                _passwordRecoveryStatus.text = "Phiên đặt lại mật khẩu đã hết hạn. Hãy yêu cầu mã mới.";
                _productRecoveryState.ReturnToRequest();
                ClearPasswordRecoverySecrets();
                RefreshPasswordRecoveryStage();
            }
            catch (AccountApiException exception) when (exception.StatusCode == 400)
            {
                _passwordRecoveryStatus.text = "Mật khẩu mới chưa hợp lệ.";
            }
            catch (Exception)
            {
                _passwordRecoveryStatus.text = "Không thể cập nhật mật khẩu lúc này. Vui lòng thử lại.";
            }
            finally
            {
                _productRecoveryInFlight = false;
                SetPasswordRecoveryActionsEnabled(true);
            }
        }

        private async void ResendPasswordRecoveryCode()
        {
            if (_productRecoveryInFlight || _productRecoveryState.Stage != ProductAccountRecoveryStage.Verify) return;
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (!_productRecoveryState.CanResend(now))
            {
                UpdatePasswordRecoveryCooldown();
                return;
            }
            _productRecoveryInFlight = true;
            SetPasswordRecoveryActionsEnabled(false);
            _passwordRecoveryStatus.text = "Đang gửi lại mã…";
            try
            {
                EnsureProductAccountClient();
                var response = await _productAccountClient.RequestPasswordRecoveryAsync(
                    _productRecoveryState.Email, ProductAuthCancellationToken);
                _productRecoveryState.AcceptRequest(response);
                _passwordRecoveryCodeField.SetValueWithoutNotify(string.Empty);
                _passwordRecoveryStatus.text = "Nếu email hợp lệ, mã xác minh mới đã được gửi.";
                RefreshPasswordRecoveryStage();
            }
            catch (OperationCanceledException) { }
            catch (AccountApiException exception) when (exception.StatusCode == 429)
            {
                _passwordRecoveryStatus.text = "Vui lòng chờ trước khi gửi lại mã.";
            }
            catch (AccountApiException exception) when (exception.StatusCode == 503)
            {
                _passwordRecoveryStatus.text = "Dịch vụ gửi mã hiện chưa khả dụng.";
            }
            catch (Exception)
            {
                _passwordRecoveryStatus.text = "Không thể gửi lại mã lúc này. Vui lòng thử lại.";
            }
            finally
            {
                _productRecoveryInFlight = false;
                SetPasswordRecoveryActionsEnabled(true);
                UpdatePasswordRecoveryCooldown();
            }
        }

        private void HandlePasswordRecoveryBack()
        {
            if (_productRecoveryInFlight) return;
            if (_productRecoveryState.Stage == ProductAccountRecoveryStage.Verify)
            {
                _productRecoveryState.ReturnToRequest();
                _passwordRecoveryCodeField?.SetValueWithoutNotify(string.Empty);
                if (_passwordRecoveryStatus != null) _passwordRecoveryStatus.text = string.Empty;
                RefreshPasswordRecoveryStage();
                return;
            }
            ClosePasswordRecovery();
        }

        private void OpenPasswordRecovery()
        {
            _passwordRecoveryOpen = true;
            _registerOpen = false;
            _serverSelectOpen = false;
            _characterSelectOpen = false;
            _entryOpen = true;
            var initial = _entryAccountField?.value ?? string.Empty;
            _productRecoveryState.Begin(initial);
            _passwordRecoveryAccountField?.SetValueWithoutNotify(initial);
            ClearPasswordRecoverySecrets();
            if (_passwordRecoveryStatus != null) _passwordRecoveryStatus.text = string.Empty;
            RefreshPasswordRecoveryStage();
            UpdateCharacterSelectScreen();
            UpdateServerSelectScreen();
            UpdateRegisterScreen();
            UpdateEntryScreen();
            UpdatePasswordRecoveryScreen();
            AnimateLgoSurfaceSwap(_passwordRecoveryOverlay);
        }

        private void ClosePasswordRecovery()
        {
            _passwordRecoveryOpen = false;
            _entryOpen = true;
            _productRecoveryState.Clear();
            ClearPasswordRecoverySecrets();
            if (_passwordRecoveryStatus != null) _passwordRecoveryStatus.text = string.Empty;
            UpdatePasswordRecoveryScreen();
            UpdateEntryScreen();
        }

        private void ClearPasswordRecoverySecrets()
        {
            _passwordRecoveryCodeField?.SetValueWithoutNotify(string.Empty);
            _passwordRecoveryNewPasswordField?.SetValueWithoutNotify(string.Empty);
            _passwordRecoveryConfirmPasswordField?.SetValueWithoutNotify(string.Empty);
        }

        private void RefreshPasswordRecoveryStage()
        {
            if (_passwordRecoveryOverlay == null) return;
            var stage = _productRecoveryState?.Stage ?? ProductAccountRecoveryStage.Request;
            var request = stage == ProductAccountRecoveryStage.Request;
            var verify = stage == ProductAccountRecoveryStage.Verify;
            var reset = stage == ProductAccountRecoveryStage.NewPassword;

            _passwordRecoveryTitle.text = request ? "KHÔI PHỤC MẬT KHẨU" : verify ? "XÁC MINH MÃ" : "ĐẶT MẬT KHẨU MỚI";
            _passwordRecoverySubtitle.text = request ? "Nhận mã bảo mật qua email đã đăng ký"
                : verify ? "Nhập mã 6 số đã gửi tới email của bạn"
                : "Tạo mật khẩu mới cho tài khoản của bạn";
            _passwordRecoveryGuidance.style.display = request ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryAccountField.style.display = request ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryMaskedEmail.style.display = verify ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryMaskedEmail.text = verify ? MaskRecoveryEmail(_productRecoveryState.Email) : string.Empty;
            _passwordRecoveryCodeField.style.display = verify ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryNewPasswordField.style.display = reset ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryConfirmPasswordField.style.display = reset ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryRule.text = verify ? "Mã có hiệu lực trong 10 phút · Tối đa 5 lần thử"
                : reset ? "Từ 8 đến 128 ký tự" : string.Empty;
            _passwordRecoveryRule.style.display = verify || reset ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoverySubmit.style.display = request ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryVerifySubmit.style.display = verify ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryNewPasswordSubmit.style.display = reset ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryVerifyFooter.style.display = verify ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryResend.style.display = verify ? DisplayStyle.Flex : DisplayStyle.None;
            _passwordRecoveryBack.text = verify ? "Quay lại" : "Quay lại đăng nhập";
            UpdatePasswordRecoveryCooldown();
        }

        private void UpdatePasswordRecoveryCooldown()
        {
            if (!_passwordRecoveryOpen || _passwordRecoveryResend == null || _productRecoveryState == null
                || _productRecoveryState.Stage != ProductAccountRecoveryStage.Verify) return;
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var remainingMs = Math.Max(0, _productRecoveryState.ResendAvailableAtUnixMs - now);
            var seconds = (remainingMs + 999) / 1000;
            _passwordRecoveryResend.text = seconds > 0 ? $"Gửi lại mã sau {seconds}s" : "Gửi lại mã";
            _passwordRecoveryResend.SetEnabled(!_productRecoveryInFlight && seconds <= 0);
        }

        private void SetPasswordRecoveryActionsEnabled(bool enabled)
        {
            _passwordRecoverySubmit?.SetEnabled(enabled);
            _passwordRecoveryVerifySubmit?.SetEnabled(enabled);
            _passwordRecoveryNewPasswordSubmit?.SetEnabled(enabled);
            _passwordRecoveryBack?.SetEnabled(enabled);
            if (enabled) UpdatePasswordRecoveryCooldown();
            else _passwordRecoveryResend?.SetEnabled(false);
        }

        private static string MaskRecoveryEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return string.Empty;
            var at = email.IndexOf('@');
            if (at <= 0) return "***";
            var local = email.Substring(0, at);
            var visible = local.Length <= 1 ? local : local.Substring(0, 1);
            return visible + "***" + email.Substring(at);
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
