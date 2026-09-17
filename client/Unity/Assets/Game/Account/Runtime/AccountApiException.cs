using System;

namespace LinhGioi.Account
{
    public sealed class AccountApiException : InvalidOperationException
    {
        public long StatusCode { get; }

        public AccountApiException(long statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
