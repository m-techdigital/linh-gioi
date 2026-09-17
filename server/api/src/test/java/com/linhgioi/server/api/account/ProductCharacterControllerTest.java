package com.linhgioi.server.api.account;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertNull;

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
    void productReadsExposeCanonicalRuntimeClassAndOptionalRuntimeState() {
        Fixture fixture = fixture();

        var listed = fixture.controller.listCharacters("Bearer " + fixture.ownerToken);

        assertEquals("class.sword", listed.get(0).classId());
        assertEquals("kiem", listed.get(0).runtimeClassId());
        assertNull(listed.get(0).runtimeState());
        assertEquals("class.martial", listed.get(1).classId());
        assertEquals("vo", listed.get(1).runtimeClassId());
        assertNull(listed.get(1).runtimeState());
    }

    @Test
    void bearerOwnerCanSaveAndReloadMap01AState() {
        Fixture fixture = fixture();

        CharacterResponse saved = fixture.controller.saveMap01AState(
                "Bearer " + fixture.ownerToken, fixture.ownerCharacter.characterId(),
                new SaveMap01AStateRequest(18.5f, -1));
        CharacterResponse loaded = fixture.controller.getCharacter(
                "Bearer " + fixture.ownerToken, fixture.ownerCharacter.characterId());

        assertEquals("kiem", saved.runtimeClassId());
        assertEquals("map-01a-cong-dong-lam", saved.runtimeState().mapId());
        assertEquals(18.5f, saved.runtimeState().laneX(), 0.0001f);
        assertEquals(-1, saved.runtimeState().facing());
        assertEquals(saved.runtimeState(), loaded.runtimeState());
    }

    @Test
    void mapStateSaveHidesCrossAccountExistenceAndRejectsInvalidSession() {
        Fixture fixture = fixture();

        ResponseStatusException crossAccount = assertThrows(ResponseStatusException.class, () ->
                fixture.controller.saveMap01AState("Bearer " + fixture.ownerToken,
                        fixture.otherCharacter.characterId(), new SaveMap01AStateRequest(0f, 1)));
        ResponseStatusException missing = assertThrows(ResponseStatusException.class, () ->
                fixture.controller.saveMap01AState("Bearer " + fixture.ownerToken,
                        "character.missing", new SaveMap01AStateRequest(0f, 1)));
        ResponseStatusException unauthorized = assertThrows(ResponseStatusException.class, () ->
                fixture.controller.saveMap01AState("Bearer invalid", fixture.ownerCharacter.characterId(),
                        new SaveMap01AStateRequest(0f, 1)));

        assertEquals(HttpStatus.NOT_FOUND, crossAccount.getStatusCode());
        assertEquals(crossAccount.getReason(), missing.getReason());
        assertEquals("character not found", crossAccount.getReason());
        assertEquals(HttpStatus.UNAUTHORIZED, unauthorized.getStatusCode());
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
