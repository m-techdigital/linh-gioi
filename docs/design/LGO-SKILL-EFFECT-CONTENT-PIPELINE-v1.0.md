# Linh Gioi Skill/Effect Content Pipeline v1.0

Marker: `LGO_SKILL_EFFECT_PIPELINE_READY`

## Purpose

Skill and effect content needs a stable planning lane before it becomes combat runtime data. This document keeps prototype visuals, player-facing labels, and future contract-owned values separated.

## Ownership Boundary

| Surface | Owner | Current status |
|---|---|---|
| Local input label | Unity UI | allowed as Vietnamese runtime copy |
| Local hit/readiness feedback | Unity visual presentation | allowed if it does not change combat semantics |
| Skill id planning | Design/content docs | allowed as planning-only ids |
| Damage, HP, cooldown, targeting rules | GameData contract | requires approved schema/data path |
| Network intent/result messages | Protocol contract | requires approved protocol path |
| Server validation | Server combat implementation | requires approved combat task |

## Planning Format

Planning-only skill/effect rows should include:

- skill id;
- Vietnamese display name;
- role tag;
- input intent;
- visual feedback cue;
- cooldown/readiness presentation note;
- contract dependency;
- implementation status;
- reviewer note.

## Id Patterns

```text
skill.<discipline>.<verb>
effect.<discipline>.<result>
vfx.<discipline>.<cue>
```

Examples:

- `skill.wind.slash`
- `effect.wind.local_hit_flash`
- `vfx.wind.slash_arc`

## Runtime Copy Rules

- Player-facing skill names and help text must be Vietnamese.
- Do not bake skill labels into images.
- Keep HUD copy short enough for mobile combat UI.
- Do not copy English labels from reference images into runtime UI.
- Keep prototype labels clearly marked when behavior is local-only.

## Safe Reuse From Current Source

- Existing local-only combat feedback can stay as presentation feedback.
- Existing target highlight, target label, hit flash, cooldown display, and warning telegraph are reusable as UX affordances.
- Existing placeholder and V2/V3B candidate assets remain temporary visual placeholders unless separately accepted by human visual review.

## Future Entry Criteria

Before implementing production skill/effect content:

- decide whether each value belongs to GameData, protocol, server code, Unity presentation, or localization;
- create a contract-change request for any missing protocol or GameData schema field;
- add positive and negative validators for ids, bounds, targeting, cooldowns, and localization coverage;
- add runtime smoke evidence for Vietnamese UI labels and feedback states;
- document failure classification for client prediction, server rejection, and visual-only fallback.

## Non-Claims

This pipeline does not claim production combat balance, production art quality, server-authoritative correctness, or final content completeness.
