# LGO World Hub Visual Debt Triage v1.0

Status: `LGO_WORLD_HUB_VISUAL_DEBT_TRIAGE_READY`

## Scope

This pass reviews the latest runtime screenshots and fixes the clearest presentation debt that is safe to resolve without new art, gameplay, or contract changes.

## Triage Result

- Login runtime is acceptable for continued iteration, but still needs future polish around panel texture/ornament integration.
- Character Hall mobile remains readable, but dense text can be softened in a later UI pass.
- World Hub mobile is readable enough after recent camera/label work.
- Session Menu mobile had the strongest immediate issue: the pause overlay competed with the left dialogue panel.

## Fix

- Session menu now suppresses the dialogue panel while open and restores dialogue state when the player returns to the world.
- This is presentation-only focus cleanup; dialogue state and gameplay semantics are unchanged.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay mechanic change.
- No runtime art import or recompression.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Refresh visual profile evidence and inspect `session-menu.png` on mobile/tablet. If the pause overlay still feels crowded, continue with panel width/position polish before adding new gameplay.
