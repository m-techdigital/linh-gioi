package com.linhgioi.server.api.account;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

import com.linhgioi.server.api.auth.AuthSessionRegistry;
import com.linhgioi.server.api.auth.JsonFileProductCredentialStore;
import com.linhgioi.server.api.auth.ProductAuthService;
import com.linhgioi.server.api.persistence.CreateCharacterCommand;
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

class ProductCharacterControllerTest {
    private final Clock clock = Clock.fixed(Instant.ofEpochMilli(1_700_000_000_000L), ZoneOffset.UTC);

    @TempDir
    Path tempDir;

    @Test
    void listUsesBearerAccountAndRejectsMissingOrInvalidSession() {
        Fixture fixture = fixture();

        var listed = fixture.controller.listCharacters("Bearer " + fixture.ownerToken);

        assertEquals(2, listed.size());
        assertEquals("OwnerOne", listed.get(0).name());
        assertEquals("OwnerTwo", listed.get(1).name());
        for (String header : new String[] { null, "", "Basic abc", "Bearer invalid" }) {
            ResponseStatusException failure = assertThrows(ResponseStatusException.class,
                    () -> fixture.controller.listCharacters(header));
            assertEquals(HttpStatus.UNAUTHORIZED, failure.getStatusCode());
            assertEquals("invalid or expired session", failure.getReason());
        }
    }

    @Test
    void loadOwnCharacterAndHideCrossAccountExistence() {
        Fixture fixture = fixture();

        CharacterResponse loaded = fixture.controller.getCharacter(
                "Bearer " + fixture.ownerToken, fixture.ownerCharacter.characterId());
        assertEquals("OwnerOne", loaded.name());

        ResponseStatusException crossAccount = assertThrows(ResponseStatusException.class,
                () -> fixture.controller.getCharacter("Bearer " + fixture.ownerToken,
                        fixture.otherCharacter.characterId()));
        ResponseStatusException missing = assertThrows(ResponseStatusException.class,
                () -> fixture.controller.getCharacter("Bearer " + fixture.ownerToken, "character.missing"));
        assertEquals(HttpStatus.NOT_FOUND, crossAccount.getStatusCode());
        assertEquals(HttpStatus.NOT_FOUND, missing.getStatusCode());
        assertEquals("character not found", crossAccount.getReason());
        assertEquals(crossAccount.getReason(), missing.getReason());
    }

    private Fixture fixture() {
        var players = new JsonFilePlayerProfileStore(tempDir, clock);
        var credentials = new JsonFileProductCredentialStore(tempDir, clock);
        var encoder = new BCryptPasswordEncoder(4);
        var sessions = new AuthSessionRegistry(Duration.ofHours(12));
        var auth = new ProductAuthService(credentials, players, encoder, sessions, clock);

        var owner = players.createProductAccount("owner@example.test");
        var other = players.createProductAccount("other@example.test");
        auth.provisionCredential(owner.accountId(), "owner@example.test", "Secret#123");
        auth.provisionCredential(other.accountId(), "other@example.test", "Secret#456");

        var ownerOne = players.createCharacter(new CreateCharacterCommand(
                owner.accountId(), "OwnerOne", "class.sword", 1));
        players.createCharacter(new CreateCharacterCommand(
                owner.accountId(), "OwnerTwo", "class.martial", 3));
        var otherCharacter = players.createCharacter(new CreateCharacterCommand(
                other.accountId(), "OtherHero", "class.sword", 1));
        var ownerToken = auth.login("owner@example.test", "Secret#123").accessToken();
        return new Fixture(new ProductCharacterController(auth, players), ownerToken, ownerOne, otherCharacter);
    }

    private record Fixture(
            ProductCharacterController controller,
            String ownerToken,
            com.linhgioi.server.api.persistence.CharacterProfile ownerCharacter,
            com.linhgioi.server.api.persistence.CharacterProfile otherCharacter) { }
}
