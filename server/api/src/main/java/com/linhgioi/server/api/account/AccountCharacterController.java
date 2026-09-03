package com.linhgioi.server.api.account;

import com.linhgioi.server.api.persistence.CreateCharacterCommand;
import com.linhgioi.server.api.persistence.DevLoginResult;
import com.linhgioi.server.api.persistence.PlayerProfileStore;
import com.linhgioi.server.api.persistence.SaveCharacterPositionCommand;
import java.util.List;
import java.util.NoSuchElementException;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

@RestController
public class AccountCharacterController {
    private final PlayerProfileStore store;

    public AccountCharacterController(PlayerProfileStore store) {
        this.store = store;
    }

    @PostMapping(path = "/dev/auth/login", consumes = MediaType.APPLICATION_JSON_VALUE, produces = MediaType.APPLICATION_JSON_VALUE)
    public DevLoginResponse login(@RequestBody DevLoginRequest request) {
        try {
            DevLoginResult result = store.loginDev(request.devKey(), request.displayName());
            List<CharacterResponse> characters = store.listCharacters(result.account().accountId()).stream()
                    .map(CharacterResponse::from)
                    .toList();
            return DevLoginResponse.from(result, characters);
        } catch (IllegalArgumentException exception) {
            throw badRequest(exception);
        }
    }

    @GetMapping(path = "/accounts/{accountId}/characters", produces = MediaType.APPLICATION_JSON_VALUE)
    public List<CharacterResponse> listCharacters(@PathVariable String accountId) {
        try {
            return store.listCharacters(accountId).stream().map(CharacterResponse::from).toList();
        } catch (NoSuchElementException exception) {
            throw notFound(exception);
        }
    }

    @PostMapping(path = "/accounts/{accountId}/characters", consumes = MediaType.APPLICATION_JSON_VALUE, produces = MediaType.APPLICATION_JSON_VALUE)
    @ResponseStatus(HttpStatus.CREATED)
    public CharacterResponse createCharacter(@PathVariable String accountId, @RequestBody CreateCharacterRequest request) {
        try {
            return CharacterResponse.from(store.createCharacter(new CreateCharacterCommand(accountId, request.name(), request.classId())));
        } catch (IllegalArgumentException exception) {
            throw badRequest(exception);
        } catch (NoSuchElementException exception) {
            throw notFound(exception);
        }
    }

    @GetMapping(path = "/characters/{characterId}", produces = MediaType.APPLICATION_JSON_VALUE)
    public CharacterResponse getCharacter(@PathVariable String characterId) {
        return CharacterResponse.from(store.findCharacter(characterId).orElseThrow(() -> notFound("character not found")));
    }

    @PostMapping(path = "/characters/{characterId}/position", consumes = MediaType.APPLICATION_JSON_VALUE, produces = MediaType.APPLICATION_JSON_VALUE)
    public CharacterResponse savePosition(@PathVariable String characterId, @RequestBody SaveCharacterPositionRequest request) {
        try {
            return CharacterResponse.from(store.saveCharacterPosition(new SaveCharacterPositionCommand(
                    characterId, request.x(), request.y(), request.z(), request.yawDegrees())));
        } catch (IllegalArgumentException exception) {
            throw badRequest(exception);
        } catch (NoSuchElementException exception) {
            throw notFound(exception);
        }
    }

    private static ResponseStatusException badRequest(IllegalArgumentException exception) {
        return new ResponseStatusException(HttpStatus.BAD_REQUEST, exception.getMessage(), exception);
    }

    private static ResponseStatusException notFound(NoSuchElementException exception) {
        return new ResponseStatusException(HttpStatus.NOT_FOUND, exception.getMessage(), exception);
    }

    private static ResponseStatusException notFound(String message) {
        return new ResponseStatusException(HttpStatus.NOT_FOUND, message);
    }
}
