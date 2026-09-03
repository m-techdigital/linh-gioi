package com.linhgioi.server.api.persistence;

public record SaveCharacterPositionCommand(String characterId, float x, float y, float z, float yawDegrees) {}
