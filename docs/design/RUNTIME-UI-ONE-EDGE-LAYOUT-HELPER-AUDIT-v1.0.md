# Runtime UI One-Edge Layout Helper Audit v1.0

Status: `LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_READY`

## Decision

Reusable margin and one-edge layout values belong in `RuntimeUiLayoutProfile` when they affect screen composition across desktop, tablet, and mobile. Shared edge application helpers belong in `RuntimeUiSkin` so runtime screens do not repeatedly write the same four style assignments.

## Adopted Helpers

- `RuntimeUiSkin.ApplyMargin` centralizes four-edge margin application.
- `RuntimeUiSkin.ApplyVerticalMargin` centralizes vertical-only margin pairs.
- Login, Character Hall, World HUD, dialogue, session menu, combat shell, and settings shell now initialize from the same `RuntimeUiLayoutProfile` values used by responsive refresh.

## Boundary

Zero-value resets, hidden developer-only controls, and one-off component-local spacing remain local. They do not justify new profile properties until they repeat across responsive states or affect viewport fit.

## Non-Claims

- No gameplay behavior change.
- No new runtime art import.
- No protocol, GameData, ADR, or design-token change.

## Follow-Up

Refresh focused runtime evidence for login, Character Hall, world hub, and session menu after the helper/profile cleanup.
