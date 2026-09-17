package com.linhgioi.server.api.persistence;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Instant;
import java.time.ZoneOffset;
import java.util.NoSuchElementException;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

class JsonFilePlayerProfileStoreTest {
    private final Clock clock = Clock.fixed(Instant.ofEpochMilli(1_700_000_000_000L), ZoneOffset.UTC);

    @TempDir
    Path tempDir;

    @Test
    void persistsChosenSlotAndRejectsOccupiedSlots() {
        var store = new JsonFilePlayerProfileStore(tempDir, clock);
        String account = store.loginDev("slot-choice", "Slots").account().accountId();
        var third = store.createCharacter(new CreateCharacterCommand(account, "ThirdHero", "class.sword", 3));
        assertEquals(3, third.slot());
        assertThrows(IllegalArgumentException.class,
                () -> store.createCharacter(new CreateCharacterCommand(account, "Collision", "class.sword", 3)));
        assertThrows(IllegalArgumentException.class,
                () -> store.createCharacter(new CreateCharacterCommand(account, "InvalidSlot", "class.sword", 4)));
        var first = store.createCharacter(new CreateCharacterCommand(account, "FirstHero", "class.sword"));
        assertEquals(1, first.slot());
        store.saveCharacterPosition(new SaveCharacterPositionCommand(third.characterId(), 1, 0, 2, 90));
        var reloaded = new JsonFilePlayerProfileStore(tempDir, clock);
        assertEquals(3, reloaded.findCharacter(third.characterId()).orElseThrow().slot());
        assertEquals(first.characterId(), reloaded.listCharacters(account).getFirst().characterId());
    }

    @Test
    void migratesLegacySlotsWithoutChangingLegacyFile() throws Exception {
        var store = new JsonFilePlayerProfileStore(tempDir, clock);
        String account = store.loginDev("legacy-slots", "Legacy").account().accountId();
        store.createCharacter(new CreateCharacterCommand(account, "OldHero", "class.sword"));
        var mapper = tools.jackson.databind.json.JsonMapper.builder().build();
        var currentFile = tempDir.resolve(JsonFilePlayerProfileStore.STORE_FILE_NAME);
        var legacy = (tools.jackson.databind.node.ObjectNode) mapper.readTree(currentFile.toFile());
        legacy.put("schemaVersion", 1);
        legacy.path("charactersById").forEach(node -> ((tools.jackson.databind.node.ObjectNode) node).remove("slot"));
        String original = mapper.writeValueAsString(legacy);
        Files.writeString(tempDir.resolve("players-v1.json"), original);
        Files.delete(currentFile);
        var migrated = new JsonFilePlayerProfileStore(tempDir, clock);
        assertEquals(1, migrated.listCharacters(account).getFirst().slot());
        migrated.createCharacter(new CreateCharacterCommand(account, "NewHero", "class.sword", 3));
        assertEquals(original, Files.readString(tempDir.resolve("players-v1.json")));
        assertEquals(2, new JsonFilePlayerProfileStore(tempDir, clock).listCharacters(account).size());
    }

    @Test
    void limitsEachAccountToThreeCharactersAcrossReload() {
        JsonFilePlayerProfileStore store = new JsonFilePlayerProfileStore(tempDir, clock);
        String account = store.loginDev("three-slots", "Slots").account().accountId();
        for (int slot = 1; slot <= 3; slot++) {
            store.createCharacter(new CreateCharacterCommand(account, "Hero" + slot, "class.sword"));
        }
        JsonFilePlayerProfileStore reloaded = new JsonFilePlayerProfileStore(tempDir, clock);
        assertThrows(IllegalArgumentException.class,
                () -> reloaded.createCharacter(new CreateCharacterCommand(account, "FourthHero", "class.sword")));
        assertEquals(3, new JsonFilePlayerProfileStore(tempDir, clock).listCharacters(account).size());
        String other = reloaded.loginDev("other-slots", "Other").account().accountId();
        reloaded.createCharacter(new CreateCharacterCommand(other, "OtherHero", "class.sword"));
        assertEquals(1, reloaded.listCharacters(other).size());
    }

