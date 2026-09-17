using System;

namespace LinhGioi.Account
{
    public sealed class ProductAuthSessionState
    {
        public AccountResponse Account { get; private set; }
        public string AccessToken { get; private set; }
        public long ExpiresAtUnixMs { get; private set; }
        public bool IsAuthenticated => Account != null && !string.IsNullOrWhiteSpace(AccessToken);

        public void Set(ProductLoginResponse response)
        {
            if (response == null || response.account == null || string.IsNullOrWhiteSpace(response.accessToken)
                || response.expiresAtUnixMs <= 0)
                throw new ArgumentException("Product login response is incomplete.", nameof(response));
            Account = response.account;
            AccessToken = response.accessToken;
            ExpiresAtUnixMs = response.expiresAtUnixMs;
        }

        public bool IsExpired(long nowUnixMs)
        {
            return !IsAuthenticated || ExpiresAtUnixMs <= nowUnixMs;
        }

        public void Clear()
        {
            Account = null;
            AccessToken = null;
            ExpiresAtUnixMs = 0;
        }
    }
}
