package com.linhgioi.server.api.persistence;

import java.nio.file.Path;
import java.time.Clock;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class PersistenceConfiguration {
    @Bean
    PlayerProfileStore playerProfileStore(@Value("${linhgioi.persistence.dir:${LG_API_PERSISTENCE_DIR:build/local-persistence/api}}") String directory) {
        return new JsonFilePlayerProfileStore(Path.of(directory), Clock.systemUTC());
    }
}
