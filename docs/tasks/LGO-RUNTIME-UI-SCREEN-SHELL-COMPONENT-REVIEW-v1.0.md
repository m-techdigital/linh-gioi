# LGO Runtime UI Screen Shell Component Review v1.0

Status: `LGO_RUNTIME_UI_SCREEN_SHELL_COMPONENT_REVIEW_READY`

## Scope

This pass extracts a small reusable screen-shell helper for repeated runtime UI panel construction while preserving the current playable client behavior.

## Implementation Notes

- Added `RuntimeUiFactory.NewSectionShell`.
- Replaced repeated `NewPreviewPanel` + `name` + `NewSectionTitle` construction in dialogue, session menu, skill preview, and local combat panels.
- Kept all stateful labels, buttons, async callbacks, visibility rules, and gameplay/session transitions inside `M4PlayableClientController`.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new runtime image payload.
- No gameplay, combat semantics, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-SCREEN-SHELL-EVIDENCE-REFRESH-v1.0`: capture and review affected runtime screens after this shell helper extraction.
