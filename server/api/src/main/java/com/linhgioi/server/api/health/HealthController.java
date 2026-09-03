package com.linhgioi.server.api.health;

import com.linhgioi.server.shared.runtime.RuntimeInfo;
import java.time.Instant;
import java.util.LinkedHashMap;
import java.util.Map;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class HealthController {
    private final RuntimeInfo runtimeInfo;

    public HealthController() {
        this(RuntimeInfo.current("api"));
    }

    HealthController(RuntimeInfo runtimeInfo) {
        this.runtimeInfo = runtimeInfo;
    }

    @GetMapping(path = "/health", produces = MediaType.APPLICATION_JSON_VALUE)
    public Map<String, Object> health() {
        Map<String, Object> payload = new LinkedHashMap<>();
        payload.put("status", "UP");
        payload.put("service", runtimeInfo.service());
        payload.put("serverTime", Instant.now().toString());
        payload.put("javaVersion", runtimeInfo.javaVersion());
        return payload;
    }
}
