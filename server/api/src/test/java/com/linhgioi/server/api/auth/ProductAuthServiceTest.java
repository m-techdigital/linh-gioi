package com.linhgioi.server.api.auth;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.server.api.persistence.JsonFilePlayerProfileStore;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Duration;
import java.time.Instant;
import java.time.ZoneId;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

class ProductAuthServiceTest {
    private static final long START_MS = 1_700_000_000_000L;

    @TempDir
    Path tempDir;

    @Test
    void provisionsBcryptCredentialAndIssuesOpaqueSessionWithoutPersistingSecrets() throws Exception {
        MutableClock clock = new MutableClock(START_MS);
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var account = players.loginDev("fixture-dev-key", "Minh").account();
        var credentials = new JsonFileProductCredentialStore(tempDir, clock);
        var sessions = new AuthSessionRegistry(Duration.ofHours(12));
        var service = new ProductAuthService(credentials, players, new BCryptPasswordEncoder(12), sessions, clock);

        var provisioned = service.provisionCredential(account.accountId(), " Minh@Example.Test ", "Secret#123");
        assertEquals("minh@example.test", provisioned.normalizedIdentifier());
        assertTrue(provisioned.passwordHash().startsWith("$2"));
        assertNotEquals("Secret#123", provisioned.passwordHash());

        var login = service.login("MINH@example.test", "Secret#123");
        assertEquals(account, login.account());
        assertEquals(43, login.accessToken().length(), "32 random bytes in base64url without padding must be 43 chars");
        assertEquals(START_MS + Duration.ofHours(12).toMillis(), login.expiresAtUnixMs());
        assertEquals(account, service.validateSession(login.accessToken()).account());

        String stored = Files.readString(tempDir.resolve(JsonFileProductCredentialStore.STORE_FILE_NAME), StandardCharsets.UTF_8);
        assertFalse(stored.contains("Secret#123"));
        assertFalse(stored.contains(login.accessToken()));
    }

    @Test
    void rejectsWrongAndUnknownCredentialsWithTheSamePublicFailure() {
        MutableClock clock = new MutableClock(START_MS);
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var account = players.loginDev("fixture-dev-key", "Minh").account();
        var service = new ProductAuthService(
                new JsonFileProductCredentialStore(tempDir, clock), players,
                new BCryptPasswordEncoder(12), new AuthSessionRegistry(Duration.ofHours(12)), clock);
        service.provisionCredential(account.accountId(), "minh@example.test", "Secret#123");

        var wrong = assertThrows(ProductAuthService.InvalidCredentialsException.class,
                () -> service.login("minh@example.test", "Wrong#123"));
        var unknown = assertThrows(ProductAuthService.InvalidCredentialsException.class,
                () -> service.login("unknown@example.test", "Wrong#123"));
        assertEquals(wrong.getMessage(), unknown.getMessage());
        assertEquals("invalid credentials", wrong.getMessage());
    }

    @Test
    void expiryAndLogoutInvalidateIssuedSession() {
        MutableClock clock = new MutableClock(START_MS);
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var account = players.loginDev("fixture-dev-key", "Minh").account();
        var service = new ProductAuthService(
                new JsonFileProductCredentialStore(tempDir, clock), players,
                new BCryptPasswordEncoder(12), new AuthSessionRegistry(Duration.ofHours(12)), clock);
        service.provisionCredential(account.accountId(), "minh@example.test", "Secret#123");

        var first = service.login("minh@example.test", "Secret#123");
        service.logout(first.accessToken());
        assertThrows(ProductAuthService.InvalidAuthSessionException.class,
                () -> service.validateSession(first.accessToken()));

        var second = service.login("minh@example.test", "Secret#123");
        clock.advance(Duration.ofHours(13));
        assertThrows(ProductAuthService.InvalidAuthSessionException.class,
                () -> service.validateSession(second.accessToken()));
        assertThrows(ProductAuthService.InvalidAuthSessionException.class,
                () -> service.logout(second.accessToken()));
    }

    private static final class MutableClock extends Clock {
        private long nowMillis;

        private MutableClock(long nowMillis) {
            this.nowMillis = nowMillis;
        }

        void advance(Duration duration) {
            nowMillis += duration.toMillis();
        }

        @Override
        public ZoneId getZone() {
            return ZoneId.of("UTC");
        }

        @Override
        public Clock withZone(ZoneId zone) {
            return this;
        }

        @Override
        public Instant instant() {
            return Instant.ofEpochMilli(nowMillis);
        }

        @Override
        public long millis() {
            return nowMillis;
        }
    }
}
