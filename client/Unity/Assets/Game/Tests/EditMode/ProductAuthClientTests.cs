using LinhGioi.Account;
using NUnit.Framework;
using UnityEngine;

namespace LinhGioi.Tests.EditMode
{
    public sealed class ProductAuthClientTests
    {
        [Test]
        public void SessionStateStartsEmptyAndTracksSetClearAndExpiry()
        {
            var state = new ProductAuthSessionState();
            Assert.That(state.IsAuthenticated, Is.False);

            var response = new ProductLoginResponse
            {
                account = new AccountResponse { accountId = "account.dev.abc", displayName = "Minh" },
                accessToken = "opaque-token",
                expiresAtUnixMs = 2_000
            };
            state.Set(response);

            Assert.That(state.IsAuthenticated, Is.True);
            Assert.That(state.Account.accountId, Is.EqualTo("account.dev.abc"));
            Assert.That(state.AccessToken, Is.EqualTo("opaque-token"));
            Assert.That(state.IsExpired(1_999), Is.False);
            Assert.That(state.IsExpired(2_000), Is.True);

            state.Clear();
            Assert.That(state.IsAuthenticated, Is.False);
            Assert.That(state.AccessToken, Is.Null);
        }

        [Test]
        public void ProductAuthDtosParseWithoutChangingExistingAccountShape()
        {
            var login = JsonUtility.FromJson<ProductLoginResponse>(
                "{\"account\":{\"accountId\":\"account.dev.abc\",\"displayName\":\"Minh\",\"createdAtUnixMs\":1,\"updatedAtUnixMs\":2},\"accessToken\":\"opaque\",\"expiresAtUnixMs\":1234}");
            var session = JsonUtility.FromJson<ProductSessionResponse>(
                "{\"account\":{\"accountId\":\"account.dev.abc\",\"displayName\":\"Minh\"},\"expiresAtUnixMs\":1234}");

            Assert.That(login.account.displayName, Is.EqualTo("Minh"));
            Assert.That(login.accessToken, Is.EqualTo("opaque"));
            Assert.That(login.expiresAtUnixMs, Is.EqualTo(1234));
            Assert.That(session.account.accountId, Is.EqualTo("account.dev.abc"));
            Assert.That(session.expiresAtUnixMs, Is.EqualTo(1234));
        }

        [Test]
        public void ProductAccountDtosAndRoutesMatchServerContract()
        {
            Assert.That(typeof(IProductAccountClient).IsAssignableFrom(typeof(AccountApiClient)), Is.True);
            Assert.That(ProductAccountRoutes.Register, Is.EqualTo("/auth/register"));
            Assert.That(ProductAccountRoutes.RecoveryRequest, Is.EqualTo("/auth/recovery/request"));
            Assert.That(ProductAccountRoutes.RecoveryVerify, Is.EqualTo("/auth/recovery/verify"));
            Assert.That(ProductAccountRoutes.RecoveryReset, Is.EqualTo("/auth/recovery/reset"));

            var registerJson = JsonUtility.ToJson(new ProductRegisterRequest("Minh@Example.COM", "Secret#123", true));
            var verifyJson = JsonUtility.ToJson(new PasswordRecoveryVerifyRequest("challenge.1", "123456"));
            Assert.That(registerJson, Does.Contain("\"email\":\"Minh@Example.COM\""));
            Assert.That(registerJson, Does.Contain("\"acceptedTerms\":true"));
            Assert.That(verifyJson, Does.Contain("\"challengeId\":\"challenge.1\""));
            Assert.That(typeof(AccountApiClient).GetMethod("RegisterAsync"), Is.Not.Null);
            Assert.That(typeof(AccountApiClient).GetMethod("RequestPasswordRecoveryAsync"), Is.Not.Null);
            Assert.That(typeof(AccountApiClient).GetMethod("VerifyPasswordRecoveryAsync"), Is.Not.Null);
            Assert.That(typeof(AccountApiClient).GetMethod("ResetPasswordAsync"), Is.Not.Null);
        }

