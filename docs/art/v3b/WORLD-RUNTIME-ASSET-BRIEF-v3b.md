# LGO World Runtime Asset Brief v3B

Marker: `LGO_WORLD_V3B_RUNTIME_ASSET_BRIEF_READY`

## Purpose

Replace the remaining V2 structural world placeholders with V3B-aligned separated runtime assets while keeping the game lightweight across PC, tablet, and mobile builds.

V3B remains a runtime candidate tier, not final production art. The goal is a coherent, attractive playable slice without importing composite sheets, oversized reference posters, or unmanaged one-off images.

## Current Evidence

Latest reviewed evidence:

- `build/visual-evidence/profiles/desktop/world-hub.png`
- `build/visual-evidence/profiles/tablet/world-hub.png`
- `build/visual-evidence/profiles/mobile/world-hub.png`

Observed state:

- Login now has V3B-aligned background, NPC, panel, button, and a lighter 512x256 logo runtime candidate.
- World hub uses V3B Spirit Gate and Training Stone.
- NPC in-world presentation still leans on the login-scale sprite and needs staging/scale review.
- Player and target dummy selected/hit/recover states now have V3B runtime candidates, with visual evidence rerun required before any runtime pass claim.
- Cherry tree, pine tree, lanterns, moss rock, bridge, and cultivation banner now have lightweight V3B runtime candidates.
- Shadow slime now has a lightweight V3B non-combat warning sprite.
- V2 props are light, but their polish level is visibly below login/reference quality.

## Asset Rules

- Do not crop or slice composite/reference sheets.
- Do not import reference-only boards or mockups into Unity.
- Generate or ingest separated transparent PNGs only.
- Keep baked text out of sprites unless the asset is the official logo.
- Prefer one clean runtime copy per role plus manifest provenance.
- Use min/max runtime budgets instead of shipping huge source art directly.

## Runtime Size Targets

| Role | Target runtime size | Max texture size | Max file size | Notes |
|---|---:|---:|---:|---|
| player male cultivator | 320x480 or 384x576 | 1024 | 180 KB | Transparent full-body, readable silhouette, no glow flood; idle pose must not hold a sword incorrectly. Prefer sheathed/back/waist weapon or empty relaxed hands. |
| player female cultivator | 320x480 or 384x576 | 1024 | 180 KB | Same visual scale as male; idle pose must keep weapon handling credible and non-combat. |
| gate keeper in-world | 384x576 | 1024 | 180 KB | Can reuse login styling, but simplified for world scale. |
| target dummy idle | 256x384 | 512 | 140 KB | V3B style, warm wood/gold/cyan target motif. |
| target dummy selected | 256x384 | 512 | 160 KB | Same pose, readable selection ring/mark. |
| target dummy hit | 256x384 | 512 | 160 KB | Same pose, local hit flash only. |
| target dummy recover | 256x384 | 512 | 150 KB | Same pose, calmer recovery color. |
| shadow slime idle | 256x256 | 512 | 90 KB | Cute readable prototype target, restrained purple glow. |
| cherry tree | 384x384 | 512 | 90 KB | Background prop, painterly but low-detail enough for size. |
| pine tree | 384x384 | 512 | 80 KB | Background prop, simple strong silhouette. |
| lantern prop | 192x384 | 512 | 60 KB | Warm light, transparent. |
| moss rock | 256x256 | 512 | 45 KB | Small foreground prop. |
| cultivation banner | 192x384 | 512 | 55 KB | No baked text. |
| wooden bridge | 512x256 | 512 | 90 KB | Foreground prop, can tolerate lower detail. |
| selection ring | 256x256 | 256 | 45 KB | Transparent VFX sprite. |
| spirit orb | 192x192 | 256 | 35 KB | Small VFX accent. |

Source high-res art may be larger in `docs/reference-art/**`, but Unity runtime copies must meet the role budget above unless a task explicitly updates this brief and validator.

## Import Ownership

Runtime candidates should live under:

- `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/World/**`
- `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/VFX/**`
- `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Combat/**`

Provenance/review sources should live under:

- `docs/reference-art/v3b/runtime-candidates/**`
- `docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv`
- `docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.json`

## Priority Order

1. Player cultivator sprite: improves every world screenshot immediately.
2. Target dummy idle/selected/hit/recover state set: removes mismatch between combat UI and world target. Closed as `LGO_WORLD_V3B_DUMMY_STATE_SET_READY`.
3. Gate Keeper in-world: aligns NPC interaction quality with login.
4. Shadow slime and props: improves scene charm after main actors are coherent. Prop pass closed as `LGO_WORLD_V3B_PROP_QUALITY_PASS_READY`; shadow slime pass closed as `LGO_WORLD_V3B_SHADOW_SLIME_QUALITY_PASS_READY`.
5. Extra VFX accents: only after readability and size budgets stay green.

## Acceptance Gates

- `python3.12 tools/validate_lgo_runtime_asset_weight.py`
- `python3.12 tools/validate_lgo_device_profile_ui_budgets.py`
- `./tools/lgo_visual_runtime_review_profiles.sh`

Human visual review must compare world screenshots against the V3B/reference direction before any `VISUAL_RUNTIME_PASS` claim.

## Rejected Candidate Notes

- `player_cultivator_v3b_candidate.png` source art reviewed on 2026-09-05 was removed from V3B candidates because the weapon grip/pose reads incorrectly for an idle cultivator. Runtime player art uses the corrected separated sprite listed in the manifest.

## Non-Claims

- This brief does not generate final production art.
- This brief does not approve V2 placeholders as final quality.
- This brief does not open new gameplay, combat, auth, DB, economy, social, or liveops scope.
