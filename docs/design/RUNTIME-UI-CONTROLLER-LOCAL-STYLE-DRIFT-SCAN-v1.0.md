# Runtime UI Controller Local Style Drift Scan v1.0

Status: `LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_READY`

## Purpose

After establishing reusable ownership for tokens, spacing, sizing, layout profiles, skin, and factory components, this pass scanned `M4PlayableClientController` for remaining local style duplication.

## Findings

- Controller-local visibility state is legitimate because screen controllers own runtime flow.
- Repeated direct `style.display = visible ? DisplayStyle.Flex : DisplayStyle.None` assignments made screen flow harder to review.
- `style.visibility = Hidden/Visible` also appeared in viewport/session overlay handling and should remain controller-owned but not repeated inline.
- Layout sizes, padding, margins, panel frames, and button visual treatments already have shared owners and should not be reintroduced locally.

## Implemented Cleanup

- Added local `IsDisplayed` helper for controller state checks.
- Added local `SetDisplayed` helper for `DisplayStyle.Flex`/`DisplayStyle.None` state updates.
- Added local `SetElementVisibility` helper for `Visibility.Visible`/`Visibility.Hidden` overlay updates.
- Replaced flow-mode, session-menu, dialogue, and local-settings visibility assignments with the helpers.

## Kept Local

- Login/lobby/world mode decisions.
- Session menu focus state.
- Dialogue visibility state.
- Mobile/tablet compact-world decisions.
- Vietnamese player-facing copy and event flow.

## Non-Goals

- No gameplay behavior change.
- No visual redesign.
- No production art import.
- No `VISUAL_RUNTIME_PASS` claim.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-EVIDENCE-REFRESH-v1.0`: refresh focused screenshots after visibility-helper cleanup to confirm Login, Character Hall, World HUD, Session Menu, and Target Dummy states still render.
