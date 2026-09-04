# M6 Combat UX Feedback Polish v0.35.0

Decision: `M6_COMBAT_UX_FEEDBACK_POLISH_SOURCE_READY_v0.35.0`

Polishes readability for the local-only target dummy loop:

- clearer target dummy area label
- clearer local hit flash feedback
- local cooldown indicator remains prototype/non-authoritative
- Vietnamese help text remains visible
- no production art claim
- no real balancing
- no multiple enemies
- no loot/reward
- no XP/level
- no inventory/economy/DB/server-authoritative combat
- no protocol/schema drift

Runtime marker inherited from Stage 2: `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS`.

## Code Quality / Duplication / Ownership Audit

PASS: The polish reuses existing local combat, HUD, and smoke marker surfaces. No new gameplay system or duplicated state machine is added.

## Frozen Surface Audit

PASS: Frozen surfaces remain unchanged.
