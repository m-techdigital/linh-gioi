# M6 Local Combat Runtime Closure Checklist v0.50.0

Decision: `M6_LOCAL_COMBAT_RUNTIME_CLOSED_LOCAL_v0.50.0`

## Baseline

- [x] v0.49 decision consumed: `M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0`
- [x] Baseline commit recorded: `f5fed7e`
- [x] Baseline tag recorded: `lgo-m6-local-combat-prototype-v0.49.0`

## Runtime Cases

- [x] Accepted local Wind Slash intent.
- [x] Rejected no-target case: `NO_TARGET`.
- [x] Rejected out-of-range case: `OUT_OF_RANGE`.
- [x] Rejected cooldown-active case: `COOLDOWN_ACTIVE`.
- [x] Cooldown recovery returns the skill to ready state.
- [x] Runtime smoke uses nonzero checks.

## Evidence

- [x] Runtime command log captured outside source.
- [x] Local combat smoke output captured.
- [x] Source-only output captured.
- [x] Package-ready output captured.
- [x] Visual evidence output captured where the local environment allows it.
- [x] SHA256 summary created.

## Frozen Surface Audit

- [x] `protocol/**` unchanged.
- [x] `gamedata/schemas/**` unchanged.
- [x] `docs/adr/**` unchanged.
- [x] `client/Unity/Assets/Game/UI/design-tokens.json` unchanged.

## Non-Claims

- [x] No production combat claim.
- [x] No production art claim.
- [x] No server-authoritative expansion claim.
- [x] No DB/auth/economy/social/liveops/inventory/loot/enemy AI claim.
