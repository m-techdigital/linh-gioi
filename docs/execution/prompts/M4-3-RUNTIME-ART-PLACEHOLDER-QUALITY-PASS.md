# Future Prompt: M4-3 Runtime Art Placeholder Quality Pass

You are working on Linh Gioi Online. Start only after the v0.11.0 design lock is accepted and M4-2 status is known.

This is an art-source quality pass, not a gameplay task.

Rules:

- Replace or improve current basic SVG placeholders with better coherent original placeholders.
- Keep assets lightweight and source-controlled.
- Preserve Unity `.meta` files.
- Keep `docs/art/LGO-RUNTIME-ART-MANIFEST-v0.10.0.md` or its successor complete.
- Do not import unlicensed web art or paid marketplace assets.
- Do not claim final production art.
- Do not touch protocol, GameData schemas, ADR, production auth, DB persistence, economy, guild, chat, or combat systems.

Goal:

Upgrade placeholder art quality for player, NPC, monster, items, skills, map/environment, UI motifs, and VFX markers while preserving runtime compatibility and smoke stability.

Required reads:

- `docs/design/LGO-DESIGN-DIRECTION-LOCK-v0.11.0.md`
- `docs/art/LGO-CONCEPT-IMAGE-PROMPTS-v0.11.0.md`
- `docs/art/LGO-RUNTIME-ART-MANIFEST-v0.10.0.md`
- `client/Unity/Assets/Game/Art/README.md`
- `tools/validate_m4_visual_foundation.py`

Validation:

- `python3.12 tools/validate_m4_visual_foundation.py`
- `python3.12 tools/validate_m4_playable_source.py`
- When environment allows, build the macOS player and run M3-B, M4-0, and M4 visual smokes.

Expected non-claims:

- final production art not claimed
- full combat/VFX system not claimed
- full MMO gameplay not claimed
- next milestone not opened automatically
