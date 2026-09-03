package com.linhgioi.server.api.persistence;

public record CreateCharacterCommand(String accountId, String name, String classId) {}
