package com.linhgioi.server.api.auth;

import com.linhgioi.server.api.account.AccountResponse;

public record ProductLoginResponse(AccountResponse account, String accessToken, long expiresAtUnixMs) {
    public static ProductLoginResponse from(ProductAuthService.LoginResult result) {
        return new ProductLoginResponse(AccountResponse.from(result.account()), result.accessToken(), result.expiresAtUnixMs());
    }
}
