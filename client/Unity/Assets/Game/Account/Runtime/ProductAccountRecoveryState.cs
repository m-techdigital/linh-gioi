using System;

namespace LinhGioi.Account
{
    public enum ProductAccountRecoveryStage
    {
        Request,
        Verify,
        NewPassword
    }

    public sealed class ProductAccountRecoveryState
    {
        public ProductAccountRecoveryStage Stage { get; private set; } = ProductAccountRecoveryStage.Request;
        public string Email { get; private set; }
        public string ChallengeId { get; private set; }
        public long ChallengeExpiresAtUnixMs { get; private set; }
        public long ResendAvailableAtUnixMs { get; private set; }
        public string ResetToken { get; private set; }
        public long ResetExpiresAtUnixMs { get; private set; }

        public void Begin(string email)
        {
            Clear();
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
        }

        public void AcceptRequest(PasswordRecoveryRequestResponse response)
        {
            if (response == null || string.IsNullOrWhiteSpace(response.challengeId)
                || response.expiresAtUnixMs <= 0 || response.resendAvailableAtUnixMs <= 0)
                throw new ArgumentException("Recovery request response is incomplete.", nameof(response));
            ChallengeId = response.challengeId;
            ChallengeExpiresAtUnixMs = response.expiresAtUnixMs;
            ResendAvailableAtUnixMs = response.resendAvailableAtUnixMs;
            ResetToken = null;
            ResetExpiresAtUnixMs = 0;
            Stage = ProductAccountRecoveryStage.Verify;
        }

        public void AcceptVerification(PasswordRecoveryVerifyResponse response)
        {
            if (response == null || string.IsNullOrWhiteSpace(response.resetToken) || response.expiresAtUnixMs <= 0)
                throw new ArgumentException("Recovery verification response is incomplete.", nameof(response));
            ChallengeId = null;
            ChallengeExpiresAtUnixMs = 0;
            ResendAvailableAtUnixMs = 0;
            ResetToken = response.resetToken;
            ResetExpiresAtUnixMs = response.expiresAtUnixMs;
            Stage = ProductAccountRecoveryStage.NewPassword;
        }

        public bool CanResend(long nowUnixMs)
        {
            return Stage == ProductAccountRecoveryStage.Verify && nowUnixMs >= ResendAvailableAtUnixMs;
        }

        public void ReturnToRequest()
        {
            ChallengeId = null;
            ChallengeExpiresAtUnixMs = 0;
            ResendAvailableAtUnixMs = 0;
            ResetToken = null;
            ResetExpiresAtUnixMs = 0;
            Stage = ProductAccountRecoveryStage.Request;
        }

        public void Clear()
        {
            Stage = ProductAccountRecoveryStage.Request;
            Email = null;
            ChallengeId = null;
            ChallengeExpiresAtUnixMs = 0;
            ResendAvailableAtUnixMs = 0;
            ResetToken = null;
            ResetExpiresAtUnixMs = 0;
        }
    }
}
