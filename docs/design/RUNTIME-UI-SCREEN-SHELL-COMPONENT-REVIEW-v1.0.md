# Runtime UI Screen Shell Component Review v1.0

Status: `LGO_RUNTIME_UI_SCREEN_SHELL_COMPONENT_REVIEW_READY`

## Decision

Extract only stateless repeated shell construction into `RuntimeUiFactory.NewSectionShell` for this pass. Do not split account, character, world, dialogue, combat, or session state machines out of `M4PlayableClientController` yet.

## Reused Pattern

The repeated pattern was:

- create `NewPreviewPanel(sigil, heading)`;
- assign a stable element name;
- add a centered section title;
- then let the controller attach stateful labels, buttons, and callbacks.

`NewSectionShell` now owns that construction for:

- dialogue shell;
- session menu overlay;
- skill preview shell;
- local combat action shell.

## Why This Boundary

- The helper reduces duplicated UI shell code without changing behavior.
- The controller still owns mutable references, async callbacks, runtime state, and screen transitions.
- This keeps the next refactor safe: future extraction can target pure layout/data helpers before any stateful screen controller split.

## Non-Claims

- No visual runtime PASS claim from source inspection.
- No gameplay, combat semantics, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-SCREEN-SHELL-EVIDENCE-REFRESH-v1.0`: build/capture the affected shell screens and review for regression before extracting another UI base.
