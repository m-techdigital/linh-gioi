package com.linhgioi.server.api.auth;

import tools.jackson.databind.ObjectMapper;
import tools.jackson.databind.SerializationFeature;
import tools.jackson.databind.json.JsonMapper;
import java.io.IOException;
import java.io.UncheckedIOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardCopyOption;
import java.time.Clock;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Objects;
import java.util.Optional;

public final class JsonFileProductCredentialStore implements ProductCredentialStore {
    public static final int SCHEMA_VERSION = 1;
    public static final String STORE_FILE_NAME = "auth-credentials-v1.json";

    private final Path storeFile;
    private final ObjectMapper mapper;
    private Snapshot snapshot;

    public JsonFileProductCredentialStore(Path rootDirectory, Clock clock) {
        Objects.requireNonNull(rootDirectory, "rootDirectory");
        Objects.requireNonNull(clock, "clock");
        storeFile = rootDirectory.resolve(STORE_FILE_NAME);
        mapper = JsonMapper.builder().enable(SerializationFeature.INDENT_OUTPUT).build();
        snapshot = loadOrCreate();
    }

    @Override
    public synchronized ProductCredential create(
            String accountId, String normalizedIdentifier, String passwordHash, long nowUnixMs) {
        requireCanonicalIdentifier(normalizedIdentifier);
        if (snapshot.credentialsByIdentifier.containsKey(normalizedIdentifier)) {
            throw new IllegalArgumentException("product identifier already exists");
        }
        ProductCredential credential = new ProductCredential(
                accountId, normalizedIdentifier, passwordHash, nowUnixMs, nowUnixMs);
        snapshot.credentialsByIdentifier.put(normalizedIdentifier, credential);
        persist();
        return credential;
    }

    @Override
    public synchronized Optional<ProductCredential> findByIdentifier(String normalizedIdentifier) {
        if (normalizedIdentifier == null || normalizedIdentifier.isBlank()) return Optional.empty();
        return Optional.ofNullable(snapshot.credentialsByIdentifier.get(normalizedIdentifier));
    }

    private Snapshot loadOrCreate() {
        try {
            Files.createDirectories(storeFile.getParent());
            if (!Files.exists(storeFile)) return new Snapshot();
            Snapshot loaded = mapper.readValue(storeFile.toFile(), Snapshot.class);
            validate(loaded);
            return loaded;
        } catch (IOException exception) {
            throw new UncheckedIOException("failed to load product credential store: " + storeFile, exception);
        }
    }

    private void persist() {
        try {
            Files.createDirectories(storeFile.getParent());
            validate(snapshot);
            Path temp = storeFile.resolveSibling(storeFile.getFileName() + ".tmp");
            mapper.writeValue(temp.toFile(), snapshot);
            try {
                Files.move(temp, storeFile, StandardCopyOption.ATOMIC_MOVE, StandardCopyOption.REPLACE_EXISTING);
            } catch (IOException atomicMoveFailed) {
                Files.move(temp, storeFile, StandardCopyOption.REPLACE_EXISTING);
            }
        } catch (IOException exception) {
            throw new UncheckedIOException("failed to persist product credential store: " + storeFile, exception);
        }
    }

    private static void validate(Snapshot value) {
        if (value == null || value.schemaVersion != SCHEMA_VERSION) {
            throw new IllegalStateException("unsupported product credential schema");
        }
        if (value.credentialsByIdentifier == null) value.credentialsByIdentifier = new LinkedHashMap<>();
        value.credentialsByIdentifier.forEach((identifier, credential) -> {
            requireCanonicalIdentifier(identifier);
            if (credential == null || !identifier.equals(credential.normalizedIdentifier())) {
                throw new IllegalStateException("product credential index drift: " + identifier);
            }
        });
    }

    private static void requireCanonicalIdentifier(String identifier) {
        if (identifier == null || identifier.isBlank() || identifier.length() < 3 || identifier.length() > 254
                || !identifier.equals(identifier.trim())) {
            throw new IllegalArgumentException("normalizedIdentifier must be canonical and 3..254 characters");
        }
    }

    public static final class Snapshot {
        private int schemaVersion = SCHEMA_VERSION;
        private Map<String, ProductCredential> credentialsByIdentifier = new LinkedHashMap<>();

        public int getSchemaVersion() {
            return schemaVersion;
        }

        public void setSchemaVersion(int schemaVersion) {
            this.schemaVersion = schemaVersion;
        }

        public Map<String, ProductCredential> getCredentialsByIdentifier() {
            return credentialsByIdentifier;
        }

        public void setCredentialsByIdentifier(Map<String, ProductCredential> credentialsByIdentifier) {
            this.credentialsByIdentifier = credentialsByIdentifier == null
                    ? new LinkedHashMap<>() : new LinkedHashMap<>(credentialsByIdentifier);
        }
    }
}
