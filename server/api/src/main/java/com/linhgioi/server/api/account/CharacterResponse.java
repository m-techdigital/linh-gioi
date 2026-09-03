package com.linhgioi.server.api.account;

import com.linhgioi.server.api.persistence.CharacterProfile;

public record CharacterResponse(
        String characterId,
        String accountId,
        String name,
        String classId,
        long entityId,
        float x,
        float y,
        float z,
        float yawDegrees,
        long createdAtUnixMs,
        long updatedAtUnixMs) {
    static CharacterResponse from(CharacterProfile character) {
        return new CharacterResponse(
                character.characterId(),
                character.accountId(),
                character.name(),
                character.classId(),
                character.entityId(),
                character.positionX(),
                character.positionY(),
                character.positionZ(),
                character.yawDegrees(),
                character.createdAtUnixMs(),
                character.updatedAtUnixMs());
    }
}
