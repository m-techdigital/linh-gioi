package com.linhgioi.server.api.auth;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;

import com.linhgioi.server.api.persistence.JsonFilePlayerProfileStore;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Duration;
import java.time.Instant;
import java.time.ZoneOffset;
import java.util.ArrayList;
import java.util.List;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;
import org.springframework.http.HttpStatus;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.web.server.ResponseStatusException;

class ProductAuthControllerTest {
    private final Clock clock = Clock.fixed(Instant.ofEpochMilli(1_700_000_000_000L), ZoneOffset.UTC);

    @TempDir
    Path tempDir;

    @Test
    void loginSessionAndLogoutUseBearerContract() {
        ProductAuthController controller = fixtureController();

        ProductLoginResponse login = controller.login(new ProductLoginRequest("minh@example.test", "Secret#123"));
        assertEquals("Minh", login.account().displayName());
        assertFalse(login.accessToken().isBlank());

        ProductSessionResponse session = controller.session("Bearer " + login.accessToken());
        assertEquals(login.account(), session.account());
        assertEquals(login.expiresAtUnixMs(), session.expiresAtUnixMs());

        controller.logout("Bearer " + login.accessToken());
        ResponseStatusException expired = assertThrows(ResponseStatusException.class,
                () -> controller.session("Bearer " + login.accessToken()));
        assertEquals(HttpStatus.UNAUTHORIZED, expired.getStatusCode());
    }

    @Test
    void wrongAndUnknownCredentialsReturnTheSameUnauthorizedResponse() {
        ProductAuthController controller = fixtureController();

        ResponseStatusException wrong = assertThrows(ResponseStatusException.class,
                () -> controller.login(new ProductLoginRequest("minh@example.test", "Wrong#123")));
        ResponseStatusException unknown = assertThrows(ResponseStatusException.class,
                () -> controller.login(new ProductLoginRequest("unknown@example.test", "Wrong#123")));

        assertEquals(HttpStatus.UNAUTHORIZED, wrong.getStatusCode());
        assertEquals(HttpStatus.UNAUTHORIZED, unknown.getStatusCode());
        assertEquals(wrong.getReason(), unknown.getReason());
    }

    @Test
    void malformedCredentialsAndBearerAreRejectedWithoutTokenEcho() {
        ProductAuthController controller = fixtureController();

        ResponseStatusException malformed = assertThrows(ResponseStatusException.class,
                () -> controller.login(new ProductLoginRequest("", "short")));
        assertEquals(HttpStatus.BAD_REQUEST, malformed.getStatusCode());

        for (String header : new String[] { null, "", "Basic abc", "Bearer " }) {
            ResponseStatusException failure = assertThrows(ResponseStatusException.class,
                    () -> controller.session(header));
            assertEquals(HttpStatus.UNAUTHORIZED, failure.getStatusCode());
            if (header != null && !header.isBlank()) {
                assertFalse(String.valueOf(failure.getReason()).contains(header));
            }
        }
    }


    @Test
    void registerReturnsAccountOnlyAndMapsDuplicateToConflict() throws Exception {
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var credentials = new JsonFileProductCredentialStore(tempDir, clock);
        var encoder = new BCryptPasswordEncoder(4);
        var auth = new ProductAuthService(credentials, players, encoder,
                new AuthSessionRegistry(Duration.ofHours(12)), clock);
        var registration = new ProductRegistrationService(credentials, players, encoder, clock);
        var controller = new ProductAuthController(auth, registration,
                new PasswordRecoveryService(credentials, encoder, new AuthSessionRegistry(Duration.ofHours(12)),
                        clock, new FakeRecoveryDelivery(true)));

        ProductRegisterResponse created = controller.register(
                new ProductRegisterRequest("Minh@Example.COM", "Secret#123", true));
        assertEquals("minh@example.com", created.account().displayName());
        assertFalse(created.toString().contains("Secret#123"));
        var method = ProductAuthController.class.getMethod("register", ProductRegisterRequest.class);
        assertEquals(HttpStatus.CREATED, method.getAnnotation(org.springframework.web.bind.annotation.ResponseStatus.class).value());

        ResponseStatusException duplicate = assertThrows(ResponseStatusException.class,
                () -> controller.register(new ProductRegisterRequest("minh@example.com", "Other#123", true)));
        assertEquals(HttpStatus.CONFLICT, duplicate.getStatusCode());
        assertEquals("identifier already registered", duplicate.getReason());
    }

