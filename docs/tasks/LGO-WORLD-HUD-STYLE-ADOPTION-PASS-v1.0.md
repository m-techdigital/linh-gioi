# LGO World HUD Style Adoption Pass v1.0

Status: `LGO_WORLD_HUD_STYLE_ADOPTION_READY`

## Scope

This pass moves common World HUD, preview panel, HUD group, compact status, and session menu frame rules into `RuntimeUiSkin` without changing gameplay, combat semantics, copy, or responsive layout decisions.

## Changes

- Added shared `RuntimeUiSkin` helpers for preview panels, world HUD groups, compact HUD status rows, session menu frames, and profile-aware HUD/session backgrounds.
- Updated `M4PlayableClientController` to use those helpers while keeping viewport sizing, visibility, and gameplay state handling in the controller.
- Added a focused validator and included it in playable source closure.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new runtime image payload.
- No gameplay, combat semantics, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-SKIN-ADOPTION-EVIDENCE-REFRESH-v1.0`: refresh desktop/tablet/mobile visual evidence for Login, Character Hall, World Hub, Dialogue, and Session Menu after the shared-skin refactor series.
