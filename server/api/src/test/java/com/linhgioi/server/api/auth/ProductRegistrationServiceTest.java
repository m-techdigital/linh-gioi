package com.linhgioi.server.api.auth;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.server.api.persistence.JsonFilePlayerProfileStore;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Instant;
import java.time.ZoneOffset;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

class ProductRegistrationServiceTest {
    private final Clock clock = Clock.fixed(Instant.ofEpochMilli(1_700_000_000_000L), ZoneOffset.UTC);

    @TempDir Path tempDir;

    @Test
    void normalizesEmailCreatesNeutralAccountAndBcryptCredential() {
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var credentials = new JsonFileProductCredentialStore(tempDir, clock);
        var encoder = new BCryptPasswordEncoder(4);
        var service = new ProductRegistrationService(credentials, players, encoder, clock);

        var result = service.register("  Minh@Example.COM ", "Secret#123", true);

        assertEquals("minh@example.com", result.account().displayName());
        assertTrue(result.account().accountId().startsWith("account.product."));
        var credential = credentials.findByIdentifier("minh@example.com").orElseThrow();
        assertEquals(result.account().accountId(), credential.accountId());
        assertTrue(encoder.matches("Secret#123", credential.passwordHash()));
        assertFalse(credential.passwordHash().contains("Secret#123"));
    }

    @Test
    void rejectsInvalidEmailPasswordAndTerms() {
        var service = service(new JsonFileProductCredentialStore(tempDir, clock), new JsonFilePlayerProfileStore(tempDir, clock));
        for (String email : new String[] { "a", "missing-at.example", "@example.com", "local@" })
            assertThrows(IllegalArgumentException.class, () -> service.register(email, "Secret#123", true));
        assertThrows(IllegalArgumentException.class, () -> service.register("ok@example.com", "short", true));
        assertThrows(IllegalArgumentException.class, () -> service.register("ok@example.com", "Secret#123", false));
    }

    @Test
    void duplicateEmailIsGenericAndCredentialFailureRollsBackAccount() throws Exception {
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var credentials = new JsonFileProductCredentialStore(tempDir, clock);
        var service = service(credentials, players);
        service.register("dup@example.com", "Secret#123", true);
        assertThrows(ProductRegistrationService.DuplicateIdentifierException.class,
                () -> service.register(" DUP@example.com ", "Other#123", true));

        Path rollbackDir = tempDir.resolve("rollback");
        var rollbackPlayers = new JsonFilePlayerProfileStore(rollbackDir, clock);
        ProductCredentialStore failing = new ProductCredentialStore() {
            @Override public ProductCredential create(String accountId, String id, String hash, long now) {
                throw new IllegalStateException("fixture credential failure");
            }
            @Override public java.util.Optional<ProductCredential> findByIdentifier(String id) { return java.util.Optional.empty(); }
            @Override public boolean deleteByIdentifier(String id) { return false; }
        };
        var rollbackService = service(failing, rollbackPlayers);
        assertThrows(IllegalStateException.class,
                () -> rollbackService.register("rollback@example.com", "Secret#123", true));
        var root = tools.jackson.databind.json.JsonMapper.builder().build()
                .readTree(rollbackDir.resolve(JsonFilePlayerProfileStore.STORE_FILE_NAME).toFile());
        assertEquals(0, root.path("accountsById").size());
    }

    private ProductRegistrationService service(ProductCredentialStore credentials, JsonFilePlayerProfileStore players) {
        return new ProductRegistrationService(credentials, players, new BCryptPasswordEncoder(4), clock);
    }
}
