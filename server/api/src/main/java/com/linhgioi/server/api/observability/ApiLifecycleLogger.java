package com.linhgioi.server.api.observability;

import com.linhgioi.server.shared.runtime.RuntimeInfo;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.context.event.ApplicationReadyEvent;
import org.springframework.context.event.ContextClosedEvent;
import org.springframework.context.event.EventListener;
import org.springframework.stereotype.Component;

@Component
public final class ApiLifecycleLogger {
    private static final Logger LOG = LoggerFactory.getLogger(ApiLifecycleLogger.class);
    private final RuntimeInfo runtimeInfo = RuntimeInfo.current("api");

    @EventListener(ApplicationReadyEvent.class)
    public void onReady() {
        LOG.atInfo()
                .addKeyValue("event", "api_started")
                .addKeyValue("service", runtimeInfo.service())
                .addKeyValue("pid", runtimeInfo.processId())
                .addKeyValue("java_version", runtimeInfo.javaVersion())
                .log("API runtime ready");
    }

    @EventListener(ContextClosedEvent.class)
    public void onStopping() {
        LOG.atInfo()
                .addKeyValue("event", "api_stopping")
                .addKeyValue("service", runtimeInfo.service())
                .addKeyValue("pid", runtimeInfo.processId())
                .log("API runtime stopping");
    }
}
