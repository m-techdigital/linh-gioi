# M6 Combat UX Readability Polish v0.53.0

Status: `M6_COMBAT_UX_READABILITY_POLISH_READY_FOR_VERIFY_v0.53.0`

## Readability Model

The combat placeholder HUD now separates:

- target selected state;
- range state;
- cooldown state;
- local hit feedback;
- accepted/rejected authority text.

## Player-Facing Rules

- Vietnamese copy is used for the combat HUD and button state.
- The selected target state uses clear target selected language and spirit/gold accents.
- The out-of-range state uses explicit `out-of-range` warning semantics in the design note and Vietnamese runtime text.
- The cooldown state uses a visible cooldown icon/button skin and Vietnamese cooldown copy.
- Rejected feedback names no target, out of range, or cooldown without changing combat rules.

## Placeholder Boundary

Only existing runtime placeholder assets are used. This is a readability pass, not production art and not a mechanic expansion.
