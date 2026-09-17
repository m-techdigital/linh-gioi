package com.linhgioi.server.api.auth;

import com.linhgioi.server.api.persistence.PlayerProfileStore;
import java.nio.file.Path;
import java.time.Clock;
import java.time.Duration;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.mail.javamail.JavaMailSenderImpl;
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
    RecoveryDelivery recoveryDelivery(
            @Value("${linhgioi.auth.recovery.smtp.host:${LG_API_RECOVERY_SMTP_HOST:}}") String host,
            @Value("${linhgioi.auth.recovery.smtp.port:${LG_API_RECOVERY_SMTP_PORT:587}}") int port,
            @Value("${linhgioi.auth.recovery.smtp.username:${LG_API_RECOVERY_SMTP_USERNAME:}}") String username,
            @Value("${linhgioi.auth.recovery.smtp.password:${LG_API_RECOVERY_SMTP_PASSWORD:}}") String password,
            @Value("${linhgioi.auth.recovery.smtp.from:${LG_API_RECOVERY_SMTP_FROM:}}") String from,
            @Value("${linhgioi.auth.recovery.smtp.starttls:${LG_API_RECOVERY_SMTP_STARTTLS:true}}") boolean startTls) {
        if (host == null || host.isBlank() || from == null || from.isBlank()) {
            return new RecoveryDelivery() {
                @Override public boolean isAvailable() { return false; }
                @Override public void sendVerificationCode(String email, String code) {
                    throw new PasswordRecoveryService.DeliveryUnavailableException();
                }
            };
        }
        JavaMailSenderImpl sender = new JavaMailSenderImpl();
        sender.setHost(host.trim());
        sender.setPort(port);
        if (username != null && !username.isBlank()) sender.setUsername(username);
        if (password != null && !password.isBlank()) sender.setPassword(password);
        sender.getJavaMailProperties().put("mail.smtp.auth", String.valueOf(username != null && !username.isBlank()));
        sender.getJavaMailProperties().put("mail.smtp.starttls.enable", String.valueOf(startTls));
        return new SmtpRecoveryDelivery(sender, from);
    }

    @Bean
    PasswordRecoveryService passwordRecoveryService(ProductCredentialStore credentials,
            PasswordEncoder productPasswordEncoder, AuthSessionRegistry sessions, Clock productAuthClock,
            RecoveryDelivery recoveryDelivery) {
        return new PasswordRecoveryService(credentials, productPasswordEncoder, sessions, productAuthClock, recoveryDelivery);
    }

    @Bean
    ProductAuthService productAuthService(ProductCredentialStore credentials, PlayerProfileStore players,
            PasswordEncoder productPasswordEncoder, AuthSessionRegistry sessions, Clock productAuthClock) {
        return new ProductAuthService(credentials, players, productPasswordEncoder, sessions, productAuthClock);
    }
}
