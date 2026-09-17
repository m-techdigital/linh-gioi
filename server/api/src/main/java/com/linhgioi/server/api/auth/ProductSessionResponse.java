package com.linhgioi.server.api.auth;

import com.linhgioi.server.api.account.AccountResponse;

public record ProductSessionResponse(AccountResponse account, long expiresAtUnixMs) {
    public static ProductSessionResponse from(ProductAuthService.SessionResult result) {
        return new ProductSessionResponse(AccountResponse.from(result.account()), result.expiresAtUnixMs());
    }
}
