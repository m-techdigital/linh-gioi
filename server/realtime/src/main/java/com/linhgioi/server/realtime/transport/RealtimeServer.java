package com.linhgioi.server.realtime.transport;

import com.linhgioi.server.realtime.config.RealtimeServerConfig;
import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.Channel;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelOption;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.MultiThreadIoEventLoopGroup;
import io.netty.channel.nio.NioIoHandler;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import io.netty.handler.codec.LengthFieldBasedFrameDecoder;
import io.netty.handler.codec.LengthFieldPrepender;
import com.linhgioi.server.realtime.protocol.HandshakeHandler;
import com.linhgioi.server.realtime.protocol.HandshakePolicy;
import com.linhgioi.server.realtime.protocol.HandshakeProtocol;
import java.net.InetSocketAddress;
import java.util.Objects;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicReference;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public final class RealtimeServer implements AutoCloseable {
    private static final Logger LOG = LoggerFactory.getLogger(RealtimeServer.class);

    private enum State {
        NEW,
        STARTING,
        RUNNING,
        STOPPING,
        STOPPED
    }

    private final RealtimeServerConfig config;
    private final AtomicReference<State> state = new AtomicReference<>(State.NEW);
    private EventLoopGroup bossGroup;
    private EventLoopGroup workerGroup;
    private Channel serverChannel;

    public RealtimeServer(RealtimeServerConfig config) {
        this.config = Objects.requireNonNull(config, "config");
    }

    public synchronized InetSocketAddress start() throws InterruptedException {
        if (!state.compareAndSet(State.NEW, State.STARTING)) {
            throw new IllegalStateException("Realtime server can only be started once; state=" + state.get());
        }

        try {
            bossGroup = new MultiThreadIoEventLoopGroup(1, NioIoHandler.newFactory());
            workerGroup = new MultiThreadIoEventLoopGroup(config.workerThreads(), NioIoHandler.newFactory());

            ServerBootstrap bootstrap = new ServerBootstrap()
                    .group(bossGroup, workerGroup)
                    .channel(NioServerSocketChannel.class)
                    .childHandler(new ChannelInitializer<SocketChannel>() {
                        @Override
                        protected void initChannel(SocketChannel channel) {
                            channel.pipeline().addLast(
                                    new LengthFieldBasedFrameDecoder(HandshakeProtocol.MAX_FRAME_BYTES, 0, 4, 0, 4),
                                    new LengthFieldPrepender(4),
                                    new HandshakeHandler(new HandshakePolicy()));
                        }
                    })
                    .childOption(ChannelOption.TCP_NODELAY, true)
                    .childOption(ChannelOption.SO_KEEPALIVE, true);

            ChannelFuture bindFuture = bootstrap.bind(config.host(), config.port());
            bindFuture.await();
            if (!bindFuture.isSuccess()) {
                throw new IllegalStateException(
                        "Failed to bind realtime server to " + config.host() + ":" + config.port(),
                        bindFuture.cause());
            }

            serverChannel = bindFuture.channel();
            state.set(State.RUNNING);
            InetSocketAddress boundAddress = (InetSocketAddress) serverChannel.localAddress();
            LOG.atInfo()
                    .addKeyValue("event", "realtime_started")
                    .addKeyValue("host", boundAddress.getHostString())
                    .addKeyValue("port", boundAddress.getPort())
                    .addKeyValue("worker_threads", config.workerThreads())
                    .log("Realtime server bound");
            return boundAddress;
        } catch (InterruptedException exception) {
            Thread.currentThread().interrupt();
            stopAfterFailedStart();
            throw exception;
        } catch (RuntimeException exception) {
            stopAfterFailedStart();
            throw exception;
        }
    }

    public boolean isRunning() {
        return state.get() == State.RUNNING && serverChannel != null && serverChannel.isActive();
    }

    public void awaitShutdown() throws InterruptedException {
        Channel channel = serverChannel;
        if (channel == null) {
            throw new IllegalStateException("Realtime server has not been started");
        }
        channel.closeFuture().sync();
    }

    public synchronized void stop() {
        State current = state.get();
        if (current == State.STOPPED || current == State.NEW) {
            state.set(State.STOPPED);
            return;
        }
        if (current == State.STOPPING) {
            return;
        }

        state.set(State.STOPPING);
        LOG.atInfo().addKeyValue("event", "realtime_stopping").log("Realtime server stopping");

        if (serverChannel != null) {
            serverChannel.close().syncUninterruptibly();
        }
        shutdownGroup(workerGroup);
        shutdownGroup(bossGroup);
        state.set(State.STOPPED);
        LOG.atInfo().addKeyValue("event", "realtime_stopped").log("Realtime server stopped gracefully");
    }

    boolean eventLoopsTerminated() {
        return isTerminated(workerGroup) && isTerminated(bossGroup);
    }

    private void stopAfterFailedStart() {
        state.set(State.STOPPING);
        if (serverChannel != null) {
            serverChannel.close().syncUninterruptibly();
        }
        shutdownGroup(workerGroup);
        shutdownGroup(bossGroup);
        state.set(State.STOPPED);
    }

    private void shutdownGroup(EventLoopGroup group) {
        if (group == null) {
            return;
        }
        group.shutdownGracefully(
                        config.gracefulQuietPeriodMillis(),
                        config.gracefulTimeoutMillis(),
                        TimeUnit.MILLISECONDS)
                .syncUninterruptibly();
    }

    private boolean isTerminated(EventLoopGroup group) {
        return group == null || group.isTerminated();
    }

    @Override
    public void close() {
        stop();
    }
}
