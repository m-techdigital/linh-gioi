package com.linhgioi.server.realtime.protocol;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.protocol.v1.ServerHello;
import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.embedded.EmbeddedChannel;
import io.netty.handler.codec.LengthFieldBasedFrameDecoder;
import io.netty.handler.codec.LengthFieldPrepender;
import org.junit.jupiter.api.Test;

class HandshakeHandlerTest {
    @Test
    void validHelloProducesAcceptedServerHello() throws Exception {
        EmbeddedChannel channel = channel();
        byte[] payload = HandshakePolicyTest.validHello().toByteArray();
        ByteBuf input = Unpooled.buffer(4 + payload.length).writeInt(payload.length).writeBytes(payload);
        channel.writeInbound(input);
        assertTrue(readServerHello(channel).getAccepted());
        channel.finishAndReleaseAll();
    }

    @Test
    void incompatibleHelloProducesRejectedServerHello() throws Exception {
        EmbeddedChannel channel = channel();
        byte[] payload = HandshakePolicyTest.validHello().toBuilder().setProtocolVersion(999).build().toByteArray();
        ByteBuf input = Unpooled.buffer(4 + payload.length).writeInt(payload.length).writeBytes(payload);
        channel.writeInbound(input);
        assertFalse(readServerHello(channel).getAccepted());
        channel.finishAndReleaseAll();
    }

    private ServerHello readServerHello(EmbeddedChannel channel) throws Exception {
        ByteBuf outbound = channel.readOutbound();
        assertNotNull(outbound, "server hello frame must be emitted");

        int length;
        byte[] responseBytes;
        int offset = 0;
        try {
            length = outbound.readInt();
            responseBytes = new byte[length];
            int readable = outbound.readableBytes();
            assertTrue(readable <= length, "server hello payload emitted more bytes than declared");
            outbound.readBytes(responseBytes, 0, readable);
            offset = readable;
        } finally {
            outbound.release();
        }

        while (offset < length) {
            ByteBuf payloadChunk = channel.readOutbound();
            assertNotNull(payloadChunk, "server hello payload must be emitted");
            try {
                int readable = payloadChunk.readableBytes();
                int remaining = length - offset;
                assertTrue(readable <= remaining, "server hello payload emitted more bytes than declared");
                payloadChunk.readBytes(responseBytes, offset, readable);
                offset += readable;
            } finally {
                payloadChunk.release();
            }
        }

        assertEquals(length, offset, "server hello payload length must match length prefix");
        return ServerHello.parseFrom(responseBytes);
    }

    private EmbeddedChannel channel() {
        return new EmbeddedChannel(
                new LengthFieldBasedFrameDecoder(HandshakeProtocol.MAX_FRAME_BYTES, 0, 4, 0, 4),
                new LengthFieldPrepender(4),
                new HandshakeHandler(new HandshakePolicy()));
    }
}
