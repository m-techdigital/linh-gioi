# Runtime UI Primitive Style Boundary Guide v1.0

Status: `LGO_RUNTIME_UI_PRIMITIVE_STYLE_BOUNDARY_GUIDE_READY`

## Purpose

Linh Giới Online runtime UI must stay consistent as Login, Character Hall, World HUD, dialogue, session menu, and combat shells evolve. This guide defines where reusable visual values belong so future work does not add the same button, panel, padding, size, or responsive rule repeatedly inside screen controllers.

## Ownership Layers

### ThemeTokens

`ThemeTokens` owns read-only access to the frozen design-token JSON values.

Use it for:

- named palette colors;
- named spacing scale accessors;
- text scale and touch target values derived from design tokens.

Do not use it for:

- screen-specific placement;
- runtime art dimensions;
- one-off layout fixes.

### RuntimeUiSpacing

`RuntimeUiSpacing` owns runtime component rhythm.

Use it for:

- reusable padding;
- reusable gaps;
- component margins;
- icon and row spacing;
- compact control spacing shared across screens.

Do not use it for:

- responsive viewport decisions;
- primitive radius or fixed dimensions;
- state or gameplay values.

### RuntimeUiSizing

`RuntimeUiSizing` owns primitive dimensions and radii.

Use it for:

- base button radius;
- base panel radius;
- modal width limits;
- progress bar height/radius;
- avatar and skill button dimensions.

Do not use it for:

- screen composition widths;
- platform asset import size budgets;
- gameplay hitbox or world scale.

### RuntimeUiLayoutProfile

`RuntimeUiLayoutProfile` owns viewport-responsive decisions.

Use it for:

- desktop/tablet/mobile safe-area placement;
- screen-specific max widths;
- responsive panel padding;
- mobile/tablet hierarchy decisions that depend on viewport class.

Do not use it for:

- per-button styling;
- palette choices;
- gameplay state.

### RuntimeUiSkin

`RuntimeUiSkin` owns reusable visual treatments.

Use it for:

- glass panel styles;
- framed rows;
- compact and primary button treatments;
- image backing, status chips, accents, and helper margin/padding application.

Do not use it for:

- building whole screen trees;
- copy text;
- event callbacks.

### RuntimeUiFactory

`RuntimeUiFactory` owns reusable UI assembly.

Use it for:

- repeated button/header/badge/action-row/section shell components;
- common composite widgets that appear in more than one screen;
- small UI building blocks that keep controllers from duplicating tree structure.

Do not use it for:

- selecting game state;
- storing player progress;
- deciding which screen is active.

### Screen Controllers

Screen controllers, including `M4PlayableClientController`, own runtime state, flow, copy, and event wiring.

Use them for:

- login/lobby/world/dialogue/session menu state transitions;
- Vietnamese player-facing copy;
- callbacks and command dispatch;
- choosing which reusable component to show.

Do not use them for:

- duplicating base button/panel/avatar/row styling;
- inventing local spacing and radius constants when a shared owner already exists;
- importing art or changing asset budgets.

## Refactor Rule

When touching runtime UI, first identify the smallest correct owner:

1. If the value comes from frozen tokens, use `ThemeTokens`.
2. If the value is reusable rhythm, use `RuntimeUiSpacing`.
3. If the value is primitive size/radius, use `RuntimeUiSizing`.
4. If the value changes by viewport profile, use `RuntimeUiLayoutProfile`.
5. If the value is visual treatment, use `RuntimeUiSkin`.
6. If the tree is a repeated component, use `RuntimeUiFactory`.
7. If it is copy, flow, or event state, keep it in the controller.

## Validation Expectations

- New reusable UI helpers must have a validator and a task marker.
- Runtime visual changes must refresh screenshot evidence when they affect player-visible layout.
- Source-only refactors may skip visual capture only when they are docs/tooling-only or covered by a recent focused evidence refresh.
- Frozen surfaces stay unchanged unless explicitly approved.

## Non-Claims

- No gameplay behavior change.
- No production art claim.
- No `VISUAL_RUNTIME_PASS` claim from this guide.
- No design-token JSON change.
- No protocol, GameData, or ADR change.
