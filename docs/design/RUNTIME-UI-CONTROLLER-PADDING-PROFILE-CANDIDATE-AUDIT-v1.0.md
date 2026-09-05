# Runtime UI Controller Padding Profile Candidate Audit v1.0

Status: `LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY`

## Decision

Screen-specific padding values that must respond to PC/tablet/mobile profiles belong in `RuntimeUiLayoutProfile`. `M4PlayableClientController` should apply those values through `RuntimeUiSkin.ApplyPadding` instead of re-declaring matching edges directly.

## Adopted Candidates

- Root shell initial padding now reads from the current layout profile.
- Auth shell, login CTA, server selector, lobby panel, character list, create panel, session menu, combat panel, settings panel, and position chip now use named profile values.
- World guidance, dialogue progress, and top status responsive padding now route through `RuntimeUiSkin.ApplyPadding`.

## Boundary

Single-edge alignment such as `LoginControlColumnPaddingBottom` remains explicit at the call site because it controls vertical composition rather than a rectangular inset. Values are still profile-owned.

## Follow-Up

Refresh runtime evidence after this source-only controller padding cleanup, then continue with a focused pass on one-edge layout values and whether any should become named alignment helpers.
