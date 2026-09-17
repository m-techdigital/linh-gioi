package com.linhgioi.server.api.persistence;

import java.util.Set;

public final class CharacterClassCompatibility {
    private static final Set<String> CANONICAL = Set.of("vo", "kiem", "phap", "co", "linh");

    private CharacterClassCompatibility() { }

    public static String normalizeStoredClassId(String classId) {
        if (classId == null || classId.isBlank()) {
            throw new IllegalArgumentException("classId must not be blank");
        }
        String normalized = classId.trim();
        toRuntimeClassIdInternal(normalized);
        return normalized;
    }

    public static String toRuntimeClassId(String classId) {
        return toRuntimeClassIdInternal(normalizeStoredClassId(classId));
    }

    private static String toRuntimeClassIdInternal(String classId) {
        if ("class.martial".equals(classId)) return "vo";
        if ("class.sword".equals(classId)) return "kiem";
        if (CANONICAL.contains(classId)) return classId;
        throw new IllegalArgumentException(
                "classId must be one of vo, kiem, phap, co, linh, class.martial or class.sword");
    }
}
