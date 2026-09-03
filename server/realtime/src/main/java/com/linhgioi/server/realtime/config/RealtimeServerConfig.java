package com.linhgioi.server.realtime.config;

import com.linhgioi.server.shared.config.EnvironmentReader;

public record RealtimeServerConfig(
        String host,
        int port,
        int workerThreads,
        long gracefulQuietPeriodMillis,
        long gracefulTimeoutMillis) {

    public static final int DEFAULT_PORT = 7777;

    public RealtimeServerConfig {
        if (host == null || host.isBlank()) {
            throw new IllegalArgumentException("host must not be blank");
        }
        if (port < 0 || port > 65535) {
            throw new IllegalArgumentException("port must be between 0 and 65535");
        }
        if (workerThreads < 1 || workerThreads > 256) {
            throw new IllegalArgumentException("workerThreads must be between 1 and 256");
        }
        if (gracefulQuietPeriodMillis < 0) {
            throw new IllegalArgumentException("gracefulQuietPeriodMillis must be non-negative");
        }
        if (gracefulTimeoutMillis < gracefulQuietPeriodMillis) {
            throw new IllegalArgumentException("gracefulTimeoutMillis must be >= gracefulQuietPeriodMillis");
        }
    }

    public static RealtimeServerConfig fromEnvironment(EnvironmentReader environment) {
        String host = environment.stringValue("LG_REALTIME_HOST", "0.0.0.0");
        int port = environment.intValue("LG_REALTIME_PORT", DEFAULT_PORT, 1, 65535);
        int workerThreads = environment.intValue(
                "LG_REALTIME_WORKER_THREADS",
                Math.min(8, Math.max(2, Runtime.getRuntime().availableProcessors())),
                1,
                256);
        long quietPeriodMillis = environment.longValue("LG_REALTIME_SHUTDOWN_QUIET_MS", 100, 0, 60_000);
        long timeoutMillis = environment.longValue("LG_REALTIME_SHUTDOWN_TIMEOUT_MS", 5_000, 1, 120_000);
        return new RealtimeServerConfig(host, port, workerThreads, quietPeriodMillis, timeoutMillis);
    }
}
