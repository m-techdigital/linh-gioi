# M5 First Playable Loop Foundation v0.15.0

Status: `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`

## Scope

This task adds a controlled first playable loop foundation to the existing M4 playable world shell:

- enter world through the existing dev account and character flow;
- see the player marker, Gate Keeper NPC marker, Training Stone marker, and non-combat Shadow Slime marker;
- move near the Gate Keeper or Training Stone;
- press F or Space to trigger local interaction feedback;
- receive objective completion text;
- keep Save Position and Back to Lobby available.

## Runtime Marker

```text
M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS
```

## Validation

```bash
python3.12 tools/validate_m5_first_playable_loop.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_playable_closure_check.sh --package-ready
./tools/lgo_playable_closure_check.sh --runtime
```

## Manual Visible UI Review

```bash
./tools/run_m4_visible_ui_review.sh --rebuild
```

The visible review harness writes `build/manual-ui/visible-ui-review-summary.json` and attempts a 1280x720 screenshot. If screenshot capture is unavailable, it records `VISIBLE_UI_SCREENSHOT_UNAVAILABLE` with the exact reason.

## Non-Claims

- not full combat.
- No damage, HP balancing, loot, inventory, economy, guild, chat, market, party, or live ops.
- No production auth.
- No database persistence beyond the inherited M3 API position save.
- No protocol or GameData schema change.
- Not final production UI.
- Not final production art.
