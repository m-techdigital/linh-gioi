# Codex Usage — LGO M6 Runtime-Usable Combat Asset Pack v0.45.0

Read this before using any asset.

Use only these files for the v0.45 asset ingest/import-rules task:
- docs/reference-art/v0.45.0/runtime-assets/*.png
- docs/reference-art/v0.45.0/preview/lgo-m6-runtime-combat-assets-preview-v0450.png

Asset role:
- transparent placeholder PNGs
- Unity import-ready references
- not final production art

Allowed use:
- document import settings
- define sprite/UI ownership
- copy approved assets into a controlled Unity placeholder asset path only if the prompt explicitly allows it
- wire validators to ensure no generated/cache files are staged
- keep visible UI text in Vietnamese through UI code, not baked into sprites

Forbidden:
- do not claim production art
- do not add new combat mechanics
- do not add loot, inventory, economy, DB, auth, guild, chat, market, party, live ops, or admin work
- do not touch protocol/**
- do not touch gamedata/schemas/**
- do not touch docs/adr/**
- do not modify client/Unity/Assets/Game/UI/design-tokens.json
- do not use these assets to expand v0.44 scope retroactively

Expected import categories:
- Sprite/target dummy state placeholders
- Sprite/VFX placeholder frames
- UI sprite/cooldown ring and combat button placeholders
- UI 9-slice panel placeholder
- World warning telegraph placeholder

Final handoff must include:
- exact assets ingested
- exact Unity import path if used
- import settings
- validator commands
- package hygiene
- non-claims