    @Test
    void preservesOverLimitLegacyDataWithoutCreatingV2File() throws Exception {
        var store = new JsonFilePlayerProfileStore(tempDir, clock);
        String account = store.loginDev("legacy-overflow", "Legacy").account().accountId();
        var character = store.createCharacter(new CreateCharacterCommand(account, "LegacyOne", "class.sword"));
        var mapper = tools.jackson.databind.json.JsonMapper.builder().build();
        var currentFile = tempDir.resolve(JsonFilePlayerProfileStore.STORE_FILE_NAME);
        var legacy = (tools.jackson.databind.node.ObjectNode) mapper.readTree(currentFile.toFile());
        legacy.put("schemaVersion", 1);
        legacy.put("nextEntityId", 1005);
        var characters = (tools.jackson.databind.node.ObjectNode) legacy.path("charactersById");
        var original = (tools.jackson.databind.node.ObjectNode) characters.path(character.characterId());
        original.remove("slot");
        for (int index = 2; index <= 4; index++) {
            var copy = original.deepCopy();
            copy.put("characterId", "character.legacy" + index);
            copy.put("name", "Legacy" + index);
            copy.put("entityId", 1000 + index);
            characters.set("character.legacy" + index, copy);
        }
        String bytes = mapper.writeValueAsString(legacy);
        Files.writeString(tempDir.resolve("players-v1.json"), bytes);
        Files.delete(currentFile);
        var failure = assertThrows(IllegalStateException.class, () -> new JsonFilePlayerProfileStore(tempDir, clock));
        assertTrue(failure.getMessage().contains("owner resolution required"));
        assertEquals(bytes, Files.readString(tempDir.resolve("players-v1.json")));
        assertFalse(Files.exists(currentFile));
    }

    @Test
    void devLoginCreatesStableAccountWithoutPersistingRawDevKey() throws Exception {
        JsonFilePlayerProfileStore store = new JsonFilePlayerProfileStore(tempDir, clock);

        DevLoginResult first = store.loginDev(" local-dev-key ", " Minh ");
        DevLoginResult second = store.loginDev("local-dev-key", "Ignored Later Name");

        assertTrue(first.created());
        assertFalse(second.created());
        assertEquals(first.account().accountId(), second.account().accountId());
        assertEquals("Minh", second.account().displayName());
        String stored = Files.readString(tempDir.resolve(JsonFilePlayerProfileStore.STORE_FILE_NAME), StandardCharsets.UTF_8);
        assertFalse(stored.contains("local-dev-key"));
        assertTrue(stored.contains("account.dev."));
    }

    @Test
    void createsListsSavesAndReloadsCharacter() {
        JsonFilePlayerProfileStore store = new JsonFilePlayerProfileStore(tempDir, clock);
        String accountId = store.loginDev("dev-key", "Minh").account().accountId();

        CharacterProfile created = store.createCharacter(new CreateCharacterCommand(accountId, "KiemTu", "class.sword"));
        CharacterProfile moved = store.saveCharacterPosition(new SaveCharacterPositionCommand(
                created.characterId(), 1.5f, 0.0f, -2.0f, 180.0f));
        JsonFilePlayerProfileStore reloaded = new JsonFilePlayerProfileStore(tempDir, clock);

        assertEquals(1, reloaded.listCharacters(accountId).size());
        CharacterProfile loaded = reloaded.findCharacter(created.characterId()).orElseThrow();
        assertEquals(created.characterId(), loaded.characterId());
        assertEquals(moved.positionX(), loaded.positionX(), 0.0001f);
        assertEquals(moved.positionZ(), loaded.positionZ(), 0.0001f);
        assertEquals(180.0f, loaded.yawDegrees(), 0.0001f);
    }

    @Test
    void rejectsInvalidCreateAndPositionRequestsWithoutCorruptingStore() {
        JsonFilePlayerProfileStore store = new JsonFilePlayerProfileStore(tempDir, clock);
        String accountId = store.loginDev("dev-key", "Minh").account().accountId();
        CharacterProfile created = store.createCharacter(new CreateCharacterCommand(accountId, "KiemTu", "class.sword"));

        assertThrows(NoSuchElementException.class, () -> store.createCharacter(new CreateCharacterCommand("account.dev.missing", "Other", "class.sword")));
        assertThrows(IllegalArgumentException.class, () -> store.createCharacter(new CreateCharacterCommand(accountId, "No", "class.sword")));
        assertThrows(IllegalArgumentException.class, () -> store.createCharacter(new CreateCharacterCommand(accountId, "KiemTu", "class.sword")));
        assertThrows(IllegalArgumentException.class, () -> store.createCharacter(new CreateCharacterCommand(accountId, "VoGia", "class.unknown")));
        assertThrows(IllegalArgumentException.class, () -> store.saveCharacterPosition(new SaveCharacterPositionCommand(created.characterId(), Float.NaN, 0, 0, 0)));

        assertEquals(1, store.listCharacters(accountId).size());
    }

