package com.linhgioi.server.api.account;

public record CreateCharacterRequest(String name, String classId, Integer slot) {
    public CreateCharacterRequest(String name, String classId) {
        this(name, classId, null);
    }
}
