# M6 Combat Visual Image Ingest + Readability Polish v0.37.0

Status: `M6_COMBAT_VISUAL_READABILITY_POLISH_SOURCE_READY_v0.37.0`

## Decision

v0.37.0 uses the accepted v0.36.0 combat visual reference boards as reference-only guidance for the existing minimal local combat prototype. The runtime remains placeholder-based and local-only.

The task does not import overview boards as production sprites, does not slice them into runtime assets, and does not claim production art.

## Reference Scope

Used only:

- `docs/reference-art/v0.36.0/lgo-m6-combat-readability-board-v0360.png`
- `docs/reference-art/v0.36.0/lgo-m6-target-dummy-state-sheet-v0360.png`
- `docs/reference-art/v0.36.0/lgo-m6-skill-feedback-sheet-v0360.png`
- `docs/reference-art/v0.36.0/lgo-m6-combat-hud-mockup-v0360.png`
- `docs/reference-art/v0.36.0/lgo-m6-enemy-telegraph-sheet-v0360.png`
- `docs/reference-art/v0.36.0/lgo-m6-hit-cooldown-feedback-sheet-v0360.png`
- `docs/reference-art/v0.36.0/lgo-m6-combat-reference-composite-v0360.png`

Not used for v0.37.0 scope:

- `docs/reference-art/future-reference-v0.36.0/**`

## Runtime Polish

The existing local-only target dummy prototype now exposes clearer player-readable feedback:

- target highlight: a gold placeholder focus ring appears when the player is near enough to try the target dummy;
- hit flash: the existing local hit flash remains the immediate hit acknowledgement;
- cooldown display: a blue placeholder cooldown ring and Vietnamese HUD text distinguish the recovery beat;
- target label: HUD text states selected, cooldown, or unselected target readability state;
- local-only prototype label: HUD copy explicitly says this is a local prototype with no real damage, reward, XP, server combat, or progression;
- tooltip/help text: the attack button describes the visual-only feedback being tested.

## Boundaries

- No new combat mechanic was added.
- No server-authoritative combat was implemented.
- No protocol files changed.
- No GameData schema files changed.
- No ADR files changed.
- `client/Unity/Assets/Game/UI/design-tokens.json` was not changed.
- Player-facing combat UI remains Vietnamese.
- English labels from reference images were not copied into runtime UI.

## Future Asset Task

If runtime-usable combat art is required, create a separate task:

`M6_RUNTIME_USABLE_COMBAT_ASSET_PACK`

That task should define individual transparent PNG sprites, icons, UI panel assets, atlas rules, Unity import settings, and provenance notes. It must not be implied by v0.37.0.

## Code Quality / Duplication / Ownership Audit

The implementation is scoped to the existing Unity world placeholder controller, playable UI controller, closure harness, and a narrow validator. No new architecture layer was introduced. The added world readability state is derived from existing local dummy position, local range, and cooldown fields, so the M6 prototype contract stays unchanged.
