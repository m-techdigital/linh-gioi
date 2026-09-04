# M5 VFX Feedback Placeholder v0.23.0

Decision marker: M5_VFX_FEEDBACK_PLACEHOLDER_SOURCE_READY_v0.23.0

Scope:

- Add visual-only runtime feedback states for portal gate pulse, spirit pulse, wind slash preview, and shadow bind warning.
- Surface the active feedback state in the existing playable HUD.
- Extend the guided training runtime smoke result with deterministic VFX feedback state assertions.

This pass is visual-only and does not add damage, HP, cooldowns, enemy attacks, loot, inventory, combat progression, protocol changes, or GameData schema changes.

Frozen surfaces remain out of scope:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`
