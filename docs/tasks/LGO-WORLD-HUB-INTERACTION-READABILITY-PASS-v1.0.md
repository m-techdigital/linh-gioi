# LGO World Hub Interaction Readability Pass v1.0

Status: `LGO_WORLD_HUB_INTERACTION_READABILITY_READY`

## Scope

This pass improves World Hub interaction readability after the viewport-aware prop staging pass. It does not add gameplay, combat semantics, protocol changes, GameData schema changes, ADR changes, or design-token edits.

## Runtime Presentation Changes

- World-space interaction prompts now use short object-aware labels: `F Gặp`, `F Luyện`, or wider `F / Space` variants on desktop.
- Prompt placement uses `CurrentInteractionPromptOffset()` so labels sit closer to the interactable on tablet/mobile instead of floating high over the scene.
- HUD interaction copy now reads from `InteractionActionText`, a presentation-only summary that keeps the guidance card shorter while preserving the full legacy `InteractionText` for smoke tests and state feedback.

## Non-Claims

- No gameplay mechanic change.
- No new runtime art import.
- No production art claim.
- No VISUAL_RUNTIME_PASS claim.

## Validation

- `python3.12 tools/validate_lgo_world_hub_interaction_readability.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `python3.12 tools/validate_package_hygiene.py`
- `git --no-pager diff --check`

## Follow-Up

Continue with `LGO-WORLD-HUB-INTERACTION-EVIDENCE-REFRESH-v1.0` to refresh desktop/tablet/mobile screenshots and review whether the shorter prompt improves readability.
