package com.linhgioi.server.api.persistence;

import tools.jackson.databind.ObjectMapper;
import tools.jackson.databind.SerializationFeature;
import tools.jackson.databind.json.JsonMapper;
import java.io.IOException;
import java.io.UncheckedIOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardCopyOption;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.time.Clock;
import java.util.Comparator;
import java.util.HexFormat;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.Objects;
import java.util.UUID;

public final class JsonFilePlayerProfileStore implements PlayerProfileStore {
    // Persistence hygiene: raw dev key values are never written to players-v1.json.
    public static final int SCHEMA_VERSION = 1;
    public static final String STORE_FILE_NAME = "players-v1.json";
    private static final long INITIAL_ENTITY_ID = 1001L;

    private final Path storeFile;
    private final ObjectMapper mapper;
    private final Clock clock;
    private PlayerPersistenceSnapshot snapshot;

    public JsonFilePlayerProfileStore(Path rootDirectory, Clock clock) {
        Objects.requireNonNull(rootDirectory, "rootDirectory");
        this.storeFile = rootDirectory.resolve(STORE_FILE_NAME);
        this.clock = Objects.requireNonNull(clock, "clock");
        this.mapper = JsonMapper.builder().enable(SerializationFeature.INDENT_OUTPUT).build();
        this.snapshot = loadOrCreate();
    }

    @Override
    public synchronized DevLoginResult loginDev(String devKey, String displayName) {
        String normalizedKey = normalizeDevKey(devKey);
        String safeDisplayName = normalizeDisplayName(displayName);
        String keyHash = sha256(normalizedKey);
        String existingAccountId = snapshot.getAccountIdByDevKeyHash().get(keyHash);
        if (existingAccountId != null) {
            AccountProfile existing = snapshot.getAccountsById().get(existingAccountId);
            if (existing == null) {
                throw new IllegalStateException("dev-key index references missing account: " + existingAccountId);
            }
            return new DevLoginResult(existing, false);
        }

        long now = clock.millis();
        String accountId = "account.dev." + keyHash.substring(0, 16);
        AccountProfile account = new AccountProfile(accountId, keyHash, safeDisplayName, now, now);
        snapshot.getAccountsById().put(account.accountId(), account);
        snapshot.getAccountIdByDevKeyHash().put(keyHash, account.accountId());
        persist();
        return new DevLoginResult(account, true);
    }

    @Override
    public synchronized List<CharacterProfile> listCharacters(String accountId) {
        requireAccount(accountId);
        return snapshot.getCharactersById().values().stream()
                .filter(character -> accountId.equals(character.accountId()))
                .sorted(Comparator.comparingLong(CharacterProfile::createdAtUnixMs))
                .toList();
    }

    @Override
    public synchronized CharacterProfile createCharacter(CreateCharacterCommand command) {
        Objects.requireNonNull(command, "command");
        requireAccount(command.accountId());
        String name = normalizeCharacterName(command.name());
        String classId = normalizeClassId(command.classId());
        boolean duplicateName = snapshot.getCharactersById().values().stream()
                .anyMatch(character -> command.accountId().equals(character.accountId()) && character.name().equalsIgnoreCase(name));
        if (duplicateName) {
            throw new IllegalArgumentException("character name already exists for account");
        }

        long now = clock.millis();
        long entityId = reserveEntityId();
        String characterId = "character." + UUID.nameUUIDFromBytes((command.accountId() + ":" + name + ":" + entityId)
                .getBytes(StandardCharsets.UTF_8)).toString().replace("-", "");
        CharacterProfile character = new CharacterProfile(
                characterId,
                command.accountId(),
                name,
                classId,
                entityId,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                now,
                now);
        snapshot.getCharactersById().put(character.characterId(), character);
        persist();
        return character;
    }

    @Override
    public synchronized java.util.Optional<CharacterProfile> findCharacter(String characterId) {
        if (characterId == null || characterId.isBlank()) {
            return java.util.Optional.empty();
        }
        return java.util.Optional.ofNullable(snapshot.getCharactersById().get(characterId.trim()));
    }

    @Override
    public synchronized CharacterProfile saveCharacterPosition(SaveCharacterPositionCommand command) {
        Objects.requireNonNull(command, "command");
        CharacterProfile existing = findCharacter(command.characterId())
                .orElseThrow(() -> new NoSuchElementException("character not found"));
        if (!Float.isFinite(command.x()) || !Float.isFinite(command.y()) || !Float.isFinite(command.z()) || !Float.isFinite(command.yawDegrees())) {
            throw new IllegalArgumentException("position/yaw values must be finite");
        }
        CharacterProfile updated = existing.withPosition(command.x(), command.y(), command.z(), command.yawDegrees(), clock.millis());
        snapshot.getCharactersById().put(updated.characterId(), updated);
        persist();
        return updated;
    }

