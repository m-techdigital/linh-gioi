package com.linhgioi.server.shared.runtime;

import java.util.Objects;

public record RuntimeInfo(String service, String javaVersion, long processId) {
    public RuntimeInfo {
        Objects.requireNonNull(service, "service");
        Objects.requireNonNull(javaVersion, "javaVersion");
        if (service.isBlank()) {
            throw new IllegalArgumentException("service must not be blank");
        }
        if (processId <= 0) {
            throw new IllegalArgumentException("processId must be positive");
        }
    }

    public static RuntimeInfo current(String service) {
        return new RuntimeInfo(service, System.getProperty("java.version"), ProcessHandle.current().pid());
    }
}
