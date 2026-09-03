# Linh Gioi Playable UI Wireframe Spec v0.11.0

Status: `LGO_PLAYABLE_UI_WIREFRAME_SPEC_READY_FOR_REVIEW_v0.11.0`

This spec describes the next UI layout direction. It is not an implementation task.

## Login / Dev Entry

Layout:

- Full-screen game shell with dark world-gate backdrop or simple runtime panel.
- Top-left or top-center title: `Linh Gioi Online`.
- Primary entry panel: dev key field, login button, API status.
- Secondary line: environment and connection target.

States:

- Loading: disable login, show concise progress.
- Success: show account ID/profile line and reveal lobby.
- Error: show short error with retry button.

## Character Select / Create

Layout:

- Character list on the left or top, depending on screen width.
- Selected character preview area with silhouette, name, class ID, persisted position summary.
- Empty state: `No character yet`, then character name/class fields and create button.
- Enter World button only active when a character is selected.

States:

- Loading list: show list skeleton or compact loading line.
- Duplicate/validation failure: keep form values and show inline error.
- API unavailable: keep retry visible.

## World HUD

Layout:

- Top status strip: account ID, character name, class ID, API state.
- Small position/debug panel: x/y/z/yaw values, visually secondary.
- Lower action cluster: Save Position, Back to Lobby.
- Control hint line: `Move: WASD/arrows  Rotate: Q/E`.

World presentation:

- Player marker centered enough to inspect.
- NPC/monster/VFX placeholders visible but not crowding the player.
- Ground tile/motif supports orientation.

## Status Panel

Content order:

- API status.
- Account ID.
- Character ID/name/class.
- Current position.
- Last save result or error.

Rules:

- Do not hide errors behind logs only.
- Do not show raw dev keys.
- Do not let debug text dominate the screen.

## Save / Back Actions

- Save Position is a clear command in world mode only.
- Back to Lobby returns without destroying account state.
- On successful save, show timestamp-neutral confirmation.
- On failed save, leave player position intact and allow retry.

## Error / Loading / Empty States

- Loading states disable only the affected controls.
- Empty lobby should lead directly to create character.
- API unavailable should not crash the shell.
- Retry should be visible on login and lobby list failures.

## Keyboard Hint Placement

- Desktop: lower-left or lower-center compact hint.
- Mobile: hide keyboard hints behind a small help/status affordance until mobile controls exist.

## Mobile Adaptation

- Stack login/lobby panels vertically.
- Keep primary button reachable below the active form.
- Collapse long account/character IDs to middle ellipsis in UI only; logs/smoke may keep full IDs.
- Avoid fixed-width layouts wider than common phone portrait screens.
