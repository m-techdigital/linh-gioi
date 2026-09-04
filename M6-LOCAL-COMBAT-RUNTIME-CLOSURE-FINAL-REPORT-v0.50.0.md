# M6 Local Combat Runtime Closure Final Report v0.50.0

Final decision: `M6_LOCAL_COMBAT_RUNTIME_CLOSED_LOCAL_v0.50.0`

## Baseline

- Baseline commit: `f5fed7e`.
- Baseline tag: `lgo-m6-local-combat-prototype-v0.49.0`.
- Consumed v0.49 decision: `M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0`.

## v0.49 Evidence Consumed

- Local combat smoke marker: `M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0`.
- Legacy marker preserved: `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS`.
- Runtime closure evidence from v0.49 established Unity player build and local combat smoke PASS on current source.

## Fixes

No new combat behavior was added. v0.50 only hardens closure validation, runtime marker placement, evidence packaging, and package hygiene cleanup.

## Accepted Cases

| Case | Evidence |
|---|---|
| Accepted Wind Slash | target selected, range valid, cooldown ready, effect amount `12`, target HP/readiness `120 -> 108` |
| Cooldown recovery | after recovery, Wind Slash is accepted again |

## Rejected Cases

| Case | Code |
|---|---|
| No target | `NO_TARGET` |
| Out of range | `OUT_OF_RANGE` |
| Cooldown active | `COOLDOWN_ACTIVE` |

## Runtime Gate Table

| Gate | Status |
|---|---|
| Source-only closure | PASS |
| Package-ready closure | PASS |
| Unity player build/runtime closure | PASS |
| Local combat smoke | PASS |
| Visual evidence closure | PASS |

Runtime marker after true gates: `M6_LOCAL_COMBAT_RUNTIME_CLOSURE_PASS_v0.50.0`.

## Visual Evidence Status

`LGO_PLAYABLE_VISUAL_EVIDENCE_READY`

Captured screenshot evidence includes Gate Entry, Character Hall, World HUD, and First Playable Loop Feedback. It remains human-review evidence and is not production art.

## Source / Package Hygiene

Package hygiene is strict: Unity caches, generated protocol output, Maven targets, Python caches, nested ZIPs, and build output are excluded from source packages.

## Frozen Surface Audit

Unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

## Non-Claims

No production combat, production art, server-authoritative combat expansion, enemy AI, loot/reward, inventory, economy, DB/auth, social, live ops, full MMO readiness, protocol mutation, GameData schema mutation, ADR change, or design token change is claimed.

## Next Allowed Task

`M6-SERVER-AUTHORITATIVE-COMBAT-PILOT-v0.51.0`, existing contract only, after independent review accepts this v0.50 closure package.
