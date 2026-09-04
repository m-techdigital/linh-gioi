# M6 Unity Combat Placeholder Asset Import v0.46.0

Status: Unity placeholder asset import and narrow runtime presentation wiring.

Scope:
- Copy v0.45 runtime-usable placeholder PNGs into Unity source under `client/Unity/Assets/Game/Art/Combat/Placeholders/Resources/CombatPlaceholders/`.
- Configure imported files as Sprite placeholders with alpha transparency.
- Wire existing world/UI combat presentation to use target dummy, hit feedback, cooldown, target marker, warning telegraph, panel, and button placeholder assets.
- Keep existing local/server-authoritative combat semantics unchanged.

Non-goals:
- No new combat mechanics.
- No production art claim.
- No protocol, gamedata schema, ADR, or design token changes.

Acceptance marker:
`M6_UNITY_COMBAT_PLACEHOLDER_ASSET_IMPORT_PASS_v0.46.0`
