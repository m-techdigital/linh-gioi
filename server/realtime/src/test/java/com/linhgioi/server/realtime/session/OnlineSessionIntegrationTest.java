package com.linhgioi.server.realtime.session;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.protocol.v1.ClientHello;
import com.linhgioi.protocol.v1.MoveIntent;
import com.linhgioi.protocol.v1.PlayerTransformSnapshot;
import com.linhgioi.protocol.v1.ServerHello;
import com.linhgioi.protocol.v1.Vec2;
import com.linhgioi.server.realtime.config.RealtimeServerConfig;
import com.linhgioi.server.realtime.protocol.HandshakeProtocol;
import com.linhgioi.server.realtime.transport.RealtimeServer;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import org.junit.jupiter.api.Test;

class OnlineSessionIntegrationTest {
    @Test
    void acceptedClientCanSendMoveIntentAndReceiveAuthoritativeSnapshot() throws Exception {
        RealtimeServerConfig config = new RealtimeServerConfig("127.0.0.1", 0, 1, 0, 1_000);
        try (RealtimeServer server = new RealtimeServer(config)) {
            InetSocketAddress address = server.start();
            try (RealtimeExchange exchange = RealtimeExchange.open(address)) {
                ServerHello hello = ServerHello.parseFrom(exchange.exchange(validHello().toByteArray()));
                assertTrue(hello.getAccepted());

                PlayerTransformSnapshot first = PlayerTransformSnapshot.parseFrom(exchange.exchange(move(1, 1.0f, 0.0f, 0.1f).toByteArray()));
                assertEquals(1, first.getAcknowledgedSequence());
                assertEquals(0.4f, first.getPosition().getX(), 0.0001f);

                PlayerTransformSnapshot duplicate = PlayerTransformSnapshot.parseFrom(exchange.exchange(move(1, 1.0f, 0.0f, 0.1f).toByteArray()));
                assertEquals(1, duplicate.getAcknowledgedSequence());
                assertEquals(0.4f, duplicate.getPosition().getX(), 0.0001f);
            }
        }
    }

    @Test
    void invalidPostHandshakeMoveClosesClientButServerSurvivesReconnect() throws Exception {
        RealtimeServerConfig config = new RealtimeServerConfig("127.0.0.1", 0, 1, 0, 1_000);
        try (RealtimeServer server = new RealtimeServer(config)) {
            InetSocketAddress address = server.start();
            try (RealtimeExchange exchange = RealtimeExchange.open(address)) {
                assertTrue(ServerHello.parseFrom(exchange.exchange(validHello().toByteArray())).getAccepted());
                exchange.sendOnly(move(0, 1.0f, 0.0f, 0.1f).toByteArray());
                assertFalse(exchange.canReadFrame(), "invalid movement should close only the offending session");
            }

            assertTrue(server.isRunning(), "server must survive invalid post-handshake client payload");
            try (RealtimeExchange exchange = RealtimeExchange.open(address)) {
                assertTrue(ServerHello.parseFrom(exchange.exchange(validHello().toByteArray())).getAccepted());
                PlayerTransformSnapshot snapshot = PlayerTransformSnapshot.parseFrom(exchange.exchange(move(1, 0.0f, 1.0f, 0.1f).toByteArray()));
                assertEquals(1, snapshot.getAcknowledgedSequence());
                assertEquals(0.4f, snapshot.getPosition().getZ(), 0.0001f);
            }
        }
    }

    private static ClientHello validHello() {
        return ClientHello.newBuilder()
                .setProtocolVersion(HandshakeProtocol.PROTOCOL_VERSION)
                .setClientVersion("m2-online-session-test")
                .setGamedataVersion(HandshakeProtocol.GAMEDATA_VERSION)
                .setPlatform("junit")
                .setLocale("vi-VN")
                .build();
    }

    private static MoveIntent move(int sequence, float x, float y, float deltaSeconds) {
        return MoveIntent.newBuilder()
                .setSequence(sequence)
                .setMoveAxis(Vec2.newBuilder().setX(x).setY(y).build())
                .setClientDeltaSeconds(deltaSeconds)
                .build();
    }

    private static final class RealtimeExchange implements AutoCloseable {
        private final Socket socket;
        private final DataOutputStream output;
        private final DataInputStream input;

        private RealtimeExchange(Socket socket) throws Exception {
            this.socket = socket;
            socket.setSoTimeout(2_000);
            output = new DataOutputStream(socket.getOutputStream());
            input = new DataInputStream(socket.getInputStream());
        }

        static RealtimeExchange open(InetSocketAddress address) throws Exception {
            return new RealtimeExchange(new Socket(address.getAddress(), address.getPort()));
        }

        byte[] exchange(byte[] payload) throws Exception {
            sendOnly(payload);
            return readFrame();
        }

        void sendOnly(byte[] payload) throws Exception {
            output.writeInt(payload.length);
            output.write(payload);
            output.flush();
        }

        boolean canReadFrame() throws Exception {
            try {
                int marker = input.read();
                return marker >= 0;
            } catch (java.net.SocketTimeoutException exception) {
                return true;
            }
        }

        byte[] readFrame() throws Exception {
            int length = input.readInt();
            if (length < 1 || length > HandshakeProtocol.MAX_FRAME_BYTES) {
                throw new IllegalStateException("Unexpected frame length: " + length);
            }
            return input.readNBytes(length);
        }

        @Override
        public void close() throws Exception {
            socket.close();
        }
    }
}
