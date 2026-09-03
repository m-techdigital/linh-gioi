package com.linhgioi.server.realtime.protocol;

public record HandshakeDecision(boolean accepted, String errorCode, String errorMessage, boolean retryable) {
    public static HandshakeDecision accept() {
        return new HandshakeDecision(true, "", "", false);
    }

    public static HandshakeDecision reject(String code, String message, boolean retryable) {
        return new HandshakeDecision(false, code, message, retryable);
    }
}
