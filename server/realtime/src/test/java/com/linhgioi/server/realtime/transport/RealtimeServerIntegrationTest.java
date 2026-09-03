package com.linhgioi.server.realtime.transport;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.server.realtime.config.RealtimeServerConfig;
import java.net.InetSocketAddress;
import java.net.ServerSocket;
import java.net.Socket;
import org.junit.jupiter.api.Test;

class RealtimeServerIntegrationTest {
    @Test
    void bindsAcceptsTcpConnectionAndShutsDownGracefully() throws Exception {
        RealtimeServerConfig config = new RealtimeServerConfig("127.0.0.1", 0, 1, 0, 1_000);

        try (RealtimeServer server = new RealtimeServer(config)) {
            InetSocketAddress boundAddress = server.start();
            assertTrue(boundAddress.getPort() > 0, "ephemeral bind must resolve a real port");
            assertTrue(server.isRunning(), "server must report running after bind");

            try (Socket socket = new Socket()) {
                socket.connect(new InetSocketAddress("127.0.0.1", boundAddress.getPort()), 1_000);
                assertTrue(socket.isConnected(), "client TCP socket must connect to Netty listener");
            }

            server.stop();
            assertFalse(server.isRunning(), "server must stop accepting connections after graceful stop");
            assertTrue(server.eventLoopsTerminated(), "all event-loop resources must terminate after stop");
        }
    }

    @Test
    void failedBindReleasesEventLoops() throws Exception {
        try (ServerSocket blocker = new ServerSocket()) {
            blocker.bind(new InetSocketAddress("127.0.0.1", 0));
            RealtimeServerConfig config = new RealtimeServerConfig(
                    "127.0.0.1", blocker.getLocalPort(), 1, 0, 1_000);

            try (RealtimeServer server = new RealtimeServer(config)) {
                assertThrows(IllegalStateException.class, server::start);
                assertFalse(server.isRunning(), "failed bind must never report RUNNING");
                assertTrue(server.eventLoopsTerminated(), "failed bind must release all event loops");
            }
        }
    }

    @Test
    void repeatedStopIsIdempotentAndDoesNotLeakEventLoops() throws Exception {
        RealtimeServerConfig config = new RealtimeServerConfig("127.0.0.1", 0, 1, 0, 1_000);

        try (RealtimeServer server = new RealtimeServer(config)) {
            server.start();
            server.stop();
            server.stop();

            assertFalse(server.isRunning());
            assertTrue(server.eventLoopsTerminated(), "repeated stop must leave event loops terminated");
        }
    }
}
