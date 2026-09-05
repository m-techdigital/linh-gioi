# LGO Character Create Form Presentation Pass v1.0

Status: `LGO_CHARACTER_CREATE_FORM_PRESENTATION_READY`

Date: `2026-09-05`

## Scope

This pass polishes the Character Hall create-form presentation so it reads like an in-world lobby control instead of a developer/debug form. It keeps the existing account and character flow semantics intact.

## Runtime Presentation Changes

- The create panel now uses shorter game-facing Vietnamese copy for the starting cultivation path.
- The visible character-name input is framed with the same dark glass, gold, and cyan language used by the current Character Hall.
- The visible label is now `Danh xưng` and the create command reads `Tạo tu sĩ`.
- The hidden class id field remains hidden and unchanged for current source compatibility.

## Non-Claims

- No account semantics change.
- No protocol or GameData schema change.
- No production art claim.
- No VISUAL_RUNTIME_PASS claim.
- No new gameplay mechanic.

## Validation

Required source validation:

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_character_create_form_presentation.py
./tools/lgo_playable_closure_check.sh --source-only
```

Runtime evidence should refresh `build/visual-evidence/latest/character-lobby.png` when the Unity/player environment is available.
