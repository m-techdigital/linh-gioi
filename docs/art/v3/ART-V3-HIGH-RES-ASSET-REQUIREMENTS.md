# ART V3 High-Res Asset Requirements

Status: REQUIRED_FOR_POLISHED_RUNTIME_ART

Purpose:

ART V3 must replace V2 temporary placeholders with high-resolution separated source assets suitable for polished runtime presentation. V3 must not depend on cropping composite sheets.

Required source rules:

- high-resolution separated source assets;
- login background at least 1920x1080;
- character and NPC assets at least 1024px height;
- UI panels and buttons generated separately at 2x or 4x runtime size;
- skill icons at least 512x512;
- VFX frames at least 1024x1024, or clean 512x512 when intentionally authored at that size;
- transparent PNG where alpha is required;
- no baked text unless explicitly intended and documented;
- no composite sheet crop as final runtime source.

Required packaging:

- each asset must be an individual PNG source file;
- include manifest with path, dimensions, SHA256, usage, alpha expectation, and provenance;
- include Unity import intent for sprite, UI, nine-slice, VFX frame, or background use;
- include replacement mapping from V2 placeholder identifiers to V3 final candidates;
- keep reference boards separate from runtime source assets.

Acceptance gates:

- alpha and edge quality review;
- scale consistency review;
- readability review in Login/Gate Entry, HUD, world, and combat states;
- Unity import validation;
- no reference/mockup/composite imports into runtime;
- no production claim until human visual acceptance explicitly approves V3 assets.
