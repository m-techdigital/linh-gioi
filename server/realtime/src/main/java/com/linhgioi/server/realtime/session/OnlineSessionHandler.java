package com.linhgioi.server.realtime.session;

import com.google.protobuf.InvalidProtocolBufferException;
import com.linhgioi.protocol.v1.MoveIntent;
import com.linhgioi.protocol.v1.PlayerTransformSnapshot;
import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.SimpleChannelInboundHandler;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public final class OnlineSessionHandler extends SimpleChannelInboundHandler<ByteBuf> {
    private static final Logger LOG = LoggerFactory.getLogger(OnlineSessionHandler.class);

    private final OnlineSession session;

    public OnlineSessionHandler(OnlineSession session) {
        this.session = session;
    }

    @Override
    public void channelActive(ChannelHandlerContext context) throws Exception {
        LOG.atInfo()
                .addKeyValue("event", "realtime_session_channel_active")
                .addKeyValue("session_id", session.sessionId())
                .log("Realtime online session channel active");
        super.channelActive(context);
    }

    @Override
    protected void channelRead0(ChannelHandlerContext context, ByteBuf frame) {
        final byte[] payload = new byte[frame.readableBytes()];
        frame.readBytes(payload);

        final MoveIntent intent;
        try {
            intent = MoveIntent.parseFrom(payload);
        } catch (InvalidProtocolBufferException exception) {
            LOG.atWarn()
                    .addKeyValue("event", "realtime_session_malformed_move_intent")
                    .addKeyValue("session_id", session.sessionId())
                    .addKeyValue("payload_bytes", payload.length)
                    .log("Malformed post-handshake MoveIntent rejected");
            context.close();
            return;
        }

        final PlayerTransformSnapshot snapshot;
        try {
            snapshot = session.applyMove(intent, System.currentTimeMillis());
        } catch (IllegalArgumentException exception) {
            LOG.atWarn()
                    .addKeyValue("event", "realtime_session_move_rejected")
                    .addKeyValue("session_id", session.sessionId())
                    .addKeyValue("sequence", intent.getSequence())
                    .addKeyValue("reason", exception.getMessage())
                    .log("Invalid post-handshake MoveIntent rejected");
            context.close();
            return;
        }

        context.writeAndFlush(Unpooled.wrappedBuffer(snapshot.toByteArray()));
        LOG.atInfo()
                .addKeyValue("event", "realtime_session_move_applied")
                .addKeyValue("session_id", session.sessionId())
                .addKeyValue("sequence", intent.getSequence())
                .addKeyValue("acknowledged_sequence", snapshot.getAcknowledgedSequence())
                .addKeyValue("entity_id", snapshot.getEntityId())
                .log("Realtime online session applied movement intent");
    }

    @Override
    public void channelInactive(ChannelHandlerContext context) throws Exception {
        LOG.atInfo()
                .addKeyValue("event", "realtime_session_closed")
                .addKeyValue("session_id", session.sessionId())
                .addKeyValue("acknowledged_sequence", session.acknowledgedSequence())
                .log("Realtime online session closed");
        super.channelInactive(context);
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext context, Throwable cause) {
        LOG.atWarn()
                .setCause(cause)
                .addKeyValue("event", "realtime_session_error")
                .addKeyValue("session_id", session.sessionId())
                .log("Realtime online session failed; closing channel");
        context.close();
    }
}
