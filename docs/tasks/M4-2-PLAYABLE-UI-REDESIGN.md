# M4-2 Playable UI Redesign

Status: `M4_2_PLAYABLE_UI_REDESIGN_SOURCE_READY`

M4-2 upgrades the existing playable UI presentation from a basic dev form into a clearer game shell based on the v0.11.0 design lock.

Scope:

- Auth / Gate Entry shell
- Character Hall lobby shell
- selected character preview
- World HUD status/action shell
- readable loading, error, empty, and retry-capable states

Non-scope:

- no new gameplay systems
- no protocol changes
- no GameData schema changes
- no production auth or DB persistence
- no final production art claim

Validation:

```bash
python3.12 tools/validate_m4_2_playable_ui.py
python3.12 tools/validate_m4_playable_source.py
python3.12 tools/validate_m4_visual_foundation.py
```
