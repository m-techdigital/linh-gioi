package com.linhgioi.server.realtime.session;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.protocol.v1.ClientHello;
import com.linhgioi.protocol.v1.PlayerTransformSnapshot;
import com.linhgioi.protocol.v1.ServerHello;
import com.linhgioi.server.realtime.protocol.HandshakeHandler;
import com.linhgioi.server.realtime.protocol.HandshakePolicy;
import com.linhgioi.server.realtime.protocol.HandshakeProtocol;
import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.embedded.EmbeddedChannel;
import io.netty.handler.codec.LengthFieldBasedFrameDecoder;
import io.netty.handler.codec.LengthFieldPrepender;
import org.junit.jupiter.api.Test;

class OnlineSessionHandlerTest {
    @Test
    void acceptedHandshakeTransitionsToOnlineMoveSession() throws Exception {
        EmbeddedChannel channel = channel();
        writeFrame(channel, validHello().toByteArray());
        assertTrue(readServerHello(channel).getAccepted());
        channel.runPendingTasks();
        assertNotNull(channel.pipeline().get(OnlineSessionHandler.class), "accepted handshake must install online session handler");

        writeFrame(channel, OnlineSessionTest.move(1, 1.0f, 0.0f, 0.1f).toByteArray());
        PlayerTransformSnapshot snapshot = PlayerTransformSnapshot.parseFrom(readFrame(channel));

        assertEquals(OnlineSession.DEFAULT_PLAYER_ENTITY_ID, snapshot.getEntityId());
        assertEquals(1, snapshot.getAcknowledgedSequence());
        assertEquals(0.4f, snapshot.getPosition().getX(), 0.0001f);
        channel.finishAndReleaseAll();
    }

    @Test
    void invalidPostHandshakeMoveClosesOnlyThatChannel() throws Exception {
        EmbeddedChannel channel = channel();
        writeFrame(channel, validHello().toByteArray());
        assertTrue(readServerHello(channel).getAccepted());
        channel.runPendingTasks();

        writeFrame(channel, OnlineSessionTest.move(0, 1.0f, 0.0f, 0.1f).toByteArray());
        channel.runPendingTasks();

        assertNull(channel.readOutbound(), "invalid move must not receive a state snapshot");
        assertTrue(!channel.isActive() || !channel.isOpen(), "invalid post-handshake move should close the channel");
        channel.finishAndReleaseAll();
    }

    private static ClientHello validHello() {
        return ClientHello.newBuilder()
                .setProtocolVersion(HandshakeProtocol.PROTOCOL_VERSION)
                .setClientVersion("m2-session-test")
                .setGamedataVersion(HandshakeProtocol.GAMEDATA_VERSION)
                .setPlatform("junit")
                .setLocale("vi-VN")
                .build();
    }

    private static EmbeddedChannel channel() {
        return new EmbeddedChannel(
                new LengthFieldBasedFrameDecoder(HandshakeProtocol.MAX_FRAME_BYTES, 0, 4, 0, 4),
                new LengthFieldPrepender(4),
                new HandshakeHandler(new HandshakePolicy()));
    }

    private static void writeFrame(EmbeddedChannel channel, byte[] payload) {
        channel.writeInbound(Unpooled.buffer(4 + payload.length).writeInt(payload.length).writeBytes(payload));
    }

    private static ServerHello readServerHello(EmbeddedChannel channel) throws Exception {
        return ServerHello.parseFrom(readFrame(channel));
    }

    private static byte[] readFrame(EmbeddedChannel channel) {
        ByteBuf outbound = channel.readOutbound();
        assertNotNull(outbound, "outbound frame must be emitted");

        int length;
        byte[] responseBytes;
        int offset = 0;
        try {
            length = outbound.readInt();
            responseBytes = new byte[length];
            int readable = Math.min(outbound.readableBytes(), length);
            outbound.readBytes(responseBytes, 0, readable);
            offset = readable;
        } finally {
            outbound.release();
        }

        while (offset < length) {
            ByteBuf chunk = channel.readOutbound();
            assertNotNull(chunk, "outbound payload chunk must be emitted");
            try {
                int readable = Math.min(chunk.readableBytes(), length - offset);
                chunk.readBytes(responseBytes, offset, readable);
                offset += readable;
            } finally {
                chunk.release();
            }
        }
        assertEquals(length, offset, "outbound payload length must match frame prefix");
        return responseBytes;
    }
}