    @Test
    void rejectsUnsupportedFutureSchemaVersion() throws Exception {
        Files.createDirectories(tempDir);
        Files.writeString(tempDir.resolve(JsonFilePlayerProfileStore.STORE_FILE_NAME), """
                {"schemaVersion":99,"nextEntityId":1001,"accountsById":{},"accountIdByDevKeyHash":{},"charactersById":{}}
                """, StandardCharsets.UTF_8);

        assertThrows(IllegalStateException.class, () -> new JsonFilePlayerProfileStore(tempDir, clock));
    }
    @Test
    void migratesV2ThroughNeutralV4WithoutChangingV2FileAndKeepsDevLogin() throws Exception {
        String devKey = "v2-dev-key";
        String hash = sha256(devKey);
        String accountId = "account.dev." + hash.substring(0, 16);
        String v2 = """
                {"schemaVersion":2,"nextEntityId":1001,
                 "accountsById":{"%s":{"accountId":"%s","devKeyHash":"%s","displayName":"Legacy","createdAtUnixMs":1700000000000,"updatedAtUnixMs":1700000000000}},
                 "accountIdByDevKeyHash":{"%s":"%s"},"charactersById":{}}
                """.formatted(accountId, accountId, hash, hash, accountId).trim();
        Path v2File = tempDir.resolve("players-v2.json");
        Files.writeString(v2File, v2, StandardCharsets.UTF_8);

        var store = new JsonFilePlayerProfileStore(tempDir, clock);

        assertEquals(v2, Files.readString(v2File, StandardCharsets.UTF_8));
        assertEquals("players-v4.json", JsonFilePlayerProfileStore.STORE_FILE_NAME);
        assertEquals(accountId, store.findAccount(accountId).orElseThrow().accountId());
        assertEquals(accountId, store.loginDev(devKey, "Ignored").account().accountId());
        String v4 = Files.readString(tempDir.resolve("players-v4.json"), StandardCharsets.UTF_8);
        assertFalse(v4.contains("devKeyHash"));
        assertTrue(v4.contains(hash));
    }

    @Test
    void productAccountCreationNeverAddsDevKeyIndex() throws Exception {
        var store = new JsonFilePlayerProfileStore(tempDir, clock);

        AccountProfile account = store.createProductAccount("minh@example.com");

        assertTrue(account.accountId().startsWith("account.product."));
        assertEquals("minh@example.com", account.displayName());
        var mapper = tools.jackson.databind.json.JsonMapper.builder().build();
        var root = mapper.readTree(tempDir.resolve(JsonFilePlayerProfileStore.STORE_FILE_NAME).toFile());
        assertEquals(0, root.path("accountIdByDevKeyHash").size());
        assertFalse(root.path("accountsById").path(account.accountId()).has("devKeyHash"));
    }

    @Test
    void rollbackDeletesOnlyEmptyProductAccounts() {
        var store = new JsonFilePlayerProfileStore(tempDir, clock);
        AccountProfile emptyProduct = store.createProductAccount("empty@example.com");
        assertTrue(store.deleteEmptyProductAccount(emptyProduct.accountId()));
        assertTrue(store.findAccount(emptyProduct.accountId()).isEmpty());

        AccountProfile occupiedProduct = store.createProductAccount("occupied@example.com");
        store.createCharacter(new CreateCharacterCommand(occupiedProduct.accountId(), "ProductHero", "class.sword"));
        assertFalse(store.deleteEmptyProductAccount(occupiedProduct.accountId()));

        AccountProfile dev = store.loginDev("dev-rollback-guard", "Dev").account();
        assertFalse(store.deleteEmptyProductAccount(dev.accountId()));
    }

    @Test
    void classCompatibilityAcceptsFiveCanonicalIdsAndLegacyAliases() {
        assertEquals("vo", CharacterClassCompatibility.toRuntimeClassId("vo"));
        assertEquals("kiem", CharacterClassCompatibility.toRuntimeClassId("kiem"));
        assertEquals("phap", CharacterClassCompatibility.toRuntimeClassId("phap"));
        assertEquals("co", CharacterClassCompatibility.toRuntimeClassId("co"));
        assertEquals("linh", CharacterClassCompatibility.toRuntimeClassId("linh"));
        assertEquals("vo", CharacterClassCompatibility.toRuntimeClassId("class.martial"));
        assertEquals("kiem", CharacterClassCompatibility.toRuntimeClassId("class.sword"));
        assertThrows(IllegalArgumentException.class,
                () -> CharacterClassCompatibility.toRuntimeClassId("class.unknown"));
    }

