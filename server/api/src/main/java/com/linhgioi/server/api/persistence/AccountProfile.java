package com.linhgioi.server.api.persistence;

import java.util.Objects;

public record AccountProfile(String accountId, String devKeyHash, String displayName, long createdAtUnixMs, long updatedAtUnixMs) {
    public AccountProfile {
        requireIdentifier(accountId, "accountId");
        requireIdentifier(devKeyHash, "devKeyHash");
        if (displayName == null || displayName.isBlank()) {
            throw new IllegalArgumentException("displayName must not be blank");
        }
        if (displayName.length() > 32) {
            throw new IllegalArgumentException("displayName must be <= 32 characters");
        }
        if (createdAtUnixMs <= 0 || updatedAtUnixMs <= 0) {
            throw new IllegalArgumentException("timestamps must be positive");
        }
        if (updatedAtUnixMs < createdAtUnixMs) {
            throw new IllegalArgumentException("updatedAtUnixMs must be >= createdAtUnixMs");
        }
    }

    private static void requireIdentifier(String value, String label) {
        Objects.requireNonNull(value, label);
        if (!value.matches("[a-z0-9][a-z0-9._-]{2,96}")) {
            throw new IllegalArgumentException(label + " must be a stable lowercase identifier");
        }
    }
}
