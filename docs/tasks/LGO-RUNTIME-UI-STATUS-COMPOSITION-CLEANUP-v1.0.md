# LGO Runtime UI Status Composition Cleanup v1.0

Status: `LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_READY`

## Changed

- Added `RuntimeUiFactory.NewHiddenMutedLabel` beside `NewHiddenStatusLabel`.
- Routed layout-profile, combat visual-state, cooldown, authority, and local prototype note labels through hidden-label helpers.
- Removed repeated controller-local `style.display = DisplayStyle.None` assignments for those labels.

## Boundaries

- No gameplay, combat, movement, NPC dialogue, account, or character-flow semantics change.
- No visual asset import or image generation.
- No frozen contract surface change.
- No `VISUAL_RUNTIME_PASS` claim.

## Next

`LGO-RUNTIME-UI-STATUS-COMPOSITION-EVIDENCE-REFRESH-v1.0`
