package com.linhgioi.server.api.auth;

public record RecoveryVerifyResponse(String resetToken, long expiresAtUnixMs) {
    public static RecoveryVerifyResponse from(PasswordRecoveryService.VerifyResult result) {
        return new RecoveryVerifyResponse(result.resetToken(), result.expiresAtUnixMs());
    }
}
