package com.linhgioi.server.shared.config;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.util.Map;
import org.junit.jupiter.api.Test;

class EnvironmentReaderTest {
    @Test
    void usesDefaultsForMissingValues() {
        EnvironmentReader reader = new EnvironmentReader(Map.of());

        assertEquals("0.0.0.0", reader.stringValue("HOST", "0.0.0.0"));
        assertEquals(7777, reader.intValue("PORT", 7777, 0, 65535));
    }

    @Test
    void trimsAndParsesConfiguredValues() {
        EnvironmentReader reader = new EnvironmentReader(Map.of("HOST", " 127.0.0.1 ", "PORT", " 9000 "));

        assertEquals("127.0.0.1", reader.stringValue("HOST", "0.0.0.0"));
        assertEquals(9000, reader.intValue("PORT", 7777, 0, 65535));
    }

    @Test
    void rejectsMalformedInteger() {
        EnvironmentReader reader = new EnvironmentReader(Map.of("PORT", "not-a-number"));

        assertThrows(IllegalArgumentException.class, () -> reader.intValue("PORT", 7777, 0, 65535));
    }

    @Test
    void rejectsOutOfRangeInteger() {
        EnvironmentReader reader = new EnvironmentReader(Map.of("PORT", "70000"));

        assertThrows(IllegalArgumentException.class, () -> reader.intValue("PORT", 7777, 0, 65535));
    }

    @Test
    void rejectsMalformedLong() {
        EnvironmentReader reader = new EnvironmentReader(Map.of("TIMEOUT", "not-a-long"));

        assertThrows(IllegalArgumentException.class, () -> reader.longValue("TIMEOUT", 5_000, 1, 120_000));
    }

    @Test
    void rejectsOutOfRangeLong() {
        EnvironmentReader reader = new EnvironmentReader(Map.of("TIMEOUT", "120001"));

        assertThrows(IllegalArgumentException.class, () -> reader.longValue("TIMEOUT", 5_000, 1, 120_000));
    }
}
