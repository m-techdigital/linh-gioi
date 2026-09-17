package com.linhgioi.server.api.auth;

public record RecoveryVerifyRequest(String challengeId, String code) { }
