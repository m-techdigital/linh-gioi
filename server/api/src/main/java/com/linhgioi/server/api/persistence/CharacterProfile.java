package com.linhgioi.server.api.persistence;

import java.util.Objects;

public record CharacterProfile(
        String characterId,
        String accountId,
        String name,
        String classId,
        long entityId,
        float positionX,
        float positionY,
        float positionZ,
        float yawDegrees,
        long createdAtUnixMs,
        long updatedAtUnixMs) {
    public CharacterProfile {
        requireIdentifier(characterId, "characterId");
        requireIdentifier(accountId, "accountId");
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("name must not be blank");
        }
        if (name.length() < 3 || name.length() > 16) {
            throw new IllegalArgumentException("name must be between 3 and 16 characters");
        }
        if (!name.matches("[A-Za-z0-9_]+")) {
            throw new IllegalArgumentException("name may only contain letters, numbers, and underscore");
        }
        if (!"class.sword".equals(classId) && !"class.martial".equals(classId)) {
            throw new IllegalArgumentException("classId must be class.sword or class.martial");
        }
        if (entityId <= 0) {
            throw new IllegalArgumentException("entityId must be positive");
        }
        if (!Float.isFinite(positionX) || !Float.isFinite(positionY) || !Float.isFinite(positionZ) || !Float.isFinite(yawDegrees)) {
            throw new IllegalArgumentException("position/yaw values must be finite");
        }
        if (createdAtUnixMs <= 0 || updatedAtUnixMs <= 0) {
            throw new IllegalArgumentException("timestamps must be positive");
        }
        if (updatedAtUnixMs < createdAtUnixMs) {
            throw new IllegalArgumentException("updatedAtUnixMs must be >= createdAtUnixMs");
        }
    }

    public CharacterProfile withPosition(float x, float y, float z, float yawDegrees, long updatedAtUnixMs) {
        return new CharacterProfile(characterId, accountId, name, classId, entityId, x, y, z, yawDegrees, createdAtUnixMs, updatedAtUnixMs);
    }

    private static void requireIdentifier(String value, String label) {
        Objects.requireNonNull(value, label);
        if (!value.matches("[a-z0-9][a-z0-9._-]{2,96}")) {
            throw new IllegalArgumentException(label + " must be a stable lowercase identifier");
        }
    }
}
