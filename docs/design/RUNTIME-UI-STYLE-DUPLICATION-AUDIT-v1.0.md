# Runtime UI Style Duplication Audit v1.0

Status: `LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY`

## Scope

This audit follows the Runtime UI Skin usage guide and removes a small set of repeated role styling from `M4PlayableClientController`.

## Centralized In RuntimeUiSkin

- Local settings panel background: `ApplyLocalSettingsPanelFrame`.
- Empty Character Hall guidance card: `ApplyEmptyCharacterCardFrame`.
- Local setting toggle state accent: `ApplySettingToggleState`.
- Combat cooldown icon base frame: `ApplyCombatCooldownIconFrame`.
- Combat cooldown icon ready/cooldown border state: `ApplyCombatCooldownIconState`.

## Remaining Direct Style Use

Allowed direct style remains in the controller when it represents:

- layout and responsive geometry;
- runtime state text and visibility;
- one-off local margins and sizing;
- asset texture assignment;
- screen-specific composition that is not repeated yet.

## Next Refactor Candidates

- `NewCombatPanelSkin` and `ApplyV2PanelSkin` can become named RuntimeUiSkin role helpers if future screens reuse them.
- `NewReadabilityRow` and `NewStatusLabel` can be reviewed for a lightweight UI factory only if they start hiding repeated construction across multiple controllers.
- `M4PlayableClientController` should be split by screen only after behavior tests are stable enough to protect login, lobby, world, dialogue, settings, and combat-preview flow.

## Non-Claims

- No gameplay, combat semantics, protocol, GameData, ADR, or design-token change.
- No visual runtime PASS claim from source inspection.
- No new runtime image payload.
