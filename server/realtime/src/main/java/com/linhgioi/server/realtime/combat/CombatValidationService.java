package com.linhgioi.server.realtime.combat;

import com.google.protobuf.Message;
import com.linhgioi.protocol.v1.CombatAccepted;
import com.linhgioi.protocol.v1.CombatIntent;
import com.linhgioi.protocol.v1.CombatRejected;
import com.linhgioi.protocol.v1.CombatResult;
import com.linhgioi.protocol.v1.CombatStateSnapshot;
import com.linhgioi.protocol.v1.ErrorInfo;
import com.linhgioi.server.realtime.protocol.HandshakeProtocol;
import java.util.List;
import java.util.HashMap;
import java.util.Map;
import java.util.Objects;
import java.util.function.LongSupplier;

public final class CombatValidationService {
    public static final long DEFAULT_DUMMY_TARGET_ENTITY_ID = 2001L;
    public static final String DEFAULT_SKILL_ID = "skill.sword.wind_slash";
    public static final int DEFAULT_SKILL_COOLDOWN_MS = 6000;
    public static final int DEFAULT_PLACEHOLDER_EFFECT_AMOUNT = 12;
    public static final float DEFAULT_SKILL_RANGE_M = 4.5f;
    public static final float DEFAULT_DUMMY_TARGET_X = 2.6f;
    public static final float DEFAULT_DUMMY_TARGET_Z = 0.5f;

    private final LongSupplier clock;
    private final long targetEntityId;
    private final String skillId;
    private final int cooldownMs;
    private final Map<Long, Long> cooldownUntilByActor = new HashMap<>();

    public CombatValidationService(LongSupplier clock) {
        this(clock, DEFAULT_DUMMY_TARGET_ENTITY_ID, DEFAULT_SKILL_ID, DEFAULT_SKILL_COOLDOWN_MS);
    }

    public CombatValidationService(LongSupplier clock, long targetEntityId, String skillId, int cooldownMs) {
        this.clock = Objects.requireNonNull(clock, "clock");
        this.targetEntityId = targetEntityId;
        this.skillId = Objects.requireNonNull(skillId, "skillId");
        this.cooldownMs = cooldownMs;
    }

    public Message validate(CombatIntent intent, long sessionActorEntityId) {
        return validatePilot(intent, sessionActorEntityId).get(0);
    }

    public List<Message> validatePilot(CombatIntent intent, long sessionActorEntityId) {
        Objects.requireNonNull(intent, "intent");
        long now = clock.getAsLong();
        String rejection = rejectionCode(intent, sessionActorEntityId, now);
        if (!rejection.isEmpty()) {
            return List.of(rejected(intent, rejection, now));
        }

        cooldownUntilByActor.put(intent.getActorEntityId(), now + cooldownMs);
        CombatStateSnapshot snapshot = snapshot(intent, cooldownMs, true, now);
        CombatAccepted accepted = CombatAccepted.newBuilder()
                .setSequence(intent.getSequence())
                .setIntentId(intent.getIntentId())
                .setActorEntityId(intent.getActorEntityId())
                .setSkillId(intent.getSkillId())
                .setCooldownMs(cooldownMs)
                .setServerTimeUnixMs(now)
                .setSnapshot(snapshot)
                .build();
        CombatResult result = CombatResult.newBuilder()
                .setSequence(intent.getSequence())
                .setIntentId(intent.getIntentId())
                .setActorEntityId(intent.getActorEntityId())
                .setTargetEntityId(intent.getTargetEntityId())
                .setSkillId(intent.getSkillId())
                .setEffectAmount(DEFAULT_PLACEHOLDER_EFFECT_AMOUNT)
                .setOutcome("SERVER_AUTHORITATIVE_PLACEHOLDER_HIT")
                .setServerTimeUnixMs(now)
                .setSnapshot(snapshot)
                .build();
        return List.of(accepted, result, snapshot);
    }

    private String rejectionCode(CombatIntent intent, long sessionActorEntityId, long now) {
        if (intent.getProtocolVersion() != HandshakeProtocol.PROTOCOL_VERSION) {
            return "protocol_version";
        }
        if (intent.getSequence() == 0) {
            return "sequence";
        }
        if (intent.getIntentId().isBlank()) {
            return "intent_id";
        }
        if (intent.getActorEntityId() != sessionActorEntityId) {
            return "actor_entity_id";
        }
        if (!skillId.equals(intent.getSkillId())) {
            return "skill_id";
        }
        if (intent.getTargetEntityId() == 0L) {
            return "no_target";
        }
        if (intent.getTargetEntityId() != targetEntityId) {
            return "target_entity_id";
        }
        if (distanceToTarget(intent) > DEFAULT_SKILL_RANGE_M) {
            return "out_of_range";
        }
        long cooldownUntil = cooldownUntilByActor.getOrDefault(intent.getActorEntityId(), 0L);
        if (cooldownUntil > now) {
            return "cooldown";
        }
        return "";
    }

    private CombatRejected rejected(CombatIntent intent, String code, long now) {
        return CombatRejected.newBuilder()
                .setSequence(intent.getSequence())
                .setIntentId(intent.getIntentId())
                .setServerTimeUnixMs(now)
                .setError(ErrorInfo.newBuilder()
                        .setCode("combat_intent_rejected_" + code)
                        .setMessage("combat intent rejected: " + code)
                        .setRetryable("cooldown".equals(code) || "out_of_range".equals(code))
                        .build())
                .setSnapshot(snapshot(intent, cooldownRemainingMs(intent.getActorEntityId(), now), false, now))
                .build();
    }

    private double distanceToTarget(CombatIntent intent) {
        float dx = intent.getTargetPosition().getX() - DEFAULT_DUMMY_TARGET_X;
        float dz = intent.getTargetPosition().getZ() - DEFAULT_DUMMY_TARGET_Z;
        return Math.sqrt(dx * dx + dz * dz);
    }

    private CombatStateSnapshot snapshot(CombatIntent intent, long cooldownRemainingMs, boolean targetValid, long now) {
        return CombatStateSnapshot.newBuilder()
                .setActorEntityId(intent.getActorEntityId())
                .setTargetEntityId(intent.getTargetEntityId())
                .setActiveSkillId(intent.getSkillId())
                .setCooldownRemainingMs((int) Math.max(0L, Math.min(Integer.MAX_VALUE, cooldownRemainingMs)))
                .setTargetValid(targetValid)
                .setServerTimeUnixMs(now)
                .build();
    }

    private long cooldownRemainingMs(long actorEntityId, long now) {
        return Math.max(0L, cooldownUntilByActor.getOrDefault(actorEntityId, 0L) - now);
    }
}
