package com.linhgioi.server.api.account;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.server.api.persistence.JsonFilePlayerProfileStore;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Instant;
import java.time.ZoneOffset;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;
import org.springframework.web.server.ResponseStatusException;

class AccountCharacterControllerTest {
    @TempDir
    Path tempDir;

    @Test
    void devLoginCreateCharacterSavePositionAndRejectInvalidRequestsOverController() {
        AccountCharacterController controller = new AccountCharacterController(
                new JsonFilePlayerProfileStore(tempDir, Clock.fixed(Instant.ofEpochMilli(1_700_000_000_000L), ZoneOffset.UTC)));

        DevLoginResponse login = controller.login(new DevLoginRequest("m3-dev-key", "Minh"));
        assertTrue(login.created());
        assertEquals("Minh", login.account().displayName());
        assertTrue(login.account().accountId().startsWith("account.dev."));

        CharacterResponse created = controller.createCharacter(
                login.account().accountId(), new CreateCharacterRequest("KiemTu", "class.sword"));
        assertEquals(1001L, created.entityId());
        assertEquals("KiemTu", created.name());

        CharacterResponse moved = controller.savePosition(
                created.characterId(), new SaveCharacterPositionRequest(1.25f, 0.0f, -2.5f, 180.0f));
        assertEquals(1.25f, moved.x(), 0.0001f);
        assertEquals(-2.5f, moved.z(), 0.0001f);
        assertEquals(180.0f, moved.yawDegrees(), 0.0001f);

        CharacterResponse loaded = controller.getCharacter(created.characterId());
        assertEquals("KiemTu", loaded.name());
        assertEquals(180.0f, loaded.yawDegrees(), 0.0001f);

        assertThrows(ResponseStatusException.class,
                () -> controller.createCharacter(login.account().accountId(), new CreateCharacterRequest("No", "class.sword")));
        assertThrows(ResponseStatusException.class,
                () -> controller.createCharacter(login.account().accountId(), new CreateCharacterRequest("VoGia", "class.unknown")));
        assertThrows(ResponseStatusException.class, () -> controller.getCharacter("character.missing"));
    }
}
