package com.linhgioi.server.realtime.combat;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertInstanceOf;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.google.protobuf.Message;
import com.linhgioi.protocol.v1.CombatAccepted;
import com.linhgioi.protocol.v1.CombatIntent;
import com.linhgioi.protocol.v1.CombatRejected;
import com.linhgioi.protocol.v1.CombatResult;
import com.linhgioi.protocol.v1.CombatStateSnapshot;
import com.linhgioi.protocol.v1.Vec3;
import com.linhgioi.server.realtime.session.OnlineSession;
import java.util.List;
import java.util.concurrent.atomic.AtomicLong;
import org.junit.jupiter.api.Test;

class CombatValidationServiceTest {
    @Test
    void validIntentIsAcceptedWithCooldownSnapshot() {
        AtomicLong now = new AtomicLong(1000L);
        CombatValidationService service = new CombatValidationService(now::get);

        Message response = service.validate(validIntent(1), OnlineSession.DEFAULT_PLAYER_ENTITY_ID);

        CombatAccepted accepted = assertInstanceOf(CombatAccepted.class, response);
        assertEquals(1, accepted.getSequence());
        assertEquals("intent-1", accepted.getIntentId());
        assertEquals("skill.sword.wind_slash", accepted.getSkillId());
        assertEquals(6000, accepted.getCooldownMs());
        assertTrue(accepted.getSnapshot().getTargetValid());
    }

    @Test
    void validPilotIntentEmitsAcceptedResultAndSnapshot() {
        AtomicLong now = new AtomicLong(1000L);
        CombatValidationService service = new CombatValidationService(now::get);

        List<Message> responses = service.validatePilot(validIntent(10), OnlineSession.DEFAULT_PLAYER_ENTITY_ID);

        assertEquals(3, responses.size());
        CombatAccepted accepted = assertInstanceOf(CombatAccepted.class, responses.get(0));
        CombatResult result = assertInstanceOf(CombatResult.class, responses.get(1));
        CombatStateSnapshot snapshot = assertInstanceOf(CombatStateSnapshot.class, responses.get(2));
        assertEquals("intent-10", accepted.getIntentId());
        assertEquals("SERVER_AUTHORITATIVE_PLACEHOLDER_HIT", result.getOutcome());
        assertEquals(CombatValidationService.DEFAULT_PLACEHOLDER_EFFECT_AMOUNT, result.getEffectAmount());
        assertEquals(CombatValidationService.DEFAULT_SKILL_COOLDOWN_MS, snapshot.getCooldownRemainingMs());
        assertTrue(snapshot.getTargetValid());
    }

    @Test
    void noTargetIsRejected() {
        AtomicLong now = new AtomicLong(1000L);
        CombatValidationService service = new CombatValidationService(now::get);

        CombatIntent invalid = validIntent(11).toBuilder().setTargetEntityId(0L).build();
        Message response = service.validate(invalid, OnlineSession.DEFAULT_PLAYER_ENTITY_ID);

        CombatRejected rejected = assertInstanceOf(CombatRejected.class, response);
        assertEquals("combat_intent_rejected_no_target", rejected.getError().getCode());
        assertTrue(!rejected.getSnapshot().getTargetValid());
    }

    @Test
    void invalidTargetIsRejected() {
        AtomicLong now = new AtomicLong(1000L);
        CombatValidationService service = new CombatValidationService(now::get);

        CombatIntent invalid = validIntent(2).toBuilder().setTargetEntityId(9999L).build();
        Message response = service.validate(invalid, OnlineSession.DEFAULT_PLAYER_ENTITY_ID);

        CombatRejected rejected = assertInstanceOf(CombatRejected.class, response);
        assertEquals("combat_intent_rejected_target_entity_id", rejected.getError().getCode());
        assertTrue(!rejected.getSnapshot().getTargetValid());
    }

    @Test
    void unknownSkillIsRejected() {
        AtomicLong now = new AtomicLong(1000L);
        CombatValidationService service = new CombatValidationService(now::get);

        CombatIntent invalid = validIntent(12).toBuilder().setSkillId("skill.unknown").build();
        Message response = service.validate(invalid, OnlineSession.DEFAULT_PLAYER_ENTITY_ID);

        CombatRejected rejected = assertInstanceOf(CombatRejected.class, response);
        assertEquals("combat_intent_rejected_skill_id", rejected.getError().getCode());
    }

    @Test
    void outOfRangeIsRejected() {
        AtomicLong now = new AtomicLong(1000L);
        CombatValidationService service = new CombatValidationService(now::get);

        CombatIntent invalid = validIntent(13).toBuilder()
                .setTargetPosition(Vec3.newBuilder().setX(-10f).setY(0.25f).setZ(0.5f).build())
                .build();
        Message response = service.validate(invalid, OnlineSession.DEFAULT_PLAYER_ENTITY_ID);

        CombatRejected rejected = assertInstanceOf(CombatRejected.class, response);
        assertEquals("combat_intent_rejected_out_of_range", rejected.getError().getCode());
        assertTrue(rejected.getError().getRetryable());
    }

    @Test
    void cooldownBlocksThenRecovers() {
        AtomicLong now = new AtomicLong(1000L);
        CombatValidationService service = new CombatValidationService(now::get);

        assertInstanceOf(CombatAccepted.class, service.validate(validIntent(3), OnlineSession.DEFAULT_PLAYER_ENTITY_ID));
        Message blocked = service.validate(validIntent(4), OnlineSession.DEFAULT_PLAYER_ENTITY_ID);
        CombatRejected rejected = assertInstanceOf(CombatRejected.class, blocked);
        assertEquals("combat_intent_rejected_cooldown", rejected.getError().getCode());
        assertTrue(rejected.getError().getRetryable());
        assertTrue(rejected.getSnapshot().getCooldownRemainingMs() > 0);

        now.addAndGet(6000L);
        assertInstanceOf(CombatAccepted.class, service.validate(validIntent(5), OnlineSession.DEFAULT_PLAYER_ENTITY_ID));
    }

    private static CombatIntent validIntent(int sequence) {
        return CombatIntent.newBuilder()
                .setProtocolVersion(1)
                .setSequence(sequence)
                .setIntentId("intent-" + sequence)
                .setActorEntityId(OnlineSession.DEFAULT_PLAYER_ENTITY_ID)
                .setTargetEntityId(CombatValidationService.DEFAULT_DUMMY_TARGET_ENTITY_ID)
                .setSkillId(CombatValidationService.DEFAULT_SKILL_ID)
                .setTargetPosition(Vec3.newBuilder()
                        .setX(CombatValidationService.DEFAULT_DUMMY_TARGET_X - 0.9f)
                        .setY(0.25f)
                        .setZ(CombatValidationService.DEFAULT_DUMMY_TARGET_Z)
                        .build())
                .setClientTimeUnixMs(100L + sequence)
                .build();
    }
}
