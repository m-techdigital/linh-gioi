package com.linhgioi.server.api.account;

import com.linhgioi.server.api.persistence.CharacterRuntimeState;

public record CharacterRuntimeStateResponse(
        String mapId,
        float laneX,
        int facing,
        long updatedAtUnixMs) {
    static CharacterRuntimeStateResponse from(CharacterRuntimeState state) {
        if (state == null) return null;
        return new CharacterRuntimeStateResponse(
                state.mapId(), state.laneX(), state.facing(), state.updatedAtUnixMs());
    }
}
