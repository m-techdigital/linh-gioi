# LGO M6 Combat Asset Import Rules v0.45.0

Scope: placeholder combat asset import rules for v0.45/v0.46 only.

Canonical source reference:
`docs/reference-art/v0.45.0/runtime-assets/`

Unity runtime import path:
`client/Unity/Assets/Game/Art/Combat/Placeholders/Resources/CombatPlaceholders/`

Texture settings:
- Texture Type: Sprite.
- Sprite Mode: Single.
- Alpha Source: Input Texture Alpha.
- Alpha Is Transparency: enabled.
- Mip Maps: disabled.
- sRGB: enabled.
- Pixels Per Unit: 100 for world sprites and 100 for UI sprites unless a future atlas spec supersedes this.
- Filter Mode: Bilinear for UI frames/buttons; Point is not required for this painted placeholder style.
- Compression: platform default is acceptable for placeholder source; production compression policy is future work.

9-slice rule:
- `combat-panel-9slice-v0450.png` is the only panel candidate in this pack.
- Runtime usage may use it as a regular placeholder background until Unity Sprite border authoring is formalized.

Runtime rules:
- Existing cooldown/combat state remains authoritative.
- Sprites can visualize target selection, hit flash, cooldown readiness, target labels, and local prototype disclaimers.
- Sprites must not introduce new combat inputs, new targeting rules, new damage rules, or server authority changes.

Non-claims:
- These are not final production art.
- These assets do not close full MMO combat.
