# M5 Guided Training Loop v0.17.0

Status: `M5_GUIDED_TRAINING_LOOP_SOURCE_READY`

Runtime close target after local smoke evidence: `M5_GUIDED_TRAINING_LOOP_RUNTIME_CLOSED_LOCAL_v0.17.0`

## Scope

This task uses the accepted visual reference pack `LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5` to harden the existing first playable loop into a clearer local, non-combat guided training sequence:

1. Enter World.
2. Objective: talk to the Gate Keeper.
3. Move near Gate Keeper.
4. Press F or Space to interact.
5. Objective updates: stabilize the Training Stone.
6. Move near Training Stone.
7. Press F or Space to interact.
8. Show spirit pulse feedback.
9. Objective complete.
10. Save Position remains available.
11. Back to Lobby remains available.

## Runtime Marker

```text
M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS
```

## Visual Reference Use

- Gate Keeper readable friendly marker language comes from the character/NPC board.
- Training Stone and spirit pulse feedback come from the item/skill/VFX board.
- HUD copy and density stay simplified from the playable HUD mockup.
- World hub direction uses the spirit gate, central safe area, cyan energy, warm gold guidance, and purple non-combat shadow marker.

## Validation

```bash
python3.12 tools/validate_visual_reference_pack.py
python3.12 tools/validate_m5_guided_training_loop.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_playable_closure_check.sh --package-ready
./tools/lgo_playable_closure_check.sh --runtime
./tools/run_m5_guided_training_loop_once.sh
```

## Non-Claims

- not full combat.
- No damage, HP balancing, loot, inventory, economy, guild, chat, market, party, or live ops.
- No production auth.
- No database persistence beyond the inherited M3 API position save.
- No protocol or GameData schema change.
- Not final production UI.
- Not final production art.
