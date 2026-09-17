using System;

namespace LinhGioi.Account
{
    public static class ProductAccountRoutes
    {
        public const string Register = "/auth/register";
        public const string RecoveryRequest = "/auth/recovery/request";
        public const string RecoveryVerify = "/auth/recovery/verify";
        public const string RecoveryReset = "/auth/recovery/reset";
    }

    [Serializable]
    public sealed class ProductRegisterRequest
    {
        public string email;
        public string password;
        public bool acceptedTerms;
        public ProductRegisterRequest(string email, string password, bool acceptedTerms)
        {
            this.email = email;
            this.password = password;
            this.acceptedTerms = acceptedTerms;
        }
    }

    [Serializable]
    public sealed class ProductRegisterResponse
    {
        public AccountResponse account;
    }

    [Serializable]
    public sealed class PasswordRecoveryRequest
    {
        public string email;
        public PasswordRecoveryRequest(string email) { this.email = email; }
    }

    [Serializable]
    public sealed class PasswordRecoveryRequestResponse
    {
        public string challengeId;
        public long expiresAtUnixMs;
        public long resendAvailableAtUnixMs;
    }

    [Serializable]
    public sealed class PasswordRecoveryVerifyRequest
    {
        public string challengeId;
        public string code;
        public PasswordRecoveryVerifyRequest(string challengeId, string code)
        {
            this.challengeId = challengeId;
            this.code = code;
        }
    }

    [Serializable]
    public sealed class PasswordRecoveryVerifyResponse
    {
        public string resetToken;
        public long expiresAtUnixMs;
    }

    [Serializable]
    public sealed class PasswordRecoveryResetRequest
    {
        public string resetToken;
        public string newPassword;
        public PasswordRecoveryResetRequest(string resetToken, string newPassword)
        {
            this.resetToken = resetToken;
            this.newPassword = newPassword;
        }
    }
}
