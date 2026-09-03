package com.linhgioi.server.realtime.config;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

import com.linhgioi.server.shared.config.EnvironmentReader;
import java.util.Map;
import org.junit.jupiter.api.Test;

class RealtimeServerConfigTest {
    @Test
    void readsExplicitRuntimeConfiguration() {
        RealtimeServerConfig config = RealtimeServerConfig.fromEnvironment(new EnvironmentReader(Map.of(
                "LG_REALTIME_HOST", "127.0.0.1",
                "LG_REALTIME_PORT", "9001",
                "LG_REALTIME_WORKER_THREADS", "3",
                "LG_REALTIME_SHUTDOWN_QUIET_MS", "25",
                "LG_REALTIME_SHUTDOWN_TIMEOUT_MS", "250")));

        assertEquals("127.0.0.1", config.host());
        assertEquals(9001, config.port());
        assertEquals(3, config.workerThreads());
        assertEquals(25, config.gracefulQuietPeriodMillis());
        assertEquals(250, config.gracefulTimeoutMillis());
    }

    @Test
    void rejectsInvalidPortFromEnvironment() {
        assertThrows(
                IllegalArgumentException.class,
                () -> RealtimeServerConfig.fromEnvironment(
                        new EnvironmentReader(Map.of("LG_REALTIME_PORT", "70000"))));
    }

    @Test
    void rejectsMalformedPortFromEnvironment() {
        assertThrows(
                IllegalArgumentException.class,
                () -> RealtimeServerConfig.fromEnvironment(
                        new EnvironmentReader(Map.of("LG_REALTIME_PORT", "not-a-number"))));
    }

    @Test
    void rejectsMalformedWorkerThreadCountFromEnvironment() {
        assertThrows(
                IllegalArgumentException.class,
                () -> RealtimeServerConfig.fromEnvironment(
                        new EnvironmentReader(Map.of("LG_REALTIME_WORKER_THREADS", "many"))));
    }

    @Test
    void rejectsMalformedShutdownTimeoutFromEnvironment() {
        assertThrows(
                IllegalArgumentException.class,
                () -> RealtimeServerConfig.fromEnvironment(
                        new EnvironmentReader(Map.of("LG_REALTIME_SHUTDOWN_TIMEOUT_MS", "later"))));
    }

    @Test
    void rejectsShutdownTimeoutShorterThanQuietPeriod() {
        assertThrows(
                IllegalArgumentException.class,
                () -> new RealtimeServerConfig("127.0.0.1", 7777, 2, 500, 100));
    }
}
