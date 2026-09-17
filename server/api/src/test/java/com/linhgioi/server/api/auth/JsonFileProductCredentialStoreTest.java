package com.linhgioi.server.api.auth;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.server.api.persistence.JsonFilePlayerProfileStore;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Instant;
import java.time.ZoneOffset;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

class JsonFileProductCredentialStoreTest {
    private final Clock clock = Clock.fixed(Instant.ofEpochMilli(1_700_000_000_000L), ZoneOffset.UTC);

    @TempDir
    Path tempDir;

    @Test
    void persistsCredentialSeparatelyAndFindsAuthoritativeAccount() throws Exception {
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var account = players.loginDev("auth-fixture-key", "Minh").account();
        assertEquals(account, players.findAccount(account.accountId()).orElseThrow());
        var store = new JsonFileProductCredentialStore(tempDir, clock);
        var created = store.create(account.accountId(), "minh@example.test", "$2a$12$fixture-hash", clock.millis());

        assertEquals(account.accountId(), created.accountId());
        assertEquals("minh@example.test", created.normalizedIdentifier());
        assertEquals(created, store.findByIdentifier("minh@example.test").orElseThrow());

        var reloaded = new JsonFileProductCredentialStore(tempDir, clock);
        assertEquals(created, reloaded.findByIdentifier("minh@example.test").orElseThrow());
        assertFalse(reloaded.findByIdentifier("unknown@example.test").isPresent());

        String stored = Files.readString(tempDir.resolve(JsonFileProductCredentialStore.STORE_FILE_NAME), StandardCharsets.UTF_8);
        assertTrue(stored.contains("$2a$12$fixture-hash"));
        assertFalse(stored.contains("PlainSecret#1"));
        assertFalse(stored.contains("auth-fixture-key"));
    }

    @Test
    void rejectsDuplicateIdentifierWithoutChangingOriginalCredential() {
        var store = new JsonFileProductCredentialStore(tempDir, clock);
        var original = store.create("account.dev.aaaaaaaaaaaaaaaa", "minh@example.test", "$2a$12$first", clock.millis());

        assertThrows(IllegalArgumentException.class,
                () -> store.create("account.dev.bbbbbbbbbbbbbbbb", "minh@example.test", "$2a$12$second", clock.millis()));
        assertEquals(original, store.findByIdentifier("minh@example.test").orElseThrow());
    }
}
