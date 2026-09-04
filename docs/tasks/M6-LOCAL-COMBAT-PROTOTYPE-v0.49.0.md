# M6 Local Combat Prototype v0.49.0

Status: `M6_LOCAL_COMBAT_PROTOTYPE_CLOSED_LOCAL_v0.49.0`.

This task implements the next allowed local-only prototype after v0.48 acceptance.

## Goal

Turn the current local-only target dummy feedback into a deterministic local combat prototype using existing `CombatIntent`, `CombatAccepted`, `CombatRejected`, `CombatResult`, `CombatStateSnapshot`, and current GameData skill/monster schema fields.

## Implementation Outcome

- Added `LocalCombatPrototypeState` as the deterministic local state owner.
- Covered accepted Wind Slash intent using existing placeholder amount `12`, range `4.5m`, cooldown `6000ms`, and target dummy max HP `120`.
- Covered rejected no-target, out-of-range, and cooldown-active paths.
- Runtime smoke writes `M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0` only when nonzero checks pass.
- Legacy local combat marker remains available for inherited closure gates.

## Allowed Scope

- Local deterministic combat state for the target dummy prototype.
- Accepted/rejected local intent paths.
- Cooldown, range, target validity, and recovery paths using existing contracts.
- Runtime smoke JSON covering pass/fail cases.
- Visual evidence for target dummy idle/selected/recover, hit spark, cooldown states, target marker, warning telegraph, combat button states, and combat panel skin.
- Vietnamese player-facing labels.

## Forbidden Scope

- No protocol changes.
- No GameData schema changes.
- No docs/ADR changes.
- No design token changes.
- No production auth, DB persistence, economy, inventory, loot, guild, chat, market, party, live ops, enemy AI, or MMO-scale combat.
- No private DTO/schema workaround.
- No production art claim.

## Required Gates

- `git --no-pager diff --check`
- `python3.12 tools/validate_m6_combat_readiness_spec.py`
- `python3.12 tools/validate_m6_runtime_usable_combat_asset_pack.py`
- `python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py`
- `python3.12 tools/validate_package_hygiene.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- Local Unity runtime smoke if environment is available.
- Visual evidence capture if screenshot environment is available.

## Entry Criteria

- `M6_COMBAT_READINESS_ACCEPTED_v0.48.0`
- v0.46.1 source/package gates PASS.
- v0.47 visual evidence is review-ready or owner accepted.
