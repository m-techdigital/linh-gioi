package com.linhgioi.server.api.auth;

import com.linhgioi.server.api.persistence.AccountProfile;
import com.linhgioi.server.api.persistence.PlayerProfileStore;
import java.time.Clock;
import java.util.Locale;
import java.util.Objects;
import org.springframework.security.crypto.password.PasswordEncoder;

public final class ProductAuthService {
    private static final String INVALID_CREDENTIALS = "invalid credentials";
    private static final String INVALID_SESSION = "invalid or expired session";

    private final ProductCredentialStore credentials;
    private final PlayerProfileStore players;
    private final PasswordEncoder passwordEncoder;
    private final AuthSessionRegistry sessions;
    private final Clock clock;
    private final String dummyPasswordHash;

    public ProductAuthService(ProductCredentialStore credentials, PlayerProfileStore players,
            PasswordEncoder passwordEncoder, AuthSessionRegistry sessions, Clock clock) {
        this.credentials = Objects.requireNonNull(credentials, "credentials");
        this.players = Objects.requireNonNull(players, "players");
        this.passwordEncoder = Objects.requireNonNull(passwordEncoder, "passwordEncoder");
        this.sessions = Objects.requireNonNull(sessions, "sessions");
        this.clock = Objects.requireNonNull(clock, "clock");
        dummyPasswordHash = passwordEncoder.encode("lgo-invalid-credential-dummy");
    }

    public ProductCredential provisionCredential(String accountId, String identifier, String rawPassword) {
        AccountProfile account = players.findAccount(accountId)
                .orElseThrow(() -> new IllegalArgumentException("account not found"));
        String normalized = normalizeIdentifier(identifier);
        String password = requirePassword(rawPassword);
        String passwordHash = passwordEncoder.encode(password);
        return credentials.create(account.accountId(), normalized, passwordHash, clock.millis());
    }

    public LoginResult login(String identifier, String rawPassword) {
        String normalized = normalizeIdentifier(identifier);
        String password = requirePassword(rawPassword);
        ProductCredential credential = credentials.findByIdentifier(normalized).orElse(null);
        String hashToCheck = credential == null ? dummyPasswordHash : credential.passwordHash();
        boolean passwordMatches = passwordEncoder.matches(password, hashToCheck);
        if (credential == null || !passwordMatches) throw new InvalidCredentialsException();

        AccountProfile account = players.findAccount(credential.accountId())
                .orElseThrow(() -> new IllegalStateException("credential references missing account"));
        AuthSessionRegistry.IssuedSession issued = sessions.issue(account.accountId(), clock.millis());
        return new LoginResult(account, issued.accessToken(), issued.expiresAtUnixMs());
    }

    public SessionResult validateSession(String rawToken) {
        AuthSessionRegistry.AuthSession session = sessions.validate(rawToken, clock.millis())
                .orElseThrow(InvalidAuthSessionException::new);
        AccountProfile account = players.findAccount(session.accountId())
                .orElseThrow(() -> new IllegalStateException("session references missing account"));
        return new SessionResult(account, session.expiresAtUnixMs());
    }

    public void logout(String rawToken) {
        validateSession(rawToken);
        sessions.invalidate(rawToken);
    }

    public static String normalizeIdentifier(String identifier) {
        if (identifier == null || identifier.isBlank()) {
            throw new IllegalArgumentException("identifier must not be blank");
        }
        String normalized = identifier.trim().toLowerCase(Locale.ROOT);
        if (normalized.length() < 3 || normalized.length() > 254) {
            throw new IllegalArgumentException("identifier must be between 3 and 254 characters");
        }
        return normalized;
    }

    private static String requirePassword(String rawPassword) {
        if (rawPassword == null || rawPassword.length() < 8 || rawPassword.length() > 128) {
            throw new IllegalArgumentException("password must be between 8 and 128 characters");
        }
        return rawPassword;
    }

    public record LoginResult(AccountProfile account, String accessToken, long expiresAtUnixMs) { }
    public record SessionResult(AccountProfile account, long expiresAtUnixMs) { }

    public static final class InvalidCredentialsException extends RuntimeException {
        public InvalidCredentialsException() { super(INVALID_CREDENTIALS); }
    }

    public static final class InvalidAuthSessionException extends RuntimeException {
        public InvalidAuthSessionException() { super(INVALID_SESSION); }
    }
}
