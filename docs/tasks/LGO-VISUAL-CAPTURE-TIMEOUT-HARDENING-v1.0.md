# LGO Visual Capture Timeout Hardening v1.0

Marker: `LGO_VISUAL_CAPTURE_TIMEOUT_HARDENING_READY`

## Scope

Make the standalone visual runtime review more useful when Unity Player capture is slow or blocked.

Allowed:

- keep the visual runtime timeout bounded but raise the default player capture window from 180s to 300s;
- preserve the explicit `LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS` override for constrained or slower machines;
- write durable timeout diagnostics under `build/visual-evidence/latest/visual-capture-timeout.json`;
- stop waiting on the Unity Player once `visual-runtime-evidence-manifest.json` and all expected screenshots exist, then terminate the player cleanly so the operator does not need to click or close the window by hand;
- keep timeout classification honest as `VISUAL_CAPTURE_TIMEOUT`;
- keep screenshot review mandatory before any visual runtime acceptance claim.

Not allowed:

- No `VISUAL_RUNTIME_PASS` claim from build/capture alone.
- No masking player failure as success.
- No protocol, GameData schema, ADR, design-token, auth, DB, economy, social, guild, liveops, or full combat change.

## Closure

This task closes when:

- `tools/lgo_visual_runtime_review.sh` leaves a structured timeout report on player capture timeout;
- `tools/lgo_visual_runtime_review.sh` auto-finishes after all required screenshots are written;
- `tools/lgo_continue_dev_loop.sh` uses the same default visual timeout unless explicitly overridden;
- source/static validation passes;
- standalone visual runtime review is rerun with an explicit longer timeout or records the real runtime blocker.
