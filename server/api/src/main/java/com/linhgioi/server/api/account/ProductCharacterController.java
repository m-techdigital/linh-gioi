package com.linhgioi.server.api.account;

import com.linhgioi.server.api.auth.ProductAuthService;
import com.linhgioi.server.api.persistence.PlayerProfileStore;
import com.linhgioi.server.api.persistence.SaveMap01AStateCommand;
import java.util.List;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

@RestController
public final class ProductCharacterController {
    private final ProductAuthService auth;
    private final PlayerProfileStore store;

    public ProductCharacterController(ProductAuthService auth, PlayerProfileStore store) {
        this.auth = auth;
        this.store = store;
    }

    @GetMapping(path = "/auth/characters", produces = MediaType.APPLICATION_JSON_VALUE)
    public List<CharacterResponse> listCharacters(
            @RequestHeader(value = "Authorization", required = false) String authorization) {
        String accountId = authenticatedAccountId(authorization);
        return store.listCharacters(accountId).stream().map(this::productResponse).toList();
    }

    @GetMapping(path = "/auth/characters/{characterId}", produces = MediaType.APPLICATION_JSON_VALUE)
    public CharacterResponse getCharacter(
            @RequestHeader(value = "Authorization", required = false) String authorization,
            @PathVariable String characterId) {
        String accountId = authenticatedAccountId(authorization);
        var character = store.findCharacter(characterId).orElseThrow(ProductCharacterController::characterNotFound);
        if (!accountId.equals(character.accountId())) throw characterNotFound();
        return productResponse(character);
    }

    @PostMapping(path = "/auth/characters/{characterId}/map01a-state",
            consumes = MediaType.APPLICATION_JSON_VALUE, produces = MediaType.APPLICATION_JSON_VALUE)
    public CharacterResponse saveMap01AState(
            @RequestHeader(value = "Authorization", required = false) String authorization,
            @PathVariable String characterId, @RequestBody SaveMap01AStateRequest request) {
        String accountId = authenticatedAccountId(authorization);
        var character = store.findCharacter(characterId).orElseThrow(ProductCharacterController::characterNotFound);
        if (!accountId.equals(character.accountId())) throw characterNotFound();
        if (request == null) throw invalidMapState();
        try {
            var state = store.saveMap01AState(new SaveMap01AStateCommand(
                    character.characterId(), request.laneX(), request.facing()));
            return CharacterResponse.from(character, state);
        } catch (IllegalArgumentException exception) {
            throw invalidMapState();
        }
    }

    private CharacterResponse productResponse(com.linhgioi.server.api.persistence.CharacterProfile character) {
        return CharacterResponse.from(character, store.findRuntimeState(character.characterId()).orElse(null));
    }

    private String authenticatedAccountId(String authorization) {
        String token = bearerToken(authorization);
        try {
            return auth.validateSession(token).account().accountId();
        } catch (ProductAuthService.InvalidAuthSessionException exception) {
            throw unauthorizedSession();
        }
    }

    private static String bearerToken(String authorization) {
        if (authorization == null || authorization.length() <= 7
                || !authorization.regionMatches(true, 0, "Bearer ", 0, 7)) {
            throw unauthorizedSession();
        }
        String token = authorization.substring(7).trim();
        if (token.isEmpty()) throw unauthorizedSession();
        return token;
    }

    private static ResponseStatusException unauthorizedSession() {
        return new ResponseStatusException(HttpStatus.UNAUTHORIZED, "invalid or expired session");
    }

    private static ResponseStatusException characterNotFound() {
        return new ResponseStatusException(HttpStatus.NOT_FOUND, "character not found");
    }

    private static ResponseStatusException invalidMapState() {
        return new ResponseStatusException(HttpStatus.BAD_REQUEST, "invalid Map01A runtime state");
    }
}
