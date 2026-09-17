package com.linhgioi.server.api.auth;

import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

@RestController
public final class ProductAuthController {
    private final ProductAuthService auth;
    private final ProductRegistrationService registration;
    private final PasswordRecoveryService recovery;

    public ProductAuthController(ProductAuthService auth, ProductRegistrationService registration,
            PasswordRecoveryService recovery) {
        this.auth = auth;
        this.registration = registration;
        this.recovery = recovery;
    }

    @PostMapping(path = "/auth/register", consumes = MediaType.APPLICATION_JSON_VALUE,
            produces = MediaType.APPLICATION_JSON_VALUE)
    @ResponseStatus(HttpStatus.CREATED)
    public ProductRegisterResponse register(@RequestBody ProductRegisterRequest request) {
        if (request == null) throw badRegistrationRequest();
        try {
            return ProductRegisterResponse.from(
                    registration.register(request.email(), request.password(), request.acceptedTerms()));
        } catch (ProductRegistrationService.DuplicateIdentifierException exception) {
            throw registrationConflict();
        } catch (IllegalArgumentException exception) {
            throw badRegistrationRequest();
        }
    }

    @PostMapping(path = "/auth/recovery/request", consumes = MediaType.APPLICATION_JSON_VALUE,
            produces = MediaType.APPLICATION_JSON_VALUE)
    @ResponseStatus(HttpStatus.ACCEPTED)
    public RecoveryRequestResponse requestRecovery(@RequestBody RecoveryRequest request) {
        if (request == null) throw badRecoveryRequest();
        try {
            return RecoveryRequestResponse.from(recovery.request(request.email()));
        } catch (PasswordRecoveryService.RecoveryCooldownException exception) {
            throw recoveryRateLimited();
        } catch (PasswordRecoveryService.DeliveryUnavailableException exception) {
            throw recoveryUnavailable();
        } catch (IllegalArgumentException exception) {
            throw badRecoveryRequest();
        }
    }

    @PostMapping(path = "/auth/recovery/verify", consumes = MediaType.APPLICATION_JSON_VALUE,
            produces = MediaType.APPLICATION_JSON_VALUE)
    public RecoveryVerifyResponse verifyRecovery(@RequestBody RecoveryVerifyRequest request) {
        if (request == null) throw badRecoveryRequest();
        try {
            return RecoveryVerifyResponse.from(recovery.verify(request.challengeId(), request.code()));
        } catch (PasswordRecoveryService.InvalidRecoveryException exception) {
            throw invalidRecoveryGrant();
        }
    }

    @PostMapping(path = "/auth/recovery/reset", consumes = MediaType.APPLICATION_JSON_VALUE)
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void resetRecovery(@RequestBody RecoveryResetRequest request) {
        if (request == null) throw badRecoveryRequest();
        try {
            recovery.reset(request.resetToken(), request.newPassword());
        } catch (PasswordRecoveryService.InvalidRecoveryException exception) {
            throw invalidRecoveryGrant();
        } catch (IllegalArgumentException exception) {
            throw badRecoveryRequest();
        }
    }

    @PostMapping(path = "/auth/login", consumes = MediaType.APPLICATION_JSON_VALUE,
            produces = MediaType.APPLICATION_JSON_VALUE)
    public ProductLoginResponse login(@RequestBody ProductLoginRequest request) {
        if (request == null) throw badRequest();
        try {
            return ProductLoginResponse.from(auth.login(request.identifier(), request.password()));
        } catch (ProductAuthService.InvalidCredentialsException exception) {
            throw unauthorizedCredentials();
        } catch (IllegalArgumentException exception) {
            throw badRequest();
        }
    }

    @GetMapping(path = "/auth/session", produces = MediaType.APPLICATION_JSON_VALUE)
    public ProductSessionResponse session(
            @RequestHeader(value = "Authorization", required = false) String authorization) {
        String token = bearerToken(authorization);
        try {
            return ProductSessionResponse.from(auth.validateSession(token));
        } catch (ProductAuthService.InvalidAuthSessionException exception) {
            throw unauthorizedSession();
        }
    }

    @PostMapping(path = "/auth/logout")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void logout(@RequestHeader(value = "Authorization", required = false) String authorization) {
        String token = bearerToken(authorization);
        try {
            auth.logout(token);
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

    private static ResponseStatusException badRegistrationRequest() {
        return new ResponseStatusException(HttpStatus.BAD_REQUEST, "invalid registration request");
    }

    private static ResponseStatusException registrationConflict() {
        return new ResponseStatusException(HttpStatus.CONFLICT, "identifier already registered");
    }

    private static ResponseStatusException badRecoveryRequest() {
        return new ResponseStatusException(HttpStatus.BAD_REQUEST, "invalid recovery request");
    }

    private static ResponseStatusException recoveryRateLimited() {
        return new ResponseStatusException(HttpStatus.TOO_MANY_REQUESTS, "recovery request rate limited");
    }

    private static ResponseStatusException recoveryUnavailable() {
        return new ResponseStatusException(HttpStatus.SERVICE_UNAVAILABLE, "recovery service unavailable");
    }

    private static ResponseStatusException invalidRecoveryGrant() {
        return new ResponseStatusException(HttpStatus.UNAUTHORIZED, "invalid or expired recovery grant");
    }

    private static ResponseStatusException badRequest() {
        return new ResponseStatusException(HttpStatus.BAD_REQUEST, "invalid login request");
    }

    private static ResponseStatusException unauthorizedCredentials() {
        return new ResponseStatusException(HttpStatus.UNAUTHORIZED, "invalid credentials");
    }

    private static ResponseStatusException unauthorizedSession() {
        return new ResponseStatusException(HttpStatus.UNAUTHORIZED, "invalid or expired session");
    }
}
