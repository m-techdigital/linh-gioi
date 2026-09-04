# Linh Gioi Sprite Sheet Import Plan

Marker: `LGO_SPRITE_IMPORT_PLAN_READY`

## Purpose

The project needs many visual assets, but not every sheet is a runtime source. This plan separates reference art, experimental sheet work, and role-sized runtime candidates so Unity stays light and maintainable.

## Asset Classes

| Class | Runtime use | Rule |
|---|---|---|
| `REFERENCE_ONLY` | none | visual direction, overview boards, mockups, posters, and contact sheets stay outside runtime import paths |
| `EXPERIMENTAL_SOURCE_ONLY` | none | sheet crops can be kept for review/evidence, but cannot be claimed as production or runtime-approved |
| `STRUCTURAL_RUNTIME_PLACEHOLDER_V2` | temporary | separated placeholder PNGs may prove mapping/layout, but are not final quality |
| `RUNTIME_CANDIDATE_SIZE_BUDGETED` | candidate | separated role-sized PNGs with manifest, alpha checks, compression notes, and Unity import settings |
| `PRODUCTION_FINAL_REVIEW_REQUIRED` | gated | high-quality separated assets still need human visual acceptance before production-final claim |

## Sheet Policy

- Do not import composite sheets, moodboards, reference posters, screen mockups, or contact sheets into Unity runtime folders.
- Do not auto-slice AI composite sheets into final runtime sprites.
- Sprite sheets are allowed only when they are authored as runtime sheets with consistent padding, clean alpha, no labels, and a manifest.
- If a sheet is used for animation frames, keep source provenance and frame order in the manifest.
- If any crop was produced from a composite sheet, classify it as `EXPERIMENTAL_SOURCE_ONLY` and keep it out of production claims.

## Role-Sized Import Rules

| Role | Preferred source | Unity import |
|---|---|---|
| Login background | separated 1920x1080 image | max 2048, no read/write, compressed after visual check |
| Logo | separated transparent image | max 1024, no read/write, no generated mip maps for UI |
| Character/NPC portrait | separated 512x768 or 768x1024 PNG | max 1024, alpha as transparency |
| Button skin | separated 256x64 to 512x128 PNG | max 512, sprite border/9-slice when stretchable |
| HUD icon | separated 64x64 to 128x128 PNG | max 128 or 256, no mip maps |
| Skill icon | separated 256x256 PNG | max 256 or 512, no baked text |
| VFX frame | separated 256x256 to 512x512 PNG | max 512, alpha checked against dark and light backgrounds |
| World prop | separated 256-512 px major axis | max 512, alpha and pivot reviewed |

## Compression Budget

Use `docs/art/RUNTIME-ASSET-SIZE-BUDGET.md` as the source of truth. Prefer a few KB to a few dozen KB for small UI/icons, and only allow larger files for fullscreen backgrounds, large NPCs, or documented quality needs.

## Manifest Fields

Every runtime candidate import should record:

- asset id;
- source file;
- class;
- runtime role;
- dimensions;
- file size bytes;
- alpha required;
- Unity max texture size;
- compression note;
- provenance;
- SHA-256;
- reviewer status.

## Validator Expectations

Validators should fail when:

- reference-only or composite sheet filenames appear in runtime import mappings;
- production-final language appears without human acceptance evidence;
- source files exceed role budgets without notes;
- frozen contract surfaces change in an art/import-only task.

## Non-Claims

This plan does not create production art, approve current placeholders as final quality, or implement new gameplay.
