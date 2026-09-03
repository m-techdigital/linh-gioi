package com.linhgioi.server.realtime.protocol;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.protocol.v1.ClientHello;
import org.junit.jupiter.api.Test;

class HandshakePolicyTest {
    private final HandshakePolicy policy = new HandshakePolicy();

    @Test
    void acceptsCompatibleClient() {
        assertTrue(policy.evaluate(validHello()).accepted());
    }

    @Test
    void rejectsUnsupportedProtocolVersion() {
        HandshakeDecision decision = policy.evaluate(validHello().toBuilder().setProtocolVersion(999).build());
        assertFalse(decision.accepted());
        assertEquals("UNSUPPORTED_PROTOCOL_VERSION", decision.errorCode());
    }

    @Test
    void rejectsMismatchedGameDataVersion() {
        HandshakeDecision decision = policy.evaluate(validHello().toBuilder().setGamedataVersion(999).build());
        assertFalse(decision.accepted());
        assertEquals("GAMEDATA_VERSION_MISMATCH", decision.errorCode());
    }

    @Test
    void rejectsBlankSemanticFields() {
        assertFalse(policy.evaluate(validHello().toBuilder().clearClientVersion().build()).accepted());
        assertFalse(policy.evaluate(validHello().toBuilder().clearPlatform().build()).accepted());
        assertFalse(policy.evaluate(validHello().toBuilder().clearLocale().build()).accepted());
    }

    static ClientHello validHello() {
        return ClientHello.newBuilder()
                .setProtocolVersion(HandshakeProtocol.PROTOCOL_VERSION)
                .setClientVersion("m0-test")
                .setGamedataVersion(HandshakeProtocol.GAMEDATA_VERSION)
                .setPlatform("test")
                .setLocale("vi-VN")
                .build();
    }
}
