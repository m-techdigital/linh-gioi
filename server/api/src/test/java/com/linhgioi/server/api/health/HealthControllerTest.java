package com.linhgioi.server.api.health;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

import com.linhgioi.server.shared.runtime.RuntimeInfo;
import java.util.Map;
import org.junit.jupiter.api.Test;

class HealthControllerTest {
    @Test
    void healthReportsApiRuntimeAsUp() {
        HealthController controller = new HealthController(new RuntimeInfo("api", "25-test", 1234));

        Map<String, Object> health = controller.health();

        assertEquals("UP", health.get("status"));
        assertEquals("api", health.get("service"));
        assertEquals("25-test", health.get("javaVersion"));
        assertNotNull(health.get("serverTime"));
    }
}
