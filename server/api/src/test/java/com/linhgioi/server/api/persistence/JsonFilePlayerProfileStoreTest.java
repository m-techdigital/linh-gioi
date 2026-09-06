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
}
