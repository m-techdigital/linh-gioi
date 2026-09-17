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
    // v1 remains untouched as a rollback baseline; v2 owns persistent character slots.
    // Persistence hygiene: raw dev key values are never written to disk.
    public static final int SCHEMA_VERSION = 3;
    public static final String STORE_FILE_NAME = "players-v3.json";
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
        AccountProfile account = new AccountProfile(accountId, safeDisplayName, now, now);
        snapshot.getAccountsById().put(account.accountId(), account);
        snapshot.getAccountIdByDevKeyHash().put(keyHash, account.accountId());
        persist();
        return new DevLoginResult(account, true);
    }

    @Override
    public synchronized java.util.Optional<AccountProfile> findAccount(String accountId) {
        if (accountId == null || accountId.isBlank()) return java.util.Optional.empty();
        return java.util.Optional.ofNullable(snapshot.getAccountsById().get(accountId.trim()));
    }

    @Override
    public synchronized AccountProfile createProductAccount(String displayName) {
        String safeDisplayName = normalizeProductDisplayName(displayName);
        long now = clock.millis();
        String accountId;
        do {
            accountId = "account.product." + UUID.randomUUID().toString().replace("-", "");
        } while (snapshot.getAccountsById().containsKey(accountId));
        AccountProfile account = new AccountProfile(accountId, safeDisplayName, now, now);
        snapshot.getAccountsById().put(accountId, account);
        persist();
        return account;
    }

    @Override
    public synchronized boolean deleteEmptyProductAccount(String accountId) {
        if (accountId == null || !accountId.startsWith("account.product.")) return false;
        AccountProfile account = snapshot.getAccountsById().get(accountId);
        if (account == null) return false;
        if (snapshot.getAccountIdByDevKeyHash().containsValue(accountId)) return false;
        boolean occupied = snapshot.getCharactersById().values().stream()
                .anyMatch(character -> accountId.equals(character.accountId()));
        if (occupied) return false;
        snapshot.getAccountsById().remove(accountId);
        persist();
        return true;
    }

    @Override
    public synchronized List<CharacterProfile> listCharacters(String accountId) {
        requireAccount(accountId);
        return snapshot.getCharactersById().values().stream()
                .filter(character -> accountId.equals(character.accountId()))
                .sorted(Comparator.comparingInt(CharacterProfile::slot))
                .toList();
    }

    @Override
    public synchronized CharacterProfile createCharacter(CreateCharacterCommand command) {
        Objects.requireNonNull(command, "command");
        requireAccount(command.accountId());
        if (listCharacters(command.accountId()).size() >= 3) {
            throw new IllegalArgumentException("account character limit reached (3)");
        }
        var occupied = listCharacters(command.accountId()).stream().map(CharacterProfile::slot).toList();
        int slot = command.slot() == null
                ? java.util.stream.IntStream.rangeClosed(1, 3).filter(value -> !occupied.contains(value)).findFirst().orElseThrow()
                : command.slot();
        if (slot < 1 || slot > 3 || occupied.contains(slot)) {
            throw new IllegalArgumentException("character slot must be free and between 1 and 3");
        }
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
                now,
                slot);
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
            if (Files.exists(storeFile)) {
                PlayerPersistenceSnapshot loaded = mapper.readValue(storeFile.toFile(), PlayerPersistenceSnapshot.class);
                validateSnapshot(loaded);
                return loaded;
            }
            Path v2File = storeFile.resolveSibling("players-v2.json");
            if (Files.exists(v2File)) return migrateLegacy(v2File, 2);
            Path v1File = storeFile.resolveSibling("players-v1.json");
            if (Files.exists(v1File)) return migrateLegacy(v1File, 1);
            PlayerPersistenceSnapshot created = new PlayerPersistenceSnapshot();
            created.setNextEntityId(INITIAL_ENTITY_ID);
            return created;
        } catch (IOException exception) {
            throw new UncheckedIOException("failed to load player persistence store: " + storeFile, exception);
        }
    }

    private PlayerPersistenceSnapshot migrateLegacy(Path legacyFile, int expectedVersion) throws IOException {
        LegacySnapshot legacy = mapper.readValue(legacyFile.toFile(), LegacySnapshot.class);
        if (legacy.schemaVersion != expectedVersion) {
            throw new IllegalStateException("unsupported legacy player schema");
        }
        if (expectedVersion == 1) assignLegacySlots(legacy);
        PlayerPersistenceSnapshot migrated = new PlayerPersistenceSnapshot();
        migrated.setSchemaVersion(SCHEMA_VERSION);
        migrated.setNextEntityId(legacy.nextEntityId);
        migrated.setAccountIdByDevKeyHash(legacy.accountIdByDevKeyHash);
        migrated.setCharactersById(legacy.charactersById);
        legacy.accountsById.forEach((accountId, account) -> {
            if (account == null || !accountId.equals(account.accountId)) {
                throw new IllegalStateException("legacy account index drift: " + accountId);
            }
            if (expectedVersion == 2 && (account.devKeyHash == null || account.devKeyHash.isBlank())) {
                throw new IllegalStateException("v2 account missing devKeyHash: " + accountId);
            }
            if (account.devKeyHash != null && !account.devKeyHash.isBlank()) {
                String indexed = legacy.accountIdByDevKeyHash.get(account.devKeyHash);
                if (!accountId.equals(indexed)) {
                    throw new IllegalStateException("dev-key index hash drift for account: " + accountId);
                }
            }
            migrated.getAccountsById().put(accountId, new AccountProfile(
                    account.accountId, account.displayName, account.createdAtUnixMs, account.updatedAtUnixMs));
        });
        validateSnapshot(migrated);
        writeSnapshot(migrated);
        return migrated;
    }

    private static void assignLegacySlots(LegacySnapshot legacy) {
        for (String accountId : legacy.accountsById.keySet()) {
            var characters = legacy.charactersById.values().stream()
                    .filter(character -> character.accountId().equals(accountId))
                    .sorted(Comparator.comparingLong(CharacterProfile::createdAtUnixMs)
                            .thenComparingLong(CharacterProfile::entityId))
                    .toList();
            if (characters.size() > 3) {
                throw new IllegalStateException(
                        "legacy account exceeds three slots; owner resolution required; v1 file preserved: " + accountId);
            }
            for (int index = 0; index < characters.size(); index++) {
                var character = characters.get(index);
                legacy.charactersById.put(character.characterId(), character.withSlot(index + 1));
            }
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
            if (!hash.matches("[0-9a-f]{64}")) {
                throw new IllegalStateException("invalid dev-key hash index");
            }
            if (!loaded.getAccountsById().containsKey(accountId)) {
                throw new IllegalStateException("dev-key index references missing account: " + accountId);
            }
        });
        loaded.getCharactersById().values().forEach(character -> {
            if (character.slot() == null) throw new IllegalStateException("v3 character is missing slot");
            if (!loaded.getAccountsById().containsKey(character.accountId())) {
                throw new IllegalStateException("character references missing account: " + character.characterId());
            }
        });
        for (String accountId : loaded.getAccountsById().keySet()) {
            var slots = loaded.getCharactersById().values().stream()
                    .filter(character -> character.accountId().equals(accountId)).map(CharacterProfile::slot).toList();
            if (slots.size() > 3 || slots.stream().distinct().count() != slots.size()) {
                throw new IllegalStateException("duplicate or excessive character slots: " + accountId);
            }
        }
    }

    private void persist() {
        writeSnapshot(snapshot);
    }

    private void writeSnapshot(PlayerPersistenceSnapshot value) {
        try {
            Files.createDirectories(storeFile.getParent());
            validateSnapshot(value);
            Path temp = storeFile.resolveSibling(storeFile.getFileName() + ".tmp");
            mapper.writeValue(temp.toFile(), value);
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

    private static String normalizeProductDisplayName(String displayName) {
        if (displayName == null || displayName.isBlank()) {
            throw new IllegalArgumentException("displayName must not be blank");
        }
        String normalized = displayName.trim();
        if (normalized.length() > 254) {
            throw new IllegalArgumentException("displayName must be <= 254 characters");
        }
        return normalized;
    }

    public static final class LegacySnapshot {
        public int schemaVersion = 1;
        public long nextEntityId = INITIAL_ENTITY_ID;
        public java.util.Map<String, LegacyAccountProfile> accountsById = new java.util.LinkedHashMap<>();
        public java.util.Map<String, String> accountIdByDevKeyHash = new java.util.LinkedHashMap<>();
        public java.util.Map<String, CharacterProfile> charactersById = new java.util.LinkedHashMap<>();

        public int getSchemaVersion() { return schemaVersion; }
        public void setSchemaVersion(int value) { schemaVersion = value; }
        public long getNextEntityId() { return nextEntityId; }
        public void setNextEntityId(long value) { nextEntityId = value; }
        public java.util.Map<String, LegacyAccountProfile> getAccountsById() { return accountsById; }
        public void setAccountsById(java.util.Map<String, LegacyAccountProfile> value) {
            accountsById = value == null ? new java.util.LinkedHashMap<>() : new java.util.LinkedHashMap<>(value);
        }
        public java.util.Map<String, String> getAccountIdByDevKeyHash() { return accountIdByDevKeyHash; }
        public void setAccountIdByDevKeyHash(java.util.Map<String, String> value) {
            accountIdByDevKeyHash = value == null ? new java.util.LinkedHashMap<>() : new java.util.LinkedHashMap<>(value);
        }
        public java.util.Map<String, CharacterProfile> getCharactersById() { return charactersById; }
        public void setCharactersById(java.util.Map<String, CharacterProfile> value) {
            charactersById = value == null ? new java.util.LinkedHashMap<>() : new java.util.LinkedHashMap<>(value);
        }
    }

    public static final class LegacyAccountProfile {
        public String accountId;
        public String devKeyHash;
        public String displayName;
        public long createdAtUnixMs;
        public long updatedAtUnixMs;

        public String getAccountId() { return accountId; }
        public void setAccountId(String value) { accountId = value; }
        public String getDevKeyHash() { return devKeyHash; }
        public void setDevKeyHash(String value) { devKeyHash = value; }
        public String getDisplayName() { return displayName; }
        public void setDisplayName(String value) { displayName = value; }
        public long getCreatedAtUnixMs() { return createdAtUnixMs; }
        public void setCreatedAtUnixMs(long value) { createdAtUnixMs = value; }
        public long getUpdatedAtUnixMs() { return updatedAtUnixMs; }
        public void setUpdatedAtUnixMs(long value) { updatedAtUnixMs = value; }
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
