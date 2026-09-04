# ART_V2 Separated Runtime Asset Requirements

Status: MISSING_RUNTIME_ASSET

## Required Package Shape

ART_V2 must deliver individual transparent PNG files, not composite boards or sheets. Each PNG must contain exactly one asset and include:

- clean alpha;
- no baked player-facing text;
- no neighboring asset fragments;
- stable canvas size and pivot guidance;
- intended Unity import type;
- 9-slice border values for panels/buttons when applicable;
- provenance notes and generation/edit history;
- contact sheet for human review only.

## Suggested Asset Groups

- Login and lobby panels, button states, dividers, and ornaments without text.
- Character slot frame, selected frame, class badge icons, and empty-state panel.
- In-world HUD panels, objective panel, status bars, cooldown rings, target marker, telegraph, feedback badges.
- NPC, gate, training stone, target dummy, simple monster, and safe-yard props as separated sprites.
- Skill and VFX frame sequences with consistent canvas and anchor.

## Acceptance Gates

- Alpha and bounds validation.
- No baked English or Vietnamese player-facing text inside sprites.
- Unity `.meta` import validation.
- Runtime screenshot evidence showing assets in context.
- Human visual acceptance checklist.

Until ART_V2 passes these gates, visual integration should use existing approved placeholders and code-owned UI polish only.
