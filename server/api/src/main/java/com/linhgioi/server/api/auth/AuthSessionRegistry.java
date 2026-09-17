package com.linhgioi.server.api.auth;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.time.Duration;
import java.util.Base64;
import java.util.HexFormat;
import java.util.Optional;
import java.util.concurrent.ConcurrentHashMap;

public final class AuthSessionRegistry {
    private static final int TOKEN_BYTES = 32;
    private final long ttlMillis;
    private final SecureRandom random = new SecureRandom();
    private final ConcurrentHashMap<String, AuthSession> sessionsByTokenHash = new ConcurrentHashMap<>();

    public AuthSessionRegistry(Duration ttl) {
        if (ttl == null || ttl.isZero() || ttl.isNegative()) {
            throw new IllegalArgumentException("session ttl must be positive");
        }
        ttlMillis = ttl.toMillis();
    }

    public IssuedSession issue(String accountId, long nowUnixMs) {
        if (accountId == null || accountId.isBlank()) throw new IllegalArgumentException("accountId must not be blank");
        byte[] bytes = new byte[TOKEN_BYTES];
        random.nextBytes(bytes);
        String token = Base64.getUrlEncoder().withoutPadding().encodeToString(bytes);
        long expiresAt = Math.addExact(nowUnixMs, ttlMillis);
        sessionsByTokenHash.put(tokenHash(token), new AuthSession(accountId, nowUnixMs, expiresAt));
        return new IssuedSession(token, expiresAt);
    }

    public Optional<AuthSession> validate(String rawToken, long nowUnixMs) {
        if (rawToken == null || rawToken.isBlank()) return Optional.empty();
        String hash = tokenHash(rawToken);
        AuthSession session = sessionsByTokenHash.get(hash);
        if (session == null) return Optional.empty();
        if (session.expiresAtUnixMs() <= nowUnixMs) {
            sessionsByTokenHash.remove(hash, session);
            return Optional.empty();
        }
        return Optional.of(session);
    }

    public boolean invalidate(String rawToken) {
        if (rawToken == null || rawToken.isBlank()) return false;
        return sessionsByTokenHash.remove(tokenHash(rawToken)) != null;
    }

    public int invalidateAccount(String accountId) {
        if (accountId == null || accountId.isBlank()) return 0;
        int[] removed = { 0 };
        sessionsByTokenHash.forEach((hash, session) -> {
            if (accountId.equals(session.accountId()) && sessionsByTokenHash.remove(hash, session)) removed[0]++;
        });
        return removed[0];
    }

    private static String tokenHash(String token) {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            return HexFormat.of().formatHex(digest.digest(token.getBytes(StandardCharsets.UTF_8)));
        } catch (NoSuchAlgorithmException exception) {
            throw new IllegalStateException("SHA-256 unavailable", exception);
        }
    }

    public record AuthSession(String accountId, long createdAtUnixMs, long expiresAtUnixMs) { }

    public record IssuedSession(String accessToken, long expiresAtUnixMs) { }
}
