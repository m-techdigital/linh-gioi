# P5 PROMPT — S3 UI FOUNDATION

You are S3. Implement the reusable UI foundation on top of the accepted S1 Unity project.

## Preconditions

- S1 Unity project is accepted and present.
- S1 owns the production bootstrap scene.
- You must not edit S1 production bootstrap scene/root prefab unless S0 explicitly reassigns ownership.

## Required reading

- `README.md`
- `docs/01-PRODUCT-CONSTITUTION.md`
- `docs/06-UI-DESIGN-SYSTEM.md`
- `docs/07-ART-BIBLE.md`
- `docs/09-DEFINITION-OF-DONE.md`
- `docs/10-INTEGRATION-RULES.md`
- `docs/11-PERFORMANCE-BUDGET.md`
- `docs/tasks/S3-UI-FOUNDATION.md`
- `docs/reference-art/**`
- current S1 Unity project structure/asmdefs
- `docs/execution/03-HANDOFF-CONTRACT.md`

## Goal

Create a cohesive reusable UI component foundation matching the M0 design system/reference art without implementing feature screens.

## Allowed paths

- `client/Unity/Assets/Game/UI/**`

## Forbidden paths

- `protocol/**`
- `gamedata/**`
- `server/**`
- gameplay/network production implementation
- S1 bootstrap scene/root production prefab
- second independent hardcoded palette

## Required implementation

1. Consume `design-tokens.json` as canonical color/spacing/theme source and generate or map it into Unity-consumable theme assets.
2. Implement reusable primitives:
   - BaseButton
   - IconButton
   - BasePanel
   - ModalPanel
   - ProgressBar
   - HealthBar
   - ManaBar
   - SkillButton placeholder
   - AvatarView placeholder
   - Nameplate
   - TabBar
   - Toast
   - CurrencyDisplay
3. Create an S3-owned showcase scene or prefab showcase; do not edit S1 production bootstrap scene.
4. Demonstrate representative 16:9 desktop layout and tall-phone/safe-area behavior.
5. Typography/icon strategy must use replaceable references; do not embed unlicensed proprietary fonts/icons.
6. Components must be interactable/testable without direct combat dependency.
7. Add token/theme/component-loading tests where feasible.

## Acceptance

- all required primitives render in showcase;
- canonical token source is respected;
- representative desktop and tall phone layouts are usable without clipping critical controls;
- no direct combat dependency;
- no production-scene ownership violation;
- project still compiles with accepted S1 asmdef boundaries.
