package com.linhgioi.server.api.auth;

import com.linhgioi.server.api.account.AccountResponse;

public record ProductRegisterResponse(AccountResponse account) {
    public static ProductRegisterResponse from(ProductRegistrationService.RegistrationResult result) {
        return new ProductRegisterResponse(AccountResponse.from(result.account()));
    }
}
