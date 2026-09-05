# LGO Runtime UI Action Row Component Review v1.0

Status: `LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_READY`

## Scope

This pass reduces repeated runtime UI row composition in the playable controller by moving stateless action-row and icon-status-row layout into `RuntimeUiFactory`.

## Implementation Notes

- Added `RuntimeUiFactory.NewActionRow` for command button rows with explicit element name, justification, and margins.
- Kept `NewButtonRow` as a compatibility/simple helper backed by `NewActionRow`.
- Added `RuntimeUiFactory.NewIconStatusRow` for compact HUD rows that pair an icon with a vertical status column.
- Adopted these helpers in Character Hall, dialogue, world footer, Session Menu, Skill Preview, and local combat shell construction.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new runtime image payload.
- No gameplay, combat semantics, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0`: build, capture, and review affected screens after action-row helper adoption.
