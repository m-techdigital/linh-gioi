package com.linhgioi.server.api.persistence;

import java.util.List;
import java.util.Optional;

public interface PlayerProfileStore {
    DevLoginResult loginDev(String devKey, String displayName);
    List<CharacterProfile> listCharacters(String accountId);
    CharacterProfile createCharacter(CreateCharacterCommand command);
    Optional<CharacterProfile> findCharacter(String characterId);
    CharacterProfile saveCharacterPosition(SaveCharacterPositionCommand command);
}
