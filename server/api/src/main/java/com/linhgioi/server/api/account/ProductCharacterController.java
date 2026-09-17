package com.linhgioi.server.api.account;

import com.linhgioi.server.api.auth.ProductAuthService;
import com.linhgioi.server.api.persistence.PlayerProfileStore;
import java.util.List;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
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
        return store.listCharacters(accountId).stream().map(CharacterResponse::from).toList();
    }

    @GetMapping(path = "/auth/characters/{characterId}", produces = MediaType.APPLICATION_JSON_VALUE)
    public CharacterResponse getCharacter(
            @RequestHeader(value = "Authorization", required = false) String authorization,
            @PathVariable String characterId) {
        String accountId = authenticatedAccountId(authorization);
        var character = store.findCharacter(characterId).orElseThrow(ProductCharacterController::characterNotFound);
        if (!accountId.equals(character.accountId())) throw characterNotFound();
        return CharacterResponse.from(character);
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
}
