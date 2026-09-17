package com.linhgioi.server.api.auth;

public interface RecoveryDelivery {
    boolean isAvailable();
    void sendVerificationCode(String normalizedEmail, String code);
}
