package com.linhgioi.server.shared.runtime;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

import org.junit.jupiter.api.Test;

class RuntimeInfoTest {
    @Test
    void capturesCurrentRuntimeIdentity() {
        RuntimeInfo info = RuntimeInfo.current("test-service");

        assertEquals("test-service", info.service());
        assertFalse(info.javaVersion().isBlank());
        assertTrue(info.processId() > 0);
    }
}
