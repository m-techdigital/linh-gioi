package com.linhgioi.server.api.auth;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.time.Clock;
import java.time.Duration;
import java.util.Base64;
import java.util.HashMap;
import java.util.HexFormat;
import java.util.Locale;
import java.util.Map;
import java.util.Objects;
import java.util.UUID;
import org.springframework.security.crypto.password.PasswordEncoder;

public final class PasswordRecoveryService {
    private static final long CHALLENGE_TTL_MS = Duration.ofMinutes(10).toMillis();
    private static final long RESET_TTL_MS = Duration.ofMinutes(10).toMillis();
    private static final long RESEND_COOLDOWN_MS = Duration.ofSeconds(60).toMillis();
    private static final int MAX_ATTEMPTS = 5;
    private static final String INVALID = "invalid or expired recovery grant";

    private final ProductCredentialStore credentials;
    private final PasswordEncoder passwordEncoder;
    private final AuthSessionRegistry sessions;
    private final Clock clock;
    private final RecoveryDelivery delivery;
    private final SecureRandom random = new SecureRandom();
    private final Map<String, Challenge> challengesById = new HashMap<>();
    private final Map<String, String> activeChallengeIdByEmail = new HashMap<>();
    private final Map<String, ResetGrant> resetGrantsByTokenHash = new HashMap<>();

    public PasswordRecoveryService(ProductCredentialStore credentials, PasswordEncoder passwordEncoder,
            AuthSessionRegistry sessions, Clock clock, RecoveryDelivery delivery) {
        this.credentials = Objects.requireNonNull(credentials, "credentials");
        this.passwordEncoder = Objects.requireNonNull(passwordEncoder, "passwordEncoder");
        this.sessions = Objects.requireNonNull(sessions, "sessions");
        this.clock = Objects.requireNonNull(clock, "clock");
        this.delivery = Objects.requireNonNull(delivery, "delivery");
    }

    public synchronized RequestResult request(String email) {
        if (!delivery.isAvailable()) throw new DeliveryUnavailableException();
        String normalized = ProductRegistrationService.normalizeEmail(email);
        long now = clock.millis();
        String activeId = activeChallengeIdByEmail.get(normalized);
        Challenge active = activeId == null ? null : challengesById.get(activeId);
        if (active != null && now < active.resendAvailableAtUnixMs) {
            throw new RecoveryCooldownException(active.resendAvailableAtUnixMs);
        }
        if (active != null) removeChallenge(active);

        ProductCredential credential = credentials.findByIdentifier(normalized).orElse(null);
        String accountId = credential == null ? null : credential.accountId();
        String code = String.format(Locale.ROOT, "%06d", random.nextInt(1_000_000));
        String codeHash = passwordEncoder.encode(code);
        String challengeId = "recovery." + UUID.randomUUID().toString().replace("-", "");
        long expiresAt = Math.addExact(now, CHALLENGE_TTL_MS);
        long resendAt = Math.addExact(now, RESEND_COOLDOWN_MS);
        Challenge challenge = new Challenge(challengeId, normalized, accountId, codeHash, expiresAt, resendAt);
        if (credential != null) {
            try {
                delivery.sendVerificationCode(normalized, code);
            } catch (RuntimeException failure) {
                throw new DeliveryUnavailableException(failure);
            }
        }
        challengesById.put(challengeId, challenge);
        activeChallengeIdByEmail.put(normalized, challengeId);
        return new RequestResult(challengeId, expiresAt, resendAt);
    }

    public synchronized VerifyResult verify(String challengeId, String code) {
        if (challengeId == null || challengeId.isBlank() || code == null || !code.matches("\\d{6}")) {
            throw new InvalidRecoveryException();
        }
        Challenge challenge = challengesById.get(challengeId);
        long now = clock.millis();
        if (challenge == null || challenge.expiresAtUnixMs <= now || challenge.attempts >= MAX_ATTEMPTS) {
            if (challenge != null) removeChallenge(challenge);
            throw new InvalidRecoveryException();
        }
        boolean matches = passwordEncoder.matches(code, challenge.codeHash);
        if (!matches || challenge.accountId == null) {
            challenge.attempts++;
            if (challenge.attempts >= MAX_ATTEMPTS) removeChallenge(challenge);
            throw new InvalidRecoveryException();
        }

        removeChallenge(challenge);
        byte[] bytes = new byte[32];
        random.nextBytes(bytes);
        String token = Base64.getUrlEncoder().withoutPadding().encodeToString(bytes);
        long expiresAt = Math.addExact(now, RESET_TTL_MS);
        resetGrantsByTokenHash.put(sha256(token), new ResetGrant(challenge.accountId, challenge.email, expiresAt));
        return new VerifyResult(token, expiresAt);
    }

