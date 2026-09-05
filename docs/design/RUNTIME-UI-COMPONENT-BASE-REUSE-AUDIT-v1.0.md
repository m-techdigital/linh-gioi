# Runtime UI Component Base Reuse Audit v1.0

Status: `LGO_RUNTIME_UI_COMPONENT_BASE_REUSE_READY`

## Purpose

Runtime UI text styling was still repeated across `RuntimeUiFactory` and the playable client controller. This pass moves shared label styling into `RuntimeUiSkin.ApplyText` so future Login, Character Hall, HUD, dialogue, and menu work can reuse one skin path instead of rewriting color, weight, size, and alignment by hand.

## Ownership

- `RuntimeUiSkin.ApplyText` owns reusable label color, optional font size, optional bold weight, and alignment.
- `RuntimeUiFactory` owns repeated component creation such as section titles, badges, status labels, toast labels, and preview panel labels.
- `M4PlayableClientController` may still set responsive font sizes during layout refresh because those values depend on current viewport profile.
- Gameplay state, account flow, combat semantics, and runtime evidence sequencing remain outside this style helper.

## Result

- Shared text styling now has one reusable base helper.
- Factory-created labels no longer repeat the same color/font/bold assignments.
- Key controller labels use the shared helper while keeping profile-driven responsive updates intact.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
