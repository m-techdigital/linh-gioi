package com.linhgioi.server.api.auth;

import java.util.Optional;

public interface ProductCredentialStore {
    ProductCredential create(String accountId, String normalizedIdentifier, String passwordHash, long nowUnixMs);
    Optional<ProductCredential> findByIdentifier(String normalizedIdentifier);
    boolean deleteByIdentifier(String normalizedIdentifier);
}
