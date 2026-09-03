package com.linhgioi.server.realtime.protocol;

import com.google.protobuf.InvalidProtocolBufferException;
import com.linhgioi.protocol.v1.ClientHello;
import com.linhgioi.protocol.v1.ErrorInfo;
import com.linhgioi.protocol.v1.ServerHello;
import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.SimpleChannelInboundHandler;
import java.util.UUID;
import com.linhgioi.server.realtime.session.OnlineSession;
import com.linhgioi.server.realtime.session.OnlineSessionHandler;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public final class HandshakeHandler extends SimpleChannelInboundHandler<ByteBuf> {
    private static final Logger LOG = LoggerFactory.getLogger(HandshakeHandler.class);

    private final String connectionId = UUID.randomUUID().toString();
    private final HandshakePolicy policy;
    private boolean helloProcessed;

    public HandshakeHandler(HandshakePolicy policy) {
        this.policy = policy;
    }

    @Override
    public void channelActive(ChannelHandlerContext context) throws Exception {
        LOG.atInfo()
                .addKeyValue("event", "realtime_connection_opened")
                .addKeyValue("connection_id", connectionId)
                .addKeyValue("remote_address", context.channel().remoteAddress())
                .log("Realtime connection opened");
        super.channelActive(context);
    }

    @Override
    protected void channelRead0(ChannelHandlerContext context, ByteBuf frame) {
        if (helloProcessed) {
            sendDecision(
                    context,
                    HandshakeDecision.reject("DUPLICATE_CLIENT_HELLO", "ClientHello may only be sent once.", false),
                    true);
            return;
        }

        final byte[] payload = new byte[frame.readableBytes()];
        frame.readBytes(payload);

        final ClientHello hello;
        try {
            hello = ClientHello.parseFrom(payload);
        } catch (InvalidProtocolBufferException exception) {
            LOG.atWarn()
                    .addKeyValue("event", "realtime_malformed_client_hello")
                    .addKeyValue("connection_id", connectionId)
                    .addKeyValue("payload_bytes", payload.length)
                    .log("Malformed ClientHello rejected");
            context.close();
            return;
        }

        helloProcessed = true;
        HandshakeDecision decision = policy.evaluate(hello);
        ChannelFuture writeFuture = sendDecision(context, decision, !decision.accepted());
        if (decision.accepted()) {
            writeFuture.addListener(ignored -> {
                if (!context.channel().isActive()) {
                    return;
                }
                OnlineSession session = new OnlineSession(connectionId);
                context.pipeline().replace(this, "onlineSession", new OnlineSessionHandler(session));
                LOG.atInfo()
                        .addKeyValue("event", "realtime_session_opened")
                        .addKeyValue("connection_id", connectionId)
                        .addKeyValue("session_id", session.sessionId())
                        .log("Realtime online session opened after accepted handshake");
            });
        }
    }

    private ChannelFuture sendDecision(ChannelHandlerContext context, HandshakeDecision decision, boolean closeAfterWrite) {
        ServerHello.Builder response = ServerHello.newBuilder()
                .setAccepted(decision.accepted())
                .setProtocolVersion(HandshakeProtocol.PROTOCOL_VERSION)
                .setRequiredGamedataVersion(HandshakeProtocol.GAMEDATA_VERSION)
                .setServerTimeUnixMs(System.currentTimeMillis());

        if (!decision.accepted()) {
            response.setError(ErrorInfo.newBuilder()
                    .setCode(decision.errorCode())
                    .setMessage(decision.errorMessage())
                    .setRetryable(decision.retryable())
                    .build());
        }

        ByteBuf output = Unpooled.wrappedBuffer(response.build().toByteArray());
        var future = context.writeAndFlush(output);
        LOG.atInfo()
                .addKeyValue("event", decision.accepted() ? "realtime_handshake_accepted" : "realtime_handshake_rejected")
                .addKeyValue("connection_id", connectionId)
                .addKeyValue("error_code", decision.errorCode())
                .log("Realtime handshake evaluated");
        if (closeAfterWrite) {
            future.addListener(ignored -> context.close());
        }
        return future;
    }

    @Override
    public void channelInactive(ChannelHandlerContext context) throws Exception {
        LOG.atInfo()
                .addKeyValue("event", "realtime_connection_closed")
                .addKeyValue("connection_id", connectionId)
                .log("Realtime connection closed");
        super.channelInactive(context);
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext context, Throwable cause) {
        LOG.atWarn()
                .setCause(cause)
                .addKeyValue("event", "realtime_connection_error")
                .addKeyValue("connection_id", connectionId)
                .log("Realtime connection failed; closing channel");
        context.close();
    }
}
