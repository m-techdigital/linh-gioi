package com.linhgioi.server.realtime.combat;

import com.google.protobuf.Message;
import com.linhgioi.protocol.v1.CombatAccepted;
import com.linhgioi.protocol.v1.CombatIntent;
import com.linhgioi.protocol.v1.CombatRejected;
import com.linhgioi.server.realtime.session.OnlineSession;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.time.Instant;

public final class CombatSmokeServer {
    private static final byte RESPONSE_ACCEPTED = 1;
    private static final byte RESPONSE_REJECTED = 2;
    private static final byte RESPONSE_MALFORMED = 3;

    private CombatSmokeServer() {
    }

    public static void main(String[] args) throws Exception {
        int port = parsePort(args);
        CombatValidationService service = new CombatValidationService(() -> Instant.now().toEpochMilli());
        try (ServerSocket server = new ServerSocket(port)) {
            System.out.println("M6_COMBAT_SMOKE_SERVER_READY port=" + server.getLocalPort());
            while (true) {
                try (Socket socket = server.accept()) {
                    handle(socket, service);
                }
            }
        }
    }

    private static void handle(Socket socket, CombatValidationService service) throws IOException {
        DataInputStream input = new DataInputStream(socket.getInputStream());
        DataOutputStream output = new DataOutputStream(socket.getOutputStream());
        int length = input.readInt();
        if (length <= 0 || length > 65536) {
            write(output, RESPONSE_MALFORMED, new byte[0]);
            return;
        }
        byte[] payload = input.readNBytes(length);
        try {
            CombatIntent intent = CombatIntent.parseFrom(payload);
            Message response = service.validate(intent, OnlineSession.DEFAULT_PLAYER_ENTITY_ID);
            if (response instanceof CombatAccepted accepted) {
                write(output, RESPONSE_ACCEPTED, accepted.toByteArray());
            } else if (response instanceof CombatRejected rejected) {
                write(output, RESPONSE_REJECTED, rejected.toByteArray());
            } else {
                write(output, RESPONSE_MALFORMED, new byte[0]);
            }
        } catch (Exception exception) {
            write(output, RESPONSE_MALFORMED, new byte[0]);
        }
    }

    private static void write(DataOutputStream output, byte kind, byte[] payload) throws IOException {
        output.writeByte(kind);
        output.writeInt(payload.length);
        output.write(payload);
        output.flush();
    }

    private static int parsePort(String[] args) {
        for (int i = 0; i < args.length - 1; i++) {
            if ("--port".equals(args[i])) {
                return Integer.parseInt(args[i + 1]);
            }
        }
        return 17843;
    }
}
