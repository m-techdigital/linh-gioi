package com.linhgioi.server.api.persistence;

public record CreateCharacterCommand(String accountId, String name, String classId, Integer slot) {
    public CreateCharacterCommand(String accountId, String name, String classId) {
        this(accountId, name, classId, null);
    }
}
