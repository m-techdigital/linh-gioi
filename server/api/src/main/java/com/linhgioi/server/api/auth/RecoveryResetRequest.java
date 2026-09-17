package com.linhgioi.server.api.auth;

public record RecoveryResetRequest(String resetToken, String newPassword) { }
