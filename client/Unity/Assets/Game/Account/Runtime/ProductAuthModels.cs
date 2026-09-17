using System;

namespace LinhGioi.Account
{
    [Serializable]
    public sealed class ProductLoginRequest
    {
        public string identifier;
        public string password;

        public ProductLoginRequest(string identifier, string password)
        {
            this.identifier = identifier;
            this.password = password;
        }
    }

    [Serializable]
    public sealed class ProductLoginResponse
    {
        public AccountResponse account;
        public string accessToken;
        public long expiresAtUnixMs;
    }

    [Serializable]
    public sealed class ProductSessionResponse
    {
        public AccountResponse account;
        public long expiresAtUnixMs;
    }
}
