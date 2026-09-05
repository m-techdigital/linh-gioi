# LGO Near Interaction Evidence Refresh v1.0

Status: `LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY`

## Evidence

Captured with `./tools/lgo_visual_runtime_review_profiles.sh` after adding explicit near-interaction checkpoints.

- desktop: `build/visual-evidence/profiles/desktop/near-gatekeeper-prompt.png`
- desktop: `build/visual-evidence/profiles/desktop/near-training-stone-prompt.png`
- tablet: `build/visual-evidence/profiles/tablet/near-gatekeeper-prompt.png`
- tablet: `build/visual-evidence/profiles/tablet/near-training-stone-prompt.png`
- mobile: `build/visual-evidence/profiles/mobile/near-gatekeeper-prompt.png`
- mobile: `build/visual-evidence/profiles/mobile/near-training-stone-prompt.png`
- profile index: `build/visual-evidence/profiles/index.md`

## Review Notes

- Near-Gate-Keeper evidence now positions the player inside interaction range and shows the short `F Gặp` / `F / Space  Gặp` world prompt plus the HUD action summary.
- Near-Training-Stone evidence now positions the player inside interaction range and shows the short `F Luyện` / `F / Space  Luyện` world prompt plus the HUD action summary.
- The Gate Keeper prompt uses a separate presentation offset so it does not collapse into the two-line Vietnamese NPC name on smaller profiles.
- Mobile still relies on the left HUD action shell as the primary accessible copy; the world-space prompt is intentionally brief to avoid covering the actor cluster.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay mechanic change.
- No new runtime art import.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Continue with the next safe post-login presentation task from `docs/execution/NEXT-ACTION.md`. If future screenshots show world-space prompt overlap, keep fixes presentation-only unless a real interaction contract bug is found.
