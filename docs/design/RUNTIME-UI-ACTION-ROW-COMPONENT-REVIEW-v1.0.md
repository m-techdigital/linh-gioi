# Runtime UI Action Row Component Review v1.0

Status: `LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_READY`

## Decision

Action/button row composition should live in `RuntimeUiFactory` when it is stateless layout. The playable controller should keep only the stateful command ownership: which button exists, what callback it runs, when it is enabled, and when it is shown.

## Reusable Helpers

- `RuntimeUiFactory.NewActionRow` owns repeated button-row flex direction, wrap, alignment, justification, and row margins.
- `RuntimeUiFactory.NewButtonRow` remains as the default simple row wrapper and delegates to `NewActionRow`.
- `RuntimeUiFactory.NewIconStatusRow` owns the repeated icon-plus-status-column row used by compact HUD/action shells.

## Adopted Call Sites

- Character Hall create/enter-world action row.
- NPC dialogue continue/close action row.
- World footer save/back action row.
- Session Menu command row.
- Skill preview command row.
- Local combat primary action row.
- Local combat cooldown-icon plus target/range status row.

## Boundaries

- Keep account, character, world, dialogue, session, and combat prototype state in `M4PlayableClientController`.
- Do not create a screen-level state controller during this pass.
- Do not change gameplay, protocol, GameData, ADR, design tokens, runtime asset payload, or player-facing Vietnamese semantics.

## Follow-Up

Refresh visual evidence for the affected action-row screens, then continue into another small reusable UI primitive only if the screenshots still look stable.