    @Test
    void recoveryHttpContractIsGenericRateLimitedAndSecretSafe() throws Exception {
        RecoveryFixture fixture = recoveryFixture(true);
        fixture.registration.register("minh@example.com", "Secret#123", true);
        RecoveryRequestResponse existing = fixture.controller.requestRecovery(new RecoveryRequest("minh@example.com"));
        RecoveryRequestResponse unknown = fixture.controller.requestRecovery(new RecoveryRequest("unknown@example.com"));
        assertFalse(existing.challengeId().isBlank());
        assertFalse(unknown.challengeId().isBlank());
        assertEquals(1, fixture.delivery.messages.size());
        assertEquals(HttpStatus.ACCEPTED, ProductAuthController.class.getMethod("requestRecovery", RecoveryRequest.class)
                .getAnnotation(org.springframework.web.bind.annotation.ResponseStatus.class).value());
        ResponseStatusException cooldown = assertThrows(ResponseStatusException.class,
                () -> fixture.controller.requestRecovery(new RecoveryRequest("minh@example.com")));
        assertEquals(HttpStatus.TOO_MANY_REQUESTS, cooldown.getStatusCode());
        String code = fixture.delivery.messages.getFirst().code;
        RecoveryVerifyResponse verified = fixture.controller.verifyRecovery(
                new RecoveryVerifyRequest(existing.challengeId(), code));
        assertFalse(verified.resetToken().isBlank());
        assertFalse(String.valueOf(verified).contains(code));
        fixture.controller.resetRecovery(new RecoveryResetRequest(verified.resetToken(), "Changed#123"));
        assertEquals(HttpStatus.NO_CONTENT, ProductAuthController.class.getMethod("resetRecovery", RecoveryResetRequest.class)
                .getAnnotation(org.springframework.web.bind.annotation.ResponseStatus.class).value());
        ResponseStatusException invalid = assertThrows(ResponseStatusException.class,
                () -> fixture.controller.verifyRecovery(new RecoveryVerifyRequest("missing", "000000")));
        assertEquals(HttpStatus.UNAUTHORIZED, invalid.getStatusCode());
        assertEquals("invalid or expired recovery grant", invalid.getReason());
        assertFalse(String.valueOf(invalid.getReason()).contains("000000"));
    }

    @Test
    void recoveryDeliveryUnavailableReturns503ForAnyEmail() {
        RecoveryFixture fixture = recoveryFixture(false);
        for (String email : new String[] { "known@example.com", "unknown@example.com" }) {
            ResponseStatusException failure = assertThrows(ResponseStatusException.class,
                    () -> fixture.controller.requestRecovery(new RecoveryRequest(email)));
            assertEquals(HttpStatus.SERVICE_UNAVAILABLE, failure.getStatusCode());
            assertEquals("recovery service unavailable", failure.getReason());
        }
    }

    private ProductAuthController fixtureController() {
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var account = players.loginDev("fixture-dev-key", "Minh").account();
        var credentials = new JsonFileProductCredentialStore(tempDir, clock);
        var encoder = new BCryptPasswordEncoder(4);
        var service = new ProductAuthService(
                credentials, players, encoder, new AuthSessionRegistry(Duration.ofHours(12)), clock);
        service.provisionCredential(account.accountId(), "minh@example.test", "Secret#123");
        var registration = new ProductRegistrationService(credentials, players, encoder, clock);
        var recovery = new PasswordRecoveryService(credentials, encoder,
                new AuthSessionRegistry(Duration.ofHours(12)), clock, new FakeRecoveryDelivery(true));
        return new ProductAuthController(service, registration, recovery);
    }

    private RecoveryFixture recoveryFixture(boolean available) {
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var credentials = new JsonFileProductCredentialStore(tempDir, clock);
        var encoder = new BCryptPasswordEncoder(4);
        var sessions = new AuthSessionRegistry(Duration.ofHours(12));
        var auth = new ProductAuthService(credentials, players, encoder, sessions, clock);
        var registration = new ProductRegistrationService(credentials, players, encoder, clock);
        var delivery = new FakeRecoveryDelivery(available);
        var recovery = new PasswordRecoveryService(credentials, encoder, sessions, clock, delivery);
        return new RecoveryFixture(new ProductAuthController(auth, registration, recovery), registration, delivery);
    }

    private record RecoveryFixture(ProductAuthController controller,
            ProductRegistrationService registration, FakeRecoveryDelivery delivery) { }

    private static final class FakeRecoveryDelivery implements RecoveryDelivery {
        private final boolean available;
        private final List<Message> messages = new ArrayList<>();
        private FakeRecoveryDelivery(boolean available) { this.available = available; }
        @Override public boolean isAvailable() { return available; }
        @Override public void sendVerificationCode(String email, String code) { messages.add(new Message(email, code)); }
        private record Message(String email, String code) { }
    }
}
