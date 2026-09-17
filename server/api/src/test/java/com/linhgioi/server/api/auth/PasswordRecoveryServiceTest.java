package com.linhgioi.server.api.auth;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.server.api.persistence.JsonFilePlayerProfileStore;
import java.lang.reflect.Field;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Duration;
import java.time.Instant;
import java.time.ZoneId;
import java.time.ZoneOffset;
import java.util.ArrayList;
import java.util.List;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

class PasswordRecoveryServiceTest {
    @TempDir Path tempDir;

    @Test
    void requestIsEnumerationSafeAndStoresOnlyHashedCode() throws Exception {
        Fixture fixture = fixture();
        fixture.registration.register("minh@example.com", "Secret#123", true);

        var existing = fixture.recovery.request(" MINH@example.com ");
        var unknown = fixture.recovery.request("unknown@example.com");

        assertFalse(existing.challengeId().isBlank());
        assertFalse(unknown.challengeId().isBlank());
        assertEquals(existing.expiresAtUnixMs() - fixture.clock.millis(), unknown.expiresAtUnixMs() - fixture.clock.millis());
        assertEquals(existing.resendAvailableAtUnixMs() - fixture.clock.millis(), unknown.resendAvailableAtUnixMs() - fixture.clock.millis());
        assertEquals(1, fixture.delivery.messages.size());
        String code = fixture.delivery.messages.getFirst().code;
        assertTrue(code.matches("\\d{6}"));
        assertFalse(privateMap(fixture.recovery, "challengesById").contains(code));
    }

    @Test
    void cooldownBlocksParallelChallengesAndAllowedResendReplacesOldChallenge() {
        Fixture fixture = fixture();
        fixture.registration.register("minh@example.com", "Secret#123", true);
        var first = fixture.recovery.request("minh@example.com");
        String firstCode = fixture.delivery.messages.getFirst().code;
        assertThrows(PasswordRecoveryService.RecoveryCooldownException.class,
                () -> fixture.recovery.request("minh@example.com"));

        fixture.clock.advance(Duration.ofSeconds(61));
        var second = fixture.recovery.request("minh@example.com");
        assertNotEquals(first.challengeId(), second.challengeId());
        assertThrows(PasswordRecoveryService.InvalidRecoveryException.class,
                () -> fixture.recovery.verify(first.challengeId(), firstCode));
    }

    @Test
    void verificationEnforcesFiveAttemptsAndTenMinuteExpiry() {
        Fixture fixture = fixture();
        fixture.registration.register("minh@example.com", "Secret#123", true);
        var request = fixture.recovery.request("minh@example.com");
        String code = fixture.delivery.messages.getFirst().code;
        for (int i = 0; i < 5; i++) {
            var error = assertThrows(PasswordRecoveryService.InvalidRecoveryException.class,
                    () -> fixture.recovery.verify(request.challengeId(), "000000".equals(code) ? "111111" : "000000"));
            assertEquals("invalid or expired recovery grant", error.getMessage());
        }
        assertThrows(PasswordRecoveryService.InvalidRecoveryException.class,
                () -> fixture.recovery.verify(request.challengeId(), code));

        fixture.clock.advance(Duration.ofSeconds(61));
        var expiring = fixture.recovery.request("minh@example.com");
        String freshCode = fixture.delivery.messages.getLast().code;
        fixture.clock.advance(Duration.ofMinutes(10).plusSeconds(1));
        assertThrows(PasswordRecoveryService.InvalidRecoveryException.class,
                () -> fixture.recovery.verify(expiring.challengeId(), freshCode));
    }

    @Test
    void resetTokenIsOneTimeChangesPasswordAndInvalidatesExistingSessions() throws Exception {
        Fixture fixture = fixture();
        var registration = fixture.registration.register("minh@example.com", "Secret#123", true);
        var auth = new ProductAuthService(fixture.credentials, fixture.players, fixture.encoder, fixture.sessions, fixture.clock);
        var login = auth.login("minh@example.com", "Secret#123");

        var request = fixture.recovery.request("minh@example.com");
        String code = fixture.delivery.messages.getFirst().code;
        var verified = fixture.recovery.verify(request.challengeId(), code);
        assertFalse(verified.resetToken().isBlank());
        assertFalse(privateMap(fixture.recovery, "resetGrantsByTokenHash").contains(verified.resetToken()));

        fixture.recovery.reset(verified.resetToken(), "Changed#123");
        assertThrows(ProductAuthService.InvalidCredentialsException.class,
                () -> auth.login("minh@example.com", "Secret#123"));
        assertEquals(registration.account().accountId(), auth.login("minh@example.com", "Changed#123").account().accountId());
        assertThrows(ProductAuthService.InvalidAuthSessionException.class,
                () -> auth.validateSession(login.accessToken()));
        assertThrows(PasswordRecoveryService.InvalidRecoveryException.class,
                () -> fixture.recovery.reset(verified.resetToken(), "Again#123"));
    }

    private Fixture fixture() {
        MutableClock clock = new MutableClock(1_700_000_000_000L);
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var credentials = new JsonFileProductCredentialStore(tempDir, clock);
        var encoder = new BCryptPasswordEncoder(4);
        var sessions = new AuthSessionRegistry(Duration.ofHours(12));
        var delivery = new FakeDelivery();
        var registration = new ProductRegistrationService(credentials, players, encoder, clock);
        var recovery = new PasswordRecoveryService(credentials, encoder, sessions, clock, delivery);
        return new Fixture(clock, players, credentials, encoder, sessions, delivery, registration, recovery);
    }

    private static String privateMap(PasswordRecoveryService service, String fieldName) throws Exception {
        Field field = PasswordRecoveryService.class.getDeclaredField(fieldName);
        field.setAccessible(true);
        return String.valueOf(field.get(service));
    }

    private record Fixture(MutableClock clock, JsonFilePlayerProfileStore players,
            JsonFileProductCredentialStore credentials, BCryptPasswordEncoder encoder,
            AuthSessionRegistry sessions, FakeDelivery delivery,
            ProductRegistrationService registration, PasswordRecoveryService recovery) { }

    private static final class FakeDelivery implements RecoveryDelivery {
        final List<Message> messages = new ArrayList<>();
        @Override public boolean isAvailable() { return true; }
        @Override public void sendVerificationCode(String email, String code) { messages.add(new Message(email, code)); }
        private record Message(String email, String code) { }
    }

    private static final class MutableClock extends Clock {
        private long millis;
        MutableClock(long millis) { this.millis = millis; }
        void advance(Duration duration) { millis += duration.toMillis(); }
        @Override public ZoneId getZone() { return ZoneOffset.UTC; }
        @Override public Clock withZone(ZoneId zone) { return this; }
        @Override public Instant instant() { return Instant.ofEpochMilli(millis); }
        @Override public long millis() { return millis; }
    }
}
