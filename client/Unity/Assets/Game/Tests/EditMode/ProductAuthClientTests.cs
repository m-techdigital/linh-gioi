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
        public void AccountApiClientImplementsProductAuthContractAndApiErrorCarriesStatus()
        {
            Assert.That(typeof(IProductAuthClient).IsAssignableFrom(typeof(AccountApiClient)), Is.True);
            var failure = new AccountApiException(401, "safe failure");
            Assert.That(failure.StatusCode, Is.EqualTo(401));
            Assert.That(failure.Message, Is.EqualTo("safe failure"));
        }
    }
}
