package com.linhgioi.server.realtime.protocol;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.protocol.v1.ClientHello;
import com.linhgioi.protocol.v1.ServerHello;
import com.linhgioi.server.realtime.config.RealtimeServerConfig;
import com.linhgioi.server.realtime.transport.RealtimeServer;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import org.junit.jupiter.api.Test;

class RealtimeHandshakeIntegrationTest {
    @Test
    void validAndRejectedClientsDoNotKillServer() throws Exception {
        RealtimeServerConfig config = new RealtimeServerConfig("127.0.0.1", 0, 1, 0, 1_000);
        try (RealtimeServer server = new RealtimeServer(config)) {
            InetSocketAddress address = server.start();

            assertTrue(exchange(address, HandshakePolicyTest.validHello()).getAccepted());
            assertFalse(exchange(address,
                    HandshakePolicyTest.validHello().toBuilder().setProtocolVersion(999).build()).getAccepted());
            assertTrue(server.isRunning());

            sendMalformed(address);
            assertTrue(server.isRunning());
            assertTrue(exchange(address, HandshakePolicyTest.validHello()).getAccepted());
        }
    }

    private ServerHello exchange(InetSocketAddress address, ClientHello hello) throws Exception {
        try (Socket socket = new Socket(address.getAddress(), address.getPort());
             DataOutputStream output = new DataOutputStream(socket.getOutputStream());
             DataInputStream input = new DataInputStream(socket.getInputStream())) {
            byte[] payload = hello.toByteArray();
            output.writeInt(payload.length);
            output.write(payload);
            output.flush();
            int length = input.readInt();
            if (length < 1 || length > HandshakeProtocol.MAX_FRAME_BYTES) {
                throw new IllegalStateException("Unexpected ServerHello frame length: " + length);
            }
            return ServerHello.parseFrom(input.readNBytes(length));
        }
    }

    private void sendMalformed(InetSocketAddress address) throws Exception {
        try (Socket socket = new Socket(address.getAddress(), address.getPort());
             DataOutputStream output = new DataOutputStream(socket.getOutputStream())) {
            output.writeInt(1);
            output.writeByte(0xFF);
            output.flush();
        }
    }
}
