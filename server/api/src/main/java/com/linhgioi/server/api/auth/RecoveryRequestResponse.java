package com.linhgioi.server.api.auth;

public record RecoveryRequestResponse(String challengeId, long expiresAtUnixMs, long resendAvailableAtUnixMs) {
    public static RecoveryRequestResponse from(PasswordRecoveryService.RequestResult result) {
        return new RecoveryRequestResponse(result.challengeId(), result.expiresAtUnixMs(), result.resendAvailableAtUnixMs());
    }
}
