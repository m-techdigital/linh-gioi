package com.linhgioi.server.api.persistence;

public record SaveMap01AStateCommand(String characterId, float laneX, int facing) {
    public SaveMap01AStateCommand {
        if (characterId == null || characterId.isBlank()) {
            throw new IllegalArgumentException("characterId must not be blank");
        }
        if (!Float.isFinite(laneX)
                || laneX < CharacterRuntimeState.MAP01A_MIN_LANE_X
                || laneX > CharacterRuntimeState.MAP01A_MAX_LANE_X) {
            throw new IllegalArgumentException("Map01A laneX must be finite and inside [-3.8, 44.4]");
        }
        if (facing != -1 && facing != 1) {
            throw new IllegalArgumentException("Map01A facing must be -1 or +1");
        }
    }
}
