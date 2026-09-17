package com.linhgioi.server.api.persistence;

import java.util.List;
import java.util.Optional;

public interface PlayerProfileStore {
    DevLoginResult loginDev(String devKey, String displayName);
    Optional<AccountProfile> findAccount(String accountId);
    AccountProfile createProductAccount(String displayName);
    boolean deleteEmptyProductAccount(String accountId);
    List<CharacterProfile> listCharacters(String accountId);
    CharacterProfile createCharacter(CreateCharacterCommand command);
    Optional<CharacterProfile> findCharacter(String characterId);
    CharacterProfile saveCharacterPosition(SaveCharacterPositionCommand command);
}
