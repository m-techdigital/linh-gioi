using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LinhGioi.Account;
using LinhGioi.UI;
using LinhGioi.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.Tests.EditMode
{
    public sealed class ProductAccountUiFlowTests
    {
        [Test]
        public void RegisterProductFlowCallsClientAndReturnsToEntryOnSuccess()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var fake = FakeProductAccountClient.Success();
            try
            {
                var host = new GameObject("register product flow test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene, fake, new ProductAuthSessionState());
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                Invoke(root.Q<Button>("Map01A Entry Register Button"));
                root.Q<TextField>("Map01A Register Account Field").value = " Minh@Example.COM ";
                root.Q<TextField>("Map01A Register Password Field").value = "Secret#123";
                root.Q<TextField>("Map01A Register Confirm Password Field").value = "Secret#123";
                Invoke(root.Q<Button>("Map01A Register Agreement"));
                Invoke(root.Q<Button>("Map01A Register Submit"));

                Assert.That(fake.RegisterCallCount, Is.EqualTo(1));
                Assert.That(root.Q("Map01A Register Overlay").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q("Map01A Entry Control Card").style.display.value, Is.EqualTo(DisplayStyle.Flex));
                Assert.That(root.Q<TextField>("Map01A Entry Account Field").value, Is.EqualTo("minh@example.com"));
                Assert.That(root.Q<TextField>("Map01A Register Password Field").value, Is.Empty);
                Assert.That(root.Q<Label>("Map01A Entry Safety Note").text,
                    Is.EqualTo("Tạo tài khoản thành công. Hãy đăng nhập."));
            }
            finally
            {
                DestroyNewRoots(before);
            }
        }

        [Test]
        public void RecoveryProductFlowAdvancesStagesAndClearsSecretsOnSuccess()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            var fake = FakeProductAccountClient.Success();
            try
            {
                var host = new GameObject("recovery product flow test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene, fake, new ProductAuthSessionState());
                var hud = host.GetComponentInChildren<CongDongLamArrivalHud>();
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                root.Q<TextField>("Map01A Entry Account Field").value = "minh@example.com";
                Invoke(root.Q<Button>("Map01A Entry Forgot Password"));
                Invoke(root.Q<Button>("Map01A Password Recovery Submit"));

                Assert.That(fake.RecoveryRequestCallCount, Is.EqualTo(1));
                var code = root.Q<TextField>("Map01A Password Recovery Code Field");
                Assert.That(code, Is.Not.Null);
                code.value = "123456";
                Invoke(root.Q<Button>("Map01A Password Recovery Verify Submit"));
                Assert.That(fake.RecoveryVerifyCallCount, Is.EqualTo(1));

                var password = root.Q<TextField>("Map01A Password Recovery New Password Field");
                var confirmation = root.Q<TextField>("Map01A Password Recovery Confirm Password Field");
                Assert.That(password, Is.Not.Null);
                password.value = "short";
                confirmation.value = "short";
                Invoke(root.Q<Button>("Map01A Password Recovery New Password Submit"));
                Assert.That(fake.RecoveryResetCallCount, Is.EqualTo(0));
                Assert.That(root.Q<Label>("Map01A Password Recovery Status").text,
                    Is.EqualTo("Mật khẩu phải từ 8 đến 128 ký tự."));
                password.value = "Changed#123";
                confirmation.value = "mismatch";
                Invoke(root.Q<Button>("Map01A Password Recovery New Password Submit"));
                Assert.That(fake.RecoveryResetCallCount, Is.EqualTo(0));
                Assert.That(root.Q<Label>("Map01A Password Recovery Status").text, Is.EqualTo("Hai mật khẩu chưa khớp."));
                confirmation.value = "Changed#123";
                Invoke(root.Q<Button>("Map01A Password Recovery New Password Submit"));
                Assert.That(fake.RecoveryResetCallCount, Is.EqualTo(1));
                Assert.That(root.Q("Map01A Password Recovery Overlay").style.display.value, Is.EqualTo(DisplayStyle.None));
                Assert.That(root.Q<Label>("Map01A Entry Safety Note").text,
                    Is.EqualTo("Mật khẩu đã cập nhật. Hãy đăng nhập."));
                var state = (ProductAccountRecoveryState)typeof(CongDongLamArrivalHud)
                    .GetField("_productRecoveryState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(hud);
                Assert.That(state.ResetToken, Is.Null);
                Assert.That(password.value, Is.Empty);
                Assert.That(confirmation.value, Is.Empty);
            }
            finally
            {
                DestroyNewRoots(before);
            }
        }

        [Test]
        public void RegisterLoadingBlocksDuplicateSubmitAndConflictUsesSafeCopy()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("register pending test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                var pending = FakeProductAccountClient.PendingRegistration();
                CongDongLamArrivalHud.Attach(scene, pending, new ProductAuthSessionState());
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                Invoke(root.Q<Button>("Map01A Entry Register Button"));
                FillValidRegister(root);
                var submit = root.Q<Button>("Map01A Register Submit");
                Invoke(submit);
                Assert.That(pending.RegisterCallCount, Is.EqualTo(1));
                Assert.That(submit.enabledSelf, Is.False);
                Assert.That(root.Q<Label>("Map01A Register Status").text, Is.EqualTo("Đang tạo tài khoản…"));
                Invoke(submit);
                Assert.That(pending.RegisterCallCount, Is.EqualTo(1));
                pending.PendingRegister.TrySetResult(new ProductRegisterResponse
                { account = new AccountResponse { accountId = "account.product.pending", displayName = "minh@example.com" } });
            }
            finally { DestroyNewRoots(before); }

            before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("register conflict test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene, FakeProductAccountClient.RegisterConflict(), new ProductAuthSessionState());
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                Invoke(root.Q<Button>("Map01A Entry Register Button"));
                FillValidRegister(root);
                Invoke(root.Q<Button>("Map01A Register Submit"));
                Assert.That(root.Q<Label>("Map01A Register Status").text, Is.EqualTo("Email này đã được đăng ký."));
            }
            finally { DestroyNewRoots(before); }
        }

        [Test]
        public void RecoveryCooldownAndInvalidVerifyRemainGeneric()
        {
            var before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("recovery cooldown test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene, FakeProductAccountClient.RecoveryCooldown(), new ProductAuthSessionState());
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                root.Q<TextField>("Map01A Entry Account Field").value = "minh@example.com";
                Invoke(root.Q<Button>("Map01A Entry Forgot Password"));
                Invoke(root.Q<Button>("Map01A Password Recovery Submit"));
                var resend = root.Q<Button>("Map01A Password Recovery Resend");
                Assert.That(resend.enabledSelf, Is.False);
                Assert.That(resend.text, Does.Contain("Gửi lại mã sau"));
            }
            finally { DestroyNewRoots(before); }

            before = new HashSet<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects());
            try
            {
                var host = new GameObject("recovery invalid verify test");
                var scene = CongDongLamMap01AArtPreview.Attach(TwoDOnboardingController.Attach(host));
                CongDongLamArrivalHud.Attach(scene, FakeProductAccountClient.RecoveryVerifyUnauthorized(), new ProductAuthSessionState());
                var root = host.GetComponentInChildren<UIDocument>().rootVisualElement;
                root.Q<TextField>("Map01A Entry Account Field").value = "minh@example.com";
                Invoke(root.Q<Button>("Map01A Entry Forgot Password"));
                Invoke(root.Q<Button>("Map01A Password Recovery Submit"));
                root.Q<TextField>("Map01A Password Recovery Code Field").value = "123456";
                Invoke(root.Q<Button>("Map01A Password Recovery Verify Submit"));
                Assert.That(root.Q<Label>("Map01A Password Recovery Status").text,
                    Is.EqualTo("Mã xác minh không hợp lệ hoặc đã hết hạn."));
            }
            finally { DestroyNewRoots(before); }
        }

        private static void FillValidRegister(VisualElement root)
        {
            root.Q<TextField>("Map01A Register Account Field").value = "minh@example.com";
            root.Q<TextField>("Map01A Register Password Field").value = "Secret#123";
            root.Q<TextField>("Map01A Register Confirm Password Field").value = "Secret#123";
            Invoke(root.Q<Button>("Map01A Register Agreement"));
        }

        private static void Invoke(Button button)
        {
            Assert.That(button, Is.Not.Null);
            var callback = typeof(Clickable).GetField("clicked", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(button.clickable) as System.Action;
            Assert.That(callback, Is.Not.Null);
            callback();
        }

        private static void DestroyNewRoots(HashSet<GameObject> before)
        {
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                if (!before.Contains(root)) Object.DestroyImmediate(root);
        }

        private sealed class FakeProductAccountClient : IProductAuthClient, IProductAccountClient
        {
            private readonly Task<ProductRegisterResponse> _registerTask;
            private readonly Task<PasswordRecoveryRequestResponse> _requestTask;
            private readonly Task<PasswordRecoveryVerifyResponse> _verifyTask;
            private readonly Task _resetTask;
            public TaskCompletionSource<ProductRegisterResponse> PendingRegister { get; }
            public int RegisterCallCount { get; private set; }
            public int RecoveryRequestCallCount { get; private set; }
            public int RecoveryVerifyCallCount { get; private set; }
            public int RecoveryResetCallCount { get; private set; }

            private FakeProductAccountClient(Task<ProductRegisterResponse> registerTask = null,
                Task<PasswordRecoveryRequestResponse> requestTask = null,
                Task<PasswordRecoveryVerifyResponse> verifyTask = null, Task resetTask = null,
                TaskCompletionSource<ProductRegisterResponse> pendingRegister = null)
            {
                _registerTask = registerTask;
                _requestTask = requestTask;
                _verifyTask = verifyTask;
                _resetTask = resetTask;
                PendingRegister = pendingRegister;
            }

            public static FakeProductAccountClient Success() => new FakeProductAccountClient();

            public static FakeProductAccountClient PendingRegistration()
            {
                var pending = new TaskCompletionSource<ProductRegisterResponse>();
                return new FakeProductAccountClient(registerTask: pending.Task, pendingRegister: pending);
            }

            public static FakeProductAccountClient RegisterConflict() => new FakeProductAccountClient(
                registerTask: Task.FromException<ProductRegisterResponse>(new AccountApiException(409, "safe conflict")));

            public static FakeProductAccountClient RecoveryCooldown() => new FakeProductAccountClient(
                requestTask: Task.FromResult(new PasswordRecoveryRequestResponse
                {
                    challengeId = "challenge.cooldown", expiresAtUnixMs = 9_999_999_999_999L,
                    resendAvailableAtUnixMs = 9_999_999_999_999L
                }));

            public static FakeProductAccountClient RecoveryVerifyUnauthorized() => new FakeProductAccountClient(
                verifyTask: Task.FromException<PasswordRecoveryVerifyResponse>(new AccountApiException(401, "safe invalid")));

            public Task<ProductLoginResponse> LoginAsync(string identifier, string password,
                CancellationToken cancellationToken)
                => Task.FromException<ProductLoginResponse>(new System.NotSupportedException());

            public Task<ProductSessionResponse> ValidateSessionAsync(string accessToken,
                CancellationToken cancellationToken)
                => Task.FromException<ProductSessionResponse>(new System.NotSupportedException());

            public Task LogoutAsync(string accessToken, CancellationToken cancellationToken) => Task.CompletedTask;

            public Task<ProductRegisterResponse> RegisterAsync(string email, string password, bool acceptedTerms,
                CancellationToken cancellationToken)
            {
                RegisterCallCount++;
                return _registerTask ?? Task.FromResult(new ProductRegisterResponse
                {
                    account = new AccountResponse { accountId = "account.product.abc", displayName = email.Trim().ToLowerInvariant() }
                });
            }

            public Task<PasswordRecoveryRequestResponse> RequestPasswordRecoveryAsync(string email,
                CancellationToken cancellationToken)
            {
                RecoveryRequestCallCount++;
                return _requestTask ?? Task.FromResult(new PasswordRecoveryRequestResponse
                {
                    challengeId = "challenge.1", expiresAtUnixMs = 9_999_999_999_999L, resendAvailableAtUnixMs = 1
                });
            }

            public Task<PasswordRecoveryVerifyResponse> VerifyPasswordRecoveryAsync(string challengeId, string code,
                CancellationToken cancellationToken)
            {
                RecoveryVerifyCallCount++;
                return _verifyTask ?? Task.FromResult(new PasswordRecoveryVerifyResponse
                {
                    resetToken = "reset-secret", expiresAtUnixMs = 9_999_999_999_999L
                });
            }

            public Task ResetPasswordAsync(string resetToken, string newPassword,
                CancellationToken cancellationToken)
            {
                RecoveryResetCallCount++;
                return _resetTask ?? Task.CompletedTask;
            }
        }
    }
}
