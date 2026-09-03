package com.linhgioi.server.shared.config;

import java.util.Map;
import java.util.Objects;

/**
 * Small, dependency-free environment adapter so runtime configuration can be validated in unit tests
 * without mutating process-global environment variables.
 */
public final class EnvironmentReader {
    private final Map<String, String> values;

    public EnvironmentReader(Map<String, String> values) {
        this.values = Map.copyOf(Objects.requireNonNull(values, "values"));
    }

    public static EnvironmentReader system() {
        return new EnvironmentReader(System.getenv());
    }

    public String stringValue(String key, String defaultValue) {
        String value = values.get(key);
        if (value == null || value.isBlank()) {
            return defaultValue;
        }
        return value.trim();
    }

    public int intValue(String key, int defaultValue, int minInclusive, int maxInclusive) {
        String raw = values.get(key);
        if (raw == null || raw.isBlank()) {
            return defaultValue;
        }

        final int parsed;
        try {
            parsed = Integer.parseInt(raw.trim());
        } catch (NumberFormatException exception) {
            throw new IllegalArgumentException(key + " must be an integer", exception);
        }

        if (parsed < minInclusive || parsed > maxInclusive) {
            throw new IllegalArgumentException(
                    key + " must be between " + minInclusive + " and " + maxInclusive + ", got " + parsed);
        }
        return parsed;
    }

    public long longValue(String key, long defaultValue, long minInclusive, long maxInclusive) {
        String raw = values.get(key);
        if (raw == null || raw.isBlank()) {
            return defaultValue;
        }

        final long parsed;
        try {
            parsed = Long.parseLong(raw.trim());
        } catch (NumberFormatException exception) {
            throw new IllegalArgumentException(key + " must be a long integer", exception);
        }

        if (parsed < minInclusive || parsed > maxInclusive) {
            throw new IllegalArgumentException(
                    key + " must be between " + minInclusive + " and " + maxInclusive + ", got " + parsed);
        }
        return parsed;
    }
}
