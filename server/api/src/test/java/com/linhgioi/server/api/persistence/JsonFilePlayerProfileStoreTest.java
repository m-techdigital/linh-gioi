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
