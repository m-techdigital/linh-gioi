package com.linhgioi.server.api.account;

import com.linhgioi.server.api.persistence.AccountProfile;

public record AccountResponse(String accountId, String displayName, long createdAtUnixMs, long updatedAtUnixMs) {
    static AccountResponse from(AccountProfile account) {
        return new AccountResponse(account.accountId(), account.displayName(), account.createdAtUnixMs(), account.updatedAtUnixMs());
    }
}