    @Test
    void storesCanonicalClassesWithoutRewritingLegacyAliases() {
        var store = new JsonFilePlayerProfileStore(tempDir, clock);
        String legacyAccount = store.loginDev("legacy-class", "Legacy").account().accountId();
        String canonicalAccount = store.loginDev("canonical-class", "Canonical").account().accountId();
        var legacy = store.createCharacter(new CreateCharacterCommand(legacyAccount, "LegacyHero", "class.sword"));
        var canonical = store.createCharacter(new CreateCharacterCommand(canonicalAccount, "PhapHero", "phap"));
        assertEquals("class.sword", legacy.classId());
        assertEquals("phap", canonical.classId());
    }

    @Test
    void migratesV3ToV4WithoutChangingV3BytesOrGuessingMapState() throws Exception {
        String v3 = """
                {"schemaVersion":3,"nextEntityId":1002,
                 "accountsById":{"account.dev.legacy":{"accountId":"account.dev.legacy","displayName":"Legacy","createdAtUnixMs":1,"updatedAtUnixMs":1}},
                 "accountIdByDevKeyHash":{},
                 "charactersById":{"character.legacy":{"characterId":"character.legacy","accountId":"account.dev.legacy","name":"KiemTu","classId":"class.sword","entityId":1001,"positionX":12.5,"positionY":0.5,"positionZ":-7.0,"yawDegrees":270.0,"createdAtUnixMs":1,"updatedAtUnixMs":2,"slot":1}}}
                """.trim();
        Path v3File = tempDir.resolve("players-v3.json");
        Files.writeString(v3File, v3, StandardCharsets.UTF_8);

        var store = new JsonFilePlayerProfileStore(tempDir, clock);

        assertEquals(v3, Files.readString(v3File, StandardCharsets.UTF_8));
        assertEquals("players-v4.json", JsonFilePlayerProfileStore.STORE_FILE_NAME);
        var character = store.findCharacter("character.legacy").orElseThrow();
        assertEquals("class.sword", character.classId());
        assertEquals(12.5f, character.positionX(), 0.0001f);
        assertEquals(-7.0f, character.positionZ(), 0.0001f);
        assertEquals(270.0f, character.yawDegrees(), 0.0001f);
        assertTrue(store.findRuntimeState(character.characterId()).isEmpty());
    }

    @Test
    void persistsMap01AStateIndependentlyFromLegacyPosition() {
        var store = new JsonFilePlayerProfileStore(tempDir, clock);
        String account = store.loginDev("map-state", "MapState").account().accountId();
        var character = store.createCharacter(new CreateCharacterCommand(account, "MapHero", "vo"));
        var moved = store.saveCharacterPosition(new SaveCharacterPositionCommand(
                character.characterId(), 3.25f, 0.5f, -7.75f, 270.0f));

        var state = store.saveMap01AState(new SaveMap01AStateCommand(character.characterId(), 18.5f, -1));
        var reloaded = new JsonFilePlayerProfileStore(tempDir, clock);
        var loadedCharacter = reloaded.findCharacter(character.characterId()).orElseThrow();
        var loadedState = reloaded.findRuntimeState(character.characterId()).orElseThrow();

        assertEquals("map-01a-cong-dong-lam", state.mapId());
        assertEquals(18.5f, loadedState.laneX(), 0.0001f);
        assertEquals(-1, loadedState.facing());
        assertEquals(moved.positionX(), loadedCharacter.positionX(), 0.0001f);
        assertEquals(moved.positionZ(), loadedCharacter.positionZ(), 0.0001f);
        assertEquals(moved.yawDegrees(), loadedCharacter.yawDegrees(), 0.0001f);
    }

    @Test
    void rejectsInvalidMap01AStateWithoutCorruptingCharacter() {
        var store = new JsonFilePlayerProfileStore(tempDir, clock);
        String account = store.loginDev("bad-map-state", "BadMapState").account().accountId();
        var character = store.createCharacter(new CreateCharacterCommand(account, "SafeHero", "linh"));
        assertThrows(IllegalArgumentException.class,
                () -> store.saveMap01AState(new SaveMap01AStateCommand(character.characterId(), -3.81f, 1)));
        assertThrows(IllegalArgumentException.class,
                () -> store.saveMap01AState(new SaveMap01AStateCommand(character.characterId(), 44.41f, 1)));
        assertThrows(IllegalArgumentException.class,
                () -> store.saveMap01AState(new SaveMap01AStateCommand(character.characterId(), 0f, 0)));
        assertTrue(store.findRuntimeState(character.characterId()).isEmpty());
        assertEquals("linh", store.findCharacter(character.characterId()).orElseThrow().classId());
    }

    private static String sha256(String value) throws Exception {
        var digest = java.security.MessageDigest.getInstance("SHA-256");
        return java.util.HexFormat.of().formatHex(digest.digest(value.getBytes(StandardCharsets.UTF_8)));
    }

}
