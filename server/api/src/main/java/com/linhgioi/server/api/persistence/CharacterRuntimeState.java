package com.linhgioi.server.api.persistence;

public record CharacterRuntimeState(
        String mapId,
        float laneX,
        int facing,
        long updatedAtUnixMs) {
    public static final String MAP01A_ID = "map-01a-cong-dong-lam";
    public static final float MAP01A_MIN_LANE_X = -3.8f;
    public static final float MAP01A_MAX_LANE_X = 44.4f;

    public CharacterRuntimeState {
        if (!MAP01A_ID.equals(mapId)) {
            throw new IllegalArgumentException("unsupported runtime mapId");
        }
        if (!Float.isFinite(laneX) || laneX < MAP01A_MIN_LANE_X || laneX > MAP01A_MAX_LANE_X) {
            throw new IllegalArgumentException("Map01A laneX must be finite and inside [-3.8, 44.4]");
        }
        if (facing != -1 && facing != 1) {
            throw new IllegalArgumentException("Map01A facing must be -1 or +1");
        }
        if (updatedAtUnixMs <= 0) {
            throw new IllegalArgumentException("updatedAtUnixMs must be positive");
        }
    }
}
