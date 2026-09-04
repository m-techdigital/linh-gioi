# M6 Local Combat Prototype Final Report v0.49.0

Final decision: `M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0`

## Source Baseline

Latest pushed `main` after `M6_SOURCE_GATE_CONSISTENCY_HOTFIX_CLOSED_v0.46.1` and `M6_COMBAT_READINESS_ACCEPTED_v0.48.0`.

## Exact Scope Implemented

- Added deterministic local-only combat state in `LocalCombatPrototypeState`.
- Wired the existing world and UI combat shell to use contract-derived Wind Slash cooldown/range/effect values.
- Extended the existing local combat smoke to prove nonzero accepted and rejected checks.

## Accepted/Rejection Cases

- accepted Wind Slash local intent: target selected, in range, cooldown ready.
- rejected no-target: `NO_TARGET`.
- rejected out-of-range: `OUT_OF_RANGE`.
- rejected cooldown: `COOLDOWN_ACTIVE`.

## Runtime Smoke Evidence

Runtime smoke marker: `M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0`.

The inherited legacy marker `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS` is preserved for older closure gates.

Local runtime closure: `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`.

## Visual Evidence Status

Visual evidence closure: `LGO_PLAYABLE_VISUAL_EVIDENCE_READY`.

Captured evidence files:

- `gate-entry.png`
- `character-hall.png`
- `world-hud.png`
- `first-playable-loop-feedback.png`

v0.49 does not import new image assets.

## Frozen Surface Audit

Unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

## Package Hygiene

Delta package excludes Unity caches, generated protocol folders, Maven output, Python caches, toolchain archives, secrets, and nested ZIPs.

## Non-Claims

non-claims: no production combat, no production art, no server-authoritative combat expansion, no DB/auth/economy/social/liveops, no inventory/loot, no enemy AI, no full MMO readiness, and no broader runtime closure.

## Next Allowed Task

Review v0.49 evidence and decide whether to open a server-authoritative combat task or a visual evidence polish task. Do not open DB/auth/economy/social/liveops from this handoff.
