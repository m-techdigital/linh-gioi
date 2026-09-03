package com.linhgioi.server.api.account;

import com.linhgioi.server.api.persistence.DevLoginResult;
import java.util.List;

public record DevLoginResponse(AccountResponse account, boolean created, List<CharacterResponse> characters) {
    static DevLoginResponse from(DevLoginResult result, List<CharacterResponse> characters) {
        return new DevLoginResponse(AccountResponse.from(result.account()), result.created(), characters);
    }
}
