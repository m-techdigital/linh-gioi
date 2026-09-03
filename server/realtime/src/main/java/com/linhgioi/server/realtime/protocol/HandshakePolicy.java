package com.linhgioi.server.realtime.protocol;

import com.linhgioi.protocol.v1.ClientHello;

public final class HandshakePolicy {
    public HandshakeDecision evaluate(ClientHello hello) {
        if (hello.getProtocolVersion() != HandshakeProtocol.PROTOCOL_VERSION) {
            return HandshakeDecision.reject(
                    "UNSUPPORTED_PROTOCOL_VERSION",
                    "Client protocol version is incompatible with this server.",
                    false);
        }
        if (hello.getGamedataVersion() != HandshakeProtocol.GAMEDATA_VERSION) {
            return HandshakeDecision.reject(
                    "GAMEDATA_VERSION_MISMATCH",
                    "Client GameData version must be updated before connecting.",
                    true);
        }
        if (hello.getClientVersion().isBlank()) {
            return HandshakeDecision.reject("INVALID_CLIENT_HELLO", "client_version must not be blank.", false);
        }
        if (hello.getPlatform().isBlank()) {
            return HandshakeDecision.reject("INVALID_CLIENT_HELLO", "platform must not be blank.", false);
        }
        if (hello.getLocale().isBlank()) {
            return HandshakeDecision.reject("INVALID_CLIENT_HELLO", "locale must not be blank.", false);
        }
        return HandshakeDecision.accept();
    }
}
