# LGO Runtime UI Skin Foundation Pass v1.0

Status: `LGO_RUNTIME_UI_SKIN_FOUNDATION_READY`

## Scope

This pass introduces a small reusable runtime UI skin helper so future Login, Character Hall, World HUD, menu, and combat UI work can share the same V3B-aligned panel, row, and button framing rules.

## Changes

- Added `RuntimeUiSkin` as a source-only UI helper under `client/Unity/Assets/Game/UI/Runtime`.
- Centralized common radius, padding, edge-frame, login CTA backing, server selector, inset row, and compact action frame styling.
- Replaced repeated style assignments in `M4PlayableClientController` for the highest-traffic panel/row/button surfaces.
- Kept visual assets, gameplay, account flow, protocol, GameData schemas, ADRs, and design tokens unchanged.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No new runtime image payload.
- No visual redesign beyond consolidating existing presentation rules.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue applying `RuntimeUiSkin` opportunistically when touching UI surfaces, especially before adding new panels or controls.
