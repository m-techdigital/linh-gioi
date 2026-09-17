package com.linhgioi.server.api.auth;

import java.util.Objects;

public record ProductCredential(
        String accountId,
        String normalizedIdentifier,
        String passwordHash,
        long createdAtUnixMs,
        long updatedAtUnixMs) {
    public ProductCredential {
        requireText(accountId, "accountId");
        requireText(normalizedIdentifier, "normalizedIdentifier");
        requireText(passwordHash, "passwordHash");
        if (normalizedIdentifier.length() < 3 || normalizedIdentifier.length() > 254) {
            throw new IllegalArgumentException("normalizedIdentifier must be between 3 and 254 characters");
        }
        if (createdAtUnixMs <= 0 || updatedAtUnixMs < createdAtUnixMs) {
            throw new IllegalArgumentException("credential timestamps are invalid");
        }
    }

    private static void requireText(String value, String label) {
        Objects.requireNonNull(value, label);
        if (value.isBlank()) throw new IllegalArgumentException(label + " must not be blank");
    }
}
