# LGO Art V2 Asset Quality Rules

Status: LGO_ART_V2_SEPARATED_ASSETS_INGESTED

Quality classification: STRUCTURAL_RUNTIME_PLACEHOLDER_V2

Structural placeholder asset rules:

- Each runtime asset must be an individual PNG from `images/runtime-ready/**`.
- No reference board, overview, mockup, contact sheet, or composite sheet may be imported into Unity runtime.
- Button, panel, feedback, and skill assets must not carry baked player-facing copy; Unity renders Vietnamese UI text.
- `login_background_spirit_gate_1920x1080.png` is allowed as the opaque background exception.
- `logo_linh_gioi_online_1024x512.png` is allowed as a title/logo asset; it is not a localization source.
- PNG SHA256 and dimensions must match `metadata/runtime-assets-v2-manifest.csv`.
- Unity `.meta` files must preserve alpha, disable mipmaps, and use Sprite 2D/UI import settings.

Art v1.1 crop outputs remain `experimental-source-only`. They must not be used to claim production art, final runtime art, or a final runtime asset pack.

V2 separated assets are allowed only for temporary integration, mapping, validator coverage, and UI layout proof. They must not be presented as production-quality or final visual-quality runtime art. ART V3 is required for polished high-resolution runtime assets.
