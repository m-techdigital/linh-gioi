package com.linhgioi.server.api.persistence;

import java.util.LinkedHashMap;
import java.util.Map;

public final class PlayerPersistenceSnapshot {
    private int schemaVersion = JsonFilePlayerProfileStore.SCHEMA_VERSION;
    private long nextEntityId = 1001L;
    private Map<String, AccountProfile> accountsById = new LinkedHashMap<>();
    private Map<String, String> accountIdByDevKeyHash = new LinkedHashMap<>();
    private Map<String, CharacterProfile> charactersById = new LinkedHashMap<>();

    public int getSchemaVersion() {
        return schemaVersion;
    }

    public void setSchemaVersion(int schemaVersion) {
        this.schemaVersion = schemaVersion;
    }

    public long getNextEntityId() {
        return nextEntityId;
    }

    public void setNextEntityId(long nextEntityId) {
        this.nextEntityId = nextEntityId;
    }

    public Map<String, AccountProfile> getAccountsById() {
        return accountsById;
    }

    public void setAccountsById(Map<String, AccountProfile> accountsById) {
        this.accountsById = accountsById == null ? new LinkedHashMap<>() : new LinkedHashMap<>(accountsById);
    }

    public Map<String, String> getAccountIdByDevKeyHash() {
        return accountIdByDevKeyHash;
    }

    public void setAccountIdByDevKeyHash(Map<String, String> accountIdByDevKeyHash) {
        this.accountIdByDevKeyHash = accountIdByDevKeyHash == null ? new LinkedHashMap<>() : new LinkedHashMap<>(accountIdByDevKeyHash);
    }

    public Map<String, CharacterProfile> getCharactersById() {
        return charactersById;
    }

    public void setCharactersById(Map<String, CharacterProfile> charactersById) {
        this.charactersById = charactersById == null ? new LinkedHashMap<>() : new LinkedHashMap<>(charactersById);
    }
}
