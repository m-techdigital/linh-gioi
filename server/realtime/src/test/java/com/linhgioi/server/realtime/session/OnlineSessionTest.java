package com.linhgioi.server.realtime.session;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.linhgioi.protocol.v1.MoveIntent;
import com.linhgioi.protocol.v1.PlayerTransformSnapshot;
import com.linhgioi.protocol.v1.Vec2;
import org.junit.jupiter.api.Test;

class OnlineSessionTest {
    @Test
    void appliesDeterministicMovementAndAcknowledgesSequence() {
        OnlineSession session = new OnlineSession("session-test");

        PlayerTransformSnapshot snapshot = session.applyMove(move(1, 1.0f, 0.0f, 0.1f), 1234L);

        assertEquals(OnlineSession.DEFAULT_PLAYER_ENTITY_ID, snapshot.getEntityId());
        assertEquals(1, snapshot.getAcknowledgedSequence());
        assertEquals(0.4f, snapshot.getPosition().getX(), 0.0001f);
        assertEquals(0.0f, snapshot.getPosition().getY(), 0.0001f);
        assertEquals(0.0f, snapshot.getPosition().getZ(), 0.0001f);
        assertEquals(90.0f, snapshot.getYawDegrees(), 0.0001f);
        assertEquals(1234L, snapshot.getServerTimeUnixMs());
    }

    @Test
    void repeatedOrLateSequenceIsIdempotentAndDoesNotMoveAgain() {
        OnlineSession session = new OnlineSession("session-test");
        session.applyMove(move(2, 0.0f, 1.0f, 0.2f), 1000L);

        PlayerTransformSnapshot snapshot = session.applyMove(move(1, 1.0f, 0.0f, 0.2f), 2000L);

        assertEquals(2, snapshot.getAcknowledgedSequence());
        assertEquals(0.0f, snapshot.getPosition().getX(), 0.0001f);
        assertEquals(0.8f, snapshot.getPosition().getZ(), 0.0001f);
        assertEquals(2000L, snapshot.getServerTimeUnixMs());
    }

    @Test
    void rejectsInvalidMoveIntentWithoutAdvancingSession() {
        OnlineSession session = new OnlineSession("session-test");

        assertThrows(IllegalArgumentException.class, () -> session.applyMove(move(0, 0.0f, 1.0f, 0.1f), 1L));
        assertThrows(IllegalArgumentException.class, () -> session.applyMove(move(1, 2.0f, 0.0f, 0.1f), 1L));
        assertThrows(IllegalArgumentException.class, () -> session.applyMove(move(1, 1.0f, 1.0f, 0.1f), 1L));
        assertThrows(IllegalArgumentException.class, () -> session.applyMove(move(1, Float.NaN, 0.0f, 0.1f), 1L));
        assertThrows(IllegalArgumentException.class, () -> session.applyMove(move(1, 1.0f, 0.0f, 0.5f), 1L));
        assertThrows(IllegalArgumentException.class, () -> session.applyMove(MoveIntent.newBuilder().setSequence(1).setClientDeltaSeconds(0.1f).build(), 1L));

        assertEquals(0, session.acknowledgedSequence());
        assertTrue(session.snapshot(2L).getPosition().getX() == 0.0f);
    }

    static MoveIntent move(int sequence, float x, float y, float deltaSeconds) {
        return MoveIntent.newBuilder()
                .setSequence(sequence)
                .setMoveAxis(Vec2.newBuilder().setX(x).setY(y).build())
                .setClientDeltaSeconds(deltaSeconds)
                .build();
    }
}
