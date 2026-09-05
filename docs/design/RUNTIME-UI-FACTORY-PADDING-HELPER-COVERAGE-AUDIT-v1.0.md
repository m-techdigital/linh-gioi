# Runtime UI Factory Padding Helper Coverage Audit v1.0

Status: `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY`

## Decision

Reusable UI helpers should not re-declare matching left/right/top/bottom padding by hand when `RuntimeUiSkin.ApplyPadding` already owns that primitive. This keeps helper styling consistent while leaving screen-specific responsive layout values in `RuntimeUiLayoutProfile` and `M4PlayableClientController`.

## Adopted Call Sites

- `RuntimeUiFactory.ApplyHudStatusCompact` now applies compact status padding through `RuntimeUiSkin.ApplyPadding`.
- `RuntimeUiFactory.NewIconStatusRow` now applies its row inset through `RuntimeUiSkin.ApplyPadding`.
- `RuntimeUiFactory.ApplyCombatButtonSkin` now applies combat button horizontal inset through `RuntimeUiSkin.ApplyPadding`.
- `RuntimeUiSkin.ApplySettingToggleStatePill` now applies pill padding through the shared helper.

## Boundary

Single-edge semantic offsets remain local when they express layout meaning, such as list-button text indentation. Screen-level responsive padding remains profile-owned and is not folded into factory helpers.

## Follow-Up

Refresh runtime evidence after this source-only helper cleanup, then continue with controller padding profile candidates that are still screen-specific.
