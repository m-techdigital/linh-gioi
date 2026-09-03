package com.linhgioi.server.realtime.session;

import com.linhgioi.protocol.v1.MoveIntent;
import com.linhgioi.protocol.v1.PlayerTransformSnapshot;
import com.linhgioi.protocol.v1.Vec2;
import com.linhgioi.protocol.v1.Vec3;
import java.util.Objects;

public final class OnlineSession {
    public static final long DEFAULT_PLAYER_ENTITY_ID = 1_001L;
    public static final float MOVE_SPEED_UNITS_PER_SECOND = 4.0f;
    public static final float MAX_CLIENT_DELTA_SECONDS = 0.25f;

    private final String sessionId;
    private final long entityId;
    private int acknowledgedSequence;
    private float x;
    private float y;
    private float z;
    private float yawDegrees;

    public OnlineSession(String sessionId) {
        this(sessionId, DEFAULT_PLAYER_ENTITY_ID);
    }

    public OnlineSession(String sessionId, long entityId) {
        if (sessionId == null || sessionId.isBlank()) {
            throw new IllegalArgumentException("sessionId must not be blank");
        }
        if (entityId <= 0) {
            throw new IllegalArgumentException("entityId must be positive");
        }
        this.sessionId = sessionId;
        this.entityId = entityId;
    }

    public String sessionId() {
        return sessionId;
    }

    public int acknowledgedSequence() {
        return acknowledgedSequence;
    }

    public PlayerTransformSnapshot applyMove(MoveIntent intent, long serverTimeUnixMs) {
        Objects.requireNonNull(intent, "intent");
        validateMoveIntent(intent);

        int sequence = intent.getSequence();
        if (sequence > acknowledgedSequence) {
            Vec2 axis = intent.getMoveAxis();
            float delta = intent.getClientDeltaSeconds();
            x += axis.getX() * MOVE_SPEED_UNITS_PER_SECOND * delta;
            z += axis.getY() * MOVE_SPEED_UNITS_PER_SECOND * delta;
            if (Math.abs(axis.getX()) > 0.0001f || Math.abs(axis.getY()) > 0.0001f) {
                yawDegrees = (float) Math.toDegrees(Math.atan2(axis.getX(), axis.getY()));
            }
            acknowledgedSequence = sequence;
        }

        return snapshot(serverTimeUnixMs);
    }

    public PlayerTransformSnapshot snapshot(long serverTimeUnixMs) {
        return PlayerTransformSnapshot.newBuilder()
                .setEntityId(entityId)
                .setAcknowledgedSequence(acknowledgedSequence)
                .setPosition(Vec3.newBuilder().setX(x).setY(y).setZ(z).build())
                .setYawDegrees(yawDegrees)
                .setServerTimeUnixMs(serverTimeUnixMs)
                .build();
    }

    private static void validateMoveIntent(MoveIntent intent) {
        if (intent.getSequence() == 0) {
            throw new IllegalArgumentException("MoveIntent.sequence must be positive");
        }
        if (!intent.hasMoveAxis()) {
            throw new IllegalArgumentException("MoveIntent.move_axis must be present");
        }
        float delta = intent.getClientDeltaSeconds();
        if (!Float.isFinite(delta) || delta <= 0f || delta > MAX_CLIENT_DELTA_SECONDS) {
            throw new IllegalArgumentException("MoveIntent.client_delta_seconds must be > 0 and <= " + MAX_CLIENT_DELTA_SECONDS);
        }
        Vec2 axis = intent.getMoveAxis();
        if (!Float.isFinite(axis.getX()) || !Float.isFinite(axis.getY())) {
            throw new IllegalArgumentException("MoveIntent.move_axis must contain finite values");
        }
        if (Math.abs(axis.getX()) > 1.0f || Math.abs(axis.getY()) > 1.0f) {
            throw new IllegalArgumentException("MoveIntent.move_axis values must be normalized to [-1, 1]");
        }
        double magnitude = Math.hypot(axis.getX(), axis.getY());
        if (magnitude > 1.0001d) {
            throw new IllegalArgumentException("MoveIntent.move_axis magnitude must be <= 1");
        }
    }
}
