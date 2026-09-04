package com.linhgioi.server.realtime.combat;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertInstanceOf;
import static org.junit.jupiter.api.Assertions.assertTrue;

import com.google.protobuf.Message;
import com.linhgioi.protocol.v1.CombatAccepted;
import com.linhgioi.protocol.v1.CombatIntent;
import com.linhgioi.protocol.v1.CombatRejected;
import com.linhgioi.server.realtime.session.OnlineSession;
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
                .setClientTimeUnixMs(100L + sequence)
                .build();
    }
}
