package com.linhgioi.server.api.health;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.URI;
import java.nio.charset.StandardCharsets;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.test.context.SpringBootTest;
import com.linhgioi.server.api.bootstrap.ApiApplication;

@SpringBootTest(classes = ApiApplication.class, webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
class HealthEndpointIntegrationTest {
    @Value("${local.server.port}")
    private int port;

    @Test
    void getHealthReturnsRuntimeSuccess() throws Exception {
        HttpURLConnection connection = (HttpURLConnection) URI.create("http://127.0.0.1:" + port + "/health")
                .toURL()
                .openConnection();
        connection.setConnectTimeout(3000);
        connection.setReadTimeout(5000);
        connection.setRequestMethod("GET");

        try {
            int status = connection.getResponseCode();
            String body = readBody(connection);

            assertEquals(200, status);
            assertTrue(body.contains("\"status\":\"UP\""));
            assertTrue(body.contains("\"service\":\"api\""));
        } finally {
            connection.disconnect();
        }
    }

    private static String readBody(HttpURLConnection connection) throws IOException {
        try (var stream = connection.getInputStream()) {
            return new String(stream.readAllBytes(), StandardCharsets.UTF_8);
        }
    }
}
