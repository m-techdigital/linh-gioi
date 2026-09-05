# Runtime UI Controller Responsibility Map v1.0

Status: `LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_READY`

## Current Accepted State

`M4PlayableClientController` remains the playable shell coordinator after `LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY`.

Reusable stateless UI construction now lives in:

- `RuntimeUiSkin`: shared frame, color, padding, border, and role styling helpers.
- `RuntimeUiFactory`: stateless leaf widgets and small composed primitives such as panels, buttons, inputs, icons, toggles, HUD groups, and ornament rows.

`M4PlayableClientController` still owns stateful orchestration for the playable client.

## Responsibility Groups

| Group | Current Owner | Why It Stays Here For Now |
|---|---|---|
| Root document and screen assembly | `M4PlayableClientController` | It wires one `UIDocument` root and coordinates auth, lobby, world, modal, and evidence states together. |
| Auth/account flow | `M4PlayableClientController` | It owns async API calls, busy state, toast/status updates, and transition into character lobby. |
| Character list/create/select flow | `M4PlayableClientController` | It combines account API responses, selected character state, create form state, and enter-world transitions. |
| World HUD and interaction labels | `M4PlayableClientController` | It subscribes to `PlayableWorldController` state and refreshes labels/actions from gameplay-facing runtime state. |
| Local combat preview UI | `M4PlayableClientController` | It bridges existing M6 local preview state to UI feedback without changing combat contract or mechanics. |
| Responsive layout profile | `M4PlayableClientController` | It needs current root size, visible screen, session menu/dialogue state, and mobile/tablet flags. |
| Runtime evidence hooks | `M4PlayableClientController` | Evidence methods intentionally drive the same real UI flow used by the player. |

## Safe Reuse Boundary

Allowed next refactors:

- Add stateless helpers to `RuntimeUiFactory` when the helper only creates or styles a leaf widget.
- Add role-level style helpers to `RuntimeUiSkin` when the rule is reusable across screens.
- Add docs/validators that protect factory ownership and prevent marker drift.

Not allowed without a separate design pass:

- Split auth, character, world, combat, or evidence state into separate controllers.
- Move async flow methods before their state ownership is mapped.
- Introduce new DTOs, protocol messages, GameData schema, or ADR changes for this UI cleanup.
- Rewrite screen composition just to reduce line count.

## Next Safe Target

`LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0`

Review responsive layout code for extractable pure calculations and shared profile constants. The next task should not move screen state; it should only identify or extract safe pure layout helpers if they reduce duplicated viewport math.

## Non-Claims

- No gameplay change.
- No account/character flow semantics change.
- No combat mechanic change.
- No runtime image payload change.
- No visual runtime PASS claim.
