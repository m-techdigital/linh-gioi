package com.linhgioi.server.api.auth;

import com.linhgioi.server.api.persistence.AccountProfile;
import com.linhgioi.server.api.persistence.PlayerProfileStore;
import java.time.Clock;
import java.util.Locale;
import java.util.Objects;
import org.springframework.security.crypto.password.PasswordEncoder;

public final class ProductRegistrationService {
    private final ProductCredentialStore credentials;
    private final PlayerProfileStore players;
    private final PasswordEncoder passwordEncoder;
    private final Clock clock;

    public ProductRegistrationService(ProductCredentialStore credentials, PlayerProfileStore players,
            PasswordEncoder passwordEncoder, Clock clock) {
        this.credentials = Objects.requireNonNull(credentials, "credentials");
        this.players = Objects.requireNonNull(players, "players");
        this.passwordEncoder = Objects.requireNonNull(passwordEncoder, "passwordEncoder");
        this.clock = Objects.requireNonNull(clock, "clock");
    }

    public synchronized RegistrationResult register(String email, String rawPassword, boolean acceptedTerms) {
        String normalized = normalizeEmail(email);
        String password = requirePassword(rawPassword);
        if (!acceptedTerms) throw new IllegalArgumentException("terms must be accepted");
        if (credentials.findByIdentifier(normalized).isPresent()) throw new DuplicateIdentifierException();

        AccountProfile account = players.createProductAccount(normalized);
        try {
            String passwordHash = passwordEncoder.encode(password);
            credentials.create(account.accountId(), normalized, passwordHash, clock.millis());
            return new RegistrationResult(account);
        } catch (RuntimeException failure) {
            players.deleteEmptyProductAccount(account.accountId());
            if (credentials.findByIdentifier(normalized).isPresent()) throw new DuplicateIdentifierException();
            throw failure;
        }
    }

    public static String normalizeEmail(String email) {
        if (email == null || email.isBlank()) throw new IllegalArgumentException("email must not be blank");
        String normalized = email.trim().toLowerCase(Locale.ROOT);
        if (normalized.length() < 3 || normalized.length() > 254) {
            throw new IllegalArgumentException("email must be between 3 and 254 characters");
        }
        int at = normalized.indexOf('@');
        if (at <= 0 || at != normalized.lastIndexOf('@') || at >= normalized.length() - 1) {
            throw new IllegalArgumentException("email must contain one non-edge @");
        }
        return normalized;
    }

    private static String requirePassword(String rawPassword) {
        if (rawPassword == null || rawPassword.length() < 8 || rawPassword.length() > 128) {
            throw new IllegalArgumentException("password must be between 8 and 128 characters");
        }
        return rawPassword;
    }

    public record RegistrationResult(AccountProfile account) { }

    public static final class DuplicateIdentifierException extends RuntimeException {
        public DuplicateIdentifierException() { super("identifier already registered"); }
    }
}
