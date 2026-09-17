package com.linhgioi.server.api.auth;

import com.linhgioi.server.api.persistence.PlayerProfileStore;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Duration;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;

@Configuration
public class ProductAuthConfiguration {
    @Bean
    Clock productAuthClock() {
        return Clock.systemUTC();
    }

    @Bean
    ProductCredentialStore productCredentialStore(
            @Value("${linhgioi.persistence.dir:${LG_API_PERSISTENCE_DIR:build/local-persistence/api}}") String directory,
            Clock productAuthClock) {
        return new JsonFileProductCredentialStore(Path.of(directory), productAuthClock);
    }

    @Bean
    PasswordEncoder productPasswordEncoder() {
        return new BCryptPasswordEncoder(12);
    }

    @Bean
    AuthSessionRegistry authSessionRegistry(
            @Value("${linhgioi.auth.session-ttl-seconds:${LG_API_AUTH_SESSION_TTL_SECONDS:43200}}") long ttlSeconds) {
        if (ttlSeconds < 1) throw new IllegalArgumentException("session ttl seconds must be positive");
        return new AuthSessionRegistry(Duration.ofSeconds(ttlSeconds));
    }

    @Bean
    ProductRegistrationService productRegistrationService(ProductCredentialStore credentials, PlayerProfileStore players,
            PasswordEncoder productPasswordEncoder, Clock productAuthClock) {
        return new ProductRegistrationService(credentials, players, productPasswordEncoder, productAuthClock);
    }

    @Bean
    ProductAuthService productAuthService(ProductCredentialStore credentials, PlayerProfileStore players,
            PasswordEncoder productPasswordEncoder, AuthSessionRegistry sessions, Clock productAuthClock) {
        return new ProductAuthService(credentials, players, productPasswordEncoder, sessions, productAuthClock);
    }
}
