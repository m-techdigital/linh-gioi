package com.linhgioi.server.realtime.protocol;

public final class HandshakeProtocol {
    public static final int PROTOCOL_VERSION = 1;
    public static final int GAMEDATA_VERSION = 1;
    public static final int MAX_FRAME_BYTES = 64 * 1024;

    private HandshakeProtocol() {}
}
