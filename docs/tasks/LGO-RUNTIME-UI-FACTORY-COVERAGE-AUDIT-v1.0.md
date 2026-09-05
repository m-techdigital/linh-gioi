# LGO Runtime UI Factory Coverage Audit v1.0

Status: `LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_READY`

## Scope

This pass adds a reusable image-layer helper for repeated runtime UI texture-backed `VisualElement` setup.

## Implementation Notes

- Added `RuntimeUiFactory.NewImageLayer`.
- Replaced repeated logo, Gate Keeper NPC, and Character Hall portrait image-layer setup with the shared helper.
- Kept screen-specific sizing, spacing, texture fallback, and stateful behavior in `M4PlayableClientController`.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new runtime image payload.
- No gameplay, combat semantics, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-IMAGE-LAYER-EVIDENCE-REFRESH-v1.0`: build, capture, and review login and Character Hall screenshots after image-layer helper adoption.
