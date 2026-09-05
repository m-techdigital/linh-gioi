# Runtime UI Character Hall Content Density Polish v1.0

Status: `LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_READY`

## Purpose

Reduce repeated text blocks in the Character Hall while keeping the same account, character creation, character selection, and enter-world behavior.

## Decision

- The selected character card now keeps two visible status rows: current state and next objective.
- The class/cultivation summary is retained in source state as `_selectedClassSummary`, but collapsed from the visible card because the same information is already present in `_selectedMeta`.
- Empty and selected states use shorter Vietnamese copy.
- The controller still owns account/character flow semantics; the polish only changes presentation density.

## Non-Goals

- No gameplay change.
- No account/character semantic change.
- No new asset import.
- No protocol, GameData, ADR, or design-token change.

## Follow-Up

Refresh runtime screenshots with `LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-EVIDENCE-REFRESH-v1.0`.
