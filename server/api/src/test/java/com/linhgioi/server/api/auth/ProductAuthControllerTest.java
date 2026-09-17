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

    private ProductAuthController fixtureController() {
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var account = players.loginDev("fixture-dev-key", "Minh").account();
        var service = new ProductAuthService(
                new JsonFileProductCredentialStore(tempDir, clock), players,
                new BCryptPasswordEncoder(4), new AuthSessionRegistry(Duration.ofHours(12)), clock);
        service.provisionCredential(account.accountId(), "minh@example.test", "Secret#123");
        return new ProductAuthController(service);
    }
}
