# Linh Gioi Animation Direction Plan

Marker: `LGO_ANIMATION_DIRECTION_READY`

## Purpose

Animation should make the playable shell feel alive while preserving clarity on small screens. This plan defines the motion language before adding more runtime animation assets.

## Motion Pillars

- Readable first: silhouette and state must be clear at mobile scale.
- Calm spiritual fantasy: idle motion should feel like breathing, floating energy, lantern sway, and gate pulse.
- Combat clarity: hit, ready, cooldown, warning, and target states need short, distinct motion cues.
- Lightweight runtime: prefer small separated frame sets, shader/color pulses, and reused UI states over oversized sheets.
- Placeholder honest: current V2/V3B assets can prove layout and mapping, but do not claim production animation quality.

## State Taxonomy

| Actor/surface | Required states before production | Current task status |
|---|---|---|
| Player | idle, walk, interact, cast intent, receive feedback | direction only |
| Gate Keeper NPC | idle, talk, guide gesture | direction only |
| Training dummy | idle, selected, hit, recover | placeholder states already mapped |
| Shadow threat | idle, alert, hit, dissolve | future art/content |
| Spirit gate | idle pulse, entry active, entry accepted | future visual task |
| UI button | normal, hover/focus, pressed, disabled/cooldown | direction only |
| Skill VFX | anticipation, travel/arc, impact, fade | direction only |
| Warning telegraph | appear, hold, resolve/fade | direction only |

## Timing Targets

| Motion | Target duration | Rule |
|---|---:|---|
| Button press feedback | 80-140 ms | visible but snappy |
| Hit flash/spark | 120-250 ms | must not obscure target label |
| Cooldown pulse | 300-700 ms loop | low contrast while unavailable |
| Target selection ring | 800-1400 ms loop | readable at rest, not noisy |
| NPC idle | 1200-2400 ms loop | subtle, no constant bounce |
| Gate pulse | 1600-3200 ms loop | slow enough for login/lobby calm |
| Warning telegraph | documented per mechanic later | cannot imply damage timing until contract-owned |

## Asset Rules

- Animation runtime sources must be separated transparent PNG frames or approved runtime sprite sheets with manifest.
- Composite/reference boards are not runtime animation sources.
- Do not crop final animation frames from overview sheets.
- Keep VFX frames role-sized, usually 256-512 px unless a documented high-res need exists.
- Do not bake Vietnamese or English labels into animation frames.
- Keep pivots consistent within a frame set.
- Record frame order, FPS, loop mode, role, dimensions, file size, compression, provenance, and SHA-256.

## Unity Runtime Rules

- Use sprite animation only where multiple frames communicate meaningful state.
- Use code-driven tint/scale/alpha pulses for simple UI readiness or highlight.
- Keep `Read/Write Enabled` off unless a tool explicitly requires CPU texture reads.
- Disable mip maps for UI sprites and pixel-stable HUD frames.
- Keep combat feedback local-only unless backed by accepted protocol/server result.

## Vietnamese UX Copy

Animation-adjacent labels and tooltips must remain runtime text:

- `Sẵn sàng`
- `Đang hồi`
- `Mục tiêu`
- `Đánh trúng`
- `Nội bộ thử nghiệm`

Do not copy English labels from reference sheets into runtime UI.

## Future Runtime Gates

Before claiming animation runtime PASS:

- Unity compile/build must pass;
- runtime smoke must show idle, selected, hit, recover, cooldown, and warning states when applicable;
- visual evidence must include desktop and mobile screenshots or video clips;
- frame budget and import settings must be validated;
- human review must accept readability and visual consistency.

## Non-Claims

This plan does not implement animation, approve production art, approve combat timing, or change gameplay semantics.
