package com.linhgioi.server.api.auth;

import java.util.Objects;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;

public final class SmtpRecoveryDelivery implements RecoveryDelivery {
    private final JavaMailSender sender;
    private final String from;

    public SmtpRecoveryDelivery(JavaMailSender sender, String from) {
        this.sender = Objects.requireNonNull(sender, "sender");
        if (from == null || from.isBlank()) throw new IllegalArgumentException("recovery mail from must not be blank");
        this.from = from.trim();
    }

    @Override
    public boolean isAvailable() {
        return true;
    }

    @Override
    public void sendVerificationCode(String normalizedEmail, String code) {
        SimpleMailMessage message = new SimpleMailMessage();
        message.setFrom(from);
        message.setTo(normalizedEmail);
        message.setSubject("Linh Giới Online - Mã xác minh");
        message.setText("Mã xác minh khôi phục mật khẩu của bạn là: " + code
                + "\nMã có hiệu lực trong 10 phút. Nếu bạn không yêu cầu, hãy bỏ qua email này.");
        sender.send(message);
    }
}