    private PlayerPersistenceSnapshot loadOrCreate() {
        try {
            Files.createDirectories(storeFile.getParent());
            if (!Files.exists(storeFile)) {
                PlayerPersistenceSnapshot created = new PlayerPersistenceSnapshot();
                created.setNextEntityId(INITIAL_ENTITY_ID);
                return created;
            }
            PlayerPersistenceSnapshot loaded = mapper.readValue(storeFile.toFile(), PlayerPersistenceSnapshot.class);
            validateSnapshot(loaded);
            return loaded;
        } catch (IOException exception) {
            throw new UncheckedIOException("failed to load player persistence store: " + storeFile, exception);
        }
    }

    private void validateSnapshot(PlayerPersistenceSnapshot loaded) {
        if (loaded.getSchemaVersion() != SCHEMA_VERSION) {
            throw new IllegalStateException("unsupported player persistence schema version: " + loaded.getSchemaVersion());
        }
        if (loaded.getNextEntityId() < INITIAL_ENTITY_ID) {
            throw new IllegalStateException("nextEntityId must be >= " + INITIAL_ENTITY_ID);
        }
        loaded.getAccountIdByDevKeyHash().forEach((hash, accountId) -> {
            if (!loaded.getAccountsById().containsKey(accountId)) {
                throw new IllegalStateException("dev-key index references missing account: " + accountId);
            }
            AccountProfile account = loaded.getAccountsById().get(accountId);
            if (!account.devKeyHash().equals(hash)) {
                throw new IllegalStateException("dev-key index hash drift for account: " + accountId);
            }
        });
        loaded.getCharactersById().values().forEach(character -> {
            if (!loaded.getAccountsById().containsKey(character.accountId())) {
                throw new IllegalStateException("character references missing account: " + character.characterId());
            }
        });
    }

    private void persist() {
        try {
            Files.createDirectories(storeFile.getParent());
            validateSnapshot(snapshot);
            Path temp = storeFile.resolveSibling(storeFile.getFileName() + ".tmp");
            mapper.writeValue(temp.toFile(), snapshot);
            try {
                Files.move(temp, storeFile, StandardCopyOption.ATOMIC_MOVE, StandardCopyOption.REPLACE_EXISTING);
            } catch (IOException atomicMoveFailed) {
                Files.move(temp, storeFile, StandardCopyOption.REPLACE_EXISTING);
            }
        } catch (IOException exception) {
            throw new UncheckedIOException("failed to persist player profile store: " + storeFile, exception);
        }
    }

    private void requireAccount(String accountId) {
        if (accountId == null || !snapshot.getAccountsById().containsKey(accountId)) {
            throw new NoSuchElementException("account not found");
        }
    }

    private long reserveEntityId() {
        long value = snapshot.getNextEntityId();
        snapshot.setNextEntityId(value + 1);
        return value;
    }

    private static String normalizeDevKey(String devKey) {
        if (devKey == null || devKey.isBlank()) {
            throw new IllegalArgumentException("devKey must not be blank");
        }
        String normalized = devKey.trim();
        if (normalized.length() < 3 || normalized.length() > 80) {
            throw new IllegalArgumentException("devKey must be between 3 and 80 characters");
        }
        return normalized;
    }

    private static String normalizeDisplayName(String displayName) {
        if (displayName == null || displayName.isBlank()) {
            throw new IllegalArgumentException("displayName must not be blank");
        }
        String normalized = displayName.trim();
        if (normalized.length() > 32) {
            throw new IllegalArgumentException("displayName must be <= 32 characters");
        }
        return normalized;
    }

    private static String normalizeCharacterName(String name) {
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("name must not be blank");
        }
        String normalized = name.trim();
        if (normalized.length() < 3 || normalized.length() > 16) {
            throw new IllegalArgumentException("name must be between 3 and 16 characters");
        }
        if (!normalized.matches("[A-Za-z0-9_]+")) {
            throw new IllegalArgumentException("name may only contain letters, numbers, and underscore");
        }
        return normalized;
    }

    private static String normalizeClassId(String classId) {
        if (classId == null || classId.isBlank()) {
            return "class.sword";
        }
        String normalized = classId.trim();
        if (!"class.sword".equals(normalized) && !"class.martial".equals(normalized)) {
            throw new IllegalArgumentException("classId must be class.sword or class.martial");
        }
        return normalized;
    }

    private static String sha256(String value) {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            return HexFormat.of().formatHex(digest.digest(value.getBytes(StandardCharsets.UTF_8)));
        } catch (NoSuchAlgorithmException exception) {
            throw new IllegalStateException("SHA-256 unavailable", exception);
        }
    }
}
