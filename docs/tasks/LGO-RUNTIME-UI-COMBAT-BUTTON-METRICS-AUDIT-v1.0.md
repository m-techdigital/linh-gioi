# LGO Runtime UI Combat Button Metrics Audit v1.0

Status: `LGO_RUNTIME_UI_COMBAT_BUTTON_METRICS_READY`

## Scope

This task reduces local numeric metrics in the reusable combat button skin helper while preserving the existing local-only combat prototype behavior.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `tools/validate_lgo_runtime_ui_combat_button_metrics_audit.py`
- Stale validators that previously pinned combat button literal metrics.

## Result

- Added `RuntimeUiSpacing.CombatButtonReadyMinWidth`.
- Added `RuntimeUiSpacing.CombatButtonCooldownMinWidth`.
- Added `RuntimeUiSpacing.CombatButtonMinHeight`.
- Added `RuntimeUiSpacing.CombatButtonReadyFontSize`.
- Added `RuntimeUiSpacing.CombatButtonCooldownFontSize`.
- Added `RuntimeUiSpacing.CombatButtonPaddingHorizontal`.
- Added `RuntimeUiSpacing.CombatButtonPaddingTop`.
- Added `RuntimeUiSpacing.CombatButtonPaddingBottom`.
- Routed `ApplyCombatButtonSkin` button metrics and padding through shared spacing constants.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No cooldown, damage, targeting, server authority, protocol, GameData, ADR, or design-token change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-COMBAT-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0`: refresh target-dummy screenshots and review ready/cooldown button fit after metric extraction.
