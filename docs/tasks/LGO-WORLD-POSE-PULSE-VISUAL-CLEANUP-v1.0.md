# LGO World Pose Pulse Visual Cleanup v1.0

Status: `LGO_WORLD_POSE_PULSE_VISUAL_CLEANUP_READY`

## Scope

This pass removes the square-looking player pose pulse artifact observed in `build/visual-evidence/latest/target-dummy-state.png`.

## Implementation Notes

- Player pose pulse now prefers the existing V3B cooldown/selection ring sprite as a lightweight world-space billboard.
- The legacy cube fallback remains only if no sprite is available, and is reduced to a much smaller scale.
- `TriggerLocalPosePulse` now tints `SpriteRenderer` feedback directly instead of replacing the sprite material with a flat opaque material.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new runtime image payload.
- No gameplay, combat semantics, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-SCREEN-SHELL-EVIDENCE-REFRESH-v1.0`: capture and review affected shell screens plus target dummy state to confirm the square pulse artifact is gone.
