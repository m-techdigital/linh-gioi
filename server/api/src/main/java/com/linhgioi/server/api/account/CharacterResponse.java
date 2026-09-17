package com.linhgioi.server.api.account;

import com.linhgioi.server.api.persistence.CharacterClassCompatibility;
import com.linhgioi.server.api.persistence.CharacterProfile;
import com.linhgioi.server.api.persistence.CharacterRuntimeState;

public record CharacterResponse(
        String characterId,
        String accountId,
        String name,
        String classId,
        String runtimeClassId,
        CharacterRuntimeStateResponse runtimeState,
        long entityId,
        float x,
        float y,
        float z,
        float yawDegrees,
        long createdAtUnixMs,
        long updatedAtUnixMs,
        int slot) {
    static CharacterResponse from(CharacterProfile character) {
        return from(character, null);
    }

    static CharacterResponse from(CharacterProfile character, CharacterRuntimeState runtimeState) {
        return new CharacterResponse(
                character.characterId(),
                character.accountId(),
                character.name(),
                character.classId(),
                CharacterClassCompatibility.toRuntimeClassId(character.classId()),
                CharacterRuntimeStateResponse.from(runtimeState),
                character.entityId(),
                character.positionX(),
                character.positionY(),
                character.positionZ(),
                character.yawDegrees(),
                character.createdAtUnixMs(),
                character.updatedAtUnixMs(),
                character.slot());
    }
}
