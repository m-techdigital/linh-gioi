package com.linhgioi.server.realtime.bootstrap;

import com.linhgioi.server.realtime.config.RealtimeServerConfig;
import com.linhgioi.server.realtime.transport.RealtimeServer;
import com.linhgioi.server.shared.config.EnvironmentReader;
import com.linhgioi.server.shared.runtime.RuntimeInfo;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public final class RealtimeApplication {
    private static final Logger LOG = LoggerFactory.getLogger(RealtimeApplication.class);

    private RealtimeApplication() {}

    public static void main(String[] args) throws Exception {
        RuntimeInfo runtimeInfo = RuntimeInfo.current("realtime");
        RealtimeServerConfig config = RealtimeServerConfig.fromEnvironment(EnvironmentReader.system());
        RealtimeServer server = new RealtimeServer(config);

        Runtime.getRuntime().addShutdownHook(new Thread(server::stop, "realtime-shutdown"));

        LOG.atInfo()
                .addKeyValue("event", "realtime_bootstrap")
                .addKeyValue("service", runtimeInfo.service())
                .addKeyValue("pid", runtimeInfo.processId())
                .addKeyValue("java_version", runtimeInfo.javaVersion())
                .log("Realtime runtime bootstrapping");

        server.start();
        server.awaitShutdown();
    }
}