    public synchronized void reset(String resetToken, String rawPassword) {
        String password = requirePassword(rawPassword);
        if (resetToken == null || resetToken.isBlank()) throw new InvalidRecoveryException();
        String tokenHash = sha256(resetToken);
        ResetGrant grant = resetGrantsByTokenHash.get(tokenHash);
        long now = clock.millis();
        if (grant == null || grant.expiresAtUnixMs <= now) {
            if (grant != null) resetGrantsByTokenHash.remove(tokenHash);
            throw new InvalidRecoveryException();
        }
        if (credentials.findByIdentifier(grant.email).isEmpty()) throw new InvalidRecoveryException();
        String passwordHash = passwordEncoder.encode(password);
        credentials.replacePasswordHash(grant.email, passwordHash, now);
        resetGrantsByTokenHash.entrySet().removeIf(entry -> grant.accountId.equals(entry.getValue().accountId));
        challengesById.values().stream()
                .filter(challenge -> grant.accountId.equals(challenge.accountId))
                .toList().forEach(this::removeChallenge);
        sessions.invalidateAccount(grant.accountId);
    }

    private void removeChallenge(Challenge challenge) {
        challengesById.remove(challenge.id);
        activeChallengeIdByEmail.remove(challenge.email, challenge.id);
    }

    private static String requirePassword(String rawPassword) {
        if (rawPassword == null || rawPassword.length() < 8 || rawPassword.length() > 128) {
            throw new IllegalArgumentException("password must be between 8 and 128 characters");
        }
        return rawPassword;
    }

    private static String sha256(String value) {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            return HexFormat.of().formatHex(digest.digest(value.getBytes(StandardCharsets.UTF_8)));
        } catch (NoSuchAlgorithmException exception) {
            throw new IllegalStateException("SHA-256 unavailable", exception);
        }
    }

    public record RequestResult(String challengeId, long expiresAtUnixMs, long resendAvailableAtUnixMs) { }
    public record VerifyResult(String resetToken, long expiresAtUnixMs) { }

    private static final class Challenge {
        final String id;
        final String email;
        final String accountId;
        final String codeHash;
        final long expiresAtUnixMs;
        final long resendAvailableAtUnixMs;
        int attempts;
        Challenge(String id, String email, String accountId, String codeHash, long expiresAt, long resendAt) {
            this.id = id;
            this.email = email;
            this.accountId = accountId;
            this.codeHash = codeHash;
            this.expiresAtUnixMs = expiresAt;
            this.resendAvailableAtUnixMs = resendAt;
        }
        @Override public String toString() {
            return "Challenge[id=" + id + ", email=" + email + ", codeHash=" + codeHash
                    + ", expiresAt=" + expiresAtUnixMs + ", attempts=" + attempts + "]";
        }
    }

    private record ResetGrant(String accountId, String email, long expiresAtUnixMs) { }

    public static final class InvalidRecoveryException extends RuntimeException {
        public InvalidRecoveryException() { super(INVALID); }
    }
    public static final class RecoveryCooldownException extends RuntimeException {
        private final long retryAtUnixMs;
        public RecoveryCooldownException(long retryAtUnixMs) {
            super("recovery resend cooldown active");
            this.retryAtUnixMs = retryAtUnixMs;
        }
        public long retryAtUnixMs() { return retryAtUnixMs; }
    }
    public static final class DeliveryUnavailableException extends RuntimeException {
        public DeliveryUnavailableException() { super("recovery delivery unavailable"); }
        public DeliveryUnavailableException(Throwable cause) { super("recovery delivery unavailable", cause); }
    }
}