        [Test]
        public void ProductCharacterRoutesUseBearerScopedEndpoints()
        {
            Assert.That(typeof(IProductCharacterClient).IsAssignableFrom(typeof(AccountApiClient)), Is.True);
            Assert.That(ProductCharacterRoutes.List, Is.EqualTo("/auth/characters"));
            Assert.That(ProductCharacterRoutes.LoadPrefix, Is.EqualTo("/auth/characters/"));
            Assert.That(typeof(AccountApiClient).GetMethod("ListProductCharactersAsync"), Is.Not.Null);
            Assert.That(typeof(AccountApiClient).GetMethod("LoadProductCharacterAsync"), Is.Not.Null);
        }

        [Test]
        public void ProductCharacterRuntimeStateDtosAndSaveRouteMatchServerContract()
        {
            var character = JsonUtility.FromJson<CharacterResponse>(
                "{\"characterId\":\"character.1\",\"accountId\":\"account.product.1\",\"name\":\"KiemTu\",\"classId\":\"class.sword\",\"runtimeClassId\":\"kiem\",\"runtimeState\":{\"mapId\":\"map-01a-cong-dong-lam\",\"laneX\":18.5,\"facing\":-1,\"updatedAtUnixMs\":1700000000000},\"slot\":1}");

            Assert.That(character.runtimeClassId, Is.EqualTo("kiem"));
            Assert.That(character.runtimeState, Is.Not.Null);
            Assert.That(character.runtimeState.mapId, Is.EqualTo("map-01a-cong-dong-lam"));
            Assert.That(character.runtimeState.laneX, Is.EqualTo(18.5f).Within(.0001f));
            Assert.That(character.runtimeState.facing, Is.EqualTo(-1));
            Assert.That(ProductCharacterRoutes.Map01AStateSuffix, Is.EqualTo("/map01a-state"));
            Assert.That(typeof(AccountApiClient).GetMethod("SaveMap01AStateAsync"), Is.Not.Null);
        }

        [Test]
        public void RecoveryStateTransitionsAndClearRemovesTransientSecrets()
        {
            var state = new ProductAccountRecoveryState();
            state.Begin(" Minh@Example.COM ");
            Assert.That(state.Stage, Is.EqualTo(ProductAccountRecoveryStage.Request));
            state.AcceptRequest(new PasswordRecoveryRequestResponse
            {
                challengeId = "challenge.1", expiresAtUnixMs = 1000, resendAvailableAtUnixMs = 500
            });
            Assert.That(state.Stage, Is.EqualTo(ProductAccountRecoveryStage.Verify));
            Assert.That(state.Email, Is.EqualTo("minh@example.com"));
            state.AcceptVerification(new PasswordRecoveryVerifyResponse { resetToken = "reset-secret", expiresAtUnixMs = 2000 });
            Assert.That(state.Stage, Is.EqualTo(ProductAccountRecoveryStage.NewPassword));
            Assert.That(state.ChallengeId, Is.Null);
            Assert.That(state.ResetToken, Is.EqualTo("reset-secret"));
            state.Clear();
            Assert.That(state.Stage, Is.EqualTo(ProductAccountRecoveryStage.Request));
            Assert.That(state.Email, Is.Null);
            Assert.That(state.ChallengeId, Is.Null);
            Assert.That(state.ResetToken, Is.Null);
        }

        [Test]
        public void AccountApiClientImplementsProductAuthContractAndApiErrorCarriesStatus()
        {
            Assert.That(typeof(IProductAuthClient).IsAssignableFrom(typeof(AccountApiClient)), Is.True);
            var failure = new AccountApiException(401, "safe failure");
            Assert.That(failure.StatusCode, Is.EqualTo(401));
            Assert.That(failure.Message, Is.EqualTo("safe failure"));
        }
    }
}
